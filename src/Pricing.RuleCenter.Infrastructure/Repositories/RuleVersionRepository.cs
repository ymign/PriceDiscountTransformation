using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则版本仓储实现，封装 PR_RULE_VERSION 的读取、插入和状态更新。
/// </summary>
/// <remarks>
/// 版本状态是规则发布状态机的重要组成部分。仓储只提供按主键或业务键定位版本以及写入目标状态的能力，
/// 不在这里判断 DRAFT、PUBLISHED、DISABLED、ROLLED_BACK 之间的合法流转。
/// </remarks>
public sealed class RuleVersionRepository : IRuleVersionRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和规则版本表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则版本仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public RuleVersionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按版本主键读取规则版本。
    /// </summary>
    /// <param name="versionId">版本主键。</param>
    /// <returns>规则版本实体；不存在时返回 <c>null</c>。</returns>
    public async Task<RuleVersion?> GetByIdAsync(long versionId)
    {
        return await _db.Queryable<RuleVersion>()
            .InSingleAsync(versionId);
    }

    /// <summary>
    /// 按规则主键和版本号读取规则版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则内版本号。</param>
    /// <returns>规则版本实体；不存在时返回 <c>null</c>。</returns>
    public async Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return await _db.Queryable<RuleVersion>()
            .FirstAsync(v => v.RuleId == ruleId && v.VersionNo == versionNo);
    }

    /// <summary>
    /// 读取某条规则的全部版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>按版本号倒序排列的版本集合。</returns>
    public async Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleVersion>()
            .Where(v => v.RuleId == ruleId)
            .OrderByDescending(v => v.VersionNo)
            .ToListAsync();
    }

    /// <summary>
    /// 插入规则版本。
    /// </summary>
    /// <param name="entity">待写入的规则版本实体。</param>
    /// <returns>Oracle 序列生成的版本主键。</returns>
    public async Task<long> InsertAsync(RuleVersion entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_VERSION.NEXTVAL FROM DUAL");
        entity.VersionId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新规则版本状态。
    /// </summary>
    /// <param name="versionId">版本主键。</param>
    /// <param name="status">目标版本状态。</param>
    /// <returns>是否至少更新了一行。</returns>
    public async Task<bool> UpdateStatusAsync(long versionId, string status)
    {
        // 只更新状态字段，避免发布状态机误改版本快照或生效期。
        var rows = await _db.Updateable<RuleVersion>()
            .SetColumns(v => v.VersionStatus == status)
            .Where(v => v.VersionId == versionId)
            .ExecuteCommandAsync();
        return rows > 0;
    }
}
