using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// DailyQtyLimitExecutor 规则动作执行器，负责执行一种可配置动作并把结果写回计价上下文。
/// </summary>
/// <remarks>
/// 该执行器处理"同一患者同一项目自然日数量上限"。它与时间窗口执行器共享同一套
/// PENDING + CONFIRMED 占额口径，区别只是窗口范围固定为业务收费日当天。
/// </remarks>
public sealed class DailyQtyLimitExecutor : IRuleActionExecutor
{
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
        // ========== 第一阶段：读取配置参数 ==========
        // 参数缺失时直接跳过，避免运行期脏配置导致所有计价请求不可用。
        // 正常情况下应由发布校验保证 MaxDailyQty 存在。
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        // ========== 第二阶段：构造自然日维度 ==========
        // 这里使用 BusinessChargeTime 的日期，而不是服务器当前日期。补录历史收费时必须进入历史业务日累计。
        var limitKey = LimitKeyGenerator.GenerateDailyKey(
            context.PatientId, context.ItemCode, context.BusinessChargeTime);
        var dimensionCode = $"{context.PatientId}:{context.ItemCode}:{context.BusinessChargeTime:yyyyMMdd}"
            .ToUpperInvariant();
        var dayStart = context.BusinessChargeTime.Date;
        var dayEnd = dayStart.AddDays(1).AddTicks(-1);

        // ========== 第三阶段：confirm 阶段锁定当天维度 ==========
        // 试算不锁，confirm 锁。锁定后再查累计，防止并发 confirm 同时突破单日上限。
        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(new[] { limitKey });
        }

        // ========== 第四阶段：查询当天待确认和已确认占用 ==========
        // PENDING 必须计入，因为它代表已经返回给渠道、正在等待 HIS 落账的占额。
        var occupiedQty = await _limitRepository.GetOccupiedQtyAsync(new LimitOccupyRangeQuery
        {
            LimitType = "DAY_QTY",
            LimitDimensionCode = dimensionCode,
            StartTime = dayStart,
            EndTime = dayEnd,
            Statuses = OccupyStatuses
        });
        occupiedQty += GetInRequestOccupiedQty(context, dimensionCode, dayStart, dayEnd);

        // ========== 第五阶段：截断数量和金额 ==========
        // 金额按当前金额比例缩放，避免覆盖前置公式动作已经算出的金额。
        var remainingQty = param.MaxDailyQty - occupiedQty;
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

        // ========== 第六阶段：记录占额草稿 ==========
        // 草稿稍后由 PricingApiService 统一写入数据库，确保 RequestId 已经生成。
        AddOccupyDraft(context, limitKey, dimensionCode);
    }

    /// <summary>
    /// 追加单日数量占额草稿。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="limitKey">限额键。</param>
    /// <param name="dimensionCode">累计查询维度。</param>
    private static void AddOccupyDraft(
        PricingContext context, string limitKey, string dimensionCode)
    {
        // 同一动作链内相同维度只写一条占额，避免规则叠加导致重复占用。
        if (context.PendingLimitOccupies.Any(o =>
                o.LimitType == "DAY_QTY" &&
                o.LimitDimensionCode == dimensionCode))
        {
            return;
        }

        context.PendingLimitOccupies.Add(new LimitOccupy
        {
            PatientId = context.PatientId,
            ItemCode = context.ItemCode,
            LimitType = "DAY_QTY",
            LimitKey = limitKey,
            LimitDimensionCode = dimensionCode,
            BusinessChargeTime = context.BusinessChargeTime,
            OccupyType = "CHARGE"
        });
    }

    private static decimal GetInRequestOccupiedQty(
        PricingContext context,
        string dimensionCode,
        DateTime dayStart,
        DateTime dayEnd)
    {
        var candidates = context.InRequestLimitOccupies
            .Where(o => string.Equals(o.LimitType, "DAY_QTY", StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (candidates.Count > 0)
        {
            return candidates
                .Where(o => o.BusinessChargeTime >= dayStart && o.BusinessChargeTime <= dayEnd)
                .Sum(o => o.OccupyQty);
        }

        var inRequestKey = $"DAY_QTY:{dimensionCode}";
        return context.InRequestOccupiedQtyByLimitDimension.TryGetValue(inRequestKey, out var cachedQty)
            ? cachedQty
            : 0m;
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

        return JsonConvert.DeserializeObject<DailyQtyParams>(json);
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
