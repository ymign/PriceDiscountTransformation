namespace Pricing.RuleCenter.Core.Constants;

/// <summary>
/// 规则动作默认追溯步骤类型。
/// </summary>
public static class RuleActionTraceStepTypes
{
    private static readonly HashSet<string> AllowedStepTypes = new(StringComparer.Ordinal)
    {
        Convert,
        Formula,
        Limit,
        Discount,
        Validate
    };

    /// <summary>
    /// 换算类追溯步骤类型。
    /// </summary>
    public const string Convert = "CONVERT";
    /// <summary>
    /// 公式类追溯步骤类型。
    /// </summary>
    public const string Formula = "FORMULA";
    /// <summary>
    /// 限额类追溯步骤类型。
    /// </summary>
    public const string Limit = "LIMIT";
    /// <summary>
    /// 折价类追溯步骤类型。
    /// </summary>
    public const string Discount = "DISCOUNT";
    /// <summary>
    /// 兜底校验类追溯步骤类型。
    /// </summary>
    public const string Validate = "VALIDATE";

    /// <summary>
    /// 按动作类型解析默认追溯步骤类型。
    /// </summary>
    /// <param name="actionType">规则动作类型编码。</param>
    /// <returns>默认追溯步骤类型。</returns>
    public static string Resolve(string actionType)
    {
        return actionType switch
        {
            "CONVERT_QTY" => Convert,
            "FORMULA_CALC" => Formula,
            "APPLY_MIN_AMOUNT" or "APPLY_MAX_AMOUNT" or "APPLY_DAY_LIMIT_QTY"
                or "APPLY_TIME_WINDOW_LIMIT" or "APPLY_ONCE_LIMIT_QTY"
                or "SAME_GROUP_MUTEX" or "SAME_OPERATION_CEILING" => Limit,
            "DISCOUNT_EXCEED_TO_ZERO" or "ADD_CHILD_ITEM" => Discount,
            _ => Validate
        };
    }

    /// <summary>
    /// 验证指定步骤类型是否属于允许集合。
    /// </summary>
    /// <param name="stepType">执行器返回的步骤类型。</param>
    /// <param name="actionType">当前动作类型，用于错误信息。</param>
    /// <returns>原始步骤类型。</returns>
    public static string EnsureAllowed(string stepType, string actionType)
    {
        if (!AllowedStepTypes.Contains(stepType))
        {
            throw new InvalidOperationException(
                $"动作 {actionType} 返回了非法追溯步骤类型 {stepType}");
        }

        return stepType;
    }
}
