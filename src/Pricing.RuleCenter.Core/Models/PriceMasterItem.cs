using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("FIN_COM_UNDRUGINFO")]
/// <summary>
/// HIS 物价主数据项，对应 FIN_COM_UNDRUGINFO。
/// </summary>
/// <remarks>
/// 规则中心只读取项目编码和单价，用于 confirm 阶段校验调用方价格是否与权威物价一致。
/// </remarks>
public sealed class PriceMasterItem
{
    [SugarColumn(ColumnName = "ITEM_CODE")]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string ItemCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "UNIT_PRICE")]
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验
    /// </summary>
    public decimal UnitPrice { get; set; }
}
