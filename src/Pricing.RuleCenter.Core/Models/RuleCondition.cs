namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则条件实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_CONDITION
/// </para>
/// <para>
/// 在规则体系中的角色：条件定义规则是否命中的前置判断。
/// 条件归属于具体规则版本（通过 RuleId + VersionNo 关联）。
/// </para>
/// <para>
/// 条件聚合逻辑：
/// <list type="bullet">
/// <item>同组条件（同一 ConditionGroup）按 AND 聚合 — 全部满足才命中</item>
/// <item>不同组（不同 ConditionGroup）按 OR 聚合 — 任一组满足即命中</item>
/// </list>
/// ConditionGroup 和 SortNo 会影响匹配解释和追踪展示。
/// </para>
/// <para>
/// 条件-动作分离设计：PR_RULE_CONDITION 定义"何时命中"，PR_RULE_ACTION 定义"命中后做什么"。
/// 新增条件类型只需新增评估器（IRuleConditionEvaluator），无需修改各渠道代码。
/// </para>
/// </remarks>
public sealed class RuleCondition
{
    /// <summary>
    /// 规则条件主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 CONDITION_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条规则条件记录。
    /// </remarks>
        public long ConditionId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，用于关联规则头、版本、条件、动作和追溯结果。
    /// 同一规则下可以有多条条件，按 ConditionGroup 分组聚合。
    /// </remarks>
        public long RuleId { get; set; }

    /// <summary>
    /// 规则版本号。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_VERSION.VERSION_NO，用于将条件锁定到特定版本。
    /// 同一规则的不同版本可以有不同的条件配置。
    /// 规则条件和动作的 VERSION_NO 必须对齐。
    /// </remarks>
        public int VersionNo { get; set; }

    /// <summary>
    /// 条件组编码。
    /// </summary>
    /// <remarks>
    /// 同组条件按 AND 处理，不同组按 OR 处理。
    /// 默认值 "DEFAULT" 表示所有条件在同一个组内（即全部 AND）。
    /// 例如：条件组 A 判断"门诊场景 AND 特定项目"，条件组 B 判断"住院场景 AND 特定部位"，
    /// 满足 A 或 B 任一组即命中规则。
    /// </remarks>
        public string ConditionGroup { get; set; } = "DEFAULT";

    /// <summary>
    /// 条件类型编码。
    /// </summary>
    /// <remarks>
    /// 决定由哪个条件评估器（IRuleConditionEvaluator）处理该条件。
    /// 常见值：
    /// <list type="bullet">
    /// <item>ITEM_CODE — 项目编码匹配</item>
    /// <item>CHARGE_SCENE — 收费场景匹配</item>
    /// <item>BODY_PART — 身体部位匹配</item>
    /// <item>PATIENT_TYPE — 患者类型匹配</item>
    /// <item>TIME_RANGE — 时间范围匹配</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
        public string ConditionType { get; set; } = string.Empty;

    /// <summary>
    /// 比较运算符。
    /// </summary>
    /// <remarks>
    /// 定义左值和右值的比较方式：
    /// <list type="bullet">
    /// <item>EQ — 等于</item>
    /// <item>IN — 在列表中（RightValue 为逗号分隔的列表）</item>
    /// <item>BETWEEN — 在范围内（RightValue 为 "min,max" 格式）</item>
    /// <item>GT / GTE / LT / LTE — 大于/大于等于/小于/小于等于</item>
    /// <item>LIKE — 模糊匹配</item>
    /// <item>IS_NULL / IS_NOT_NULL — 空值判断</item>
    /// </list>
    /// 默认值为 "EQ"。
    /// </remarks>
        public string? OperatorType { get; set; } = "EQ";

    /// <summary>
    /// 条件左值字段名。
    /// </summary>
    /// <remarks>
    /// 通常对应请求上下文中的结构化字段路径，如 "itemCode"、"chargeScene"、"bodyPartCode"。
    /// 由条件评估器根据该字段名从请求上下文中提取实际值。
    /// 空值表示该条件类型不需要左值（如自定义条件）。
    /// </remarks>
        public string? LeftKey { get; set; }

    /// <summary>
    /// 条件右值。
    /// </summary>
    /// <remarks>
    /// 来自规则配置的期望值，与左值进行比较。
    /// 格式取决于 OperatorType：
    /// <list type="bullet">
    /// <item>EQ — 单个值，如 "ITEM_1001"</item>
    /// <item>IN — 逗号分隔列表，如 "ITEM_1001,ITEM_1002,ITEM_1003"</item>
    /// <item>BETWEEN — 范围，如 "0,100"</item>
    /// </list>
    /// 空值表示该条件类型不需要右值（如 IS_NULL 判断）。
    /// </remarks>
        public string? RightValue { get; set; }

    /// <summary>
    /// 扩展参数 JSON。
    /// </summary>
    /// <remarks>
    /// 用于承载条件的可变配置，格式由评估器定义。
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// 空值表示该条件不需要额外参数。
    /// </remarks>
        public string? ParamsJson { get; set; }

    /// <summary>
    /// 排序号。
    /// </summary>
    /// <remarks>
    /// 数字越小越先评估。用于：
    /// 1. 控制同一条件组内多条条件的评估顺序
    /// 2. 追踪展示时按顺序列出匹配的条件
    /// 默认值通常从 10 开始，间隔 10，便于插入。
    /// </remarks>
        public int SortNo { get; set; }

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该条件参与规则匹配，"N" 表示禁用。
    /// 禁用的条件在规则匹配时被跳过，不影响其他条件的评估。
    /// 工作台可以通过该标识临时禁用某条条件而不删除。
    /// 默认值为 "Y"。
    /// </remarks>
        public string IsEnabled { get; set; } = "Y";
}
