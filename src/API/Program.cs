using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation;
using Pricing.RuleCenter.Api.HealthChecks;
using Pricing.RuleCenter.Api.Middleware;
using Pricing.RuleCenter.Application.Common;
using Pricing.RuleCenter.Application.Common.Behaviors;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.Pricing.UseCases;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Application.Rules.Publishing;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Catalog;
using Pricing.RuleCenter.Application.Templates;
using Pricing.RuleCenter.Application.Trace;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Api.Security;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.Evaluators;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Pricing.RuleCenter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((context, _, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/yyyy-MM-dd.log", rollingInterval: RollingInterval.Day);
});

// ========== 第一阶段：注册基础设施层 ==========
// DependencyInjection.AddInfrastructure 统一注册 SqlSugar、全部仓储、内存缓存和后台服务。
// Api 层无需直接 using Infrastructure 命名空间，也无需逐个注册仓储。
builder.Services.AddInfrastructure(builder.Configuration);

// ========== 第二阶段：注册应用服务 ==========
// 应用服务属于 Api 层，负责编排业务用例和事务边界，不属于基础设施层。
builder.Services.AddScoped<DictAppService>();
builder.Services.AddScoped<FormulaDefAppService>();
builder.Services.AddScoped<RuleHeaderAppService>();
builder.Services.AddScoped<RuleVersionAppService>();
builder.Services.AddScoped<RuleConditionAppService>();
builder.Services.AddScoped<RuleActionAppService>();
builder.Services.AddScoped<RuleEditGuard>();
builder.Services.AddScoped<RuleApprovalAppService>();
builder.Services.AddScoped<RulePublishLifecycleRepositories>();
builder.Services.AddScoped<RulePublishDefinitionRepositories>();
builder.Services.AddScoped<RulePublishTransactionWriter>();
builder.Services.AddScoped<RulePublishCacheInvalidator>();
builder.Services.AddScoped<RuleCacheInvalidationOutboxProcessor>();
builder.Services.AddScoped<FormulaFunctionRegistry>();
builder.Services.AddScoped<FormulaExpressionEvaluator>();
builder.Services.AddScoped<FormulaExpressionValidator>();
builder.Services.AddScoped<RuleActionParameterValidator>();
builder.Services.AddScoped<RuleCapabilityRegistry>();
builder.Services.AddScoped<RuleCapabilityGuard>();
builder.Services.AddScoped<RuleCriticalActionGuard>();
builder.Services.AddScoped<RuleChildItemGuard>();
builder.Services.AddScoped<RuleTestCaseGate>();
builder.Services.AddScoped<RuleApprovalGate>();
builder.Services.AddScoped<RuleConflictDetector>();
builder.Services.AddScoped<RulePublishGuard>();
builder.Services.AddScoped<PublishRuleUseCase>();
builder.Services.AddScoped<DisableRuleUseCase>();
builder.Services.AddScoped<RollbackRuleUseCase>();
builder.Services.AddScoped<RulePublishAppService>();
builder.Services.AddScoped<RuleCacheOutboxAppService>();
builder.Services.AddScoped<TemplateAppService>();
builder.Services.AddScoped<TemplateVersionAppService>();
builder.Services.AddScoped<PolicyExpressionGuard>();
builder.Services.AddScoped<PolicyAppService>();
builder.Services.AddScoped<PolicyImportService>();
builder.Services.AddScoped<PolicyVersionAppService>();
builder.Services.AddScoped<PolicyPreviewAppService>();
builder.Services.AddScoped<PolicyPriorityKeyFactory>();
builder.Services.AddScoped<PolicyValidationService>();
builder.Services.AddScoped<PolicyConflictService>();
builder.Services.AddScoped<PolicyPublishProfileResolver>();
builder.Services.AddScoped<PolicyPublishEligibilityService>();
builder.Services.AddScoped<PolicyReviewAppService>();
builder.Services.AddScoped<PolicyPackageDiffService>();
builder.Services.AddScoped<RuntimeRuleProjectionFactory>();
builder.Services.AddScoped<RuntimePackageCompiler>();
builder.Services.AddScoped<RuntimePackageTraceResolver>();
builder.Services.AddScoped<RuntimePackageQueryAppService>();
builder.Services.AddScoped<RuntimePackageActivationService>();
builder.Services.AddScoped<RuntimePackageRollbackService>();
builder.Services.AddScoped<RuntimePackagePublishService>();
builder.Services.AddScoped<PricingAppCalculationDependencies>();
builder.Services.AddScoped<PricingAppPersistenceRepositories>();
builder.Services.AddScoped<AuthorityPriceChecker>();
builder.Services.AddScoped<PricingIdempotencyService>();
builder.Services.AddScoped<PricingRequestLogWriter>();
builder.Services.AddScoped<PricingTraceStepWriter>();
builder.Services.AddScoped<PricingDiscountDetailWriter>();
builder.Services.AddScoped<PricingLimitOccupyWriter>();
builder.Services.AddScoped<PricingReverseLogWriter>();
builder.Services.AddScoped<SimulatePricingUseCase>();
builder.Services.AddScoped<ConfirmPricingUseCase>();
builder.Services.AddScoped<CommitPricingUseCase>();
builder.Services.AddScoped<CancelPricingUseCase>();
builder.Services.AddScoped<ReversePricingUseCase>();
builder.Services.AddScoped<GetSpecialFlagUseCase>();
builder.Services.AddScoped<PricingAppService>();
builder.Services.AddScoped<TraceQueryAppService>();
builder.Services.AddSingleton<CacheVersionLocalState>();
builder.Services.AddScoped<ICacheVersionSynchronizer, CacheVersionSynchronizer>();
builder.Services.AddMediatR(typeof(ApplicationAssemblyMarker));
builder.Services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ========== 第二点五阶段：注册认证和授权 ==========
// 规则中心是服务间 API，不在代码层依赖外部网关兜底。默认所有控制器都要求 API Key；
// 规则维护/发布类接口再通过 policy 限定管理员角色。
builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.SchemeName)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationOptions.SchemeName,
        options =>
        {
            options.HeaderName = builder.Configuration["Authentication:ApiKey:HeaderName"]
                ?? ApiKeyAuthenticationOptions.DefaultHeaderName;

            foreach (var item in builder.Configuration.GetSection("Authentication:ApiKey:Keys").GetChildren())
            {
                var key = item["Key"];
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var credential = new ApiKeyCredential
                {
                    Key = key,
                    Name = item["Name"] ?? "api-key"
                };
                credential.Roles.AddRange(item.GetSection("Roles")
                    .GetChildren()
                    .Select(role => role.Value)
                    .Where(role => !string.IsNullOrWhiteSpace(role))
                    .Select(role => role!));
                options.Keys.Add(credential);
            }
        });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PricingService", policy =>
    {
        policy.AddAuthenticationSchemes(ApiKeyAuthenticationOptions.SchemeName);
        policy.RequireRole("pricing.service", "pricing.admin");
    });
    options.AddPolicy("RuleAdmin", policy =>
    {
        policy.AddAuthenticationSchemes(ApiKeyAuthenticationOptions.SchemeName);
        policy.RequireRole("pricing.admin");
    });
});

