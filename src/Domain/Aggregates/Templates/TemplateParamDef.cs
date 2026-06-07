using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Templates;

public sealed class TemplateParamDef
{
    public long ParamDefId { get; set; }

    public long TemplateVersionId { get; set; }

    public string ParamCode { get; set; } = string.Empty;

    public string ParamName { get; set; } = string.Empty;

    public string ValueType { get; set; } = string.Empty;

    public string IsRequired { get; set; } = EnableFlag.No;

    public string? DefaultText { get; set; }

    public decimal? DefaultNumber { get; set; }

    public string? DefaultBool { get; set; }

    public string? DictType { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public string? RegexRule { get; set; }

    public string? UiControl { get; set; }

    public string? HelpText { get; set; }

    public string? RiskFlag { get; set; }

    public int SortNo { get; set; }
}
