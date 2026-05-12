namespace HIS.Pricing.Client
{
    /// <summary>
    /// 特殊计价决策结果。
    /// </summary>
    public sealed class SpecialPricingDecision
    {
        /// <summary>是否为特殊计价项目。</summary>
        public bool IsSpecial { get; private set; }

        /// <summary>是否允许按普通计价流程继续收费。</summary>
        public bool AllowOrdinaryPricing { get; private set; }

        /// <summary>是否需要打开特殊计价确认弹窗。</summary>
        public bool ShouldOpenPopup { get; private set; }

        /// <summary>
        /// 是否必须阻断收费。字段名保留为 ServiceUnavailable，是为了兼容旧接入代码；
        /// 产品语义上也用于表达本地入参不合法等必须阻断的情况。
        /// </summary>
        public bool ServiceUnavailable { get; private set; }

        /// <summary>决策说明信息，可展示给收费人员或记录到日志。</summary>
        public string Message { get; private set; }

        /// <summary>
        /// 创建“允许普通计价”决策。用于非特殊项目的正常放行。
        /// </summary>
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

        /// <summary>
        /// 创建“需要弹窗”决策。用于特殊项目的收费流程。
        /// </summary>
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

        /// <summary>
        /// 创建“阻断收费”决策。用于计价服务不可用或本地入参不合法等不能继续收费的场景。
        /// </summary>
        public static SpecialPricingDecision Blocked(string message)
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

        /// <summary>
        /// 创建“服务不可用阻断”决策。保留旧命名，内部统一走阻断语义。
        /// </summary>
        public static SpecialPricingDecision BlockAsServiceUnavailable(string message)
        {
            return Blocked(message);
        }
    }
}
