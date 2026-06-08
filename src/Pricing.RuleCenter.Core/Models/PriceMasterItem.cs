namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// HIS 物价主数据项实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：FIN_COM_UNDRUGINFO（HIS 系统表，非 PR_ 前缀）
/// </para>
/// <para>
/// 在计价链路中的角色：规则中心只读取项目编码和当前收费链路需要的权威价格列，
/// 用于记录调用方传入基础单价与 HIS 物价的诊断差异。
/// </para>
/// <para>
/// 权威单价诊断：
/// <list type="bullet">
/// <item>基础单价仍由 HIS 负责带出，规则中心不因单价差异阻断计价流程</item>
/// <item>规则中心可读取 HIS 物价主数据并记录差异日志，服务联调和对账</item>
/// <item>如需恢复强校验，必须先接管可信价格形态、合同单位、患者事实和价格版本</item>
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
    /// 是规则匹配、价格诊断和限额累计的核心维度。
    /// "特殊项目"和"折价项目"是同一标识，通过该编码区分是否为特殊计价项目。
    /// </remarks>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>
    /// 三甲权威单价。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据表 FIN_COM_UNDRUGINFO.UNIT_PRICE。
    /// 普通患者默认按该价格做诊断比较。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// 禁止使用 double 或 float，始终使用 decimal。
    /// </remarks>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 儿童权威单价。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据表 FIN_COM_UNDRUGINFO.UNIT_PRICE1。
    /// 6 岁以下儿童患者默认按该价格做诊断比较。
    /// </remarks>
    public decimal? ChildPrice { get; set; }

    /// <summary>
    /// 围产权威单价。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据表 FIN_COM_UNDRUGINFO.WEICHAN_PRICE。
    /// 围产患者按该价格做诊断比较。
    /// </remarks>
    public decimal? PerinatalPrice { get; set; }
}
