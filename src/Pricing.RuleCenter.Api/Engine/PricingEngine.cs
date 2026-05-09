using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine;

public sealed class PricingEngine : IPricingEngine
{
    private readonly RuleMatchService _ruleMatchService;
    private readonly ActionExecutionPipeline _pipeline;
    private readonly ILogger<PricingEngine> _logger;

    public PricingEngine(
        RuleMatchService ruleMatchService,
        ActionExecutionPipeline pipeline,
        ILogger<PricingEngine> logger)
    {
        _ruleMatchService = ruleMatchService;
        _pipeline = pipeline;
        _logger = logger;
    }

    public async Task<PricingResult> CalculateAsync(PricingContext context)
    {
        context.ConvertedQty = context.InputQty;
        context.FinalQty = context.InputQty;
        context.FinalAmount = context.UnitPrice * context.InputQty;
        context.FormulaAmount = 0;
        context.LimitedAmount = 0;
        context.DiscountAmount = 0;

        var (matchedRules, orderedActions) = await _ruleMatchService.MatchAsync(context);

        if (matchedRules.Count == 0)
        {
            return BuildResult(context, false);
        }

        context.MatchedRules = matchedRules;
        context.OrderedActions = orderedActions;

        await _pipeline.ExecuteAsync(orderedActions, context);

        var originalAmount = context.UnitPrice * context.InputQty;
        context.DiscountAmount = originalAmount - context.FinalAmount;

        _logger.LogInformation(
            "计价完成 ItemCode={ItemCode}, 原金额={OriginalAmount}, 最终金额={FinalAmount}, 折扣={DiscountAmount}",
            context.ItemCode, originalAmount, context.FinalAmount, context.DiscountAmount);

        return BuildResult(context, true);
    }

    private static PricingResult BuildResult(PricingContext context, bool isSpecial)
    {
        return new PricingResult
        {
            IsSpecialItem = isSpecial,
            InputQty = context.InputQty,
            FinalQty = context.FinalQty,
            UnitPrice = context.UnitPrice,
            FinalAmount = context.FinalAmount,
            DiscountAmount = context.DiscountAmount,
            TraceSteps = context.TraceSteps.ToList(),
            MatchedRuleIds = context.MatchedRules.Select(r => r.RuleId).ToList()
        };
    }
}
