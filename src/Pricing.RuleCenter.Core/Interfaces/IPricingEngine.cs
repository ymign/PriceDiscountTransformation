using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 统一计价引擎接口 —— 规则中心的核心计算入口。
///
/// 架构位置：
///   位于领域层（Domain），是规则匹配、公式执行、折价计算的唯一入口。
///   应用层（Application）通过此接口调用计价引擎，不直接接触规则匹配和公式执行的内部实现。
///
/// 职责边界：
///   - 接收计价上下文（PricingContext），执行完整的计价流程：
///     1. 按项目、场景、部位、时间匹配规则
///     2. 双单位换算（如已配置）
///     3. 公式计算（如已配置）
///     4. 金额下限 → 金额上限 → 日数量限制 → 时间窗数量限制
///     5. 同组互斥 / 同手术封顶
///     6. 子项加收 / 附加项目
///     7. 超出部分 → 0元（不是整单，不是拒单）
///   - 返回计价结果（PricingResult），包含折价明细、计算步骤追溯信息。
///   - 试算（simulate）不占用额度，仅确认（confirm）才占用。
///
/// 实现约束：
///   - 金额类型始终使用 decimal，禁止 double/float。
///   - 中间计算保留全部精度，最终金额保留2位小数、四舍五入。
///   - 权威单价必须由引擎内部读取，不得直接信任渠道传入的 unitPrice。
///   - 公式不自动跳过限制：是否执行数量/时间窗/同组/同手术限制以规则动作配置为准。
/// </summary>
public interface IPricingEngine
{
    /// <summary>
    /// 执行计价计算（试算或确认）。
    ///
    /// 计算流程按固定顺序执行：规则匹配 → 双单位换算 → 公式计算 → 金额限制 → 数量限制 → 互斥/封顶 → 子项加收。
    /// 试算不占用额度，不写入持久化追溯日志；确认计价占用额度并写入完整的追溯链。
    /// </summary>
    /// <param name="context">
    /// 计价上下文，包含：
    /// - 项目编码、收费场景、部位信息
    /// - 业务收费发生时间（非技术时间）
    /// - 数量、渠道来源（sourceSystem）、业务请求号（businessRequestNo）
    /// - 调用类型（CallType：试算/确认/提交/取消/冲正）
    /// - 多部位明细（pricingParts），用于多肿物、多部位、多面积项目的精确计价
    /// </param>
    /// <param name="batchContext">
    /// 批量计价共享上下文，可选。当一批请求包含多个项目时传入，
    /// 确保同批内限额累计、同组互斥和同手术封顶的跨项目判断正确。
    /// 为空时按单项目模式计算，不做跨项目共享。
    /// </param>
    /// <returns>
    /// 计价结果，包含：
    /// - 原始金额、折后金额、折扣率
    /// - 折价明细列表（每条规则动作的计算结果）
    /// - 计算步骤追溯信息（用于追溯查询接口）
    /// - 匹配到的规则版本信息
    /// </returns>
    Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null);
}
