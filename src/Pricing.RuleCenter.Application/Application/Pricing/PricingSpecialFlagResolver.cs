using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.EffectiveRules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 特殊项目标识解析器。
/// </summary>
/// <remarks>
/// special-flag 是渠道决定“是否必须调用统一计价服务”的前置接口。它不能漏判特殊项目：
/// 若漏判，渠道可能按普通价格收费，绕过折价规则、限额和追溯。
/// </remarks>
public sealed class PricingSpecialFlagResolver
{
    private const int MaxBatchItemCount = 50;

    /// <summary>
    /// 规则主档仓储，用于未注入当前规则读取器时按项目粗判。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;

    /// <summary>
    /// 统一时钟，用于 businessChargeTime 未传入时判断规则生效期。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// 条件组匹配器，用于 special-flag 查询时提前按场景、部位、就诊类型等条件预判命中。
    /// </summary>
    private readonly IRuleConditionGroupMatcher? _conditionMatcher;

    /// <summary>
    /// 当前请求可见规则统一读取入口。
    /// </summary>
    private readonly EffectiveRuleReader? _effectiveRuleReader;

    /// <summary>
    /// 初始化特殊项目标识解析器。
    /// </summary>
    /// <param name="headerRepository">规则头仓储，用于读取项目关联规则。</param>
    /// <param name="clock">技术时间提供者，用于按当前时间过滤有效规则。</param>
    /// <param name="conditionMatcher">条件组匹配器，用于按查询维度预判规则命中。</param>
    /// <param name="effectiveRuleReader">统一当前规则读取入口。</param>
    public PricingSpecialFlagResolver(
        IRuleHeaderRepository headerRepository,
        IClock clock,
        IRuleConditionGroupMatcher? conditionMatcher = null,
        EffectiveRuleReader? effectiveRuleReader = null)
    {
        _headerRepository = headerRepository;
        _clock = clock;
        _conditionMatcher = conditionMatcher;
        _effectiveRuleReader = effectiveRuleReader;
    }

    /// <summary>
    /// 解析项目是否属于当前必须进入特殊计价流程的特殊项目。
    /// </summary>
    /// <param name="itemCode">项目编码。</param>
    /// <returns>特殊项目标识响应。</returns>
    public async Task<SpecialFlagResponse> ResolveAsync(string itemCode)
    {
        return await ResolveAsync(new SpecialFlagRequest { ItemCode = itemCode });
    }

    /// <summary>
    /// 解析项目是否属于当前必须进入特殊计价流程的特殊项目。
    /// </summary>
    /// <param name="request">特殊项目查询请求。</param>
    /// <returns>特殊项目标识响应。</returns>
    public async Task<SpecialFlagResponse> ResolveAsync(SpecialFlagRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedItemCode = NormalizeString(request.ItemCode)
            ?? throw new ArgumentException("项目编码不能为空", nameof(request.ItemCode));
        var decisionTime = _clock.Now;
        var businessTime = request.BusinessChargeTime ?? decisionTime;

        if (_effectiveRuleReader is not null)
        {
            return await ResolveFromRuleReaderAsync(normalizedItemCode, request, businessTime, decisionTime);
        }

        // 无当前规则读取器时只按项目、发布状态和生效期粗判。粗判宁可多返回特殊项目，也不能漏判。
        var rules = await _headerRepository.GetByItemCodeAsync(normalizedItemCode);
        var published = rules
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
            .Where(r => r.IsEffectiveAt(businessTime))
            .ToList();

        return BuildPublishedRuleResponse(normalizedItemCode, published, decisionTime);
    }

    /// <summary>
    /// 批量解析本次收费动作中多条费用明细的特殊项目标识。
    /// </summary>
    /// <param name="request">批量特殊项目标识查询请求。</param>
    /// <returns>批量特殊项目标识响应。</returns>
    public async Task<SpecialFlagBatchResponse> ResolveBatchAsync(SpecialFlagBatchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("Items 不能为空", nameof(request.Items));
        }

        if (request.Items.Count > MaxBatchItemCount)
        {
            throw new ArgumentException($"Items 最多支持 {MaxBatchItemCount} 条", nameof(request.Items));
        }

