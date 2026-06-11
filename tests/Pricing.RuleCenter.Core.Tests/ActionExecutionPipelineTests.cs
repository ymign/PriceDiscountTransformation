using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class ActionExecutionPipelineTests
{
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
    public async Task ExecuteAsync_StopsWhenFormulaExecutorCodeIsNotRegistered()
    {
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new FormulaExecutor("KNOWN_FORMULA", 11m)
            }),
            NullLogger<ActionExecutionPipeline>.Instance);
        var context = new PricingContext
        {
            FinalAmount = 100m
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(new[]
            {
                new RuleAction
                {
                    ActionId = 123,
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "UNKNOWN_FORMULA",
                    IsEnabled = "Y",
                    OnError = "STOP"
                }
            }, context));

        Assert.Contains("UNKNOWN_FORMULA", exception.Message);
        Assert.Equal(100m, context.FinalAmount);
        Assert.Empty(context.TraceSteps);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("stop")]
    public async Task ExecuteAsync_TreatsMissingOrLowerCaseOnErrorAsStopWhenExecutorCodeIsUnknown(string? onError)
    {
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new FormulaExecutor("KNOWN_FORMULA", 11m)
            }),
            NullLogger<ActionExecutionPipeline>.Instance);
        var context = new PricingContext
        {
            FinalAmount = 100m
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(new[]
            {
                new RuleAction
                {
                    ActionId = 125,
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "UNKNOWN_FORMULA",
                    IsEnabled = "Y",
                    OnError = onError!
                }
            }, context));

        Assert.Equal(100m, context.FinalAmount);
        Assert.Empty(context.TraceSteps);
    }

    [Fact]
    public async Task ExecuteAsync_StopsWhenMultipleExecutorsCanHandleSameAction()
    {
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new FormulaExecutor("DUPLICATE_FORMULA", 11m),
                new FormulaExecutor("DUPLICATE_FORMULA", 22m)
            }),
            NullLogger<ActionExecutionPipeline>.Instance);
        var context = new PricingContext
        {
            FinalAmount = 100m
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(new[]
            {
                new RuleAction
                {
                    ActionId = 124,
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "DUPLICATE_FORMULA",
                    IsEnabled = "Y",
                    OnError = "STOP"
                }
            }, context));

        Assert.Contains("多个执行器", exception.Message);
        Assert.Equal(100m, context.FinalAmount);
        Assert.Empty(context.TraceSteps);
    }

    [Fact]
    public async Task ExecuteAsync_AssignsSequentialStepNumbersWhenExecutorAddsTraceStep()
    {
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new TracingExecutor()
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
                ActionType = "TRACE_TEST",
                ExecutorCode = "TracingExecutor",
                IsEnabled = "Y",
                OnError = "STOP"
            }
        }, context);

        Assert.Collection(
            context.TraceSteps,
            first => Assert.Equal(1, first.StepNo),
            second => Assert.Equal(2, second.StepNo));
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

    private sealed class TracingExecutor : IRuleActionExecutor
    {
        public string ActionType => "TRACE_TEST";

        public Task ExecuteAsync(RuleAction action, PricingContext context)
        {
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = "执行器内部追溯",
                InputValue = context.FinalAmount,
                OutputValue = context.FinalAmount
            });

            return Task.CompletedTask;
        }
    }
}
