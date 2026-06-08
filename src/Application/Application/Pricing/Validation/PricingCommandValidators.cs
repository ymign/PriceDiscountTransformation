using FluentValidation;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Commands;
using Pricing.RuleCenter.Application.Pricing.Queries;

namespace Pricing.RuleCenter.Application.Pricing.Validation;

/// <summary>计价请求共享验证规则。</summary>
internal static class PricingValidationRules
{
    public static void ApplyCalculateRules<T>(IRuleBuilderInitial<T, PricingCalculateRequest> rule)
    {
        rule.NotNull().WithMessage("计价请求不能为空")
            .DependentRules(() =>
            {
                rule.ChildRules(request =>
                {
                    request.RuleFor(x => x.SourceSystem).NotEmpty().WithMessage("来源系统不能为空");
                    request.RuleFor(x => x.PatientId).NotEmpty().WithMessage("患者ID不能为空");
                    request.RuleFor(x => x.BusinessChargeTime)
                        .NotEqual(default(DateTime))
                        .WithMessage("业务收费发生时间不能为空");
                    request.RuleFor(x => x.Items).NotEmpty().WithMessage("费用明细不能为空");
                    request.RuleForEach(x => x.Items)
                        .NotNull()
                        .WithMessage("费用明细不能为空")
                        .ChildRules(item =>
                        {
                            item.RuleFor(x => x.ItemCode).NotEmpty().WithMessage("项目编码不能为空");
                            item.RuleFor(x => x.InputQty).GreaterThan(0).WithMessage("数量必须大于0");
                            item.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("单价不能小于0");
                            item.RuleFor(x => x.BusinessChargeTime)
                                .Must(value => !value.HasValue || value.Value != default)
                                .WithMessage("业务收费发生时间不能为空");
                            item.RuleForEach(x => x.PricingParts)
                                .NotNull()
                                .WithMessage("片段不能为空")
                                .ChildRules(part =>
                                {
                                    part.RuleFor(x => x.Qty).GreaterThan(0).WithMessage("片段数量必须大于0");
                                });
                        });
                });
            });
    }
}

/// <summary>试算命令验证器。</summary>
public sealed class SimulatePricingCommandValidator : AbstractValidator<SimulatePricingCommand>
{
    /// <summary>初始化验证规则。</summary>
    public SimulatePricingCommandValidator()
    {
        PricingValidationRules.ApplyCalculateRules(RuleFor(x => x.Request));
    }
}

/// <summary>确认计价命令验证器。</summary>
public sealed class ConfirmPricingCommandValidator : AbstractValidator<ConfirmPricingCommand>
{
    /// <summary>初始化验证规则。</summary>
    public ConfirmPricingCommandValidator()
    {
        PricingValidationRules.ApplyCalculateRules(RuleFor(x => x.Request));
        RuleFor(x => x.Request.BusinessRequestNo)
            .NotEmpty()
            .WithMessage("确认计价必须传入业务请求号")
            .When(x => x.Request is not null);
    }
}

/// <summary>提交命令验证器。</summary>
public sealed class CommitPricingCommandValidator : AbstractValidator<CommitPricingCommand>
{
    /// <summary>初始化验证规则。</summary>
    public CommitPricingCommandValidator()
    {
        RuleFor(x => x.Request).NotNull().WithMessage("提交请求不能为空")
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.RequestId).GreaterThan(0).WithMessage("RequestId 必须大于0");
                RuleFor(x => x.Request.ActualTotalAmount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("实际落账总金额不能小于0")
                    .When(x => x.Request.ActualTotalAmount.HasValue);
                RuleFor(x => x.Request.CommittedAt)
                    .Must(value => !value.HasValue || value.Value != default)
                    .WithMessage("HIS落账业务时间不能为空")
                    .When(x => x.Request.CommittedAt.HasValue);
                RuleForEach(x => x.Request.ActualItems)
                    .NotNull()
                    .WithMessage("实际落账明细不能为空")
                    .ChildRules(item =>
                    {
                        item.RuleFor(x => x.ItemCode).NotEmpty().WithMessage("实际落账明细项目编码不能为空");
                        item.RuleFor(x => x.FinalQty).GreaterThanOrEqualTo(0).WithMessage("实际落账数量不能小于0");
                        item.RuleFor(x => x.FinalAmount).GreaterThanOrEqualTo(0).WithMessage("实际落账金额不能小于0");
                    });
            });
    }
}

/// <summary>取消命令验证器。</summary>
public sealed class CancelPricingCommandValidator : AbstractValidator<CancelPricingCommand>
{
    /// <summary>初始化验证规则。</summary>
    public CancelPricingCommandValidator()
    {
        RuleFor(x => x.Request).NotNull().WithMessage("取消请求不能为空")
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.RequestId).GreaterThan(0).WithMessage("RequestId 必须大于0");
                RuleFor(x => x.Request.CancelledAt)
                    .Must(value => !value.HasValue || value.Value != default)
                    .WithMessage("HIS取消业务时间不能为空")
                    .When(x => x.Request.CancelledAt.HasValue);
            });
    }
}

/// <summary>冲正命令验证器。</summary>
public sealed class ReversePricingCommandValidator : AbstractValidator<ReversePricingCommand>
{
    /// <summary>初始化验证规则。</summary>
    public ReversePricingCommandValidator()
    {
        RuleFor(x => x.Request).NotNull().WithMessage("冲正请求不能为空")
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.OriginalRequestId).GreaterThan(0).WithMessage("OriginalRequestId 必须大于0");
                RuleFor(x => x.Request.ReverseNo).NotEmpty().WithMessage("ReverseNo 不能为空");
                RuleFor(x => x.Request.ReverseQty)
                    .GreaterThan(0)
                    .WithMessage("退费数量必须大于0")
                    .When(x => x.Request.ReverseQty.HasValue);
                RuleFor(x => x.Request.ReverseAmt)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("退费金额不能小于0")
                    .When(x => x.Request.ReverseAmt.HasValue);
                RuleFor(x => x.Request.ReverseTime)
                    .Must(value => !value.HasValue || value.Value != default)
                    .WithMessage("退费业务时间不能为空")
                    .When(x => x.Request.ReverseTime.HasValue);
            });
    }
}

/// <summary>特殊项目查询验证器。</summary>
public sealed class GetSpecialFlagQueryValidator : AbstractValidator<GetSpecialFlagQuery>
{
    /// <summary>初始化验证规则。</summary>
    public GetSpecialFlagQueryValidator()
    {
        RuleFor(x => x.ItemCode).NotEmpty().WithMessage("项目编码不能为空");
        RuleFor(x => x.BusinessChargeTime)
            .Must(value => !value.HasValue || value.Value != default)
            .WithMessage("业务收费发生时间不能为空")
            .When(x => x.BusinessChargeTime.HasValue);
    }
}