// ExpireCleanupService 位于 Api 层，在此注册为后台服务。
// 它是 Singleton BackgroundService，通过 IServiceScopeFactory 创建 Scoped 依赖。
builder.Services.AddHostedService<ExpireCleanupAppService>();
builder.Services.AddHostedService<CacheVersionSyncService>();

// ========== 第三阶段：注册规则引擎和执行器 ==========
// 条件执行器负责规则条件匹配（项目、场景、部位、时间等维度）。
// 多个执行器注册为同一接口的集合，由 ConditionEvaluatorFactory 按名称分发。
builder.Services.AddScoped<IRuleConditionEvaluator, ItemMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, ChargeSceneMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, BodyPartMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, TimeRangeEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, PregnancyMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, VisitTypeMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, AgeMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, GroupMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, ChargeDeptExcludeEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, InsuranceTypeMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, DiagnosisMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, DeviceTypeMatchEvaluator>();

// 动作执行器负责规则命中后的具体计算动作（金额上下限、数量限制、换算、公式等）。
// 动作类型是一级分派；FORMULA_CALC 等大类可注册多个执行器，再由 ExecutorCode 做二级分派。
// 新增动作或公式类型只需新增执行器并注册，无需修改引擎主流程。
builder.Services.AddScoped<IRuleActionExecutor, AmountFloorExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, AmountCeilingExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, IncrementPercentExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, TimeWindowLimitExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, DailyQtyLimitExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, OnceQtyLimitExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, ExceedToZeroExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, UnitConvertExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, SameGroupMutexExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, SameOperationCeilingExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, AddChildItemExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, AreaStepIncrementExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, ConvertQtyByPartExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, ChildItemPercentExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, ExpressionFormulaExecutor>();

