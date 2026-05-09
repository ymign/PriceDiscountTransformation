using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IPricingEngine
{
    Task<PricingResult> CalculateAsync(PricingContext context);
}
