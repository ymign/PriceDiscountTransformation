using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 计价追踪查询请求 DTO。
/// </summary>
public sealed class TraceQueryRequest
{
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long? RequestId { get; init; }
    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    public string? PatientId { get; init; }
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }
    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; init; }
    /// <summary>
    /// 请求时间开始筛选条件。
    /// </summary>
    public DateTime? StartTime { get; init; }
    /// <summary>
    /// 请求时间结束筛选条件。
    /// </summary>
    public DateTime? EndTime { get; init; }
    /// <summary>
    /// 页码，从 1 开始。
    /// </summary>
    public int PageIndex { get; init; } = 1;
    /// <summary>
    /// 每页记录数。
    /// </summary>
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// 计价追踪查询响应 DTO。
/// </summary>
public sealed class TraceQueryResponse
{
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; init; }
    /// <summary>
    /// 调用方或服务端生成的技术请求流水号，用于排查单次 HTTP 调用
    /// </summary>
    public string RequestNo { get; init; } = string.Empty;
    /// <summary>
    /// 调用类型，例如 SIMULATE、CONFIRM、COMMIT、CANCEL 或 REVERSE
    /// </summary>
    public string CallType { get; init; } = string.Empty;
    /// <summary>
    /// 业务状态，描述请求在试算、待确认、已落账、取消、过期、冲正等状态机中的位置
    /// </summary>
    public string BusinessStatus { get; init; } = string.Empty;
    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    public string? PatientId { get; init; }
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; init; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    public decimal? InputQty { get; init; }
    /// <summary>
    /// 计价中心收到请求的技术时间
    /// </summary>
    public DateTime RequestAt { get; init; }
    /// <summary>
    /// 请求是否成功落入计价中心处理链路，Y 表示成功
    /// </summary>
    public string IsSuccess { get; init; } = string.Empty;
    /// <summary>
    /// 本次计价的执行步骤。
    /// </summary>
    public IReadOnlyList<TraceStepResponse> Steps { get; init; } = Array.Empty<TraceStepResponse>();
    /// <summary>
    /// 本次计价的折扣明细。
    /// </summary>
    public IReadOnlyList<TraceDiscountResponse> Discounts { get; init; } = Array.Empty<TraceDiscountResponse>();
}

/// <summary>
/// 计价追踪步骤响应 DTO。
/// </summary>
public sealed class TraceStepResponse
{
    /// <summary>
    /// 步骤序号，按计价链路执行顺序递增
    /// </summary>
    public int StepNo { get; init; }
    /// <summary>
    /// 步骤类型，只使用 DDL 允许的 MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR
    /// </summary>
    public string StepType { get; init; } = string.Empty;
    /// <summary>
    /// 步骤说明，解释命中规则、限制口径或折价原因
    /// </summary>
    public string? StepDesc { get; init; }
    /// <summary>
    /// 步骤执行前的输入快照
    /// </summary>
    public string? InputSnapshot { get; init; }
    /// <summary>
    /// 步骤执行后的输出快照
    /// </summary>
    public string? OutputSnapshot { get; init; }
}

/// <summary>
/// 计价追踪折扣明细响应 DTO。
/// </summary>
public sealed class TraceDiscountResponse
{
    /// <summary>
    /// 折扣明细主键。
    /// </summary>
    public long DiscountId { get; init; }
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }
    /// <summary>
    /// 原始数量，保存折价前的业务输入
    /// </summary>
    public decimal? OriginalQty { get; init; }
    /// <summary>
    /// 最终可收费数量
    /// </summary>
    public decimal? FinalQty { get; init; }
    /// <summary>
    /// 原始金额，通常为原始数量乘权威单价
    /// </summary>
    public decimal? OriginalAmt { get; init; }
    /// <summary>
    /// 最终收费金额
    /// </summary>
    public decimal? FinalAmt { get; init; }
    /// <summary>
    /// 折价金额，等于原始金额减最终金额
    /// </summary>
    public decimal? DiscountAmt { get; init; }
    /// <summary>
    /// 当前记录状态，具体含义由所在表的状态机定义
    /// </summary>
    public string Status { get; init; } = string.Empty;
}
