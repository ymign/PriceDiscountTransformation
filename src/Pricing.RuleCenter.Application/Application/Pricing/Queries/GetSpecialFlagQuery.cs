using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Queries;

/// <summary>
/// 特殊项目标识查询，表示渠道在收费录入前判断某项目是否必须进入统一计价服务。
/// </summary>
/// <remarks>
/// 查询维度除项目编码外，还可以携带收费场景、业务时间、就诊类型、部位和收费科室。
/// 这些维度用于提前评估规则条件，减少只按 ItemCode 粗判造成的误弹窗。
/// </remarks>
public sealed record GetSpecialFlagQuery(
    string ItemCode,
    string? ChargeScene = null,
    DateTime? BusinessChargeTime = null,
    string? VisitType = null,
    string? BodyPartCode = null,
    string? ChargeDeptCode = null) : IRequest<SpecialFlagResponse>
{
    /// <summary>
    /// 转换为特殊项目解析器使用的完整请求。
    /// </summary>
    /// <returns>合并路径参数和查询参数后的请求对象。</returns>
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

/// <summary>
/// 特殊项目标识查询处理器，负责缓存轻量查询并调用 <see cref="PricingSpecialFlagResolver"/>。
/// </summary>
/// <remarks>
/// special-flag 是收费录入界面高频接口，延迟要求比 confirm 更紧。这里使用内存缓存减少规则条件重复评估；
/// 规则发布、停用或运行包激活后会通过 <see cref="SpecialFlagCacheKeys"/> 清理缓存。
/// </remarks>
public sealed class GetSpecialFlagQueryHandler : IRequestHandler<GetSpecialFlagQuery, SpecialFlagResponse>
{
    /// <summary>
    /// 特殊项目标识查询的本地缓存时间。
    /// </summary>
    /// <remarks>
    /// 缓存时间不宜过长。规则发布后会主动清理缓存，5 分钟只是防止漏清理时的最大陈旧窗口。
    /// </remarks>
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 特殊项目解析器，负责从运行包或旧规则模型判断项目是否命中特殊计价规则。
    /// </summary>
    private readonly PricingSpecialFlagResolver _resolver;
    /// <summary>
    /// 进程内缓存，用于降低收费录入界面频繁查询同项目造成的规则匹配压力。
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// 初始化特殊项目标识查询处理器。
    /// </summary>
    /// <param name="resolver">特殊项目解析器。</param>
    /// <param name="cache">进程内缓存。</param>
    public GetSpecialFlagQueryHandler(PricingSpecialFlagResolver resolver, IMemoryCache cache)
    {
        _resolver = resolver;
        _cache = cache;
    }

    /// <summary>
    /// 执行特殊项目标识查询。
    /// </summary>
    /// <param name="request">特殊项目标识查询。</param>
    /// <param name="cancellationToken">MediatR 管道取消令牌；当前解析器内部尚未逐层传递。</param>
    /// <returns>特殊项目标识响应。</returns>
    public async Task<SpecialFlagResponse> Handle(GetSpecialFlagQuery request, CancellationToken cancellationToken)
    {
        // 缓存键必须包含所有查询维度。否则同一个 itemCode 在不同场景、部位或时间下可能复用错误结果。
        var cacheKey = SpecialFlagCacheKeys.Register(request);
        if (_cache.TryGetValue<SpecialFlagResponse>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        // 缓存未命中时才执行运行包/规则条件匹配。响应中会返回命中规则和回滚模式，供渠道决定是否弹窗和如何降级。
        var result = await _resolver.ResolveAsync(request.ToRequest());
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }
}
