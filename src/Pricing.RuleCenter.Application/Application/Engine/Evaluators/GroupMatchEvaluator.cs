using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Evaluators;

/// <summary>
/// 项目组匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】判断当前计价项目是否属于指定项目组。典型场景：
/// <list type="bullet">
///   <item><description>皮肤科治疗项目组：组内所有项目适用同一折价公式</description></item>
///   <item><description>同手术项目组：同一手术下的多个项目共享限额</description></item>
///   <item><description>同类互斥项目组：同组项目只执行优先级最高的一条</description></item>
/// </list>
/// </para>
/// <para>
/// 【与 PricingContext.ItemGroupCode 的关系】
/// PricingContext.ItemGroupCode 是调用方直接传入的项目组编码，适用于调用方已知项目组归属的场景。
/// 本评估器通过查询 PR_ITEM_GROUP_DETAIL 表判断项目是否属于指定组，适用于调用方未传入组编码
/// 或需要按规则条件动态匹配不同项目组的场景。两者互补：
/// <list type="bullet">
///   <item><description>ItemGroupCode 有值时可直接比较（快速路径）</description></item>
///   <item><description>ItemGroupCode 为空时需查询数据库（完整路径）</description></item>
/// </list>
/// </para>
/// <para>
/// 【空值处理策略】
/// <list type="bullet">
///   <item><description>condition.RightValue 为空 → 通过（规则未配置项目组限制）</description></item>
///   <item><description>context.ItemCode 为空 → 不通过（项目编码是必选维度，缺失应阻断）</description></item>
/// </list>
/// </para>
/// <para>
/// 【保守策略】评估器异常时默认不通过（安全优先），防止因异常导致错误放行。
/// </para>
/// </remarks>
public sealed class GroupMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 项目组仓储，用于按编码查找项目组。
    /// </summary>
    private readonly IItemGroupRepository _itemGroupRepository;

    /// <summary>
    /// 项目组明细仓储，用于查询组内项目成员。
    /// </summary>
    private readonly IItemGroupDetailRepository _itemGroupDetailRepository;

    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "GROUP_MATCH"。
    /// </summary>
    public string ConditionType => "GROUP_MATCH";

    /// <summary>
    /// 初始化项目组匹配评估器。
    /// </summary>
    /// <param name="itemGroupRepository">项目组仓储，由 DI 容器按 Scoped 生命周期注入。</param>
    /// <param name="itemGroupDetailRepository">项目组明细仓储，由 DI 容器按 Scoped 生命周期注入。</param>
    public GroupMatchEvaluator(
        IItemGroupRepository itemGroupRepository,
        IItemGroupDetailRepository itemGroupDetailRepository)
    {
        _itemGroupRepository = itemGroupRepository;
        _itemGroupDetailRepository = itemGroupDetailRepository;
    }

    /// <summary>
    /// 异步判断当前计价项目是否属于规则条件指定的项目组。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存目标项目组编码（来自 PR_RULE_CONDITION.RIGHT_VALUE 列）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。ItemCode 来自本次收费请求。
    /// </param>
    /// <returns>
    /// 项目属于指定项目组时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制项目组）。
    /// ItemCode 为空时返回 <c>false</c>（项目编码是必选维度）。
    /// 项目组不存在时返回 <c>false</c>（配置错误保守处理）。
    /// </returns>
    public async ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        // 项目组条件为空时视为不限制项目组，避免可选条件缺失导致规则整体失效。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 项目编码是必选维度。与 ItemMatchEvaluator 一致：项目条件缺少目标值时不应放行。
        if (string.IsNullOrEmpty(context.ItemCode))
        {
            return false;
        }

        // ========== 快速路径：直接比较 ItemGroupCode ==========
        // 如果调用方已在上下文中传入了项目组编码，且与条件配置一致，直接返回通过。
        // 这避免了不必要的数据库查询，是性能优化的快速路径。
        if (!string.IsNullOrEmpty(context.ItemGroupCode) &&
            string.Equals(context.ItemGroupCode, condition.RightValue, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // ========== 完整路径：查询数据库判断项目组归属 ==========
        // 调用方未传入 ItemGroupCode 或与条件不一致时，需要查询数据库。
        // 评估接口保持异步，避免在 ASP.NET Core 请求线程上同步等待数据库 I/O。
        try
        {
            // 第一步：按项目组编码查找项目组。
            var group = await _itemGroupRepository.GetByCodeAsync(condition.RightValue);

            if (group == null)
            {
                // 项目组不存在，配置错误，按不命中处理。
                return false;
            }

            // 第二步：查询项目组明细，判断当前项目是否属于该组。
            var details = await _itemGroupDetailRepository.GetByGroupIdAsync(group.GroupId);

            // 按项目编码匹配，不区分大小写。
            var matched = details.Any(d =>
                string.Equals(d.ItemCode, context.ItemCode, StringComparison.OrdinalIgnoreCase));
            if (matched)
            {
                context.ItemGroupCode = group.GroupCode;
            }

            return matched;
        }
        catch
        {
            // 评估器异常时默认不通过（安全优先）。
            // 异常详情可通过日志框架记录，不影响规则匹配流程。
            return false;
        }
    }
}
