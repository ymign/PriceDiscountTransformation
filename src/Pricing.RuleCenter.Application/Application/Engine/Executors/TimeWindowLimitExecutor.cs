using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// TimeWindowLimitExecutor 规则动作执行器，负责执行一种可配置动作并把结果写回计价上下文。
/// </summary>
/// <remarks>
/// 该执行器实现"最近 N 分钟内同一患者同一项目最多收费 M 个"的口径。
/// 试算时只读累计，不写占额；confirm 时会先锁定窗口覆盖到的全部小时桶，
/// 再读取 PENDING + CONFIRMED 净占用，最后向计价结果中挂上待写入的占额草稿。
/// </remarks>
public sealed class TimeWindowLimitExecutor : IRuleActionExecutor
{
    private const string LimitType = "TIME_WINDOW";
    /// <summary>
    /// 时间窗口累计时必须计入的占用状态。PENDING 表示已经 confirm 但尚未 commit 的保护占用，
    /// CONFIRMED 表示 HIS 已经落账成功的正式占用。
    /// </summary>
    private static readonly string[] OccupyStatuses = { "PENDING", "CONFIRMED" };
    /// <summary>
    /// 限额占用仓储，负责查询窗口累计、创建锁行并执行 SELECT FOR UPDATE。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 初始化时间窗口数量限制执行器。
    /// </summary>
    /// <param name="limitRepository">限额占用仓储。</param>
    public TimeWindowLimitExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    /// <summary>
    /// 获取规则动作类型编码。该编码必须与 PR_DICT.ACTION_TYPE 和规则配置保持一致。
    /// </summary>
    public string ActionType => "APPLY_TIME_WINDOW_LIMIT";

    /// <summary>
    /// 执行时间窗口数量限制。
    /// </summary>
    /// <param name="action">规则动作配置，参数中可配置 windowMinutes/windowHours 与 limitQty/maxQty。</param>
    /// <param name="context">计价上下文，包含患者、项目、业务收费时间、当前数量和当前金额。</param>
    /// <returns>异步任务。</returns>
    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        var (limitQty, windowStart, windowEnd, dimensionCode, lockKeys) = BuildWindowScope(param, context);
        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(lockKeys);
        }

        var occupiedQty = await GetOccupiedQtyAsync(context, dimensionCode, windowStart, windowEnd);
        occupiedQty += context.LegacyOccupiedQty;
        LimitExecutionSupport.ApplyRemainingQty(context, limitQty - occupiedQty);
        LimitOccupyDraftAppender.AddDraft(context, LimitType, lockKeys[0], dimensionCode);
    }

    private static (decimal LimitQty, DateTime WindowStart, DateTime WindowEnd, string DimensionCode, List<string> LockKeys)
        BuildWindowScope(TimeWindowParams param, PricingContext context)
    {
        var windowMinutes = param.WindowMinutes > 0
            ? param.WindowMinutes
            : Math.Max(param.WindowHours, 1) * 60;
        var windowStart = context.BusinessChargeTime.AddMinutes(-windowMinutes);
        var windowEnd = context.BusinessChargeTime;
        var dimensionCode = $"{context.PatientId}:{context.ItemCode}".ToUpperInvariant();
        return (
            param.LimitQty > 0 ? param.LimitQty : param.MaxQty,
            windowStart,
            windowEnd,
            dimensionCode,
            BuildLockKeys(context, windowStart, windowEnd));
    }

    /// <summary>
    /// 枚举滑动窗口覆盖的全部小时桶锁键。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="windowStart">窗口开始时间。</param>
    /// <param name="windowEnd">窗口结束时间。</param>
    /// <returns>需要按顺序锁定的锁键列表。</returns>
    private static List<string> BuildLockKeys(PricingContext context, DateTime windowStart, DateTime windowEnd)
    {
        // 2 小时窗口在 10:01 这种整点边界会覆盖 08、09、10 三个小时桶。
        // 因此不能只锁当前小时和前一小时，必须枚举窗口覆盖到的所有小时。
        var keys = new List<string>();
        var current = new DateTime(windowStart.Year, windowStart.Month, windowStart.Day, windowStart.Hour, 0, 0);
        var end = new DateTime(windowEnd.Year, windowEnd.Month, windowEnd.Day, windowEnd.Hour, 0, 0);

        while (current <= end)
        {
            keys.Add($"TW:{context.PatientId}:{context.ItemCode}:{current:yyyyMMddHH}".ToUpperInvariant());
            current = current.AddHours(1);
        }

        return keys;
    }

    private async Task<decimal> GetOccupiedQtyAsync(
        PricingContext context,
        string dimensionCode,
        DateTime windowStart,
        DateTime windowEnd)
    {
        var occupiedQty = await _limitRepository.GetOccupiedQtyAsync(new LimitOccupyRangeQuery
        {
            LimitType = LimitType,
            LimitDimensionCode = dimensionCode,
            StartTime = windowStart,
            EndTime = windowEnd,
            Statuses = OccupyStatuses
        });
        return occupiedQty + SharedLimitStateReader.GetOccupiedQty(
            context,
            LimitType,
            dimensionCode,
            windowStart,
            windowEnd);
    }

    /// <summary>
    /// 解析时间窗口动作参数。
    /// </summary>
    /// <param name="json">动作参数 JSON。</param>
    /// <returns>解析后的参数对象；参数为空时返回 null。</returns>
    private static TimeWindowParams? DeserializeParams(string? json)
    {
        // 空参数代表规则配置不完整。这里不抛异常，是为了让规则发布校验承担配置完整性；
        // 运行期保持保守跳过可以减少因历史脏配置导致的接口整体不可用。
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return RuleCenterJsonSerializer.Deserialize<TimeWindowParams>(json);
    }

    /// <summary>
    /// 时间窗口数量限制参数。
    /// </summary>
    private sealed class TimeWindowParams
    {
        /// <summary>
        /// 兼容字段，表示窗口小时数；当 WindowMinutes 未配置时使用，默认 2 小时。
        /// </summary>
        public int WindowHours { get; set; } = 2;
        /// <summary>
        /// 推荐字段，表示滑动窗口分钟数；大于 0 时优先于 WindowHours。
        /// </summary>
        public int WindowMinutes { get; set; }
        /// <summary>
        /// 兼容字段，表示窗口内最大允许数量；当 LimitQty 未配置时使用。
        /// </summary>
        public decimal MaxQty { get; set; }
        /// <summary>
        /// 推荐字段，表示窗口内最大允许数量。
        /// </summary>
        public decimal LimitQty { get; set; }
    }
}
