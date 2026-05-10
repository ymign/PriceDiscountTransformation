using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则条件仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_CONDITION 表的版本内读取、批量写入和清空操作。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_RULE_CONDITION 存储规则版本下的条件集合。每条记录定义一个匹配条件，如：
///   - 项目编码等于某值
///   - 场景（门诊/住院/手术）等于某值
///   - 部位匹配某范围
///   - 时间在某区间内
/// 条件通过 RULE_ID + VERSION_NO 关联到规则版本。
/// </para>
/// <para>
/// 【条件组机制】
/// 条件支持分组（CONDITION_GROUP）：
///   - 同组内条件为 AND 关系（全部满足才命中）
///   - 不同组之间为 OR 关系（任一组满足即命中）
/// 组内通过 SORT_NO 排序，组间通过 CONDITION_GROUP 排序。
/// </para>
/// <para>
/// 【条件-动作分离模式】
/// 条件集合决定一条规则是否命中。读取时只返回启用条件，并先按条件组、再按组内顺序排序，
/// 以便匹配服务能稳定地还原配置人员定义的判断顺序。
/// </para>
/// <para>
/// 【保存策略】
/// 与动作一样，条件保存采用"整版本替换"模式：先删除全部条件，再批量插入新条件。
/// </para>
/// </remarks>
public sealed class RuleConditionRepository : IRuleConditionRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和规则条件表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则条件仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleConditionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则版本下的启用条件，按条件组和组内顺序排列。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>按 CONDITION_GROUP 升序、SORT_NO 升序排列的启用条件集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_CONDITION
    /// WHERE RULE_ID = :ruleId AND VERSION_NO = :versionNo AND IS_ENABLED = 'Y'
    /// ORDER BY CONDITION_GROUP ASC, SORT_NO ASC
    /// </code>
    /// 【过滤策略】只返回启用条件，停用条件不参与规则匹配。
    /// 【排序要求】双层排序（先 CONDITION_GROUP 再 SORT_NO）保证匹配服务能稳定还原配置顺序。
    /// 排序稳定性对追踪步骤解释很重要——追踪页面按此顺序展示每步条件的匹配结果。
    /// </remarks>
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
    /// <param name="entities">待插入的规则条件集合，调用方应已设置好 ConditionGroup 和 SortNo。</param>
    /// <remarks>
    /// 【SQL 语义】分两个阶段执行：
    /// <code>
    /// -- 第一阶段：逐条获取序列主键
    /// SELECT SEQ_PR_RULE_CONDITION.NEXTVAL FROM DUAL  -- 对每条记录执行一次
    /// -- 第二阶段：批量插入
    /// INSERT ALL
    ///   INTO PR_RULE_CONDITION (...) VALUES (...)
    ///   INTO PR_RULE_CONDITION (...) VALUES (...)
    ///   ...
    /// SELECT 1 FROM DUAL
    /// </code>
    /// 【主键与排序】条件主键（CONDITION_ID）只承担数据库标识作用；
    /// 匹配顺序由 CONDITION_GROUP 和 SORT_NO 决定，二者不要混用。
    /// 【事务边界】调用方已校验版本为 DRAFT。仓储不再查版本表，避免跨聚合读写职责扩散。
    /// 通常在"删除旧条件 + 插入新条件"的整版本替换流程中，两步在同一事务内执行。
    /// </remarks>
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
    /// 删除指定规则版本下的全部条件（不区分启用/停用）。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// DELETE FROM PR_RULE_CONDITION
    /// WHERE RULE_ID = :ruleId AND VERSION_NO = :versionNo
    /// </code>
    /// 【使用场景】保存条件时的"整版本替换"流程第一步。删除后由 InsertBatchAsync 写入新条件。
    /// 【删除范围】覆盖启用和停用条件，避免旧数据残留。
    /// 【事务要求】必须与 InsertBatchAsync 在同一事务中执行，保证原子替换。
    /// </remarks>
    public async Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 保存条件采用"整版本替换"，删除范围必须覆盖启用和停用条件，避免旧数据残留。
        await _db.Deleteable<RuleCondition>()
            .Where(c => c.RuleId == ruleId && c.VersionNo == versionNo)
            .ExecuteCommandAsync();
    }
}
