using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则动作执行管线，负责把已按全局顺序排序的动作逐个派发给对应执行器执行。
/// </summary>
/// <remarks>
/// <para>
/// 【职责边界】该管线不决定规则是否命中，也不决定动作全局顺序——这些由
/// <see cref="RuleMatchService"/> 完成。管线关注运行期执行策略：
/// <list type="bullet">
///   <item><description>找不到执行器时：根据 OnError 配置决定跳过还是中断</description></item>
///   <item><description>执行失败时：STOP 中断并回滚、WARN 记录后继续、SKIP 静默继续</description></item>
///   <item><description>每步执行后：写追溯步骤（TraceStep），记录金额变化</description></item>
/// </list>
/// </para>
/// <para>
/// 【执行顺序保证】动作链在进入管线前已由 <see cref="RuleMatchService.OrderActions"/> 排序：
/// 先按全局动作类别（换算 → 公式 → 限额 → 折价），再按同类动作的 SortNo。
/// 管线严格按此顺序执行，不做任何重排。
/// </para>
/// <para>
/// 【资金安全】资金相关动作（公式、限额、折价）默认 OnError=STOP，
/// 宁可让渠道失败重试，也不能用不完整动作链收费。非资金提示类动作可配置为 WARN 或 SKIP。
/// </para>
/// </remarks>
public sealed class ActionExecutionPipeline
{
    /// <summary>
    /// 动作执行器工厂，用于根据规则动作类型（ActionType）找到具体执行器实例。
    /// </summary>
    private readonly ActionExecutorFactory _executorFactory;

    /// <summary>
    /// 管线日志，用于记录缺失执行器、动作执行异常等运行期诊断信息。
    /// </summary>
    private readonly ILogger<ActionExecutionPipeline> _logger;

    /// <summary>
    /// 初始化规则动作执行管线。
    /// </summary>
    /// <param name="executorFactory">动作执行器工厂，负责 ActionType 到执行器的映射。</param>
    /// <param name="logger">日志对象，用于结构化日志输出。</param>
    public ActionExecutionPipeline(
        ActionExecutorFactory executorFactory,
        ILogger<ActionExecutionPipeline> logger)
    {
        _executorFactory = executorFactory;
        _logger = logger;
    }

