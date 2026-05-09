namespace Pricing.RuleCenter.Core.Options;

/// <summary>
/// 计价规则中心运行配置。
/// </summary>
/// <remarks>
/// 该对象通常绑定自 appsettings 的 Pricing 配置段，集中控制数据库连接、confirm 保护期、
/// 过期清理频率和权威价格校验开关。
/// </remarks>
public sealed class PricingOptions
{
    /// <summary>
    /// Oracle 数据库连接字符串，用于访问规则中心 PR_ 表和必要的 HIS 主数据视图。
    /// </summary>
    public string OracleConnectionString { get; set; } = string.Empty;
    /// <summary>
    /// confirm 保护占用的过期分钟数。超过后后台清理会把待确认记录置为过期。
    /// </summary>
    public int ConfirmExpireMinutes { get; set; } = 30;
    /// <summary>
    /// 过期清理后台任务的轮询间隔，单位秒。
    /// </summary>
    public int ExpireCleanupIntervalSeconds { get; set; } = 300;
    /// <summary>
    /// 预留 HTTP 调用超时时间，单位秒，用于后续对接外部服务时统一读取。
    /// </summary>
    public int HttpTimeoutSeconds { get; set; } = 10;
    /// <summary>
    /// 预留最大重试次数，用于后续外部调用或补偿任务统一配置。
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;
    /// <summary>
    /// 是否启用权威物价校验。启用后 confirm 会校验请求单价与 HIS 物价主数据是否一致。
    /// </summary>
    public bool EnableAuthorityPriceCheck { get; set; } = true;
}
