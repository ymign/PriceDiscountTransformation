using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 规则主档仓储实现，封装 PR_RULE_HEADER 的读取、分页、有效规则查询和写入。
/// </summary>
/// <remarks>
/// 规则主档是规则匹配的第一层过滤对象。计价引擎会按业务时间读取已发布且启用的主档，
/// 再根据 CurrentVersion 查找版本明细，因此这里的有效规则查询必须严格过滤状态、生效期和排序。
/// </remarks>
public sealed class RuleHeaderRepository : IRuleHeaderRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和规则主档表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则主档仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public RuleHeaderRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按主键读取规则主档。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则主档实体；不存在时返回 <c>null</c>。</returns>
    public async Task<RuleHeader?> GetByIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleHeader>()
            .InSingleAsync(ruleId);
    }

    /// <summary>
    /// 按规则编码读取规则主档。
    /// </summary>
    /// <param name="ruleCode">规则编码。</param>
    /// <returns>规则主档实体；不存在时返回 <c>null</c>。</returns>
    public async Task<RuleHeader?> GetByCodeAsync(string ruleCode)
    {
        return await _db.Queryable<RuleHeader>()
            .FirstAsync(r => r.RuleCode == ruleCode);
    }

    /// <summary>
    /// 按项目编码读取启用规则主档。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>按优先级升序排列的规则主档集合。</returns>
    public async Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode)
    {
        return await _db.Queryable<RuleHeader>()
            .Where(r => r.ItemCode == itemCode && r.IsEnabled == "Y")
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// 分页查询规则主档。
    /// </summary>
    /// <param name="itemCode">项目编码筛选条件；为空时不限制。</param>
    /// <param name="status">规则状态筛选条件；为空时不限制。</param>
    /// <param name="category">规则分类筛选条件；为空时不限制。</param>
    /// <param name="pageIndex">页码，从 1 开始。</param>
    /// <param name="pageSize">每页记录数。</param>
    /// <returns>当前页规则主档和符合条件的总记录数。</returns>
    public async Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(
        string? itemCode, string? status, string? category, int pageIndex, int pageSize)
    {
        // 配置页面查询使用动态条件，排序保持按优先级查看，便于发现同一项目下的执行顺序。
        var total = new RefAsync<int>();
        var items = await _db.Queryable<RuleHeader>()
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
    /// <param name="businessTime">HIS 业务发生时间，而不是接口到达时间。</param>
    /// <returns>满足启用、已发布和生效期条件的规则主档集合。</returns>
    public async Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime)
    {
        // ========== 第一阶段：过滤启用且已发布规则 ==========
        // 草稿、停用和回滚版本都不能进入计价引擎，否则会出现未审批规则影响真实收费。
        return await _db.Queryable<RuleHeader>()
            .Where(r => r.IsEnabled == "Y" && r.Status == "PUBLISHED")
            // ========== 第二阶段：按业务发生时间过滤生效期 ==========
            // 补录或延迟提交场景应以收费业务时间为准，而不是当前系统时间。
            .Where(r => (r.EffectiveFrom == null || r.EffectiveFrom <= businessTime))
            .Where(r => (r.EffectiveTo == null || r.EffectiveTo >= businessTime))
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// 插入规则主档。
    /// </summary>
    /// <param name="entity">待写入的规则主档实体。</param>
    /// <returns>Oracle 序列生成的规则主键。</returns>
    public async Task<long> InsertAsync(RuleHeader entity)
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
    /// 更新规则主档。
    /// </summary>
    /// <param name="entity">包含最新主档状态或基础信息的规则主档实体。</param>
    /// <returns>是否至少更新了一行。</returns>
    public async Task<bool> UpdateAsync(RuleHeader entity)
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
    /// <returns>存在时返回 <c>true</c>。</returns>
    public async Task<bool> ExistsAsync(string ruleCode)
    {
        return await _db.Queryable<RuleHeader>()
            .AnyAsync(r => r.RuleCode == ruleCode);
    }
}
