using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则动作执行器工厂，负责根据动作类型编码（ActionType）定位对应执行器实例。
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
/// 【查找策略】按 ActionType 做大小写不敏感的字典索引，查找复杂度 O(1)。
/// 执行管线（<see cref="ActionExecutionPipeline"/>）在每个动作执行前调用 <see cref="GetExecutor"/>，
/// 未找到时由管线根据 OnError 配置决定跳过还是中断，工厂本身不抛异常。
/// </para>
/// <para>
/// 【二级分派】多个公式执行器可以共享 ActionType="FORMULA_CALC"，再通过 ExecutorCode 做二级分派
/// （参见 <c>IncrementPercentExecutor</c>）。工厂只负责一级分派。
/// </para>
/// </remarks>
public sealed class ActionExecutorFactory
{
    /// <summary>
    /// 动作类型到执行器实例的索引。
    /// </summary>
    /// <remarks>
    /// 键不区分大小写（OrdinalIgnoreCase），降低字典配置中大小写差异带来的运行风险。
    /// 同一 ActionType 只能注册一个执行器；如果 DI 中存在多个同 ActionType 的实现，
    /// ToDictionary 会抛 ArgumentException，需在启动阶段发现。
    /// </remarks>
    private readonly Dictionary<string, IRuleActionExecutor> _executors;

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
        // 在构造阶段一次性建索引，运行时执行动作时只做字典查找，避免每个动作都遍历执行器集合。
        // 如果存在重复 ActionType，此处会在应用启动时立即失败（fail-fast），
        // 便于开发阶段及时发现注册冲突。
        _executors = executors.ToDictionary(e => e.ActionType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 根据动作类型编码获取对应的执行器实例。
    /// </summary>
    /// <param name="actionType">
    /// 规则动作类型编码，来自 PR_RULE_ACTION.ACTION_TYPE 列。
    /// 典型值包括：CONVERT_QTY、FORMULA_CALC、APPLY_MIN_AMOUNT、APPLY_MAX_AMOUNT、
    /// APPLY_DAY_LIMIT_QTY、APPLY_TIME_WINDOW_LIMIT、DISCOUNT_EXCEED_TO_ZERO 等。
    /// </param>
    /// <returns>
    /// 匹配到的执行器实例；未注册时返回 <c>null</c>。
    /// 返回 null 而非抛异常，是因为不同动作可通过 OnError 配置决定跳过还是中断——
    /// 如果工厂抛异常，就剥夺了执行管线根据上下文做差异化处理的能力。
    /// </returns>
    public IRuleActionExecutor? GetExecutor(string actionType)
    {
        // 未找到执行器时不在工厂抛异常，因为不同动作可以通过 OnError 配置决定跳过还是中断。
        // 资金动作默认 OnError=STOP，非资金动作可配置为 SKIP/WARN。
        _executors.TryGetValue(actionType, out var executor);
        return executor;
    }
}
