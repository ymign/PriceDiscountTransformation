using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 项目组明细仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_ITEM_GROUP_DETAIL 表的查询、批量写入和删除操作，包括：
/// 按项目组查询成员列表、按项目编码查询所属组、批量插入、按组删除。
/// </para>
/// <para>
/// 【项目组明细模型】
/// 项目组明细记录项目组内包含哪些收费项目，以及每个项目在组内的角色。
/// 通过 GROUP_ID 关联 PR_ITEM_GROUP，通过 ITEM_CODE 关联 HIS 物价主数据。
/// </para>
/// <para>
/// 【主子项目关系】
/// <list type="bullet">
/// <item>MAIN — 主项目，收费时触发子项目加收</item>
/// <item>CHILD — 子项目，随主项目一起收费，必须与主项目同 resultGroupNo 原子 commit/cancel</item>
/// </list>
/// </para>
/// <para>
/// 【批量操作说明】
/// InsertBatchAsync 使用逐条获取序列号后批量插入，提高大数据量场景的性能。
/// DeleteByGroupIdAsync 按组删除，用于项目组成员的整体维护。
/// </para>
/// </remarks>
public sealed class ItemGroupDetailRepository : IItemGroupDetailRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和项目组明细表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化项目组明细仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public ItemGroupDetailRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按项目组主键查询所有明细记录。
    /// </summary>
    /// <param name="groupId">项目组主键（PR_ITEM_GROUP.GROUP_ID）。</param>
    /// <returns>项目组明细列表；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_ITEM_GROUP_DETAIL
    /// WHERE GROUP_ID = :groupId
    /// </code>
    /// 【使用场景】项目组详情页展示成员列表、计价引擎按组匹配项目。
    /// </remarks>
    public async Task<IReadOnlyList<ItemGroupDetail>> GetByGroupIdAsync(long groupId)
    {
        return await _db.Queryable<ItemGroupDetail>()
            .Where(d => d.GroupId == groupId)
            .ToListAsync();
    }

    /// <summary>
    /// 按项目编码查询所属的所有项目组明细。
    /// </summary>
    /// <param name="itemCode">项目编码（HIS 物价主数据 ITEM_CODE）。</param>
    /// <returns>包含该项目的所有项目组明细列表；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_ITEM_GROUP_DETAIL WHERE ITEM_CODE = :itemCode
    /// </code>
    /// 【使用场景】判断某项目是否属于某个项目组类型，或查找项目所属的所有组。
    /// 一个项目可能同时属于多个不同类型的项目组。
    /// </remarks>
    public async Task<IReadOnlyList<ItemGroupDetail>> GetByItemCodeAsync(string itemCode)
    {
        return await _db.Queryable<ItemGroupDetail>()
            .Where(d => d.ItemCode == itemCode)
            .ToListAsync();
    }

    /// <summary>
    /// 批量插入项目组明细记录。
    /// </summary>
    /// <param name="entities">待写入的项目组明细实体列表。</param>
    /// <remarks>
    /// 【SQL 语义】对每条记录分两步执行：
    /// <code>
    /// SELECT SEQ_PR_ITEM_GROUP_DETAIL.NEXTVAL FROM DUAL
    /// INSERT INTO PR_ITEM_GROUP_DETAIL (...) VALUES (...)
    /// </code>
    /// 【批量策略】逐条获取序列号后使用 SqlSugar 的 Insertable 批量插入，
    /// 提高大数据量场景的性能。每条记录独立获取序列号，保证主键唯一性。
    /// 【事务要求】批量插入必须在事务中执行，保证全部成功或全部回滚。
    /// </remarks>
    public async Task InsertBatchAsync(IReadOnlyList<ItemGroupDetail> entities)
    {
        if (entities.Count == 0) return;

        // 为每条记录分配序列号
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_ITEM_GROUP_DETAIL.NEXTVAL FROM DUAL");
            entity.DetailId = seq;
        }

        // 批量插入
        await _db.Insertable(entities.ToArray()).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除指定项目组下的所有明细记录。
    /// </summary>
    /// <param name="groupId">项目组主键（PR_ITEM_GROUP.GROUP_ID）。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// DELETE FROM PR_ITEM_GROUP_DETAIL WHERE GROUP_ID = :groupId
    /// </code>
    /// 【使用场景】配置人员清空项目组成员或删除项目组时调用。
    /// 【约束】删除前应确认是否有规则引用了该项目组，避免规则匹配失效。
    /// 该约束由应用服务保证，仓储只执行删除操作。
    /// </remarks>
    public async Task DeleteByGroupIdAsync(long groupId)
    {
        await _db.Deleteable<ItemGroupDetail>()
            .Where(d => d.GroupId == groupId)
            .ExecuteCommandAsync();
    }
}
