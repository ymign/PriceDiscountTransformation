using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// HIS 物价主数据仓储，负责按项目编码读取当前单价。
/// </summary>
/// <remarks>
/// 价格校验使用 HIS 主数据作为权威来源。该仓储只暴露规则中心需要的最小读接口，
/// 避免把物价主表完整结构泄漏到应用服务和计价引擎。
/// </remarks>
public sealed class PriceMasterRepository : IPriceMasterRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于读取 HIS 物价主数据视图或表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化物价主数据仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public PriceMasterRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按项目编码读取 HIS 当前单价。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>找到时返回单价；不存在时返回 <c>null</c>，由上层决定是否按价格不一致处理。</returns>
    public async Task<decimal?> GetUnitPriceAsync(string itemCode)
    {
        // 只读取首条匹配项目。物价版本、生效期等复杂逻辑应在 PriceMasterItem 映射的来源视图中先行处理。
        var item = await _db.Queryable<PriceMasterItem>()
            .FirstAsync(p => p.ItemCode == itemCode);
        return item?.UnitPrice;
    }
}
