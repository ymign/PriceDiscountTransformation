using System.Text.Json.Serialization;

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
    [JsonPropertyName("items")]
    public IReadOnlyList<PricingCalculateItemResponse> Items { get; init; } = Array.Empty<PricingCalculateItemResponse>();
    /// <summary>
    /// 计价追踪号，用于跨请求主表、步骤、折价明细、限额占用查看同一次计价链路。
    /// </summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; init; }
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    [JsonPropertyName("request_id")]
    public long RequestId { get; init; }

    /// <summary>
    /// 调用方下一步动作编码。simulate 通常返回 CONFIRM_BEFORE_CHARGE，confirm 返回 COMMIT_OR_CANCEL。
    /// </summary>
    [JsonPropertyName("next_action")]
    public string NextAction { get; init; } = string.Empty;

    /// <summary>
    /// 本次请求在计价中心的业务状态，例如 SIMULATED 或 CONFIRM_PENDING。
    /// </summary>
    [JsonPropertyName("business_status")]
    public string? BusinessStatus { get; init; }

    /// <summary>
    /// 本次决策读取规则快照的时间，用于排查规则发布前后结果差异。
    /// </summary>
    [JsonPropertyName("rule_snapshot_time")]
    public DateTime RuleSnapshotTime { get; init; }

    /// <summary>
    /// 是否命中特殊计价规则。
    /// </summary>
    [JsonPropertyName("is_special_item")]
    public bool IsSpecialItem { get; init; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    [JsonPropertyName("input_qty")]
    public decimal InputQty { get; init; }
    /// <summary>
    /// 最终可收费数量
    /// </summary>
    [JsonPropertyName("final_qty")]
    public decimal FinalQty { get; init; }
    /// <summary>
    /// 项目单价，沿用本次请求明细进入计价链路的单价。
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 原始总金额，等于各明细原始数量 × 单价之和。多明细响应应优先读取该字段而非根 UnitPrice。
    /// </summary>
    [JsonPropertyName("total_original_amount")]
    public decimal TotalOriginalAmount { get; init; }
    /// <summary>
    /// 最终总金额，语义等同于兼容字段 FinalAmount。
    /// </summary>
    [JsonPropertyName("total_final_amount")]
    public decimal TotalFinalAmount { get; init; }
    /// <summary>
    /// 总折价金额，语义等同于兼容字段 DiscountAmount。
    /// </summary>
    [JsonPropertyName("total_discount_amount")]
    public decimal TotalDiscountAmount { get; init; }
    /// <summary>
    /// 最终可收费金额。
    /// </summary>
    [JsonPropertyName("final_amount")]
    public decimal FinalAmount { get; init; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    [JsonPropertyName("discount_amount")]
    public decimal DiscountAmount { get; init; }
    /// <summary>
    /// confirm 结果有效期。为空表示本次响应不是正式确认占用结果。
    /// </summary>
    [JsonPropertyName("expire_at")]
    public DateTime? ExpireAt { get; init; }
    /// <summary>
    /// confirm 结果剩余有效秒数。为空表示本次响应不是正式确认占用结果。
    /// </summary>
    [JsonPropertyName("expire_seconds")]
    public int? ExpireSeconds { get; init; }
    /// <summary>
    /// 兼容字段：根层不再返回单条明细的重复追踪步骤，调用方应读取 items[].trace_steps。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("trace_steps")]
    public IReadOnlyList<PricingTraceStepResponse>? TraceSteps { get; init; }
    /// <summary>
    /// 本次计价命中的规则主键集合。
    /// </summary>
    [JsonPropertyName("matched_rule_ids")]
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();
}

/// <summary>
/// 单条费用明细计价响应 DTO。
/// </summary>
public sealed class PricingCalculateItemResponse
{
    /// <summary>
    /// 单条费用明细的请求号，用于和调用方传入的 itemRequestNo 对齐。
    /// </summary>
    [JsonPropertyName("item_request_no")]
    public string? ItemRequestNo { get; init; }
    /// <summary>
    /// 收费明细号，用于定位单条收费项目。
    /// </summary>
    [JsonPropertyName("charge_detail_no")]
    public string? ChargeDetailNo { get; init; }
    /// <summary>
    /// 计价追踪号，与响应根对象 TraceId 一致。
    /// </summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; init; }
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用。
    /// </summary>
    [JsonPropertyName("request_id")]
    public long RequestId { get; init; }
    /// <summary>
    /// 项目编码，是规则匹配、价格诊断和限额累计的核心维度。
    /// 该字段属于单条费用明细响应；多明细请求通过 Items 返回多条结果，响应根对象不再放单个 ItemCode。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明。
    /// </summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }
    /// <summary>
    /// 是否命中特殊计价规则。
    /// </summary>
    [JsonPropertyName("is_special_item")]
    public bool IsSpecialItem { get; init; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖。
    /// </summary>
    [JsonPropertyName("input_qty")]
    public decimal InputQty { get; init; }
    /// <summary>
    /// 最终可收费数量。
    /// </summary>
    [JsonPropertyName("final_qty")]
    public decimal FinalQty { get; init; }
    /// <summary>
    /// 双单位换算后的计价数量。
    /// </summary>
    [JsonPropertyName("converted_qty")]
    public decimal ConvertedQty { get; init; }
    /// <summary>
    /// 项目单价，沿用本条费用明细进入计价链路的单价。
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 原始金额，等于原始数量 × 单价。
    /// </summary>
    [JsonPropertyName("original_amount")]
    public decimal OriginalAmount { get; init; }
    /// <summary>
    /// 最终可收费金额。
    /// </summary>
    [JsonPropertyName("final_amount")]
    public decimal FinalAmount { get; init; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    [JsonPropertyName("discount_amount")]
    public decimal DiscountAmount { get; init; }
    /// <summary>
    /// 超出限额的数量。
    /// </summary>
    [JsonPropertyName("exceed_qty")]
    public decimal ExceedQty { get; init; }
    /// <summary>
    /// REPLACE 模式下的替换子项信息。
    /// </summary>
    [JsonPropertyName("replacement_item")]
    public PricingReplacementItemResponse? ReplacementItem { get; init; }
    /// <summary>
    /// ADD_CHILD_ITEM 动作生成的普通加收子项集合。
    /// </summary>
    [JsonPropertyName("child_items")]
    public IReadOnlyList<PricingChildItemResponse> ChildItems { get; init; } = Array.Empty<PricingChildItemResponse>();
    /// <summary>
    /// 本条费用计价追踪步骤，用于接口调用方展示或排查。
    /// </summary>
    [JsonPropertyName("trace_steps")]
    public IReadOnlyList<PricingTraceStepResponse> TraceSteps { get; init; } = Array.Empty<PricingTraceStepResponse>();
    /// <summary>
    /// 本条费用命中的规则主键集合。
    /// </summary>
    [JsonPropertyName("matched_rule_ids")]
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();
}

/// <summary>
/// 超限替换子项响应 DTO。
/// </summary>
public sealed class PricingReplacementItemResponse
{
    /// <summary>
    /// 替换子项编码。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 替换子项名称。
    /// </summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }
    /// <summary>
    /// 替换数量。
    /// </summary>
    [JsonPropertyName("qty")]
    public decimal Qty { get; init; }
    /// <summary>
    /// 替换单价。
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 替换金额。
    /// </summary>
    [JsonPropertyName("amount")]
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
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 子项名称。
    /// </summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }
    /// <summary>
    /// 子项数量。
    /// </summary>
    [JsonPropertyName("qty")]
    public decimal Qty { get; init; }
    /// <summary>
    /// 子项单价。
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 子项金额。
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    /// <summary>
    /// 是否与主项目共享限额。
    /// </summary>
    [JsonPropertyName("share_parent_limit")]
    public bool ShareParentLimit { get; init; }
}

/// <summary>
/// 计价追踪步骤响应 DTO。
/// </summary>
public sealed class PricingTraceStepResponse
{
    /// <summary>
    /// 节点唯一键，供调用方前端渲染列表时使用。
    /// </summary>
    [JsonPropertyName("node_key")]
    public string NodeKey { get; init; } = string.Empty;

    /// <summary>
    /// 节点标题，例如规则匹配、限额处理、折价处理。
    /// </summary>
    [JsonPropertyName("node_title")]
    public string NodeTitle { get; init; } = string.Empty;

    /// <summary>
    /// 面向业务展示的节点说明。
    /// </summary>
    [JsonPropertyName("node_desc")]
    public string? NodeDesc { get; init; }

    /// <summary>
    /// 步骤序号，按计价链路执行顺序递增
    /// </summary>
    [JsonPropertyName("step_no")]
    public int StepNo { get; init; }
    /// <summary>
    /// 步骤类型，只使用 DDL 允许的 MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR
    /// </summary>
    [JsonPropertyName("step_type")]
    public string StepType { get; init; } = string.Empty;
    /// <summary>
    /// 步骤说明，解释命中规则、限制口径或折价原因
    /// </summary>
    [JsonPropertyName("step_desc")]
    public string? StepDesc { get; init; }
    /// <summary>
    /// 当前步骤处理前的关键数值，通常为数量或金额。
    /// </summary>
    [JsonPropertyName("input_value")]
    public decimal? InputValue { get; init; }
    /// <summary>
    /// 当前步骤处理后的关键数值，通常为数量或金额。
    /// </summary>
    [JsonPropertyName("output_value")]
    public decimal? OutputValue { get; init; }
    /// <summary>
    /// 产生本步骤的规则主键。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public long? RuleId { get; init; }

    /// <summary>
    /// 产生本步骤的规则编码。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rule_code")]
    public string? RuleCode { get; init; }

    /// <summary>
    /// 产生本步骤的规则名称。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; init; }

    /// <summary>
    /// 产生本步骤的动作编码。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("action_code")]
    public string? ActionCode { get; init; }

    /// <summary>
    /// 产生本步骤的动作名称。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("action_name")]
    public string? ActionName { get; init; }

    /// <summary>
    /// 执行器编码，用于区分同一动作类型下的具体公式或策略。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("executor_code")]
    public string? ExecutorCode { get; init; }

    /// <summary>
    /// 输入输出数值的业务类型，例如 AMOUNT、QTY、MATCH_RESULT。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("value_type")]
    public string? ValueType { get; init; }

    /// <summary>
    /// 输入输出数值的单位，例如元、次、个。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("value_unit")]
    public string? ValueUnit { get; init; }

    /// <summary>
    /// 输入值的业务名称。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("input_name")]
    public string? InputName { get; init; }

    /// <summary>
    /// 输出值的业务名称。
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("output_name")]
    public string? OutputName { get; init; }
}
