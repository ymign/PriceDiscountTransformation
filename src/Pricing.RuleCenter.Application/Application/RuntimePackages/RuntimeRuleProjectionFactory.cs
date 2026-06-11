using System.Text.Json;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 运行期规则投影工厂，负责把策略版本和模板步骤映射为引擎直接消费的运行时快照。
/// </summary>
public sealed class RuntimeRuleProjectionFactory
{
    /// <summary>
    /// 兼容旧规则动作参数 JSON 的特殊参数编码。
    /// </summary>
    public const string LegacyActionParamsJsonParamCode = "LEGACY_ACTION_PARAMS_JSON";

    private readonly IPolicyPriorityKeyFactory _priorityKeyFactory;

    /// <summary>
    /// 初始化运行期规则投影工厂。
    /// </summary>
    /// <param name="priorityKeyFactory">优先级键生成器。</param>
    public RuntimeRuleProjectionFactory(IPolicyPriorityKeyFactory priorityKeyFactory)
    {
        _priorityKeyFactory = priorityKeyFactory;
    }

    /// <summary>
    /// 把策略版本、绑定、作用域和模板步骤投影为运行期规则快照。
    /// </summary>
    /// <param name="policyVersion">策略版本。</param>
    /// <param name="templateVersion">模板版本。</param>
    /// <param name="bindings">绑定对象集合。</param>
    /// <param name="scopes">作用域集合。</param>
    /// <param name="parameters">策略参数集合。</param>
    /// <param name="stepDefs">模板步骤定义集合。</param>
    /// <returns>可直接写入运行包的规则快照集合。</returns>
    public IReadOnlyList<RuntimeRuleSnapshot> Create(
        PolicyVersion policyVersion,
        TemplateVersion templateVersion,
        IReadOnlyList<PolicyBinding> bindings,
        IReadOnlyList<PolicyScope> scopes,
        IReadOnlyList<PolicyParam> parameters,
        IReadOnlyList<TemplateStepDef> stepDefs)
    {
        var actionSteps = stepDefs
            .Where(step => string.Equals(step.StepKind, "ACTION", StringComparison.OrdinalIgnoreCase))
            .OrderBy(step => step.StepNo)
            .ToArray();
        var conditions = scopes
            .Select((scope, index) => MapScopeToCondition(scope, index + 1))
            .Where(condition => condition is not null)
            .Select(condition => condition!)
            .ToArray();

        return bindings.Select(binding =>
        {
            var priorityKey = _priorityKeyFactory.Build(policyVersion, binding, scopes);
            return new RuntimeRuleSnapshot
            {
                Rule = new RuntimeRule
                {
                    SourceTemplateVersionId = templateVersion.TemplateVersionId,
                    SourcePolicyVersionId = policyVersion.PolicyVersionId,
                    CapabilityFamily = templateVersion.CapabilityFamily,
                    MergeMode = templateVersion.MergeMode,
                    TargetItemCode = Normalize(binding.ItemCode),
                    TargetGroupCode = Normalize(binding.GroupCode),
                    ScopeLevel = policyVersion.ScopeLevel,
                    PriorityKey = priorityKey,
                    EffectiveFrom = policyVersion.EffectiveFrom,
                    EffectiveTo = policyVersion.EffectiveTo,
                    MatchKey = BuildMatchKey(binding, templateVersion.CapabilityFamily, policyVersion.ScopeLevel)
                },
                Conditions = conditions
                    .Select(CloneCondition)
                    .ToList(),
                Actions = actionSteps
                    .Select(step => new RuntimeAction
                    {
                        StepNo = step.StepNo,
                        ActionType = step.ActionType ?? string.Empty,
                        ExecutorCode = step.ExecutorCode ?? string.Empty,
                        ParamsJson = BuildActionParamsJson(step, parameters),
                        SortNo = step.StepNo,
                        OnError = string.IsNullOrWhiteSpace(step.OnError) ? ActionOnErrorCodes.Stop : step.OnError
                    })
                    .ToList()
            };
        }).ToList();
    }

