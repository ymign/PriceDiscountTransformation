using System.Threading;
using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 单次计价请求捕获的运行时包快照。
/// </summary>
/// <remarks>
/// 该快照只记录活动包指针，不直接缓存规则内容。规则内容仍按 PackageId 从运行时读模型读取，
/// 但同一请求内的匹配、计算和追溯持久化必须使用同一个活动包指针，避免发布切换竞态导致
/// “按旧包计算、按新包追溯”的错链。
/// </remarks>
public sealed class RuntimePackageTraceContext
{
    public long? ActivePackageId { get; init; }

    public long? ActivePackageVersion { get; init; }

    public static RuntimePackageTraceContext From(RuntimePackageState? state)
    {
        return new RuntimePackageTraceContext
        {
            ActivePackageId = state?.ActivePackageId > 0 ? state.ActivePackageId : null,
            ActivePackageVersion = state?.ActivePackageVersion > 0 ? state.ActivePackageVersion : null
        };
    }
}

/// <summary>
/// 运行时包追溯上下文访问器。
/// </summary>
/// <remarks>
/// 使用 AsyncLocal 保证同一异步调用链内可见，同时不会串到其他并发请求。该访问器由应用层
/// 在 workflow 开始时设置，规则加载器和追溯解析器读取同一份快照。
/// </remarks>
public sealed class RuntimePackageTraceContextAccessor
{
    private readonly AsyncLocal<RuntimePackageTraceContext?> _current = new();

    public RuntimePackageTraceContext? Current => _current.Value;

    public IDisposable Push(RuntimePackageTraceContext context)
    {
        var previous = _current.Value;
        _current.Value = context;
        return new RuntimePackageTraceContextScope(this, previous);
    }

    private sealed class RuntimePackageTraceContextScope : IDisposable
    {
        private readonly RuntimePackageTraceContextAccessor _accessor;
        private readonly RuntimePackageTraceContext? _previous;
        private bool _disposed;

        public RuntimePackageTraceContextScope(
            RuntimePackageTraceContextAccessor accessor,
            RuntimePackageTraceContext? previous)
        {
            _accessor = accessor;
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _accessor._current.Value = _previous;
            _disposed = true;
        }
    }
}
