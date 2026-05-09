using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 计价折扣明细仓储，负责 PR_CHARGE_DISCOUNT_DETAIL 的查询、写入和状态推进。
/// </summary>
/// <remarks>
/// 折扣明细记录“原始数量/金额”和“规则处理后的数量/金额”。它是对账和追踪页面解释计价结果的核心表，
/// 因此状态会随着 CONFIRM、COMMIT、CANCEL、EXPIRE、REVERSE 同步变化。
/// </remarks>
public sealed class ChargeDiscountDetailRepository : IChargeDiscountDetailRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和折扣明细表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化折扣明细仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public ChargeDiscountDetailRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按请求日志主键读取折扣明细。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <returns>该次计价产生的折扣明细集合。</returns>
    public async Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeDiscountDetail>()
            .Where(d => d.RequestId == requestId)
            .ToListAsync();
    }

    /// <summary>
    /// 插入折扣明细。
    /// </summary>
    /// <param name="entity">待写入的折扣明细实体。</param>
    /// <returns>Oracle 序列生成的折扣明细主键。</returns>
    public async Task<long> InsertAsync(ChargeDiscountDetail entity)
    {
        // 折扣明细主键由 Oracle 序列生成，便于和数据库脚本中的 PR_ 表保持一致。
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_DISC_DTL.NEXTVAL FROM DUAL");
        entity.DiscountId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 按请求日志主键批量更新折扣明细状态。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <param name="status">目标状态，例如 CONFIRMED、CANCELLED、EXPIRED 或 REVERSED。</param>
    public async Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        // ========== 第一阶段：构造基础状态更新 ==========
        // 应用服务已经保证状态流转合法，仓储只把指定状态同步到该请求下所有折扣明细。
        var now = DateTime.Now;
        var update = _db.Updateable<ChargeDiscountDetail>()
            .SetColumns(d => d.Status == status);

        // ========== 第二阶段：根据状态补记业务时间 ==========
        // 这些时间字段用于追踪页面和后续对账区分“何时确认、何时取消、何时过期、何时冲正”。
        update = status switch
        {
            "CONFIRMED" => update.SetColumns(d => d.CommittedAt == now),
            "CANCELLED" => update.SetColumns(d => d.CancelledAt == now),
            "EXPIRED" => update.SetColumns(d => d.ExpiredAt == now),
            "REVERSED" => update.SetColumns(d => d.ReversedAt == now),
            _ => update
        };

        await update.Where(d => d.RequestId == requestId).ExecuteCommandAsync();
    }
}
