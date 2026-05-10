using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 计价冲正日志仓储，负责写入 PR_CHARGE_REVERSE_LOG。
/// </summary>
/// <remarks>
/// 冲正日志记录撤销已提交业务的原因、来源和关联请求。它不替代请求日志状态，
/// 而是作为反向业务动作的独立审计流水存在。
/// </remarks>
public sealed class ChargeReverseLogRepository : IChargeReverseLogRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列和冲正日志表。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化冲正日志仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public ChargeReverseLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按原请求读取已经发生的冲正流水。
    /// </summary>
    /// <param name="originalRequestId">原确认请求主键。</param>
    /// <returns>该原请求下的冲正流水集合。</returns>
    public async Task<IReadOnlyList<ChargeReverseLog>> GetByOriginalRequestIdAsync(long originalRequestId)
    {
        return await _db.Queryable<ChargeReverseLog>()
            .Where(r => r.OriginalRequestId == originalRequestId)
            .ToListAsync();
    }

    /// <summary>
    /// 插入冲正日志。
    /// </summary>
    /// <param name="entity">待写入的冲正日志实体。</param>
    /// <returns>Oracle 序列生成的冲正日志主键。</returns>
    public async Task<long> InsertAsync(ChargeReverseLog entity)
    {
        // 冲正日志独立取号，便于后续按 ReverseId 对账或定位人工冲正记录。
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_REVERSE.NEXTVAL FROM DUAL");
        entity.ReverseId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}
