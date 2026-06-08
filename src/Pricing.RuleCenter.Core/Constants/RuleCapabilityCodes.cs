namespace Pricing.RuleCenter.Core.Constants;

/// <summary>
/// 规则条件类型编码。
/// </summary>
public static class RuleConditionTypeCodes
{
    /// <summary>项目匹配条件。</summary>
    public const string ItemMatch = "ITEM_MATCH";

    /// <summary>项目匹配条件的历史兼容别名。</summary>
    public const string ItemCode = "ITEM_CODE";

    /// <summary>部位匹配条件。</summary>
    public const string BodyPart = "BODY_PART";

    /// <summary>部位匹配条件的种子字典别名。</summary>
    public const string BodyPartMatch = "BODY_PART_MATCH";

    /// <summary>收费场景匹配条件。</summary>
    public const string ChargeScene = "CHARGE_SCENE";

    /// <summary>收费场景匹配条件的种子字典别名。</summary>
    public const string ChargeSceneMatch = "CHARGE_SCENE_MATCH";

    /// <summary>时间范围匹配条件。</summary>
    public const string TimeRange = "TIME_RANGE";

    /// <summary>同孕次匹配条件。</summary>
    public const string PregnancyMatch = "PREGNANCY_MATCH";

    /// <summary>就诊类型匹配条件。</summary>
    public const string VisitTypeMatch = "VISIT_TYPE_MATCH";

    /// <summary>年龄匹配条件。</summary>
    public const string AgeMatch = "AGE_MATCH";

    /// <summary>项目组匹配条件。</summary>
    public const string GroupMatch = "GROUP_MATCH";

    /// <summary>收费科室排除条件。</summary>
    public const string ChargeDeptExclude = "CHARGE_DEPT_EXCLUDE";

    /// <summary>医保类型匹配条件。</summary>
    public const string InsuranceTypeMatch = "INSURANCE_TYPE_MATCH";

    /// <summary>诊断匹配条件。</summary>
    public const string DiagnosisMatch = "DIAGNOSIS_MATCH";

    /// <summary>设备类型匹配条件。</summary>
    public const string DeviceTypeMatch = "DEVICE_TYPE_MATCH";

    private static readonly HashSet<string> Supported = CreateSet(
        ItemMatch,
        ItemCode,
        BodyPart,
        BodyPartMatch,
        ChargeScene,
        ChargeSceneMatch,
        TimeRange,
        PregnancyMatch,
        VisitTypeMatch,
        AgeMatch,
        GroupMatch,
        ChargeDeptExclude,
        InsuranceTypeMatch,
        DiagnosisMatch,
        DeviceTypeMatch);

