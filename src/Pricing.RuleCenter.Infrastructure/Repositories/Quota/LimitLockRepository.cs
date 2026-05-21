using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Quota;

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
    private readonly ISqlSugarClient _db;

    public LimitLockRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取指定锁键的锁（SELECT FOR UPDATE）。
    /// </summary>
    /// <remarks>
    /// 成功时无返回值（Task）；失败时抛出 InvalidOperationException。
    /// 不存在"获取失败返回 false"的场景——数据库异常必须向上抛出，不能静默放行。
    /// </remarks>
    public async Task AcquireLockAsync(string lockKey, DateTime expireAt)
    {
        try
        {
            // 第一阶段：按需创建锁行（MERGE = UPSERT）
            await _db.Ado.ExecuteCommandAsync(
                "MERGE INTO PR_LIMIT_LOCK T " +
                "USING (SELECT :LockKey LOCK_KEY FROM DUAL) S " +
                "ON (T.LOCK_KEY = S.LOCK_KEY) " +
                "WHEN NOT MATCHED THEN " +
                "INSERT (LOCK_KEY, LOCK_DESC, UPDATED_AT, EXPIRE_AT) " +
                "VALUES (S.LOCK_KEY, 'AUTO', SYSDATE, :ExpireAt)",
                new { LockKey = lockKey, ExpireAt = expireAt });

            // 第二阶段：执行 SELECT FOR UPDATE
            await _db.Ado.GetStringAsync(
                "SELECT LOCK_KEY FROM PR_LIMIT_LOCK WHERE LOCK_KEY = :LockKey FOR UPDATE",
                new { LockKey = lockKey });
        }
        catch (Exception ex)
        {
            // 限额锁是资金安全的关键并发控制点，不能静默吞没数据库异常。
            throw new InvalidOperationException(
                $"获取限额锁失败，LockKey={lockKey}。限额锁是资金安全关键控制点，" +
                "数据库异常不能静默放行，否则将导致限额被突破。", ex);
        }
    }

    /// <summary>
    /// 释放指定锁键的锁（删除锁行）。
    /// </summary>
    public async Task ReleaseLockAsync(string lockKey)
    {
        if (string.IsNullOrEmpty(lockKey))
        {
            throw new ArgumentNullException(nameof(lockKey), "锁键不能为空");
        }

        await _db.Deleteable<LimitLock>()
            .Where(l => l.LockKey == lockKey)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 清理过期的锁行记录。
    /// </summary>
    public async Task CleanupExpiredAsync(DateTime expireBefore)
    {
        await _db.Deleteable<LimitLock>()
            .Where(l => l.ExpireAt < expireBefore)
            .ExecuteCommandAsync();
    }
}