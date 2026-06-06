using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 计价追踪步骤写入器。
/// </summary>
public sealed class PricingTraceStepWriter
{
    private readonly IChargeTraceStepRepository _traceStepRepository;
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
        var steps = calculations
            .SelectMany(c => c.Result.TraceSteps.Select(s => (c.Item, Step: s)))
            .ToList();
        if (steps.Count == 0)
        {
            return;
        }

        var now = _clock.Now;
        var stepNo = 1;
        var entities = steps.Select(s => new ChargeTraceStep
        {
            RequestId = requestId,
            TraceId = traceId,
            StepNo = stepNo++,
            StepName = s.Step.StepType,
            StepType = s.Step.StepType,
            InputSnapshot = s.Step.InputValue?.ToString(),
            OutputSnapshot = s.Step.OutputValue?.ToString(),
            StepDesc = $"{s.Item.ItemCode}: {s.Step.StepDesc}",
            CreatedAt = now
        }).ToList();

        await _traceStepRepository.InsertBatchAsync(entities);
    }
}
