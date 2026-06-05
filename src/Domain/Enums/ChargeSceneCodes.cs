namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// 收费场景编码常量。
/// </summary>
/// <remarks>
/// 全院累计口径按收费动作入口统计，当前确认纳入门诊收费保存、手术/医技划价、终端确认、
/// 护士过医嘱或分解医嘱四类入口。规则条件中的 chargeScene 应使用这些稳定编码。
/// </remarks>
public static class ChargeSceneCodes
{
    /// <summary>
    /// 门诊收费保存。
    /// </summary>
    public const string OutpatientCharge = "OUTPATIENT_CHARGE";

    /// <summary>
    /// 手术或医技划价。
    /// </summary>
    public const string SurgeryMedicalTechPricing = "SURGERY_MEDICAL_TECH_PRICING";

    /// <summary>
    /// 终端确认。
    /// </summary>
    public const string TerminalConfirm = "TERMINAL_CONFIRM";

    /// <summary>
    /// 护士过医嘱或分解医嘱。
    /// </summary>
    public const string NurseOrderExecution = "NURSE_ORDER_EXECUTION";

    /// <summary>
    /// 全院累计纳入统计的收费场景集合。
    /// </summary>
    public static readonly IReadOnlySet<string> HospitalWideLimitScenes =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            OutpatientCharge,
            SurgeryMedicalTechPricing,
            TerminalConfirm,
            NurseOrderExecution
        };
}
