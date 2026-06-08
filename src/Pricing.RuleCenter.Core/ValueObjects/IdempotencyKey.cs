namespace Pricing.RuleCenter.Core.ValueObjects;

/// <summary>
/// 幂等键值对象，封装 sourceSystem + businessRequestNo + callType 三元组。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 计价确认接口的幂等判断依赖三个字段的组合键。
/// 本值对象将三元组封装为不可变类型，统一 Equals/GetHashCode 行为，
/// 避免各处手动拼接字符串导致的一致性风险。
/// </para>
/// <para>
/// 【幂等性约束】
/// 幂等键为 sourceSystem + businessRequestNo + callType 的三元组。
/// 同一业务号的 confirm 重试必须复用同一 businessRequestNo，
/// 且保存 REQUEST_FINGERPRINT 用于判断同一业务号下参数是否发生变化，防止静默数据错乱。
/// </para>
/// <para>
/// 【字段说明】
/// <list type="bullet">
/// <item>SourceSystem — 来源系统编码，如 HIS、KIOSK、WECHAT</item>
/// <item>BusinessRequestNo — 调用方稳定业务号，confirm 超时重试时必须复用</item>
/// <item>CallType — 调用类型，如 CONFIRM、COMMIT、CANCEL、REVERSE</item>
/// </list>
/// </para>
/// <para>
/// 【值对象特性】
/// 本类型为 record，具有不可变性、值相等性和编译器生成的 Equals/GetHashCode。
/// 可直接用作字典键或 HashSet 元素。
/// </para>
/// </remarks>
public sealed record IdempotencyKey
{
    /// <summary>
    /// 来源系统编码，标识本次请求来自哪个渠道系统。
    /// </summary>
    /// <remarks>
    /// 常见值：HIS（医院信息系统）、KIOSK（自助机）、WECHAT（微信公众号）。
    /// 不同来源系统的限额统计独立计算（按收费途径统计）。
    /// 不可为空。
    /// </remarks>
    public string SourceSystem { get; init; }

    /// <summary>
    /// 调用方稳定业务号，来源为调用方传入的 businessRequestNo。
    /// </summary>
    /// <remarks>
    /// confirm 超时重试时必须复用同一业务号。幂等判断的三元组之一。
    /// 空值表示该调用类型不支持幂等（如 simulate 试算）。
    /// HIS 能否提供稳定的 businessRequestNo 是待确认问题。
    /// </remarks>
    public string BusinessRequestNo { get; init; }

    /// <summary>
    /// 调用类型，标识本次请求的操作类型，是幂等三元组之一。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>SIMULATE — 试算，不占额度，不落日志</item>
    /// <item>CONFIRM — 确认计价，占用额度，写入 PENDING 占用</item>
    /// <item>COMMIT — HIS 落账成功通知，推进占用为 CONFIRMED</item>
    /// <item>CANCEL — 取消确认，释放 PENDING 占用</item>
    /// <item>REVERSE — 退费/冲正，生成对冲占用记录</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    public string CallType { get; init; }

    /// <summary>
    /// 创建幂等键值对象。
    /// </summary>
    /// <param name="SourceSystem">来源系统编码，不可为空。</param>
    /// <param name="BusinessRequestNo">调用方稳定业务号，可为空（表示不支持幂等）。</param>
    /// <param name="CallType">调用类型，不可为空。</param>
    public IdempotencyKey(string SourceSystem, string BusinessRequestNo, string CallType)
    {
        this.SourceSystem = SourceSystem;
        this.BusinessRequestNo = BusinessRequestNo;
        this.CallType = CallType;
    }

    /// <summary>
    /// 返回幂等键的字符串表示，格式为 "sourceSystem|businessRequestNo|callType"。
    /// </summary>
    /// <returns>管道符分隔的幂等键字符串。</returns>
    /// <remarks>
    /// <para>
    /// 【格式说明】
    /// 使用管道符（|）分隔三个字段，便于日志输出和调试。
    /// 示例：HIS|REQ-20260510-001|CONFIRM
    /// </para>
    /// <para>
    /// 【不用于数据库查询】
    /// 数据库查询幂等记录时，应使用三个字段分别作为条件，
    /// 而非拼接字符串后模糊匹配，以确保索引命中。
    /// </para>
    /// </remarks>
    public override string ToString() => $"{SourceSystem}|{BusinessRequestNo}|{CallType}";
}
