using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Queries;

/// <summary>特殊项目标识查询。</summary>
public sealed record GetSpecialFlagQuery(
    string ItemCode,
    string? ChargeScene = null,
    DateTime? BusinessChargeTime = null,
    string? VisitType = null,
    string? BodyPartCode = null,
    string? ChargeDeptCode = null) : IRequest<SpecialFlagResponse>
{
    /// <summary>转换为应用服务请求。</summary>
    public SpecialFlagRequest ToRequest()
    {
        return new SpecialFlagRequest
        {
            ItemCode = ItemCode,
            ChargeScene = ChargeScene,
            BusinessChargeTime = BusinessChargeTime,
            VisitType = VisitType,
            BodyPartCode = BodyPartCode,
            ChargeDeptCode = ChargeDeptCode
        };
    }
}

/// <summary>特殊项目标识查询处理器。</summary>
public sealed class GetSpecialFlagQueryHandler : IRequestHandler<GetSpecialFlagQuery, SpecialFlagResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly PricingAppService _service;
    private readonly IMemoryCache _cache;

    /// <summary>初始化处理器。</summary>
    public GetSpecialFlagQueryHandler(PricingAppService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<SpecialFlagResponse> Handle(GetSpecialFlagQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = SpecialFlagCacheKeys.Register(request);
        if (_cache.TryGetValue<SpecialFlagResponse>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        var result = await _service.GetSpecialFlagAsync(request.ToRequest());
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }
}
