namespace Pricing.RuleCenter.Core.Events;

/// <summary>
/// 领域事件标记接口，所有领域事件必须实现此接口。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 领域事件是 DDD 中解耦聚合根行为与副作用的核心机制。
/// 聚合根在执行状态转换时只关心"业务规则是否满足"，
/// 而"缓存失效、审计日志、异步通知"等副作用由事件处理器负责。
/// 这样聚合根保持纯粹，不依赖基础设施细节。
/// </para>
/// <para>
/// 【事件生命周期】
/// 1. 聚合根方法执行状态转换 → 收集事件到 DomainEvents 列表
/// 2. 应用服务在事务提交后遍历 DomainEvents → 分发给处理器
/// 3. 处理器执行副作用（清除缓存、写审计日志等）
/// </para>
/// <para>
/// 【当前实现状态】
/// 事件定义已完成，聚合根已预留 DomainEvents 属性。
/// 事件分发机制（IDomainEventDispatcher）和应用服务集成待后续迭代实现。
/// 当前缓存失效和审计日志仍由应用服务直接执行，
/// 待事件分发机制就绪后逐步迁移到事件驱动模式。
/// </para>
/// </remarks>
public interface IDomainEvent
{
    /// <summary>
    /// 事件发生时间，由聚合根在状态转换时自动填充。
    /// </summary>
    /// <remarks>
    /// 使用服务器本地时间而非 UTC，与 Oracle 数据库的 DATE 类型口径一致。
    /// 事件处理器不应依赖此时间做跨时区判断，仅用于审计和排序。
    /// </remarks>
    DateTime OccurredAt { get; }
}