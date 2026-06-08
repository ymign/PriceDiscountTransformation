using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Pricing.RuleCenter.Api.HealthChecks;
using Pricing.RuleCenter.Api.Middleware;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Serilog;

namespace Pricing.RuleCenter.Api.Startup;

internal static class RuleCenterApiApplicationBuilderExtensions
{
    public static WebApplication ConfigureRuleCenterApiResultClock(this WebApplication app)
    {
        var apiResultClock = app.Services.GetRequiredService<IClock>();
        ApiResultClock.Configure(() => apiResultClock.Now);

        return app;
    }

    public static WebApplication UseRuleCenterApiPipeline(this WebApplication app)
    {
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        }).AllowAnonymous();
        app.MapControllers();

        return app;
    }
}
