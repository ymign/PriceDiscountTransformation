namespace Pricing.RuleCenter.Core.Constants;

public static class RuntimePackageStatusCodes
{
    public const string Building = "BUILDING";
    public const string Built = "BUILT";
    public const string Active = "ACTIVE";
    public const string Superseded = "SUPERSEDED";
    public const string BuildFailed = "BUILD_FAILED";
}

public static class RuntimePackageStateCodes
{
    public const string Active = "ACTIVE";
}

public static class RuntimeBuildScopeCodes
{
    public const string Full = "FULL";
}

public static class RuntimeMergeModeCodes
{
    public const string SingleWinner = "SINGLE_WINNER";
    public const string MultiAllowed = "MULTI_ALLOWED";
}
