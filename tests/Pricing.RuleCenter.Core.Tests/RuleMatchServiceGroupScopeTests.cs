using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Infrastructure;
using Pricing.RuleCenter.Infrastructure.Repositories.Rules;
using SqlSugar;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class RuleMatchServiceGroupScopeTests
{
    [Fact]
    public async Task MatchAsync_should_include_group_scoped_rule_when_item_belongs_to_group()
    {
        await using var fixture = await SqlSugarFixture.CreateAsync();
        await fixture.SeedGroupScopedRuleAsync();

        var service = new RuleMatchService(
            new RuleMatchRepositories(
                new RuleHeaderRepository(fixture.Db, new SystemClock()),
                new StubRuleConditionRepository(),
                new StubRuleActionRepository(),
                new StubDictRepository()),
            new ConditionEvaluatorFactory(Array.Empty<Pricing.RuleCenter.Core.Interfaces.IRuleConditionEvaluator>()),
            NullLogger<RuleMatchService>.Instance);

        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM_A",
            InputQty = 1,
            UnitPrice = 100,
            BusinessChargeTime = new DateTime(2026, 5, 14, 9, 0, 0)
        };

        var (rules, actions) = await service.MatchAsync(context);

        Assert.Single(rules);
        Assert.Equal("RULE_GROUP_A", rules[0].RuleCode);
        Assert.Empty(actions);
    }

    private sealed class SqlSugarFixture : IAsyncDisposable
    {
        private readonly string _dbPath;

        private SqlSugarFixture(string dbPath, SqlSugarClient db)
        {
            _dbPath = dbPath;
            Db = db;
        }

        public SqlSugarClient Db { get; }

        public static async Task<SqlSugarFixture> CreateAsync()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"pricing-rule-center-tests-{Guid.NewGuid():N}.db");
            var db = new SqlSugarClient(new ConnectionConfig
            {
                DbType = DbType.Sqlite,
                ConnectionString = $"DataSource={dbPath}",
                IsAutoCloseConnection = true
            });

            db.CodeFirst.InitTables<RuleAggregate, ItemGroup, ItemGroupDetail>();
            return await Task.FromResult(new SqlSugarFixture(dbPath, db));
        }

        public async Task SeedGroupScopedRuleAsync()
        {
            await Db.Insertable(new ItemGroup
            {
                GroupId = 1,
                GroupCode = "GROUP_A",
                GroupName = "Group A",
                GroupType = "MUTUAL_EXCLUSIVE",
                IsEnabled = "Y",
                Remark = string.Empty,
                CreatedBy = string.Empty,
                UpdatedBy = string.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ExecuteCommandAsync();

            await Db.Insertable(new ItemGroupDetail
            {
                DetailId = 1,
                GroupId = 1,
                ItemCode = "ITEM_A",
                ItemName = "Item A",
                RoleType = "MEMBER",
                SortNo = 10,
                IsEnabled = "Y"
            }).ExecuteCommandAsync();

            await Db.Insertable(new RuleAggregate
            {
                RuleId = 1,
                RuleCode = "RULE_GROUP_A",
                RuleName = "Group scoped rule",
                RuleCategory = "MIXED",
                RuleScope = "GROUP",
                ItemCode = string.Empty,
                GroupCode = "GROUP_A",
                Priority = 10,
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                ItemName = string.Empty,
                RollbackMode = string.Empty,
                Remark = string.Empty,
                CreatedBy = string.Empty,
                UpdatedBy = string.Empty,
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ExecuteCommandAsync();
        }

        public ValueTask DisposeAsync()
        {
            Db.Dispose();
            try
            {
                if (File.Exists(_dbPath))
                {
                    File.Delete(_dbPath);
                }
            }
            catch (IOException)
            {
                // SQLite 文件在测试进程结束前可能仍被短暂占用，这里只做最佳努力清理。
            }

            return ValueTask.CompletedTask;
        }
    }
}
