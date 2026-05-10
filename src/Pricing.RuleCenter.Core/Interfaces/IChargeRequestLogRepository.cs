using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 计价请求日志仓储接口 —— 折价追溯中心的核心数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_CHARGE_REQUEST_LOG。
///
/// 职责边界：
///   - 记录每一笔计价请求的完整生命周期（从 confirm 到 commit/cancel/reverse/expire）。
///   - 提供幂等性校验能力（通过 businessKey 和 fingerprint 查询）。
///   - 支持追溯查询接口的分页查询。
///   - 支持后台挂起清理任务扫描超时记录。
///
/// 幂等性约束：
///   幂等键 = sourceSystem + businessRequestNo + callType。
///   confirm 接口必须幂等：重复调用（相同幂等键）返回相同结果，不重复占用额度。
///   REQUEST_FINGERPRINT 字段保存请求指纹，用于超时重试场景的精确匹配。
///
/// 状态同步约束：
///   本表的 STATUS 字段使用 BusinessStatus 枚举。
///   状态变更时必须同时更新关联的额度占用（OccupyStatus）和折价明细状态。
/// </summary>
public interface IChargeRequestLogRepository
{
    /// <summary>
    /// 根据主键ID查询请求日志。
    ///
    /// 使用场景：内部关联查询，如根据请求ID查询关联的折价明细和追溯步骤。
    /// </summary>
    /// <param name="requestId">请求日志主键ID（PR_CHARGE_REQUEST_LOG.ID）。</param>
    /// <returns>请求日志实体，不存在时返回 null。</returns>
    Task<ChargeRequestLog?> GetByIdAsync(long requestId);

    /// <summary>
    /// 根据业务键查询请求日志，用于幂等性校验。
    ///
    /// 幂等键 = sourceSystem + businessRequestNo + callType。
    /// confirm 接口在执行前先查询：如果已存在相同业务键的记录，直接返回已有结果，
    /// 不重复执行计价计算和额度占用。
    /// </summary>
    /// <param name="sourceSystem">来源系统标识（如"HIS"、"SELF_SERVICE"、"WECHAT"）。</param>
    /// <param name="businessRequestNo">业务请求号（HIS 侧的唯一单据号）。</param>
    /// <param name="callType">调用类型（Simulate/Confirm/Commit/Cancel/Reverse 的字符串表示）。</param>
    /// <returns>匹配的请求日志实体，不存在时返回 null。</returns>
    Task<ChargeRequestLog?> GetByBusinessKeyAsync(
        string sourceSystem, string businessRequestNo, string callType);

    /// <summary>
    /// 根据请求指纹查询请求日志，用于超时重试场景的精确匹配。
    ///
    /// 请求指纹（REQUEST_FINGERPRINT）是对请求参数的哈希摘要，用于在业务键不确定时
    /// （如 HIS 侧 businessRequestNo 不稳定）精确匹配请求。
    /// </summary>
    /// <param name="fingerprint">请求指纹字符串。</param>
    /// <returns>匹配的请求日志实体，不存在时返回 null。</returns>
    Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint);

    /// <summary>
    /// 插入一条请求日志记录，返回自增主键。
    ///
    /// 调用时机：confirm 接口首次调用时（幂等校验通过后）创建请求日志。
    /// Oracle 无自增，主键通过 SEQUENCE 生成。
    /// </summary>
    /// <param name="entity">请求日志实体，包含来源系统、业务请求号、调用类型、状态、患者信息、项目信息等。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(ChargeRequestLog entity);

    /// <summary>
    /// 更新请求日志实体（全量更新）。
    ///
    /// 调用时机：状态变更（confirm→commit/cancel/reverse/expire）时更新日志记录。
    /// 更新时必须保持请求日志、额度占用、折价明细三者状态一致。
    /// </summary>
    /// <param name="entity">要更新的请求日志实体，ID 必须存在。</param>
    Task UpdateAsync(ChargeRequestLog entity);

    /// <summary>
    /// 分页查询请求日志，支持按患者、项目、收费单号、时间范围筛选。
    ///
    /// 使用场景：追溯查询接口（/api/pricing/trace/query）的分页列表展示。
    /// </summary>
    /// <param name="patientId">患者ID（可选筛选条件）。</param>
    /// <param name="itemCode">项目编码（可选筛选条件）。</param>
    /// <param name="chargeNo">收费单号（可选筛选条件）。</param>
    /// <param name="startTime">开始时间（可选筛选条件，按业务时间过滤）。</param>
    /// <param name="endTime">结束时间（可选筛选条件，按业务时间过滤）。</param>
    /// <param name="pageIndex">页码（从0开始）。</param>
    /// <param name="pageSize">每页记录数。</param>
    /// <returns>分页结果，包含当前页数据列表和总记录数。</returns>
    Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
        string? patientId, string? itemCode, string? chargeNo,
        DateTime? startTime, DateTime? endTime,
        int pageIndex, int pageSize);

    /// <summary>
    /// 查询所有待处理且已超时的请求日志，用于后台挂起清理任务。
    ///
    /// 挂起清理机制：扫描长时间处于 ConfirmPending 状态的记录，
    /// 自动标记为 Expired 并释放额度占用，防止因 HIS 侧异常导致额度永久占用。
    /// </summary>
    /// <param name="expireBefore">过期阈值时间，查询状态为 ConfirmPending 且创建时间早于此时间的记录。</param>
    /// <returns>需要过期处理的请求日志列表。</returns>
    Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore);
}
