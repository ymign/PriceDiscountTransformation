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
            BizException ex => new ApiExceptionMapping(
                ex.HttpStatusCode,
                ex.Code,
                GetBizExceptionClientMessage(ex),
                GetMachineReadableErrorCode(ex.Code)),
            ValidationException ex => new ApiExceptionMapping(400, ex.Code, ex.Message, "INVALID_REQUEST", ex.Errors),
            NotFoundException ex => new ApiExceptionMapping(404, ex.Code, ex.Message, GetMachineReadableErrorCode(ex.Code)),
            DomainException ex => new ApiExceptionMapping(409, ex.Code, ex.Message, GetMachineReadableErrorCode(ex.Code)),
            LimitLockException ex => new ApiExceptionMapping(
                ex.IsConcurrencyConflict ? 409 : 500,
                ex.IsConcurrencyConflict ? BizErrorCode.ConcurrencyConflict : BizErrorCode.LimitLockFailed,
                GetLimitLockClientMessage(ex),
                ex.IsConcurrencyConflict ? "CONCURRENCY_CONFLICT" : "PRICING_SERVICE_UNAVAILABLE"),
            ArgumentException => new ApiExceptionMapping(400, 400, "请求参数不合法", "INVALID_REQUEST"),
            KeyNotFoundException => new ApiExceptionMapping(404, 404, "资源不存在", "NOT_FOUND"),
            InvalidOperationException => new ApiExceptionMapping(409, 409, "当前状态不允许执行该操作", "INVALID_STATE"),
            _ => new ApiExceptionMapping(500, 500, "服务器内部错误", "PRICING_SERVICE_UNAVAILABLE")
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

    private static string GetMachineReadableErrorCode(int code)
    {
        return code switch
        {
            400 => "INVALID_REQUEST",
            404 => "NOT_FOUND",
            409 => "INVALID_STATE",
            500 => "PRICING_SERVICE_UNAVAILABLE",
            BizErrorCode.BusinessRequestNoDuplicated => "BUSINESS_REQUEST_NO_DUPLICATED",
            BizErrorCode.IdempotencyConflict => "IDEMPOTENT_CONFLICT",
            BizErrorCode.CommitDetailMismatch
                or BizErrorCode.CommitQtyMismatch
                or BizErrorCode.CommitAmountMismatch
                or BizErrorCode.CommitActualItemsRequired
                or BizErrorCode.CommitDetailNotFound => "COMMIT_DETAIL_MISMATCH",
            BizErrorCode.ReverseNotAllowed => "REVERSE_NOT_ALLOWED",
            BizErrorCode.ServiceDegraded
                or BizErrorCode.DatabaseError
                or BizErrorCode.LimitLockFailed => "PRICING_SERVICE_UNAVAILABLE",
            _ => code >= 1000 && code < 2000 ? "RULE_CONFIG_ERROR" :
                 code >= 2000 && code < 3000 ? "PRICING_BUSINESS_ERROR" :
                 code >= 3000 && code < 4000 ? "INVALID_STATE" :
                 code >= 4000 && code < 5000 ? "CONCURRENCY_CONFLICT" :
                 "PRICING_SERVICE_UNAVAILABLE"
        };
    }
}

/// <summary>
/// API 异常映射结果。
/// </summary>
public sealed record ApiExceptionMapping(
    int StatusCode,
    int Code,
    string Message,
    string ErrorCode,
    IReadOnlyDictionary<string, string[]>? Errors = null);
