using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using SqlSugar;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【健康检查控制器】
/// <para>
/// 职责范围：验证服务进程是否存活、数据库连接是否正常、基础数据表是否可访问。
/// </para>
/// <para>
/// 路由：<c>/health</c>（绝对路径，不跟随控制器路由前缀）
/// </para>
/// <para>
/// 用途：
/// <list type="bullet">
///   <item>负载均衡器健康探测（判断实例是否可接收流量）</item>
///   <item>运维监控面板状态展示</item>
///   <item>部署后快速验证数据库连通性</item>
///   <item>灰度发布时确认新版本服务可用</item>
/// </list>
/// </para>
/// <para>
/// 检查项：
/// <list type="number">
///   <item>数据库连通性 — 执行最小化 SQL 查询验证 Oracle 连接池可用</item>
///   <item>PR_DICT 表可访问性 — 验证核心基础表存在且可查询</item>
///   <item>服务端时间 — 用于排查跨时区或时钟偏移问题</item>
/// </list>
/// </para>
/// </summary>
/// <remarks>
/// 该控制器直接注入 <see cref="ISqlSugarClient"/> 而非应用服务，
/// 因为健康检查需要绕过业务层直接探测基础设施可用性。
/// </remarks>
[ApiController]
[Route("[controller]")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// SqlSugar 数据库客户端实例，用于执行最小化数据库连通性检查。
    /// <para>
    /// 直接使用 ORM 客户端而非应用服务，因为健康检查不需要业务逻辑封装。
    /// </para>
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数，通过依赖注入获取 SqlSugar 数据库客户端。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端（<see cref="ISqlSugarClient"/>）。</param>
    public HealthController(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 【健康检查接口】— 验证服务和数据库的可用性状态。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/health</c>
    /// </para>
    /// <para>
    /// 检查逻辑：
    /// <list type="number">
    ///   <item>执行 <c>SELECT COUNT(*) FROM PR_DICT WHERE ROWNUM = 1</c> 验证数据库连接和 PR_DICT 表可访问</item>
    ///   <item>查询成功则标记 Database = "connected"，DictTableReady = true</item>
    ///   <item>查询失败则标记 Database = "error: {message}"，异常信息仅包含消息不包含堆栈（安全考虑）</item>
    ///   <item>根据 Database 状态自动计算 Status：connected → "healthy"，否则 → "unhealthy"</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <returns>
    /// 健康检查结果（<see cref="HealthResult"/>），包含：
    /// <list type="bullet">
    ///   <item><c>Status</c> — 综合健康状态（"healthy" 或 "unhealthy"）</item>
    ///   <item><c>Database</c> — 数据库连接状态描述</item>
    ///   <item><c>DictTableReady</c> — PR_DICT 表是否可访问</item>
    ///   <item><c>ServerTime</c> — 服务端当前时间（用于时钟偏移排查）</item>
    /// </list>
    /// </returns>
    [HttpGet("/health")]
    public async Task<ApiResponse<HealthResult>> CheckAsync()
    {
        var result = new HealthResult();

        try
        {
            // 执行最小化查询：仅取一行验证连接可用，避免全表扫描
            var count = await _db.Ado.GetIntAsync(
                "SELECT COUNT(*) FROM PR_DICT WHERE ROWNUM = 1");
            result.Database = "connected";
            // count >= 0 表示查询成功（即使表为空也返回 0）
            result.DictTableReady = count >= 0;
        }
        catch (Exception ex)
        {
            // 异常仅记录消息，不暴露堆栈信息（安全考虑）
            result.Database = $"error: {ex.Message}";
        }

        return ApiResponse<HealthResult>.Ok(result);
    }
}

/// <summary>
/// 【健康检查结果 DTO】— 封装服务健康状态的检查结果。
/// <para>
/// 用于 <c>/health</c> 接口的响应体，供负载均衡器和运维监控系统解析。
/// </para>
/// </summary>
public sealed class HealthResult
{
    /// <summary>
    /// 综合健康状态。根据数据库连接状态自动计算：
    /// <list type="bullet">
    ///   <item>Database 以 "connected" 开头 → "healthy"</item>
    ///   <item>其他情况 → "unhealthy"</item>
    /// </list>
    /// </summary>
    public string Status => Database.StartsWith("connected") ? "healthy" : "unhealthy";

    /// <summary>
    /// 数据库连接状态说明。成功时为 "connected"，失败时为 "error: {异常消息}"。
    /// </summary>
    public string Database { get; set; } = "unknown";

    /// <summary>
    /// PR_DICT 表是否可访问。true 表示表存在且可查询，false 表示查询失败。
    /// </summary>
    public bool DictTableReady { get; set; }

    /// <summary>
    /// 生成健康检查结果时的服务端时间（只读）。
    /// 用于排查跨时区问题或服务端时钟偏移。
    /// </summary>
    public DateTime ServerTime { get; init; } = DateTime.Now;
}
