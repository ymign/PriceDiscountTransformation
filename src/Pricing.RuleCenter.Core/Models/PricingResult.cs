using Pricing.RuleCenter.Core.Aggregates.Quota;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 计价引擎输出结果。
/// </summary>
/// <remarks>
/// <para>
/// 该类是统一计价引擎的输出载体，不直接对应数据库持久化实体。
/// 引擎只负责计算并返回该对象，由应用服务根据调用类型（SIMULATE / CONFIRM / COMMIT / CANCEL / REVERSE）
/// 决定是仅返回试算结果，还是把结果持久化并推进状态机。
/// </para>
/// <para>
/// 在计价链路中的角色：作为核心引擎的输出 DTO，串联 PricingResult → ChargeRequest（主表）
/// → ChargeDiscountDetail（折扣明细） → LimitOccupy（限额占用） → ChargeTraceStep（步骤日志）。
/// </para>
/// <para>
/// 金额约束：所有金额字段类型为 decimal（对应 Oracle NUMBER(18,4)），禁止使用 double 或 float。
/// 中间计算保留全部精度，最终对外金额保留 2 位小数、四舍五入。
/// </para>
/// </remarks>
public sealed class PricingResult
{
    /// <summary>
    /// 是否命中特殊计价规则。
    /// </summary>
    /// <remarks>
    /// true 表示该项目不是普通按单价乘数量直接收费，而是走公式折算、限额截断、超额归零等特殊逻辑。
    /// 该标识由计价引擎在规则匹配阶段设置，用于渠道层判断是否弹出特殊计价确认界面。
    /// "特殊项目"和"折价项目"是同一标识，仅命名不同。
    /// </remarks>
    public bool IsSpecialItem { get; set; }

    /// <summary>
    /// 调用方录入的原始数量。
    /// </summary>
    /// <remarks>
    /// 保留调用方传入的原始数量不变，用于与 FinalQty 对比展示折价过程。
    /// 金额计算前不得随意覆盖该字段。单位取决于项目配置（如 PART、CM2、EACH）。
    /// 多肿物、多部位项目应通过 pricingParts 明细表达，不能压成单个数量粗算。
    /// </remarks>
    public decimal InputQty { get; set; }

    /// <summary>
    /// 双单位换算后的计价数量。
    /// </summary>
    /// <remarks>
    /// 该值保留双单位换算后的数量，用于区分"换算导致数量变化"和"限额导致数量截断"。
    /// 若未配置换算规则，通常等于 InputQty。
    /// </remarks>
    public decimal ConvertedQty { get; set; }

    /// <summary>
    /// 最终可收费数量。
    /// </summary>
    /// <remarks>
    /// 经过双单位换算、公式计算、日数量限制、时间窗数量限制、同组互斥等全部处理后的最终数量。
    /// 若存在数量下限或上限，该值已在限制范围内。
    /// 若超出限制，超出部分为 0（不是整单归零，不是拒单）。
    /// 该字段为中间结果，最终金额由 FinalAmount 决定。
    /// </remarks>
    public decimal FinalQty { get; set; }

    /// <summary>
    /// 项目单价。
    /// </summary>
    /// <remarks>
    /// 当前计价结果沿用渠道传入的收费单价。
    /// 规则中心会按 HIS 物价主数据记录权威单价诊断日志，但不会因差异返回 PRICE_MISMATCH。
    /// 单位：元（人民币），精度 NUMBER(18,4)。
    /// </remarks>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 最终应返回给 HIS 的可收费金额。
    /// </summary>
    /// <remarks>
    /// 经过公式计算、金额上下限、数量限制等全部处理后的最终收费金额。
    /// 最终对外金额保留 2 位小数，四舍五入（roundMode 可配置）。
    /// 若项目超出限额，超出部分为 0 元，最终金额可能小于原始金额。
    /// 试算时不占用额度，仅确认计价（confirm）才占用。
    /// </remarks>
    public decimal FinalAmount { get; set; }

    /// <summary>
    /// 折价金额。
    /// </summary>
    /// <remarks>
    /// 通常等于原始金额（InputQty * UnitPrice）减最终金额（FinalAmount）。
    /// 若未命中折价规则，该值为 0。
    /// 用于 HIS 对账和审计，展示折价让利了多少。
    /// 精度保留 2 位小数，四舍五入。
    /// </remarks>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 超出限额的数量。
    /// </summary>
    public decimal ExceedQty { get; set; }

    /// <summary>
    /// REPLACE 模式下的替换子项信息。
    /// </summary>
    public ReplaceChildResult? ReplaceChildResult { get; set; }

    /// <summary>
    /// ADD_CHILD_ITEM 动作生成的普通子项目计价结果集合。
    /// </summary>
    public IReadOnlyList<ChildPricingResult> ChildPricingResults { get; set; } = Array.Empty<ChildPricingResult>();

    /// <summary>
    /// 本次计价的追踪步骤集合。
    /// </summary>
    /// <remarks>
    /// 按 StepNo 顺序排列，用于解释命中了哪些规则、执行了哪些动作、每一步的输入输出快照。
    /// 试算时返回给调用方用于前端展示，确认计价时持久化到 PR_CHARGE_TRACE_STEP 表。
    /// 步骤类型包括：MATCH（规则匹配）、CONVERT（双单位换算）、FORMULA（公式计算）、
    /// LIMIT（限额校验）、DISCOUNT（折价计算）、VALIDATE（校验）、ERROR（异常）。
    /// </remarks>
    public IReadOnlyList<TraceStep> TraceSteps { get; set; } = Array.Empty<TraceStep>();

    /// <summary>
    /// 本次计价命中的规则主键集合。
    /// </summary>
    /// <remarks>
    /// 记录本次计价过程中匹配到的所有 PR_RULE_HEADER.RULE_ID。
    /// 用于追溯查询和规则变更影响分析。
    /// 一条计价可能命中多条规则（如同时命中换算规则和折价规则）。
    /// </remarks>
    public IReadOnlyList<long> MatchedRuleIds { get; set; } = Array.Empty<long>();

    /// <summary>
    /// 本次计价生成的限额占用草稿或明细。
    /// </summary>
    /// <remarks>
    /// 试算时仅用于展示本次计价会占用哪些维度和数量，不实际写入数据库。
    /// confirm 阶段应用服务将这些草稿持久化到 PR_LIMIT_OCCUPY 表，状态为 PENDING。
    /// commit 后推进为 CONFIRMED，cancel/expire/reverse 再释放或终止占用。
    /// 累计限额时需同时看 PENDING 和 CONFIRMED 状态，防止并发突破上限。
    /// </remarks>
    public IReadOnlyList<LimitOccupy> LimitOccupies { get; set; } = Array.Empty<LimitOccupy>();
}
