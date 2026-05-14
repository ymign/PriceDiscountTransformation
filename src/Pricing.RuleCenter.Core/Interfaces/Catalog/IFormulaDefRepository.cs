using Pricing.RuleCenter.Core.Aggregates.Catalog;

namespace Pricing.RuleCenter.Core.Interfaces.Catalog;

/// <summary>
/// 公式定义仓储接口 —— 规则维护中心的公式管理数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_FORMULA_DEF。
///
/// 职责边界：
///   - 管理计价公式的定义（公式编码、公式名称、公式表达式、参数说明等）。
///   - 公式定义被规则动作（PR_RULE_ACTION）引用，计价引擎在执行折价公式动作时加载公式。
///   - 公式本身是可复用的：多条规则可引用同一公式定义，通过不同参数执行。
///
/// 公式与限制的交互：
///   - 公式优先于限制：先执行公式计算，再将结果与金额/数量限制比较。
///   - 公式项目是否执行数量/时间窗/同组/同手术限制，以规则动作配置为准。
///   - 禁止代码层"一见公式就跳过全部数量类限制"的硬编码逻辑。
///
/// 数据约束：
///   - formulaCode 全局唯一。
///   - 每条公式有 isEnabled 字段，支持软删除。
///   - 公式表达式存储在 CLOB 字段中（Oracle 11g 无原生 JSON 支持）。
/// </summary>
public interface IFormulaDefRepository
{
    /// <summary>
    /// 查询所有公式定义。
    ///
    /// 使用场景：工作台的公式列表页面展示，以及计价引擎启动时预加载公式缓存。
    /// </summary>
    /// <returns>所有公式定义列表。</returns>
    Task<IReadOnlyList<FormulaDef>> GetAllAsync();

    /// <summary>
    /// 根据主键ID查询公式定义。
    /// </summary>
    /// <param name="formulaId">公式定义主键ID。</param>
    /// <returns>公式定义实体，不存在时返回 null。</returns>
    Task<FormulaDef?> GetByIdAsync(long formulaId);

    /// <summary>
    /// 根据公式编码查询公式定义。
    ///
    /// 使用场景：计价引擎在执行折价公式动作时，通过公式编码查找公式定义。
    /// </summary>
    /// <param name="formulaCode">公式编码（全局唯一）。</param>
    /// <returns>公式定义实体，不存在时返回 null。</returns>
    Task<FormulaDef?> GetByCodeAsync(string formulaCode);

    /// <summary>
    /// 插入一条公式定义，返回自增主键。
    ///
    /// 插入前应校验 formulaCode 是否已存在。
    /// </summary>
    /// <param name="entity">公式定义实体，包含编码、名称、表达式、参数说明、是否启用等。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(FormulaDef entity);

    /// <summary>
    /// 更新公式定义（全量更新）。
    ///
    /// 注意：更新公式表达式后，引用该公式的已发布规则不受影响（规则绑定的是版本快照）。
    /// 但新发布的规则版本将使用最新的公式定义。
    /// </summary>
    /// <param name="entity">要更新的公式定义实体，ID 必须存在。</param>
    /// <returns>是否更新成功。</returns>
    Task<bool> UpdateAsync(FormulaDef entity);

    /// <summary>
    /// 设置公式定义的启用/禁用状态（软删除）。
    ///
    /// 禁用后，新规则版本不能再引用该公式；已发布规则中引用该公式的动作在执行时
    /// 应报错（由 ActionOnError 策略决定处理方式）。
    /// </summary>
    /// <param name="formulaId">公式定义主键ID。</param>
    /// <param name="isEnabled">启用状态（"Y" 启用，"N" 禁用）。</param>
    /// <returns>是否设置成功。</returns>
    Task<bool> SetEnabledAsync(long formulaId, string isEnabled);
}
