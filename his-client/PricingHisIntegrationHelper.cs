using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    public sealed class PricingHisIntegrationHelper
    {
        private readonly PricingApiClient _client;

        public PricingHisIntegrationHelper(PricingApiClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
        }

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

        public PricingPopupResult ShowPricingPopup(
            IWin32Window owner,
            PricingCalculateRequest request)
        {
            FrmPricingPopup popup = new FrmPricingPopup(_client, request);
            DialogResult result = popup.ShowDialog(owner);
            if (result != DialogResult.OK)
            {
                return PricingPopupResult.Cancelled();
            }

            return PricingPopupResult.FromConfirmed(popup.ConfirmedResponse, popup.ConfirmedRequestId);
        }

        public ApiResponse CommitAfterHisSuccess(long requestId, string chargeNo)
        {
            return _client.Commit(new PricingCommitRequest
            {
                RequestId = requestId,
                ChargeNo = chargeNo
            });
        }

        public ApiResponse CancelAfterHisFailure(long requestId)
        {
            return _client.Cancel(new PricingCancelRequest
            {
                RequestId = requestId
            });
        }

        public static string EnsureBusinessRequestNo(string existingBusinessRequestNo, string chargeNo)
        {
            if (!string.IsNullOrEmpty(existingBusinessRequestNo))
            {
                return existingBusinessRequestNo;
            }

            if (!string.IsNullOrEmpty(chargeNo))
            {
                return "HIS_CHARGE_" + chargeNo;
            }

            return "HIS_PENDING_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        }

        public static PricingCalculateRequest BuildSingleItemRequest(
            string patientId,
            string visitId,
            string chargeScene,
            string chargeNo,
            string businessRequestNo,
            string operatorId,
            string operatorName,
            string itemCode,
            string itemName,
            decimal qty,
            string unit,
            decimal unitPrice,
            string bodyPartCode)
        {
            PricingCalculateRequest request = new PricingCalculateRequest();
            request.RequestNo = "HIS_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
            request.PatientId = patientId;
            request.VisitId = visitId;
            request.ChargeScene = chargeScene;
            request.BusinessChargeTime = DateTime.Now;
            request.SourceSystem = "HIS";
            request.ChargeNo = chargeNo;
            request.BusinessRequestNo = EnsureBusinessRequestNo(businessRequestNo, chargeNo);
            request.OperatorId = operatorId;
            request.OperatorName = operatorName;
            request.Items = new List<PricingCalculateItemRequest>();
            request.Items.Add(new PricingCalculateItemRequest
            {
                ItemRequestNo = "ITEM_1",
                ItemCode = itemCode,
                ItemName = itemName,
                InputQty = qty,
                Unit = unit,
                UnitPrice = unitPrice,
                BodyPartCode = bodyPartCode
            });
            return request;
        }
    }

    public sealed class SpecialPricingDecision
    {
        public bool IsSpecial { get; private set; }
        public bool AllowOrdinaryPricing { get; private set; }
        public bool ShouldOpenPopup { get; private set; }
        public bool ServiceUnavailable { get; private set; }
        public string Message { get; private set; }

        public static SpecialPricingDecision AllowOrdinary(string message)
        {
            return new SpecialPricingDecision
            {
                IsSpecial = false,
                AllowOrdinaryPricing = true,
                ShouldOpenPopup = false,
                ServiceUnavailable = false,
                Message = message
            };
        }

        public static SpecialPricingDecision RequirePopup(string message)
        {
            return new SpecialPricingDecision
            {
                IsSpecial = true,
                AllowOrdinaryPricing = false,
                ShouldOpenPopup = true,
                ServiceUnavailable = false,
                Message = message
            };
        }

        public static SpecialPricingDecision BlockAsServiceUnavailable(string message)
        {
            return new SpecialPricingDecision
            {
                IsSpecial = true,
                AllowOrdinaryPricing = false,
                ShouldOpenPopup = false,
                ServiceUnavailable = true,
                Message = message
            };
        }
    }

    public sealed class PricingPopupResult
    {
        public bool Confirmed { get; private set; }
        public long RequestId { get; private set; }
        public PricingCalculateResponse Response { get; private set; }

        public static PricingPopupResult FromConfirmed(PricingCalculateResponse response, long requestId)
        {
            return new PricingPopupResult
            {
                Confirmed = true,
                Response = response,
                RequestId = requestId
            };
        }

        public static PricingPopupResult Cancelled()
        {
            return new PricingPopupResult
            {
                Confirmed = false
            };
        }
    }
}
