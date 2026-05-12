using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HIS.Pricing.Client;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Registration;

namespace Neusoft.HISFC.BizProcess.Integrate
{
    /// <summary>
    /// 旧 HIS 收费链路到统一计价 Agent 的适配层。
    /// </summary>
    /// <remarks>
    /// 该类型只做边界翻译：把旧 HIS 的门诊/住院 <c>FeeItemList</c> 组装成统一计价请求，
    /// 把 confirm 返回的主项目金额、替换子项和加收子项回填到旧 HIS 明细集合，并在 HIS
    /// 事务真正提交后组装真实落账明细调用 commit。
    ///
    /// 这里刻意不写旧 HIS 的 2 小时历史占用 SQL。仓库没有完整的旧表查询口径时，自动补一段
    /// 看似合理的 SQL 会制造资金风险，所以该部分仍留给现场依据真实表结构补齐。
    /// </remarks>
    internal sealed class PricingLegacyChargeBridge
    {
        /// <summary>
        /// 默认来源系统编码。配置文件未显式声明 SourceSystem 时使用该值，避免 HIS 多入口幂等键为空。
        /// </summary>
        private const string SourceSystem = "HIS_WY";

        /// <summary>
        /// 门诊收费场景编码。统一计价服务用该字段参与规则条件匹配和追溯过滤。
        /// </summary>
        private const string OutpatientScene = "OUTPATIENT";

        /// <summary>
        /// 住院收费场景编码。与门诊分开，避免同一项目在住院和门诊使用相同规则口径。
        /// </summary>
        private const string InpatientScene = "INPATIENT";

        /// <summary>
        /// HIS 非药品项目业务层，用于校验统一计价返回的替换/加收子项是否存在于本地项目主数据。
        /// </summary>
        private readonly Neusoft.HISFC.BizLogic.Fee.Item itemManager;

        /// <summary>
        /// 已 confirm 但尚未 commit/cancel 的计价请求。
        /// HIS 先 confirm，再写本地库，最后提交事务；这个列表承接这段事务窗口内的状态。
        /// </summary>
        private readonly List<PricingPendingCharge> pendingCharges = new List<PricingPendingCharge>();

        /// <summary>
        /// 延迟创建的 PricingAgent 实例。旧 HIS 未部署配置时不会初始化，降低未启用环境的运行影响。
        /// </summary>
        private PricingAgent agent;

        /// <summary>
        /// 当前 HIS 通道编码。默认使用 HIS_WY；如果 pricing-agent.config 配置了 SourceSystem，
        /// 以配置值为准，保证 confirm、commit、cancel 的幂等维度与部署配置一致。
        /// </summary>
        private string sourceSystem = SourceSystem;

        /// <summary>
        /// Agent 初始化失败后的缓存错误。失败后不反复读取配置和创建窗口，直接返回同一个阻断原因。
        /// </summary>
        private string agentLoadError;

        /// <summary>
        /// 创建旧 HIS 计价适配层。
        /// </summary>
        /// <param name="itemManager">HIS 项目业务层，用于读取替换/加收子项主数据。</param>
        public PricingLegacyChargeBridge(Neusoft.HISFC.BizLogic.Fee.Item itemManager)
        {
            this.itemManager = itemManager;
        }

