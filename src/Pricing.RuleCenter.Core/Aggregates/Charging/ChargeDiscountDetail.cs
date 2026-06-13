namespace Pricing.RuleCenter.Core.Aggregates.Charging;

/// <summary>
/// 计价折扣明细实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_CHARGE_DISCOUNT_DETAIL
/// </para>
/// <para>
/// 在计价链路中的角色：保存一次计价中原始数量/金额、换算数量、最终数量/金额和折价原因。
/// 它跟随请求状态流转，用于 HIS 对账、规则追踪和后续退费冲正。
/// </para>
/// <para>
/// 金额精度约束：所有金额字段类型为 decimal（对应 Oracle NUMBER(18,4)）。
/// 中间计算保留全部精度（ORIGINAL_AMT、CALCULATED_AMT），最终对外金额（FINAL_AMT）
/// 保留 2 位小数、四舍五入。
/// </para>
/// <para>
/// 状态机流转：PENDING → CONFIRMED（commit 后）→ CANCELLED（cancel 后）| EXPIRED（过期清理）
/// | REVERSED（冲正后）。主子项目同 resultGroupNo 原子 commit/cancel。
/// </para>
/// </remarks>
public sealed class ChargeDiscountDetail
{
    /// <summary>
    /// 折扣明细主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 DISCOUNT_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条折扣明细记录。
    /// </remarks>
        public long DiscountId { get; set; }

    /// <summary>
    /// 关联的计价请求日志主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID，用于串联请求、步骤、折价明细和限额占用四张表。
    /// 一条请求日志可以产生多条折扣明细（如多部位拆分、主子项目）。
    /// </remarks>
        public long RequestId { get; set; }

    /// <summary>
    /// 计价追踪号。
    /// </summary>
    /// <remarks>
    /// 全局唯一的追踪标识，用于跨表查看一次完整计价过程。
    /// 与 ChargeRequest.TraceId 一致。
    /// </remarks>
        public string? TraceId { get; set; }

    /// <summary>
    /// 收费单号。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 传入的收费单号，用于与 HIS 落账结果关联。
    /// 一张收费单可能包含多条收费明细。
    /// </remarks>
        public string? ChargeNo { get; set; }

    /// <summary>
    /// 收费明细号。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 传入的收费明细号，用于定位单条收费项目。
    /// "单条"指单条收费明细，与"单次"（单次收费动作）不同。
    /// </remarks>
        public string? ChargeDetailNo { get; set; }

    /// <summary>
    /// 患者标识。
    /// </summary>
    /// <remarks>
    /// 限额累计和追溯查询的重要维度。
    /// 空值理论上不应出现。
    /// </remarks>
        public string? PatientId { get; set; }

    /// <summary>
    /// 就诊标识。
    /// </summary>
    /// <remarks>
    /// 可为空，存在时用于缩小追溯和对账范围。
    /// </remarks>
        public string? VisitId { get; set; }

    /// <summary>
    /// 项目编码。
    /// </summary>
    /// <remarks>
    /// 对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// 是规则匹配、价格诊断和限额累计的核心维度。
    /// </remarks>
        public string? ItemCode { get; set; }

    /// <summary>
    /// 项目名称。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据，用于展示、审计和追溯说明。
    /// 非计算字段，仅用于可读性。
    /// </remarks>
        public string? ItemName { get; set; }

    /// <summary>
    /// 产生本次折价结果的规则主键。
    /// </summary>
    /// <remarks>
    /// 关联 PR_RULE_HEADER.RULE_ID，用于追溯该折价是由哪条规则触发的。
    /// 空值表示未命中折价规则（普通计价，原价收费）。
    /// </remarks>
        public long? RuleId { get; set; }

    /// <summary>
    /// 产生本次折价结果的规则版本号。
    /// </summary>
    /// <remarks>
    /// 关联 PR_RULE_VERSION.VERSION_NO，用于锁定产生折价时的规则版本，
    /// 便于后续版本回滚或变更时评估影响范围。
    /// </remarks>
        public int? RuleVersionNo { get; set; }

    /// <summary>
    /// 历史运行包兼容列，当前直接规则链路不再写入。
    /// </summary>
    public long? RuntimePackageId { get; set; }

