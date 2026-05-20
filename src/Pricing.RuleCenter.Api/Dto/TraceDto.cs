using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 计价追溯查询请求 DTO，用于查询计价请求的全链路追踪记录。
/// </summary>
/// <remarks>
/// <para>
/// 追溯查询是"可追溯性作为架构原则"的核心体现。每笔折价必须可沿三条链路追溯：
/// 规则变更链（PR_RULE_CHANGE_LOG）、计算过程链（PR_CHARGE_TRACE_STEP）、
/// 最终结果链（PR_CHARGE_DISCOUNT_DETAIL）。此 DTO 用于查询计算过程链和最终结果链。
/// </para>
/// <para>
/// 所有筛选条件均为选填，为空时不过滤。多个条件之间按 AND 逻辑组合。
/// 时间范围筛选基于 <see cref="StartTime"/> 和 <see cref="EndTime"/>，
/// 对应 PR_CHARGE_REQUEST_LOG.REQUEST_AT 字段。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/pricing/trace/query</c>。
/// </para>
/// </remarks>
public sealed class TraceQueryRequest
{
    /// <summary>
    /// 计价请求日志主键（选填），对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID。
    /// 已知 RequestId 时可精确查询单次计价请求的全链路记录。
    /// </summary>
    public long? RequestId { get; init; }

    /// <summary>
    /// 患者标识（选填），是限额累计和追溯查询的重要维度。
    /// 查询某患者的所有计价记录时使用。
    /// </summary>
    public string? PatientId { get; init; }

    /// <summary>
    /// 项目编码（选填），精确匹配。
    /// 查询某项目的所有计价记录时使用，对应 HIS 物价主数据中的项目编码。
    /// </summary>
    public string? ItemCode { get; init; }

    /// <summary>
    /// 收费单号（选填），用于与 HIS 落账结果关联。
    /// 收费单号由 HIS 生成，计价中心在 commit 时记录。
    /// </summary>
    public string? ChargeNo { get; init; }

    /// <summary>
    /// 请求时间范围起始（选填），包含此时间点。格式建议 ISO 8601。
    /// 与 <see cref="EndTime"/> 配合使用，筛选 [StartTime, EndTime] 区间内的请求。
    /// </summary>
    public DateTime? StartTime { get; init; }

    /// <summary>
    /// 请求时间范围结束（选填），包含此时间点。格式建议 ISO 8601。
    /// 与 <see cref="StartTime"/> 配合使用。
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// 页码（选填），从 1 开始，默认 1。
    /// </summary>
    public int PageIndex { get; init; } = 1;

    /// <summary>
    /// 每页记录数（选填），默认 20。建议不超过 100 以保证查询性能。
    /// </summary>
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// 计价追溯查询响应 DTO，返回单次计价请求的全链路追踪记录。
/// </summary>
/// <remarks>
/// <para>
/// 追溯响应包含三个维度的信息：
/// 1. 请求概况（<see cref="RequestId"/>、<see cref="CallType"/>、<see cref="BusinessStatus"/> 等）
/// 2. 计算过程（<see cref="Steps"/> — 规则匹配、单位换算、公式计算、限额校验等步骤）
/// 3. 最终结果（<see cref="Discounts"/> — 每个项目的折价明细）
/// </para>
/// <para>
/// 通过 <see cref="RequestId"/> 可关联查询 PR_LIMIT_OCCUPY（限额占用）和
/// PR_CHARGE_REVERSE_LOG（冲销记录）等扩展信息。
/// </para>
/// </remarks>
public sealed class TraceQueryResponse
{
    /// <summary>
    /// 计价请求日志主键，对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID。
    /// 是串联请求、步骤、折价明细和限额占用的核心外键。
    /// </summary>
    public long RequestId { get; init; }

    /// <summary>
    /// 技术请求流水号，调用方传入或服务端生成，用于排查单次 HTTP 调用。
    /// 与 <see cref="RequestId"/> 的区别：RequestNo 是业务流水号，RequestId 是数据库主键。
    /// </summary>
    public string RequestNo { get; init; } = string.Empty;

    /// <summary>
    /// 调用类型，描述本次计价请求的操作类型。
    /// 常见值：SIMULATE（试算，不占额度）、CONFIRM（确认计价，占用额度）、
    /// COMMIT（HIS 结算成功通知）、CANCEL（取消确认，释放额度）、REVERSE（退费/冲销）。
    /// </summary>
    public string CallType { get; init; } = string.Empty;

