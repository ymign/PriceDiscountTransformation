using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 计价计算请求 DTO。
/// </summary>
/// <remarks>
/// 该请求同时服务试算和确认计费。试算可以没有稳定业务请求号，确认计费必须尽量提供
/// BusinessRequestNo，便于规则中心做幂等保护和重复提交识别。
/// </remarks>
public sealed class PricingCalculateRequest
{
    /// <summary>
    /// 调用方或服务端生成的技术请求流水号，用于排查单次 HTTP 调用
    /// </summary>
    public string? RequestNo { get; init; }

    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    [Required(ErrorMessage = "患者ID不能为空")]
    public string PatientId { get; init; } = string.Empty;

    /// <summary>
    /// 就诊标识，可为空，存在时用于缩小追溯和对账范围
    /// </summary>
    public string? VisitId { get; init; }
    /// <summary>
    /// 就诊类型编码，用于匹配门诊、住院、急诊等就诊类型条件。
    /// </summary>
    public string? VisitType { get; init; }
    /// <summary>
    /// 收费科室编码，用于排除特定科室的折价规则（如挂号部 7021）。
    /// 为空时科室排除条件按"不排除"处理。
    /// </summary>
    public string? ChargeDeptCode { get; init; }
    /// <summary>
    /// 患者年龄（岁），用于年龄范围条件匹配。
    /// </summary>
    public int? PatientAge { get; init; }
    /// <summary>
    /// 门诊号、住院号或就诊流水号，用于与 HIS 业务上下文对齐
    /// </summary>
    public string? EncounterNo { get; init; }

    /// <summary>
    /// 收费场景编码，用于匹配门诊、住院、手术批费等差异化规则
    /// </summary>
    public string? ChargeScene { get; init; }
    /// <summary>
    /// 业务收费发生时间，用于规则生效判断和滑动窗口累计
    /// </summary>
    public DateTime BusinessChargeTime { get; init; }

    /// <summary>
    /// 来源系统编码，例如 HIS、自助机或微信公众号
    /// </summary>
    [Required(ErrorMessage = "来源系统不能为空")]
    public string SourceSystem { get; init; } = string.Empty;

    /// <summary>
    /// 来源终端或站点标识，用于定位具体调用入口
    /// </summary>
    public string? SourceTerminal { get; init; }
    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; init; }
    /// <summary>
    /// 调用方稳定业务号，confirm 重试必须复用，用于幂等保护
    /// </summary>
    public string? BusinessRequestNo { get; init; }
    /// <summary>
    /// 操作员工号或系统账号，用于审计谁发起了本次计价。
    /// </summary>
    public string? OperatorId { get; init; }
    /// <summary>
    /// 操作员姓名，用于追踪页面展示。
    /// </summary>
    public string? OperatorName { get; init; }
    /// <summary>
    /// 调用方扩展参数。confirm 幂等指纹会纳入该字段，影响规则匹配或金额的参数必须稳定传入。
    /// </summary>
    public Dictionary<string, object?>? ExtraParams { get; init; }
    /// <summary>
    /// 本次结算包含的费用明细集合。一次结算可同时传入多条费用，每条费用独立携带项目、数量和单价。
    /// </summary>
    [Required(ErrorMessage = "费用明细不能为空")]
    [MinLength(1, ErrorMessage = "费用明细至少包含一条")]
    public IReadOnlyList<PricingCalculateItemRequest> Items { get; init; } = Array.Empty<PricingCalculateItemRequest>();
}

/// <summary>
/// 计价请求中的单条费用明细 DTO。
/// </summary>
public sealed class PricingCalculateItemRequest
{
    /// <summary>
    /// 单条费用明细的请求号，用于批量响应和调用方本地行号关联。
    /// </summary>
    public string? ItemRequestNo { get; init; }
    /// <summary>
    /// 收费明细号，用于定位单条收费项目。
    /// </summary>
    public string? ChargeDetailNo { get; init; }

    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度。
    /// 该字段属于费用明细层；统一计价请求根对象不再放单个 ItemCode。
    /// </summary>
    [Required(ErrorMessage = "项目编码不能为空")]
    public string ItemCode { get; init; } = string.Empty;

    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明。
    /// </summary>
    public string? ItemName { get; init; }
    /// <summary>
    /// 项目组编码，用于同组互斥、同手术封顶等组维度规则。
    /// </summary>
    public string? ItemGroupCode { get; init; }

    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖。
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    public decimal InputQty { get; init; }

    /// <summary>
    /// 调用方录入数量的单位，例如 PART、CM2、EACH。
    /// </summary>
    public string? Unit { get; init; }
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验。
    /// </summary>
    public decimal UnitPrice { get; init; }
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹。
    /// </summary>
    public string? BodyPartCode { get; init; }
    /// <summary>
    /// 单条费用的业务收费发生时间。为空时使用结算请求上的 BusinessChargeTime。
    /// </summary>
    public DateTime? BusinessChargeTime { get; init; }
    /// <summary>
    /// 单条费用扩展参数。与结算级 ExtraParams 合并后进入规则上下文和幂等指纹。
    /// </summary>
    public Dictionary<string, object?>? ExtraParams { get; init; }
    /// <summary>
    /// HIS 旧系统在当前时间窗口内已收费的数量（方案B兜底查询）。
    /// 上线过渡期由 HIS 从旧表查询后传入；新旧系统数据对齐后可置 null，引擎行为不变。
    /// </summary>
    public decimal? LegacyOccupiedQty { get; init; }
    /// <summary>
    /// 多部位或多片段明细，用于更细粒度的部位匹配、面积折算和追踪展示。
    /// </summary>
    public IReadOnlyList<PricingPartItemRequest>? PricingParts { get; init; }
}

/// <summary>
/// 计价请求中的部位或片段明细 DTO。
/// </summary>
/// <remarks>
/// 明细项是可选扩展结构，用于表达一个收费项目内部的多个部位、面积或病灶。当前项目主字段仍然以
/// PricingCalculateItemRequest 上的 ItemCode、InputQty 和 UnitPrice 为准。
/// </remarks>
public sealed class PricingPartItemRequest
{
    /// <summary>
    /// 片段序号，用于保持调用方传入顺序。
    /// </summary>
    public int? PartSeq { get; init; }
    /// <summary>
    /// 片段编码，可对应部位、材料子项或其他业务拆分编码。
    /// </summary>
    public string? PartCode { get; init; }
    /// <summary>
    /// 片段名称，用于追踪页面展示。
    /// </summary>
    public string? PartName { get; init; }
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹
    /// </summary>
    public string? BodyPartCode { get; init; }
    /// <summary>
    /// 该片段对应的数量。
    /// </summary>
    public decimal Qty { get; init; }
    /// <summary>
    /// 该片段面积，适用于按面积折算的项目。
    /// </summary>
    public decimal? Area { get; init; }
    /// <summary>
    /// 计量类型，例如面积、长度、次数或病灶数量。
    /// </summary>
    public string? MeasureType { get; init; }
    /// <summary>
    /// 计量数值，与 MeasureType 和 MeasureUnit 配合解释。
    /// </summary>
    public decimal? MeasureValue { get; init; }
    /// <summary>
    /// 计量单位，例如 cm2、cm、次。
    /// </summary>
    public string? MeasureUnit { get; init; }
    /// <summary>
    /// 病灶数量，适用于按病灶个数折算或限额的项目。
    /// </summary>
    public int? LesionCount { get; init; }
}
