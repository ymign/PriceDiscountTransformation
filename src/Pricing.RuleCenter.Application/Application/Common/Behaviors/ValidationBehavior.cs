using FluentValidation;
using MediatR;
using ValidationException = Pricing.RuleCenter.Core.Exceptions.ValidationException;

namespace Pricing.RuleCenter.Application.Common.Behaviors;

/// <summary>
/// MediatR 参数验证管道，在 Handler 执行前统一运行 FluentValidation。
/// </summary>
/// <typeparam name="TRequest">MediatR 请求类型。</typeparam>
/// <typeparam name="TResponse">MediatR 响应类型。</typeparam>
/// <remarks>
/// 该管道位于 Controller 和 Handler 之间，负责统一把参数错误转成应用层验证异常。
/// workflow 内仍保留 Guard 校验，是为了保护内部调用和测试直接调用 workflow 时不绕过关键校验。
/// </remarks>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// 初始化验证管道。
    /// </summary>
    /// <param name="validators">当前 MediatR 请求类型注册的全部 FluentValidation 验证器。</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// 在 Handler 执行前运行全部验证器。
    /// </summary>
    /// <param name="request">MediatR 请求对象。</param>
    /// <param name="next">下一个管道节点，通常是具体 Handler。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>后续 Handler 的响应。</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            // 没有验证器时直接进入 Handler，避免无谓创建 ValidationContext。
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        // 多个验证器可能对同一属性返回重复消息，这里按属性分组并去重，便于 API 统一返回。
        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

        if (failures.Count > 0)
        {
            throw new ValidationException("请求参数校验失败", failures);
        }

        return await next();
    }
}