// 引擎核心组件：工厂负责按名称分发执行器，管道负责按优先级串联所有动作执行器。
builder.Services.AddScoped<ConditionEvaluatorFactory>();
builder.Services.AddScoped<ActionExecutorFactory>();
builder.Services.AddScoped(provider => new RuleMatchRepositories(
    provider.GetRequiredService<IRuleHeaderRepository>(),
    provider.GetRequiredService<IRuleConditionRepository>(),
    provider.GetRequiredService<IRuleActionRepository>(),
    provider.GetRequiredService<IDictRepository>(),
    provider.GetService<IRuntimePackageStateRepository>(),
    provider.GetService<IRuntimeRuleReadRepository>()));
builder.Services.AddScoped<EffectiveRuleSnapshotLoader>();
builder.Services.AddScoped<EffectiveRuleSnapshotCache>();
builder.Services.AddScoped<RuleMatchService>();
builder.Services.AddScoped<IRuleRuntimeCacheInvalidator>(provider =>
    provider.GetRequiredService<RuleMatchService>());
builder.Services.AddScoped<ActionExecutionPipeline>();
builder.Services.AddScoped<IPricingEngine, PricingEngine>();

// ========== 第四阶段：注册健康检查 ==========
// 健康检查供负载均衡器和运维监控探测服务可用性。
// "database:oracle" 会真实执行 Oracle 探测 SQL；"third-party-services" 会探测配置中的关键第三方服务。
builder.Services.AddHttpClient("health-checks");
builder.Services.AddHealthChecks()
    .AddCheck<OracleHealthCheck>("database:oracle")
    .AddCheck<ThirdPartyServicesHealthCheck>("third-party-services");

// ========== 第五阶段：注册控制器、Swagger 和全局异常处理中间件 ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var title = builder.Configuration["Swagger:Title"] ?? "Pricing RuleCenter API";
    var version = builder.Configuration["Swagger:Version"] ?? "v1";
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = title,
        Version = version,
        Description = "医院物价折价规则中心 API，按 DDD 边界展示计价、规则、字典、追溯和运维接口。"
    });

    options.TagActionsBy(api =>
    {
        var controller = api.ActionDescriptor.RouteValues["controller"] ?? "API";
        return new[]
        {
            controller switch
            {
                "Pricing" => "Application - 计价用例",
                "RuleHeader" or "RuleVersion" or "RuleCondition" or "RuleAction" or "RuleApproval" or "RulePublish" => "Application - 规则生命周期",
                "Template" or "Policy" or "RuntimePackage" => "Application - 新规则平台",
                "Dict" or "FormulaDef" => "Application - 基础配置",
                "Trace" => "Application - 计价追溯",
                "Health" => "API - 运维健康检查",
                _ => $"API - {controller}"
            }
        };
    });

    var xmlFiles = new[]
    {
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
        "Pricing.RuleCenter.Application.xml"
    };
    foreach (var xmlFile in xmlFiles)
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    }
});

var app = builder.Build();

var apiResultClock = app.Services.GetRequiredService<IClock>();
ApiResultClock.Configure(() => apiResultClock.Now);

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseSerilogRequestLogging();

var swaggerEnabledSetting = app.Configuration["Swagger:Enabled"];
var swaggerEnabled = string.IsNullOrWhiteSpace(swaggerEnabledSetting)
    ? app.Environment.IsDevelopment()
    : bool.TryParse(swaggerEnabledSetting, out var enabled) && enabled;
if (swaggerEnabled)
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

app.Run();

/// <summary>
/// API 程序入口类型，供 WebApplicationFactory 集成测试定位宿主程序集。
/// </summary>
public partial class Program
{
}
