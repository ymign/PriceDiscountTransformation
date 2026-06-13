using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 特殊计价项目标识查询参数 DTO。
/// </summary>
/// <remarks>
/// 路径参数 itemCode 仍是主定位条件；以下查询参数用于在收费入口提前按场景、业务时间、
/// 就诊类型、部位和收费科室判断规则是否会命中，避免只按项目编码粗判导致不必要弹窗。
/// </remarks>
public sealed class SpecialFlagQueryRequest
{
    /// <summary>
    /// 收费场景编码，例如门诊收费、住院收费、手术划价或医技划价。
    /// </summary>
    [JsonPropertyName("charge_scene")]
    public string? ChargeScene { get; init; }

    /// <summary>
    /// 业务收费发生时间。为空时按计价中心当前技术时间判断生效期。
    /// </summary>
    [JsonPropertyName("business_charge_time")]
    public DateTime? BusinessChargeTime { get; init; }

    /// <summary>
    /// 就诊类型编码，例如 OUTPATIENT、INPATIENT、EMERGENCY。
    /// </summary>
    [JsonPropertyName("visit_type")]
    public string? VisitType { get; init; }

    /// <summary>
    /// 身体部位编码，用于按部位差异化规则提前判断。
    /// </summary>
    [JsonPropertyName("body_part_code")]
    public string? BodyPartCode { get; init; }

    /// <summary>
    /// 收费科室编码，用于排除特定科室的规则提前判断。
    /// </summary>
    [JsonPropertyName("charge_dept_code")]
    public string? ChargeDeptCode { get; init; }
}

/// <summary>
/// 特殊计价项目标识查询完整请求。
/// </summary>
public sealed class SpecialFlagRequest
{
    /// <summary>
    /// 项目编码，是规则匹配、价格诊断和限额累计的核心维度。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;

    /// <inheritdoc cref="SpecialFlagQueryRequest.ChargeScene" />
    [JsonPropertyName("charge_scene")]
    public string? ChargeScene { get; init; }

    /// <summary>
    /// 项目组编码，用于 special-flag 提前模拟项目组相关规则条件。
    /// </summary>
    [JsonPropertyName("item_group_code")]
    public string? ItemGroupCode { get; init; }

    /// <summary>
    /// 诊断用输入数量。用于 special-flag 提前模拟部分规则条件，不作为最终收费数量。
    /// </summary>
    [JsonPropertyName("input_qty")]
    public decimal? InputQty { get; init; }

    /// <summary>
    /// 诊断用录入单位。用于提前模拟单位、部位或扩展条件。
    /// </summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    /// <summary>
    /// 诊断用单价。用于提前模拟金额类条件，不作为最终落账金额。
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal? UnitPrice { get; init; }

    /// <summary>
    /// 诊断用多部位或多片段明细。复杂项目可提前传入，方便排查规则误判。
    /// </summary>
    [JsonPropertyName("pricing_parts")]
    public IReadOnlyList<PricingPartItemRequest>? PricingParts { get; init; }

    /// <inheritdoc cref="SpecialFlagQueryRequest.BusinessChargeTime" />
    [JsonPropertyName("business_charge_time")]
    public DateTime? BusinessChargeTime { get; init; }

    /// <inheritdoc cref="SpecialFlagQueryRequest.VisitType" />
    [JsonPropertyName("visit_type")]
    public string? VisitType { get; init; }

    /// <inheritdoc cref="SpecialFlagQueryRequest.BodyPartCode" />
    [JsonPropertyName("body_part_code")]
    public string? BodyPartCode { get; init; }

    /// <inheritdoc cref="SpecialFlagQueryRequest.ChargeDeptCode" />
    [JsonPropertyName("charge_dept_code")]
    public string? ChargeDeptCode { get; init; }

    /// <summary>
    /// 扩展上下文参数，用于承载手术号、孕次号、设备类型等暂未结构化但可能参与规则条件的字段。
    /// </summary>
    [JsonPropertyName("extra_params")]
    public Dictionary<string, object?>? ExtraParams { get; init; }

