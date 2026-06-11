using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.Executors;
using Pricing.RuleCenter.Application.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class FormulaExpressionEvaluatorTests
{
    private readonly FormulaExpressionEvaluator _evaluator = new(new FormulaFunctionRegistry());

    [Theory]
    [InlineData("unitPrice * finalQty * 0.5", 100)]
    [InlineData("min(unitPrice * finalQty, 440)", 200)]
    [InlineData("max(unitPrice * finalQty, 10)", 200)]
    [InlineData("round(unitPrice * finalQty / 3, 2)", 66.67)]
    public void Evaluate_ReturnsExpectedDecimalResult(string expression, decimal expected)
    {
        var context = new FormulaEvaluationContext
        {
            UnitPrice = 100m,
            FinalQty = 2m,
            InputQty = 2m,
            ConvertedQty = 2m,
            OriginalAmount = 200m,
            FinalAmount = 200m,
            PartCount = 0m,
            Area = 0m
        };

        var actual = _evaluator.Evaluate(expression, context);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Evaluate_RejectsUnknownVariable()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _evaluator.Evaluate("unitPrice * unknownQty", new FormulaEvaluationContext()));

        Assert.Contains("unknownQty", ex.Message);
    }

    [Fact]
    public void Evaluate_RejectsUnknownFunction()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _evaluator.Evaluate("pow(unitPrice, 2)", new FormulaEvaluationContext()));

        Assert.Contains("pow", ex.Message);
    }

    [Fact]
    public void Evaluate_RejectsInvalidExpression()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _evaluator.Evaluate("unitPrice *", new FormulaEvaluationContext()));
    }

    [Fact]
    public void Validator_AcceptsWhitelistedExpression()
    {
        var validator = new FormulaExpressionValidator(_evaluator);

        validator.Validate("min(unitPrice * finalQty * 0.5, 440)");
    }

    [Fact]
    public async Task ExpressionFormulaExecutor_WritesFormulaAmountAndFinalAmount()
    {
        var executor = new ExpressionFormulaExecutor(_evaluator);
        var context = new PricingContext
        {
            InputQty = 3m,
            ConvertedQty = 3m,
            FinalQty = 2m,
            UnitPrice = 100m,
            FinalAmount = 300m,
            PricingParts = new[]
            {
                new PricingPartItem { Area = 10m },
                new PricingPartItem { Area = 20m }
            }
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "EXPRESSION_FORMULA",
            ParamsJson = JsonSerializer.Serialize(new
            {
                Expression = "min(unitPrice * finalQty * 0.5 + area, 440)",
                AmountField = "FinalAmount"
            })
        }, context);

        Assert.Equal(130m, context.FormulaAmount);
        Assert.Equal(130m, context.FinalAmount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("{}")]
    [InlineData("{\"Expression\":\" \"}")]
    public async Task ExpressionFormulaExecutor_ThrowsWhenExpressionParamIsMissing(string? paramsJson)
    {
        var executor = new ExpressionFormulaExecutor(_evaluator);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(new RuleAction
            {
                ActionType = "FORMULA_CALC",
                ExecutorCode = "EXPRESSION_FORMULA",
                ParamsJson = paramsJson
            }, new PricingContext()));

        Assert.Contains("EXPRESSION_FORMULA", ex.Message);
        Assert.Contains("expression", ex.Message);
    }

    [Fact]
    public async Task ActionExecutionPipeline_ExecutesExpressionFormulaByExecutorCode()
    {
        var expressionExecutor = new ExpressionFormulaExecutor(_evaluator);
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[] { expressionExecutor }),
            NullLogger<ActionExecutionPipeline>.Instance);
        var context = new PricingContext
        {
            InputQty = 2m,
            ConvertedQty = 2m,
            FinalQty = 2m,
            UnitPrice = 100m,
            FinalAmount = 200m
        };

        await pipeline.ExecuteAsync(new[]
        {
            new RuleAction
            {
                ActionType = "FORMULA_CALC",
                ExecutorCode = "EXPRESSION_FORMULA",
                OnError = "STOP",
                ParamsJson = JsonSerializer.Serialize(new
                {
                    Expression = "unitPrice * finalQty * 0.5",
                    AmountField = "FinalAmount"
                })
            }
        }, context);

        Assert.Equal(100m, context.FinalAmount);
        Assert.Equal(100m, context.FormulaAmount);
        var step = Assert.Single(context.TraceSteps);
        Assert.Equal("FORMULA", step.StepType);
    }
}
