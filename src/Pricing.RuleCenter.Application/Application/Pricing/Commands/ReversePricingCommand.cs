using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>退费冲正命令。</summary>
public sealed record ReversePricingCommand(PricingReverseRequest Request) : IRequest;

/// <summary>退费冲正命令处理器。</summary>
public sealed class ReversePricingCommandHandler : IRequestHandler<ReversePricingCommand>
{
    private readonly PricingReverseWorkflow _workflow;

    /// <summary>初始化处理器。</summary>
    public ReversePricingCommandHandler(PricingReverseWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(ReversePricingCommand request, CancellationToken cancellationToken)
    {
        await _workflow.ExecuteAsync(request.Request);
        return Unit.Value;
    }
}
