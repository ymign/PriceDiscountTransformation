using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Events;

namespace Pricing.RuleCenter.Core.Aggregates.Quota;

/// <summary>
/// 限额占用记录聚合根，封装限额占用状态转换和领域事件发布。
/// </summary>
/// <remarks>
/// <para>
/// 【聚合边界】
/// 本聚合根是额度聚合的唯一入口。聚合边界包含：
///   - LimitOccupy（本类）— 占用记录身份、状态机、数量/金额
///   - LimitLock — 并发锁，SELECT ... FOR UPDATE 模式
///   - LimitOccupyRangeQuery — 查询值对象，用于累计查询
/// </para>
/// <para>
/// 【对应 Oracle 表】
/// PR_LIMIT_OCCUPY。主键 OCCUPY_ID 由 SEQUENCE 生成。
/// </para>
/// <para>
/// 【在计价链路中的角色】
/// 该表是限额控制的事实表。三阶段确认模型中：
///   - confirm 阶段：先写 PENDING 保护占用
///   - commit 阶段：推进为 CONFIRMED
///   - cancel/expire/reverse 阶段：释放或终止占用
/// 累计限额时需要同时看 PENDING 和 CONFIRMED，防止并发突破上限。
/// </para>
/// <para>
/// 【并发控制】
/// 配套 PR_LIMIT_LOCK 表使用 SELECT ... FOR UPDATE 模式，串行化同一限额维度的 confirm。
/// TIME_WINDOW 类型必须锁定业务时间窗口覆盖的全部小时桶。
/// </para>
/// <para>
/// 【退费规则】
/// 当日退费按退费数量释放额度（OccupyQty 可为负数）；
/// 隔日退费重收后按重收当天重新做额度校验。
/// </para>
/// <para>
/// 【状态机流转】
/// <code>
/// PENDING（confirm 阶段写入）
///       │
///       ├──→ CONFIRMED（HIS commit 成功）
///       │           │
///       │           └──→ REVERSED（退费冲正）
///       │
///       ├──→ CANCELLED（HIS cancel 或 expire 清理）
///       │
///       └──→ EXPIRED（后台清理任务释放）
/// </code>
/// 状态转换必须通过 Confirm()/Cancel()/Reverse()/Expire() 方法执行，不允许外部直接赋值。
/// </para>
/// <para>
/// 【领域事件机制】
/// 聚合根在执行状态转换时收集领域事件到 DomainEvents 列表。
/// 当前实现状态：事件收集已完成，分发机制待后续迭代集成。
/// </para>
/// </remarks>
public sealed class LimitOccupy
{
    /// <summary>
    /// 领域事件收集列表，聚合根在状态转换时将事件追加到此列表。
    /// </summary>
    /// <remarks>
    /// 当前预留扩展点，待事件分发机制就绪后集成。
    /// </remarks>
    public List<IDomainEvent> DomainEvents { get; } = new();

    /// <summary>
    /// 限额占用主键，对应 Oracle 列 OCCUPY_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// </summary>
    /// <remarks>
    /// 全局唯一标识一条占用记录，用于冲正记录关联（OriginalOccupyId）。
    /// 不可修改，由数据库在 Insert 时自动填充。
    /// </remarks>
    public long OccupyId { get; set; }

    /// <summary>
    /// 关联的计价请求日志主键，对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID。
    /// </summary>
    /// <remarks>
    /// 用于串联请求、步骤、折价明细和限额占用四张表。
    /// 一条请求日志可以产生多条限额占用（如同时占用日限额和时间窗限额）。
    /// 不可为空，必须在 confirm 阶段由计价引擎填充。
    /// </remarks>
    public long RequestId { get; set; }

    /// <summary>
    /// 计价追踪号，全局唯一的追踪标识。
    /// </summary>
    /// <remarks>
    /// 用于跨表（REQUEST_LOG、TRACE_STEP、DISCOUNT_DETAIL、LIMIT_OCCUPY）
    /// 查看一次完整计价过程。
    /// 可为空，空值表示追踪号未生成（理论上不应出现）。
    /// </remarks>
    public string? TraceId { get; set; }

    /// <summary>
    /// 患者标识，来源为调用方传入的 patientId。
    /// </summary>
    /// <remarks>
    /// 限额累计和追溯查询的重要维度。日限额、时间窗限额均按患者维度累计。
    /// 不可为空，空值表示该请求不关联特定患者（理论上不应出现）。
    /// </remarks>
    public string PatientId { get; set; } = string.Empty;

