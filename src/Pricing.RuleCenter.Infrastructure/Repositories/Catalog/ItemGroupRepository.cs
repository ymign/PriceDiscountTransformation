using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Catalog;

/// <summary>
/// 项目组仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_ITEM_GROUP 表的查询和写入操作，包括：
/// 按主键查询、按编码查询、按类型查询、插入、更新。
/// </para>
/// <para>
/// 【项目组模型】
/// 项目组将多个收费项目组织在一起，便于规则按组匹配和管理。
/// 规则头（PR_RULE_HEADER）的 RULE_SCOPE = "GROUP" 时，通过 GROUP_CODE 匹配项目组。
/// </para>
/// <para>
/// 【项目组类型说明】
/// <list type="bullet">
/// <item>MAIN_CHILD — 主子项目组，主项目收费时自动加收子项目</item>
/// <item>MUTUAL_EXCLUSIVE — 同类互斥项目组，同组项目只执行优先级最高的一条</item>
/// <item>SAME_OPERATION — 同手术项目组，同一手术下的多个项目共享限额</item>
/// </list>
/// </para>
/// <para>
/// 【与项目组明细的关系】
/// PR_ITEM_GROUP 保存组的元数据，PR_ITEM_GROUP_DETAIL 保存组内项目列表。
/// 两者是一对多关系。
/// </para>
/// </remarks>
public sealed class ItemGroupRepository : IItemGroupRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和项目组表。
    /// </remarks>
    private readonly ISqlSugarClient _db;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化项目组仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    /// <param name="clock">技术时间提供者，用于统一写入审计时间。</param>
    public ItemGroupRepository(ISqlSugarClient db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <summary>
    /// 按主键查询单个项目组。
    /// </summary>
    /// <param name="groupId">项目组主键（PR_ITEM_GROUP.GROUP_ID）。</param>
    /// <returns>项目组实体，不存在时返回 null。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_ITEM_GROUP WHERE GROUP_ID = :groupId
    /// </code>
    /// 【使用场景】项目组详情页回显数据、编辑时校验是否存在。
    /// </remarks>
    public async Task<ItemGroup?> GetByIdAsync(long groupId)
    {
        return await _db.Queryable<ItemGroup>()
            .Where(g => g.GroupId == groupId)
            .FirstAsync();
    }

    /// <summary>
    /// 按项目组编码查询单个项目组。
    /// </summary>
    /// <param name="groupCode">项目组编码（PR_ITEM_GROUP.GROUP_CODE）。</param>
    /// <returns>项目组实体，不存在时返回 null。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_ITEM_GROUP WHERE GROUP_CODE = :groupCode
    /// </code>
    /// 【使用场景】计价引擎按条件配置的项目组编码查找项目组，再进一步判断项目是否属于该组。
    /// GroupCode 是全局唯一的业务键，查询效率等同于按主键查询。
    /// </remarks>
    public async Task<ItemGroup?> GetByCodeAsync(string groupCode)
    {
        return await _db.Queryable<ItemGroup>()
            .Where(g => g.GroupCode == groupCode)
            .FirstAsync();
    }

    /// <summary>
    /// 按项目组类型查询项目组列表。
    /// </summary>
    /// <param name="groupType">项目组类型编码（MAIN_CHILD / MUTUAL_EXCLUSIVE / SAME_OPERATION）。</param>
    /// <returns>指定类型的项目组列表；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_ITEM_GROUP WHERE GROUP_TYPE = :groupType
    /// </code>
    /// 【使用场景】工作台按类型筛选项目组，如下拉列表展示所有互斥组。
    /// </remarks>
    public async Task<IReadOnlyList<ItemGroup>> GetByTypeAsync(string groupType)
    {
        return await _db.Queryable<ItemGroup>()
            .Where(g => g.GroupType == groupType)
            .ToListAsync();
    }

    /// <summary>
    /// 插入项目组记录。
    /// </summary>
    /// <param name="entity">待写入的项目组实体。</param>
    /// <returns>Oracle 序列生成的项目组主键（GROUP_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_ITEM_GROUP.NEXTVAL FROM DUAL
    /// INSERT INTO PR_ITEM_GROUP (...) VALUES (...)
    /// </code>
    /// 【事务边界】本方法不开启事务。调用方（项目组服务）应在更大的事务中调用本方法，
    /// 保证项目组与项目组明细的原子更新。
    /// </remarks>
    public async Task<long> InsertAsync(ItemGroup entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_ITEM_GROUP.NEXTVAL FROM DUAL");
        entity.GroupId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新项目组记录。
    /// </summary>
    /// <param name="entity">待更新的项目组实体，必须包含 GroupId 作为更新条件。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_ITEM_GROUP
    /// SET GROUP_CODE = :groupCode,
    ///     GROUP_NAME = :groupName,
    ///     GROUP_TYPE = :groupType,
    ///     IS_ENABLED = :isEnabled,
    ///     REMARK = :remark,
    ///     UPDATED_BY = :updatedBy,
    ///     UPDATED_AT = SYSDATE
    /// WHERE GROUP_ID = :groupId
    /// </code>
    /// 【使用场景】配置人员编辑项目组信息时调用。
    /// 【约束】更新时应同步更新 UpdatedAt 和 UpdatedBy 字段，本方法自动填充 UpdatedAt。
    /// </remarks>
    public async Task UpdateAsync(ItemGroup entity)
    {
        entity.UpdatedAt = _clock.Now;
        await _db.Updateable(entity)
            .Where(g => g.GroupId == entity.GroupId)
            .ExecuteCommandAsync();
    }
}
