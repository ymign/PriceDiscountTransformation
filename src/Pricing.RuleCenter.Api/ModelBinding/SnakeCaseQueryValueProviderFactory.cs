using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Pricing.RuleCenter.Api.ModelBinding;

/// <summary>
/// 允许查询字符串使用 snake_case 参数名绑定到 C# PascalCase/camelCase 参数。
/// </summary>
public sealed class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
{
    /// <inheritdoc />
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        var query = context.ActionContext.HttpContext.Request.Query;
        if (query.Count > 0)
        {
            var inner = new QueryStringValueProvider(
                BindingSource.Query,
                query,
                CultureInfo.InvariantCulture);
            context.ValueProviders.Insert(0, new SnakeCaseQueryValueProvider(inner));
        }

        return Task.CompletedTask;
    }

    private sealed class SnakeCaseQueryValueProvider : IBindingSourceValueProvider
    {
        private readonly QueryStringValueProvider _inner;

        public SnakeCaseQueryValueProvider(QueryStringValueProvider inner)
        {
            _inner = inner;
        }

        public bool ContainsPrefix(string prefix)
        {
            return _inner.ContainsPrefix(prefix) ||
                   _inner.ContainsPrefix(ConvertQueryKey(prefix));
        }

        public ValueProviderResult GetValue(string key)
        {
            var value = _inner.GetValue(key);
            return value == ValueProviderResult.None
                ? _inner.GetValue(ConvertQueryKey(key))
                : value;
        }

        public IValueProvider? Filter(BindingSource bindingSource)
        {
            return bindingSource.CanAcceptDataFrom(BindingSource.Query) ? this : null;
        }
    }

    private static string ConvertQueryKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        var parts = key.Split('.');
        for (var i = 0; i < parts.Length; i++)
        {
            parts[i] = ConvertSegment(parts[i]);
        }

        return string.Join('.', parts);
    }

    private static string ConvertSegment(string segment)
    {
        var bracketIndex = segment.IndexOf('[', StringComparison.Ordinal);
        if (bracketIndex < 0)
        {
            return JsonNamingPolicy.SnakeCaseLower.ConvertName(segment);
        }

        var name = segment[..bracketIndex];
        return JsonNamingPolicy.SnakeCaseLower.ConvertName(name) + segment[bracketIndex..];
    }
}
