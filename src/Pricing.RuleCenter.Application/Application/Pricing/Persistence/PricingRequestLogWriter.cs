using Pricing.RuleCenter.Application.RuntimePackages;
using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 保存计价请求日志所需的输入。
/// </summary>
/// <remarks>
/// 请求日志是整条计价追溯链的主表。它既保存原始请求 JSON，也保存响应快照和运行包版本，
/// 后续幂等重试、commit/cancel/reverse 和追溯查询都依赖该记录。
/// </remarks>
internal sealed record RequestLogSaveInput
{
    /// <summary>原始计价请求。</summary>
    public PricingCalculateRequest Request { get; init; } = null!;

    /// <summary>已校验的费用明细集合。</summary>
    public IReadOnlyList<PricingCalculateItemRequest> Items { get; init; } =
        Array.Empty<PricingCalculateItemRequest>();

    /// <summary>每条费用明细与引擎结果的配对集合。</summary>
    public IReadOnlyList<ItemPricingCalculation> Calculations { get; init; } =
        Array.Empty<ItemPricingCalculation>();

    /// <summary>调用类型，例如 SIMULATE、CONFIRM、REVERSE。</summary>
    public string CallType { get; init; } = string.Empty;

    /// <summary>业务状态，例如 SIMULATED、CONFIRM_PENDING。</summary>
    public string BusinessStatus { get; init; } = string.Empty;

    /// <summary>confirm 幂等请求指纹；simulate 可为空。</summary>
    public string? Fingerprint { get; init; }

    /// <summary>运行包追溯解析结果。</summary>
    public RuntimePackageTraceResolution? RuntimeTrace { get; init; }
}

/// <summary>
/// 计价请求日志写入器，负责保存请求主表和响应快照。
/// </summary>
public sealed class PricingRequestLogWriter
{
    /// <summary>
    /// 请求日志仓储。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;
    /// <summary>
    /// 统一时钟。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价请求日志写入器。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingRequestLogWriter(
        IChargeRequestLogRepository requestLogRepository,
        IClock clock)
    {
        _requestLogRepository = requestLogRepository;
        _clock = clock;
    }

    internal async Task<ChargeRequest> SaveAsync(RequestLogSaveInput input)
    {
        // 请求日志先保存计算结果的临时 JSON，后续 workflow 会调用 SaveResponseJsonAsync 覆盖成完整响应快照。
        // 这样即使响应构建前出现异常，也能在事务回滚前保留清晰的写入边界。
        var request = input.Request;
        var items = input.Items;
        var calculations = input.Calculations;
        var now = _clock.Now;
        var log = new ChargeRequest
        {
            RequestNo = NormalizeString(request.RequestNo) ?? $"REQ-{now:yyyyMMddHHmmssfff}",
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            RequestFingerprint = input.Fingerprint,
            TraceId = PricingTraceIdGenerator.Build(input.CallType, request.RequestNo, request.BusinessRequestNo, now),
            CallType = input.CallType,
            BusinessStatus = input.BusinessStatus,
            SourceSystem = request.SourceSystem.Trim(),
            SourceTerminal = NormalizeString(request.SourceTerminal),
            PatientId = request.PatientId.Trim(),
            VisitId = NormalizeString(request.VisitId),
            ChargeScene = NormalizeString(request.ChargeScene),
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = items.Count == 1 ? NormalizeString(items[0].ChargeDetailNo) : null,
            // 单明细时把项目字段冗余到主表，方便追溯查询按项目快速过滤；多明细时项目放在明细表和响应 Items 中。
            ItemCode = items.Count == 1 ? items[0].ItemCode.Trim() : null,
            ItemName = items.Count == 1 ? NormalizeString(items[0].ItemName) : null,
            InputQty = items.Count == 1 ? items[0].InputQty : null,
            InputUnit = items.Count == 1 ? NormalizeString(items[0].Unit) : null,
            BodyPartCode = items.Count == 1 ? NormalizeString(items[0].BodyPartCode) : null,
            BusinessChargeTime = items.Count == 1
                ? items[0].BusinessChargeTime ?? request.BusinessChargeTime
                : request.BusinessChargeTime,
            RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
            RuntimePackageVersion = input.RuntimeTrace?.RuntimePackageVersion,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(calculations.Select(c => c.Result).ToList()),
            RequestAt = now,
            ResponseAt = now,
            IsSuccess = EnableFlag.Yes
        };

        await _requestLogRepository.InsertAsync(log);
        return log;
    }

    internal async Task SaveResponseJsonAsync(ChargeRequest log, PricingCalculateResponse response)
    {
        // ResponseJson 是 confirm 幂等重试的事实来源，必须保存完整响应而不是只保存核心引擎结果。
        log.ResponseJson = JsonConvert.SerializeObject(response);
        log.ResponseAt = _clock.Now;
        await _requestLogRepository.UpdateAsync(log);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
