using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageTraceResolver
{
    private readonly IRuntimePackageStateRepository _runtimePackageStateRepository;
    private readonly IRuntimeRuleReadRepository _runtimeRuleReadRepository;
    private readonly RuntimePackageTraceContextAccessor _traceContextAccessor;

    public RuntimePackageTraceResolver(
        IRuntimePackageStateRepository runtimePackageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository)
        : this(
            runtimePackageStateRepository,
            runtimeRuleReadRepository,
            new RuntimePackageTraceContextAccessor())
    {
    }

    public RuntimePackageTraceResolver(
        IRuntimePackageStateRepository runtimePackageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository,
        RuntimePackageTraceContextAccessor traceContextAccessor)
    {
        _runtimePackageStateRepository = runtimePackageStateRepository;
        _runtimeRuleReadRepository = runtimeRuleReadRepository;
        _traceContextAccessor = traceContextAccessor;
    }

    public async Task<RuntimePackageTraceContext> CaptureContextAsync()
    {
        var activeState = await _runtimePackageStateRepository.GetActiveAsync();
        return RuntimePackageTraceContext.From(activeState);
    }

    public IDisposable BeginScope(RuntimePackageTraceContext context)
    {
        return _traceContextAccessor.Push(context);
    }

    public async Task<RuntimePackageTraceResolution> ResolveAsync(IReadOnlyCollection<long> runtimeRuleIds)
    {
        var currentContext = _traceContextAccessor.Current;
        var activeState = currentContext is null
            ? RuntimePackageTraceContext.From(await _runtimePackageStateRepository.GetActiveAsync())
            : currentContext;
        var normalizedRuleIds = runtimeRuleIds
            .Where(ruleId => ruleId > 0)
            .Distinct()
            .ToArray();

        var runtimeRules = normalizedRuleIds.Length == 0
            ? Array.Empty<RuntimeRule>()
            : await _runtimeRuleReadRepository.GetRulesByIdsAsync(normalizedRuleIds);

        return new RuntimePackageTraceResolution
        {
            RuntimePackageId = activeState.ActivePackageId,
            RuntimePackageVersion = activeState.ActivePackageVersion,
            RuntimeRulesById = runtimeRules.ToDictionary(rule => rule.RuntimeRuleId)
        };
    }

    internal Task<RuntimePackageTraceResolution> ResolveAsync(
        IReadOnlyList<ItemPricingCalculation> calculations)
    {
        return ResolveAsync(
            calculations.SelectMany(calculation => calculation.Result.MatchedRuleIds).ToArray());
    }

    public async Task<RuntimePackageRuleSnapshotResolution> LoadActiveRuleSnapshotsByItemCodeAsync(string itemCode)
    {
        var normalizedItemCode = itemCode.Trim();
        if (normalizedItemCode.Length == 0)
        {
            return RuntimePackageRuleSnapshotResolution.Empty(null, null);
        }

        var currentContext = _traceContextAccessor.Current;
        var activeState = currentContext is null
            ? RuntimePackageTraceContext.From(await _runtimePackageStateRepository.GetActiveAsync())
            : currentContext;

        if (!activeState.ActivePackageId.HasValue || activeState.ActivePackageId.Value <= 0)
        {
            return RuntimePackageRuleSnapshotResolution.Empty(null, null);
        }

        var rules = await _runtimeRuleReadRepository.GetRulesByItemCodeAsync(
            activeState.ActivePackageId.Value,
            normalizedItemCode);
        if (rules.Count == 0)
        {
            return RuntimePackageRuleSnapshotResolution.Empty(
                activeState.ActivePackageId,
                activeState.ActivePackageVersion);
        }

        var ruleIds = rules.Select(rule => rule.RuntimeRuleId).ToArray();
        var conditions = await _runtimeRuleReadRepository.GetConditionsByRuleIdsAsync(ruleIds);
        var actions = await _runtimeRuleReadRepository.GetActionsByRuleIdsAsync(ruleIds);
        var snapshots = rules.Select(rule => new RuntimeRuleSnapshot
        {
            Rule = rule,
            Conditions = conditions.TryGetValue(rule.RuntimeRuleId, out var ruleConditions)
                ? ruleConditions
                : Array.Empty<RuntimeCondition>(),
            Actions = actions.TryGetValue(rule.RuntimeRuleId, out var ruleActions)
                ? ruleActions
                : Array.Empty<RuntimeAction>()
        }).ToList();

        return new RuntimePackageRuleSnapshotResolution
        {
            RuntimePackageId = activeState.ActivePackageId,
            RuntimePackageVersion = activeState.ActivePackageVersion,
            Snapshots = snapshots
        };
    }
}

public sealed class RuntimePackageRuleSnapshotResolution
{
    public long? RuntimePackageId { get; init; }

    public long? RuntimePackageVersion { get; init; }

    public bool HasActiveRuntimePackage => RuntimePackageId.HasValue && RuntimePackageId.Value > 0;

    public IReadOnlyList<RuntimeRuleSnapshot> Snapshots { get; init; } =
        Array.Empty<RuntimeRuleSnapshot>();

    public static RuntimePackageRuleSnapshotResolution Empty(
        long? runtimePackageId,
        long? runtimePackageVersion)
    {
        return new RuntimePackageRuleSnapshotResolution
        {
            RuntimePackageId = runtimePackageId,
            RuntimePackageVersion = runtimePackageVersion
        };
    }
}
