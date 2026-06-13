using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照加载器。
/// </summary>
/// <remarks>
/// 计价主链路直接读取 <c>PR_RULE_*</c> 生效规则。当前业务规则发布只在无人收费时段启用，
/// 不再维护独立的包化读模型。
/// </remarks>
public sealed class EffectiveRuleSnapshotLoader
{
    /// <summary>
    /// 规则表读取所需的仓储集合。
    /// </summary>
    private readonly RuleMatchRepositories _repositories;

    /// <summary>
    /// 初始化运行期生效规则快照加载器。
    /// </summary>
    /// <param name="repositories">规则匹配所需的仓储集合。</param>
    public EffectiveRuleSnapshotLoader(RuleMatchRepositories repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// 按项目编码加载候选规则及其当前版本条件、动作。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>当前可参与匹配的规则快照集合。</returns>
    public async Task<IReadOnlyList<EffectiveRuleSnapshot>> LoadByItemCodeAsync(string itemCode)
    {
        return (await LoadCurrentAsync(itemCode)).Snapshots;
    }

    /// <summary>
    /// 按项目编码加载当前请求可见的规则快照。
    /// </summary>
    public async Task<EffectiveRuleSnapshotLoadResult> LoadCurrentAsync(string itemCode)
    {
        var headers = await _repositories.HeaderRepository.GetByItemCodeAsync(itemCode);
        if (headers.Count == 0)
        {
            return new EffectiveRuleSnapshotLoadResult();
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

        return new EffectiveRuleSnapshotLoadResult
        {
            Snapshots = snapshots
        };
    }
}

/// <summary>
/// 当前请求可见规则快照加载结果。
/// </summary>
public sealed class EffectiveRuleSnapshotLoadResult
{
    /// <summary>
    /// Gets the effective rule snapshots visible to the current request.
    /// </summary>
    public IReadOnlyList<EffectiveRuleSnapshot> Snapshots { get; init; } = Array.Empty<EffectiveRuleSnapshot>();
}
