using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 单次收费动作数量限制执行器。
/// </summary>
/// <remarks>
/// “单次”按单次收费动作统计，维度为来源系统 + 稳定业务请求号 + 项目编码。它不能用收费明细号作为
/// 维度，否则一次结算内同一项目多条费用会绕过单次上限。
/// </remarks>
public sealed class OnceQtyLimitExecutor : IRuleActionExecutor
{
    private static readonly string[] OccupyStatuses = { "PENDING", "CONFIRMED" };
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 初始化单次限额执行器。
    /// </summary>
    /// <param name="limitRepository">限额占用仓储。</param>
    public OnceQtyLimitExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    /// <summary>
    /// 获取动作类型编码。
    /// </summary>
    public string ActionType => "APPLY_ONCE_LIMIT_QTY";

    /// <summary>
    /// 执行单次收费动作数量限制。
    /// </summary>
    /// <param name="action">规则动作配置。</param>
    /// <param name="context">计价上下文。</param>
    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        var maxOnceQty = param?.GetMaxOnceQty() ?? 0;
        if (maxOnceQty <= 0)
        {
            return;
        }

        var dimensionCode = BuildDimensionCode(context);
        var limitKey = $"OQ:{dimensionCode}";

        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(new[] { limitKey });
        }

        var occupiedQty = 0m;
        foreach (var status in OccupyStatuses)
        {
            occupiedQty += await _limitRepository.GetOccupiedQtyAsync(limitKey, status);
        }

        var inRequestKey = $"ONCE_QTY:{dimensionCode}";
        if (context.InRequestOccupiedQtyByLimitDimension.TryGetValue(inRequestKey, out var inRequestQty))
        {
            occupiedQty += inRequestQty;
        }

        var remainingQty = maxOnceQty - occupiedQty;
        if (remainingQty <= 0)
        {
            context.FinalQty = 0;
            context.FinalAmount = 0;
            AddOccupyDraft(context, limitKey, dimensionCode);
            return;
        }

        if (context.FinalQty > remainingQty)
        {
            var beforeQty = context.FinalQty;
            context.FinalQty = remainingQty;
            context.FinalAmount = beforeQty == 0
                ? 0
                : context.FinalAmount * context.FinalQty / beforeQty;
        }

        AddOccupyDraft(context, limitKey, dimensionCode);
    }

    /// <summary>
    /// 生成单次限额维度。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <returns>来源系统、收费动作号和项目编码拼接出的稳定维度。</returns>
    public static string BuildDimensionCode(PricingContext context)
    {
        var businessRequestNo = string.IsNullOrWhiteSpace(context.BusinessRequestNo)
            ? "UNKNOWN"
            : context.BusinessRequestNo.Trim();
        return $"{context.SourceSystem}:{businessRequestNo}:{context.ItemCode}".ToUpperInvariant();
    }

    private static void AddOccupyDraft(PricingContext context, string limitKey, string dimensionCode)
    {
        if (context.PendingLimitOccupies.Any(o =>
                o.LimitType == "ONCE_QTY" &&
                o.LimitDimensionCode == dimensionCode))
        {
            return;
        }

        context.PendingLimitOccupies.Add(new LimitOccupy
        {
            PatientId = context.PatientId,
            ItemCode = context.ItemCode,
            LimitType = "ONCE_QTY",
            LimitKey = limitKey,
            LimitDimensionCode = dimensionCode,
            BusinessChargeTime = context.BusinessChargeTime,
            OccupyType = "CHARGE"
        });
    }

    private static OnceQtyParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<OnceQtyParams>(json);
    }

    private sealed class OnceQtyParams
    {
        public decimal MaxOnceQty { get; set; }
        public decimal LimitQty { get; set; }
        public decimal MaxQty { get; set; }

        public decimal GetMaxOnceQty()
        {
            return MaxOnceQty > 0
                ? MaxOnceQty
                : LimitQty > 0
                    ? LimitQty
                    : MaxQty;
        }
    }
}
