using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则发布流水仓储实现，封装 PR_RULE_PUBLISH 的查询和写入。
/// </summary>
/// <remarks>
/// 发布流水记录发布、停用、回滚这些状态机事件。它用于审计和页面展示，不直接决定计价是否生效；
/// 真实生效状态仍以规则主档和版本表为准。
/// </remarks>
public sealed class RulePublishRepository : IRulePublishRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和发布流水表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则发布流水仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public RulePublishRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则的发布流水。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>按发布时间倒序排列的发布流水集合。</returns>
    public async Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RulePublish>()
            .Where(p => p.RuleId == ruleId)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 插入发布流水。
    /// </summary>
    /// <param name="entity">待写入的发布流水实体。</param>
    /// <returns>Oracle 序列生成的发布流水主键。</returns>
    public async Task<long> InsertAsync(RulePublish entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_PUBLISH.NEXTVAL FROM DUAL");
        entity.PublishId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}