    /// <summary>
    /// 从路径 itemCode 和查询参数构造完整请求。
    /// </summary>
    public static SpecialFlagRequest From(string itemCode, SpecialFlagQueryRequest? query)
    {
        return new SpecialFlagRequest
        {
            ItemCode = itemCode,
            ChargeScene = query?.ChargeScene,
            BusinessChargeTime = query?.BusinessChargeTime,
            VisitType = query?.VisitType,
            BodyPartCode = query?.BodyPartCode,
            ChargeDeptCode = query?.ChargeDeptCode
        };
    }
}

/// <summary>
/// 批量特殊计价项目标识查询请求。
/// </summary>
/// <remarks>
/// 一次收费动作通常包含多条费用明细。批量接口在同一个请求里判断多项目是否需要进入统一计价，
/// 并保留收费动作级上下文，便于后续按请求号、患者、收费单和操作员排查误判问题。
/// </remarks>
public sealed class SpecialFlagBatchRequest
{
    /// <summary>调用方技术请求流水号，用于定位一次 HTTP 调用。</summary>
    [JsonPropertyName("request_no")]
    public string? RequestNo { get; init; }

    /// <summary>来源系统编码，例如 HIS、SELF_MACHINE、WECHAT。</summary>
    [JsonPropertyName("source_system")]
    public string? SourceSystem { get; init; }

    /// <summary>来源终端、站点或服务实例标识。</summary>
    [JsonPropertyName("source_terminal")]
    public string? SourceTerminal { get; init; }

    /// <summary>患者 ID，用于排查具体患者收费动作。</summary>
    [JsonPropertyName("patient_id")]
    public string? PatientId { get; init; }

    /// <summary>就诊 ID，用于区分同一患者多次就诊。</summary>
    [JsonPropertyName("visit_id")]
    public string? VisitId { get; init; }

    /// <summary>默认就诊类型，明细未传时使用该值。</summary>
    [JsonPropertyName("visit_type")]
    public string? VisitType { get; init; }

    /// <summary>门诊号、住院号或就诊流水号。</summary>
    [JsonPropertyName("encounter_no")]
    public string? EncounterNo { get; init; }

    /// <summary>默认收费场景，明细未传时使用该值。</summary>
    [JsonPropertyName("charge_scene")]
    public string? ChargeScene { get; init; }

    /// <summary>默认收费科室编码，明细未传时使用该值。</summary>
    [JsonPropertyName("charge_dept_code")]
    public string? ChargeDeptCode { get; init; }

    /// <summary>收费单号，用于和 HIS 收费动作关联。</summary>
    [JsonPropertyName("charge_no")]
    public string? ChargeNo { get; init; }

    /// <summary>
    /// 调用方稳定业务请求号。special-flags 中该字段可选，仅用于诊断和串联日志，不做幂等校验。
    /// </summary>
    [JsonPropertyName("business_request_no")]
    public string? BusinessRequestNo { get; init; }

    /// <summary>默认业务收费发生时间，明细未传时使用该值；为空时使用计价中心当前时间。</summary>
    [JsonPropertyName("business_charge_time")]
    public DateTime? BusinessChargeTime { get; init; }

    /// <summary>操作员编码。</summary>
    [JsonPropertyName("operator_id")]
    public string? OperatorId { get; init; }

    /// <summary>操作员姓名。</summary>
    [JsonPropertyName("operator_name")]
    public string? OperatorName { get; init; }

    /// <summary>收费动作级扩展参数。明细级同名参数会覆盖该值。</summary>
    [JsonPropertyName("extra_params")]
    public Dictionary<string, object?>? ExtraParams { get; init; }

    /// <summary>本次收费动作内需要判断特殊标识的费用明细集合。</summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<SpecialFlagBatchItemRequest> Items { get; init; } = Array.Empty<SpecialFlagBatchItemRequest>();
}

