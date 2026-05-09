using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 计价计算请求 DTO。
/// </summary>
/// <remarks>
/// 该请求同时服务试算和确认计费。试算可以没有稳定业务请求号，确认计费必须尽量提供
/// BusinessRequestNo，便于规则中心做幂等保护和重复提交识别。
/// </remarks>
public sealed class PricingCalculateRequest
{
    /// <summary>
    /// 调用方或服务端生成的技术请求流水号，用于排查单次 HTTP 调用
    /// </summary>
    public string? RequestNo { get; init; }

    [Required(ErrorMessage = "患者ID不能为空")]
    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    public string PatientId { get; init; } = string.Empty;

    /// <summary>
    /// 就诊标识，可为空，存在时用于缩小追溯和对账范围
    /// </summary>
    public string? VisitId { get; init; }
    /// <summary>
    /// 门诊号、住院号或就诊流水号，用于与 HIS 业务上下文对齐
    /// </summary>
    public string? EncounterNo { get; init; }

    [Required(ErrorMessage = "项目编码不能为空")]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string ItemCode { get; init; } = string.Empty;

    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; init; }

    [Required(ErrorMessage = "数量不能为空")]
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    public decimal InputQty { get; init; }

    /// <summary>
    /// 调用方录入数量的单位，例如 PART、CM2、EACH
    /// </summary>
    public string? Unit { get; init; }
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验
    /// </summary>
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹
    /// </summary>
    public string? BodyPartCode { get; init; }
    /// <summary>
    /// 收费场景编码，用于匹配门诊、住院、手术批费等差异化规则
    /// </summary>
    public string? ChargeScene { get; init; }
    /// <summary>
    /// 业务收费发生时间，用于规则生效判断和滑动窗口累计
    /// </summary>
    public DateTime BusinessChargeTime { get; init; }

    [Required(ErrorMessage = "来源系统不能为空")]
    /// <summary>
    /// 来源系统编码，例如 HIS、自助机或微信公众号
    /// </summary>
    public string SourceSystem { get; init; } = string.Empty;

    /// <summary>
    /// 来源终端或站点标识，用于定位具体调用入口
    /// </summary>
    public string? SourceTerminal { get; init; }
    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; init; }
    /// <summary>
    /// 收费明细号，用于定位单条收费项目
    /// </summary>
    public string? ChargeDetailNo { get; init; }
    /// <summary>
    /// 调用方稳定业务号，confirm 重试必须复用，用于幂等保护
    /// </summary>
    public string? BusinessRequestNo { get; init; }
    /// <summary>
    /// 操作员工号或系统账号，用于审计谁发起了本次计价。
    /// </summary>
    public string? OperatorId { get; init; }
    /// <summary>
    /// 操作员姓名，用于追踪页面展示。
    /// </summary>
    public string? OperatorName { get; init; }
    /// <summary>
    /// 调用方扩展参数。规则中心不会把它作为核心幂等字段，除非后续显式纳入指纹规则。
    /// </summary>
    public Dictionary<string, object?>? ExtraParams { get; init; }
    /// <summary>
    /// 多部位或多片段明细，用于更细粒度的部位匹配、面积折算和追踪展示。
    /// </summary>
    public IReadOnlyList<PricingPartItemRequest>? PricingParts { get; init; }
}

/// <summary>
/// 计价请求中的部位或片段明细 DTO。
/// </summary>
/// <remarks>
/// 明细项是可选扩展结构，用于表达一个收费项目内部的多个部位、面积或病灶。当前项目主字段仍然以
/// PricingCalculateRequest 上的 ItemCode、InputQty 和 UnitPrice 为准。
/// </remarks>
public sealed class PricingPartItemRequest
{
    /// <summary>
    /// 片段序号，用于保持调用方传入顺序。
    /// </summary>
    public int? PartSeq { get; init; }
    /// <summary>
    /// 片段编码，可对应部位、材料子项或其他业务拆分编码。
    /// </summary>
    public string? PartCode { get; init; }
    /// <summary>
    /// 片段名称，用于追踪页面展示。
    /// </summary>
    public string? PartName { get; init; }
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹
    /// </summary>
    public string? BodyPartCode { get; init; }
    /// <summary>
    /// 该片段对应的数量。
    /// </summary>
    public decimal Qty { get; init; }
    /// <summary>
    /// 该片段面积，适用于按面积折算的项目。
    /// </summary>
    public decimal? Area { get; init; }
    /// <summary>
    /// 计量类型，例如面积、长度、次数或病灶数量。
    /// </summary>
    public string? MeasureType { get; init; }
    /// <summary>
    /// 计量数值，与 MeasureType 和 MeasureUnit 配合解释。
    /// </summary>
    public decimal? MeasureValue { get; init; }
    /// <summary>
    /// 计量单位，例如 cm2、cm、次。
    /// </summary>
    public string? MeasureUnit { get; init; }
    /// <summary>
    /// 病灶数量，适用于按病灶个数折算或限额的项目。
    /// </summary>
    public int? LesionCount { get; init; }
}

