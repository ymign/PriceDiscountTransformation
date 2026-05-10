using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 内置字典仓储接口 —— 规则维护中心的字典数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_DICT。
///
/// 职责边界：
///   - 管理规则中心内部使用的业务字典数据。
///   - 字典类型包括：计价类型、计量单位、公式类型、条件类型、动作类型等。
///   - 字典数据由 HIS 特殊计价规则维护工作台管理，不从外部系统同步。
///
/// 使用场景：
///   - 前端工作台的下拉列表选项加载。
///   - 规则配置时的值域校验（如动作类型必须在字典中存在）。
///   - 计价引擎的类型映射（如字典编码到枚举值的映射）。
///
/// 数据约束：
///   - dictType + dictCode 组合唯一。
///   - 每条字典记录有 isEnabled 字段，支持软删除。
/// </summary>
public interface IDictRepository
{
    /// <summary>
    /// 按字典类型查询该类型下的所有字典项。
    ///
    /// 使用场景：前端加载某类字典的所有选项（如所有计价类型、所有计量单位）。
    /// </summary>
    /// <param name="dictType">字典类型编码（如"PRICING_TYPE"、"UNIT"、"FORMULA_TYPE"）。</param>
    /// <returns>该类型下的字典项列表，按排序字段排序。</returns>
    Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType);

    /// <summary>
    /// 根据主键ID查询单条字典记录。
    /// </summary>
    /// <param name="dictId">字典记录主键ID。</param>
    /// <returns>字典实体，不存在时返回 null。</returns>
    Task<Dict?> GetByIdAsync(long dictId);

    /// <summary>
    /// 查询所有已使用的字典类型列表。
    ///
    /// 使用场景：工作台展示字典分类列表，供管理员选择要维护的字典类型。
    /// </summary>
    /// <returns>字典类型编码列表（去重）。</returns>
    Task<IReadOnlyList<string>> GetAllTypesAsync();

    /// <summary>
    /// 插入一条字典记录，返回自增主键。
    ///
    /// 插入前应调用 ExistsAsync 校验 dictType + dictCode 是否重复。
    /// </summary>
    /// <param name="entity">字典实体，包含类型、编码、名称、排序、是否启用等字段。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(Dict entity);

    /// <summary>
    /// 更新字典记录（全量更新）。
    /// </summary>
    /// <param name="entity">要更新的字典实体，ID 必须存在。</param>
    /// <returns>是否更新成功（ID 不存在时返回 false）。</returns>
    Task<bool> UpdateAsync(Dict entity);

    /// <summary>
    /// 设置字典记录的启用/禁用状态（软删除）。
    ///
    /// 使用场景：工作台中禁用某字典项，不物理删除，保留历史引用完整性。
    /// </summary>
    /// <param name="dictId">字典记录主键ID。</param>
    /// <param name="isEnabled">启用状态（"Y" 启用，"N" 禁用）。</param>
    /// <returns>是否设置成功。</returns>
    Task<bool> SetEnabledAsync(long dictId, string isEnabled);

    /// <summary>
    /// 检查指定字典类型和编码的组合是否已存在。
    ///
    /// 使用场景：新增字典记录前的唯一性校验。
    /// </summary>
    /// <param name="dictType">字典类型编码。</param>
    /// <param name="dictCode">字典项编码。</param>
    /// <returns>存在返回 true，不存在返回 false。</returns>
    Task<bool> ExistsAsync(string dictType, string dictCode);
}
