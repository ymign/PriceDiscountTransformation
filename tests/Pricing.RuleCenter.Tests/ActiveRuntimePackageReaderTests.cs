using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ActiveRuntimePackageReaderTests
{
    [Fact]
    public async Task EffectiveRuleSnapshotLoader_LoadsRulesFromDirectRuleTables()
    {
        var loader = new EffectiveRuleSnapshotLoader(new RuleMatchRepositories(
            new FixedRuleHeaderRepository(new RuleAggregate
            {
                RuleId = 101,
                RuleCode = "RULE_DIRECT_001",
                RuleName = "直接规则",
                ItemCode = "ITEM001",
                Status = RuleStatusCodes.Published,
                IsEnabled = EnableFlag.Yes,
                CurrentVersion = 2,
                RuleCategory = "FORMULA_PRICING"
            }),
            new FixedRuleConditionRepository(new RuleCondition
            {
                RuleId = 101,
                VersionNo = 2,
                ConditionType = RuleConditionTypeCodes.ItemMatch,
                IsEnabled = EnableFlag.Yes
            }),
            new FixedRuleActionRepository(new RuleAction
            {
                RuleId = 101,
                VersionNo = 2,
                ActionType = RuleActionTypeCodes.FormulaCalc,
                IsEnabled = EnableFlag.Yes
            }),
            new EmptyDictRepository()));

        var result = await loader.LoadCurrentAsync("ITEM001");

        var snapshot = Assert.Single(result.Snapshots);
        Assert.Equal(101, snapshot.Header.RuleId);
        Assert.Equal("ITEM001", snapshot.Header.ItemCode);
        Assert.Single(snapshot.Conditions);
        Assert.Single(snapshot.Actions);
    }

    [Fact]
    public void RuleMatchRepositories_DoesNotExposeRuntimePackageReadRepositories()
    {
        var source = File.ReadAllText(ResolveRepoFile(
            "src",
            "Pricing.RuleCenter.Application",
            "Application",
            "Engine",
            "RuleMatchRepositories.cs"));

        Assert.DoesNotContain("IRuntimePackageStateRepository", source);
        Assert.DoesNotContain("IRuntimeRuleReadRepository", source);
        Assert.DoesNotContain("RuntimePackageStateRepository", source);
        Assert.DoesNotContain("RuntimeRuleReadRepository", source);
    }

    [Fact]
    public void EffectiveRuleSnapshotLoader_DoesNotReferenceRuntimePackageReadModel()
    {
        var source = File.ReadAllText(ResolveRepoFile(
            "src",
            "Pricing.RuleCenter.Application",
            "Application",
            "Engine",
            "RuleRuntimeSnapshot",
            "EffectiveRuleSnapshotLoader.cs"));

        Assert.DoesNotContain("ActiveRuntimePackageReader", source);
        Assert.DoesNotContain("RuntimeRuleProjectionAdapter", source);
        Assert.DoesNotContain("RuntimeRulesById", source);
        Assert.DoesNotContain("RuntimePackageId", source);
        Assert.DoesNotContain("RuntimePackageVersion", source);
    }

    private static string ResolveRepoFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"无法定位仓库文件：{Path.Combine(pathParts)}");
    }

    private sealed class FixedRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly IReadOnlyList<RuleAggregate> _headers;

        public FixedRuleHeaderRepository(params RuleAggregate[] headers)
        {
            _headers = headers;
        }

        public Task<RuleAggregate?> GetByIdAsync(long ruleId) =>
            Task.FromResult(_headers.SingleOrDefault(header => header.RuleId == ruleId));

        public Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId) => GetByIdAsync(ruleId);

        public Task<RuleAggregate?> GetByCodeAsync(string ruleCode) =>
            Task.FromResult(_headers.SingleOrDefault(header => header.RuleCode == ruleCode));

        public Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)_headers
                .Where(header => string.Equals(header.ItemCode, itemCode, StringComparison.Ordinal))
                .ToList());

        public Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(
            string? itemCode,
            string? status,
            string? category,
            int pageIndex,
            int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>(), 0));

        public Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>());

        public Task<long> InsertAsync(RuleAggregate entity) => Task.FromResult(0L);

        public Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null) =>
            Task.FromResult(false);

        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class FixedRuleConditionRepository : IRuleConditionRepository
    {
        private readonly IReadOnlyList<RuleCondition> _conditions;

        public FixedRuleConditionRepository(params RuleCondition[] conditions)
        {
            _conditions = conditions;
        }

        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleCondition>)_conditions
                .Where(condition => condition.RuleId == ruleId && condition.VersionNo == versionNo)
                .ToList());

        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities) => Task.CompletedTask;

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class FixedRuleActionRepository : IRuleActionRepository
    {
        private readonly IReadOnlyList<RuleAction> _actions;

        public FixedRuleActionRepository(params RuleAction[] actions)
        {
            _actions = actions;
        }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleAction>)_actions
                .Where(action => action.RuleId == ruleId && action.VersionNo == versionNo)
                .ToList());

        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class EmptyDictRepository : IDictRepository
    {
        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);

        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)Array.Empty<Dict>());

        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());

        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);

        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);

        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);

        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }
}
