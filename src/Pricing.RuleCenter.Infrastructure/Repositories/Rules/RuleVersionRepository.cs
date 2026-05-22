using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则版本仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_VERSION 表的全部数据访问操作，包括：按主键查询、按规则+版本号查询、
/// 按规则查询全部版本、插入、更新状态。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_RULE_VERSION 是规则发布状态机的重要组成部分。每条规则主档（PR_RULE_HEADER）
/// 可以有多个版本（1:N 关系），版本通过 RULE_ID + VERSION_NO 联合唯一标识。
/// 版本状态（VERSION_STATUS）决定该版本是否可参与计价匹配。
/// </para>
/// <para>
/// 【版本状态枚举】
///   - DRAFT        — 草稿，可编辑条件和动作
///   - PUBLISHED    — 已发布，可参与计价匹配
///   - DISABLED     — 已停用，不参与匹配
///   - ROLLED_BACK  — 已回滚，不参与匹配
/// </para>
/// <para>
/// 【状态机边界】
/// 仓储只提供按主键或业务键定位版本以及写入目标状态的能力，
/// 不在这里判断 DRAFT → PUBLISHED → DISABLED → ROLLED_BACK 之间的合法流转。
/// 状态流转合法性由应用服务层的发布状态机保证。
/// </para>
/// </remarks>
public sealed class RuleVersionRepository : IRuleVersionRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和规则版本表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则版本仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleVersionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按版本主键读取规则版本。
    /// </summary>
    /// <param name="versionId">版本主键（由 Oracle 序列 SEQ_PR_RULE_VERSION 生成）。</param>
    /// <returns>规则版本实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_VERSION WHERE VERSION_ID = :versionId
    /// </code>
    /// 【使用场景】版本详情查看、发布前校验。
    /// </remarks>
    public async Task<RuleVersion?> GetByIdAsync(long versionId)
    {
        return await _db.Queryable<RuleVersion>()
            .InSingleAsync(versionId);
    }

    /// <summary>
    /// 按规则主键和版本号读取规则版本。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <param name="versionNo">规则内版本号，从 1 开始递增。</param>
    /// <returns>规则版本实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_VERSION
    /// WHERE RULE_ID = :ruleId AND VERSION_NO = :versionNo
    /// </code>
    /// 【使用场景】计价引擎通过规则主档的 CURRENT_VERSION 定位当前生效版本，
    /// 再读取该版本下的条件和动作。
    /// </remarks>
    public async Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return await _db.Queryable<RuleVersion>()
            .FirstAsync(v => v.RuleId == ruleId && v.VersionNo == versionNo);
    }

    /// <summary>
    /// 按规则主键和版本号读取规则版本并执行数据库行锁。
    /// </summary>
    /// <remarks>
    /// 使用 Oracle `SELECT ... FOR UPDATE` 把目标版本锁到当前事务，避免并发发布/回滚同时修改同一版本状态。
    /// 必须在外层事务已开启时调用。
    /// </remarks>
    public async Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo)
    {
        var rows = await _db.Ado.SqlQueryAsync<RuleVersion>(
            "SELECT * FROM PR_RULE_VERSION WHERE RULE_ID = :RuleId AND VERSION_NO = :VersionNo FOR UPDATE",
            new { RuleId = ruleId, VersionNo = versionNo });
        return rows.FirstOrDefault();
    }

    /// <summary>
    /// 读取某条规则的全部版本，按版本号倒序排列。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <returns>按版本号倒序排列的版本集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_VERSION
    /// WHERE RULE_ID = :ruleId
    /// ORDER BY VERSION_NO DESC
    /// </code>
    /// 【排序策略】倒序排列使最新版本出现在列表最前面，符合版本管理页面的浏览习惯。
    /// 【使用场景】规则配置页面的版本历史列表。
    /// </remarks>
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
    /// <returns>Oracle 序列生成的版本主键（VERSION_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_VERSION.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_VERSION (...) VALUES (...)
    /// </code>
    /// 【约束】RULE_ID + VERSION_NO 的唯一性由数据库约束保证。
    /// 新版本的 VERSION_NO 通常由应用服务按 MAX(VERSION_NO) + 1 计算。
    /// </remarks>
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
    /// <param name="status">目标版本状态（DRAFT / PUBLISHED / DISABLED / ROLLED_BACK）。</param>
    /// <returns>是否至少更新了一行（true 表示成功，false 表示记录不存在）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_RULE_VERSION SET VERSION_STATUS = :status WHERE VERSION_ID = :versionId
    /// </code>
    /// 【字段保护】只更新 VERSION_STATUS 字段，避免发布状态机误改版本快照或生效期。
    /// 版本快照（条件和动作的配置内容）在发布后应不可变。
    /// 【事务要求】版本状态更新通常与规则主档状态更新、发布流水写入在同一事务中执行。
    /// </remarks>
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
