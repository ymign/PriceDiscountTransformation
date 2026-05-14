using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Events;

namespace Pricing.RuleCenter.Core.Aggregates.Charging;

/// <summary>
/// 计价请求聚合根，封装计价请求生命周期状态转换和领域事件发布。
/// </summary>
/// <remarks>
/// <para>
/// 【聚合边界】
/// 本聚合根是收费聚合的唯一入口。聚合边界包含：
///   - ChargeRequest（本类）— 请求身份、幂等键、状态机
///   - ChargeDiscountDetail — 折价明细，记录最终折价金额和规则关联
///   - ChargeTraceStep — 计算步骤日志，记录规则匹配和动作执行过程
///   - ChargeReverseLog — 冲正日志，记录退费和撤销流水
///   - LimitOccupy — 限额占用，记录额度累计和状态推进
/// </para>
/// <para>
/// 【对应 Oracle 表】
/// PR_CHARGE_REQUEST_LOG。主键 REQUEST_ID 由 SEQUENCE 生成。
/// </para>
/// <para>
/// 【在计价链路中的角色】
/// 这是计价链路的主表，每次试算（simulate）、确认（confirm）、
/// 提交（commit）、取消（cancel）或冲正（reverse）都会形成一条请求日志。
/// 幂等判断、响应重放、追踪查询和过期清理都以该表为入口。
/// </para>
/// <para>
/// 【幂等性约束】
/// 幂等键为 sourceSystem + businessRequestNo + callType 的三元组。
/// 同一业务号的 confirm 重试必须复用同一 businessRequestNo，且保存 REQUEST_FINGERPRINT
/// 用于判断同一业务号下参数是否发生变化，防止静默数据错乱。
/// </para>
/// <para>
/// 【状态机流转】
/// <code>
/// SIMULATE（试算，不占额度）
///       │
///       └──→ CONFIRM_PENDING（确认，已占额度）
///                   │
///                   ├──→ COMMITTED（HIS 落账成功）
///                   │         │
///                   │         └──→ REVERSED（冲正，已对冲）
///                   │
///                   ├──→ CANCELLED（取消，已释放）
///                   │
///                   └──→ EXPIRED（过期，已清理）
/// </code>
/// 状态转换必须通过 MarkXxx() 方法执行，不允许外部直接赋值。
/// </para>
/// <para>
/// 【领域事件机制】
/// 聚合根在执行状态转换时收集领域事件到 DomainEvents 列表。
/// 当前实现状态：事件收集已完成，分发机制待后续迭代集成。
/// </para>
/// </remarks>
public sealed class ChargeRequest
{
    /// <summary>
    /// 领域事件收集列表，聚合根在状态转换时将事件追加到此列表。
    /// </summary>
    /// <remarks>
    /// 当前预留扩展点，待事件分发机制就绪后集成。
    /// </remarks>
    public List<IDomainEvent> DomainEvents { get; } = new();

    /// <summary>
    /// 计价请求日志主键，对应 Oracle 列 REQUEST_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// </summary>
    /// <remarks>
    /// 全局唯一标识一条请求日志，是串联 REQUEST_LOG → TRACE_STEP → DISCOUNT_DETAIL → LIMIT_OCCUPY
    /// 四张表的核心外键。不可修改，由数据库在 Insert 时自动填充。
    /// </remarks>
    public long RequestId { get; set; }

    /// <summary>
    /// 技术请求流水号，由调用方或服务端生成的技术流水号。
    /// </summary>
    /// <remarks>
    /// 用于排查单次 HTTP 调用。与 businessRequestNo 的区别：
    /// requestNo 每次调用都不同，businessRequestNo 在重试时保持不变。
    /// 格式建议：REQ-yyyyMMddHHmmssfff。
    /// </remarks>
    public string RequestNo { get; set; } = string.Empty;

