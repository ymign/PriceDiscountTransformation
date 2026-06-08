using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Policies;

public sealed class PolicyPackageDiffResult
{
    [JsonPropertyName("candidate_package_id")]
    public long CandidatePackageId { get; init; }

    [JsonPropertyName("active_package_id")]
    public long? ActivePackageId { get; init; }

    [JsonPropertyName("added_policy_version_ids")]
    public IReadOnlyList<long> AddedPolicyVersionIds { get; init; } = Array.Empty<long>();

    [JsonPropertyName("removed_policy_version_ids")]
    public IReadOnlyList<long> RemovedPolicyVersionIds { get; init; } = Array.Empty<long>();

    [JsonPropertyName("unchanged_policy_version_ids")]
    public IReadOnlyList<long> UnchangedPolicyVersionIds { get; init; } = Array.Empty<long>();
}
