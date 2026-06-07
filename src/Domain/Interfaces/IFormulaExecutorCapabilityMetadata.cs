namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 公式动作执行器能力元数据。
/// </summary>
/// <remarks>
/// 多个执行器共享 <c>FORMULA_CALC</c> 动作类型，发布门禁需要知道每个执行器实际支持哪些
/// <c>ExecutorCode</c>。该元数据由执行器实现显式声明，避免运行期 DI 注册和发布期静态白名单分叉。
/// </remarks>
public interface IFormulaExecutorCapabilityMetadata
{
    /// <summary>
    /// 当前执行器支持的公式执行器编码集合。
    /// </summary>
    IReadOnlyCollection<string> SupportedExecutorCodes { get; }
}
