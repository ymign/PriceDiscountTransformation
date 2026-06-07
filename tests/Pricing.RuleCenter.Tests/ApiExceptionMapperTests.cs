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
    [InlineData(2014, "RequestId=100 ResponseJson={not-json", "幂等响应快照异常，请联系管理员处理")]
    [InlineData(BizErrorCode.CommitAmountMismatch, "ITEM001 expected=20 actual=21", "HIS实际落账明细与确认计价结果不一致")]
    [InlineData(BizErrorCode.CommitQtyMismatch, "ITEM001 expectedQty=2 actualQty=3", "HIS实际落账明细与确认计价结果不一致")]
    [InlineData(BizErrorCode.CommitDetailMismatch, "ChargeDetailNo=CD001 缺失", "HIS实际落账明细与确认计价结果不一致")]
    public void Map_ShouldHideSensitiveBusinessDetails(
        int code,
        string internalMessage,
        string expectedClientMessage)
    {
        var mapped = ApiExceptionMapper.Map(new BizException(code, 409, internalMessage));

        Assert.Equal(409, mapped.StatusCode);
        Assert.Equal(code, mapped.Code);
        Assert.Equal(expectedClientMessage, mapped.Message);
        Assert.DoesNotContain("ITEM001", mapped.Message);
        Assert.DoesNotContain("RequestId", mapped.Message);
        Assert.DoesNotContain("ChargeDetailNo", mapped.Message);
    }

    [Theory]
    [InlineData(true, 409, BizErrorCode.ConcurrencyConflict, "限额锁竞争失败，请稍后重试")]
    [InlineData(false, 500, BizErrorCode.LimitLockFailed, "限额锁处理失败")]
    public void Map_ShouldHideLimitLockDetails(
        bool isConcurrencyConflict,
        int expectedStatusCode,
        int expectedCode,
        string expectedMessage)
    {
        var mapped = ApiExceptionMapper.Map(new LimitLockException(
            "LIMIT:P001:ITEM001:20260510",
            isConcurrencyConflict,
            "锁表数据库故障: LIMIT:P001:ITEM001:20260510"));

        Assert.Equal(expectedStatusCode, mapped.StatusCode);
        Assert.Equal(expectedCode, mapped.Code);
        Assert.Equal(expectedMessage, mapped.Message);
        Assert.DoesNotContain("P001", mapped.Message);
        Assert.DoesNotContain("ITEM001", mapped.Message);
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
            "限额锁竞争失败，请稍后重试"
        };
        yield return new object[]
        {
            new LimitLockException("LK-002", false, "锁表数据库故障"),
            500,
            BizErrorCode.LimitLockFailed,
            "限额锁处理失败"
        };
        yield return new object[]
        {
            new ArgumentException("参数错误"),
            400,
            400,
            "请求参数不合法"
        };
        yield return new object[]
        {
            new KeyNotFoundException("资源缺失"),
            404,
            404,
            "资源不存在"
        };
        yield return new object[]
        {
            new InvalidOperationException("状态不允许"),
            409,
            409,
            "当前状态不允许执行该操作"
        };
        yield return new object[]
        {
            new Exception("敏感内部错误"),
            500,
            500,
            "服务器内部错误"
        };
    }

    [Theory]
    [InlineData("RequestId=100, ActionId=200")]
    [InlineData("AuthorityPrice=10.0000, RequestPrice=1.0000")]
    public void Map_ShouldHideInternalDetailsForInvalidOperationException(string internalMessage)
    {
        var mapped = ApiExceptionMapper.Map(new InvalidOperationException(internalMessage));

        Assert.Equal(409, mapped.StatusCode);
        Assert.Equal(409, mapped.Code);
        Assert.Equal("当前状态不允许执行该操作", mapped.Message);
        Assert.DoesNotContain("RequestId", mapped.Message);
        Assert.DoesNotContain("ActionId", mapped.Message);
        Assert.DoesNotContain("AuthorityPrice", mapped.Message);
    }
}
