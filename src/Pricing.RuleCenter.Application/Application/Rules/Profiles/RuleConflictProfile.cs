namespace Pricing.RuleCenter.Application.Rules.Profiles;

/// <summary>
/// 规则发布冲突检测使用的简化规则画像。
/// </summary>
internal sealed record RuleConflictProfile(
    IReadOnlyList<RuleConditionScope> ConditionScopes,
    HashSet<string> Actions);
