using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageActivationService
{
    private readonly IRuntimePackageRepository _runtimePackageRepository;
    private readonly IRuntimePackageStateRepository _runtimePackageStateRepository;
    private readonly ICacheVersionRepository _cacheVersionRepository;
    private readonly IRuleCacheInvalidationOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public RuntimePackageActivationService(
        IRuntimePackageRepository runtimePackageRepository,
        IRuntimePackageStateRepository runtimePackageStateRepository,
        ICacheVersionRepository cacheVersionRepository,
        IRuleCacheInvalidationOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _runtimePackageRepository = runtimePackageRepository;
        _runtimePackageStateRepository = runtimePackageStateRepository;
        _cacheVersionRepository = cacheVersionRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public Task<RuntimePackage> ActivateAsync(long packageId, string activatedBy) =>
        SwitchActivePackageAsync(packageId, activatedBy, "ACTIVATE", allowBuiltOnly: true);

    protected internal async Task<RuntimePackage> SwitchActivePackageAsync(
        long packageId,
        string operatedBy,
        string operationType,
        bool allowBuiltOnly)
    {
        await _unitOfWork.BeginAsync();
        try
        {
            var state = await _runtimePackageStateRepository.GetActiveForUpdateAsync()
                ?? new RuntimePackageState { StateCode = RuntimePackageStateCodes.Active };
            var target = await _runtimePackageRepository.GetByIdAsync(packageId)
                ?? throw new BizException(BizErrorCode.RuntimePackageNotFound, 404, $"运行时包不存在: {packageId}");

            if (allowBuiltOnly &&
                !string.Equals(target.PackageStatus, RuntimePackageStatusCodes.Built, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.RuntimePackageStatusNotAllowed,
                    409,
                    $"运行时包 {packageId} 当前状态 {target.PackageStatus} 不能激活。");
            }

            RuntimePackage? previous = null;
            if (state.ActivePackageId > 0)
            {
                previous = await _runtimePackageRepository.GetByIdAsync(state.ActivePackageId);
            }

            var now = _clock.Now;
            if (previous is not null && previous.PackageId != target.PackageId)
            {
                previous.PackageStatus = RuntimePackageStatusCodes.Superseded;
                await _runtimePackageRepository.UpdateAsync(previous);
            }

            target.PackageStatus = RuntimePackageStatusCodes.Active;
            target.ActivatedBy = operatedBy.Trim();
            target.ActivatedAt = now;
            await _runtimePackageRepository.UpdateAsync(target);

            state.ActivePackageId = target.PackageId;
            state.ActivePackageVersion = target.PackageVersion;
            state.UpdatedAt = now;
            state.UpdatedBy = operatedBy.Trim();
            await _runtimePackageStateRepository.UpsertAsync(state);

            await _cacheVersionRepository.IncreaseVersionAsync(CacheVersionSynchronizer.EffectiveRulesScope);
            await _cacheVersionRepository.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope);
            await InsertOutboxAsync(target.PackageId, operationType, CacheVersionSynchronizer.EffectiveRulesScope, now);
            await InsertOutboxAsync(target.PackageId, operationType, CacheVersionSynchronizer.ActionTypeOrderScope, now);

            await _unitOfWork.CommitAsync();
            return target;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private Task<long> InsertOutboxAsync(long packageId, string operationType, string cacheScope, DateTime now)
    {
        return _outboxRepository.InsertAsync(new RuleCacheInvalidationOutbox
        {
            CacheScope = cacheScope,
            OperationType = operationType,
            RuleId = packageId,
            VersionNo = null,
            Status = CacheInvalidationOutboxStatusCodes.Pending,
            RetryCount = 0,
            CreatedAt = now
        });
    }
}
