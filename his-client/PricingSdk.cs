using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 统一计价产品 SDK。
    /// 该层只负责业务接口编排，不负责弹窗；适合 HIS、移动端、自助机、第三方系统共同复用。
    /// </summary>
    public sealed class PricingSdk
    {
        private readonly PricingApiClient _client;
        private readonly string _sourceSystem;
        private readonly string _defaultChargeScene;
        private readonly PricingSdkOptions _options;
        private readonly PricingAgentLogger _logger;
        private readonly PricingCompensationStore _compensationStore;

        /// <summary>
        /// 使用配置创建 SDK。适合产品化交付时从配置文件读取 BaseUrl 后直接初始化。
        /// </summary>
        public PricingSdk(PricingSdkOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            options.ValidateForHttpClient();
            options.ValidateForProductRuntime();
            _client = new PricingApiClient(
                options.GetNormalizedBaseUrl(),
                options.TimeoutMs,
                options.MaxRetry,
                options.RetryDelayMs);
            _sourceSystem = !string.IsNullOrEmpty(options.SourceSystem) ? options.SourceSystem : "HIS";
            _defaultChargeScene = !string.IsNullOrEmpty(options.DefaultChargeScene) ? options.DefaultChargeScene : "OUTPATIENT";
            _options = options;
            _logger = PricingAgentLogger.Create(options);
            _compensationStore = PricingCompensationStore.Create(options);
        }

        /// <summary>
        /// 使用调用方已创建的 HTTP 客户端创建 SDK。
        /// </summary>
        public PricingSdk(PricingApiClient client)
            : this(client, null)
        {
        }

        /// <summary>
        /// 使用调用方已创建的 HTTP 客户端和产品配置创建 SDK。
        /// </summary>
        public PricingSdk(PricingApiClient client, PricingSdkOptions options)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            if (options != null)
            {
                options.ValidateForProductRuntime();
            }
            _sourceSystem = options != null && !string.IsNullOrEmpty(options.SourceSystem) ? options.SourceSystem : "HIS";
            _defaultChargeScene = options != null && !string.IsNullOrEmpty(options.DefaultChargeScene) ? options.DefaultChargeScene : "OUTPATIENT";
            _options = options;
            _logger = PricingAgentLogger.Create(options);
            _compensationStore = PricingCompensationStore.Create(options);
        }

        /// <summary>底层 HTTP 客户端，供高级集成场景直接访问。</summary>
        public PricingApiClient Client
        {
            get { return _client; }
        }

        /// <summary>当前 SDK 运行配置。调用方直接传入 HTTP 客户端时可能为空。</summary>
        public PricingSdkOptions Options
        {
            get { return _options; }
        }

        /// <summary>本地日志目录。未启用日志时为空。</summary>
        public string LogDirectory
        {
            get { return _logger == null ? null : _logger.LogDirectory; }
        }

        /// <summary>本地补偿队列目录。未启用补偿队列时为空。</summary>
        public string CompensationDirectory
        {
            get { return _compensationStore == null ? null : _compensationStore.DirectoryPath; }
        }

        /// <summary>
        /// 查询服务健康状态并校验协议版本。
        /// </summary>
        public ApiResponse<PricingServiceHealthResponse> CheckServiceCompatibility()
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                ApiResponse<PricingServiceHealthResponse> response = _client.GetHealth();
                watch.Stop();
                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    LogError("health", null, null, null, response == null ? null : response.TraceId,
                        response == null ? null : (int?)response.Code,
                        watch.ElapsedMilliseconds,
                        response == null ? "健康检查无响应。" : response.Message);
                    return response;
                }

                PricingAgentVersion.EnsureCompatibleService(
                    response.Data,
                    _options == null ? PricingAgentVersion.ProtocolVersion : _options.ExpectedProtocolVersion);
                LogInfo("health", null, null, null, response.TraceId, response.Code, watch.ElapsedMilliseconds, "服务健康检查通过。");
                return response;
            }
            catch (Exception ex)
            {
                watch.Stop();
                LogError("health", null, null, null, null, null, watch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 检查项目是否需要进入特殊计价流程。
        /// 计价服务不可用时返回阻断决策，不能按普通计价放行。
        /// </summary>
        public SpecialPricingDecision CheckSpecialPricingRequired(string itemCode)
        {
            if (itemCode == null || itemCode.Trim().Length == 0)
            {
                return SpecialPricingDecision.Blocked("项目编码为空，禁止继续收费。");
            }

            itemCode = itemCode.Trim();
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                ApiResponse<SpecialFlagResponse> response = _client.GetSpecialFlag(itemCode);
                watch.Stop();
                LogInfo("special-flag", null, null, null, response == null ? null : response.TraceId,
                    response == null ? null : (int?)response.Code,
                    watch.ElapsedMilliseconds,
                    response == null ? "特殊计价标识查询无响应。" : response.Message);

                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    return SpecialPricingDecision.BlockAsServiceUnavailable(
                        "特殊计价标识查询失败，禁止按普通计价继续收费。");
                }

                if (response.Data.IsSpecial)
                {
                    return SpecialPricingDecision.RequirePopup("命中特殊计价项目。");
                }

                return SpecialPricingDecision.AllowOrdinary("非特殊计价项目。");
            }
            catch (Exception ex)
            {
                watch.Stop();
                LogError("special-flag", null, null, null, null, null, watch.ElapsedMilliseconds, ex.Message);
                return SpecialPricingDecision.BlockAsServiceUnavailable(
                    "计价服务暂时不可用，禁止按普通计价继续收费：" + ex.Message);
            }
        }

        /// <summary>
        /// 使用轻量收费上下文构造计价请求。
        /// </summary>
        public PricingCalculateRequest BuildChargeRequest(PricingChargeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.ToCalculateRequest(_sourceSystem, _defaultChargeScene);
        }

        /// <summary>
        /// 试算，不占用额度。
        /// </summary>
        public ApiResponse<PricingCalculateResponse> Simulate(PricingCalculateRequest request)
        {
            PrepareCalculateRequest(request);
            return ExecuteWithLog("simulate", request, delegate { return _client.Simulate(request); });
        }

        /// <summary>
        /// 收费保存前确认计价，占用额度。
        /// HIS 本地落账成功后必须调用 CommitAfterHisSuccess；落账失败才调用 CancelAfterHisFailure。
        /// </summary>
        public ApiResponse<PricingCalculateResponse> ConfirmBeforeCharge(PricingCalculateRequest request)
        {
            ValidateConfirmRequest(request);
            return ExecuteWithLog("confirm", request, delegate { return _client.Confirm(request); });
        }

        /// <summary>
        /// HIS 落账成功后提交计价中心状态，并回传真实落账明细。
        /// </summary>
        public ApiResponse CommitAfterHisSuccess(
            long requestId,
            string chargeNo,
            List<PricingCommitActualItemRequest> actualItems,
            decimal? actualTotalAmount)
        {
            PricingCalculateRequestValidator.ValidateCommitActuals(requestId, actualItems);

            PricingCommitRequest request = new PricingCommitRequest();
            request.RequestId = requestId;
            request.ChargeNo = chargeNo;
            request.ActualItems = actualItems;
            request.ActualTotalAmount = actualTotalAmount;
            ApiResponse response = ExecuteNoDataWithLog(
                "commit",
                request.RequestId.ToString(),
                chargeNo,
                null,
                request,
                delegate { return _client.Commit(request); });
            SaveCompensationIfFailed("commit", request.RequestId.ToString(), request, response, null);
            return response;
        }

        /// <summary>
        /// HIS 落账失败后取消 confirm 占用。
        /// 注意：HIS 已真实落账成功但 commit 通知失败时不能调用本方法。
        /// </summary>
        public ApiResponse CancelAfterHisFailure(long requestId)
        {
            PricingCalculateRequestValidator.ValidateCancelRequest(requestId);
            PricingCancelRequest request = new PricingCancelRequest();
            request.RequestId = requestId;
            ApiResponse response = ExecuteNoDataWithLog(
                "cancel",
                request.RequestId.ToString(),
                null,
                null,
                request,
                delegate { return _client.Cancel(request); });
            SaveCompensationIfFailed("cancel", request.RequestId.ToString(), request, response, null);
            return response;
        }

        /// <summary>
        /// HIS 退费或冲正后通知计价中心释放额度。
        /// </summary>
        public ApiResponse ReverseAfterHisRefund(PricingReverseRequest request)
        {
            PricingCalculateRequestValidator.ValidateReverseRequest(request);
            ApiResponse response = ExecuteNoDataWithLog(
                "reverse",
                request.OriginalRequestId.ToString(),
                request.ReverseNo,
                null,
                request,
                delegate { return _client.Reverse(request); });
            SaveCompensationIfFailed("reverse", request.ReverseNo, request, response, null);
            return response;
        }

        /// <summary>
        /// 补齐计价请求的产品默认字段，并做最小必要校验。
        /// </summary>
        public void PrepareCalculateRequest(PricingCalculateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (string.IsNullOrEmpty(request.SourceSystem))
            {
                request.SourceSystem = _sourceSystem;
            }

            if (string.IsNullOrEmpty(request.ChargeScene))
            {
                request.ChargeScene = _defaultChargeScene;
            }

            if (string.IsNullOrEmpty(request.RequestNo))
            {
                request.RequestNo = request.SourceSystem + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
            }

            if (request.BusinessChargeTime == default(DateTime))
            {
                request.BusinessChargeTime = DateTime.Now;
            }

            if (string.IsNullOrEmpty(request.BusinessRequestNo) && !string.IsNullOrEmpty(request.ChargeNo))
            {
                request.BusinessRequestNo = PricingBusinessRequestNoHelper.EnsureBusinessRequestNo(null, request.ChargeNo);
            }

            PricingCalculateRequestValidator.ValidateForCalculate(request);
        }

        /// <summary>
        /// 校验 confirm 请求是否满足幂等要求。
        /// confirm 前必须存在稳定业务号，不能由 SDK 在无 ChargeNo 时随机生成。
        /// </summary>
        public void ValidateConfirmRequest(PricingCalculateRequest request)
        {
            PrepareCalculateRequest(request);
            PricingCalculateRequestValidator.ValidateForConfirm(request);
        }

        private ApiResponse<T> ExecuteWithLog<T>(
            string operation,
            PricingCalculateRequest request,
            Func<ApiResponse<T>> action)
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                ApiResponse<T> response = action();
                watch.Stop();
                LogInfo(operation, null, request.BusinessRequestNo, request.RequestNo,
                    response == null ? null : response.TraceId,
                    response == null ? null : (int?)response.Code,
                    watch.ElapsedMilliseconds,
                    response == null ? "接口无响应。" : response.Message);
                return response;
            }
            catch (Exception ex)
            {
                watch.Stop();
                LogError(operation, null, request.BusinessRequestNo, request.RequestNo, null, null, watch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        private ApiResponse ExecuteNoDataWithLog(
            string operation,
            string requestId,
            string businessNo,
            string requestNo,
            object requestPayload,
            Func<ApiResponse> action)
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                ApiResponse response = action();
                watch.Stop();
                LogInfo(operation, requestId, businessNo, requestNo,
                    response == null ? null : response.TraceId,
                    response == null ? null : (int?)response.Code,
                    watch.ElapsedMilliseconds,
                    response == null ? "接口无响应。" : response.Message);
                return response;
            }
            catch (Exception ex)
            {
                watch.Stop();
                LogError(operation, requestId, businessNo, requestNo, null, null, watch.ElapsedMilliseconds, ex.Message);
                SaveCompensationIfFailed(operation, businessNo != null ? businessNo : requestId, requestPayload, null, ex);
                throw;
            }
        }

        private void SaveCompensationIfFailed(
            string operation,
            string businessKey,
            object request,
            ApiResponse response,
            Exception exception)
        {
            if (_compensationStore == null)
            {
                return;
            }

            if (exception == null && response != null && response.IsSuccess)
            {
                return;
            }

            _compensationStore.SavePending(
                operation,
                businessKey,
                request,
                response == null ? null : (int?)response.Code,
                response == null ? null : response.Message,
                response == null ? null : response.TraceId,
                exception);
        }

        private void LogInfo(
            string operation,
            string requestId,
            string businessNo,
            string requestNo,
            string traceId,
            int? code,
            long elapsedMs,
            string message)
        {
            if (_logger != null)
            {
                _logger.Info(operation, requestId, businessNo, requestNo, traceId, code, elapsedMs, message);
            }
        }

        private void LogError(
            string operation,
            string requestId,
            string businessNo,
            string requestNo,
            string traceId,
            int? code,
            long elapsedMs,
            string message)
        {
            if (_logger != null)
            {
                _logger.Error(operation, requestId, businessNo, requestNo, traceId, code, elapsedMs, message);
            }
        }
    }
}
