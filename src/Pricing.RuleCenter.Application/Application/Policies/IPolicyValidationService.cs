using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Templates;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略版本编译校验服务契约。
/// </summary>
public interface IPolicyValidationService
{
    /// <summary>
    /// 校验策略版本是否具备生成直接规则配置的条件。
    /// </summary>
    void ValidateForCompile(
        PolicyAggregate policy,
        PolicyVersion version,
        TemplateVersion templateVersion,
        IReadOnlyList<TemplateParamDef> paramDefs,
        IReadOnlyList<TemplateStepDef> stepDefs,
        IReadOnlyList<TemplateScopeDef> scopeDefs,
        IReadOnlyList<PolicyBinding> bindings,
        IReadOnlyList<PolicyScope> scopes,
        IReadOnlyList<PolicyParam> parameters,
        bool requirePublishReadyStatus = true);
}
