using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 限额占用仓储实现，封装 PR_LIMIT_OCCUPY 与 PR_LIMIT_LOCK 的累计、写入和行锁操作。
/// </summary>
/// <remarks>
/// 限额占用是资金安全里的并发控制点。规则执行器会在 confirm 阶段先锁定
/// PR_LIMIT_LOCK，再查询 PR_LIMIT_OCCUPY 的 PENDING/CONFIRMED 净占用，最后写入新的
/// PENDING 占用。这个仓储只负责数据库读写和锁语义，不负责解释具体业务限额。
/// </remarks>
public sealed class LimitOccupyRepository : ILimitOccupyRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于执行限额占用查询、写入和 Oracle 行锁语句。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化限额占用仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public LimitOccupyRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按单个限额键和状态查询占用数量。
    /// </summary>
    /// <param name="limitKey">限额键。</param>
    /// <param name="status">要累计的状态。</param>
    /// <returns>指定键和状态下的占用数量合计。</returns>
    public async Task<decimal> GetOccupiedQtyAsync(string limitKey, string status)
    {
        var result = await _db.Queryable<LimitOccupy>()
            .Where(o => o.LimitKey == limitKey && o.Status == status)
            .SumAsync(o => o.OccupyQty);
        return result;
    }

    /// <summary>
    /// 按限额类型、业务维度和业务时间窗口查询净占用数量。
    /// </summary>
    /// <param name="limitType">限额类型，例如 TIME_WINDOW 或 DAY_QTY。</param>
    /// <param name="limitDimensionCode">稳定查询维度，例如患者加项目。</param>
    /// <param name="startTime">业务时间窗口开始时间。</param>
    /// <param name="endTime">业务时间窗口结束时间。</param>
    /// <param name="statuses">需要计入累计的状态集合。</param>
    /// <returns>窗口内的占用数量合计，包含退费负数记录时自然抵扣。</returns>
    public async Task<decimal> GetOccupiedQtyAsync(
        string limitType,
        string limitDimensionCode,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<string> statuses)
    {
        // ========== 第一阶段：固定状态集合 ==========
        // 调用方通常传 PENDING + CONFIRMED。PENDING 必须计入，否则两个渠道并发 confirm 时
        // 都会忽略对方尚未 commit 的保护占用，从而突破上限。
        var statusArray = statuses.ToArray();

        // ========== 第二阶段：按 BUSINESS_CHARGE_TIME 查询 ==========
        // 这里故意不使用 OCCUPIED_AT。补缴费、补录和延迟提交都应该按 HIS 业务发生时间参与窗口累计。
        var result = await _db.Queryable<LimitOccupy>()
            .Where(o =>
                o.LimitType == limitType &&
                o.LimitDimensionCode == limitDimensionCode &&
                statusArray.Contains(o.Status) &&
                o.BusinessChargeTime >= startTime &&
                o.BusinessChargeTime <= endTime)
            .SumAsync(o => o.OccupyQty);
        return result;
    }

    /// <summary>
    /// 按单个限额键和状态查询占用金额。
    /// </summary>
    /// <param name="limitKey">限额键。</param>
    /// <param name="status">要累计的状态。</param>
    /// <returns>指定键和状态下的占用金额合计。</returns>
    public async Task<decimal> GetOccupiedAmtAsync(string limitKey, string status)
    {
        var result = await _db.Queryable<LimitOccupy>()
            .Where(o => o.LimitKey == limitKey && o.Status == status)
            .SumAsync(o => o.OccupyAmt);
        return result;
    }

    /// <summary>
    /// 按请求日志主键读取限额占用明细。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <returns>该次请求产生的限额占用记录集合。</returns>
    public async Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<LimitOccupy>()
            .Where(o => o.RequestId == requestId)
            .ToListAsync();
    }

    /// <summary>
    /// 确保限额锁行存在，并按固定顺序执行数据库行锁。
    /// </summary>
    /// <param name="lockKeys">需要锁定的全部限额锁键。时间窗口可能覆盖多个小时桶。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 这个方法必须在外层事务中调用，否则 SELECT FOR UPDATE 的锁会在语句结束后失去意义。
    /// 多个锁键按字典序排序，是为了不同请求在锁多个小时桶时保持同一加锁顺序，降低死锁概率。
    /// </remarks>
    public async Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys)
    {
        foreach (var lockKey in lockKeys.OrderBy(k => k, StringComparer.Ordinal))
        {
            // ========== 第一阶段：按需创建锁行 ==========
            // TIME_WINDOW 锁键是动态小时桶，不能预先初始化所有可能组合。MERGE 能在行不存在时创建，
            // 已存在时不做变更，避免唯一键冲突。
            await _db.Ado.ExecuteCommandAsync(
                "MERGE INTO PR_LIMIT_LOCK T " +
                "USING (SELECT :LockKey LOCK_KEY FROM DUAL) S " +
                "ON (T.LOCK_KEY = S.LOCK_KEY) " +
                "WHEN NOT MATCHED THEN " +
                "INSERT (LOCK_KEY, LOCK_DESC, UPDATED_AT) VALUES (S.LOCK_KEY, 'AUTO', SYSDATE)",
                new { LockKey = lockKey });

            // ========== 第二阶段：执行 SELECT FOR UPDATE ==========
            // 这里读取字符串不是为了使用返回值，而是为了让 Oracle 在当前事务中锁住该行。
            // 锁住后，执行器再查询 PR_LIMIT_OCCUPY，才能保证同维度并发 confirm 串行通过。
            await _db.Ado.GetStringAsync(
                "SELECT LOCK_KEY FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :LockKey FOR UPDATE",
                new { LockKey = lockKey });
        }
    }

    /// <summary>
    /// 插入限额占用记录。
    /// </summary>
    /// <param name="entity">待写入的限额占用实体。</param>
    /// <returns>Oracle 序列生成的限额占用主键。</returns>
    public async Task<long> InsertAsync(LimitOccupy entity)
    {
        // 占用记录必须有独立主键，便于后续 commit/cancel/reverse 按请求或按明细定位。
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_LIMIT_OCCUPY.NEXTVAL FROM DUAL");
        entity.OccupyId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 按占用主键更新单条限额占用状态。
    /// </summary>
    /// <param name="occupyId">限额占用主键。</param>
    /// <param name="status">目标占用状态。</param>
    public async Task UpdateStatusAsync(long occupyId, string status)
    {
        await _db.Updateable<LimitOccupy>()
            .SetColumns(o => o.Status == status)
            .Where(o => o.OccupyId == occupyId)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 按请求日志主键批量更新限额占用状态。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <param name="status">目标占用状态。</param>
    public async Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        // ========== 第一阶段：构造状态更新 ==========
        // limit occupy 的状态必须跟请求日志、折价明细同步推进。这里不做状态合法性判断，
        // 合法流转由应用服务统一保证，仓储只执行指定更新。
        var update = _db.Updateable<LimitOccupy>()
            .SetColumns(o => o.Status == status);

        // ========== 第二阶段：确认时间补记 ==========
        // CONFIRMED 表示 HIS 已经落账成功，此时补记 ConfirmedAt 便于后续对账和延迟分析。
        if (status == "CONFIRMED")
        {
            var now = DateTime.Now;
            update = update.SetColumns(o => o.ConfirmedAt == now);
        }

        await update.Where(o => o.RequestId == requestId).ExecuteCommandAsync();
    }
}
