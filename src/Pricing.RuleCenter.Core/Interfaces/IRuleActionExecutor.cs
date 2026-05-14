using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则动作执行器接口 —— 条件-动作分离模式中"动作"侧的核心抽象。
///
/// 架构位置：
///   位于领域层（Domain），由计价引擎在规则匹配成功后的动作执行阶段调用。
///   每种动作类型对应一个独立的执行器实现，通过 ActionType 属性注册到引擎。
///
/// 设计模式：
///   执行器模式（Strategy Pattern）—— 新增动作类型只需新增一个 IRuleActionExecutor 实现类，
///   无需修改计价引擎的核心执行逻辑。引擎在运行时通过 ActionType 查找对应的执行器。
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
    /// 必须与 PR_RULE_ACTION.ACTION_TYPE 字段的值一一对应。
    /// 例如："DISCOUNT_FORMULA"、"AMOUNT_CEILING"、"DAY_QTY_LIMIT"。
    /// </summary>
    string ActionType { get; }

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