/// <summary>
/// 批量特殊计价项目标识查询的单条费用明细请求。
/// </summary>
public sealed class SpecialFlagBatchItemRequest
{
    /// <summary>调用方单条费用请求号，用于响应行关联。</summary>
    [JsonPropertyName("item_request_no")]
    public string? ItemRequestNo { get; init; }

    /// <summary>收费明细号，用于定位单条收费项目。</summary>
    [JsonPropertyName("charge_detail_no")]
    public string? ChargeDetailNo { get; init; }

    /// <summary>项目编码，是特殊项目判断的主定位条件。</summary>
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;

    /// <summary>项目名称，用于响应展示和排查日志。</summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }

    /// <summary>项目组编码，用于保留同组规则排查上下文。</summary>
    [JsonPropertyName("item_group_code")]
    public string? ItemGroupCode { get; init; }

    /// <summary>诊断用输入数量。用于提前模拟部分规则条件，不作为最终收费数量。</summary>
    [JsonPropertyName("input_qty")]
    public decimal? InputQty { get; init; }

    /// <summary>诊断用录入单位。用于提前模拟单位、部位或扩展条件。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    /// <summary>诊断用单价。用于提前模拟金额类条件，不作为最终落账金额。</summary>
    [JsonPropertyName("unit_price")]
    public decimal? UnitPrice { get; init; }

    /// <summary>诊断用多部位或多片段明细。复杂项目可提前传入，方便排查规则误判。</summary>
    [JsonPropertyName("pricing_parts")]
    public IReadOnlyList<PricingPartItemRequest>? PricingParts { get; init; }

    /// <summary>明细级收费场景；为空时使用批量请求的 charge_scene。</summary>
    [JsonPropertyName("charge_scene")]
    public string? ChargeScene { get; init; }

    /// <summary>明细级业务收费发生时间；为空时使用批量请求的 business_charge_time。</summary>
    [JsonPropertyName("business_charge_time")]
    public DateTime? BusinessChargeTime { get; init; }

    /// <summary>明细级就诊类型；为空时使用批量请求的 visit_type。</summary>
    [JsonPropertyName("visit_type")]
    public string? VisitType { get; init; }

    /// <summary>身体部位编码，用于按部位差异化判断规则是否命中。</summary>
    [JsonPropertyName("body_part_code")]
    public string? BodyPartCode { get; init; }

    /// <summary>明细级收费科室编码；为空时使用批量请求的 charge_dept_code。</summary>
    [JsonPropertyName("charge_dept_code")]
    public string? ChargeDeptCode { get; init; }

    /// <summary>单条费用扩展参数。与批量级 extra_params 合并后进入规则上下文。</summary>
    [JsonPropertyName("extra_params")]
    public Dictionary<string, object?>? ExtraParams { get; init; }
}

/// <summary>
/// 批量特殊计价项目标识查询响应。
/// </summary>
public sealed class SpecialFlagBatchResponse
{
    /// <summary>调用方技术请求流水号，原样返回便于排查。</summary>
    [JsonPropertyName("request_no")]
    public string? RequestNo { get; init; }

    /// <summary>调用方稳定业务请求号，原样返回便于排查。</summary>
    [JsonPropertyName("business_request_no")]
    public string? BusinessRequestNo { get; init; }

    /// <summary>本批次是否存在任一特殊项目。</summary>
    [JsonPropertyName("is_special")]
    public bool IsSpecial { get; init; }

    /// <summary>本批次费用明细总数。</summary>
    [JsonPropertyName("item_count")]
    public int ItemCount { get; init; }

    /// <summary>本批次命中特殊计价规则的费用明细数量。</summary>
    [JsonPropertyName("special_item_count")]
    public int SpecialItemCount { get; init; }

    /// <summary>本批次建议调用方执行的下一步动作。</summary>
    [JsonPropertyName("next_action")]
    public string NextAction { get; init; } = PricingNextActionCodes.NormalPricing;

