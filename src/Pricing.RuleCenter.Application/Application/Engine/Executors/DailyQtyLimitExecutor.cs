using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// DailyQtyLimitExecutor 规则动作执行器，负责执行一种可配置动作并把结果写回计价上下文。
/// </summary>
/// <remarks>
/// 该执行器处理"同一患者同一项目自然日数量上限"。它与时间窗口执行器共享同一套
/// PENDING + CONFIRMED 占额口径，区别只是窗口范围固定为业务收费日当天。
/// </remarks>
public sealed class DailyQtyLimitExecutor : IRuleActionExecutor
{
    private const string LimitType = "DAY_QTY";
    /// <summary>
    /// 单日累计必须计入的占用状态。PENDING 是待提交保护占用，CONFIRMED 是已落账正式占用。
    /// </summary>
    private static readonly string[] OccupyStatuses = { "PENDING", "CONFIRMED" };
    /// <summary>
    /// 限额占用仓储，负责单日累计查询、锁行创建和占额明细落库。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 初始化单日数量限制执行器。
    /// </summary>
    /// <param name="limitRepository">限额占用仓储。</param>
    public DailyQtyLimitExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    /// <summary>
    /// 获取规则动作类型编码。该编码必须与规则动作字典中的 APPLY_DAY_LIMIT_QTY 一致。
    /// </summary>
    public string ActionType => "APPLY_DAY_LIMIT_QTY";

    /// <summary>
    /// 执行单日数量上限动作。
    /// </summary>
    /// <param name="action">规则动作配置，参数中必须提供 MaxDailyQty。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>异步任务。</returns>
    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        var (limitKey, dimensionCode, dayStart, dayEnd) = BuildDailyScope(context);
        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(new[] { limitKey });
        }

        var occupiedQty = await GetOccupiedQtyAsync(context, dimensionCode, dayStart, dayEnd);
        LimitExecutionSupport.ApplyRemainingQty(context, param.MaxDailyQty - occupiedQty);
        LimitOccupyDraftAppender.AddDraft(context, LimitType, limitKey, dimensionCode);
    }

    private static (string LimitKey, string DimensionCode, DateTime DayStart, DateTime DayEnd) BuildDailyScope(
        PricingContext context)
    {
        var limitKey = LimitKeyGenerator.GenerateDailyKey(
            context.PatientId,
            context.ItemCode,
            context.BusinessChargeTime);
        var dimensionCode = $"{context.PatientId}:{context.ItemCode}:{context.BusinessChargeTime:yyyyMMdd}"
            .ToUpperInvariant();
        var dayStart = context.BusinessChargeTime.Date;
        var dayEnd = dayStart.AddDays(1).AddTicks(-1);
        return (limitKey, dimensionCode, dayStart, dayEnd);
    }

    private async Task<decimal> GetOccupiedQtyAsync(
        PricingContext context,
        string dimensionCode,
        DateTime dayStart,
        DateTime dayEnd)
    {
        var occupiedQty = await _limitRepository.GetOccupiedQtyAsync(new LimitOccupyRangeQuery
        {
            LimitType = LimitType,
            LimitDimensionCode = dimensionCode,
            StartTime = dayStart,
            EndTime = dayEnd,
            Statuses = OccupyStatuses
        });
        return occupiedQty + SharedLimitStateReader.GetOccupiedQty(
            context,
            LimitType,
            dimensionCode,
            dayStart,
            dayEnd);
    }

    /// <summary>
    /// 解析单日数量限制参数。
    /// </summary>
    /// <param name="json">动作参数 JSON。</param>
    /// <returns>解析后的参数对象；参数为空时返回 null。</returns>
    private static DailyQtyParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return RuleCenterJsonSerializer.Deserialize<DailyQtyParams>(json);
    }

    /// <summary>
    /// 单日数量限制参数。
    /// </summary>
    private sealed class DailyQtyParams
    {
        /// <summary>
        /// 同一患者同一项目在业务自然日内允许收费的最大数量。
        /// </summary>
        public decimal MaxDailyQty { get; set; }
    }
}
