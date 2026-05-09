using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 字典仓储实现，封装 PR_DICT 的基础查询、写入和启停更新。
/// </summary>
/// <remarks>
/// 字典表服务于规则配置页面的元数据展示。仓储层只处理启用过滤、排序和唯一性查询，
/// 不解释具体字典类型的业务含义。
/// </remarks>
public sealed class DictRepository : IDictRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和字典表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化字典仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public DictRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按字典类型读取启用字典项。
    /// </summary>
    /// <param name="dictType">字典类型编码。</param>
    /// <returns>按 SortNo 排序的启用字典项。</returns>
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
    /// <param name="dictId">字典主键。</param>
    /// <returns>字典实体；不存在时返回 <c>null</c>。</returns>
    public async Task<Dict?> GetByIdAsync(long dictId)
    {
        return await _db.Queryable<Dict>()
            .InSingleAsync(dictId);
    }

    /// <summary>
    /// 查询当前启用字典项中出现过的全部字典类型。
    /// </summary>
    /// <returns>去重后的字典类型编码集合。</returns>
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
    /// <param name="entity">待写入的字典实体。</param>
    /// <returns>Oracle 序列生成的字典主键。</returns>
    public async Task<long> InsertAsync(Dict entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_DICT.NEXTVAL FROM DUAL");
        entity.DictId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新字典项。
    /// </summary>
    /// <param name="entity">包含最新展示信息的字典实体。</param>
    /// <returns>是否至少更新了一行。</returns>
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
    /// <param name="isEnabled">目标启用标志，通常为 Y 或 N。</param>
    /// <returns>是否至少更新了一行。</returns>
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
    /// <returns>存在时返回 <c>true</c>。</returns>
    public async Task<bool> ExistsAsync(string dictType, string dictCode)
    {
        return await _db.Queryable<Dict>()
            .AnyAsync(d => d.DictType == dictType && d.DictCode == dictCode);
    }
}
