using Pricing.RuleCenter.Core.Engine.Evaluators;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ExtraParamConditionEvaluatorTests
{
    [Theory]
    [InlineData("INSURANCE_TYPE_MATCH", "insuranceType", "URBAN_EMPLOYEE")]
    [InlineData("DIAGNOSIS_MATCH", "diagnosisCodes", "I10,E11")]
    [InlineData("DEVICE_TYPE_MATCH", "deviceType", "CT64")]
    public async Task EvaluateAsync_ReturnsTrueWhenExtraParamMatches(
        string conditionType,
        string extraKey,
        string extraValue)
    {
        var evaluator = CreateEvaluator(conditionType);
        var condition = new RuleCondition
        {
            ConditionType = conditionType,
            RightValue = extraValue.Split(',')[0]
        };
        var context = new PricingContext
        {
            ExtraParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [extraKey] = extraValue
            }
        };

        var matched = await evaluator.EvaluateAsync(condition, context);

        Assert.True(matched);
    }

    [Theory]
    [InlineData("INSURANCE_TYPE_MATCH")]
    [InlineData("DIAGNOSIS_MATCH")]
    [InlineData("DEVICE_TYPE_MATCH")]
    public async Task EvaluateAsync_ReturnsTrueWhenRightValueIsEmpty(string conditionType)
    {
        var evaluator = CreateEvaluator(conditionType);

        var matched = await evaluator.EvaluateAsync(
            new RuleCondition { ConditionType = conditionType, RightValue = null },
            new PricingContext());

        Assert.True(matched);
    }

    [Theory]
    [InlineData("INSURANCE_TYPE_MATCH")]
    [InlineData("DIAGNOSIS_MATCH")]
    [InlineData("DEVICE_TYPE_MATCH")]
    public async Task EvaluateAsync_ReturnsFalseWhenContextValueMissing(string conditionType)
    {
        var evaluator = CreateEvaluator(conditionType);

        var matched = await evaluator.EvaluateAsync(
            new RuleCondition { ConditionType = conditionType, RightValue = "A" },
            new PricingContext());

        Assert.False(matched);
    }

    [Theory]
    [InlineData("INSURANCE_TYPE_MATCH", "insuranceType", "SELF_PAY", "URBAN_EMPLOYEE")]
    [InlineData("DIAGNOSIS_MATCH", "diagnosisCodes", "K35", "I10,E11")]
    [InlineData("DEVICE_TYPE_MATCH", "deviceType", "MRI", "CT64")]
    public async Task EvaluateAsync_ReturnsFalseWhenExtraParamDoesNotMatch(
        string conditionType,
        string extraKey,
        string expected,
        string actual)
    {
        var evaluator = CreateEvaluator(conditionType);
        var context = new PricingContext
        {
            ExtraParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [extraKey] = actual
            }
        };

        var matched = await evaluator.EvaluateAsync(
            new RuleCondition { ConditionType = conditionType, RightValue = expected },
            context);

        Assert.False(matched);
    }

    private static IRuleConditionEvaluator CreateEvaluator(string conditionType)
    {
        return conditionType switch
        {
            "INSURANCE_TYPE_MATCH" => new InsuranceTypeMatchEvaluator(),
            "DIAGNOSIS_MATCH" => new DiagnosisMatchEvaluator(),
            "DEVICE_TYPE_MATCH" => new DeviceTypeMatchEvaluator(),
            _ => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, null)
        };
    }
}
