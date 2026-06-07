using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
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

    /// <summary>
    /// 初始化特殊项目标识解析器。
    /// </summary>
    /// <param name="headerRepository">规则头仓储，用于读取项目关联规则。</param>
    /// <param name="clock">技术时间提供者，用于按当前时间过滤有效规则。</param>
    public PricingSpecialFlagResolver(
        IRuleHeaderRepository headerRepository,
        IClock clock)
    {
        _headerRepository = headerRepository;
        _clock = clock;
    }

    /// <summary>
    /// 解析项目是否属于当前必须进入特殊计价流程的特殊项目。
    /// </summary>
    /// <param name="itemCode">项目编码。</param>
    /// <returns>特殊项目标识响应。</returns>
    public async Task<SpecialFlagResponse> ResolveAsync(string itemCode)
    {
        var normalizedItemCode = NormalizeString(itemCode)
            ?? throw new ArgumentException("项目编码不能为空", nameof(itemCode));

        var rules = await _headerRepository.GetByItemCodeAsync(normalizedItemCode);
        var now = _clock.Now;
        var published = rules
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
            .Where(r => r.IsEffectiveAt(now))
            .ToList();

        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = published.Count > 0,
            RuleCount = published.Count,
            RollbackMode = ResolveRollbackMode(published)
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
