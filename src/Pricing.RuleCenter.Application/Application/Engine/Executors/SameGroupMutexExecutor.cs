using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 同组互斥动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】同组内同类项目在最近 2 小时窗口内只允许收费指定数量，超出部分金额归零。
/// 该口径对齐旧 HIS 的 RestrictingfeeZT / RestrictingfeeTX1 SQL：
/// 历史收费只向前看 2 小时，而不是按自然日整天互斥。
/// </para>
/// <para>
/// 【执行顺序】在全局动作排序中，SAME_GROUP_MUTEX 排在限额动作之后、
/// DISCOUNT_EXCEED_TO_ZERO 之前。先由互斥动作截断数量，再由超额归零同步金额。
/// </para>
/// <para>
/// 【互斥组维度】通过 groupDimension 参数指定互斥维度：
/// <list type="bullet">
///   <item><description>ITEM_GROUP — 按项目组编码互斥（默认）</description></item>
///   <item><description>EXCLUSIVE_GROUP — 按规则动作的 ExclusiveGroup 互斥</description></item>
/// </list>
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>超出 = 0元：不是拒单，不是整单归零，仅超出部分为 0 元</description></item>
///   <item><description>FinalQty 为 0 时金额归零（由后续 DISCOUNT_EXCEED_TO_ZERO 同步）</description></item>
///   <item><description>同组互斥只截断数量，金额归零由 ExceedToZeroExecutor 统一处理</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class SameGroupMutexExecutor : IRuleActionExecutor
{
    private const string LimitType = "SAME_GROUP";
    private static readonly string[] OccupyStatuses = { "PENDING", "CONFIRMED" };

    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 初始化同组互斥执行器。
    /// </summary>
    /// <remarks>
    /// 为兼容纯单元测试和未注入仓储的场景，保留无参构造并退化为只看请求内累计；
    /// 生产环境通过 DI 会优先使用带仓储构造，从而补齐跨 confirm 的历史互斥口径。
    /// </remarks>
    public SameGroupMutexExecutor() : this(new NullLimitOccupyRepository())
    {
    }

    /// <summary>
    /// 初始化同组互斥执行器。
    /// </summary>
    /// <param name="limitRepository">限额占用仓储，用于查询和锁定历史同组互斥占用。</param>
    public SameGroupMutexExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    /// <summary>
    /// 获取动作类型编码，对应规则动作中的同组互斥动作。
    /// </summary>
    public string ActionType => "SAME_GROUP_MUTEX";

    /// <summary>
    /// 执行同组互斥判断，超出配额的项目数量归零。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>groupDimension — 互斥维度，默认 ITEM_GROUP</description></item>
    ///   <item><description>maxCountPerGroup — 同组内允许计费的最大项目数，默认 1</description></item>
    ///   <item><description>windowMinutes — 历史互斥窗口分钟数，默认 120，兼容旧 HIS 2 小时窗口</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>ItemGroupCode — 当前项目所属的项目组编码</description></item>
    ///   <item><description>OrderedActions — 已排序的动作链，用于判断同组内已处理的项目</description></item>
    ///   <item><description>FinalQty — 最终数量（本执行器可能归零）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务。</returns>
    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析互斥参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        var maxCount = param?.MaxCountPerGroup > 0 ? param.MaxCountPerGroup : 1;
        var windowMinutes = param?.WindowMinutes > 0 ? param.WindowMinutes : 120;
        var groupDimension = !string.IsNullOrEmpty(param?.GroupDimension)
            ? param.GroupDimension
            : "ITEM_GROUP";

        // ========== 第二阶段：获取当前项目的互斥组标识 ==========
        // 按互斥维度确定当前项目属于哪个互斥组。
        // ITEM_GROUP 模式使用 PricingContext.ItemGroupCode；
        // EXCLUSIVE_GROUP 模式使用规则动作的 ExclusiveGroup 字段。
        var currentGroupKey = GetGroupKey(context, action, groupDimension);
        if (string.IsNullOrEmpty(currentGroupKey))
        {
            // 无法确定互斥组时静默跳过——可能该项目不属于任何互斥组。
            return;
        }

        // ========== 第三阶段：构造历史互斥维度并加锁 ==========
        // 新架构中同组互斥不只看同批内，还要看同一患者在同一互斥组内最近 2 小时已经收费的记录。
        // 这样才能对齐旧 HIS 的 getRestrictingfeeZT / RestrictingfeeTX1 行为。
        // 注意：旧 SQL 是 fee_date >= SYSDATE - (2/24)，不是“当天互斥”。
        var dimensionCode = $"{context.PatientId}:{currentGroupKey}"
            .ToUpperInvariant();
        var windowStart = context.BusinessChargeTime.AddMinutes(-windowMinutes);
        var windowEnd = context.BusinessChargeTime;
        var lockKeys = BuildLockKeys(context.PatientId, currentGroupKey, windowStart, windowEnd);

        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(lockKeys);
        }

        // ========== 第四阶段：汇总历史 + 同请求内已占用的互斥计数 ==========
        var processedCount = await _limitRepository.GetOccupiedQtyAsync(new Pricing.RuleCenter.Core.Aggregates.Quota.LimitOccupyRangeQuery
        {
            LimitType = LimitType,
            LimitDimensionCode = dimensionCode,
            StartTime = windowStart,
            EndTime = windowEnd,
            Statuses = OccupyStatuses
        });
        processedCount += SharedLimitStateReader.GetOccupiedQty(
            context,
            LimitType,
            dimensionCode,
            windowStart,
            windowEnd,
            RequestSharedStateKeys.BuildMutexKey(currentGroupKey));

        // ========== 第五阶段：超出配额时归零数量 ==========
        // 当同组内已处理的项目数 >= maxCountPerGroup 时，当前项目数量归零。
        // 归零逻辑只改 FinalQty，不改 FinalAmount——金额归零由后续 DISCOUNT_EXCEED_TO_ZERO 统一处理。
        // 这是"只负责判断，不负责同步"的设计，与 ExceedToZeroExecutor 配合。
        if (processedCount >= maxCount)
        {
            var beforeQty = context.FinalQty;
            context.FinalQty = 0;

            // 记录追踪步骤：解释为什么数量被归零。
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = $"同组互斥：项目组 {currentGroupKey} 在最近 {windowMinutes} 分钟内已处理 {processedCount} 个项目，" +
                           $"达到上限 {maxCount}，当前项目数量归零",
                InputValue = beforeQty,
                OutputValue = 0,
                ParamsJson = action.ParamsJson
            });

            return;
        }

        // ========== 第六阶段：当前项目通过互斥校验 ==========
        // 通过校验时不修改数量，仅记录追踪步骤说明互斥校验结果。
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = context.TraceSteps.Count + 1,
            StepType = "LIMIT",
            StepDesc = $"同组互斥校验通过：项目组 {currentGroupKey} 在最近 {windowMinutes} 分钟内已处理 {processedCount} 个项目，" +
                       $"未超过上限 {maxCount}",
            InputValue = context.FinalQty,
            OutputValue = context.FinalQty,
            ParamsJson = action.ParamsJson
        });

        LimitOccupyDraftAppender.AddDraft(
            context,
            LimitType,
            lockKeys[0],
            dimensionCode,
            requirePositiveFinalQty: true);
    }

    /// <summary>
    /// 根据互斥维度获取当前项目的互斥组标识。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="action">规则动作，用于获取 ExclusiveGroup。</param>
    /// <param name="groupDimension">互斥维度。</param>
    /// <returns>互斥组标识；无法确定时返回空字符串。</returns>
    private static string GetGroupKey(
        PricingContext context, RuleAction action, string groupDimension)
    {
        return groupDimension.ToUpperInvariant() switch
        {
            // 按项目组编码互斥：使用 PricingContext.ItemGroupCode。
            "ITEM_GROUP" => context.ItemGroupCode ?? string.Empty,
            // 按规则动作的 ExclusiveGroup 互斥：直接使用动作配置中的互斥组编码。
            "EXCLUSIVE_GROUP" => action.ExclusiveGroup ?? string.Empty,
            // 默认使用项目组编码。
            _ => context.ItemGroupCode ?? string.Empty
        };
    }

    /// <summary>
    /// 枚举同组互斥窗口覆盖到的全部小时桶锁键。
    /// </summary>
    /// <param name="patientId">患者标识。</param>
    /// <param name="groupKey">互斥组标识。</param>
    /// <param name="windowStart">窗口开始时间。</param>
    /// <param name="windowEnd">窗口结束时间。</param>
    /// <returns>需要按顺序锁定的锁键列表。</returns>
    private static List<string> BuildLockKeys(string patientId, string groupKey, DateTime windowStart, DateTime windowEnd)
    {
        var keys = new List<string>();
        var current = new DateTime(windowStart.Year, windowStart.Month, windowStart.Day, windowStart.Hour, 0, 0);
        var end = new DateTime(windowEnd.Year, windowEnd.Month, windowEnd.Day, windowEnd.Hour, 0, 0);

        while (current <= end)
        {
            keys.Add(LimitKeyGenerator.GenerateSameGroupWindowKey(patientId, groupKey, current).ToUpperInvariant());
            current = current.AddHours(1);
        }

        return keys;
    }

    /// <summary>
    /// 解析同组互斥参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static SameGroupMutexParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<SameGroupMutexParams>(json);
    }

    /// <summary>
    /// 同组互斥参数模型。
    /// </summary>
    private sealed class SameGroupMutexParams
    {
        /// <summary>
        /// 互斥维度编码。
        /// ITEM_GROUP — 按项目组编码互斥（默认）。
        /// EXCLUSIVE_GROUP — 按规则动作的 ExclusiveGroup 字段互斥。
        /// </summary>
        public string? GroupDimension { get; set; }

        /// <summary>
        /// 同组内允许计费的最大项目数。
        /// 默认为 1，即同组只允许一条项目计费。
        /// NULL 或 0 时默认为 1。
        /// </summary>
        public int MaxCountPerGroup { get; set; }

        /// <summary>
        /// 历史互斥窗口分钟数。
        /// 默认 120 分钟，用于对齐旧 HIS RestrictingfeeZT / RestrictingfeeTX1 的 2 小时窗口。
        /// </summary>
        public int WindowMinutes { get; set; } = 120;
    }

    private sealed class NullLimitOccupyRepository : ILimitOccupyRepository
    {
        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status)
        {
            return Task.FromResult(0m);
        }

        public Task<decimal> GetOccupiedQtyAsync(Pricing.RuleCenter.Core.Aggregates.Quota.LimitOccupyRangeQuery query)
        {
            return Task.FromResult(0m);
        }

        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status)
        {
            return Task.FromResult(0m);
        }

        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status)
        {
            return Task.FromResult(0m);
        }

        public Task<IReadOnlyList<Pricing.RuleCenter.Core.Aggregates.Quota.LimitOccupy>> GetByRequestIdAsync(long requestId)
        {
            return Task.FromResult<IReadOnlyList<Pricing.RuleCenter.Core.Aggregates.Quota.LimitOccupy>>(
                Array.Empty<Pricing.RuleCenter.Core.Aggregates.Quota.LimitOccupy>());
        }

        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys)
        {
            return Task.CompletedTask;
        }

        public Task<long> InsertAsync(Pricing.RuleCenter.Core.Aggregates.Quota.LimitOccupy entity)
        {
            throw new NotSupportedException();
        }

        public Task UpdateStatusAsync(long occupyId, string status)
        {
            throw new NotSupportedException();
        }

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            throw new NotSupportedException();
        }
    }
}
