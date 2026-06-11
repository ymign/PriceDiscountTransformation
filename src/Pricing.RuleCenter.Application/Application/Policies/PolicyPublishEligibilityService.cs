using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Policies;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略发布资格校验服务。
/// </summary>
internal sealed class PolicyPublishEligibilityService : IPolicyPublishEligibilityService
{
    private readonly PolicyPublishProfileResolver _publishProfileResolver;
    private readonly IPolicyReviewRepository _policyReviewRepository;

    /// <summary>
    /// 初始化策略发布资格校验服务。
    /// </summary>
    public PolicyPublishEligibilityService(
        PolicyPublishProfileResolver publishProfileResolver,
        IPolicyReviewRepository policyReviewRepository)
    {
        _publishProfileResolver = publishProfileResolver;
        _policyReviewRepository = policyReviewRepository;
    }

    /// <summary>
    /// 校验策略版本是否允许进入运行时包发布流程。
    /// </summary>
    public async Task EnsureEligibleAsync(PolicyAggregate policy, PolicyVersion version)
    {
        if (_publishProfileResolver.RequiresReview(policy))
        {
            if (!string.Equals(version.PolicyStatus, PolicyLifecycleCodes.Approved, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(version.PolicyStatus, PolicyLifecycleCodes.PublishReady, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.PolicyStatusNotAllowed,
                    409,
                    $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 尚未审批通过，不能发布。");
            }

            var latestReview = await _policyReviewRepository.GetLatestByPolicyVersionIdAsync(version.PolicyVersionId);
            if (latestReview is null)
            {
                throw new BizException(
                    BizErrorCode.PolicyReviewRequired,
                    409,
                    $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 缺少审批记录。");
            }

            if (string.Equals(latestReview.ReviewStatus, PolicyReviewStatusCodes.Rejected, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.PolicyReviewRejected,
                    409,
                    $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 最近一次审批已驳回。");
            }

            if (!string.Equals(latestReview.ReviewStatus, PolicyReviewStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.PolicyReviewRequired,
                    409,
                    $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 尚未审批通过。");
            }

            if (!string.Equals(latestReview.SourceChecksum, version.Checksum, StringComparison.Ordinal))
            {
                throw new BizException(
                    BizErrorCode.PolicyReviewOutdated,
                    409,
                    $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 审批后又被修改，请重新提审。");
            }

            return;
        }

        if (!string.Equals(version.PolicyStatus, PolicyLifecycleCodes.Validated, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(version.PolicyStatus, PolicyLifecycleCodes.PublishReady, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.PolicyStatusNotAllowed,
                409,
                $"策略 {policy.PolicyCode} 的版本 {version.VersionNo} 当前不是 VALIDATED，不能直发。");
        }
    }
}