        var responses = new List<SpecialFlagBatchItemResponse>(request.Items.Count);
        for (var index = 0; index < request.Items.Count; index++)
        {
            var item = request.Items[index];
            var normalizedItemCode = NormalizeString(item.ItemCode)
                ?? throw new ArgumentException($"第 {index + 1} 行项目编码不能为空", nameof(request.Items));
            var effectiveChargeScene = NormalizeString(item.ChargeScene) ?? NormalizeString(request.ChargeScene);
            var effectiveBusinessTime = item.BusinessChargeTime ?? request.BusinessChargeTime ?? _clock.Now;
            var effectiveVisitType = NormalizeString(item.VisitType) ?? NormalizeString(request.VisitType);
            var effectiveBodyPartCode = NormalizeString(item.BodyPartCode);
            var effectiveChargeDeptCode = NormalizeString(item.ChargeDeptCode) ?? NormalizeString(request.ChargeDeptCode);
            var effectiveExtraParams = MergeExtraParams(request.ExtraParams, item.ExtraParams);

            var singleResult = await ResolveAsync(new SpecialFlagRequest
            {
                ItemCode = normalizedItemCode,
                ItemGroupCode = NormalizeString(item.ItemGroupCode),
                InputQty = item.InputQty,
                Unit = NormalizeString(item.Unit),
                UnitPrice = item.UnitPrice,
                PricingParts = item.PricingParts,
                ChargeScene = effectiveChargeScene,
                BusinessChargeTime = effectiveBusinessTime,
                VisitType = effectiveVisitType,
                BodyPartCode = effectiveBodyPartCode,
                ChargeDeptCode = effectiveChargeDeptCode,
                ExtraParams = ToObjectDictionary(effectiveExtraParams)
            });

            responses.Add(new SpecialFlagBatchItemResponse
            {
                ItemRequestNo = NormalizeString(item.ItemRequestNo),
                ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
                ItemCode = singleResult.ItemCode,
                ItemName = NormalizeString(item.ItemName),
                ItemGroupCode = NormalizeString(item.ItemGroupCode),
                IsSpecial = singleResult.IsSpecial,
                RuleCount = singleResult.RuleCount,
                RollbackMode = singleResult.RollbackMode,
                MatchedRuleIds = singleResult.MatchedRuleIds,
                MatchedRules = singleResult.MatchedRules,
                NextAction = singleResult.NextAction,
                DecisionReason = singleResult.DecisionReason,
                Blocking = singleResult.Blocking,
                RuleReadTime = singleResult.RuleReadTime,
                EffectiveChargeScene = effectiveChargeScene,
                EffectiveBusinessChargeTime = effectiveBusinessTime,
                EffectiveVisitType = effectiveVisitType,
                EffectiveBodyPartCode = effectiveBodyPartCode,
                EffectiveChargeDeptCode = effectiveChargeDeptCode,
                EffectiveExtraParams = effectiveExtraParams
            });
        }

        var specialItemCount = responses.Count(item => item.IsSpecial);
        var blocking = responses.Any(item => item.Blocking);
        var ruleReadTime = responses.Count == 0
            ? _clock.Now
            : responses.Max(item => item.RuleReadTime);

