using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine.Formula;

namespace Pricing.RuleCenter.Application.Policies;

public sealed class PolicyValidationService
{
    private static readonly HashSet<string> SupportedScopeDimensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "SCENE",
        "BODY_PART",
        "VISIT_TYPE",
        "TIME_RANGE"
    };

    private readonly FormulaExpressionValidator _expressionValidator;

    public PolicyValidationService(FormulaExpressionValidator expressionValidator)
    {
        _expressionValidator = expressionValidator;
    }

    public void ValidateForCompile(
        PolicyAggregate policy,
        PolicyVersion version,
        TemplateVersion templateVersion,
        IReadOnlyList<TemplateParamDef> paramDefs,
        IReadOnlyList<TemplateStepDef> stepDefs,
        IReadOnlyList<TemplateScopeDef> scopeDefs,
        IReadOnlyList<PolicyBinding> bindings,
        IReadOnlyList<PolicyScope> scopes,
        IReadOnlyList<PolicyParam> parameters)
    {
        if (!string.Equals(version.PolicyStatus, PolicyLifecycleCodes.PublishReady, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.PolicyStatusNotAllowed,
                409,
                $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 当前不是 PUBLISH_READY，不能参与构建。");
        }

        if (bindings.Count == 0)
        {
            throw new BizException(
                BizErrorCode.PolicyBindingMissing,
                400,
                $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 未配置绑定对象。");
        }

        foreach (var scope in scopes)
        {
            if (!SupportedScopeDimensions.Contains(scope.ScopeDimension))
            {
                throw new BizException(
                    BizErrorCode.PolicyScopeUnsupported,
                    400,
                    $"策略 {policy.PolicyCode} 的作用域维度 {scope.ScopeDimension} 当前阶段不支持。");
            }
        }

        var allowedScopeDimensions = scopeDefs
            .Select(def => def.ScopeDimension)
            .Where(dimension => !string.IsNullOrWhiteSpace(dimension))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (allowedScopeDimensions.Count > 0)
        {
            foreach (var scope in scopes)
            {
                if (!allowedScopeDimensions.Contains(scope.ScopeDimension))
                {
                    throw new BizException(
                        BizErrorCode.PolicyScopeUnsupported,
                        400,
                        $"策略 {policy.PolicyCode} 的作用域维度 {scope.ScopeDimension} 不在模板允许范围内。");
                }
            }
        }

        foreach (var requiredScopeDef in scopeDefs.Where(def => def.IsRequired == EnableFlag.Yes))
        {
            if (!scopes.Any(scope => string.Equals(scope.ScopeDimension, requiredScopeDef.ScopeDimension, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BizException(
                    BizErrorCode.PolicyScopeUnsupported,
                    400,
                    $"策略 {policy.PolicyCode} 缺少必填作用域 {requiredScopeDef.ScopeDimension}。");
            }
        }

        var paramsByCode = parameters.ToDictionary(param => param.ParamCode, StringComparer.OrdinalIgnoreCase);
        foreach (var paramDef in paramDefs)
        {
            if (!paramsByCode.TryGetValue(paramDef.ParamCode, out var parameter))
            {
                if (paramDef.IsRequired == EnableFlag.Yes)
                {
                    throw new BizException(
                        BizErrorCode.PolicyParamMissing,
                        400,
                        $"策略 {policy.PolicyCode} 缺少必填参数 {paramDef.ParamCode}。");
                }

                continue;
            }

            ValidateParameter(policy, paramDef, parameter);
        }

        foreach (var stepDef in stepDefs.Where(step => string.Equals(step.StepKind, "ACTION", StringComparison.OrdinalIgnoreCase)))
        {
            if (!RuleActionTypeCodes.IsSupported(stepDef.ActionType))
            {
                throw new BizException(
                    BizErrorCode.RuleActionUnsupported,
                    400,
                    $"模板版本 {templateVersion.TemplateVersionId} 使用了当前引擎不支持的动作类型 {stepDef.ActionType}。");
            }

            if (string.Equals(stepDef.ActionType, RuleActionTypeCodes.FormulaCalc, StringComparison.OrdinalIgnoreCase) &&
                !FormulaExecutorCodes.IsSupported(stepDef.ExecutorCode))
            {
                throw new BizException(
                    BizErrorCode.RuleFormulaUnsupported,
                    400,
                    $"模板版本 {templateVersion.TemplateVersionId} 使用了当前引擎不支持的公式执行器 {stepDef.ExecutorCode}。");
            }
        }
    }

    private void ValidateParameter(PolicyAggregate policy, TemplateParamDef paramDef, PolicyParam parameter)
    {
        if (!string.IsNullOrWhiteSpace(parameter.ExprText))
        {
            if (!string.Equals(parameter.ExprLevel, "WEAK", StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.PolicyParamInvalid,
                    400,
                    $"策略 {policy.PolicyCode} 的参数 {paramDef.ParamCode} 当前仅允许弱表达式。");
            }

            try
            {
                _expressionValidator.Validate(parameter.ExprText);
            }
            catch (Exception ex)
            {
                throw new BizException(
                    BizErrorCode.RuleFormulaUnsupported,
                    400,
                    $"策略 {policy.PolicyCode} 的参数 {paramDef.ParamCode} 表达式非法: {ex.Message}");
            }
        }

        if (string.Equals(paramDef.ValueType, "NUMBER", StringComparison.OrdinalIgnoreCase) &&
            parameter.ValueNumber.HasValue)
        {
            if (paramDef.MinValue.HasValue && parameter.ValueNumber.Value < paramDef.MinValue.Value)
            {
                throw new BizException(
                    BizErrorCode.PolicyParamInvalid,
                    400,
                    $"策略 {policy.PolicyCode} 的参数 {paramDef.ParamCode} 小于最小值 {paramDef.MinValue.Value}。");
            }

            if (paramDef.MaxValue.HasValue && parameter.ValueNumber.Value > paramDef.MaxValue.Value)
            {
                throw new BizException(
                    BizErrorCode.PolicyParamInvalid,
                    400,
                    $"策略 {policy.PolicyCode} 的参数 {paramDef.ParamCode} 大于最大值 {paramDef.MaxValue.Value}。");
            }
        }
    }
}