        /// <summary>
        /// 判断当前 HIS 运行目录是否已经部署 PricingAgent 配置。
        /// </summary>
        /// <returns>存在配置文件时返回 true；读取配置路径异常时按未启用处理。</returns>
        public static bool IsConfigured()
        {
            try
            {
                return File.Exists(PricingSdkConfigLoader.GetDefaultConfigPath());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 门诊收费写 HIS 本地库前执行统一计价 confirm。
        /// </summary>
        /// <param name="owner">弹窗宿主窗口；旧收费链路拿不到窗口时允许为空。</param>
        /// <param name="register">门诊挂号信息，用于患者、就诊和年龄映射。</param>
        /// <param name="feeDetails">HIS 即将保存的门诊费用明细集合，confirm 成功后会被原地回填。</param>
        /// <param name="chargeNo">HIS 本次收费单号或组合发票号，用于业务幂等和后续对账。</param>
        /// <param name="chargeTime">业务收费时间，规则生效和窗口累计都以该时间为准。</param>
        /// <param name="operatorId">收费操作员工号。</param>
        /// <param name="operatorName">收费操作员姓名。</param>
        /// <param name="chargeDeptCode">收费科室编码。</param>
        /// <returns>允许、阻断、普通计价或已确认的统一结果。</returns>
        public PricingConfirmResult ConfirmOutpatientBeforeSave(
            IWin32Window owner,
            Register register,
            ArrayList feeDetails,
            string chargeNo,
            DateTime chargeTime,
            string operatorId,
            string operatorName,
            string chargeDeptCode)
        {
            // ========== 第一阶段：配置开关 ==========
            // 旧 HIS 部署面广，不能因为未部署 Agent 配置就改变既有收费行为。
            if (!IsConfigured())
            {
                return PricingConfirmResult.NotConfigured();
            }

            // ========== 第二阶段：加载 Agent ==========
            // 配置错误或 DLL 初始化失败时必须阻断，不能悄悄回落普通计价。
            string error = null;
            PricingAgent pricingAgent = GetAgent(ref error);
            if (pricingAgent == null)
            {
                return PricingConfirmResult.Blocked(error);
            }

            // ========== 第三阶段：构造统一计价请求 ==========
            // itemMap 保留 ItemRequestNo 到 HIS 原明细对象的映射，后续响应回填必须靠它定位原行。
            Dictionary<string, object> itemMap = new Dictionary<string, object>();
            PricingCalculateRequest request = BuildOutpatientRequest(
                register,
                feeDetails,
                chargeNo,
                chargeTime,
                operatorId,
                operatorName,
                chargeDeptCode,
                itemMap,
                ref error);

            if (request == null)
            {
                return PricingConfirmResult.Blocked(error);
            }

            // ========== 第四阶段：弹窗确认并占用额度 ==========
            // confirm 成功后只是保护占用，HIS 本地落账成功后还必须 commit。
            PricingAgentChargeResult result = pricingAgent.ConfirmBeforeCharge(owner, request);
            if (result == null || !result.AllowCharge)
            {
                return PricingConfirmResult.Blocked(result == null ? "统一计价 Agent 未返回确认结果。" : result.Message);
            }

            if (result.OrdinaryPricing)
            {
                return PricingConfirmResult.Ordinary(result.Message);
            }

            // ========== 第五阶段：把计价结果回填到旧 HIS 明细 ==========
            // 如果替换/加收子项无法在 HIS 主数据中找到，必须 cancel 保护占用并阻断落账。
            if (!ApplyOutpatientResult(feeDetails, itemMap, result.Response, ref error))
            {
                SafeCancel(pricingAgent, result.RequestId);
                return PricingConfirmResult.Blocked(error);
            }

            // ========== 第六阶段：登记待提交状态 ==========
            // 此时 HIS 尚未写库成功，只能暂存；真正的 actualItems 要等 HIS 保存成功后从明细集合重建。
            PricingPendingCharge pending = RegisterPending(result.RequestId, chargeNo, feeDetails, OutpatientScene);
            return PricingConfirmResult.ConfirmedCharge(pending, result.Message);
        }

        /// <summary>
        /// 门诊收费界面展示前执行统一计价试算。
        /// </summary>
        /// <remarks>
        /// simulate 只用于界面预览：不占用额度、不生成待 commit 状态，也不向 HIS 明细集合追加替换/加收子项。
        /// 真正点击收费时仍必须走 <see cref="ConfirmOutpatientBeforeSave"/>，以 confirm 返回结果为准落账。
        /// </remarks>
        public PricingOutpatientPreviewResult SimulateOutpatientForDisplay(
            Register register,
            ArrayList feeDetails,
            string chargeNo,
            DateTime chargeTime,
            string operatorId,
            string operatorName,
            string chargeDeptCode)
        {
            if (!IsConfigured())
            {
                return PricingOutpatientPreviewResult.NotConfigured();
            }

            string error = null;
            PricingAgent pricingAgent = GetAgent(ref error);
            if (pricingAgent == null)
            {
                return PricingOutpatientPreviewResult.Failed(error);
            }

            Dictionary<string, object> itemMap = new Dictionary<string, object>();
            PricingCalculateRequest request = BuildOutpatientRequest(
                register,
                feeDetails,
                chargeNo,
                chargeTime,
                operatorId,
                operatorName,
                chargeDeptCode,
                itemMap,
                ref error);

            if (request == null)
            {
                return PricingOutpatientPreviewResult.Failed(error);
            }

            try
            {
                ApiResponse<PricingCalculateResponse> response = pricingAgent.Sdk.Simulate(request);
                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    return PricingOutpatientPreviewResult.Failed(
                        response == null ? "统一计价试算无响应。" : response.Message);
                }

                return PricingOutpatientPreviewResult.FromResponse(response.Data);
            }
            catch (Exception ex)
            {
                return PricingOutpatientPreviewResult.Failed("统一计价试算失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 住院正向收费写 HIS 本地库前执行统一计价 confirm。
        /// </summary>
        /// <param name="owner">弹窗宿主窗口；旧收费链路拿不到窗口时允许为空。</param>
        /// <param name="patient">住院患者信息，用于患者、住院号和年龄映射。</param>
        /// <param name="feeDetails">HIS 即将保存的住院费用明细集合，confirm 成功后会被原地回填。</param>
        /// <param name="chargeNo">本次住院收费业务号，优先使用已生成处方号，缺失时由调用方兜底。</param>
        /// <param name="chargeTime">业务收费时间。</param>
        /// <param name="operatorId">收费操作员工号。</param>
        /// <param name="operatorName">收费操作员姓名。</param>
        /// <param name="chargeDeptCode">收费科室编码。</param>
        /// <returns>允许、阻断、普通计价或已确认的统一结果。</returns>
        public PricingConfirmResult ConfirmInpatientBeforeSave(
            IWin32Window owner,
            Neusoft.HISFC.Models.RADT.PatientInfo patient,
            ArrayList feeDetails,
            string chargeNo,
            DateTime chargeTime,
            string operatorId,
            string operatorName,
            string chargeDeptCode)
        {
            // ========== 第一阶段：配置开关 ==========
            // 未部署 Agent 时不改变旧 HIS 住院收费行为。
            if (!IsConfigured())
            {
                return PricingConfirmResult.NotConfigured();
            }

            // ========== 第二阶段：加载 Agent ==========
            // 统一计价已启用但不可用时，特殊计价相关收费必须保守阻断。
            string error = null;
            PricingAgent pricingAgent = GetAgent(ref error);
            if (pricingAgent == null)
            {
                return PricingConfirmResult.Blocked(error);
            }

            // ========== 第三阶段：构造住院多明细请求 ==========
            // 统一请求根对象不再放单个 ItemCode，所有项目编码都在 Items 明细层。
            Dictionary<string, object> itemMap = new Dictionary<string, object>();
            PricingCalculateRequest request = BuildInpatientRequest(
                patient,
                feeDetails,
                chargeNo,
                chargeTime,
                operatorId,
                operatorName,
                chargeDeptCode,
                itemMap,
                ref error);

            if (request == null)
            {
                return PricingConfirmResult.Blocked(error);
            }

            // ========== 第四阶段：confirm 并获取可落账结果 ==========
            PricingAgentChargeResult result = pricingAgent.ConfirmBeforeCharge(owner, request);
            if (result == null || !result.AllowCharge)
            {
                return PricingConfirmResult.Blocked(result == null ? "统一计价 Agent 未返回确认结果。" : result.Message);
            }

            if (result.OrdinaryPricing)
            {
                return PricingConfirmResult.Ordinary(result.Message);
            }

            // ========== 第五阶段：回填住院明细 ==========
            // 回填失败必须 cancel，因为 confirm 已经占用了限额保护额度。
            if (!ApplyInpatientResult(feeDetails, itemMap, result.Response, ref error))
            {
                SafeCancel(pricingAgent, result.RequestId);
                return PricingConfirmResult.Blocked(error);
            }

            // ========== 第六阶段：登记待提交状态 ==========
            PricingPendingCharge pending = RegisterPending(result.RequestId, chargeNo, feeDetails, InpatientScene);
            return PricingConfirmResult.ConfirmedCharge(pending, result.Message);
        }

        /// <summary>
        /// HIS 本地费用明细保存成功后标记待提交请求，并从真实 HIS 明细重建 actualItems。
        /// </summary>
        /// <param name="pending">confirm 阶段登记的待提交对象。</param>
        public void MarkHisSaveSucceeded(PricingPendingCharge pending)
        {
            if (pending == null || pending.HisSaveSucceeded)
            {
                return;
            }

            pending.ActualItems = BuildActualItems(pending.FeeDetails, pending.Scene);
            pending.ActualTotalAmount = SumActualAmount(pending.ActualItems);
            pending.HisSaveSucceeded = true;
        }

        /// <summary>
        /// HIS 事务提交成功后调用 commit，把保护占用推进为正式占用。
        /// </summary>
        /// <remarks>
        /// commit 失败不能再 cancel，因为 HIS 本地库已经提交；SDK 会把失败请求写入本地补偿队列，
        /// 后续由诊断或补偿任务重试。
        /// </remarks>
        public void CommitSavedCharges()
        {
            if (pendingCharges.Count == 0)
            {
                return;
            }

            PricingAgent pricingAgent = null;
            string error = null;

            for (int i = pendingCharges.Count - 1; i >= 0; i--)
            {
                PricingPendingCharge pending = pendingCharges[i];
                if (!pending.HisSaveSucceeded)
                {
                    continue;
                }

                if (pricingAgent == null)
                {
                    pricingAgent = GetAgent(ref error);
                }

                if (pricingAgent != null)
                {
                    try
                    {
                        pricingAgent.CommitAfterHisSuccess(
                            pending.RequestId,
                            pending.ChargeNo,
                            pending.ActualItems,
                            pending.ActualTotalAmount);
                    }
                    catch
                    {
                        // SDK 已写入本地补偿队列；HIS 数据库事务已经提交，不能再 cancel。
                    }
                }

                pendingCharges.RemoveAt(i);
            }
        }

        /// <summary>
        /// HIS 写库失败、医保提交失败或事务回滚时取消尚未提交的 confirm 占用。
        /// </summary>
        public void CancelUncommittedCharges()
        {
            if (pendingCharges.Count == 0)
            {
                return;
            }

            PricingAgent pricingAgent = null;
            string error = null;

            for (int i = pendingCharges.Count - 1; i >= 0; i--)
            {
                PricingPendingCharge pending = pendingCharges[i];
                if (pricingAgent == null)
                {
                    pricingAgent = GetAgent(ref error);
                }

                if (pricingAgent != null)
                {
                    SafeCancel(pricingAgent, pending.RequestId);
                }

                pendingCharges.RemoveAt(i);
            }
        }

        /// <summary>
        /// 获取或创建 PricingAgent 实例。
        /// </summary>
        /// <param name="error">初始化失败时返回可展示的阻断原因。</param>
        /// <returns>可用的 Agent；初始化失败时返回 null。</returns>
        private PricingAgent GetAgent(ref string error)
        {
            if (agent != null)
            {
                return agent;
            }

            if (!string.IsNullOrEmpty(agentLoadError))
            {
                error = agentLoadError;
                return null;
            }

            try
            {
                PricingSdkOptions options = PricingSdkConfigLoader.LoadDefault();
                if (string.IsNullOrEmpty(options.SourceSystem))
                {
                    options.SourceSystem = SourceSystem;
                }

                sourceSystem = options.SourceSystem;
                agent = new PricingAgent(options);
                return agent;
            }
            catch (Exception ex)
            {
                agentLoadError = "统一计价 Agent 初始化失败：" + ex.Message;
                error = agentLoadError;
                return null;
            }
        }

        /// <summary>
        /// 把门诊 HIS 明细转换为统一计价多费用明细请求。
        /// </summary>
        /// <param name="register">门诊挂号信息。</param>
        /// <param name="feeDetails">待收费门诊明细集合。</param>
        /// <param name="chargeNo">HIS 收费单号或组合发票号。</param>
        /// <param name="chargeTime">业务收费时间。</param>
        /// <param name="operatorId">操作员工号。</param>
        /// <param name="operatorName">操作员姓名。</param>
        /// <param name="chargeDeptCode">收费科室编码。</param>
        /// <param name="itemMap">输出 ItemRequestNo 到 HIS 原明细对象的映射。</param>
        /// <param name="error">构造失败时返回错误原因。</param>
        /// <returns>统一计价请求；无有效明细时返回 null。</returns>
        private PricingCalculateRequest BuildOutpatientRequest(
            Register register,
            ArrayList feeDetails,
            string chargeNo,
            DateTime chargeTime,
            string operatorId,
            string operatorName,
            string chargeDeptCode,
            Dictionary<string, object> itemMap,
            ref string error)
        {
            if (register == null)
            {
                error = "门诊计价缺少患者挂号信息。";
                return null;
            }

            // ========== 第一阶段：创建请求根对象 ==========
            // 门诊 patientId 优先取就诊卡号，兼容现场按卡号累计限额的口径；没有卡号时回落挂号流水。
            PricingCalculateRequest request = CreateBaseRequest(
                !IsBlank(register.PID.CardNO) ? register.PID.CardNO : register.ID,
                register.ID,
                "OUTPATIENT",
                CalculateAge(register.Birthday, chargeTime),
                register.ID,
                OutpatientScene,
                chargeNo,
                chargeTime,
                operatorId,
                operatorName,
                chargeDeptCode);

            // ========== 第二阶段：逐条转换费用明细 ==========
            // 项目编码保留在 Items 明细层；一次缴费可以包含多条费用，根对象不再承载单个 ItemCode。
            for (int i = 0; i < feeDetails.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList fee =
                    feeDetails[i] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                if (fee == null)
                {
                    continue;
                }

                string itemRequestNo = BuildItemRequestNo(fee.RecipeNO, fee.SequenceNO, i + 1);
                itemMap[itemRequestNo] = fee;
                request.Items.Add(new PricingCalculateItemRequest
                {
                    ItemRequestNo = itemRequestNo,
                    ChargeDetailNo = BuildChargeDetailNo(fee.RecipeNO, fee.SequenceNO),
                    ItemCode = Safe(fee.Item.ID),
                    ItemName = Safe(fee.Item.Name),
                    ItemGroupCode = Safe(fee.UndrugComb.ID),
                    InputQty = fee.Item.Qty,
                    Unit = Safe(fee.Item.PriceUnit),
                    UnitPrice = fee.Item.Price,
                    BodyPartCode = Safe(fee.Order.CheckPartRecord),
                    BusinessChargeTime = chargeTime,
                    ExtraParams = BuildCommonExtraParams(fee.RecipeNO, fee.SequenceNO, fee.Order.ID, fee.UndrugComb.ID)
                });
            }

            if (request.Items.Count == 0)
            {
                error = "门诊计价明细为空，不能调用统一计价。";
                return null;
            }

            return request;
        }

        /// <summary>
        /// 把住院 HIS 明细转换为统一计价多费用明细请求。
        /// </summary>
        /// <param name="patient">住院患者信息。</param>
        /// <param name="feeDetails">待收费住院明细集合。</param>
        /// <param name="chargeNo">HIS 收费业务号。</param>
        /// <param name="chargeTime">业务收费时间。</param>
        /// <param name="operatorId">操作员工号。</param>
        /// <param name="operatorName">操作员姓名。</param>
        /// <param name="chargeDeptCode">收费科室编码。</param>
        /// <param name="itemMap">输出 ItemRequestNo 到 HIS 原明细对象的映射。</param>
        /// <param name="error">构造失败时返回错误原因。</param>
        /// <returns>统一计价请求；无有效明细时返回 null。</returns>
        private PricingCalculateRequest BuildInpatientRequest(
            Neusoft.HISFC.Models.RADT.PatientInfo patient,
            ArrayList feeDetails,
            string chargeNo,
            DateTime chargeTime,
            string operatorId,
            string operatorName,
            string chargeDeptCode,
            Dictionary<string, object> itemMap,
            ref string error)
        {
            if (patient == null)
            {
                error = "住院计价缺少患者信息。";
                return null;
            }

            // ========== 第一阶段：创建请求根对象 ==========
            // 住院限额累计通常按住院患者 ID 和住院号聚合，因此 patientId/visitId 均使用 patient.ID。
            PricingCalculateRequest request = CreateBaseRequest(
                patient.ID,
                patient.ID,
                "INPATIENT",
                CalculateAge(patient.Birthday, chargeTime),
                patient.PID.PatientNO,
                InpatientScene,
                chargeNo,
                chargeTime,
                operatorId,
                operatorName,
                chargeDeptCode);

            // ========== 第二阶段：逐条转换费用明细 ==========
            // 每条住院费用保留自己的 ChargeDetailNo 和 ItemRequestNo，便于 commit 对账。
            for (int i = 0; i < feeDetails.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList fee =
                    feeDetails[i] as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                if (fee == null)
                {
                    continue;
                }

                string itemRequestNo = BuildItemRequestNo(fee.RecipeNO, fee.SequenceNO, i + 1);
                itemMap[itemRequestNo] = fee;
                request.Items.Add(new PricingCalculateItemRequest
                {
                    ItemRequestNo = itemRequestNo,
                    ChargeDetailNo = BuildChargeDetailNo(fee.RecipeNO, fee.SequenceNO),
                    ItemCode = Safe(fee.Item.ID),
                    ItemName = Safe(fee.Item.Name),
                    ItemGroupCode = Safe(fee.UndrugComb.ID),
                    InputQty = fee.Item.Qty,
                    Unit = Safe(fee.Item.PriceUnit),
                    UnitPrice = fee.Item.Price,
                    BodyPartCode = Safe(fee.Order.CheckPartRecord),
                    BusinessChargeTime = chargeTime,
                    ExtraParams = BuildCommonExtraParams(fee.RecipeNO, fee.SequenceNO, fee.Order.ID, fee.UndrugComb.ID)
                });
            }

            if (request.Items.Count == 0)
            {
                error = "住院计价明细为空，不能调用统一计价。";
                return null;
            }

            return request;
        }

        /// <summary>
        /// 创建门诊/住院共用的统一计价请求根对象。
        /// </summary>
        /// <returns>已填充结算级上下文、但尚未加入费用明细的请求。</returns>
        private PricingCalculateRequest CreateBaseRequest(
            string patientId,
            string visitId,
            string visitType,
            int? age,
            string encounterNo,
            string chargeScene,
            string chargeNo,
            DateTime chargeTime,
            string operatorId,
            string operatorName,
            string chargeDeptCode)
        {
            PricingCalculateRequest request = new PricingCalculateRequest();
            request.SourceSystem = Safe(sourceSystem);
            request.PatientId = Safe(patientId);
            request.VisitId = Safe(visitId);
            request.VisitType = visitType;
            request.PatientAge = age;
            request.EncounterNo = Safe(encounterNo);
            request.ChargeScene = chargeScene;
            request.BusinessChargeTime = chargeTime;
            request.ChargeNo = Safe(chargeNo);
            request.BusinessRequestNo = PricingBusinessRequestNoHelper.EnsureBusinessRequestNo(null, chargeNo);
            request.OperatorId = Safe(operatorId);
            request.OperatorName = Safe(operatorName);
            request.ChargeDeptCode = Safe(chargeDeptCode);
            request.Items = new List<PricingCalculateItemRequest>();
            return request;
        }

        /// <summary>
        /// 将 confirm 响应回填到门诊费用明细集合。
        /// </summary>
        /// <param name="feeDetails">待保存的门诊明细集合，方法会原地修改和追加子项。</param>
        /// <param name="itemMap">请求明细号到 HIS 原明细的映射。</param>
        /// <param name="response">统一计价 confirm 响应。</param>
        /// <param name="error">回填失败时返回错误原因。</param>
        /// <returns>全部回填成功时返回 true。</returns>
        private bool ApplyOutpatientResult(
            ArrayList feeDetails,
            Dictionary<string, object> itemMap,
            PricingCalculateResponse response,
            ref string error)
        {
            if (response == null || response.Items == null || response.Items.Count == 0)
            {
                error = "统一计价 confirm 未返回费用明细。";
                return false;
            }

            // ========== 第一阶段：确定新增子项序号 ==========
            // 替换/加收子项要进入同一个 HIS 明细集合，SequenceNO 必须避开原有明细。
            int nextSequence = GetNextOutpatientSequence(feeDetails);
            foreach (PricingCalculateItemResponse item in response.Items)
            {
                // ========== 第二阶段：用 ItemRequestNo 找回 HIS 原始行 ==========
                // 统一计价响应不能凭 itemCode 回填，因为一次收费中同一项目可能出现多行。
                object mapped;
                if (!itemMap.TryGetValue(item.ItemRequestNo, out mapped))
                {
                    error = "统一计价返回了 HIS 未提交的明细：" + item.ItemRequestNo;
                    return false;
                }

                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList parent =
                    mapped as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                if (parent == null)
                {
                    error = "门诊费用明细映射类型错误：" + item.ItemRequestNo;
                    return false;
                }

                // ========== 第三阶段：拆分主项目金额和子项金额 ==========
                // 服务端 FinalAmount 是该请求行的总额；旧 HIS 落账需要主项和新增子项分别成行。
                decimal childAmount = SumChildAmount(item);
                decimal mainAmount = item.FinalAmount - childAmount;
                if (mainAmount < 0)
                {
                    error = "统一计价返回的子项金额大于总金额：" + item.ItemRequestNo;
                    return false;
                }

                // ========== 第四阶段：回写主项目金额 ==========
                ApplyAmount(parent, item.FinalQty, mainAmount);

                // ========== 第五阶段：追加超限替换子项 ==========
                if (item.ReplacementItem != null && item.ReplacementItem.Qty > 0)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList replacement =
                        CreateOutpatientChild(parent, item.ReplacementItem.ItemCode, item.ReplacementItem.ItemName,
                            item.ReplacementItem.Qty, item.ReplacementItem.UnitPrice, item.ReplacementItem.Amount,
                            nextSequence++, ref error);
                    if (replacement == null)
                    {
                        return false;
                    }

                    feeDetails.Add(replacement);
                }

                // ========== 第六阶段：追加普通加收子项 ==========
                if (item.ChildItems != null)
                {
                    foreach (PricingChildItemResponse child in item.ChildItems)
                    {
                        if (child == null || child.Qty <= 0)
                        {
                            continue;
                        }

                        Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList childFee =
                            CreateOutpatientChild(parent, child.ItemCode, child.ItemName,
                                child.Qty, child.UnitPrice, child.Amount, nextSequence++, ref error);
                        if (childFee == null)
                        {
                            return false;
                        }

                        feeDetails.Add(childFee);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 将 confirm 响应回填到住院费用明细集合。
        /// </summary>
        /// <param name="feeDetails">待保存的住院明细集合，方法会原地修改和追加子项。</param>
        /// <param name="itemMap">请求明细号到 HIS 原明细的映射。</param>
        /// <param name="response">统一计价 confirm 响应。</param>
        /// <param name="error">回填失败时返回错误原因。</param>
        /// <returns>全部回填成功时返回 true。</returns>
        private bool ApplyInpatientResult(
            ArrayList feeDetails,
            Dictionary<string, object> itemMap,
            PricingCalculateResponse response,
            ref string error)
        {
            if (response == null || response.Items == null || response.Items.Count == 0)
            {
                error = "统一计价 confirm 未返回费用明细。";
                return false;
            }

            // ========== 第一阶段：确定新增住院子项序号 ==========
            int nextSequence = GetNextInpatientSequence(feeDetails);
            foreach (PricingCalculateItemResponse item in response.Items)
            {
                // ========== 第二阶段：用请求行号定位 HIS 原始住院明细 ==========
                object mapped;
                if (!itemMap.TryGetValue(item.ItemRequestNo, out mapped))
                {
                    error = "统一计价返回了 HIS 未提交的明细：" + item.ItemRequestNo;
                    return false;
                }

                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList parent =
                    mapped as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                if (parent == null)
                {
                    error = "住院费用明细映射类型错误：" + item.ItemRequestNo;
                    return false;
                }

                // ========== 第三阶段：拆分主项金额和子项金额 ==========
                // 住院与门诊保持同一口径：主项金额 = 响应总金额 - 替换/加收子项金额。
                decimal childAmount = SumChildAmount(item);
                decimal mainAmount = item.FinalAmount - childAmount;
                if (mainAmount < 0)
                {
                    error = "统一计价返回的子项金额大于总金额：" + item.ItemRequestNo;
                    return false;
                }

                // ========== 第四阶段：回写住院主项目金额 ==========
                ApplyAmount(parent, item.FinalQty, mainAmount);

                // ========== 第五阶段：追加住院替换子项 ==========
                if (item.ReplacementItem != null && item.ReplacementItem.Qty > 0)
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList replacement =
                        CreateInpatientChild(parent, item.ReplacementItem.ItemCode, item.ReplacementItem.ItemName,
                            item.ReplacementItem.Qty, item.ReplacementItem.UnitPrice, item.ReplacementItem.Amount,
                            nextSequence++, ref error);
                    if (replacement == null)
                    {
                        return false;
                    }

                    feeDetails.Add(replacement);
                }

                // ========== 第六阶段：追加住院普通加收子项 ==========
                if (item.ChildItems != null)
                {
                    foreach (PricingChildItemResponse child in item.ChildItems)
                    {
                        if (child == null || child.Qty <= 0)
                        {
                            continue;
                        }

                        Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList childFee =
                            CreateInpatientChild(parent, child.ItemCode, child.ItemName,
                                child.Qty, child.UnitPrice, child.Amount, nextSequence++, ref error);
                        if (childFee == null)
                        {
                            return false;
                        }

                        feeDetails.Add(childFee);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 根据统一计价返回的替换/加收子项创建门诊 HIS 明细。
        /// </summary>
        /// <remarks>
        /// 子项必须来自 HIS 本地非药品主数据，不能只根据计价中心返回的编码临时拼出一条费用；
        /// 否则 HIS 后续医保、对账、报表链路会出现项目主数据缺失。
        /// </remarks>
        private Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList CreateOutpatientChild(
            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList parent,
            string itemCode,
            string itemName,
            decimal qty,
            decimal unitPrice,
            decimal amount,
            int sequenceNo,
            ref string error)
        {
            Neusoft.HISFC.Models.Fee.Item.Undrug item = GetUndrugItem(itemCode, ref error);
            if (item == null)
            {
                return null;
            }

            // 旧 HIS 明细字段很多，克隆父项可以继承患者、发票、科室、收费员等上下文。
            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList child = parent.Clone();
            child.Item = item.Clone();
            child.Item.ID = itemCode;
            child.Item.Name = !IsBlank(itemName) ? itemName : item.Name;
            child.Item.Qty = qty;
            child.Item.Price = unitPrice;
            child.SequenceNO = sequenceNo;
            // 新增子项不是原医嘱项目，清空 Order.ID，避免被误认为原医嘱重复收费。
            child.Order.ID = string.Empty;
            child.UndrugComb.ID = parent.Item.ID;
            child.UndrugComb.Name = parent.Item.Name;
            ApplyAmount(child, qty, amount);
            return child;
        }

        /// <summary>
        /// 根据统一计价返回的替换/加收子项创建住院 HIS 明细。
        /// </summary>
        /// <remarks>
        /// 住院明细同样继承父项上下文，只替换项目、数量、单价、金额和序号；这样可以最大限度复用
        /// HIS 原有记账、科室归属、患者账户和退费字段。
        /// </remarks>
        private Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList CreateInpatientChild(
            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList parent,
            string itemCode,
            string itemName,
            decimal qty,
            decimal unitPrice,
            decimal amount,
            int sequenceNo,
            ref string error)
        {
            Neusoft.HISFC.Models.Fee.Item.Undrug item = GetUndrugItem(itemCode, ref error);
            if (item == null)
            {
                return null;
            }

            // 继承父项上下文后只覆盖真正发生变化的项目和金额字段。
            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList child = parent.Clone();
            child.Item = item.Clone();
            child.Item.ID = itemCode;
            child.Item.Name = !IsBlank(itemName) ? itemName : item.Name;
            child.Item.Qty = qty;
            child.Item.Price = unitPrice;
            child.SequenceNO = sequenceNo;
            child.UndrugComb.ID = parent.Item.ID;
            child.UndrugComb.Name = parent.Item.Name;
            // 新增住院子项尚未退费，未退数量应等于本次落账数量。
            child.NoBackQty = qty;
            ApplyAmount(child, qty, amount);
            return child;
        }

        /// <summary>
        /// 从 HIS 项目主数据读取非药品项目。
        /// </summary>
        /// <param name="itemCode">统一计价返回的替换或加收项目编码。</param>
        /// <param name="error">项目不存在或编码为空时返回错误。</param>
        /// <returns>HIS 非药品项目主数据；不存在时返回 null。</returns>
        private Neusoft.HISFC.Models.Fee.Item.Undrug GetUndrugItem(string itemCode, ref string error)
        {
            if (IsBlank(itemCode))
            {
                error = "统一计价返回的替换/加收子项编码为空。";
                return null;
            }

            Neusoft.HISFC.Models.Fee.Item.Undrug item = itemManager.GetItemByUndrugCode(itemCode);
            if (item == null)
            {
                error = "HIS 未找到统一计价返回的替换/加收子项：" + itemCode + "，已阻断落账。";
                return null;
            }

            return item;
        }

        /// <summary>
        /// 将统一计价确认后的数量和金额写回 HIS 费用明细。
        /// </summary>
        /// <remarks>
        /// 这里把自费、医保、公费和折让拆分归零后只保留 TotCost/OwnCost，是因为统一计价当前只负责
        /// 物价折价金额，不接管医保结算拆分。后续医保分摊仍由 HIS 原链路处理。
        /// </remarks>
        private static void ApplyAmount(Neusoft.HISFC.Models.Fee.FeeItemBase fee, decimal qty, decimal amount)
        {
            fee.Item.Qty = qty;
            fee.FT.TotCost = RoundMoney(amount);
            fee.FT.OwnCost = RoundMoney(amount);
            fee.FT.PayCost = 0;
            fee.FT.PubCost = 0;
            fee.FT.RebateCost = 0;
            if (fee.Item.DefPrice == 0)
            {
                fee.Item.DefPrice = fee.Item.Price;
            }
            fee.FT.DefTotCost = RoundMoney(fee.Item.DefPrice * qty);
        }

        /// <summary>
        /// 登记一个已 confirm、等待 HIS 保存结果的请求。
        /// </summary>
        private PricingPendingCharge RegisterPending(long requestId, string chargeNo, ArrayList feeDetails, string scene)
        {
            PricingPendingCharge pending = new PricingPendingCharge();
            pending.RequestId = requestId;
            pending.ChargeNo = chargeNo;
            pending.FeeDetails = feeDetails;
            pending.Scene = scene;
            pendingCharges.Add(pending);
            return pending;
        }

        /// <summary>
        /// 从 HIS 已保存成功的费用明细集合构造 commit 所需的真实落账明细。
        /// </summary>
        /// <remarks>
        /// 不能直接拿 confirm 响应拼 actualItems。commit 的目的就是校验 HIS 实际落账是否与 confirm 一致，
        /// 因此必须回读或复用 HIS 已经保存成功的明细对象。
        /// </remarks>
        private static List<PricingCommitActualItemRequest> BuildActualItems(ArrayList feeDetails, string scene)
        {
            List<PricingCommitActualItemRequest> actualItems = new List<PricingCommitActualItemRequest>();
            for (int i = 0; i < feeDetails.Count; i++)
            {
                if (scene == OutpatientScene)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList fee =
                        feeDetails[i] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    if (fee == null)
                    {
                        continue;
                    }

                    actualItems.Add(new PricingCommitActualItemRequest
                    {
                        ChargeDetailNo = BuildChargeDetailNo(fee.RecipeNO, fee.SequenceNO),
                        ItemCode = Safe(fee.Item.ID),
                        FinalQty = fee.Item.Qty,
                        FinalAmount = fee.FT.TotCost
                    });
                }
                else
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList fee =
                        feeDetails[i] as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                    if (fee == null)
                    {
                        continue;
                    }

                    actualItems.Add(new PricingCommitActualItemRequest
                    {
                        ChargeDetailNo = BuildChargeDetailNo(fee.RecipeNO, fee.SequenceNO),
                        ItemCode = Safe(fee.Item.ID),
                        FinalQty = fee.Item.Qty,
                        FinalAmount = fee.FT.TotCost
                    });
                }
            }

            return actualItems;
        }

        /// <summary>
        /// 汇总 HIS 实际落账金额，用于 commit 总金额校验。
        /// </summary>
        private static decimal SumActualAmount(List<PricingCommitActualItemRequest> actualItems)
        {
            decimal total = 0;
            for (int i = 0; i < actualItems.Count; i++)
            {
                total += actualItems[i].FinalAmount;
            }

            return total;
        }

        /// <summary>
        /// 汇总一个计价响应行中所有替换/加收子项金额。
        /// </summary>
        private static decimal SumChildAmount(PricingCalculateItemResponse item)
        {
            decimal amount = 0;
            if (item.ReplacementItem != null)
            {
                amount += item.ReplacementItem.Amount;
            }

            if (item.ChildItems != null)
            {
                foreach (PricingChildItemResponse child in item.ChildItems)
                {
                    if (child != null)
                    {
                        amount += child.Amount;
                    }
                }
            }

            return amount;
        }

        /// <summary>
        /// 获取门诊新增子项可使用的下一个 SequenceNO。
        /// </summary>
        private static int GetNextOutpatientSequence(ArrayList feeDetails)
        {
            int max = 0;
            foreach (object obj in feeDetails)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList fee =
                    obj as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                if (fee != null && fee.SequenceNO > max)
                {
                    max = fee.SequenceNO;
                }
            }

            return max + 1;
        }

        /// <summary>
        /// 获取住院新增子项可使用的下一个 SequenceNO。
        /// </summary>
        private static int GetNextInpatientSequence(ArrayList feeDetails)
        {
            int max = 0;
            foreach (object obj in feeDetails)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList fee =
                    obj as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                if (fee != null && fee.SequenceNO > max)
                {
                    max = fee.SequenceNO;
                }
            }

            return max + 1;
        }

        /// <summary>
        /// 生成统一计价使用的收费明细号。
        /// </summary>
        /// <remarks>
        /// 旧 HIS 在保存前可能只有处方号和序号，没有独立明细主键，因此这里用
        /// <c>RecipeNO-SequenceNO</c> 形成稳定行号。
        /// </remarks>
        private static string BuildChargeDetailNo(string recipeNo, int sequenceNo)
        {
            if (IsBlank(recipeNo))
            {
                return sequenceNo.ToString();
            }

            return recipeNo + "-" + sequenceNo.ToString();
        }

        /// <summary>
        /// 生成本次请求内唯一的费用明细请求号。
        /// </summary>
        private static string BuildItemRequestNo(string recipeNo, int sequenceNo, int index)
        {
            return BuildChargeDetailNo(recipeNo, sequenceNo) + "-" + index.ToString();
        }

        /// <summary>
        /// 构造统一计价明细扩展参数，保留旧 HIS 行号、医嘱号和组套关系。
        /// </summary>
        private static Dictionary<string, object> BuildCommonExtraParams(
            string recipeNo,
            int sequenceNo,
            string orderId,
            string packageCode)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values["recipeNo"] = Safe(recipeNo);
            values["sequenceNo"] = sequenceNo;
            values["orderId"] = Safe(orderId);
            values["packageCode"] = Safe(packageCode);
            return values;
        }

        /// <summary>
        /// 按业务收费时间计算患者年龄。
        /// </summary>
        private static int? CalculateAge(DateTime birthday, DateTime businessTime)
        {
            if (birthday <= DateTime.MinValue || businessTime <= birthday)
            {
                return null;
            }

            int age = businessTime.Year - birthday.Year;
            if (businessTime.Month < birthday.Month
                || (businessTime.Month == birthday.Month && businessTime.Day < birthday.Day))
            {
                age--;
            }

            return age < 0 ? (int?)null : age;
        }

        /// <summary>
        /// 安全取消 confirm 保护占用。
        /// </summary>
        /// <remarks>
        /// cancel 失败不能打断旧 HIS 的回滚流程，SDK 会负责写本地补偿队列。
        /// </remarks>
        private static void SafeCancel(PricingAgent pricingAgent, long requestId)
        {
            if (pricingAgent == null || requestId <= 0)
            {
                return;
            }

            try
            {
                pricingAgent.CancelAfterHisFailure(requestId);
            }
            catch
            {
                // SDK 已写入本地补偿队列，旧 HIS 事务回滚流程不能再被网络异常打断。
            }
        }

        /// <summary>
        /// 按收费金额口径保留两位小数。
        /// </summary>
        private static decimal RoundMoney(decimal value)
        {
            return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 判断旧 HIS 字符串是否为空白。
        /// </summary>
        private static bool IsBlank(string value)
        {
            return value == null || value.Trim().Length == 0;
        }

        /// <summary>
        /// 将 null 转为空字符串并去除首尾空白，便于老接口安全接收。
        /// </summary>
        private static string Safe(string value)
        {
            return value == null ? string.Empty : value.Trim();
        }
    }

    /// <summary>
    /// 旧 HIS 接入层内部使用的 confirm 结果。
    /// </summary>
    /// <remarks>
    /// 该类型不暴露给 PricingAgent SDK，也不作为接口 DTO。它只帮助旧 HIS 调用点区分
    /// 未配置、普通计价、已确认和阻断四类结果，避免每个收费方法重复写判断。
    /// </remarks>
    internal sealed class PricingConfirmResult
    {
        /// <summary>
        /// 是否允许 HIS 继续执行本地写库。
        /// </summary>
        public bool AllowCharge;

        /// <summary>
        /// 是否已经检测到 PricingAgent 配置。false 表示完全走旧逻辑。
        /// </summary>
        public bool Configured;

        /// <summary>
        /// 是否完成特殊计价 confirm，并产生了待 commit 的请求。
        /// </summary>
        public bool Confirmed;

        /// <summary>
        /// 是否为普通计价结果。普通计价不需要 commit/cancel。
        /// </summary>
        public bool OrdinaryPricing;

        /// <summary>
        /// 返回给旧 HIS 错误框或日志的说明。
        /// </summary>
        public string Message;

        /// <summary>
        /// 已 confirm 的待提交对象。只有 Confirmed 为 true 时有值。
        /// </summary>
        public PricingPendingCharge PendingCharge;

        /// <summary>
        /// 创建未配置结果。未配置时必须允许旧 HIS 原有逻辑继续运行。
        /// </summary>
        public static PricingConfirmResult NotConfigured()
        {
            return new PricingConfirmResult
            {
                AllowCharge = true,
                Configured = false,
                Confirmed = false,
                OrdinaryPricing = true
            };
        }

        /// <summary>
        /// 创建普通计价结果。表示 Agent 已启用，但本次收费未命中特殊计价。
        /// </summary>
        public static PricingConfirmResult Ordinary(string message)
        {
            return new PricingConfirmResult
            {
                AllowCharge = true,
                Configured = true,
                Confirmed = false,
                OrdinaryPricing = true,
                Message = message
            };
        }

        /// <summary>
        /// 创建已确认结果。调用方后续必须在 HIS 保存成功后标记并在事务提交后 commit。
        /// </summary>
        public static PricingConfirmResult ConfirmedCharge(PricingPendingCharge pending, string message)
        {
            return new PricingConfirmResult
            {
                AllowCharge = true,
                Configured = true,
                Confirmed = true,
                OrdinaryPricing = false,
                PendingCharge = pending,
                Message = message
            };
        }

        /// <summary>
        /// 创建阻断结果。调用方必须停止收费并把 Message 返回给 HIS 用户或日志。
        /// </summary>
        public static PricingConfirmResult Blocked(string message)
        {
            return new PricingConfirmResult
            {
                AllowCharge = false,
                Configured = true,
                Confirmed = false,
                OrdinaryPricing = false,
                Message = message
            };
        }
    }

    /// <summary>
    /// 已 confirm、等待 HIS 本地事务结论的请求快照。
    /// </summary>
    /// <remarks>
    /// confirm 到 commit/cancel 之间存在一个旧 HIS 本地事务窗口。该对象保存 RequestId 和明细集合引用，
    /// 让事务成功后能用 HIS 实际明细构造 actualItems，事务失败后能释放保护占用。
    /// </remarks>
    internal sealed class PricingPendingCharge
    {
        /// <summary>
        /// 统一计价 confirm 返回的请求 ID。
        /// </summary>
        public long RequestId;

        /// <summary>
        /// HIS 收费单号或业务号，commit 时回传用于对账。
        /// </summary>
        public string ChargeNo;

        /// <summary>
        /// 收费场景，用于区分 FeeDetails 中对象是门诊明细还是住院明细。
        /// </summary>
        public string Scene;

        /// <summary>
        /// HIS 原始费用明细集合引用。保存成功后从这里构造真实 actualItems。
        /// </summary>
        public ArrayList FeeDetails;

        /// <summary>
        /// HIS 本地费用明细是否已经保存成功。
        /// </summary>
        public bool HisSaveSucceeded;

        /// <summary>
        /// HIS 保存成功后重建的真实落账明细。
        /// </summary>
        public List<PricingCommitActualItemRequest> ActualItems;

        /// <summary>
        /// HIS 保存成功后的真实落账总金额。
        /// </summary>
        public decimal? ActualTotalAmount;
    }

    /// <summary>
    /// 门诊收费界面使用的统一计价预览门面。
    /// </summary>
    /// <remarks>
    /// 该类型故意把 HIS.Pricing.Client 的 DTO 隔离在 Integrate 程序集内部，界面工程只依赖
    /// HISFC.BizProcess.Integrate 即可拿到可展示的金额、折价和加收摘要。
    /// </remarks>
    public static class PricingOutpatientPreviewService
    {
        /// <summary>
        /// 对门诊费用明细执行 simulate 预览。
        /// </summary>
        public static PricingOutpatientPreviewResult Simulate(
            Register register,
            ArrayList feeDetails,
            string chargeNo,
            DateTime chargeTime,
            Employee oper,
            string fallbackDeptCode,
            Neusoft.HISFC.BizLogic.Fee.Item itemManager)
        {
            if (itemManager == null)
            {
                return PricingOutpatientPreviewResult.Failed("统一计价试算缺少 HIS 项目业务层。");
            }

            PricingLegacyChargeBridge bridge = new PricingLegacyChargeBridge(itemManager);
            return bridge.SimulateOutpatientForDisplay(
                register,
                feeDetails,
                chargeNo,
                chargeTime,
                oper == null ? string.Empty : oper.ID,
                oper == null ? string.Empty : oper.Name,
                GetOperatorDeptCode(oper, fallbackDeptCode));
        }

        /// <summary>
        /// 收费员科室优先取登录操作员科室；为空时回落到患者当前看诊科室。
        /// </summary>
        private static string GetOperatorDeptCode(Employee oper, string fallbackDeptCode)
        {
            if (oper != null && oper.Dept != null && !string.IsNullOrEmpty(oper.Dept.ID))
            {
                return oper.Dept.ID;
            }

            return fallbackDeptCode;
        }
    }

    /// <summary>
    /// 门诊统一计价试算结果。
    /// </summary>
    public sealed class PricingOutpatientPreviewResult
    {
        /// <summary>
        /// 是否已启用 PricingAgent 配置。
        /// </summary>
        public bool Configured;

        /// <summary>
        /// simulate 是否成功返回可展示结果。
        /// </summary>
        public bool Success;

        /// <summary>
        /// 失败或普通说明。
        /// </summary>
        public string Message;

        /// <summary>
        /// 按请求明细顺序排列的预览结果。
        /// </summary>
        public ArrayList Items = new ArrayList();

        /// <summary>
        /// 创建未启用结果。界面收到该结果时继续走旧展示逻辑。
        /// </summary>
        public static PricingOutpatientPreviewResult NotConfigured()
        {
            return new PricingOutpatientPreviewResult
            {
                Configured = false,
                Success = false,
                Message = "未启用统一计价。"
            };
        }

        /// <summary>
        /// 创建失败结果。界面可以保留原价展示，最终收费 confirm 仍会再次校验并阻断。
        /// </summary>
        public static PricingOutpatientPreviewResult Failed(string message)
        {
            return new PricingOutpatientPreviewResult
            {
                Configured = true,
                Success = false,
                Message = message
            };
        }

        /// <summary>
        /// 从统一计价响应转换为界面预览模型。
        /// </summary>
        internal static PricingOutpatientPreviewResult FromResponse(PricingCalculateResponse response)
        {
            PricingOutpatientPreviewResult result = new PricingOutpatientPreviewResult();
            result.Configured = true;
            result.Success = true;
            result.Message = "统一计价试算成功。";

            if (response == null || response.Items == null)
            {
                return result;
            }

            foreach (PricingCalculateItemResponse item in response.Items)
            {
                if (item == null)
                {
                    continue;
                }

                PricingOutpatientPreviewItem preview = new PricingOutpatientPreviewItem();
                preview.ItemRequestNo = item.ItemRequestNo;
                preview.ChargeDetailNo = item.ChargeDetailNo;
                preview.ItemCode = item.ItemCode;
                preview.ItemName = item.ItemName;
                preview.InputQty = item.InputQty;
                preview.FinalQty = item.FinalQty;
                preview.UnitPrice = item.UnitPrice;
                preview.OriginalAmount = RoundMoney(item.InputQty * item.UnitPrice);
                preview.FinalAmount = item.FinalAmount;
                preview.DiscountAmount = item.DiscountAmount;
                preview.IsSpecialItem = item.IsSpecialItem;
                preview.ChildAmount = SumChildAmount(item);
                preview.Summary = BuildSummary(item, preview);
                result.Items.Add(preview);
            }

            return result;
        }

        /// <summary>
        /// 按明细顺序获取预览结果。
        /// </summary>
        public PricingOutpatientPreviewItem GetItem(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return null;
            }

            return Items[index] as PricingOutpatientPreviewItem;
        }

        private static string BuildSummary(PricingCalculateItemResponse item, PricingOutpatientPreviewItem preview)
        {
            List<string> parts = new List<string>();
            parts.Add("统一计价预览");
            parts.Add("原价" + FormatMoney(preview.OriginalAmount));
            parts.Add("最终" + FormatMoney(preview.FinalAmount));

            if (preview.DiscountAmount != 0)
            {
                parts.Add("折价" + FormatMoney(preview.DiscountAmount));
            }

            if (preview.InputQty != preview.FinalQty)
            {
                parts.Add("数量" + FormatQty(preview.InputQty) + "->" + FormatQty(preview.FinalQty));
            }

            if (item.ReplacementItem != null && item.ReplacementItem.Qty > 0)
            {
                parts.Add("替换子项" + Safe(item.ReplacementItem.ItemCode)
                    + "/" + FormatMoney(item.ReplacementItem.Amount));
            }

            if (item.ChildItems != null && item.ChildItems.Count > 0)
            {
                parts.Add("加收子项" + FormatMoney(preview.ChildAmount));
            }

            return string.Join("；", parts.ToArray());
        }

        private static decimal SumChildAmount(PricingCalculateItemResponse item)
        {
            decimal amount = 0;
            if (item.ReplacementItem != null)
            {
                amount += item.ReplacementItem.Amount;
            }

            if (item.ChildItems != null)
            {
                foreach (PricingChildItemResponse child in item.ChildItems)
                {
                    if (child != null)
                    {
                        amount += child.Amount;
                    }
                }
            }

            return amount;
        }

        private static string FormatMoney(decimal value)
        {
            return RoundMoney(value).ToString("0.00");
        }

        private static string FormatQty(decimal value)
        {
            return value.ToString("0.####");
        }

        private static decimal RoundMoney(decimal value)
        {
            return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private static string Safe(string value)
        {
            return value == null ? string.Empty : value;
        }
    }

    /// <summary>
    /// 单条门诊费用的统一计价预览结果。
    /// </summary>
    public sealed class PricingOutpatientPreviewItem
    {
        public string ItemRequestNo;
        public string ChargeDetailNo;
        public string ItemCode;
        public string ItemName;
        public decimal InputQty;
        public decimal FinalQty;
        public decimal UnitPrice;
        public decimal OriginalAmount;
        public decimal FinalAmount;
        public decimal DiscountAmount;
        public decimal ChildAmount;
        public bool IsSpecialItem;
        public string Summary;
    }
}
