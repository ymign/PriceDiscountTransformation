using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>
/// 退费冲正命令，表示 HIS 对已落账计价结果发起退费或冲销。
/// </summary>
public sealed record ReversePricingCommand(PricingReverseRequest Request) : IRequest;

/// <summary>
/// 退费冲正命令处理器，负责把 reverse 命令交给 <see cref="PricingReverseWorkflow"/>。
/// </summary>
public sealed class ReversePricingCommandHandler : IRequestHandler<ReversePricingCommand>
{
    /// <summary>
    /// reverse 工作流，封装退费幂等、历史已退校验、冲正日志和负向占用。
    /// </summary>
    private readonly PricingReverseWorkflow _workflow;

    /// <summary>
    /// 初始化退费冲正命令处理器。
    /// </summary>
    /// <param name="workflow">退费冲正工作流。</param>
    public ReversePricingCommandHandler(PricingReverseWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// 处理退费冲正命令。
    /// </summary>
    /// <param name="request">退费冲正命令。</param>
    /// <param name="cancellationToken">MediatR 管道取消令牌；当前 workflow 内部尚未逐层传递。</param>
    /// <returns>MediatR 空返回值。</returns>
    public async Task<Unit> Handle(ReversePricingCommand request, CancellationToken cancellationToken)
    {
        // reverse 与 cancel 的业务边界不同：这里处理已落账事实，不能直接删除原占用。
        await _workflow.ExecuteAsync(request.Request);
        return Unit.Value;
    }
}
