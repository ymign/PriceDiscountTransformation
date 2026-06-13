using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Application.Engine.Formula;
using Pricing.RuleCenter.Core.Constants;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyValidationServiceTests
{
    [Fact]
    public void ValidateForCompile_RejectsWhenRequiredParamIsMissing()
    {
        var service = CreateService();

        var ex = Assert.Throws<BizException>(() => service.ValidateForCompile(
            new PolicyAggregate { PolicyCode = "POL001" },
            new PolicyVersion { VersionNo = 1, PolicyStatus = PolicyLifecycleCodes.PublishReady, ScopeLevel = "SCENE" },
            new TemplateVersion { TemplateVersionId = 11, CapabilityFamily = TemplateCapabilityCodes.FormulaPricing, MergeMode = "SINGLE_WINNER" },
            new[] { new TemplateParamDef { ParamCode = "RATE", ValueType = "NUMBER", IsRequired = EnableFlag.Yes } },
            new[] { new TemplateStepDef { StepKind = "ACTION", ActionType = RuleActionTypeCodes.FormulaCalc, ExecutorCode = FormulaExecutorCodes.IncrementPercent } },
            new[] { new TemplateScopeDef { ScopeDimension = "SCENE", IsRequired = EnableFlag.Yes } },
            new[] { new PolicyBinding { ItemCode = "ITEM001" } },
            new[] { new PolicyScope { ScopeDimension = "SCENE", ScopeValueText = "OUTPATIENT" } },
            Array.Empty<PolicyParam>()));

        Assert.Equal(BizErrorCode.PolicyParamMissing, ex.Code);
    }

    [Fact]
    public void ValidateForCompile_RejectsUnsupportedScopeDimension()
    {
        var service = CreateService();

        var ex = Assert.Throws<BizException>(() => service.ValidateForCompile(
            new PolicyAggregate { PolicyCode = "POL002" },
            new PolicyVersion { VersionNo = 1, PolicyStatus = PolicyLifecycleCodes.PublishReady, ScopeLevel = "DEPT" },
            new TemplateVersion { TemplateVersionId = 12, CapabilityFamily = TemplateCapabilityCodes.FormulaPricing, MergeMode = "SINGLE_WINNER" },
            Array.Empty<TemplateParamDef>(),
            new[] { new TemplateStepDef { StepKind = "ACTION", ActionType = RuleActionTypeCodes.FormulaCalc, ExecutorCode = FormulaExecutorCodes.IncrementPercent } },
            new[] { new TemplateScopeDef { ScopeDimension = "DEPT", IsRequired = EnableFlag.No } },
            new[] { new PolicyBinding { ItemCode = "ITEM001" } },
            new[] { new PolicyScope { ScopeDimension = "DEPT", ScopeValueText = "D001" } },
            Array.Empty<PolicyParam>()));

        Assert.Equal(BizErrorCode.PolicyScopeUnsupported, ex.Code);
    }

    [Fact]
    public void ValidateForCompile_AllowsWeakExpressionAndValidParams()
    {
        var service = CreateService();

        service.ValidateForCompile(
            new PolicyAggregate { PolicyCode = "POL003" },
            new PolicyVersion { VersionNo = 1, PolicyStatus = PolicyLifecycleCodes.PublishReady, ScopeLevel = "SCENE" },
            new TemplateVersion { TemplateVersionId = 13, CapabilityFamily = TemplateCapabilityCodes.FormulaPricing, MergeMode = "SINGLE_WINNER" },
            new[]
            {
                new TemplateParamDef { ParamCode = "RATE", ValueType = "NUMBER", IsRequired = EnableFlag.Yes, MinValue = 0m, MaxValue = 1m }
            },
            new[] { new TemplateStepDef { StepKind = "ACTION", ActionType = RuleActionTypeCodes.FormulaCalc, ExecutorCode = FormulaExecutorCodes.IncrementPercent } },
            new[] { new TemplateScopeDef { ScopeDimension = "SCENE", IsRequired = EnableFlag.Yes } },
            new[] { new PolicyBinding { ItemCode = "ITEM001" } },
            new[] { new PolicyScope { ScopeDimension = "SCENE", ScopeValueText = "OUTPATIENT" } },
            new[]
            {
                new PolicyParam { ParamCode = "RATE", ValueType = "NUMBER", ValueNumber = 0.8m, ExprText = "finalQty * 0.5", ExprLevel = "WEAK" }
            });
    }

    private static PolicyValidationService CreateService()
    {
        var registry = new FormulaFunctionRegistry();
        var evaluator = new FormulaExpressionEvaluator(registry);
        var validator = new FormulaExpressionValidator(evaluator);
        return new PolicyValidationService(validator);
    }
}
