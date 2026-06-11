using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine;

/// <summary>
/// 规则匹配服务，负责从已发布规则中找出本次计价应执行的规则和动作链。
/// </summary>
/// <remarks>
/// <para>
/// 【匹配流程概述】规则匹配分为六阶段：
/// <list type="number">
///   <item><description>按项目编码取候选规则（初筛）</description></item>
///   <item><description>过滤已发布、启用、业务时间有效的规则</description></item>
///   <item><description>逐条评估条件组（同组 AND，跨组 OR）</description></item>
///   <item><description>按规则优先级排序</description></item>
///   <item><description>收集命中规则的动作</description></item>
///   <item><description>按全局动作顺序整理动作链</description></item>
/// </list>
/// </para>
/// <para>
/// 【多规则叠加】同一项目可能命中多条规则（如不同部位的换算规则）。
/// 多规则的动作统一按全局动作顺序排列，确保换算 → 数量限制/互斥 → 公式折价 → TOPPRICE 封顶 → 超限归零兜底
/// 的执行顺序不被单条规则内部 SortNo 打乱。
/// </para>
/// <para>
/// 【业务时间】匹配使用 BusinessChargeTime（HIS 业务发生时间），而不是服务器当前时间。
/// 这确保补录历史费用时按历史规则时间窗口命中，而不是按补录操作时间。
/// </para>
/// <para>
/// 【核心约束引用】
/// <list type="bullet">
///   <item><description>NULL ≠ 0：条件值为 NULL 表示"不校验"，0 表示"限制为零"</description></item>
///   <item><description>HIS 兼容顺序：先做数量类限制，再用限制后的数量执行比例折价，最后做 TOPPRICE 封顶</description></item>
///   <item><description>超出 = 0元：不是拒单，不是整单归零——由 ExceedToZeroExecutor 实现</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class RuleMatchService : IRuleRuntimeCacheInvalidator
{
    private readonly IEffectiveRuleSnapshotCache _snapshotCache;
    private readonly IRuleConditionGroupMatcher _conditionMatcher;
    private readonly IRuleActionPlanBuilder _actionPlanBuilder;

    /// <summary>
    /// 匹配日志，用于记录未知评估器、命中数量等运行期诊断信息。
    /// </summary>
    private readonly ILogger<RuleMatchService> _logger;

    /// <summary>
    /// 初始化规则匹配服务。
    /// </summary>
    /// <param name="snapshotCache">运行期候选规则快照缓存。</param>
    /// <param name="conditionMatcher">条件组匹配器。</param>
    /// <param name="actionPlanBuilder">动作执行计划构建器。</param>
    /// <param name="logger">日志对象。</param>
    public RuleMatchService(
        IEffectiveRuleSnapshotCache snapshotCache,
        IRuleConditionGroupMatcher conditionMatcher,
        IRuleActionPlanBuilder actionPlanBuilder,
        ILogger<RuleMatchService> logger)
    {
        _snapshotCache = snapshotCache;
        _conditionMatcher = conditionMatcher;
        _actionPlanBuilder = actionPlanBuilder;
        _logger = logger;
    }

    /// <summary>
    /// 匹配当前计价上下文对应的规则和动作链。
    /// </summary>
    /// <param name="context">
    /// 计价上下文，至少包含 ItemCode（项目编码）和 BusinessChargeTime（业务收费时间）。
    /// </param>
    /// <returns>
    /// 命中的规则集合（按优先级排序），以及按全局顺序排列后的动作集合。
    /// 两个集合的对应关系通过 RuleId + Version 可追溯。
    /// </returns>
    public async Task<(IReadOnlyList<RuleAggregate> Rules, IReadOnlyList<RuleAction> Actions)>
        MatchAsync(PricingContext context)
    {
        // ========== 第一阶段：按项目编码取候选规则 ==========
        // 初筛只看 ITEM_CODE，避免全表扫描。项目组规则后续可扩展为按 GROUP_CODE 查询。
        // 仓储实现应利用索引 PR_RULE_HEADER(ITEM_CODE, STATUS, IS_ENABLED) 加速查询。
        var candidates = await _snapshotCache.GetByItemCodeAsync(context.ItemCode);

        // ========== 第二阶段：过滤已发布、启用、业务时间有效的规则 ==========
        // 三重过滤：
        //   Status == "PUBLISHED"：只使用已发布的规则，草稿和审批中的规则不参与计价
        //   IsEnabled == "Y"：运行时可临时禁用规则而无需删除
        //   业务时间在生效区间内：EffectiveFrom <= BusinessChargeTime <= EffectiveTo
        // 业务时间使用 BusinessChargeTime，而不是当前系统时间，确保补录按真实收费时间匹配历史规则。
        var published = candidates
            .Where(snapshot => snapshot.Header.Status == "PUBLISHED" && snapshot.Header.IsEnabled == "Y")
            .Where(snapshot => IsInEffectiveRange(snapshot.Header, context.BusinessChargeTime))
            .ToList();

        // ========== 第三阶段：逐条评估条件组 ==========
        // 条件表支持同组 AND、跨组 OR 语义（参见 EvaluateConditionsAsync 方法）。
        // 只要任一条件组全部满足，该规则就命中。这是"OR 组，AND 组内"的经典规则引擎模式。
        var matchedRules = new List<RuleAggregate>();

        foreach (var snapshot in published)
        {
            if (await _conditionMatcher.EvaluateAsync(snapshot.Conditions, context))
            {
                matchedRules.Add(snapshot.Header);
            }
        }

        // ========== 第四阶段：按规则优先级排序 ==========
        // 数字越小优先级越高。当多条规则同时命中时，优先级决定动作的执行顺序基准。
        // 注意：最终动作顺序还受全局 ActionTypeOrder 影响，优先级主要影响同类动作的先后。
        matchedRules.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // ========== 第五阶段：收集命中规则的动作 ==========
        // 只收集已启用的动作（IsEnabled == "Y"），允许规则维护时临时禁用单个动作。
        // 动作是否真正执行还取决于 ActionExecutionPipeline 和具体 Executor 的判断。
        var allActions = new List<RuleAction>();
        foreach (var rule in matchedRules)
        {
            var snapshot = published.First(item => item.Header.RuleId == rule.RuleId);
            allActions.AddRange(snapshot.Actions.Where(a => a.IsEnabled == "Y"));
        }
        var ordered = await _actionPlanBuilder.BuildAsync(allActions, matchedRules);

        _logger.LogInformation(
            "规则匹配 项目编码={ItemCode}, 命中规则数={RuleCount}, 动作数={ActionCount}",
            context.ItemCode, matchedRules.Count, ordered.Count);

        return (matchedRules, ordered);
    }

    /// <summary>
    /// 清除动作执行顺序缓存，强制下次 MatchAsync 从字典重新加载。
    ///
    /// 调用时机：
    ///   - 规则发布（PublishAsync）后，动作顺序可能已调整。
    ///   - 规则停用（DisableAsync）后，需要刷新缓存。
    ///   - 规则回滚（RollbackAsync）后，需要刷新缓存。
    ///   - 字典维护界面修改 ACTION_TYPE_ORDER 类型字典后。
    ///
    /// 线程安全：
    ///   使用 volatile 写入确保多线程可见性。
    ///   清除后下一个 MatchAsync 调用会通过 EnsureActionTypeOrderLoadedAsync 重新加载。
    /// </summary>
    public void ClearActionTypeOrderCache()
    {
        _actionPlanBuilder.ClearCache();
    }

    /// <summary>
    /// 清除规则匹配运行期缓存。
    /// </summary>
    /// <remarks>
    /// 目前运行期缓存主要是 ACTION_TYPE_ORDER 动作顺序。通过接口暴露失效能力，
    /// 发布服务不需要知道 RuleMatchService 的具体缓存结构。
    /// </remarks>
    public void ClearRuntimeCache()
    {
        ClearActionTypeOrderCache();
        _snapshotCache.Clear();
    }

    /// <summary>
    /// 判断业务时间是否落在规则生效区间内（闭区间）。
    /// </summary>
    /// <param name="rule">规则主档，包含 EffectiveFrom 和 EffectiveTo。</param>
    /// <param name="businessTime">业务收费时间（来自 HIS，非服务器时间）。</param>
    /// <returns>在生效区间内返回 <c>true</c>。</returns>
    /// <remarks>
    /// 生效区间按闭区间处理：[EffectiveFrom, EffectiveTo]。
    /// NULL 值表示不设限：EffectiveFrom 为 null 表示无开始时间限制，
    /// EffectiveTo 为 null 表示无结束时间限制。
    /// 这与业务规则"NULL ≠ 0"一致：NULL 表示"不校验"。
    /// </remarks>
    private static bool IsInEffectiveRange(RuleAggregate rule, DateTime businessTime)
    {
        // 生效区间按闭区间处理：小于开始时间不生效，大于结束时间不生效。
        // EffectiveFrom/EffectiveTo 为 null 时不设限。
        if (rule.EffectiveFrom.HasValue && businessTime < rule.EffectiveFrom.Value)
        {
            return false;
        }

        if (rule.EffectiveTo.HasValue && businessTime > rule.EffectiveTo.Value)
        {
            return false;
        }

        return true;
    }
}
