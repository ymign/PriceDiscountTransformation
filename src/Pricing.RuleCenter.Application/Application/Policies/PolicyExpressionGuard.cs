using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略参数表达式门禁。
/// </summary>
internal sealed class PolicyExpressionGuard : IPolicyExpressionGuard
{
    /// <summary>
    /// 校验策略参数请求是否满足当前表达式级别约束。
    /// </summary>
    /// <param name="parameters">待校验的策略参数。</param>
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
