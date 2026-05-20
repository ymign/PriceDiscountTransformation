using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则动作执行器接口 —— 条件-动作分离模式中"动作"侧的核心抽象。
///
/// 架构位置：
///   位于领域层（Domain），由计价引擎在规则匹配成功后的动作执行阶段调用。
///   ActionType 是一级动作大类；一个 ActionType 可以对应一个或多个执行器。
///   例如 FORMULA_CALC 可同时注册 INCREMENT_PERCENT、AREA_STEP_INCREMENT 等多个公式执行器，
///   再由 RuleAction.ExecutorCode 做二级分派。
///
/// 设计模式：
///   执行器模式（Strategy Pattern）—— 新增动作类型或公式类型只需新增一个
///   IRuleActionExecutor 实现类并完成 DI 注册，无需修改计价引擎的核心执行逻辑。
///   引擎在运行时先通过 ActionType 找到候选执行器集合，再由执行器内部结合 ExecutorCode
///   判断是否处理当前动作。管线会先调用 CanHandle 做显式校验，避免 ExecutorCode 配错时
///   所有候选执行器都静默跳过，导致资金规则漏执行。
///
/// 职责边界：
///   - 执行规则动作（RuleAction）定义的计价操作，将结果写入计价上下文。
///   - 不负责条件评估，条件由 IRuleConditionEvaluator 负责。
///   - 不负责额度校验的具体实现，由各执行器内部或通过 ILimitOccupyRepository 完成。
///
/// 动作类型示例：
///   - 折价公式（DISCOUNT_FORMULA）：执行公式计算折后金额
///   - 金额上限（AMOUNT_CEILING）：限制最高金额
///   - 金额下限（AMOUNT_FLOOR）：限制最低金额
///   - 日数量限制（DAY_QTY_LIMIT）：校验全院日累计数量
///   - 时间窗限制（TIME_WINDOW_LIMIT）：校验滑动窗口内累计数量
///   - 同组互斥（GROUP_MUTEX）：同组项目共享配额
///   - 子项加收（SUB_ITEM_SURCHARGE）：附加项目的额外收费
///   - 双单位换算（UNIT_CONVERSION）：不同计量单位间的换算
///
/// 异常处理：
///   执行器内部异常根据 ActionOnError 枚举决定处理策略：
///   - Stop：中止后续动作，返回错误
///   - Skip：跳过当前动作，继续执行后续动作
///   - Warn：记录警告，继续执行后续动作
///
/// 金额约束：
///   - 中间计算保留全部精度，最终金额保留2位小数、四舍五入。
///   - 超出限额的部分按0元处理，不是整单归零，不是拒单。
///   - NULL 表示"不校验"，0 表示"限制为零"。
/// </summary>
public interface IRuleActionExecutor
{
    /// <summary>
    /// 动作类型标识符，用于引擎在运行时查找对应的执行器。
    /// 必须与 PR_RULE_ACTION.ACTION_TYPE 字段的值一致。
    /// 多个执行器可以返回同一个动作类型，例如多个公式执行器共享 "FORMULA_CALC"。
    /// 例如："FORMULA_CALC"、"APPLY_MAX_AMOUNT"、"APPLY_DAY_LIMIT_QTY"。
    /// </summary>
    string ActionType { get; }

    /// <summary>
    /// 判断当前执行器是否真正处理该规则动作。
    /// </summary>
    /// <param name="action">
    /// 待执行的规则动作。普通一对一动作只需要匹配 ActionType；
    /// 多个执行器共享同一 ActionType 时，还必须结合 ExecutorCode 做二级分派。
    /// </param>
    /// <returns>
    /// <c>true</c> 表示该执行器会处理当前动作；<c>false</c> 表示应继续寻找其他候选执行器。
    /// </returns>
    /// <remarks>
    /// 默认实现覆盖一对一动作类型，例如 APPLY_MAX_AMOUNT、APPLY_DAY_LIMIT_QTY。
    /// FORMULA_CALC 这类共享动作类型必须由具体公式执行器覆写，按 ExecutorCode 精确匹配。
    /// 这样管线可以区分“ActionType 已注册但 ExecutorCode 未注册”和“执行器内部合法跳过”
    /// 两种情况，前者在 OnError=STOP 时必须中断，避免漏执行公式后继续收费。
    /// </remarks>
    bool CanHandle(RuleAction action)
    {
        return string.Equals(action.ActionType, ActionType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 执行规则动作定义的计价操作。
    ///
    /// 调用时机：计价引擎遍历规则的动作列表时，逐条调用此方法执行。
    /// 执行顺序按 PR_RULE_ACTION.SORT_NO 排序，依次执行。
    /// 执行结果（折后金额、折扣率、计算步骤等）直接写入 PricingContext 的结果集合中。
    /// </summary>
    /// <param name="action">
    /// 规则动作实体，包含：
    /// - ActionType：动作类型（与本执行器的 ActionType 属性匹配）
    /// - ActionValue：动作参数（如公式编码、金额上限值、数量限制值等）
    /// - ActionConfig：动作配置（JSON 格式，存储复杂参数如换算系数、互斥组编码等）
    /// </param>
    /// <param name="context">
    /// 计价上下文，既作为输入（当前项目信息、数量、单价等），
    /// 也作为输出载体（执行结果写入 context 的结果集合）。
    /// </param>
    /// <returns>异步任务，执行完成即返回；执行失败时根据 ActionOnError 策略处理。</returns>
    Task ExecuteAsync(RuleAction action, PricingContext context);
}
