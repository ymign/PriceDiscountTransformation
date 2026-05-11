using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Infrastructure.Database;
using Pricing.RuleCenter.Infrastructure.Repositories;

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
///   - PricingOptions 配置绑定
/// </para>
/// <para>
/// 【设计意图】
/// Api 层的 Program.cs 只需调用 <c>builder.Services.AddInfrastructure(builder.Configuration)</c>，
/// 无需直接 using Infrastructure 命名空间或逐个注册仓储。
/// 这样做有两个好处：
///   1. Program.cs 保持简洁，只关注 Api 层自身的服务和中间件配置
///   2. 基础设施层增删仓储时只需修改此文件，不影响 Api 层
/// </para>
/// <para>
/// 【注册生命周期说明】
///   - SqlSugarClient：Scoped（每次 HTTP 请求或后台任务一个实例，避免全局单例并发状态污染）
///   - 仓储：Scoped（与 SqlSugarClient 同生命周期，共享同一数据库上下文）
///   - 内存缓存：Singleton（跨请求共享，依赖主动失效机制保证一致性）
///   - 后台服务：Singleton（由 ASP.NET Core Host 托管，通过 IServiceScopeFactory 创建 Scoped 依赖）
/// </para>
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// 注册基础设施层全部服务，包括 SqlSugar、仓储和后台任务。
    /// </summary>
    /// <param name="services">ASP.NET Core 依赖注入服务集合。</param>
    /// <param name="configuration">应用配置，用于读取 "Pricing" 配置段绑定 <see cref="PricingOptions"/>。</param>
    /// <returns>注册后的服务集合，便于链式调用。</returns>
    /// <remarks>
    /// <para>
    /// 【调用方式】
    /// 在 Api 层 Program.cs 中：
    /// <code>
    /// builder.Services.AddInfrastructure(builder.Configuration);
    /// </code>
    /// </para>
    /// <para>
    /// 【注册内容清单】
    ///   1. PricingOptions 配置绑定
    ///   2. SqlSugar Oracle 客户端（Scoped）
    ///   3. 内存缓存（Singleton，规则 TTL 5 分钟，字典 TTL 30 分钟）
    ///   4. 20 个仓储接口到实现的映射（Scoped）
    /// </para>
    /// <para>
    /// 【不包含的内容】
    /// 以下组件属于 Api 层或 Domain 层，由 Program.cs 直接注册：
    ///   - 应用服务（PricingApiService、DictService 等）
    ///   - 后台服务（ExpireCleanupService，位于 Api 层）
    ///   - 规则引擎（IPricingEngine、PricingEngine）
    ///   - 条件执行器（IRuleConditionEvaluator 实现）
    ///   - 动作执行器（IRuleActionExecutor 实现）
    ///   - 控制器和过滤器
    /// </para>
    /// </remarks>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ========== 第一阶段：绑定配置 ==========
        // PricingOptions 通过 ASP.NET Core Options 模式绑定自 appsettings.json 的 "Pricing" 段。
        // 同时创建一个实例用于传入 SqlSugarSetup（因为 SqlSugarSetup 需要连接字符串立即初始化配置）。
        var pricingOptions = new PricingOptions();
        configuration.GetSection("Pricing").Bind(pricingOptions);
        services.Configure<PricingOptions>(options => configuration.GetSection("Pricing").Bind(options));

        // ========== 第二阶段：注册 SqlSugar Oracle 客户端 ==========
        // SqlSugarSetup.AddSqlSugarOracle 将 ISqlSugarClient 注册为 Scoped，
        // 同一请求内的多个仓储共享同一个客户端上下文。
        services.AddSqlSugarOracle(pricingOptions);

        // ========== 第三阶段：注册内存缓存 ==========
        // 规则和字典查询结果使用内存缓存提升性能。
        // 规则缓存 TTL 5 分钟（发布/停用/回滚时主动清除），字典缓存 TTL 30 分钟（增删改时主动清除）。
        // 缓存失效策略由应用服务层控制，基础设施层只负责注册缓存提供程序。
        services.AddMemoryCache();

        // ========== 第四阶段：注册全部仓储 ==========
        // 仓储接口定义在 Core 层（I*Repository），实现在 Infrastructure 层（Repositories/）。
        // 手动注册而非程序集扫描，原因是：
        //   1. 明确列出所有仓储，避免遗漏或误注册
        //   2. 便于审查每个仓储的生命周期是否正确
        //   3. 新增仓储时在此处添加一行即可，编译器会提示缺少实现
        //
        // --- 规则配置仓储 ---
        services.AddScoped<IDictRepository, DictRepository>();
        services.AddScoped<IRuleHeaderRepository, RuleHeaderRepository>();
        services.AddScoped<IRuleVersionRepository, RuleVersionRepository>();
        services.AddScoped<IRuleConditionRepository, RuleConditionRepository>();
        services.AddScoped<IRuleActionRepository, RuleActionRepository>();
        services.AddScoped<IFormulaDefRepository, FormulaDefRepository>();
        services.AddScoped<IRulePublishRepository, RulePublishRepository>();
        services.AddScoped<IRuleChangeLogRepository, RuleChangeLogRepository>();
        services.AddScoped<IRuleApprovalRepository, RuleApprovalRepository>();

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

        // 注意：ExpireCleanupService 位于 Api 层（Pricing.RuleCenter.Api.Services），
        // 基础设施层不引用 Api 层，因此该后台服务在 Program.cs 中注册。

        return services;
    }
}
