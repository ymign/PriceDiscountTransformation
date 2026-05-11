using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 统一计价服务 HTTP 客户端（.NET Framework 3.5 兼容）。
    /// 基于 HttpWebRequest 的同步调用封装，支持超时配置和失败重试。
    ///
    /// 职责边界：
    /// - 此类仅负责 HTTP 通信层（序列化、发送、反序列化），不包含业务逻辑
    /// - 业务编排（如三阶段确认、弹窗流程）由 PricingHisIntegrationHelper 负责
    /// - URL 构建由 PricingApiUrlBuilder 负责，此类只拼接 baseUrl + path
    ///
    /// 线程安全性：此类的所有方法都是无状态的（状态仅在构造时初始化），
    /// 可安全地在多线程环境中共享同一实例。
    /// </summary>
    public sealed class PricingApiClient
    {
        /// <summary>
        /// 计价服务基础 URL（不含尾部斜杠），如 "http://pricing-center:8080"。
        /// 构造时自动去除尾部斜杠，避免拼接路径时出现双斜杠。
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// HTTP 请求超时时间（毫秒）。同时应用于 Timeout 和 ReadWriteTimeout。
        /// 默认 10 秒。超时后抛出 WebException，触发重试逻辑。
        /// </summary>
        private readonly int _timeoutMs;

        /// <summary>最大重试次数（含首次请求）。默认 3 次。仅 confirm 接口使用重试。</summary>
        private readonly int _maxRetry;

        /// <summary>重试间隔（毫秒）。默认 2000ms。当前使用固定间隔，未实现指数退避。</summary>
        private readonly int _retryDelayMs;

        /// <summary>
        /// 构造计价服务 HTTP 客户端（完整参数）。
        /// </summary>
        /// <param name="baseUrl">计价服务基础 URL</param>
        /// <param name="timeoutMs">HTTP 请求超时（毫秒）</param>
        /// <param name="maxRetry">最大重试次数</param>
        /// <param name="retryDelayMs">重试间隔（毫秒）</param>
        public PricingApiClient(string baseUrl, int timeoutMs, int maxRetry, int retryDelayMs)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _timeoutMs = timeoutMs;
            _maxRetry = maxRetry;
            _retryDelayMs = retryDelayMs;
        }

        /// <summary>
        /// 构造计价服务 HTTP 客户端（使用默认配置：超时 10s，重试 3 次，间隔 2s）。
        /// </summary>
        /// <param name="baseUrl">计价服务基础 URL</param>
        public PricingApiClient(string baseUrl)
            : this(baseUrl, 10000, 3, 2000)
        {
        }

        // ================================================================
        // 计价计算接口（三阶段确认模式）
        // ================================================================

        /// <summary>
        /// 试算接口（simulate）。不占用额度，仅返回计算结果。
        /// 用于特殊计价弹窗中的预览展示，以及批量试算场景。
        /// 接口幂等，可安全重复调用。
        /// </summary>
        /// <param name="request">计价请求上下文</param>
        /// <returns>计价响应（含最终金额、计算步骤、匹配规则等）</returns>
        public ApiResponse<PricingCalculateResponse> Simulate(PricingCalculateRequest request)
        {
            return Post<PricingCalculateResponse>("/api/pricing/calculate/simulate", request);
        }

        /// <summary>
        /// 确认计价接口（confirm）。占用额度，是三阶段确认的第一步。
        ///
        /// 资金安全特性：
        /// - 幂等性：以 sourceSystem + businessRequestNo + callType 为幂等键
        /// - 重试安全：超时重试必须复用同一 businessRequestNo，不会重复占用额度
        /// - 权威单价校验：服务端会校验请求中的单价与权威单价是否一致
        ///
        /// 使用 PostWithRetry 封装，WebException 时自动重试（最多 _maxRetry 次）。
        /// </summary>
        /// <param name="request">计价请求上下文</param>
        /// <returns>计价响应（含 RequestId，用于后续 commit/cancel）</returns>
        public ApiResponse<PricingCalculateResponse> Confirm(PricingCalculateRequest request)
        {
            return PostWithRetry<PricingCalculateResponse>(
                "/api/pricing/calculate/confirm", request);
        }

        /// <summary>
        /// 提交接口（commit）。HIS 落账成功后调用，将 confirm 阶段占用的额度正式消费。
        /// 状态流转：CONFIRMED -> COMMITTED。
        /// </summary>
        /// <param name="request">提交请求（含 RequestId 和 ChargeNo）</param>
        /// <returns>API 响应</returns>
        public ApiResponse Commit(PricingCommitRequest request)
        {
            return PostNoData("/api/pricing/calculate/commit", request);
        }

        /// <summary>
        /// 取消接口（cancel）。HIS 落账失败后调用，释放 confirm 阶段占用的额度。
        /// 状态流转：CONFIRMED -> CANCELLED。
        /// 资金安全约束：落账失败必须调用此接口，否则额度被永久占用。
        /// </summary>
        /// <param name="request">取消请求（含 RequestId）</param>
        /// <returns>API 响应</returns>
        public ApiResponse Cancel(PricingCancelRequest request)
        {
            return PostNoData("/api/pricing/calculate/cancel", request);
        }

        /// <summary>
        /// 退费/冲销接口（reverse）。HIS 退费时调用，释放已占用的额度。
        /// 当日退费按退费数量释放额度；隔日退费重收后按重收当天重新校验额度。
        /// </summary>
        /// <param name="request">退费请求（含原请求 ID、退费数量、退费时间等）</param>
        /// <returns>API 响应</returns>
        public ApiResponse Reverse(PricingReverseRequest request)
        {
            return PostNoData("/api/pricing/calculate/reverse", request);
        }

        // ================================================================
        // 特殊项目标识查询
        // ================================================================

        /// <summary>
        /// 查询指定项目是否为特殊计价项目。
        /// HIS 收费时在录入每个项目后调用，用于判断是否需要弹出计价确认弹窗。
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>特殊标识响应（含 IsSpecial 标记和匹配的规则数量）</returns>
        public ApiResponse<SpecialFlagResponse> GetSpecialFlag(string itemCode)
        {
            return Get<SpecialFlagResponse>(PricingApiUrlBuilder.BuildSpecialFlag(itemCode));
        }

        // ================================================================
        // 规则管理接口（供规则维护工作台使用）
        // ================================================================

        /// <summary>
        /// 分页查询规则列表。支持按项目编码、状态、类别筛选。
        /// </summary>
        /// <param name="itemCode">项目编码筛选（可为空）</param>
        /// <param name="status">状态筛选（可为空）</param>
        /// <param name="category">类别筛选（可为空）</param>
        /// <param name="pageIndex">页码（从 1 开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>分页规则列表</returns>
        public ApiResponse<PagedResponse<RuleHeaderResponse>> GetRules(
            string itemCode, string status, string category, int pageIndex, int pageSize)
        {
            return Get<PagedResponse<RuleHeaderResponse>>(
                PricingApiUrlBuilder.BuildRulesQuery(itemCode, status, category, pageIndex, pageSize));
        }

        /// <summary>查询单条规则详情</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>规则头信息</returns>
        public ApiResponse<RuleHeaderResponse> GetRule(long ruleId)
        {
            return Get<RuleHeaderResponse>(PricingApiUrlBuilder.BuildRuleById(ruleId));
        }

        /// <summary>按项目编码查询关联的规则列表（不分页）</summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>规则列表</returns>
        public ApiResponse<List<RuleHeaderResponse>> GetRulesByItemCode(string itemCode)
        {
            return Get<List<RuleHeaderResponse>>(PricingApiUrlBuilder.BuildRulesByItemCode(itemCode));
        }

        /// <summary>新建规则头，返回新规则的 ID</summary>
        /// <param name="request">新建规则请求</param>
        /// <returns>新规则 ID</returns>
        public ApiResponse<long> CreateRule(RuleHeaderCreateRequest request)
        {
            return Post<long>("/api/pricing/rules", request);
        }

        /// <summary>更新已有规则头（不含 RuleCode，编码不允许修改）</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>API 响应</returns>
        public ApiResponse UpdateRule(long ruleId, RuleHeaderUpdateRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildRuleById(ruleId), request);
        }

        /// <summary>查询指定规则的所有版本列表</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>版本列表</returns>
        public ApiResponse<List<RuleVersionResponse>> GetRuleVersions(long ruleId)
        {
            return Get<List<RuleVersionResponse>>(PricingApiUrlBuilder.BuildRuleVersions(ruleId));
        }

        /// <summary>查询指定规则的单个版本详情</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionId">版本 ID</param>
        /// <returns>版本信息</returns>
        public ApiResponse<RuleVersionResponse> GetRuleVersion(long ruleId, long versionId)
        {
            return Get<RuleVersionResponse>(PricingApiUrlBuilder.BuildRuleVersionById(ruleId, versionId));
        }

        /// <summary>
        /// 创建草稿版本。同一规则同时只允许一个 DRAFT 状态的版本。
        /// 新版本继承上一版本的条件和动作（若有）。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>新版本 ID</returns>
        public ApiResponse<long> CreateDraftVersion(long ruleId)
        {
            return Post<long>(PricingApiUrlBuilder.BuildRuleVersions(ruleId), null);
        }

        // ================================================================
        // 条件与动作管理接口
        // ================================================================

        /// <summary>查询指定版本的所有条件</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
        /// <returns>条件列表</returns>
        public ApiResponse<List<RuleConditionResponse>> GetConditions(long ruleId, int versionNo)
        {
            return Get<List<RuleConditionResponse>>(PricingApiUrlBuilder.BuildRuleConditions(ruleId, versionNo));
        }

        /// <summary>
        /// 全量保存条件（替换模式）。服务端删除该版本的所有旧条件，插入新列表。
        /// 仅 DRAFT 版本可编辑。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
        /// <param name="request">条件保存请求（全量）</param>
        /// <returns>API 响应</returns>
        public ApiResponse SaveConditions(long ruleId, int versionNo, RuleConditionSaveRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildRuleConditions(ruleId, versionNo), request);
        }

        /// <summary>查询指定版本的所有动作</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
        /// <returns>动作列表</returns>
        public ApiResponse<List<RuleActionResponse>> GetActions(long ruleId, int versionNo)
        {
            return Get<List<RuleActionResponse>>(PricingApiUrlBuilder.BuildRuleActions(ruleId, versionNo));
        }

        /// <summary>
        /// 全量保存动作（替换模式）。服务端删除该版本的所有旧动作，插入新列表。
        /// 仅 DRAFT 版本可编辑。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
        /// <param name="request">动作保存请求（全量）</param>
        /// <returns>API 响应</returns>
        public ApiResponse SaveActions(long ruleId, int versionNo, RuleActionSaveRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildRuleActions(ruleId, versionNo), request);
        }

        // ================================================================
        // 发布、停用、回滚、审计接口
        // ================================================================

        /// <summary>查询指定规则的发布历史</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>发布记录列表</returns>
        public ApiResponse<List<RulePublishResponse>> GetPublishHistory(long ruleId)
        {
            return Get<List<RulePublishResponse>>(PricingApiUrlBuilder.BuildPublishHistory(ruleId));
        }

        /// <summary>查询指定规则的变更日志</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>变更日志列表</returns>
        public ApiResponse<List<RuleChangeLogResponse>> GetChangeLogs(long ruleId)
        {
            return Get<List<RuleChangeLogResponse>>(PricingApiUrlBuilder.BuildChangeLogs(ruleId));
        }

        /// <summary>
        /// 发布规则版本。将草稿版本升级为 PUBLISHED，服务端会：
        /// 1. 校验规则完整性
        /// 2. 校验规则冲突
        /// 3. 生成规则快照
        /// 4. 失效缓存
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="request">发布请求</param>
        /// <returns>API 响应</returns>
        public ApiResponse PublishRule(long ruleId, RulePublishRequest request)
        {
            return PostNoData(PricingApiUrlBuilder.BuildRulePublish(ruleId), request);
        }

        /// <summary>停用规则。规则状态置为 DISABLED，引擎不再匹配。</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="request">停用请求</param>
        /// <returns>API 响应</returns>
        public ApiResponse DisableRule(long ruleId, RuleDisableRequest request)
        {
            return PostNoData(PricingApiUrlBuilder.BuildRuleDisable(ruleId), request);
        }

        /// <summary>回滚规则。回退到上一个已发布版本，用于紧急修复。</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="request">回滚请求</param>
        /// <returns>API 响应</returns>
        public ApiResponse RollbackRule(long ruleId, RuleRollbackRequest request)
        {
            return PostNoData(PricingApiUrlBuilder.BuildRuleRollback(ruleId), request);
        }

        // ================================================================
        // 字典管理接口
        // ================================================================

        /// <summary>按字典类型查询字典项列表</summary>
        /// <param name="dictType">字典类型（如 "RULE_CATEGORY"）</param>
        /// <returns>字典项列表</returns>
        public ApiResponse<List<DictResponse>> GetDicts(string dictType)
        {
            return Get<List<DictResponse>>(PricingApiUrlBuilder.BuildDictsQuery(dictType));
        }

        /// <summary>查询所有字典类型列表</summary>
        /// <returns>字典类型名称列表</returns>
        public ApiResponse<List<string>> GetDictTypes()
        {
            return Get<List<string>>(PricingApiUrlBuilder.BuildDictTypes());
        }

        /// <summary>新建字典项</summary>
        /// <param name="request">新建请求</param>
        /// <returns>新字典项 ID</returns>
        public ApiResponse<long> CreateDict(DictCreateRequest request)
        {
            return Post<long>("/api/pricing/dicts", request);
        }

        /// <summary>更新字典项</summary>
        /// <param name="dictId">字典项 ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>API 响应</returns>
        public ApiResponse UpdateDict(long dictId, DictUpdateRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildDictById(dictId), request);
        }

        /// <summary>删除字典项</summary>
        /// <param name="dictId">字典项 ID</param>
        /// <returns>API 响应</returns>
        public ApiResponse DeleteDict(long dictId)
        {
            string responseText = SendRequest("DELETE", PricingApiUrlBuilder.BuildDictById(dictId), null);
            return JsonConvert.DeserializeObject<ApiResponse>(responseText);
        }

        // ================================================================
        // 公式管理接口
        // ================================================================

        /// <summary>查询所有公式定义列表</summary>
        /// <returns>公式定义列表</returns>
        public ApiResponse<List<FormulaDefResponse>> GetFormulas()
        {
            return Get<List<FormulaDefResponse>>("/api/pricing/formulas");
        }

        /// <summary>查询单个公式定义详情</summary>
        /// <param name="formulaId">公式 ID</param>
        /// <returns>公式定义</returns>
        public ApiResponse<FormulaDefResponse> GetFormula(long formulaId)
        {
            return Get<FormulaDefResponse>(PricingApiUrlBuilder.BuildFormulaById(formulaId));
        }

        /// <summary>新建公式定义</summary>
        /// <param name="request">新建请求</param>
        /// <returns>新公式 ID</returns>
        public ApiResponse<long> CreateFormula(FormulaDefCreateRequest request)
        {
            return Post<long>("/api/pricing/formulas", request);
        }

        /// <summary>更新公式定义</summary>
        /// <param name="formulaId">公式 ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>API 响应</returns>
        public ApiResponse UpdateFormula(long formulaId, FormulaDefUpdateRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildFormulaById(formulaId), request);
        }

        // ================================================================
        // 内部 HTTP 通信方法
        // ================================================================

        /// <summary>
        /// 带重试的 POST 请求。仅捕获 WebException（网络层错误）进行重试，
        /// 业务层错误（HTTP 200 但 Code != 0）不重试。
        ///
        /// 重试策略：
        /// - 最多 _maxRetry 次（含首次）
        /// - 固定间隔 _retryDelayMs 毫秒
        /// - 最后一次失败后抛出 PricingApiException
        ///
        /// 使用场景：confirm 接口（因超时可能已成功占用额度，幂等重试安全）
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="path">API 路径</param>
        /// <param name="body">请求体</param>
        /// <returns>API 响应</returns>
        private ApiResponse<T> PostWithRetry<T>(string path, object body)
        {
            Exception lastEx = null;
            for (int i = 0; i < _maxRetry; i++)
            {
                try
                {
                    return Post<T>(path, body);
                }
                catch (WebException ex)
                {
                    lastEx = ex;
                    // 非最后一次重试时等待
                    if (i < _maxRetry - 1)
                    {
                        Thread.Sleep(_retryDelayMs);
                    }
                }
            }
            throw new PricingApiException("计价服务请求失败，已重试 " + _maxRetry + " 次", lastEx);
        }

        /// <summary>发送 POST 请求并反序列化为带数据的响应</summary>
        private ApiResponse<T> Post<T>(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            string responseText = SendRequest("POST", path, json);
            return JsonConvert.DeserializeObject<ApiResponse<T>>(responseText);
        }

        /// <summary>发送 POST 请求并反序列化为无数据的响应</summary>
        private ApiResponse PostNoData(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            string responseText = SendRequest("POST", path, json);
            return JsonConvert.DeserializeObject<ApiResponse>(responseText);
        }

        /// <summary>发送 PUT 请求并反序列化为无数据的响应</summary>
        private ApiResponse PutNoData(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            string responseText = SendRequest("PUT", path, json);
            return JsonConvert.DeserializeObject<ApiResponse>(responseText);
        }

        /// <summary>发送 GET 请求并反序列化为带数据的响应</summary>
        private ApiResponse<T> Get<T>(string path)
        {
            string responseText = SendRequest("GET", path, null);
            return JsonConvert.DeserializeObject<ApiResponse<T>>(responseText);
        }

        /// <summary>
        /// 底层 HTTP 请求发送方法。
        /// 使用 HttpWebRequest（.NET Framework 3.5 无 HttpClient），
        /// 同步阻塞调用，通过 Timeout 和 ReadWriteTimeout 控制超时。
        ///
        /// 错误处理策略：
        /// - HTTP 2xx 但业务码非 0：由调用方通过 ApiResponse.IsSuccess 判断
        /// - HTTP 4xx/5xx：捕获 WebException，读取错误响应体后包装为 PricingApiException 抛出
        /// - 网络层异常（超时、连接拒绝等）：直接抛出 WebException
        /// </summary>
        /// <param name="method">HTTP 方法（GET/POST/PUT/DELETE）</param>
        /// <param name="path">API 路径（不含 baseUrl）</param>
        /// <param name="jsonBody">请求体 JSON（GET/DELETE 时为 null）</param>
        /// <returns>响应体 JSON 字符串</returns>
        private string SendRequest(string method, string path, string jsonBody)
        {
            string url = _baseUrl + path;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.ContentType = "application/json; charset=utf-8";
            req.Accept = "application/json";
            req.Timeout = _timeoutMs;
            req.ReadWriteTimeout = _timeoutMs;

            // 非 GET 请求且有请求体时写入
            if (jsonBody != null && method != "GET")
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonBody);
                req.ContentLength = data.Length;
                using (Stream stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    return ReadResponse(resp);
                }
            }
            catch (WebException ex)
            {
                // HTTP 4xx/5xx 时 WebException 包含 Response，读取错误体后重新包装抛出
                if (ex.Response != null)
                {
                    using (HttpWebResponse errResp = (HttpWebResponse)ex.Response)
                    {
                        string errBody = ReadResponse(errResp);
                        throw new PricingApiException(
                            "HTTP " + (int)errResp.StatusCode + ": " + errBody, ex);
                    }
                }
                throw;
            }
        }

        /// <summary>读取 HTTP 响应体为 UTF-8 字符串</summary>
        private static string ReadResponse(HttpWebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }

    // ================================================================
    // 计价请求/响应 DTO（计价计算专用）
    // ================================================================

    /// <summary>
    /// 计价计算请求 DTO。是 simulate/confirm 两个接口的统一请求结构。
    /// 包含收费动作的完整上下文：患者信息、收费场景、费用明细等。
    ///
    /// 关键字段说明：
    /// - BusinessRequestNo：幂等键的核心组成部分，confirm 超时重试必须复用同一值
    /// - BusinessChargeTime：业务收费发生时间（非技术时间），用于规则生效、2小时窗口、单日累计
    /// - Items：费用明细列表，多肿物/多部位项目必须使用 PricingParts 明细表达
    /// </summary>
    public sealed class PricingCalculateRequest
    {
        /// <summary>
        /// 请求编号（技术层），由调用方生成的全局唯一标识。
        /// 用于日志追踪和问题排查，与 BusinessRequestNo（业务层）不同。
        /// </summary>
        public string RequestNo { get; set; }

        /// <summary>患者 ID</summary>
        public string PatientId { get; set; }

        /// <summary>就诊 ID（门诊号或住院号）</summary>
        public string VisitId { get; set; }

        /// <summary>就诊序号（可选，部分场景需要）</summary>
        public string EncounterNo { get; set; }

        /// <summary>
        /// 收费场景，如 "OUTPATIENT"（门诊）、"INPATIENT"（住院）、"SURGERY"（手术）、"EMERGENCY"（急诊）。
        /// 用于规则匹配——同一项目在不同场景下可能有不同折价规则。
        /// </summary>
        public string ChargeScene { get; set; }

        /// <summary>
        /// 业务收费发生时间。规则生效判断、2小时窗口、单日累计均以此时间为准，
        /// 不得使用技术占用时间（DateTime.Now）替代。
        /// </summary>
        public DateTime BusinessChargeTime { get; set; }

        /// <summary>
        /// 来源系统标识，如 "HIS"、"SELF_SERVICE"（自助机）、"WECHAT"（微信）。
        /// 幂等键的组成部分之一，确保不同渠道的同名业务号不会冲突。
        /// </summary>
        public string SourceSystem { get; set; }

        /// <summary>来源终端标识（可选，如自助机编号、IP 地址等）</summary>
        public string SourceTerminal { get; set; }

        /// <summary>HIS 收费单号（可选），用于关联 HIS 侧的收费记录</summary>
        public string ChargeNo { get; set; }

        /// <summary>
        /// 业务请求号。幂等键 = sourceSystem + businessRequestNo + callType。
        /// confirm 超时重试时必须复用同一值，确保不会重复占用额度。
        /// </summary>
        public string BusinessRequestNo { get; set; }

        /// <summary>
        /// 收费科室编码（可选）。用于排除特定科室的折价规则（如挂号部 7021）。
        /// HIS 侧从当前收费上下文中获取科室编码后传入；为空时按"不排除"处理。
        /// </summary>
        public string ChargeDeptCode { get; set; }

        /// <summary>操作员工号</summary>
        public string OperatorId { get; set; }

        /// <summary>操作员姓名</summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 扩展参数。预留的通用键值对，用于传递非标准化的上下文信息。
        /// 如手术 ID（同手术封顶）、孕次（同孕次限额）等。
        /// </summary>
        public Dictionary<string, object> ExtraParams { get; set; }

        /// <summary>
        /// 费用明细列表。一个收费动作可包含多条明细。
        /// 多肿物、多部位项目必须使用 PricingParts 明细表达，不能压成单个数量粗算。
        /// </summary>
        public List<PricingCalculateItemRequest> Items { get; set; }
    }

    /// <summary>
    /// 计价计算请求中的费用明细 DTO。
    /// 每条明细对应一个收费项目，包含项目编码、数量、单价等。
    /// 对于多肿物/多部位项目，通过 PricingParts 子列表表达部位级明细。
    /// </summary>
    public sealed class PricingCalculateItemRequest
    {
        /// <summary>明细请求编号（本次收费动作内唯一），用于关联响应中的明细结果</summary>
        public string ItemRequestNo { get; set; }

        /// <summary>HIS 收费明细单号（可选），用于关联 HIS 侧的明细记录</summary>
        public string ChargeDetailNo { get; set; }

        /// <summary>项目编码</summary>
        public string ItemCode { get; set; }

        /// <summary>项目名称（展示用，不参与匹配）</summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 输入数量。为收费人员录入的原始数量，引擎可能根据换算规则转换为换算后数量。
        /// 换算数量固定为1（业务约束），公式使用换算后数量。
        /// </summary>
        public decimal InputQty { get; set; }

        /// <summary>计量单位（如 "次"、"个"、"cm"）</summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价。注意：confirm 时计价中心会读取权威单价校验，
        /// 若与渠道传入的单价不一致返回 PRICE_MISMATCH 错误。
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>部位编码（可选），用于按部位匹配不同换算规则</summary>
        public string BodyPartCode { get; set; }

        /// <summary>
        /// 明细级业务收费时间（可选）。若不传则使用请求级的 BusinessChargeTime。
        /// 用于同一收费动作中不同明细有不同收费时间的场景。
        /// </summary>
        public DateTime? BusinessChargeTime { get; set; }

        /// <summary>明细级扩展参数</summary>
        public Dictionary<string, object> ExtraParams { get; set; }

        /// <summary>
        /// HIS 旧系统在当前2小时窗口内已收费的数量（方案B兜底查询）。
        /// 上线过渡期由 HIS 从旧收费明细表查询后传入，新系统将此值纳入窗口累计，
        /// 防止上线当天出现历史数据断层导致的超限放行。
        /// 新旧数据对齐后（通常上线1-2天后）可不再查询，传 null 即可。
        /// 查询方式：调用 PricingHisIntegrationHelper.QueryLegacyOccupiedQty 方法。
        /// </summary>
        public decimal? LegacyOccupiedQty { get; set; }

        /// <summary>
        /// 部位明细列表。多肿物、多部位、多面积项目必须使用此字段表达，
        /// 不能将多个部位压成单个数量粗算。
        /// 例如：3 个肿物各 1cm，应传 3 个 PricingParts（每个 qty=1），而非 InputQty=3。
        /// </summary>
        public List<PricingPartItemRequest> PricingParts { get; set; }
    }

    /// <summary>
    /// 部位明细 DTO。表达多肿物/多部位/多面积项目的具体部位信息。
    /// 用于"复杂输入明细化"要求——多肿物、多部位、多面积项目必须逐部位表达。
    /// </summary>
    public sealed class PricingPartItemRequest
    {
        /// <summary>部位序号（从 1 开始），用于排序和展示</summary>
        public int? PartSeq { get; set; }

        /// <summary>部位编码</summary>
        public string PartCode { get; set; }

        /// <summary>部位名称（展示用）</summary>
        public string PartName { get; set; }

        /// <summary>身体部位编码（可选，用于更细粒度的规则匹配）</summary>
        public string BodyPartCode { get; set; }

        /// <summary>该部位的数量</summary>
        public decimal Qty { get; set; }

        /// <summary>面积（可选，面积计价项目使用，单位取决于 MeasureType）</summary>
        public decimal? Area { get; set; }

        /// <summary>测量类型（如 "DIAMETER" 直径、"AREA" 面积）</summary>
        public string MeasureType { get; set; }

        /// <summary>测量值（与 MeasureType 配合使用）</summary>
        public decimal? MeasureValue { get; set; }

        /// <summary>测量单位（如 "cm"、"cm2"）</summary>
        public string MeasureUnit { get; set; }

        /// <summary>肿物数量（皮肤科等按肿物个数计价的场景使用）</summary>
        public int? LesionCount { get; set; }
    }

    /// <summary>
    /// 计价计算响应 DTO。包含计价引擎的完整计算结果。
    /// 顶层字段为聚合结果，Items 列表为各明细的独立结果。
    ///
    /// 金额说明：
    /// - FinalAmount：最终金额（折后价），保留 2 位小数，四舍五入
    /// - DiscountAmount：折价金额 = 原价 - 折后价
    /// - 中间计算保留全部精度，不在中间步骤取整
    /// </summary>
    public sealed class PricingCalculateResponse
    {
        /// <summary>各费用明细的计价结果列表</summary>
        public List<PricingCalculateItemResponse> Items { get; set; }

        /// <summary>
        /// 请求 ID。confirm 接口返回此值，后续 commit/cancel/reverse 均需引用。
        /// </summary>
        public long RequestId { get; set; }

        /// <summary>是否命中特殊计价规则</summary>
        public bool IsSpecialItem { get; set; }

        /// <summary>输入数量（聚合值，仅当 Items 为空时使用）</summary>
        public decimal InputQty { get; set; }

        /// <summary>最终数量（换算后、限额校验后的实际计价数量）</summary>
        public decimal FinalQty { get; set; }

        /// <summary>单价（权威单价，可能与请求中的单价不同）</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>最终金额（折后价），保留 2 位小数，四舍五入</summary>
        public decimal FinalAmount { get; set; }

        /// <summary>折价金额 = 原价 - 折后价</summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>全局级计算步骤（如全局限额校验、同组互斥等）</summary>
        public List<PricingTraceStepResponse> TraceSteps { get; set; }

        /// <summary>本次计价匹配到的规则 ID 列表，用于审计追溯</summary>
        public List<long> MatchedRuleIds { get; set; }
    }

    /// <summary>
    /// 计价计算响应中的费用明细 DTO。
    /// 每条明细对应请求中的一条 PricingCalculateItemRequest。
    /// </summary>
    public sealed class PricingCalculateItemResponse
    {
        /// <summary>明细请求编号（与请求中的 ItemRequestNo 对应）</summary>
        public string ItemRequestNo { get; set; }

        /// <summary>HIS 收费明细单号</summary>
        public string ChargeDetailNo { get; set; }

        /// <summary>请求 ID（同顶层 RequestId）</summary>
        public long RequestId { get; set; }

        /// <summary>项目编码</summary>
        public string ItemCode { get; set; }

        /// <summary>项目名称</summary>
        public string ItemName { get; set; }

        /// <summary>是否命中特殊计价规则</summary>
        public bool IsSpecialItem { get; set; }

        /// <summary>输入数量</summary>
        public decimal InputQty { get; set; }

        /// <summary>最终数量</summary>
        public decimal FinalQty { get; set; }

        /// <summary>单价</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>最终金额（折后价）</summary>
        public decimal FinalAmount { get; set; }

        /// <summary>折价金额</summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>该明细的计算步骤（用于"计算过程链"追溯）</summary>
        public List<PricingTraceStepResponse> TraceSteps { get; set; }

        /// <summary>该明细匹配到的规则 ID 列表</summary>
        public List<long> MatchedRuleIds { get; set; }
    }

    /// <summary>
    /// 计算步骤响应 DTO。
    /// 计价引擎在执行过程中记录每一步计算，形成"计算过程链"，
    /// 支持审计人员完整回溯一笔折价的计算逻辑。
    ///
    /// 步骤类型示例：
    /// - MATCH：规则匹配
    /// - CONVERT：双单位换算
    /// - FORMULA：公式计算
    /// - LIMIT_QTY：数量限制校验
    /// - LIMIT_AMOUNT：金额上下限校验
    /// - MUTEX：同组互斥校验
    /// </summary>
    public sealed class PricingTraceStepResponse
    {
        /// <summary>步骤序号，标识在计算链中的位置（从 1 开始）</summary>
        public int StepNo { get; set; }

        /// <summary>步骤类型（引擎内部枚举的字符串表示）</summary>
        public string StepType { get; set; }

        /// <summary>步骤描述，人类可读的计算说明</summary>
        public string StepDesc { get; set; }

        /// <summary>输入值（如原数量、原金额），NULL 表示无输入</summary>
        public decimal? InputValue { get; set; }

        /// <summary>输出值（如计算后数量、计算后金额），NULL 表示无输出</summary>
        public decimal? OutputValue { get; set; }
    }

    // ================================================================
    // commit / cancel / reverse 请求 DTO
    // ================================================================

    /// <summary>
    /// 提交请求 DTO（commit）。HIS 落账成功后调用。
    /// 将 confirm 阶段占用的额度正式消费。
    /// </summary>
    public sealed class PricingCommitRequest
    {
        /// <summary>confirm 阶段返回的请求 ID</summary>
        public long RequestId { get; set; }

        /// <summary>HIS 落账后的收费单号，用于关联 HIS 侧的收费记录</summary>
        public string ChargeNo { get; set; }
    }

    /// <summary>
    /// 取消请求 DTO（cancel）。HIS 落账失败后调用。
    /// 释放 confirm 阶段占用的额度。
    /// </summary>
    public sealed class PricingCancelRequest
    {
        /// <summary>confirm 阶段返回的请求 ID</summary>
        public long RequestId { get; set; }
    }

    /// <summary>
    /// 退费/冲销请求 DTO（reverse）。HIS 退费时调用。
    ///
    /// 退费额度释放规则：
    /// - 当日退费：按退费数量释放额度
    /// - 隔日退费重收：按重收当天重新做额度校验
    /// - 2 小时规则：按收费时间（包含重收时间）向前查 2 小时
    /// - 部分退费：校验本次退费 + 历史已退不超过原有效收费
    /// </summary>
    public sealed class PricingReverseRequest
    {
        /// <summary>原确认计价的请求 ID（即 confirm 阶段返回的 RequestId）</summary>
        public long OriginalRequestId { get; set; }

        /// <summary>退费单号（HIS 生成的退费业务编号）</summary>
        public string ReverseNo { get; set; }

        /// <summary>收费明细单号（指定退费的具体明细）</summary>
        public string ChargeDetailNo { get; set; }

        /// <summary>项目编码</summary>
        public string ItemCode { get; set; }

        /// <summary>部位序号（多部位项目退费时指定具体部位）</summary>
        public int? PartSeq { get; set; }

        /// <summary>退费时间（业务时间，非技术时间）</summary>
        public DateTime? ReverseTime { get; set; }

        /// <summary>退费数量（按数量释放额度）</summary>
        public decimal? ReverseQty { get; set; }

        /// <summary>退费金额（可选，部分场景按金额退费）</summary>
        public decimal? ReverseAmt { get; set; }

        /// <summary>退费操作人工号</summary>
        public string ReversedBy { get; set; }

        /// <summary>退费原因</summary>
        public string Reason { get; set; }
    }

    /// <summary>
    /// 特殊项目标识查询响应 DTO。
    /// 用于 HIS 收费时快速判断某项目是否为特殊计价项目。
    /// </summary>
    public sealed class SpecialFlagResponse
    {
        /// <summary>项目编码（回显）</summary>
        public string ItemCode { get; set; }

        /// <summary>是否为特殊计价项目。true 时 HIS 必须弹出计价确认弹窗。</summary>
        public bool IsSpecial { get; set; }

        /// <summary>匹配到的规则数量（信息性字段，供展示用）</summary>
        public int RuleCount { get; set; }
    }

    // ================================================================
    // 通用 API 响应封装
    // ================================================================

    /// <summary>
    /// 带业务数据的 API 响应封装。
    /// 服务端统一返回格式：Code=0 表示成功，非 0 表示失败。
    /// TraceId 用于全链路追踪（请求 -> 计价引擎 -> 数据库）。
    /// </summary>
    /// <typeparam name="T">业务数据类型</typeparam>
    public sealed class ApiResponse<T>
    {
        /// <summary>业务状态码，0 表示成功，非 0 表示失败</summary>
        public int Code { get; set; }

        /// <summary>状态消息（成功时可能为空，失败时为错误描述）</summary>
        public string Message { get; set; }

        /// <summary>业务数据（仅 Code=0 时有值）</summary>
        public T Data { get; set; }

        /// <summary>全链路追踪 ID，用于问题排查时关联日志</summary>
        public string TraceId { get; set; }

        /// <summary>是否成功（Code == 0 的快捷判断）</summary>
        public bool IsSuccess
        {
            get { return Code == 0; }
        }
    }

    /// <summary>
    /// 不带业务数据的 API 响应封装。
    /// 用于 commit、cancel、reverse 等不需要返回业务数据的接口。
    /// </summary>
    public sealed class ApiResponse
    {
        /// <summary>业务状态码，0 表示成功</summary>
        public int Code { get; set; }

        /// <summary>状态消息</summary>
        public string Message { get; set; }

        /// <summary>全链路追踪 ID</summary>
        public string TraceId { get; set; }

        /// <summary>是否成功</summary>
        public bool IsSuccess
        {
            get { return Code == 0; }
        }
    }

    /// <summary>
    /// 计价 API 异常。包装 HTTP 错误和业务错误，供调用方统一捕获。
    /// 区分于 WebException（网络层）和 ApplicationException（业务层），
    /// 此异常表示与计价服务通信相关的所有可预期错误。
    /// </summary>
    public sealed class PricingApiException : Exception
    {
        /// <summary>
        /// 构造计价 API 异常。
        /// </summary>
        /// <param name="message">错误描述（含 HTTP 状态码或业务错误信息）</param>
        /// <param name="innerException">内部异常（通常是 WebException）</param>
        public PricingApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
