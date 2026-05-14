namespace Pricing.RuleCenter.Core.Aggregates.Quota;

/// <summary>
/// 限额锁实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_LIMIT_LOCK
/// </para>
/// <para>
/// 在并发控制中的角色：该表不保存占用事实，只提供 Oracle SELECT FOR UPDATE 的锁行载体，
/// 用于串行化同一限额维度的 confirm 请求，防止多渠道并发突破限额上限。
/// </para>
/// <para>
/// 使用方式：
/// <list type="bullet">
/// <item>confirm 阶段：先对 PR_LIMIT_LOCK 执行 SELECT ... FOR UPDATE 获取行锁</item>
/// <item>在锁保护下读取 PR_LIMIT_OCCUPY 累计占用并判断是否超限</item>
/// <item>未超限时写入新的占用记录</item>
/// <item>事务提交后自动释放锁</item>
/// </list>
/// </para>
/// <para>
/// 锁键设计：LOCK_KEY 由限额类型、患者、项目、时间桶等维度组成。
/// TIME_WINDOW 类型必须锁定业务时间窗口覆盖的全部小时桶，防止跨桶并发突破。
/// </para>
/// </remarks>
public sealed class LimitLock
{
    /// <summary>
    /// 锁行主键（锁键）。
    /// </summary>
    /// <remarks>
    /// 由限额类型、患者标识、项目编码、时间桶等维度组合生成的唯一键。
    /// 用于 SELECT ... FOR UPDATE 锁行，串行化同一维度的 confirm 请求。
    /// 格式示例：DAY_QTY|PATIENT_001|ITEM_1001|20260510
    /// 该键必须与 PR_LIMIT_OCCUPY.LIMIT_KEY 对应维度一致。
    /// </remarks>
        public string LockKey { get; set; } = string.Empty;

    /// <summary>
    /// 锁说明。
    /// </summary>
    /// <remarks>
    /// 人工可读的锁行来源说明，用于排查锁等待和死锁问题。
    /// 例如："患者张三|项目1001|日限额|2026-05-10"。
    /// </remarks>
        public string? LockDesc { get; set; }

    /// <summary>
    /// 记录最后更新时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在每次更新时自动填充。
    /// 用于排查长时间未释放的锁行（理论上不应出现，因为 FOR UPDATE 在事务提交后自动释放）。
    /// </remarks>
        public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 锁过期时间。
    /// </summary>
    /// <remarks>
    /// 用于后台清理长时间未释放的锁行。
    /// 通常设置为锁创建时间加上保护期（如 30 分钟）。
    /// 后台定时任务通过 CleanupExpiredAsync 清理早于该时间的锁行，
    /// 防止因异常情况（如应用崩溃、事务超时）导致锁行堆积。
    /// </remarks>
        public DateTime? ExpireAt { get; set; }
}
