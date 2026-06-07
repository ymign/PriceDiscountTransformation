using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Charging;

/// <summary>
/// 计价折扣明细仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_CHARGE_DISCOUNT_DETAIL 表的全部数据访问操作，包括：按请求ID查询、插入、按请求ID批量更新状态。
/// </para>
/// <para>
/// 【表结构语义】
/// PR_CHARGE_DISCOUNT_DETAIL 记录每一次计价请求中每条收费明细的"原始数量/金额"和
/// "规则处理后的数量/金额"。它是对账和追溯页面解释计价结果的核心表。
/// 一条请求日志（PR_CHARGE_REQUEST_LOG）对应 N 条折扣明细（1:N 关系）。
/// </para>
/// <para>
/// 【状态流转】
/// 折扣明细状态会随着请求日志的生命周期同步变化：
///   - confirm 时写入 PENDING
///   - commit 时更新为 CONFIRMED
///   - cancel 时更新为 CANCELLED
///   - 过期清理时更新为 EXPIRED
///   - 退费冲销时更新为 REVERSED
/// 状态合法性由应用服务保证，仓储只执行指定更新。
/// </para>
/// <para>
/// 【事务要求】
/// 插入操作必须与请求日志在同一个事务中执行，保证失败时一起回滚。
/// 批量状态更新也应与请求日志和限额占用的状态更新在同一事务中。
/// </para>
/// </remarks>
public sealed class ChargeDiscountDetailRepository : IChargeDiscountDetailRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和折扣明细表。
    /// </remarks>
    private readonly ISqlSugarClient _db;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化折扣明细仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    /// <param name="clock">技术时间提供者，用于统一补记状态时间。</param>
    public ChargeDiscountDetailRepository(ISqlSugarClient db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <summary>
    /// 按请求日志主键读取该次计价产生的全部折扣明细。
    /// </summary>
    /// <param name="requestId">请求日志主键（PR_CHARGE_REQUEST_LOG.REQUEST_ID）。</param>
    /// <returns>该次请求关联的折扣明细集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_CHARGE_DISCOUNT_DETAIL WHERE REQUEST_ID = :requestId
    /// </code>
    /// 【使用场景】
    ///   - 追溯页面展示计价结果明细（原始 vs 折后）
    ///   - 对账服务比对折扣明细与 HIS 实际收费
    ///   - 退费时查找原收费明细以计算释放额度
    /// 【返回排序】仓储不强制排序；若需按收费明细序号排序，由调用方处理。
    /// </remarks>
    public async Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeDiscountDetail>()
            .Where(d => d.RequestId == requestId)
            .ToListAsync();
    }

    /// <summary>
    /// 插入一条折扣明细。
    /// </summary>
    /// <param name="entity">待写入的折扣明细实体，调用方应确保 RequestId 已关联到有效的请求日志。</param>
    /// <returns>Oracle 序列生成的折扣明细主键（DISCOUNT_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// -- 第一步：从 Oracle 序列获取自增主键
    /// SELECT SEQ_PR_CHARGE_DISC_DTL.NEXTVAL FROM DUAL
    /// -- 第二步：插入完整记录
    /// INSERT INTO PR_CHARGE_DISCOUNT_DETAIL (...) VALUES (...)
    /// </code>
    /// 【主键生成策略】使用 Oracle 序列生成，便于和数据库脚本中的 PR_ 前缀表保持一致。
    /// 序列值在内存中赋给实体后再执行 INSERT。
    /// 【事务边界】本方法不开启事务。调用方应在更大的事务中调用本方法，
    /// 以保证折扣明细与请求日志、执行步骤、限额占用的原子写入。
    /// 【金额类型】所有金额字段在 C# 中为 decimal，Oracle 中为 NUMBER(18,4)，
    /// 中间计算保留全部精度，最终金额保留 2 位小数四舍五入。
    /// </remarks>
    public async Task<long> InsertAsync(ChargeDiscountDetail entity)
    {
        // 折扣明细主键由 Oracle 序列生成，便于和数据库脚本中的 PR_ 表保持一致。
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_DISC_DTL.NEXTVAL FROM DUAL");
        entity.DiscountId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 按请求日志主键批量更新折扣明细状态及对应业务时间。
    /// </summary>
    /// <param name="requestId">请求日志主键，定位要更新的折扣明细范围。</param>
    /// <param name="status">
    /// 目标状态字符串。合法值包括：
    ///   - CONFIRMED — HIS 已落账成功
    ///   - CANCELLED — 确认已取消
    ///   - EXPIRED   — 保护期超时自动过期
    ///   - REVERSED  — 退费冲销
    /// </param>
    /// <remarks>
    /// 【SQL 语义】根据 status 值不同，等价于以下之一：
    /// <code>
    /// -- status = CONFIRMED
    /// UPDATE PR_CHARGE_DISCOUNT_DETAIL
    /// SET STATUS = 'CONFIRMED', COMMITTED_AT = SYSDATE
    /// WHERE REQUEST_ID = :requestId
    ///
    /// -- status = CANCELLED
    /// UPDATE PR_CHARGE_DISCOUNT_DETAIL
    /// SET STATUS = 'CANCELLED', CANCELLED_AT = SYSDATE
    /// WHERE REQUEST_ID = :requestId
    ///
    /// -- status = EXPIRED
    /// UPDATE PR_CHARGE_DISCOUNT_DETAIL
    /// SET STATUS = 'EXPIRED', EXPIRED_AT = SYSDATE
    /// WHERE REQUEST_ID = :requestId
    ///
    /// -- status = REVERSED
    /// UPDATE PR_CHARGE_DISCOUNT_DETAIL
    /// SET STATUS = 'REVERSED', REVERSED_AT = SYSDATE
    /// WHERE REQUEST_ID = :requestId
    /// </code>
    /// 【业务时间补记】不同状态对应不同业务时间字段，用于追溯页面展示
    /// "何时确认、何时取消、何时过期、何时冲正"。
    /// 【状态一致性】应用服务必须在调用本方法的同时更新请求日志状态和限额占用状态，
    /// 保证三张表在同一事务中同步推进。
    /// </remarks>
    public async Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        // ========== 第一阶段：构造基础状态更新 ==========
        // 应用服务已经保证状态流转合法，仓储只把指定状态同步到该请求下所有折扣明细。
        var now = _clock.Now;
        var update = _db.Updateable<ChargeDiscountDetail>()
            .SetColumns(d => d.Status == status);

        // ========== 第二阶段：根据状态补记业务时间 ==========
        // 这些时间字段用于追踪页面和后续对账区分"何时确认、何时取消、何时过期、何时冲正"。
        // 每个分支只设置对应的时间字段，避免覆盖其他状态的历史时间。
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
