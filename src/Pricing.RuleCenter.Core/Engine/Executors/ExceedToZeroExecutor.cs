using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 可配置的超限动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】当收费数量超出限额时，决定超出部分如何处理。
/// 通过 ParamsJson 配置超限后的行为模式，不改代码即可适配不同规则。
/// </para>
/// <para>
/// 【支持的超限模式】
/// <list type="bullet">
///   <item><description>ZERO — 超出部分归零（默认，兼容旧逻辑）。适用场景：床旁加收超限</description></item>
///   <item><description>REPLACE — 超出部分替换为加收项目，按加收项目单价收费。适用场景：CT/MR 2小时超部位限制</description></item>
///   <item><description>CEILING — 超出部分按封顶价收费。适用场景：单项目最高限价</description></item>
/// </list>
/// </para>
/// <para>
/// 【执行顺序】在全局动作排序中排在所有限额动作之后，
/// 确保先由限额动作（日限、时间窗限、单次限、互斥）截断数量，再由本执行器处理超出部分。
/// </para>
/// <para>
/// 【配置示例】
/// <code>
/// // ZERO 模式（默认，不配 ParamsJson 即为归零）
/// { "exceedAction": "ZERO" }
///
/// // REPLACE 模式（超出部分替换为加收项目）
/// {
///   "exceedAction": "REPLACE",
///   "replaceItemCode": "CT_FLAT_SURCHARGE",
///   "replaceItemName": "CT平扫（加收）",
///   "replaceUnitPrice": 150.00
/// }
///
/// // CEILING 模式（超出部分按封顶价）
/// {
///   "exceedAction": "CEILING",
///   "ceilingPrice": 440.00
/// }
/// </code>
/// </para>
/// </remarks>
public sealed class ExceedToZeroExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的超额处理动作。
    /// </summary>
    public string ActionType => "DISCOUNT_EXCEED_TO_ZERO";

    /// <summary>
    /// 根据配置的超限模式处理超出部分。
    /// </summary>
    /// <param name="action">规则动作配置，ParamsJson 中可配置 exceedAction、replaceItemCode 等参数。</param>
    /// <param name="context">计价上下文，包含当前数量、金额和原始输入数量。</param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：读取超限模式 ==========
        // 未配置 ParamsJson 时默认 ZERO 模式，兼容旧规则和不配置的场景。
        var param = DeserializeParams(action.ParamsJson);
        var exceedAction = param?.ExceedAction ?? "ZERO";

        // ========== 第二阶段：判断是否发生超限 ==========
        // 超限只看"限额动作是否把计价数量截短"。双单位换算会合法地让 FinalQty 小于 InputQty，
        // 因此这里必须以 ConvertedQty 为限额前基准，不能拿原始输入数量比较。
        var limitBaseQty = Math.Max(context.ConvertedQty, 0m);
        var allowedQty = Math.Max(context.FinalQty, 0m);

        if (limitBaseQty <= 0 || allowedQty >= limitBaseQty)
        {
            return Task.CompletedTask;
        }

        // ========== 第三阶段：按模式处理超出部分 ==========
        var exceedQty = limitBaseQty - allowedQty;
        context.ExceedQty = exceedQty;

        switch (exceedAction.ToUpperInvariant())
        {
            case "REPLACE":
                // REPLACE 和 CEILING 模式需要参数配置，param 为 null 时降级为 ZERO 模式。
                if (param is null)
                {
                    context.TraceSteps.Add(new TraceStep
                    {
                        StepNo = context.TraceSteps.Count + 1,
                        StepType = "LIMIT",
                        StepDesc = "超限处理降级：REPLACE 模式缺少 ParamsJson 配置，降级为 ZERO 模式",
                        InputValue = context.FinalAmount,
                        OutputValue = context.FinalAmount,
                        ParamsJson = action.ParamsJson
                    });
                    HandleZero(context);
                }
                else
                {
                    HandleReplace(context, param, exceedQty);
                }
                break;

            case "CEILING":
                // CEILING 模式需要 ceilingPrice 参数，param 为 null 时降级为 ZERO 模式。
                if (param is null)
                {
                    context.TraceSteps.Add(new TraceStep
                    {
                        StepNo = context.TraceSteps.Count + 1,
                        StepType = "LIMIT",
                        StepDesc = "超限处理降级：CEILING 模式缺少 ParamsJson 配置，降级为 ZERO 模式",
                        InputValue = context.FinalAmount,
                        OutputValue = context.FinalAmount,
                        ParamsJson = action.ParamsJson
                    });
                    HandleZero(context);
                }
                else
                {
                    HandleCeiling(context, param, exceedQty);
                }
                break;

            default: // ZERO
                HandleZero(context);
                break;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// ZERO 模式：超出部分金额归零。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    private static void HandleZero(PricingContext context)
    {
        // 数量限额执行器已经按截断比例处理过 FinalAmount。这里不能再按比例缩放，
        // 否则部分超限会被重复打折；只对防御性全限额场景确保金额为 0。
        if (context.FinalQty <= 0)
        {
            context.FinalAmount = 0;
        }
    }

    /// <summary>
    /// REPLACE 模式：超出部分替换为加收项目，按加收项目单价收费。
    /// </summary>
    /// <remarks>
    /// 该模式保留原有项目的正常收费数量和金额，超出部分按加收项目单独计算。
    /// 最终金额 = 正常部分金额 + 加收部分金额。
    /// 加收项目信息通过 ReplaceChildResult 输出，由 PricingApiService 写入折价明细。
    /// </remarks>
    /// <param name="context">计价上下文。</param>
    /// <param name="param">超限参数配置。</param>
    /// <param name="exceedQty">超出数量。</param>
    private static void HandleReplace(PricingContext context, ExceedParams param, decimal exceedQty)
    {
        // 加收部分金额：超出数量 × 加收项目单价。
        var replaceUnitPrice = param.ReplaceUnitPrice ?? context.UnitPrice;
        var replaceAmount = exceedQty * replaceUnitPrice;

        // FinalAmount 已是前置限额动作保留下来的正常部分金额，这里只追加替换项目金额。
        context.FinalAmount += replaceAmount;

        // 输出加收项目信息，供追溯和折价明细使用。
        context.ReplaceChildResult = new ReplaceChildResult
        {
            ItemCode = param.ReplaceItemCode ?? string.Empty,
            ItemName = param.ReplaceItemName ?? string.Empty,
            Qty = exceedQty,
            UnitPrice = replaceUnitPrice,
            Amount = replaceAmount
        };
    }

    /// <summary>
    /// CEILING 模式：超出部分按封顶价收费。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="param">超限参数配置。</param>
    /// <param name="exceedQty">超出数量。</param>
    private static void HandleCeiling(PricingContext context, ExceedParams param, decimal exceedQty)
    {
        var ceilingPrice = param.CeilingPrice ?? 0;

        // 超出部分按封顶价，但不超过正常单价。
        var effectivePrice = Math.Min(ceilingPrice, context.UnitPrice);
        var exceedAmount = exceedQty * effectivePrice;

        // FinalAmount 已是前置限额动作保留下来的正常部分金额，这里只追加超出部分封顶金额。
        context.FinalAmount += exceedAmount;
    }

    /// <summary>
    /// 解析超限动作参数。
    /// </summary>
    /// <param name="json">动作参数 JSON。</param>
    /// <returns>解析后的参数对象；为空时返回 null（使用默认 ZERO 模式）。</returns>
    private static ExceedParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ExceedParams>(json);
    }

    /// <summary>
    /// 超限动作参数。
    /// </summary>
    private sealed class ExceedParams
    {
        /// <summary>
        /// 超限后的行为模式：ZERO（归零）、REPLACE（替换为加收项目）、CEILING（按封顶价）。
        /// 默认 ZERO。
        /// </summary>
        public string ExceedAction { get; set; } = "ZERO";

        /// <summary>
        /// REPLACE 模式：加收项目编码。
        /// </summary>
        public string? ReplaceItemCode { get; set; }

        /// <summary>
        /// REPLACE 模式：加收项目名称。
        /// </summary>
        public string? ReplaceItemName { get; set; }

        /// <summary>
        /// REPLACE 模式：加收项目单价。为空时使用原项目单价。
        /// </summary>
        public decimal? ReplaceUnitPrice { get; set; }

        /// <summary>
        /// CEILING 模式：封顶单价。
        /// </summary>
        public decimal? CeilingPrice { get; set; }
    }
}
