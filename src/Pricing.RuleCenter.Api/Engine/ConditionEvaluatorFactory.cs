using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Api.Engine;

/// <summary>
/// 规则条件评估器工厂，负责根据条件类型编码定位对应评估器。
/// </summary>
/// <remarks>
/// 条件类型来自规则配置表。工厂把所有 <see cref="IRuleConditionEvaluator"/> 实现按 ConditionType
/// 建立索引，使规则匹配服务不需要知道具体评估器类名。
/// </remarks>
public sealed class ConditionEvaluatorFactory
{
    /// <summary>
    /// 条件类型到评估器实例的索引。键不区分大小写，兼容配置表中的大小写差异。
    /// </summary>
    private readonly Dictionary<string, IRuleConditionEvaluator> _evaluators;

    /// <summary>
    /// 初始化条件评估器工厂。
    /// </summary>
    /// <param name="evaluators">依赖注入容器中注册的全部条件评估器。</param>
    public ConditionEvaluatorFactory(IEnumerable<IRuleConditionEvaluator> evaluators)
    {
        // 构造时建索引，匹配规则时每个条件都可以 O(1) 查到对应评估器。
        _evaluators = evaluators.ToDictionary(e => e.ConditionType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 根据条件类型获取评估器。
    /// </summary>
    /// <param name="conditionType">规则条件类型编码。</param>
    /// <returns>匹配到的评估器；未注册时返回 <c>null</c>，由匹配服务决定是否视为不命中。</returns>
    public IRuleConditionEvaluator? GetEvaluator(string conditionType)
    {
        // 不在工厂内抛异常，避免一个未知条件类型直接终止所有规则匹配；上层可以记录追踪步骤。
        _evaluators.TryGetValue(conditionType, out var evaluator);
        return evaluator;
    }
}
