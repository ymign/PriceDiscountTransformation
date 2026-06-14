using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Interfaces.Rules;

/// <summary>
/// 规则主表仓储接口 —— 规则维护中心的核心数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_HEADER。
///
/// 职责边界：
///   - 管理规则的元数据（规则编码、规则名称、适用项目、状态等）。
///   - 提供规则的增删改查、分页查询、按项目查询、按生效时间查询等能力。
///   - 规则主表是规则体系的顶层实体，下挂规则版本（PR_RULE_VERSION）→ 条件（PR_RULE_CONDITION）+ 动作（PR_RULE_ACTION）。
///
/// 规则冲突校验约束：
///   同一项目、同一场景、同一生效期内不允许多套折价公式、金额上下限或额度规则同时生效。
///   发布前必须校验规则重叠、动作组冲突、重复子项目、缺少测试用例等阻断项。
///
/// 数据模型：
///   - ruleCode 全局唯一，用于规则的业务标识。
///   - itemCode 关联到 HIS 物价项目编码。
///   - status 使用 RuleStatus 枚举（Draft/Published/Disabled）。
/// </summary>
public interface IRuleHeaderRepository
{
    /// <summary>
    /// 根据主键ID查询规则主表。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.ID）。</param>
    /// <returns>规则主表实体，不存在时返回 null。</returns>
    Task<RuleAggregate?> GetByIdAsync(long ruleId);

    /// <summary>
    /// 根据主键ID查询规则主表并加行锁。
    /// </summary>
    /// <remarks>
    /// 用于发布、停用、回滚等需要串行化状态机推进的场景。
    /// 调用方必须在已开启事务的上下文中调用，否则 `FOR UPDATE` 锁没有意义。
    /// </remarks>
    Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId);

    /// <summary>
    /// 根据规则编码查询规则主表。
    ///
    /// 使用场景：通过业务编码快速定位规则，如规则详情页面的编码搜索。
    /// </summary>
    /// <param name="ruleCode">规则编码（全局唯一）。</param>
    /// <returns>规则主表实体，不存在时返回 null。</returns>
    Task<RuleAggregate?> GetByCodeAsync(string ruleCode);

    /// <summary>
    /// 根据项目编码查询关联的所有规则。
    ///
    /// 使用场景：
    /// - 工作台展示某项目下的所有规则（包括草稿、已发布、已停用）。
    /// - 规则冲突校验时查询同项目的其他规则。
    /// </summary>
    /// <param name="itemCode">项目编码（HIS 物价项目编码）。</param>
    /// <returns>该项目关联的规则列表。</returns>
    Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode);

    /// <summary>
    /// 根据多个项目编码批量查询关联的所有规则。
    /// </summary>
    /// <param name="itemCodes">项目编码集合（HIS 物价项目编码）。</param>
    /// <returns>按项目编码分组的规则列表。</returns>
    /// <remarks>
    /// 默认实现按项目编码逐个委托 <see cref="GetByItemCodeAsync"/> 组装，保证任意实现都有正确的批量语义。
    /// 基础设施实现可覆盖为单条 SQL IN 批量查询以减少数据库往返。
    /// </remarks>
    async Task<IReadOnlyDictionary<string, IReadOnlyList<RuleAggregate>>> GetByItemCodesAsync(
        IReadOnlyCollection<string> itemCodes)
    {
        var groups = new Dictionary<string, IReadOnlyList<RuleAggregate>>(StringComparer.OrdinalIgnoreCase);
        foreach (var itemCode in itemCodes)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
            {
                continue;
            }

            var normalized = itemCode.Trim();
            if (!groups.ContainsKey(normalized))
            {
                groups[normalized] = await GetByItemCodeAsync(normalized);
            }
        }

        return groups;
    }

    /// <summary>
    /// 分页查询规则主表，支持按项目编码、状态、分类筛选。
    ///
    /// 使用场景：工作台的规则列表页面，支持多条件筛选和分页。
    /// </summary>
    /// <param name="itemCode">项目编码（可选筛选条件）。</param>
    /// <param name="status">规则状态（可选筛选条件，RuleStatus 的字符串表示）。</param>
    /// <param name="category">规则分类（可选筛选条件）。</param>
    /// <param name="pageIndex">页码（从0开始）。</param>
    /// <param name="pageSize">每页记录数。</param>
    /// <returns>分页结果，包含当前页数据列表和总记录数。</returns>
    Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(
        string? itemCode, string? status, string? category, int pageIndex, int pageSize);

    /// <summary>
    /// 查询在指定业务时间点所有已发布且在有效期内的规则。
    ///
    /// 使用场景：计价引擎在匹配规则时，加载当前时间点所有生效规则。
    /// 生效条件：RuleStatus = Published + 有效期内（startDate &lt;= businessTime &lt;= endDate）。
    ///
    /// 性能考虑：此查询在每次计价请求时调用，应使用缓存优化（规则发布/停用时刷新缓存）。
    /// </summary>
    /// <param name="businessTime">业务收费发生时间（非技术时间）。</param>
    /// <returns>生效规则列表。</returns>
    Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime);

    /// <summary>
    /// 插入一条规则主表记录，返回自增主键。
    ///
    /// 插入前应校验 ruleCode 是否已存在（ExistsAsync）。
    /// </summary>
    /// <param name="entity">规则主表实体，包含编码、名称、项目编码、状态等。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RuleAggregate entity);

    /// <summary>
    /// 更新规则主表（全量更新）。
    ///
    /// 使用场景：修改规则的基本信息（名称、描述、有效期等）。
    /// </summary>
    /// <param name="entity">要更新的规则主表实体，ID 必须存在。</param>
    /// <param name="expectedCurrentStatus">
    /// 期望的当前状态。传入后会做条件更新，只有数据库中的当前状态与期望值一致时才允许更新。
    /// 用于发布状态机做 CAS 式主档推进，避免并发事务在状态已经变化后继续覆盖写入。
    /// </param>
    /// <returns>是否更新成功。</returns>
    Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null);

    /// <summary>
    /// 检查指定规则编码是否已存在。
    ///
    /// 使用场景：新增规则前的唯一性校验。
    /// </summary>
    /// <param name="ruleCode">规则编码。</param>
    /// <returns>存在返回 true，不存在返回 false。</returns>
    Task<bool> ExistsAsync(string ruleCode);
}
