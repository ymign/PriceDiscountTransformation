using MediatR;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Commands;

/// <summary>
/// 试算计价命令，表示 HTTP simulate/batch-simulate 入口已经完成模型绑定，准备进入应用层试算 workflow。
/// </summary>
/// <remarks>
/// 命令对象只携带请求 DTO，不承载业务逻辑。这样控制器、校验管道和 workflow 的边界清晰：
/// 控制器负责路由，FluentValidation 负责基础参数校验，workflow 负责规则匹配、试算日志和响应构建。
/// </remarks>
public sealed record SimulatePricingCommand(PricingCalculateRequest Request)
    : IRequest<PricingCalculateResponse>;

/// <summary>
/// 试算计价命令处理器，负责把 MediatR 命令转交给 <see cref="PricingSimulateWorkflow"/>。
/// </summary>
/// <remarks>
/// 处理器刻意保持极薄，不在这里做规则计算或写日志，避免同一业务逻辑分散在 Handler 和 Workflow 两处。
/// </remarks>
public sealed class SimulatePricingCommandHandler : IRequestHandler<SimulatePricingCommand, PricingCalculateResponse>
{
    /// <summary>
    /// 试算工作流，封装完整的不占额计价流程。
    /// </summary>
    private readonly PricingSimulateWorkflow _workflow;

    /// <summary>
    /// 初始化试算命令处理器。
    /// </summary>
    /// <param name="workflow">试算计价工作流。</param>
    public SimulatePricingCommandHandler(PricingSimulateWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// 处理试算命令并返回试算响应。
    /// </summary>
    /// <param name="request">试算命令。</param>
    /// <param name="cancellationToken">MediatR 管道取消令牌；当前 workflow 内部尚未逐层传递。</param>
    /// <returns>试算计价响应。</returns>
    public Task<PricingCalculateResponse> Handle(SimulatePricingCommand request, CancellationToken cancellationToken)
    {
        // Handler 只做转发，保证 simulate 和 batch-simulate 都进入同一个 workflow，
        // 由 workflow 根据 Items 数量决定是否启用批量上下文。
        return _workflow.ExecuteAsync(request.Request);
    }
}
