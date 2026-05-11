using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则审核仓储接口 —— 规则维护中心的审核流程数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_APPROVAL。
///
/// 职责边界：
///   - 管理规则发布、停用、回滚等操作的审核申请。
///   - 支持按规则查询审核历史、按状态查询待审核列表。
///   - 支持审核状态流转（通过/驳回）。
///
/// 审核流程说明：
///   1. 规则维护人员提交审核申请（INSERT，状态 = PENDING）
///   2. 审核人员查看待审核列表（SELECT，状态 = PENDING）
///   3. 审核人员执行审核操作（UPDATE，状态 = APPROVED / REJECTED）
///   4. 审核通过后，发布服务方可执行实际的发布/停用/回滚操作
///
/// 与发布流水（PR_RULE_PUBLISH）的关系：
///   - PR_RULE_APPROVAL 记录审核流程，是操作前的审批环节。
///   - PR_RULE_PUBLISH 记录已执行的发布操作，是操作后的审计痕迹。
///   - 两者在时间线上先后出现：先审核、后发布。
/// </summary>
public interface IRuleApprovalRepository
{
    /// <summary>
    /// 查询指定规则的所有审核记录。
    ///
    /// 使用场景：规则详情页展示审核历史，或审计追溯。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.RULE_ID）。</param>
    /// <returns>审核记录列表，按提交时间倒序排列（最新的在前）。</returns>
    Task<IReadOnlyList<RuleApproval>> GetByRuleIdAsync(long ruleId);

    /// <summary>
    /// 查询所有待审核的记录。
    ///
    /// 使用场景：审核工作台展示待审核列表，供审核人员处理。
    /// </summary>
    /// <returns>待审核记录列表，按提交时间升序排列（先提交的先审核）。</returns>
    Task<IReadOnlyList<RuleApproval>> GetPendingAsync();

    /// <summary>
    /// 插入一条审核申请记录，返回自增主键。
    ///
    /// 调用时机：规则维护人员提交审核申请时写入，初始状态为 PENDING。
    /// </summary>
    /// <param name="entity">
    /// 审核记录实体，包含：
    /// - RuleId：关联的规则ID
    /// - VersionNo：审核针对的版本号（可选）
    /// - ActionType：操作类型（PUBLISH/DISABLE/ROLLBACK）
    /// - SubmittedBy：提交人
    /// - SubmittedAt：提交时间
    /// </param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RuleApproval entity);

    /// <summary>
    /// 更新审核记录的状态，记录审核人、审核时间和审核意见。
    ///
    /// 调用时机：审核人员执行审核操作（通过或驳回）时调用。
    /// </summary>
    /// <param name="approvalId">审核记录主键（PR_RULE_APPROVAL.APPROVAL_ID）。</param>
    /// <param name="status">目标审核状态（APPROVED / REJECTED）。</param>
    /// <param name="reviewedBy">审核人。</param>
    /// <param name="reviewComment">审核意见（通过时可选，驳回时应填写驳回理由）。</param>
    Task UpdateStatusAsync(long approvalId, string status, string reviewedBy, string reviewComment);
}
