using Newtonsoft.Json;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class PricingApiService
{
    private readonly IPricingEngine _engine;
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly ILogger<PricingApiService> _logger;

    public PricingApiService(
        IPricingEngine engine,
        IRuleHeaderRepository headerRepository,
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        ILogger<PricingApiService> logger)
    {
        _engine = engine;
        _headerRepository = headerRepository;
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _logger = logger;
    }

    public async Task<PricingCalculateResponse> SimulateAsync(PricingCalculateRequest request)
    {
        var context = BuildContext(request);
        var result = await _engine.CalculateAsync(context);

        var requestLog = await SaveRequestLog(request, result, "SIMULATE", "SIMULATED");

        return BuildResponse(requestLog.RequestId, result);
    }

    public async Task<PricingCalculateResponse> ConfirmAsync(PricingCalculateRequest request)
    {
        var fingerprint = BuildFingerprint(request);
        var existing = await _requestLogRepository.GetByFingerprintAsync(fingerprint);
        if (existing is not null && existing.BusinessStatus == "CONFIRMED")
        {
            _logger.LogInformation("幂等命中 Fingerprint={Fingerprint}, RequestId={RequestId}",
                fingerprint, existing.RequestId);
            var details = await _discountRepository.GetByRequestIdAsync(existing.RequestId);
            return BuildIdempotentResponse(existing, details);
        }

        var context = BuildContext(request);
        var result = await _engine.CalculateAsync(context);

        var requestLog = await SaveRequestLog(request, result, "CONFIRM", "CONFIRMED");
        requestLog.RequestFingerprint = fingerprint;
        await _requestLogRepository.UpdateAsync(requestLog);

        if (result.IsSpecialItem)
        {
            await SaveDiscountDetail(requestLog.RequestId, request, result);
        }

        return BuildResponse(requestLog.RequestId, result);
    }

    public async Task CommitAsync(PricingCommitRequest request)
    {
        var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
            ?? throw new KeyNotFoundException($"请求不存在: {request.RequestId}");

        if (log.BusinessStatus != "CONFIRMED")
        {
            throw new InvalidOperationException(
                $"只有CONFIRMED状态可以COMMIT, 当前: {log.BusinessStatus}");
        }

        log.BusinessStatus = "COMMITTED";
        log.ChargeNo = request.ChargeNo;
        log.ResponseAt = DateTime.Now;
        await _requestLogRepository.UpdateAsync(log);

        await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, "COMMITTED");
        await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, "COMMITTED");

        _logger.LogInformation("COMMIT RequestId={RequestId}", request.RequestId);
    }

    public async Task CancelAsync(PricingCancelRequest request)
    {
        var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
            ?? throw new KeyNotFoundException($"请求不存在: {request.RequestId}");

        if (log.BusinessStatus != "CONFIRMED")
        {
            throw new InvalidOperationException(
                $"只有CONFIRMED状态可以CANCEL, 当前: {log.BusinessStatus}");
        }

        log.BusinessStatus = "CANCELLED";
        log.ResponseAt = DateTime.Now;
        await _requestLogRepository.UpdateAsync(log);

        await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CANCELLED");
        await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CANCELLED");

        _logger.LogInformation("CANCEL RequestId={RequestId}", request.RequestId);
    }

    public async Task ReverseAsync(PricingReverseRequest request)
    {
        var log = await _requestLogRepository.GetByIdAsync(request.OriginalRequestId)
            ?? throw new KeyNotFoundException($"原请求不存在: {request.OriginalRequestId}");

        if (log.BusinessStatus != "COMMITTED")
        {
            throw new InvalidOperationException(
                $"只有COMMITTED状态可以REVERSE, 当前: {log.BusinessStatus}");
        }

        log.BusinessStatus = "REVERSED";
        log.ResponseAt = DateTime.Now;
        await _requestLogRepository.UpdateAsync(log);

        await _discountRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, "REVERSED");
        await _limitRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, "REVERSED");

        _logger.LogInformation("REVERSE OriginalRequestId={OriginalRequestId}", request.OriginalRequestId);
    }

    public async Task<SpecialFlagResponse> GetSpecialFlagAsync(string itemCode)
    {
        var rules = await _headerRepository.GetByItemCodeAsync(itemCode);
        var published = rules.Where(r => r.Status == "PUBLISHED" && r.IsEnabled == "Y").ToList();

        return new SpecialFlagResponse
        {
            ItemCode = itemCode,
            IsSpecial = published.Count > 0,
            RuleCount = published.Count
        };
    }

    private static PricingContext BuildContext(PricingCalculateRequest request)
    {
        return new PricingContext
        {
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            InputQty = request.InputQty,
            Unit = request.Unit,
            UnitPrice = request.UnitPrice,
            BodyPartCode = request.BodyPartCode,
            ChargeScene = request.ChargeScene,
            BusinessChargeTime = request.BusinessChargeTime,
            SourceSystem = request.SourceSystem,
            ChargeNo = request.ChargeNo,
            BusinessRequestNo = request.BusinessRequestNo,
            PricingParts = request.PricingParts?.Select(p => new PricingPartItem
            {
                PartCode = p.PartCode,
                PartName = p.PartName,
                Qty = p.Qty,
                Area = p.Area
            }).ToList()
        };
    }

    private async Task<ChargeRequestLog> SaveRequestLog(
        PricingCalculateRequest request, PricingResult result,
        string callType, string businessStatus)
    {
        var log = new ChargeRequestLog
        {
            RequestNo = $"REQ-{DateTime.Now:yyyyMMddHHmmssfff}",
            BusinessRequestNo = request.BusinessRequestNo,
            CallType = callType,
            BusinessStatus = businessStatus,
            SourceSystem = request.SourceSystem,
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ChargeScene = request.ChargeScene,
            ChargeNo = request.ChargeNo,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            InputQty = request.InputQty,
            InputUnit = request.Unit,
            BodyPartCode = request.BodyPartCode,
            BusinessChargeTime = request.BusinessChargeTime,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(result),
            RequestAt = DateTime.Now,
            ResponseAt = DateTime.Now,
            IsSuccess = "Y"
        };

        await _requestLogRepository.InsertAsync(log);
        return log;
    }

    private async Task SaveDiscountDetail(
        long requestId, PricingCalculateRequest request, PricingResult result)
    {
        var detail = new ChargeDiscountDetail
        {
            RequestId = requestId,
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            OriginalQty = request.InputQty,
            FinalQty = result.FinalQty,
            UnitPrice = result.UnitPrice,
            OriginalAmt = request.UnitPrice * request.InputQty,
            CalculatedAmt = result.FinalAmount,
            FinalAmt = result.FinalAmount,
            DiscountAmt = result.DiscountAmount,
            Status = "CONFIRMED",
            OccurredAt = DateTime.Now
        };

        await _discountRepository.InsertAsync(detail);
    }

    private static string BuildFingerprint(PricingCalculateRequest request)
    {
        return $"{request.SourceSystem}:{request.BusinessRequestNo}:CONFIRM";
    }

    private static PricingCalculateResponse BuildResponse(long requestId, PricingResult result)
    {
        return new PricingCalculateResponse
        {
            RequestId = requestId,
            IsSpecialItem = result.IsSpecialItem,
            InputQty = result.InputQty,
            FinalQty = result.FinalQty,
            UnitPrice = result.UnitPrice,
            FinalAmount = result.FinalAmount,
            DiscountAmount = result.DiscountAmount,
            TraceSteps = result.TraceSteps.Select(s => new PricingTraceStepResponse
            {
                StepNo = s.StepNo,
                StepType = s.StepType,
                StepDesc = s.StepDesc,
                InputValue = s.InputValue,
                OutputValue = s.OutputValue
            }).ToList(),
            MatchedRuleIds = result.MatchedRuleIds
        };
    }

    private static PricingCalculateResponse BuildIdempotentResponse(
        ChargeRequestLog log, IReadOnlyList<ChargeDiscountDetail> details)
    {
        var detail = details.FirstOrDefault();
        return new PricingCalculateResponse
        {
            RequestId = log.RequestId,
            IsSpecialItem = true,
            InputQty = log.InputQty ?? 0,
            FinalQty = detail?.FinalQty ?? 0,
            UnitPrice = detail?.UnitPrice ?? 0,
            FinalAmount = detail?.FinalAmt ?? 0,
            DiscountAmount = detail?.DiscountAmt ?? 0
        };
    }
}
