using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则动作执行器工厂，负责根据动作类型编码（ActionType）定位对应执行器实例集合。
/// </summary>
/// <remarks>
/// <para>
/// 【设计模式】工厂 + 策略模式。规则表（PR_RULE_ACTION）只保存动作类型编码和执行器编码，
/// 不直接引用具体 C# 类型。新增动作类型只需实现 <see cref="IRuleActionExecutor"/> 并在 DI 注册，
/// 无需修改本工厂或执行管线。
/// </para>
/// <para>
/// 【注册机制】ASP.NET Core 启动时，通过 <c>services.Scan(...)</c> 或逐个 <c>AddTransient</c>
/// 注册所有 <see cref="IRuleActionExecutor"/> 实现。构造函数接收 <c>IEnumerable&lt;IRuleActionExecutor&gt;</c>
/// 由 DI 容器自动注入全部已注册实现，一次性建索引。
/// </para>
/// <para>
/// 【查找策略】按 ActionType 做大小写不敏感的一对多字典索引，查找复杂度 O(1)。
/// 执行管线（<see cref="ActionExecutionPipeline"/>）在每个动作执行前调用 <see cref="GetExecutors"/>，
/// 未找到时由管线根据 OnError 配置决定跳过还是中断，工厂本身不抛异常。
/// </para>
/// <para>
/// 【二级分派】多个公式执行器可以共享 ActionType="FORMULA_CALC"，再通过 ExecutorCode 做二级分派
/// （参见 <c>IncrementPercentExecutor</c>）。工厂只负责一级分派，具体是否处理由执行器内部判断。
/// </para>
/// </remarks>
public sealed class ActionExecutorFactory
{
    /// <summary>
    /// 动作类型到执行器实例集合的索引。
    /// </summary>
    /// <remarks>
    /// 键不区分大小写（OrdinalIgnoreCase），降低字典配置中大小写差异带来的运行风险。
    /// 同一 ActionType 允许注册多个执行器，典型场景是多个公式执行器共享 FORMULA_CALC，
    /// 再根据 PR_RULE_ACTION.EXECUTOR_CODE 做二级分派。
    /// </remarks>
    private readonly Dictionary<string, IReadOnlyList<IRuleActionExecutor>> _executors;

    /// <summary>
    /// 初始化动作执行器工厂。
    /// </summary>
    /// <param name="executors">
    /// 依赖注入容器中注册的全部规则动作执行器实现。
    /// 典型实现包括：IncrementPercentExecutor（增量比例）、AmountCeilingExecutor（金额封顶）、
    /// AmountFloorExecutor（金额保底）、ExceedToZeroExecutor（超额归零）等。
    /// </param>
    public ActionExecutorFactory(IEnumerable<IRuleActionExecutor> executors)
    {
        // 在构造阶段一次性建一对多索引，运行时执行动作时只做字典查找，
        // 避免每个动作都遍历全量执行器集合。
        //
        // 这里不能再使用 ToDictionary(ActionType)，因为 FORMULA_CALC 是动作大类，
        // IncrementPercentExecutor、AreaStepIncrementExecutor、ConvertQtyByPartExecutor 等
        // 多个公式执行器会共享该 ActionType，再由 ExecutorCode 做二级分派。
        _executors = executors
            .GroupBy(e => e.ActionType, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<IRuleActionExecutor>)group.ToList(),
                StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 根据动作类型编码获取对应的执行器实例集合。
    /// </summary>
    /// <param name="actionType">
    /// 规则动作类型编码，来自 PR_RULE_ACTION.ACTION_TYPE 列。
    /// 典型值包括：CONVERT_QTY、FORMULA_CALC、APPLY_MIN_AMOUNT、APPLY_MAX_AMOUNT、
    /// APPLY_DAY_LIMIT_QTY、APPLY_TIME_WINDOW_LIMIT、DISCOUNT_EXCEED_TO_ZERO 等。
    /// </param>
    /// <returns>
    /// 匹配到的执行器实例集合；未注册时返回空集合。
    /// 返回空集合而非抛异常，是因为不同动作可通过 OnError 配置决定跳过还是中断——
    /// 如果工厂抛异常，就剥夺了执行管线根据上下文做差异化处理的能力。
    /// </returns>
    public IReadOnlyList<IRuleActionExecutor> GetExecutors(string actionType)
    {
        // 未找到执行器时不在工厂抛异常，因为不同动作可以通过 OnError 配置决定跳过还是中断。
        // 资金动作默认 OnError=STOP，非资金动作可配置为 SKIP/WARN。
        return _executors.TryGetValue(actionType, out var executors)
            ? executors
            : Array.Empty<IRuleActionExecutor>();
    }

    /// <summary>
    /// 根据动作类型编码获取单个执行器实例。
    /// </summary>
    /// <remarks>
    /// 保留该方法是为了兼容旧调用方。新执行管线应使用 <see cref="GetExecutors"/>，
    /// 因为 FORMULA_CALC 这类动作类型可能对应多个执行器。
    /// </remarks>
    /// <param name="actionType">规则动作类型编码。</param>
    /// <returns>匹配到的第一个执行器；未注册时返回 <c>null</c>。</returns>
    public IRuleActionExecutor? GetExecutor(string actionType)
    {
        var executors = GetExecutors(actionType);
        return executors.Count > 0 ? executors[0] : null;
    }
}
