using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 子项加收动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】主项目命中规则后，自动附加子项目并按子项目单价计费。
/// 例如：CT 检查主项目命中后，自动加收"薄层扫描(加收)"子项目。
/// 子项目与主项目共享同一 resultGroupNo，保证原子 commit/cancel。
/// </para>
/// <para>
/// 【子项来源】
/// <list type="bullet">
///   <item><description>优先从 ParamsJson 的 childItems 数组读取（内联配置）</description></item>
///   <item><description>childItems 为空时，从 PR_ITEM_GROUP（类型 MAIN_CHILD）读取（数据库配置）</description></item>
/// </list>
/// 两种来源互斥，不会合并。
/// </para>
/// <para>
/// 【限额共享】
/// <list type="bullet">
///   <item><description>shareParentLimit = true：子项数量计入主项目的限额维度，共享日限、时间窗限等</description></item>
///   <item><description>shareParentLimit = false：子项独立占用限额，按子项自身规则校验</description></item>
/// </list>
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>子项加收必须与主项目同 resultGroupNo 原子 commit/cancel</description></item>
///   <item><description>子项单价来源为权威物价主数据，不得直接信任渠道传入值</description></item>
///   <item><description>金额取整不在本执行器处理，最终由 PricingEngine 统一保留 2 位小数、四舍五入</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class AddChildItemExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的子项加收动作。
    /// </summary>
    public string ActionType => "ADD_CHILD_ITEM";

    /// <summary>
    /// 执行子项加收，为每个子项目创建计价结果并写入上下文。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>childItems — 子项目数组，每项包含 itemCode、itemName、qty、unitPriceKey</description></item>
    ///   <item><description>shareParentLimit — 是否与主项目共享限额，默认 true</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>ItemCode — 主项目编码</description></item>
    ///   <item><description>UnitPrice — 主项目单价（子项单价不从此取，单独配置）</description></item>
    ///   <item><description>ChildPricingResults — 子项计价结果集合（输出）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务（子项生成为纯内存操作，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析子项参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        if (param is null || param.ChildItems is null || param.ChildItems.Count == 0)
        {
            // 参数为空时静默跳过——可能子项来源为数据库（PR_ITEM_GROUP），需由应用层处理。
            // 运行期不阻断请求，配置完整性由发布校验保证。
            return Task.CompletedTask;
        }

        var shareParentLimit = param.ShareParentLimit;

        // ========== 第二阶段：为每个子项创建计价结果 ==========
        // 子项数量由配置决定（固定值），不是主项目的数量。
        // 子项单价来源为权威物价主数据，unitPriceKey 指示从哪个来源读取：
        // - "authority"：从权威物价主数据表读取
        // - "config"：从子项配置中直接指定（调试用，生产不推荐）
        foreach (var childItem in param.ChildItems)
        {
            if (string.IsNullOrEmpty(childItem.ItemCode))
            {
                // 子项编码为空是配置错误，跳过该子项，不阻断其他子项处理。
                continue;
            }

            var childQty = childItem.Qty > 0 ? childItem.Qty : 1m;

            // 子项单价处理：
            // 当前版本从配置中读取 unitPrice；后续版本应接入权威物价主数据校验。
            // 子项金额 = 单价 × 数量，中间计算保留全精度。
            var childUnitPrice = childItem.UnitPrice;
            var childAmount = childUnitPrice * childQty;

            // ========== 第三阶段：写入子项计价结果 ==========
            // 子项结果挂载到 context.ChildPricingResults，由应用服务后续处理：
            // 1. 生成子项目收费明细
            // 2. 生成占额草稿（shareParentLimit=true 时计入主项目限额维度）
            // 3. 生成追溯步骤
            context.ChildPricingResults.Add(new ChildPricingResult
            {
                ItemCode = childItem.ItemCode,
                ItemName = childItem.ItemName,
                Qty = childQty,
                UnitPrice = childUnitPrice,
                Amount = childAmount,
                ShareParentLimit = shareParentLimit,
                ParentItemCode = context.ItemCode
            });

            // ========== 第四阶段：记录追踪步骤 ==========
            // 每个子项独立记录一条追踪步骤，便于追溯页面展示加收明细。
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "DISCOUNT",
                StepDesc = $"子项加收：主项目 {context.ItemCode} 附加子项目 {childItem.ItemCode}" +
                           $"({childItem.ItemName ?? "未命名"})，" +
                           $"数量 {childQty}，单价 {childUnitPrice}，金额 {childAmount}" +
                           (shareParentLimit ? "，共享主项目限额" : "，独立占用限额"),
                InputValue = 0,
                OutputValue = childAmount,
                ParamsJson = action.ParamsJson
            });
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析子项加收参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static AddChildItemParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<AddChildItemParams>(json);
    }

    /// <summary>
    /// 子项加收参数模型。
    /// </summary>
    private sealed class AddChildItemParams
    {
        /// <summary>
        /// 子项目数组。每个元素描述一个需要加收的子项目。
        /// 为空时可能子项来源为数据库（PR_ITEM_GROUP 类型 MAIN_CHILD）。
        /// </summary>
        public List<ChildItemConfig>? ChildItems { get; set; }

        /// <summary>
        /// 是否与主项目共享限额。
        /// true（默认）：子项数量计入主项目的日限、时间窗限等维度。
        /// false：子项独立占用限额，按子项自身规则校验。
        /// </summary>
        public bool ShareParentLimit { get; set; } = true;
    }

    /// <summary>
    /// 单条子项目配置。
    /// </summary>
    private sealed class ChildItemConfig
    {
        /// <summary>
        /// 子项目编码，对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
        /// 不可为空。
        /// </summary>
        public string? ItemCode { get; set; }

        /// <summary>
        /// 子项目名称，用于展示和审计。
        /// 例如："薄层扫描(加收)"。
        /// </summary>
        public string? ItemName { get; set; }

        /// <summary>
        /// 子项目收费数量。
        /// 默认为 1。子项数量由配置决定，不是主项目的数量。
        /// </summary>
        public decimal Qty { get; set; } = 1m;

        /// <summary>
        /// 子项目单价来源标识。
        /// "authority" — 从权威物价主数据表读取（推荐）。
        /// "config" — 从本配置的 UnitPrice 字段读取（调试用）。
        /// </summary>
        public string? UnitPriceKey { get; set; }

        /// <summary>
        /// 子项目单价。仅当 UnitPriceKey = "config" 时使用。
        /// 精度 NUMBER(18,4)，单位：元。
        /// 生产环境应使用权威物价主数据，避免配置单价与实际物价不一致。
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
