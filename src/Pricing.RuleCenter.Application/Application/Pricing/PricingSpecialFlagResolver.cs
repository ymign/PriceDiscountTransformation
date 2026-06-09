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
    public PricingSpecialFlagResolver(
        IRuleHeaderRepository headerRepository,
        IClock clock,
        RuntimePackageTraceResolver? runtimePackageTraceResolver = null,
        IRuleConditionGroupMatcher? conditionMatcher = null)
    {
        _headerRepository = headerRepository;
        _clock = clock;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _conditionMatcher = conditionMatcher;
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
        // 运行包快照需要适配成规则聚合视角，复用与正式计价一致的条件组匹配逻辑。
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

            // special-flag 查询只构造轻量 PricingContext，不做金额计算。
            // 目的只是提前判断该项目在当前场景下是否会命中特殊规则。
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
            // 运行包路径目前按最保守 STOP_CHARGE 返回。渠道在服务异常时不能按普通计价回退。
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
