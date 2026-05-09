using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pricing.RuleCenter.Api.Dto;

namespace Pricing.RuleCenter.Api.Filters;

/// <summary>
/// 全局异常过滤器，把应用层异常统一转换为标准 API 响应。
/// </summary>
/// <remarks>
/// 控制器和服务层可以抛出明确的业务异常，由该过滤器集中映射 HTTP 状态码。这样接口返回结构保持一致，
/// 同时避免每个控制器重复 try/catch。
/// </remarks>
public sealed class GlobalExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// 日志对象，用于记录未被业务代码显式处理的异常及请求路径。
    /// </summary>
    private readonly ILogger<GlobalExceptionFilter> _logger;

    /// <summary>
    /// 初始化全局异常过滤器。
    /// </summary>
    /// <param name="logger">日志对象。</param>
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理 MVC 管道中冒泡出来的异常。
    /// </summary>
    /// <param name="context">异常上下文，包含异常对象、HTTP 请求和结果写入入口。</param>
    public void OnException(ExceptionContext context)
    {
        // ========== 第一阶段：记录原始异常 ==========
        // 无论最终返回给调用方的是业务错误还是 500，都保留完整异常栈和请求路径，便于排查。
        _logger.LogError(context.Exception, "未处理异常: {Path}", context.HttpContext.Request.Path);

        // ========== 第二阶段：按异常类型映射业务响应 ==========
        // 参数错误、资源不存在和状态冲突是可预期业务错误；其他异常统一隐藏内部细节，返回 500。
        var response = context.Exception switch
        {
            ArgumentException ex => ApiResponse.Fail(ex.Message, 400),
            KeyNotFoundException ex => ApiResponse.Fail(ex.Message, 404),
            InvalidOperationException ex => ApiResponse.Fail(ex.Message, 409),
            _ => ApiResponse.Fail("服务器内部错误", 500)
        };

        // ========== 第三阶段：设置 HTTP 状态码 ==========
        // ApiResponse.Code 与 HTTP StatusCode 保持一致，方便前端既能按 HTTP 状态处理，也能读统一响应体。
        context.Result = new ObjectResult(response)
        {
            StatusCode = response.Code switch
            {
                400 => 400,
                404 => 404,
                409 => 409,
                _ => 500
            }
        };

        // ========== 第四阶段：终止异常继续冒泡 ==========
        // 标记已处理后，MVC 不会再把异常交给默认异常处理中间件生成另一套响应结构。
        context.ExceptionHandled = true;
    }
}
