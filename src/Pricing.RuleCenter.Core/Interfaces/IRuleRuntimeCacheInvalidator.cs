namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则运行期缓存失效接口。
/// </summary>
/// <remarks>
/// 发布服务只需要声明"规则运行期缓存需要失效"，不应直接依赖计价引擎或具体匹配服务。
/// 当前实现由 RuleMatchService 承担，用于清除动作执行顺序等跨请求共享缓存。
/// </remarks>
public interface IRuleRuntimeCacheInvalidator
{
    /// <summary>
    /// 清除规则运行期缓存，使下一次计价重新加载最新配置。
    /// </summary>
    void ClearRuntimeCache();
}
