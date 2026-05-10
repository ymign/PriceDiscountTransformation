using System;
using System.Collections.Generic;

namespace HIS.Pricing.Client
{
    public sealed class PagedResponse<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class RuleHeaderResponse
    {
        public long RuleId { get; set; }
        public string RuleCode { get; set; }
        public string RuleName { get; set; }
        public string RuleCategory { get; set; }
        public string RuleScope { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GroupCode { get; set; }
        public int Priority { get; set; }
        public int CurrentVersion { get; set; }
        public string Status { get; set; }
        public string IsEnabled { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public sealed class RuleHeaderCreateRequest
    {
        public string RuleCode { get; set; }
        public string RuleName { get; set; }
        public string RuleCategory { get; set; }
        public string RuleScope { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GroupCode { get; set; }
        public int Priority { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
    }

    public sealed class RuleHeaderUpdateRequest
    {
        public string RuleName { get; set; }
        public string RuleCategory { get; set; }
        public string RuleScope { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GroupCode { get; set; }
        public int Priority { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Remark { get; set; }
        public string UpdatedBy { get; set; }
    }

    public sealed class RuleVersionResponse
    {
        public long VersionId { get; set; }
        public long RuleId { get; set; }
        public int VersionNo { get; set; }
        public string VersionStatus { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string RuleSnapshot { get; set; }
        public string PublishedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string PublishRemark { get; set; }
    }

    public sealed class RuleConditionResponse
    {
        public long ConditionId { get; set; }
        public long RuleId { get; set; }
        public int VersionNo { get; set; }
        public string ConditionGroup { get; set; }
        public string ConditionType { get; set; }
        public string OperatorType { get; set; }
        public string LeftKey { get; set; }
        public string RightValue { get; set; }
        public string ParamsJson { get; set; }
        public int SortNo { get; set; }
        public string IsEnabled { get; set; }
    }

    public sealed class RuleConditionItemRequest
    {
        public string ConditionGroup { get; set; }
        public string ConditionType { get; set; }
        public string OperatorType { get; set; }
        public string LeftKey { get; set; }
        public string RightValue { get; set; }
        public string ParamsJson { get; set; }
        public int SortNo { get; set; }
        public string IsEnabled { get; set; }
    }

    public sealed class RuleConditionSaveRequest
    {
        public List<RuleConditionItemRequest> Conditions { get; set; }
    }

    public sealed class RuleActionResponse
    {
        public long ActionId { get; set; }
        public long RuleId { get; set; }
        public int VersionNo { get; set; }
        public string ActionType { get; set; }
        public string ExecutorCode { get; set; }
        public string ParamsJson { get; set; }
        public string ExclusiveGroup { get; set; }
        public int SortNo { get; set; }
        public string OnError { get; set; }
        public string IsEnabled { get; set; }
    }

    public sealed class RuleActionItemRequest
    {
        public string ActionType { get; set; }
        public string ExecutorCode { get; set; }
        public string ParamsJson { get; set; }
        public string ExclusiveGroup { get; set; }
        public int SortNo { get; set; }
        public string OnError { get; set; }
        public string IsEnabled { get; set; }
    }

    public sealed class RuleActionSaveRequest
    {
        public List<RuleActionItemRequest> Actions { get; set; }
    }

    public sealed class RulePublishRequest
    {
        public int VersionNo { get; set; }
        public string PublishedBy { get; set; }
        public string Remark { get; set; }
    }

    public sealed class RuleDisableRequest
    {
        public string PublishedBy { get; set; }
        public string Remark { get; set; }
    }

    public sealed class RuleRollbackRequest
    {
        public string PublishedBy { get; set; }
        public string Remark { get; set; }
    }

    public sealed class RulePublishResponse
    {
        public long PublishId { get; set; }
        public string PublishNo { get; set; }
        public long RuleId { get; set; }
        public int? FromVersion { get; set; }
        public int ToVersion { get; set; }
        public string ActionType { get; set; }
        public string PublishedBy { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Remark { get; set; }
    }

    public sealed class RuleChangeLogResponse
    {
        public long ChangeId { get; set; }
        public long RuleId { get; set; }
        public int? VersionNo { get; set; }
        public string ChangeType { get; set; }
        public string ChangeSummary { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string SourceSystem { get; set; }
    }

    public sealed class DictResponse
    {
        public long DictId { get; set; }
        public string DictType { get; set; }
        public string DictCode { get; set; }
        public string DictName { get; set; }
        public string ParentCode { get; set; }
        public int SortNo { get; set; }
        public string IsEnabled { get; set; }
        public string Remark { get; set; }
    }

    public sealed class DictCreateRequest
    {
        public string DictType { get; set; }
        public string DictCode { get; set; }
        public string DictName { get; set; }
        public string ParentCode { get; set; }
        public int SortNo { get; set; }
        public string Remark { get; set; }
    }

    public sealed class DictUpdateRequest
    {
        public string DictName { get; set; }
        public string ParentCode { get; set; }
        public int SortNo { get; set; }
        public string Remark { get; set; }
    }

    public sealed class FormulaDefResponse
    {
        public long FormulaId { get; set; }
        public string FormulaCode { get; set; }
        public string FormulaName { get; set; }
        public string FormulaDesc { get; set; }
        public string ExecutorCode { get; set; }
        public string ParamSchemaJson { get; set; }
        public string IsEnabled { get; set; }
        public string Remark { get; set; }
    }

    public sealed class FormulaDefCreateRequest
    {
        public string FormulaCode { get; set; }
        public string FormulaName { get; set; }
        public string FormulaDesc { get; set; }
        public string ExecutorCode { get; set; }
        public string ParamSchemaJson { get; set; }
        public string Remark { get; set; }
    }

    public sealed class FormulaDefUpdateRequest
    {
        public string FormulaName { get; set; }
        public string FormulaDesc { get; set; }
        public string ExecutorCode { get; set; }
        public string ParamSchemaJson { get; set; }
        public string Remark { get; set; }
    }
}
