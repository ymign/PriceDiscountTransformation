using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 特殊项目标识解析器。
/// </summary>
/// <remarks>
/// <para>
/// special-flag 是渠道决定“是否必须调用统一计价服务”的前置接口。它不能漏判特殊项目：
/// 若漏判，渠道可能按普通价格收费，绕过折价规则、限额和追溯。
/// </para>
/// <para>
/// 解析器优先读取当前激活运行包；没有运行包时回退旧规则主档。
/// 运行包路径可以按查询条件预评估规则条件，旧规则路径只做项目和生效期粗判。
/// </para>
/// </remarks>
public sealed class PricingSpecialFlagResolver
{
    private readonly RuntimeRuleProjectionAdapter _runtimeProjectionAdapter = new();

    /// <summary>
    /// 旧规则主档仓储，用于未激活运行包时按项目读取已发布规则。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;
    /// <summary>
    /// 统一时钟，用于 businessChargeTime 未传入时判断规则生效期。
    /// </summary>
    private readonly IClock _clock;
    /// <summary>
    /// 运行包追溯解析器，用于读取当前激活运行包中的运行时规则快照。
    /// </summary>
    private readonly RuntimePackageTraceResolver? _runtimePackageTraceResolver;
    /// <summary>
    /// 当前请求可见规则快照统一入口。存在时优先使用该入口，统一封装运行包和旧规则回退逻辑。
    /// </summary>
    private readonly EffectiveRuleSnapshotLoader? _effectiveRuleSnapshotLoader;
    /// <summary>
    /// 条件组匹配器，用于 special-flag 查询时提前按场景、部位、就诊类型等条件预判命中。
    /// </summary>
    private readonly IRuleConditionGroupMatcher? _conditionMatcher;

    /// <summary>
    /// 初始化特殊项目标识解析器。
    /// </summary>
    /// <param name="headerRepository">规则头仓储，用于读取项目关联规则。</param>
    /// <param name="clock">技术时间提供者，用于按当前时间过滤有效规则。</param>
    /// <param name="runtimePackageTraceResolver">运行时包追溯解析器，用于优先读取激活运行时包。</param>
    /// <param name="conditionMatcher">条件组匹配器，用于按查询维度预判规则命中。</param>
    /// <param name="effectiveRuleSnapshotLoader">统一规则快照加载入口，用于优先复用运行包和旧规则回退逻辑。</param>
    public PricingSpecialFlagResolver(
        IRuleHeaderRepository headerRepository,
        IClock clock,
        RuntimePackageTraceResolver? runtimePackageTraceResolver = null,
        IRuleConditionGroupMatcher? conditionMatcher = null,
        EffectiveRuleSnapshotLoader? effectiveRuleSnapshotLoader = null)
    {
        _headerRepository = headerRepository;
        _clock = clock;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _conditionMatcher = conditionMatcher;
        _effectiveRuleSnapshotLoader = effectiveRuleSnapshotLoader;
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
        var businessTime = request.BusinessChargeTime ?? _clock.Now;

        if (_effectiveRuleSnapshotLoader is not null)
        {
            return await ResolveFromSnapshotLoaderAsync(normalizedItemCode, request, businessTime);
        }

        // 优先使用激活运行包。运行包是发布后的稳定快照，能避免查询过程中规则主表被编辑造成判断不一致。
        if (_runtimePackageTraceResolver is not null)
        {
            var runtimeResolution = await _runtimePackageTraceResolver.LoadActiveRuleSnapshotsByItemCodeAsync(normalizedItemCode);
            if (runtimeResolution.HasActiveRuntimePackage)
            {
                return await ResolveFromRuntimePackageAsync(
                    normalizedItemCode,
                    request,
                    businessTime,
                    runtimeResolution);
            }
        }

        // 没有运行包时回退旧规则模型。此路径无法完整执行所有条件，只按项目、发布状态和生效期粗判。
        // 粗判宁可多返回特殊项目，也不能漏掉需要统一计价的项目。
        var rules = await _headerRepository.GetByItemCodeAsync(normalizedItemCode);
        var published = rules
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
            .Where(r => r.IsEffectiveAt(businessTime))
            .ToList();

        return BuildPublishedRuleResponse(normalizedItemCode, published);
    }

