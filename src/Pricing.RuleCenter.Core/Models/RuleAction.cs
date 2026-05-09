using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_ACTION")]
/// <summary>
/// 规则动作实体，对应 PR_RULE_ACTION。
/// </summary>
/// <remarks>
/// 动作定义规则命中后要执行的计算、限额或折扣策略。动作归属于具体规则版本，
/// 发布后应保持稳定，后续调整需要创建新版本。
/// </remarks>
public sealed class RuleAction
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "ACTION_ID")]
    /// <summary>
    /// 规则动作主键
    /// </summary>
    public long ActionId { get; set; }

    [SugarColumn(ColumnName = "RULE_ID")]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "VERSION_NO")]
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int VersionNo { get; set; }

    [SugarColumn(ColumnName = "ACTION_TYPE")]
    /// <summary>
    /// 动作类型，决定由哪个动作执行器处理
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "EXECUTOR_CODE")]
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PARAMS_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; set; }

    [SugarColumn(ColumnName = "EXCLUSIVE_GROUP", IsNullable = true)]
    /// <summary>
    /// 互斥组编码，同组动作只应执行优先级最高的一条
    /// </summary>
    public string? ExclusiveGroup { get; set; }

    [SugarColumn(ColumnName = "SORT_NO")]
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "ON_ERROR")]
    /// <summary>
    /// 动作异常处理策略，资金相关动作默认应 STOP
    /// </summary>
    public string OnError { get; set; } = "STOP";

    [SugarColumn(ColumnName = "IS_ENABLED")]
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; set; } = "Y";
}
