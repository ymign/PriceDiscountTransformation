using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>试算计价命令。</summary>
public sealed record SimulatePricingCommand(PricingCalculateRequest Request)
    : IRequest<PricingCalculateResponse>;

/// <summary>试算计价命令处理器。</summary>
public sealed class SimulatePricingCommandHandler : IRequestHandler<SimulatePricingCommand, PricingCalculateResponse>
{
    private readonly PricingSimulateWorkflow _workflow;

    /// <summary>初始化处理器。</summary>
    public SimulatePricingCommandHandler(PricingSimulateWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    public Task<PricingCalculateResponse> Handle(SimulatePricingCommand request, CancellationToken cancellationToken)
    {
        return _workflow.ExecuteAsync(request.Request);
    }
}
