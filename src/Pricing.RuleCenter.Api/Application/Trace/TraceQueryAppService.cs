using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Api.Application.Trace;

/// <summary>
/// 计价追踪查询服务，负责把请求日志、执行步骤和折扣明细组装成可查看的追踪结果。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：该服务只读取已经落库的审计数据，不重新执行规则，也不修正历史结果。
/// 这样可以保证追踪页面展示的是当次计价实际发生过的状态，而不是当前规则重新计算后的结果。
/// </para>
/// <para>
/// 三条追溯链路：
/// 1. 规则变更链 — 通过 RequestId 关联到规则版本快照，还原当时生效的条件和动作
/// 2. 计算过程链 — PR_CHARGE_TRACE_STEP 记录每一步匹配、换算、公式、限额的输入输出
/// 3. 最终结果链 — PR_CHARGE_DISCOUNT_DETAIL 记录每个项目的最终数量、金额和折扣差额
/// </para>
/// <para>
/// 查询模式：支持两种模式——按 RequestId 精确查询（从接口返回或日志跳转），
/// 和按业务条件分页查询（患者、项目、收费单号、时间范围）。
/// </para>
/// </remarks>
public sealed class TraceQueryAppService
{
    /// <summary>
    /// 请求日志仓储，提供 PR_CHARGE_REQUEST_LOG 表的单条查询和分页查询。
    /// 请求日志是追踪的入口，包含患者、项目、调用类型和业务状态等主维度。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 执行步骤仓储，用于读取 PR_CHARGE_TRACE_STEP 表中规则匹配、动作执行和状态推进的步骤快照。
    /// 步骤解释"为什么这样计算"，是计价过程链的核心。
    /// </summary>
    private readonly IChargeTraceStepRepository _traceStepRepository;

    /// <summary>
    /// 折扣明细仓储，用于读取 PR_CHARGE_DISCOUNT_DETAIL 表中最终数量、金额和折扣差额。
    /// 折扣明细解释"最终变成了什么"，是结果链的核心。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;

    /// <summary>
    /// 初始化计价追踪查询服务。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储，提供计价请求主记录查询。</param>
    /// <param name="traceStepRepository">执行步骤仓储，提供规则匹配和计算过程的步骤快照。</param>
    /// <param name="discountRepository">折扣明细仓储，提供最终计价结果的数量和金额明细。</param>
    public TraceQueryAppService(
        IChargeRequestLogRepository requestLogRepository,
        IChargeTraceStepRepository traceStepRepository,
        IChargeDiscountDetailRepository discountRepository)
    {
        _requestLogRepository = requestLogRepository;
        _traceStepRepository = traceStepRepository;
        _discountRepository = discountRepository;
    }

    /// <summary>
    /// 查询计价追踪记录。
    /// </summary>
    /// <param name="request">追踪查询条件；指定 RequestId 时走精确查询，否则走分页列表查询。</param>
    /// <returns>分页包装后的追踪结果，每条结果包含主请求、步骤和折扣明细。</returns>
    public async Task<PagedResponse<TraceQueryResponse>> QueryAsync(TraceQueryRequest request)
    {
        // ========== 第一阶段：RequestId 精确查询优先 ==========
        // 精确查询用于从接口返回或日志中直接跳转到某次计价。此时忽略其他筛选条件，避免多条件组合导致查不到目标记录。
        if (request.RequestId.HasValue)
        {
            var log = await _requestLogRepository.GetByIdAsync(request.RequestId.Value);
            if (log is null)
            {
                // 找不到主请求时返回空分页，而不是抛异常；追踪页面可以自然展示"无数据"。
                return new PagedResponse<TraceQueryResponse> { PageIndex = 1, PageSize = 1 };
            }

            var detail = await BuildTraceDetail(log);
            return new PagedResponse<TraceQueryResponse>
            {
                Items = new[] { detail },
                Total = 1,
                PageIndex = 1,
                PageSize = 1
            };
        }

        // ========== 第二阶段：按业务条件分页查询主请求 ==========
        // 分页只针对请求主表做，步骤和折扣明细随后按请求逐条补齐，避免明细表 join 放大分页结果。
        var (items, total) = await _requestLogRepository.GetPagedAsync(
            request.PatientId, request.ItemCode, request.ChargeNo,
            request.StartTime, request.EndTime,
            request.PageIndex, request.PageSize);

        // ========== 第三阶段：组装每条追踪明细 ==========
        // 这里逐条读取明细，换取返回结构清晰；追踪查询通常不是高频交易路径，不影响计价主链路性能。
        var results = new List<TraceQueryResponse>();
        foreach (var item in items)
        {
            results.Add(await BuildTraceDetail(item));
        }

        return new PagedResponse<TraceQueryResponse>
        {
            Items = results,
            Total = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 根据请求主记录组装完整追踪明细。
    /// </summary>
    /// <remarks>
    /// 该方法从三张表分别读取数据并组装：请求主表提供主维度，步骤表提供计算过程，
    /// 折扣表提供最终结果。三张表通过 RequestId 关联，不使用 JOIN 查询，
    /// 因为步骤和明细的数量级通常很小（单次计价一般不超过 20 步），多次查询的开销可接受。
    /// </remarks>
    /// <param name="log">请求主记录，提供患者、项目、调用类型和业务状态等主维度。</param>
    /// <returns>包含步骤快照和折扣明细的追踪响应，用于前端追踪页面完整展示。</returns>
    private async Task<TraceQueryResponse> BuildTraceDetail(ChargeRequest log)
    {
        // ========== 第一阶段：读取关联明细 ==========
        // 步骤表解释"为什么这样计算"，折扣表解释"最终数量和金额变成了什么"。
        // 两张表都按 RequestId 查询，数据量通常很小（单次计价步骤 < 20，折扣明细 < 10）。
        var steps = await _traceStepRepository.GetByRequestIdAsync(log.RequestId);
        var discounts = await _discountRepository.GetByRequestIdAsync(log.RequestId);

        // ========== 第二阶段：保持历史快照原样返回 ==========
        // InputSnapshot/OutputSnapshot 是计价时落库的 JSON 快照，不在查询层重新解析或转换。
        // 原因：如果当前 DTO 结构发生变化（如字段重命名），重新反序列化会破坏历史记录可读性。
        // 前端追踪页面直接展示原始 JSON 或按快照版本号选择解析策略。
        return new TraceQueryResponse
        {
            RequestId = log.RequestId,
            RequestNo = log.RequestNo,
            CallType = log.CallType,
            BusinessStatus = log.BusinessStatus,
            PatientId = log.PatientId,
            ItemCode = log.ItemCode,
            ItemName = log.ItemName,
            InputQty = log.InputQty,
            RequestAt = log.RequestAt,
            IsSuccess = log.IsSuccess,
            Steps = steps.Select(s => new TraceStepResponse
            {
                StepNo = s.StepNo,
                StepType = s.StepType,
                StepDesc = s.StepDesc,
                InputSnapshot = s.InputSnapshot,
                OutputSnapshot = s.OutputSnapshot
            }).ToList(),
            Discounts = discounts.Select(d => new TraceDiscountResponse
            {
                DiscountId = d.DiscountId,
                ItemCode = d.ItemCode,
                OriginalQty = d.OriginalQty,
                FinalQty = d.FinalQty,
                OriginalAmt = d.OriginalAmt,
                FinalAmt = d.FinalAmt,
                DiscountAmt = d.DiscountAmt,
                Status = d.Status
            }).ToList()
        };
    }
}


