using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

public sealed class TimeWindowLimitExecutor : IRuleActionExecutor
{
    private readonly ILimitOccupyRepository _limitRepository;

    public TimeWindowLimitExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    public string ActionType => "TIME_WINDOW_LIMIT";

    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        var limitKey = BuildLimitKey(context, param.WindowHours);
        var occupiedQty = await _limitRepository.GetOccupiedQtyAsync(limitKey, "CONFIRMED");

        var remainingQty = param.MaxQty - occupiedQty;
        if (remainingQty <= 0)
        {
            context.FinalQty = 0;
            context.FinalAmount = 0;
            return;
        }

        if (context.FinalQty > remainingQty)
        {
            var exceedQty = context.FinalQty - remainingQty;
            context.FinalQty = remainingQty;
            context.FinalAmount = context.UnitPrice * context.FinalQty;
        }
    }

    private static string BuildLimitKey(PricingContext context, int windowHours)
    {
        var windowStart = context.BusinessChargeTime.AddHours(-windowHours);
        return $"TW:{context.PatientId}:{context.ItemCode}:{windowStart:yyyyMMddHH}-{context.BusinessChargeTime:yyyyMMddHH}";
    }

    private static TimeWindowParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<TimeWindowParams>(json);
    }

    private sealed class TimeWindowParams
    {
        public int WindowHours { get; set; } = 2;
        public decimal MaxQty { get; set; }
    }
}
