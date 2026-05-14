using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则条件评估器接口 —— 条件-动作分离模式中"条件"侧的核心抽象。
///
/// 架构位置：
///   位于领域层（Domain），由计价引擎在规则匹配阶段调用。
///   每种条件类型对应一个独立的评估器实现，通过 ConditionType 属性注册到引擎。
///
/// 设计模式：
///   执行器模式（Strategy Pattern）—— 新增条件类型只需新增一个 IRuleConditionEvaluator 实现类，
///   无需修改计价引擎的核心匹配逻辑。引擎在运行时通过 ConditionType 查找对应的评估器。
///
/// 职责边界：
///   - 判断给定的规则条件（RuleCondition）在当前计价上下文（PricingContext）下是否满足。
///   - 不负责条件组合逻辑（AND/OR），组合逻辑由计价引擎的规则匹配器负责。
///   - 不负责动作执行，动作由 IRuleActionExecutor 负责。
///
/// 条件类型示例：
///   - 项目编码匹配（ITEM_CODE）
///   - 收费场景匹配（CHARGE_SCENE）
///   - 部位匹配（BODY_PART）
///   - 时间范围匹配（TIME_RANGE）
///   - 患者类型匹配（PATIENT_TYPE）
/// </summary>
public interface IRuleConditionEvaluator
{
    /// <summary>
    /// 条件类型标识符，用于引擎在运行时查找对应的评估器。
    /// 必须与 PR_RULE_CONDITION.CONDITION_TYPE 字段的值一一对应。
    /// 例如："ITEM_CODE"、"CHARGE_SCENE"、"BODY_PART"、"TIME_RANGE"。
    /// </summary>
    string ConditionType { get; }

    /// <summary>
    /// 异步评估指定条件在当前计价上下文中是否满足。
    ///
    /// 调用时机：计价引擎遍历规则的条件列表时，逐条调用此方法判断是否满足。
    /// 全部条件满足（AND 逻辑）时，该规则进入动作执行阶段。
    /// </summary>
    /// <param name="condition">
    /// 规则条件实体，包含：
    /// - ConditionType：条件类型（与本评估器的 ConditionType 属性匹配）
    /// - Operator：比较操作符（等于、包含、范围等）
    /// - ConditionValue：条件值（可能是单值、逗号分隔多值、JSON 范围等）
    /// </param>
    /// <param name="context">
    /// 计价上下文，包含当前计价请求的项目编码、场景、部位、时间、患者信息等。
    /// 评估器从中提取对应字段与条件值比较。
    /// </param>
    /// <returns>
    /// true 表示条件满足，false 表示不满足。
    /// 计价引擎对同一规则的所有条件取 AND，全部满足才执行动作。
    /// </returns>
    ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context);
}
