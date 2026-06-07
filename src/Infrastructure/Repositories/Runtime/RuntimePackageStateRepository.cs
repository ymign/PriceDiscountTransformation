using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Runtime;

/// <summary>
/// 当前活动运行时包状态仓储。
/// </summary>
public sealed class RuntimePackageStateRepository : IRuntimePackageStateRepository
{
    private readonly ISqlSugarClient _db;

    public RuntimePackageStateRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<RuntimePackageState?> GetActiveAsync()
    {
        return await _db.Queryable<RuntimePackageState>()
            .InSingleAsync(RuntimePackageStateCodes.Active);
    }

    public async Task<RuntimePackageState?> GetActiveForUpdateAsync()
    {
        var rows = await _db.Ado.SqlQueryAsync<RuntimePackageState>(
            "SELECT * FROM PR_RUNTIME_PACKAGE_STATE WHERE STATE_CODE = :StateCode FOR UPDATE",
            new { StateCode = RuntimePackageStateCodes.Active });
        return rows.FirstOrDefault();
    }

    public async Task UpsertAsync(RuntimePackageState entity)
    {
        var existing = await GetActiveAsync();
        if (existing is null)
        {
            await _db.Insertable(entity).ExecuteCommandAsync();
            return;
        }

        await _db.Updateable(entity)
            .Where(state => state.StateCode == entity.StateCode)
            .ExecuteCommandAsync();
    }
}
