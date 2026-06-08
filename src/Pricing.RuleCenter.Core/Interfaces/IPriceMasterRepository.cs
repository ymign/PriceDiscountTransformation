using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 物价主数据仓储接口 —— 权威单价查询的数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 HIS 侧的物价主数据表（具体表名和同步方式待确认）。
///
/// 职责边界：
///   - 提供项目权威价格的查询能力。
///   - 计价链路可读取权威单价并记录诊断日志，用于联调、对账和发现 HIS 传价异常。
///   - 当前不在规则中心按权威单价阻断业务流程，基础单价仍由 HIS 负责带出。
///
/// 资金安全约束：
///   - 权威单价诊断是资金安全的辅助观测手段，防止把不完整上下文下的半套校验误当成强防线。
///   - 试算（simulate）和确认（confirm）当前都使用渠道传入基础单价，规则中心仅记录诊断差异。
///   - 后续如果规则中心要恢复强校验，必须同步接管可信价格形态、合同单位、患者事实和价格版本。
/// </summary>
public interface IPriceMasterRepository
{
    /// <summary>
    /// 查询指定项目的权威单价。
    ///
    /// 使用场景：计价链路记录渠道传入单价与三甲权威单价是否一致。
    /// 当前只作为诊断数据，不用于拒绝本次计价。
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
    /// 使用场景：批量试算、批量确认或权威单价诊断时，避免对每条明细逐个访问数据库。
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
            if (string.IsNullOrWhiteSpace(itemCode))
            {
                continue;
            }

            var normalizedItemCode = itemCode.Trim();
            if (result.ContainsKey(normalizedItemCode))
            {
                continue;
            }

            result[normalizedItemCode] = await GetUnitPriceAsync(normalizedItemCode);
        }

        return result;
    }

    /// <summary>
    /// 批量查询多个项目的权威物价主数据。
    ///
    /// 使用场景：权威单价诊断需要根据患者上下文选择三甲价、儿童价或围产价时，
    /// 不能只返回单一 UNIT_PRICE，否则诊断日志会把儿童或围产患者错误地按普通三甲价比较。
    /// 默认实现为了兼容旧测试桩，会基于 <see cref="GetUnitPricesAsync(IReadOnlyCollection{string})"/>
    /// 构造只包含三甲价的结果；生产仓储应覆盖为单次批量查询实现。
    /// </summary>
    /// <param name="itemCodes">项目编码集合。</param>
    /// <returns>以项目编码为键的物价主数据；项目不存在时值为 null。</returns>
    async Task<IReadOnlyDictionary<string, PriceMasterItem?>> GetPriceItemsAsync(IReadOnlyCollection<string> itemCodes)
    {
        var unitPrices = await GetUnitPricesAsync(itemCodes);
        var result = new Dictionary<string, PriceMasterItem?>(StringComparer.OrdinalIgnoreCase);
        foreach (var itemCode in unitPrices.Keys)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
            {
                continue;
            }

            var normalizedItemCode = itemCode.Trim();
            result[normalizedItemCode] = unitPrices[itemCode].HasValue
                ? new PriceMasterItem
                {
                    ItemCode = normalizedItemCode,
                    UnitPrice = unitPrices[itemCode]
                }
                : null;
        }

        return result;
    }
}
