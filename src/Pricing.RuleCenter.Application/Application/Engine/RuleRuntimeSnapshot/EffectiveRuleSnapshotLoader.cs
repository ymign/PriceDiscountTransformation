using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Application.RuntimePackages;

namespace Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照加载器。
/// </summary>
/// <remarks>
/// <para>
/// 规则中心存在两套规则来源：旧规则表 <c>PR_RULE_*</c> 和新策略平台编译出的运行包表。
/// 当运行包仓储已注册时，运行期计价必须优先读取当前激活运行包，保证发布/回滚具备版本一致性；
/// 仅在运行包基础设施不存在时才回退旧规则表。
/// </para>
/// </remarks>
public sealed class EffectiveRuleSnapshotLoader
{
    /// <summary>
    /// 旧规则表和运行包读取所需的仓储集合。
    /// </summary>
    private readonly RuleMatchRepositories _repositories;

    /// <summary>
    /// 激活运行包读取器。存在时表示当前环境支持新策略平台运行期模型。
    /// </summary>
    private readonly ActiveRuntimePackageReader? _runtimePackageReader;

    /// <summary>
    /// 运行包规则投影适配器，把运行期规则结构适配成引擎统一快照结构。
    /// </summary>
    private readonly RuntimeRuleProjectionAdapter _runtimeProjectionAdapter = new();

    /// <summary>
    /// 初始化运行期生效规则快照加载器。
    /// </summary>
    /// <param name="repositories">规则匹配所需的仓储集合。</param>
    public EffectiveRuleSnapshotLoader(RuleMatchRepositories repositories)
    {
        _repositories = repositories;
        if (repositories.RuntimePackageStateRepository is not null &&
            repositories.RuntimeRuleReadRepository is not null)
        {
            _runtimePackageReader = new ActiveRuntimePackageReader(
                repositories.RuntimePackageStateRepository,
                repositories.RuntimeRuleReadRepository,
                repositories.RuntimePackageTraceContextAccessor);
        }
    }

    /// <summary>
    /// 按项目编码加载候选规则及其当前版本条件、动作。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>当前运行期可参与匹配的规则快照集合。</returns>
    public async Task<IReadOnlyList<EffectiveRuleSnapshot>> LoadByItemCodeAsync(string itemCode)
    {
        if (_runtimePackageReader is not null)
        {
            // 新策略平台路径：只读取当前激活运行包。
            // 这样规则发布瞬间不会混读旧版本和新版本，也方便追踪每次计价命中的运行包版本。
            var runtimeSnapshots = await _runtimePackageReader.LoadByItemCodeAsync(itemCode);
            return runtimeSnapshots.Select(_runtimeProjectionAdapter.Adapt).ToList();
        }

        // 旧规则表回退路径：用于尚未启用策略平台/运行包表的环境。
        var headers = await _repositories.HeaderRepository.GetByItemCodeAsync(itemCode);
        if (headers.Count == 0)
        {
            return Array.Empty<EffectiveRuleSnapshot>();
        }

        var ruleVersions = headers
            .Select(header => (header.RuleId, header.CurrentVersion))
            .Distinct()
            .ToArray();
        var conditionsByRuleVersion = await _repositories.ConditionRepository.GetByRuleVersionsAsync(ruleVersions);
        var actionsByRuleVersion = await _repositories.ActionRepository.GetByRuleVersionsAsync(ruleVersions);
        var snapshots = new List<EffectiveRuleSnapshot>(headers.Count);

        foreach (var header in headers)
        {
            // 条件和动作按 ruleId + currentVersion 聚合，确保使用规则头当前版本，不混入历史版本配置。
            var key = (header.RuleId, header.CurrentVersion);
            IReadOnlyList<RuleCondition> conditions = conditionsByRuleVersion.TryGetValue(key, out var conditionItems)
                ? conditionItems
                : Array.Empty<RuleCondition>();
            IReadOnlyList<RuleAction> actions = actionsByRuleVersion.TryGetValue(key, out var actionItems)
                ? actionItems
                : Array.Empty<RuleAction>();

            snapshots.Add(new EffectiveRuleSnapshot
            {
                Header = header,
                Conditions = conditions,
                Actions = actions
            });
        }

        return snapshots;
    }
}
