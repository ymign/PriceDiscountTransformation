namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 对外接口下一步动作编码。
/// </summary>
/// <remarks>
/// 该编码给 HIS、自助机、微信等调用方做程序判断，展示文案仍由调用方自行决定。
/// </remarks>
public static class PricingNextActionCodes
{
    /// <summary>可按普通价格流程继续收费。</summary>
    public const string NormalPricing = "NORMAL_PRICING";

    /// <summary>必须调用统一计价试算或确认接口。</summary>
    public const string CallSimulate = "CALL_SIMULATE";

    /// <summary>暂停收费，转人工或稍后重试。</summary>
    public const string StopCharge = "STOP_CHARGE";

    /// <summary>转人工复核。</summary>
    public const string ManualReview = "MANUAL_REVIEW";

    /// <summary>试算后下一步应调用 confirm 锁定计价结果。</summary>
    public const string ConfirmBeforeCharge = "CONFIRM_BEFORE_CHARGE";

    /// <summary>确认后下一步应调用 commit 或 cancel 收尾。</summary>
    public const string CommitOrCancel = "COMMIT_OR_CANCEL";

    /// <summary>本接口生命周期动作已经完成，无需继续调用计价中心。</summary>
    public const string NoFurtherAction = "NO_FURTHER_ACTION";
}
