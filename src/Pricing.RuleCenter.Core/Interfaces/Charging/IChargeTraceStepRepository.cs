using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Core.Interfaces.Charging;

/// <summary>
/// 计价追溯步骤仓储接口 —— 折价追溯中心中"计算过程链"的数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_CHARGE_TRACE_STEP。
///
/// 职责边界：
///   - 记录计价引擎在计算过程中的每一个步骤（规则匹配、条件评估、公式执行、限制校验等）。
///   - 支持追溯查询接口展示完整的计算过程链，便于问题排查和审计。
///
/// 追溯链路：
///   计价请求有三条追溯链路：
///   1. 规则变更链：PR_RULE_CHANGE_LOG —— 规则何时被谁修改
///   2. 计算过程链：PR_CHARGE_TRACE_STEP —— 本次计价经历了哪些步骤（本接口管理）
///   3. 最终结果链：PR_CHARGE_DISCOUNT_DETAIL —— 最终折价结果明细
///
/// 写入时机：
///   仅在 confirm（确认计价）时写入，试算（simulate）不写入追溯步骤。
///   每个步骤记录步骤序号、步骤类型、输入参数、输出结果、耗时等信息。
/// </summary>
public interface IChargeTraceStepRepository
{
    /// <summary>
    /// 根据请求ID查询该笔计价请求的所有追溯步骤。
    ///
    /// 使用场景：追溯查询接口（/api/pricing/trace/query）展示某笔请求的完整计算过程。
    /// 返回结果按步骤序号（STEP_NO）排序，还原计算引擎的执行顺序。
    /// </summary>
    /// <param name="requestId">计价请求日志ID（PR_CHARGE_REQUEST_LOG.ID）。</param>
    /// <returns>追溯步骤列表，按步骤序号排序；无记录时返回空列表。</returns>
    Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId);

    /// <summary>
    /// 批量插入追溯步骤记录。
    ///
    /// 调用时机：计价引擎完成计算后，将所有步骤一次性批量写入。
    /// 使用批量插入而非逐条插入，减少数据库交互次数，提升性能。
    /// </summary>
    /// <param name="entities">
    /// 追溯步骤实体列表，每条包含：
    /// - RequestId：关联的计价请求日志ID
    /// - StepNo：步骤序号（排序用）
    /// - StepType：步骤类型（如"RULE_MATCH"、"CONDITION_EVAL"、"FORMULA_EXEC"、"LIMIT_CHECK"）
    /// - StepDetail：步骤详情（JSON 格式，存储输入参数和输出结果）
    /// - ElapsedMs：步骤耗时（毫秒）
    /// </param>
    Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities);
}
