using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 规则能力注册表。
/// </summary>
/// <remarks>
/// 从运行期已注册的条件评估器和动作执行器构建能力矩阵，供发布门禁复用，
/// 避免“运行期已支持但发布门禁未放行”或“静态白名单放行但运行期无实现”的漂移。
/// </remarks>
public sealed class RuleCapabilityRegistry
{
    private readonly HashSet<string> _conditionTypes;
    private readonly HashSet<string> _actionTypes;
    private readonly HashSet<string> _formulaExecutorCodes;

    /// <summary>
    /// 初始化规则能力注册表。
    /// </summary>
    public RuleCapabilityRegistry(
        IEnumerable<IRuleConditionEvaluator> conditionEvaluators,
        IEnumerable<IRuleActionExecutor> actionExecutors)
    {
        _conditionTypes = conditionEvaluators
            .Where(evaluator => !string.IsNullOrWhiteSpace(evaluator.ConditionType))
            .SelectMany(evaluator => RuleConditionTypeCodes.GetAliases(evaluator.ConditionType))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var executorList = actionExecutors.ToList();
        _actionTypes = executorList
            .Where(executor => !string.IsNullOrWhiteSpace(executor.ActionType))
            .Select(executor => executor.ActionType)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _formulaExecutorCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var executor in executorList.Where(IsFormulaActionExecutor))
        {
            if (executor is not IFormulaExecutorCapabilityMetadata metadata)
            {
                throw new InvalidOperationException(
                    $"FORMULA_CALC 执行器 {executor.GetType().Name} 未声明 {nameof(IFormulaExecutorCapabilityMetadata)}。");
            }

            foreach (var executorCode in metadata.SupportedExecutorCodes.Where(code => !string.IsNullOrWhiteSpace(code)))
            {
                _formulaExecutorCodes.Add(executorCode);
            }
        }
    }

    /// <summary>
    /// 判断条件类型是否受当前运行期支持。
    /// </summary>
    public bool SupportsConditionType(string? conditionType) =>
        !string.IsNullOrWhiteSpace(conditionType) && _conditionTypes.Contains(conditionType);

    /// <summary>
    /// 判断动作类型是否受当前运行期支持。
    /// </summary>
    public bool SupportsActionType(string? actionType) =>
        !string.IsNullOrWhiteSpace(actionType) && _actionTypes.Contains(actionType);

    /// <summary>
    /// 判断公式执行器编码是否受当前运行期支持。
    /// </summary>
    public bool SupportsFormulaExecutorCode(string? executorCode) =>
        !string.IsNullOrWhiteSpace(executorCode) && _formulaExecutorCodes.Contains(executorCode);

    private static bool IsFormulaActionExecutor(IRuleActionExecutor executor) =>
        string.Equals(executor.ActionType, RuleActionTypeCodes.FormulaCalc, StringComparison.OrdinalIgnoreCase);
}
