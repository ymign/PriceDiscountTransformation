using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 收费项目条件评估器。
/// </summary>
/// <remarks>
/// 项目匹配是最基础的规则命中条件。与部位、场景不同，项目条件如果没有 RightValue，
/// 视为配置不完整并返回不命中，避免一条本应限定项目的条件变成全局通配。
/// </remarks>
public sealed class ItemMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public string ConditionType => "ITEM_MATCH";

    /// <summary>
    /// 判断请求项目编码是否等于规则条件中的项目编码。
    /// </summary>
    /// <param name="condition">规则条件，RightValue 保存目标项目编码。</param>
    /// <param name="context">计价上下文，ItemCode 来自本次收费请求。</param>
    /// <returns>项目编码相同时返回 <c>true</c>。</returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 项目条件缺少目标值时不应放行，否则可能让本该限定单项目的规则影响所有项目。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return false;
        }

        // 项目编码按业务编码比较，不区分大小写，兼容 HIS 或配置端大小写差异。
        return string.Equals(context.ItemCode, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
