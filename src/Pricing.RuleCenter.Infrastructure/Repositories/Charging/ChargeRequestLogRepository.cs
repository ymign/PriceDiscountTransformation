using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Charging;

/// <summary>
/// 计价请求日志仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_CHARGE_REQUEST_LOG 表的全部数据访问操作，包括：按主键查询、按业务键查询、
/// 按指纹查询、插入、更新、分页查询和过期保护占用扫描。
/// </para>
/// <para>
/// 【在计价链路中的位置】
/// 请求日志是计价链路的主审计表。每一次计价请求（simulate / confirm / commit / cancel / reverse）
/// 都会在 PR_CHARGE_REQUEST_LOG 中产生一条记录。该记录同时承担三重职责：
///   1. 幂等判断入口 —— GetByBusinessKeyAsync 和 GetByFingerprintAsync 用于防重复提交；
///   2. 追溯查询起点 —— 追溯页面先按请求日志定位，再关联折扣明细和执行步骤；
///   3. 过期清理依据 —— 后台任务通过 GetPendingExpiredAsync 扫描超时未结算的确认请求。
/// </para>
/// <para>
/// 【状态机边界】
/// 仓储层只负责按稳定键读取和持久化实体，不解释业务状态是否允许流转。
/// 状态机规则（如 CONFIRM_PENDING → CONFIRMED → COMMITTED）由应用服务层维护。
/// </para>
/// <para>
/// 【并发与事务】
/// 本仓储的方法本身不持有数据库锁。幂等判断在应用层调用 GetByBusinessKeyAsync 后，
/// 由应用层根据返回结果决定是复用还是新建。并发额度控制由 LimitOccupyRepository 负责。
/// </para>
/// </remarks>
public sealed class ChargeRequestLogRepository : IChargeRequestLogRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器在请求作用域内创建，一次 HTTP 请求或后台任务中共享同一个客户端上下文。
    /// 该客户端用于访问 Oracle 序列、请求日志表和分页查询能力。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化计价请求日志仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public ChargeRequestLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按请求日志主键读取单条请求记录。
    /// </summary>
    /// <param name="requestId">请求日志主键（由 Oracle 序列 SEQ_PR_CHARGE_REQ_LOG 生成的 NUMBER(18) 值）。</param>
    /// <returns>
    /// 请求日志实体；不存在时返回 <c>null</c>。
    /// 调用方应判断返回值是否为 null，避免空引用。
    /// </returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_CHARGE_REQUEST_LOG WHERE REQUEST_ID = :requestId
    /// </code>
    /// 【使用场景】commit、cancel、reverse 等后续操作需要先按主键定位原始请求日志，
    /// 再根据其 BusinessStatus 判断是否允许状态流转。
    /// </remarks>
    public async Task<ChargeRequest?> GetByIdAsync(long requestId)
    {
        // InSingleAsync 按主键查询单条记录，不存在时返回 null 而非抛异常。
        return await _db.Queryable<ChargeRequest>().InSingleAsync(requestId);
    }

    /// <summary>
    /// 按来源系统、业务请求号和调用类型读取请求日志。
    /// </summary>
    /// <param name="sourceSystem">调用来源系统编码，如 HIS、SELF_SERVICE、WECHAT。</param>
    /// <param name="businessRequestNo">来源系统生成的业务请求号，必须在来源系统内唯一。</param>
    /// <param name="callType">调用类型枚举值：SIMULATE / CONFIRM / COMMIT / CANCEL / REVERSE。</param>
    /// <returns>匹配到的请求日志；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_CHARGE_REQUEST_LOG
    /// WHERE SOURCE_SYSTEM = :sourceSystem
    ///   AND BUSINESS_REQUEST_NO = :businessRequestNo
    ///   AND CALL_TYPE = :callType
    /// </code>
    /// 【幂等策略】这是接口级幂等的第一道防线。同一个外部请求（相同 sourceSystem + businessRequestNo + callType）
    /// 重复进入时，应命中同一条请求日志。应用层根据返回的 BusinessStatus 决定：
    ///   - 已完成 → 直接返回历史响应快照
    ///   - 进行中 → 返回"处理中"提示
    ///   - 不存在 → 正常创建新请求
    /// </remarks>
    public async Task<ChargeRequest?> GetByBusinessKeyAsync(
        string sourceSystem, string businessRequestNo, string callType)
    {
        // 业务键用于接口级幂等：同一个外部请求重复进入时，应命中同一条请求日志。
        return await _db.Queryable<ChargeRequest>()
            .FirstAsync(r =>
                r.SourceSystem == sourceSystem &&
                r.BusinessRequestNo == businessRequestNo &&
                r.CallType == callType);
    }

    /// <summary>
    /// 按请求指纹读取请求日志。
    /// </summary>
    /// <param name="fingerprint">
    /// 由关键业务字段（如项目编码、数量、金额、患者ID等）经哈希计算出的请求指纹字符串。
    /// 指纹生成逻辑由应用服务负责，仓储只做等值匹配。
    /// </param>
    /// <returns>匹配到的请求日志；不存在时返回 <c>null</c>。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_CHARGE_REQUEST_LOG WHERE REQUEST_FINGERPRINT = :fingerprint
    /// </code>
    /// 【兜底幂等策略】当上游渠道无法稳定提供 businessRequestNo（如历史系统对接）时，
    /// 指纹机制通过对请求核心内容哈希来识别重复提交。指纹存储在 REQUEST_FINGERPRINT 列，
    /// 建议对该列建立索引以加速查询。
    /// 【与 GetByBusinessKeyAsync 的关系】应用层通常先尝试业务键幂等，未命中再尝试指纹幂等。
    /// </remarks>
    public async Task<ChargeRequest?> GetByFingerprintAsync(string fingerprint)
    {
        // 指纹用于兜底幂等。当上游无法稳定提供业务请求号时，仍可通过核心请求内容识别重复提交。
        return await _db.Queryable<ChargeRequest>()
            .FirstAsync(r => r.RequestFingerprint == fingerprint);
    }

    /// <summary>
    /// 插入计价请求日志。
    /// </summary>
    /// <param name="entity">待写入的请求日志实体，调用方应在写入前填充全部业务字段。</param>
    /// <returns>Oracle 序列生成的请求日志主键（REQUEST_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// -- 第一步：从 Oracle 序列获取自增主键
    /// SELECT SEQ_PR_CHARGE_REQ_LOG.NEXTVAL FROM DUAL
    /// -- 第二步：插入完整记录
    /// INSERT INTO PR_CHARGE_REQUEST_LOG (...) VALUES (...)
    /// </code>
    /// 【主键生成策略】使用 Oracle 序列而非 SqlSugar 内置自增，保证与既有 Oracle 表结构和脚本保持一致。
    /// 序列值在内存中赋给实体后再执行 INSERT，插入后返回给调用方，便于关联写入折扣明细和执行步骤。
    /// 【事务边界】本方法不开启事务。调用方（通常是应用服务）应在更大的事务中调用本方法，
    /// 以保证请求日志、折扣明细、执行步骤和限额占用的原子写入。
    /// </remarks>
    public async Task<long> InsertAsync(ChargeRequest entity)
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
    /// <param name="entity">
    /// 包含最新业务状态、响应快照和错误信息的请求日志实体。
    /// 调用方应确保 RequestId 已正确设置，SqlSugar 按主键定位更新行。
    /// </param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_CHARGE_REQUEST_LOG
    /// SET BUSINESS_STATUS = ..., RESPONSE_SNAPSHOT = ..., ERROR_MESSAGE = ..., ...
    /// WHERE REQUEST_ID = :entity.RequestId
    /// </code>
    /// 【使用场景】状态流转（如 CONFIRM_PENDING → CONFIRMED）、记录响应结果、记录错误信息。
    /// 【约束】应用服务必须保证状态流转合法后再调用本方法；仓储不校验状态前置条件。
    /// </remarks>
    public async Task UpdateAsync(ChargeRequest entity)
    {
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 分页查询计价请求日志。
    /// </summary>
    /// <param name="patientId">患者标识筛选条件；为 null 或空字符串时不限制。</param>
    /// <param name="itemCode">收费项目编码筛选条件；为 null 或空字符串时不限制。</param>
    /// <param name="chargeNo">HIS 收费单号筛选条件；为 null 或空字符串时不限制。</param>
    /// <param name="startTime">请求时间开始边界（含）；为 null 时不限制。</param>
    /// <param name="endTime">请求时间结束边界（含）；为 null 时不限制。</param>
    /// <param name="pageIndex">页码，从 1 开始。</param>
    /// <param name="pageSize">每页记录数，建议不超过 100。</param>
    /// <returns>元组：(当前页请求日志列表, 符合条件的总记录数)。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于（以全部条件为例）：
    /// <code>
    /// SELECT COUNT(*) FROM PR_CHARGE_REQUEST_LOG
    /// WHERE PATIENT_ID = :patientId AND ITEM_CODE = :itemCode ...
    /// -- 分页
    /// SELECT * FROM (
    ///   SELECT ROWNUM RN, T.* FROM (
    ///     SELECT * FROM PR_CHARGE_REQUEST_LOG
    ///     WHERE PATIENT_ID = :patientId AND ITEM_CODE = :itemCode ...
    ///     ORDER BY REQUEST_AT DESC
    ///   ) T WHERE ROWNUM &lt;= :endRow
    /// ) WHERE RN &gt; :startRow
    /// </code>
    /// 【分页策略】只分页主请求表，明细由查询服务按 RequestId 再读取，
    /// 避免明细 JOIN 导致行数膨胀和分页不准确。
    /// 【动态条件】使用 WhereIF 避免手写多套 SQL 分支。空值条件不会加入 WHERE 子句。
    /// </remarks>
    public async Task<(IReadOnlyList<ChargeRequest> Items, int Total)> GetPagedAsync(
        string? patientId, string? itemCode, string? chargeNo,
        DateTime? startTime, DateTime? endTime,
        int pageIndex, int pageSize)
    {
        // ========== 第一阶段：构造动态条件 ==========
        // 追踪页面的筛选条件都是可选项，使用 WhereIF 能避免手写多套 SQL 分支。
        var total = new RefAsync<int>();
        var items = await _db.Queryable<ChargeRequest>()
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
    /// <param name="expireBefore">
    /// 过期判断边界时间。请求时间（REQUEST_AT）早于该值且业务状态仍为 CONFIRM_PENDING 的记录会被返回。
    /// 通常由后台任务按"当前时间 - 保护窗口时长"计算得出。
    /// </param>
    /// <returns>待过期清理的确认请求集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_CHARGE_REQUEST_LOG
    /// WHERE CALL_TYPE = 'CONFIRM'
    ///   AND BUSINESS_STATUS = 'CONFIRM_PENDING'
    ///   AND REQUEST_AT &lt; :expireBefore
    /// </code>
    /// 【业务背景】confirm 接口会先写入 PENDING 状态的限额占用。如果 HIS 长时间不调用 commit 或 cancel，
    /// 占用会一直锁住额度。后台任务定期扫描超时请求，自动过期并释放占用。
    /// 【清理范围】只清理 CONFIRM 阶段留下的保护占用：
    ///   - SIMULATE 不写保护占用，无需清理；
    ///   - COMMIT / CANCEL / REVERSE 已经有明确后续状态，不会卡在 PENDING。
    /// 【事务要求】调用方在处理每条过期记录时，应同时更新请求日志状态、限额占用状态和折价明细状态，
    /// 保证三张表状态一致。
    /// </remarks>
    public async Task<IReadOnlyList<ChargeRequest>> GetPendingExpiredAsync(DateTime expireBefore)
    {
        // 只清理 CONFIRM 阶段留下的保护占用。SIMULATE 不写保护占用，COMMIT/CANCEL/REVERSE 已经有明确后续状态。
        return await _db.Queryable<ChargeRequest>()
            .Where(r =>
                r.CallType == "CONFIRM" &&
                r.BusinessStatus == "CONFIRM_PENDING" &&
                r.RequestAt < expireBefore)
            .ToListAsync();
    }
}