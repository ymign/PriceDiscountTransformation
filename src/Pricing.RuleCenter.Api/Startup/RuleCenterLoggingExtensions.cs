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
        LogStartupEvent("application_starting", RuleCenterStartupInfo.Create(configuration, environment));
    }

    public static void LogApplicationStartedOnStarted(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            LogStartupEvent(
                "application_started",
                RuleCenterStartupInfo.Create(app.Configuration, app.Environment, app.Urls));
        });
    }

    public static void LogApplicationStartupFailed(Exception exception, WebApplication? app)
    {
        if (app is null)
        {
            Log.Fatal(exception, "application_startup_failed");
            return;
        }

        LogStartupEvent(
            LogEventLevel.Fatal,
            exception,
            "application_startup_failed",
            RuleCenterStartupInfo.Create(app.Configuration, app.Environment, app.Urls));
    }

    public static void LogApplicationStopped(WebApplication? app)
    {
        if (app is null)
        {
            return;
        }

        LogStartupEvent(
            "application_stopped",
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
            "{event_name} service_name={service_name} environment={environment} content_root={content_root} urls={urls} swagger_enabled={swagger_enabled} build_commit={build_commit} build_branch={build_branch} build_time_utc={build_time_utc}",
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
