using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Database;

/// <summary>
/// SqlSugar 实体类型配置，使用 Fluent API 替代 Core 模型上的注解特性。
/// </summary>
/// <remarks>
/// 这是 Clean Architecture 的关键保障：Core 领域模型不依赖任何 ORM 框架，
/// 所有数据库映射细节封装在 Infrastructure 层。
/// </remarks>
public static class EntityTypeConfigs
{
    private static readonly IReadOnlyDictionary<Type, string> PrimaryKeyProperties =
        new Dictionary<Type, string>
        {
            [typeof(RuleAggregate)] = nameof(RuleAggregate.RuleId),
            [typeof(RuleVersion)] = nameof(RuleVersion.VersionId),
            [typeof(RuleCondition)] = nameof(RuleCondition.ConditionId),
            [typeof(RuleAction)] = nameof(RuleAction.ActionId),
            [typeof(RulePublish)] = nameof(RulePublish.PublishId),
            [typeof(RuleChangeLog)] = nameof(RuleChangeLog.ChangeId),
            [typeof(Dict)] = nameof(Dict.DictId),
            [typeof(CacheVersion)] = nameof(CacheVersion.CacheScope),
            [typeof(RuleCacheInvalidationOutbox)] = nameof(RuleCacheInvalidationOutbox.OutboxId),
            [typeof(FormulaDef)] = nameof(FormulaDef.FormulaId),
            [typeof(ItemGroup)] = nameof(ItemGroup.GroupId),
            [typeof(ItemGroupDetail)] = nameof(ItemGroupDetail.DetailId),
            [typeof(LimitLock)] = nameof(LimitLock.LockKey),
            [typeof(LimitOccupy)] = nameof(LimitOccupy.OccupyId),
            [typeof(ChargeRequest)] = nameof(ChargeRequest.RequestId),
            [typeof(ChargeDiscountDetail)] = nameof(ChargeDiscountDetail.DiscountId),
            [typeof(ChargeTraceStep)] = nameof(ChargeTraceStep.StepId),
            [typeof(ChargeReverseLog)] = nameof(ChargeReverseLog.ReverseId),
            [typeof(RuleApproval)] = nameof(RuleApproval.ApprovalId),
            [typeof(RuleTestCase)] = nameof(RuleTestCase.TestCaseId),
            [typeof(RuleTestRun)] = nameof(RuleTestRun.TestRunId),
            [typeof(TemplateAggregate)] = nameof(TemplateAggregate.TemplateId),
            [typeof(TemplateVersion)] = nameof(TemplateVersion.TemplateVersionId),
            [typeof(TemplateParamDef)] = nameof(TemplateParamDef.ParamDefId),
            [typeof(TemplateStepDef)] = nameof(TemplateStepDef.StepDefId),
            [typeof(TemplateScopeDef)] = nameof(TemplateScopeDef.ScopeDefId),
            [typeof(PolicyAggregate)] = nameof(PolicyAggregate.PolicyId),
            [typeof(PolicyVersion)] = nameof(PolicyVersion.PolicyVersionId),
            [typeof(PolicyBinding)] = nameof(PolicyBinding.PolicyBindingId),
            [typeof(PolicyScope)] = nameof(PolicyScope.PolicyScopeId),
            [typeof(PolicyParam)] = nameof(PolicyParam.PolicyParamId),
            [typeof(PolicyReview)] = nameof(PolicyReview.ReviewId),
            [typeof(RuntimePackage)] = nameof(RuntimePackage.PackageId),
            [typeof(RuntimePackagePolicy)] = nameof(RuntimePackagePolicy.PackagePolicyId),
            [typeof(RuntimeRule)] = nameof(RuntimeRule.RuntimeRuleId),
            [typeof(RuntimeCondition)] = nameof(RuntimeCondition.RuntimeConditionId),
            [typeof(RuntimeAction)] = nameof(RuntimeAction.RuntimeActionId),
            [typeof(RuntimePackageState)] = nameof(RuntimePackageState.StateCode)
        };

    /// <summary>
    /// 创建 SqlSugar 外部实体配置，用于在不污染 Core 模型的前提下补充主键元数据。
    /// </summary>
    public static ConfigureExternalServices CreateExternalServices()
    {
        return new ConfigureExternalServices
        {
            EntityService = (property, column) =>
            {
                if (property.DeclaringType is null)
                {
                    return;
                }

                // DomainEvents 是领域事件集合，不映射到数据库列
                if (string.Equals(property.Name, "DomainEvents", StringComparison.Ordinal))
                {
                    column.IsIgnore = true;
                    return;
                }

                if (PrimaryKeyProperties.TryGetValue(property.DeclaringType, out var primaryKey) &&
                    string.Equals(property.Name, primaryKey, StringComparison.Ordinal))
                {
                    column.IsPrimarykey = true;
                }
            }
        };
    }

