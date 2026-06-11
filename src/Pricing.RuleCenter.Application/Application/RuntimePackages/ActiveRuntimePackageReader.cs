using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 激活运行包读取器，按当前运行包状态读取运行期规则快照。
/// </summary>
/// <remarks>
/// <para>
/// 该类只读取已经编译并激活的运行期规则，不访问策略草稿、审批数据或旧规则表。
/// 计价请求通过它获得稳定的运行包视图，从而保证一次请求内规则版本可追溯、可回滚。
/// </para>
/// </remarks>
internal sealed class ActiveRuntimePackageReader
{
    /// <summary>
    /// 运行包状态仓储，用于读取当前激活包 ID。
    /// </summary>
    private readonly IRuntimePackageStateRepository _packageStateRepository;

    /// <summary>
    /// 运行期规则读取仓储，用于读取运行包内的规则、条件和动作。
    /// </summary>
    private readonly IRuntimeRuleReadRepository _runtimeRuleReadRepository;

    /// <summary>
    /// 当前请求的运行包追踪上下文。存在时优先使用上下文中的包 ID，保证一单内不受发布瞬间影响。
    /// </summary>
    private readonly RuntimePackageTraceContextAccessor? _traceContextAccessor;

    /// <summary>
    /// 初始化激活运行包读取器。
    /// </summary>
    /// <param name="packageStateRepository">运行包状态仓储。</param>
    /// <param name="runtimeRuleReadRepository">运行期规则读取仓储。</param>
    /// <param name="traceContextAccessor">运行包追踪上下文访问器。</param>
    public ActiveRuntimePackageReader(
        IRuntimePackageStateRepository packageStateRepository,
        IRuntimeRuleReadRepository runtimeRuleReadRepository,
        RuntimePackageTraceContextAccessor? traceContextAccessor = null)
    {
        _packageStateRepository = packageStateRepository;
        _runtimeRuleReadRepository = runtimeRuleReadRepository;
        _traceContextAccessor = traceContextAccessor;
    }

    /// <summary>
    /// 按项目编码读取当前激活运行包内的运行期规则快照。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>运行期规则快照集合；无激活包或无匹配项目时返回空集合。</returns>
    public async Task<IReadOnlyList<RuntimeRuleSnapshot>> LoadByItemCodeAsync(string itemCode)
    {
        return (await LoadCurrentPackageAsync(itemCode)).Snapshots;
    }

    /// <summary>
    /// 读取当前激活运行包下指定项目的运行期规则及包元数据。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>当前请求可见的运行包及规则快照读取结果。</returns>
    public async Task<ActiveRuntimePackageReadResult> LoadCurrentPackageAsync(string itemCode)
    {
        var normalizedItemCode = itemCode.Trim();
        if (normalizedItemCode.Length == 0)
        {
            return ActiveRuntimePackageReadResult.Empty(null, null);
        }

        var activeContext = _traceContextAccessor?.Current;
        var packageId = activeContext?.ActivePackageId;
        var packageVersion = activeContext?.ActivePackageVersion;
        if (activeContext is null)
        {
            // 没有请求级追踪上下文时，直接读取数据库中的当前激活包。
            // 这种路径主要用于单次查询或旧调用方式；计价工作流会提前捕获上下文。
            var activeState = await _packageStateRepository.GetActiveAsync();
            packageId = activeState?.ActivePackageId > 0 ? activeState.ActivePackageId : null;
            packageVersion = activeState?.ActivePackageVersion > 0 ? activeState.ActivePackageVersion : null;
        }

        if (!packageId.HasValue || packageId.Value <= 0)
        {
            return ActiveRuntimePackageReadResult.Empty(null, null);
        }

        // 先按项目读取规则，再按规则 ID 批量读取条件和动作，减少数据库往返次数。
        var rules = await _runtimeRuleReadRepository.GetRulesByItemCodeAsync(packageId.Value, normalizedItemCode);
        if (rules.Count == 0)
        {
            return ActiveRuntimePackageReadResult.Empty(packageId, packageVersion);
        }

        var ruleIds = rules.Select(rule => rule.RuntimeRuleId).ToArray();
        var conditions = await _runtimeRuleReadRepository.GetConditionsByRuleIdsAsync(ruleIds);
        var actions = await _runtimeRuleReadRepository.GetActionsByRuleIdsAsync(ruleIds);
        var snapshots = new List<RuntimeRuleSnapshot>(rules.Count);

        foreach (var rule in rules)
        {
            // 条件和动作不存在时使用空集合，后续匹配/执行管线按“无条件/无动作”的业务规则处理。
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

        return new ActiveRuntimePackageReadResult
        {
            RuntimePackageId = packageId,
            RuntimePackageVersion = packageVersion,
            Snapshots = snapshots
        };
    }
}

/// <summary>
/// 当前激活运行包读取结果。
/// </summary>
internal sealed class ActiveRuntimePackageReadResult
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
    /// 当前请求可见的运行时规则快照集合。
    /// </summary>
    public IReadOnlyList<RuntimeRuleSnapshot> Snapshots { get; init; } = Array.Empty<RuntimeRuleSnapshot>();

    /// <summary>
    /// 创建空的运行包读取结果。
    /// </summary>
    /// <param name="runtimePackageId">运行包主键。</param>
    /// <param name="runtimePackageVersion">运行包版本号。</param>
    /// <returns>不含规则快照的读取结果。</returns>
    public static ActiveRuntimePackageReadResult Empty(long? runtimePackageId, long? runtimePackageVersion)
    {
        return new ActiveRuntimePackageReadResult
        {
            RuntimePackageId = runtimePackageId,
            RuntimePackageVersion = runtimePackageVersion
        };
    }
}
