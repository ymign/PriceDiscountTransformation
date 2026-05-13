namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 限额锁仓储接口 —— 并发控制的数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_LIMIT_LOCK。
///
/// 职责边界：
///   - 管理限额锁行的创建、释放和过期清理。
///   - PR_LIMIT_LOCK 是纯锁表，只存储 LOCK_KEY，用于 SELECT FOR UPDATE 的加锁目标。
///   - 与 PR_LIMIT_OCCUPY 分离设计，避免占用表的行数膨胀影响锁性能。
///
/// 并发控制机制：
///   1. confirm 阶段：先对 PR_LIMIT_LOCK 执行 SELECT ... FOR UPDATE 获取行锁
///   2. 在锁保护下读取 PR_LIMIT_OCCUPY 累计占用并判断是否超限
///   3. 未超限时写入新的占用记录
///   4. 事务提交后自动释放锁
///
/// 锁键设计：
///   LOCK_KEY 由限额类型、患者标识、项目编码、时间桶等维度组成。
///   TIME_WINDOW 类型必须锁定业务时间窗口覆盖的全部小时桶。
///
/// 与 ILimitOccupyRepository 的关系：
///   - ILimitLockRepository 仅管理锁行（创建、释放、清理）。
///   - ILimitOccupyRepository 管理占用记录和锁的获取（EnsureAndLockAsync）。
///   - 两者配合实现完整的并发额度控制。
/// </summary>
public interface ILimitLockRepository
{
    /// <summary>
    /// 获取指定锁键的锁（SELECT FOR UPDATE）。
    ///
    /// 使用场景：在 confirm 阶段，对指定限额维度加行锁，串行化并发请求。
    /// 必须在事务中调用，否则锁会在语句结束后立即释放。
    /// </summary>
    /// <param name="lockKey">
    /// 锁键，由限额维度组合生成。
    /// 格式示例：DAY_QTY|PATIENT_001|ITEM_1001|20260510
    /// </param>
    /// <param name="expireAt">
    /// 锁的过期时间，用于后台清理长时间未释放的锁行。
    /// 通常设置为当前时间加上保护期（如 30 分钟）。
    /// </param>
    /// <returns>成功获取锁时返回 true。</returns>
    /// <exception cref="InvalidOperationException">
    /// 数据库异常时抛出，包含锁键信息。限额锁是资金安全关键控制点，
    /// 数据库异常不能静默放行（返回 false 会让调用方误认为锁被占用而跳过限额校验），
    /// 必须向上抛出由调用方决定重试或失败。
    /// </exception>
    Task<bool> AcquireLockAsync(string lockKey, DateTime expireAt);

    /// <summary>
    /// 释放指定锁键的锁（删除锁行）。
    ///
    /// 使用场景：cancel/reverse 操作时主动释放锁，或业务完成后清理。
    /// 注意：正常情况下 SELECT FOR UPDATE 的锁会在事务提交后自动释放，
    /// 本方法主要用于主动清理锁行记录，而非释放数据库行锁。
    /// </summary>
    /// <param name="lockKey">锁键。</param>
    Task ReleaseLockAsync(string lockKey);

    /// <summary>
    /// 清理过期的锁行记录。
    ///
    /// 使用场景：后台定时任务调用，清理长时间未释放的锁行，
    /// 防止因异常情况导致锁行堆积。
    /// </summary>
    /// <param name="expireBefore">过期阈值时间，早于该时间的锁行将被清理。</param>
    Task CleanupExpiredAsync(DateTime expireBefore);
}
