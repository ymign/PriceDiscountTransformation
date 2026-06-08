namespace Pricing.RuleCenter.Core.Exceptions;

/// <summary>
/// 限额锁获取异常，表示悲观锁获取过程中发生了并发冲突或数据库故障。
/// </summary>
/// <remarks>
/// 该异常把“状态冲突”和“基础设施锁故障”从普通 InvalidOperationException 中分离出来，
/// 便于全局异常过滤器稳定映射为额度并发相关的业务错误码。
/// </remarks>
public sealed class LimitLockException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LimitLockException"/> class.
    /// </summary>
    /// <param name="lockKey">The lock key that failed to acquire or validate.</param>
    /// <param name="isConcurrencyConflict">A value indicating whether the failure is caused by a concurrency conflict.</param>
    /// <param name="message">The message that describes the failure.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public LimitLockException(
        string lockKey,
        bool isConcurrencyConflict,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        LockKey = lockKey;
        IsConcurrencyConflict = isConcurrencyConflict;
    }

    /// <summary>
    /// 发生异常的锁键。
    /// </summary>
    public string LockKey { get; }

    /// <summary>
    /// 是否属于锁竞争/死锁/等待超时等并发冲突。
    /// </summary>
    public bool IsConcurrencyConflict { get; }
}
