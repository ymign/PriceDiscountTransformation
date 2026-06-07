namespace Pricing.RuleCenter.Application.Dto;

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

    /// <summary>
    /// 当前有效规则中最保守的故障降级模式。
    /// </summary>
    /// <remarks>
    /// 渠道在计价服务不可用时必须按该字段处理特殊项目，不能自行回退为普通计价。
    /// 常见值：STOP_CHARGE、MANUAL_REVIEW、LEGACY_EQUIVALENT。
    /// </remarks>
    public string RollbackMode { get; init; } = "STOP_CHARGE";
}
