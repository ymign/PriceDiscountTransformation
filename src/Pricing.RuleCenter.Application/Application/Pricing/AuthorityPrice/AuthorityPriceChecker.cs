using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.AuthorityPrice;

/// <summary>
/// 权威物价诊断器，负责记录渠道传入单价与物价主数据的差异。
/// </summary>
public sealed class AuthorityPriceChecker
{
    /// <summary>
    /// 可显式指定价格类型的扩展参数键名集合。
    /// </summary>
    /// <remarks>
    /// 不同渠道或历史系统对价格类型命名不统一，这里集中兼容，避免各入口重复判断。
    /// </remarks>
    private static readonly string[] PriceTypeKeys =
    [
        "price_type",
        "patient_price_type",
        "price_form",
        "pact_price_form",
        "patient_type"
    ];

    /// <summary>
    /// 可表示围产价格的扩展参数键名集合。
    /// </summary>
    private static readonly string[] PerinatalFlagKeys =
    [
        "is_perinatal",
        "perinatal",
        "is_weichan",
        "is_wei_chan",
        "weichan_flag",
        "wei_chan_flag"
    ];

    /// <summary>
    /// 权威物价主数据仓储。
    /// </summary>
    private readonly IPriceMasterRepository _priceMasterRepository;
    /// <summary>
    /// 计价配置，控制是否启用权威价格诊断和儿童价格年龄阈值。
    /// </summary>
    private readonly PricingOptions _options;
    /// <summary>
    /// 诊断日志组件。
    /// </summary>
    private readonly ILogger<AuthorityPriceChecker> _logger;