    /// <summary>
    /// 项目编码，对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// </summary>
    /// <remarks>
    /// 是规则匹配、价格校验和限额累计的核心维度。
    /// 可为空，空值表示该占用记录不关联特定项目（如全局限额场景）。
    /// </remarks>
    public string? ItemCode { get; set; }

    /// <summary>
    /// 收费明细号。
    /// </summary>
    /// <remarks>
    /// confirm 阶段从调用方费用明细透传，用于在同一请求内存在相同项目多条收费明细时，
    /// reverse 能精确定位应该释放哪一条原占额。
    /// 空值表示调用方未提供明细号，此时只能退回到项目维度或结果组维度匹配。
    /// </remarks>
    public string? ChargeDetailNo { get; set; }

    /// <summary>
    /// 主子项目同组结果号。
    /// </summary>
    /// <remarks>
    /// 当一次计价会派生替换子项或加收子项时，限额占用与折价明细共享同一 ResultGroupNo。
    /// reverse 若只拿到主项目明细号，仍可通过该组号把同组占额一起释放，保证主子项目口径一致。
    /// </remarks>
    public string? ResultGroupNo { get; set; }

    /// <summary>
    /// 产生本次占用的规则主键，关联 PR_RULE_HEADER.RULE_ID。
    /// </summary>
    /// <remarks>
    /// 用于追溯该占用是由哪条规则触发的。
    /// 可为空，空值表示非规则触发的占用（如手工限额调整）。
    /// </remarks>
    public long? RuleId { get; set; }

    /// <summary>
    /// 产生本次占用的规则版本号，关联 PR_RULE_VERSION.VERSION_NO。
    /// </summary>
    /// <remarks>
    /// 用于锁定产生占用时的规则版本，便于后续版本回滚或变更时评估影响范围。
    /// 可为空，空值表示未记录规则版本（理论上不应出现）。
    /// </remarks>
    public int? RuleVersionNo { get; set; }

    /// <summary>
    /// 限额类型编码，标识本次占用属于哪类限额。
    /// </summary>
    /// <remarks>
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
    /// 限额维度键，由计价中心根据限额类型、患者、项目、业务时间等维度组合生成。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【用途】
    /// 用于 SELECT ... FOR UPDATE 锁行和累计查询。
    /// </para>
    /// <para>
    /// 【安全约束】
    /// 渠道不得传入，必须由计价中心生成，防止渠道绕过限额校验。
    /// </para>
    /// <para>
    /// 【格式示例】
    /// DAY_QTY|PATIENT_001|ITEM_1001|20260510
    /// </para>
    /// </remarks>
    public string LimitKey { get; set; } = string.Empty;

    /// <summary>
    /// 本次占用数量，正数表示收费占用，负数表示退费释放。
    /// </summary>
    /// <remarks>
    /// 累计时 SUM(OCCUPY_QTY) WHERE STATUS IN ('PENDING','CONFIRMED') 即为当前已占用总量。
    /// 精度 NUMBER(18,4)，与项目数量单位一致（如次、个、CM2）。
    /// </remarks>
    public decimal OccupyQty { get; set; }

    /// <summary>
    /// 本次占用金额，正数表示收费占用，负数表示退费释放。
    /// </summary>
    /// <remarks>
    /// 用于金额维度的限额校验（如单次收费金额上限、日累计金额上限）。
    /// 精度 NUMBER(18,4)，单位：元（人民币）。
    /// </remarks>
    public decimal OccupyAmt { get; set; }

    /// <summary>
    /// 占用类型，标识本次占用是收费还是退费。
    /// </summary>
    /// <remarks>
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
    /// 业务收费发生时间，来源为调用方传入的 businessChargeTime。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【业务时间优先】
    /// 不是计价中心的技术时间。业务时间优先于技术时间，用于：
    /// </para>
    /// <list type="bullet">
    /// <item>规则生效期判断（EFFECTIVE_FROM / EFFECTIVE_TO）</item>
    /// <item>日数量限额的当日划分（按业务日期而非系统日期）</item>
    /// <item>2 小时滑动窗口的起点计算（按业务收费时间向前查 2 小时）</item>
    /// </list>
    /// </remarks>
    public DateTime BusinessChargeTime { get; set; }

