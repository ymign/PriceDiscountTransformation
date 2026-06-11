using Pricing.RuleCenter.Application.Engine.Evaluators;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ChargeDeptExcludeEvaluatorTests
{
    private readonly ChargeDeptExcludeEvaluator _evaluator = new();

    [Fact]
    public void RightValue为空_应通过()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = null };
        var context = new PricingContext { ChargeDeptCode = "7021" };

        Assert.True(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void ChargeDeptCode为空_应通过()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = "7021" };
        var context = new PricingContext { ChargeDeptCode = null };

        Assert.True(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void ChargeDeptCode匹配排除列表_应拒绝()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = "7021" };
        var context = new PricingContext { ChargeDeptCode = "7021" };

        Assert.False(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void ChargeDeptCode不在排除列表_应通过()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = "7021" };
        var context = new PricingContext { ChargeDeptCode = "1001" };

        Assert.True(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void 多个排除科室逗号分隔_命中其中一个_应拒绝()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = "7021,8050,9001" };
        var context = new PricingContext { ChargeDeptCode = "8050" };

        Assert.False(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void 多个排除科室逗号分隔_全不命中_应通过()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = "7021,8050,9001" };
        var context = new PricingContext { ChargeDeptCode = "2000" };

        Assert.True(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void 大小写不敏感_应拒绝()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = "DEPT_A" };
        var context = new PricingContext { ChargeDeptCode = "dept_a" };

        Assert.False(_evaluator.Evaluate(condition, context));
    }

    [Fact]
    public void 带前后空格的排除值_应拒绝()
    {
        var condition = new RuleCondition { ConditionType = "CHARGE_DEPT_EXCLUDE", RightValue = " 7021 " };
        var context = new PricingContext { ChargeDeptCode = "7021" };

        Assert.False(_evaluator.Evaluate(condition, context));
    }
}
