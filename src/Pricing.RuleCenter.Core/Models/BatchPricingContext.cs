namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 批量计价共享上下文，确保同批请求内的项目能正确共享限额累计和互斥判断。
/// </summary>
/// <remarks>
/// <para>
/// 【核心问题】
/// 当一批请求包含多个项目时（如同一收费动作中的 CT 平扫 + CT 增强），
/// 如果逐项独立计算，第2个项目看不到第1个项目刚产生的限额占用，导致同批内突破限制。
/// 此上下文在同批内共享已占用的数量/金额，确保批量场景下的限额正确累计。
/// </para>
/// <para>
/// 【生命周期】
/// 由 PricingApiService 在处理批量 items 时创建。第一个项目计算前初始化，
/// 每个项目计算完成后由 AccumulateToBatch 更新，后续项目计算时作为"同批已占用"传入。
/// </para>
/// <para>
/// 【与 PricingContext.InRequestOccupiedQtyByLimitDimension 的关系】
/// PricingContext 上的 InRequestOccupiedQtyByLimitDimension 是轻量的维度累计字典，
/// 仅用于限额执行器快速判断。BatchPricingContext 是更完整的批量共享状态，
/// 额外维护了同组互斥、同手术封顶的跨项目判断所需的信息。
/// </para>
/// </remarks>
public sealed class BatchPricingContext
{
    /// <summary>
    /// 同批内按 LimitDimension 累计的已占用数量。
    /// </summary>
    /// <remarks>
    /// <para>
    /// key = LimitDimensionCode（如 DQ:patientId:itemCode:20260511），value = 累计数量。
    /// </para>
    /// <para>
    /// 用途：限额执行器在查询数据库历史占用之前，先检查本批内前面项目已经占用的数量，
    /// 避免同批内两个项目都看到"还有额度"然后一起占用导致突破限制。
    /// </para>
    /// <para>
    /// 精度：decimal，与项目数量单位一致（次、个、CM2 等）。
    /// </para>
    /// </remarks>
    public Dictionary<string, decimal> InBatchOccupiedQtyByDimension { get; set; }
        = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 同批内按 LimitDimension 累计的已占用金额。
    /// </summary>
    /// <remarks>
    /// <para>
    /// key = LimitDimensionCode，value = 累计金额。
    /// </para>
    /// <para>
    /// 用途：金额维度的限额校验（如单次收费金额上限、日累计金额上限）。
    /// 与 InBatchOccupiedQtyByDimension 配对使用，数量和金额独立累计。
    /// </para>
    /// <para>
    /// 精度：decimal NUMBER(18,4)，单位元（人民币）。
    /// </para>
    /// </remarks>
    public Dictionary<string, decimal> InBatchOccupiedAmtByDimension { get; set; }
        = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 同批内已处理的项目结果列表，用于同组互斥和同手术封顶的跨项目判断。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 同组互斥执行器需要知道同批内前面的项目是否已经命中了同组规则，
    /// 如果是，后续同组项目应被互斥截断。该列表提供完整的结果快照。
    /// </para>
    /// <para>
    /// 同手术封顶执行器需要累计同批内同一手术标识下所有项目的金额，
    /// 判断是否达到封顶额度。该列表提供跨项目的金额累计基础。
    /// </para>
    /// </remarks>
    public List<PricingResult> ProcessedResults { get; set; } = new List<PricingResult>();

    /// <summary>
    /// 同批内按 ItemGroupCode 累计的已处理项目数，用于同组互斥判断。
    /// </summary>
    /// <remarks>
    /// <para>
    /// key = ItemGroupCode, value = 已处理数量。
    /// </para>
    /// <para>
    /// 同组互斥规则要求同一组内只允许一个项目通过（或按优先级选择）。
    /// 当第 N 个项目进入时，如果它所属的组已经有项目被处理过，
    /// 互斥执行器可以直接跳过或截断，无需再查数据库。
    /// </para>
    /// </remarks>
    public Dictionary<string, int> InBatchItemCountByGroup { get; set; }
        = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 同批内按 operationId + groupCode 累计的已占用金额，用于同手术封顶判断。
    /// </summary>
    /// <remarks>
    /// <para>
    /// key = "operationId:groupCode", value = 累计金额。
    /// </para>
    /// <para>
    /// 同手术封顶规则要求同一手术下同一组项目的总金额不超过封顶值。
    /// 该字典在同批内跨项目累计，确保第2个项目能正确看到第1个项目已经占用的金额。
    /// </para>
    /// <para>
    /// operationId 来源为 ExtraParams["operationNo"]，groupCode 来源为 PricingContext.ItemGroupCode。
    /// 两者用冒号拼接作为 key，大小写不敏感。
    /// </para>
    /// </remarks>
    public Dictionary<string, decimal> InBatchOccupiedAmtByOperation { get; set; }
        = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 同批内按 LimitDimension 累计的限额占用草稿，用于时间窗执行器精确过滤。
    /// </summary>
    /// <remarks>
    /// <para>
    /// TIME_WINDOW 类型的限额需要按业务收费时间判断前序明细是否落入当前滑动窗口。
    /// 单纯按维度累计数量会把窗口外的明细也算进去，因此保留占额草稿本身，
    /// 供时间窗执行器按 BusinessChargeTime 做精确过滤。
    /// </para>
    /// <para>
    /// 与 PricingContext.InRequestLimitOccupies 作用相同，但作用域扩展到整个 BatchPricingContext。
    /// </para>
    /// </remarks>
    public List<LimitOccupy> InBatchLimitOccupies { get; set; } = new List<LimitOccupy>();

