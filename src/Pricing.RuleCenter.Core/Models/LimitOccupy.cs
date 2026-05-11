namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 限额占用记录实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_LIMIT_OCCUPY
/// </para>
/// <para>
/// 在计价链路中的角色：该表是限额控制的事实表。三阶段确认模型中，
/// confirm 阶段先写 PENDING 保护占用 → commit 后推进为 CONFIRMED → cancel/expire/reverse 再释放或终止占用。
/// 累计限额时需要同时看 PENDING 和 CONFIRMED，防止并发突破上限。
/// </para>
/// <para>
/// 并发控制：配套 PR_LIMIT_LOCK 表使用 SELECT ... FOR UPDATE 模式，串行化同一限额维度的 confirm。
/// TIME_WINDOW 类型必须锁定业务时间窗口覆盖的全部小时桶。
/// </para>
/// <para>
/// 退费规则：当日退费按退费数量释放额度（OccupyQty 可为负数）；
/// 隔日退费重收后按重收当天重新做额度校验。
/// </para>
/// </remarks>
public sealed class LimitOccupy
{
    /// <summary>
    /// 限额占用主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 OCCUPY_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条占用记录，用于冲正记录关联（OriginalOccupyId）。
    /// </remarks>
        public long OccupyId { get; set; }

    /// <summary>
    /// 关联的计价请求日志主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID，用于串联请求、步骤、折价明细和限额占用四张表。
    /// 一条请求日志可以产生多条限额占用（如同时占用日限额和时间窗限额）。
    /// </remarks>
        public long RequestId { get; set; }

    /// <summary>
    /// 计价追踪号。
    /// </summary>
    /// <remarks>
    /// 全局唯一的追踪标识，用于跨表（REQUEST_LOG、TRACE_STEP、DISCOUNT_DETAIL、LIMIT_OCCUPY）
    /// 查看一次完整计价过程。可为空，空值表示追踪号未生成。
    /// </remarks>
        public string? TraceId { get; set; }

    /// <summary>
    /// 患者标识。
    /// </summary>
    /// <remarks>
    /// 限额累计和追溯查询的重要维度。日限额、时间窗限额均按患者维度累计。
    /// 来源为调用方传入的 patientId。
    /// </remarks>
        public string PatientId { get; set; } = string.Empty;

    /// <summary>
    /// 项目编码。
    /// </summary>
    /// <remarks>
    /// 对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// 是规则匹配、价格校验和限额累计的核心维度。
    /// 该字段可为空，空值表示该占用记录不关联特定项目（如全局限额场景）。
    /// </remarks>
        public string? ItemCode { get; set; }

    /// <summary>
    /// 产生本次占用的规则主键。
    /// </summary>
    /// <remarks>
    /// 关联 PR_RULE_HEADER.RULE_ID，用于追溯该占用是由哪条规则触发的。
    /// 可为空，空值表示非规则触发的占用（如手工限额调整）。
    /// </remarks>
        public long? RuleId { get; set; }

    /// <summary>
    /// 产生本次占用的规则版本号。
    /// </summary>
    /// <remarks>
    /// 关联 PR_RULE_VERSION.VERSION_NO，用于锁定产生占用时的规则版本，
    /// 便于后续版本回滚或变更时评估影响范围。
    /// </remarks>
        public int? RuleVersionNo { get; set; }

    /// <summary>
    /// 限额类型编码。
    /// </summary>
    /// <remarks>
    /// 标识本次占用属于哪类限额，常见值：
    /// <list type="bullet">
    /// <item>DAY_QTY — 日数量限额，按患者+项目+当日累计</item>
    /// <item>TIME_WINDOW — 滑动窗口限额（如 2 小时窗），按患者+项目+时间窗累计</item>
    /// <item>SAME_OPERATION — 同手术限额，按患者+手术标识累计</item>
    /// <item>SAME_GROUP — 同组互斥，同组项目只执行优先级最高的一条</item>
    /// </list>
    /// 不可为空，必须由计价引擎在规则匹配阶段确定。
    /// </remarks>
        public string LimitType { get; set; } = string.Empty;

    /// <summary>
    /// 限额维度键。
    /// </summary>
    /// <remarks>
    /// 由计价中心根据限额类型、患者、项目、业务时间等维度组合生成的唯一键。
    /// 用于 SELECT ... FOR UPDATE 锁行和累计查询。
    /// 渠道不得传入，必须由计价中心生成，防止渠道绕过限额校验。
    /// 格式示例：DAY_QTY|PATIENT_001|ITEM_1001|20260510
    /// </remarks>
        public string LimitKey { get; set; } = string.Empty;