    /// <summary>
    /// 历史运行时规则兼容列，当前直接规则链路不再写入。
    /// </summary>
    public long? RuntimeRuleId { get; set; }

    /// <summary>
    /// 产生本次折价结果的来源策略版本主键。
    /// </summary>
    public long? SourcePolicyVersionId { get; set; }

    /// <summary>
    /// 产生本次折价结果的来源模板版本主键。
    /// </summary>
    public long? SourceTemplateVersionId { get; set; }

    /// <summary>
    /// 折价类型编码。
    /// </summary>
    /// <remarks>
    /// 标识本次折价的计算方式：
    /// <list type="bullet">
    /// <item>FORMULA — 公式折算，通过配置的公式计算最终金额</item>
    /// <item>LIMIT_CUT — 限额截断，超出限额部分为 0 元</item>
    /// <item>EXCESS_ZERO — 超额归零，超出部分全部为 0 元</item>
    /// <item>GROUP_EXCLUSIVE — 同组互斥，同组只保留优先级最高的一条</item>
    /// </list>
    /// 空值表示未命中折价规则。
    /// </remarks>
        public string? DiscountType { get; set; }

    /// <summary>
    /// 折扣明细记录状态。
    /// </summary>
    /// <remarks>
    /// 状态机流转：
    /// <list type="bullet">
    /// <item>PENDING — confirm 阶段写入，尚未 commit</item>
    /// <item>CONFIRMED — HIS 调用 commit 后确认</item>
    /// <item>CANCELLED — HIS 调用 cancel 后取消</item>
    /// <item>EXPIRED — 超时过期，被后台清理任务释放</item>
    /// <item>REVERSED — HIS 调用 reverse 后冲正</item>
    /// </list>
    /// 默认值为 "PENDING"。
    /// </remarks>
        public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 主子项目同组结果号。
    /// </summary>
    /// <remarks>
    /// 子项加收必须与主项目同 resultGroupNo 原子 commit/cancel。
    /// 同组的所有明细在同一 confirm 中一起提交，commit 和 cancel 也必须一起处理。
    /// 空值表示该明细不涉及主子项目关系。
    /// </remarks>
        public string? ResultGroupNo { get; set; }

    /// <summary>
    /// 父折扣明细主键。
    /// </summary>
    /// <remarks>
    /// 用于主子项目或拆分明细之间建立层级关系。
    /// 例如：主项目的 DiscountId 作为子项目的 ParentDiscountId。
    /// 空值表示该明细是顶层记录，没有父级。
    /// </remarks>
        public long? ParentDiscountId { get; set; }

    /// <summary>
    /// 多部位或多片段计价时的片段序号。
    /// </summary>
    /// <remarks>
    /// 当一个收费项目包含多个部位或片段时（如多肿物切除），每个部位独立计价。
    /// PartSeq 从 1 开始递增，用于区分同一请求内的不同部位明细。
    /// 空值表示该项目不涉及多部位拆分。
    /// </remarks>
        public int? PartSeq { get; set; }

    /// <summary>
    /// 原始数量。
    /// </summary>
    /// <remarks>
    /// 保存折价前的业务输入数量，单位为调用方传入的 inputUnit。
    /// 用于与 FinalQty 对比展示折价过程。
    /// 精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? OriginalQty { get; set; }

    /// <summary>
    /// 换算后的计价数量。
    /// </summary>
    /// <remarks>
    /// 经过双单位换算后的数量，是数量限制之前的计价数量基准。
    /// 无数量限制时，公式使用的 FinalQty 通常与该值一致；有限制时，公式使用限制后的 FinalQty。
    /// 若未配置换算规则，该值等于 OriginalQty。
    /// 精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? ConvertedQty { get; set; }

    /// <summary>
    /// 最终可收费数量。
    /// </summary>
    /// <remarks>
    /// 经过日数量限制、时间窗数量限制、同组互斥等全部处理后的最终数量。
    /// 若存在数量下限或上限，该值已在限制范围内。
    /// 若超出限制，超出部分为 0（不是整单归零，不是拒单）。
    /// 精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? FinalQty { get; set; }

