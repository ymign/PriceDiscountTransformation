using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Tests;

internal sealed class FixedClock : IClock
{
    public FixedClock(DateTime now)
    {
        Now = now;
    }

    public DateTime Now { get; set; }
}
