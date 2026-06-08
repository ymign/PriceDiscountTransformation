using Microsoft.AspNetCore.Authentication;

namespace Pricing.RuleCenter.Api.Security;

/// <summary>
/// API Key 认证配置。
/// </summary>
public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>认证方案名称。</summary>
    public const string SchemeName = "ApiKey";

    /// <summary>默认请求头名称。</summary>
    public const string DefaultHeaderName = "X-Api-Key";

    /// <summary>请求头名称。</summary>
    public string HeaderName { get; set; } = DefaultHeaderName;

    /// <summary>允许访问规则中心的 API Key 列表。</summary>
    public List<ApiKeyCredential> Keys { get; } = new();
}

/// <summary>
/// 单个 API Key 凭据。
/// </summary>
public sealed class ApiKeyCredential
{
    /// <summary>API Key 明文。生产环境必须通过环境变量或密钥系统注入。</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>调用方名称，用于审计日志和 ClaimsIdentity。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>调用方角色，例如 pricing.service、pricing.admin。</summary>
    public List<string> Roles { get; } = new();
}
