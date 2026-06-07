using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// HIS 物价主数据仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 按项目编码读取 HIS 当前单价。只暴露规则中心需要的最小读接口，
/// 避免把物价主表完整结构泄漏到应用服务和计价引擎。
/// </para>
/// <para>
/// 【权威单价校验】
/// 根据稳定性要求，confirm 接口不得直接信任渠道传入的 unitPrice，
/// 必须由计价中心读取权威单价或强校验，不一致时返回 PRICE_MISMATCH。
/// 本仓储提供的 GetUnitPriceAsync 就是权威单价的读取入口。
/// </para>
/// <para>
/// 【数据来源】
/// PriceMasterItem 映射的可能是 HIS 物价主表、物价同步视图或缓存表。
/// 物价版本、生效期等复杂逻辑应在数据源层面（视图或同步脚本）先行处理，
/// 本仓储只读取当前有效单价。
/// </para>
/// <para>
/// 【待确认事项】
/// 权威物价单价从 HIS 哪张表或同步表读取，价格版本如何追溯 —— 需与 HIS 团队确认。
/// </para>
/// </remarks>
public sealed class PriceMasterRepository : IPriceMasterRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于读取 HIS 物价主数据视图或表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化物价主数据仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public PriceMasterRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按项目编码读取 HIS 当前单价。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>
    /// 找到时返回单价（decimal，NUMBER(18,4)）；不存在时返回 <c>null</c>，
    /// 由上层决定是否按价格不一致（PRICE_MISMATCH）处理。
    /// </returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT UNIT_PRICE FROM PRICE_MASTER_ITEM WHERE ITEM_CODE = :itemCode
    /// </code>
    /// 只读取首条匹配记录。若物价主表中同一项目存在多条生效版本，
    /// 应在 PriceMasterItem 映射的来源视图中通过 WHERE 条件预先过滤。
    /// 【金额类型】返回值为 decimal（C#）对应 NUMBER(18,4)（Oracle），
    /// 禁止使用 double 或 float 替代，避免浮点精度丢失导致金额错误。
    /// 【使用场景】
    ///   - confirm 接口的权威单价校验
    ///   - simulate 接口的单价参考（不强制校验）
    /// </remarks>
    public async Task<decimal?> GetUnitPriceAsync(string itemCode)
    {
        // 只读取首条匹配项目。物价版本、生效期等复杂逻辑应在 PriceMasterItem 映射的来源视图中先行处理。
        var item = await _db.Queryable<PriceMasterItem>()
            .FirstAsync(p => p.ItemCode == itemCode);
        return item?.UnitPrice;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<string, decimal?>> GetUnitPricesAsync(IReadOnlyCollection<string> itemCodes)
    {
        var normalizedCodes = itemCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedCodes.Length == 0)
        {
            return new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
        }

        var items = await _db.Queryable<PriceMasterItem>()
            .Where(item => normalizedCodes.Contains(item.ItemCode))
            .ToListAsync();

        var result = normalizedCodes.ToDictionary(
            code => code,
            _ => (decimal?)null,
            StringComparer.OrdinalIgnoreCase);

        foreach (var group in items.GroupBy(item => item.ItemCode, StringComparer.OrdinalIgnoreCase))
        {
            result[group.Key] = group.First().UnitPrice;
        }

        return result;
    }
}
