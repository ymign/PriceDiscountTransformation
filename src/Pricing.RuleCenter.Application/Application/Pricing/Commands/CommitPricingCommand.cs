using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>落账提交命令。</summary>
public sealed record CommitPricingCommand(PricingCommitRequest Request) : IRequest;

/// <summary>落账提交命令处理器。</summary>
public sealed class CommitPricingCommandHandler : IRequestHandler<CommitPricingCommand>
{
    private readonly PricingAppService _service;

    /// <summary>初始化处理器。</summary>
    public CommitPricingCommandHandler(PricingAppService service)
    {
        _service = service;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(CommitPricingCommand request, CancellationToken cancellationToken)
    {
        await _service.CommitAsync(request.Request);
        return Unit.Value;
    }
}
