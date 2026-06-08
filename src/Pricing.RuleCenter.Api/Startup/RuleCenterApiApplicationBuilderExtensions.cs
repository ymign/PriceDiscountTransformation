using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Pricing.RuleCenter.Api.HealthChecks;
using Pricing.RuleCenter.Api.Middleware;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Serilog;

namespace Pricing.RuleCenter.Api.Startup;

/// <summary>
/// API HTTP 请求管道配置扩展。
/// </summary>
/// <remarks>
/// <para>
/// 该类只负责中间件顺序、Swagger、认证授权、健康检查和控制器端点映射。
/// 服务注册放在 <see cref="RuleCenterApiServiceCollectionExtensions"/> 中，避免启动逻辑和依赖注册相互缠绕。
/// </para>
/// <para>
/// 中间件顺序对计价接口很关键：异常处理中间件必须在最前面，认证授权必须早于控制器映射，
/// 健康检查允许匿名访问，避免探针依赖业务 API Key。
/// </para>
/// </remarks>
internal static class RuleCenterApiApplicationBuilderExtensions
{
    /// <summary>
    /// 配置统一 API 响应使用的时钟。
    /// </summary>
    /// <param name="app">已构建的 Web 应用。</param>
    /// <returns>原始 Web 应用，便于链式调用。</returns>
    /// <remarks>
    /// <para>
    /// <see cref="ApiResult"/> 是 DTO 层的静态工厂，不能直接通过构造函数注入 <see cref="IClock"/>。
    /// 因此在应用启动后把 DI 容器中的时钟适配成委托，统一响应时间戳来源。
    /// </para>
    /// </remarks>
    public static WebApplication ConfigureRuleCenterApiResultClock(this WebApplication app)
    {
        var apiResultClock = app.Services.GetRequiredService<IClock>();
        ApiResultClock.Configure(() => apiResultClock.Now);

        return app;
    }

    /// <summary>
    /// 配置规则中心 API 的 HTTP 中间件管道和端点映射。
    /// </summary>
    /// <param name="app">已构建的 Web 应用。</param>
    /// <returns>原始 Web 应用，便于链式调用。</returns>
    public static WebApplication UseRuleCenterApiPipeline(this WebApplication app)
    {
        // 全局异常处理要放在最前面，确保业务异常、验证异常和未处理异常都能转成统一 ApiResult。
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "http_request_completed method={RequestMethod} path={RequestPath} status_code={StatusCode} elapsed_ms={Elapsed:0.0000}";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("trace_id", httpContext.TraceIdentifier);
                diagnosticContext.Set("request_method", httpContext.Request.Method);
                diagnosticContext.Set("request_path", httpContext.Request.Path.Value ?? string.Empty);
                diagnosticContext.Set("status_code", httpContext.Response.StatusCode);
            };
        });

        // Swagger 只按配置/环境开启。生产默认关闭，避免把管理接口暴露给非授权访问者。
        if (RuleCenterStartupInfo.ResolveSwaggerEnabled(app.Configuration, app.Environment.EnvironmentName))
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pricing RuleCenter API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        // 认证授权必须早于 MapControllers，否则 [Authorize] 策略不会在控制器执行前生效。
        app.UseAuthentication();
        app.UseAuthorization();

        // 健康检查用于本地联调、Docker 探针和部署探活，不依赖 API Key。
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        }).AllowAnonymous();
        app.MapControllers();

        return app;
    }
}
