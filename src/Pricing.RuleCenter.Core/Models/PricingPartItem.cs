namespace Pricing.RuleCenter.Core.Models;

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
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹。
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
    /// 计量数值，与 <see cref="MeasureType"/> 和 <see cref="MeasureUnit"/> 配合解释。
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
