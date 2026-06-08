using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 按部位换算面积为数量的公式执行器（B-4 类规则）。
/// </summary>
/// <remarks>
/// <para>
/// 【业务场景】巨痣、血管瘤、脉管畸形、神经纤维瘤等肿物切除手术，
/// 计价以面积换算为数量为基础，不同身体部位的基准面积不同（头面部 4cm²，躯干 144cm² 等）。
/// 每个病灶独立换算、独立封顶，多个病灶的金额相加为最终金额。
/// 公式来源：12-历史规则分类与公式定义.md B-4 类。
/// </para>
/// <para>
/// 【公式定义】对 PricingParts 中每个病灶：
/// <code>
/// convertedQty  = ceil(area / baseAreaForPart)
/// lesionAmount  = unitPrice × convertedQty
/// lesionAmount  = min(lesionAmount, maxAmountPerLesion)   // 若配置了单肿物封顶
/// totalAmount   = Σ lesionAmount
/// </code>
/// </para>
/// <para>
/// 【数据来源】
/// <list type="bullet">
///   <item><description>面积：PricingPartItem.Area（cm²）</description></item>
///   <item><description>部位：PricingPartItem.BodyPartCode；为空时回落 context.BodyPartCode</description></item>
///   <item><description>基准面积：PartConvertRules 按部位编码查找，未匹配时用 DefaultBaseArea</description></item>
/// </list>
/// </para>
/// <para>
/// 【执行器编码二级分派】ActionType 为 FORMULA_CALC，ExecutorCode 为 CONVERT_QTY_BY_PART。
/// 其他 FORMULA_CALC 动作不匹配时静默跳过。
/// </para>
/// </remarks>
public sealed class ConvertQtyByPartExecutor : IRuleActionExecutor, IFormulaExecutorCapabilityMetadata
{
    /// <inheritdoc />
    public IReadOnlyCollection<string> SupportedExecutorCodes { get; } = new[]
    {
        FormulaExecutorCodes.ConvertQtyByPart,
        FormulaExecutorCodes.ConvertQtyByPartExecutor
    };

    /// <summary>
    /// 获取动作类型编码。与其他公式执行器共享 FORMULA_CALC，通过 ExecutorCode 做二级分派。
    /// </summary>
    public string ActionType => RuleActionTypeCodes.FormulaCalc;

    /// <summary>
    /// 判断当前公式动作是否应由按部位面积换算公式执行器处理。
    /// </summary>
    /// <param name="action">待执行的规则动作，ExecutorCode 必须是按部位换算公式编码或历史类名。</param>
    /// <returns>匹配按部位换算公式时返回 <c>true</c>，否则返回 <c>false</c>。</returns>
    public bool CanHandle(RuleAction action)
    {
        return FormulaExecutorCodes.IsConvertQtyByPart(action.ExecutorCode);
    }

    /// <summary>
    /// 执行按部位面积换算公式，把结果写回计价上下文。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>PartConvertRules — 部位换算规则数组（BodyPartCode + BaseArea）</description></item>
    ///   <item><description>DefaultBaseArea — 部位未匹配时的默认基准面积，默认 100</description></item>
    ///   <item><description>MaxAmountPerLesion — 单病灶封顶金额，null 表示不封顶</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，读取 UnitPrice 和 PricingParts，写入 FormulaAmount、FinalAmount 和 ConvertedQty。
    /// </param>
    /// <returns>已完成的异步任务（计算为纯内存操作，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：二级执行器编码过滤 ==========
        if (!CanHandle(action))
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：解析动作参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        var defaultBaseArea = param?.DefaultBaseArea > 0 ? param.DefaultBaseArea : 100m;
        var maxAmountPerLesion = param?.MaxAmountPerLesion;

