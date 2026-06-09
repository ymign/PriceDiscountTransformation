using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>
/// 确认计价命令，表示外部渠道准备正式收费并请求规则中心占用额度。
/// </summary>
/// <remarks>
/// confirm 是资金相关命令，后续必须进入 <see cref="PricingConfirmWorkflow"/> 做幂等、规则计算、
/// 限额加锁、请求日志、折价明细和待确认占用落库。命令本身只保存请求 DTO。
/// </remarks>
public sealed record ConfirmPricingCommand(PricingCalculateRequest Request)
    : IRequest<PricingCalculateResponse>;

/// <summary>
/// 确认计价命令处理器，负责将 MediatR 命令转交给确认计价 workflow。
/// </summary>
public sealed class ConfirmPricingCommandHandler : IRequestHandler<ConfirmPricingCommand, PricingCalculateResponse>
{
    /// <summary>
    /// 确认计价工作流，封装幂等、事务、占额和折价明细写入。
    /// </summary>
    private readonly PricingConfirmWorkflow _workflow;

    /// <summary>
    /// 初始化确认计价命令处理器。
    /// </summary>
    /// <param name="workflow">确认计价工作流。</param>
    public ConfirmPricingCommandHandler(PricingConfirmWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// 处理确认计价命令。
    /// </summary>
    /// <param name="request">确认计价命令。</param>
    /// <param name="cancellationToken">MediatR 管道取消令牌；当前 workflow 内部尚未逐层传递。</param>
    /// <returns>正式确认计价结果，包含 RequestId 和过期时间。</returns>
    public Task<PricingCalculateResponse> Handle(ConfirmPricingCommand request, CancellationToken cancellationToken)
    {
        // 不在 Handler 中做幂等判断。幂等必须和事务内二次检查、响应快照读取保持在同一个 workflow 中。
        return _workflow.ExecuteAsync(request.Request);
    }
}
