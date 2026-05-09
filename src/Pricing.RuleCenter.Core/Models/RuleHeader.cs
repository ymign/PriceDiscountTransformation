using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_HEADER")]
/// <summary>
/// 规则主档实体，对应 PR_RULE_HEADER。
/// </summary>
/// <remarks>
/// 规则主档保存规则身份、适用范围、优先级和当前发布版本。条件和动作挂在具体版本上，
/// 主档的 CurrentVersion 决定计价时读取哪套版本明细。
/// </remarks>
public sealed class RuleHeader
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "RULE_ID")]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_CODE")]
    /// <summary>
    /// 规则编码，全局唯一，用于业务配置和运维识别
    /// </summary>
    public string RuleCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_NAME")]
    /// <summary>
    /// 规则名称，用于工作台展示和审计
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_CATEGORY")]
    /// <summary>
    /// 规则类别，例如折价、公式、限额或混合规则
    /// </summary>
    public string RuleCategory { get; set; } = "MIXED";

    [SugarColumn(ColumnName = "RULE_SCOPE")]
    /// <summary>
    /// 规则作用范围，例如单项目、项目组或场景
    /// </summary>
    public string RuleScope { get; set; } = "ITEM";

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "GROUP_CODE", IsNullable = true)]
    /// <summary>
    /// 项目组编码，规则作用范围为项目组时用于匹配。
    /// </summary>
    public string? GroupCode { get; set; }

    [SugarColumn(ColumnName = "PRIORITY")]
    /// <summary>
    /// 规则优先级，数字越小越先参与匹配和动作排序
    /// </summary>
    public int Priority { get; set; } = 100;

    [SugarColumn(ColumnName = "CURRENT_VERSION")]
    /// <summary>
    /// 当前生效版本号，发布或回滚时由规则生命周期服务维护
    /// </summary>
    public int CurrentVersion { get; set; }

    [SugarColumn(ColumnName = "STATUS")]
    /// <summary>
    /// 当前记录状态，具体含义由所在表的状态机定义
    /// </summary>
    public string Status { get; set; } = "DRAFT";

    [SugarColumn(ColumnName = "IS_ENABLED")]
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; set; } = "Y";

    [SugarColumn(ColumnName = "EFFECTIVE_FROM", IsNullable = true)]
    /// <summary>
    /// 规则或版本的生效开始时间
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    [SugarColumn(ColumnName = "EFFECTIVE_TO", IsNullable = true)]
    /// <summary>
    /// 规则或版本的生效结束时间，空值表示未设失效时间
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    /// <summary>
    /// 规则备注。
    /// </summary>
    public string? Remark { get; set; }

    [SugarColumn(ColumnName = "CREATED_BY", IsNullable = true)]
    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    [SugarColumn(ColumnName = "CREATED_AT")]
    /// <summary>
    /// 记录创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    [SugarColumn(ColumnName = "UPDATED_BY", IsNullable = true)]
    /// <summary>
    /// 最后修改人
    /// </summary>
    public string? UpdatedBy { get; set; }

    [SugarColumn(ColumnName = "UPDATED_AT")]
    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
