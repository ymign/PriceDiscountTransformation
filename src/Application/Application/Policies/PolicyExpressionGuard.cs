using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Policies;

public sealed class PolicyExpressionGuard
{
    public void EnsureAllowed(IReadOnlyList<PolicyParamDto> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (string.IsNullOrWhiteSpace(parameter.ExprText))
            {
                continue;
            }

            if (!string.Equals(parameter.ExprLevel, "WEAK", StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.PolicyParamInvalid,
                    400,
                    $"参数 {parameter.ParamCode} 当前仅允许弱表达式。");
            }
        }
    }
}
