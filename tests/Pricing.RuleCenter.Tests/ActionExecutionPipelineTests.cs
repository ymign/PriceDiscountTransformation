using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Core.Aggregates.Rules;
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

    [Fact]
    public async Task ExecuteAsync_AllowsMultipleFormulaExecutorsAndExecutesMatchingExecutorCode()
    {
        var first = new FormulaExecutor("FIRST_FORMULA", 11m);
        var second = new FormulaExecutor("SECOND_FORMULA", 22m);
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                first,
                second
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
                ActionType = "FORMULA_CALC",
                ExecutorCode = "SECOND_FORMULA",
                IsEnabled = "Y",
                OnError = "STOP"
            }
        }, context);

        Assert.False(first.WasExecuted);
        Assert.True(second.WasExecuted);
        Assert.Equal(22m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsExecutorProvidedInvalidTraceStepType()
    {
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new InvalidTraceStepExecutor()
            }),
            NullLogger<ActionExecutionPipeline>.Instance);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(new[]
            {
                new RuleAction
                {
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "INVALID_TRACE",
                    IsEnabled = "Y",
                    OnError = "STOP"
                }
            }, new PricingContext { FinalAmount = 100m }));

        Assert.Contains("非法追溯步骤类型", ex.Message);
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

    private sealed class FormulaExecutor : IRuleActionExecutor
    {
        private readonly string _executorCode;
        private readonly decimal _amount;

        public FormulaExecutor(string executorCode, decimal amount)
        {
            _executorCode = executorCode;
            _amount = amount;
        }

        public string ActionType => "FORMULA_CALC";

        public bool WasExecuted { get; private set; }

        public bool CanHandle(RuleAction action)
        {
            return string.Equals(action.ExecutorCode, _executorCode, StringComparison.OrdinalIgnoreCase);
        }

        public Task ExecuteAsync(RuleAction action, PricingContext context)
        {
            if (!CanHandle(action))
            {
                return Task.CompletedTask;
            }

            WasExecuted = true;
            context.FinalAmount = _amount;
            return Task.CompletedTask;
        }
    }

    private sealed class InvalidTraceStepExecutor : IRuleActionExecutor
    {
        public string ActionType => "FORMULA_CALC";

        public string TraceStepType => "CUSTOM";

        public bool CanHandle(RuleAction action)
        {
            return string.Equals(action.ExecutorCode, "INVALID_TRACE", StringComparison.OrdinalIgnoreCase);
        }

        public Task ExecuteAsync(RuleAction action, PricingContext context)
        {
            context.FinalAmount = 123m;
            return Task.CompletedTask;
        }
    }
}
