namespace Pricing.RuleCenter.Application.Dto;

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
    /// 多费用明细计价结果。一次结算包含多条费用时，调用方应优先使用该集合落账和展示。
    /// </summary>
    public IReadOnlyList<PricingCalculateItemResponse> Items { get; init; } = Array.Empty<PricingCalculateItemResponse>();
    /// <summary>
    /// 计价追踪号，用于跨请求主表、步骤、折价明细、限额占用查看同一次计价链路。
    /// </summary>
    public string? TraceId { get; init; }
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; init; }
    /// <summary>
    /// 本次计价使用的运行时包主键。为空表示当前仍走旧规则读模型或未激活运行时包。
    /// </summary>
    public long? RuntimePackageId { get; init; }
    /// <summary>
    /// 本次计价使用的运行时包版本号。
    /// </summary>
    public long? RuntimePackageVersion { get; init; }
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
    /// 原始总金额，等于各明细原始数量 × 单价之和。多明细响应应优先读取该字段而非根 UnitPrice。
    /// </summary>
    public decimal TotalOriginalAmount { get; init; }
    /// <summary>
    /// 最终总金额，语义等同于兼容字段 FinalAmount。
    /// </summary>
    public decimal TotalFinalAmount { get; init; }
    /// <summary>
    /// 总折价金额，语义等同于兼容字段 DiscountAmount。
    /// </summary>
    public decimal TotalDiscountAmount { get; init; }
    /// <summary>
    /// 最终可收费金额。
    /// </summary>
    public decimal FinalAmount { get; init; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>
    /// confirm 结果有效期。为空表示本次响应不是正式确认占用结果。
    /// </summary>
    public DateTime? ExpireAt { get; init; }
    /// <summary>
    /// confirm 结果剩余有效秒数。为空表示本次响应不是正式确认占用结果。
    /// </summary>
    public int? ExpireSeconds { get; init; }
    /// <summary>
    /// 本次计价追踪步骤，用于接口调用方展示或排查。
    /// </summary>
    public IReadOnlyList<PricingTraceStepResponse> TraceSteps { get; init; } = Array.Empty<PricingTraceStepResponse>();
    /// <summary>
    /// 本次计价命中的规则主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();
    /// <summary>
    /// 本次计价命中的运行时规则主键集合。当前响应存在 RuntimePackageId 时，该字段与 MatchedRuleIds 口径一致。
    /// </summary>
    public IReadOnlyList<long> MatchedRuntimeRuleIds { get; init; } = Array.Empty<long>();
    /// <summary>
    /// 本次计价命中的来源策略版本主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedPolicyVersionIds { get; init; } = Array.Empty<long>();
    /// <summary>
    /// 本次计价命中的来源模板版本主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedTemplateVersionIds { get; init; } = Array.Empty<long>();
}

/// <summary>
/// 单条费用明细计价响应 DTO。
/// </summary>
public sealed class PricingCalculateItemResponse
{
    /// <summary>
    /// 单条费用明细的请求号，用于和调用方传入的 itemRequestNo 对齐。
    /// </summary>
    public string? ItemRequestNo { get; init; }
    /// <summary>
    /// 收费明细号，用于定位单条收费项目。
    /// </summary>
    public string? ChargeDetailNo { get; init; }
    /// <summary>
    /// 计价追踪号，与响应根对象 TraceId 一致。
    /// </summary>
    public string? TraceId { get; init; }
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用。
    /// </summary>
    public long RequestId { get; init; }
    /// <summary>
    /// 本条费用使用的运行时包主键。
    /// </summary>
    public long? RuntimePackageId { get; init; }
    /// <summary>
    /// 本条费用使用的运行时包版本号。
    /// </summary>
    public long? RuntimePackageVersion { get; init; }
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度。
    /// 该字段属于单条费用明细响应；多明细请求通过 Items 返回多条结果，响应根对象不再放单个 ItemCode。
    /// </summary>
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明。
    /// </summary>
    public string? ItemName { get; init; }
    /// <summary>
    /// 是否命中特殊计价规则。
    /// </summary>
    public bool IsSpecialItem { get; init; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖。
    /// </summary>
    public decimal InputQty { get; init; }
    /// <summary>
    /// 最终可收费数量。
    /// </summary>
    public decimal FinalQty { get; init; }
    /// <summary>
    /// 双单位换算后的计价数量。
    /// </summary>
    public decimal ConvertedQty { get; init; }
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验。
    /// </summary>
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 原始金额，等于原始数量 × 单价。
    /// </summary>
    public decimal OriginalAmount { get; init; }
    /// <summary>
    /// 最终可收费金额。
    /// </summary>
    public decimal FinalAmount { get; init; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>
    /// 超出限额的数量。
    /// </summary>
    public decimal ExceedQty { get; init; }
    /// <summary>
    /// REPLACE 模式下的替换子项信息。
    /// </summary>
    public PricingReplacementItemResponse? ReplacementItem { get; init; }
    /// <summary>
    /// ADD_CHILD_ITEM 动作生成的普通加收子项集合。
    /// </summary>
    public IReadOnlyList<PricingChildItemResponse> ChildItems { get; init; } = Array.Empty<PricingChildItemResponse>();
    /// <summary>
    /// 本条费用计价追踪步骤，用于接口调用方展示或排查。
    /// </summary>
    public IReadOnlyList<PricingTraceStepResponse> TraceSteps { get; init; } = Array.Empty<PricingTraceStepResponse>();
    /// <summary>
    /// 本条费用命中的规则主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();
    /// <summary>
    /// 本条费用命中的运行时规则主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedRuntimeRuleIds { get; init; } = Array.Empty<long>();
    /// <summary>
    /// 本条费用命中的来源策略版本主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedPolicyVersionIds { get; init; } = Array.Empty<long>();
    /// <summary>
    /// 本条费用命中的来源模板版本主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedTemplateVersionIds { get; init; } = Array.Empty<long>();
}

/// <summary>
/// 超限替换子项响应 DTO。
/// </summary>
public sealed class PricingReplacementItemResponse
{
    /// <summary>
    /// 替换子项编码。
    /// </summary>
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 替换子项名称。
    /// </summary>
    public string? ItemName { get; init; }
    /// <summary>
    /// 替换数量。
    /// </summary>
    public decimal Qty { get; init; }
    /// <summary>
    /// 替换单价。
    /// </summary>
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 替换金额。
    /// </summary>
    public decimal Amount { get; init; }
}

/// <summary>
/// 普通子项加收响应 DTO。
/// </summary>
public sealed class PricingChildItemResponse
{
    /// <summary>
    /// 子项编码。
    /// </summary>
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 子项名称。
    /// </summary>
    public string? ItemName { get; init; }
    /// <summary>
    /// 子项数量。
    /// </summary>
    public decimal Qty { get; init; }
    /// <summary>
    /// 子项单价。
    /// </summary>
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 子项金额。
    /// </summary>
    public decimal Amount { get; init; }
    /// <summary>
    /// 是否与主项目共享限额。
    /// </summary>
    public bool ShareParentLimit { get; init; }
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
    /// <summary>
    /// 产生本步骤的运行时规则主键。
    /// </summary>
    public long? RuntimeRuleId { get; init; }
    /// <summary>
    /// 产生本步骤的来源策略版本主键。
    /// </summary>
    public long? SourcePolicyVersionId { get; init; }
    /// <summary>
    /// 产生本步骤的来源模板版本主键。
    /// </summary>
    public long? SourceTemplateVersionId { get; init; }
}
