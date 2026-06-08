using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageBuildContext
{
    public string BuiltBy { get; init; } = string.Empty;

    public DateTime? BuildAt { get; init; }

    public string BuildScope { get; init; } = RuntimeBuildScopeCodes.Full;

    public IReadOnlyCollection<long>? PolicyVersionIds { get; init; }

    /// <summary>
    /// 是否要求候选策略版本已显式处于 PUBLISH_READY 状态。
    /// </summary>
    /// <remarks>
    /// 直接调用编译器构建候选包时保持 true，防止未发布准备版本被绕过构建。
    /// RuntimePackagePublishService 会先执行发布资格校验，再以 false 调用编译器，
    /// 这样可避免在编译成功前把版本状态提前改成 PUBLISH_READY。
    /// </remarks>
    public bool RequirePublishReadyStatus { get; init; } = true;
}
