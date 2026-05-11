namespace Pricing.RuleCenter.Core.Services;

/// <summary>
/// 限额键（LIMIT_KEY）统一生成器，负责所有限制类型的维度键生成。
/// </summary>
/// <remarks>
/// <para>
/// 【核心作用】
/// LIMIT_KEY 是限额控制的核心维度键，由计价中心在确认（confirm）时生成，渠道不得传入。
/// 每种限制类型使用不同的前缀和维度组合，确保同一维度的限额累计能正确聚合，
/// 同时避免不同维度之间产生碰撞。
/// </para>
/// <para>
/// 【格式规范】
/// <list type="bullet">
/// <item>前缀（2 字符）+ 冒号分隔各维度段，段内不允许出现冒号</item>
/// <item>日期统一使用 yyyyMMdd 格式，小时使用 yyyyMMddHH 格式</item>
/// <item>patientId / itemCode 等业务键直接拼接，不做 URL 编码</item>
/// </list>
/// </para>
/// <para>
/// 【并发安全】
/// LIMIT_KEY 生成后用于 PR_LIMIT_LOCK 表的 SELECT ... FOR UPDATE 锁定，
/// 因此格式必须在业务时间窗口内覆盖全部需要锁定的小时桶（TimeWindowKey）。
/// </para>
/// <para>
/// 【NULL ≠ 0 原则】
/// 当某限制维度的输入参数为 null 时，表示该维度不参与校验，
/// 调用方应在调用本生成器之前判断是否需要生成对应 LIMIT_KEY。
/// </para>
/// </remarks>
public static class LimitKeyGenerator
{
    /// <summary>
    /// 生成日数量限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>DQ:{patientId}:{itemCode}:{yyyyMMdd}</c>
    /// 【用途】控制同一患者同一天对同一项目的累计收费数量，防止重复收费。
    /// 【聚合语义】同一天（按业务日期）内的所有收费动作共享同一个 LIMIT_KEY，
    /// 退费时按退费数量释放对应额度。当日退费释放当日数量，隔日退费重收后按重收当天重新校验。
    /// 【示例】patientId="P001", itemCode="ITEM_SKIN_01", businessDate=2026-05-10
    /// → <c>DQ:P001:ITEM_SKIN_01:20260510</c>
    /// </remarks>
    /// <param name="patientId">患者唯一标识，通常为住院号或门诊号。</param>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <param name="businessDate">业务收费发生日期（取日期部分），非技术时间。</param>
    /// <returns>日数量限额键字符串。</returns>
    public static string DayQtyKey(string patientId, string itemCode, DateTime businessDate)
    {
        return $"DQ:{patientId}:{itemCode}:{businessDate:yyyyMMdd}";
    }

    /// <summary>
    /// 生成日金额限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>DA:{patientId}:{itemCode}:{yyyyMMdd}</c>
    /// 【用途】控制同一患者同一天对同一项目的累计收费金额上限，属于金额维度的全院累计。
    /// 【聚合语义】与 DayQtyKey 类似，但累计对象是金额而非数量。
    /// 退费时按退费金额释放对应额度。
    /// 【示例】patientId="P001", itemCode="ITEM_SKIN_01", businessDate=2026-05-10
    /// → <c>DA:P001:ITEM_SKIN_01:20260510</c>
    /// </remarks>
    /// <param name="patientId">患者唯一标识。</param>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <param name="businessDate">业务收费发生日期（取日期部分），非技术时间。</param>
    /// <returns>日金额限额键字符串。</returns>
    public static string DayAmountKey(string patientId, string itemCode, DateTime businessDate)
    {
        return $"DA:{patientId}:{itemCode}:{businessDate:yyyyMMdd}";
    }

    /// <summary>
    /// 生成单次限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>OQ:{sourceSystem}:{businessRequestNo}:{itemCode}</c>
    /// 【用途】控制单次收费动作中某项目的收费数量上限，防止一次收费动作中重复收取同一项目。
    /// 【单次口径】"单次"指单次收费动作（如门诊收费保存、手术/医技划价、终端确认、护士过医嘱），
    /// 而非单条收费明细。维度应使用 sourceSystem + businessRequestNo + itemCode，
    /// 不能按单条收费明细号替代。
    /// 【幂等性关联】与 confirm 接口的幂等键（sourceSystem + businessRequestNo + callType）共享
    /// businessRequestNo，确保超时重试复用同一业务号时不会重复占用额度。
    /// 【示例】sourceSystem="HIS", businessRequestNo="REQ20260510001", itemCode="ITEM_SKIN_01"
    /// → <c>OQ:HIS:REQ20260510001:ITEM_SKIN_01</c>
    /// </remarks>
    /// <param name="sourceSystem">来源系统标识，如 HIS、SELF_SERVICE、WECHAT。</param>
    /// <param name="businessRequestNo">业务请求号，由渠道生成的唯一收费动作标识。</param>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>单次限额键字符串。</returns>
    public static string OnceQtyKey(string sourceSystem, string businessRequestNo, string itemCode)
    {
        return $"OQ:{sourceSystem}:{businessRequestNo}:{itemCode}";
    }

