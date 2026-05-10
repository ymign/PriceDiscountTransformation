using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 公式定义仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_FORMULA_DEF 表的全部数据访问操作，包括：查询全部、按主键查询、按编码查询、
/// 插入、更新、启用/停用。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_FORMULA_DEF 把页面上的公式编码（FORMULA_CODE）与运行时执行器编码（EXECUTOR_CODE）
/// 连接起来。每条记录定义一种计价公式，如"按面积计价"、"按部位数量计价"等。
/// PARAM_SCHEMA_JSON 以 JSON 字符串形式存储公式参数的结构定义，仓储不解析其内容。
/// </para>
/// <para>
/// 【与规则动作的关系】
/// PR_RULE_ACTION 中的 ACTION_TYPE = 'FORMULA' 时，ACTION_VALUE 存储公式编码。
/// 计价引擎通过公式编码查找本表，获取执行器编码后调用对应的公式计算逻辑。
/// </para>
/// <para>
/// 【设计原则】
/// 仓储不解析参数结构（PARAM_SCHEMA_JSON），只保存元数据文本。
/// 公式参数的运行时解析和校验由 IPricingFormulaExecutor 的具体实现负责。
/// </para>
/// </remarks>
public sealed class FormulaDefRepository : IFormulaDefRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和公式定义表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化公式定义仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public FormulaDefRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取全部公式定义，按主键升序排列。
    /// </summary>
    /// <returns>按 FORMULA_ID 升序排列的公式定义集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_FORMULA_DEF ORDER BY FORMULA_ID ASC
    /// </code>
    /// 【使用场景】规则配置页面的公式下拉框数据源、公式管理页面列表。
    /// 不过滤启用状态，因为配置页面需要展示全部公式（含已停用）以供管理。
    /// </remarks>
    public async Task<IReadOnlyList<FormulaDef>> GetAllAsync()
    {
        return await _db.Queryable<FormulaDef>()
            .OrderBy(f => f.FormulaId)
            .ToListAsync();
    }

    /// <summary>
    /// 按主键读取公式定义。
    /// </summary>
    /// <param name="formulaId">公式定义主键（由 Oracle 序列 SEQ_PR_FORMULA_DEF 生成）。</param>
    /// <returns>公式定义实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_FORMULA_DEF WHERE FORMULA_ID = :formulaId
    /// </code>
    /// 【使用场景】公式详情查看、编辑前回显。
    /// </remarks>
    public async Task<FormulaDef?> GetByIdAsync(long formulaId)
    {
        return await _db.Queryable<FormulaDef>()
            .InSingleAsync(formulaId);
    }

    /// <summary>
    /// 按公式编码读取公式定义。
    /// </summary>
    /// <param name="formulaCode">公式编码，如 AREA_PRICING、PART_QTY_PRICING。</param>
    /// <returns>公式定义实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_FORMULA_DEF WHERE FORMULA_CODE = :formulaCode
    /// </code>
    /// 【使用场景】计价引擎在执行规则动作时，通过 FORMULA_CODE 查找公式定义，
    /// 获取 EXECUTOR_CODE 后定位具体的公式执行器实现。
    /// </remarks>
    public async Task<FormulaDef?> GetByCodeAsync(string formulaCode)
    {
        return await _db.Queryable<FormulaDef>()
            .FirstAsync(f => f.FormulaCode == formulaCode);
    }

    /// <summary>
    /// 插入公式定义。
    /// </summary>
    /// <param name="entity">待写入的公式定义实体。</param>
    /// <returns>Oracle 序列生成的公式定义主键（FORMULA_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_FORMULA_DEF.NEXTVAL FROM DUAL
    /// INSERT INTO PR_FORMULA_DEF (...) VALUES (...)
    /// </code>
    /// 【约束】FORMULA_CODE 的唯一性由数据库约束保证，违反时 Oracle 抛出 ORA-00001。
    /// </remarks>
    public async Task<long> InsertAsync(FormulaDef entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_FORMULA_DEF.NEXTVAL FROM DUAL");
        entity.FormulaId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新公式定义（主键不可更新）。
    /// </summary>
    /// <param name="entity">包含最新公式元数据的实体。</param>
    /// <returns>是否至少更新了一行（true 表示成功，false 表示记录不存在）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_FORMULA_DEF
    /// SET FORMULA_CODE = ..., FORMULA_NAME = ..., EXECUTOR_CODE = ...,
    ///     PARAM_SCHEMA_JSON = ..., DESCRIPTION = ..., IS_ENABLED = ...
    /// WHERE FORMULA_ID = :entity.FormulaId
    /// </code>
    /// 注意：FORMULA_ID 不会被更新（通过 IgnoreColumns 排除）。
    /// 【约束】公式编码是否可改由服务层约束，仓储只负责执行实体更新。
    /// </remarks>
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
    /// <param name="isEnabled">目标启用标志，通常为 'Y'（启用）或 'N'（停用）。</param>
    /// <returns>是否至少更新了一行（true 表示成功，false 表示记录不存在）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_FORMULA_DEF SET IS_ENABLED = :isEnabled WHERE FORMULA_ID = :formulaId
    /// </code>
    /// 【业务影响】停用公式后，规则配置页面不再展示该公式选项。
    /// 已有规则中引用该公式的动作不受影响（公式编码已冗余存储在规则动作中），
    /// 但计价引擎在执行时应校验公式是否启用，停用公式应触发告警。
    /// </remarks>
    public async Task<bool> SetEnabledAsync(long formulaId, string isEnabled)
    {
        var rows = await _db.Updateable<FormulaDef>()
            .SetColumns(f => f.IsEnabled == isEnabled)
            .Where(f => f.FormulaId == formulaId)
            .ExecuteCommandAsync();
        return rows > 0;
    }
}
