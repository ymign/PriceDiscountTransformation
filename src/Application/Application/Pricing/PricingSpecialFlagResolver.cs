using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 特殊项目标识解析器。
/// </summary>
public sealed class PricingSpecialFlagResolver
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IClock _clock;
    private readonly RuntimePackageTraceResolver? _runtimePackageTraceResolver;
    private readonly RuleConditionGroupMatcher? _conditionMatcher;

    /// <summary>
    /// 初始化特殊项目标识解析器。
    /// </summary>
    /// <param name="headerRepository">规则头仓储，用于读取项目关联规则。</param>
    /// <param name="clock">技术时间提供者，用于按当前时间过滤有效规则。</param>
    /// <param name="runtimePackageTraceResolver">运行时包追溯解析器，用于优先读取激活运行时包。</param>
    /// <param name="conditionEvaluatorFactory">条件评估器工厂，用于按查询维度预判规则命中。</param>
    /// <param name="logger">日志组件。</param>
    public PricingSpecialFlagResolver(
        IRuleHeaderRepository headerRepository,
        IClock clock,
        RuntimePackageTraceResolver? runtimePackageTraceResolver = null,
        ConditionEvaluatorFactory? conditionEvaluatorFactory = null,
        ILogger? logger = null)
    {
        _headerRepository = headerRepository;
        _clock = clock;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _conditionMatcher = conditionEvaluatorFactory is null
            ? null
            : new RuleConditionGroupMatcher(
                conditionEvaluatorFactory,
                logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
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

        var rules = await _headerRepository.GetByItemCodeAsync(normalizedItemCode);
        var published = rules
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
            .Where(r => r.IsEffectiveAt(businessTime))
            .ToList();

        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = published.Count > 0,
            RuleCount = published.Count,
            RollbackMode = ResolveRollbackMode(published),
            MatchedRuleIds = published.Select(r => r.RuleId).Distinct().ToList()
        };
    }

    private async Task<SpecialFlagResponse> ResolveFromRuntimePackageAsync(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime,
        RuntimePackageRuleSnapshotResolution runtimeResolution)
    {
        var adapter = new RuntimeRuleProjectionAdapter();
        var context = BuildPricingContext(normalizedItemCode, request, businessTime);
        var matchedSnapshots = new List<RuntimeRuleSnapshot>();

        foreach (var runtimeSnapshot in runtimeResolution.Snapshots)
        {
            var snapshot = adapter.Adapt(runtimeSnapshot);
            if (!snapshot.Header.IsEffectiveAt(businessTime))
            {
                continue;
            }

            if (_conditionMatcher is not null &&
                !await _conditionMatcher.EvaluateAsync(snapshot.Conditions, context))
            {
                continue;
            }

            matchedSnapshots.Add(runtimeSnapshot);
        }

        var runtimeRuleIds = matchedSnapshots
            .Select(snapshot => snapshot.Rule.RuntimeRuleId)
            .Where(id => id > 0)
            .Distinct()
            .ToList();
        var policyVersionIds = matchedSnapshots
            .Select(snapshot => snapshot.Rule.SourcePolicyVersionId)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = runtimeRuleIds.Count > 0,
            RuleCount = runtimeRuleIds.Count,
            RollbackMode = "STOP_CHARGE",
            RuntimePackageId = runtimeResolution.RuntimePackageId,
            RuntimePackageVersion = runtimeResolution.RuntimePackageVersion,
            MatchedRuleIds = runtimeRuleIds,
            MatchedRuntimeRuleIds = runtimeRuleIds,
            MatchedPolicyVersionIds = policyVersionIds
        };
    }

    private static PricingContext BuildPricingContext(
        string normalizedItemCode,
        SpecialFlagRequest request,
        DateTime businessTime)
    {
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
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
