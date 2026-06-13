using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 计价追踪步骤写入器。
/// </summary>
/// <remarks>
/// 步骤日志记录规则匹配和动作执行过程，是“为什么这样收费”的解释链。
/// 它和请求日志、折价明细共同构成折价追溯中心的三条链路之一。
/// </remarks>
public sealed class PricingTraceStepWriter
{
    /// <summary>
    /// 追踪步骤仓储。
    /// </summary>
    private readonly IChargeTraceStepRepository _traceStepRepository;
    /// <summary>
    /// 统一时钟。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价追踪步骤写入器。
    /// </summary>
    /// <param name="traceStepRepository">追踪步骤仓储。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingTraceStepWriter(
        IChargeTraceStepRepository traceStepRepository,
        IClock clock)
    {
        _traceStepRepository = traceStepRepository;
        _clock = clock;
    }

    internal async Task SaveAsync(
        long requestId,
        string? traceId,
        IReadOnlyList<ItemPricingCalculation> calculations)
    {
        // 多明细请求会把所有明细的步骤扁平化写入同一请求下，StepDesc 前缀补项目编码，方便追溯页面区分。
        var steps = calculations
            .SelectMany(c => c.Result.TraceSteps.Select(s => (c.Item, Step: s)))
            .ToList();
        if (steps.Count == 0)
        {
            // 未命中特殊规则的普通计价可能没有动作步骤，此时不写空步骤。
            return;
        }

        var now = _clock.Now;
        var stepNo = 1;
        var entities = steps.Select(s => new ChargeTraceStep
        {
            RequestId = requestId,
            TraceId = traceId,
            StepNo = stepNo++,
            // StepNo 在整个请求范围内连续编号，而不是每条费用明细从 1 开始，便于按请求时间线回放。
            StepName = s.Step.StepType,
            StepType = s.Step.StepType,
            // TraceStep.RuntimeRuleId 是引擎内的历史命名兼容字段，落库时写入直接规则 RuleId。
            RuleId = s.Step.RuntimeRuleId,
            InputSnapshot = s.Step.InputValue?.ToString(),
            OutputSnapshot = s.Step.OutputValue?.ToString(),
            StepDesc = $"{s.Item.ItemCode}: {s.Step.StepDesc}",
            CreatedAt = now
        }).ToList();

        await _traceStepRepository.InsertBatchAsync(entities);
    }
}
