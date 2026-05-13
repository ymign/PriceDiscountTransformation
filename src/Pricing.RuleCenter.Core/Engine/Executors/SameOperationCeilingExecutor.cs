using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 同手术封顶动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】同一手术下，同项目组的累计金额不超过封顶值。
/// 例如：某手术的材料费组封顶 50000 元，单条材料封顶 5000 元。
/// 超出封顶值的部分金额归零（不是整单归零，不是拒单）。
/// </para>
/// <para>
/// 【执行顺序】在全局动作排序中，SAME_OPERATION_CEILING 排在公式计算（FORMULA_CALC）
/// 之后、超额归零（DISCOUNT_EXCEED_TO_ZERO）之前。先算金额，再做封顶截断。
/// </para>
/// <para>
/// 【封顶层次】
/// <list type="bullet">
///   <item><description>ceilingPerItem — 单条项目金额上限，先检查</description></item>
///   <item><description>ceilingPerOperation — 同手术同组累计金额上限，后检查</description></item>
/// </list>
/// 两级封顶独立生效：单条超 ceilingPerItem 时截断到 ceilingPerItem，
/// 累计超 ceilingPerOperation 时截断到剩余可收费额度。
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>超出 = 0元：不是拒单，不是整单归零，仅超出部分为 0 元</description></item>
///   <item><description>NULL ≠ 0：NULL 表示不校验，0 表示限制为零</description></item>
///   <item><description>金额取整不在本执行器处理，最终由 PricingEngine 统一保留 2 位小数、四舍五入</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class SameOperationCeilingExecutor : IRuleActionExecutor
{
    private const string LimitType = "SAME_OPERATION";
    private static readonly string[] OccupyStatuses = { "PENDING", "CONFIRMED" };

    private readonly ILimitOccupyRepository _limitRepository;

    public SameOperationCeilingExecutor(ILimitOccupyRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    /// <summary>
    /// 获取动作类型编码，对应规则动作中的同手术封顶动作。
    /// </summary>
    public string ActionType => "SAME_OPERATION_CEILING";

    /// <summary>
    /// 执行同手术封顶判断，超出封顶值的金额截断。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>ceilingPerOperation — 同手术同组累计金额上限</description></item>
    ///   <item><description>ceilingPerItem — 单条项目金额上限</description></item>
    ///   <item><description>operationIdKey — ExtraParams 中手术标识的键名，默认 operationId</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>ExtraParams — 扩展参数字典，包含手术标识</description></item>
    ///   <item><description>FinalAmount — 当前金额（本执行器可能截断）</description></item>
    ///   <item><description>FinalQty — 当前数量（本执行器可能归零）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务（封顶判断为纯内存操作，无 IO）。</returns>
    public async Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析封顶参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return;
        }

        var ceilingPerItem = param.CeilingPerItem;
        var ceilingPerOperation = param.CeilingPerOperation;
        var operationIdKey = !string.IsNullOrEmpty(param.OperationIdKey)
            ? param.OperationIdKey
            : "operationNo";

        // ========== 第二阶段：检查单条金额上限 ==========
        // ceilingPerItem 限制单条项目的最高金额。
        // NULL 表示不校验（不截断），0 表示限制为零（免费）。
        if (ceilingPerItem.HasValue && context.FinalAmount > ceilingPerItem.Value)
        {
            var beforeAmount = context.FinalAmount;
            context.FinalAmount = ceilingPerItem.Value;

            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = $"同手术封顶-单条截断：金额 {beforeAmount} 超过单条上限 {ceilingPerItem.Value}，截断为 {context.FinalAmount}",
                InputValue = beforeAmount,
                OutputValue = context.FinalAmount,
                ParamsJson = action.ParamsJson
            });
        }

        // ========== 第三阶段：检查同手术同组累计金额上限 ==========
        // ceilingPerOperation 限制同一手术下同项目组的累计金额。
        // NULL 表示不校验，0 表示限制为零（该手术下该组不收费）。
        if (!ceilingPerOperation.HasValue)
        {
            return;
        }

        // ========== 第四阶段：获取手术标识并加锁 ==========
        // 手术标识来源优先使用规则配置的 operationIdKey，未配置时使用 HIS 推荐字段 operationNo。
        var operationId = GetExtraParam(context, operationIdKey);
        if (string.IsNullOrEmpty(operationId) && !string.Equals(operationIdKey, "operationNo", StringComparison.OrdinalIgnoreCase))
        {
            operationId = GetExtraParam(context, "operationNo");
        }

        if (string.IsNullOrEmpty(operationId) && !string.Equals(operationIdKey, "operationId", StringComparison.OrdinalIgnoreCase))
        {
            operationId = GetExtraParam(context, "operationId");
        }

        if (string.IsNullOrEmpty(operationId))
        {
            // 缺少手术标识时不能执行同手术封顶校验。
            // 这里不抛异常阻断整条计价链路，而是记录追踪步骤后静默跳过。
            // 原因：非手术场景（如门诊收费）的项目也可能配置了同手术封顶规则，
            // 但请求中不会携带手术标识。强制抛异常会导致这些场景全部失败。
            // 正确做法是规则条件中配置 CHARGE_SCENE_MATCH 限定手术场景，
            // 让非手术场景的项目不命中该规则。
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = $"同手术封顶跳过：请求未携带手术标识(operationNo/operationId)，无法执行同手术封顶校验",
                InputValue = context.FinalAmount,
                OutputValue = context.FinalAmount,
                ParamsJson = action.ParamsJson
            });
            return;
        }

        var groupCode = context.ItemGroupCode ?? context.ItemCode;
        var limitKey = LimitKeyGenerator.SameOperationKey(operationId, groupCode);
        var dimensionCode = $"{operationId}:{groupCode}".ToUpperInvariant();

        if (context.ShouldLockLimits)
        {
            await _limitRepository.EnsureAndLockAsync(new[] { limitKey });
        }

        var historicalAmount = 0m;
        foreach (var status in OccupyStatuses)
        {
            historicalAmount += await _limitRepository.GetOccupiedAmtAsync(limitKey, status);
        }

        historicalAmount += GetInRequestOccupiedAmount(context, dimensionCode);

        var remainingAmount = ceilingPerOperation.Value - historicalAmount;

        // ========== 第五阶段：超出累计封顶时截断 ==========
        // 剩余额度 <= 0 时，当前项目金额归零。
        // 剩余额度 > 0 但小于当前金额时，截断到剩余额度。
        // 这是"超出 = 0元"的实现——仅超出部分为 0 元，不是整单归零。
        if (remainingAmount <= 0)
        {
            var beforeAmount = context.FinalAmount;
            context.FinalAmount = 0;
            context.FinalQty = 0;

            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = $"同手术封顶-累计归零：手术 {operationId} 下项目组 {groupCode} " +
                           $"历史累计 {historicalAmount} 已达上限 {ceilingPerOperation.Value}，金额归零",
                InputValue = beforeAmount,
                OutputValue = 0,
                ParamsJson = action.ParamsJson
            });

            AddOccupyDraft(context, limitKey, dimensionCode);
            return;
        }

        if (context.FinalAmount > remainingAmount)
        {
            var beforeAmount = context.FinalAmount;
            context.FinalAmount = remainingAmount;

            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = $"同手术封顶-累计截断：手术 {operationId} 下项目组 {groupCode} " +
                           $"历史累计 {historicalAmount}，剩余额度 {remainingAmount}，" +
                           $"金额从 {beforeAmount} 截断为 {context.FinalAmount}",
                InputValue = beforeAmount,
                OutputValue = context.FinalAmount,
                ParamsJson = action.ParamsJson
            });
        }

        AddOccupyDraft(context, limitKey, dimensionCode);
    }

    private static decimal GetInRequestOccupiedAmount(PricingContext context, string dimensionCode)
    {
        var candidates = context.InRequestLimitOccupies
            .Where(o => string.Equals(o.LimitType, LimitType, StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (candidates.Count > 0)
        {
            return candidates.Sum(o => o.OccupyAmt);
        }

        var opCeilingKey = $"OP_CEILING:{dimensionCode}".ToUpperInvariant();
        return context.InRequestOccupiedQtyByLimitDimension.TryGetValue(opCeilingKey, out var cachedAmount)
            ? cachedAmount
            : 0m;
    }

    private static void AddOccupyDraft(PricingContext context, string limitKey, string dimensionCode)
    {
        if (context.PendingLimitOccupies.Any(o =>
                string.Equals(o.LimitType, LimitType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(o.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        context.PendingLimitOccupies.Add(new LimitOccupy
        {
            PatientId = context.PatientId,
            ItemCode = context.ItemCode,
            LimitType = LimitType,
            LimitKey = limitKey,
            LimitDimensionCode = dimensionCode,
            BusinessChargeTime = context.BusinessChargeTime,
            OccupyType = "CHARGE"
        });
    }

    /// <summary>
    /// 从计价上下文的扩展参数中获取指定键的值。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="key">参数键名。</param>
    /// <returns>参数值；键不存在或字典为空时返回空字符串。</returns>
    private static string GetExtraParam(PricingContext context, string key)
    {
        if (context.ExtraParams is null || context.ExtraParams.Count == 0)
        {
            return string.Empty;
        }

        return context.ExtraParams.TryGetValue(key, out var value) ? value ?? string.Empty : string.Empty;
    }

    /// <summary>
    /// 解析同手术封顶参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static SameOperationCeilingParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<SameOperationCeilingParams>(json);
    }

    /// <summary>
    /// 同手术封顶参数模型。
    /// </summary>
    private sealed class SameOperationCeilingParams
    {
        /// <summary>
        /// 同手术同项目组的累计金额上限。
        /// NULL 表示不校验（不截断），0 表示限制为零（该手术下该组不收费）。
        /// 精度 NUMBER(18,4)，单位：元。
        /// </summary>
        public decimal? CeilingPerOperation { get; set; }

        /// <summary>
        /// 单条项目金额上限。
        /// NULL 表示不校验，0 表示限制为零。
        /// 先于 CeilingPerOperation 检查。
        /// </summary>
        public decimal? CeilingPerItem { get; set; }

        /// <summary>
        /// ExtraParams 中手术标识的键名。
        /// 默认为 "operationId"。调用方可通过该字段自定义键名。
        /// </summary>
        public string? OperationIdKey { get; set; }
    }
}
