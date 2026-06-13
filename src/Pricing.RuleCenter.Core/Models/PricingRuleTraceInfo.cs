namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 计价结果中的命中规则追溯信息。
/// </summary>
/// <remarks>
/// 该对象只承载对外追溯展示需要的规则身份信息，不参与规则计算。
/// </remarks>
public sealed class PricingRuleTraceInfo
{
    /// <summary>
    /// 规则主键，对应 PR_RULE_HEADER.RULE_ID。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 规则编码，用于运维和配置人员识别规则。
    /// </summary>
    public string? RuleCode { get; init; }

    /// <summary>
    /// 规则名称，用于接口调用方展示折价原因。
    /// </summary>
    public string? RuleName { get; init; }
}
