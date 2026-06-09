using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 限额占额草稿最终结算器。
/// </summary>
public interface ILimitOccupyValueFinalizer
{
    /// <summary>
    /// 是否为兜底结算器。
    /// </summary>
    /// <remarks>
    /// 当存在至少一个非兜底结算器命中时，计价引擎应忽略兜底结算器。
    /// 这样新增专项限额类型时，不会因为兜底实现也命中而产生多匹配冲突。
    /// </remarks>
    bool IsFallback => false;

    /// <summary>
    /// 判断当前结算器是否处理该占额草稿。
    /// </summary>
    /// <param name="occupy">待结算的占额草稿。</param>
    /// <returns>可处理返回 <c>true</c>。</returns>
    bool CanHandle(LimitOccupy occupy);

    /// <summary>
    /// 将最终计价结果回填到占额草稿。
    /// </summary>
    /// <param name="occupy">待结算的占额草稿。</param>
    /// <param name="context">当前计价上下文。</param>
    void Apply(LimitOccupy occupy, PricingContext context);
}
