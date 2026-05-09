using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Api.Engine;

/// <summary>
/// 规则动作执行器工厂，负责根据动作类型编码定位对应执行器。
/// </summary>
/// <remarks>
/// 规则表只保存动作类型和执行器编码，不直接依赖具体 C# 类型。工厂在应用启动后收集所有
/// <see cref="IRuleActionExecutor"/> 实现，并按 ActionType 做大小写不敏感索引，供执行流水线快速查找。
/// </remarks>
public sealed class ActionExecutorFactory
{
    /// <summary>
    /// 动作类型到执行器实例的索引。键不区分大小写，降低字典配置大小写差异带来的运行风险。
    /// </summary>
    private readonly Dictionary<string, IRuleActionExecutor> _executors;

    /// <summary>
    /// 初始化动作执行器工厂。
    /// </summary>
    /// <param name="executors">依赖注入容器中注册的全部规则动作执行器。</param>
    public ActionExecutorFactory(IEnumerable<IRuleActionExecutor> executors)
    {
        // 在构造阶段一次性建索引，运行时执行动作时只做字典查找，避免每个动作都遍历执行器集合。
        _executors = executors.ToDictionary(e => e.ActionType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 根据动作类型获取执行器。
    /// </summary>
    /// <param name="actionType">规则动作类型编码。</param>
    /// <returns>匹配到的执行器；未注册时返回 <c>null</c>，由执行流水线决定错误策略。</returns>
    public IRuleActionExecutor? GetExecutor(string actionType)
    {
        // 未找到执行器时不在工厂抛异常，因为不同动作可以通过 OnError 配置决定跳过还是中断。
        _executors.TryGetValue(actionType, out var executor);
        return executor;
    }
}