    private static readonly Dictionary<string, string[]> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        [ItemMatch] = new[] { ItemMatch, ItemCode },
        [BodyPart] = new[] { BodyPart, BodyPartMatch },
        [ChargeScene] = new[] { ChargeScene, ChargeSceneMatch }
    };

    /// <summary>
    /// 判断条件类型是否可由当前规则引擎表达。
    /// </summary>
    /// <param name="conditionType">条件类型编码。</param>
    /// <returns><see langword="true" /> 表示支持；否则为 <see langword="false" />。</returns>
    public static bool IsSupported(string? conditionType) =>
        !string.IsNullOrWhiteSpace(conditionType) && Supported.Contains(conditionType);

    /// <summary>
    /// 获取条件类型的标准码和兼容别名。
    /// </summary>
    /// <param name="conditionType">条件类型标准编码。</param>
    /// <returns>可映射到同一评估器的一组条件类型编码。</returns>
    public static IReadOnlyList<string> GetAliases(string conditionType) =>
        Aliases.TryGetValue(conditionType, out var aliases)
            ? aliases
            : new[] { conditionType };

    private static HashSet<string> CreateSet(params string[] codes) =>
        new(codes, StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// 规则动作类型编码。
/// </summary>
public static class RuleActionTypeCodes
{
    /// <summary>数量换算动作。</summary>
    public const string ConvertQty = "CONVERT_QTY";

    /// <summary>公式计算动作。</summary>
    public const string FormulaCalc = "FORMULA_CALC";

    /// <summary>金额下限动作。</summary>
    public const string ApplyMinAmount = "APPLY_MIN_AMOUNT";

    /// <summary>金额上限动作。</summary>
    public const string ApplyMaxAmount = "APPLY_MAX_AMOUNT";

    /// <summary>日数量限制动作。</summary>
    public const string ApplyDayLimitQty = "APPLY_DAY_LIMIT_QTY";

    /// <summary>时间窗数量限制动作。</summary>
    public const string ApplyTimeWindowLimit = "APPLY_TIME_WINDOW_LIMIT";

    /// <summary>单次数量限制动作。</summary>
    public const string ApplyOnceLimitQty = "APPLY_ONCE_LIMIT_QTY";

    /// <summary>同组互斥动作。</summary>
    public const string SameGroupMutex = "SAME_GROUP_MUTEX";

    /// <summary>同手术封顶动作。</summary>
    public const string SameOperationCeiling = "SAME_OPERATION_CEILING";

    /// <summary>子项加收动作。</summary>
    public const string AddChildItem = "ADD_CHILD_ITEM";

    /// <summary>超出部分归零动作。</summary>
    public const string DiscountExceedToZero = "DISCOUNT_EXCEED_TO_ZERO";

    private static readonly HashSet<string> Supported = CreateSet(
        ConvertQty,
        FormulaCalc,
        ApplyMinAmount,
        ApplyMaxAmount,
        ApplyDayLimitQty,
        ApplyTimeWindowLimit,
        ApplyOnceLimitQty,
        SameGroupMutex,
        SameOperationCeiling,
        AddChildItem,
        DiscountExceedToZero);

    /// <summary>
    /// 判断动作类型是否可由当前规则引擎表达。
    /// </summary>
    /// <param name="actionType">动作类型编码。</param>
    /// <returns><see langword="true" /> 表示支持；否则为 <see langword="false" />。</returns>
    public static bool IsSupported(string? actionType) =>
        !string.IsNullOrWhiteSpace(actionType) && Supported.Contains(actionType);

    private static HashSet<string> CreateSet(params string[] codes) =>
        new(codes, StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// 规则动作异常处理策略编码。
/// </summary>
public static class ActionOnErrorCodes
{
    /// <summary>动作失败时中断后续计价。</summary>
    public const string Stop = "STOP";

    /// <summary>动作失败时跳过当前动作。</summary>
    public const string Skip = "SKIP";

    /// <summary>动作失败时记录警告并继续。</summary>
    public const string Warn = "WARN";
}

/// <summary>
/// 公式执行器编码。
/// </summary>
public static class FormulaExecutorCodes
{
    /// <summary>增量比例公式编码。</summary>
    public const string IncrementPercent = "INCREMENT_PERCENT";

    /// <summary>增量比例公式历史类名别名。</summary>
    public const string IncrementPercentExecutor = "IncrementPercentExecutor";

    /// <summary>面积分段递增公式编码。</summary>
    public const string AreaStepIncrement = "AREA_STEP_INCREMENT";

    /// <summary>面积分段递增公式种子执行器别名。</summary>
    public const string AreaStepIncrementExecutor = "AreaStepIncrementExecutor";

    /// <summary>分部位数量换算公式编码。</summary>
    public const string ConvertQtyByPart = "CONVERT_QTY_BY_PART";

    /// <summary>分部位数量换算公式种子执行器别名。</summary>
    public const string ConvertQtyByPartExecutor = "ConvertQtyByPartExecutor";

    /// <summary>子项百分比加收公式编码。</summary>
    public const string ChildItemPercent = "CHILD_ITEM_PERCENT";

    /// <summary>子项百分比加收公式种子执行器别名。</summary>
    public const string ChildItemPercentExecutor = "ChildItemPercentExecutor";

    /// <summary>表达式公式编码。</summary>
    public const string ExpressionFormula = "EXPRESSION_FORMULA";

    private static readonly HashSet<string> Supported = CreateSet(
        IncrementPercent,
        IncrementPercentExecutor,
        AreaStepIncrement,
        AreaStepIncrementExecutor,
        ConvertQtyByPart,
        ConvertQtyByPartExecutor,
        ChildItemPercent,
        ChildItemPercentExecutor,
        ExpressionFormula);

    /// <summary>
    /// 判断公式执行器编码是否可由当前规则引擎表达。
    /// </summary>
    /// <param name="executorCode">公式执行器编码。</param>
    /// <returns><see langword="true" /> 表示支持；否则为 <see langword="false" />。</returns>
    public static bool IsSupported(string? executorCode) =>
        !string.IsNullOrWhiteSpace(executorCode) && Supported.Contains(executorCode);

    /// <summary>
    /// 判断是否为增量比例公式编码或别名。
    /// </summary>
    /// <param name="executorCode">公式执行器编码。</param>
    /// <returns><see langword="true" /> 表示匹配；否则为 <see langword="false" />。</returns>
    public static bool IsIncrementPercent(string? executorCode) =>
        IsAny(executorCode, IncrementPercent, IncrementPercentExecutor);

    /// <summary>
    /// 判断是否为面积分段递增公式编码或别名。
    /// </summary>
    /// <param name="executorCode">公式执行器编码。</param>
    /// <returns><see langword="true" /> 表示匹配；否则为 <see langword="false" />。</returns>
    public static bool IsAreaStepIncrement(string? executorCode) =>
        IsAny(executorCode, AreaStepIncrement, AreaStepIncrementExecutor);

    /// <summary>
    /// 判断是否为分部位数量换算公式编码或别名。
    /// </summary>
    /// <param name="executorCode">公式执行器编码。</param>
    /// <returns><see langword="true" /> 表示匹配；否则为 <see langword="false" />。</returns>
    public static bool IsConvertQtyByPart(string? executorCode) =>
        IsAny(executorCode, ConvertQtyByPart, ConvertQtyByPartExecutor);

    /// <summary>
    /// 判断是否为子项百分比加收公式编码或别名。
    /// </summary>
    /// <param name="executorCode">公式执行器编码。</param>
    /// <returns><see langword="true" /> 表示匹配；否则为 <see langword="false" />。</returns>
    public static bool IsChildItemPercent(string? executorCode) =>
        IsAny(executorCode, ChildItemPercent, ChildItemPercentExecutor);

    /// <summary>
    /// 判断是否为表达式公式编码。
    /// </summary>
    /// <param name="executorCode">公式执行器编码。</param>
    /// <returns><see langword="true" /> 表示匹配；否则为 <see langword="false" />。</returns>
    public static bool IsExpressionFormula(string? executorCode) =>
        IsAny(executorCode, ExpressionFormula);

    private static bool IsAny(string? value, params string[] candidates) =>
        candidates.Any(candidate => string.Equals(value, candidate, StringComparison.OrdinalIgnoreCase));

    private static HashSet<string> CreateSet(params string[] codes) =>
        new(codes, StringComparer.OrdinalIgnoreCase);
}
