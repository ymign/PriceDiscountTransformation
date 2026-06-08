using Pricing.RuleCenter.Api.Startup;
using Serilog;

Log.Logger = RuleCenterLoggingExtensions.CreateBootstrapLogger();

WebApplication? app = null;
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables();
    builder.Host.AddRuleCenterLogging();

    RuleCenterLoggingExtensions.LogApplicationStarting(builder.Configuration, builder.Environment);

    builder.Services.AddRuleCenterApi(builder.Configuration);

    app = builder.Build();
    app.ConfigureRuleCenterApiResultClock();
    app.UseRuleCenterApiPipeline();
    app.LogApplicationStartedOnStarted();

    app.Run();
}
catch (Exception ex)
{
    RuleCenterLoggingExtensions.LogApplicationStartupFailed(ex, app);
    throw;
}
finally
{
    RuleCenterLoggingExtensions.LogApplicationStopped(app);
    Log.CloseAndFlush();
}

/// <summary>
/// API 程序入口类型，供 WebApplicationFactory 集成测试定位宿主程序集。
/// </summary>
public partial class Program
{
}