    /// <summary>
    /// 项目单价。
    /// </summary>
    /// <remarks>
    /// 本次计价实际采用的项目单价，通常来自渠道请求明细。
    /// 权威物价主数据差异只进入诊断日志，不影响该明细落库。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 原始金额。
    /// </summary>
    /// <remarks>
    /// 通常为 OriginalQty * UnitPrice，即折价前的原始收费金额。
    /// 用于与 FinalAmt 对比展示折价让利了多少。
    /// 中间计算字段，保留全部精度，不做取整。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? OriginalAmt { get; set; }

    /// <summary>
    /// 公式或动作链中间计算金额。
    /// </summary>
    /// <remarks>
    /// 经过公式计算后的中间金额，尚未经过金额上下限校验。
    /// FIN_DISCOUNT_FEE 类比例折价使用数量限制后的 FinalQty 计算；TOPPRICE 再在该金额之后封顶。
    /// 中间计算字段，保留全部精度，不做取整。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? CalculatedAmt { get; set; }

    /// <summary>
    /// 最终收费金额。
    /// </summary>
    /// <remarks>
    /// 经过数量限制、公式折价、金额上下限和超限兜底等全部处理后的最终收费金额。
    /// 最终对外金额保留 2 位小数，四舍五入（roundMode 可配置）。
    /// 若项目超出限额，超出部分为 0 元。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? FinalAmt { get; set; }

    /// <summary>
    /// 折价金额。
    /// </summary>
    /// <remarks>
    /// 等于 OriginalAmt - FinalAmt，即折价让利的金额。
    /// 若未命中折价规则，该值为 0。
    /// 用于 HIS 对账和审计，展示折价让利了多少。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// </remarks>
        public decimal? DiscountAmt { get; set; }

    /// <summary>
    /// 折价原因编码。
    /// </summary>
    /// <remarks>
    /// 便于与规则或业务原因字典关联。
    /// 常见值：FORMULA（公式折算）、LIMIT_EXCEEDED（超限）、ZERO_EXCESS（超额归零）。
    /// </remarks>
        public string? ReasonCode { get; set; }

    /// <summary>
    /// 折价原因描述。
    /// </summary>
    /// <remarks>
    /// 面向追踪页面或对账说明展示的人类可读描述。
    /// 例如："超出日限额 3 次，超出部分为 0 元"。
    /// </remarks>
        public string? ReasonDesc { get; set; }

    /// <summary>
    /// 限额计算依据快照 JSON。
    /// </summary>
    /// <remarks>
    /// 存储限额校验时的累计依据，用于解释当时累计了哪些维度和窗口。
    /// 包含：已占用数量、已占用金额、时间窗口范围、同组项目列表等。
    /// Oracle 存储类型为 CLOB。
    /// </remarks>
        public string? LimitBaseInfo { get; set; }

    /// <summary>
    /// 折价明细生成时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在 confirm 阶段自动填充。
    /// 用于审计和排序。
    /// </remarks>
        public DateTime OccurredAt { get; set; }

    /// <summary>
    /// 确认操作人或来源系统账号。
    /// </summary>
    /// <remarks>
    /// 记录 confirm 操作的执行者，用于审计。
    /// 可为空，空值表示来源系统未提供操作人信息。
    /// </remarks>
        public string? ConfirmedBy { get; set; }

    /// <summary>
    /// HIS 成功落账并调用 commit 的时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在 commit 阶段填充。
    /// 空值表示尚未 commit（仍为 PENDING 状态）。
    /// 用于审计和对账，确认 HIS 何时完成落账。
    /// </remarks>
        public DateTime? CommittedAt { get; set; }

    /// <summary>
    /// 保护占用被取消的时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在 cancel 阶段填充。
    /// 空值表示未被取消。
    /// </remarks>
        public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// confirm 保护期过期并被清理的时间。
    /// </summary>
    /// <remarks>
    /// 由后台挂起清理任务（expire）在超时后填充。
    /// 防止 HIS 超时未调用 commit/cancel 导致额度被永久占用。
    /// </remarks>
        public DateTime? ExpiredAt { get; set; }

    /// <summary>
    /// 已提交折扣明细被冲正的时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在 reverse 阶段填充。
    /// 空值表示未被冲正。
    /// 退费规则：当日退费释放数量；隔日退费重收后按重收当天重新做额度校验。
    /// </remarks>
        public DateTime? ReversedAt { get; set; }
}
