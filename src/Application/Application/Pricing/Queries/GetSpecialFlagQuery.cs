using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Queries;

/// <summary>特殊项目标识查询。</summary>
public sealed record GetSpecialFlagQuery(string ItemCode) : IRequest<SpecialFlagResponse>;

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
        var cacheKey = SpecialFlagCacheKeys.Register(request.ItemCode);
        if (_cache.TryGetValue<SpecialFlagResponse>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        var result = await _service.GetSpecialFlagAsync(request.ItemCode);
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }
}
