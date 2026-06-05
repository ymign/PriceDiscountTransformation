using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Exceptions;

namespace Pricing.RuleCenter.Api.Filters;

/// <summary>
/// 全局异常过滤器，把应用层异常统一转换为标准 API 响应。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 拦截 MVC 管道中冒泡出来的未处理异常，按异常类型映射为不同 HTTP 状态码的 ApiResponse。
/// 控制器和服务层可以抛出明确的业务异常，由该过滤器集中映射 HTTP 状态码。
/// </para>
/// <para>
/// 【设计目的】
///   1. 接口返回结构保持一致 —— 所有错误响应都是 ApiResponse 格式
///   2. 避免每个控制器重复 try/catch —— 业务异常集中处理
///   3. 隐藏内部实现细节 —— 非业务异常统一返回"服务器内部错误"
///   4. 保留完整审计日志 —— 所有异常都记录 Error 级别日志
/// </para>
/// <para>
/// 【异常分类映射】
///   - ArgumentException → 400 Bad Request（参数校验失败）
///   - KeyNotFoundException → 404 Not Found（资源不存在）
///   - InvalidOperationException → 409 Conflict（状态冲突，如重复确认）
///   - 其他异常 → 500 Internal Server Error（隐藏内部细节）
/// </para>
/// <para>
/// 【注册方式】
/// 在 Program.cs 或 Startup.cs 中注册为全局过滤器：
/// <code>
/// builder.Services.AddControllers(options =>
/// {
///     options.Filters.Add&lt;GlobalExceptionFilter&gt;();
/// });
/// </code>
/// </para>
/// </remarks>
public sealed class GlobalExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// 日志对象，用于记录未被业务代码显式处理的异常及请求路径。
    /// </summary>
    /// <remarks>
    /// 使用 ILogger&lt;GlobalExceptionFilter&gt;，日志类别自动为过滤器类的全限定名。
    /// </remarks>
    private readonly ILogger<GlobalExceptionFilter> _logger;

    /// <summary>
    /// 初始化全局异常过滤器。
    /// </summary>
    /// <param name="logger">日志对象，由 DI 容器注入。</param>
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理 MVC 管道中冒泡出来的异常。
    /// </summary>
    /// <param name="context">
    /// 异常上下文，包含：
    ///   - Exception — 原始异常对象
    ///   - HttpContext — HTTP 请求信息（路径、方法、头部等）
    ///   - Result — 写入响应结果的入口
    /// </param>
    /// <remarks>
    /// 【处理流程】
    ///   第一阶段：记录原始异常（无论最终返回什么状态码）
    ///   第二阶段：按异常类型映射业务响应
    ///   第三阶段：设置 HTTP 状态码
    ///   第四阶段：标记异常已处理，阻止继续冒泡
    /// </remarks>
    public void OnException(ExceptionContext context)
    {
        // ========== 第一阶段：按异常类型分级记录日志 ==========
        // 业务异常（参数错误、资源不存在）使用 Warning 级别，避免大量 Error 日志淹没真正的系统故障。
        if (context.Exception is ArgumentException or KeyNotFoundException or BizException or LimitLockException)
        {
            _logger.LogWarning(context.Exception, "业务异常: {Path}", context.HttpContext.Request.Path);
        }
        else
        {
            _logger.LogError(context.Exception, "未处理异常: {Path}", context.HttpContext.Request.Path);
        }

        // ========== 第二阶段：按异常类型映射业务响应 ==========
        // 参数错误、资源不存在和状态冲突是可预期业务错误；其他异常统一隐藏内部细节，返回 500。
        // 这样前端可以按 HTTP 状态码做分支处理，同时 ApiResponse.Code 提供更细粒度的错误码。
        var response = context.Exception switch
        {
            BizException ex => ApiResponse.Fail(ex.Code, ex.Message),
            LimitLockException ex => ApiResponse.Fail(
                ex.IsConcurrencyConflict ? BizErrorCode.ConcurrencyConflict : BizErrorCode.LimitLockFailed,
                ex.Message),
            // ArgumentException — 参数校验失败，如缺少必填字段、格式不合法
            ArgumentException ex => ApiResponse.Fail(ex.Message, 400),
            // KeyNotFoundException — 资源不存在，如按ID查询无结果
            KeyNotFoundException ex => ApiResponse.Fail(ex.Message, 404),
            // InvalidOperationException — 状态冲突，如重复确认、状态不允许流转
            InvalidOperationException ex => ApiResponse.Fail(ex.Message, 409),
            // 其他异常 — 隐藏内部细节，返回通用错误信息
            _ => ApiResponse.Fail("服务器内部错误", 500)
        };

        // ========== 第三阶段：设置 HTTP 状态码 ==========
        // ApiResponse.Code 与 HTTP StatusCode 保持一致，方便前端既能按 HTTP 状态处理，也能读统一响应体。
        // 前端推荐优先使用 HTTP 状态码做分支，ApiResponse.Message 做用户提示。
        context.Result = new ObjectResult(response)
        {
            StatusCode = context.Exception switch
            {
                BizException ex => ex.HttpStatusCode,
                LimitLockException ex => ex.IsConcurrencyConflict ? 409 : 500,
                _ => response.Code switch
                {
                    400 => 400,
                    404 => 404,
                    409 => 409,
                    _ => 500
                }
            }
        };

        // ========== 第四阶段：终止异常继续冒泡 ==========
        // 标记已处理后，MVC 不会再把异常交给默认异常处理中间件生成另一套响应结构。
        // 如果不标记，ASP.NET Core 的 DeveloperExceptionPageMiddleware 或
        // ExceptionHandlerMiddleware 可能会生成不一致的错误响应格式。
        context.ExceptionHandled = true;
    }
}

