using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine;

/// <summary>
/// 规则动作执行管线，负责把已经排序好的动作逐个派发给对应执行器。
/// </summary>
/// <remarks>
/// 该管线不决定规则是否命中，也不决定动作全局顺序；这些由 RuleMatchService 完成。
/// 它关注运行期执行策略：找不到执行器时如何处理，执行失败时是否停止，如何写追溯步骤。
/// </remarks>
public sealed class ActionExecutionPipeline
{
    /// <summary>
    /// 动作执行器工厂，用于根据规则动作类型找到具体执行器。
    /// </summary>
    private readonly ActionExecutorFactory _executorFactory;
    /// <summary>
    /// 管线日志，用于记录缺失执行器和动作执行异常。
    /// </summary>
    private readonly ILogger<ActionExecutionPipeline> _logger;

    /// <summary>
    /// 初始化规则动作执行管线。
    /// </summary>
    /// <param name="executorFactory">动作执行器工厂。</param>
    /// <param name="logger">日志对象。</param>
    public ActionExecutionPipeline(
        ActionExecutorFactory executorFactory,
        ILogger<ActionExecutionPipeline> logger)
    {
        _executorFactory = executorFactory;
        _logger = logger;
    }

    /// <summary>
    /// 按顺序执行规则动作链。
    /// </summary>
    /// <param name="actions">已经按全局顺序排序的规则动作。</param>
    /// <param name="context">计价上下文，执行器会持续修改其中的数量、金额和占额草稿。</param>
    /// <returns>异步任务。</returns>
    public async Task ExecuteAsync(IReadOnlyList<RuleAction> actions, PricingContext context)
    {
        // 从已有追溯步骤后继续编号。PricingEngine 会先写 MATCH 步骤，
        // 动作管线从下一号开始，保证追溯顺序和执行顺序一致。
        var stepNo = context.TraceSteps.Count + 1;

        foreach (var action in actions)
        {
            // ========== 第一阶段：按 ACTION_TYPE 选择执行器 ==========
            // ActionType 是规则动作字典中的稳定编码，不使用 ExecutorCode 直接找类，
            // 是为了允许 FORMULA_CALC 这类动作在执行器内部再按 ExecutorCode 细分公式。
            var executor = _executorFactory.GetExecutor(action.ActionType);
            if (executor is null)
            {
                _logger.LogWarning("未找到动作执行器: {ActionType}", action.ActionType);

                // 资金动作默认 STOP。没有执行器时继续收费会造成规则漏执行，风险高于接口失败。
                if (action.OnError == "STOP")
                {
                    throw new InvalidOperationException($"未找到动作执行器: {action.ActionType}");
                }

                // 非资金或显式允许跳过的动作才继续执行后续动作。
                continue;
            }

            // 追溯步骤保存动作执行前后的金额变化。数量变化目前未单独快照，
            // 后续可以扩展 InputSnapshot/OutputSnapshot 保存完整上下文 JSON。
            var inputValue = context.FinalAmount;

            try
            {
                // ========== 第二阶段：执行具体动作 ==========
                // 执行器可以修改 FinalQty、FinalAmount、FormulaAmount，也可以追加限额占用草稿。
                await executor.ExecuteAsync(action, context);

                // ========== 第三阶段：写成功追溯步骤 ==========
                // StepType 必须映射到数据库 CHECK 约束允许的稳定类别，而不是直接写 ACTION_TYPE。
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
                // 执行器异常分为可继续和必须中断。STOP 会向上抛出并让外层事务回滚；
                // WARN 会记录错误步骤后继续，SKIP 则仅记录日志并继续。
                _logger.LogError(ex, "执行动作失败 ActionType={ActionType}, ActionId={ActionId}",
                    action.ActionType, action.ActionId);

                if (action.OnError == "STOP")
                {
                    // STOP 是资金动作的默认策略。宁可让渠道失败重试，也不能用不完整动作链收费。
                    throw;
                }

                if (action.OnError == "WARN")
                {
                    // WARN 只适合非资金提示类动作。这里保留错误步骤，方便追溯时看到动作曾经失败。
                    context.TraceSteps.Add(new TraceStep
                    {
                        StepNo = stepNo++,
                        StepType = "ERROR",
                        StepDesc = $"执行异常(已跳过): {ex.Message}",
                        InputValue = inputValue,
                        OutputValue = context.FinalAmount
                    });
                }
            }
        }
    }

    /// <summary>
    /// 将动作类型映射为追踪步骤类型。
    /// </summary>
    /// <param name="actionType">规则动作类型。</param>
    /// <returns>数据库步骤表允许的步骤类型。</returns>
    private static string GetStepType(string actionType)
    {
        // 数据库步骤表的 STEP_TYPE 是受控枚举。将具体动作映射到受控步骤类型，
        // 能避免新增动作时频繁改 DDL，也让追溯查询按大类展示。
        return actionType switch
        {
            "CONVERT_QTY" => "CONVERT",
            "FORMULA_CALC" => "FORMULA",
            "APPLY_MIN_AMOUNT" or "APPLY_MAX_AMOUNT" or "APPLY_DAY_LIMIT_QTY"
                or "APPLY_TIME_WINDOW_LIMIT" or "APPLY_ONCE_LIMIT_QTY"
                or "SAME_GROUP_MUTEX" => "LIMIT",
            "DISCOUNT_EXCEED_TO_ZERO" => "DISCOUNT",
            _ => "VALIDATE"
        };
    }
}
