using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>取消确认命令。</summary>
public sealed record CancelPricingCommand(PricingCancelRequest Request) : IRequest;

/// <summary>取消确认命令处理器。</summary>
public sealed class CancelPricingCommandHandler : IRequestHandler<CancelPricingCommand>
{
    private readonly PricingCancelWorkflow _workflow;

    /// <summary>初始化处理器。</summary>
    public CancelPricingCommandHandler(PricingCancelWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(CancelPricingCommand request, CancellationToken cancellationToken)
    {
        await _workflow.ExecuteAsync(request.Request);
        return Unit.Value;
    }
}
