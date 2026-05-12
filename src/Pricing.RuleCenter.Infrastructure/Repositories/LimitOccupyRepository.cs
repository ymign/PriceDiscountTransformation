using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

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
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于执行限额占用查询、写入和 Oracle 行锁语句。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化限额占用仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public LimitOccupyRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按单个限额键和状态查询占用数量合计。
    /// </summary>
    /// <param name="limitKey">限额键，由业务维度（如患者+项目+日期）组合生成。</param>
    /// <param name="status">要累计的状态，如 PENDING 或 CONFIRMED。</param>
    /// <returns>指定键和状态下的占用数量合计（decimal）；无记录时返回 0。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT NVL(SUM(OCCUPY_QTY), 0) FROM PR_LIMIT_OCCUPY
    /// WHERE LIMIT_KEY = :limitKey AND STATUS = :status
    /// </code>
    /// 【使用场景】单限额键的快速累计查询，如按项目+患者+日期的单日数量限制。
    /// 【NULL ≠ 0】Oracle 的 SUM 在无匹配行时返回 NULL，SqlSugar 会将其转为 0。
    /// </remarks>
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
    /// <param name="query">限额类型、维度编码、业务时间范围和状态集合。</param>
    /// <returns>窗口内的占用数量合计（decimal）；退费负数记录会自然抵扣。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT NVL(SUM(OCCUPY_QTY), 0) FROM PR_LIMIT_OCCUPY
    /// WHERE LIMIT_TYPE = :limitType
    ///   AND LIMIT_DIMENSION_CODE = :limitDimensionCode
    ///   AND STATUS IN (:statuses)
    ///   AND BUSINESS_CHARGE_TIME BETWEEN :startTime AND :endTime
    /// </code>
    /// 【业务时间字段】这里故意不使用 OCCUPIED_AT（技术占用时间），
    /// 而是使用 BUSINESS_CHARGE_TIME（HIS 业务发生时间）。
    /// 补缴费、补录和延迟提交都应该按 HIS 业务发生时间参与窗口累计。
    /// 【退费抵扣】退费记录的 OCCUPY_QTY 为负数，SUM 时自然抵扣正数占用。
    /// </remarks>
    public async Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query)
    {
        // ========== 第一阶段：固定状态集合 ==========
        // 调用方通常传 PENDING + CONFIRMED。PENDING 必须计入，否则两个渠道并发 confirm 时
        // 都会忽略对方尚未 commit 的保护占用，从而突破上限。
        var statusArray = query.Statuses.ToArray();

        // ========== 第二阶段：按 BUSINESS_CHARGE_TIME 查询 ==========
        // 这里故意不使用 OCCUPIED_AT。补缴费、补录和延迟提交都应该按 HIS 业务发生时间参与窗口累计。
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
    /// <param name="limitKey">限额键，由业务维度组合生成。</param>
    /// <param name="status">要累计的状态，如 PENDING 或 CONFIRMED。</param>
    /// <returns>指定键和状态下的占用金额合计（decimal, NUMBER(18,4)）；无记录时返回 0。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT NVL(SUM(OCCUPY_AMT), 0) FROM PR_LIMIT_OCCUPY
    /// WHERE LIMIT_KEY = :limitKey AND STATUS = :status
    /// </code>
    /// 【使用场景】金额类限额的累计查询。
    /// 【金额类型】返回值为 decimal（C#）对应 NUMBER(18,4)（Oracle），禁止使用 double/float。
    /// </remarks>
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
    /// <param name="requestId">请求日志主键（PR_CHARGE_REQUEST_LOG.REQUEST_ID）。</param>
    /// <returns>该次请求产生的限额占用记录集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_LIMIT_OCCUPY WHERE REQUEST_ID = :requestId
    /// </code>
    /// 【使用场景】
    ///   - cancel/reverse 时查找需要释放的占用记录
    ///   - 追溯页面展示限额占用明细
    /// </remarks>
    public async Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<LimitOccupy>()
            .Where(o => o.RequestId == requestId)
            .ToListAsync();
    }

    /// <summary>
    /// 确保限额锁行存在，并按固定顺序执行数据库行锁。
    /// </summary>
    /// <param name="lockKeys">
    /// 需要锁定的全部限额锁键。时间窗口（TIME_WINDOW）类型可能覆盖多个小时桶，
    /// 每个桶对应一个独立的锁键。
    /// </param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 【SQL 语义】对每个锁键执行两步操作：
    /// <code>
    /// -- 第一步：按需创建锁行（MERGE = UPSERT）
    /// MERGE INTO PR_LIMIT_LOCK T
    /// USING (SELECT :LockKey LOCK_KEY FROM DUAL) S
    /// ON (T.LOCK_KEY = S.LOCK_KEY)
    /// WHEN NOT MATCHED THEN
    /// INSERT (LOCK_KEY, LOCK_DESC, UPDATED_AT) VALUES (S.LOCK_KEY, 'AUTO', SYSDATE)
    ///
    /// -- 第二步：执行行锁
    /// SELECT LOCK_KEY FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :LockKey FOR UPDATE
    /// </code>
    /// 【MERGE 语句说明】
    /// TIME_WINDOW 锁键是动态小时桶（如"患者A+项目B+2026050714"表示14点桶），
    /// 不能预先初始化所有可能组合。MERGE 在行不存在时创建，已存在时不做变更，
    /// 避免唯一键冲突。
    /// 【FOR UPDATE 语句说明】
    /// SELECT FOR UPDATE 的目的不是读取数据，而是在当前事务中锁住该行。
    /// 锁住后，执行器再查询 PR_LIMIT_OCCUPY 的累计占用，才能保证同维度并发 confirm 串行通过。
    /// 【加锁顺序】
    /// 多个锁键按字典序（StringComparer.Ordinal）排序后逐个锁定，
    /// 保证不同请求在锁多个小时桶时保持同一加锁顺序，降低死锁概率。
    /// 【事务要求】
    /// 必须在外层事务中调用。如果不在事务中，SELECT FOR UPDATE 的锁会在语句结束后立即释放，
    /// 失去并发控制意义。通常由应用服务在 BeginTransaction / CommitTransaction 之间调用。
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
    /// <returns>Oracle 序列生成的限额占用主键（OCCUPY_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_LIMIT_OCCUPY.NEXTVAL FROM DUAL
    /// INSERT INTO PR_LIMIT_OCCUPY (...) VALUES (...)
    /// </code>
    /// 【独立主键】占用记录必须有独立主键，便于后续 commit/cancel/reverse 按请求或按明细定位。
    /// 【事务边界】必须在 EnsureAndLockAsync 的行锁保护下执行，
    /// 保证查询累计占用和写入新占用之间不被其他并发请求插入。
    /// 【退费场景】退费时插入 OCCUPY_QTY 为负数的记录，实现额度释放。
    /// </remarks>
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
    /// <param name="occupyId">限额占用主键（PR_LIMIT_OCCUPY.OCCUPY_ID）。</param>
    /// <param name="status">目标占用状态（PENDING / CONFIRMED / CANCELLED / EXPIRED / REVERSED）。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_LIMIT_OCCUPY SET STATUS = :status WHERE OCCUPY_ID = :occupyId
    /// </code>
    /// 【使用场景】针对单条占用记录的状态更新，如退费时逐条释放特定占用。
    /// 【约束】状态合法性由应用服务保证，仓储只执行指定更新。
    /// </remarks>
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
    /// <param name="requestId">请求日志主键（PR_CHARGE_REQUEST_LOG.REQUEST_ID）。</param>
    /// <param name="status">目标占用状态。</param>
    /// <remarks>
    /// 【SQL 语义】根据 status 值不同，等价于：
    /// <code>
    /// -- 非 CONFIRMED 状态
    /// UPDATE PR_LIMIT_OCCUPY SET STATUS = :status WHERE REQUEST_ID = :requestId
    ///
    /// -- CONFIRMED 状态（补记确认时间）
    /// UPDATE PR_LIMIT_OCCUPY
    /// SET STATUS = 'CONFIRMED', CONFIRMED_AT = SYSDATE
    /// WHERE REQUEST_ID = :requestId
    /// </code>
    /// 【状态一致性】limit occupy 的状态必须跟请求日志、折价明细同步推进。
    /// 应用服务必须在调用本方法的同时更新请求日志状态和折价明细状态，
    /// 保证三张表在同一事务中同步推进。
    /// 【确认时间补记】CONFIRMED 表示 HIS 已经落账成功，此时补记 ConfirmedAt 便于后续对账和延迟分析。
    /// </remarks>
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
