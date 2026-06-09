using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>
/// 落账提交命令，表示 HIS 已经成功写入收费明细，要求规则中心把 confirm 结果推进为正式确认。
/// </summary>
public sealed record CommitPricingCommand(PricingCommitRequest Request) : IRequest;

/// <summary>
/// 落账提交命令处理器，负责把 commit 命令交给 <see cref="PricingCommitWorkflow"/>。
/// </summary>
public sealed class CommitPricingCommandHandler : IRequestHandler<CommitPricingCommand>
{
    /// <summary>
    /// commit 工作流，封装请求锁、过期校验、HIS 落账明细对账和状态推进。
    /// </summary>
    private readonly PricingCommitWorkflow _workflow;

    /// <summary>
    /// 初始化落账提交命令处理器。
    /// </summary>
    /// <param name="workflow">落账提交工作流。</param>
    public CommitPricingCommandHandler(PricingCommitWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// 处理落账提交命令。
    /// </summary>
    /// <param name="request">落账提交命令。</param>
    /// <param name="cancellationToken">MediatR 管道取消令牌；当前 workflow 内部尚未逐层传递。</param>
    /// <returns>MediatR 空返回值。</returns>
    public async Task<Unit> Handle(CommitPricingCommand request, CancellationToken cancellationToken)
    {
        // commit 不重新计价，只基于 confirm 保存的折价明细和 HIS 实际落账明细推进状态。
        await _workflow.ExecuteAsync(request.Request);
        return Unit.Value;
    }
}
