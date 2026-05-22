using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;

namespace Pricing.RuleCenter.Api.Application.Rules;

/// <summary>
/// 规则发布状态机依赖的生命周期仓储集合。
/// </summary>
/// <remarks>
/// 生命周期仓储共同维护规则主档、规则版本、发布流水和变更日志。发布、停用和回滚必须同时更新这些仓储，
/// 因此作为一个强类型依赖包注入，比在服务构造函数中暴露过长参数列表更清晰。
/// </remarks>
public sealed class RulePublishLifecycleRepositories
{
    /// <summary>
    /// 初始化规则发布生命周期仓储集合。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="publishRepository">发布记录仓储。</param>
    /// <param name="changeLogRepository">规则变更日志仓储。</param>
    public RulePublishLifecycleRepositories(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRulePublishRepository publishRepository,
        IRuleChangeLogRepository changeLogRepository)
    {
        HeaderRepository = headerRepository;
        VersionRepository = versionRepository;
        PublishRepository = publishRepository;
        ChangeLogRepository = changeLogRepository;
    }

    /// <summary>
    /// 规则主档仓储，用于更新当前版本、规则状态和启用标识。
    /// </summary>
    public IRuleHeaderRepository HeaderRepository { get; }

    /// <summary>
    /// 规则版本仓储，用于草稿发布、旧版本失效和历史版本回滚。
    /// </summary>
    public IRuleVersionRepository VersionRepository { get; }

    /// <summary>
    /// 发布记录仓储，用于保存发布、停用和回滚流水。
    /// </summary>
    public IRulePublishRepository PublishRepository { get; }

    /// <summary>
    /// 变更日志仓储，用于保存面向配置人员和审计人员的变更摘要。
    /// </summary>
    public IRuleChangeLogRepository ChangeLogRepository { get; }
}

/// <summary>
/// 规则发布前冲突校验依赖的规则定义仓储集合。
/// </summary>
public sealed class RulePublishDefinitionRepositories
{
    /// <summary>
    /// 初始化规则定义仓储集合。
    /// </summary>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="dictRepository">字典仓储。</param>
    /// <param name="testCaseRepository">规则测试用例仓储。</param>
    /// <param name="testRunRepository">规则测试运行仓储。</param>
    public RulePublishDefinitionRepositories(
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository dictRepository,
        IRuleTestCaseRepository testCaseRepository,
        IRuleTestRunRepository testRunRepository)
    {
        ConditionRepository = conditionRepository;
        ActionRepository = actionRepository;
        DictRepository = dictRepository;
        TestCaseRepository = testCaseRepository;
        TestRunRepository = testRunRepository;
    }

    /// <summary>
    /// 规则条件仓储，用于读取收费场景、部位等冲突判断维度。
    /// </summary>
    public IRuleConditionRepository ConditionRepository { get; }

    /// <summary>
    /// 规则动作仓储，用于识别公式、金额限制、数量限制等互斥动作。
    /// </summary>
    public IRuleActionRepository ActionRepository { get; }

    /// <summary>
    /// 字典仓储，用于读取可配置的互斥动作类型。
    /// </summary>
    public IDictRepository DictRepository { get; }

    /// <summary>
    /// 规则测试用例仓储，用于发布前校验是否存在启用用例。
    /// </summary>
    public IRuleTestCaseRepository TestCaseRepository { get; }

    /// <summary>
    /// 规则测试运行仓储，用于发布前校验启用用例的最新运行是否全部通过。
    /// </summary>
    public IRuleTestRunRepository TestRunRepository { get; }
}


