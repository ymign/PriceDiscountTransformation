using System.Text.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Microsoft.Extensions.Options;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 确认计价工作流：正式计价、幂等保护、额度占用和折价明细落库。
/// </summary>
public sealed class PricingConfirmWorkflow
{
    private readonly IPricingEngine _engine;
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingConfirmationPersistenceService _persistenceService;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PricingOptions _options;
    private readonly IClock _clock;
    private readonly ILogger<PricingConfirmWorkflow> _logger;

    public PricingConfirmWorkflow(
        IPricingEngine engine,
        IChargeRequestLogRepository requestLogRepository,
        AuthorityPriceChecker authorityPriceChecker,
        PricingConfirmationPersistenceService persistenceService,
        ILimitOccupyRepository limitRepository,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<PricingConfirmWorkflow> logger)
    {
        _engine = engine;
        _requestLogRepository = requestLogRepository;
        _authorityPriceChecker = authorityPriceChecker;
        _persistenceService = persistenceService;
        _limitRepository = limitRepository;
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行确认计价：校验 → 幂等检查 → 事务内逐条计价+持久化。
    /// </summary>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = await ValidateRequestAsync(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "确认计价开始 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        var fingerprint = PricingRequestFingerprintBuilder.BuildConfirmFingerprint(request, items, "CONFIRM");
        var existingResponse = await TryReadExistingResponseAsync(request, fingerprint, firstItem);
        if (existingResponse is not null)
        {
            return existingResponse;
        }

        return await ExecuteInTransactionAsync(() =>
            ExecuteConfirmInTransactionAsync(request, items, fingerprint, firstItem));
    }

    private async Task<IReadOnlyList<PricingCalculateItemRequest>> ValidateRequestAsync(PricingCalculateRequest request)
    {
        var items = PricingRequestGuard.GetRequiredItems(request);
        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning("确认计价校验失败：业务请求号为空，来源系统={SourceSystem}", request.SourceSystem);
            PricingRequestGuard.EnsureConfirmRequest(request);
        }

        await _authorityPriceChecker.CheckAsync(request, items);
        return items;
    }

    private async Task<PricingCalculateResponse> ExecuteConfirmInTransactionAsync(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        string fingerprint,
        PricingCalculateItemRequest firstItem)
    {
        await LockIdempotencyAsync(request);

        var existingResponse = await TryReadExistingResponseAsync(request, fingerprint, firstItem);
        if (existingResponse is not null)
        {
            return existingResponse;
        }

        var calculations = await CalculateItemsAsync(request, items);
        var response = await _persistenceService.PersistAsync(new PricingConfirmationPersistenceInput
        {
            Request = request,
            Items = items,
            Calculations = calculations,
            Fingerprint = fingerprint
        });

        _logger.LogInformation(
            "确认计价成功 请求ID={RequestId}, 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 项目编码={ItemCode}, 最终数量={FinalQty}, 最终金额={FinalAmount}, 是否特殊项目={IsSpecialItem}",
            response.RequestId, request.SourceSystem, request.BusinessRequestNo,
            firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

        return response;
    }

    private async Task<IReadOnlyList<ItemPricingCalculation>> CalculateItemsAsync(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items)
    {
        // 逐条明细计价，共享请求内累计状态（同组互斥、同手术封顶等）。
        var sharedState = new RequestSharedPricingState();
        var calculations = new List<ItemPricingCalculation>(items.Count);
        foreach (var item in items)
        {
            var context = PricingContextFactory.Create(new PricingContextBuildInput
            {
                Request = request,
                Item = item,
                CallType = "CONFIRM",
                ShouldLockLimits = true,
                RequestSharedState = sharedState
            });
            var result = await _engine.CalculateAsync(context);
            sharedState.Accumulate(result, context);
            calculations.Add(new ItemPricingCalculation(item, result));
        }

        return calculations;
    }

    private async Task<PricingCalculateResponse?> TryReadExistingResponseAsync(
        PricingCalculateRequest request,
        string fingerprint,
        PricingCalculateItemRequest firstItem)
    {
        var existingRequest = await _requestLogRepository.GetByBusinessKeyAsync(
            request.SourceSystem,
            request.BusinessRequestNo!,
            "CONFIRM");
        return TryReadExistingResponse(request, fingerprint, firstItem, existingRequest);
    }

    private PricingCalculateResponse? TryReadExistingResponse(
        PricingCalculateRequest request,
        string fingerprint,
        PricingCalculateItemRequest firstItem,
        ChargeRequest? existingRequest)
    {
        if (existingRequest is null)
        {
            return null;
        }

        if (!string.Equals(existingRequest.RequestFingerprint, fingerprint, StringComparison.Ordinal))
        {
            throw new BizException(
                BizErrorCode.IdempotencyConflict,
                409,
                $"BusinessRequestNo={request.BusinessRequestNo} 已存在，但本次参数与首次请求不一致");
        }

        _logger.LogInformation(
            "确认计价幂等命中 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 请求ID={RequestId}, 项目编码={ItemCode}, 原状态={Status}",
            request.SourceSystem, request.BusinessRequestNo, existingRequest.RequestId, firstItem.ItemCode, existingRequest.BusinessStatus);

        if (string.IsNullOrWhiteSpace(existingRequest.ResponseJson))
        {
            throw new BizException(
                BizErrorCode.IdempotencyResponseSnapshotInvalid,
                409,
                $"RequestId={existingRequest.RequestId} 的幂等响应快照缺失");
        }

        try
        {
            var response = RuleCenterJsonSerializer.Deserialize<PricingCalculateResponse>(existingRequest.ResponseJson);
            if (response is null || response.RequestId <= 0)
            {
                var normalizedLegacyJson =
                    RuleCenterJsonSerializer.RewritePropertyNamesToSnakeCase(existingRequest.ResponseJson);
                response = RuleCenterJsonSerializer.Deserialize<PricingCalculateResponse>(normalizedLegacyJson);
            }

            if (response is not null)
            {
                return response;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "幂等响应快照解析失败 请求ID={RequestId}", existingRequest.RequestId);
        }

        throw new BizException(
            BizErrorCode.IdempotencyResponseSnapshotInvalid,
            409,
            $"RequestId={existingRequest.RequestId} 的幂等响应快照不可解析");
    }

    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        try
        {
            await _unitOfWork.BeginAsync();
            var result = await action();
            await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private Task LockIdempotencyAsync(PricingCalculateRequest request)
    {
        return _limitRepository.EnsureAndLockAsync(new[]
        {
            PricingLockKeyBuilder.BuildIdempotencyLockKey(request.SourceSystem, request.BusinessRequestNo!, "CONFIRM")
        });
    }
}
