using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Options;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Database;

/// <summary>
/// SqlSugar 注册扩展，负责把 Oracle 数据库客户端加入依赖注入容器。
/// </summary>
/// <remarks>
/// 基础设施层统一在这里配置数据库连接、实体特性映射和 SQL 日志。应用层只依赖仓储接口，
/// 不直接知道 SqlSugar 的连接细节。
/// </remarks>
public static class SqlSugarSetup
{
    /// <summary>
    /// 注册面向 Oracle 的 SqlSugar 客户端。
    /// </summary>
    /// <param name="services">依赖注入服务集合。</param>
    /// <param name="options">计价规则中心配置，包含 Oracle 连接字符串。</param>
    /// <returns>注册后的服务集合，便于链式调用。</returns>
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
            // InitKeyType.Attribute 表示模型上的 SugarTable/SugarColumn 特性是表结构映射来源。
            var db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = options.OracleConnectionString,
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });

            // ========== 第三阶段：接入 SQL 日志 ==========
            // SQL 输出为 Debug 级别，正常生产日志不会被大量 SQL 冲满；排查问题时可提高日志级别查看。
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                logger.LogDebug("SqlSugar SQL: {Sql}", sql);
            };

            // ========== 第四阶段：统一记录数据库异常 ==========
            // 仓储方法仍把异常向上抛出，交给事务边界或全局异常过滤器处理；这里仅补充 SQL 层上下文。
            db.Aop.OnError = ex =>
            {
                logger.LogError(ex, "SqlSugar 执行异常");
            };

            return db;
        });

        return services;
    }
}
