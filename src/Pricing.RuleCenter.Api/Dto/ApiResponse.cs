namespace Pricing.RuleCenter.Api.Dto;

public sealed class ApiResponse<T>
{
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "success")
    {
        return new ApiResponse<T> { Code = 0, Message = message, Data = data };
    }

    public static ApiResponse<T> Fail(string message, int code = -1)
    {
        return new ApiResponse<T> { Code = code, Message = message };
    }
}

public sealed class ApiResponse
{
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? TraceId { get; init; }

    public static ApiResponse Ok(string message = "success")
    {
        return new ApiResponse { Code = 0, Message = message };
    }

    public static ApiResponse Fail(string message, int code = -1)
    {
        return new ApiResponse { Code = code, Message = message };
    }
}
