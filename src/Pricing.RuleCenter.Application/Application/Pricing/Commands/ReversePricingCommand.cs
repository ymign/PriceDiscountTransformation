using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>退费冲正命令。</summary>
public sealed record ReversePricingCommand(PricingReverseRequest Request) : IRequest;

/// <summary>退费冲正命令处理器。</summary>
public sealed class ReversePricingCommandHandler : IRequestHandler<ReversePricingCommand>
{
    private readonly PricingAppService _service;

    /// <summary>初始化处理器。</summary>
    public ReversePricingCommandHandler(PricingAppService service)
    {
        _service = service;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(ReversePricingCommand request, CancellationToken cancellationToken)
    {
        await _service.ReverseAsync(request.Request);
        return Unit.Value;
    }
}
