using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

internal sealed record PricingContextBuildInput
{
    public PricingCalculateRequest Request { get; init; } = null!;

    public PricingCalculateItemRequest Item { get; init; } = null!;

    public string CallType { get; init; } = string.Empty;

    public bool ShouldLockLimits { get; init; }

    public IReadOnlyDictionary<string, decimal>? InRequestOccupiedQtyByLimitDimension { get; init; }

    public IReadOnlyList<LimitOccupy>? InRequestLimitOccupies { get; init; }
}

internal static class PricingContextFactory
{
    public static PricingContext Create(PricingContextBuildInput input)
    {
        var request = input.Request;
        var item = input.Item;

        return new PricingContext
        {
            CallType = input.CallType,
            ShouldLockLimits = input.ShouldLockLimits,
            PatientId = request.PatientId.Trim(),
            VisitId = NormalizeString(request.VisitId),
            ItemCode = item.ItemCode.Trim(),
            ItemName = NormalizeString(item.ItemName),
            InputQty = item.InputQty,
            Unit = NormalizeString(item.Unit),
            UnitPrice = item.UnitPrice,
            BodyPartCode = NormalizeString(item.BodyPartCode),
            ChargeScene = NormalizeString(request.ChargeScene),
            ItemGroupCode = NormalizeString(item.ItemGroupCode),
            VisitType = NormalizeString(request.VisitType),
            PatientAge = request.PatientAge,
            BusinessChargeTime = item.BusinessChargeTime ?? request.BusinessChargeTime,
            SourceSystem = request.SourceSystem.Trim(),
            ChargeNo = NormalizeString(request.ChargeNo),
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            ChargeDeptCode = NormalizeString(request.ChargeDeptCode),
            LegacyOccupiedQty = item.LegacyOccupiedQty ?? 0m,
            ExtraParams = MergeExtraParams(request.ExtraParams, item.ExtraParams),
            InRequestOccupiedQtyByLimitDimension =
                input.InRequestOccupiedQtyByLimitDimension?.ToDictionary(
                    item => item.Key,
                    item => item.Value,
                    StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, decimal>(),
            InRequestLimitOccupies = input.InRequestLimitOccupies is null
                ? Array.Empty<LimitOccupy>()
                : input.InRequestLimitOccupies.ToList(),
            PricingParts = item.PricingParts?.Select(p => new PricingPartItem
            {
                PartSeq = p.PartSeq,
                PartCode = NormalizeString(p.PartCode),
                PartName = NormalizeString(p.PartName),
                BodyPartCode = NormalizeString(p.BodyPartCode),
                Qty = p.Qty,
                Area = p.Area,
                MeasureType = NormalizeString(p.MeasureType),
                MeasureValue = p.MeasureValue,
                MeasureUnit = NormalizeString(p.MeasureUnit),
                LesionCount = p.LesionCount
            }).ToList()
        };
    }

    private static IReadOnlyDictionary<string, string>? MergeExtraParams(
        IReadOnlyDictionary<string, object?>? requestParams,
        IReadOnlyDictionary<string, object?>? itemParams)
    {
        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddExtraParams(merged, requestParams);
        AddExtraParams(merged, itemParams);
        return merged.Count == 0 ? null : merged;
    }

    private static void AddExtraParams(
        Dictionary<string, string> target,
        IReadOnlyDictionary<string, object?>? source)
    {
        if (source is null)
        {
            return;
        }

        foreach (var pair in source)
        {
            var key = NormalizeString(pair.Key);
            if (key is null)
            {
                continue;
            }

            var normalizedValue = PricingRequestFingerprintBuilder.NormalizeExtraValue(pair.Value);
            var textValue = NormalizeString(normalizedValue?.ToString());
            if (textValue is not null)
            {
                target[key] = textValue;
            }
        }
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
