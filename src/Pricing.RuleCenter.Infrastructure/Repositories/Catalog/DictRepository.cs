using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Catalog;

/// <summary>
/// 字典仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_DICT 表的全部数据访问操作，包括：按类型查询启用项、按主键查询、查询全部类型、
/// 插入、更新、启用/停用、判断编码是否存在。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_DICT 是规则配置页面的元数据字典表，存储如"计价类型"、"单位"、"公式类型"等枚举值。
/// 每条记录由 DICT_TYPE（字典类型）和 DICT_CODE（字典编码）联合唯一标识。
/// 字典项支持启用/停用（IS_ENABLED = 'Y'/'N'），停用后仍保留在表中用于历史数据引用。
/// </para>
/// <para>
/// 【设计原则】
/// 仓储层只处理启用过滤、排序和唯一性查询，不解释具体字典类型的业务含义。
/// 字典类型的业务语义（如"计价类型"对应哪些计价策略）由应用服务或计价引擎解释。
/// </para>
/// </remarks>
public sealed class DictRepository : IDictRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和字典表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化字典仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public DictRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按字典类型读取启用字典项，按排序号升序排列。
    /// </summary>
    /// <param name="dictType">字典类型编码，如 PRICING_TYPE、UNIT、FORMULA_TYPE。</param>
    /// <returns>按 SortNo 升序排列的启用字典项集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_DICT
    /// WHERE DICT_TYPE = :dictType AND IS_ENABLED = 'Y'
    /// ORDER BY SORT_NO ASC
    /// </code>
    /// 【使用场景】规则配置页面的下拉框数据源。只返回启用项，避免用户选择已废弃的字典值。
    /// 【NULL ≠ 0】IS_ENABLED 使用字符串 'Y'/'N' 而非布尔值，与 Oracle 表设计一致。
    /// </remarks>
    public async Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType)
    {
        return await _db.Queryable<Dict>()
            .Where(d => d.DictType == dictType && d.IsEnabled == "Y")
            .OrderBy(d => d.SortNo)
            .ToListAsync();
    }

    /// <summary>
    /// 按主键读取字典项。
    /// </summary>
    /// <param name="dictId">字典主键（由 Oracle 序列 SEQ_PR_DICT 生成）。</param>
    /// <returns>字典实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_DICT WHERE DICT_ID = :dictId
    /// </code>
    /// 【使用场景】字典项详情查看、编辑前回显。
    /// </remarks>
    public async Task<Dict?> GetByIdAsync(long dictId)
    {
        return await _db.Queryable<Dict>()
            .InSingleAsync(dictId);
    }

    /// <summary>
    /// 查询当前启用字典项中出现过的全部字典类型。
    /// </summary>
    /// <returns>去重后的字典类型编码集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT DISTINCT DICT_TYPE FROM PR_DICT WHERE IS_ENABLED = 'Y'
    /// </code>
    /// 【过滤策略】只返回启用项所在类型，避免前端展示已经完全停用的历史类型。
    /// 如果某个字典类型下所有项都已停用，则该类型不会出现在结果中。
    /// 【使用场景】字典管理页面的类型列表，用于分类展示字典项。
    /// </remarks>
    public async Task<IReadOnlyList<string>> GetAllTypesAsync()
    {
        // 只返回启用项所在类型，避免前端展示已经完全停用的历史类型。
        return await _db.Queryable<Dict>()
            .Where(d => d.IsEnabled == "Y")
            .GroupBy(d => d.DictType)
            .Select(d => d.DictType)
            .ToListAsync();
    }

    /// <summary>
    /// 插入字典项。
    /// </summary>
    /// <param name="entity">待写入的字典实体，调用方应确保 DICT_TYPE + DICT_CODE 不重复。</param>
    /// <returns>Oracle 序列生成的字典主键（DICT_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_DICT.NEXTVAL FROM DUAL
    /// INSERT INTO PR_DICT (...) VALUES (...)
    /// </code>
    /// 【唯一性约束】DICT_TYPE + DICT_CODE 的唯一性由数据库约束保证。
    /// 若违反唯一约束，Oracle 会抛出 ORA-00001 异常，由全局异常过滤器统一处理。
    /// </remarks>
    public async Task<long> InsertAsync(Dict entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_DICT.NEXTVAL FROM DUAL");
        entity.DictId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新字典项（主键不可更新）。
    /// </summary>
    /// <param name="entity">包含最新展示信息的字典实体。</param>
    /// <returns>是否至少更新了一行（true 表示成功，false 表示记录不存在）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_DICT
    /// SET DICT_TYPE = ..., DICT_CODE = ..., DICT_NAME = ..., SORT_NO = ..., IS_ENABLED = ...
    /// WHERE DICT_ID = :entity.DictId
    /// </code>
    /// 注意：DICT_ID 不会被更新（通过 IgnoreColumns 排除），防止主键被意外修改。
    /// 【约束】业务编码（DICT_CODE）是否允许修改由服务层控制，仓储只负责执行实体更新。
    /// </remarks>
    public async Task<bool> UpdateAsync(Dict entity)
    {
        // 主键不能被更新；业务编码是否允许修改由服务层控制。
        var rows = await _db.Updateable(entity)
            .IgnoreColumns(d => d.DictId)
            .ExecuteCommandAsync();
        return rows > 0;
    }

    /// <summary>
    /// 更新字典项启用标志。
    /// </summary>
    /// <param name="dictId">字典主键。</param>
    /// <param name="isEnabled">目标启用标志，通常为 'Y'（启用）或 'N'（停用）。</param>
    /// <returns>是否至少更新了一行（true 表示成功，false 表示记录不存在）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_DICT SET IS_ENABLED = :isEnabled WHERE DICT_ID = :dictId
    /// </code>
    /// 【业务影响】停用字典项后，规则配置页面的下拉框将不再展示该项，
    /// 但已有规则中引用该字典值的记录不受影响（字典值已冗余存储在规则条件中）。
    /// </remarks>
    public async Task<bool> SetEnabledAsync(long dictId, string isEnabled)
    {
        var rows = await _db.Updateable<Dict>()
            .SetColumns(d => d.IsEnabled == isEnabled)
            .Where(d => d.DictId == dictId)
            .ExecuteCommandAsync();
        return rows > 0;
    }

    /// <summary>
    /// 判断同类型下是否存在指定字典编码。
    /// </summary>
    /// <param name="dictType">字典类型编码。</param>
    /// <param name="dictCode">字典项编码。</param>
    /// <returns>存在时返回 <c>true</c>（不论启用状态）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT CASE WHEN EXISTS (
    ///   SELECT 1 FROM PR_DICT WHERE DICT_TYPE = :dictType AND DICT_CODE = :dictCode
    /// ) THEN 1 ELSE 0 END FROM DUAL
    /// </code>
    /// 【使用场景】新增字典项前的唯一性校验。注意此查询不过滤 IS_ENABLED，
    /// 即使已停用的字典项也会返回 true，防止重复编码。
    /// </remarks>
    public async Task<bool> ExistsAsync(string dictType, string dictCode)
    {
        return await _db.Queryable<Dict>()
            .AnyAsync(d => d.DictType == dictType && d.DictCode == dictCode);
    }
}