    /// <summary>
    /// 生成时间窗限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>TW:{patientId}:{itemCode}:{yyyyMMddHH}</c>
    /// 【用途】控制同一患者在指定时间窗（如 2 小时）内对同一项目的累计收费数量。
    /// 时间窗按业务收费时间向前回溯，精确到小时桶。
    /// 【锁定语义】并发额度控制使用 PR_LIMIT_LOCK + SELECT FOR UPDATE，
    /// TIME_WINDOW 必须锁定业务时间窗口覆盖的全部小时桶，防止多渠道同时突破限额。
    /// 【业务时间优先】必须使用业务收费发生时间（BusinessChargeTime），
    /// 不得使用技术占用时间替代。
    /// 【示例】patientId="P001", itemCode="ITEM_SKIN_01", businessHour=2026-05-10 14:00
    /// → <c>TW:P001:ITEM_SKIN_01:2026051014</c>
    /// </remarks>
    /// <param name="patientId">患者唯一标识。</param>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <param name="businessHour">业务收费时间（取到小时），用于确定小时桶。</param>
    /// <returns>时间窗限额键字符串。</returns>
    public static string TimeWindowKey(string patientId, string itemCode, DateTime businessHour)
    {
        return $"TW:{patientId}:{itemCode}:{businessHour:yyyyMMddHH}";
    }

    /// <summary>
    /// 生成同手术限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>SO:{operationId}:{groupCode}</c>
    /// 【用途】控制同一手术中同组项目的累计收费数量或金额，实现"同手术封顶"逻辑。
    /// 【业务场景】同一手术中某些项目属于互斥组或有封顶限制，通过 operationId 关联到具体手术。
    /// 【注意】operationId 的来源（HIS 手术申请号或医嘱号）需与 HIS 侧确认。
    /// 【示例】operationId="OP20260510001", groupCode="GRP_SKIN_MULTI"
    /// → <c>SO:OP20260510001:GRP_SKIN_MULTI</c>
    /// </remarks>
    /// <param name="operationId">手术唯一标识，通常来自 HIS 手术申请号。</param>
    /// <param name="groupCode">项目组编码，对应 PR_ITEM_GROUP.GROUP_CODE。</param>
    /// <returns>同手术限额键字符串。</returns>
    public static string SameOperationKey(string operationId, string groupCode)
    {
        return $"SO:{operationId}:{groupCode}".ToUpperInvariant();
    }

    /// <summary>
    /// 生成同胎次限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>PG:{pregnancyId}:{groupCode}</c>
    /// 【用途】控制同一孕次（胎次）中同组项目的累计收费数量或金额。
    /// 【业务场景】产科项目按孕次累计，同一孕次内某些检查或治疗有次数或金额上限。
    /// 【注意】pregnancyId 的来源（HIS 孕次标识或产检档案号）需与 HIS 侧确认。
    /// 【示例】pregnancyId="PG2026001", groupCode="GRP_OBSTETRICS"
    /// → <c>PG:PG2026001:GRP_OBSTETRICS</c>
    /// </remarks>
    /// <param name="pregnancyId">孕次唯一标识，通常来自 HIS 产检档案。</param>
    /// <param name="groupCode">项目组编码，对应 PR_ITEM_GROUP.GROUP_CODE。</param>
    /// <returns>同胎次限额键字符串。</returns>
    public static string SamePregnancyKey(string pregnancyId, string groupCode)
    {
        return $"PG:{pregnancyId}:{groupCode}";
    }

    /// <summary>
    /// 生成同组互斥限额键。
    /// </summary>
    /// <remarks>
    /// 【格式】<c>SG:{groupCode}:{yyyyMMdd}</c>
    /// 【用途】控制同一患者在同一天内同一互斥组只能收取一次（或有限次数），
    /// 实现"同组互斥"逻辑。互斥组内的项目共享该限额键。
    /// 【业务场景】某些项目属于同一互斥组（如皮肤科多肿物只收一次），同一天内只允许收费一次。
    /// 【聚合语义】以 groupCode + 业务日期为维度，同组内所有项目共享该键。
    /// 【示例】groupCode="GRP_SKIN_EXCLUSIVE", businessDate=2026-05-10
    /// → <c>SG:GRP_SKIN_EXCLUSIVE:20260510</c>
    /// </remarks>
    /// <param name="groupCode">互斥组编码，对应 PR_ITEM_GROUP.GROUP_CODE。</param>
    /// <param name="businessDate">业务收费发生日期（取日期部分），非技术时间。</param>
    /// <returns>同组互斥限额键字符串。</returns>
    public static string SameGroupKey(string groupCode, DateTime businessDate)
    {
        return $"SG:{groupCode}:{businessDate:yyyyMMdd}";
    }
}
