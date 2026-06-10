namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 子项加收计价结果。
/// </summary>
/// <remarks>
/// 子项是主项目命中规则后自动附加的收费项目。子项与主项目共享同一 `resultGroupNo`，
/// 保证 commit/cancel 的原子性。
/// </remarks>
public sealed class ChildPricingResult
{
    /// <summary>
    /// 子项目编码，对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// </summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>
    /// 子项目名称，用于展示和审计。
    /// </summary>
    public string? ItemName { get; set; }

    /// <summary>
    /// 子项目收费数量。
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 子项目单价，来源为权威物价主数据。
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 子项目应收金额 = 单价 × 数量，中间计算保留全精度。
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 是否与主项目共享限额。
    /// </summary>
    public bool ShareParentLimit { get; set; }

    /// <summary>
    /// 关联的主项目编码，用于追溯和审计。
    /// </summary>
    public string ParentItemCode { get; set; } = string.Empty;
}
