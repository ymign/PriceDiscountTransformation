namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则动作实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_ACTION
/// </para>
/// <para>
/// 在规则体系中的角色：动作定义规则命中后要执行的计算、限额或折扣策略。
/// 动作归属于具体规则版本（通过 RuleId + VersionNo 关联），
/// 发布后应保持稳定，后续调整需要创建新版本。
/// </para>
/// <para>
/// 条件-动作分离设计：规则建模为"满足条件时执行动作"。
/// PR_RULE_CONDITION 定义"何时命中"，PR_RULE_ACTION 定义"命中后做什么"。
/// 新增规则类型只需新增执行器（IRuleConditionEvaluator / IRuleActionExecutor），
/// 无需修改各渠道代码。
/// </para>
/// <para>
/// 互斥组：同组动作只应执行优先级最高的一条（由 ExclusiveGroup + SortNo 控制）。
/// 例如同组互斥项目只保留优先级最高的那条收费。
/// </para>
/// </remarks>
public sealed class RuleAction
{
    /// <summary>
    /// 规则动作主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 ACTION_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条规则动作记录。
    /// </remarks>
    public long ActionId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，用于关联规则头、版本、条件、动作和追溯结果。
    /// 同一规则下可以有多条动作，按 SortNo 排序执行。
    /// </remarks>
    public long RuleId { get; set; }

    /// <summary>
    /// 规则版本号。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_VERSION.VERSION_NO，用于将动作锁定到特定版本。
    /// 同一规则的不同版本可以有不同的动作配置。
    /// 规则条件和动作的 VERSION_NO 必须对齐。
    /// </remarks>
    public int VersionNo { get; set; }

    /// <summary>
    /// 动作类型编码。
    /// </summary>
    /// <remarks>
    /// 决定由哪个动作执行器（IRuleActionExecutor）处理该动作。
    /// 常见值：
    /// <list type="bullet">
    /// <item>CONVERT_QTY — 双单位换算</item>
    /// <item>FORMULA_CALC — 公式计算，由公式定义或执行器参数决定具体公式</item>
    /// <item>APPLY_MIN_AMOUNT / APPLY_MAX_AMOUNT — 金额下限或上限</item>
    /// <item>APPLY_DAY_LIMIT_QTY / APPLY_TIME_WINDOW_LIMIT / APPLY_ONCE_LIMIT_QTY — 数量限额</item>
    /// <item>SAME_GROUP_MUTEX / SAME_OPERATION_CEILING — 同组互斥或同手术封顶</item>
    /// <item>DISCOUNT_EXCEED_TO_ZERO / ADD_CHILD_ITEM — 超出归零或子项加收</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// 执行器编码。
    /// </summary>
    /// <remarks>
    /// 用于在同一动作类型下区分具体公式或策略。
    /// 例如 ACTION_TYPE = FORMULA_CALC 时，ExecutorCode 指定使用哪个公式定义（PR_FORMULA_DEF.FORMULA_CODE）。
    /// 由计价引擎根据该编码查找对应的执行器实例。
    /// </remarks>
    public string ExecutorCode { get; set; } = string.Empty;

    /// <summary>
    /// 扩展参数 JSON。
    /// </summary>
    /// <remarks>
    /// 用于承载动作或条件的可变配置，格式由执行器定义。
    /// 例如公式动作的参数可能包含：{"formulaId": 1, "baseQty": 1, "multiplier": 2.5}
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// 空值表示该动作不需要额外参数。
    /// </remarks>
    public string? ParamsJson { get; set; }

    /// <summary>
    /// 互斥组编码。
    /// </summary>
    /// <remarks>
    /// 同组动作只应执行优先级最高的一条（由 SortNo 控制优先级）。
    /// 例如同手术互斥：同一手术下的多个项目，只执行优先级最高的那条收费规则。
    /// 空值表示该动作不参与互斥判断。
    /// </remarks>
    public string? ExclusiveGroup { get; set; }

    /// <summary>
    /// 排序号。
    /// </summary>
    /// <remarks>
    /// 数字越小越先执行。用于：
    /// 1. 控制同一规则内多条动作的执行顺序（如先换算、再公式、再限额）
    /// 2. 互斥组内确定优先级（数字越小优先级越高）
    /// 默认值通常从 10 开始，间隔 10，便于插入。
    /// </remarks>
    public int SortNo { get; set; }

    /// <summary>
    /// 动作异常处理策略。
    /// </summary>
    /// <remarks>
    /// 当动作执行器抛出异常时的处理方式：
    /// <list type="bullet">
    /// <item>STOP — 停止执行后续动作，返回错误（资金相关动作默认策略）</item>
    /// <item>SKIP — 跳过该动作，继续执行后续动作</item>
    /// <item>DEFAULT — 使用默认值继续</item>
    /// </list>
    /// 资金相关动作（如 FORMULA、LIMIT_AMT）默认应为 STOP，防止异常导致金额错误。
    /// 默认值为 "STOP"。
    /// </remarks>
    public string OnError { get; set; } = "STOP";

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该动作参与规则匹配和执行，"N" 表示禁用。
    /// 禁用的动作在规则发布后不会被执行器处理。
    /// 工作台可以通过该标识临时禁用某条动作而不删除。
    /// 默认值为 "Y"。
    /// </remarks>
    public string IsEnabled { get; set; } = "Y";
}
