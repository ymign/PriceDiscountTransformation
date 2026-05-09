using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Api.Engine;

public sealed class ActionExecutorFactory
{
    private readonly Dictionary<string, IRuleActionExecutor> _executors;

    public ActionExecutorFactory(IEnumerable<IRuleActionExecutor> executors)
    {
        _executors = executors.ToDictionary(e => e.ActionType, StringComparer.OrdinalIgnoreCase);
    }

    public IRuleActionExecutor? GetExecutor(string actionType)
    {
        _executors.TryGetValue(actionType, out var executor);
        return executor;
    }
}
