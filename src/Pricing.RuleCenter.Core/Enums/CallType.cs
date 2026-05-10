namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// 计价接口调用类型枚举，对应统一计价引擎的五个 API 端点。
///
/// 三阶段确认模型：
///   Simulate → Confirm → Commit（正常流程）
///                  ↘ Cancel（取消流程）
///                  ↘ Reverse（退费/冲销流程）
///
/// 使用场景：
/// - 幂等性键（idempotency key）的组成部分：sourceSystem + businessRequestNo + callType。
/// - PR_CHARGE_REQUEST_LOG.CALL_TYPE 字段记录本次调用类型。
/// - PR_LIMIT_OCCUPY 中的占用记录按 callType 区分确认占用与冲正释放。
/// </summary>
public enum CallType
{
    /// <summary>
    /// 试算：对应 POST /api/pricing/calculate/simulate。
    /// 仅计算折价结果返回给前端预览，不占用额度，不写入持久化追溯日志。
    /// </summary>
    Simulate,

    /// <summary>
    /// 确认：对应 POST /api/pricing/calculate/confirm。
    /// 正式占用额度，写入请求日志和折价明细，进入 ConfirmPending 状态。
    /// 接口必须幂等，重复调用（相同幂等键）返回相同结果。
    /// </summary>
    Confirm,

    /// <summary>
    /// 提交（结算）：对应 POST /api/pricing/calculate/commit。
    /// HIS 侧结算成功后回调，将状态从 Confirmed 推进为 Committed，折价明细归档。
    /// </summary>
    Commit,

    /// <summary>
    /// 取消：对应 POST /api/pricing/calculate/cancel。
    /// 取消已确认的计价，释放额度占用，状态推进为 Cancelled。
    /// </summary>
    Cancel,

    /// <summary>
    /// 冲正（退费）：对应 POST /api/pricing/calculate/reverse。
    /// 退费或冲销操作，按退费数量释放额度，记录逆向折价明细。
    /// 当日退费释放数量；隔日退费重收后按重收当天重新校验额度。
    /// </summary>
    Reverse
}
