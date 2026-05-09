using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 公式定义仓储实现，封装 PR_FORMULA_DEF 的查询、写入和启停更新。
/// </summary>
/// <remarks>
/// 公式定义把页面上的公式编码与运行时执行器编码连接起来。仓储不解析参数结构，
/// 只保存 ParamSchemaJson 这类元数据文本。
/// </remarks>
public sealed class FormulaDefRepository : IFormulaDefRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和公式定义表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化公式定义仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public FormulaDefRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取全部公式定义。
    /// </summary>
    /// <returns>按主键升序排列的公式定义集合。</returns>
    public async Task<IReadOnlyList<FormulaDef>> GetAllAsync()
    {
        return await _db.Queryable<FormulaDef>()
            .OrderBy(f => f.FormulaId)
            .ToListAsync();
    }

    /// <summary>
    /// 按主键读取公式定义。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <returns>公式定义实体；不存在时返回 <c>null</c>。</returns>
    public async Task<FormulaDef?> GetByIdAsync(long formulaId)
    {
        return await _db.Queryable<FormulaDef>()
            .InSingleAsync(formulaId);
    }

    /// <summary>
    /// 按公式编码读取公式定义。
    /// </summary>
    /// <param name="formulaCode">公式编码。</param>
    /// <returns>公式定义实体；不存在时返回 <c>null</c>。</returns>
    public async Task<FormulaDef?> GetByCodeAsync(string formulaCode)
    {
        return await _db.Queryable<FormulaDef>()
            .FirstAsync(f => f.FormulaCode == formulaCode);
    }

    /// <summary>
    /// 插入公式定义。
    /// </summary>
    /// <param name="entity">待写入的公式定义实体。</param>
    /// <returns>Oracle 序列生成的公式定义主键。</returns>
    public async Task<long> InsertAsync(FormulaDef entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_FORMULA_DEF.NEXTVAL FROM DUAL");
        entity.FormulaId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新公式定义。
    /// </summary>
    /// <param name="entity">包含最新公式元数据的实体。</param>
    /// <returns>是否至少更新了一行。</returns>
    public async Task<bool> UpdateAsync(FormulaDef entity)
    {
        // 主键保持不变；公式编码是否可改由服务层约束，仓储只负责执行实体更新。
        var rows = await _db.Updateable(entity)
            .IgnoreColumns(f => f.FormulaId)
            .ExecuteCommandAsync();
        return rows > 0;
    }

    /// <summary>
    /// 更新公式定义启用标志。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <param name="isEnabled">目标启用标志，通常为 Y 或 N。</param>
    /// <returns>是否至少更新了一行。</returns>
    public async Task<bool> SetEnabledAsync(long formulaId, string isEnabled)
    {
        var rows = await _db.Updateable<FormulaDef>()
            .SetColumns(f => f.IsEnabled == isEnabled)
            .Where(f => f.FormulaId == formulaId)
            .ExecuteCommandAsync();
        return rows > 0;
    }
}
