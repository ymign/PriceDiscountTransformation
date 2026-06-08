using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Pricing.RuleCenter.Infrastructure.Database;
using Pricing.RuleCenter.Infrastructure.Repositories;
using Pricing.RuleCenter.Infrastructure.Repositories.Rules;
using Pricing.RuleCenter.Infrastructure.Repositories.Charging;
using Pricing.RuleCenter.Infrastructure.Repositories.Quota;
using Pricing.RuleCenter.Infrastructure.Repositories.Catalog;
using Pricing.RuleCenter.Infrastructure.Repositories.Policies;
using Pricing.RuleCenter.Infrastructure.Repositories.Runtime;
using Pricing.RuleCenter.Infrastructure.Repositories.Templates;

namespace Pricing.RuleCenter.Infrastructure;

/// <summary>
/// 基础设施层依赖注入扩展，封装所有仓储和数据库配置的注册。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 该扩展方法将基础设施层的所有组件注册集中到一处，包括：
///   - SqlSugar 数据库客户端（面向 Oracle 11g）
///   - 全部仓储实现（20 个仓储，覆盖规则配置、计价追溯、并发控制等）
///   - 内存缓存（规则和字典查询结果缓存）
///   - 分布式缓存抽象（默认内存实现，可由部署环境替换为 Redis 等实现）
///   - PricingOptions 配置绑定
///   - IUnitOfWork 工作单元（封装跨仓储事务管理）
/// </para>
/// <para>
/// 【设计意图】
/// Api 层的 Program.cs 只需调用 <c>builder.Services.AddInfrastructure(builder.Configuration)</c>，
/// 无需直接 using Infrastructure 命名空间或逐个注册仓储。
/// </para>
/// <para>
/// 【注册生命周期说明】
///   - SqlSugarClient：Scoped
///   - 仓储：Scoped（与 SqlSugarClient 同生命周期）
///   - IUnitOfWork：Scoped（与 SqlSugarClient 同生命周期）
///   - 内存缓存：Singleton
///   - 分布式缓存：Singleton（默认内存分布式缓存）
/// </para>
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// 注册基础设施层全部服务。
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var pricingOptions = new PricingOptions();
        configuration.GetSection("Pricing").Bind(pricingOptions);
        services.AddOptions<PricingOptions>()
            .Configure(options => configuration.GetSection("Pricing").Bind(options));
        services.AddSingleton<IValidateOptions<PricingOptions>, PricingOptionsValidator>();
        services.AddHostedService<OptionsValidationStartupService>();

        services.AddSqlSugarOracle(pricingOptions);
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IUnitOfWork, SqlSugarUnitOfWork>();

        // --- 规则配置仓储 ---
        services.AddScoped<IDictRepository, DictRepository>();
        services.AddScoped<ICacheVersionRepository, CacheVersionRepository>();
        services.AddScoped<IRuleCacheInvalidationOutboxRepository, RuleCacheInvalidationOutboxRepository>();
        services.AddScoped<IRuleHeaderRepository, RuleHeaderRepository>();
        services.AddScoped<IRuleVersionRepository, RuleVersionRepository>();
        services.AddScoped<IRuleConditionRepository, RuleConditionRepository>();
        services.AddScoped<IRuleActionRepository, RuleActionRepository>();
        services.AddScoped<IFormulaDefRepository, FormulaDefRepository>();
        services.AddScoped<IRulePublishRepository, RulePublishRepository>();
        services.AddScoped<IRuleChangeLogRepository, RuleChangeLogRepository>();
        services.AddScoped<IRuleApprovalRepository, RuleApprovalRepository>();
        services.AddScoped<IRuntimePackageRepository, RuntimePackageRepository>();
        services.AddScoped<IRuntimePackageStateRepository, RuntimePackageStateRepository>();
        services.AddScoped<IRuntimeRuleBuildRepository, RuntimeRuleBuildRepository>();
        services.AddScoped<IRuntimeRuleReadRepository, RuntimeRuleReadRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IPolicyReviewRepository, PolicyReviewRepository>();

        // --- 项目分组仓储 ---
        services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
        services.AddScoped<IItemGroupDetailRepository, ItemGroupDetailRepository>();

        // --- 权威物价仓储 ---
        services.AddScoped<IPriceMasterRepository, PriceMasterRepository>();

        // --- 计价追溯仓储 ---
        services.AddScoped<IChargeRequestLogRepository, ChargeRequestLogRepository>();
        services.AddScoped<IChargeDiscountDetailRepository, ChargeDiscountDetailRepository>();
        services.AddScoped<IChargeTraceStepRepository, ChargeTraceStepRepository>();
        services.AddScoped<IChargeReverseLogRepository, ChargeReverseLogRepository>();

        // --- 并发控制仓储 ---
        services.AddScoped<ILimitOccupyRepository, LimitOccupyRepository>();
        services.AddScoped<ILimitLockRepository, LimitLockRepository>();

        // --- 测试仓储 ---
        services.AddScoped<IRuleTestCaseRepository, RuleTestCaseRepository>();
        services.AddScoped<IRuleTestRunRepository, RuleTestRunRepository>();

        return services;
    }
}
