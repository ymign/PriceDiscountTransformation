using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Options;
using Microsoft.Extensions.Options;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 确认计价工作流：正式计价、幂等保护、额度占用和折价明细落库。
/// </summary>
/// <remarks>
/// confirm 是收费链路中唯一会占用限额的入口。事务内完成规则计算、请求日志、追踪步骤、
/// 折价明细和限额占用写入。幂等键为 sourceSystem + businessRequestNo + callType。
/// </remarks>
public sealed class PricingConfirmWorkflow
{
    private readonly PricingItemCalculationRunner _calculationRunner;
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingConfirmationPersistenceService _persistenceService;
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PricingOptions _options;
    private readonly IClock _clock;
    private readonly ILogger<PricingConfirmWorkflow> _logger;

    public PricingConfirmWorkflow(
        PricingItemCalculationRunner calculationRunner,
        IChargeRequestLogRepository requestLogRepository,
        AuthorityPriceChecker authorityPriceChecker,
        PricingConfirmationPersistenceService persistenceService,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        ILimitOccupyRepository limitRepository,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<PricingConfirmWorkflow> logger)
    {
        _calculationRunner = calculationRunner;
        _requestLogRepository = requestLogRepository;
        _authorityPriceChecker = authorityPriceChecker;
        _persistenceService = persistenceService;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _limitRepository = limitRepository;
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行确认计价：校验 → 幂等检查 → 事务内计价+持久化。
    /// </summary>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = ValidateRequestAsync(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "确认计价开始 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        // 事务外快路径幂等检查
        var fingerprint = PricingRequestFingerprintBuilder.BuildConfirmFingerprint(request, items, "CONFIRM");
        var existing = await _requestLogRepository.GetByBusinessKeyAsync(
            request.SourceSystem, request.BusinessRequestNo!, "CONFIRM");
        var existingResponse = TryReadExistingResponse(request, fingerprint, firstItem, existing);
        if (existingResponse is not null)
        {
            return existingResponse;
        }

        // 事务内：锁幂等 → 二次检查 → 计价 → 持久化
        return await ExecuteInTransactionAsync(async () =>
        {
            await LockIdempotencyAsync(request);

            var existingInTransaction = await _requestLogRepository.GetByBusinessKeyAsync(
                request.SourceSystem, request.BusinessRequestNo!, "CONFIRM");
            var transactionResponse = TryReadExistingResponse(request, fingerprint, firstItem, existingInTransaction);
            if (transactionResponse is not null)
            {
                return transactionResponse;
            }

            var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
            using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

            var calculations = await _calculationRunner.RunAsync(request, items, "CONFIRM", shouldLockLimits: true);
            var runtimeTrace = await _runtimePackageTraceResolver.ResolveAsync(calculations);
            var response = await _persistenceService.PersistAsync(new PricingConfirmationPersistenceInput
            {
                Request = request,
                Items = items,
                Calculations = calculations,
                Fingerprint = fingerprint,
                RuntimeTrace = runtimeTrace
            });

            _logger.LogInformation(
                "确认计价成功 请求ID={RequestId}, 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 项目编码={ItemCode}, 最终数量={FinalQty}, 最终金额={FinalAmount}, 是否特殊项目={IsSpecialItem}",
                response.RequestId, request.SourceSystem, request.BusinessRequestNo,
                firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

            return response;
        });
    }

    private IReadOnlyList<PricingCalculateItemRequest> ValidateRequestAsync(PricingCalculateRequest request)
    {
        var items = PricingRequestGuard.GetRequiredItems(request);
        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning("确认计价校验失败：业务请求号为空，来源系统={SourceSystem}", request.SourceSystem);
            PricingRequestGuard.EnsureConfirmRequest(request);
        }

        // 同步等待：AuthorityPriceChecker 只做诊断日志，不阻断流程，此处保持方法签名同步以简化调用链。
        _authorityPriceChecker.CheckAsync(request, items).GetAwaiter().GetResult();
        return items;
    }

    /// <summary>
    /// 事务外或事务内读取已有请求，校验指纹一致性后返回首次响应快照。
    /// </summary>
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

        // 指纹一致说明是同一业务动作重试；不一致说明复用了业务号但参数不同，必须拒绝。
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

        // 读取首次 confirm 保存的响应快照，不能重新计算（规则/限额可能已变化）。
        if (string.IsNullOrWhiteSpace(existingRequest.ResponseJson))
        {
            throw new BizException(
                BizErrorCode.IdempotencyResponseSnapshotInvalid,
                409,
                $"RequestId={existingRequest.RequestId} 的幂等响应快照缺失");
        }

        try
        {
            var response = JsonConvert.DeserializeObject<PricingCalculateResponse>(existingRequest.ResponseJson);
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

    /// <summary>
    /// 在 Oracle 事务内执行操作，异常时自动回滚。
    /// </summary>
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
