namespace Pricing.RuleCenter.Application.Policies;

public sealed class PolicyPackageDiffResult
{
    public long CandidatePackageId { get; init; }

    public long? ActivePackageId { get; init; }

    public IReadOnlyList<long> AddedPolicyVersionIds { get; init; } = Array.Empty<long>();

    public IReadOnlyList<long> RemovedPolicyVersionIds { get; init; } = Array.Empty<long>();

    public IReadOnlyList<long> UnchangedPolicyVersionIds { get; init; } = Array.Empty<long>();
}
