using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>确认计价命令。</summary>
public sealed record ConfirmPricingCommand(PricingCalculateRequest Request)
    : IRequest<PricingCalculateResponse>;

/// <summary>确认计价命令处理器。</summary>
public sealed class ConfirmPricingCommandHandler : IRequestHandler<ConfirmPricingCommand, PricingCalculateResponse>
{
    private readonly PricingConfirmWorkflow _workflow;

    /// <summary>初始化处理器。</summary>
    public ConfirmPricingCommandHandler(PricingConfirmWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    public Task<PricingCalculateResponse> Handle(ConfirmPricingCommand request, CancellationToken cancellationToken)
    {
        return _workflow.ExecuteAsync(request.Request);
    }
}
