using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则动作仓储实现，封装 PR_RULE_ACTION 的版本内读取、批量写入和清空。
/// </summary>
/// <remarks>
/// 动作链决定计价引擎如何修改数量、金额或限额占用。读取时只返回启用动作并按 SortNo 排序，
/// 保存时由应用服务先校验草稿状态，再通过删除重建保证动作链与前端提交完全一致。
/// </remarks>
public sealed class RuleActionRepository : IRuleActionRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和规则动作表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则动作仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public RuleActionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取指定规则版本下的启用动作。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>按执行顺序排列的动作集合。</returns>
    public async Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 运行时只执行启用动作；停用动作保留在表中用于配置页面展示和历史排查。
        return await _db.Queryable<RuleAction>()
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo && a.IsEnabled == "Y")
            .OrderBy(a => a.SortNo)
            .ToListAsync();
    }

    /// <summary>
    /// 批量插入规则动作。
    /// </summary>
    /// <param name="entities">待插入的规则动作集合。</param>
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
    /// 删除指定规则版本下的全部动作。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    public async Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        // 保存动作采用“整版本替换”，所以删除不区分启用/停用，确保旧动作不会残留影响展示或执行。
        await _db.Deleteable<RuleAction>()
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo)
            .ExecuteCommandAsync();
    }
}
