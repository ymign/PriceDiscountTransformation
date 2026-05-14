using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则审核仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_APPROVAL 表的查询和写入操作，包括：
/// 按规则查询审核历史、查询待审核列表、插入审核申请、更新审核状态。
/// </para>
/// <para>
/// 【审核流程模型】
/// 审核表记录规则发布、停用、回滚等操作的审批流程。
/// 每次规则状态变更前需要提交审核申请，审核通过后方可执行实际操作。
/// 审核状态流转：PENDING → APPROVED / REJECTED。
/// </para>
/// <para>
/// 【与发布流水的关系】
/// PR_RULE_APPROVAL 记录审核流程，是操作前的审批环节。
/// PR_RULE_PUBLISH 记录已执行的发布操作，是操作后的审计痕迹。
/// 两者在时间线上先后出现：先审核、后发布。
/// </para>
/// <para>
/// 【事务要求】
/// InsertAsync 不开启事务。调用方应在更大的事务中调用，
/// 保证审核记录与规则状态的原子更新。
/// UpdateStatusAsync 同样不开启事务，由调用方保证事务边界。
/// </para>
/// </remarks>
public sealed class RuleApprovalRepository : IRuleApprovalRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和审核表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则审核仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleApprovalRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按规则主键读取审核记录，按提交时间倒序排列。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <returns>按提交时间倒序排列的审核记录集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_APPROVAL
    /// WHERE RULE_ID = :ruleId
    /// ORDER BY SUBMITTED_AT DESC
    /// </code>
    /// 【排序策略】倒序排列使最近一次审核操作出现在列表最前面，便于查看最新审核状态。
    /// 【使用场景】规则配置页面的审核历史列表、审计追溯。
    /// </remarks>
    public async Task<IReadOnlyList<RuleApproval>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleApproval>()
            .Where(a => a.RuleId == ruleId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 查询所有待审核的记录，按提交时间升序排列。
    /// </summary>
    /// <returns>待审核记录列表；无待审核记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_APPROVAL
    /// WHERE APPROVAL_STATUS = 'PENDING'
    /// ORDER BY SUBMITTED_AT ASC
    /// </code>
    /// 【排序策略】升序排列使先提交的审核申请排在前面，保证公平处理。
    /// 【使用场景】审核工作台的待审核列表。
    /// </remarks>
    public async Task<IReadOnlyList<RuleApproval>> GetPendingAsync()
    {
        return await _db.Queryable<RuleApproval>()
            .Where(a => a.ApprovalStatus == "PENDING")
            .OrderBy(a => a.SubmittedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 插入审核申请记录。
    /// </summary>
    /// <param name="entity">待写入的审核记录实体。</param>
    /// <returns>Oracle 序列生成的审核主键（APPROVAL_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_APPROVAL.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_APPROVAL (...) VALUES (...)
    /// </code>
    /// 【事务边界】本方法不开启事务。调用方（审核服务）应在更大的事务中调用本方法，
    /// 保证审核记录与规则状态的原子更新。
    /// </remarks>
    public async Task<long> InsertAsync(RuleApproval entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_APPROVAL.NEXTVAL FROM DUAL");
        entity.ApprovalId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 更新审核记录的状态，记录审核人、审核时间和审核意见。
    /// </summary>
    /// <param name="approvalId">审核记录主键（PR_RULE_APPROVAL.APPROVAL_ID）。</param>
    /// <param name="status">目标审核状态（APPROVED / REJECTED）。</param>
    /// <param name="reviewedBy">审核人。</param>
    /// <param name="reviewComment">审核意见。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// UPDATE PR_RULE_APPROVAL
    /// SET APPROVAL_STATUS = :status,
    ///     REVIEWED_BY = :reviewedBy,
    ///     REVIEWED_AT = SYSDATE,
    ///     REVIEW_COMMENT = :reviewComment
    /// WHERE APPROVAL_ID = :approvalId
    /// </code>
    /// 【使用场景】审核人员执行审核操作（通过或驳回）时调用。
    /// 【约束】状态合法性由应用服务保证，仓储只执行指定更新。
    /// </remarks>
    public async Task UpdateStatusAsync(long approvalId, string status, string reviewedBy, string reviewComment)
    {
        await _db.Updateable<RuleApproval>()
            .SetColumns(a => a.ApprovalStatus == status)
            .SetColumns(a => a.ReviewedBy == reviewedBy)
            .SetColumns(a => a.ReviewedAt == DateTime.Now)
            .SetColumns(a => a.ReviewComment == reviewComment)
            .Where(a => a.ApprovalId == approvalId)
            .ExecuteCommandAsync();
    }
}