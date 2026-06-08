using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using Pricing.RuleCenter.Api.HealthChecks;
using Pricing.RuleCenter.Api.ModelBinding;
using Pricing.RuleCenter.Api.Security;
using Pricing.RuleCenter.Api.Serialization;
using Pricing.RuleCenter.Api.Swagger;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Catalog;
using Pricing.RuleCenter.Application.Common;
using Pricing.RuleCenter.Application.Common.Behaviors;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Application.Rules.Publishing;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Templates;
using Pricing.RuleCenter.Application.Trace;
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

namespace Pricing.RuleCenter.Api.Startup;

internal static class RuleCenterApiServiceCollectionExtensions
{
    public static IServiceCollection AddRuleCenterApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddRuleCenterApplicationServices();
        services.AddRuleCenterSecurity(configuration);
        services.AddRuleCenterHostedServices();
        services.AddRuleCenterRuleEngine();
        services.AddRuleCenterHealthChecks();
        services.AddRuleCenterControllersAndDocumentation(configuration);

        return services;
    }

    private static IServiceCollection AddRuleCenterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<DictAppService>();
        services.AddScoped<FormulaDefAppService>();
        services.AddScoped<RuleHeaderAppService>();
        services.AddScoped<RuleVersionAppService>();
        services.AddScoped<RuleConditionAppService>();
        services.AddScoped<RuleActionAppService>();
        services.AddScoped<RuleEditGuard>();
        services.AddScoped<RuleApprovalAppService>();
        services.AddScoped<RulePublishLifecycleRepositories>();
        services.AddScoped<RulePublishDefinitionRepositories>();
        services.AddScoped<RulePublishTransactionWriter>();
        services.AddScoped<RulePublishCacheInvalidator>();
        services.AddScoped<RuleCacheInvalidationOutboxProcessor>();
        services.AddScoped<FormulaFunctionRegistry>();
        services.AddScoped<FormulaExpressionEvaluator>();
        services.AddScoped<FormulaExpressionValidator>();
        services.AddScoped<RuleActionParameterValidator>();
        services.AddScoped<RuleCapabilityRegistry>();
        services.AddScoped<RuleCapabilityGuard>();
        services.AddScoped<RuleCriticalActionGuard>();
        services.AddScoped<RuleChildItemGuard>();
        services.AddScoped<RuleTestCaseGate>();
        services.AddScoped<RuleApprovalGate>();
        services.AddScoped<RuleConflictDetector>();
        services.AddScoped<RulePublishGuard>();
        services.AddScoped<PublishRuleUseCase>();
        services.AddScoped<DisableRuleUseCase>();
        services.AddScoped<RollbackRuleUseCase>();
        services.AddScoped<RulePublishAppService>();
        services.AddScoped<RuleCacheOutboxAppService>();
        services.AddScoped<TemplateAppService>();
        services.AddScoped<TemplateVersionAppService>();
        services.AddScoped<PolicyExpressionGuard>();
        services.AddScoped<PolicyAppService>();
        services.AddScoped<PolicyImportService>();
        services.AddScoped<PolicyVersionAppService>();
        services.AddScoped<PolicyPreviewAppService>();
        services.AddScoped<PolicyPriorityKeyFactory>();
        services.AddScoped<PolicyValidationService>();
        services.AddScoped<PolicyConflictService>();
        services.AddScoped<PolicyPublishProfileResolver>();
        services.AddScoped<PolicyPublishEligibilityService>();
        services.AddScoped<PolicyReviewAppService>();
        services.AddScoped<PolicyPackageDiffService>();
        services.AddScoped<RuntimeRuleProjectionFactory>();
        services.AddScoped<RuntimePackageCompiler>();
        services.AddScoped<RuntimePackageTraceContextAccessor>();
        services.AddScoped<RuntimePackageTraceResolver>();
        services.AddScoped<RuntimePackageQueryAppService>();
        services.AddScoped<RuntimePackageActivationService>();
        services.AddScoped<RuntimePackageRollbackService>();
        services.AddScoped<RuntimePackagePublishService>();
        services.AddScoped<LegacyRuleAuthoringGuardFilter>();
        services.AddScoped<AuthorityPriceChecker>();
        services.AddScoped<PricingIdempotencyService>();
        services.AddScoped<PricingRequestLogWriter>();
        services.AddScoped<PricingTraceStepWriter>();
        services.AddScoped<PricingDiscountDetailWriter>();
        services.AddScoped<PricingLimitOccupyWriter>();
        services.AddScoped<PricingReverseLogWriter>();
        services.AddScoped<PricingTransactionExecutor>();
        services.AddScoped<PricingIdempotentResponseReader>();
        services.AddScoped<PricingReverseHistoryReader>();
        services.AddScoped<PricingSpecialFlagResolver>();
        services.AddScoped<PricingSimulateWorkflow>();
        services.AddScoped<PricingConfirmWorkflow>();
        services.AddScoped<PricingCommitWorkflow>();
        services.AddScoped<PricingCancelWorkflow>();
        services.AddScoped<PricingReverseWorkflow>();
        services.AddScoped<TraceQueryAppService>();
        services.AddSingleton<CacheVersionLocalState>();
        services.AddScoped<ICacheVersionSynchronizer, CacheVersionSynchronizer>();
        services.AddMediatR(typeof(ApplicationAssemblyMarker));
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static IServiceCollection AddRuleCenterSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(ApiKeyAuthenticationOptions.SchemeName)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.SchemeName,
                options =>
                {
                    options.HeaderName = configuration["Authentication:ApiKey:HeaderName"]
                        ?? ApiKeyAuthenticationOptions.DefaultHeaderName;

                    foreach (var item in configuration.GetSection("Authentication:ApiKey:Keys").GetChildren())
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

        services.AddAuthorization(options =>
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

        return services;
    }

    private static IServiceCollection AddRuleCenterHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<ExpireCleanupAppService>();
        services.AddHostedService<CacheVersionSyncService>();

        return services;
    }

    private static IServiceCollection AddRuleCenterRuleEngine(this IServiceCollection services)
    {
        services.AddScoped<IRuleConditionEvaluator, ItemMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, ChargeSceneMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, BodyPartMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, TimeRangeEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, PregnancyMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, VisitTypeMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, AgeMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, GroupMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, ChargeDeptExcludeEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, InsuranceTypeMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, DiagnosisMatchEvaluator>();
        services.AddScoped<IRuleConditionEvaluator, DeviceTypeMatchEvaluator>();

        services.AddScoped<IRuleActionExecutor, AmountFloorExecutor>();
        services.AddScoped<IRuleActionExecutor, AmountCeilingExecutor>();
        services.AddScoped<IRuleActionExecutor, IncrementPercentExecutor>();
        services.AddScoped<IRuleActionExecutor, TimeWindowLimitExecutor>();
        services.AddScoped<IRuleActionExecutor, DailyQtyLimitExecutor>();
        services.AddScoped<IRuleActionExecutor, OnceQtyLimitExecutor>();
        services.AddScoped<IRuleActionExecutor, ExceedToZeroExecutor>();
        services.AddScoped<IRuleActionExecutor, UnitConvertExecutor>();
        services.AddScoped<IRuleActionExecutor, SameGroupMutexExecutor>();
        services.AddScoped<IRuleActionExecutor, SameOperationCeilingExecutor>();
        services.AddScoped<IRuleActionExecutor, AddChildItemExecutor>();
        services.AddScoped<IRuleActionExecutor, AreaStepIncrementExecutor>();
        services.AddScoped<IRuleActionExecutor, ConvertQtyByPartExecutor>();
        services.AddScoped<IRuleActionExecutor, ChildItemPercentExecutor>();
        services.AddScoped<IRuleActionExecutor, ExpressionFormulaExecutor>();

        services.AddScoped<ConditionEvaluatorFactory>();
        services.AddScoped<ActionExecutorFactory>();
        services.AddScoped(provider => new RuleMatchRepositories(
            provider.GetRequiredService<IRuleHeaderRepository>(),
            provider.GetRequiredService<IRuleConditionRepository>(),
            provider.GetRequiredService<IRuleActionRepository>(),
            provider.GetRequiredService<IDictRepository>(),
            provider.GetService<IRuntimePackageStateRepository>(),
            provider.GetService<IRuntimeRuleReadRepository>(),
            provider.GetRequiredService<RuntimePackageTraceContextAccessor>()));
        services.AddScoped<EffectiveRuleSnapshotLoader>();
        services.AddScoped<EffectiveRuleSnapshotCache>();
        services.AddScoped<RuleMatchService>();
        services.AddScoped<IRuleRuntimeCacheInvalidator>(provider =>
            provider.GetRequiredService<RuleMatchService>());
        services.AddScoped<ActionExecutionPipeline>();
        services.AddScoped<IPricingEngine, PricingEngine>();

        return services;
    }

    private static IServiceCollection AddRuleCenterHealthChecks(this IServiceCollection services)
    {
        services.AddHttpClient("health-checks");
        services.AddHealthChecks()
            .AddCheck<OracleHealthCheck>("database:oracle")
            .AddCheck<ThirdPartyServicesHealthCheck>("third-party-services");

        return services;
    }

    private static IServiceCollection AddRuleCenterControllersAndDocumentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers(options =>
            {
                options.ValueProviderFactories.Insert(0, new SnakeCaseQueryValueProviderFactory());
            })
            .AddJsonOptions(options => ApiJsonSerializerOptions.Configure(options.JsonSerializerOptions));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var title = configuration["Swagger:Title"] ?? "Pricing RuleCenter API";
            var version = configuration["Swagger:Version"] ?? "v1";
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = "医院物价折价规则中心 API，按 DDD 边界展示计价、规则、字典、追溯和运维接口。"
            });
            options.OperationFilter<SnakeCaseQueryParameterOperationFilter>();

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

        return services;
    }
}