        return new SpecialFlagBatchResponse
        {
            RequestNo = NormalizeString(request.RequestNo),
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            ItemCount = responses.Count,
            SpecialItemCount = specialItemCount,
            IsSpecial = specialItemCount > 0,
            NextAction = blocking ? PricingNextActionCodes.CallSimulate : PricingNextActionCodes.NormalPricing,
            Blocking = blocking,
            DecisionReason = BuildBatchDecisionReason(responses.Count, specialItemCount),
            RuleReadTime = ruleReadTime,
            Items = responses
        };
    }

    private async Task<SpecialFlagResponse> ResolveFromRuleReaderAsync(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime,
        DateTime decisionTime)
    {
        var ruleSet = await _effectiveRuleReader!.ReadCurrentAsync(normalizedItemCode);
        var context = BuildPricingContext(normalizedItemCode, request, businessTime);
        var matchedRules = new List<EffectiveRuleView>();

        foreach (var rule in ruleSet.Rules)
        {
            if (rule.Header.Status != RuleStatusCodes.Published ||
                rule.Header.IsEnabled != EnableFlag.Yes ||
                !rule.Header.IsEffectiveAt(businessTime))
            {
                continue;
            }

            if (_conditionMatcher is not null &&
                !await _conditionMatcher.EvaluateAsync(rule.Conditions, context))
            {
                continue;
            }

            matchedRules.Add(rule);
        }

        var matchedRuleHeaders = matchedRules
            .Select(rule => rule.Header)
            .ToList();

        return BuildPublishedRuleResponse(normalizedItemCode, matchedRuleHeaders, decisionTime);
    }

    private static SpecialFlagResponse BuildPublishedRuleResponse(
        string normalizedItemCode,
        IReadOnlyList<RuleAggregate> publishedRules,
        DateTime decisionTime)
    {
        var rollbackMode = ResolveRollbackMode(publishedRules);
        var isSpecial = publishedRules.Count > 0;
        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = isSpecial,
            RuleCount = publishedRules.Count,
            RollbackMode = rollbackMode,
            MatchedRuleIds = publishedRules.Select(rule => rule.RuleId).Distinct().ToList(),
            MatchedRules = BuildMatchedRuleResponses(publishedRules),
            NextAction = isSpecial ? PricingNextActionCodes.CallSimulate : PricingNextActionCodes.NormalPricing,
            DecisionReason = BuildItemDecisionReason(publishedRules, rollbackMode),
            Blocking = isSpecial,
            RuleReadTime = decisionTime
        };
    }

    private static PricingContext BuildPricingContext(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime)
    {
        var inputQty = request.InputQty.HasValue && request.InputQty.Value > 0
            ? request.InputQty.Value
            : 1m;
        var unitPrice = request.UnitPrice.GetValueOrDefault();

        // special-flag 只需要规则匹配条件，不进行最终金额计算。
        // 数量、单位、单价和 pricingParts 只作为提前模拟条件的诊断上下文。
        return new PricingContext
        {
            CallType = "SPECIAL_FLAG",
            PatientId = "-",
            ItemCode = normalizedItemCode,
            InputQty = inputQty,
            ConvertedQty = inputQty,
            FinalQty = inputQty,
            Unit = NormalizeString(request.Unit),
            UnitPrice = unitPrice,
            FinalAmount = inputQty * unitPrice,
            ChargeScene = NormalizeString(request.ChargeScene),
            BusinessChargeTime = businessTime,
            SourceSystem = "SPECIAL_FLAG_QUERY",
            ItemGroupCode = NormalizeString(request.ItemGroupCode),
            ExtraParams = NormalizeExtraParams(request.ExtraParams),
            BodyPartCode = NormalizeString(request.BodyPartCode),
            VisitType = NormalizeString(request.VisitType),
            ChargeDeptCode = NormalizeString(request.ChargeDeptCode),
            PricingParts = request.PricingParts?.Select(p => new PricingPartItem
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

    private static IReadOnlyList<SpecialFlagMatchedRuleResponse> BuildMatchedRuleResponses(
        IReadOnlyList<RuleAggregate> publishedRules)
    {
        return publishedRules
            .GroupBy(rule => rule.RuleId)
            .Select(group =>
            {
                var rule = group.First();
                return new SpecialFlagMatchedRuleResponse
                {
                    RuleId = rule.RuleId,
                    RuleCode = NormalizeString(rule.RuleCode),
                    RuleName = NormalizeString(rule.RuleName),
                    RollbackMode = NormalizeString(rule.RollbackMode) ?? "STOP_CHARGE"
                };
            })
            .ToList();
    }

    private static string BuildItemDecisionReason(
        IReadOnlyList<RuleAggregate> publishedRules,
        string rollbackMode)
    {
        if (publishedRules.Count == 0)
        {
            return "未命中特殊计价规则，可按普通价格流程收费";
        }

        var ruleNames = publishedRules
            .Select(rule => NormalizeString(rule.RuleName) ?? NormalizeString(rule.RuleCode) ?? rule.RuleId.ToString())
            .Distinct(StringComparer.Ordinal)
            .ToList();
        return $"命中 {publishedRules.Count} 条特殊计价规则：{string.Join("、", ruleNames)}；下一步需调用统一计价；计价服务不可用时按 {rollbackMode} 处理";
    }

    private static string BuildBatchDecisionReason(int itemCount, int specialItemCount)
    {
        return specialItemCount == 0
            ? $"本批次 {itemCount} 条费用均未命中特殊计价规则，可按普通价格流程收费"
            : $"本批次 {itemCount} 条费用中有 {specialItemCount} 条特殊项目，需先调用统一计价";
    }

    private static string ResolveRollbackMode(IReadOnlyList<RuleAggregate> rules)
    {
        // 多条规则同时存在时取最保守回滚模式。
        // STOP_CHARGE > NEW_SERVICE_ONLY > MANUAL_REVIEW > LEGACY_EQUIVALENT。
        if (rules.Count == 0)
        {
            return "STOP_CHARGE";
        }

        var modes = rules
            .Select(r => NormalizeString(r.RollbackMode) ?? "STOP_CHARGE")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (modes.Contains("STOP_CHARGE"))
        {
            return "STOP_CHARGE";
        }

        if (modes.Contains("MANUAL_REVIEW") || modes.Contains("NEW_SERVICE_ONLY"))
        {
            return modes.Contains("NEW_SERVICE_ONLY") ? "NEW_SERVICE_ONLY" : "MANUAL_REVIEW";
        }

        if (modes.Contains("LEGACY_EQUIVALENT"))
        {
            return "LEGACY_EQUIVALENT";
        }

        return "STOP_CHARGE";
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

    private static IReadOnlyDictionary<string, string>? NormalizeExtraParams(
        IReadOnlyDictionary<string, object?>? source)
    {
        var normalized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddExtraParams(normalized, source);
        return normalized.Count == 0 ? null : normalized;
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

    private static Dictionary<string, object?>? ToObjectDictionary(IReadOnlyDictionary<string, string>? source)
    {
        return source is null
            ? null
            : source.ToDictionary(pair => pair.Key, pair => (object?)pair.Value, StringComparer.OrdinalIgnoreCase);
    }

    private static string? NormalizeString(string? value)
    {
        // 空白字符串按 null 处理，避免缓存和匹配维度出现空串/空格差异。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
