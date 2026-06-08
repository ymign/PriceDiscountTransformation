using Pricing.RuleCenter.Application.Pricing.Builders;
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
            ? Array.Empty<Core.Aggregates.Runtime.RuntimeRule>()
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
}
