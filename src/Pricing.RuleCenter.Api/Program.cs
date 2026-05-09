using Pricing.RuleCenter.Api.Engine;
using Pricing.RuleCenter.Api.Engine.Evaluators;
using Pricing.RuleCenter.Api.Engine.Executors;
using Pricing.RuleCenter.Api.Filters;
using Pricing.RuleCenter.Api.Services;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Infrastructure.Database;
using Pricing.RuleCenter.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var pricingOptions = new PricingOptions();
builder.Configuration.GetSection("Pricing").Bind(pricingOptions);
builder.Services.Configure<PricingOptions>(builder.Configuration.GetSection("Pricing"));

builder.Services.AddSqlSugarOracle(pricingOptions);

builder.Services.AddScoped<IDictRepository, DictRepository>();
builder.Services.AddScoped<IRuleHeaderRepository, RuleHeaderRepository>();
builder.Services.AddScoped<IRuleVersionRepository, RuleVersionRepository>();
builder.Services.AddScoped<IRuleConditionRepository, RuleConditionRepository>();
builder.Services.AddScoped<IRuleActionRepository, RuleActionRepository>();
builder.Services.AddScoped<IFormulaDefRepository, FormulaDefRepository>();
builder.Services.AddScoped<IRulePublishRepository, RulePublishRepository>();
builder.Services.AddScoped<IRuleChangeLogRepository, RuleChangeLogRepository>();
builder.Services.AddScoped<ILimitOccupyRepository, LimitOccupyRepository>();
builder.Services.AddScoped<IChargeRequestLogRepository, ChargeRequestLogRepository>();
builder.Services.AddScoped<IChargeDiscountDetailRepository, ChargeDiscountDetailRepository>();
builder.Services.AddScoped<IChargeTraceStepRepository, ChargeTraceStepRepository>();
builder.Services.AddScoped<IChargeReverseLogRepository, ChargeReverseLogRepository>();
builder.Services.AddScoped<IPriceMasterRepository, PriceMasterRepository>();

builder.Services.AddScoped<DictService>();
builder.Services.AddScoped<FormulaDefService>();
builder.Services.AddScoped<RuleHeaderService>();
builder.Services.AddScoped<RuleVersionService>();
builder.Services.AddScoped<RuleConditionService>();
builder.Services.AddScoped<RuleActionService>();
builder.Services.AddScoped<RulePublishService>();
builder.Services.AddScoped<PricingApiService>();
builder.Services.AddScoped<TraceQueryService>();
builder.Services.AddHostedService<ExpireCleanupService>();

builder.Services.AddScoped<IRuleConditionEvaluator, ItemMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, ChargeSceneMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, BodyPartMatchEvaluator>();
builder.Services.AddScoped<IRuleConditionEvaluator, TimeRangeEvaluator>();

builder.Services.AddScoped<IRuleActionExecutor, AmountFloorExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, AmountCeilingExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, IncrementPercentExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, TimeWindowLimitExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, DailyQtyLimitExecutor>();
builder.Services.AddScoped<IRuleActionExecutor, ExceedToZeroExecutor>();

builder.Services.AddScoped<ConditionEvaluatorFactory>();
builder.Services.AddScoped<ActionExecutorFactory>();
builder.Services.AddScoped<RuleMatchService>();
builder.Services.AddScoped<ActionExecutionPipeline>();
builder.Services.AddScoped<IPricingEngine, PricingEngine>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

var app = builder.Build();

app.MapControllers();

app.Run();
