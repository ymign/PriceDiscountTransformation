using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;

namespace Pricing.RuleCenter.Tests;

internal sealed class PricingWorkflowHarness
{
    private readonly PricingSimulateWorkflow _simulateWorkflow;
    private readonly PricingConfirmWorkflow _confirmWorkflow;
    private readonly PricingCommitWorkflow _commitWorkflow;
    private readonly PricingCancelWorkflow _cancelWorkflow;
    private readonly PricingReverseWorkflow _reverseWorkflow;
    private readonly PricingSpecialFlagResolver _specialFlagResolver;

    public PricingWorkflowHarness(
        PricingSimulateWorkflow simulateWorkflow,
        PricingConfirmWorkflow confirmWorkflow,
        PricingCommitWorkflow commitWorkflow,
        PricingCancelWorkflow cancelWorkflow,
        PricingReverseWorkflow reverseWorkflow,
        PricingSpecialFlagResolver specialFlagResolver)
    {
        _simulateWorkflow = simulateWorkflow;
        _confirmWorkflow = confirmWorkflow;
        _commitWorkflow = commitWorkflow;
        _cancelWorkflow = cancelWorkflow;
        _reverseWorkflow = reverseWorkflow;
        _specialFlagResolver = specialFlagResolver;
    }

    public Task<PricingCalculateResponse> SimulateAsync(PricingCalculateRequest request)
    {
        return _simulateWorkflow.ExecuteAsync(request);
    }

    public Task<PricingCalculateResponse> ConfirmAsync(PricingCalculateRequest request)
    {
        return _confirmWorkflow.ExecuteAsync(request);
    }

    public Task CommitAsync(PricingCommitRequest request)
    {
        return _commitWorkflow.ExecuteAsync(request);
    }

    public Task CancelAsync(PricingCancelRequest request)
    {
        return _cancelWorkflow.ExecuteAsync(request);
    }

    public Task ReverseAsync(PricingReverseRequest request)
    {
        return _reverseWorkflow.ExecuteAsync(request);
    }

    public Task<SpecialFlagResponse> GetSpecialFlagAsync(string itemCode)
    {
        return _specialFlagResolver.ResolveAsync(itemCode);
    }

    public Task<SpecialFlagResponse> GetSpecialFlagAsync(SpecialFlagRequest request)
    {
        return _specialFlagResolver.ResolveAsync(request);
    }
}