    /// <summary>
    /// 业务状态，描述请求在状态机中的当前位置。
    /// 常见值：SIMULATED（已试算）、CONFIRM_PENDING（待确认，已占额度）、
    /// CONFIRMED（已落账）、CANCELLED（已取消，已释放额度）、EXPIRED（已过期，后台清理）、
    /// REVERSED（已冲销）。
    /// </summary>
    public string BusinessStatus { get; init; } = string.Empty;

    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度。
    /// </summary>
    public string? PatientId { get; init; }

    /// <summary>
    /// 项目编码，对应 HIS 物价主数据中的项目编码。
    /// </summary>
    public string? ItemCode { get; init; }

    /// <summary>
    /// 项目名称，用于展示和审计说明。
    /// </summary>
    public string? ItemName { get; init; }

    /// <summary>
    /// 调用方录入的原始数量（业务数量，非换算后数量）。
    /// 金额计算前不得随意覆盖此值，必须保留原始输入用于追溯。
    /// 使用 decimal 类型，精度由 NUMBER(18,4) 保证。
    /// </summary>
    public decimal? InputQty { get; init; }

    /// <summary>
    /// 计价中心收到请求的技术时间（服务端时间），用于追溯和超时清理。
    /// 与业务收费时间（由调用方传入）区分：技术时间用于运维排查，业务时间用于规则匹配。
    /// </summary>
    public DateTime RequestAt { get; init; }

    /// <summary>
    /// 请求是否成功落入计价中心处理链路。"Y" 表示成功处理；"N" 表示处理失败。
    /// 失败原因可通过 <see cref="Steps"/> 中的 ERROR 类型步骤查看。
    /// </summary>
    public string IsSuccess { get; init; } = string.Empty;

    /// <summary>
    /// 本次计价的执行步骤列表，按计价链路执行顺序排列。
    /// 包含规则匹配、单位换算、公式计算、限额校验、折价计算等完整计算过程。
    /// 每个步骤记录了输入快照和输出快照，可还原完整的计算过程。
    /// </summary>
    public IReadOnlyList<TraceStepResponse> Steps { get; init; } = Array.Empty<TraceStepResponse>();

    /// <summary>
    /// 本次计价的折价明细列表，每个元素对应一个项目的最终计价结果。
    /// 包含原始数量/金额、最终数量/金额、折价金额等，是计价的最终产出。
    /// </summary>
    public IReadOnlyList<TraceDiscountResponse> Discounts { get; init; } = Array.Empty<TraceDiscountResponse>();
}

/// <summary>
/// 计价追踪步骤响应 DTO，返回单次计价请求中某个执行步骤的详情。
/// </summary>
/// <remarks>
/// <para>
/// 计价引擎的执行链路由多个步骤组成，步骤类型由 DDL 约束为固定枚举。
/// 每个步骤记录了输入快照和输出快照（JSON 格式），可完整还原该步骤的计算过程。
/// </para>
/// <para>
/// 步骤按 <see cref="StepNo"/> 递增排列，对应旧 HIS 兼容计价链路：
/// 规则匹配 → 双单位换算 → 数量限制/时间窗限制/同组互斥 →
/// 公式折价 → TOPPRICE/金额封顶 → 同手术封顶 → 子项加收/附加项目 → 超出部分归零兜底。
/// </para>
/// </remarks>
public sealed class TraceStepResponse
{
    /// <summary>
    /// 步骤序号，按计价链路执行顺序从 1 开始递增。
    /// 序号连续但可能有跳号（某些条件不满足时跳过对应步骤）。
    /// </summary>
    public int StepNo { get; init; }

    /// <summary>
    /// 步骤类型，由 DDL 约束为固定枚举值。
    /// MATCH（规则匹配）、CONVERT（双单位换算）、FORMULA（公式计算）、
    /// LIMIT（数量/金额限制校验）、DISCOUNT（折价计算）、VALIDATE（校验，如权威单价校验）、
    /// ERROR（异常步骤，记录失败原因）。
    /// </summary>
    public string StepType { get; init; } = string.Empty;

