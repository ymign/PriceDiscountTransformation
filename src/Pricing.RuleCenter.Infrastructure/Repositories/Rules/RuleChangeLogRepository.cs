using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则变更日志仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_CHANGE_LOG 表的查询和写入操作。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_RULE_CHANGE_LOG 保存面向人的变更摘要，如"发布版本2，新增折价条件"。
/// 每条记录包含：变更类型、变更描述、操作人、变更时间。
/// </para>
/// <para>
/// 【与发布流水的区别】
///   - PR_RULE_PUBLISH：记录状态机事件（发布/停用/回滚），偏技术审计
///   - PR_RULE_CHANGE_LOG：记录面向人的变更摘要，偏业务审计
/// 两者互补，共同构成完整的规则变更审计记录。
/// </para>
/// <para>
/// 【追溯链路】
/// 变更日志是"规则变更链"的一部分。排查"什么时候谁发布/停用了规则"时，
/// 先从变更日志获取概览，再从发布流水获取详细状态变更。
/// </para>
/// <para>
/// 【设计原则】
/// 变更日志不参与计价匹配，但对审计和问题排查非常关键。
/// 仓储只负责读写，变更摘要的内容由应用服务构造。
/// </para>
/// </remarks>
public sealed class RuleChangeLogRepository : IRuleChangeLogRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和规则变更日志表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则变更日志仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleChangeLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则的变更日志，按变更时间倒序排列。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <returns>按变更时间倒序排列的日志集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_CHANGE_LOG
    /// WHERE RULE_ID = :ruleId
    /// ORDER BY CHANGED_AT DESC
    /// </code>
    /// 【排序策略】倒序排列使最近一次变更出现在列表最前面。
    /// 【使用场景】规则配置页面的变更历史列表、审计追溯。
    /// </remarks>
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
    /// <returns>Oracle 序列生成的变更日志主键（CHANGE_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_CHANGE_LOG.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_CHANGE_LOG (...) VALUES (...)
    /// </code>
    /// 【事务边界】本方法不开启事务。调用方（发布服务或配置服务）应在更大的事务中调用本方法，
    /// 保证变更日志与发布流水、规则状态的原子写入。
    /// </remarks>
    public async Task<long> InsertAsync(RuleChangeLog entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_CHANGE_LOG.NEXTVAL FROM DUAL");
        entity.ChangeId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}