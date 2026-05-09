using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则变更日志仓储实现，封装 PR_RULE_CHANGE_LOG 的查询和写入。
/// </summary>
/// <remarks>
/// 变更日志保存面向人的变更摘要，和发布流水相比更适合在配置页面展示。它不参与计价匹配，
/// 但对排查“什么时候谁发布/停用了规则”非常关键。
/// </remarks>
public sealed class RuleChangeLogRepository : IRuleChangeLogRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和规则变更日志表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则变更日志仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public RuleChangeLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则的变更日志。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>按变更时间倒序排列的日志集合。</returns>
    public async Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleChangeLog>()
            .Where(c => c.RuleId == ruleId)
            .OrderByDescending(c => c.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 插入规则变更日志。
    /// </summary>
    /// <param name="entity">待写入的变更日志实体。</param>
    /// <returns>Oracle 序列生成的变更日志主键。</returns>
    public async Task<long> InsertAsync(RuleChangeLog entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_CHANGE_LOG.NEXTVAL FROM DUAL");
        entity.ChangeId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}
