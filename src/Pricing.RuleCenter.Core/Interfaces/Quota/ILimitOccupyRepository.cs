using Pricing.RuleCenter.Core.Aggregates.Quota;

namespace Pricing.RuleCenter.Core.Interfaces.Quota;

/// <summary>
/// 额度占用仓储接口 —— 并发控制与额度管理的核心数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_LIMIT_OCCUPY 和 PR_LIMIT_LOCK。
///
/// 职责边界：
///   - 管理计价请求产生的额度占用记录（数量占用、金额占用）。
///   - 提供并发锁定能力（PR_LIMIT_LOCK + SELECT ... FOR UPDATE），防止多渠道限额突破。
///   - 支持按多种维度查询已占用量：日数量、日金额、时间窗、同组、同手术等。
///
/// 并发控制机制：
///   1. 计价引擎在校验限额前，先调用 EnsureAndLockAsync 锁定相关的限额键。
///   2. 使用 SELECT ... FOR UPDATE 悲观锁，防止并发请求同时读取-修改-写入。
///   3. 时间窗限制必须锁定窗口内全部小时桶，避免跨小时并发穿透。
///   4. 锁定后查询已占用量，判断是否超限，再决定是否新增占用记录。
///
/// 状态流转：
///   OccupyStatus 状态机：Pending → Confirmed / Cancelled / Reversed / Expired。
///   状态变更必须与请求日志（BusinessStatus）同步。
///
/// 金额约束：
///   - 所有金额类型使用 decimal（Oracle NUMBER(18,4)），禁止 double/float。
///   - NULL 表示"不校验"，0 表示"限制为零"。
/// </summary>
public interface ILimitOccupyRepository
{
    /// <summary>
    /// 查询指定限额键在指定状态下的已占用数量。
    ///
    /// 使用场景：日数量限制、单次数量限制、时间窗数量限制的已占用量查询。
    /// 限额键格式由计价引擎根据 LimitType 规则生成。
    /// </summary>
    /// <param name="limitKey">限额键（如 "DAY_QTY|ITEM_001|20260510"）。</param>
    /// <param name="status">占用状态过滤（通常传 "Pending" 或 "Confirmed"，仅统计有效占用）。</param>
    /// <returns>已占用的累计数量（decimal）。</returns>
    Task<decimal> GetOccupiedQtyAsync(string limitKey, string status);

    /// <summary>
    /// 按多维度查询指定时间范围内的已占用数量。
    ///
    /// 使用场景：时间窗限制的已占用量查询，需要按限制类型、维度编码、时间范围聚合。
    /// 例如：查询某项目在最近2小时内的累计数量。
    /// </summary>
    /// <param name="query">时间范围占用查询参数。</param>
    /// <returns>已占用的累计数量（decimal）。</returns>
    Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query);

    /// <summary>
    /// 查询指定限额键在指定状态下的已占用金额。
    ///
    /// 使用场景：日金额限制的已占用金额查询。
    /// </summary>
    /// <param name="limitKey">限额键（如 "DAY_AMT|ITEM_001|20260510"）。</param>
    /// <param name="status">占用状态过滤。</param>
    /// <returns>已占用的累计金额（decimal）。</returns>
    Task<decimal> GetOccupiedAmtAsync(string limitKey, string status);

    /// <summary>
    /// 按维度编码查询指定状态下的已占用金额。
    ///
    /// 使用场景：同手术封顶的已占用金额查询，需要按 patientId:operationId:groupCode 维度聚合。
    /// 与 GetOccupiedAmtAsync(limitKey) 的区别：本方法按 LimitDimensionCode 过滤，支持含 groupCode 的精确维度。
    /// </summary>
    /// <param name="dimensionCode">维度编码（如 "PAT001:OP001:GROUP_A"）。</param>
    /// <param name="status">占用状态过滤。</param>
    /// <returns>已占用的累计金额（decimal）。</returns>
    Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status);

    /// <summary>
    /// 根据请求ID查询该笔计价请求产生的所有额度占用记录。
    ///
    /// 使用场景：cancel/reverse/expire 操作时，查找需要释放的占用记录。
    /// </summary>
    /// <param name="requestId">计价请求日志ID。</param>
    /// <returns>占用记录列表。</returns>
    Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId);

    /// <summary>
    /// 确保指定的限额锁键存在并加行级锁（SELECT ... FOR UPDATE）。
    ///
    /// 并发控制的核心方法：
    /// 1. 如果 PR_LIMIT_LOCK 表中不存在该锁键，先插入（幂等）。
    /// 2. 对锁键行加 SELECT ... FOR UPDATE 悲观锁。
    /// 3. 锁定后才能安全地查询已占用量并新增占用记录。
    ///
    /// 时间窗限制场景：必须传入窗口覆盖的全部小时桶对应的锁键，避免跨小时并发穿透。
    /// </summary>
    /// <param name="lockKeys">
    /// 锁键集合。锁键格式示例：
    /// - 日限制："DAY_QTY|ITEM_001|20260510"
    /// - 时间窗："TIME_WINDOW|ITEM_001|2026051014"（小时桶）
    /// </param>
    Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys);

    /// <summary>
    /// 插入一条额度占用记录，返回自增主键。
    ///
    /// 调用时机：confirm 接口校验通过后，创建占用记录锁定额度。
    /// </summary>
    /// <param name="entity">占用记录实体，包含限额键、限制类型、占用数量/金额、状态等。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(LimitOccupy entity);

    /// <summary>
    /// 更新单条占用记录的状态。
    ///
    /// 使用场景：单条占用记录的状态流转（如从 Pending 更新为 Confirmed）。
    /// </summary>
    /// <param name="occupyId">占用记录主键ID。</param>
    /// <param name="status">目标状态（OccupyStatus 的字符串表示）。</param>
    Task UpdateStatusAsync(long occupyId, string status);

    /// <summary>
    /// 批量更新指定请求下所有占用记录的状态。
    ///
    /// 使用场景：commit/cancel/reverse/expire 操作时，同步更新该请求下所有占用记录的状态。
    /// 保证请求日志、额度占用、折价明细三者状态一致。
    /// </summary>
    /// <param name="requestId">计价请求日志ID。</param>
    /// <param name="status">目标状态（OccupyStatus 的字符串表示）。</param>
    Task UpdateStatusByRequestIdAsync(long requestId, string status);
}
