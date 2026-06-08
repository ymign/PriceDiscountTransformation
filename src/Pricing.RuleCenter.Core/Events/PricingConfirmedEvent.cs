namespace Pricing.RuleCenter.Core.Events;

/// <summary>
/// 计价确认领域事件，当 confirm 调用成功、额度占用完成时触发。
/// </summary>
/// <remarks>
/// <para>
/// 【触发场景】
/// 渠道调用 POST /api/pricing/calculate/confirm，PricingConfirmWorkflow 在事务内
/// 完成引擎计算、写请求日志、写折价明细、写限额占用后，产生此事件。
/// </para>
/// <para>
/// 【预期副作用】
/// 1. 异步通知 HIS 确认结果已就绪（待集成消息总线后实现）
/// 2. 写入审计日志，记录"谁在什么时间确认了什么项目的计价"
/// 3. 触发对账预检（confirm 结果与 HIS 落账明细的异步比对，待实现）
/// </para>
/// <para>
/// 【为什么不用事件替代事务内的写入】
/// confirm 的核心写入（请求日志、折价明细、限额占用）必须在同一事务中完成，
/// 否则会出现资金口径断裂。领域事件只用于事务提交后的副作用，
/// 不能替代事务内的同步写入。这是"事件用于解耦副作用，不用于解耦一致性"的原则。
/// </para>
/// </remarks>
public sealed record PricingConfirmedEvent : IDomainEvent
{
    /// <summary>
    /// 初始化计价确认事件。
    /// </summary>
    /// <param name="requestId">计价请求日志主键，confirm 返回给渠道的 RequestId。</param>
    /// <param name="traceId">计价追踪号，用于跨表追溯一次完整计价过程。</param>
    /// <param name="patientId">患者标识，用于审计和通知定位。</param>
    /// <param name="itemCode">项目编码，用于审计和通知定位。</param>
    /// <param name="occurredAt">事件发生时间，由应用服务在 confirm 事务提交后传入。</param>
    public PricingConfirmedEvent(
        long requestId,
        string? traceId,
        string? patientId,
        string? itemCode,
        DateTime occurredAt)
    {
        RequestId = requestId;
        TraceId = traceId;
        PatientId = patientId;
        ItemCode = itemCode;
        OccurredAt = occurredAt;
    }

    /// <summary>
    /// 计价请求日志主键，对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID。
    /// </summary>
    /// <remarks>
    /// 渠道后续 commit/cancel 必须携带此 ID。
    /// 事件处理器可用此 ID 查询完整的请求上下文。
    /// </remarks>
    public long RequestId { get; }

    /// <summary>
    /// 计价追踪号，用于跨表追溯一次完整计价过程。
    /// </summary>
    /// <remarks>
    /// 可为空。空值表示该请求未生成追踪号（理论上不应出现）。
    /// </remarks>
    public string? TraceId { get; }

    /// <summary>
    /// 患者标识，用于审计和通知定位。
    /// </summary>
    /// <remarks>
    /// 可为空。空值表示该请求不关联特定患者。
    /// </remarks>
    public string? PatientId { get; }

    /// <summary>
    /// 项目编码，用于审计和通知定位。
    /// </summary>
    /// <remarks>
    /// 可为空。空值表示该请求不关联特定项目（如批量场景）。
    /// </remarks>
    public string? ItemCode { get; }

    /// <summary>
    /// 事件发生时间，由应用服务在 confirm 事务提交后传入。
    /// </summary>
    public DateTime OccurredAt { get; }
}
