using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 候选运行时包与当前激活包之间的策略差异结果。
/// </summary>
public sealed class PolicyPackageDiffResult
{
    /// <summary>
    /// 候选运行时包主键。
    /// </summary>
    [JsonPropertyName("candidate_package_id")]
    public long CandidatePackageId { get; init; }

    /// <summary>
    /// 当前激活运行时包主键；不存在激活包时为空。
    /// </summary>
    [JsonPropertyName("active_package_id")]
    public long? ActivePackageId { get; init; }

    /// <summary>
    /// 仅候选包包含的策略版本主键集合。
    /// </summary>
    [JsonPropertyName("added_policy_version_ids")]
    public IReadOnlyList<long> AddedPolicyVersionIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// 仅当前激活包包含的策略版本主键集合。
    /// </summary>
    [JsonPropertyName("removed_policy_version_ids")]
    public IReadOnlyList<long> RemovedPolicyVersionIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// 候选包与当前激活包共同包含的策略版本主键集合。
    /// </summary>
    [JsonPropertyName("unchanged_policy_version_ids")]
    public IReadOnlyList<long> UnchangedPolicyVersionIds { get; init; } = Array.Empty<long>();
}