    /// <summary>本批次决策原因，面向排查和日志展示。</summary>
    [JsonPropertyName("decision_reason")]
    public string DecisionReason { get; init; } = string.Empty;

    /// <summary>是否阻断普通收费流程。存在特殊项目时应先进入统一计价。</summary>
    [JsonPropertyName("blocking")]
    public bool Blocking { get; init; }

    /// <summary>本次判断读取当前规则的时间。</summary>
    [JsonPropertyName("rule_read_time")]
    public DateTime RuleReadTime { get; init; }

    /// <summary>逐费用明细特殊标识判断结果。</summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<SpecialFlagBatchItemResponse> Items { get; init; } = Array.Empty<SpecialFlagBatchItemResponse>();
}

/// <summary>
/// 特殊项目命中规则摘要。
/// </summary>
public sealed class SpecialFlagMatchedRuleResponse
{
    /// <summary>规则主键。</summary>
    [JsonPropertyName("rule_id")]
    public long RuleId { get; init; }

    /// <summary>规则编码。</summary>
    [JsonPropertyName("rule_code")]
    public string? RuleCode { get; init; }

    /// <summary>规则名称。</summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; init; }

    /// <summary>计价服务不可用时该规则的降级处理模式。</summary>
    [JsonPropertyName("rollback_mode")]
    public string RollbackMode { get; init; } = "STOP_CHARGE";
}

/// <summary>
/// 批量特殊计价项目标识查询的单条费用明细响应。
/// </summary>
public sealed class SpecialFlagBatchItemResponse
{
    /// <inheritdoc cref="SpecialFlagBatchItemRequest.ItemRequestNo" />
    [JsonPropertyName("item_request_no")]
    public string? ItemRequestNo { get; init; }

    /// <inheritdoc cref="SpecialFlagBatchItemRequest.ChargeDetailNo" />
    [JsonPropertyName("charge_detail_no")]
    public string? ChargeDetailNo { get; init; }

    /// <inheritdoc cref="SpecialFlagResponse.ItemCode" />
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;

    /// <inheritdoc cref="SpecialFlagBatchItemRequest.ItemName" />
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }

    /// <inheritdoc cref="SpecialFlagBatchItemRequest.ItemGroupCode" />
    [JsonPropertyName("item_group_code")]
    public string? ItemGroupCode { get; init; }

    /// <inheritdoc cref="SpecialFlagResponse.IsSpecial" />
    [JsonPropertyName("is_special")]
    public bool IsSpecial { get; init; }

    /// <inheritdoc cref="SpecialFlagResponse.RuleCount" />
    [JsonPropertyName("rule_count")]
    public int RuleCount { get; init; }

    /// <inheritdoc cref="SpecialFlagResponse.RollbackMode" />
    [JsonPropertyName("rollback_mode")]
    public string RollbackMode { get; init; } = "STOP_CHARGE";

    /// <inheritdoc cref="SpecialFlagResponse.MatchedRuleIds" />
    [JsonPropertyName("matched_rule_ids")]
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();

    /// <inheritdoc cref="SpecialFlagResponse.MatchedRules" />
    [JsonPropertyName("matched_rules")]
    public IReadOnlyList<SpecialFlagMatchedRuleResponse> MatchedRules { get; init; } =
        Array.Empty<SpecialFlagMatchedRuleResponse>();

    /// <inheritdoc cref="SpecialFlagResponse.NextAction" />
    [JsonPropertyName("next_action")]
    public string NextAction { get; init; } = PricingNextActionCodes.NormalPricing;

    /// <inheritdoc cref="SpecialFlagResponse.DecisionReason" />
    [JsonPropertyName("decision_reason")]
    public string DecisionReason { get; init; } = string.Empty;

    /// <inheritdoc cref="SpecialFlagResponse.Blocking" />
    [JsonPropertyName("blocking")]
    public bool Blocking { get; init; }

    /// <inheritdoc cref="SpecialFlagResponse.RuleReadTime" />
    [JsonPropertyName("rule_read_time")]
    public DateTime RuleReadTime { get; init; }

    /// <summary>本行最终用于规则匹配的收费场景。</summary>
    [JsonPropertyName("effective_charge_scene")]
    public string? EffectiveChargeScene { get; init; }

    /// <summary>本行最终用于规则匹配的业务收费发生时间。</summary>
    [JsonPropertyName("effective_business_charge_time")]
    public DateTime EffectiveBusinessChargeTime { get; init; }

    /// <summary>本行最终用于规则匹配的就诊类型。</summary>
    [JsonPropertyName("effective_visit_type")]
    public string? EffectiveVisitType { get; init; }

    /// <summary>本行最终用于规则匹配的身体部位编码。</summary>
    [JsonPropertyName("effective_body_part_code")]
    public string? EffectiveBodyPartCode { get; init; }

    /// <summary>本行最终用于规则匹配的收费科室编码。</summary>
    [JsonPropertyName("effective_charge_dept_code")]
    public string? EffectiveChargeDeptCode { get; init; }

    /// <summary>本行最终进入规则上下文的扩展参数。</summary>
    [JsonPropertyName("effective_extra_params")]
    public IReadOnlyDictionary<string, string>? EffectiveExtraParams { get; init; }
}

