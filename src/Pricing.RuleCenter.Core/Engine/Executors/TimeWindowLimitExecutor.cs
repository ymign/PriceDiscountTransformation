using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

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
        // ========== 第一阶段：读取动作参数 ==========
        // 参数为空时无法确定窗口长度或上限，当前动作直接不生效。生产规则发布前应通过配置校验阻断这种情况。
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        // ========== 第二阶段：确定业务窗口和锁维度 ==========
        // 优先使用 windowMinutes，兼容旧配置 windowHours。窗口起止都基于 BusinessChargeTime，
        // 不是当前服务器时间，避免补录和补缴费进入错误窗口。
        var windowMinutes = param.WindowMinutes > 0
            ? param.WindowMinutes
            : Math.Max(param.WindowHours, 1) * 60;
        var limitQty = param.LimitQty > 0 ? param.LimitQty : param.MaxQty;
        var windowStart = context.BusinessChargeTime.AddMinutes(-windowMinutes);
        var windowEnd = context.BusinessChargeTime;
        var dimensionCode = BuildDimensionCode(context);
        var lockKeys = BuildLockKeys(context, windowStart, windowEnd);

        // ========== 第三阶段：confirm 阶段加锁 ==========
        // 试算不加锁，因为试算不产生占额。confirm 必须加锁，确保窗口累计与后续写入占额在同一事务中串行。
        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(lockKeys);
        }

        // ========== 第四阶段：查询 PENDING + CONFIRMED 累计 ==========
        // PENDING 是并发保护占用，必须计入剩余额度；否则两个未 commit 的 confirm 会同时通过。
        var occupiedQty = await _limitRepository.GetOccupiedQtyAsync(
            "TIME_WINDOW", dimensionCode, windowStart, windowEnd, OccupyStatuses);
        occupiedQty += GetInRequestOccupiedQty(context, dimensionCode, windowStart, windowEnd);

        // ========== 第五阶段：按剩余额度截断数量和金额 ==========
        // 当剩余额度小于等于 0 时，本次有效数量为 0，金额也归 0。
        var remainingQty = limitQty - occupiedQty;
        if (remainingQty <= 0)
        {
            context.FinalQty = 0;
            context.FinalAmount = 0;
            AddOccupyDraft(context, lockKeys[0], dimensionCode);
            return;
        }

        // 如果本次数量超出剩余额度，只保留允许收费的数量。金额按当前金额比例缩放，
        // 这样可以兼容前面已经执行过公式动作的情况，而不是简单用单价乘数量覆盖。
        if (context.FinalQty > remainingQty)
        {
            var beforeQty = context.FinalQty;
            context.FinalQty = remainingQty;
            context.FinalAmount = beforeQty == 0
                ? 0
                : context.FinalAmount * context.FinalQty / beforeQty;
        }

        // ========== 第六阶段：追加占额草稿 ==========
        // 执行器只负责生成占额草稿，真正落库由 PricingApiService 在 confirm 事务中完成。
        AddOccupyDraft(context, lockKeys[0], dimensionCode);
    }

    /// <summary>
    /// 生成稳定累计维度。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <returns>患者加项目维度的大写编码。</returns>
    private static string BuildDimensionCode(PricingContext context)
    {
        return $"{context.PatientId}:{context.ItemCode}".ToUpperInvariant();
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

    /// <summary>
    /// 向计价上下文追加待写入的限额占用草稿。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="limitKey">用于加锁和审计的限额键。</param>
    /// <param name="dimensionCode">用于累计查询的稳定维度。</param>
    private static void AddOccupyDraft(
        PricingContext context, string limitKey, string dimensionCode)
    {
        // 同一个动作链可能因为规则叠加重复进入同一限额维度。这里去重，避免同一次 confirm 写多条重复占额。
        if (context.PendingLimitOccupies.Any(o =>
                o.LimitType == "TIME_WINDOW" &&
                o.LimitDimensionCode == dimensionCode))
        {
            return;
        }

        context.PendingLimitOccupies.Add(new LimitOccupy
        {
            PatientId = context.PatientId,
            ItemCode = context.ItemCode,
            LimitType = "TIME_WINDOW",
            LimitKey = limitKey,
            LimitDimensionCode = dimensionCode,
            BusinessChargeTime = context.BusinessChargeTime,
            OccupyType = "CHARGE"
        });
    }

    private static decimal GetInRequestOccupiedQty(
        PricingContext context,
        string dimensionCode,
        DateTime windowStart,
        DateTime windowEnd)
    {
        var candidates = context.InRequestLimitOccupies
            .Where(o => string.Equals(o.LimitType, "TIME_WINDOW", StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (candidates.Count > 0)
        {
            return candidates
                .Where(o => o.BusinessChargeTime >= windowStart && o.BusinessChargeTime <= windowEnd)
                .Sum(o => o.OccupyQty);
        }

        var inRequestKey = $"TIME_WINDOW:{dimensionCode}";
        return context.InRequestOccupiedQtyByLimitDimension.TryGetValue(inRequestKey, out var cachedQty)
            ? cachedQty
            : 0m;
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

        return JsonConvert.DeserializeObject<TimeWindowParams>(json);
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
