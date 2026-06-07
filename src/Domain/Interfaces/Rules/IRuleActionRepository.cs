using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Interfaces.Rules;

/// <summary>
/// 规则动作仓储接口 —— 规则维护中心的动作配置数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_ACTION。
///
/// 职责边界：
///   - 管理规则版本关联的动作配置（PR_RULE_ACTION）。
///   - 动作是条件-动作分离模式中的"动作"侧，由 IRuleActionExecutor 执行。
///   - 每个规则版本可关联多个动作，按 SORT_NO 排序依次执行。
///
/// 数据模型：
///   - ruleId + versionNo 关联到具体的规则版本（PR_RULE_VERSION）。
///   - ACTION_TYPE 对应 IRuleActionExecutor.ActionType，引擎通过此类型查找执行器。
///   - ACTION_VALUE 存储动作参数（如公式编码、金额上限值等）。
///   - ACTION_CONFIG 存储复杂配置（JSON/CLOB 格式，如换算系数、互斥组编码等）。
///
/// 版本快照机制：
///   规则发布时，当前版本的动作配置作为快固化，后续修改公式定义不影响已发布版本。
/// </summary>
public interface IRuleActionRepository
{
    /// <summary>
    /// 查询指定规则版本的所有动作配置。
    ///
    /// 使用场景：
    /// - 计价引擎加载规则时，获取该版本的全部动作列表。
    /// - 工作台编辑规则时，展示当前版本的动作配置。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.ID）。</param>
    /// <param name="versionNo">版本号（PR_RULE_VERSION.VERSION_NO）。</param>
    /// <returns>动作配置列表，按 SORT_NO 排序。</returns>
    Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo);

    /// <summary>
    /// 批量查询多个规则版本的动作配置。
    ///
    /// 使用场景：
    /// - 生效规则快照批量加载
    /// - 发布冲突检测批量构建规则 profile
    ///
    /// 默认实现退回逐条调用 <see cref="GetByRuleAndVersionAsync(long, int)"/>，
    /// 以兼容旧测试桩；生产仓储应覆盖为真正的批量数据库查询。
    /// </summary>
    /// <param name="ruleVersions">规则ID与版本号键集合。</param>
    /// <returns>以 (RuleId, VersionNo) 为键的动作集合字典。</returns>
    async Task<IReadOnlyDictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>> GetByRuleVersionsAsync(
        IReadOnlyCollection<(long RuleId, int VersionNo)> ruleVersions)
    {
        var result = new Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>();
        foreach (var ruleVersion in ruleVersions)
        {
            if (result.ContainsKey(ruleVersion))
            {
                continue;
            }

            result[ruleVersion] = await GetByRuleAndVersionAsync(ruleVersion.RuleId, ruleVersion.VersionNo);
        }

        return result;
    }

    /// <summary>
    /// 批量插入动作配置记录。
    ///
    /// 使用场景：规则版本保存/发布时，先删除旧动作，再批量插入新动作。
    /// </summary>
    /// <param name="entities">动作配置实体列表。</param>
    Task InsertBatchAsync(IReadOnlyList<RuleAction> entities);

    /// <summary>
    /// 删除指定规则版本的所有动作配置。
    ///
    /// 使用场景：规则版本编辑时，先清除旧动作再重新插入（先删后插模式）。
    /// </summary>
    /// <param name="ruleId">规则主表ID。</param>
    /// <param name="versionNo">版本号。</param>
    Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo);
}
