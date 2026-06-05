namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// 额度占用记录的状态枚举，描述 PR_LIMIT_OCCUPY 表中单条占用记录的生命周期。
///
/// 状态机流转路径：
///   Pending（待确认）—— confirm 调用后创建，额度已锁定
///   Confirmed（已确认）—— HIS commit 后确认，额度正式扣减
///   Cancelled（已取消）—— cancel 调用后释放额度
///   Reversed（已冲正）—— reverse（退费/冲销）后释放额度，记录逆向明细
///   Expired（已过期）—— 后台挂起清理任务自动过期，释放额度
///
/// 与 BusinessStatus 的关系：
/// - BusinessStatus 描述整笔计价请求的业务状态。
/// - OccupyStatus 描述该请求产生的额度占用记录的状态。
/// - confirm/commit/cancel/reverse/expire 操作必须同时更新两者，保证数据一致性。
///
/// 并发控制：PR_LIMIT_LOCK 表 + SELECT ... FOR UPDATE 防止多渠道限额突破。
/// </summary>
public enum OccupyStatus
{
    /// <summary>
    /// 待确认：confirm 调用后创建的占用记录，额度已锁定但尚未最终确认。
    /// 此状态下额度被占用，其他渠道的计价请求会看到已占用量。
    /// </summary>
    Pending,

    /// <summary>
    /// 已确认：HIS commit 后确认的占用记录，额度正式扣减。
    /// 此状态为终态之一，表示计价流程正常完结。
    /// </summary>
    Confirmed,

    /// <summary>
    /// 已取消：cancel 调用后释放的占用记录，额度恢复。
    /// 仅 Pending 或 Confirmed 状态可转为 Cancelled。
    /// </summary>
    Cancelled,

    /// <summary>
    /// 已冲正：reverse（退费/冲销）后释放的占用记录，额度按退费数量恢复。
    /// 当日退费释放数量，隔日退费重收后按重收当天重新校验。
    /// </summary>
    Reversed,

    /// <summary>
    /// 已过期：后台挂起清理任务将超时的 Pending 记录自动标记为过期并释放额度。
    /// 防止因 HIS 侧异常（如网络超时、进程崩溃）导致额度永久占用。
    /// </summary>
    Expired
}