    /// <summary>
    /// 将一个项目计算完成后的结果写入批量上下文，供后续项目使用。
    /// </summary>
    /// <param name="result">当前项目计算完成后的计价结果。</param>
    /// <param name="context">当前项目的计价上下文，用于提取 ItemGroupCode、ExtraParams 等。</param>
    /// <remarks>
    /// <para>
    /// 该方法在每个项目计算完成后由 PricingApiService 调用，按以下维度更新共享状态：
    /// 1. 限额维度累计（数量和金额）
    /// 2. 同组互斥计数
    /// 3. 同手术封顶金额
    /// 4. 时间窗占额草稿
    /// 5. 已处理结果列表
    /// </para>
    /// <para>
    /// 金额精度：中间累计保留全精度，不提前取整。
    /// </para>
    /// </remarks>
    public void AccumulateToBatch(PricingResult result, PricingContext context)
    {
        // ========== 阶段一：将限额占用草稿写入批量共享状态 ==========
        // 限额执行器在计算下一个项目时，需要知道前面项目已经占用了哪些维度。
        // 这里把 result 中有效的占额草稿（有实际数量或金额的）追加到批量占额列表。
        foreach (var occupy in result.LimitOccupies)
        {
            if (occupy.OccupyQty != 0 || occupy.OccupyAmt != 0)
            {
                InBatchLimitOccupies.Add(occupy);
            }
        }

        // ========== 阶段二：按限额维度累计数量和金额 ==========
        // key 格式为 "LimitType:LimitDimensionCode"，与 PricingApiService.BuildInRequestLimitKey 一致。
        // 后续项目在限额校验时会读取这些累计值，避免同批内突破限制。
        foreach (var occupy in result.LimitOccupies)
        {
            if (string.IsNullOrWhiteSpace(occupy.LimitType) ||
                string.IsNullOrWhiteSpace(occupy.LimitDimensionCode))
            {
                continue;
            }

            var dimKey = $"{occupy.LimitType.Trim().ToUpperInvariant()}:{occupy.LimitDimensionCode.Trim().ToUpperInvariant()}";

            // 累计数量
            InBatchOccupiedQtyByDimension.TryGetValue(dimKey, out var existingQty);
            InBatchOccupiedQtyByDimension[dimKey] = existingQty + occupy.OccupyQty;

            // 累计金额
            InBatchOccupiedAmtByDimension.TryGetValue(dimKey, out var existingAmt);
            InBatchOccupiedAmtByDimension[dimKey] = existingAmt + occupy.OccupyAmt;
        }

        // ========== 阶段三：更新同组互斥计数 ==========
        // 如果当前项目属于某个项目组，将其计入同组已处理计数。
        // 后续同组项目进入时，互斥执行器检查该计数即可判断是否需要截断。
        if (!string.IsNullOrWhiteSpace(context.ItemGroupCode))
        {
            var groupCode = context.ItemGroupCode.Trim().ToUpperInvariant();
            InBatchItemCountByGroup.TryGetValue(groupCode, out var existingCount);
            InBatchItemCountByGroup[groupCode] = existingCount + 1;
        }

        // ========== 阶段四：更新同手术封顶金额 ==========
        // 手术标识来源为 ExtraParams["operationNo"]。存在时将当前项目的最终金额
        // 累计到对应手术+组的维度下，供封顶执行器判断是否达到上限。
        var operationNo = GetExtraParam(context.ExtraParams, "operationNo")
            ?? GetExtraParam(context.ExtraParams, "operationId");
        if (!string.IsNullOrWhiteSpace(operationNo) && !string.IsNullOrWhiteSpace(context.ItemGroupCode))
        {
            var opKey = $"{operationNo.Trim().ToUpperInvariant()}:{context.ItemGroupCode.Trim().ToUpperInvariant()}";
            InBatchOccupiedAmtByOperation.TryGetValue(opKey, out var existingOpAmt);
            InBatchOccupiedAmtByOperation[opKey] = existingOpAmt + result.FinalAmount;
        }

        // ========== 阶段五：记录已处理结果 ==========
        // 完整结果快照保留，供后续可能的跨项目判断（如同组互斥需要比较优先级）。
        ProcessedResults.Add(result);
    }

    /// <summary>
    /// 从扩展参数字典中安全获取指定键的字符串值。
    /// </summary>
    /// <param name="extraParams">扩展参数字典。</param>
    /// <param name="key">目标键名。</param>
    /// <returns>键对应的字符串值，不存在或为空时返回 null。</returns>
    private static string? GetExtraParam(IReadOnlyDictionary<string, string>? extraParams, string key)
    {
        if (extraParams is null || !extraParams.TryGetValue(key, out var value))
        {
            return null;
        }

        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
