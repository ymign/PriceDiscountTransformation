using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Infrastructure.Database;
using SqlSugar;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class SqlSugarEntityTypeConfigTests
{
    [Fact]
    public void EntityTypeConfigs_MarksRepositoryPrimaryKeys()
    {
        using var db = CreateClient();

        AssertPrimaryKey<RuleHeader>(db, nameof(RuleHeader.RuleId));
        AssertPrimaryKey<RuleVersion>(db, nameof(RuleVersion.VersionId));
        AssertPrimaryKey<RuleCondition>(db, nameof(RuleCondition.ConditionId));
        AssertPrimaryKey<RuleAction>(db, nameof(RuleAction.ActionId));
        AssertPrimaryKey<RulePublish>(db, nameof(RulePublish.PublishId));
        AssertPrimaryKey<RuleChangeLog>(db, nameof(RuleChangeLog.ChangeId));
        AssertPrimaryKey<Dict>(db, nameof(Dict.DictId));
        AssertPrimaryKey<CacheVersion>(db, nameof(CacheVersion.CacheScope));
        AssertPrimaryKey<RuleCacheInvalidationOutbox>(db, nameof(RuleCacheInvalidationOutbox.OutboxId));
        AssertPrimaryKey<FormulaDef>(db, nameof(FormulaDef.FormulaId));
        AssertPrimaryKey<ItemGroup>(db, nameof(ItemGroup.GroupId));
        AssertPrimaryKey<ItemGroupDetail>(db, nameof(ItemGroupDetail.DetailId));
        AssertPrimaryKey<LimitLock>(db, nameof(LimitLock.LockKey));
        AssertPrimaryKey<LimitOccupy>(db, nameof(LimitOccupy.OccupyId));
        AssertPrimaryKey<ChargeRequestLog>(db, nameof(ChargeRequestLog.RequestId));
        AssertPrimaryKey<ChargeDiscountDetail>(db, nameof(ChargeDiscountDetail.DiscountId));
        AssertPrimaryKey<ChargeTraceStep>(db, nameof(ChargeTraceStep.StepId));
        AssertPrimaryKey<ChargeReverseLog>(db, nameof(ChargeReverseLog.ReverseId));
        AssertPrimaryKey<RuleApproval>(db, nameof(RuleApproval.ApprovalId));
        AssertPrimaryKey<RuleTestCase>(db, nameof(RuleTestCase.TestCaseId));
        AssertPrimaryKey<RuleTestRun>(db, nameof(RuleTestRun.TestRunId));
        AssertPrimaryKey<TemplateAggregate>(db, nameof(TemplateAggregate.TemplateId));
        AssertPrimaryKey<TemplateVersion>(db, nameof(TemplateVersion.TemplateVersionId));
        AssertPrimaryKey<TemplateParamDef>(db, nameof(TemplateParamDef.ParamDefId));
        AssertPrimaryKey<TemplateStepDef>(db, nameof(TemplateStepDef.StepDefId));
        AssertPrimaryKey<TemplateScopeDef>(db, nameof(TemplateScopeDef.ScopeDefId));
        AssertPrimaryKey<PolicyAggregate>(db, nameof(PolicyAggregate.PolicyId));
        AssertPrimaryKey<PolicyVersion>(db, nameof(PolicyVersion.PolicyVersionId));
        AssertPrimaryKey<PolicyBinding>(db, nameof(PolicyBinding.PolicyBindingId));
        AssertPrimaryKey<PolicyScope>(db, nameof(PolicyScope.PolicyScopeId));
        AssertPrimaryKey<PolicyParam>(db, nameof(PolicyParam.PolicyParamId));
        AssertPrimaryKey<PolicyReview>(db, nameof(PolicyReview.ReviewId));
    }

    [Fact]
    public void EntityTypeConfigs_MapsNewRuntimeTraceColumns()
    {
        using var db = CreateClient();

        AssertMappedColumn<ChargeRequestLog>(db, nameof(ChargeRequestLog.RuntimePackageId), "RUNTIME_PACKAGE_ID");
        AssertMappedColumn<ChargeRequestLog>(db, nameof(ChargeRequestLog.RuntimePackageVersion), "RUNTIME_PACKAGE_VERSION");
        AssertMappedColumn<ChargeTraceStep>(db, nameof(ChargeTraceStep.RuntimePackageId), "RUNTIME_PACKAGE_ID");
        AssertMappedColumn<ChargeTraceStep>(db, nameof(ChargeTraceStep.RuntimeRuleId), "RUNTIME_RULE_ID");
        AssertMappedColumn<ChargeTraceStep>(db, nameof(ChargeTraceStep.SourcePolicyVersionId), "SOURCE_POLICY_VERSION_ID");
        AssertMappedColumn<ChargeTraceStep>(db, nameof(ChargeTraceStep.SourceTemplateVersionId), "SOURCE_TEMPLATE_VERSION_ID");
        AssertMappedColumn<ChargeDiscountDetail>(db, nameof(ChargeDiscountDetail.RuntimePackageId), "RUNTIME_PACKAGE_ID");
        AssertMappedColumn<ChargeDiscountDetail>(db, nameof(ChargeDiscountDetail.RuntimeRuleId), "RUNTIME_RULE_ID");
        AssertMappedColumn<ChargeDiscountDetail>(db, nameof(ChargeDiscountDetail.SourcePolicyVersionId), "SOURCE_POLICY_VERSION_ID");
        AssertMappedColumn<ChargeDiscountDetail>(db, nameof(ChargeDiscountDetail.SourceTemplateVersionId), "SOURCE_TEMPLATE_VERSION_ID");
    }

    private static SqlSugarClient CreateClient()
    {
        var db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = "Data Source=unused",
            DbType = DbType.Oracle,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            ConfigureExternalServices = EntityTypeConfigs.CreateExternalServices()
        });

        EntityTypeConfigs.ApplyAllConfigs(db);
        return db;
    }

    private static void AssertPrimaryKey<T>(SqlSugarClient db, string propertyName)
    {
        var entityInfo = db.EntityMaintenance.GetEntityInfo<T>();
        Assert.Contains(entityInfo.Columns, column =>
            column.PropertyName == propertyName && column.IsPrimarykey);
    }

    private static void AssertMappedColumn<T>(SqlSugarClient db, string propertyName, string dbColumnName)
    {
        var entityInfo = db.EntityMaintenance.GetEntityInfo<T>();
        Assert.Contains(entityInfo.Columns, column =>
            column.PropertyName == propertyName &&
            string.Equals(column.DbColumnName, dbColumnName, StringComparison.Ordinal));
    }
}
