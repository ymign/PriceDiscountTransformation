using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Catalog;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class FormulaDefAppServiceTests
{
    [Fact]
    public async Task CreateAsync_ReturnsResourceAlreadyExistsBizCodeWhenFormulaCodeExists()
    {
        var repository = new InMemoryFormulaDefRepository
        {
            ByCode = new FormulaDef
            {
                FormulaId = 1,
                FormulaCode = "F001",
                FormulaName = "旧公式"
            }
        };
        var service = new FormulaDefAppService(
            repository,
            new EmptyRuleChangeLogRepository(),
            NullLogger<FormulaDefAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.CreateAsync(new FormulaDefCreateRequest
        {
            FormulaCode = "F001",
            FormulaName = "重复公式",
            ExecutorCode = "EXEC001"
        }));

        Assert.Equal(BizErrorCode.ResourceAlreadyExists, ex.Code);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFormulaNotFoundBizCodeWhenEntityMissing()
    {
        var repository = new InMemoryFormulaDefRepository();
        var service = new FormulaDefAppService(
            repository,
            new EmptyRuleChangeLogRepository(),
            NullLogger<FormulaDefAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.UpdateAsync(999, new FormulaDefUpdateRequest
        {
            FormulaName = "不存在",
            ExecutorCode = "EXEC999"
        }));

        Assert.Equal(BizErrorCode.FormulaNotFound, ex.Code);
    }

    [Fact]
    public async Task ToggleAsync_ReturnsFormulaNotFoundBizCodeWhenEntityMissing()
    {
        var repository = new InMemoryFormulaDefRepository();
        var service = new FormulaDefAppService(
            repository,
            new EmptyRuleChangeLogRepository(),
            NullLogger<FormulaDefAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.ToggleAsync(999));

        Assert.Equal(BizErrorCode.FormulaNotFound, ex.Code);
    }

    private sealed class InMemoryFormulaDefRepository : IFormulaDefRepository
    {
        public FormulaDef? ByCode { get; set; }
        public FormulaDef? ById { get; set; }

        public Task<IReadOnlyList<FormulaDef>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<FormulaDef>)Array.Empty<FormulaDef>());

        public Task<FormulaDef?> GetByIdAsync(long formulaId) => Task.FromResult(ById);

        public Task<FormulaDef?> GetByCodeAsync(string formulaCode) => Task.FromResult(ByCode);

        public Task<long> InsertAsync(FormulaDef entity) => Task.FromResult(1L);

        public Task<bool> UpdateAsync(FormulaDef entity) => Task.FromResult(true);

        public Task<bool> SetEnabledAsync(long formulaId, string isEnabled) => Task.FromResult(true);
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());

        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }
}
