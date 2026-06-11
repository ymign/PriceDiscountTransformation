using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 计价请求指纹构建器，用于把业务请求规范化后生成不可逆哈希。
/// </summary>
/// <remarks>
/// <para>
/// 指纹用于 confirm/reverse 幂等冲突判断。相同业务号再次请求时，如果指纹一致，说明是同一次业务动作重试；
/// 如果指纹不同，说明调用方复用了业务号但修改了患者、项目、数量、部位、扩展参数或多片段明细，必须拒绝。
/// </para>
/// <para>
/// 这里做的是“规范化 JSON + SHA256”，不是直接序列化原始请求。规范化的目标是排除无意义差异
/// （空白、键顺序、decimal 表示法），同时保留会影响计价结果的业务差异。
/// </para>
/// </remarks>
internal static class PricingRequestFingerprintBuilder
{
    /// <summary>
    /// 构建 confirm 请求指纹。
    /// </summary>
    /// <param name="request">确认计价请求。</param>
    /// <param name="items">已校验的费用明细集合。</param>
    /// <param name="callType">调用类型，confirm 固定为 CONFIRM。</param>
    /// <returns>SHA256 十六进制指纹。</returns>
    public static string BuildConfirmFingerprint(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        string callType)
    {
        // 指纹至少覆盖会影响规则匹配、金额计算、限额维度和追溯事实的字段。
        // requestNo 不纳入指纹：它是技术请求流水，HIS 超时重试时可以变化。
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
                // 多明细请求按稳定键排序，避免调用方 JSON 数组顺序变化造成误判冲突。
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
                        // 多 part 明细按 partSeq 排序，保证同一业务事实生成同一指纹。
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

    /// <summary>
    /// 构建 reverse 请求指纹。
    /// </summary>
    /// <param name="request">退费冲正请求。</param>
    /// <param name="originalLog">原始已落账请求日志。</param>
    /// <param name="reverseTime">最终采用的退费业务时间。</param>
    /// <returns>SHA256 十六进制指纹。</returns>
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

    /// <summary>
    /// 规范化 ExtraParams 中的值。
    /// </summary>
    /// <param name="value">原始扩展参数值，统一按 System.Text.Json 语义规整。</param>
    /// <returns>规范化后的值。</returns>
    /// <remarks>
    /// ExtraParams 同时进入规则上下文和幂等指纹。这里统一处理字符串、decimal、浮点和 JsonElement，
    /// 避免同一业务值因为 JSON 反序列化类型不同导致指纹不一致。
    /// </remarks>
    public static object? NormalizeExtraValue(object? value)
    {
        return value switch
        {
            null => null,
            string text => NormalizeString(text),
            decimal number => Math.Round(number, 4),
            double number => Math.Round((decimal)number, 4),
            float number => Math.Round((decimal)number, 4),
            JsonElement { ValueKind: JsonValueKind.Null or JsonValueKind.Undefined } => null,
            JsonElement { ValueKind: JsonValueKind.String } element => NormalizeString(element.GetString()),
            JsonElement { ValueKind: JsonValueKind.Number } element => element.GetRawText(),
            JsonElement { ValueKind: JsonValueKind.True } => "true",
            JsonElement { ValueKind: JsonValueKind.False } => "false",
            JsonElement { ValueKind: JsonValueKind.Object or JsonValueKind.Array } element =>
                RuleCenterJsonSerializer.SerializeElement(element),
            JsonDocument document => RuleCenterJsonSerializer.SerializeElement(document.RootElement),
            _ => value
        };
    }

    private static string HashPayload(object payload)
    {
        // 使用无格式 JSON 保证哈希输入稳定；SHA256 足够用于冲突检测，不需要可逆加密。
        var json = RuleCenterJsonSerializer.Serialize(payload);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }

    private static IReadOnlyDictionary<string, object?>? NormalizeExtraParams(
        IReadOnlyDictionary<string, object?>? extraParams)
    {
        // 字典键按序排序，排除调用方 JSON 键顺序变化导致的误判。
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
        // 空白字符串按 null 处理，与请求上下文构建器保持一致：null 表示不参与匹配。
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
