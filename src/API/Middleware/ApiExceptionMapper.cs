using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Exceptions;
using ValidationException = Pricing.RuleCenter.Core.Exceptions.ValidationException;

namespace Pricing.RuleCenter.Api.Middleware;

/// <summary>
/// API 异常到统一错误响应的映射器。
/// </summary>
public static class ApiExceptionMapper
{
    /// <summary>
    /// 将异常映射为 HTTP 状态码、业务错误码、消息和字段级错误。
    /// </summary>
    public static ApiExceptionMapping Map(Exception exception)
    {
        return exception switch
        {
            BizException ex => new ApiExceptionMapping(ex.HttpStatusCode, ex.Code, GetBizExceptionClientMessage(ex)),
            ValidationException ex => new ApiExceptionMapping(400, ex.Code, ex.Message, ex.Errors),
            NotFoundException ex => new ApiExceptionMapping(404, ex.Code, ex.Message),
            DomainException ex => new ApiExceptionMapping(409, ex.Code, ex.Message),
            LimitLockException ex => new ApiExceptionMapping(
                ex.IsConcurrencyConflict ? 409 : 500,
                ex.IsConcurrencyConflict ? BizErrorCode.ConcurrencyConflict : BizErrorCode.LimitLockFailed,
                GetLimitLockClientMessage(ex)),
            ArgumentException => new ApiExceptionMapping(400, 400, "请求参数不合法"),
            KeyNotFoundException => new ApiExceptionMapping(404, 404, "资源不存在"),
            InvalidOperationException => new ApiExceptionMapping(409, 409, "当前状态不允许执行该操作"),
            _ => new ApiExceptionMapping(500, 500, "服务器内部错误")
        };
    }

    private static string GetBizExceptionClientMessage(BizException exception)
    {
        return exception.Code switch
        {
            BizErrorCode.PriceMismatch => "单价与权威物价不一致",
            BizErrorCode.IdempotencyResponseSnapshotInvalid => "幂等响应快照异常，请联系管理员处理",
            BizErrorCode.CommitDetailMismatch
                or BizErrorCode.CommitQtyMismatch
                or BizErrorCode.CommitAmountMismatch => "HIS实际落账明细与确认计价结果不一致",
            _ => exception.Message
        };
    }

    private static string GetLimitLockClientMessage(LimitLockException exception)
    {
        return exception.IsConcurrencyConflict
            ? "限额锁竞争失败，请稍后重试"
            : "限额锁处理失败";
    }
}

/// <summary>
/// API 异常映射结果。
/// </summary>
public sealed record ApiExceptionMapping(
    int StatusCode,
    int Code,
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors = null);
