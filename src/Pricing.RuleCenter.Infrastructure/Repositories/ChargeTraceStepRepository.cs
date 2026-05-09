using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 计价执行步骤仓储，负责持久化和读取 PR_CHARGE_TRACE_STEP。
/// </summary>
/// <remarks>
/// 执行步骤是“为什么得到这个价格”的解释链。它保存每个匹配、动作和状态推进阶段的输入输出快照，
/// 查询时必须按 StepNo 排序，才能还原原始执行顺序。
/// </remarks>
public sealed class ChargeTraceStepRepository : IChargeTraceStepRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和追踪步骤表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化执行步骤仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public ChargeTraceStepRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按请求日志主键读取执行步骤。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <returns>按步骤号升序排列的执行步骤集合。</returns>
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
    /// <param name="entities">待插入的步骤实体集合。</param>
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