    /// <summary>
    /// 调用方稳定业务号，来源为调用方传入的 businessRequestNo。
    /// </summary>
    /// <remarks>
    /// confirm 超时重试时必须复用同一业务号。幂等判断的三元组之一
    /// （sourceSystem + businessRequestNo + callType）。
    /// 空值表示该调用类型不支持幂等（如 simulate 试算）。
    /// HIS 能否提供稳定的 businessRequestNo 是待确认问题。
    /// </remarks>
    public string? BusinessRequestNo { get; set; }

    /// <summary>
    /// 规范化请求指纹，由计价中心对请求参数计算 SHA256 哈希值。
    /// </summary>
    /// <remarks>
    /// 用于判断同一 businessRequestNo 下参数是否发生变化：
    /// 若指纹不一致，说明调用方在重试时修改了参数，应拒绝并返回错误。
    /// 该字段是资金安全硬约束，必须在 confirm 接口编码前实现。
    /// </remarks>
    public string? RequestFingerprint { get; set; }

    /// <summary>
    /// 计价追踪号，全局唯一的追踪标识。
    /// </summary>
    /// <remarks>
    /// 用于跨表查看一次完整计价过程。由计价中心在首次 confirm 时生成，
    /// 后续 commit/cancel/reverse 复用同一追踪号。
    /// 格式建议：TRACE + 时间戳 + 随机数。
    /// </remarks>
    public string? TraceId { get; set; }

    /// <summary>
    /// 调用类型，标识本次请求的操作类型，是幂等三元组之一。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>SIMULATE — 试算，不占额度，不落日志</item>
    /// <item>CONFIRM — 确认计价，占用额度，写入 PENDING 占用</item>
    /// <item>COMMIT — HIS 落账成功通知，推进占用为 CONFIRMED</item>
    /// <item>CANCEL — 取消确认，释放 PENDING 占用</item>
    /// <item>REVERSE — 退费/冲正，生成对冲占用记录</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    public string CallType { get; set; } = string.Empty;

    /// <summary>
    /// 业务状态，描述请求在状态机中的当前位置。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>SIMULATED — 试算完成（仅 SIMULATE 类型）</item>
    /// <item>CONFIRM_PENDING — 已确认，等待 HIS commit 或 cancel</item>
    /// <item>COMMITTED — HIS 已落账</item>
    /// <item>CANCELLED — 已取消，额度已释放</item>
    /// <item>EXPIRED — 超时过期，已被后台清理任务释放</item>
    /// <item>REVERSED — 已冲正，生成对冲占用</item>
    /// </list>
    /// 不可为空，默认值由 callType 决定。状态转换必须通过 MarkXxx() 方法执行。
    /// </remarks>
    public string BusinessStatus { get; set; } = string.Empty;

    /// <summary>
    /// 来源系统编码，标识本次请求来自哪个渠道系统，是幂等三元组之一。
    /// </summary>
    /// <remarks>
    /// 常见值：HIS（医院信息系统）、KIOSK（自助机）、WECHAT（微信公众号）。
    /// 不同来源系统的限额统计独立计算（按收费途径统计）。
    /// </remarks>
    public string SourceSystem { get; set; } = string.Empty;

    /// <summary>
    /// 来源终端或站点标识，用于定位具体调用入口。
    /// </summary>
    /// <remarks>
    /// 如 HIS 工作站编号、自助机编号、微信 openid。
    /// 可为空，空值表示来源终端未知或不重要。
    /// </remarks>
    public string? SourceTerminal { get; set; }

    /// <summary>
    /// 患者标识，来源为调用方传入的 patientId。
    /// </summary>
    /// <remarks>
    /// 是限额累计和追溯查询的重要维度。日限额、时间窗限额均按患者维度累计。
    /// 空值表示该请求不关联特定患者（理论上不应出现）。
    /// </remarks>
    public string? PatientId { get; set; }

