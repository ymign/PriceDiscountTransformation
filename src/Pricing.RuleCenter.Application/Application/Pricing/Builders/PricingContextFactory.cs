using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

/// <summary>
/// 构建单条费用计价上下文所需的输入。
/// </summary>
/// <remarks>
/// 请求级字段和明细级字段在 HTTP DTO 中分开表达；进入核心引擎前需要合并成单条费用视角的
/// <see cref="PricingContext"/>，让规则匹配器不再关心原始请求结构。
/// </remarks>
internal sealed record PricingContextBuildInput
{
    /// <summary>
    /// 原始计价请求，提供患者、就诊、来源系统、业务收费时间等请求级上下文。
    /// </summary>
    public PricingCalculateRequest Request { get; init; } = null!;

    /// <summary>
    /// 当前正在计价的费用明细。
    /// </summary>
    public PricingCalculateItemRequest Item { get; init; } = null!;

    /// <summary>
    /// 调用类型，例如 SIMULATE 或 CONFIRM，用于规则日志、限额占用和幂等维度。
    /// </summary>
    public string CallType { get; init; } = string.Empty;

    /// <summary>
    /// 是否在规则执行时锁定限额。试算为 <see langword="false"/>，确认计价为 <see langword="true"/>。
    /// </summary>
    public bool ShouldLockLimits { get; init; }

    /// <summary>
    /// 本次请求共享的计价运行态状态。
    /// </summary>
    public RequestSharedPricingState? RequestSharedState { get; init; }
}

/// <summary>
/// 计价上下文工厂，把 HTTP DTO 转换为核心引擎可直接消费的 <see cref="PricingContext"/>。
/// </summary>
/// <remarks>
/// <para>
/// 该工厂是 API 契约和核心引擎之间的边界。DTO 可以保留对 HIS 友好的 snake_case 字段和请求/明细层级，
/// 核心引擎只看到规范化后的单条费用上下文。
/// </para>
/// <para>
/// 所有字符串在这里统一 trim 和空串转 <see langword="null"/>，避免每个 evaluator/executor 重复处理空白值。
/// </para>
/// </remarks>
internal static class PricingContextFactory
{
    /// <summary>
    /// 创建单条费用的计价上下文。
    /// </summary>
    /// <param name="input">计价上下文构建输入。</param>
    /// <returns>核心引擎使用的单条费用上下文。</returns>
    public static PricingContext Create(PricingContextBuildInput input)
    {
        var request = input.Request;
        var item = input.Item;

        // 请求级字段负责描述一次收费动作，明细级字段负责描述具体收费项目。
        // BusinessChargeTime 优先使用明细时间，支持一单多明细但业务发生时间不同的兼容场景。
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
            ChargeScene = NormalizeString(item.ChargeScene) ?? NormalizeString(request.ChargeScene),
            ItemGroupCode = NormalizeString(item.ItemGroupCode),
            VisitType = NormalizeString(item.VisitType) ?? NormalizeString(request.VisitType),
            PatientAge = request.PatientAge,
            BusinessChargeTime = item.BusinessChargeTime ?? request.BusinessChargeTime,
            SourceSystem = request.SourceSystem.Trim(),
            ChargeNo = NormalizeString(request.ChargeNo),
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            ChargeDeptCode = NormalizeString(item.ChargeDeptCode) ?? NormalizeString(request.ChargeDeptCode),
            LegacyOccupiedQty = item.LegacyOccupiedQty ?? 0m,
            ExtraParams = MergeExtraParams(request.ExtraParams, item.ExtraParams),
            RequestSharedState = input.RequestSharedState?.CreateSnapshot() ?? new RequestSharedPricingState(),
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
        // 明细级扩展参数覆盖请求级同名参数，便于一单中不同项目携带不同规则扩展值。
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

            // 与请求指纹使用同一套扩展值规范化逻辑，保证“参与计价的值”和“参与幂等比对的值”口径一致。
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
        // 统一把空串视为 null。规则条件中 null 表示“不参与匹配”，避免空白字符串误命中 EQ 条件。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
