namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 超限替换的加收项目信息。
/// </summary>
/// <remarks>
/// 当收费数量超出限额且配置为“替换为加收项目”时，超出部分按加收项目单价重新计算。
/// 该对象记录加收项目的编码、名称、数量和金额，供追溯展示和折价明细使用。
/// </remarks>
public sealed class ReplaceChildResult
{
    /// <summary>
    /// 加收项目编码，对应 HIS 物价主数据表中的折价项目编码。
    /// </summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>
    /// 加收项目名称，用于追溯页面展示。
    /// </summary>
    public string? ItemName { get; set; }

    /// <summary>
    /// 加收数量，等于超出限额的数量。
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 加收项目单价，来源为规则配置或权威物价主数据。
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 加收金额 = 数量 × 单价。
    /// </summary>
    public decimal Amount { get; set; }
}
