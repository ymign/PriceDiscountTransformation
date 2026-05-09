using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

/// <summary>
/// 计价请求日志仓储，封装 PR_CHARGE_REQUEST_LOG 的查询、写入和分页检索。
/// </summary>
/// <remarks>
/// 请求日志是计价链路的主审计表，也是幂等判断、追踪查询和过期清理的入口。仓储层只负责按稳定键读取
/// 和持久化实体，不解释业务状态是否允许流转，状态机规则由应用服务维护。
/// </remarks>
public sealed class ChargeRequestLogRepository : IChargeRequestLogRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于访问 Oracle 序列、请求日志表和分页查询能力。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化计价请求日志仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public ChargeRequestLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按请求日志主键读取单条请求记录。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <returns>请求日志实体；不存在时返回 <c>null</c>。</returns>
    public async Task<ChargeRequestLog?> GetByIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeRequestLog>().InSingleAsync(requestId);
    }

    /// <summary>
    /// 按来源系统、业务请求号和调用类型读取请求日志。
    /// </summary>
    /// <param name="sourceSystem">调用来源系统编码。</param>
    /// <param name="businessRequestNo">来源系统生成的业务请求号。</param>
    /// <param name="callType">调用类型，例如 SIMULATE、CONFIRM、COMMIT、CANCEL 或 REVERSE。</param>
    /// <returns>匹配到的请求日志；不存在时返回 <c>null</c>。</returns>
    public async Task<ChargeRequestLog?> GetByBusinessKeyAsync(
        string sourceSystem, string businessRequestNo, string callType)
    {
        // 业务键用于接口级幂等：同一个外部请求重复进入时，应命中同一条请求日志。
        return await _db.Queryable<ChargeRequestLog>()
            .FirstAsync(r =>
                r.SourceSystem == sourceSystem &&
                r.BusinessRequestNo == businessRequestNo &&
                r.CallType == callType);
    }

    /// <summary>
    /// 按请求指纹读取请求日志。
    /// </summary>
    /// <param name="fingerprint">由关键业务字段计算出的请求指纹。</param>
    /// <returns>匹配到的请求日志；不存在时返回 <c>null</c>。</returns>
    public async Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint)
    {
        // 指纹用于兜底幂等。当上游无法稳定提供业务请求号时，仍可通过核心请求内容识别重复提交。
        return await _db.Queryable<ChargeRequestLog>()
            .FirstAsync(r => r.RequestFingerprint == fingerprint);
    }

    /// <summary>
    /// 插入计价请求日志。
    /// </summary>
    /// <param name="entity">待写入的请求日志实体。</param>
    /// <returns>Oracle 序列生成的请求日志主键。</returns>
    public async Task<long> InsertAsync(ChargeRequestLog entity)
    {
        // 使用数据库序列生成主键，保证与既有 Oracle 表结构和脚本保持一致。
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_REQ_LOG.NEXTVAL FROM DUAL");
        entity.RequestId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新计价请求日志。
    /// </summary>
    /// <param name="entity">包含最新业务状态、响应快照和错误信息的请求日志实体。</param>
    public async Task UpdateAsync(ChargeRequestLog entity)
    {
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 分页查询计价请求日志。
    /// </summary>
    /// <param name="patientId">患者标识筛选条件；为空时不限制。</param>
    /// <param name="itemCode">收费项目编码筛选条件；为空时不限制。</param>
    /// <param name="chargeNo">HIS 收费单号筛选条件；为空时不限制。</param>
    /// <param name="startTime">请求时间开始边界；为空时不限制。</param>
    /// <param name="endTime">请求时间结束边界；为空时不限制。</param>
    /// <param name="pageIndex">页码，从 1 开始。</param>
    /// <param name="pageSize">每页记录数。</param>
    /// <returns>当前页请求日志以及符合条件的总记录数。</returns>
    public async Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
        string? patientId, string? itemCode, string? chargeNo,
        DateTime? startTime, DateTime? endTime,
        int pageIndex, int pageSize)
    {
        // ========== 第一阶段：构造动态条件 ==========
        // 追踪页面的筛选条件都是可选项，使用 WhereIF 能避免手写多套 SQL 分支。
        var total = new RefAsync<int>();
        var items = await _db.Queryable<ChargeRequestLog>()
            .WhereIF(!string.IsNullOrEmpty(patientId), r => r.PatientId == patientId)
            .WhereIF(!string.IsNullOrEmpty(itemCode), r => r.ItemCode == itemCode)
            .WhereIF(!string.IsNullOrEmpty(chargeNo), r => r.ChargeNo == chargeNo)
            .WhereIF(startTime.HasValue, r => r.RequestAt >= startTime!.Value)
            .WhereIF(endTime.HasValue, r => r.RequestAt <= endTime!.Value)
            .OrderByDescending(r => r.RequestAt)
            // ========== 第二阶段：数据库分页 ==========
            // 只分页主请求表，明细由查询服务按 RequestId 再读取，避免明细 join 放大行数。
            .ToPageListAsync(pageIndex, pageSize, total);
        return (items, total.Value);
    }

    /// <summary>
    /// 查询超过保护期仍未提交或取消的确认请求。
    /// </summary>
    /// <param name="expireBefore">过期判断边界，请求时间早于该值且仍为 CONFIRM_PENDING 才会返回。</param>
    /// <returns>待过期清理的确认请求集合。</returns>
    public async Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore)
    {
        // 只清理 CONFIRM 阶段留下的保护占用。SIMULATE 不写保护占用，COMMIT/CANCEL/REVERSE 已经有明确后续状态。
        return await _db.Queryable<ChargeRequestLog>()
            .Where(r =>
                r.CallType == "CONFIRM" &&
                r.BusinessStatus == "CONFIRM_PENDING" &&
                r.RequestAt < expireBefore)
            .ToListAsync();
    }
}
