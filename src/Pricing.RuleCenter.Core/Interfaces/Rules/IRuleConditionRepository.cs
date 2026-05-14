using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Interfaces.Rules;

/// <summary>
/// 规则条件仓储接口 —— 规则维护中心的条件配置数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_CONDITION。
///
/// 职责边界：
///   - 管理规则版本关联的条件配置（PR_RULE_CONDITION）。
///   - 条件是条件-动作分离模式中的"条件"侧，由 IRuleConditionEvaluator 评估。
///   - 每个规则版本可关联多个条件，全部条件满足（AND 逻辑）时才执行动作。
///
/// 数据模型：
///   - ruleId + versionNo 关联到具体的规则版本（PR_RULE_VERSION）。
///   - CONDITION_TYPE 对应 IRuleConditionEvaluator.ConditionType，引擎通过此类型查找评估器。
///   - OPERATOR 定义比较操作符（等于、包含、范围等）。
///   - CONDITION_VALUE 存储条件值（可能是单值、逗号分隔多值、JSON 范围等）。
///
/// 版本快照机制：
///   规则发布时，当前版本的条件配置作为快照固化，后续修改不影响已发布版本。
/// </summary>
public interface IRuleConditionRepository
{
    /// <summary>
    /// 查询指定规则版本的所有条件配置。
    ///
    /// 使用场景：
    /// - 计价引擎加载规则时，获取该版本的全部条件列表用于匹配。
    /// - 工作台编辑规则时，展示当前版本的条件配置。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.ID）。</param>
    /// <param name="versionNo">版本号（PR_RULE_VERSION.VERSION_NO）。</param>
    /// <returns>条件配置列表。</returns>
    Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo);

    /// <summary>
    /// 批量插入条件配置记录。
    ///
    /// 使用场景：规则版本保存/发布时，先删除旧条件，再批量插入新条件。
    /// </summary>
    /// <param name="entities">条件配置实体列表。</param>
    Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities);

    /// <summary>
    /// 删除指定规则版本的所有条件配置。
    ///
    /// 使用场景：规则版本编辑时，先清除旧条件再重新插入（先删后插模式）。
    /// </summary>
    /// <param name="ruleId">规则主表ID。</param>
    /// <param name="versionNo">版本号。</param>
    Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo);
}
