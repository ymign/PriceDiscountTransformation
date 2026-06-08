using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageTraceResolver
{
    private readonly IRuntimePackageStateRepository _runtimePackageStateRepository;
    private readonly IRuntimeRuleReadRepository _runtimeRuleReadRepository;

    public RuntimePackageTraceResolver(
        IRuntimePackageStateRepository runtimePackageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository)
    {
        _runtimePackageStateRepository = runtimePackageStateRepository;
        _runtimeRuleReadRepository = runtimeRuleReadRepository;
    }

    public async Task<RuntimePackageTraceResolution> ResolveAsync(IReadOnlyCollection<long> runtimeRuleIds)
    {
        var activeState = await _runtimePackageStateRepository.GetActiveAsync();
        var normalizedRuleIds = runtimeRuleIds
            .Where(ruleId => ruleId > 0)
            .Distinct()
            .ToArray();

        var runtimeRules = normalizedRuleIds.Length == 0
            ? Array.Empty<Core.Aggregates.Runtime.RuntimeRule>()
            : await _runtimeRuleReadRepository.GetRulesByIdsAsync(normalizedRuleIds);

        return new RuntimePackageTraceResolution
        {
            RuntimePackageId = activeState?.ActivePackageId,
            RuntimePackageVersion = activeState?.ActivePackageVersion,
            RuntimeRulesById = runtimeRules.ToDictionary(rule => rule.RuntimeRuleId)
        };
    }

    internal Task<RuntimePackageTraceResolution> ResolveAsync(
        IReadOnlyList<ItemPricingCalculation> calculations)
    {
        return ResolveAsync(
            calculations.SelectMany(calculation => calculation.Result.MatchedRuleIds).ToArray());
    }
}
