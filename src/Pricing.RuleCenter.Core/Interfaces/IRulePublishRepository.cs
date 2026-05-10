using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则发布记录仓储接口 —— 规则维护中心的发布审计数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_PUBLISH。
///
/// 职责边界：
///   - 记录规则的每次发布操作（发布人、发布时间、发布版本号等）。
///   - 支持追溯查询接口展示规则的发布历史。
///   - 发布记录是审计追溯的关键数据，不可物理删除。
///
/// 与规则变更日志（PR_RULE_CHANGE_LOG）的关系：
///   - PR_RULE_CHANGE_LOG 记录所有类型的变更（创建、修改、发布、停用、回滚）。
///   - PR_RULE_PUBLISH 仅记录发布操作，包含发布前的校验结果和发布的版本快照信息。
///   - 两者互补：变更日志提供全景视图，发布记录提供发布专项审计。
///
/// 发布前置校验（由应用层调用，非本接口职责）：
///   - 规则重叠校验：同项目、同场景、同生效期不允许多套冲突规则。
///   - 动作组冲突校验：同一规则版本内不允许重复的动作类型。
///   - 重复子项目校验：子项加收不允许引用重复项目。
///   - 测试用例校验：发布前必须有通过的测试用例。
/// </summary>
public interface IRulePublishRepository
{
    /// <summary>
    /// 查询指定规则的所有发布记录。
    ///
    /// 使用场景：追溯查询接口展示规则的发布历史，或工作台展示规则的审计信息。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.ID）。</param>
    /// <returns>发布记录列表，按发布时间倒序排列（最新的发布在前）。</returns>
    Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId);

    /// <summary>
    /// 插入一条发布记录，返回自增主键。
    ///
    /// 调用时机：规则发布操作完成后写入，记录本次发布的版本号、发布人、发布时间等。
    /// </summary>
    /// <param name="entity">
    /// 发布记录实体，包含：
    /// - RuleId：关联的规则ID
    /// - VersionNo：发布的版本号
    /// - Publisher：发布人
    /// - PublishTime：发布时间
    /// - PreCheckResult：发布前校验结果（JSON/CLOB）
    /// </param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RulePublish entity);
}
