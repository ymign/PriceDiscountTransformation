using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 字典项实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_DICT
/// </para>
/// <para>
/// 在系统中的角色：为规则维护中心提供可配置的枚举值，避免硬编码。
/// 字典类型包括：计价类型、收费场景、身体部位、单位、公式类型、限额类型等。
/// 工作台内置字典维护功能，由配置人员管理。
/// </para>
/// <para>
/// 字典约束：
/// <list type="bullet">
/// <item>同一 DICT_TYPE 下 DICT_CODE 必须唯一</item>
/// <item>支持级联关系（通过 PARENT_CODE 实现二级字典）</item>
/// <item>禁用的字典项（IS_ENABLED = N）不出现在下拉列表中，但已引用的历史数据不受影响</item>
/// </list>
/// </para>
/// </remarks>
[SugarTable("PR_DICT")]
public sealed class Dict
{
    /// <summary>
    /// 字典项主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 DICT_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条字典项记录。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DICT_ID")]
    public long DictId { get; set; }

    /// <summary>
    /// 字典类型编码。
    /// </summary>
    /// <remarks>
    /// 对字典项进行分类，同一类型下的字典项构成一个枚举列表。
    /// 常见类型：
    /// <list type="bullet">
    /// <item>CHARGE_SCENE — 收费场景（门诊、住院、手术等）</item>
    /// <item>BODY_PART — 身体部位</item>
    /// <item>INPUT_UNIT — 输入单位（次、个、CM2 等）</item>
    /// <item>FORMULA_TYPE — 公式类型</item>
    /// <item>LIMIT_TYPE — 限额类型</item>
    /// <item>RULE_CATEGORY — 规则类别</item>
    /// <item>ACTION_TYPE — 动作类型</item>
    /// <item>CONDITION_TYPE — 条件类型</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    [SugarColumn(ColumnName = "DICT_TYPE")]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典项编码。
    /// </summary>
    /// <remarks>
    /// 同一 DICT_TYPE 下必须唯一，作为规则配置中的引用键。
    /// 例如 DICT_TYPE = "CHARGE_SCENE" 时，DICT_CODE 可以是 "OUTPATIENT"、"INPATIENT"。
    /// 该编码在规则条件、动作参数中被引用，修改编码需评估影响范围。
    /// </remarks>
    [SugarColumn(ColumnName = "DICT_CODE")]
    public string DictCode { get; set; } = string.Empty;

    /// <summary>
    /// 字典项显示名称。
    /// </summary>
    /// <remarks>
    /// 面向配置人员和终端用户展示的中文名称。
    /// 例如 DICT_CODE = "OUTPATIENT" 对应 DICT_NAME = "门诊"。
    /// </remarks>
    [SugarColumn(ColumnName = "DICT_NAME")]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 父级字典编码。
    /// </summary>
    /// <remarks>
    /// 用于级联或分组展示，实现二级字典结构。
    /// 例如：身体部位字典中，"头部" 下有 "左眼"、"右眼" 等子项。
    /// 空值表示该字典项为顶级项，没有父级。
    /// </remarks>
    [SugarColumn(ColumnName = "PARENT_CODE", IsNullable = true)]
    public string? ParentCode { get; set; }

    /// <summary>
    /// 排序号。
    /// </summary>
    /// <remarks>
    /// 数字越小越靠前展示。用于控制下拉列表和页面展示顺序。
    /// 默认值通常从 10 开始，间隔 10，便于插入。
    /// </remarks>
    [SugarColumn(ColumnName = "SORT_NO")]
    public int SortNo { get; set; }

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该字典项在下拉列表中可见，"N" 表示禁用。
    /// 禁用的字典项不出现在下拉列表中，但已引用的历史数据不受影响。
    /// 默认值为 "Y"。
    /// </remarks>
    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

    /// <summary>
    /// 字典项备注。
    /// </summary>
    /// <remarks>
    /// 配置人员填写的补充说明，用于解释字典项的用途或特殊含义。
    /// </remarks>
    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }
}
