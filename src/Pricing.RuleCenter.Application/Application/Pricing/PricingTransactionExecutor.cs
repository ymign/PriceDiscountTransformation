using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 计价事务执行器。
/// </summary>
public sealed class PricingTransactionExecutor
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PricingTransactionExecutor> _logger;

    /// <summary>
    /// 初始化计价事务执行器。
    /// </summary>
    /// <param name="unitOfWork">工作单元，用于开启、提交和回滚事务。</param>
    /// <param name="logger">日志组件，用于记录事务失败上下文。</param>
    public PricingTransactionExecutor(
        IUnitOfWork unitOfWork,
        ILogger<PricingTransactionExecutor> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 在统一事务边界内执行异步操作并返回结果。
    /// </summary>
    /// <typeparam name="T">操作返回值类型。</typeparam>
    /// <param name="action">需要放入事务中的异步操作。</param>
    /// <returns>事务内操作结果。</returns>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            await _unitOfWork.BeginAsync();
            var result = await action();
            await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// 在统一事务边界内执行无返回值异步操作。
    /// </summary>
    /// <param name="action">需要放入事务中的异步操作。</param>
    public async Task ExecuteAsync(Func<Task> action)
    {
        await ExecuteAsync(async () =>
        {
            await action();
            return true;
        });
    }
}
