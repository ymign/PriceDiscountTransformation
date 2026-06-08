using System.Text.Json;

namespace Pricing.RuleCenter.Api.Serialization;

/// <summary>
/// API 对外 JSON 字段命名配置。
/// </summary>
public static class ApiJsonSerializerOptions
{
    /// <summary>
    /// 创建 API 手工序列化使用的 JSON 配置。
    /// </summary>
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        Configure(options);
        return options;
    }

    /// <summary>
    /// 统一配置控制器和手工序列化的字段命名策略。
    /// </summary>
    public static void Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.PropertyNameCaseInsensitive = true;
    }
}
