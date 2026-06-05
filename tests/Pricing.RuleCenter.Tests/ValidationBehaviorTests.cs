using Pricing.RuleCenter.Application.Common.Behaviors;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Queries;
using Pricing.RuleCenter.Application.Pricing.Validation;
using ValidationException = Pricing.RuleCenter.Core.Exceptions.ValidationException;
using Xunit;

namespace Pricing.RuleCenter.Tests;

/// <summary>
/// Application 层 MediatR 验证管道测试。
/// </summary>
public sealed class ValidationBehaviorTests
{
    /// <summary>
    /// Query 参数非法时，ValidationBehavior 应在 Handler 执行前抛出字段级 ValidationException。
    /// </summary>
    [Fact]
    public async Task Handle_ThrowsValidationExceptionBeforeHandlerWhenQueryIsInvalid()
    {
        var behavior = new ValidationBehavior<GetSpecialFlagQuery, SpecialFlagResponse>(
            new[] { new GetSpecialFlagQueryValidator() });
        var handlerCalled = false;

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(
                new GetSpecialFlagQuery(" "),
                () =>
                {
                    handlerCalled = true;
                    return Task.FromResult(new SpecialFlagResponse());
                },
                CancellationToken.None));

        Assert.False(handlerCalled);
        Assert.Equal(400, ex.Code);
        Assert.Equal("请求参数校验失败", ex.Message);
        Assert.True(ex.Errors.ContainsKey("ItemCode"));
        Assert.Contains("项目编码不能为空", ex.Errors["ItemCode"]);
    }
}
