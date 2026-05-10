using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则发布流水仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_PUBLISH 表的查询和写入操作。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_RULE_PUBLISH 记录规则生命周期中的发布、停用、回滚等状态机事件。
/// 每条记录包含：发布类型（PUBLISH / DISABLE / ROLLBACK）、目标版本号、
/// 操作人、发布时间和备注。
/// 它用于审计和页面展示，不直接决定计价是否生效。
/// </para>
/// <para>
/// 【与规则主档/版本表的关系】
/// 真实生效状态以 PR_RULE_HEADER.STATUS 和 PR_RULE_VERSION.VERSION_STATUS 为准。
/// 发布流水只是这些状态变化的审计痕迹。发布时三张表通常在同一事务中更新。
/// </para>
/// <para>
/// 【追溯链路】
/// 发布流水是"规则变更链"的一部分，配合 PR_RULE_CHANGE_LOG 共同构成
/// "什么时候谁发布/停用了规则"的完整审计记录。
/// </para>
/// </remarks>
public sealed class RulePublishRepository : IRulePublishRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和发布流水表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则发布流水仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RulePublishRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则的发布流水，按发布时间倒序排列。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <returns>按发布时间倒序排列的发布流水集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_PUBLISH
    /// WHERE RULE_ID = :ruleId
    /// ORDER BY PUBLISHED_AT DESC
    /// </code>
    /// 【排序策略】倒序排列使最近一次发布操作出现在列表最前面，便于查看最新状态变更。
    /// 【使用场景】规则配置页面的发布历史列表、审计追溯。
    /// </remarks>
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
    /// <returns>Oracle 序列生成的发布流水主键（PUBLISH_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_PUBLISH.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_PUBLISH (...) VALUES (...)
    /// </code>
    /// 【事务边界】本方法不开启事务。调用方（发布服务）应在更大的事务中调用本方法，
    /// 保证发布流水与规则主档状态、版本状态的原子更新。
    /// </remarks>
    public async Task<long> InsertAsync(RulePublish entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_PUBLISH.NEXTVAL FROM DUAL");
        entity.PublishId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}
