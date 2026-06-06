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
        var snapshots = new List<EffectiveRuleSnapshot>(headers.Count);

        foreach (var header in headers)
        {
            var conditions = await _repositories.ConditionRepository.GetByRuleAndVersionAsync(
                header.RuleId,
                header.CurrentVersion);
            var actions = await _repositories.ActionRepository.GetByRuleAndVersionAsync(
                header.RuleId,
                header.CurrentVersion);

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
