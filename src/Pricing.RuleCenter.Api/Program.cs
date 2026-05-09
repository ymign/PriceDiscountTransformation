using Pricing.RuleCenter.Api.Filters;
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

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

var app = builder.Build();

app.MapControllers();

app.Run();
