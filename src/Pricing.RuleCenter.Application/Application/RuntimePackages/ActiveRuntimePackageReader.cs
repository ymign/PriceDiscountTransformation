using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class ActiveRuntimePackageReader
{
    private readonly IRuntimePackageStateRepository _packageStateRepository;
    private readonly IRuntimeRuleReadRepository _runtimeRuleReadRepository;
    private readonly RuntimePackageTraceContextAccessor? _traceContextAccessor;

    public ActiveRuntimePackageReader(
        IRuntimePackageStateRepository packageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository,
        RuntimePackageTraceContextAccessor? traceContextAccessor = null)
    {
        _packageStateRepository = packageStateRepository;
        _runtimeRuleReadRepository = runtimeRuleReadRepository;
        _traceContextAccessor = traceContextAccessor;
    }

    public async Task<IReadOnlyList<RuntimeRuleSnapshot>> LoadByItemCodeAsync(string itemCode)
    {
        var normalizedItemCode = itemCode.Trim();
        if (normalizedItemCode.Length == 0)
        {
            return Array.Empty<RuntimeRuleSnapshot>();
        }

        var activeContext = _traceContextAccessor?.Current;
        var packageId = activeContext?.ActivePackageId;
        if (activeContext is null)
        {
            var activeState = await _packageStateRepository.GetActiveAsync();
            packageId = activeState?.ActivePackageId > 0 ? activeState.ActivePackageId : null;
        }

        if (!packageId.HasValue || packageId.Value <= 0)
        {
            return Array.Empty<RuntimeRuleSnapshot>();
        }

        var rules = await _runtimeRuleReadRepository.GetRulesByItemCodeAsync(packageId.Value, normalizedItemCode);
        if (rules.Count == 0)
        {
            return Array.Empty<RuntimeRuleSnapshot>();
        }

        var ruleIds = rules.Select(rule => rule.RuntimeRuleId).ToArray();
        var conditions = await _runtimeRuleReadRepository.GetConditionsByRuleIdsAsync(ruleIds);
        var actions = await _runtimeRuleReadRepository.GetActionsByRuleIdsAsync(ruleIds);
        var snapshots = new List<RuntimeRuleSnapshot>(rules.Count);

        foreach (var rule in rules)
        {
            snapshots.Add(new RuntimeRuleSnapshot
            {
                Rule = rule,
                Conditions = conditions.TryGetValue(rule.RuntimeRuleId, out var ruleConditions)
                    ? ruleConditions
                    : Array.Empty<RuntimeCondition>(),
                Actions = actions.TryGetValue(rule.RuntimeRuleId, out var ruleActions)
                    ? ruleActions
                    : Array.Empty<RuntimeAction>()
            });
        }

        return snapshots;
    }
}
