using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// 双单位换算动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】将调用方输入的数量（如面积 CM2）换算为计价数量（如次数 COUNT）。
/// 例如：皮肤科激光项目按面积收费，输入为 CM2，换算规则为"每 4 CM2 算 1 次"，
/// 则 10 CM2 换算为 ⌈10 / 4⌉ = 3 次。
/// </para>
/// <para>
/// 【执行顺序】在全局动作排序中，CONVERT_QTY 排在数量限制和 FORMULA_CALC 之前。
/// 本执行器会把换算结果同时写入 ConvertedQty 和 FinalQty；后续若没有数量限制，
/// 公式读取到的 FinalQty 就等于换算后数量；若有限制，公式读取限制后的 FinalQty。
/// </para>
/// <para>
/// 【部位匹配】同一项目允许按不同部位维护不同换算规则（如头部/面部 4 CM2 = 1 次，
/// 躯干 144 CM2 = 1 次）。执行器按 BodyPartCode 优先匹配，无匹配时使用默认除数。
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>换算数量固定为1：这里是"每 1 个计价单位需要多少输入单位"（除数）</description></item>
///   <item><description>公式读取 FinalQty：无数量限制时等于换算后数量，有限制时等于限制后的可收费数量</description></item>
///   <item><description>中间计算保留全精度（decimal），不提前取整</description></item>
///   <item><description>NULL ≠ 0：NULL 表示不校验，0 表示限制为零</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class UnitConvertExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的双单位换算动作。
    /// </summary>
    public string ActionType => "CONVERT_QTY";

    /// <summary>
    /// 执行双单位换算，将输入数量换算为计价数量并写入上下文。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>convertRules — 按部位编码匹配的换算规则数组</description></item>
    ///   <item><description>defaultDivisor — 无匹配时的默认除数，默认为 1</description></item>
    ///   <item><description>defaultRoundMode — 无匹配时的默认取整模式，默认为 CEILING</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>InputQty — 调用方录入的原始数量</description></item>
    ///   <item><description>BodyPartCode — 身体部位编码，用于匹配换算规则</description></item>
    ///   <item><description>ConvertedQty — 换算后数量（输出）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务（换算为纯内存计算，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析换算参数 ==========
        // 参数为空时静默跳过——缺失参数应在发布校验阶段发现，运行期不阻断请求。
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：按部位匹配换算规则 ==========
        // 同一项目允许按不同部位维护不同换算规则。优先按 BodyPartCode 精确匹配，
        // 无匹配时使用默认除数。匹配逻辑不区分大小写，降低配置中大小写差异带来的运行风险。
        var divisor = param.DefaultDivisor > 0 ? param.DefaultDivisor : 1m;
        var roundMode = !string.IsNullOrEmpty(param.DefaultRoundMode)
            ? param.DefaultRoundMode
            : "CEILING";

        if (param.ConvertRules != null && param.ConvertRules.Count > 0 &&
            !string.IsNullOrEmpty(context.BodyPartCode))
        {
            var matchedRule = param.ConvertRules.FirstOrDefault(r =>
                !string.IsNullOrEmpty(r.BodyPartCode) &&
                string.Equals(r.BodyPartCode, context.BodyPartCode, StringComparison.OrdinalIgnoreCase));

            if (matchedRule != null)
            {
                // 匹配到部位规则时，覆盖默认除数和取整模式。
                divisor = matchedRule.Divisor > 0 ? matchedRule.Divisor : divisor;
                if (!string.IsNullOrEmpty(matchedRule.RoundMode))
                {
                    roundMode = matchedRule.RoundMode;
                }
            }
        }

        // ========== 第三阶段：执行换算 ==========
        // 公式：换算数量 = ⌈输入数量 / divisor⌉（向上取整）
        // 向上取整保证不足 1 个计价单位的部分也按 1 个计价单位收费。
        // 例如：10 CM2 / 4 = 2.5，向上取整 = 3 次。
        // 中间计算保留全精度（decimal），不提前取整。
        var beforeQty = context.ConvertedQty > 0 ? context.ConvertedQty : context.InputQty;
        decimal convertedQty;

        if (divisor <= 0)
        {
            // 除数为 0 或负数是配置错误，防御性处理：直接使用输入数量。
            convertedQty = context.InputQty;
        }
        else
        {
            var rawQty = context.InputQty / divisor;
            convertedQty = ApplyRoundMode(rawQty, roundMode);
        }

        // ========== 第四阶段：写入换算结果 ==========
        // ConvertedQty 供后续公式执行器（FORMULA_CALC）使用。
        // FinalQty 也同步更新，供后续限额动作使用。金额必须同步按换算后的
        // 计价数量重算，否则只有换算动作、没有公式动作的项目会继续按原始数量收费。
        context.ConvertedQty = convertedQty;
        context.FinalQty = convertedQty;
        context.FinalAmount = context.UnitPrice * convertedQty;

        // ========== 第五阶段：记录追踪步骤 ==========
        // 追踪步骤记录换算前后的数量，便于追溯页面展示换算过程。
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = context.TraceSteps.Count + 1,
            StepType = "CONVERT",
            StepDesc = $"双单位换算：输入数量 {context.InputQty} ÷ 除数 {divisor}，取整模式 {roundMode}，换算后数量 {convertedQty}",
            InputValue = beforeQty,
            OutputValue = convertedQty,
            ParamsJson = action.ParamsJson
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// 按取整模式对换算结果取整。
    /// </summary>
    /// <param name="value">原始换算值。</param>
    /// <param name="roundMode">取整模式：CEILING（向上）、FLOOR（向下）、ROUND（四舍五入）。</param>
    /// <returns>取整后的值。</returns>
    private static decimal ApplyRoundMode(decimal value, string roundMode)
    {
        return roundMode.ToUpperInvariant() switch
        {
            // 向上取整：不足 1 个计价单位的部分也按 1 个收费。
            "CEILING" => Math.Ceiling(value),
            // 向下取整：不足 1 个计价单位的部分不收费。
            "FLOOR" => Math.Floor(value),
            // 四舍五入：不足 0.5 按 0 收费，0.5 及以上按 1 收费。
            "ROUND" => Math.Round(value, MidpointRounding.AwayFromZero),
            // 默认向上取整，与业务惯例一致。
            _ => Math.Ceiling(value)
        };
    }

    /// <summary>
    /// 解析双单位换算参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static UnitConvertParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return RuleCenterJsonSerializer.Deserialize<UnitConvertParams>(json);
    }

    /// <summary>
    /// 双单位换算参数模型。
    /// </summary>
    private sealed class UnitConvertParams
    {
        /// <summary>
        /// 按部位编码匹配的换算规则数组。
        /// 每条规则指定一个 BodyPartCode 及其对应的 Divisor 和 RoundMode。
        /// 同一项目允许按不同部位维护不同换算规则。
        /// </summary>
        public List<ConvertRule>? ConvertRules { get; set; }

        /// <summary>
        /// 无部位匹配时的默认除数。
        /// NULL 或 0 时默认为 1（即不换算，输入数量 = 计价数量）。
        /// </summary>
        public decimal DefaultDivisor { get; set; }

        /// <summary>
        /// 无部位匹配时的默认取整模式。
        /// 支持 CEILING（向上）、FLOOR（向下）、ROUND（四舍五入）。
        /// 默认为 CEILING。
        /// </summary>
        public string? DefaultRoundMode { get; set; }
    }

    /// <summary>
    /// 单条部位换算规则。
    /// </summary>
    private sealed class ConvertRule
    {
        /// <summary>
        /// 身体部位编码，用于与 PricingContext.BodyPartCode 匹配。
        /// 例如 HEAD_FACE、TRUNK、LIMB 等。
        /// </summary>
        public string? BodyPartCode { get; set; }

        /// <summary>
        /// 输入单位，例如 CM2、CM。仅用于描述和审计，不参与计算。
        /// </summary>
        public string? InputUnit { get; set; }

        /// <summary>
        /// 输出单位，例如 COUNT、次。仅用于描述和审计，不参与计算。
        /// </summary>
        public string? OutputUnit { get; set; }

        /// <summary>
        /// 换算除数。输入数量 ÷ 除数 = 计价数量。
        /// 例如：头部/面部面积除数为 4，表示每 4 CM2 算 1 次。
        /// 必须大于 0，否则该规则无效。
        /// </summary>
        public decimal Divisor { get; set; }

        /// <summary>
        /// 取整模式：CEILING（向上）、FLOOR（向下）、ROUND（四舍五入）。
        /// 默认为 CEILING。
        /// </summary>
        public string? RoundMode { get; set; }
    }
}
