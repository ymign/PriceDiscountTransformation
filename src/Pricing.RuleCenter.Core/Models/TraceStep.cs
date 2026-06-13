namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 计价追踪步骤的内存表示。
/// </summary>
/// <remarks>
/// 该对象在引擎运行时生成，随后被应用服务转换为持久化步骤表。它保留每个阶段的输入值、输出值和参数快照，
/// 用于解释规则为什么命中、数量为什么变化、金额为什么被截断。
/// </remarks>
public sealed class TraceStep
{
    /// <summary>
    /// 步骤序号，按计价链路执行顺序递增。
    /// </summary>
    public int StepNo { get; set; }

    /// <summary>
    /// 步骤类型，只使用 DDL 允许的 MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR。
    /// </summary>
    public string StepType { get; set; } = string.Empty;

    /// <summary>
    /// 步骤说明，解释命中规则、限制口径或折价原因。
    /// </summary>
    public string? StepDesc { get; set; }

    /// <summary>
    /// 当前步骤处理前的关键数值，通常是数量或金额。
    /// </summary>
    public decimal? InputValue { get; set; }

    /// <summary>
    /// 当前步骤处理后的关键数值，通常是数量或金额。
    /// </summary>
    public decimal? OutputValue { get; set; }

    /// <summary>
    /// 产生本步骤的规则主键。
    /// </summary>
    /// <remarks>
    /// 历史命名兼容字段。当前直接规则链路用它暂存 PR_RULE_HEADER.RULE_ID，
    /// 对外响应和落库时统一映射为 RuleId。
    /// </remarks>
    public long? RuntimeRuleId { get; set; }

    /// <summary>
    /// 产生本步骤的规则编码。
    /// </summary>
    public string? RuleCode { get; set; }

    /// <summary>
    /// 产生本步骤的规则名称。
    /// </summary>
    public string? RuleName { get; set; }

    /// <summary>
    /// 产生本步骤的动作编码，例如 APPLY_TIME_WINDOW_LIMIT。
    /// </summary>
    public string? ActionCode { get; set; }

    /// <summary>
    /// 产生本步骤的执行器编码，用于区分同一动作类型下的具体公式或策略。
    /// </summary>
    public string? ExecutorCode { get; set; }

    /// <summary>
    /// 输入输出数值的业务类型，例如 AMOUNT、QTY、MATCH_RESULT。
    /// </summary>
    public string? ValueType { get; set; }

    /// <summary>
    /// 输入输出数值的单位，例如元、次、个。
    /// </summary>
    public string? ValueUnit { get; set; }

    /// <summary>
    /// 输入值的业务名称，例如处理前金额。
    /// </summary>
    public string? InputName { get; set; }

    /// <summary>
    /// 输出值的业务名称，例如处理后金额。
    /// </summary>
    public string? OutputName { get; set; }

    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置。
    /// </summary>
    public string? ParamsJson { get; set; }
}
