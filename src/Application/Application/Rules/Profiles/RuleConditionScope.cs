namespace Pricing.RuleCenter.Application.Rules.Profiles;

/// <summary>
/// 规则条件在冲突检测中的收费场景和部位作用域。
/// </summary>
internal sealed record RuleConditionScope(
    HashSet<string> ChargeScenes,
    HashSet<string> BodyParts)
{
    /// <summary>
    /// 表示未配置收费场景或部位约束的通配作用域。
    /// </summary>
    public static RuleConditionScope Wildcard { get; } = new(
        new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        new HashSet<string>(StringComparer.OrdinalIgnoreCase));
}
