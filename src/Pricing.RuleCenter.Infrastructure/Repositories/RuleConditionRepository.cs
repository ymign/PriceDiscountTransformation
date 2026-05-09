using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则条件仓储实现，封装 PR_RULE_CONDITION 的版本内读取、批量写入和清空。
/// </summary>
/// <remarks>
/// 条件集合决定一条规则是否命中。读取时只返回启用条件，并先按条件组、再按组内顺序排序，
/// 以便匹配服务能稳定地还原配置人员定义的判断顺序。
/// </remarks>
public sealed class RuleConditionRepository : IRuleConditionRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和规则条件表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则条件仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public RuleConditionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则版本下的启用条件。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>按条件组和组内顺序排列的条件集合。</returns>
    public async Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 只返回启用条件，停用条件不参与规则匹配；排序稳定性对追踪步骤解释很重要。
        return await _db.Queryable<RuleCondition>()
            .Where(c => c.RuleId == ruleId && c.VersionNo == versionNo && c.IsEnabled == "Y")
            .OrderBy(c => c.ConditionGroup)
            .OrderBy(c => c.SortNo)
            .ToListAsync();
    }

    /// <summary>
    /// 批量插入规则条件。
    /// </summary>
    /// <param name="entities">待插入的规则条件集合。</param>
    public async Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities)
    {
        // ========== 第一阶段：为每个条件分配数据库主键 ==========
        // 条件主键只承担数据库标识作用；匹配顺序由 ConditionGroup 和 SortNo 决定。
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_CONDITION.NEXTVAL FROM DUAL");
            entity.ConditionId = seq;
        }

        // ========== 第二阶段：批量写入条件集合 ==========
        // 调用方已校验版本为 DRAFT。仓储不再查版本表，避免跨聚合读写职责扩散。
        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除指定规则版本下的全部条件。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    public async Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 保存条件采用“整版本替换”，删除范围必须覆盖启用和停用条件，避免旧数据残留。
        await _db.Deleteable<RuleCondition>()
            .Where(c => c.RuleId == ruleId && c.VersionNo == versionNo)
            .ExecuteCommandAsync();
    }
}
