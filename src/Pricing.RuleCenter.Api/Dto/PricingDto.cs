using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class PricingCalculateRequest
{
    [Required(ErrorMessage = "患者ID不能为空")]
    public string PatientId { get; init; } = string.Empty;

    public string? VisitId { get; init; }

    [Required(ErrorMessage = "项目编码不能为空")]
    public string ItemCode { get; init; } = string.Empty;

    public string? ItemName { get; init; }

    [Required(ErrorMessage = "数量不能为空")]
    public decimal InputQty { get; init; }

    public string? Unit { get; init; }
    public decimal UnitPrice { get; init; }
    public string? BodyPartCode { get; init; }
    public string? ChargeScene { get; init; }
    public DateTime BusinessChargeTime { get; init; }

    [Required(ErrorMessage = "来源系统不能为空")]
    public string SourceSystem { get; init; } = string.Empty;

    public string? ChargeNo { get; init; }
    public string? BusinessRequestNo { get; init; }
    public IReadOnlyList<PricingPartItemRequest>? PricingParts { get; init; }
}

public sealed class PricingPartItemRequest
{
    public string? PartCode { get; init; }
    public string? PartName { get; init; }
    public decimal Qty { get; init; }
    public decimal? Area { get; init; }
}

public sealed class PricingCalculateResponse
{
    public long RequestId { get; init; }
    public bool IsSpecialItem { get; init; }
    public decimal InputQty { get; init; }
    public decimal FinalQty { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal FinalAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public IReadOnlyList<PricingTraceStepResponse> TraceSteps { get; init; } = Array.Empty<PricingTraceStepResponse>();
    public IReadOnlyList<long> MatchedRuleIds { get; init; } = Array.Empty<long>();
}

public sealed class PricingTraceStepResponse
{
    public int StepNo { get; init; }
    public string StepType { get; init; } = string.Empty;
    public string? StepDesc { get; init; }
    public decimal? InputValue { get; init; }
    public decimal? OutputValue { get; init; }
}

public sealed class PricingCommitRequest
{
    [Required(ErrorMessage = "请求ID不能为空")]
    public long RequestId { get; init; }

    public string? ChargeNo { get; init; }
}

public sealed class PricingCancelRequest
{
    [Required(ErrorMessage = "请求ID不能为空")]
    public long RequestId { get; init; }
}

public sealed class PricingReverseRequest
{
    [Required(ErrorMessage = "原请求ID不能为空")]
    public long OriginalRequestId { get; init; }

    public string? Reason { get; init; }
}

public sealed class SpecialFlagResponse
{
    public string ItemCode { get; init; } = string.Empty;
    public bool IsSpecial { get; init; }
    public int RuleCount { get; init; }
}
