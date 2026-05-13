namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 业务错误码常量定义。
/// </summary>
/// <remarks>
/// <para>
/// 按业务域分段编码，便于调用方按区间识别错误类别。
/// 全局异常过滤器和服务层应使用这些常量而非硬编码数字。
/// </para>
/// <para>
/// 编码分段规则：
/// <list type="bullet">
///   <item>0 — 成功</item>
///   <item>1001-1099 — 规则配置类错误（规则不存在、版本冲突等）</item>
///   <item>2001-2099 — 计价计算类错误（单价校验失败、限额超限等）</item>
///   <item>3001-3099 — 状态流转类错误（版本状态不允许发布、请求状态不允许操作等）</item>
///   <item>4001-4099 — 限额占用类错误（额度不足、并发冲突、锁获取失败等）</item>
///   <item>9001-9099 — 系统级错误（数据库异常、服务降级等）</item>
/// </list>
/// </para>
/// <para>
/// 通用错误码保留给全局异常过滤器使用：
/// <list type="bullet">
///   <item>-1 — 通用业务失败（未分类）</item>
///   <item>400 — 参数校验失败</item>
///   <item>409 — 资源冲突/不存在</item>
///   <item>500 — 系统异常</item>
/// </list>
/// </para>
/// </remarks>
public static class BizErrorCode
{
    // ========== 规则配置类错误（1001-1099） ==========

    /// <summary>规则不存在。</summary>
    public const int RuleNotFound = 1001;

    /// <summary>规则版本不存在。</summary>
    public const int RuleVersionNotFound = 1002;

    /// <summary>规则编码已存在（创建时重复）。</summary>
    public const int RuleCodeDuplicate = 1003;

    /// <summary>规则条件不存在。</summary>
    public const int RuleConditionNotFound = 1004;

    /// <summary>规则重叠冲突（发布前校验）。</summary>
    public const int RuleOverlapConflict = 1005;

    /// <summary>动作组配置冲突（发布前校验）。</summary>
    public const int ActionGroupConflict = 1006;

    /// <summary>缺少测试用例（发布前校验）。</summary>
    public const int MissingTestCase = 1007;

    /// <summary>公式定义不存在。</summary>
    public const int FormulaNotFound = 1008;

    /// <summary>资源已存在（字典项、公式等重复创建）。</summary>
    public const int ResourceAlreadyExists = 1009;

    /// <summary>草稿版本已存在（同一规则同时只能有一个草稿）。</summary>
    public const int DraftVersionAlreadyExists = 1010;

    /// <summary>字典项不存在。</summary>
    public const int DictNotFound = 1011;

    // ========== 计价计算类错误（2001-2099） ==========

    /// <summary>单价校验失败（渠道传入单价与权威单价不一致）。</summary>
    public const int PriceMismatch = 2001;

    /// <summary>规则匹配失败（未找到匹配的规则）。</summary>
    public const int RuleMatchFailed = 2002;

    /// <summary>限额超限（日数量/时间窗/单次数量等超出限制）。</summary>
    public const int LimitExceeded = 2003;

    /// <summary>幂等冲突（重复请求但参数不一致）。</summary>
    public const int IdempotencyConflict = 2004;

    /// <summary>请求已落账不可取消。</summary>
    public const int AlreadyCommitted = 2005;

    /// <summary>原请求不可冲正（状态不允许或不存在）。</summary>
    public const int ReverseNotAllowed = 2006;

    /// <summary>请求不存在。</summary>
    public const int RequestNotFound = 2007;

    /// <summary>批量试算中部分项目计价失败。</summary>
    public const int BatchPartialFailure = 2008;

    // ========== 状态流转类错误（3001-3099） ==========

    /// <summary>版本状态不允许发布（如已 DISABLED 或已有草稿）。</summary>
    public const int VersionStatusNotAllowed = 3001;

    /// <summary>规则状态不允许停用（如已是 DISABLED）。</summary>
    public const int RuleAlreadyDisabled = 3002;

    /// <summary>回滚目标版本不存在或不可用。</summary>
    public const int RollbackTargetNotAvailable = 3003;

    /// <summary>请求状态不允许当前操作（如 CONFIRM_PENDING 才能 commit）。</summary>
    public const int RequestStatusNotAllowed = 3004;

    // ========== 限额占用类错误（4001-4099） ==========

    /// <summary>额度不足（日限额/时间窗限额/单次限额等）。</summary>
    public const int InsufficientQuota = 4001;

    /// <summary>并发冲突（SELECT FOR UPDATE 超时或死锁）。</summary>
    public const int ConcurrencyConflict = 4002;

    /// <summary>限额锁获取失败（数据库异常）。</summary>
    public const int LimitLockFailed = 4003;

    // ========== 系统级错误（9001-9099） ==========

    /// <summary>数据库操作异常。</summary>
    public const int DatabaseError = 9001;

    /// <summary>服务降级（计价服务不可用，需转人工或降级处理）。</summary>
    public const int ServiceDegraded = 9002;
}
