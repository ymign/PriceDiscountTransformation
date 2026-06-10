using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Aggregates.Quota;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 计价上下文，承载一次规则计算从输入到输出的全部运行态数据。
/// </summary>
/// <remarks>
/// 该对象不是数据库实体，而是计价引擎和动作执行器之间共享的可变工作区。请求进入后先被转换成上下文，
/// 规则匹配、公式计算、限额截断、追踪步骤和待写入占额都会逐步写回这里，最后再转换为响应和审计表数据。
/// </remarks>
public sealed class PricingContext
{
    /// <summary>
    /// 调用类型，例如 SIMULATE、CONFIRM、COMMIT、CANCEL 或 REVERSE
    /// </summary>
    public string CallType { get; set; } = string.Empty;
    /// <summary>
    /// 是否需要对限额维度执行数据库行锁。试算为 false，确认计费为 true。
    /// </summary>
    public bool ShouldLockLimits { get; set; }
    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    public string PatientId { get; set; } = string.Empty;
    /// <summary>
    /// 就诊标识，可为空，存在时用于缩小追溯和对账范围
    /// </summary>
    public string? VisitId { get; set; }
    /// <summary>
    /// 项目编码，是规则匹配、价格诊断和限额累计的核心维度
    /// </summary>
    public string ItemCode { get; set; } = string.Empty;
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; set; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    public decimal InputQty { get; set; }
    /// <summary>
    /// 调用方录入数量的单位，例如 PART、CM2、EACH
    /// </summary>
    public string? Unit { get; set; }
    /// <summary>
    /// 项目单价，来源为渠道传入值；权威物价差异仅记录诊断日志。
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹
    /// </summary>
    public string? BodyPartCode { get; set; }
    /// <summary>
    /// 收费场景编码，用于匹配门诊、住院、手术批费等差异化规则
    /// </summary>
    public string? ChargeScene { get; set; }
    /// <summary>
    /// 当前项目所属的项目组编码，来源为 PR_ITEM_GROUP.GROUP_CODE。
    /// 同组互斥、同手术封顶等执行器依赖该字段判断项目归属。
    /// 为空时表示该项目不属于任何项目组。
    /// </summary>
    public string? ItemGroupCode { get; set; }
    /// <summary>
    /// 扩展参数字典，用于承载调用方传入的业务上下文（如手术标识、孕次、住院号等）。
    /// 键名由各执行器自行约定，计价中心不做统一校验。
    /// 为空时表示本次计价请求没有额外扩展参数。
    /// </summary>
    public IReadOnlyDictionary<string, string>? ExtraParams { get; set; }
    /// <summary>
    /// 收费科室编码，用于排除特定科室的折价规则（如挂号部 7021）。
    /// 为空时表示渠道未传入科室信息，科室排除条件评估器应按"不排除"（通过）处理。
    /// </summary>
    public string? ChargeDeptCode { get; set; }

