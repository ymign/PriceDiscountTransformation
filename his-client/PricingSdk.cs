using System;
using System.Collections.Generic;

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
            _client = new PricingApiClient(
                options.GetNormalizedBaseUrl(),
                options.TimeoutMs,
                options.MaxRetry,
                options.RetryDelayMs);
            _sourceSystem = !string.IsNullOrEmpty(options.SourceSystem) ? options.SourceSystem : "HIS";
            _defaultChargeScene = !string.IsNullOrEmpty(options.DefaultChargeScene) ? options.DefaultChargeScene : "OUTPATIENT";
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
            _sourceSystem = options != null && !string.IsNullOrEmpty(options.SourceSystem) ? options.SourceSystem : "HIS";
            _defaultChargeScene = options != null && !string.IsNullOrEmpty(options.DefaultChargeScene) ? options.DefaultChargeScene : "OUTPATIENT";
        }

        /// <summary>底层 HTTP 客户端，供高级集成场景直接访问。</summary>
        public PricingApiClient Client
        {
            get { return _client; }
        }

        /// <summary>
        /// 检查项目是否需要进入特殊计价流程。
        /// 计价服务不可用时返回阻断决策，不能按普通计价放行。
        /// </summary>
        public SpecialPricingDecision CheckSpecialPricingRequired(string itemCode)
        {
            if (string.IsNullOrEmpty(itemCode))
            {
                return SpecialPricingDecision.AllowOrdinary("项目编码为空，按原流程处理。");
            }

            try
            {
                ApiResponse<SpecialFlagResponse> response = _client.GetSpecialFlag(itemCode);
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
            return _client.Simulate(request);
        }

        /// <summary>
        /// 收费保存前确认计价，占用额度。
        /// HIS 本地落账成功后必须调用 CommitAfterHisSuccess；落账失败才调用 CancelAfterHisFailure。
        /// </summary>
        public ApiResponse<PricingCalculateResponse> ConfirmBeforeCharge(PricingCalculateRequest request)
        {
            ValidateConfirmRequest(request);
            return _client.Confirm(request);
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
            if (actualItems == null || actualItems.Count == 0)
            {
                throw new ArgumentException("commit 必须回传 HIS 实际落账明细 actualItems。", "actualItems");
            }

            PricingCommitRequest request = new PricingCommitRequest();
            request.RequestId = requestId;
            request.ChargeNo = chargeNo;
            request.ActualItems = actualItems;
            request.ActualTotalAmount = actualTotalAmount;
            return _client.Commit(request);
        }

        /// <summary>
        /// HIS 落账失败后取消 confirm 占用。
        /// 注意：HIS 已真实落账成功但 commit 通知失败时不能调用本方法。
        /// </summary>
        public ApiResponse CancelAfterHisFailure(long requestId)
        {
            PricingCancelRequest request = new PricingCancelRequest();
            request.RequestId = requestId;
            return _client.Cancel(request);
        }

        /// <summary>
        /// HIS 退费或冲正后通知计价中心释放额度。
        /// </summary>
        public ApiResponse ReverseAfterHisRefund(PricingReverseRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return _client.Reverse(request);
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
                request.BusinessRequestNo = PricingHisIntegrationHelper.EnsureBusinessRequestNo(null, request.ChargeNo);
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                throw new InvalidOperationException("PricingCalculateRequest.Items 不能为空。");
            }
        }

        /// <summary>
        /// 校验 confirm 请求是否满足幂等要求。
        /// confirm 前必须存在稳定业务号，不能由 SDK 在无 ChargeNo 时随机生成。
        /// </summary>
        public void ValidateConfirmRequest(PricingCalculateRequest request)
        {
            PrepareCalculateRequest(request);

            if (string.IsNullOrEmpty(request.BusinessRequestNo))
            {
                throw new InvalidOperationException(
                    "confirm 前必须传入稳定的 BusinessRequestNo；若 HIS 尚未生成收费单号，请先预生成一次收费确认流水。");
            }
        }
    }
}
