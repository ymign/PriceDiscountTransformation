using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine;

public sealed class ActionExecutionPipeline
{
    private readonly ActionExecutorFactory _executorFactory;
    private readonly ILogger<ActionExecutionPipeline> _logger;

    public ActionExecutionPipeline(
        ActionExecutorFactory executorFactory,
        ILogger<ActionExecutionPipeline> logger)
    {
        _executorFactory = executorFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync(IReadOnlyList<RuleAction> actions, PricingContext context)
    {
        var stepNo = 1;

        foreach (var action in actions)
        {
            var executor = _executorFactory.GetExecutor(action.ActionType);
            if (executor is null)
            {
                _logger.LogWarning("未找到动作执行器: {ActionType}", action.ActionType);

                if (action.OnError == "STOP")
                {
                    throw new InvalidOperationException($"未找到动作执行器: {action.ActionType}");
                }

                continue;
            }

            var inputValue = context.FinalAmount;

            try
            {
                await executor.ExecuteAsync(action, context);

                context.TraceSteps.Add(new TraceStep
                {
                    StepNo = stepNo++,
                    StepType = action.ActionType,
                    StepDesc = $"执行 {action.ActionType}",
                    InputValue = inputValue,
                    OutputValue = context.FinalAmount,
                    ParamsJson = action.ParamsJson
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行动作失败 ActionType={ActionType}, ActionId={ActionId}",
                    action.ActionType, action.ActionId);

                if (action.OnError == "STOP")
                {
                    throw;
                }

                if (action.OnError == "WARN")
                {
                    context.TraceSteps.Add(new TraceStep
                    {
                        StepNo = stepNo++,
                        StepType = action.ActionType,
                        StepDesc = $"执行异常(已跳过): {ex.Message}",
                        InputValue = inputValue,
                        OutputValue = context.FinalAmount
                    });
                }
            }
        }
    }
}
