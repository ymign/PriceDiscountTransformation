using Pricing.RuleCenter.Application.Common.Behaviors;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Commands;
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
    /// 试算入口验证口径必须与应用服务兜底校验一致，避免 0 数量、空业务时间或无效片段进入计价链路。
    /// </summary>
    [Fact]
    public void SimulatePricingCommandValidator_RejectsInvalidCalculateShape()
    {
        var validator = new SimulatePricingCommandValidator();
        var request = CreateCalculateRequest(
            businessChargeTime: DateTime.MinValue,
            inputQty: 0m,
            unitPrice: -1m,
            partQty: 0m);

        var result = validator.Validate(new SimulatePricingCommand(request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.BusinessChargeTime"
            && failure.ErrorMessage == "业务收费发生时间不能为空");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.Items[0].InputQty"
            && failure.ErrorMessage == "数量必须大于0");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.Items[0].UnitPrice"
            && failure.ErrorMessage == "单价不能小于0");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.Items[0].PricingParts[0].Qty"
            && failure.ErrorMessage == "片段数量必须大于0");
    }

    /// <summary>
    /// commit 入口需要提前拒绝负金额和负数量，避免错误落账明细进入资金状态推进。
    /// </summary>
    [Fact]
    public void CommitPricingCommandValidator_RejectsNegativeActualAmounts()
    {
        var validator = new CommitPricingCommandValidator();
        var request = new PricingCommitRequest
        {
            RequestId = 1,
            ActualTotalAmount = -1m,
            ActualItems = new[]
            {
                new PricingCommitActualItemRequest
                {
                    ItemCode = " ",
                    FinalQty = -1m,
                    FinalAmount = -1m
                }
            }
        };

        var result = validator.Validate(new CommitPricingCommand(request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.ActualTotalAmount"
            && failure.ErrorMessage == "实际落账总金额不能小于0");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.ActualItems[0].ItemCode"
            && failure.ErrorMessage == "实际落账明细项目编码不能为空");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.ActualItems[0].FinalQty"
            && failure.ErrorMessage == "实际落账数量不能小于0");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.ActualItems[0].FinalAmount"
            && failure.ErrorMessage == "实际落账金额不能小于0");
    }

    /// <summary>
    /// reverse 入口需要提前拒绝非正退费数量和负退费金额，保持与应用服务冲正兜底校验一致。
    /// </summary>
    [Fact]
    public void ReversePricingCommandValidator_RejectsInvalidReverseQtyAndAmount()
    {
        var validator = new ReversePricingCommandValidator();
        var request = new PricingReverseRequest
        {
            OriginalRequestId = 1,
            ReverseNo = "REV-1",
            ReverseQty = 0m,
            ReverseAmt = -1m
        };

        var result = validator.Validate(new ReversePricingCommand(request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.ReverseQty"
            && failure.ErrorMessage == "退费数量必须大于0");
        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Request.ReverseAmt"
            && failure.ErrorMessage == "退费金额不能小于0");
    }

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

    private static PricingCalculateRequest CreateCalculateRequest(
        DateTime? businessChargeTime = null,
        decimal inputQty = 1m,
        decimal unitPrice = 10m,
        decimal partQty = 1m)
    {
        return new PricingCalculateRequest
        {
            SourceSystem = "HIS",
            PatientId = "P001",
            BusinessChargeTime = businessChargeTime ?? new DateTime(2026, 5, 10, 9, 0, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ItemCode = "ITEM001",
                    InputQty = inputQty,
                    UnitPrice = unitPrice,
                    PricingParts = new[]
                    {
                        new PricingPartItemRequest
                        {
                            PartSeq = 1,
                            Qty = partQty
                        }
                    }
                }
            }
        };
    }
}