    /// <summary>
    /// 初始化权威物价诊断器。
    /// </summary>
    /// <param name="priceMasterRepository">权威物价主数据仓储。</param>
    /// <param name="options">计价配置项。</param>
    /// <param name="logger">日志组件。</param>
    public AuthorityPriceChecker(
        IPriceMasterRepository priceMasterRepository,
        IOptions<PricingOptions> options,
        ILogger<AuthorityPriceChecker> logger)
    {
        _priceMasterRepository = priceMasterRepository;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// 诊断每条费用明细的请求单价是否与权威物价一致。
    /// </summary>
    /// <param name="request">计价请求上下文，用于解析患者年龄、围产标识等价格类型条件。</param>
    /// <param name="items">计价费用明细。</param>
    /// <remarks>
    /// 该方法只记录诊断日志，不抛出 PRICE_MISMATCH，也不影响试算或确认流程。
    /// 基础单价仍由 HIS 负责带出；规则中心只把差异作为后续联调和对账线索。
    /// </remarks>
    public async Task CheckAsync(PricingCalculateRequest request, IReadOnlyList<PricingCalculateItemRequest> items)
    {
        if (!_options.EnableAuthorityPriceCheck)
        {
            return;
        }

        var priceItems = await _priceMasterRepository.GetPriceItemsAsync(
            items.Select(item => item.ItemCode).ToArray());

        foreach (var item in items)
        {
            var itemCode = item.ItemCode.Trim();
            var priceKind = ResolvePriceKind(request, item);
            var priceKindName = GetPriceKindName(priceKind);
            if (!priceItems.TryGetValue(itemCode, out var priceItem) || priceItem is null)
            {
                _logger.LogWarning(
                    "权威单价诊断：未找到项目权威单价 项目编码={ItemCode}, 价格类型={PriceKind}, 请求单价={RequestPrice}",
                    itemCode, priceKindName, item.UnitPrice);
                continue;
            }

            var authorityPrice = GetAuthorityPrice(priceItem, priceKind);
            if (!authorityPrice.HasValue)
            {
                _logger.LogWarning(
                    "权威单价诊断：项目价格列为空 项目编码={ItemCode}, 价格类型={PriceKind}, 请求单价={RequestPrice}",
                    itemCode, priceKindName, item.UnitPrice);
                continue;
            }

            if (Math.Round(authorityPrice.Value, 4) != Math.Round(item.UnitPrice, 4))
            {
                _logger.LogWarning(
                    "权威单价诊断：单价不一致 项目编码={ItemCode}, 价格类型={PriceKind}, 权威单价={AuthorityPrice}, 请求单价={RequestPrice}",
                    itemCode, priceKindName, authorityPrice.Value, item.UnitPrice);
            }
        }
    }

    private AuthorityPriceKind ResolvePriceKind(PricingCalculateRequest request, PricingCalculateItemRequest item)
    {
        // 明细级参数优先于请求级参数。一次收费动作中不同项目可能适用不同价格类型，
        // 例如同单既有儿童价项目，也有普通三甲价项目。
        if (TryResolveExplicitPriceKind(item.ExtraParams, out var itemPriceKind))
        {
            return itemPriceKind;
        }

        if (TryResolveExplicitPriceKind(request.ExtraParams, out var requestPriceKind))
        {
            return requestPriceKind;
        }

        if (TryResolveBoolean(item.ExtraParams, PerinatalFlagKeys, out var itemIsPerinatal) && itemIsPerinatal)
        {
            return AuthorityPriceKind.Perinatal;
        }

        if (TryResolveBoolean(request.ExtraParams, PerinatalFlagKeys, out var requestIsPerinatal) && requestIsPerinatal)
        {
            return AuthorityPriceKind.Perinatal;
        }

        var childPriceAgeExclusive = _options.ChildPriceAgeExclusive <= 0
            ? 6
            : _options.ChildPriceAgeExclusive;
        // 未显式传价格类型时按年龄推导儿童价。阈值可配置，默认 6 岁以下。
        if (request.PatientAge is >= 0 && request.PatientAge < childPriceAgeExclusive)
        {
            return AuthorityPriceKind.Child;
        }

        return AuthorityPriceKind.Normal;
    }

    private static decimal? GetAuthorityPrice(PriceMasterItem priceItem, AuthorityPriceKind priceKind) =>
        priceKind switch
        {
            AuthorityPriceKind.Child => priceItem.ChildPrice,
            AuthorityPriceKind.Perinatal => priceItem.PerinatalPrice,
            _ => priceItem.UnitPrice
        };

    private static string GetPriceKindName(AuthorityPriceKind priceKind) =>
        priceKind switch
        {
            AuthorityPriceKind.Child => "儿童价",
            AuthorityPriceKind.Perinatal => "围产价",
            _ => "三甲价"
        };

    private static bool TryResolveExplicitPriceKind(
        Dictionary<string, object?>? extraParams,
        out AuthorityPriceKind priceKind)
    {
        priceKind = AuthorityPriceKind.Normal;
        if (!TryGetExtraParam(extraParams, PriceTypeKeys, out var rawValue))
        {
            return false;
        }

        var text = ReadString(rawValue);
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var normalized = text
            .Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

        switch (normalized)
        {
            // 同时兼容数字枚举、英文编码和中文显示值，降低 HIS 历史字段差异导致的诊断误判。
            case "5":
            case "PERINATAL":
            case "WEICHAN":
            case "WEICHANPRICE":
            case "WEICHANCENTER":
            case "围产":
            case "围产价":
            case "围产中心价":
                priceKind = AuthorityPriceKind.Perinatal;
                return true;

            case "2":
            case "CHILD":
            case "CHILDPRICE":
            case "UNITPRICE1":
            case "儿童":
            case "儿童价":
                priceKind = AuthorityPriceKind.Child;
                return true;

            case "0":
            case "NORMAL":
            case "DEFAULT":
            case "STANDARD":
            case "UNITPRICE":
            case "三甲":
            case "三甲价":
                priceKind = AuthorityPriceKind.Normal;
                return true;

            default:
                return false;
        }
    }

    private static bool TryResolveBoolean(
        Dictionary<string, object?>? extraParams,
        IReadOnlyCollection<string> keys,
        out bool result)
    {
        result = false;
        if (!TryGetExtraParam(extraParams, keys, out var rawValue))
        {
            return false;
        }

        return TryReadBoolean(rawValue, out result);
    }

    private static bool TryGetExtraParam(
        Dictionary<string, object?>? extraParams,
        IReadOnlyCollection<string> keys,
        out object? value)
    {
        value = null;
        if (extraParams is null)
        {
            return false;
        }

        foreach (var pair in extraParams)
        {
            if (keys.Any(key => string.Equals(key, pair.Key, StringComparison.OrdinalIgnoreCase)))
            {
                value = pair.Value;
                return true;
            }
        }

        return false;
    }

    private static string? ReadString(object? value) =>
        value switch
        {
            // ExtraParams 可能来自 System.Text.Json，也可能来自 Newtonsoft.Json。
            // 统一读成字符串后再做兼容判断。
            null => null,
            string text => text,
            bool flag => flag ? "true" : "false",
            JsonElement { ValueKind: JsonValueKind.String } element => element.GetString(),
            JsonElement { ValueKind: JsonValueKind.Number } element => element.GetRawText(),
            JsonElement { ValueKind: JsonValueKind.True } => "true",
            JsonElement { ValueKind: JsonValueKind.False } => "false",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };

    private static bool TryReadBoolean(object? value, out bool result)
    {
        result = false;
        switch (value)
        {
            case bool flag:
                result = flag;
                return true;
            case string text:
                return TryReadBooleanText(text, out result);
            case JsonElement { ValueKind: JsonValueKind.True }:
                result = true;
                return true;
            case JsonElement { ValueKind: JsonValueKind.False }:
                result = false;
                return true;
            case JsonElement { ValueKind: JsonValueKind.String } element:
                return TryReadBooleanText(element.GetString(), out result);
            case JsonElement { ValueKind: JsonValueKind.Number } element:
                return TryReadBooleanText(element.GetRawText(), out result);
            case IFormattable formattable:
                return TryReadBooleanText(formattable.ToString(null, CultureInfo.InvariantCulture), out result);
            default:
                return false;
        }
    }

    private static bool TryReadBooleanText(string? text, out bool result)
    {
        result = false;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var normalized = text.Trim().ToUpperInvariant();
        if (normalized is "TRUE" or "1" or "Y" or "YES" or "是")
        {
            result = true;
            return true;
        }

        if (normalized is "FALSE" or "0" or "N" or "NO" or "否")
        {
            result = false;
            return true;
        }

        return false;
    }

    private enum AuthorityPriceKind
    {
        /// <summary>普通三甲价。</summary>
        Normal,
        /// <summary>儿童价。</summary>
        Child,
        /// <summary>围产中心价。</summary>
        Perinatal
    }
}
