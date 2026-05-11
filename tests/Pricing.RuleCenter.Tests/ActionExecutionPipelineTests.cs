using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ActionExecutionPipelineTests
{
    [Theory]
    [InlineData("SAME_OPERATION_CEILING", "LIMIT")]
    [InlineData("ADD_CHILD_ITEM", "DISCOUNT")]
    public async Task ExecuteAsync_MapsKnownActionsToSemanticTraceStepTypes(
        string actionType,
        string expectedStepType)
    {
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new NoOpExecutor("SAME_OPERATION_CEILING"),
                new NoOpExecutor("ADD_CHILD_ITEM")
            }),
            NullLogger<ActionExecutionPipeline>.Instance);
        var context = new PricingContext
        {
            FinalAmount = 100m
        };

        await pipeline.ExecuteAsync(new[]
        {
            new RuleAction
            {
                ActionType = actionType,
                IsEnabled = "Y"
            }
        }, context);

        var step = Assert.Single(context.TraceSteps);
        Assert.Equal(expectedStepType, step.StepType);
    }

    private sealed class NoOpExecutor : IRuleActionExecutor
    {
        public NoOpExecutor(string actionType)
        {
            ActionType = actionType;
        }

        public string ActionType { get; }

        public Task ExecuteAsync(RuleAction action, PricingContext context) => Task.CompletedTask;
    }
}
