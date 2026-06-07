using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleCapabilityRegistryTests
{
    [Fact]
    public void Registry_ShouldIncludeConditionAliasesAndFormulaExecutorAliases()
    {
        var registry = new RuleCapabilityRegistry(
            new IRuleConditionEvaluator[]
            {
                new FakeConditionEvaluator(RuleConditionTypeCodes.ItemMatch),
                new FakeConditionEvaluator(RuleConditionTypeCodes.ChargeScene)
            },
            new IRuleActionExecutor[]
            {
                new FakeFormulaExecutor(new[]
                {
                    FormulaExecutorCodes.IncrementPercent,
                    FormulaExecutorCodes.IncrementPercentExecutor
                }),
                new FakeActionExecutor(RuleActionTypeCodes.ApplyMaxAmount)
            });

        Assert.True(registry.SupportsConditionType(RuleConditionTypeCodes.ItemMatch));
        Assert.True(registry.SupportsConditionType(RuleConditionTypeCodes.ItemCode));
        Assert.True(registry.SupportsConditionType(RuleConditionTypeCodes.ChargeSceneMatch));
        Assert.True(registry.SupportsActionType(RuleActionTypeCodes.FormulaCalc));
        Assert.True(registry.SupportsActionType(RuleActionTypeCodes.ApplyMaxAmount));
        Assert.True(registry.SupportsFormulaExecutorCode(FormulaExecutorCodes.IncrementPercent));
        Assert.True(registry.SupportsFormulaExecutorCode(FormulaExecutorCodes.IncrementPercentExecutor));
    }

    [Fact]
    public void Registry_ShouldRejectFormulaExecutorWithoutMetadata()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new RuleCapabilityRegistry(
            Array.Empty<IRuleConditionEvaluator>(),
            new IRuleActionExecutor[] { new MissingMetadataFormulaExecutor() }));

        Assert.Contains(nameof(IFormulaExecutorCapabilityMetadata), ex.Message, StringComparison.Ordinal);
    }

    private sealed class FakeConditionEvaluator : IRuleConditionEvaluator
    {
        public FakeConditionEvaluator(string conditionType)
        {
            ConditionType = conditionType;
        }

        public string ConditionType { get; }

        public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context) =>
            ValueTask.FromResult(true);
    }

    private sealed class FakeActionExecutor : IRuleActionExecutor
    {
        public FakeActionExecutor(string actionType)
        {
            ActionType = actionType;
        }

        public string ActionType { get; }

        public Task ExecuteAsync(RuleAction action, PricingContext context) => Task.CompletedTask;
    }

    private sealed class FakeFormulaExecutor : IRuleActionExecutor, IFormulaExecutorCapabilityMetadata
    {
        public FakeFormulaExecutor(IReadOnlyCollection<string> supportedExecutorCodes)
        {
            SupportedExecutorCodes = supportedExecutorCodes;
        }

        public string ActionType => RuleActionTypeCodes.FormulaCalc;

        public IReadOnlyCollection<string> SupportedExecutorCodes { get; }

        public Task ExecuteAsync(RuleAction action, PricingContext context) => Task.CompletedTask;
    }

    private sealed class MissingMetadataFormulaExecutor : IRuleActionExecutor
    {
        public string ActionType => RuleActionTypeCodes.FormulaCalc;

        public Task ExecuteAsync(RuleAction action, PricingContext context) => Task.CompletedTask;
    }
}
