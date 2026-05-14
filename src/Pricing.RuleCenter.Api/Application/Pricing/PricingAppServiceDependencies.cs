using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;

namespace Pricing.RuleCenter.Api.Application.Pricing;

/// <summary>
/// 计价应用服务的计算侧依赖集合。
/// </summary>
/// <remarks>
/// 该类型只聚合参与"计算前后判定"的依赖，不包含状态写入仓储。这样既能降低
/// <see cref="PricingAppService"/> 构造函数参数数量，又不会把所有依赖混成一个不透明对象。
/// </remarks>
public sealed class PricingAppCalculationDependencies
{
    /// <summary>
    /// 初始化计价计算侧依赖。
    /// </summary>
    /// <param name="engine">计价引擎，负责规则匹配和动作执行。</param>
    /// <param name="headerRepository">规则头仓储，用于 special-flag 查询。</param>
    /// <param name="priceMasterRepository">权威物价仓储，用于校验渠道传入单价。</param>
    public PricingAppCalculationDependencies(
        IPricingEngine engine,
        IRuleHeaderRepository headerRepository,
        IPriceMasterRepository priceMasterRepository)
    {
        Engine = engine;
        HeaderRepository = headerRepository;
        PriceMasterRepository = priceMasterRepository;
    }

    /// <summary>
    /// 计价引擎，负责规则匹配、动作执行和计算结果生成。
    /// </summary>
    public IPricingEngine Engine { get; }

    /// <summary>
    /// 规则头仓储，用于判断项目是否存在已发布特殊规则。
    /// </summary>
    public IRuleHeaderRepository HeaderRepository { get; }

    /// <summary>
    /// 权威物价仓储，用于 confirm/simulate 前的单价强校验。
    /// </summary>
    public IPriceMasterRepository PriceMasterRepository { get; }
}

/// <summary>
/// 计价应用服务的持久化侧仓储集合。
/// </summary>
/// <remarks>
/// 该类型聚合请求日志、折价明细、步骤日志、限额占用和冲正日志五类状态仓储。
/// 它们共同服务同一条资金状态链路，放在一个依赖包中比散落在服务构造函数里更容易审查事务边界。
/// </remarks>
public sealed class PricingAppPersistenceRepositories
{
    /// <summary>
    /// 初始化计价持久化侧仓储集合。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="discountRepository">折价明细仓储。</param>
    /// <param name="traceStepRepository">计价步骤仓储。</param>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="reverseLogRepository">冲正日志仓储。</param>
    public PricingAppPersistenceRepositories(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        IChargeTraceStepRepository traceStepRepository,
        ILimitOccupyRepository limitRepository,
        IChargeReverseLogRepository reverseLogRepository)
    {
        RequestLogRepository = requestLogRepository;
        DiscountRepository = discountRepository;
        TraceStepRepository = traceStepRepository;
        LimitRepository = limitRepository;
        ReverseLogRepository = reverseLogRepository;
    }

    /// <summary>
    /// 计价请求日志仓储，保存请求快照、响应快照、幂等键和业务状态。
    /// </summary>
    public IChargeRequestLogRepository RequestLogRepository { get; }

    /// <summary>
    /// 折价明细仓储，保存最终落账明细并参与 commit/cancel/reverse 状态同步。
    /// </summary>
    public IChargeDiscountDetailRepository DiscountRepository { get; }

    /// <summary>
    /// 计价步骤仓储，保存规则匹配和动作执行过程。
    /// </summary>
    public IChargeTraceStepRepository TraceStepRepository { get; }

    /// <summary>
    /// 限额占用仓储，负责额度累计、锁定和状态推进。
    /// </summary>
    public ILimitOccupyRepository LimitRepository { get; }

    /// <summary>
    /// 冲正日志仓储，保存退费和撤销流水。
    /// </summary>
    public IChargeReverseLogRepository ReverseLogRepository { get; }
}