    /// <summary>
    /// 步骤说明，以自然语言解释该步骤的执行结果。
    /// 例如："命中规则 RULE_SKIN_AREA_DISCOUNT""公式计算结果：面积=5cm²，单价=100元，折后=400元"、
    /// "日限额校验：已用2次，本次1次，剩余0次，超出部分归零"。
    /// </summary>
    public string? StepDesc { get; init; }

    /// <summary>
    /// 步骤执行前的输入快照（JSON 格式），记录该步骤接收的完整参数。
    /// 用于还原计算过程和排查异常。例如公式计算步骤的输入可能包含：
    /// { "unitPrice": 100.00, "quantity": 5, "params": { "area": 3.5 } }。
    /// </summary>
    public string? InputSnapshot { get; init; }

    /// <summary>
    /// 步骤执行后的输出快照（JSON 格式），记录该步骤的计算结果。
    /// 用于还原计算过程和排查异常。例如公式计算步骤的输出可能包含：
    /// { "calculatedAmount": 350.00, "formula": "unitPrice * area" }。
    /// </summary>
    public string? OutputSnapshot { get; init; }
}

/// <summary>
/// 计价追踪折扣明细响应 DTO，返回单次计价请求中某个项目的最终计价结果。
/// </summary>
/// <remarks>
/// <para>
/// 折扣明细（PR_CHARGE_DISCOUNT_DETAIL 表）是计价的最终产出，记录了每个项目
/// 经过规则匹配、公式计算、限额校验后的最终数量和金额。
/// </para>
/// <para>
/// 金额关系：OriginalAmt（原始金额）- FinalAmt（最终金额）= DiscountAmt（折价金额）。
/// 当折价金额为 0 时表示该项目未享受折价；当 FinalAmt 为 0 时表示该项目完全免费
/// （超出限额部分为 0 元，不是拒单）。
/// </para>
/// <para>
/// 所有金额字段使用 decimal 类型，精度由 NUMBER(18,4) 保证。
/// 最终对外金额保留 2 位小数、四舍五入。
/// </para>
/// </remarks>
public sealed class TraceDiscountResponse
{
    /// <summary>
    /// 折扣明细主键，对应 PR_CHARGE_DISCOUNT_DETAIL.DISCOUNT_ID。
    /// </summary>
    public long DiscountId { get; init; }

    /// <summary>
    /// 项目编码，对应 HIS 物价主数据中的项目编码。
    /// </summary>
    public string? ItemCode { get; init; }

    /// <summary>
    /// 原始数量，保存折价前的业务输入数量（未经过限额截断和公式换算）。
    /// 使用 decimal 类型，精度由 NUMBER(18,4) 保证。
    /// </summary>
    public decimal? OriginalQty { get; init; }

    /// <summary>
    /// 最终可收费数量，经过限额校验和换算后的实际收费数量。
    /// 当 OriginalQty > FinalQty 时，差额部分为超出限额被截断的数量。
    /// 使用 decimal 类型，精度由 NUMBER(18,4) 保证。
    /// </summary>
    public decimal? FinalQty { get; init; }

    /// <summary>
    /// 原始金额，通常为 OriginalQty * 权威单价。未经任何折价和限额处理。
    /// 使用 decimal 类型，精度由 NUMBER(18,4) 保证。
    /// </summary>
    public decimal? OriginalAmt { get; init; }

    /// <summary>
    /// 最终收费金额，经过折价计算和限额校验后的实际收费金额。
    /// 最终对外金额保留 2 位小数、四舍五入。
    /// 使用 decimal 类型，精度由 NUMBER(18,4) 保证。
    /// </summary>
    public decimal? FinalAmt { get; init; }

    /// <summary>
    /// 折价金额，等于 OriginalAmt - FinalAmt。
    /// 为 0 表示未享受折价；等于 OriginalAmt 表示完全免费（超出限额部分为 0 元）。
    /// 使用 decimal 类型，精度由 NUMBER(18,4) 保证。
    /// </summary>
    public decimal? DiscountAmt { get; init; }

    /// <summary>
    /// 折扣明细状态，描述该条明细在生命周期中的位置。
    /// 常见值：PENDING（待 HIS 落账）、CONFIRMED（已落账）、CANCELLED（已取消，已释放额度）、
    /// REVERSED（已冲销，已释放额度）。
    /// </summary>
    public string Status { get; init; } = string.Empty;
}
