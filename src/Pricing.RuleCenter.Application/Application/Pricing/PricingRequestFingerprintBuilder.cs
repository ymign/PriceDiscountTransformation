using System.Security.Cryptography;
using Pricing.RuleCenter.Core.Constants;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

internal static class PricingRequestFingerprintBuilder
{
    public static string BuildConfirmFingerprint(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        string callType)
    {
        var payload = new
        {
            sourceSystem = NormalizeString(request.SourceSystem),
            businessRequestNo = NormalizeString(request.BusinessRequestNo),
            callType,
            patientId = NormalizeString(request.PatientId),
            visitId = NormalizeString(request.VisitId),
            visitType = NormalizeString(request.VisitType),
            patientAge = request.PatientAge,
            encounterNo = NormalizeString(request.EncounterNo),
            chargeScene = NormalizeString(request.ChargeScene),
            chargeNo = NormalizeString(request.ChargeNo),
            chargeTime = request.BusinessChargeTime,
            operationNo = GetExtraParam(request.ExtraParams, "operationNo"),
            pregnancyNo = GetExtraParam(request.ExtraParams, "pregnancyNo"),
            mainChargeDetailNo = GetExtraParam(request.ExtraParams, "mainChargeDetailNo"),
            extraParams = NormalizeExtraParams(request.ExtraParams),
            items = items
                .OrderBy(i => i.ChargeDetailNo)
                .ThenBy(i => i.ItemRequestNo)
                .ThenBy(i => i.ItemCode)
                .Select(i => new
                {
                    itemRequestNo = NormalizeString(i.ItemRequestNo),
                    chargeDetailNo = NormalizeString(i.ChargeDetailNo),
                    itemCode = NormalizeString(i.ItemCode),
                    itemName = NormalizeString(i.ItemName),
                    itemGroupCode = NormalizeString(i.ItemGroupCode),
                    inputQty = Math.Round(i.InputQty, 4),
                    inputUnit = NormalizeString(i.Unit),
                    unitPrice = Math.Round(i.UnitPrice, 4),
                    chargeTime = i.BusinessChargeTime ?? request.BusinessChargeTime,
                    bodyPartCode = NormalizeString(i.BodyPartCode),
                    operationNo = GetExtraParam(i.ExtraParams, "operationNo"),
                    pregnancyNo = GetExtraParam(i.ExtraParams, "pregnancyNo"),
                    mainChargeDetailNo = GetExtraParam(i.ExtraParams, "mainChargeDetailNo"),
                    extraParams = NormalizeExtraParams(i.ExtraParams),
                    pricingParts = i.PricingParts?
                        .OrderBy(p => p.PartSeq ?? int.MaxValue)
                        .ThenBy(p => p.PartCode)
                        .Select(p => new
                        {
                            partSeq = p.PartSeq,
                            partCode = NormalizeString(p.PartCode),
                            partName = NormalizeString(p.PartName),
                            bodyPartCode = NormalizeString(p.BodyPartCode),
                            qty = Math.Round(p.Qty, 4),
                            area = p.Area.HasValue ? Math.Round(p.Area.Value, 4) : (decimal?)null,
                            measureType = NormalizeString(p.MeasureType),
                            measureValue = p.MeasureValue.HasValue ? Math.Round(p.MeasureValue.Value, 4) : (decimal?)null,
                            measureUnit = NormalizeString(p.MeasureUnit),
                            lesionCount = p.LesionCount
                        })
                        .ToList()
                })
                .ToList()
        };

        return HashPayload(payload);
    }

    public static string BuildReverseFingerprint(
        PricingReverseRequest request,
        ChargeRequest originalLog,
        DateTime reverseTime)
    {
        var payload = new
        {
            sourceSystem = NormalizeString(request.SourceSystem) ?? NormalizeString(originalLog.SourceSystem),
            sourceTerminal = NormalizeString(request.SourceTerminal) ?? NormalizeString(originalLog.SourceTerminal),
            callType = PricingCallTypeCodes.Reverse,
            originalRequestId = request.OriginalRequestId,
            reverseNo = NormalizeString(request.ReverseNo),
            chargeNo = NormalizeString(originalLog.ChargeNo),
            chargeDetailNo = NormalizeString(request.ChargeDetailNo),
            itemCode = NormalizeString(request.ItemCode),
            partSeq = request.PartSeq,
            reverseTime,
            reverseQty = request.ReverseQty.HasValue ? Math.Round(request.ReverseQty.Value, 4) : (decimal?)null,
            reverseAmt = request.ReverseAmt.HasValue ? PricingAmountRounder.RoundFinal(request.ReverseAmt.Value) : (decimal?)null,
            reversedBy = NormalizeString(request.ReversedBy),
            reason = NormalizeString(request.Reason)
        };

        return HashPayload(payload);
    }

    public static object? NormalizeExtraValue(object? value)
    {
        return value switch
        {
            null => null,
            string text => NormalizeString(text),
            decimal number => Math.Round(number, 4),
            double number => Math.Round((decimal)number, 4),
            float number => Math.Round((decimal)number, 4),
            JValue jValue => jValue.Type == JTokenType.String
                ? NormalizeString(jValue.Value<string>())
                : jValue.ToString(),
            JToken jToken => jToken.ToString(Formatting.None),
            _ => value
        };
    }

    private static string HashPayload(object payload)
    {
        var json = JsonConvert.SerializeObject(payload, Formatting.None);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }

    private static IReadOnlyDictionary<string, object?>? NormalizeExtraParams(
        IReadOnlyDictionary<string, object?>? extraParams)
    {
        return extraParams?
            .OrderBy(k => k.Key, StringComparer.Ordinal)
            .ToDictionary(k => k.Key.Trim(), k => NormalizeExtraValue(k.Value));
    }

    private static object? GetExtraParam(IReadOnlyDictionary<string, object?>? extraParams, string key)
    {
        if (extraParams is null ||
            !extraParams.TryGetValue(key, out var value))
        {
            return null;
        }

        return NormalizeExtraValue(value);
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
