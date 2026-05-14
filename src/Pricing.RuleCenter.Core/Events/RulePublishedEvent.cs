namespace Pricing.RuleCenter.Core.Events;

/// <summary>
/// 规则发布领域事件，当规则从"草稿/已停用"状态推进到"已发布"时触发。
/// </summary>
/// <remarks>
/// <para>
/// 【触发场景】
/// 规则维护人员在工作台点击"发布"按钮，规则版本通过校验后，
/// RuleAggregate.Publish() 方法将状态推进为 Published 并产生此事件。
/// </para>
/// <para>
/// 【预期副作用】
/// 1. 清除该规则的生效缓存（EffectiveRuleCacheKeys），确保下次计价请求读取最新版本
/// 2. 写入规则变更审计日志（PR_RULE_CHANGE_LOG）
/// 3. 通知 HIS 工作台刷新规则列表（待集成消息总线后实现）
/// </para>
/// <para>
/// 【为什么用事件而不是直接调缓存】
/// 聚合根不应依赖缓存基础设施。如果 RuleAggregate 直接调用 ICache.Clear()，
/// 就违反了领域层不依赖基础设施的原则。事件机制让聚合根只声明"我发布了"，
/// 具体做什么由应用层/基础设施层的事件处理器决定。
/// </para>
/// </remarks>
public sealed record RulePublishedEvent : IDomainEvent
{
    /// <summary>
    /// 初始化规则发布事件。
    /// </summary>
    /// <param name="ruleHeaderId">规则头主键，用于定位缓存和审计记录。</param>
    /// <param name="versionNo">发布的目标版本号，用于审计追溯。</param>
    /// <param name="occurredAt">事件发生时间，由聚合根在 Publish() 方法中传入 DateTime.Now。</param>
    public RulePublishedEvent(long ruleHeaderId, int versionNo, DateTime occurredAt)
    {
        RuleHeaderId = ruleHeaderId;
        VersionNo = versionNo;
        OccurredAt = occurredAt;
    }

    /// <summary>
    /// 规则头主键，对应 PR_RULE_HEADER.ID。
    /// </summary>
    /// <remarks>
    /// 事件处理器用此字段定位需要清除缓存的规则，
    /// 以及写入 PR_RULE_CHANGE_LOG 的关联外键。
    /// </remarks>
    public long RuleHeaderId { get; }

    /// <summary>
    /// 发布的目标版本号，对应 PR_RULE_VERSION.VERSION_NO。
    /// </summary>
    /// <remarks>
    /// 审计日志需要记录"从哪个版本发布到哪个版本"，
    /// 此字段与聚合根的 PreviousVersion 配合使用。
    /// </remarks>
    public int VersionNo { get; }

    /// <summary>
    /// 事件发生时间，由聚合根在状态转换时自动填充。
    /// </summary>
    public DateTime OccurredAt { get; }
}