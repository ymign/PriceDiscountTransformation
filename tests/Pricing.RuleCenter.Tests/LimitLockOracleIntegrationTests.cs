using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Infrastructure.Repositories.Quota;
using SqlSugar;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class LimitLockOracleIntegrationTests
{
    [Fact]
    [Trait("Category", "OracleIntegration")]
    public async Task AcquireLockAsync_ShouldSerializeConcurrentAccessOnSameLockKey()
    {
        var connectionString = Environment.GetEnvironmentVariable("PRICING_ORACLE_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var lockKey = $"UT_LOCK_{Guid.NewGuid():N}";
        using var db1 = CreateOracleClient(connectionString);
        using var db2 = CreateOracleClient(connectionString);
        using var cleanupDb = CreateOracleClient(connectionString);
        var repo1 = new LimitLockRepository(db1);
        var repo2 = new LimitLockRepository(db2);
        var expireAt = new DateTime(2099, 1, 1, 0, 0, 0);

        try
        {
            await db1.Ado.BeginTranAsync();
            await db2.Ado.BeginTranAsync();

            await repo1.AcquireLockAsync(lockKey, expireAt);

            var secondAcquireTask = repo2.AcquireLockAsync(lockKey, expireAt);
            await Task.Delay(500);
            Assert.False(secondAcquireTask.IsCompleted, "第二个事务应在第一个事务提交前等待行锁释放。");

            await db1.Ado.CommitTranAsync();

            var completedTask = await Task.WhenAny(secondAcquireTask, Task.Delay(5000));
            Assert.Same(secondAcquireTask, completedTask);
            await secondAcquireTask;

            await db2.Ado.CommitTranAsync();
        }
        finally
        {
            await SafeRollbackAsync(db1);
            await SafeRollbackAsync(db2);

            await cleanupDb.Deleteable<LimitLock>()
                .Where(item => item.LockKey == lockKey)
                .ExecuteCommandAsync();
        }
    }

    private static SqlSugarClient CreateOracleClient(string connectionString)
    {
        return new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = DbType.Oracle,
            IsAutoCloseConnection = false
        });
    }

    private static async Task SafeRollbackAsync(SqlSugarClient db)
    {
        try
        {
            await db.Ado.RollbackTranAsync();
        }
        catch
        {
            // 忽略清理阶段的回滚异常，避免覆盖真实断言结果。
        }
    }
}
