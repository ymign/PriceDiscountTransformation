using System.Text.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pricing.RuleCenter.Api.Swagger;

/// <summary>
/// 将 Swagger 中的 query 参数名展示为 snake_case。
/// </summary>
public sealed class SnakeCaseQueryParameterOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters.Where(parameter => parameter.In == ParameterLocation.Query))
        {
            parameter.Name = JsonNamingPolicy.SnakeCaseLower.ConvertName(parameter.Name);
        }
    }
}
