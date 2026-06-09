using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 单次收费动作数量限制执行器。
/// </summary>
/// <remarks>
/// "单次"按单次收费动作统计，维度为来源系统 + 稳定业务请求号 + 项目编码。它不能用收费明细号作为
/// 维度，否则一次结算内同一项目多条费用会绕过单次上限。
/// </remarks>
public sealed class OnceQtyLimitExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 单次限额累计时计入的占用状态。
    /// </summary>
    /// <remarks>
    /// PENDING 代表 confirm 后尚未落账但仍占住保护额度，CONFIRMED 代表已经落账的正式占用。
    /// 两者都必须计入，否则并发收费或迟到 commit 会突破单次上限。
    /// </remarks>
    private static readonly string[] OccupyStatuses = { "PENDING", "CONFIRMED" };

    /// <summary>
    /// 限额占用仓储，用于查询本次收费动作维度下已经占用的数量，并在 confirm 阶段锁定维度。
    /// </summary>
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
        // ========== 第一阶段：解析单次上限 ==========
        // 支持 MaxOnceQty/LimitQty/MaxQty 多个历史参数名，避免旧规则迁移后因字段名不同直接跳过。
        var param = DeserializeParams(action.ParamsJson);
        var maxOnceQty = param?.GetMaxOnceQty();
        if (!maxOnceQty.HasValue)
        {
            return;
        }

        // ========== 第二阶段：构建单次收费动作维度 ==========
        // 单次口径必须使用稳定业务请求号，不能用单条收费明细号。
        var dimensionCode = BuildDimensionCode(context);
        var limitKey = $"OQ:{dimensionCode}";

        if (context.ShouldLockLimits)
        {
            // confirm 阶段锁定单次维度，防止同一业务号并发重复确认突破单次上限。
            await _limitRepository.EnsureAndLockAsync(new[] { limitKey });
        }

        // ========== 第三阶段：汇总历史和本请求内占用 ==========
        // PENDING + CONFIRMED 都要计入，批量请求中前序明细也要计入。
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

        // ========== 第四阶段：按剩余额度截断数量和金额 ==========
        var remainingQty = maxOnceQty.Value - occupiedQty;
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

        // ========== 第五阶段：追加占额草稿 ==========
        // 执行器只生成草稿，最终 RequestId、TraceId 和状态由应用层写入。
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
        // 同一请求内相同单次维度只写一条占用，避免多条规则重复生成占额。
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
        // 空参数表示规则配置不完整。运行期不抛出，由发布校验负责阻断新脏配置。
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<OnceQtyParams>(json);
    }

    private sealed class OnceQtyParams
    {
        /// <summary>
        /// 当前版本推荐使用的单次收费动作最大允许数量。
        /// </summary>
        public decimal? MaxOnceQty { get; set; }

        /// <summary>
        /// 历史参数名，兼容旧配置中使用 LimitQty 表达单次上限的规则动作。
        /// </summary>
        public decimal? LimitQty { get; set; }

        /// <summary>
        /// 历史参数名，兼容旧配置中使用 MaxQty 表达单次上限的规则动作。
        /// </summary>
        public decimal? MaxQty { get; set; }

        /// <summary>
        /// 按兼容优先级解析最终可用的单次上限数量。
        /// </summary>
        /// <returns>优先返回 MaxOnceQty，其次 LimitQty，最后 MaxQty；三者都未配置时返回 null。</returns>
        /// <remarks>
        /// 规则中心经历过多版参数命名。这里不要求历史规则一次性清洗，而是在执行器入口统一兼容，
        /// 避免旧动作参数因为字段名不同被当作"未配置上限"直接跳过。
        /// </remarks>
        public decimal? GetMaxOnceQty()
        {
            if (MaxOnceQty.HasValue)
            {
                return MaxOnceQty.Value;
            }

            if (LimitQty.HasValue)
            {
                return LimitQty.Value;
            }

            return MaxQty;
        }
    }
}
