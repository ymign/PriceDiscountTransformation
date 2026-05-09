using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

public sealed class DailyQtyLimitExecutor : IRuleActionExecutor
{
    private readonly ILimitOccupyRepository _limitRepository;

    public DailyQtyLimitExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    public string ActionType => "DAILY_QTY_LIMIT";

    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        var limitKey = $"DAY:{context.PatientId}:{context.ItemCode}:{context.BusinessChargeTime:yyyyMMdd}";
        var occupiedQty = await _limitRepository.GetOccupiedQtyAsync(limitKey, "CONFIRMED");

        var remainingQty = param.MaxDailyQty - occupiedQty;
        if (remainingQty <= 0)
        {
            context.FinalQty = 0;
            context.FinalAmount = 0;
            return;
        }

        if (context.FinalQty > remainingQty)
        {
            context.FinalQty = remainingQty;
            context.FinalAmount = context.UnitPrice * context.FinalQty;
        }
    }

    private static DailyQtyParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<DailyQtyParams>(json);
    }

    private sealed class DailyQtyParams
    {
        public decimal MaxDailyQty { get; set; }
    }
}
