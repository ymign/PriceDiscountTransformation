using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Idempotency;

/// <summary>
/// confirm 幂等检查结果。
/// </summary>
/// <param name="HasExisting">是否已存在相同业务键的请求日志。</param>
/// <param name="ExistingRequest">已有请求日志；不存在时为 null。</param>
/// <param name="Fingerprint">本次请求按规范化规则生成的指纹。</param>
/// <remarks>
/// ExistingRequest 和 Fingerprint 必须一起返回。即使事务外没有命中已有记录，workflow 进入事务后仍要复用
/// 同一个 Fingerprint 做二次检查，避免前后两次规范化口径不一致。
/// </remarks>
internal sealed record PricingIdempotencyResult(
    bool HasExisting,
    ChargeRequest? ExistingRequest,
    string Fingerprint);
