using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_CONDITION")]
/// <summary>
/// 规则条件实体，对应 PR_RULE_CONDITION。
/// </summary>
/// <remarks>
/// 条件定义规则是否命中的前置判断。条件归属于具体规则版本，同组条件按 AND 聚合，不同组按 OR 聚合，
/// 因此 ConditionGroup 和 SortNo 会影响匹配解释和追踪展示。
/// </remarks>
public sealed class RuleCondition
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "CONDITION_ID")]
    /// <summary>
    /// 规则条件主键
    /// </summary>
    public long ConditionId { get; set; }

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

    [SugarColumn(ColumnName = "CONDITION_GROUP")]
    /// <summary>
    /// 条件组，同组条件按 AND 处理，不同组按 OR 处理
    /// </summary>
    public string ConditionGroup { get; set; } = "DEFAULT";

    [SugarColumn(ColumnName = "CONDITION_TYPE")]
    /// <summary>
    /// 条件类型，决定由哪个条件评估器处理
    /// </summary>
    public string ConditionType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "OPERATOR_TYPE", IsNullable = true)]
    /// <summary>
    /// 比较运算符，例如 EQ、IN、BETWEEN
    /// </summary>
    public string? OperatorType { get; set; } = "EQ";

    [SugarColumn(ColumnName = "LEFT_KEY", IsNullable = true)]
    /// <summary>
    /// 条件左值字段名，通常对应请求上下文中的结构化字段
    /// </summary>
    public string? LeftKey { get; set; }

    [SugarColumn(ColumnName = "RIGHT_VALUE", IsNullable = true)]
    /// <summary>
    /// 条件右值，来自规则配置
    /// </summary>
    public string? RightValue { get; set; }

    [SugarColumn(ColumnName = "PARAMS_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; set; }

    [SugarColumn(ColumnName = "SORT_NO")]
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; set; } = "Y";
}
