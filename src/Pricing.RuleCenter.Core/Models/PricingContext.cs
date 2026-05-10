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
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
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
    /// 项目单价，confirm 时应与权威物价主数据强校验
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
    public IReadOnlyList<RuleHeader> MatchedRules { get; set; } = Array.Empty<RuleHeader>();
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
    /// 本次计价过程中产生的追踪步骤。应用服务会把它持久化为 PR_CHARGE_TRACE_STEP。
    /// </summary>
    public List<TraceStep> TraceSteps { get; set; } = new();
    /// <summary>
    /// 待写入数据库的限额占用草稿。执行器只生成草稿，RequestId 生成后由应用服务统一补齐并落库。
    /// </summary>
    public List<LimitOccupy> PendingLimitOccupies { get; set; } = new();

    /// <summary>
    /// 同一次收费动作内已经被前序费用明细占用的数量缓存。
    /// </summary>
    /// <remarks>
    /// 一次结算请求可以携带多条收费明细。单次限额的业务口径是“单次收费动作”，不是单条收费明细，
    /// 因此应用服务在循环计算 items[] 时需要把前面明细已经占用的单次额度传给后续明细。
    /// </remarks>
    public IReadOnlyDictionary<string, decimal> InRequestOccupiedQtyByLimitDimension { get; set; } =
        new Dictionary<string, decimal>();

    /// <summary>
    /// 同一次收费动作内前序费用明细已经生成的限额占用草稿。
    /// </summary>
    /// <remarks>
    /// TIME_WINDOW 需要按业务收费时间判断前序明细是否落入当前滑动窗口，单纯按维度累计会把窗口外明细也算进去。
    /// 因此这里保留占额草稿本身，供时间窗执行器按 BusinessChargeTime 做精确过滤。
    /// </remarks>
    public IReadOnlyList<LimitOccupy> InRequestLimitOccupies { get; set; } = Array.Empty<LimitOccupy>();
}

/// <summary>
/// 计价部位或片段明细。
/// </summary>
/// <remarks>
/// 当一个收费项目内部包含多个部位、病灶或面积片段时，调用方可提供该集合。当前规则中心会保留这些数据，
/// 供后续部位匹配、数量换算或追踪展示使用。
/// </remarks>
public sealed class PricingPartItem
{
    /// <summary>
    /// 片段序号，用于保持调用方传入的明细顺序。
    /// </summary>
    public int? PartSeq { get; set; }
    /// <summary>
    /// 片段编码，可对应检查部位、材料子项或其他业务拆分编码。
    /// </summary>
    public string? PartCode { get; set; }
    /// <summary>
    /// 片段名称，用于追踪页面展示。
    /// </summary>
    public string? PartName { get; set; }
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹
    /// </summary>
    public string? BodyPartCode { get; set; }
    /// <summary>
    /// 该片段对应的数量。
    /// </summary>
    public decimal Qty { get; set; }
    /// <summary>
    /// 该片段面积，适用于按面积折算的项目。
    /// </summary>
    public decimal? Area { get; set; }
    /// <summary>
    /// 计量类型，例如面积、长度、次数或病灶数量。
    /// </summary>
    public string? MeasureType { get; set; }
    /// <summary>
    /// 计量数值，与 MeasureType 和 MeasureUnit 配合解释。
    /// </summary>
    public decimal? MeasureValue { get; set; }
    /// <summary>
    /// 计量单位，例如 cm2、cm、次。
    /// </summary>
    public string? MeasureUnit { get; set; }
    /// <summary>
    /// 病灶数量，适用于按病灶个数限制或折算的项目。
    /// </summary>
    public int? LesionCount { get; set; }
}

/// <summary>
/// 计价追踪步骤的内存表示。
/// </summary>
/// <remarks>
/// 该对象在引擎运行时生成，随后被应用服务转换为持久化步骤表。它保留每个阶段的输入值、输出值和参数快照，
/// 用于解释规则为什么命中、数量为什么变化、金额为什么被截断。
/// </remarks>
public sealed class TraceStep
{
    /// <summary>
    /// 步骤序号，按计价链路执行顺序递增
    /// </summary>
    public int StepNo { get; set; }
    /// <summary>
    /// 步骤类型，只使用 DDL 允许的 MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR
    /// </summary>
    public string StepType { get; set; } = string.Empty;
    /// <summary>
    /// 步骤说明，解释命中规则、限制口径或折价原因
    /// </summary>
    public string? StepDesc { get; set; }
    /// <summary>
    /// 当前步骤处理前的关键数值，通常是数量或金额。
    /// </summary>
    public decimal? InputValue { get; set; }
    /// <summary>
    /// 当前步骤处理后的关键数值，通常是数量或金额。
    /// </summary>
    public decimal? OutputValue { get; set; }
    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; set; }
}
