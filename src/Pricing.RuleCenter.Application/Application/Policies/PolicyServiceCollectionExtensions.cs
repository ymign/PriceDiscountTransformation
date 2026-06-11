using Microsoft.Extensions.DependencyInjection;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略平台应用服务注册扩展。
/// </summary>
public static class PolicyServiceCollectionExtensions
{
    /// <summary>
    /// 注册策略平台所需的应用服务和内部协作者。
    /// </summary>
    /// <param name="services">依赖注入服务集合。</param>
    /// <returns>注册后的服务集合。</returns>
    public static IServiceCollection AddPolicyApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPolicyExpressionGuard, PolicyExpressionGuard>();
        services.AddScoped<PolicyAppService>();
        services.AddScoped<PolicyImportService>();
        services.AddScoped<PolicyVersionAppService>();
        services.AddScoped<PolicyPreviewAppService>();
        services.AddScoped<IPolicyPriorityKeyFactory, PolicyPriorityKeyFactory>();
        services.AddScoped<IPolicyValidationService, PolicyValidationService>();
        services.AddScoped<IPolicyConflictService, PolicyConflictService>();
        services.AddScoped<PolicyPublishProfileResolver>();
        services.AddScoped<IPolicyPublishEligibilityService, PolicyPublishEligibilityService>();
        services.AddScoped<PolicyReviewAppService>();
        services.AddScoped<PolicyPackageDiffService>();

        return services;
    }
}
