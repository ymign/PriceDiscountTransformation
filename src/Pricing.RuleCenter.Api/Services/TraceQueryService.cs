using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class TraceQueryService
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeTraceStepRepository _traceStepRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;

    public TraceQueryService(
        IChargeRequestLogRepository requestLogRepository,
        IChargeTraceStepRepository traceStepRepository,
        IChargeDiscountDetailRepository discountRepository)
    {
        _requestLogRepository = requestLogRepository;
        _traceStepRepository = traceStepRepository;
        _discountRepository = discountRepository;
    }

    public async Task<PagedResponse<TraceQueryResponse>> QueryAsync(TraceQueryRequest request)
    {
        if (request.RequestId.HasValue)
        {
            var log = await _requestLogRepository.GetByIdAsync(request.RequestId.Value);
            if (log is null)
            {
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

        var (items, total) = await _requestLogRepository.GetPagedAsync(
            request.PatientId, request.ItemCode, request.ChargeNo,
            request.StartTime, request.EndTime,
            request.PageIndex, request.PageSize);

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

    private async Task<TraceQueryResponse> BuildTraceDetail(ChargeRequestLog log)
    {
        var steps = await _traceStepRepository.GetByRequestIdAsync(log.RequestId);
        var discounts = await _discountRepository.GetByRequestIdAsync(log.RequestId);

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
