namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// 规则版本（PR_RULE_VERSION）的状态枚举，描述单个规则版本在其生命周期中的当前阶段。
///
/// 状态机流转路径：
///   Draft（草稿）→ Published（已发布）→ Disabled（已停用）
///        ↘ RolledBack（已回滚）
///                               ↘ RolledBack（已回滚，从已发布回滚）
///
/// 业务说明：
/// - 一个规则（PR_RULE_HEADER）可同时拥有多个版本，但同一时刻只有一个 Published 版本参与匹配。
/// - 版本回滚（RolledBack）用于线上紧急修复：回滚到上一个稳定版本，当前版本标记为已回滚。
/// - 版本状态变更必须记录 PR_RULE_CHANGE_LOG，并触发缓存失效。
/// - 版本中的条件（PR_RULE_CONDITION）和动作（PR_RULE_ACTION）按 ruleId + versionNo 关联。
///
/// 与 RuleStatus 的关系：
/// - RuleStatus 控制规则整体是否参与匹配。
/// - VersionStatus 控制单个版本是否为当前生效版本。
/// - 规则 Published + 版本 Published = 该版本参与计价引擎匹配。
/// </summary>
public enum VersionStatus
{
    /// <summary>
    /// 草稿：版本正在编辑中，不参与计价引擎匹配。
    /// 支持修改条件和动作，不影响已发布版本的计价行为。
    /// </summary>
    Draft,

    /// <summary>
    /// 已发布：版本正式生效，参与计价引擎匹配。
    /// 发布时如果存在其他 Published 版本，旧版本自动变为 Disabled 或 RolledBack。
    /// </summary>
    Published,

    /// <summary>
    /// 已回滚：版本被回滚，不再参与计价引擎匹配。
    /// 回滚操作记录变更日志并触发缓存失效，用于线上紧急版本切换。
    /// </summary>
    RolledBack,

    /// <summary>
    /// 已停用：版本被停用，不再参与计价引擎匹配。
    /// 停用不同于回滚——停用是正常的生命周期结束，回滚是紧急修复操作。
    /// </summary>
    Disabled
}