        // ========== 第三阶段：处理无明细时的降级处理 ==========
        // PricingParts 为空时，说明调用方未提供病灶明细。
        // 此时无法按部位换算，静默跳过以避免阻断计价请求；调用方应在联调时补充 pricingParts。
        if (context.PricingParts is null || context.PricingParts.Count == 0)
        {
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "FORMULA",
                StepDesc = "按部位面积换算：PricingParts 为空，跳过换算（调用方未提供病灶明细）",
                InputValue = 0,
                OutputValue = context.FinalAmount,
                ParamsJson = action.ParamsJson
            });
            return Task.CompletedTask;
        }

        // ========== 第四阶段：按部位逐病灶换算 ==========
        var totalConvertedQty = 0m;
        var totalAmount = 0m;

        foreach (var part in context.PricingParts)
        {
            var area = part.Area ?? 0m;
            var bodyPartCode = !string.IsNullOrEmpty(part.BodyPartCode)
                ? part.BodyPartCode
                : context.BodyPartCode;

            // 查找该部位对应的基准面积，未匹配时用默认值。
            var baseArea = FindBaseArea(param?.PartConvertRules, bodyPartCode, defaultBaseArea);

            // 换算数量 = ceil(area / baseArea)，不足一个基准面积按一个计。
            // baseArea 为 0 时跳过该病灶，避免除零。
            if (baseArea <= 0m)
            {
                continue;
            }

            var convertedQty = Math.Ceiling(area / baseArea);
            var lesionAmount = context.UnitPrice * convertedQty;

            // 单病灶封顶：每个病灶独立封顶，多病灶相加。
            if (maxAmountPerLesion.HasValue && lesionAmount > maxAmountPerLesion.Value)
            {
                var beforeCap = lesionAmount;
                lesionAmount = maxAmountPerLesion.Value;

                context.TraceSteps.Add(new TraceStep
                {
                    StepNo = context.TraceSteps.Count + 1,
                    StepType = "FORMULA",
                    StepDesc = $"按部位换算-单病灶封顶：部位 {bodyPartCode ?? "未知"}，面积 {area}cm²，" +
                               $"基准 {baseArea}cm²，换算数量 {convertedQty}，金额 {beforeCap} 超过单病灶封顶 {maxAmountPerLesion.Value}，截断为 {lesionAmount}",
                    InputValue = beforeCap,
                    OutputValue = lesionAmount,
                    ParamsJson = action.ParamsJson
                });
            }

            totalConvertedQty += convertedQty;
            totalAmount += lesionAmount;
        }

        // ========== 第五阶段：写回上下文 ==========
        context.ConvertedQty = totalConvertedQty;
        context.FinalQty = totalConvertedQty;
        context.FormulaAmount = totalAmount;
        context.FinalAmount = totalAmount;

        context.TraceSteps.Add(new TraceStep
        {
            StepNo = context.TraceSteps.Count + 1,
            StepType = "FORMULA",
            StepDesc = $"按部位面积换算完成：共 {context.PricingParts.Count} 个病灶，" +
                       $"总换算数量 {totalConvertedQty}，总金额 {totalAmount}",
            InputValue = context.PricingParts.Count,
            OutputValue = totalAmount,
            ParamsJson = action.ParamsJson
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据部位编码查找对应的基准面积。
    /// </summary>
    /// <param name="rules">部位换算规则列表。</param>
    /// <param name="bodyPartCode">当前病灶的部位编码。</param>
    /// <param name="defaultBaseArea">未匹配时的默认基准面积。</param>
    /// <returns>该部位对应的基准面积。</returns>
    private static decimal FindBaseArea(
        List<PartConvertRule>? rules,
        string? bodyPartCode,
        decimal defaultBaseArea)
    {
        if (rules is null || rules.Count == 0 || string.IsNullOrEmpty(bodyPartCode))
        {
            return defaultBaseArea;
        }

        foreach (var rule in rules)
        {
            if (string.Equals(rule.BodyPartCode, bodyPartCode, StringComparison.OrdinalIgnoreCase))
            {
                return rule.BaseArea > 0 ? rule.BaseArea : defaultBaseArea;
            }
        }

        return defaultBaseArea;
    }

    /// <summary>
    /// 解析按部位换算公式参数。
    /// </summary>
    private static ConvertQtyByPartParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ConvertQtyByPartParams>(json);
    }

    /// <summary>
    /// 按部位换算公式参数模型。
    /// </summary>
    private sealed class ConvertQtyByPartParams
    {
        /// <summary>
        /// 部位换算规则列表，每条规则指定一个部位编码及其对应的基准面积。
        /// 典型配置：HEAD_FACE=4cm²，TRUNK=144cm²。
        /// </summary>
        public List<PartConvertRule>? PartConvertRules { get; set; }

        /// <summary>
        /// 部位未匹配时的默认基准面积（cm²），默认 100。
        /// </summary>
        public decimal DefaultBaseArea { get; set; }

        /// <summary>
        /// 单病灶封顶金额（元）。null 表示不封顶，每个病灶独立适用，多病灶相加后为总金额。
        /// </summary>
        public decimal? MaxAmountPerLesion { get; set; }
    }

    /// <summary>
    /// 单条部位换算规则，指定某个部位的基准面积。
    /// </summary>
    private sealed class PartConvertRule
    {
        /// <summary>
        /// 身体部位编码，如 HEAD_FACE、TRUNK、LIMBS 等。大小写不敏感。
        /// </summary>
        public string? BodyPartCode { get; set; }

        /// <summary>
        /// 该部位的基准面积（cm²），换算数量 = ceil(实际面积 / BaseArea)。
        /// </summary>
        public decimal BaseArea { get; set; }
    }
}
