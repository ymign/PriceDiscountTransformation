using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Pricing.RuleCenter.Api.Security;

/// <summary>
/// 基于 X-Api-Key 请求头的服务间认证处理器。
/// </summary>
public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    /// <summary>初始化 API Key 认证处理器。</summary>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var values))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var submittedKey = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(submittedKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key 不能为空"));
        }

        var credential = Options.Keys.FirstOrDefault(item => IsSameKey(item.Key, submittedKey));
        if (credential is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key 无效"));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, string.IsNullOrWhiteSpace(credential.Name) ? "api-key" : credential.Name),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(credential.Name) ? "api-key" : credential.Name)
        };
        claims.AddRange(credential.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => new Claim(ClaimTypes.Role, role.Trim())));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static bool IsSameKey(string configuredKey, string submittedKey)
    {
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            return false;
        }

        var configuredBytes = Encoding.UTF8.GetBytes(configuredKey);
        var submittedBytes = Encoding.UTF8.GetBytes(submittedKey);
        return configuredBytes.Length == submittedBytes.Length &&
            CryptographicOperations.FixedTimeEquals(configuredBytes, submittedBytes);
    }
}
