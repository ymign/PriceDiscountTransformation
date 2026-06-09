using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>
/// 取消确认命令，表示 confirm 后 HIS 未落账，需要释放待确认保护占用。
/// </summary>
public sealed record CancelPricingCommand(PricingCancelRequest Request) : IRequest;

/// <summary>
/// 取消确认命令处理器，负责把 cancel 命令交给 <see cref="PricingCancelWorkflow"/>。
/// </summary>
public sealed class CancelPricingCommandHandler : IRequestHandler<CancelPricingCommand>
{
    /// <summary>
    /// cancel 工作流，封装请求锁、状态校验和占用释放。
    /// </summary>
    private readonly PricingCancelWorkflow _workflow;

    /// <summary>
    /// 初始化取消确认命令处理器。
    /// </summary>
    /// <param name="workflow">取消确认工作流。</param>
    public CancelPricingCommandHandler(PricingCancelWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// 处理取消确认命令。
    /// </summary>
    /// <param name="request">取消确认命令。</param>
    /// <param name="cancellationToken">MediatR 管道取消令牌；当前 workflow 内部尚未逐层传递。</param>
    /// <returns>MediatR 空返回值。</returns>
    public async Task<Unit> Handle(CancelPricingCommand request, CancellationToken cancellationToken)
    {
        // cancel 的边界是 CONFIRM_PENDING。已落账记录不能在 Handler 层特殊处理，必须由 workflow 拒绝并要求 reverse。
        await _workflow.ExecuteAsync(request.Request);
        return Unit.Value;
    }
}
