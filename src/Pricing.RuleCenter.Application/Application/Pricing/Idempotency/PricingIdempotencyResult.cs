using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Idempotency;

internal sealed record PricingIdempotencyResult(
    bool HasExisting,
    ChargeRequest? ExistingRequest,
    string Fingerprint);