    /// <summary>
    /// 就诊类型编码，用于匹配就诊类型条件（如门诊、住院、急诊）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 来源渠道：HIS/自助机/微信在请求时传入，由渠道侧的就诊记录决定。
    /// </para>
    /// <para>
    /// 典型值：OUTPATIENT（门诊）、INPATIENT（住院）、EMERGENCY（急诊）、
    /// PHYSICAL（体检）、DAY_SURGERY（日间手术）。
    /// </para>
    /// <para>
    /// 空值表示渠道未传入就诊类型，就诊类型条件评估器应按"不校验"（通过）处理。
    /// </para>
    /// </remarks>
    public string? VisitType { get; set; }
    /// <summary>
    /// 患者年龄（岁），用于年龄范围条件匹配（如儿童加收规则）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 来源渠道：HIS 从患者主索引（MPI）获取出生日期后计算得出，或直接传入年龄值。
    /// </para>
    /// <para>
    /// 精度说明：以"岁"为单位的整数年龄。如需更精确的月龄/日龄判断，
    /// 渠道应自行换算后以小数形式传入（如 0.5 表示 6 个月）。
    /// </para>
    /// <para>
    /// 空值表示渠道未传入年龄信息，年龄条件评估器应按"不校验"（通过）处理。
    /// </para>
    /// </remarks>
    public int? PatientAge { get; set; }
    /// <summary>
    /// 业务收费发生时间，用于规则生效判断和滑动窗口累计
    /// </summary>
    public DateTime BusinessChargeTime { get; set; }
    /// <summary>
    /// 来源系统编码，例如 HIS、自助机或微信公众号
    /// </summary>
    public string SourceSystem { get; set; } = string.Empty;
    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; set; }
    /// <summary>
    /// 调用方稳定业务号，confirm 重试必须复用，用于幂等保护
    /// </summary>
    public string? BusinessRequestNo { get; set; }

    /// <summary>
    /// 多部位或多片段明细。存在时可用于部位换算、面积累计和更精细的折价追踪。
    /// </summary>
    public IReadOnlyList<PricingPartItem>? PricingParts { get; set; }

    /// <summary>
    /// 已命中的规则主档集合。由规则匹配服务写入，后续用于动作排序和响应中的命中规则列表。
    /// </summary>
    public IReadOnlyList<RuleAggregate> MatchedRules { get; set; } = Array.Empty<RuleAggregate>();
    /// <summary>
    /// 已按优先级、互斥组和动作顺序整理后的执行动作链。
    /// </summary>
    public IReadOnlyList<RuleAction> OrderedActions { get; set; } = Array.Empty<RuleAction>();

    /// <summary>
    /// 换算后的计价数量
    /// </summary>
    public decimal ConvertedQty { get; set; }
    /// <summary>
    /// 公式动作计算得到的金额中间值，供后续封顶、保底或限额动作继续处理。
    /// </summary>
    public decimal FormulaAmount { get; set; }
    /// <summary>
    /// 限额动作处理后的金额中间值，用于区分公式金额和限额截断后的金额。
    /// </summary>
    public decimal LimitedAmount { get; set; }
    /// <summary>
    /// 最终可收费数量
    /// </summary>
    public decimal FinalQty { get; set; }
    /// <summary>
    /// 最终应返回给 HIS 的可收费金额。
    /// </summary>
    public decimal FinalAmount { get; set; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 超出限额的数量，由 ExceedToZeroExecutor 在处理超限时写入。
    /// 该值按双单位换算后的计价数量口径计算，不直接拿原始 InputQty 做差。
    /// 为 0 表示未超限，等于换算后的基准数量表示全部超限。
    /// 用于追溯展示和折价明细中的超出原因说明。
    /// </summary>
    public decimal ExceedQty { get; set; }

    /// <summary>
    /// REPLACE 模式下的加收项目信息，由 ExceedToZeroExecutor 在超限替换时写入。
    /// 为空时表示未发生替换（ZERO 或 CEILING 模式，或未超限）。
    /// 应用服务在生成折价明细时，需将此信息写入 ChargeDiscountDetail.ReasonDesc。
    /// </summary>
    public ReplaceChildResult? ReplaceChildResult { get; set; }

    /// <summary>
    /// 本次计价过程中产生的追踪步骤。应用服务会把它持久化为 PR_CHARGE_TRACE_STEP。
    /// </summary>
    public List<TraceStep> TraceSteps { get; set; } = new();
    /// <summary>
    /// 待写入数据库的限额占用草稿。执行器只生成草稿，RequestId 生成后由应用服务统一补齐并落库。
    /// </summary>
    public List<LimitOccupy> PendingLimitOccupies { get; set; } = new();
    /// <summary>
    /// 子项加收执行器产生的子项目计价结果集合。
    /// 主项目计价完成后，应用服务需遍历此集合逐个生成子项目收费明细和占额草稿。
    /// 子项与主项目共享同一 resultGroupNo，保证原子 commit/cancel。
    /// </summary>
    public List<ChildPricingResult> ChildPricingResults { get; set; } = new();

    /// <summary>
    /// 当前请求内全部费用明细共享的运行态状态。
    /// </summary>
    /// <remarks>
    /// 单次请求如果携带多条 Items，后续明细需要看到前序明细已经产生的请求内占额、
    /// 同组互斥计数、同手术累计金额和父项最终金额。该对象承载这份共享工作区。
    /// </remarks>
    public RequestSharedPricingState RequestSharedState { get; set; } = new();

    /// <summary>
    /// HIS 旧系统在当前时间窗口内已收费的数量（方案B兜底查询）。
    /// 上线过渡期由 HIS 从旧表查询后传入；TimeWindowLimitExecutor 会将此值累加到占用总量，
    /// 确保新系统上线初期与旧系统数据之间无间隙。新旧数据对齐后置 0 即可，引擎行为不变。
    /// </summary>
    public decimal LegacyOccupiedQty { get; set; }

    /// <summary>
    /// 按普通计价口径初始化本次计算的基础数量和金额。
    /// </summary>
    public void InitializeForCalculation()
    {
        ConvertedQty = InputQty;
        FinalQty = InputQty;
        FinalAmount = GetOriginalAmount();
        FormulaAmount = 0;
        LimitedAmount = 0;
        DiscountAmount = 0;
    }

    /// <summary>
    /// 计算原始金额。
    /// </summary>
    public decimal GetOriginalAmount()
    {
        return UnitPrice * InputQty;
    }
}
