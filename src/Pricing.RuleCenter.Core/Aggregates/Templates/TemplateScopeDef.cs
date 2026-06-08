using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Templates;

public sealed class TemplateScopeDef
{
    public long ScopeDefId { get; set; }

    public long TemplateVersionId { get; set; }

    public string ScopeDimension { get; set; } = string.Empty;

    public string IsRequired { get; set; } = EnableFlag.No;

    public string AllowMultiple { get; set; } = EnableFlag.No;

    public int SortNo { get; set; }
}
