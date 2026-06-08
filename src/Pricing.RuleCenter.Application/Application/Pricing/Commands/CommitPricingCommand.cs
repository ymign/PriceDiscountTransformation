using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>落账提交命令。</summary>
public sealed record CommitPricingCommand(PricingCommitRequest Request) : IRequest;

/// <summary>落账提交命令处理器。</summary>
public sealed class CommitPricingCommandHandler : IRequestHandler<CommitPricingCommand>
{
    private readonly PricingCommitWorkflow _workflow;

    /// <summary>初始化处理器。</summary>
    public CommitPricingCommandHandler(PricingCommitWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(CommitPricingCommand request, CancellationToken cancellationToken)
    {
        await _workflow.ExecuteAsync(request.Request);
        return Unit.Value;
    }
}
