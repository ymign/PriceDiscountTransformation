using System;
using System.Collections.Generic;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// HIS 一次收费动作的轻量上下文。
    /// 用于产品化接入时减少 HIS 侧 DTO 组装代码；复杂批量收费仍可直接构造 PricingCalculateRequest。
    /// </summary>
    public sealed class PricingChargeContext
    {
        public string PatientId { get; set; }

        public string VisitId { get; set; }

        public string VisitType { get; set; }

        public int? PatientAge { get; set; }

        public string EncounterNo { get; set; }

        public string ChargeScene { get; set; }

        public string ChargeNo { get; set; }

        public string BusinessRequestNo { get; set; }

        public DateTime? BusinessChargeTime { get; set; }

        public string OperatorId { get; set; }

        public string OperatorName { get; set; }

        public string ChargeDeptCode { get; set; }

        public string ItemRequestNo { get; set; }

        public string ChargeDetailNo { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string ItemGroupCode { get; set; }

        public decimal InputQty { get; set; }

        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public string BodyPartCode { get; set; }

        public decimal? LegacyOccupiedQty { get; set; }

        public Dictionary<string, object> ExtraParams { get; set; }

        public List<PricingPartItemRequest> PricingParts { get; set; }

        /// <summary>
        /// 构造成计价中心请求。该方法会补齐 RequestNo、BusinessRequestNo、SourceSystem 和默认场景。
        /// </summary>
        public PricingCalculateRequest ToCalculateRequest(string sourceSystem, string defaultChargeScene)
        {
            if (string.IsNullOrEmpty(ItemCode))
            {
                throw new InvalidOperationException("PricingChargeContext.ItemCode 不能为空。");
            }

            if (InputQty <= 0)
            {
                throw new InvalidOperationException("PricingChargeContext.InputQty 必须大于 0。");
            }

            PricingCalculateRequest request = new PricingCalculateRequest();
            request.RequestNo = "HIS_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
            request.PatientId = PatientId;
            request.VisitId = VisitId;
            request.VisitType = VisitType;
            request.PatientAge = PatientAge;
            request.EncounterNo = EncounterNo;
            request.ChargeScene = !string.IsNullOrEmpty(ChargeScene) ? ChargeScene : defaultChargeScene;
            request.BusinessChargeTime = BusinessChargeTime.HasValue ? BusinessChargeTime.Value : DateTime.Now;
            request.SourceSystem = !string.IsNullOrEmpty(sourceSystem) ? sourceSystem : "HIS";
            request.ChargeNo = ChargeNo;
            if (!string.IsNullOrEmpty(BusinessRequestNo) || !string.IsNullOrEmpty(ChargeNo))
            {
                request.BusinessRequestNo = PricingHisIntegrationHelper.EnsureBusinessRequestNo(BusinessRequestNo, ChargeNo);
            }
            request.OperatorId = OperatorId;
            request.OperatorName = OperatorName;
            request.ChargeDeptCode = ChargeDeptCode;
            request.Items = new List<PricingCalculateItemRequest>();
            request.Items.Add(new PricingCalculateItemRequest
            {
                ItemRequestNo = !string.IsNullOrEmpty(ItemRequestNo) ? ItemRequestNo : "ITEM_1",
                ChargeDetailNo = ChargeDetailNo,
                ItemCode = ItemCode,
                ItemName = ItemName,
                ItemGroupCode = ItemGroupCode,
                InputQty = InputQty,
                Unit = Unit,
                UnitPrice = UnitPrice,
                BodyPartCode = BodyPartCode,
                BusinessChargeTime = BusinessChargeTime,
                LegacyOccupiedQty = LegacyOccupiedQty,
                ExtraParams = ExtraParams,
                PricingParts = PricingParts
            });

            return request;
        }
    }
}
