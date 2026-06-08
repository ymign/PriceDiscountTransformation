using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 幂等响应快照读取器。
/// </summary>
public sealed class PricingIdempotentResponseReader
{
    private readonly ILogger<PricingIdempotentResponseReader> _logger;

    /// <summary>
    /// 初始化幂等响应快照读取器。
    /// </summary>
    /// <param name="logger">日志组件，用于记录快照解析失败的上下文。</param>
    public PricingIdempotentResponseReader(ILogger<PricingIdempotentResponseReader> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 从请求日志中读取首次 confirm 保存的完整响应快照。
    /// </summary>
    /// <param name="log">已存在的请求日志记录。</param>
    /// <returns>反序列化后的计价响应。</returns>
    public Task<PricingCalculateResponse> ReadAsync(ChargeRequest log)
    {
        if (string.IsNullOrWhiteSpace(log.ResponseJson))
        {
            throw new BizException(
                BizErrorCode.IdempotencyResponseSnapshotInvalid,
                409,
                $"RequestId={log.RequestId} 的幂等响应快照缺失");
        }

        try
        {
            var response = JsonConvert.DeserializeObject<PricingCalculateResponse>(log.ResponseJson);
            if (response is not null)
            {
                return Task.FromResult(response);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "幂等响应快照解析失败 请求ID={RequestId}", log.RequestId);
        }

        throw new BizException(
            BizErrorCode.IdempotencyResponseSnapshotInvalid,
            409,
            $"RequestId={log.RequestId} 的幂等响应快照不可解析");
    }
}
