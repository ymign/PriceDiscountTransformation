namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 单次请求共享状态键生成器。
/// </summary>
public static class RequestSharedStateKeys
{
    public static string BuildLimitDimensionKey(string limitType, string dimensionCode)
    {
        return $"{Normalize(limitType)}:{Normalize(dimensionCode)}";
    }

    public static string BuildMutexKey(string groupCode)
    {
        return $"MUTEX:{Normalize(groupCode)}";
    }

    public static string BuildOperationCeilingKey(string operationNo, string groupCode)
    {
        return $"OP_CEILING:{Normalize(operationNo)}:{Normalize(groupCode)}";
    }

    public static string BuildParentItemAmountKey(string itemCode)
    {
        return $"ITEM_AMT:{Normalize(itemCode)}";
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
}
