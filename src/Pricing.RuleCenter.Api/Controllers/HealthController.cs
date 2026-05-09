using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using SqlSugar;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("[controller]")]
/// <summary>
/// 健康检查控制器，用于验证服务进程和数据库基础表是否可用。
/// </summary>
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// SqlSugar 数据库客户端，用于执行最小化数据库连通性检查。
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化健康检查控制器。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端。</param>
    public HealthController(ISqlSugarClient db)
    {
        _db = db;
    }

    [HttpGet("/health")]
    /// <summary>
    /// 执行服务健康检查。
    /// </summary>
    /// <returns>数据库连通性、字典表可访问状态和服务端时间。</returns>
    public async Task<ApiResponse<HealthResult>> CheckAsync()
    {
        var result = new HealthResult();

        try
        {
            var count = await _db.Ado.GetIntAsync(
                "SELECT COUNT(*) FROM PR_DICT WHERE ROWNUM = 1");
            result.Database = "connected";
            result.DictTableReady = count >= 0;
        }
        catch (Exception ex)
        {
            result.Database = $"error: {ex.Message}";
        }

        return ApiResponse<HealthResult>.Ok(result);
    }
}

/// <summary>
/// 健康检查结果。
/// </summary>
public sealed class HealthResult
{
    /// <summary>
    /// 综合健康状态。数据库连通时视为 healthy，否则为 unhealthy。
    /// </summary>
    public string Status => Database.StartsWith("connected") ? "healthy" : "unhealthy";
    /// <summary>
    /// 数据库连接状态说明。
    /// </summary>
    public string Database { get; set; } = "unknown";
    /// <summary>
    /// PR_DICT 表是否可访问。
    /// </summary>
    public bool DictTableReady { get; set; }
    /// <summary>
    /// 生成健康检查结果时的服务端时间。
    /// </summary>
    public DateTime ServerTime { get; init; } = DateTime.Now;
}
