namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 项目组实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_ITEM_GROUP
/// </para>
/// <para>
/// 在规则体系中的角色：项目组将多个收费项目组织在一起，便于规则按组匹配和管理。
/// 规则头（PR_RULE_HEADER）的 RULE_SCOPE = "GROUP" 时，通过 GROUP_CODE 匹配项目组。
/// </para>
/// <para>
/// 项目组类型说明：
/// <list type="bullet">
/// <item>MAIN_CHILD — 主子项目组，主项目收费时自动加收子项目</item>
/// <item>MUTUAL_EXCLUSIVE — 同类互斥项目组，同组项目只执行优先级最高的一条</item>
/// <item>SAME_OPERATION — 同手术项目组，同一手术下的多个项目共享限额</item>
/// </list>
/// </para>
/// <para>
/// 多规则边界约束：同一项目允许按不同部位维护不同换算规则；
/// 不允许同一项目维护不同折价公式或不同折价额度规则。
/// </para>
/// </remarks>
public sealed class ItemGroup
{
    /// <summary>
    /// 项目组主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 GROUP_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条项目组记录。
    /// </remarks>
        public long GroupId { get; set; }

    /// <summary>
    /// 项目组编码。
    /// </summary>
    /// <remarks>
    /// 全局唯一的业务键，规则作用范围为项目组时通过该编码匹配。
    /// 规则头的 GROUP_CODE 字段引用该编码。
    /// 例如：GROUP_001、MUTUAL_EXCLUSIVE_SKIN。
    /// </remarks>
        public string GroupCode { get; set; } = string.Empty;

    /// <summary>
    /// 项目组名称。
    /// </summary>
    /// <remarks>
    /// 面向配置人员展示的中文名称。
    /// 例如：编码 "GROUP_001" 对应名称 "皮肤科治疗项目组"。
    /// </remarks>
        public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 项目组类型编码。
    /// </summary>
    /// <remarks>
    /// 标识该组的业务用途，决定计价引擎如何处理组内项目：
    /// <list type="bullet">
    /// <item>MAIN_CHILD — 主子项目组，主项目收费时自动加收子项目</item>
    /// <item>MUTUAL_EXCLUSIVE — 同类互斥项目组</item>
    /// <item>SAME_OPERATION — 同手术项目组</item>
    /// </list>
    /// 该值应与 PR_DICT 中的 GROUP_TYPE 字典对应。
    /// </remarks>
        public string GroupType { get; set; } = string.Empty;

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该项目组参与规则匹配，"N" 表示禁用。
    /// 禁用的项目组不出现在工作台下拉列表中，但已引用的规则不受影响。
    /// 默认值为 "Y"。
    /// </remarks>
        public string IsEnabled { get; set; } = "Y";

    /// <summary>
    /// 项目组备注。
    /// </summary>
    /// <remarks>
    /// 配置人员填写的补充说明，用于解释项目组的用途或特殊含义。
    /// </remarks>
        public string? Remark { get; set; }

    /// <summary>
    /// 创建人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计。
    /// </remarks>
        public string? CreatedBy { get; set; }

    /// <summary>
    /// 记录创建时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和排序。
    /// </remarks>
        public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后修改人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计。
    /// </remarks>
        public string? UpdatedBy { get; set; }

    /// <summary>
    /// 记录最后更新时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在每次更新时自动填充，用于乐观锁和审计。
    /// </remarks>
        public DateTime UpdatedAt { get; set; }
}