    /// <summary>
    /// 就诊标识，用于缩小追溯和对账范围。
    /// </summary>
    /// <remarks>
    /// 一个患者可能有多次就诊，visitId 可区分不同就诊的收费记录。
    /// 可为空。
    /// </remarks>
    public string? VisitId { get; set; }

    /// <summary>
    /// 收费场景编码，用于匹配差异化规则。
    /// </summary>
    /// <remarks>
    /// 常见值：OUTPATIENT（门诊）、INPATIENT（住院）、SURGERY（手术）、MEDICAL_TECH（医技）。
    /// 空值表示不按场景区分规则。
    /// </remarks>
    public string? ChargeScene { get; set; }

    /// <summary>
    /// 收费单号，来源为 HIS 传入的收费单号。
    /// </summary>
    /// <remarks>
    /// 用于与 HIS 落账结果关联。一张收费单可能包含多条收费明细，通过 chargeNo 聚合。
    /// </remarks>
    public string? ChargeNo { get; set; }

    /// <summary>
    /// 收费明细号，来源为 HIS 传入的收费明细号。
    /// </summary>
    /// <remarks>
    /// 用于定位单条收费项目。"单条"指单条收费明细，
    /// 与"单次"（单次收费动作，即一张收费单）不同。
    /// </remarks>
    public string? ChargeDetailNo { get; set; }

    /// <summary>
    /// 主子项目同组结果号，用于原子 commit/cancel。
    /// </summary>
    /// <remarks>
    /// 子项加收必须与主项目同 resultGroupNo。同组的所有明细在同一 confirm 中一起提交，
    /// commit 和 cancel 也必须一起处理。空值表示该请求不涉及主子项目关系。
    /// </remarks>
    public string? ResultGroupNo { get; set; }

    /// <summary>
    /// 项目编码，对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// </summary>
    /// <remarks>
    /// 是规则匹配、价格校验和限额累计的核心维度。
    /// "特殊项目"和"折价项目"是同一标识，通过该编码区分是否为特殊计价项目。
    /// </remarks>
    public string? ItemCode { get; set; }

    /// <summary>
    /// 项目名称，来源为 HIS 物价主数据。
    /// </summary>
    /// <remarks>
    /// 非计算字段，仅用于展示、审计和追溯可读性。
    /// </remarks>
    public string? ItemName { get; set; }

    /// <summary>
    /// 调用方录入的原始数量，保留调用方传入的原始数量不变。
    /// </summary>
    /// <remarks>
    /// 金额计算前不得随意覆盖。单位取决于 inputUnit 的配置（如 PART、CM2、EACH）。
    /// 多肿物、多部位项目应通过 pricingParts 明细表达，不能压成单个数量粗算。
    /// </remarks>
    public decimal? InputQty { get; set; }

    /// <summary>
    /// 调用方录入数量的单位。
    /// </summary>
    /// <remarks>
    /// 常见值：PART（部位/个）、CM2（平方厘米）、EACH（每个）、次。
    /// 用于双单位换算时确定输入单位。
    /// </remarks>
    public string? InputUnit { get; set; }

    /// <summary>
    /// 身体部位编码，用于分部位换算和部位差异规则。
    /// </summary>
    /// <remarks>
    /// 同一项目允许按不同部位维护不同换算规则。
    /// 空值表示不按部位区分规则。
    /// </remarks>
    public string? BodyPartCode { get; set; }

    /// <summary>
    /// 业务收费发生时间，来源为调用方传入的 businessChargeTime。
    /// </summary>
    /// <remarks>
    /// 不是计价中心的技术时间。业务时间优先于技术时间，用于：
    /// 1. 规则生效期判断（EFFECTIVE_FROM / EFFECTIVE_TO）
    /// 2. 日数量限额的当日划分
    /// 3. 2 小时滑动窗口的起点计算
    /// 4. 退费时按收费时间（包含重收时间）向前查 2 小时
    /// </remarks>
    public DateTime? BusinessChargeTime { get; set; }

