using Pricing.RuleCenter.Core.Interfaces;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Database;

/// <summary>
/// 基于 SqlSugar 的工作单元实现，封装 Oracle 事务管理。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 应用服务层通过 IUnitOfWork 接口管理事务边界，不再直接依赖 ISqlSugarClient。
/// 该实现内部持有 ISqlSugarClient，将 Begin/Commit/Rollback 委托给 SqlSugar 的 ADO 事务。
/// </para>
/// <para>
/// 【生命周期】
/// 与 ISqlSugarClient 共享 Scoped 生命周期，同一请求内的所有仓储操作使用同一事务上下文。
/// DI 注册时必须确保 IUnitOfWork 和 ISqlSugarClient 同为 Scoped。
/// </para>
/// <para>
/// 【事务隔离级别】
/// Oracle 默认事务隔离级别为 READ COMMITTED，满足本项目的并发控制需求。
/// 不需要显式设置 SERIALIZABLE，因为并发控制通过 PR_LIMIT_LOCK 的
/// SELECT ... FOR UPDATE 实现，而非依赖事务隔离级别。
/// </para>
/// <para>
/// 【Dispose 语义】
/// Dispose 不主动回滚事务。如果事务已 Begin 但未 Commit/Rollback，
/// 连接释放时 Oracle 会自动回滚。但最佳实践是在 catch 块中显式 RollbackAsync，
/// 不要依赖 Dispose 的隐式回滚。
/// </para>
/// </remarks>
public sealed class SqlSugarUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于执行 ADO 级别的事务操作。
    /// </summary>
    /// <remarks>
    /// 通过 DI 注入的 Scoped 实例，与同一请求内的所有仓储共享同一连接和事务。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 释放标记，防止重复释放。
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// 初始化工作单元。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器注入的 Scoped 实例。</param>
    public SqlSugarUnitOfWork(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 开始数据库事务。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 调用 SqlSugar 的 ADO 层 BeginTranAsync，底层执行 Oracle 的 BEGIN TRANSACTION。
    /// 调用后同一 ISqlSugarClient 上的所有操作将在同一事务中执行。
    /// </remarks>
    public async Task BeginAsync()
    {
        await _db.Ado.BeginTranAsync();
    }

    /// <summary>
    /// 提交数据库事务。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 调用 SqlSugar 的 ADO 层 CommitTranAsync，将所有挂起变更持久化到 Oracle。
    /// 提交后事务结束，后续操作不再受事务保护。
    /// </remarks>
    public async Task CommitAsync()
    {
        await _db.Ado.CommitTranAsync();
    }

    /// <summary>
    /// 回滚数据库事务。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 调用 SqlSugar 的 ADO 层 RollbackTranAsync，丢弃所有挂起变更。
    /// 通常在 catch 块中调用，确保异常时不会部分写入。
    /// </remarks>
    public async Task RollbackAsync()
    {
        await _db.Ado.RollbackTranAsync();
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【释放策略】
    /// Dispose 不主动回滚事务。如果事务已 Begin 但未 Commit/Rollback，
    /// 连接释放时 Oracle 会自动回滚。
    /// </para>
    /// <para>
    /// 【最佳实践】
    /// 不要依赖 Dispose 的隐式回滚，应在 catch 块中显式调用 RollbackAsync。
    /// 隐式回滚无法记录日志，不利于排查问题。
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}
