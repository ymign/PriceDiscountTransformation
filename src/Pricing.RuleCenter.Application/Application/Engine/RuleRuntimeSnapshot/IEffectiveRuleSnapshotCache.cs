namespace Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照缓存抽象。
/// </summary>
public interface IEffectiveRuleSnapshotCache
{
    /// <summary>
    /// 按项目编码获取当前运行期候选规则快照。
    /// </summary>
    /// <param name="itemCode">收费项目编码。</param>
    /// <returns>候选规则快照集合。</returns>
    Task<IReadOnlyList<EffectiveRuleSnapshot>> GetByItemCodeAsync(string itemCode);

    /// <summary>
    /// 清除当前缓存中的全部快照项。
    /// </summary>
    /// <returns>实际移除的缓存键数量。</returns>
    int Clear();
}
