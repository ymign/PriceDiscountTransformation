using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Options;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Database;

/// <summary>
/// SqlSugar 依赖注入注册扩展方法。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 把面向 Oracle 的 SqlSugar 客户端注册到 ASP.NET Core 依赖注入容器中。
/// 基础设施层统一在这里配置数据库连接、实体特性映射和 SQL 日志。
/// 应用层只依赖仓储接口（如 IRuleHeaderRepository），不直接知道 SqlSugar 的连接细节。
/// </para>
/// <para>
/// 【技术栈说明】
///   - 数据库：Oracle 11g（无原生 JSON 支持，用 CLOB；无自增，用 SEQUENCE）
///   - ORM：SqlSugarCore
///   - 驱动：Oracle.ManagedDataAccess.Core
///   - 金额类型：C# decimal ↔ Oracle NUMBER(18,4)，禁止 double/float
/// </para>
/// <para>
/// 【生命周期选择】
/// 采用 Scoped 生命周期（每次 HTTP 请求或后台任务创建一个实例）。
/// 这样同一请求内的多个仓储共享同一个客户端上下文，避免把连接对象做成全局单例造成并发状态污染。
/// </para>
/// </remarks>
public static class SqlSugarSetup
{
    /// <summary>
    /// 注册面向 Oracle 的 SqlSugar 客户端到依赖注入容器。
    /// </summary>
    /// <param name="services">ASP.NET Core 依赖注入服务集合。</param>
    /// <param name="options">计价规则中心配置，包含 Oracle 连接字符串。</param>
    /// <returns>注册后的服务集合，便于链式调用。</returns>
    /// <remarks>
    /// 【注册内容】
    ///   - ISqlSugarClient → SqlSugarClient（Scoped 生命周期）
    ///   - 连接字符串从 PricingOptions.OracleConnectionString 读取
    ///   - 实体映射使用 Attribute 模式（InitKeyType.Attribute）
    ///   - SQL 日志输出为 Debug 级别
    ///   - 数据库异常统一记录 Error 级别日志
    /// 【使用方式】
    /// 在 Program.cs 或 Startup.cs 中调用：
    /// <code>
    /// builder.Services.AddSqlSugarOracle(pricingOptions);
    /// </code>
    /// 之后仓储类通过构造函数注入 ISqlSugarClient 即可。
    /// </remarks>
    public static IServiceCollection AddSqlSugarOracle(
        this IServiceCollection services,
        PricingOptions options)
    {
        // ========== 第一阶段：按作用域创建数据库客户端 ==========
        // Scoped 生命周期可以让一次请求或后台任务内的仓储共享同一个客户端上下文，
        // 同时避免把连接对象做成全局单例造成并发状态污染。
        services.AddScoped<ISqlSugarClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<SqlSugarClient>>();

            // ========== 第二阶段：配置 Oracle 连接和实体映射 ==========
            // Core 模型不再直接标注 SqlSugar 特性，表名、列名和主键统一由 EntityTypeConfigs 配置。
            // InitKeyType.Attribute 表示 SqlSugar 会尝试读取特性注解，但 EntityTypeConfigs 的 Fluent API 会覆盖这些配置。
            var db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = options.OracleConnectionString,
                DbType = DbType.Oracle,
                // IsAutoCloseConnection = true 表示每次操作后自动关闭连接，
                // 适合 Scoped 生命周期，避免连接泄漏。
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = EntityTypeConfigs.CreateExternalServices()
            });

            // ========== 应用 Fluent API 实体映射 ==========
            // Core 模型已移除 SqlSugar 特性注解，所有表名和列映射通过 EntityTypeConfigs 统一配置。
            // 这保证 Core 领域模型不依赖任何 ORM 框架，是 Clean Architecture 的关键保障。
            EntityTypeConfigs.ApplyAllConfigs(db);

            // ========== 第三阶段：接入 SQL 日志 ==========
            // SQL 输出为 Debug 级别，正常生产日志不会被大量 SQL 冲满；
            // 排查问题时可将日志级别提高到 Debug 查看实际执行的 SQL。
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                logger.LogDebug("SqlSugar 执行 SQL：{Sql}", sql);
            };

            // ========== 第四阶段：统一记录数据库异常 ==========
            // 仓储方法仍把异常向上抛出，交给事务边界或全局异常过滤器处理；
            // 这里仅补充 SQL 层上下文，确保异常日志中包含数据库操作信息。
            db.Aop.OnError = ex =>
            {
                logger.LogError(ex, "SqlSugar 执行异常");
            };

            return db;
        });

        return services;
    }
}
