using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则发布生命周期事务执行器。
/// </summary>
public sealed class RulePublishTransactionWriter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RulePublishTransactionWriter> _logger;

    /// <summary>
    /// 初始化规则发布生命周期事务执行器。
    /// </summary>
    public RulePublishTransactionWriter(
        IUnitOfWork unitOfWork,
        ILogger<RulePublishTransactionWriter> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 在同一数据库事务中执行多表状态变更。
    /// </summary>
    public async Task ExecuteAsync(Func<Task> action)
    {
        try
        {
            await _unitOfWork.BeginAsync();
            await action();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则发布事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
