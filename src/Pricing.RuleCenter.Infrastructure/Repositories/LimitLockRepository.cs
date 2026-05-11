using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 限额锁仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_LIMIT_LOCK 表的锁行创建、释放和过期清理操作。
/// PR_LIMIT_LOCK 是纯锁表，只存储 LOCK_KEY，用于 SELECT FOR UPDATE 的加锁目标。
/// </para>
/// <para>
/// 【并发控制模型】
/// 限额锁是资金安全中的并发控制点。采用 SELECT FOR UPDATE 模式实现悲观锁：
///   1. confirm 阶段先对 PR_LIMIT_LOCK 执行 SELECT ... FOR UPDATE 获取行锁
///   2. 锁定后查询 PR_LIMIT_OCCUPY 的累计占用并判断是否超限
///   3. 未超限时写入新的占用记录
///   4. 事务提交后自动释放锁
/// </para>
/// <para>
/// 【与 ILimitOccupyRepository 的关系】
/// ILimitLockRepository 仅管理锁行（创建、释放、清理）。
/// ILimitOccupyRepository 管理占用记录和锁的获取（EnsureAndLockAsync）。
/// 两者配合实现完整的并发额度控制。
/// </para>
/// <para>
/// 【锁键设计】
/// LOCK_KEY 由限额类型、患者标识、项目编码、时间桶等维度组成。
/// TIME_WINDOW 类型必须锁定业务时间窗口覆盖的全部小时桶。
/// </para>
/// </remarks>
public sealed class LimitLockRepository : ILimitLockRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 限额锁表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化限额锁仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public LimitLockRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取指定锁键的锁（SELECT FOR UPDATE）。
    /// </summary>
    /// <param name="lockKey">锁键，由限额维度组合生成。</param>
    /// <param name="expireAt">锁的过期时间，用于后台清理长时间未释放的锁行。</param>
    /// <returns>是否成功获取锁（true 表示成功，false 表示锁已被占用）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// -- 第一步：按需创建锁行（MERGE = UPSERT）
    /// MERGE INTO PR_LIMIT_LOCK T
    /// USING (SELECT :LockKey LOCK_KEY FROM DUAL) S
    /// ON (T.LOCK_KEY = S.LOCK_KEY)
    /// WHEN NOT MATCHED THEN
    /// INSERT (LOCK_KEY, LOCK_DESC, UPDATED_AT, EXPIRE_AT)
    /// VALUES (S.LOCK_KEY, 'AUTO', SYSDATE, :expireAt)
    ///
    /// -- 第二步：执行行锁
    /// SELECT LOCK_KEY FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :LockKey FOR UPDATE
    /// </code>
    /// 【MERGE 语句说明】
    /// 锁键是动态生成的，不能预先初始化所有可能组合。MERGE 在行不存在时创建，
    /// 已存在时不做变更，避免唯一键冲突。
    /// 【FOR UPDATE 语句说明】
    /// SELECT FOR UPDATE 的目的不是读取数据，而是在当前事务中锁住该行。
    /// 锁住后，执行器再查询 PR_LIMIT_OCCUPY 的累计占用，才能保证同维度并发 confirm 串行通过。
    /// 【事务要求】
    /// 必须在外层事务中调用。如果不在事务中，SELECT FOR UPDATE 的锁会在语句结束后立即释放，
    /// 失去并发控制意义。
    /// </remarks>
    public async Task<bool> AcquireLockAsync(string lockKey, DateTime expireAt)
    {
        try
        {
            // ========== 第一阶段：按需创建锁行 ==========
            // 锁键是动态生成的，不能预先初始化所有可能组合。MERGE 能在行不存在时创建，
            // 已存在时不做变更，避免唯一键冲突。
            await _db.Ado.ExecuteCommandAsync(
                "MERGE INTO PR_LIMIT_LOCK T " +
                "USING (SELECT :LockKey LOCK_KEY FROM DUAL) S " +
                "ON (T.LOCK_KEY = S.LOCK_KEY) " +
                "WHEN NOT MATCHED THEN " +
                "INSERT (LOCK_KEY, LOCK_DESC, UPDATED_AT, EXPIRE_AT) " +
                "VALUES (S.LOCK_KEY, 'AUTO', SYSDATE, :expireAt)",
                new { LockKey = lockKey, ExpireAt = expireAt });

            // ========== 第二阶段：执行 SELECT FOR UPDATE ==========
            // 这里读取字符串不是为了使用返回值，而是为了让 Oracle 在当前事务中锁住该行。
            await _db.Ado.GetStringAsync(
                "SELECT LOCK_KEY FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :LockKey FOR UPDATE",
                new { LockKey = lockKey });

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 释放指定锁键的锁（删除锁行）。
    /// </summary>
    /// <param name="lockKey">锁键。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// DELETE FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :lockKey
    /// </code>
    /// 【使用场景】cancel/reverse 操作时主动释放锁，或业务完成后清理。
    /// 【注意】正常情况下 SELECT FOR UPDATE 的锁会在事务提交后自动释放，
    /// 本方法主要用于主动清理锁行记录，而非释放数据库行锁。
    /// </remarks>
    public async Task ReleaseLockAsync(string lockKey)
    {
        await _db.Deleteable<LimitLock>()
            .Where(l => l.LockKey == lockKey)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 清理过期的锁行记录。
    /// </summary>
    /// <param name="expireBefore">过期阈值时间，早于该时间的锁行将被清理。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// DELETE FROM PR_LIMIT_LOCK WHERE EXPIRE_AT &lt; :expireBefore
    /// </code>
    /// 【使用场景】后台定时任务调用，清理长时间未释放的锁行，
    /// 防止因异常情况（如应用崩溃、事务超时）导致锁行堆积。
    /// 【清理策略】按 EXPIRE_AT 字段判断是否过期，早于阈值时间的锁行将被删除。
    /// </remarks>
    public async Task CleanupExpiredAsync(DateTime expireBefore)
    {
        await _db.Deleteable<LimitLock>()
            .Where(l => l.ExpireAt < expireBefore)
            .ExecuteCommandAsync();
    }
}
