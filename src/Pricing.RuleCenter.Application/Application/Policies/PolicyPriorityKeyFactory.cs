using Pricing.RuleCenter.Core.Aggregates.Policies;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略运行时优先级键生成器。
/// </summary>
internal sealed class PolicyPriorityKeyFactory : IPolicyPriorityKeyFactory
{
    /// <summary>
    /// 根据版本、绑定和作用域计算稳定优先级键。
    /// </summary>
    public string Build(PolicyVersion version, PolicyBinding binding, IReadOnlyList<PolicyScope> scopes)
    {
        var bindingRank = GetBindingRank(binding);
        var scopeOwnerRank = GetScopeOwnerRank(version.ScopeLevel);
        var specificityScore = scopes.Count(HasValue);
        var specificitySegment = Math.Max(0, 999 - specificityScore);
        var dimensionRank = scopes
            .Where(HasValue)
            .Select(scope => GetDimensionRank(scope.ScopeDimension))
            .DefaultIfEmpty(999)
            .Min();
        var weight = Math.Clamp(version.PriorityWeight, 0, 9999);
        var versionSegment = Math.Max(0, 999999 - Math.Clamp(version.VersionNo, 0, 999999));
        var policyVersionIdSegment = version.PolicyVersionId <= 0
            ? 999999999999L
            : Math.Min(version.PolicyVersionId, 999999999999L);

        return $"{bindingRank:D3}|{scopeOwnerRank:D3}|{specificitySegment:D3}|{dimensionRank:D3}|{weight:D4}|{versionSegment:D6}|{policyVersionIdSegment:D12}";
    }

    private static int GetBindingRank(PolicyBinding binding)
    {
        if (!string.IsNullOrWhiteSpace(binding.ItemCode))
        {
            return 1;
        }

        if (!string.IsNullOrWhiteSpace(binding.GroupCode))
        {
            return 2;
        }

        return 9;
    }

    private static int GetScopeOwnerRank(string? scopeLevel) =>
        scopeLevel?.Trim().ToUpperInvariant() switch
        {
            "DEPT" => 1,
            "SCENE" => 2,
            "HOSPITAL" => 3,
            "PLATFORM" => 4,
            _ => 9
        };

    private static bool HasValue(PolicyScope scope) =>
        !string.IsNullOrWhiteSpace(scope.ScopeValueText) ||
        scope.ScopeValueNumber.HasValue ||
        scope.ScopeValueDate.HasValue ||
        !string.IsNullOrWhiteSpace(scope.ScopeJson);

    private static int GetDimensionRank(string? scopeDimension) =>
        scopeDimension?.Trim().ToUpperInvariant() switch
        {
            "BODY_PART" => 1,
            "VISIT_TYPE" => 2,
            "TIME_RANGE" => 3,
            "DEPT" => 4,
            "SCENE" => 5,
            _ => 999
        };
}
