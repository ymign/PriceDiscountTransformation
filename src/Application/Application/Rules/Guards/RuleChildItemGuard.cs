using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 子项目加收动作门禁。
/// </summary>
public sealed class RuleChildItemGuard
{
    /// <summary>
    /// 校验 ADD_CHILD_ITEM 配置中子项目编码有效且不重复。
    /// </summary>
    public void EnsureValid(IReadOnlyList<RuleAction> actions)
    {
        var normalizedChildCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var action in actions.Where(a =>
                     a.IsEnabled == EnableFlag.Yes &&
                     string.Equals(NormalizeActionType(a.ActionType), RuleActionTypeCodes.AddChildItem, StringComparison.OrdinalIgnoreCase)))
        {
            var config = ParseAddChildItemConfig(action.ParamsJson);
            if (config?.ChildItems is null || config.ChildItems.Count == 0)
            {
                continue;
            }

            foreach (var child in config.ChildItems)
            {
                var normalizedItemCode = NormalizeChildItemCode(child.ItemCode);
                if (normalizedItemCode is null)
                {
                    throw new BizException(
                        BizErrorCode.ChildItemInvalid,
                        409,
                        "ADD_CHILD_ITEM 的 childItems[].itemCode 不能为空");
                }

                if (!normalizedChildCodes.Add(normalizedItemCode))
                {
                    throw new BizException(
                        BizErrorCode.ChildItemDuplicate,
                        409,
                        $"ADD_CHILD_ITEM 重复引用子项目 {normalizedItemCode}");
                }
            }
        }
    }

    private static AddChildItemConfig? ParseAddChildItemConfig(string? paramsJson)
    {
        if (string.IsNullOrWhiteSpace(paramsJson))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<AddChildItemConfig>(paramsJson);
        }
        catch
        {
            throw new BizException(
                BizErrorCode.ActionParamInvalid,
                409,
                "ADD_CHILD_ITEM 动作参数不是合法 JSON");
        }
    }

    private static string NormalizeActionType(string? actionType) =>
        actionType?.Trim().ToUpperInvariant() ?? string.Empty;

    private static string? NormalizeChildItemCode(string? itemCode)
    {
        var normalized = itemCode?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized.ToUpperInvariant();
    }

    private sealed class AddChildItemConfig
    {
        public List<AddChildItemChildConfig>? ChildItems { get; set; }
    }

    private sealed class AddChildItemChildConfig
    {
        public string? ItemCode { get; set; }
    }
}
