using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pricing.RuleCenter.Api.Swagger;

/// <summary>
/// 为受授权保护的 Swagger 操作标记 API Key 安全要求。
/// </summary>
public sealed class ApiKeySecurityOperationFilter : IOperationFilter
{
    /// <summary>
    /// Swagger security scheme 名称。
    /// </summary>
    public const string SchemeName = "ApiKey";

    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (AllowsAnonymous(context) || !RequiresAuthorization(context))
        {
            return;
        }

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = SchemeName
                    }
                }
            ] = Array.Empty<string>()
        });
    }

    private static bool AllowsAnonymous(OperationFilterContext context)
    {
        return context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
            || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true;
    }

    private static bool RequiresAuthorization(OperationFilterContext context)
    {
        return context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
            || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true;
    }
}
