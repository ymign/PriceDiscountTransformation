using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_TRACE_STEP")]
/// <summary>
/// 计价追踪步骤实体，对应 PR_CHARGE_TRACE_STEP。
/// </summary>
/// <remarks>
/// 每条记录描述计价过程中的一个阶段，按 StepNo 串起来即可还原规则匹配和动作执行顺序。
/// </remarks>
public sealed class ChargeTraceStep
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "STEP_ID")]
    /// <summary>
    /// 计价步骤日志主键
    /// </summary>
    public long StepId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_ID")]
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    /// <summary>
    /// 计价追踪号，用于跨表查看一次计价过程
    /// </summary>
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "STEP_NO")]
    /// <summary>
    /// 步骤序号，按计价链路执行顺序递增
    /// </summary>
    public int StepNo { get; set; }

    [SugarColumn(ColumnName = "STEP_NAME")]
    /// <summary>
    /// 步骤名称，用于追溯展示
    /// </summary>
    public string StepName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "STEP_TYPE")]
    /// <summary>
    /// 步骤类型，只使用 DDL 允许的 MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR
    /// </summary>
    public string StepType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_ID", IsNullable = true)]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long? RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_VERSION_NO", IsNullable = true)]
    /// <summary>
    /// 产生该步骤的规则版本号。
    /// </summary>
    public int? RuleVersionNo { get; set; }

    [SugarColumn(ColumnName = "INPUT_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 步骤执行前的输入快照
    /// </summary>
    public string? InputSnapshot { get; set; }

    [SugarColumn(ColumnName = "OUTPUT_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 步骤执行后的输出快照
    /// </summary>
    public string? OutputSnapshot { get; set; }

    [SugarColumn(ColumnName = "STEP_DESC", IsNullable = true)]
    /// <summary>
    /// 步骤说明，解释命中规则、限制口径或折价原因
    /// </summary>
    public string? StepDesc { get; set; }

    [SugarColumn(ColumnName = "CREATED_AT")]
    /// <summary>
    /// 记录创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