/// <summary>
/// 特殊计价项目标识响应 DTO。
/// </summary>
public sealed class SpecialFlagResponse
{
    /// <summary>
    /// 项目编码，是规则匹配、价格诊断和限额累计的核心维度
    /// </summary>
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;
    /// <summary>
    /// 是否存在已发布且启用的特殊计价规则。
    /// </summary>
    [JsonPropertyName("is_special")]
    public bool IsSpecial { get; init; }
    /// <summary>
    /// 当前项目命中的有效规则数量。
    /// </summary>
    [JsonPropertyName("rule_count")]
    public int RuleCount { get; init; }

    /// <summary>
    /// 当前有效规则中最保守的故障降级模式。
    /// </summary>
    /// <remarks>
    /// 渠道在计价服务不可用时必须按该字段处理特殊项目，不能自行回退为普通计价。
    /// 常见值：STOP_CHARGE、MANUAL_REVIEW、LEGACY_EQUIVALENT。
    /// </remarks>
    [JsonPropertyName("rollback_mode")]
    public string RollbackMode { get; init; } = "STOP_CHARGE";

    /// <summary>
    /// 本次查询命中的规则主键集合。
    /// </summary>
    [JsonPropertyName("matched_rule_ids")]
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// 本次查询命中的规则摘要，包含规则编码、名称和降级模式，便于调用方展示和排查。
    /// </summary>
    [JsonPropertyName("matched_rules")]
    public IReadOnlyList<SpecialFlagMatchedRuleResponse> MatchedRules { get; init; } =
        Array.Empty<SpecialFlagMatchedRuleResponse>();

    /// <summary>
    /// 建议调用方执行的下一步动作。普通项目为 NORMAL_PRICING，特殊项目为 CALL_SIMULATE。
    /// </summary>
    [JsonPropertyName("next_action")]
    public string NextAction { get; init; } = PricingNextActionCodes.NormalPricing;

    /// <summary>
    /// 本次判断的业务原因说明。
    /// </summary>
    [JsonPropertyName("decision_reason")]
    public string DecisionReason { get; init; } = string.Empty;

    /// <summary>
    /// 是否阻断普通收费流程。特殊项目必须先进入统一计价。
    /// </summary>
    [JsonPropertyName("blocking")]
    public bool Blocking { get; init; }

    /// <summary>
    /// 本次判断读取当前规则的时间。
    /// </summary>
    [JsonPropertyName("rule_read_time")]
    public DateTime RuleReadTime { get; init; }
}
