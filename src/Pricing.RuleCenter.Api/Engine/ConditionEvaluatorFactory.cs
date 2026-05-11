using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Api.Engine;

/// <summary>
/// 规则条件评估器工厂，负责根据条件类型编码（ConditionType）定位对应评估器实例。
/// </summary>
/// <remarks>
/// <para>
/// 【设计模式】工厂 + 策略模式。规则条件表（PR_RULE_CONDITION）保存条件类型编码，
/// 工厂按编码查找具体评估器，使规则匹配服务不需要知道具体评估器类名。
/// </para>
/// <para>
/// 【注册机制】与 <see cref="ActionExecutorFactory"/> 一致：ASP.NET Core 启动时通过 DI 注册
/// 所有 <see cref="IRuleConditionEvaluator"/> 实现，构造函数一次性注入并建索引。
/// </para>
/// <para>
/// 【评估器类型】典型实现包括：
/// <list type="bullet">
///   <item><description>ITEM_MATCH — 项目编码匹配</description></item>
///   <item><description>BODY_PART — 检查部位匹配</description></item>
///   <item><description>CHARGE_SCENE — 收费场景匹配（门诊/住院/急诊）</description></item>
///   <item><description>TIME_RANGE — 业务时间范围匹配</description></item>
///   <item><description>PREGNANCY_MATCH — 同胎次匹配（产科彩超）</description></item>
///   <item><description>VISIT_TYPE_MATCH — 就诊类型匹配（门诊/住院/急诊）</description></item>
///   <item><description>AGE_MATCH — 年龄范围匹配（儿童加收）</description></item>
///   <item><description>GROUP_MATCH — 项目组匹配（按组适用规则）</description></item>
/// </list>
/// </para>
/// <para>
/// 【错误处理策略】未注册的条件类型返回 null，由 <see cref="Engine.RuleMatchService"/>
/// 决定视为不命中（保守策略）。这样可以避免一个未知条件类型直接终止所有规则匹配，
/// 同时通过追踪步骤记录未知评估器，便于运维发现配置问题。
/// </para>
/// </remarks>
public sealed class ConditionEvaluatorFactory
{
    /// <summary>
    /// 条件类型到评估器实例的索引。
    /// </summary>
    /// <remarks>
    /// 键不区分大小写（OrdinalIgnoreCase），兼容配置表中可能出现的大小写差异。
    /// 同一 ConditionType 只能注册一个评估器；重复注册会在启动阶段 fail-fast。
    /// </remarks>
    private readonly Dictionary<string, IRuleConditionEvaluator> _evaluators;

    /// <summary>
    /// 初始化条件评估器工厂。
    /// </summary>
    /// <param name="evaluators">
    /// 依赖注入容器中注册的全部条件评估器实现。
    /// DI 容器按注册顺序注入，工厂统一按 ConditionType 建索引。
    /// </param>
    public ConditionEvaluatorFactory(IEnumerable<IRuleConditionEvaluator> evaluators)
    {
        // 构造时建索引，匹配规则时每个条件都可以 O(1) 查到对应评估器。
        // 规则匹配阶段可能评估数百条条件，O(1) 查找对性能至关重要。
        _evaluators = evaluators.ToDictionary(e => e.ConditionType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 根据条件类型编码获取对应的评估器实例。
    /// </summary>
    /// <param name="conditionType">
    /// 规则条件类型编码，来自 PR_RULE_CONDITION.CONDITION_TYPE 列。
    /// 典型值：ITEM_MATCH、BODY_PART、CHARGE_SCENE、TIME_RANGE。
    /// </param>
    /// <returns>
    /// 匹配到的评估器实例；未注册时返回 <c>null</c>。
    /// 上层匹配服务收到 null 后将该条件视为不命中，整条规则不会因为一个未知条件而误命中。
    /// </returns>
    public IRuleConditionEvaluator? GetEvaluator(string conditionType)
    {
        // 不在工厂内抛异常，避免一个未知条件类型直接终止所有规则匹配。
        // 上层可以记录追踪步骤（TraceStep），运维可通过追溯查询发现未识别的条件类型。
        _evaluators.TryGetValue(conditionType, out var evaluator);
        return evaluator;
    }
}
