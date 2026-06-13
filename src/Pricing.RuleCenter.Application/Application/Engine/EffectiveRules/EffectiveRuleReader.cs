using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Engine.EffectiveRules;

/// <summary>
/// 当前生效规则读取器。
/// </summary>
/// <remarks>
/// 计价主链路直接读取 <c>PR_RULE_*</c> 生效规则。当前业务规则发布只在无人收费时段启用，
/// 不再维护独立的包化读模型。
/// </remarks>
public sealed class EffectiveRuleReader
{
    /// <summary>
    /// 规则表读取所需的仓储集合。
    /// </summary>
    private readonly RuleMatchRepositories _repositories;

    /// <summary>
    /// 初始化当前生效规则读取器。
    /// </summary>
    /// <param name="repositories">规则匹配所需的仓储集合。</param>
    public EffectiveRuleReader(RuleMatchRepositories repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// 按项目编码读取候选规则及其当前版本条件、动作。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>当前可参与匹配的规则视图集合。</returns>
    public async Task<IReadOnlyList<EffectiveRuleView>> LoadByItemCodeAsync(string itemCode)
    {
        return (await ReadCurrentAsync(itemCode)).Rules;
    }

    /// <summary>
    /// 按项目编码读取当前请求可见的规则视图。
    /// </summary>
    public async Task<EffectiveRuleReadResult> ReadCurrentAsync(string itemCode)
    {
        var headers = await _repositories.HeaderRepository.GetByItemCodeAsync(itemCode);
        if (headers.Count == 0)
        {
            return new EffectiveRuleReadResult();
        }

        var ruleVersions = headers
            .Select(header => (header.RuleId, header.CurrentVersion))
            .Distinct()
            .ToArray();
        var conditionsByRuleVersion = await _repositories.ConditionRepository.GetByRuleVersionsAsync(ruleVersions);
        var actionsByRuleVersion = await _repositories.ActionRepository.GetByRuleVersionsAsync(ruleVersions);
        var rules = new List<EffectiveRuleView>(headers.Count);

        foreach (var header in headers)
        {
            var key = (header.RuleId, header.CurrentVersion);
            IReadOnlyList<RuleCondition> conditions = conditionsByRuleVersion.TryGetValue(key, out var conditionItems)
                ? conditionItems
                : Array.Empty<RuleCondition>();
            IReadOnlyList<RuleAction> actions = actionsByRuleVersion.TryGetValue(key, out var actionItems)
                ? actionItems
                : Array.Empty<RuleAction>();

            rules.Add(new EffectiveRuleView
            {
                Header = header,
                Conditions = conditions,
                Actions = actions
            });
        }

        return new EffectiveRuleReadResult
        {
            Rules = rules
        };
    }
}

/// <summary>
/// 当前请求可见规则读取结果。
/// </summary>
public sealed class EffectiveRuleReadResult
{
    /// <summary>
    /// 当前请求可见的生效规则视图集合。
    /// </summary>
    public IReadOnlyList<EffectiveRuleView> Rules { get; init; } = Array.Empty<EffectiveRuleView>();
}