    /// <summary>
    /// 限额查询维度编码，用于按业务时间稳定累计。
    /// </summary>
    /// <remarks>
    /// 避免技术时间漂移导致累计偏差。
    /// 例如日限额维度编码格式为 "PATIENT_001|ITEM_1001|20260510"。
    /// 可为空。
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
    /// 占用记录状态，描述限额占用在状态机中的当前位置。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>PENDING — confirm 阶段写入的保护占用，尚未 commit</item>
    /// <item>CONFIRMED — HIS 调用 commit 后确认，占用正式生效</item>
    /// <item>CANCELLED — HIS 调用 cancel 后释放，或被 expire 清理任务释放</item>
    /// <item>REVERSED — HIS 调用 reverse 后冲正，生成对冲的 REVERSE 类型记录</item>
    /// <item>EXPIRED — 后台清理任务释放的超时占用</item>
    /// </list>
    /// 累计限额时只统计 PENDING 和 CONFIRMED 状态。
    /// 默认值为 "PENDING"。状态转换必须通过 Confirm()/Cancel()/Reverse()/Expire() 方法执行。
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
    /// 可为空，空值表示未设置过期时间（依赖全局默认过期策略）。
    /// </remarks>
    public DateTime? ExpireAt { get; set; }

    /// <summary>
    /// 将占用状态从待确认推进为已确认，仅允许从 PENDING 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// HIS 调用 commit 接口，应用服务校验请求状态和落账明细后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 PENDING 允许确认。已取消、已过期或已冲正的记录
    /// 再次确认会被拒绝，防止状态机出现非法跳跃。
    /// </para>
    /// </remarks>
    /// <param name="now">当前技术时间，由应用层统一传入。</param>
    /// <exception cref="InvalidOperationException">当前状态不是 PENDING 时抛出。</exception>
    public void Confirm(DateTime now)
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 PENDING 允许确认。已取消、已过期或已冲正的记录
        // 再次确认会被拒绝，防止状态机出现非法跳跃。
        if (Status != OccupyStatusCodes.Pending)
            throw new InvalidOperationException($"占用 {OccupyId} 当前状态为 {Status}，仅 PENDING 允许确认。");

        // ========== 第二阶段：推进状态 ==========
        Status = OccupyStatusCodes.Confirmed;
        ConfirmedAt = now;
    }

    /// <summary>
    /// 将占用状态推进为已取消，仅允许从 PENDING 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// HIS 调用 cancel 接口，应用服务校验请求状态后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 PENDING 允许取消。已确认的记录应走 reverse 流程，
    /// 不能直接 cancel，否则会出现"已落账但被取消"的资金口径断裂。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 PENDING 时抛出。</exception>
    public void Cancel()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 PENDING 允许取消。已确认的记录应走 reverse 流程，
        // 不能直接 cancel，否则会出现"已落账但被取消"的资金口径断裂。
        if (Status != OccupyStatusCodes.Pending)
            throw new InvalidOperationException($"占用 {OccupyId} 当前状态为 {Status}，仅 PENDING 允许取消。");

        // ========== 第二阶段：推进状态 ==========
        Status = OccupyStatusCodes.Cancelled;
    }

    /// <summary>
    /// 将占用状态推进为已冲正，仅允许从 CONFIRMED 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// HIS 调用 reverse 接口，应用服务校验请求状态和退费数量后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 CONFIRMED 允许冲正。PENDING 应走 cancel 流程，
    /// 不能直接 reverse，否则会出现"未落账但被冲正"的资金口径断裂。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 CONFIRMED 时抛出。</exception>
    public void Reverse()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 CONFIRMED 允许冲正。PENDING 应走 cancel 流程，
        // 不能直接 reverse，否则会出现"未落账但被冲正"的资金口径断裂。
        if (Status != OccupyStatusCodes.Confirmed)
            throw new InvalidOperationException($"占用 {OccupyId} 当前状态为 {Status}，仅 CONFIRMED 允许冲正。");

        // ========== 第二阶段：推进状态 ==========
        Status = OccupyStatusCodes.Reversed;
    }

    /// <summary>
    /// 将占用状态推进为已过期，仅允许从 PENDING 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// 后台清理任务（ExpireCleanupAppService）扫描长时间停留在 PENDING 的记录后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 PENDING 允许过期。已确认的记录不能被过期清理，
    /// 否则会出现"已落账但被清理"的资金口径断裂。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 PENDING 时抛出。</exception>
    public void Expire()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 PENDING 允许过期。已确认的记录不能被过期清理，
        // 否则会出现"已落账但被清理"的资金口径断裂。
        if (Status != OccupyStatusCodes.Pending)
            throw new InvalidOperationException($"占用 {OccupyId} 当前状态为 {Status}，仅 PENDING 允许过期。");

        // ========== 第二阶段：推进状态 ==========
        Status = OccupyStatusCodes.Expired;
    }

    /// <summary>
    /// 清空领域事件列表，通常在应用服务分发完事件后调用。
    /// </summary>
    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}