    /// <summary>
    /// 配置 PR_RULE_HEADER 表映射。
    /// </summary>
    public static void ConfigureRuleAggregate(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleAggregate).Name, "PR_RULE_HEADER");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.RuleCode), DbColumnName = "RULE_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.RuleName), DbColumnName = "RULE_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.RuleCategory), DbColumnName = "RULE_CATEGORY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.RuleScope), DbColumnName = "RULE_SCOPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.GroupCode), DbColumnName = "GROUP_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.Priority), DbColumnName = "PRIORITY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.CurrentVersion), DbColumnName = "CURRENT_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.IsEnabled), DbColumnName = "IS_ENABLED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.EffectiveFrom), DbColumnName = "EFFECTIVE_FROM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.EffectiveTo), DbColumnName = "EFFECTIVE_TO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.RollbackMode), DbColumnName = "ROLLBACK_MODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.Remark), DbColumnName = "REMARK" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.CreatedBy), DbColumnName = "CREATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.CreatedAt), DbColumnName = "CREATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.UpdatedBy), DbColumnName = "UPDATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAggregate), PropertyName = nameof(RuleAggregate.UpdatedAt), DbColumnName = "UPDATED_AT" });
    }

    /// <summary>
    /// 配置 PR_RULE_VERSION 表映射。
    /// </summary>
    public static void ConfigureRuleVersion(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleVersion).Name, "PR_RULE_VERSION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.VersionId), DbColumnName = "VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.VersionStatus), DbColumnName = "VERSION_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.EffectiveFrom), DbColumnName = "EFFECTIVE_FROM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.EffectiveTo), DbColumnName = "EFFECTIVE_TO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.RuleSnapshot), DbColumnName = "RULE_SNAPSHOT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.PublishedBy), DbColumnName = "PUBLISHED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.PublishedAt), DbColumnName = "PUBLISHED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleVersion), PropertyName = nameof(RuleVersion.PublishRemark), DbColumnName = "PUBLISH_REMARK" });
    }

    /// <summary>
    /// 配置 PR_RULE_CONDITION 表映射。
    /// </summary>
    public static void ConfigureRuleCondition(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleCondition).Name, "PR_RULE_CONDITION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.ConditionId), DbColumnName = "CONDITION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.ConditionGroup), DbColumnName = "CONDITION_GROUP" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.ConditionType), DbColumnName = "CONDITION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.OperatorType), DbColumnName = "OPERATOR_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.LeftKey), DbColumnName = "LEFT_KEY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.RightValue), DbColumnName = "RIGHT_VALUE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.ParamsJson), DbColumnName = "PARAMS_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.SortNo), DbColumnName = "SORT_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCondition), PropertyName = nameof(RuleCondition.IsEnabled), DbColumnName = "IS_ENABLED" });
    }

    /// <summary>
    /// 配置 PR_RULE_ACTION 表映射。
    /// </summary>
    public static void ConfigureRuleAction(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleAction).Name, "PR_RULE_ACTION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.ActionId), DbColumnName = "ACTION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.ActionType), DbColumnName = "ACTION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.ExecutorCode), DbColumnName = "EXECUTOR_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.ParamsJson), DbColumnName = "PARAMS_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.ExclusiveGroup), DbColumnName = "EXCLUSIVE_GROUP" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.SortNo), DbColumnName = "SORT_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.OnError), DbColumnName = "ON_ERROR" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleAction), PropertyName = nameof(RuleAction.IsEnabled), DbColumnName = "IS_ENABLED" });
    }

    /// <summary>
    /// 配置 PR_RULE_PUBLISH 表映射。
    /// </summary>
    public static void ConfigureRulePublish(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RulePublish).Name, "PR_RULE_PUBLISH");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.PublishId), DbColumnName = "PUBLISH_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.PublishNo), DbColumnName = "PUBLISH_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.FromVersion), DbColumnName = "FROM_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.ToVersion), DbColumnName = "TO_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.ActionType), DbColumnName = "ACTION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.PublishedBy), DbColumnName = "PUBLISHED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.PublishedAt), DbColumnName = "PUBLISHED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RulePublish), PropertyName = nameof(RulePublish.Remark), DbColumnName = "REMARK" });
    }

    /// <summary>
    /// 配置 PR_RULE_CHANGE_LOG 表映射。
    /// </summary>
    public static void ConfigureRuleChangeLog(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleChangeLog).Name, "PR_RULE_CHANGE_LOG");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.ChangeId), DbColumnName = "CHANGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.ChangeType), DbColumnName = "CHANGE_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.ChangeSummary), DbColumnName = "CHANGE_SUMMARY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.BeforeSnapshot), DbColumnName = "BEFORE_SNAPSHOT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.AfterSnapshot), DbColumnName = "AFTER_SNAPSHOT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.ChangedBy), DbColumnName = "CHANGED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.ChangedAt), DbColumnName = "CHANGED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleChangeLog), PropertyName = nameof(RuleChangeLog.SourceSystem), DbColumnName = "SOURCE_SYSTEM" });
    }

    /// <summary>
    /// 配置 PR_DICT 表映射。
    /// </summary>
    public static void ConfigureDict(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(Dict).Name, "PR_DICT");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.DictId), DbColumnName = "DICT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.DictType), DbColumnName = "DICT_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.DictCode), DbColumnName = "DICT_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.DictName), DbColumnName = "DICT_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.ParentCode), DbColumnName = "PARENT_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.SortNo), DbColumnName = "SORT_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.IsEnabled), DbColumnName = "IS_ENABLED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(Dict), PropertyName = nameof(Dict.Remark), DbColumnName = "REMARK" });
    }

    /// <summary>
    /// 配置 PR_CACHE_VERSION 表映射。
    /// </summary>
    public static void ConfigureCacheVersion(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(CacheVersion).Name, "PR_CACHE_VERSION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(CacheVersion), PropertyName = nameof(CacheVersion.CacheScope), DbColumnName = "CACHE_SCOPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(CacheVersion), PropertyName = nameof(CacheVersion.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(CacheVersion), PropertyName = nameof(CacheVersion.UpdatedAt), DbColumnName = "UPDATED_AT" });
    }

    /// <summary>
    /// 配置 PR_CACHE_INVALIDATION_OUTBOX 表映射。
    /// </summary>
    public static void ConfigureRuleCacheInvalidationOutbox(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleCacheInvalidationOutbox).Name, "PR_CACHE_INVALIDATION_OUTBOX");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.OutboxId), DbColumnName = "OUTBOX_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.CacheScope), DbColumnName = "CACHE_SCOPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.OperationType), DbColumnName = "OPERATION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.RetryCount), DbColumnName = "RETRY_COUNT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.NextRetryAt), DbColumnName = "NEXT_RETRY_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.LastError), DbColumnName = "LAST_ERROR" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.CreatedAt), DbColumnName = "CREATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleCacheInvalidationOutbox), PropertyName = nameof(RuleCacheInvalidationOutbox.ProcessedAt), DbColumnName = "PROCESSED_AT" });
    }

    /// <summary>
    /// 配置 PR_FORMULA_DEF 表映射。
    /// </summary>
    public static void ConfigureFormulaDef(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(FormulaDef).Name, "PR_FORMULA_DEF");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.FormulaId), DbColumnName = "FORMULA_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.FormulaCode), DbColumnName = "FORMULA_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.FormulaName), DbColumnName = "FORMULA_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.FormulaDesc), DbColumnName = "FORMULA_DESC" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.ExecutorCode), DbColumnName = "EXECUTOR_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.ParamSchemaJson), DbColumnName = "PARAM_SCHEMA_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.IsEnabled), DbColumnName = "IS_ENABLED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(FormulaDef), PropertyName = nameof(FormulaDef.Remark), DbColumnName = "REMARK" });
    }

    /// <summary>
    /// 配置 PR_ITEM_GROUP 表映射。
    /// </summary>
    public static void ConfigureItemGroup(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ItemGroup).Name, "PR_ITEM_GROUP");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.GroupId), DbColumnName = "GROUP_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.GroupCode), DbColumnName = "GROUP_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.GroupName), DbColumnName = "GROUP_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.GroupType), DbColumnName = "GROUP_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.IsEnabled), DbColumnName = "IS_ENABLED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.Remark), DbColumnName = "REMARK" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.CreatedBy), DbColumnName = "CREATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.CreatedAt), DbColumnName = "CREATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.UpdatedBy), DbColumnName = "UPDATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroup), PropertyName = nameof(ItemGroup.UpdatedAt), DbColumnName = "UPDATED_AT" });
    }

    /// <summary>
    /// 配置 PR_ITEM_GROUP_DETAIL 表映射。
    /// </summary>
    public static void ConfigureItemGroupDetail(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ItemGroupDetail).Name, "PR_ITEM_GROUP_DETAIL");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.DetailId), DbColumnName = "DETAIL_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.GroupId), DbColumnName = "GROUP_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.RoleType), DbColumnName = "ROLE_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.SortNo), DbColumnName = "SORT_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ItemGroupDetail), PropertyName = nameof(ItemGroupDetail.IsEnabled), DbColumnName = "IS_ENABLED" });
    }

    /// <summary>
    /// 配置 PR_LIMIT_LOCK 表映射。
    /// </summary>
    public static void ConfigureLimitLock(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(LimitLock).Name, "PR_LIMIT_LOCK");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitLock), PropertyName = nameof(LimitLock.LockKey), DbColumnName = "LOCK_KEY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitLock), PropertyName = nameof(LimitLock.LockDesc), DbColumnName = "LOCK_DESC" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitLock), PropertyName = nameof(LimitLock.UpdatedAt), DbColumnName = "UPDATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitLock), PropertyName = nameof(LimitLock.ExpireAt), DbColumnName = "EXPIRE_AT" });
    }

    /// <summary>
    /// 配置 PR_LIMIT_OCCUPY 表映射。
    /// </summary>
    public static void ConfigureLimitOccupy(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(LimitOccupy).Name, "PR_LIMIT_OCCUPY");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.OccupyId), DbColumnName = "OCCUPY_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.RequestId), DbColumnName = "REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.TraceId), DbColumnName = "TRACE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.PatientId), DbColumnName = "PATIENT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.ChargeDetailNo), DbColumnName = "CHARGE_DETAIL_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.ResultGroupNo), DbColumnName = "RESULT_GROUP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.RuleVersionNo), DbColumnName = "RULE_VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.LimitType), DbColumnName = "LIMIT_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.LimitKey), DbColumnName = "LIMIT_KEY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.OccupyQty), DbColumnName = "OCCUPY_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.OccupyAmt), DbColumnName = "OCCUPY_AMT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.OccupyType), DbColumnName = "OCCUPY_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.OriginalOccupyId), DbColumnName = "ORIGINAL_OCCUPY_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.BusinessChargeTime), DbColumnName = "BUSINESS_CHARGE_TIME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.LimitDimensionCode), DbColumnName = "LIMIT_DIMENSION_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.PartSeq), DbColumnName = "PART_SEQ" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.OccupiedAt), DbColumnName = "OCCUPIED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.ConfirmedAt), DbColumnName = "CONFIRMED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(LimitOccupy), PropertyName = nameof(LimitOccupy.ExpireAt), DbColumnName = "EXPIRE_AT" });
    }

    /// <summary>
    /// 配置 FIN_COM_UNDRUGINFO 表映射。
    /// </summary>
    public static void ConfigurePriceMasterItem(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PriceMasterItem).Name, "FIN_COM_UNDRUGINFO");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PriceMasterItem), PropertyName = nameof(PriceMasterItem.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PriceMasterItem), PropertyName = nameof(PriceMasterItem.UnitPrice), DbColumnName = "UNIT_PRICE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PriceMasterItem), PropertyName = nameof(PriceMasterItem.ChildPrice), DbColumnName = "UNIT_PRICE1" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PriceMasterItem), PropertyName = nameof(PriceMasterItem.PerinatalPrice), DbColumnName = "WEICHAN_PRICE" });
    }

    public static void ConfigureTemplateAggregate(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(TemplateAggregate).Name, "PR_TEMPLATE");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.TemplateId), DbColumnName = "TEMPLATE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.TemplateCode), DbColumnName = "TEMPLATE_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.TemplateName), DbColumnName = "TEMPLATE_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.Category), DbColumnName = "CATEGORY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.RiskLevel), DbColumnName = "RISK_LEVEL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.ExpressionMode), DbColumnName = "EXPRESSION_MODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.CurrentVersionNo), DbColumnName = "CURRENT_VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.CreatedBy), DbColumnName = "CREATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.CreatedAt), DbColumnName = "CREATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.UpdatedBy), DbColumnName = "UPDATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateAggregate), PropertyName = nameof(TemplateAggregate.UpdatedAt), DbColumnName = "UPDATED_AT" });
    }

    public static void ConfigureTemplateVersion(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(TemplateVersion).Name, "PR_TEMPLATE_VERSION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.TemplateVersionId), DbColumnName = "TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.TemplateId), DbColumnName = "TEMPLATE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.VersionStatus), DbColumnName = "VERSION_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.CapabilityFamily), DbColumnName = "CAPABILITY_FAMILY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.MergeMode), DbColumnName = "MERGE_MODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.Checksum), DbColumnName = "CHECKSUM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.Description), DbColumnName = "DESCRIPTION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.PublishedBy), DbColumnName = "PUBLISHED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateVersion), PropertyName = nameof(TemplateVersion.PublishedAt), DbColumnName = "PUBLISHED_AT" });
    }

    public static void ConfigureTemplateParamDef(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(TemplateParamDef).Name, "PR_TEMPLATE_PARAM_DEF");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.ParamDefId), DbColumnName = "PARAM_DEF_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.TemplateVersionId), DbColumnName = "TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.ParamCode), DbColumnName = "PARAM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.ParamName), DbColumnName = "PARAM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.ValueType), DbColumnName = "VALUE_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.IsRequired), DbColumnName = "IS_REQUIRED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.DefaultText), DbColumnName = "DEFAULT_TEXT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.DefaultNumber), DbColumnName = "DEFAULT_NUMBER" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.DefaultBool), DbColumnName = "DEFAULT_BOOL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.DictType), DbColumnName = "DICT_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.MinValue), DbColumnName = "MIN_VALUE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.MaxValue), DbColumnName = "MAX_VALUE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.RegexRule), DbColumnName = "REGEX_RULE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.UiControl), DbColumnName = "UI_CONTROL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.HelpText), DbColumnName = "HELP_TEXT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.RiskFlag), DbColumnName = "RISK_FLAG" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateParamDef), PropertyName = nameof(TemplateParamDef.SortNo), DbColumnName = "SORT_NO" });
    }

    public static void ConfigureTemplateStepDef(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(TemplateStepDef).Name, "PR_TEMPLATE_STEP_DEF");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.StepDefId), DbColumnName = "STEP_DEF_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.TemplateVersionId), DbColumnName = "TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.StepNo), DbColumnName = "STEP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.StepKind), DbColumnName = "STEP_KIND" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.CapabilityCode), DbColumnName = "CAPABILITY_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.ActionType), DbColumnName = "ACTION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.ExecutorCode), DbColumnName = "EXECUTOR_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.OnError), DbColumnName = "ON_ERROR" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateStepDef), PropertyName = nameof(TemplateStepDef.StepConfigClob), DbColumnName = "STEP_CONFIG_CLOB" });
    }

    public static void ConfigureTemplateScopeDef(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(TemplateScopeDef).Name, "PR_TEMPLATE_SCOPE_DEF");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateScopeDef), PropertyName = nameof(TemplateScopeDef.ScopeDefId), DbColumnName = "SCOPE_DEF_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateScopeDef), PropertyName = nameof(TemplateScopeDef.TemplateVersionId), DbColumnName = "TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateScopeDef), PropertyName = nameof(TemplateScopeDef.ScopeDimension), DbColumnName = "SCOPE_DIMENSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateScopeDef), PropertyName = nameof(TemplateScopeDef.IsRequired), DbColumnName = "IS_REQUIRED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateScopeDef), PropertyName = nameof(TemplateScopeDef.AllowMultiple), DbColumnName = "ALLOW_MULTIPLE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(TemplateScopeDef), PropertyName = nameof(TemplateScopeDef.SortNo), DbColumnName = "SORT_NO" });
    }

    public static void ConfigurePolicyAggregate(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PolicyAggregate).Name, "PR_POLICY");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.PolicyId), DbColumnName = "POLICY_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.PolicyCode), DbColumnName = "POLICY_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.PolicyName), DbColumnName = "POLICY_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.TemplateId), DbColumnName = "TEMPLATE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.OwnerType), DbColumnName = "OWNER_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.PublishProfile), DbColumnName = "PUBLISH_PROFILE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.CurrentVersionNo), DbColumnName = "CURRENT_VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.CreatedBy), DbColumnName = "CREATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.CreatedAt), DbColumnName = "CREATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.UpdatedBy), DbColumnName = "UPDATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyAggregate), PropertyName = nameof(PolicyAggregate.UpdatedAt), DbColumnName = "UPDATED_AT" });
    }

    public static void ConfigurePolicyVersion(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PolicyVersion).Name, "PR_POLICY_VERSION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.PolicyVersionId), DbColumnName = "POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.PolicyId), DbColumnName = "POLICY_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.TemplateVersionId), DbColumnName = "TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.PolicyStatus), DbColumnName = "POLICY_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.EffectiveFrom), DbColumnName = "EFFECTIVE_FROM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.EffectiveTo), DbColumnName = "EFFECTIVE_TO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.BindingType), DbColumnName = "BINDING_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.ScopeLevel), DbColumnName = "SCOPE_LEVEL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.PriorityWeight), DbColumnName = "PRIORITY_WEIGHT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.Checksum), DbColumnName = "CHECKSUM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyVersion), PropertyName = nameof(PolicyVersion.LastBuiltPackageId), DbColumnName = "LAST_BUILT_PACKAGE_ID" });
    }

    public static void ConfigurePolicyBinding(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PolicyBinding).Name, "PR_POLICY_BINDING");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.PolicyBindingId), DbColumnName = "POLICY_BINDING_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.PolicyVersionId), DbColumnName = "POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.BindingType), DbColumnName = "BINDING_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.GroupCode), DbColumnName = "GROUP_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyBinding), PropertyName = nameof(PolicyBinding.GroupName), DbColumnName = "GROUP_NAME" });
    }

    public static void ConfigurePolicyScope(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PolicyScope).Name, "PR_POLICY_SCOPE");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.PolicyScopeId), DbColumnName = "POLICY_SCOPE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.PolicyVersionId), DbColumnName = "POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.ScopeDimension), DbColumnName = "SCOPE_DIMENSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.ScopeOperator), DbColumnName = "SCOPE_OPERATOR" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.ScopeValueText), DbColumnName = "SCOPE_VALUE_TEXT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.ScopeValueNumber), DbColumnName = "SCOPE_VALUE_NUMBER" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.ScopeValueDate), DbColumnName = "SCOPE_VALUE_DATE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyScope), PropertyName = nameof(PolicyScope.ScopeJson), DbColumnName = "SCOPE_JSON" });
    }

    public static void ConfigurePolicyParam(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PolicyParam).Name, "PR_POLICY_PARAM");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.PolicyParamId), DbColumnName = "POLICY_PARAM_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.PolicyVersionId), DbColumnName = "POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ParamCode), DbColumnName = "PARAM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ValueType), DbColumnName = "VALUE_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ValueText), DbColumnName = "VALUE_TEXT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ValueNumber), DbColumnName = "VALUE_NUMBER" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ValueDate), DbColumnName = "VALUE_DATE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ValueBool), DbColumnName = "VALUE_BOOL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ExprText), DbColumnName = "EXPR_TEXT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyParam), PropertyName = nameof(PolicyParam.ExprLevel), DbColumnName = "EXPR_LEVEL" });
    }

    public static void ConfigurePolicyReview(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(PolicyReview).Name, "PR_POLICY_REVIEW");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.ReviewId), DbColumnName = "REVIEW_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.PolicyVersionId), DbColumnName = "POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.ReviewStatus), DbColumnName = "REVIEW_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.ReviewStage), DbColumnName = "REVIEW_STAGE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.SubmittedBy), DbColumnName = "SUBMITTED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.SubmittedAt), DbColumnName = "SUBMITTED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.ReviewedBy), DbColumnName = "REVIEWED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.ReviewedAt), DbColumnName = "REVIEWED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.ReviewComment), DbColumnName = "REVIEW_COMMENT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(PolicyReview), PropertyName = nameof(PolicyReview.SourceChecksum), DbColumnName = "SOURCE_CHECKSUM" });
    }

    public static void ConfigureRuntimePackage(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuntimePackage).Name, "PR_RUNTIME_PACKAGE");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.PackageId), DbColumnName = "PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.PackageVersion), DbColumnName = "PACKAGE_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.PackageStatus), DbColumnName = "PACKAGE_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.BuildScope), DbColumnName = "BUILD_SCOPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.SourceChecksum), DbColumnName = "SOURCE_CHECKSUM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.BuiltBy), DbColumnName = "BUILT_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.BuiltAt), DbColumnName = "BUILT_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.ActivatedBy), DbColumnName = "ACTIVATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.ActivatedAt), DbColumnName = "ACTIVATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackage), PropertyName = nameof(RuntimePackage.RolledBackFromPackageId), DbColumnName = "ROLLED_BACK_FROM_PACKAGE_ID" });
    }

    public static void ConfigureRuntimePackagePolicy(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuntimePackagePolicy).Name, "PR_RUNTIME_PACKAGE_POLICY");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.PackagePolicyId), DbColumnName = "PACKAGE_POLICY_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.PackageId), DbColumnName = "PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.PolicyVersionId), DbColumnName = "POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.PolicyCode), DbColumnName = "POLICY_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.TemplateVersionId), DbColumnName = "TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.CapabilityFamily), DbColumnName = "CAPABILITY_FAMILY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackagePolicy), PropertyName = nameof(RuntimePackagePolicy.PriorityKey), DbColumnName = "PRIORITY_KEY" });
    }

    public static void ConfigureRuntimeRule(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuntimeRule).Name, "PR_RUNTIME_RULE");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.RuntimeRuleId), DbColumnName = "RUNTIME_RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.PackageId), DbColumnName = "PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.SourceTemplateVersionId), DbColumnName = "SOURCE_TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.SourcePolicyVersionId), DbColumnName = "SOURCE_POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.CapabilityFamily), DbColumnName = "CAPABILITY_FAMILY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.MergeMode), DbColumnName = "MERGE_MODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.TargetItemCode), DbColumnName = "TARGET_ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.TargetGroupCode), DbColumnName = "TARGET_GROUP_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.ScopeLevel), DbColumnName = "SCOPE_LEVEL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.PriorityKey), DbColumnName = "PRIORITY_KEY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.EffectiveFrom), DbColumnName = "EFFECTIVE_FROM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.EffectiveTo), DbColumnName = "EFFECTIVE_TO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeRule), PropertyName = nameof(RuntimeRule.MatchKey), DbColumnName = "MATCH_KEY" });
    }

    public static void ConfigureRuntimeCondition(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuntimeCondition).Name, "PR_RUNTIME_CONDITION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.RuntimeConditionId), DbColumnName = "RUNTIME_CONDITION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.RuntimeRuleId), DbColumnName = "RUNTIME_RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.ConditionGroup), DbColumnName = "CONDITION_GROUP" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.ConditionType), DbColumnName = "CONDITION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.OperatorType), DbColumnName = "OPERATOR_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.LeftKey), DbColumnName = "LEFT_KEY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.RightValue), DbColumnName = "RIGHT_VALUE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.ParamsJson), DbColumnName = "PARAMS_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeCondition), PropertyName = nameof(RuntimeCondition.SortNo), DbColumnName = "SORT_NO" });
    }

    public static void ConfigureRuntimeAction(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuntimeAction).Name, "PR_RUNTIME_ACTION");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.RuntimeActionId), DbColumnName = "RUNTIME_ACTION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.RuntimeRuleId), DbColumnName = "RUNTIME_RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.StepNo), DbColumnName = "STEP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.ActionType), DbColumnName = "ACTION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.ExecutorCode), DbColumnName = "EXECUTOR_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.ParamsJson), DbColumnName = "PARAMS_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.ExclusiveGroup), DbColumnName = "EXCLUSIVE_GROUP" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.SortNo), DbColumnName = "SORT_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimeAction), PropertyName = nameof(RuntimeAction.OnError), DbColumnName = "ON_ERROR" });
    }

    public static void ConfigureRuntimePackageState(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuntimePackageState).Name, "PR_RUNTIME_PACKAGE_STATE");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackageState), PropertyName = nameof(RuntimePackageState.StateCode), DbColumnName = "STATE_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackageState), PropertyName = nameof(RuntimePackageState.ActivePackageId), DbColumnName = "ACTIVE_PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackageState), PropertyName = nameof(RuntimePackageState.ActivePackageVersion), DbColumnName = "ACTIVE_PACKAGE_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackageState), PropertyName = nameof(RuntimePackageState.UpdatedAt), DbColumnName = "UPDATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuntimePackageState), PropertyName = nameof(RuntimePackageState.UpdatedBy), DbColumnName = "UPDATED_BY" });
    }

    /// <summary>
    /// 配置 PR_CHARGE_REQUEST_LOG 表映射。
    /// </summary>
    public static void ConfigureChargeRequest(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ChargeRequest).Name, "PR_CHARGE_REQUEST_LOG");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RequestId), DbColumnName = "REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RequestNo), DbColumnName = "REQUEST_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.BusinessRequestNo), DbColumnName = "BUSINESS_REQUEST_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RequestFingerprint), DbColumnName = "REQUEST_FINGERPRINT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.TraceId), DbColumnName = "TRACE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.CallType), DbColumnName = "CALL_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.BusinessStatus), DbColumnName = "BUSINESS_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.SourceSystem), DbColumnName = "SOURCE_SYSTEM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.SourceTerminal), DbColumnName = "SOURCE_TERMINAL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.PatientId), DbColumnName = "PATIENT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.VisitId), DbColumnName = "VISIT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ChargeScene), DbColumnName = "CHARGE_SCENE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ChargeNo), DbColumnName = "CHARGE_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ChargeDetailNo), DbColumnName = "CHARGE_DETAIL_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ResultGroupNo), DbColumnName = "RESULT_GROUP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.InputQty), DbColumnName = "INPUT_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.InputUnit), DbColumnName = "INPUT_UNIT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.BodyPartCode), DbColumnName = "BODY_PART_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.BusinessChargeTime), DbColumnName = "BUSINESS_CHARGE_TIME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.PriceVersion), DbColumnName = "PRICE_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RuntimePackageId), DbColumnName = "RUNTIME_PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RuntimePackageVersion), DbColumnName = "RUNTIME_PACKAGE_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RequestJson), DbColumnName = "REQUEST_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ResponseJson), DbColumnName = "RESPONSE_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.RequestAt), DbColumnName = "REQUEST_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ResponseAt), DbColumnName = "RESPONSE_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.IsSuccess), DbColumnName = "IS_SUCCESS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequest), PropertyName = nameof(ChargeRequest.ErrorMessage), DbColumnName = "ERROR_MESSAGE" });
    }

    /// <summary>
    /// 配置 PR_CHARGE_DISCOUNT_DETAIL 表映射。
    /// </summary>
    public static void ConfigureChargeDiscountDetail(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ChargeDiscountDetail).Name, "PR_CHARGE_DISCOUNT_DETAIL");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.DiscountId), DbColumnName = "DISCOUNT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.RequestId), DbColumnName = "REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.TraceId), DbColumnName = "TRACE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ChargeNo), DbColumnName = "CHARGE_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ChargeDetailNo), DbColumnName = "CHARGE_DETAIL_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.PatientId), DbColumnName = "PATIENT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.VisitId), DbColumnName = "VISIT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.RuleVersionNo), DbColumnName = "RULE_VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.RuntimePackageId), DbColumnName = "RUNTIME_PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.RuntimeRuleId), DbColumnName = "RUNTIME_RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.SourcePolicyVersionId), DbColumnName = "SOURCE_POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.SourceTemplateVersionId), DbColumnName = "SOURCE_TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.DiscountType), DbColumnName = "DISCOUNT_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ResultGroupNo), DbColumnName = "RESULT_GROUP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ParentDiscountId), DbColumnName = "PARENT_DISCOUNT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.PartSeq), DbColumnName = "PART_SEQ" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.OriginalQty), DbColumnName = "ORIGINAL_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ConvertedQty), DbColumnName = "CONVERTED_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.FinalQty), DbColumnName = "FINAL_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.UnitPrice), DbColumnName = "UNIT_PRICE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.OriginalAmt), DbColumnName = "ORIGINAL_AMT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.CalculatedAmt), DbColumnName = "CALCULATED_AMT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.FinalAmt), DbColumnName = "FINAL_AMT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.DiscountAmt), DbColumnName = "DISCOUNT_AMT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ReasonCode), DbColumnName = "REASON_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ReasonDesc), DbColumnName = "REASON_DESC" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.LimitBaseInfo), DbColumnName = "LIMIT_BASE_INFO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.OccurredAt), DbColumnName = "OCCURRED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ConfirmedBy), DbColumnName = "CONFIRMED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.CommittedAt), DbColumnName = "COMMITTED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.CancelledAt), DbColumnName = "CANCELLED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ExpiredAt), DbColumnName = "EXPIRED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeDiscountDetail), PropertyName = nameof(ChargeDiscountDetail.ReversedAt), DbColumnName = "REVERSED_AT" });
    }

    /// <summary>
    /// 配置 PR_CHARGE_TRACE_STEP 表映射。
    /// </summary>
    public static void ConfigureChargeTraceStep(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ChargeTraceStep).Name, "PR_CHARGE_TRACE_STEP");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.StepId), DbColumnName = "STEP_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.RequestId), DbColumnName = "REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.TraceId), DbColumnName = "TRACE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.StepNo), DbColumnName = "STEP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.StepName), DbColumnName = "STEP_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.StepType), DbColumnName = "STEP_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.RuleVersionNo), DbColumnName = "RULE_VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.RuntimePackageId), DbColumnName = "RUNTIME_PACKAGE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.RuntimeRuleId), DbColumnName = "RUNTIME_RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.SourcePolicyVersionId), DbColumnName = "SOURCE_POLICY_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.SourceTemplateVersionId), DbColumnName = "SOURCE_TEMPLATE_VERSION_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.InputSnapshot), DbColumnName = "INPUT_SNAPSHOT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.OutputSnapshot), DbColumnName = "OUTPUT_SNAPSHOT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.StepDesc), DbColumnName = "STEP_DESC" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeTraceStep), PropertyName = nameof(ChargeTraceStep.CreatedAt), DbColumnName = "CREATED_AT" });
    }

    /// <summary>
    /// 配置 PR_CHARGE_REVERSE_LOG 表映射。
    /// </summary>
    public static void ConfigureChargeReverseLog(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ChargeReverseLog).Name, "PR_CHARGE_REVERSE_LOG");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReverseId), DbColumnName = "REVERSE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.OriginalRequestId), DbColumnName = "ORIGINAL_REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReverseRequestId), DbColumnName = "REVERSE_REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ChargeNo), DbColumnName = "CHARGE_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReverseNo), DbColumnName = "REVERSE_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ChargeDetailNo), DbColumnName = "CHARGE_DETAIL_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.PartSeq), DbColumnName = "PART_SEQ" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReverseQty), DbColumnName = "REVERSE_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReverseAmt), DbColumnName = "REVERSE_AMT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReverseReason), DbColumnName = "REVERSE_REASON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReversedBy), DbColumnName = "REVERSED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeReverseLog), PropertyName = nameof(ChargeReverseLog.ReversedAt), DbColumnName = "REVERSED_AT" });
    }

    /// <summary>
    /// 配置 PR_RULE_APPROVAL 表映射。
    /// </summary>
    public static void ConfigureRuleApproval(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleApproval).Name, "PR_RULE_APPROVAL");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.ApprovalId), DbColumnName = "APPROVAL_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.ActionType), DbColumnName = "ACTION_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.ApprovalStatus), DbColumnName = "APPROVAL_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.SubmittedBy), DbColumnName = "SUBMITTED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.SubmittedAt), DbColumnName = "SUBMITTED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.ReviewedBy), DbColumnName = "REVIEWED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.ReviewedAt), DbColumnName = "REVIEWED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleApproval), PropertyName = nameof(RuleApproval.ReviewComment), DbColumnName = "REVIEW_COMMENT" });
    }

    /// <summary>
    /// 配置 PR_RULE_TEST_CASE 表映射。
    /// </summary>
    public static void ConfigureRuleTestCase(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleTestCase).Name, "PR_RULE_TEST_CASE");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.TestCaseId), DbColumnName = "TEST_CASE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.VersionNo), DbColumnName = "VERSION_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.CaseName), DbColumnName = "CASE_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.InputJson), DbColumnName = "INPUT_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.ExpectedJson), DbColumnName = "EXPECTED_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.IsEnabled), DbColumnName = "IS_ENABLED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.CreatedBy), DbColumnName = "CREATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestCase), PropertyName = nameof(RuleTestCase.CreatedAt), DbColumnName = "CREATED_AT" });
    }

    /// <summary>
    /// 配置 PR_RULE_TEST_RUN 表映射。
    /// </summary>
    public static void ConfigureRuleTestRun(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleTestRun).Name, "PR_RULE_TEST_RUN");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.TestRunId), DbColumnName = "TEST_RUN_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.TestCaseId), DbColumnName = "TEST_CASE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.ActualJson), DbColumnName = "ACTUAL_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.IsPass), DbColumnName = "IS_PASS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.RunAt), DbColumnName = "RUN_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.RunBy), DbColumnName = "RUN_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleTestRun), PropertyName = nameof(RuleTestRun.ErrorMessage), DbColumnName = "ERROR_MESSAGE" });
    }

    /// <summary>
    /// 注册所有实体的表名和列映射。
    /// </summary>
    public static void ApplyAllConfigs(SqlSugarClient db)
    {
        ConfigureRuleAggregate(db);
        ConfigureRuleVersion(db);
        ConfigureRuleCondition(db);
        ConfigureRuleAction(db);
        ConfigureRulePublish(db);
        ConfigureRuleChangeLog(db);
        ConfigureDict(db);
        ConfigureCacheVersion(db);
        ConfigureRuleCacheInvalidationOutbox(db);
        ConfigureFormulaDef(db);
        ConfigureItemGroup(db);
        ConfigureItemGroupDetail(db);
        ConfigureLimitLock(db);
        ConfigureLimitOccupy(db);
        ConfigurePriceMasterItem(db);
        ConfigureTemplateAggregate(db);
        ConfigureTemplateVersion(db);
        ConfigureTemplateParamDef(db);
        ConfigureTemplateStepDef(db);
        ConfigureTemplateScopeDef(db);
        ConfigurePolicyAggregate(db);
        ConfigurePolicyVersion(db);
        ConfigurePolicyBinding(db);
        ConfigurePolicyScope(db);
        ConfigurePolicyParam(db);
        ConfigurePolicyReview(db);
        ConfigureRuntimePackage(db);
        ConfigureRuntimePackagePolicy(db);
        ConfigureRuntimeRule(db);
        ConfigureRuntimeCondition(db);
        ConfigureRuntimeAction(db);
        ConfigureRuntimePackageState(db);
        ConfigureChargeRequest(db);
        ConfigureChargeDiscountDetail(db);
        ConfigureChargeTraceStep(db);
        ConfigureChargeReverseLog(db);
        ConfigureRuleApproval(db);
        ConfigureRuleTestCase(db);
        ConfigureRuleTestRun(db);
    }
}
