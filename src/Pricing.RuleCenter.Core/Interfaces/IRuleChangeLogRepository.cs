using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则变更日志仓储接口 —— 折价追溯中心中"规则变更链"的数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_CHANGE_LOG。
///
/// 职责边界：
///   - 记录规则的所有变更历史（创建、修改、发布、停用、回滚等）。
///   - 支持追溯查询接口展示规则的变更链，便于审计和问题排查。
///
/// 追溯链路：
///   计价请求有三条追溯链路：
///   1. 规则变更链：PR_RULE_CHANGE_LOG —— 规则何时被谁修改（本接口管理）
///   2. 计算过程链：PR_CHARGE_TRACE_STEP —— 本次计价经历了哪些步骤
///   3. 最终结果链：PR_CHARGE_DISCOUNT_DETAIL —— 最终折价结果明细
///
/// 写入时机：
///   - 规则创建、修改、发布、停用、回滚时写入。
///   - 每条变更日志记录变更类型、变更内容、操作人、操作时间等信息。
///   - 变更内容以 JSON/CLOB 格式存储变更前后的差异。
/// </summary>
public interface IRuleChangeLogRepository
{
    /// <summary>
    /// 查询指定规则的所有变更日志。
    ///
    /// 使用场景：追溯查询接口展示规则的完整变更历史，或工作台展示规则的审计日志。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.ID）。</param>
    /// <returns>变更日志列表，按变更时间倒序排列（最新的变更在前）。</returns>
    Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId);

    /// <summary>
    /// 插入一条变更日志记录，返回自增主键。
    ///
    /// 调用时机：规则的任何变更操作（创建、修改、发布、停用、回滚）完成后写入。
    /// </summary>
    /// <param name="entity">
    /// 变更日志实体，包含：
    /// - RuleId：关联的规则ID
    /// - ChangeType：变更类型（CREATE/MODIFY/PUBLISH/DISABLE/ROLLBACK）
    /// - ChangeContent：变更内容详情（JSON/CLOB）
    /// - Operator：操作人
    /// - OperateTime：操作时间
    /// </param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RuleChangeLog entity);
}
