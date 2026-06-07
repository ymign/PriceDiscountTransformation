using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则动作仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_ACTION 表的版本内读取、批量写入和清空操作。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_RULE_ACTION 存储规则版本下的动作链。每条记录定义一个计价动作，如：
///   - 折价（DISCOUNT）—— 按公式或固定比例修改金额
///   - 限额（LIMIT）—— 设置数量/金额上限
///   - 子项加收（ADD_ITEM）—— 追加附加收费项目
///   - 公式（FORMULA）—— 调用公式执行器计算金额
/// 动作通过 RULE_ID + VERSION_NO 关联到规则版本，通过 SORT_NO 决定执行顺序。
/// </para>
/// <para>
/// 【条件-动作分离模式】
/// 规则建模为"满足条件时执行动作"。条件存储在 PR_RULE_CONDITION，
/// 动作存储在本表 PR_RULE_ACTION。计价引擎先匹配条件，命中后按 SortNo 顺序执行动作链。
/// </para>
/// <para>
/// 【保存策略】
/// 动作保存采用"整版本替换"模式：先删除该版本下全部动作，再批量插入新动作。
/// 这保证了动作链与前端提交完全一致，避免增量更新导致的遗漏或冲突。
/// </para>
/// </remarks>
public sealed class RuleActionRepository : IRuleActionRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和规则动作表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则动作仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleActionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则版本下的启用动作，按执行顺序排列。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>按 SORT_NO 升序排列的启用动作集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_ACTION
    /// WHERE RULE_ID = :ruleId AND VERSION_NO = :versionNo AND IS_ENABLED = 'Y'
    /// ORDER BY SORT_NO ASC
    /// </code>
    /// 【过滤策略】运行时只执行启用动作；停用动作保留在表中用于配置页面展示和历史排查。
    /// 【排序要求】SortNo 决定动作执行顺序。计价引擎按此顺序逐个调用 IRuleActionExecutor 实现。
    /// 排序稳定性对追踪步骤解释很重要——追踪页面按此顺序展示每步动作的输入输出。
    /// </remarks>
    public async Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 运行时只执行启用动作；停用动作保留在表中用于配置页面展示和历史排查。
        return await _db.Queryable<RuleAction>()
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo && a.IsEnabled == "Y")
            .OrderBy(a => a.SortNo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>> GetByRuleVersionsAsync(
        IReadOnlyCollection<(long RuleId, int VersionNo)> ruleVersions)
    {
        if (ruleVersions.Count == 0)
        {
            return new Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>();
        }

        var keys = ruleVersions.Distinct().ToArray();
        var keySet = keys.ToHashSet();
        var ruleIds = keys.Select(item => item.RuleId).Distinct().ToArray();
        var versionNos = keys.Select(item => item.VersionNo).Distinct().ToArray();

        var items = await _db.Queryable<RuleAction>()
            .Where(a => ruleIds.Contains(a.RuleId) && versionNos.Contains(a.VersionNo) && a.IsEnabled == "Y")
            .ToListAsync();

        var grouped = items
            .Where(item => keySet.Contains((item.RuleId, item.VersionNo)))
            .GroupBy(item => (item.RuleId, item.VersionNo))
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<RuleAction>)group
                    .OrderBy(item => item.SortNo)
                    .ToList());

        var result = new Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>();
        foreach (var key in keys)
        {
            result[key] = grouped.TryGetValue(key, out var actions)
                ? actions
                : Array.Empty<RuleAction>();
        }

        return result;
    }

    /// <summary>
    /// 批量插入规则动作。
    /// </summary>
    /// <param name="entities">待插入的规则动作集合，调用方应已设置好 SortNo。</param>
    /// <remarks>
    /// 【SQL 语义】分两个阶段执行：
    /// <code>
    /// -- 第一阶段：逐条获取序列主键
    /// SELECT SEQ_PR_RULE_ACTION.NEXTVAL FROM DUAL  -- 对每条记录执行一次
    /// -- 第二阶段：批量插入
    /// INSERT ALL
    ///   INTO PR_RULE_ACTION (...) VALUES (...)
    ///   INTO PR_RULE_ACTION (...) VALUES (...)
    ///   ...
    /// SELECT 1 FROM DUAL
    /// </code>
    /// 【主键与排序】动作主键（ACTION_ID）由 Oracle 序列生成，SortNo 由配置请求决定，二者不要混用。
    /// ACTION_ID 是数据库标识，SORT_NO 是业务执行顺序。
    /// 【事务边界】调用方已经完成版本草稿校验；这里不重复判断版本状态，避免仓储依赖更多业务表。
    /// 通常在"删除旧动作 + 插入新动作"的整版本替换流程中，两步在同一事务内执行。
    /// </remarks>
    public async Task InsertBatchAsync(IReadOnlyList<RuleAction> entities)
    {
        // ========== 第一阶段：为每个动作分配数据库主键 ==========
        // 动作主键由 Oracle 序列生成，SortNo 仍然由配置请求决定，二者不要混用。
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_ACTION.NEXTVAL FROM DUAL");
            entity.ActionId = seq;
        }

        // ========== 第二阶段：批量写入动作链 ==========
        // 调用方已经完成版本草稿校验；这里不重复判断版本状态，避免仓储依赖更多业务表。
        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除指定规则版本下的全部动作（不区分启用/停用）。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// DELETE FROM PR_RULE_ACTION
    /// WHERE RULE_ID = :ruleId AND VERSION_NO = :versionNo
    /// </code>
    /// 【使用场景】保存动作时的"整版本替换"流程第一步。删除后由 InsertBatchAsync 写入新动作。
    /// 【删除范围】不区分启用/停用，确保旧动作不会残留影响展示或执行。
    /// 【事务要求】必须与 InsertBatchAsync 在同一事务中执行，保证原子替换。
    /// </remarks>
    public async Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 保存动作采用"整版本替换"，所以删除不区分启用/停用，确保旧动作不会残留影响展示或执行。
        await _db.Deleteable<RuleAction>()
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo)
            .ExecuteCommandAsync();
    }
}
