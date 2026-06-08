using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Api.Security;

/// <summary>
/// 旧规则作者写接口退役保护。
/// </summary>
public sealed class LegacyRuleAuthoringGuardFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public LegacyRuleAuthoringGuardFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (IsLegacyAuthoringEnabled() ||
            HttpMethods.IsGet(context.HttpContext.Request.Method) ||
            HttpMethods.IsHead(context.HttpContext.Request.Method) ||
            HttpMethods.IsOptions(context.HttpContext.Request.Method))
        {
            return next();
        }

        context.Result = new ObjectResult(ApiResult.Fail(410, "旧规则写维护入口已退役，请改用模板/策略/运行时包接口。"))
        {
            StatusCode = StatusCodes.Status410Gone
        };
        return Task.CompletedTask;
    }

    private bool IsLegacyAuthoringEnabled()
    {
        return bool.TryParse(
                   _configuration["Authoring:LegacyRuleAuthoringEnabled"],
                   out var enabled) &&
               enabled;
    }
}