    private static RuntimeCondition? MapScopeToCondition(PolicyScope scope, int sortNo)
    {
        var conditionType = scope.ScopeDimension.Trim().ToUpperInvariant() switch
        {
            "SCENE" => RuleConditionTypeCodes.ChargeScene,
            "BODY_PART" => RuleConditionTypeCodes.BodyPart,
            "VISIT_TYPE" => RuleConditionTypeCodes.VisitTypeMatch,
            "TIME_RANGE" => RuleConditionTypeCodes.TimeRange,
            _ => null
        };
        if (conditionType is null)
        {
            return null;
        }

        return new RuntimeCondition
        {
            ConditionGroup = "DEFAULT",
            ConditionType = conditionType,
            OperatorType = string.IsNullOrWhiteSpace(scope.ScopeOperator) ? "EQ" : scope.ScopeOperator,
            LeftKey = scope.ScopeDimension,
            RightValue = GetScopeValue(scope),
            ParamsJson = scope.ScopeJson,
            SortNo = sortNo
        };
    }

    private static string BuildActionParamsJson(TemplateStepDef stepDef, IReadOnlyList<PolicyParam> parameters)
    {
        var legacyRawJson = parameters.FirstOrDefault(parameter =>
            string.Equals(parameter.ParamCode, LegacyActionParamsJsonParamCode, StringComparison.OrdinalIgnoreCase));
        if (legacyRawJson is not null && !string.IsNullOrWhiteSpace(legacyRawJson.ValueText))
        {
            return legacyRawJson.ValueText!;
        }

        var payload = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(stepDef.StepConfigClob))
        {
            payload["_templateStepConfig"] = stepDef.StepConfigClob;
        }

        foreach (var parameter in parameters)
        {
            object? value = parameter.ExprText;
            if (value is null && !string.IsNullOrWhiteSpace(parameter.ValueText))
            {
                value = parameter.ValueText;
            }

            if (value is null && parameter.ValueNumber.HasValue)
            {
                value = parameter.ValueNumber.Value;
            }

            if (value is null && parameter.ValueDate.HasValue)
            {
                value = parameter.ValueDate.Value.ToString("O");
            }

            if (value is null && !string.IsNullOrWhiteSpace(parameter.ValueBool))
            {
                value = parameter.ValueBool;
            }

            payload[parameter.ParamCode] = value;
        }

        return JsonSerializer.Serialize(payload);
    }

    private static RuntimeCondition CloneCondition(RuntimeCondition source)
    {
        return new RuntimeCondition
        {
            ConditionGroup = source.ConditionGroup,
            ConditionType = source.ConditionType,
            OperatorType = source.OperatorType,
            LeftKey = source.LeftKey,
            RightValue = source.RightValue,
            ParamsJson = source.ParamsJson,
            SortNo = source.SortNo
        };
    }

    private static string BuildMatchKey(PolicyBinding binding, string capabilityFamily, string scopeLevel)
    {
        var target = !string.IsNullOrWhiteSpace(binding.ItemCode)
            ? $"ITEM:{binding.ItemCode!.Trim().ToUpperInvariant()}"
            : $"GROUP:{Normalize(binding.GroupCode) ?? "*"}";
        var normalizedScopeLevel = string.IsNullOrWhiteSpace(scopeLevel) ? "HOSPITAL" : scopeLevel.Trim().ToUpperInvariant();
        return $"{capabilityFamily.Trim().ToUpperInvariant()}|{target}|{normalizedScopeLevel}";
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? GetScopeValue(PolicyScope scope)
    {
        if (!string.IsNullOrWhiteSpace(scope.ScopeValueText))
        {
            return scope.ScopeValueText.Trim();
        }

        if (scope.ScopeValueNumber.HasValue)
        {
            return scope.ScopeValueNumber.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        if (scope.ScopeValueDate.HasValue)
        {
            return scope.ScopeValueDate.Value.ToString("O");
        }

        return null;
    }
}