/// <summary>
/// 计价计算响应 DTO。
/// </summary>
/// <remarks>
/// 试算响应和确认响应都使用该结构。确认响应中的 RequestId 是后续 commit/cancel/reverse 的关键引用，
/// 调用方必须保存。
/// </remarks>
public sealed class PricingCalculateResponse
{
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; init; }
    /// <summary>
    /// 是否命中特殊计价规则。
    /// </summary>
    public bool IsSpecialItem { get; init; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    public decimal InputQty { get; init; }
    /// <summary>
    /// 最终可收费数量
    /// </summary>
    public decimal FinalQty { get; init; }
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验
    /// </summary>
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 最终可收费金额。
    /// </summary>
    public decimal FinalAmount { get; init; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>
    /// 本次计价追踪步骤，用于接口调用方展示或排查。
    /// </summary>
    public IReadOnlyList<PricingTraceStepResponse> TraceSteps { get; init; } = Array.Empty<PricingTraceStepResponse>();
    /// <summary>
    /// 本次计价命中的规则主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();
}

/// <summary>
/// 计价追踪步骤响应 DTO。
/// </summary>
public sealed class PricingTraceStepResponse
{
    /// <summary>
    /// 步骤序号，按计价链路执行顺序递增
    /// </summary>
    public int StepNo { get; init; }
    /// <summary>
    /// 步骤类型，只使用 DDL 允许的 MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR
    /// </summary>
    public string StepType { get; init; } = string.Empty;
    /// <summary>
    /// 步骤说明，解释命中规则、限制口径或折价原因
    /// </summary>
    public string? StepDesc { get; init; }
    /// <summary>
    /// 当前步骤处理前的关键数值，通常为数量或金额。
    /// </summary>
    public decimal? InputValue { get; init; }
    /// <summary>
    /// 当前步骤处理后的关键数值，通常为数量或金额。
    /// </summary>
    public decimal? OutputValue { get; init; }
}

/// <summary>
/// 确认计费提交请求 DTO。
/// </summary>
/// <remarks>
/// commit 用于告诉规则中心 HIS 已经成功落账。成功后 CONFIRM_PENDING 会推进到 COMMITTED，
/// 保护占额会变成正式占用。
/// </remarks>
public sealed class PricingCommitRequest
{
    [Required(ErrorMessage = "请求ID不能为空")]
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; init; }

    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; init; }
}

/// <summary>
/// 确认计费取消请求 DTO。
/// </summary>
/// <remarks>
/// cancel 用于释放 confirm 阶段已经产生、但最终未落账的保护占用。
/// </remarks>
public sealed class PricingCancelRequest
{
    [Required(ErrorMessage = "请求ID不能为空")]
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; init; }
}

/// <summary>
/// 已提交计价结果冲正请求 DTO。
/// </summary>
/// <remarks>
/// reverse 针对已经 commit 成功的原请求，通常用于退费或撤销收费。冲正会写入冲正日志，
/// 并通过负向占用释放对应限额。
/// </remarks>
public sealed class PricingReverseRequest
{
    [Required(ErrorMessage = "原请求ID不能为空")]
    /// <summary>
    /// 原始确认请求的 RequestId，必须指向已经提交成功的计价记录。
    /// </summary>
    public long OriginalRequestId { get; init; }

    /// <summary>
    /// 调用方冲正流水号，用于和 HIS 退费或撤销单据关联。
    /// </summary>
    public string? ReverseNo { get; init; }
    /// <summary>
    /// 本次冲正数量；为空时可由服务层按原请求数量处理。
    /// </summary>
    public decimal? ReverseQty { get; init; }
    /// <summary>
    /// 本次冲正金额；为空时可由服务层按原请求金额处理。
    /// </summary>
    public decimal? ReverseAmt { get; init; }
    /// <summary>
    /// 冲正操作人或系统账号。
    /// </summary>
    public string? ReversedBy { get; init; }
    /// <summary>
    /// 冲正原因说明，用于审计和追踪页面展示。
    /// </summary>
    public string? Reason { get; init; }
}

/// <summary>
/// 特殊计价项目标识响应 DTO。
/// </summary>
public sealed class SpecialFlagResponse
{
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 是否存在已发布且启用的特殊计价规则。
    /// </summary>
    public bool IsSpecial { get; init; }
    /// <summary>
    /// 当前项目命中的有效规则数量。
    /// </summary>
    public int RuleCount { get; init; }
}
