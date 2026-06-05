using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>试算计价命令。</summary>
public sealed record SimulatePricingCommand(PricingCalculateRequest Request)
    : IRequest<PricingCalculateResponse>;

/// <summary>试算计价命令处理器。</summary>
public sealed class SimulatePricingCommandHandler : IRequestHandler<SimulatePricingCommand, PricingCalculateResponse>
{
    private readonly PricingAppService _service;

    /// <summary>初始化处理器。</summary>
    public SimulatePricingCommandHandler(PricingAppService service)
    {
        _service = service;
    }

    /// <inheritdoc />
    public Task<PricingCalculateResponse> Handle(SimulatePricingCommand request, CancellationToken cancellationToken)
    {
        return _service.SimulateAsync(request.Request);
    }
}
