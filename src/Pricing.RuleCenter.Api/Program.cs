using Pricing.RuleCenter.Api.Startup;
using Serilog;

// ========== 第一阶段：创建启动期日志 ==========
// 在完整依赖注入容器创建前，先准备 bootstrap logger，保证配置绑定或 DI 注册阶段失败时也能落日志。
Log.Logger = RuleCenterLoggingExtensions.CreateBootstrapLogger();

WebApplication? app = null;
try
{
    // ========== 第二阶段：构建宿主与注册服务 ==========
    // 配置读取顺序保持 ASP.NET Core 默认行为，并显式追加环境变量，便于 Docker、CI 和本地脚本覆盖敏感配置。
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables();
    builder.Host.AddRuleCenterLogging();

    RuleCenterLoggingExtensions.LogApplicationStarting(builder.Configuration, builder.Environment);

    builder.Services.AddRuleCenterApi(builder.Configuration);

    // ========== 第三阶段：组装 HTTP 管道并启动 ==========
    // 管道配置被拆到 Startup 扩展类中，Program 只保留启动编排，避免入口文件重新膨胀。
    app = builder.Build();
    app.ConfigureRuleCenterApiResultClock();
    app.UseRuleCenterApiPipeline();
    app.LogApplicationStartedOnStarted();

    app.Run();
}
catch (Exception ex)
{
    // 启动失败通常发生在配置校验、Oracle 连接串缺失、DI 注册错误等阶段，必须记录完整异常。
    RuleCenterLoggingExtensions.LogApplicationStartupFailed(ex, app);
    throw;
}
finally
{
    // 正常关闭和启动失败都要 flush Serilog，否则控制台/文件 sink 可能丢失尾部日志。
    RuleCenterLoggingExtensions.LogApplicationStopped(app);
    Log.CloseAndFlush();
}

/// <summary>
/// API 程序入口类型，供 WebApplicationFactory 集成测试定位宿主程序集。
/// </summary>
public partial class Program
{
}
