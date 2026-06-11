using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 运行时包追溯解析器，负责在一次调用链内固定活动运行包视图。
/// </summary>
public sealed class RuntimePackageTraceResolver
{
    private readonly IRuntimePackageStateRepository _runtimePackageStateRepository;
    private readonly IRuntimeRuleReadRepository _runtimeRuleReadRepository;
    private readonly RuntimePackageTraceContextAccessor _traceContextAccessor;
    private readonly ActiveRuntimePackageReader _activeRuntimePackageReader;

    /// <summary>
    /// 初始化运行时包追溯解析器。
    /// </summary>
    /// <param name="runtimePackageStateRepository">运行时包状态仓储。</param>
    /// <param name="runtimeRuleReadRepository">运行时规则读取仓储。</param>
    public RuntimePackageTraceResolver(
        IRuntimePackageStateRepository runtimePackageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository)
        : this(
            runtimePackageStateRepository,
            runtimeRuleReadRepository,
            new RuntimePackageTraceContextAccessor())
    {
    }

    /// <summary>
    /// 初始化运行时包追溯解析器。
    /// </summary>
    /// <param name="runtimePackageStateRepository">运行时包状态仓储。</param>
    /// <param name="runtimeRuleReadRepository">运行时规则读取仓储。</param>
    /// <param name="traceContextAccessor">请求级运行时包上下文访问器。</param>
    public RuntimePackageTraceResolver(
        IRuntimePackageStateRepository runtimePackageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository,
        RuntimePackageTraceContextAccessor traceContextAccessor)
    {
        _runtimePackageStateRepository = runtimePackageStateRepository;
        _runtimeRuleReadRepository = runtimeRuleReadRepository;
        _traceContextAccessor = traceContextAccessor;
        _activeRuntimePackageReader = new ActiveRuntimePackageReader(
            runtimePackageStateRepository,
            runtimeRuleReadRepository,
            traceContextAccessor);
    }

    /// <summary>
    /// 捕获当前活动运行时包上下文。
    /// </summary>
    public async Task<RuntimePackageTraceContext> CaptureContextAsync()
    {
        var activeState = await _runtimePackageStateRepository.GetActiveAsync();
        return RuntimePackageTraceContext.From(activeState);
    }

    /// <summary>
    /// 在当前异步调用链内压入运行时包上下文。
    /// </summary>
    /// <param name="context">待压入的运行时包上下文。</param>
    public IDisposable BeginScope(RuntimePackageTraceContext context)
    {
        return _traceContextAccessor.Push(context);
    }

    /// <summary>
    /// 根据运行时规则主键集合解析当前请求使用的运行时包元数据。
    /// </summary>
    /// <param name="runtimeRuleIds">运行时规则主键集合。</param>
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

    /// <summary>
    /// 读取当前活动运行时包下指定项目的规则快照。
    /// </summary>
    /// <param name="itemCode">收费项目编码。</param>
    internal async Task<RuntimePackageRuleSnapshotResolution> LoadActiveRuleSnapshotsByItemCodeAsync(string itemCode)
    {
        var runtimeReadResult = await _activeRuntimePackageReader.LoadCurrentPackageAsync(itemCode);

        return new RuntimePackageRuleSnapshotResolution
        {
            RuntimePackageId = runtimeReadResult.RuntimePackageId,
            RuntimePackageVersion = runtimeReadResult.RuntimePackageVersion,
            Snapshots = runtimeReadResult.Snapshots
        };
    }
}

/// <summary>
/// 当前活动运行时包下按项目读取规则快照后的结果。
/// </summary>
internal sealed class RuntimePackageRuleSnapshotResolution
{
    /// <summary>
    /// 当前请求命中的运行包主键。
    /// </summary>
    public long? RuntimePackageId { get; init; }

    /// <summary>
    /// 当前请求命中的运行包版本号。
    /// </summary>
    public long? RuntimePackageVersion { get; init; }

    /// <summary>
    /// 是否存在可见的活动运行包。
    /// </summary>
    public bool HasActiveRuntimePackage => RuntimePackageId.HasValue && RuntimePackageId.Value > 0;

    /// <summary>
    /// 当前项目在活动运行包下可见的规则快照集合。
    /// </summary>
    public IReadOnlyList<RuntimeRuleSnapshot> Snapshots { get; init; } =
        Array.Empty<RuntimeRuleSnapshot>();

    /// <summary>
    /// 创建空的规则快照读取结果。
    /// </summary>
    /// <param name="runtimePackageId">运行包主键。</param>
    /// <param name="runtimePackageVersion">运行包版本号。</param>
    /// <returns>不含规则快照的读取结果。</returns>
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
