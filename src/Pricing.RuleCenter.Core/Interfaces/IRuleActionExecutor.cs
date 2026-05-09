using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRuleActionExecutor
{
    string ActionType { get; }
    Task ExecuteAsync(RuleAction action, PricingContext context);
}
