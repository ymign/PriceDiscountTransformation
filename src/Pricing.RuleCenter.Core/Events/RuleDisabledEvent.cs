namespace Pricing.RuleCenter.Core.Events;

/// <summary>
/// 规则停用领域事件，当已发布规则被手动停用时触发。
/// </summary>
/// <remarks>
/// <para>
/// 【触发场景】
/// 规则维护人员在工作台点击"停用"按钮，RuleAggregate.Disable() 方法
/// 将状态从 Published 推进为 Disabled 并产生此事件。
/// </para>
/// <para>
/// 【预期副作用】
/// 1. 清除该规则的生效缓存（EffectiveRuleCacheKeys），确保停用后不再被计价引擎匹配
/// 2. 写入规则变更审计日志（PR_RULE_CHANGE_LOG），记录停用原因和操作人
/// 3. 通知 HIS 工作台刷新规则列表（待集成消息总线后实现）
/// </para>
/// <para>
/// 【与 RulePublishedEvent 的区别】
/// 两者都需要清除缓存，但审计日志的操作类型不同（PUBLISH vs DISABLE）。
/// 分开两个事件类型，比用一个 RuleStatusChangedEvent + 枚举参数更符合 DDD 的
/// "显式建模"原则——每个事件只表达一种业务语义，处理器逻辑更清晰。
/// </para>
/// </remarks>
public sealed record RuleDisabledEvent : IDomainEvent
{
    /// <summary>
    /// 初始化规则停用事件。
    /// </summary>
    /// <param name="ruleHeaderId">规则头主键，用于定位缓存和审计记录。</param>
    /// <param name="reason">停用原因，由维护人员填写，写入审计日志。</param>
    /// <param name="occurredAt">事件发生时间，由聚合根在 Disable() 方法中传入 DateTime.Now。</param>
    public RuleDisabledEvent(long ruleHeaderId, string reason, DateTime occurredAt)
    {
        RuleHeaderId = ruleHeaderId;
        Reason = reason;
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
    /// 停用原因，由维护人员在停用操作时填写。
    /// </summary>
    /// <remarks>
    /// 此字段写入审计日志，确保每次停用都有可追溯的业务原因。
    /// 如果维护人员未填写原因，应用层应传入默认值如"管理员手动停用"。
    /// </remarks>
    public string Reason { get; }

    /// <summary>
    /// 事件发生时间，由聚合根在状态转换时自动填充。
    /// </summary>
    public DateTime OccurredAt { get; }
}