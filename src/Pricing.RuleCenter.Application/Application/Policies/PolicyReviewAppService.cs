using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;

namespace Pricing.RuleCenter.Application.Policies;

public sealed class PolicyReviewAppService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IPolicyReviewRepository _policyReviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public PolicyReviewAppService(
        IPolicyRepository policyRepository,
        IPolicyReviewRepository policyReviewRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _policyRepository = policyRepository;
        _policyReviewRepository = policyReviewRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> SubmitAsync(long policyVersionId, string submittedBy, string reviewStage)
    {
        var version = await _policyRepository.GetVersionAsync(policyVersionId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
        if (!string.Equals(version.PolicyStatus, PolicyLifecycleCodes.Validated, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(version.PolicyStatus, PolicyLifecycleCodes.ReviewPending, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.PolicyStatusNotAllowed,
                409,
                $"策略版本 {policyVersionId} 当前状态 {version.PolicyStatus} 不允许提审。");
        }

        var latest = await _policyReviewRepository.GetLatestByPolicyVersionIdAsync(policyVersionId);
        if (latest is not null &&
            string.Equals(latest.ReviewStatus, PolicyReviewStatusCodes.Pending, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(latest.SourceChecksum, version.Checksum, StringComparison.Ordinal))
        {
            throw new BizException(
                BizErrorCode.ResourceAlreadyExists,
                409,
                $"策略版本 {policyVersionId} 已存在相同内容的待审记录。");
        }

        version.PolicyStatus = PolicyLifecycleCodes.ReviewPending;
        var now = _clock.Now;
        var review = new PolicyReview
        {
            PolicyVersionId = policyVersionId,
            ReviewStatus = PolicyReviewStatusCodes.Pending,
            ReviewStage = reviewStage.Trim(),
            SubmittedBy = submittedBy.Trim(),
            SubmittedAt = now,
            SourceChecksum = version.Checksum
        };

        await _unitOfWork.BeginAsync();
        try
        {
            await _policyRepository.UpdateVersionAsync(version);
            var reviewId = await _policyReviewRepository.InsertAsync(review);
            await _unitOfWork.CommitAsync();
            return reviewId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task ApproveAsync(long policyVersionId, string reviewedBy, string? reviewComment)
    {
        var version = await _policyRepository.GetVersionAsync(policyVersionId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
        var latest = await _policyReviewRepository.GetLatestByPolicyVersionIdAsync(policyVersionId)
            ?? throw new BizException(BizErrorCode.PolicyReviewRequired, 409, $"策略版本 {policyVersionId} 不存在待审记录。");

        if (!string.Equals(latest.ReviewStatus, PolicyReviewStatusCodes.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(BizErrorCode.PolicyReviewRequired, 409, $"策略版本 {policyVersionId} 当前不存在待审记录。");
        }

        if (!string.Equals(latest.SourceChecksum, version.Checksum, StringComparison.Ordinal))
        {
            latest.ReviewStatus = PolicyReviewStatusCodes.Outdated;
            await _policyReviewRepository.UpdateAsync(latest);
            throw new BizException(BizErrorCode.PolicyReviewOutdated, 409, $"策略版本 {policyVersionId} 审批后又被修改，请重新提审。");
        }

        latest.ReviewStatus = PolicyReviewStatusCodes.Approved;
        latest.ReviewedBy = reviewedBy.Trim();
        latest.ReviewedAt = _clock.Now;
        latest.ReviewComment = reviewComment;
        version.PolicyStatus = PolicyLifecycleCodes.Approved;

        await _unitOfWork.BeginAsync();
        try
        {
            await _policyReviewRepository.UpdateAsync(latest);
            await _policyRepository.UpdateVersionAsync(version);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task RejectAsync(long policyVersionId, string reviewedBy, string? reviewComment)
    {
        var version = await _policyRepository.GetVersionAsync(policyVersionId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
        var latest = await _policyReviewRepository.GetLatestByPolicyVersionIdAsync(policyVersionId)
            ?? throw new BizException(BizErrorCode.PolicyReviewRequired, 409, $"策略版本 {policyVersionId} 不存在待审记录。");

        if (!string.Equals(latest.ReviewStatus, PolicyReviewStatusCodes.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(BizErrorCode.PolicyReviewRequired, 409, $"策略版本 {policyVersionId} 当前不存在待审记录。");
        }

        latest.ReviewStatus = PolicyReviewStatusCodes.Rejected;
        latest.ReviewedBy = reviewedBy.Trim();
        latest.ReviewedAt = _clock.Now;
        latest.ReviewComment = reviewComment;
        version.PolicyStatus = PolicyLifecycleCodes.Draft;

        await _unitOfWork.BeginAsync();
        try
        {
            await _policyReviewRepository.UpdateAsync(latest);
            await _policyRepository.UpdateVersionAsync(version);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
