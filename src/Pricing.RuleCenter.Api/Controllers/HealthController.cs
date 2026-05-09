using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using SqlSugar;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly ISqlSugarClient _db;

    public HealthController(ISqlSugarClient db)
    {
        _db = db;
    }

    [HttpGet("/health")]
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

public sealed class HealthResult
{
    public string Status => Database.StartsWith("connected") ? "healthy" : "unhealthy";
    public string Database { get; set; } = "unknown";
    public bool DictTableReady { get; set; }
    public DateTime ServerTime { get; init; } = DateTime.Now;
}