    /// <summary>
    /// 权威价格版本或价格快照版本，用于金额追溯。
    /// </summary>
    /// <remarks>
    /// 记录 confirm 时使用的权威物价版本号。若后续价格调整，
    /// 可通过该版本号追溯当时的计价依据。
    /// 权威物价单价从 HIS 哪张表或同步表读取是待确认问题。
    /// </remarks>
    public string? PriceVersion { get; set; }

    /// <summary>
    /// 原始请求快照 JSON，存储调用方传入的完整请求参数。
    /// </summary>
    /// <remarks>
    /// 用于：
    /// 1. 幂等排查（同一业务号参数是否变化）
    /// 2. 事后复算（用原始参数重新计算验证结果）
    /// 3. 审计和合规
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// </remarks>
    public string? RequestJson { get; set; }

    /// <summary>
    /// 计价响应快照 JSON，存储计价中心返回的完整响应。
    /// </summary>
    /// <remarks>
    /// 用于：
    /// 1. 幂等重放（同一业务号直接返回缓存结果）
    /// 2. 追溯展示（查看当时计算的详细结果）
    /// 3. 审计和合规
    /// Oracle 存储类型为 CLOB。
    /// </remarks>
    public string? ResponseJson { get; set; }

    /// <summary>
    /// 计价中心收到请求的技术时间，由计价中心自动填充。
    /// </summary>
    /// <remarks>
    /// 用于审计和性能监控。不得用该时间替代 BusinessChargeTime 做业务判断。
    /// </remarks>
    public DateTime RequestAt { get; set; }

    /// <summary>
    /// 计价中心完成响应或状态更新的技术时间。
    /// </summary>
    /// <remarks>
    /// 用于性能监控（responseAt - requestAt = 处理耗时）。
    /// 空值表示请求尚未处理完成。
    /// </remarks>
    public DateTime? ResponseAt { get; set; }

    /// <summary>
    /// 请求是否成功落入计价中心处理链路。
    /// </summary>
    /// <remarks>
    /// "Y" 表示成功进入处理链路，"N" 表示请求被拒绝或处理失败。
    /// 注意：该字段表示计价中心是否成功处理，不代表 HIS commit 是否成功。
    /// 默认值为 "N"，处理成功后更新为 "Y"。
    /// </remarks>
    public string IsSuccess { get; set; } = "N";

    /// <summary>
    /// 请求失败时记录的错误信息。
    /// </summary>
    /// <remarks>
    /// 当 IsSuccess = "N" 时，记录具体错误原因，用于排查和运维。
    /// 常见错误码：PRICE_MISMATCH（单价不匹配）、RULE_NOT_FOUND（规则未找到）、
    /// LIMIT_EXCEEDED（限额超限）、IDEMPOTENT_MISMATCH（幂等指纹不匹配）。
    /// </remarks>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 标记为确认待落账状态（CONFIRM_PENDING）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// confirm 接口在事务内完成引擎计算、写请求日志、写折价明细、写限额占用后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 此方法不校验前置状态，因为调用方是新创建的 ChargeRequest 实例，
    /// BusinessStatus 为默认值。如果需要从其他状态转换，应使用其他 MarkXxx 方法。
    /// </para>
    /// </remarks>
    public void MarkConfirmPending()
    {
        BusinessStatus = BusinessStatusCodes.ConfirmPending;
        IsSuccess = EnableFlag.Yes;
        ResponseAt = DateTime.Now;
    }

    /// <summary>
    /// 标记为已落账状态（COMMITTED），仅允许从 CONFIRM_PENDING 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// HIS 调用 commit 接口，应用服务校验请求状态和落账明细后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 CONFIRM_PENDING 允许 commit。已取消、已过期或已冲正的记录
    /// 再次 commit 会被拒绝，防止状态机出现非法跳跃。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 CONFIRM_PENDING 时抛出。</exception>
    public void MarkCommitted()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 CONFIRM_PENDING 允许 commit。已取消、已过期或已冲正的记录
        // 再次 commit 会被拒绝，防止状态机出现非法跳跃。
        if (BusinessStatus != BusinessStatusCodes.ConfirmPending)
            throw new InvalidOperationException($"请求 {RequestNo} 当前状态为 {BusinessStatus}，仅 CONFIRM_PENDING 允许 commit。");

