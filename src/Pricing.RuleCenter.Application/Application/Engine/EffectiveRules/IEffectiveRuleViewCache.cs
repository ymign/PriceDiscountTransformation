namespace Pricing.RuleCenter.Application.Engine.EffectiveRules;

/// <summary>
/// 当前生效规则视图缓存抽象。
/// </summary>
public interface IEffectiveRuleViewCache
{
    /// <summary>
    /// 按项目编码获取当前运行期候选规则视图。
    /// </summary>
    /// <param name="itemCode">收费项目编码。</param>
    /// <returns>候选规则视图集合。</returns>
    Task<IReadOnlyList<EffectiveRuleView>> GetByItemCodeAsync(string itemCode);

    /// <summary>
    /// 清除当前缓存中的全部规则视图项。
    /// </summary>
    /// <returns>实际移除的缓存键数量。</returns>
    int Clear();
}
