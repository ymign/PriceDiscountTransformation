namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 单次请求共享状态键生成器。
/// </summary>
public static class RequestSharedStateKeys
{
    /// <summary>
    /// 生成限额维度累计键。
    /// </summary>
    /// <param name="limitType">一个表示限额类型的编码。</param>
    /// <param name="dimensionCode">一个表示累计维度的编码。</param>
    /// <returns>一个标准化后的请求共享状态键。</returns>
    public static string BuildLimitDimensionKey(string limitType, string dimensionCode)
    {
        return $"{Normalize(limitType)}:{Normalize(dimensionCode)}";
    }

    /// <summary>
    /// 生成同组互斥累计键。
    /// </summary>
    /// <param name="groupCode">一个表示项目组或互斥组的编码。</param>
    /// <returns>一个标准化后的请求共享状态键。</returns>
    public static string BuildMutexKey(string groupCode)
    {
        return $"MUTEX:{Normalize(groupCode)}";
    }

    /// <summary>
    /// 生成同手术封顶累计金额键。
    /// </summary>
    /// <param name="operationNo">一个表示手术标识的编码。</param>
    /// <param name="groupCode">一个表示项目组的编码。</param>
    /// <returns>一个标准化后的请求共享状态键。</returns>
    public static string BuildOperationCeilingKey(string operationNo, string groupCode)
    {
        return $"OP_CEILING:{Normalize(operationNo)}:{Normalize(groupCode)}";
    }

    /// <summary>
    /// 生成父项目最终金额键。
    /// </summary>
    /// <param name="itemCode">一个表示父项目的编码。</param>
    /// <returns>一个标准化后的请求共享状态键。</returns>
    public static string BuildParentItemAmountKey(string itemCode)
    {
        return $"ITEM_AMT:{Normalize(itemCode)}";
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
}
