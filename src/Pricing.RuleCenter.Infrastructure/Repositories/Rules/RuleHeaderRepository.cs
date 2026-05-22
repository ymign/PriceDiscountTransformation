using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则主档仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_HEADER 表的全部数据访问操作，包括：按主键查询、按编码查询、按项目编码查询、
/// 分页查询、查询生效规则、插入、更新、判断编码是否存在。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_RULE_HEADER 是规则匹配的第一层过滤对象。每条记录代表一条计价规则的主档信息，
/// 包括规则编码、项目编码、状态、优先级、生效期等。
/// 规则主档通过 CURRENT_VERSION 字段关联当前生效的规则版本（PR_RULE_VERSION）。
/// </para>
/// <para>
/// 【规则匹配流程】
/// 计价引擎按以下顺序使用本仓储：
///   1. 调用 GetEffectiveAsync(businessTime) 获取当前可参与匹配的规则主档集合
///   2. 按优先级（Priority）逐条尝试匹配条件（PR_RULE_CONDITION）
///   3. 命中后通过 CURRENT_VERSION 查找版本明细，再读取动作（PR_RULE_ACTION）
/// </para>
/// <para>
/// 【状态枚举】
///   - DRAFT     — 草稿，不可参与计价
///   - PUBLISHED — 已发布，可参与计价
///   - DISABLED  — 已停用，不可参与计价
///   - ROLLED_BACK — 已回滚，不可参与计价
/// </para>
/// </remarks>
public sealed class RuleHeaderRepository : IRuleHeaderRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和规则主档表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则主档仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleHeaderRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按主键读取规则主档。
    /// </summary>
    /// <param name="ruleId">规则主键（由 Oracle 序列 SEQ_PR_RULE_HEADER 生成）。</param>
    /// <returns>规则聚合实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_HEADER WHERE RULE_ID = :ruleId
    /// </code>
    /// 【使用场景】规则详情查看、编辑前回显、发布前校验。
    /// </remarks>
    public async Task<RuleAggregate?> GetByIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleAggregate>()
            .InSingleAsync(ruleId);
    }

    /// <summary>
    /// 按主键读取规则主档并执行数据库行锁。
    /// </summary>
    /// <remarks>
    /// 使用 Oracle `SELECT ... FOR UPDATE` 串行化发布状态机入口，避免两个事务同时读取到同一条主档并各自推进状态。
    /// 必须在外层事务已开启时调用。
    /// </remarks>
    public async Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId)
    {
        var rows = await _db.Ado.SqlQueryAsync<RuleAggregate>(
            "SELECT * FROM PR_RULE_HEADER WHERE RULE_ID = :RuleId FOR UPDATE",
            new { RuleId = ruleId });
        return rows.FirstOrDefault();
    }

    /// <summary>
    /// 按规则编码读取规则主档。
    /// </summary>
    /// <param name="ruleCode">规则编码（全局唯一）。</param>
    /// <returns>规则聚合实体；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_HEADER WHERE RULE_CODE = :ruleCode
    /// </code>
    /// 【使用场景】按编码定位规则，如 API 入参使用规则编码而非主键时。
    /// </remarks>
    public async Task<RuleAggregate?> GetByCodeAsync(string ruleCode)
    {
        return await _db.Queryable<RuleAggregate>()
            .FirstAsync(r => r.RuleCode == ruleCode);
    }

    /// <summary>
    /// 按项目编码读取启用规则主档候选，按优先级升序排列。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>按 Priority 升序排列的启用规则聚合集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【候选范围】
    /// <list type="bullet">
    /// <item><description>ITEM 作用域：PR_RULE_HEADER.ITEM_CODE = itemCode</description></item>
    /// <item><description>GROUP 作用域：当前项目在 PR_ITEM_GROUP_DETAIL 中所属组对应的 PR_RULE_HEADER.GROUP_CODE</description></item>
    /// </list>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_HEADER
    /// WHERE IS_ENABLED = 'Y'
    ///   AND (
    ///     ITEM_CODE = :itemCode
    ///     OR (
    ///       RULE_SCOPE = 'GROUP'
    ///       AND GROUP_CODE IN (
    ///         SELECT G.GROUP_CODE
    ///         FROM PR_ITEM_GROUP_DETAIL D
    ///         JOIN PR_ITEM_GROUP G ON G.GROUP_ID = D.GROUP_ID
    ///         WHERE D.ITEM_CODE = :itemCode
    ///       )
    ///     )
    ///   )
    /// ORDER BY PRIORITY ASC
    /// </code>
    /// 【使用场景】计价引擎匹配候选规则。
    /// 【优先级语义】Priority 值越小优先级越高，计价引擎按此顺序逐条尝试匹配。
    /// </remarks>
    public async Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
        {
            return Array.Empty<RuleAggregate>();
        }

        var normalizedItemCode = itemCode.Trim();

        var groupCodes = await _db.Queryable<ItemGroupDetail, ItemGroup>((detail, group) => new JoinQueryInfos(
                JoinType.Inner, detail.GroupId == group.GroupId))
            .Where((detail, group) =>
                detail.ItemCode == normalizedItemCode &&
                detail.IsEnabled == "Y" &&
                group.IsEnabled == "Y")
            .Select((detail, group) => group.GroupCode)
            .Distinct()
            .ToListAsync();

        var query = _db.Queryable<RuleAggregate>()
            .Where(r => r.IsEnabled == "Y");

        query = groupCodes.Count == 0
            ? query.Where(r => r.ItemCode == normalizedItemCode)
            : query.Where(r =>
                r.ItemCode == normalizedItemCode ||
                (r.RuleScope == "GROUP" &&
                 r.GroupCode != null &&
                 groupCodes.Contains(r.GroupCode)));

        return await query
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// 分页查询规则主档。
    /// </summary>
    /// <param name="itemCode">项目编码筛选条件；为 null 或空字符串时不限制。</param>
    /// <param name="status">规则状态筛选条件（DRAFT/PUBLISHED/DISABLED/ROLLED_BACK）；为 null 时不限制。</param>
    /// <param name="category">规则分类筛选条件；为 null 时不限制。</param>
    /// <param name="pageIndex">页码，从 1 开始。</param>
    /// <param name="pageSize">每页记录数。</param>
    /// <returns>元组：(当前页规则聚合列表, 符合条件的总记录数)。</returns>
    /// <remarks>
    /// 【SQL 语义】以全部条件为例，等价于：
    /// <code>
    /// SELECT COUNT(*) FROM PR_RULE_HEADER
    /// WHERE ITEM_CODE = :itemCode AND STATUS = :status AND RULE_CATEGORY = :category
    /// -- 分页
    /// SELECT * FROM (
    ///   SELECT ROWNUM RN, T.* FROM (
    ///     SELECT * FROM PR_RULE_HEADER
    ///     WHERE ITEM_CODE = :itemCode AND STATUS = :status AND RULE_CATEGORY = :category
    ///     ORDER BY PRIORITY ASC
    ///   ) T WHERE ROWNUM &lt;= :endRow
    /// ) WHERE RN &gt; :startRow
    /// </code>
    /// 【动态条件】使用 WhereIF 避免手写多套 SQL 分支。空值条件不会加入 WHERE 子句。
    /// 【排序】按优先级排序便于发现同一项目下的执行顺序。
    /// </remarks>
    public async Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(
        string? itemCode, string? status, string? category, int pageIndex, int pageSize)
    {
        // 配置页面查询使用动态条件，排序保持按优先级查看，便于发现同一项目下的执行顺序。
        var total = new RefAsync<int>();
        var items = await _db.Queryable<RuleAggregate>()
            .WhereIF(!string.IsNullOrEmpty(itemCode), r => r.ItemCode == itemCode)
            .WhereIF(!string.IsNullOrEmpty(status), r => r.Status == status)
            .WhereIF(!string.IsNullOrEmpty(category), r => r.RuleCategory == category)
            .OrderBy(r => r.Priority)
            .ToPageListAsync(pageIndex, pageSize, total);
        return (items, total.Value);
    }

    /// <summary>
    /// 按业务时间读取当前可参与计价匹配的规则主档。
    /// </summary>
    /// <param name="businessTime">
    /// HIS 业务发生时间（不是接口到达时间）。
    /// 补录或延迟提交场景应以收费业务时间为准。
    /// </param>
    /// <returns>满足启用、已发布和生效期条件的规则聚合集合，按优先级升序排列。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_HEADER
    /// WHERE IS_ENABLED = 'Y'
    ///   AND STATUS = 'PUBLISHED'
    ///   AND (EFFECTIVE_FROM IS NULL OR EFFECTIVE_FROM &lt;= :businessTime)
    ///   AND (EFFECTIVE_TO IS NULL OR EFFECTIVE_TO &gt;= :businessTime)
    /// ORDER BY PRIORITY ASC
    /// </code>
    /// 【过滤逻辑分层说明】
    ///   第一层 — IS_ENABLED = 'Y'：排除手动停用的规则
    ///   第二层 — STATUS = 'PUBLISHED'：排除草稿、回滚版本，未审批规则不能影响真实收费
    ///   第三层 — 生效期过滤：NULL 表示不设限（"NULL ≠ 0"原则），已设值则按业务时间比较
    /// 【业务时间优先】使用 businessTime 而非 DateTime.Now，保证补录和延迟提交场景的正确性。
    /// 【缓存建议】生效规则集合变化频率低（仅发布/停用/回滚时变化），建议在应用层做缓存，
    /// 规则变更时立即失效缓存。
    /// </remarks>
    public async Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime)
    {
        // ========== 第一阶段：过滤启用且已发布规则 ==========
        // 草稿、停用和回滚版本都不能进入计价引擎，否则会出现未审批规则影响真实收费。
        return await _db.Queryable<RuleAggregate>()
            .Where(r => r.IsEnabled == "Y" && r.Status == "PUBLISHED")
            // ========== 第二阶段：按业务发生时间过滤生效期 ==========
            // 补录或延迟提交场景应以收费业务时间为准，而不是当前系统时间。
            // EffectiveFrom / EffectiveTo 为 NULL 表示不设限（NULL ≠ 0 原则）。
            .Where(r => (r.EffectiveFrom == null || r.EffectiveFrom <= businessTime))
            .Where(r => (r.EffectiveTo == null || r.EffectiveTo >= businessTime))
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// 插入规则主档。
    /// </summary>
    /// <param name="entity">待写入的规则聚合实体。</param>
    /// <returns>Oracle 序列生成的规则主键（RULE_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_HEADER.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_HEADER (...) VALUES (...)
    /// </code>
    /// 【审计字段】CreatedAt 和 UpdatedAt 由仓储兜底写入，保证绕过服务层的内部调用也能保留基础审计字段。
    /// 【约束】RULE_CODE 的唯一性由数据库约束保证。
    /// </remarks>
    public async Task<long> InsertAsync(RuleAggregate entity)
    {
        // 主档创建时间由仓储兜底写入，保证绕过服务层的内部调用也能保留基础审计字段。
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_HEADER.NEXTVAL FROM DUAL");
        entity.RuleId = seq;
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新规则主档（主键、编码和创建信息不可更新）。
    /// </summary>
    /// <param name="entity">包含最新主档状态或基础信息的规则聚合实体。</param>
    /// <returns>是否至少更新了一行（true 表示成功，false 表示记录不存在）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_RULE_HEADER
    /// SET ITEM_CODE = ..., RULE_CATEGORY = ..., RULE_NAME = ..., PRIORITY = ...,
    ///     STATUS = ..., IS_ENABLED = ..., EFFECTIVE_FROM = ..., EFFECTIVE_TO = ...,
    ///     DESCRIPTION = ..., UPDATED_AT = SYSDATE, UPDATED_BY = ...
    /// WHERE RULE_ID = :entity.RuleId
    /// </code>
    /// 注意：RULE_ID、RULE_CODE、CREATED_BY、CREATED_AT 不会被更新（通过 IgnoreColumns 排除）。
    /// 【不可变字段】RuleId 和 RuleCode 保持不可变，避免更新入口破坏历史审计和外部引用。
    /// CreatedBy 和 CreatedAt 不可变，创建信息只在 InsertAsync 时写入。
    /// </remarks>
    public async Task<bool> UpdateAsync(RuleAggregate entity)
    {
        // RuleId、RuleCode 和创建信息保持不可变，避免更新入口破坏历史审计和外部引用。
        entity.UpdatedAt = DateTime.Now;
        var rows = await _db.Updateable(entity)
            .IgnoreColumns(r => new { r.RuleId, r.RuleCode, r.CreatedBy, r.CreatedAt })
            .ExecuteCommandAsync();
        return rows > 0;
    }

    /// <summary>
    /// 判断规则编码是否已经存在。
    /// </summary>
    /// <param name="ruleCode">规则编码。</param>
    /// <returns>存在时返回 <c>true</c>（不论规则状态）。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT CASE WHEN EXISTS (
    ///   SELECT 1 FROM PR_RULE_HEADER WHERE RULE_CODE = :ruleCode
    /// ) THEN 1 ELSE 0 END FROM DUAL
    /// </code>
    /// 【使用场景】新增规则前的唯一性校验。不过滤状态，防止编码冲突。
    /// </remarks>
    public async Task<bool> ExistsAsync(string ruleCode)
    {
        return await _db.Queryable<RuleAggregate>()
            .AnyAsync(r => r.RuleCode == ruleCode);
    }
}
