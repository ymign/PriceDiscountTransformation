using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略参数表达式门禁。
/// </summary>
public interface IPolicyExpressionGuard
{
    /// <summary>
    /// 校验策略参数请求是否满足当前表达式级别约束。
    /// </summary>
    /// <param name="parameters">待校验的策略参数。</param>
    void EnsureAllowed(IReadOnlyList<PolicyParamDto> parameters);
}
