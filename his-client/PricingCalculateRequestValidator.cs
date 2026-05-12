using System;
using System.Collections.Generic;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// SDK 本地请求校验器。
    /// </summary>
    public static class PricingCalculateRequestValidator
    {
        /// <summary>
        /// 校验 simulate/confirm 共享的基础计价请求字段。
        /// </summary>
        public static void ValidateForCalculate(PricingCalculateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (IsBlank(request.SourceSystem))
            {
                throw new InvalidOperationException("PricingCalculateRequest.SourceSystem 不能为空。");
            }

            if (IsBlank(request.PatientId))
            {
                throw new InvalidOperationException("PricingCalculateRequest.PatientId 不能为空。");
            }

            if (request.BusinessChargeTime == default(DateTime))
            {
                throw new InvalidOperationException("PricingCalculateRequest.BusinessChargeTime 不能为空。");
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                throw new InvalidOperationException("PricingCalculateRequest.Items 不能为空。");
            }

            for (int i = 0; i < request.Items.Count; i++)
            {
                ValidateCalculateItem(request.Items[i], i + 1);
            }
        }

        /// <summary>
        /// 校验 confirm 请求必须具备稳定业务号。
        /// </summary>
        public static void ValidateForConfirm(PricingCalculateRequest request)
        {
            ValidateForCalculate(request);

            if (IsBlank(request.BusinessRequestNo))
            {
                throw new InvalidOperationException(
                    "confirm 前必须传入稳定的 BusinessRequestNo；若 HIS 尚未生成收费单号，请先预生成一次收费确认流水。");
            }
        }

        /// <summary>
        /// 校验 commit 请求中的 HIS 真实落账明细。
        /// </summary>
        public static void ValidateCommitActuals(
            long requestId,
            IList<PricingCommitActualItemRequest> actualItems)
        {
            if (requestId <= 0)
            {
                throw new InvalidOperationException("commit 必须传入有效的 RequestId。");
            }

            if (actualItems == null || actualItems.Count == 0)
            {
                throw new ArgumentException("commit 必须回传 HIS 实际落账明细 actualItems。", "actualItems");
            }

            for (int i = 0; i < actualItems.Count; i++)
            {
                PricingCommitActualItemRequest item = actualItems[i];
                string prefix = "actualItems[" + i + "]";
                if (item == null)
                {
                    throw new InvalidOperationException(prefix + " 不能为空。");
                }

                if (IsBlank(item.ItemCode))
                {
                    throw new InvalidOperationException(prefix + ".ItemCode 不能为空。");
                }

                if (item.FinalQty < 0)
                {
                    throw new InvalidOperationException(prefix + ".FinalQty 不能小于 0。");
                }

                if (item.FinalAmount < 0)
                {
                    throw new InvalidOperationException(prefix + ".FinalAmount 不能小于 0。");
                }

                if (item.FinalQty == 0m && item.FinalAmount == 0m)
                {
                    throw new InvalidOperationException(prefix + " 不能同时为 0 数量和 0 金额。");
                }
            }
        }

        /// <summary>
        /// 校验 cancel 请求的最小字段。
        /// </summary>
        public static void ValidateCancelRequest(long requestId)
        {
            if (requestId <= 0)
            {
                throw new InvalidOperationException("cancel 必须传入有效的 RequestId。");
            }
        }

        /// <summary>
        /// 校验 reverse 请求的最小幂等字段。
        /// </summary>
        public static void ValidateReverseRequest(PricingReverseRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.OriginalRequestId <= 0)
            {
                throw new InvalidOperationException("reverse 必须传入有效的 OriginalRequestId。");
            }

            if (IsBlank(request.ReverseNo))
            {
                throw new InvalidOperationException("reverse 必须传入稳定的 ReverseNo。");
            }

            if (request.ReverseQty.HasValue && request.ReverseQty.Value <= 0)
            {
                throw new InvalidOperationException("ReverseQty 必须大于 0。");
            }

            if (request.ReverseAmt.HasValue && request.ReverseAmt.Value < 0)
            {
                throw new InvalidOperationException("ReverseAmt 不能小于 0。");
            }
        }

        private static void ValidateCalculateItem(PricingCalculateItemRequest item, int displayIndex)
        {
            string prefix = "PricingCalculateRequest.Items[" + displayIndex + "]";
            if (item == null)
            {
                throw new InvalidOperationException(prefix + " 不能为空。");
            }

            if (IsBlank(item.ItemCode))
            {
                throw new InvalidOperationException(prefix + ".ItemCode 不能为空。");
            }

            if (item.InputQty <= 0)
            {
                throw new InvalidOperationException(prefix + ".InputQty 必须大于 0。");
            }

            if (item.UnitPrice < 0)
            {
                throw new InvalidOperationException(prefix + ".UnitPrice 不能小于 0。");
            }

            if (item.PricingParts == null)
            {
                return;
            }

            for (int i = 0; i < item.PricingParts.Count; i++)
            {
                PricingPartItemRequest part = item.PricingParts[i];
                if (part == null)
                {
                    throw new InvalidOperationException(prefix + ".PricingParts[" + i + "] 不能为空。");
                }

                if (part.Qty <= 0)
                {
                    throw new InvalidOperationException(prefix + ".PricingParts[" + i + "].Qty 必须大于 0。");
                }
            }
        }

        private static bool IsBlank(string value)
        {
            return value == null || value.Trim().Length == 0;
        }
    }
}
