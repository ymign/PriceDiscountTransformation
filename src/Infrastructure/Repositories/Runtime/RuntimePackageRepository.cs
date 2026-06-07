using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Runtime;

/// <summary>
/// 运行时包仓储。
/// </summary>
public sealed class RuntimePackageRepository : IRuntimePackageRepository
{
    private readonly ISqlSugarClient _db;

    public RuntimePackageRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<RuntimePackage?> GetByIdAsync(long packageId)
    {
        return await _db.Queryable<RuntimePackage>()
            .InSingleAsync(packageId);
    }

    public async Task<IReadOnlyList<RuntimePackage>> GetHistoryAsync(int take)
    {
        return await _db.Queryable<RuntimePackage>()
            .OrderBy(package => package.PackageVersion, OrderByType.Desc)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<RuntimePackagePolicy>> GetPackagePoliciesAsync(long packageId)
    {
        return await _db.Queryable<RuntimePackagePolicy>()
            .Where(item => item.PackageId == packageId)
            .OrderBy(item => item.PackagePolicyId)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(RuntimePackage entity)
    {
        var packageId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RUNTIME_PACKAGE.NEXTVAL FROM DUAL");
        entity.PackageId = packageId;
        if (entity.PackageVersion <= 0)
        {
            entity.PackageVersion = packageId;
        }

        await _db.Insertable(entity).ExecuteCommandAsync();
        return packageId;
    }

    public async Task UpdateAsync(RuntimePackage entity)
    {
        await _db.Updateable(entity)
            .Where(package => package.PackageId == entity.PackageId)
            .ExecuteCommandAsync();
    }
}
