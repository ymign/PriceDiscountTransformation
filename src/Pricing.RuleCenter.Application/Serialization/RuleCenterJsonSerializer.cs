using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Serialization;

/// <summary>
/// 规则中心内部 JSON 序列化入口。
/// </summary>
/// <remarks>
/// <para>
/// 该类型用于内部参数解析、请求快照、响应快照和指纹规范化。
/// </para>
/// <para>
/// API 对外响应仍由 <c>Pricing.RuleCenter.Api.Serialization.ApiJsonSerializerOptions</c>
/// 统一控制；这里不承载 HTTP 层命名策略。
/// </para>
/// </remarks>
internal static class RuleCenterJsonSerializer
{
    private static readonly JsonSerializerOptions s_options = CreateOptions();

    /// <summary>
    /// 获取规则中心内部统一 JSON 配置。
    /// </summary>
    public static JsonSerializerOptions Options => s_options;

    /// <summary>
    /// 将对象序列化为 JSON。
    /// </summary>
    /// <typeparam name="T">序列化对象类型。</typeparam>
    /// <param name="value">待序列化对象。</param>
    /// <returns>JSON 字符串。</returns>
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, s_options);

    /// <summary>
    /// 将 JSON 反序列化为目标类型。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="json">JSON 字符串。</param>
    /// <returns>反序列化结果；输入为空时返回默认值。</returns>
    public static T? Deserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, s_options);
    }

    /// <summary>
    /// 解析 JSON 文档。
    /// </summary>
    /// <param name="json">原始 JSON 字符串。</param>
    /// <returns>JSON 文档。</returns>
    public static JsonDocument ParseDocument(string json)
    {
        return JsonDocument.Parse(json, new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        });
    }

    /// <summary>
    /// 把旧快照中的 PascalCase 属性名重写为 snake_case。
    /// </summary>
    /// <param name="json">旧版快照 JSON。</param>
    /// <returns>属性名已规范化的新 JSON。</returns>
    public static string RewritePropertyNamesToSnakeCase(string json)
    {
        using var document = ParseDocument(json);
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        WriteElement(document.RootElement, writer, static name => ToSnakeCase(name));
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    /// <summary>
    /// 把 <see cref="JsonElement"/> 规整为无格式 JSON 文本。
    /// </summary>
    /// <param name="element">JSON 元素。</param>
    /// <returns>无格式 JSON 文本。</returns>
    public static string SerializeElement(JsonElement element)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        WriteElement(element, writer, propertyNameTransform: null);
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static void WriteElement(
        JsonElement element,
        Utf8JsonWriter writer,
        Func<string, string>? propertyNameTransform)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(propertyNameTransform?.Invoke(property.Name) ?? property.Name);
                    WriteElement(property.Value, writer, propertyNameTransform);
                }

                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteElement(item, writer, propertyNameTransform);
                }

                writer.WriteEndArray();
                break;

            default:
                element.WriteTo(writer);
                break;
        }
    }

    private static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length + 8);
        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];
            if (character == '_')
            {
                builder.Append(character);
                continue;
            }

            if (char.IsUpper(character))
            {
                if (index > 0 &&
                    builder.Length > 0 &&
                    builder[^1] != '_' &&
                    (!char.IsUpper(value[index - 1]) ||
                     (index + 1 < value.Length && char.IsLower(value[index + 1]))))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
                continue;
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}

/// <summary>
/// <see cref="JsonElement"/> 读取辅助扩展。
/// </summary>
internal static class RuleCenterJsonElementExtensions
{
    /// <summary>
    /// 大小写不敏感读取对象属性。
    /// </summary>
    /// <param name="element">JSON 对象。</param>
    /// <param name="propertyName">目标属性名。</param>
    /// <param name="value">命中时返回属性值。</param>
    /// <returns>是否命中属性。</returns>
    public static bool TryGetPropertyIgnoreCase(
        this JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// 判断元素是否为 JSON null/undefined。
    /// </summary>
    /// <param name="element">JSON 元素。</param>
    /// <returns>为 null 或 undefined 时返回 <c>true</c>。</returns>
    public static bool IsNullOrUndefined(this JsonElement element) =>
        element.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined;

    /// <summary>
    /// 读取十进制数值，兼容数字和数字字符串。
    /// </summary>
    /// <param name="element">JSON 元素。</param>
    /// <param name="value">读取成功时的十进制值。</param>
    /// <returns>是否读取成功。</returns>
    public static bool TryReadDecimal(this JsonElement element, out decimal value)
    {
        value = default;
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.TryGetDecimal(out value),
            JsonValueKind.String => decimal.TryParse(
                element.GetString(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value),
            _ => false
        };
    }

    /// <summary>
    /// 把 JSON 元素读取为字符串。
    /// </summary>
    /// <param name="element">JSON 元素。</param>
    /// <returns>字符串值；null/undefined 返回 <c>null</c>。</returns>
    public static string? ReadAsString(this JsonElement element)
    {
        if (element.IsNullOrUndefined())
        {
            return null;
        }

        return element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : element.GetRawText();
    }
}
