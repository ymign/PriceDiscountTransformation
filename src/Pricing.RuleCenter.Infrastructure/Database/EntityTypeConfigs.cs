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
            [typeof(RuleHeader)] = nameof(RuleHeader.RuleId),
            [typeof(RuleVersion)] = nameof(RuleVersion.VersionId),
            [typeof(RuleCondition)] = nameof(RuleCondition.ConditionId),
            [typeof(RuleAction)] = nameof(RuleAction.ActionId),
            [typeof(RulePublish)] = nameof(RulePublish.PublishId),
            [typeof(RuleChangeLog)] = nameof(RuleChangeLog.ChangeId),
            [typeof(Dict)] = nameof(Dict.DictId),
            [typeof(FormulaDef)] = nameof(FormulaDef.FormulaId),
            [typeof(ItemGroup)] = nameof(ItemGroup.GroupId),
            [typeof(ItemGroupDetail)] = nameof(ItemGroupDetail.DetailId),
            [typeof(LimitLock)] = nameof(LimitLock.LockKey),
            [typeof(LimitOccupy)] = nameof(LimitOccupy.OccupyId),
            [typeof(ChargeRequestLog)] = nameof(ChargeRequestLog.RequestId),
            [typeof(ChargeDiscountDetail)] = nameof(ChargeDiscountDetail.DiscountId),
            [typeof(ChargeTraceStep)] = nameof(ChargeTraceStep.StepId),
            [typeof(ChargeReverseLog)] = nameof(ChargeReverseLog.ReverseId),
            [typeof(RuleApproval)] = nameof(RuleApproval.ApprovalId),
            [typeof(RuleTestCase)] = nameof(RuleTestCase.TestCaseId),
            [typeof(RuleTestRun)] = nameof(RuleTestRun.TestRunId)
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
    public static void ConfigureRuleHeader(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(RuleHeader).Name, "PR_RULE_HEADER");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.RuleId), DbColumnName = "RULE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.RuleCode), DbColumnName = "RULE_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.RuleName), DbColumnName = "RULE_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.RuleCategory), DbColumnName = "RULE_CATEGORY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.RuleScope), DbColumnName = "RULE_SCOPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.GroupCode), DbColumnName = "GROUP_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.Priority), DbColumnName = "PRIORITY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.CurrentVersion), DbColumnName = "CURRENT_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.Status), DbColumnName = "STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.IsEnabled), DbColumnName = "IS_ENABLED" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.EffectiveFrom), DbColumnName = "EFFECTIVE_FROM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.EffectiveTo), DbColumnName = "EFFECTIVE_TO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.RollbackMode), DbColumnName = "ROLLBACK_MODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.Remark), DbColumnName = "REMARK" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.CreatedBy), DbColumnName = "CREATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.CreatedAt), DbColumnName = "CREATED_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.UpdatedBy), DbColumnName = "UPDATED_BY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(RuleHeader), PropertyName = nameof(RuleHeader.UpdatedAt), DbColumnName = "UPDATED_AT" });
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
    }

    /// <summary>
    /// 配置 PR_CHARGE_REQUEST_LOG 表映射。
    /// </summary>
    public static void ConfigureChargeRequestLog(SqlSugarClient db)
    {
        db.MappingTables.Add(typeof(ChargeRequestLog).Name, "PR_CHARGE_REQUEST_LOG");
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.RequestId), DbColumnName = "REQUEST_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.RequestNo), DbColumnName = "REQUEST_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.BusinessRequestNo), DbColumnName = "BUSINESS_REQUEST_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.RequestFingerprint), DbColumnName = "REQUEST_FINGERPRINT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.TraceId), DbColumnName = "TRACE_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.CallType), DbColumnName = "CALL_TYPE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.BusinessStatus), DbColumnName = "BUSINESS_STATUS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.SourceSystem), DbColumnName = "SOURCE_SYSTEM" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.SourceTerminal), DbColumnName = "SOURCE_TERMINAL" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.PatientId), DbColumnName = "PATIENT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.VisitId), DbColumnName = "VISIT_ID" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ChargeScene), DbColumnName = "CHARGE_SCENE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ChargeNo), DbColumnName = "CHARGE_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ChargeDetailNo), DbColumnName = "CHARGE_DETAIL_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ResultGroupNo), DbColumnName = "RESULT_GROUP_NO" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ItemCode), DbColumnName = "ITEM_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ItemName), DbColumnName = "ITEM_NAME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.InputQty), DbColumnName = "INPUT_QTY" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.InputUnit), DbColumnName = "INPUT_UNIT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.BodyPartCode), DbColumnName = "BODY_PART_CODE" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.BusinessChargeTime), DbColumnName = "BUSINESS_CHARGE_TIME" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.PriceVersion), DbColumnName = "PRICE_VERSION" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.RequestJson), DbColumnName = "REQUEST_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ResponseJson), DbColumnName = "RESPONSE_JSON" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.RequestAt), DbColumnName = "REQUEST_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ResponseAt), DbColumnName = "RESPONSE_AT" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.IsSuccess), DbColumnName = "IS_SUCCESS" });
        db.MappingColumns.Add(new MappingColumn { EntityName = nameof(ChargeRequestLog), PropertyName = nameof(ChargeRequestLog.ErrorMessage), DbColumnName = "ERROR_MESSAGE" });
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
        ConfigureRuleHeader(db);
        ConfigureRuleVersion(db);
        ConfigureRuleCondition(db);
        ConfigureRuleAction(db);
        ConfigureRulePublish(db);
        ConfigureRuleChangeLog(db);
        ConfigureDict(db);
        ConfigureFormulaDef(db);
        ConfigureItemGroup(db);
        ConfigureItemGroupDetail(db);
        ConfigureLimitLock(db);
        ConfigureLimitOccupy(db);
        ConfigurePriceMasterItem(db);
        ConfigureChargeRequestLog(db);
        ConfigureChargeDiscountDetail(db);
        ConfigureChargeTraceStep(db);
        ConfigureChargeReverseLog(db);
        ConfigureRuleApproval(db);
        ConfigureRuleTestCase(db);
        ConfigureRuleTestRun(db);
    }
}
