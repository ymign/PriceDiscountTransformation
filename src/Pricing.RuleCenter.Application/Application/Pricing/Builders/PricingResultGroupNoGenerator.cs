using System.Security.Cryptography;
using System.Text;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

/// <summary>
/// 计价结果组号生成器，用于把主项目、替换子项和加收子项绑定到同一原子结果组。
/// </summary>
/// <remarks>
/// resultGroupNo 是主子项目 commit/cancel/reverse 的关键串联字段。HIS 落账时可能给子项生成新的收费明细号，
/// 因此不能只依赖 chargeDetailNo 关联主子项目，必须有规则中心生成的稳定组号。
/// </remarks>
internal static class PricingResultGroupNoGenerator
{
    /// <summary>
    /// 按请求 ID、费用明细和组类型生成稳定结果组号。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <param name="item">主项目费用明细。</param>
    /// <param name="groupType">组类型，例如 REPLACE 或 CHILD。</param>
    /// <returns>稳定结果组号。</returns>
    public static string Build(long requestId, PricingCalculateItemRequest item, string groupType)
    {
        var chargeDetailNo = NormalizeString(item.ChargeDetailNo) ?? "NO_DETAIL";
        var itemRequestNo = NormalizeString(item.ItemRequestNo) ?? "NO_ITEM_REQUEST";
        var rawKey = $"{item.ItemCode.Trim()}:{chargeDetailNo}:{itemRequestNo}".ToUpperInvariant();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)))[..12];
        return $"{groupType}:{requestId}:{hash}";
    }

    /// <summary>
    /// 根据计价结果判断是否需要生成结果组号。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <param name="item">主项目费用明细。</param>
    /// <param name="result">计价结果。</param>
    /// <returns>存在替换子项或加收子项时返回组号；普通项目返回 null。</returns>
    public static string? Resolve(long requestId, PricingCalculateItemRequest item, PricingResult result)
    {
        var hasChildItems = result.ChildPricingResults.Count > 0;
        return result.ReplaceChildResult is null && !hasChildItems
            ? null
            : Build(requestId, item, result.ReplaceChildResult is not null ? "REPLACE" : "CHILD");
    }

    private static string? NormalizeString(string? value)
    {
        // 组号原始键需要排除空白差异，避免同一 HIS 明细因空格不同生成不同组号。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
