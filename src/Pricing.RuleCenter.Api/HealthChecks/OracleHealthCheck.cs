using Microsoft.Extensions.Diagnostics.HealthChecks;
using SqlSugar;

namespace Pricing.RuleCenter.Api.HealthChecks;

/// <summary>
/// Oracle 数据库健康检查。
/// </summary>
/// <remarks>
/// 检查逻辑使用最小化查询验证当前实例是否能访问数据库连接池以及规则中心关键表。
/// 该检查被注册到 <c>/health</c>，用于负载均衡和运维探针。
/// </remarks>
public sealed class OracleHealthCheck : IHealthCheck
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<OracleHealthCheck> _logger;

    /// <summary>
    /// 初始化 Oracle 健康检查。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    /// <param name="logger">健康检查日志。</param>
    public OracleHealthCheck(ISqlSugarClient db, ILogger<OracleHealthCheck> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dictCount = await _db.Ado.GetIntAsync("SELECT COUNT(*) FROM PR_DICT WHERE ROWNUM = 1");
            var ruleHeaderCount = await _db.Ado.GetIntAsync("SELECT COUNT(*) FROM PR_RULE_HEADER WHERE ROWNUM = 1");

            return HealthCheckResult.Healthy("Oracle connection and required PR_ tables are reachable.", new Dictionary<string, object>
            {
                ["dictTableReady"] = dictCount >= 0,
                ["ruleHeaderTableReady"] = ruleHeaderCount >= 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oracle 健康检查失败");
            return HealthCheckResult.Unhealthy("Oracle connection or required PR_ tables are unavailable.", ex);
        }
    }
}
