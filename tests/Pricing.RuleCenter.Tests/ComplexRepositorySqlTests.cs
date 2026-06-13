using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Infrastructure;
using Pricing.RuleCenter.Infrastructure.Database;
using Pricing.RuleCenter.Infrastructure.Repositories.Rules;
using SqlSugar;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ComplexRepositorySqlTests
{
    [Fact]
    public async Task RuleHeaderRepository_GetByItemCodeAsync_ShouldUseSingleSelectForGroupScopedMatch()
    {
        await using var fixture = await SqliteRepositoryFixture.CreateAsync();
        await fixture.SeedRuleHeaderGroupScopedDataAsync();
        fixture.ClearSqlLogs();

        var repository = new RuleHeaderRepository(fixture.Db, new SystemClock());

        var items = await repository.GetByItemCodeAsync("ITEM_A");

        Assert.Equal(new[] { "RULE_GROUP_A" }, items.Select(item => item.RuleCode).ToArray());
        Assert.Equal(1, fixture.CountSelectStatements());
    }

    private sealed class SqliteRepositoryFixture : IAsyncDisposable
    {
        private readonly string _dbPath;
        private readonly List<string> _sqlLogs = new();

        private SqliteRepositoryFixture(string dbPath, SqlSugarClient db)
        {
            _dbPath = dbPath;
            Db = db;
        }

        public SqlSugarClient Db { get; }

        public static async Task<SqliteRepositoryFixture> CreateAsync()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"pricing-repository-sql-{Guid.NewGuid():N}.db");
            var db = new SqlSugarClient(new ConnectionConfig
            {
                DbType = DbType.Sqlite,
                ConnectionString = $"DataSource={dbPath}",
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = EntityTypeConfigs.CreateExternalServices()
            });

            EntityTypeConfigs.ApplyAllConfigs(db);
            db.CodeFirst.InitTables<RuleAggregate, ItemGroup, ItemGroupDetail>();

            var fixture = new SqliteRepositoryFixture(dbPath, db);
            db.Aop.OnLogExecuting = (sql, _) => fixture._sqlLogs.Add(sql);
            return await Task.FromResult(fixture);
        }

        public void ClearSqlLogs()
        {
            _sqlLogs.Clear();
        }

        public int CountSelectStatements()
        {
            return _sqlLogs.Count(sql =>
                sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase));
        }

        public async Task SeedRuleHeaderGroupScopedDataAsync()
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
                SortNo = 1,
                IsEnabled = "Y"
            }).ExecuteCommandAsync();

            await Db.Insertable(new RuleAggregate
            {
                RuleId = 11,
                RuleCode = "RULE_GROUP_A",
                RuleName = "Group scoped rule",
                RuleCategory = "MIXED",
                RuleScope = "GROUP",
                ItemCode = string.Empty,
                ItemName = string.Empty,
                GroupCode = "GROUP_A",
                Priority = 10,
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31),
                RollbackMode = string.Empty,
                Remark = string.Empty,
                CreatedBy = string.Empty,
                UpdatedBy = string.Empty,
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
            }

            return ValueTask.CompletedTask;
        }
    }
}
