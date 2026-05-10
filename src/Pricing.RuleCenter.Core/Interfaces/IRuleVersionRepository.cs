using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则版本仓储接口 —— 规则维护中心的版本管理数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_VERSION。
///
/// 职责边界：
///   - 管理规则的版本信息（版本号、状态、生效时间、失效时间等）。
///   - 一个规则（PR_RULE_HEADER）可拥有多个版本，同一时刻只有一个 Published 版本参与匹配。
///   - 版本是条件（PR_RULE_CONDITION）和动作（PR_RULE_ACTION）的归属单元。
///
/// 版本快照机制：
///   规则发布时，当前版本的条件和动作配置作为快照固化。
///   后续修改公式定义或字典数据不影响已发布版本的计价行为。
///   新的修改需要创建新版本，发布时旧版本自动变为 Disabled 或 RolledBack。
///
/// 版本状态流转（VersionStatus）：
///   Draft → Published → Disabled（正常生命周期）
///   Draft → Published → RolledBack（紧急回滚）
///   Draft → RolledBack（草稿阶段回滚）
///
/// 数据约束：
///   - 同一规则下 versionNo 唯一。
///   - 同一规则下最多只有一个 Published 状态的版本。
/// </summary>
public interface IRuleVersionRepository
{
    /// <summary>
    /// 根据主键ID查询规则版本。
    /// </summary>
    /// <param name="versionId">规则版本主键ID（PR_RULE_VERSION.ID）。</param>
    /// <returns>规则版本实体，不存在时返回 null。</returns>
    Task<RuleVersion?> GetByIdAsync(long versionId);

    /// <summary>
    /// 根据规则ID和版本号查询规则版本。
    ///
    /// 使用场景：计价引擎在匹配到规则后，加载指定版本的条件和动作配置。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.ID）。</param>
    /// <param name="versionNo">版本号。</param>
    /// <returns>规则版本实体，不存在时返回 null。</returns>
    Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo);

    /// <summary>
    /// 查询指定规则的所有版本。
    ///
    /// 使用场景：工作台展示规则的版本历史列表，支持版本切换和回滚操作。
    /// </summary>
    /// <param name="ruleId">规则主表ID。</param>
    /// <returns>版本列表，按版本号排序。</returns>
    Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId);

    /// <summary>
    /// 插入一条规则版本记录，返回自增主键。
    ///
    /// 使用场景：规则新建时自动创建第一个版本（Draft 状态），或在已有规则上创建新版本。
    /// </summary>
    /// <param name="entity">规则版本实体，包含规则ID、版本号、状态、生效时间、失效时间等。</param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RuleVersion entity);

    /// <summary>
    /// 更新规则版本的状态。
    ///
    /// 使用场景：
    /// - 发布：Draft → Published（同时将旧 Published 版本置为 Disabled）。
    /// - 停用：Published → Disabled。
    /// - 回滚：Published → RolledBack（同时激活目标版本）。
    /// 状态变更必须记录 PR_RULE_CHANGE_LOG，并触发缓存失效。
    /// </summary>
    /// <param name="versionId">规则版本主键ID。</param>
    /// <param name="status">目标状态（VersionStatus 的字符串表示）。</param>
    /// <returns>是否更新成功。</returns>
    Task<bool> UpdateStatusAsync(long versionId, string status);
}
