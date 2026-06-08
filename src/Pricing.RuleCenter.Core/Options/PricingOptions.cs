namespace Pricing.RuleCenter.Core.Options;

/// <summary>
/// 计价规则中心运行配置，集中管理计价服务的核心运行参数。
/// </summary>
/// <remarks>
/// <para>
/// 该配置对象通过 ASP.NET Core 的 Options 模式绑定自 appsettings.json 的 "Pricing" 配置段。
/// 注册方式：<c>builder.Services.Configure&lt;PricingOptions&gt;(builder.Configuration.GetSection("Pricing"));</c>
/// </para>
/// <para>
/// 配置涵盖四个维度：
/// 1. 数据库连接（<see cref="OracleConnectionString"/>）
/// 2. confirm 占用保护（<see cref="ConfirmExpireMinutes"/>）和过期清理（<see cref="ExpireCleanupIntervalSeconds"/>）
/// 3. 外部调用容错（<see cref="HttpTimeoutSeconds"/>、<see cref="MaxRetryCount"/>）
/// 4. 权威单价诊断开关（<see cref="EnableAuthorityPriceCheck"/>）
/// </para>
/// <para>
/// 所有时间相关的配置均使用整数类型，避免配置文件中的浮点解析精度问题。
/// 修改配置后需重启服务生效（不支持运行时热更新）。
/// </para>
/// </remarks>
public sealed class PricingOptions
{
    /// <summary>
    /// Oracle 数据库连接字符串，用于访问规则中心 PR_ 前缀表和必要的 HIS 主数据视图。
    /// 连接使用 Oracle.ManagedDataAccess.Core 驱动，格式遵循 Oracle 连接字符串规范。
    /// 此连接字符串同时用于 SqlSugarCore 的 DbContext 初始化。
    /// </summary>
    public string OracleConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// confirm 保护占用的过期分钟数，默认 30 分钟。
    /// 当调用方执行 confirm（确认计价，占用额度）后，若超过此时间仍未收到 commit（HIS 结算成功）
    /// 或 cancel（取消确认），后台清理任务会将该记录状态置为 EXPIRED 并释放额度。
    /// 此机制防止因调用方异常（如 HIS 崩溃、网络中断）导致额度永久占用。
    /// 建议值：根据 HIS 结算流程的正常耗时设置，一般 15-30 分钟。
    /// </summary>
    public int ConfirmExpireMinutes { get; set; } = 30;

    /// <summary>
    /// 过期清理后台任务的轮询间隔，单位秒，默认 300 秒（5 分钟）。
    /// 后台 IHostedService 按此间隔扫描 PR_LIMIT_OCCUPY 和 PR_CHARGE_REQUEST_LOG，
    /// 将超过 <see cref="ConfirmExpireMinutes"/> 的 CONFIRM_PENDING 记录置为 EXPIRED。
    /// 间隔越短清理越及时，但数据库负载越高。建议值：120-600 秒。
    /// </summary>
    public int ExpireCleanupIntervalSeconds { get; set; } = 300;

    /// <summary>
    /// HTTP 调用超时时间，单位秒，默认 10 秒。
    /// 用于后续对接外部服务（如 HIS 物价主数据查询、权威单价诊断）时统一设置超时。
    /// 超时后应按重试策略（<see cref="MaxRetryCount"/>）进行有限次重试。
    /// </summary>
    public int HttpTimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// 最大重试次数，默认 3 次。
    /// 用于外部服务调用失败时的重试策略，以及后台补偿任务的执行次数上限。
    /// 重试之间建议采用指数退避策略（如 1s、2s、4s）。
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// 缓存版本同步后台任务的轮询间隔，单位秒，默认 3 秒。
    /// </summary>
    /// <remarks>
    /// 多实例部署下，各节点通过轮询 `PR_CACHE_VERSION` 表感知规则缓存版本变化。
    /// 数值越小，跨实例缓存收敛越快；数值越大，数据库轮询压力越低。
    /// </remarks>
    public int CacheVersionSyncIntervalSeconds { get; set; } = 3;

    /// <summary>
    /// 是否启用权威物价诊断日志，默认 true。
    /// 启用后，试算和确认计价会比较调用方传入单价与 HIS 物价主数据是否一致。
    /// 不一致时只记录 Warning 日志，不返回 PRICE_MISMATCH，也不影响业务流程。
    /// 该字段沿用历史命名以减少配置破坏面，语义已经从强校验降级为诊断。
    /// </summary>
    public bool EnableAuthorityPriceCheck { get; set; } = true;

    /// <summary>
    /// 儿童价年龄上界（不包含），默认 6，表示 6 岁以下使用儿童价。
    /// </summary>
    /// <remarks>
    /// 该值只影响权威单价诊断选择哪一个 HIS 价格列，不替代规则引擎中的年龄条件。
    /// 如业务口径调整为“6 岁及以下”，应把这里改为 7 或补充更精确的出生日期判断。
    /// </remarks>
    public int ChildPriceAgeExclusive { get; set; } = 6;
}
