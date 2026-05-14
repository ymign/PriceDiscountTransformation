namespace Pricing.RuleCenter.Core.Aggregates.Charging;

/// <summary>
/// 冲正日志实体，记录退费和撤销流水。
/// </summary>
/// <remarks>
/// <para>
/// 【对应 Oracle 表】
/// PR_CHARGE_REVERSE_LOG。主键 REVERSE_ID 由 SEQUENCE 生成。
/// </para>
/// <para>
/// 【在计价链路中的角色】
/// 冲正日志记录每次退费/撤销操作的详细信息，用于：
/// <list type="bullet">
/// <item>双向追溯 — 从原收费记录找到冲正记录，或从冲正记录找到原始收费</item>
/// <item>退费额度释放 — 当日退费按退费数量释放额度，隔日退费重收后按重收当天重新校验</item>
/// <item>审计和对账 — 记录退费原因、退费数量、退费金额和操作人</item>
/// </list>
/// </para>
/// <para>
/// 【退费规则】
/// <list type="bullet">
/// <item>当日退费：按退费数量释放额度</item>
/// <item>隔日退费：重收后按重收当天重新做额度校验</item>
/// <item>部分退费：必须校验本次退费 + 历史已退不超过原有效收费</item>
/// <item>2 小时规则：按收费时间（包含重收时间）向前查 2 小时</item>
/// </list>
/// </para>
/// </remarks>
public sealed class ChargeReverseLog
{
    /// <summary>
    /// 冲正日志主键，对应 Oracle 列 REVERSE_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// </summary>
    /// <remarks>
    /// 全局唯一标识一条冲正记录。
    /// </remarks>
    public long ReverseId { get; set; }

    /// <summary>
    /// 关联的原始计价请求日志主键，对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID。
    /// </summary>
    /// <remarks>
    /// 用于双向追溯：从原收费记录找到冲正记录，或从冲正记录找到原始收费。
    /// 不可为空。
    /// </remarks>
    public long OriginalRequestId { get; set; }

    /// <summary>
    /// 冲正请求日志主键，对应冲正操作自身产生的 PR_CHARGE_REQUEST_LOG.REQUEST_ID。
    /// </summary>
    /// <remarks>
    /// 冲正操作也会产生一条新的请求日志，该字段关联冲正请求的 REQUEST_ID。
    /// 可为空，空值表示冲正请求尚未写入。
    /// </remarks>
    public long? ReverseRequestId { get; set; }

    /// <summary>
    /// 计价追踪号，与原始请求的 TraceId 相同。
    /// </summary>
    /// <remarks>
    /// 用于跨表追溯一次完整计价过程（包含收费和冲正）。
    /// 可为空。
    /// </remarks>
    public string? TraceId { get; set; }

    /// <summary>
    /// 收费单号，来源为 HIS 传入的收费单号。
    /// </summary>
    /// <remarks>
    /// 用于与 HIS 落账结果关联。
    /// 可为空。
    /// </remarks>
    public string? ChargeNo { get; set; }

    /// <summary>
    /// 冲正流水号，由调用方或计价中心生成的技术流水号。
    /// </summary>
    /// <remarks>
    /// 用于排查单次冲正操作。格式建议：REV-yyyyMMddHHmmssfff。
    /// 可为空。
    /// </remarks>
    public string? ReverseNo { get; set; }

    /// <summary>
    /// 患者标识，来源为调用方传入的 patientId。
    /// </summary>
    /// <remarks>
    /// 用于退费额度释放时定位患者的限额占用记录。
    /// </remarks>
    public string? PatientId { get; set; }

    /// <summary>
    /// 项目编码，对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// </summary>
    /// <remarks>
    /// 用于退费额度释放时定位项目的限额占用记录。
    /// </remarks>
    public string? ItemCode { get; set; }

    /// <summary>
    /// 被退费或冲正的原收费明细号。
    /// </summary>
    /// <remarks>
    /// 用于定位单条收费项目的退费。可为空。
    /// </remarks>
    public string? ChargeDetailNo { get; set; }

    /// <summary>
    /// 多部位或多片段退费时的片段序号。
    /// </summary>
    /// <remarks>
    /// 当一个收费项目包含多个部位或片段时，每个部位独立退费。
    /// PartSeq 从 1 开始递增，用于区分同一请求内的不同部位退费。
    /// 空值表示该项目不涉及多部位拆分。
    /// </remarks>
    public int? PartSeq { get; set; }

    /// <summary>
    /// 冲正类型，标识本次冲正的操作类型。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>FULL — 全额退费，退费数量等于原收费数量</item>
    /// <item>PARTIAL — 部分退费，退费数量小于原收费数量</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    public string ReverseType { get; set; } = string.Empty;

    /// <summary>
    /// 退费数量，必须大于 0。
    /// </summary>
    /// <remarks>
    /// 部分退费时必须校验本次退费 + 历史已退不超过原有效收费。
    /// 精度 NUMBER(18,4)，与项目数量单位一致。
    /// 可为空，空值表示数量未知。
    /// </remarks>
    public decimal? ReverseQty { get; set; }

    /// <summary>
    /// 退费金额，必须大于 0。
    /// </summary>
    /// <remarks>
    /// 对应退费数量 × 折后单价的金额。
    /// 精度 NUMBER(18,4)，单位：元（人民币）。
    /// 可为空，空值表示金额未知。
    /// </remarks>
    public decimal? ReverseAmt { get; set; }

    /// <summary>
    /// 冲正原因，来源为调用方传入或操作人填写。
    /// </summary>
    /// <remarks>
    /// 用于审计和追溯。常见原因：患者退费、收费错误、医嘱撤销等。
    /// </remarks>
    public string? ReverseReason { get; set; }

    /// <summary>
    /// 冲正操作人，来源为 HIS 工作站登录用户。
    /// </summary>
    /// <remarks>
    /// 用于审计和责任追溯。
    /// </remarks>
    public string? ReversedBy { get; set; }

    /// <summary>
    /// 冲正发生时间，由计价中心自动填充。
    /// </summary>
    /// <remarks>
    /// 用于审计和对账。
    /// </remarks>
    public DateTime ReversedAt { get; set; }
}
