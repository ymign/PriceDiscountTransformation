namespace Pricing.RuleCenter.Core.Services;

/// <summary>
/// 限额维度键生成领域服务，统一限额键生成逻辑。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 限额键（LimitKey）由限额类型、患者、项目、业务时间等维度组合生成，
/// 用于 SELECT ... FOR UPDATE 锁行和累计查询。
/// 渠道不得传入，必须由计价中心生成，防止渠道绕过限额校验。
/// </para>
/// <para>
/// 【安全约束】
/// 限额键是并发控制的核心，必须由计价中心统一生成：
///   - 渠道不得传入 LimitKey，防止绕过限额校验
///   - 同一维度必须生成相同键，确保锁行命中
///   - 键格式变更需评估历史数据兼容性
/// </para>
/// <para>
/// 【键格式说明】
/// 使用管道符（|）分隔各维度，格式为：
///   LIMIT_TYPE|PATIENT_ID|ITEM_CODE|TIME_DIMENSION
/// 例如：
///   - DAY_QTY|P001|ITEM_01|20260510
///   - TIME_WINDOW|P001|ITEM_01|2026051014
///   - SAME_OP|P001|OP_001
/// </para>
/// <para>
/// 【并发控制】
/// 生成的键用于 PR_LIMIT_LOCK 表的 SELECT ... FOR UPDATE 锁行，
/// 确保同一限额维度的 confirm 请求串行化执行，防止并发突破上限。
/// </para>
/// <para>
/// 【静态类设计】
/// 本服务为静态类，因为：
///   - 无状态依赖
///   - 纯函数式设计（输入 → 输出，无副作用）
///   - 可全局调用，无需 DI 注册
/// </para>
/// </remarks>
public static class LimitKeyGenerator
{
    /// <summary>
    /// 生成日数量限额键。
    /// </summary>
    /// <param name="patientId">患者标识，不可为空。</param>
    /// <param name="itemCode">项目编码，不可为空。</param>
    /// <param name="businessDate">业务收费发生日期，仅取日期部分。</param>
    /// <returns>日数量限额键，格式：DAY_QTY|{patientId}|{itemCode}|{yyyyMMdd}</returns>
    /// <remarks>
    /// <para>
    /// 【键格式】
    /// DAY_QTY|{patientId}|{itemCode}|{yyyyMMdd}
    /// 例如：DAY_QTY|P001|ITEM_01|20260510
    /// </para>
    /// <para>
    /// 【业务时间优先】
    /// 参数必须使用业务收费发生时间（BusinessChargeTime），
    /// 不能用 DateTime.Now 或 RequestAt 替代。
    /// 否则日限额累计会受服务器时钟漂移影响。
    /// </para>
    /// <para>
    /// 【累计维度】
    /// 同一患者、同一项目、同一业务日期的所有占用记录共享同一限额键。
    /// </para>
    /// </remarks>
    public static string GenerateDailyKey(string patientId, string itemCode, DateTime businessDate)
    {
        return $"DAY_QTY|{patientId}|{itemCode}|{businessDate:yyyyMMdd}";
    }

    /// <summary>
    /// 生成时间窗限额键（滑动窗口）。
    /// </summary>
    /// <param name="patientId">患者标识，不可为空。</param>
    /// <param name="itemCode">项目编码，不可为空。</param>
    /// <param name="businessTime">业务收费发生时间，用于计算时间桶。</param>
    /// <param name="windowHours">时间窗口小时数，默认为 2。</param>
    /// <returns>时间窗限额键，格式：TIME_WINDOW|{patientId}|{itemCode}|{yyyyMMddHH}</returns>
    /// <remarks>
    /// <para>
    /// 【键格式】
    /// TIME_WINDOW|{patientId}|{itemCode}|{yyyyMMddHH}
    /// 例如：TIME_WINDOW|P001|ITEM_01|2026051014
    /// </para>
    /// <para>
    /// 【时间桶计算】
    /// 将业务时间向下取整到时间窗口的起始小时。
    /// 例如 windowHours = 2 时：
    ///   - 14:30 → 14:00（桶）
    ///   - 15:59 → 14:00（桶）
    ///   - 16:00 → 16:00（新桶）
    /// </para>
    /// <para>
    /// 【业务时间优先】
    /// 参数必须使用业务收费发生时间（BusinessChargeTime），
    /// 不能用 DateTime.Now 或 RequestAt 替代。
    /// </para>
    /// <para>
    /// 【滑动窗口语义】
    /// 退费时按收费时间（包含重收时间）向前查 2 小时，
    /// 而非按退费时间向后查。
    /// </para>
    /// </remarks>
    public static string GenerateTimeWindowKey(string patientId, string itemCode, DateTime businessTime, int windowHours = 2)
    {
        // ========== 计算时间桶起点 ==========
        // 将业务时间向下取整到时间窗口的起始小时。
        // 例如 windowHours = 2 时：14:30 → 14:00，15:59 → 14:00，16:00 → 16:00
        var bucket = new DateTime(businessTime.Year, businessTime.Month, businessTime.Day, businessTime.Hour / windowHours * windowHours, 0, 0);
        return $"TIME_WINDOW|{patientId}|{itemCode}|{bucket:yyyyMMddHH}";
    }

    /// <summary>
    /// 生成同手术限额键。
    /// </summary>
    /// <param name="patientId">患者标识，不可为空。</param>
    /// <param name="operationId">手术标识，来源为 HIS 传入的手术编号。</param>
    /// <returns>同手术限额键，格式：SAME_OP|{patientId}|{operationId}</returns>
    /// <remarks>
    /// <para>
    /// 【键格式】
    /// SAME_OP|{patientId}|{operationId}
    /// 例如：SAME_OP|P001|OP_001
    /// </para>
    /// <para>
    /// 【业务语义】
    /// 同一患者、同一手术的所有收费项目共享同一手术封顶限额。
    /// 手术标识来源为 HIS 传入的手术编号，需确认 HIS 是否能稳定提供。
    /// </para>
    /// <para>
    /// 【待确认问题】
    /// "同手术""同孕次"标识在现有系统中的来源需确认。
    /// </para>
    /// </remarks>
    public static string GenerateSameOperationKey(string patientId, string operationId)
    {
        return $"SAME_OP|{patientId}|{operationId}";
    }
}
