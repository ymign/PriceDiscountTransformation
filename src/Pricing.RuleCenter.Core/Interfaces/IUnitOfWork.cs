namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 工作单元接口，封装跨仓储的事务管理。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 应用服务需要跨多个仓储的事务一致性（如 confirm 同时写入请求日志和限额占用）。
/// 该接口解除应用服务对 ORM（ISqlSugarClient）的直接依赖，
/// 由基础设施层提供具体实现（SqlSugarUnitOfWork）。
/// </para>
/// <para>
/// 【为什么需要 IUnitOfWork】
/// 原实现中 PricingApiService 直接依赖 ISqlSugarClient 开启事务，
/// 违反了依赖倒置原则（应用层不应依赖基础设施层）。
/// 引入 IUnitOfWork 后，应用层只依赖接口，事务管理由基础设施层实现。
/// </para>
/// <para>
/// 【事务边界】
/// confirm、commit、cancel、reverse 等操作必须保证请求日志、折价明细、
/// 限额占用三张表在同一事务中写入，否则会出现资金口径断裂。
/// </para>
/// <para>
/// 【生命周期】
/// IUnitOfWork 注册为 Scoped，与 HTTP 请求同生命周期。
/// 同一请求内的所有仓储操作共享同一事务上下文。
/// </para>
/// <para>
/// 【使用模式】
/// <code>
/// await _uow.BeginAsync();
/// try
/// {
///     // 执行多个仓储操作
///     await _requestLogRepo.InsertAsync(request);
///     await _discountDetailRepo.InsertAsync(detail);
///     await _limitOccupyRepo.InsertAsync(occupy);
///     await _uow.CommitAsync();
/// }
/// catch
/// {
///     await _uow.RollbackAsync();
///     throw;
/// }
/// </code>
/// </para>
/// </remarks>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 开始数据库事务。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 调用后所有仓储操作将在同一事务中执行，直到 CommitAsync 或 RollbackAsync。
    /// </remarks>
    Task BeginAsync();

    /// <summary>
    /// 提交数据库事务，将所有挂起的变更持久化到数据库。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 提交后事务结束，后续仓储操作不再受事务保护。
    /// 提交失败时事务自动回滚。
    /// </remarks>
    Task CommitAsync();

    /// <summary>
    /// 回滚数据库事务，丢弃所有挂起的变更。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// 回滚后事务结束，数据库恢复到 BeginAsync 之前的状态。
    /// 通常在 catch 块中调用，确保异常时不会部分写入。
    /// </remarks>
    Task RollbackAsync();
}
