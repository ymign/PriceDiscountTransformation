namespace Pricing.RuleCenter.Core.Aggregates.Catalog;

/// <summary>
/// 项目组明细实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_ITEM_GROUP_DETAIL
/// </para>
/// <para>
/// 在规则体系中的角色：记录项目组内包含哪些收费项目，以及每个项目在组内的角色。
/// 通过 GROUP_ID 关联 PR_ITEM_GROUP，通过 ITEM_CODE 关联 HIS 物价主数据。
/// </para>
/// <para>
/// 主子项目关系：
/// <list type="bullet">
/// <item>MAIN — 主项目，收费时触发子项目加收</item>
/// <item>CHILD — 子项目，随主项目一起收费，必须与主项目同 resultGroupNo 原子 commit/cancel</item>
/// </list>
/// </para>
/// </remarks>
public sealed class ItemGroupDetail
{
    /// <summary>
    /// 项目组明细主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 DETAIL_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条项目组明细记录。
    /// </remarks>
        public long DetailId { get; set; }

    /// <summary>
    /// 所属项目组主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_ITEM_GROUP.GROUP_ID，标识该明细属于哪个项目组。
    /// 一个项目组可以包含多条明细（多个项目）。
    /// </remarks>
        public long GroupId { get; set; }

    /// <summary>
    /// 项目编码。
    /// </summary>
    /// <remarks>
    /// 对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// 是规则匹配、价格校验和限额累计的核心维度。
    /// 同一项目组内同一项目编码不应重复。
    /// </remarks>
        public string ItemCode { get; set; } = string.Empty;

    /// <summary>
    /// 项目名称。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据，用于工作台展示和审计。
    /// 非计算字段，仅用于可读性。
    /// </remarks>
        public string? ItemName { get; set; }

    /// <summary>
    /// 项目在组内的角色类型。
    /// </summary>
    /// <remarks>
    /// 标识该项目在组内的角色：
    /// <list type="bullet">
    /// <item>MAIN — 主项目，收费时触发子项目加收</item>
    /// <item>CHILD — 子项目，随主项目一起收费</item>
    /// </list>
    /// 主子项目关系：子项加收必须与主项目同 resultGroupNo 原子 commit/cancel。
    /// 默认值为 "MAIN"。
    /// </remarks>
        public string RoleType { get; set; } = "MAIN";

    /// <summary>
    /// 排序号。
    /// </summary>
    /// <remarks>
    /// 数字越小越靠前展示。用于控制工作台项目列表的展示顺序。
    /// 默认值通常从 10 开始，间隔 10，便于插入。
    /// </remarks>
        public int SortNo { get; set; }

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该明细参与规则匹配，"N" 表示禁用。
    /// 禁用的明细不影响组内其他项目的匹配。
    /// 默认值为 "Y"。
    /// </remarks>
        public string IsEnabled { get; set; } = "Y";
}
