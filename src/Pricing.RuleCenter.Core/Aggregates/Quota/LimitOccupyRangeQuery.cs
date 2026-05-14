namespace Pricing.RuleCenter.Core.Aggregates.Quota;

/// <summary>
/// 按业务时间范围查询限额占用数量的参数对象。
/// </summary>
/// <remarks>
/// 时间窗限额需要同时携带限制类型、维度编码、开始时间、结束时间和状态集合。
/// 使用参数对象可以避免仓储接口暴露过长参数列表，也便于后续增加院区、收费场景等过滤条件。
/// </remarks>
public sealed class LimitOccupyRangeQuery
{
    /// <summary>
    /// 限制类型，例如 TIME_WINDOW。
    /// </summary>
    public string LimitType { get; init; } = string.Empty;

    /// <summary>
    /// 限制维度编码，例如患者 + 项目组成的窗口维度。
    /// </summary>
    public string LimitDimensionCode { get; init; } = string.Empty;

    /// <summary>
    /// 查询起始业务时间，包含该时间点。
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// 查询结束业务时间，包含该时间点。
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// 需要计入累计的占用状态集合，通常包含 PENDING 和 CONFIRMED。
    /// </summary>
    public IReadOnlyCollection<string> Statuses { get; init; } = Array.Empty<string>();
}
