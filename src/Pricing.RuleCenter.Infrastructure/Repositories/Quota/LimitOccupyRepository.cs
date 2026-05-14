using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Quota;

/// <summary>
/// 限额占用仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_LIMIT_OCCUPY 与 PR_LIMIT_LOCK 两张表的全部数据访问操作，包括：
/// 占用数量/金额查询、占用明细查询、锁行创建与锁定、占用记录插入、状态更新。
/// </para>
/// <para>
/// 【并发控制模型】
/// 限额占用是资金安全中的并发控制点。采用 SELECT FOR UPDATE 模式实现悲观锁：
///   1. 规则执行器在 confirm 阶段先调用 EnsureAndLockAsync 锁定 PR_LIMIT_LOCK
///   2. 锁定后查询 PR_LIMIT_OCCUPY 的 PENDING/CONFIRMED 净占用
///   3. 校验未超限后写入新的 PENDING 占用
///   4. 事务提交后锁自动释放
/// </para>
/// <para>
/// 【两张表的分工】
///   - PR_LIMIT_LOCK：纯锁表，只存储 LOCK_KEY，用于 SELECT FOR UPDATE 的加锁目标
///   - PR_LIMIT_OCCUPY：占用明细表，存储每次计价的限额占用记录
/// 分离设计避免占用表的行数膨胀影响锁性能。
/// </para>
/// <para>
/// 【锁键排序策略】
/// 多个锁键按字典序（StringComparer.Ordinal）排序后逐个锁定，
/// 保证不同请求在锁多个小时桶时保持同一加锁顺序，降低死锁概率。
/// </para>
/// <para>
/// 【状态枚举】
///   - PENDING   — 已占用但未确认（confirm 后、commit 前）
///   - CONFIRMED — 已确认（HIS 落账成功）
///   - CANCELLED — 已取消（cancel 后释放）
///   - EXPIRED   — 已过期（保护期超时自动释放）
///   - REVERSED  — 已冲销（退费后释放）
/// </para>
/// <para>
/// 【事务要求】
/// EnsureAndLockAsync 必须在外层事务中调用，否则 SELECT FOR UPDATE 的锁
/// 会在语句结束后立即释放，失去并发控制意义。
/// </para>
/// </remarks>
public sealed class LimitOccupyRepository : ILimitOccupyRepository
{
    private readonly ISqlSugarClient _db;

    public LimitOccupyRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按单个限额键和状态查询占用数量合计。
    /// </summary>
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
    public async Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query)
    {
        var statusArray = query.Statuses.ToArray();

        var result = await _db.Queryable<LimitOccupy>()
            .Where(o =>
                o.LimitType == query.LimitType &&
                o.LimitDimensionCode == query.LimitDimensionCode &&
                statusArray.Contains(o.Status) &&
                o.BusinessChargeTime >= query.StartTime &&
                o.BusinessChargeTime <= query.EndTime)
            .SumAsync(o => o.OccupyQty);
        return result;
    }

    /// <summary>
    /// 按单个限额键和状态查询占用金额合计。
    /// </summary>
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
    public async Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<LimitOccupy>()
            .Where(o => o.RequestId == requestId)
            .ToListAsync();
    }

    /// <summary>
    /// 确保限额锁行存在，并按固定顺序执行数据库行锁。
    /// </summary>
    public async Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys)
    {
        foreach (var lockKey in lockKeys.OrderBy(k => k, StringComparer.Ordinal))
        {
            await _db.Ado.ExecuteCommandAsync(
                "MERGE INTO PR_LIMIT_LOCK T " +
                "USING (SELECT :LockKey LOCK_KEY FROM DUAL) S " +
                "ON (T.LOCK_KEY = S.LOCK_KEY) " +
                "WHEN NOT MATCHED THEN " +
                "INSERT (LOCK_KEY, LOCK_DESC, UPDATED_AT) VALUES (S.LOCK_KEY, 'AUTO', SYSDATE)",
                new { LockKey = lockKey });

            await _db.Ado.GetStringAsync(
                "SELECT LOCK_KEY FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :LockKey FOR UPDATE",
                new { LockKey = lockKey });
        }
    }

    /// <summary>
    /// 插入限额占用记录。
    /// </summary>
    public async Task<long> InsertAsync(LimitOccupy entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_LIMIT_OCCUPY.NEXTVAL FROM DUAL");
        entity.OccupyId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 按占用主键更新单条限额占用状态。
    /// </summary>
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
    public async Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        var update = _db.Updateable<LimitOccupy>()
            .SetColumns(o => o.Status == status);

        if (status == "CONFIRMED")
        {
            var now = DateTime.Now;
            update = update.SetColumns(o => o.ConfirmedAt == now);
        }

        await update.Where(o => o.RequestId == requestId).ExecuteCommandAsync();
    }
}