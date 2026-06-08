using Serilog;
using Serilog.Events;

namespace Pricing.RuleCenter.Api.Startup;

internal static class RuleCenterLoggingExtensions
{
    public static Serilog.ILogger CreateBootstrapLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/yyyy-MM-dd.log", rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();
    }

    public static ConfigureHostBuilder AddRuleCenterLogging(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/yyyy-MM-dd.log", rollingInterval: RollingInterval.Day);
        });

        return host;
    }

    public static void LogApplicationStarting(IConfiguration configuration, IWebHostEnvironment environment)
    {
        LogStartupEvent("应用启动中", RuleCenterStartupInfo.Create(configuration, environment));
    }

    public static void LogApplicationStartedOnStarted(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            LogStartupEvent(
                "应用启动完成",
                RuleCenterStartupInfo.Create(app.Configuration, app.Environment, app.Urls));
        });
    }

    public static void LogApplicationStartupFailed(Exception exception, WebApplication? app)
    {
        if (app is null)
        {
            Log.Fatal(exception, "应用启动失败");
            return;
        }

        LogStartupEvent(
            LogEventLevel.Fatal,
            exception,
            "应用启动失败",
            RuleCenterStartupInfo.Create(app.Configuration, app.Environment, app.Urls));
    }

    public static void LogApplicationStopped(WebApplication? app)
    {
        if (app is null)
        {
            return;
        }

        LogStartupEvent(
            "应用已停止",
            RuleCenterStartupInfo.Create(app.Configuration, app.Environment, app.Urls));
    }

    private static void LogStartupEvent(string eventName, RuleCenterStartupInfo info)
    {
        LogStartupEvent(LogEventLevel.Information, exception: null, eventName, info);
    }

    private static void LogStartupEvent(
        LogEventLevel level,
        Exception? exception,
        string eventName,
        RuleCenterStartupInfo info)
    {
        Log.Write(
            level,
            exception,
            "启动事件={event_name} 服务名称={service_name} 运行环境={environment} 内容根目录={content_root} 监听地址={urls} Swagger是否启用={swagger_enabled} 构建提交={build_commit} 构建分支={build_branch} 构建时间UTC={build_time_utc}",
            eventName,
            info.ServiceName,
            info.Environment,
            info.ContentRoot,
            info.Urls,
            info.SwaggerEnabled,
            info.BuildCommit,
            info.BuildBranch,
            info.BuildTimeUtc);
    }
}
