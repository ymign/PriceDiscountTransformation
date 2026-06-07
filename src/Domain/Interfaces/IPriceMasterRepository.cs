namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 物价主数据仓储接口 —— 权威单价查询的数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 HIS 侧的物价主数据表（具体表名和同步方式待确认）。
///
/// 职责边界：
///   - 提供项目权威单价的查询能力。
///   - 计价引擎在 confirm（确认计价）时，必须读取权威单价，不得直接信任渠道传入的 unitPrice。
///   - 如果渠道传入的单价与权威单价不一致，返回 PRICE_MISMATCH 错误。
///
/// 资金安全约束：
///   - 权威单价是资金安全的关键防线，防止渠道侧篡改单价导致资损。
///   - 试算（simulate）可以使用渠道传入单价（仅供参考展示），但确认计价必须校验。
///   - 权威物价单价从 HIS 哪张表或同步表读取，以及价格版本如何追溯，待确认。
/// </summary>
public interface IPriceMasterRepository
{
    /// <summary>
    /// 查询指定项目的权威单价。
    ///
    /// 使用场景：计价引擎在 confirm 阶段校验渠道传入单价与权威单价是否一致。
    /// 如果不一致，返回 PRICE_MISMATCH 错误，拒绝本次计价。
    ///
    /// 注意：
    /// - 返回 null 表示该项目在物价主数据中不存在。
    /// - 单价类型为 decimal（Oracle NUMBER(18,4)），禁止 double/float。
    /// </summary>
    /// <param name="itemCode">项目编码（与 HIS 物价项目编码一致）。</param>
    /// <returns>项目权威单价（decimal），项目不存在时返回 null。</returns>
    Task<decimal?> GetUnitPriceAsync(string itemCode);

    /// <summary>
    /// 批量查询多个项目的权威单价。
    ///
    /// 使用场景：批量试算、批量确认或权威单价校验时，避免对每条明细逐个访问数据库。
    /// 默认实现为了兼容旧测试桩，会退回逐条调用 <see cref="GetUnitPriceAsync(string)"/>；
    /// 生产仓储应覆盖为单次批量查询实现。
    /// </summary>
    /// <param name="itemCodes">项目编码集合。</param>
    /// <returns>以项目编码为键的单价字典；项目不存在时值为 null。</returns>
    async Task<IReadOnlyDictionary<string, decimal?>> GetUnitPricesAsync(IReadOnlyCollection<string> itemCodes)
    {
        var result = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
        foreach (var itemCode in itemCodes)
        {
            if (string.IsNullOrWhiteSpace(itemCode) || result.ContainsKey(itemCode))
            {
                continue;
            }

            result[itemCode] = await GetUnitPriceAsync(itemCode);
        }

        return result;
    }
}
