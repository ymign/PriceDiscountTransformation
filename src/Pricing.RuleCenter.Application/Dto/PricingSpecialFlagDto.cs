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
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;

    /// <inheritdoc cref="SpecialFlagQueryRequest.ChargeScene" />
    [JsonPropertyName("charge_scene")]
    public string? ChargeScene { get; init; }

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
/// 特殊计价项目标识响应 DTO。
/// </summary>
public sealed class SpecialFlagResponse
{
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
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
    /// 本次查询使用的运行时包主键。为空表示当前按旧规则读模型判断或未激活运行时包。
    /// </summary>
    [JsonPropertyName("runtime_package_id")]
    public long? RuntimePackageId { get; init; }

    /// <summary>
    /// 本次查询使用的运行时包版本号。
    /// </summary>
    [JsonPropertyName("runtime_package_version")]
    public long? RuntimePackageVersion { get; init; }

    /// <summary>
    /// 本次查询命中的规则主键集合。运行时包查询时为运行时规则主键。
    /// </summary>
    [JsonPropertyName("matched_rule_ids")]
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// 本次查询命中的运行时规则主键集合。
    /// </summary>
    [JsonPropertyName("matched_runtime_rule_ids")]
    public IReadOnlyList<long> MatchedRuntimeRuleIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// 本次查询命中的来源策略版本主键集合。
    /// </summary>
    [JsonPropertyName("matched_policy_version_ids")]
    public IReadOnlyList<long> MatchedPolicyVersionIds { get; init; } = Array.Empty<long>();
}
