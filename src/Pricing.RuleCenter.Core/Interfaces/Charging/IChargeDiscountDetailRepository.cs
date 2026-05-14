using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Core.Interfaces.Charging;

/// <summary>
/// 折价明细仓储接口 —— 折价追溯中心的数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_CHARGE_DISCOUNT_DETAIL。
///
/// 职责边界：
///   - 管理单笔计价请求产生的折价结果明细记录。
///   - 每条明细记录一条规则动作的计算结果（原始金额、折后金额、折扣率、匹配规则信息）。
///   - 明细记录与请求日志（PR_CHARGE_REQUEST_LOG）通过 requestId 关联。
///
/// 追溯链路：
///   折价明细是"最终结果链"的核心数据，支持 /api/pricing/trace/query 接口的折价结果查询。
///   每条明细可关联到触发它的规则版本和计算步骤（PR_CHARGE_TRACE_STEP）。
///
/// 状态同步约束：
///   confirm/commit/cancel/reverse/expire 操作必须同时更新本表的状态字段，
///   与请求日志（BusinessStatus）和额度占用（OccupyStatus）保持一致。
/// </summary>
public interface IChargeDiscountDetailRepository
{
    /// <summary>
    /// 根据请求ID查询该笔计价请求的所有折价明细。
    ///
    /// 使用场景：追溯查询接口（/api/pricing/trace/query）展示某笔请求的折价结果明细列表。
    /// </summary>
    /// <param name="requestId">计价请求日志ID（PR_CHARGE_REQUEST_LOG.ID）。</param>
    /// <returns>折价明细列表，按创建时间排序；无记录时返回空列表。</returns>
    Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId);

    /// <summary>
    /// 插入一条折价明细记录，返回自增主键。
    ///
    /// 调用时机：计价引擎计算完成后，将每条规则动作的计算结果写入明细表。
    /// 注意：Oracle 无自增，主键通过 SEQUENCE 生成（SqlSugar 配置）。
    /// </summary>
    /// <param name="entity">折价明细实体，包含请求ID、项目编码、原始金额、折后金额、折扣率、规则版本ID等。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(ChargeDiscountDetail entity);

    /// <summary>
    /// 批量更新指定请求下所有折价明细的状态。
    ///
    /// 调用时机：commit/cancel/reverse/expire 操作时，同步更新该请求下所有明细的状态。
    /// 保证请求日志、额度占用、折价明细三者状态一致。
    /// </summary>
    /// <param name="requestId">计价请求日志ID。</param>
    /// <param name="status">目标状态值（与 BusinessStatus 对应的字符串表示）。</param>
    Task UpdateStatusByRequestIdAsync(long requestId, string status);
}
