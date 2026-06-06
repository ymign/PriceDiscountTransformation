using Pricing.RuleCenter.Api.Middleware;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Exceptions;
using Xunit;
using ValidationException = Pricing.RuleCenter.Core.Exceptions.ValidationException;

namespace Pricing.RuleCenter.Tests;

public sealed class ApiExceptionMapperTests
{
    [Fact]
    public void Map_ShouldReturnFieldErrorsForValidationException()
    {
        var errors = new Dictionary<string, string[]>
        {
            ["items"] = new[] { "费用明细不能为空" }
        };

        var mapped = ApiExceptionMapper.Map(new ValidationException("参数校验失败", errors));

        Assert.Equal(400, mapped.StatusCode);
        Assert.Equal(400, mapped.Code);
        Assert.Equal("参数校验失败", mapped.Message);
        Assert.Same(errors, mapped.Errors);
    }

    [Fact]
    public void Map_ShouldHidePriceDetailsForPriceMismatch()
    {
        var exception = new BizException(
            BizErrorCode.PriceMismatch,
            409,
            "项目 ITEM001 权威单价=10.0000, 请求单价=1.0000");

        var mapped = ApiExceptionMapper.Map(exception);

        Assert.Equal(409, mapped.StatusCode);
        Assert.Equal(BizErrorCode.PriceMismatch, mapped.Code);
        Assert.Equal("单价与权威物价不一致", mapped.Message);
        Assert.DoesNotContain("10.0000", mapped.Message);
        Assert.DoesNotContain("1.0000", mapped.Message);
    }

    [Theory]
    [MemberData(nameof(ExceptionCases))]
    public void Map_ShouldUseSameStatusCodeAndBusinessCodeForSupportedExceptions(
        Exception exception,
        int expectedStatusCode,
        int expectedCode,
        string expectedMessage)
    {
        var mapped = ApiExceptionMapper.Map(exception);

        Assert.Equal(expectedStatusCode, mapped.StatusCode);
        Assert.Equal(expectedCode, mapped.Code);
        Assert.Equal(expectedMessage, mapped.Message);
        Assert.Null(mapped.Errors);
    }

    public static IEnumerable<object[]> ExceptionCases()
    {
        yield return new object[]
        {
            new BizException(BizErrorCode.RuleNotFound, 404, "规则不存在"),
            404,
            BizErrorCode.RuleNotFound,
            "规则不存在"
        };
        yield return new object[]
        {
            new NotFoundException("资源不存在", BizErrorCode.FormulaNotFound),
            404,
            BizErrorCode.FormulaNotFound,
            "资源不存在"
        };
        yield return new object[]
        {
            new DomainException("状态冲突", 3010),
            409,
            3010,
            "状态冲突"
        };
        yield return new object[]
        {
            new LimitLockException("LK-001", true, "锁竞争失败"),
            409,
            BizErrorCode.ConcurrencyConflict,
            "锁竞争失败"
        };
        yield return new object[]
        {
            new LimitLockException("LK-002", false, "锁表数据库故障"),
            500,
            BizErrorCode.LimitLockFailed,
            "锁表数据库故障"
        };
        yield return new object[]
        {
            new ArgumentException("参数错误"),
            400,
            400,
            "参数错误"
        };
        yield return new object[]
        {
            new KeyNotFoundException("资源缺失"),
            404,
            404,
            "资源缺失"
        };
        yield return new object[]
        {
            new InvalidOperationException("状态不允许"),
            409,
            409,
            "状态不允许"
        };
        yield return new object[]
        {
            new Exception("敏感内部错误"),
            500,
            500,
            "服务器内部错误"
        };
    }
}
