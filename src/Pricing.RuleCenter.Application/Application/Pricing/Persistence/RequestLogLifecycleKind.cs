using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 请求日志创建阶段的生命周期初始化类型。
/// </summary>
internal enum RequestLogLifecycleKind
{
    /// <summary>
    /// 试算请求日志。
    /// </summary>
    Simulated,

    /// <summary>
    /// 确认计价请求日志。
    /// </summary>
    ConfirmPending,

    /// <summary>
    /// reverse 派生请求日志。
    /// </summary>
    Reversed
}

/// <summary>
/// 计价请求日志生命周期初始化器。
/// </summary>
/// <remarks>
/// <para>
/// 该初始化器只处理“新建请求日志时”的初始状态，避免 simulate/confirm/reverse
/// 各自在 writer 或 persistence service 中直接拼装 <c>BusinessStatus</c>、
/// <c>IsSuccess</c> 和 <c>ResponseAt</c>。
/// </para>
/// <para>
/// 对已有请求日志的状态推进仍应优先走聚合根公开方法，如
/// <see cref="ChargeRequest.MarkCommitted"/>、<see cref="ChargeRequest.MarkCancelled"/>、
/// <see cref="ChargeRequest.MarkExpired"/> 和 <see cref="ChargeRequest.MarkReversed"/>。
/// </para>
/// </remarks>
internal static class RequestLogLifecycleInitializer
{
    /// <summary>
    /// 对新建请求日志应用初始生命周期状态。
    /// </summary>
    /// <param name="requestLog">新建但尚未持久化的请求日志。</param>
    /// <param name="lifecycleKind">初始生命周期类型。</param>
    /// <param name="now">当前技术时间。</param>
    public static void Apply(ChargeRequest requestLog, RequestLogLifecycleKind lifecycleKind, DateTime now)
    {
        switch (lifecycleKind)
        {
            case RequestLogLifecycleKind.Simulated:
                requestLog.BusinessStatus = BusinessStatusCodes.Simulated;
                requestLog.IsSuccess = EnableFlag.Yes;
                requestLog.ResponseAt = now;
                return;

            case RequestLogLifecycleKind.ConfirmPending:
                requestLog.MarkConfirmPending(now);
                return;

            case RequestLogLifecycleKind.Reversed:
                // 这里初始化的是“reverse 派生请求日志”自身的事实状态，
                // 不是把原收费请求从 CONFIRMED 推进到 REVERSED。
                requestLog.BusinessStatus = BusinessStatusCodes.Reversed;
                requestLog.IsSuccess = EnableFlag.Yes;
                requestLog.ResponseAt = now;
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifecycleKind), lifecycleKind, "未知的请求日志生命周期类型。");
        }
    }
}
