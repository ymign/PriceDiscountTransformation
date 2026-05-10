namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// 计价请求的业务状态枚举，描述一笔计价请求在其生命周期中的当前阶段。
///
/// 状态机流转路径：
///   Simulated（试算）—— 试算请求，不占用额度，不进入后续流程
///   ConfirmPending（待确认）—— confirm 调用后等待 commit/cancel 的中间态
///   Confirmed（已确认）—— 额度已占用，等待 HIS 侧结算确认
///   Committed（已提交）—— HIS 结算成功，计价流程完结
///   Cancelled（已取消）—— 主动取消，释放额度
///   Expired（已过期）—— 超过挂起清理时限，由后台任务自动过期并释放额度
///   Reversed（已冲正）—— 退费/冲销操作，释放额度并记录逆向明细
///   Failed（失败）—— 计价过程出错，不占用额度
///
/// 存储位置：PR_CHARGE_REQUEST_LOG.STATUS 字段。
/// 注意：此状态与 OccupyStatus（额度占用状态）独立维护，需在 confirm/commit/cancel/reverse/expire
/// 操作中同时更新两者，保证一致性。
/// </summary>
public enum BusinessStatus
{
    /// <summary>
    /// 试算：模拟计价，不占用额度，不落持久化日志（或标记为试算）。
    /// 前端"预览费用"场景使用。
    /// </summary>
    Simulated,

    /// <summary>
    /// 待确认：confirm 接口已调用，额度已锁定，等待 HIS 侧 commit 或 cancel。
    /// 此状态有超时清理机制（后台扫描 PR_CHARGE_REQUEST_LOG 中长时间未变更的记录）。
    /// </summary>
    ConfirmPending,

    /// <summary>
    /// 已确认：HIS 侧已确认计价结果，额度正式占用，等待结算完成。
    /// 介于 confirm 与 commit 之间的稳态。
    /// </summary>
    Confirmed,

    /// <summary>
    /// 已提交：HIS 结算成功通知（commit），计价流程最终完结状态之一。
    /// 额度不再释放，折价明细归档。
    /// </summary>
    Committed,

    /// <summary>
    /// 已取消：主动调用 cancel 接口，释放已占用额度。
    /// 仅 ConfirmPending 或 Confirmed 状态可转为 Cancelled。
    /// </summary>
    Cancelled,

    /// <summary>
    /// 已过期：后台挂起清理任务将超时的 ConfirmPending 记录自动标记为过期并释放额度。
    /// 防止因 HIS 侧异常导致额度永久占用。
    /// </summary>
    Expired,

    /// <summary>
    /// 已冲正：退费或冲销操作（reverse），按退费数量释放额度，记录逆向折价明细。
    /// 当日退费释放数量，隔日退费重收后按重收当天重新校验额度。
    /// </summary>
    Reversed,

    /// <summary>
    /// 失败：计价过程中发生不可恢复错误（如规则匹配失败、公式执行异常），不占用额度。
    /// </summary>
    Failed
}
