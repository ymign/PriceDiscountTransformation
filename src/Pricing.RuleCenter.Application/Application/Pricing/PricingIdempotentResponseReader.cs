using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 幂等响应快照读取器。
/// </summary>
/// <remarks>
/// confirm 重试不能重新计算，因为规则发布、历史占用和业务数据可能已经变化。
/// 正确做法是读取首次 confirm 在同一事务内保存的 ResponseJson 快照，原样返回给调用方。
/// </remarks>
public sealed class PricingIdempotentResponseReader
{
    /// <summary>
    /// 日志组件，用于记录响应快照缺失或 JSON 解析失败。
    /// </summary>
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
            // 快照缺失时不能退回重新计算，否则同一业务号可能拿到不同价格。
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
                // 返回首次响应快照，保证幂等重试的 RequestId、金额、过期时间和追溯字段一致。
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
