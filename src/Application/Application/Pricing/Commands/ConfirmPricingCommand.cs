using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>确认计价命令。</summary>
public sealed record ConfirmPricingCommand(PricingCalculateRequest Request)
    : IRequest<PricingCalculateResponse>;

/// <summary>确认计价命令处理器。</summary>
public sealed class ConfirmPricingCommandHandler : IRequestHandler<ConfirmPricingCommand, PricingCalculateResponse>
{
    private readonly PricingAppService _service;

    /// <summary>初始化处理器。</summary>
    public ConfirmPricingCommandHandler(PricingAppService service)
    {
        _service = service;
    }

    /// <inheritdoc />
    public Task<PricingCalculateResponse> Handle(ConfirmPricingCommand request, CancellationToken cancellationToken)
    {
        return _service.ConfirmAsync(request.Request);
    }
}
