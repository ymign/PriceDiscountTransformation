using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Charging;

/// <summary>
/// 计价执行步骤仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_CHARGE_TRACE_STEP 表的全部数据访问操作，包括：按请求ID查询和批量插入。
/// </para>
/// <para>
/// 【表结构语义】
/// 执行步骤是"为什么得到这个价格"的解释链。它保存计价引擎每个阶段的输入输出快照：
///   - 规则匹配阶段：哪些条件命中了
///   - 公式计算阶段：公式参数和计算结果
///   - 限额检查阶段：累计数量和剩余空间
///   - 状态推进阶段：confirm/commit/cancel 的状态变化
/// 每条记录包含 STEP_NO（步骤序号）用于还原原始执行顺序。
/// </para>
/// <para>
/// 【追溯链路】
/// 计价结果必须可沿三条链路追溯：
///   1. 规则变更链 —— 由 PR_RULE_CHANGE_LOG 和 PR_RULE_PUBLISH 承载
///   2. 计算过程链 —— 由本表 PR_CHARGE_TRACE_STEP 承载
///   3. 最终结果链 —— 由 PR_CHARGE_DISCOUNT_DETAIL 承载
/// 本仓储负责第二条链路的数据持久化。
/// </para>
/// <para>
/// 【事务要求】
/// 批量插入必须与请求日志在同一事务中写入，确保失败时一起回滚。
/// 追踪数据不参与后续业务状态判断，但如果请求日志写入成功而步骤丢失，
/// 追溯页面将无法解释计价结果。
/// </para>
/// </remarks>
public sealed class ChargeTraceStepRepository : IChargeTraceStepRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和追踪步骤表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化执行步骤仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public ChargeTraceStepRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按请求日志主键读取执行步骤，按步骤号升序排列。
    /// </summary>
    /// <param name="requestId">请求日志主键（PR_CHARGE_REQUEST_LOG.REQUEST_ID）。</param>
    /// <returns>按 STEP_NO 升序排列的执行步骤集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_CHARGE_TRACE_STEP
    /// WHERE REQUEST_ID = :requestId
    /// ORDER BY STEP_NO ASC
    /// </code>
    /// 【排序要求】查询时必须按 STEP_NO 排序，才能还原原始执行顺序。
    /// 追溯页面需要按顺序展示每一步的输入输出，乱序会导致解释链断裂。
    /// 【使用场景】追溯查询接口（POST /api/pricing/trace/query）调用本方法获取计算过程链。
    /// </remarks>
    public async Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeTraceStep>()
            .Where(s => s.RequestId == requestId)
            .OrderBy(s => s.StepNo)
            .ToListAsync();
    }

    /// <summary>
    /// 批量插入执行步骤。
    /// </summary>
    /// <param name="entities">待插入的步骤实体集合，调用方应已按 STEP_NO 排好序。</param>
    /// <remarks>
    /// 【SQL 语义】分两个阶段执行：
    /// <code>
    /// -- 第一阶段：逐条获取序列主键
    /// SELECT SEQ_PR_CHARGE_TRACE_STEP.NEXTVAL FROM DUAL  -- 对每条记录执行一次
    /// -- 第二阶段：批量插入
    /// INSERT ALL
    ///   INTO PR_CHARGE_TRACE_STEP (...) VALUES (...)
    ///   INTO PR_CHARGE_TRACE_STEP (...) VALUES (...)
    ///   ...
    /// SELECT 1 FROM DUAL
    /// </code>
    /// 【主键生成】逐条分配 Oracle 序列，步骤通常在内存中已按 STEP_NO 排好序，
    /// 这里仅补数据库主键（STEP_ID），不改变业务顺序。
    /// 【性能考虑】序列分配是逐条执行的（Oracle 序列不支持批量获取），
    /// 但追踪步骤通常数量有限（每条请求 5-20 步），不会成为瓶颈。
    /// 【事务边界】批量写入必须在调用方事务中执行，保证与请求日志原子一致。
    /// </remarks>
    public async Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities)
    {
        // ========== 第一阶段：逐条分配 Oracle 序列 ==========
        // 步骤通常在内存中已经按 StepNo 排好序；这里仅补数据库主键，不改变业务顺序。
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_TRACE_STEP.NEXTVAL FROM DUAL");
            entity.StepId = seq;
        }

        // ========== 第二阶段：批量写入 ==========
        // 追踪数据不参与后续业务状态判断，但必须和请求日志在同一事务里写入，确保失败时一起回滚。
        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }
}