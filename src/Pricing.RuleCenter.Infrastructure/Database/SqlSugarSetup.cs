using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Options;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Database;

public static class SqlSugarSetup
{
    public static IServiceCollection AddSqlSugarOracle(
        this IServiceCollection services,
        PricingOptions options)
    {
        services.AddScoped<ISqlSugarClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<SqlSugarClient>>();

            var db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = options.OracleConnectionString,
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });

            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                logger.LogDebug("SqlSugar SQL: {Sql}", sql);
            };

            db.Aop.OnError = ex =>
            {
                logger.LogError(ex, "SqlSugar 执行异常");
            };

            return db;
        });

        return services;
    }
}