    private async Task<SpecialFlagResponse> ResolveFromSnapshotLoaderAsync(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime)
    {
        var ruleSet = await _effectiveRuleSnapshotLoader!.LoadCurrentAsync(normalizedItemCode);
        if (ruleSet.HasRuntimePackage)
        {
            return await ResolveFromRuntimeSnapshotSetAsync(
                normalizedItemCode,
                request,
                businessTime,
                ruleSet);
        }

        var publishedRules = ruleSet.Snapshots
            .Select(snapshot => snapshot.Header)
            .Where(rule => rule.Status == RuleStatusCodes.Published && rule.IsEnabled == EnableFlag.Yes)
            .Where(rule => rule.IsEffectiveAt(businessTime))
            .ToList();

        return BuildPublishedRuleResponse(normalizedItemCode, publishedRules);
    }

    private async Task<SpecialFlagResponse> ResolveFromRuntimeSnapshotSetAsync(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime,
        EffectiveRuleSnapshotLoadResult ruleSet)
    {
        var context = BuildPricingContext(normalizedItemCode, request, businessTime);
        var matchedSnapshots = new List<EffectiveRuleSnapshot>();

        foreach (var snapshot in ruleSet.Snapshots)
        {
            if (!snapshot.Header.IsEffectiveAt(businessTime))
            {
                continue;
            }

            if (_conditionMatcher is not null &&
                !await _conditionMatcher.EvaluateAsync(snapshot.Conditions, context))
            {
                continue;
            }

            matchedSnapshots.Add(snapshot);
        }

        var runtimeRuleIds = matchedSnapshots
            .Select(snapshot => snapshot.Header.RuleId)
            .Where(id => id > 0)
            .Distinct()
            .ToList();
        var policyVersionIds = runtimeRuleIds
            .Where(ruleSet.RuntimeRulesById.ContainsKey)
            .Select(ruleId => ruleSet.RuntimeRulesById[ruleId].SourcePolicyVersionId)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = runtimeRuleIds.Count > 0,
            RuleCount = runtimeRuleIds.Count,
            RollbackMode = "STOP_CHARGE",
            RuntimePackageId = ruleSet.RuntimePackageId,
            RuntimePackageVersion = ruleSet.RuntimePackageVersion,
            MatchedRuleIds = runtimeRuleIds,
            MatchedRuntimeRuleIds = runtimeRuleIds,
            MatchedPolicyVersionIds = policyVersionIds
        };
    }

    private async Task<SpecialFlagResponse> ResolveFromRuntimePackageAsync(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime,
        RuntimePackageRuleSnapshotResolution runtimeResolution)
    {
        return await ResolveFromRuntimeSnapshotSetAsync(
            normalizedItemCode,
            request,
            businessTime,
            new EffectiveRuleSnapshotLoadResult
            {
                RuntimePackageId = runtimeResolution.RuntimePackageId,
                RuntimePackageVersion = runtimeResolution.RuntimePackageVersion,
                Snapshots = runtimeResolution.Snapshots
                    .Select(_runtimeProjectionAdapter.Adapt)
                    .ToList(),
                RuntimeRulesById = runtimeResolution.Snapshots
                    .Select(snapshot => snapshot.Rule)
                    .ToDictionary(rule => rule.RuntimeRuleId)
            });
    }

    private static SpecialFlagResponse BuildPublishedRuleResponse(
        string normalizedItemCode,
        IReadOnlyList<RuleAggregate> publishedRules)
    {
        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = publishedRules.Count > 0,
            RuleCount = publishedRules.Count,
            RollbackMode = ResolveRollbackMode(publishedRules),
            MatchedRuleIds = publishedRules.Select(rule => rule.RuleId).Distinct().ToList()
        };
    }

    private static PricingContext BuildPricingContext(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime)
    {
        // special-flag 只需要规则匹配条件，不需要真实数量和单价。
        // 使用 1 和 0 的占位值是为了满足 PricingContext 的必填字段，执行器不会在此路径运行。
        return new PricingContext
        {
            CallType = "SPECIAL_FLAG",
            PatientId = "-",
            ItemCode = normalizedItemCode,
            InputQty = 1m,
            ConvertedQty = 1m,
            FinalQty = 1m,
            UnitPrice = 0m,
            ChargeScene = NormalizeString(request.ChargeScene),
            BusinessChargeTime = businessTime,
            SourceSystem = "SPECIAL_FLAG_QUERY",
            BodyPartCode = NormalizeString(request.BodyPartCode),
            VisitType = NormalizeString(request.VisitType),
            ChargeDeptCode = NormalizeString(request.ChargeDeptCode)
        };
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

    private static string? NormalizeString(string? value)
    {
        // 空白字符串按 null 处理，避免缓存和匹配维度出现空串/空格差异。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
