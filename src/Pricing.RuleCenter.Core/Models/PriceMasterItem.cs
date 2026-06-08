namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// HIS 物价主数据项实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：FIN_COM_UNDRUGINFO（HIS 系统表，非 PR_ 前缀）
/// </para>
/// <para>
/// 在计价链路中的角色：规则中心只读取项目编码和单价，用于 confirm 阶段校验
/// 调用方传入的价格是否与权威物价一致。
/// </para>
/// <para>
/// 权威单价约束（资金安全硬约束）：
/// <list type="bullet">
/// <item>confirm 阶段计价中心不得直接信任渠道传入的 unitPrice</item>
/// <item>必须由计价中心读取权威单价或强校验</item>
/// <item>不一致时返回 PRICE_MISMATCH 错误码，拒绝本次计价</item>
/// </list>
/// </para>
/// <para>
/// 待确认问题：权威物价单价从 HIS 哪张表或同步表读取，价格版本如何追溯。
/// </para>
/// </remarks>
public sealed class PriceMasterItem
{
    /// <summary>
    /// 项目编码。
    /// </summary>
    /// <remarks>
    /// 对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// 是规则匹配、价格校验和限额累计的核心维度。
    /// "特殊项目"和"折价项目"是同一标识，通过该编码区分是否为特殊计价项目。
    /// </remarks>
        public string ItemCode { get; set; } = string.Empty;

    /// <summary>
    /// 项目权威单价。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据表 FIN_COM_UNDRUGINFO.UNIT_PRICE。
    /// confirm 阶段计价中心必须读取该单价与调用方传入的单价进行强校验。
    /// 不一致时返回 PRICE_MISMATCH 错误码，拒绝本次计价。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// 禁止使用 double 或 float，始终使用 decimal。
    /// </remarks>
        public decimal UnitPrice { get; set; }
}
