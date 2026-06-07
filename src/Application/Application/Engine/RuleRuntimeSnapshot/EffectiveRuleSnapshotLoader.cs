using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照加载器。
/// </summary>
public sealed class EffectiveRuleSnapshotLoader
{
    private readonly RuleMatchRepositories _repositories;

    /// <summary>
    /// 初始化运行期生效规则快照加载器。
    /// </summary>
    public EffectiveRuleSnapshotLoader(RuleMatchRepositories repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// 按项目编码加载候选规则及其当前版本条件、动作。
    /// </summary>
    public async Task<IReadOnlyList<EffectiveRuleSnapshot>> LoadByItemCodeAsync(string itemCode)
    {
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
