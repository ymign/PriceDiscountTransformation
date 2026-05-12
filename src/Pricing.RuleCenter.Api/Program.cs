using Pricing.RuleCenter.Api.Filters;
using Pricing.RuleCenter.Api.Services;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.Evaluators;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ========== 第一阶段：注册基础设施层 ==========
// DependencyInjection.AddInfrastructure 统一注册 SqlSugar、全部仓储、内存缓存和后台服务。
// Api 层无需直接 using Infrastructure 命名空间，也无需逐个注册仓储。
builder.Services.AddInfrastructure(builder.Configuration);

// ========== 第二阶段：注册应用服务 ==========
// 应用服务属于 Api 层，负责编排业务用例和事务边界，不属于基础设施层。
builder.Services.AddScoped<DictService>();
builder.Services.AddScoped<FormulaDefService>();
builder.Services.AddScoped<RuleHeaderService>();
builder.Services.AddScoped<RuleVersionService>();
builder.Services.AddScoped<RuleConditionService>();
builder.Services.AddScoped<RuleActionService>();
builder.Services.AddScoped<RulePublishLifecycleRepositories>();
builder.Services.AddScoped<RulePublishDefinitionRepositories>();
builder.Services.AddScoped<RulePublishService>();
builder.Services.AddScoped<PricingApiCalculationDependencies>();
builder.Services.AddScoped<PricingApiPersistenceRepositories>();
builder.Services.AddScoped<PricingApiService>();
builder.Services.AddScoped<TraceQueryService>();

// ExpireCleanupService 位于 Api 层，在此注册为后台服务。
// 它是 Singleton BackgroundService，通过 IServiceScopeFactory 创建 Scoped 依赖。
builder.Services.AddHostedService<ExpireCleanupService>();

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

// 动作执行器负责规则命中后的具体计算动作（金额上下限、数量限制、换算、公式等）。
// 每个执行器只处理一种动作类型，新增动作类型只需新增执行器，无需修改引擎主流程。
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

// 引擎核心组件：工厂负责按名称分发执行器，管道负责按优先级串联所有动作执行器。
builder.Services.AddScoped<ConditionEvaluatorFactory>();
builder.Services.AddScoped<ActionExecutorFactory>();
builder.Services.AddScoped<RuleMatchRepositories>();
builder.Services.AddScoped<RuleMatchService>();
builder.Services.AddScoped<ActionExecutionPipeline>();
builder.Services.AddScoped<IPricingEngine, PricingEngine>();

// ========== 第四阶段：注册健康检查 ==========
// 健康检查供负载均衡器和运维监控探测服务可用性。
// "oracle" 检查项验证数据库连接池是否可达，HealthController 会进一步做详细检查。
builder.Services.AddHealthChecks()
    .AddCheck("oracle", () =>
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Oracle connection OK"));

// ========== 第五阶段：配置结构化日志 ==========
// 清除默认日志提供程序，只保留 Console 输出，便于容器化部署时统一收集日志。
// 最低级别设为 Information，避免 Debug 级别的 SqlSugar SQL 日志污染生产环境。
// 开发调试时可通过 appsettings.json 将特定命名空间提升到 Debug 级别。
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ========== 第六阶段：注册控制器和全局异常过滤器 ==========
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

var app = builder.Build();

app.MapControllers();

app.Run();
