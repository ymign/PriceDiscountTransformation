using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class TraceQueryRequest
{
    public long? RequestId { get; init; }
    public string? PatientId { get; init; }
    public string? ItemCode { get; init; }
    public string? ChargeNo { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class TraceQueryResponse
{
    public long RequestId { get; init; }
    public string RequestNo { get; init; } = string.Empty;
    public string CallType { get; init; } = string.Empty;
    public string BusinessStatus { get; init; } = string.Empty;
    public string? PatientId { get; init; }
    public string? ItemCode { get; init; }
    public string? ItemName { get; init; }
    public decimal? InputQty { get; init; }
    public DateTime RequestAt { get; init; }
    public string IsSuccess { get; init; } = string.Empty;
    public IReadOnlyList<TraceStepResponse> Steps { get; init; } = Array.Empty<TraceStepResponse>();
    public IReadOnlyList<TraceDiscountResponse> Discounts { get; init; } = Array.Empty<TraceDiscountResponse>();
}

public sealed class TraceStepResponse
{
    public int StepNo { get; init; }
    public string StepType { get; init; } = string.Empty;
    public string? StepDesc { get; init; }
    public string? InputSnapshot { get; init; }
    public string? OutputSnapshot { get; init; }
}

public sealed class TraceDiscountResponse
{
    public long DiscountId { get; init; }
    public string? ItemCode { get; init; }
    public decimal? OriginalQty { get; init; }
    public decimal? FinalQty { get; init; }
    public decimal? OriginalAmt { get; init; }
    public decimal? FinalAmt { get; init; }
    public decimal? DiscountAmt { get; init; }
    public string Status { get; init; } = string.Empty;
}
