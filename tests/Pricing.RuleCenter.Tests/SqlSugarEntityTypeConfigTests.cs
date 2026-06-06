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
}