    /// <summary>
    /// 按顺序执行规则动作链。每个动作执行后都会写追溯步骤，记录金额变化。
    /// </summary>
    /// <param name="actions">
    /// 已按全局顺序排序的规则动作集合。排序由 <see cref="RuleMatchService.OrderActions"/> 完成，
    /// 保证先换算、再公式、再限额、最后折价。
    /// </param>
    /// <param name="context">
    /// 计价上下文。执行器会持续修改其中的 FinalQty（最终数量）、FinalAmount（最终金额）、
    /// FormulaAmount（公式金额）以及限额占用草稿（PendingOccupies）。
    /// </param>
    /// <returns>异步任务。</returns>
    /// <exception cref="InvalidOperationException">
    /// 当资金动作的执行器未注册时抛出（OnError=STOP）。
    /// </exception>
    /// <exception cref="Exception">
    /// 当资金动作执行失败时抛出原始异常（OnError=STOP）。
    /// </exception>
    public async Task ExecuteAsync(IReadOnlyList<RuleAction> actions, PricingContext context)
    {
        // 从已有追溯步骤后继续编号。PricingEngine 在调用本管线前会先写 MATCH 步骤，
        // 动作管线从下一号开始，保证追溯步骤顺序与实际执行顺序严格一致。
        // 追溯顺序一致是折价追溯中心的基础要求——审计人员按步骤号回放计算过程。
        var stepNo = context.TraceSteps.Count + 1;

        foreach (var action in actions)
        {
            // ========== 第一阶段：按 ACTION_TYPE 选择执行器 ==========
            // ActionType 是规则动作字典中的稳定编码（如 FORMULA_CALC、APPLY_MAX_AMOUNT），
            // 不使用 ExecutorCode 直接找类，是为了允许 FORMULA_CALC 这类动作在执行器内部
            // 再按 ExecutorCode 细分公式类型（如 INCREMENT_PERCENT、LINEAR_DISCOUNT）。
            // 这是"一级分派 + 二级分派"的设计，新增公式类型只需新增执行器，无需改 ActionType 映射。
            var executor = _executorFactory.GetExecutor(action.ActionType);

            if (executor is null)
            {
                _logger.LogWarning("未找到动作执行器: {ActionType}，ActionId={ActionId}",
                    action.ActionType, action.ActionId);

                // 资金动作默认 STOP。没有执行器时继续收费会造成规则漏执行，
                // 风险高于接口失败——宁可让渠道重试，也不能漏执行。
                if (action.OnError == "STOP")
                {
                    throw new InvalidOperationException(
                        $"未找到动作执行器: {action.ActionType}，ActionId={action.ActionId}");
                }

                // 非资金或显式允许跳过的动作才继续执行后续动作。
                // 典型场景：ADD_CHILD_ITEM（子项加收）配置为 SKIP，缺失时不影响核心计价。
                continue;
            }

            // 追溯步骤保存动作执行前后的金额变化，用于折价追溯中心的计算过程回放。
            // 数量变化目前未单独快照（只记录金额），后续可扩展 InputSnapshot/OutputSnapshot
            // 保存完整上下文 JSON，支持更细粒度的审计。
            var inputValue = context.FinalAmount;

            try
            {
                // ========== 第二阶段：执行具体动作 ==========
                // 执行器可以修改 FinalQty（如数量截断）、FinalAmount（如金额封顶/保底）、
                // FormulaAmount（如公式计算结果），也可以追加限额占用草稿到 PendingOccupies。
                // 执行器内部应保证中间计算保留全精度，不提前取整。
                await executor.ExecuteAsync(action, context);

                // ========== 第三阶段：写成功追溯步骤 ==========
                // StepType 必须映射到数据库 CHECK 约束允许的稳定类别（CONVERT/FORMULA/LIMIT/DISCOUNT/VALIDATE），
                // 而不是直接写 ACTION_TYPE。好处：
                //   1. 新增 ActionType 时无需改 DDL 的 CHECK 约束
                //   2. 追溯查询可以按大类展示，避免前端枚举膨胀
                context.TraceSteps.Add(new TraceStep
                {
                    StepNo = stepNo++,
                    StepType = GetStepType(action.ActionType),
                    StepDesc = $"执行 {action.ActionType}",
                    InputValue = inputValue,
                    OutputValue = context.FinalAmount,
                    ParamsJson = action.ParamsJson
                });
            }
            catch (Exception ex)
            {
                // ========== 第四阶段：动作异常策略 ==========
                // 执行器异常分为三级处理：
                //   STOP — 中断并向上抛出，外层事务回滚。资金动作的默认策略。
                //   WARN — 记录错误步骤后继续。适合非资金提示类动作。
                //   SKIP — 仅记录日志后继续。适合纯辅助动作。
                _logger.LogError(ex, "执行动作失败 ActionType={ActionType}, ActionId={ActionId}",
                    action.ActionType, action.ActionId);

                if (action.OnError == "STOP")
                {
                    // STOP 是资金动作的默认策略。宁可让渠道失败重试，也不能用不完整动作链收费。
                    // 向上抛出后，外层 PricingEngine 会回滚事务，渠道收到错误码后可重试。
                    throw;
                }

                if (action.OnError == "WARN")
                    // WARN 只适合非资金提示类动作。这里保留错误步骤，方便追溯时看到动作曾经失败。
                    // StepType 设为 "ERROR" 便于追溯查询按类型过滤异常步骤。
                {
                    context.TraceSteps.Add(new TraceStep
                    {
                        StepNo = stepNo++,
                        StepType = "ERROR",
                        StepDesc = $"执行异常(已跳过): {ex.Message}",
                        InputValue = inputValue,
                        OutputValue = context.FinalAmount
                    });
                }

                // OnError == "SKIP" 时：仅日志记录，不写追溯步骤，不中断。
            }
        }
    }

    /// <summary>
    /// 将动作类型映射为追踪步骤类型。映射关系遵循数据库 STEP_TYPE 列的 CHECK 约束。
    /// </summary>
    /// <param name="actionType">规则动作类型编码。</param>
    /// <returns>
    /// 数据库步骤表允许的步骤类型，取值范围：
    /// CONVERT（换算）、FORMULA（公式）、LIMIT（限额）、DISCOUNT（折价）、VALIDATE（兜底）。
    /// </returns>
    /// <remarks>
    /// 映射原则：同一业务大类的动作共享步骤类型。好处：
    /// <list type="bullet">
    ///   <item><description>新增 ActionType 时无需改 DDL 的 CHECK 约束</description></item>
    ///   <item><description>追溯查询可以按大类过滤和聚合</description></item>
    ///   <item><description>前端展示时按大类分组，避免枚举膨胀</description></item>
    /// </list>
    /// </remarks>
    private static string GetStepType(string actionType)
    {
        // 数据库步骤表的 STEP_TYPE 是受控枚举。将具体动作映射到受控步骤类型，
        // 能避免新增动作时频繁改 DDL，也让追溯查询按大类展示。
        return actionType switch
        {
            // 换算类：双单位换算
            "CONVERT_QTY" => "CONVERT",

            // 公式类：各种计价公式计算
            "FORMULA_CALC" => "FORMULA",

            // 限额类：金额上下限、日限额、时间窗限额、单次限额、同组互斥
            "APPLY_MIN_AMOUNT" or "APPLY_MAX_AMOUNT" or "APPLY_DAY_LIMIT_QTY"
                or "APPLY_TIME_WINDOW_LIMIT" or "APPLY_ONCE_LIMIT_QTY"
                or "SAME_GROUP_MUTEX" or "SAME_OPERATION_CEILING" => "LIMIT",

            // 折价类：超额归零、子项加收
            "DISCOUNT_EXCEED_TO_ZERO" or "ADD_CHILD_ITEM" => "DISCOUNT",

            // 兜底：未知动作类型映射为 VALIDATE，便于追溯时识别未分类动作
            _ => "VALIDATE"
        };
    }
}
