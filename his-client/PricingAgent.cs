using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 统一计价 Agent 产品入口。
    /// 形态类似合理用药：HIS 调用一个 DLL 接口，Agent 自己判断特殊项目、弹出自己的窗口、返回处理结果。
    /// </summary>
    public sealed class PricingAgent
    {
        private readonly PricingSdk _sdk;

        /// <summary>
        /// 使用产品配置创建 Agent。
        /// </summary>
        public PricingAgent(PricingSdkOptions options)
        {
            _sdk = new PricingSdk(options);
        }

        /// <summary>
        /// 使用已有 SDK 创建 Agent。
        /// </summary>
        public PricingAgent(PricingSdk sdk)
        {
            if (sdk == null)
            {
                throw new ArgumentNullException("sdk");
            }

            _sdk = sdk;
        }

        /// <summary>
        /// SDK 实例，供调用方执行 commit、cancel、reverse 等无界面操作。
        /// </summary>
        public PricingSdk Sdk
        {
            get { return _sdk; }
        }

        /// <summary>
        /// 收费保存前调用。Agent 会先查特殊标识；非特殊项目直接放行，特殊项目弹出确认窗口。
        /// </summary>
        public PricingAgentChargeResult ConfirmBeforeCharge(
            IWin32Window owner,
            PricingCalculateRequest request)
        {
            try
            {
                _sdk.PrepareCalculateRequest(request);
                SpecialPricingDecision decision = CheckRequestDecision(request);
                if (decision.ServiceUnavailable)
                {
                    return PricingAgentChargeResult.CreateBlocked(decision.Message);
                }

                if (decision.AllowOrdinaryPricing)
                {
                    return PricingAgentChargeResult.Ordinary(decision.Message);
                }

                _sdk.ValidateConfirmRequest(request);
                FrmPricingPopup popup = new FrmPricingPopup(_sdk.Client, request);
                DialogResult result = owner == null ? popup.ShowDialog() : popup.ShowDialog(owner);
                if (result != DialogResult.OK)
                {
                    return PricingAgentChargeResult.Cancelled("操作员取消特殊计价确认。");
                }

                return PricingAgentChargeResult.CreateConfirmed(
                    popup.ConfirmedResponse,
                    popup.ConfirmedRequestId,
                    "特殊计价已确认，HIS 应按返回结果落账。");
            }
            catch (Exception ex)
            {
                return PricingAgentChargeResult.CreateBlocked("特殊计价 Agent 执行失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 使用轻量上下文执行收费前确认。
        /// </summary>
        public PricingAgentChargeResult ConfirmBeforeCharge(
            IWin32Window owner,
            PricingChargeContext context)
        {
            PricingCalculateRequest request = _sdk.BuildChargeRequest(context);
            return ConfirmBeforeCharge(owner, request);
        }

        /// <summary>
        /// 打开规则维护工作台。适合 HIS 菜单或物价管理入口调用。
        /// </summary>
        public void OpenRuleWorkbench(IWin32Window owner, string operatorId)
        {
            FrmPricingRuleWorkbench workbench = new FrmPricingRuleWorkbench(_sdk.Client, operatorId);
            if (owner == null)
            {
                workbench.Show();
            }
            else
            {
                workbench.Show(owner);
            }
        }

        /// <summary>
        /// 打开 PricingAgent 诊断窗口，用于现场查看版本、日志目录、补偿记录和服务健康状态。
        /// </summary>
        public void OpenDiagnostics(IWin32Window owner)
        {
            FrmPricingAgentDiagnostics diagnostics = new FrmPricingAgentDiagnostics(_sdk);
            if (owner == null)
            {
                diagnostics.Show();
            }
            else
            {
                diagnostics.Show(owner);
            }
        }

        /// <summary>
        /// HIS 落账成功后提交计价中心状态。
        /// </summary>
        public ApiResponse CommitAfterHisSuccess(
            long requestId,
            string chargeNo,
            List<PricingCommitActualItemRequest> actualItems,
            decimal? actualTotalAmount)
        {
            return _sdk.CommitAfterHisSuccess(requestId, chargeNo, actualItems, actualTotalAmount);
        }

        /// <summary>
        /// HIS 落账失败后取消 confirm 占用。
        /// </summary>
        public ApiResponse CancelAfterHisFailure(long requestId)
        {
            return _sdk.CancelAfterHisFailure(requestId);
        }

        /// <summary>
        /// HIS 退费或冲正后释放额度。
        /// </summary>
        public ApiResponse ReverseAfterHisRefund(PricingReverseRequest request)
        {
            return _sdk.ReverseAfterHisRefund(request);
        }

        private SpecialPricingDecision CheckRequestDecision(PricingCalculateRequest request)
        {
            bool hasSpecial = false;
            string specialMessage = null;

            for (int i = 0; i < request.Items.Count; i++)
            {
                PricingCalculateItemRequest item = request.Items[i];
                SpecialPricingDecision itemDecision = _sdk.CheckSpecialPricingRequired(item.ItemCode);
                if (itemDecision.ServiceUnavailable)
                {
                    return itemDecision;
                }

                if (itemDecision.ShouldOpenPopup)
                {
                    hasSpecial = true;
                    specialMessage = itemDecision.Message;
                }
            }

            if (hasSpecial)
            {
                return SpecialPricingDecision.RequirePopup(specialMessage);
            }

            return SpecialPricingDecision.AllowOrdinary("本次收费未命中特殊计价项目。");
        }
    }

    /// <summary>
    /// Agent 收费前确认结果。HIS 只需要根据该对象决定是否继续本地落账。
    /// </summary>
    public sealed class PricingAgentChargeResult
    {
        /// <summary>
        /// Agent 内部状态码。用于 HIS 日志或现场排查，不建议作为金额判断依据。
        /// </summary>
        public string StatusCode { get; private set; }

        /// <summary>
        /// 是否允许 HIS 继续本地落账。该字段是调用方最重要的放行开关。
        /// </summary>
        public bool AllowCharge { get; private set; }

        /// <summary>
        /// 是否未命中特殊计价，允许 HIS 按普通价格继续处理。
        /// </summary>
        public bool OrdinaryPricing { get; private set; }

        /// <summary>
        /// 是否已经完成特殊计价 confirm。为 true 时 HIS 必须按 Response 回填后的结果落账。
        /// </summary>
        public bool Confirmed { get; private set; }

        /// <summary>
        /// 是否由操作员主动取消弹窗。该状态不应自动改为普通计价。
        /// </summary>
        public bool CancelledByUser { get; private set; }

        /// <summary>
        /// 是否因配置、服务、校验或执行异常被阻断。阻断时 HIS 不应继续收费。
        /// </summary>
        public bool Blocked { get; private set; }

        /// <summary>
        /// confirm 成功后由计价中心返回的请求 ID，后续 commit、cancel、reverse 都依赖该值。
        /// </summary>
        public long RequestId { get; private set; }

        /// <summary>
        /// confirm 成功后的计价结果，包含主项目、替换子项、加收子项和追踪步骤。
        /// </summary>
        public PricingCalculateResponse Response { get; private set; }

        /// <summary>
        /// 面向 HIS 操作日志或错误提示的人类可读说明。
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 创建普通计价结果。该结果表示没有特殊规则需要弹窗，HIS 可以继续原收费流程。
        /// </summary>
        public static PricingAgentChargeResult Ordinary(string message)
        {
            return new PricingAgentChargeResult
            {
                StatusCode = "ORDINARY",
                AllowCharge = true,
                OrdinaryPricing = true,
                Confirmed = false,
                CancelledByUser = false,
                Blocked = false,
                Message = message
            };
        }

        /// <summary>
        /// 创建特殊计价已确认结果。工厂方法避免与 Confirmed 属性同名，兼容 C# 成员命名规则。
        /// </summary>
        public static PricingAgentChargeResult CreateConfirmed(
            PricingCalculateResponse response,
            long requestId,
            string message)
        {
            return new PricingAgentChargeResult
            {
                StatusCode = "CONFIRMED",
                AllowCharge = true,
                OrdinaryPricing = false,
                Confirmed = true,
                CancelledByUser = false,
                Blocked = false,
                Response = response,
                RequestId = requestId,
                Message = message
            };
        }

        /// <summary>
        /// 创建操作员取消结果。取消只释放本次交互，不代表项目可以按普通计价继续。
        /// </summary>
        public static PricingAgentChargeResult Cancelled(string message)
        {
            return new PricingAgentChargeResult
            {
                StatusCode = "CANCELLED",
                AllowCharge = false,
                OrdinaryPricing = false,
                Confirmed = false,
                CancelledByUser = true,
                Blocked = false,
                Message = message
            };
        }

        /// <summary>
        /// 创建阻断结果。工厂方法避免与 Blocked 属性同名，防止旧框架编译失败。
        /// </summary>
        public static PricingAgentChargeResult CreateBlocked(string message)
        {
            return new PricingAgentChargeResult
            {
                StatusCode = "BLOCKED",
                AllowCharge = false,
                OrdinaryPricing = false,
                Confirmed = false,
                CancelledByUser = false,
                Blocked = true,
                Message = message
            };
        }
    }
}