    /// <summary>
    /// 本次占用数量。
    /// </summary>
    /// <remarks>
    /// 正数表示收费占用，负数表示退费释放。
    /// 累计时 SUM(OCCUPY_QTY) WHERE STATUS IN ('PENDING','CONFIRMED') 即为当前已占用总量。
    /// 精度 NUMBER(18,4)，与项目数量单位一致（如次、个、CM2）。
    /// </remarks>
        public decimal OccupyQty { get; set; }

    /// <summary>
    /// 本次占用金额。
    /// </summary>
    /// <remarks>
    /// 正数表示收费占用，负数表示退费释放。
    /// 用于金额维度的限额校验（如单次收费金额上限、日累计金额上限）。
    /// 精度 NUMBER(18,4)，单位：元（人民币）。
    /// </remarks>
        public decimal OccupyAmt { get; set; }

    /// <summary>
    /// 占用类型。
    /// </summary>
    /// <remarks>
    /// 标识本次占用是收费还是退费：
    /// <list type="bullet">
    /// <item>CHARGE — 收费占用，confirm 阶段生成</item>
    /// <item>REVERSE — 退费释放，reverse 阶段生成，OccupyQty 和 OccupyAmt 为负数</item>
    /// </list>
    /// 默认值为 "CHARGE"。
    /// </remarks>
        public string OccupyType { get; set; } = "CHARGE";

    /// <summary>
    /// 冲正记录关联的原占用记录主键。
    /// </summary>
    /// <remarks>
    /// 当 OccupyType = REVERSE 时，该字段指向被冲正的原 CHARGE 类型占用记录的 OccupyId。
    /// 用于双向追溯：从原占用找到其冲正记录，或从冲正记录找到原始占用。
    /// 空值表示该记录不是冲正记录。
    /// </remarks>
        public long? OriginalOccupyId { get; set; }

    /// <summary>
    /// 业务收费发生时间。
    /// </summary>
    /// <remarks>
    /// 来源为调用方传入的 businessChargeTime，不是计价中心的技术时间。
    /// 业务时间优先于技术时间，用于：
    /// 1. 规则生效期判断（EFFECTIVE_FROM / EFFECTIVE_TO）
    /// 2. 日数量限额的当日划分（按业务日期而非系统日期）
    /// 3. 2 小时滑动窗口的起点计算（按业务收费时间向前查 2 小时）
    /// </remarks>
        public DateTime BusinessChargeTime { get; set; }

    /// <summary>
    /// 限额查询维度编码。
    /// </summary>
    /// <remarks>
    /// 用于按业务时间稳定累计，避免技术时间漂移导致累计偏差。
    /// 例如日限额维度编码格式为 "PATIENT_001|ITEM_1001|20260510"。
    /// </remarks>
        public string? LimitDimensionCode { get; set; }

    /// <summary>
    /// 多部位或多片段计价时的片段序号。
    /// </summary>
    /// <remarks>
    /// 当一个收费项目包含多个部位或片段时（如多肿物切除），每个部位独立计价和占额。
    /// PartSeq 从 1 开始递增，用于区分同一请求内的不同部位占用。
    /// 空值表示该项目不涉及多部位拆分。
    /// </remarks>
        public int? PartSeq { get; set; }

    /// <summary>
    /// 占用记录状态。
    /// </summary>
    /// <remarks>
    /// 限额占用的状态机流转：
    /// <list type="bullet">
    /// <item>PENDING — confirm 阶段写入的保护占用，尚未 commit</item>
    /// <item>CONFIRMED — HIS 调用 commit 后确认，占用正式生效</item>
    /// <item>CANCELLED — HIS 调用 cancel 后释放，或被 expire 清理任务释放</item>
    /// <item>REVERSED — HIS 调用 reverse 后冲正，生成对冲的 REVERSE 类型记录</item>
    /// </list>
    /// 累计限额时只统计 PENDING 和 CONFIRMED 状态。
    /// 默认值为 "PENDING"。
    /// </remarks>
        public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 计价中心创建占额记录的技术时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在 confirm 阶段自动填充，用于审计和过期清理。
    /// 不得用该时间替代 BusinessChargeTime 做业务判断。
    /// </remarks>
        public DateTime OccupiedAt { get; set; }

    /// <summary>
    /// HIS commit 成功后的确认时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在 commit 阶段填充。
    /// 空值表示尚未 commit（仍为 PENDING 状态）。
    /// 用于审计和对账，确认 HIS 何时完成落账。
    /// </remarks>
        public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// confirm 结果过期时间。
    /// </summary>
    /// <remarks>
    /// confirm 阶段写入，超过该时间后该 PENDING 占用不能再 commit，
    /// 后台挂起清理任务（expire）会将其状态推进为 CANCELLED 并释放额度。
    /// 防止 HIS 超时未调用 commit/cancel 导致额度被永久占用。
    /// </remarks>
        public DateTime? ExpireAt { get; set; }
}
