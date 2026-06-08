using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.AuthorityPrice;

/// <summary>
/// 权威物价校验器，负责在计价前核对渠道传入单价与物价主数据是否一致。
/// </summary>
public sealed class AuthorityPriceChecker
{
    private readonly IPriceMasterRepository _priceMasterRepository;
    private readonly PricingOptions _options;
    private readonly ILogger<AuthorityPriceChecker> _logger;

    /// <summary>
    /// 初始化权威物价校验器。
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
    /// 校验每条费用明细的请求单价是否与权威物价一致。
    /// </summary>
    /// <param name="items">计价费用明细。</param>
    public async Task CheckAsync(IReadOnlyList<PricingCalculateItemRequest> items)
    {
        if (!_options.EnableAuthorityPriceCheck)
        {
            return;
        }

        var authorityPrices = await _priceMasterRepository.GetUnitPricesAsync(
            items.Select(item => item.ItemCode).ToArray());

        foreach (var item in items)
        {
            if (!authorityPrices.TryGetValue(item.ItemCode, out var authorityPrice) || !authorityPrice.HasValue)
            {
                _logger.LogWarning(
                    "权威单价校验失败: 未找到项目权威单价 ItemCode={ItemCode}",
                    item.ItemCode);
                throw new BizException(
                    BizErrorCode.PriceMismatch,
                    409,
                    $"未找到项目 {item.ItemCode} 的权威单价");
            }

            if (Math.Round(authorityPrice.Value, 4) != Math.Round(item.UnitPrice, 4))
            {
                _logger.LogWarning(
                    "权威单价校验失败: 单价不一致 ItemCode={ItemCode}, AuthorityPrice={AuthorityPrice}, RequestPrice={RequestPrice}",
                    item.ItemCode, authorityPrice.Value, item.UnitPrice);
                throw new BizException(
                    BizErrorCode.PriceMismatch,
                    409,
                    $"项目 {item.ItemCode} 权威单价={authorityPrice.Value}, 请求单价={item.UnitPrice}");
            }
        }
    }
}