        // ========== 第二阶段：推进状态 ==========
        BusinessStatus = BusinessStatusCodes.Committed;
        ResponseAt = DateTime.Now;
    }

    /// <summary>
    /// 标记为已取消状态（CANCELLED），仅允许从 CONFIRM_PENDING 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// HIS 调用 cancel 接口，应用服务校验请求状态后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 CONFIRM_PENDING 允许 cancel。已 commit 的记录应走 reverse 流程，
    /// 不能直接 cancel，否则会出现"已落账但被取消"的资金口径断裂。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 CONFIRM_PENDING 时抛出。</exception>
    public void MarkCancelled()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 CONFIRM_PENDING 允许 cancel。已 commit 的记录应走 reverse 流程，
        // 不能直接 cancel，否则会出现"已落账但被取消"的资金口径断裂。
        if (BusinessStatus != BusinessStatusCodes.ConfirmPending)
            throw new InvalidOperationException($"请求 {RequestNo} 当前状态为 {BusinessStatus}，仅 CONFIRM_PENDING 允许 cancel。");

        // ========== 第二阶段：推进状态 ==========
        BusinessStatus = BusinessStatusCodes.Cancelled;
        ResponseAt = DateTime.Now;
    }

    /// <summary>
    /// 标记为已过期状态（EXPIRED），仅允许从 CONFIRM_PENDING 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// 后台清理任务（ExpireCleanupAppService）扫描长时间停留在 CONFIRM_PENDING 的记录后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 CONFIRM_PENDING 允许过期。已 commit 的记录不能被过期清理，
    /// 否则会出现"已落账但被清理"的资金口径断裂。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 CONFIRM_PENDING 时抛出。</exception>
    public void MarkExpired()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 CONFIRM_PENDING 允许过期。已 commit 的记录不能被过期清理，
        // 否则会出现"已落账但被清理"的资金口径断裂。
        if (BusinessStatus != BusinessStatusCodes.ConfirmPending)
            throw new InvalidOperationException($"请求 {RequestNo} 当前状态为 {BusinessStatus}，仅 CONFIRM_PENDING 允许过期。");

        // ========== 第二阶段：推进状态 ==========
        BusinessStatus = BusinessStatusCodes.Expired;
        ResponseAt = DateTime.Now;
    }

    /// <summary>
    /// 标记为已冲正状态（REVERSED），仅允许从 COMMITTED 转换。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// HIS 调用 reverse 接口，应用服务校验请求状态和退费数量后调用。
    /// </para>
    /// <para>
    /// 【状态机约束】
    /// 只有 COMMITTED 允许冲正。CONFIRM_PENDING 应走 cancel 流程，
    /// 不能直接 reverse，否则会出现"未落账但被冲正"的资金口径断裂。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">当前状态不是 COMMITTED 时抛出。</exception>
    public void MarkReversed()
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 COMMITTED 允许冲正。CONFIRM_PENDING 应走 cancel 流程，
        // 不能直接 reverse，否则会出现"未落账但被冲正"的资金口径断裂。
        if (BusinessStatus != BusinessStatusCodes.Committed)
            throw new InvalidOperationException($"请求 {RequestNo} 当前状态为 {BusinessStatus}，仅 COMMITTED 允许冲正。");

        // ========== 第二阶段：推进状态 ==========
        BusinessStatus = BusinessStatusCodes.Reversed;
        ResponseAt = DateTime.Now;
    }

    /// <summary>
    /// 清空领域事件列表，通常在应用服务分发完事件后调用。
    /// </summary>
    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}