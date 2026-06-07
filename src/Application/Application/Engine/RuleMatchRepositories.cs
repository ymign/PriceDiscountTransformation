using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则匹配服务需要读取的规则配置仓储集合。
/// </summary>
/// <remarks>
/// 规则匹配阶段只读规则主档、条件、动作和字典顺序，不负责发布或写状态。
/// 将这些只读依赖聚合为专用依赖包，可以让 <see cref="RuleMatchService"/> 构造函数保持短小，
/// 同时仍然清晰表达规则匹配的真实数据来源。
/// </remarks>
public sealed class RuleMatchRepositories
{
    /// <summary>
    /// 初始化规则匹配仓储集合。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="dictRepository">字典仓储。</param>
    public RuleMatchRepositories(
        IRuleHeaderRepository headerRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository dictRepository)
        : this(
            headerRepository,
            conditionRepository,
            actionRepository,
            dictRepository,
            null,
            null)
    {
    }

    /// <summary>
    /// 初始化规则匹配仓储集合，并可选附带新的运行时包读取依赖。
    /// </summary>
    public RuleMatchRepositories(
        IRuleHeaderRepository headerRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository dictRepository,
        IRuntimePackageStateRepository? runtimePackageStateRepository,
        IRuntimeRuleReadRepository? runtimeRuleReadRepository)
    {
        HeaderRepository = headerRepository;
        ConditionRepository = conditionRepository;
        ActionRepository = actionRepository;
        DictRepository = dictRepository;
        RuntimePackageStateRepository = runtimePackageStateRepository;
        RuntimeRuleReadRepository = runtimeRuleReadRepository;
    }

    /// <summary>
    /// 规则主档仓储，用于按项目获取候选规则。
    /// </summary>
    public IRuleHeaderRepository HeaderRepository { get; }

    /// <summary>
    /// 规则条件仓储，用于读取候选规则的条件集合。
    /// </summary>
    public IRuleConditionRepository ConditionRepository { get; }

    /// <summary>
    /// 规则动作仓储，用于读取命中规则版本的动作集合。
    /// </summary>
    public IRuleActionRepository ActionRepository { get; }

    /// <summary>
    /// 字典仓储，用于读取动作执行顺序配置。
    /// </summary>
    public IDictRepository DictRepository { get; }

    /// <summary>
    /// 运行时包活动指针仓储。存在时优先走新运行时读模型。
    /// </summary>
    public IRuntimePackageStateRepository? RuntimePackageStateRepository { get; }

    /// <summary>
    /// 运行时规则读仓储。存在时优先走新运行时读模型。
    /// </summary>
    public IRuntimeRuleReadRepository? RuntimeRuleReadRepository { get; }
}
