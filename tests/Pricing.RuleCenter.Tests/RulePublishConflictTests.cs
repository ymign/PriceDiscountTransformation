using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Rules.Publishing;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RulePublishConflictTests
{
    [Fact]
    public async Task PublishAsync_RejectsWhenApprovalIsMissing()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var approvalRepository = new InMemoryRuleApprovalRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            approvalRepository: approvalRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 101,
            RuleCode = "R-APPROVAL-MISSING",
            ItemCode = "ITEM101",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            UpdatedAt = new DateTime(2026, 5, 22, 9, 0, 0)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 1001,
            RuleId = 101,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(101, 1, new RuleAction
        {
            RuleId = 101,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 101, 1, 2101, 3101);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(101, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.ApprovalRequired, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_RejectsWhenApprovalIsOlderThanLatestDraftChange()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var approvalRepository = new InMemoryRuleApprovalRepository();
        var changeLogRepository = new InMemoryRuleChangeLogRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            approvalRepository: approvalRepository,
            changeLogRepository: changeLogRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 102,
            RuleCode = "R-APPROVAL-STALE",
            ItemCode = "ITEM102",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            UpdatedAt = new DateTime(2026, 5, 22, 10, 0, 0)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 1002,
            RuleId = 102,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(102, 1, new RuleAction
        {
            RuleId = 102,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        approvalRepository.Items.Add(new RuleApproval
        {
            ApprovalId = 1,
            RuleId = 102,
            VersionNo = 1,
            ActionType = "PUBLISH",
            ApprovalStatus = "APPROVED",
            SubmittedBy = "maker",
            SubmittedAt = new DateTime(2026, 5, 22, 8, 0, 0),
            ReviewedBy = "checker",
            ReviewedAt = new DateTime(2026, 5, 22, 8, 30, 0),
            ReviewComment = "通过"
        });
        changeLogRepository.Items.Add(new RuleChangeLog
        {
            ChangeId = 11,
            RuleId = 102,
            VersionNo = 1,
            ChangeType = "SAVE_ACTIONS",
            ChangedBy = "maker",
            ChangedAt = new DateTime(2026, 5, 22, 9, 30, 0),
            ChangeSummary = "审批后又改了动作"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 102, 1, 2102, 3102);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(102, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.ApprovalOutdated, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_AllowsWhenLatestApprovalPassedAfterLatestDraftChange()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var approvalRepository = new InMemoryRuleApprovalRepository();
        var changeLogRepository = new InMemoryRuleChangeLogRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            approvalRepository: approvalRepository,
            changeLogRepository: changeLogRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 103,
            RuleCode = "R-APPROVAL-OK",
            ItemCode = "ITEM103",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            UpdatedAt = new DateTime(2026, 5, 22, 9, 0, 0)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 1003,
            RuleId = 103,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(103, 1, new RuleAction
        {
            RuleId = 103,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        changeLogRepository.Items.Add(new RuleChangeLog
        {
            ChangeId = 12,
            RuleId = 103,
            VersionNo = 1,
            ChangeType = "SAVE_ACTIONS",
            ChangedBy = "maker",
            ChangedAt = new DateTime(2026, 5, 22, 9, 30, 0),
            ChangeSummary = "完成动作调整"
        });
        approvalRepository.Items.Add(new RuleApproval
        {
            ApprovalId = 2,
            RuleId = 103,
            VersionNo = 1,
            ActionType = "PUBLISH",
            ApprovalStatus = "APPROVED",
            SubmittedBy = "maker",
            SubmittedAt = new DateTime(2026, 5, 22, 9, 40, 0),
            ReviewedBy = "checker",
            ReviewedAt = new DateTime(2026, 5, 22, 10, 0, 0),
            ReviewComment = "通过"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 103, 1, 2103, 3103);

        await service.PublishAsync(103, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal("PUBLISHED", headerRepository.Headers.Single(h => h.RuleId == 103).Status);
    }

    [Fact]
    public async Task PublishAsync_BlocksFormulaConflictForSameItemSceneAndEffectiveRange()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            new FixedDictRepository(new[]
            {
                new Dict
                {
                    DictType = "MUTUALLY_EXCLUSIVE_ACTION_TYPE",
                    DictCode = "APPLY_MAX_AMOUNT",
                    SortNo = 10,
                    IsEnabled = "Y"
                }
            }),
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.AddRange(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "R-OLD",
                ItemCode = "ITEM001",
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "R-NEW",
                ItemCode = "ITEM001",
                CurrentVersion = 0,
                Status = "DRAFT",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 6, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            }
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 21, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" }
        });
        conditionRepository.Add(1, 1, new RuleCondition
        {
            RuleId = 1,
            VersionNo = 1,
            ConditionType = "CHARGE_SCENE",
            RightValue = "OUTPATIENT",
            IsEnabled = "Y"
        });
        conditionRepository.Add(2, 1, new RuleCondition
        {
            RuleId = 2,
            VersionNo = 1,
            ConditionType = "CHARGE_SCENE",
            RightValue = "OUTPATIENT",
            IsEnabled = "Y"
        });
        actionRepository.Add(1, 1, new RuleAction
        {
            RuleId = 1,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            IsEnabled = "Y"
        });
        actionRepository.Add(2, 1, new RuleAction
        {
            RuleId = 2,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 2, 1, 2001, 3001);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(2, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));
        Assert.Equal(BizErrorCode.RuleOverlapConflict, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_AllowsConvertQtyRulesWithDifferentBodyParts()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.AddRange(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "R-HEAD",
                ItemCode = "ITEM001",
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "R-BODY",
                ItemCode = "ITEM001",
                CurrentVersion = 0,
                Status = "DRAFT",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            }
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 21, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" }
        });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionType = "BODY_PART", RightValue = "HEAD", IsEnabled = "Y" });
        conditionRepository.Add(2, 1, new RuleCondition { RuleId = 2, VersionNo = 1, ConditionType = "BODY_PART", RightValue = "BODY", IsEnabled = "Y" });
        actionRepository.Add(1, 1, new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });
        actionRepository.Add(2, 1, new RuleAction { RuleId = 2, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });
        AddPassingTestCase(testCaseRepository, testRunRepository, 2, 1, 2002, 3002);

        await service.PublishAsync(2, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal("PUBLISHED", headerRepository.Headers.Single(h => h.RuleId == 2).Status);
    }

    [Fact]
    public async Task PublishAsync_AllowsConvertQtyWhenSceneAndBodyOnlyOverlapAcrossDifferentConditionGroups()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.AddRange(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "R-OLD",
                ItemCode = "ITEM001",
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "R-NEW",
                ItemCode = "ITEM001",
                CurrentVersion = 0,
                Status = "DRAFT",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            }
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 21, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" }
        });

        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G1", ConditionType = "CHARGE_SCENE", RightValue = "SCENE_A", IsEnabled = "Y" });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G1", ConditionType = "BODY_PART", RightValue = "HEAD", IsEnabled = "Y" });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G2", ConditionType = "CHARGE_SCENE", RightValue = "SCENE_B", IsEnabled = "Y" });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G2", ConditionType = "BODY_PART", RightValue = "TRUNK", IsEnabled = "Y" });

        conditionRepository.Add(2, 1, new RuleCondition { RuleId = 2, VersionNo = 1, ConditionGroup = "G1", ConditionType = "CHARGE_SCENE", RightValue = "SCENE_B", IsEnabled = "Y" });
        conditionRepository.Add(2, 1, new RuleCondition { RuleId = 2, VersionNo = 1, ConditionGroup = "G1", ConditionType = "BODY_PART", RightValue = "HEAD", IsEnabled = "Y" });

        actionRepository.Add(1, 1, new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });
        actionRepository.Add(2, 1, new RuleAction { RuleId = 2, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });
        AddPassingTestCase(testCaseRepository, testRunRepository, 2, 1, 2003, 3003);

        await service.PublishAsync(2, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal("PUBLISHED", headerRepository.Headers.Single(h => h.RuleId == 2).Status);
    }

    [Fact]
    public async Task PublishAsync_ClearsRuntimeCacheAfterSuccessfulPublish()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var runtimeCache = new CapturingRuleRuntimeCacheInvalidator();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            runtimeCacheInvalidator: runtimeCache,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 3,
            RuleCode = "R-PUBLISH",
            ItemCode = "ITEM003",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 31,
            RuleId = 3,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(3, 1, new RuleAction
        {
            RuleId = 3,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 3, 1, 2004, 3004);

        await service.PublishAsync(3, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal(1, runtimeCache.ClearCount);
    }
    [Fact]
    public async Task PublishAsync_WhenCacheVersionBroadcastFails_KeepsRetryableOutboxRows()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var runtimeCache = new CapturingRuleRuntimeCacheInvalidator();
        var outboxRepository = new InMemoryRuleCacheInvalidationOutboxRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            runtimeCacheInvalidator: runtimeCache,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository,
            cacheVersionSynchronizer: new ThrowingCacheVersionSynchronizer(),
            cacheInvalidationOutboxRepository: outboxRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 14,
            RuleCode = "R-OUTBOX",
            ItemCode = "ITEM014",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 141,
            RuleId = 14,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(14, 1, new RuleAction
        {
            RuleId = 14,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 14, 1, 2014, 3014);

        await service.PublishAsync(14, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal("PUBLISHED", headerRepository.Headers.Single().Status);
        Assert.Equal(1, runtimeCache.ClearCount);
        Assert.Equal(2, outboxRepository.Items.Count);
        Assert.Contains(outboxRepository.Items, item => item.CacheScope == CacheVersionSynchronizer.EffectiveRulesScope);
        Assert.Contains(outboxRepository.Items, item => item.CacheScope == CacheVersionSynchronizer.ActionTypeOrderScope);
        Assert.All(outboxRepository.Items, item =>
        {
            Assert.Equal(CacheInvalidationOutboxStatusCodes.Failed, item.Status);
            Assert.Equal(1, item.RetryCount);
            Assert.NotNull(item.NextRetryAt);
        });
    }

    [Fact]
    public async Task PublishAsync_ReEnablesRuleWhenPublishingFromDisabledHeader()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 5,
            RuleCode = "R-REPUBLISH",
            ItemCode = "ITEM005",
            CurrentVersion = 1,
            Status = "DISABLED",
            IsEnabled = "N",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion
            {
                VersionId = 51,
                RuleId = 5,
                VersionNo = 1,
                VersionStatus = "DISABLED"
            },
            new RuleVersion
            {
                VersionId = 52,
                RuleId = 5,
                VersionNo = 2,
                VersionStatus = "DRAFT"
            }
        });
        actionRepository.Add(5, 2, new RuleAction
        {
            RuleId = 5,
            VersionNo = 2,
            ActionType = "FORMULA_CALC",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 5, 2, 2005, 3005);

        await service.PublishAsync(5, new RulePublishRequest { VersionNo = 2, PublishedBy = "tester" });

        var header = Assert.Single(headerRepository.Headers);
        Assert.Equal("PUBLISHED", header.Status);
        Assert.Equal("Y", header.IsEnabled);
        Assert.Equal(2, header.CurrentVersion);
    }

    [Fact]
    public async Task RollbackAsync_UsesPublishHistoryInsteadOfHighestDisabledVersion()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var publishRepository = new InMemoryRulePublishRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            publishRepository: publishRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 6,
            RuleCode = "R-ROLLBACK",
            ItemCode = "ITEM006",
            CurrentVersion = 5,
            Status = "PUBLISHED",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 61, RuleId = 6, VersionNo = 3, VersionStatus = "DISABLED" },
            new RuleVersion { VersionId = 62, RuleId = 6, VersionNo = 4, VersionStatus = "DISABLED" },
            new RuleVersion { VersionId = 63, RuleId = 6, VersionNo = 5, VersionStatus = "PUBLISHED" }
        });
        publishRepository.Items.AddRange(new[]
        {
            new RulePublish
            {
                PublishId = 601,
                RuleId = 6,
                FromVersion = null,
                ToVersion = 3,
                ActionType = "PUBLISH",
                PublishedAt = new DateTime(2026, 5, 1, 9, 0, 0)
            },
            new RulePublish
            {
                PublishId = 602,
                RuleId = 6,
                FromVersion = 3,
                ToVersion = 5,
                ActionType = "PUBLISH",
                PublishedAt = new DateTime(2026, 5, 2, 9, 0, 0)
            }
        });

        await service.RollbackAsync(6, new RuleRollbackRequest { PublishedBy = "tester" });

        var header = Assert.Single(headerRepository.Headers);
        Assert.Equal(3, header.CurrentVersion);
        Assert.Equal("PUBLISHED", header.Status);
        Assert.Equal("Y", header.IsEnabled);
        Assert.Equal("PUBLISHED", versionRepository.Versions.Single(v => v.VersionId == 61).VersionStatus);
        Assert.Equal("ROLLED_BACK", versionRepository.Versions.Single(v => v.VersionId == 63).VersionStatus);
    }

    [Fact]
    public async Task PublishAsync_RejectsLimitActionMissingRequiredParams()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var service = CreateService(headerRepository, versionRepository, conditionRepository, actionRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 4,
            RuleCode = "R-BAD-ACTION",
            ItemCode = "ITEM004",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 41,
            RuleId = 4,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(4, 1, new RuleAction
        {
            RuleId = 4,
            VersionNo = 1,
            ActionType = "APPLY_TIME_WINDOW_LIMIT",
            ParamsJson = "{\"windowMinutes\":120}",
            IsEnabled = "Y"
        });

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(4, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.ActionParamMissing, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_RejectsWhenEnabledTestCasesAreMissing()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: new InMemoryRuleTestCaseRepository(),
            testRunRepository: new InMemoryRuleTestRunRepository());

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 7,
            RuleCode = "R-NO-TEST",
            ItemCode = "ITEM007",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 71,
            RuleId = 7,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(7, 1, new RuleAction
        {
            RuleId = 7,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(7, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.MissingTestCase, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_ReturnsRuleNotFoundBizCodeWhenHeaderIsMissing()
    {
        var service = CreateService(
            new InMemoryRuleHeaderRepository(),
            new InMemoryRuleVersionRepository(),
            new InMemoryRuleConditionRepository(),
            new InMemoryRuleActionRepository());

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(999, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.RuleNotFound, ex.Code);
        Assert.Equal(404, ex.HttpStatusCode);
    }

    [Fact]
    public async Task PublishAsync_ReturnsRuleVersionNotFoundBizCodeWhenVersionIsMissing()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 14,
            RuleCode = "R-NO-VERSION",
            ItemCode = "ITEM014",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });

        var service = CreateService(
            headerRepository,
            new InMemoryRuleVersionRepository(),
            new InMemoryRuleConditionRepository(),
            new InMemoryRuleActionRepository());

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(14, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.RuleVersionNotFound, ex.Code);
        Assert.Equal(404, ex.HttpStatusCode);
    }

    [Fact]
    public async Task PublishAsync_RejectsWhenLatestEnabledTestRunDidNotPass()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 8,
            RuleCode = "R-FAILED-TEST",
            ItemCode = "ITEM008",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 81,
            RuleId = 8,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(8, 1, new RuleAction
        {
            RuleId = 8,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        testCaseRepository.Items.Add(new RuleTestCase
        {
            TestCaseId = 801,
            RuleId = 8,
            VersionNo = 1,
            CaseName = "失败用例",
            InputJson = "{\"itemCode\":\"ITEM008\"}",
            ExpectedJson = "{\"finalAmount\":10}",
            IsEnabled = "Y"
        });
        testRunRepository.Items.Add(new RuleTestRun
        {
            TestRunId = 8001,
            TestCaseId = 801,
            RuleId = 8,
            IsPass = "N",
            RunAt = new DateTime(2026, 5, 22, 10, 0, 0)
        });

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(8, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.TestRunFailed, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_RejectsDuplicateChildItemsInAddChildAction()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 9,
            RuleCode = "R-DUP-CHILD",
            ItemCode = "ITEM009",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 91,
            RuleId = 9,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(9, 1, new RuleAction
        {
            RuleId = 9,
            VersionNo = 1,
            ActionType = "ADD_CHILD_ITEM",
            OnError = "STOP",
            IsEnabled = "Y",
            ParamsJson = JsonConvert.SerializeObject(new
            {
                childItems = new[]
                {
                    new { itemCode = "CHILD001", itemName = "子项1", qty = 1m, unitPrice = 10m },
                    new { itemCode = "CHILD001", itemName = "子项1重复", qty = 1m, unitPrice = 10m }
                }
            })
        });
        testCaseRepository.Items.Add(new RuleTestCase
        {
            TestCaseId = 901,
            RuleId = 9,
            VersionNo = 1,
            CaseName = "通过用例",
            InputJson = "{\"itemCode\":\"ITEM009\"}",
            ExpectedJson = "{\"finalAmount\":10}",
            IsEnabled = "Y"
        });
        testRunRepository.Items.Add(new RuleTestRun
        {
            TestRunId = 9001,
            TestCaseId = 901,
            RuleId = 9,
            IsPass = "Y",
            RunAt = new DateTime(2026, 5, 22, 10, 0, 0)
        });

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(9, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.ChildItemDuplicate, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_RejectsCriticalActionWhenOnErrorIsNotStop()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 10,
            RuleCode = "R-ONERROR",
            ItemCode = "ITEM010",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 101,
            RuleId = 10,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(10, 1, new RuleAction
        {
            RuleId = 10,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "SKIP",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 10, 1, 2010, 3010);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.PublishAsync(10, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));

        Assert.Equal(BizErrorCode.ActionOnErrorInvalid, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_LocksHeaderAndTargetVersionInsideTransaction()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 11,
            RuleCode = "R-LOCK",
            ItemCode = "ITEM011",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 111,
            RuleId = 11,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(11, 1, new RuleAction
        {
            RuleId = 11,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 11, 1, 2011, 3011);

        await service.PublishAsync(11, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.True(headerRepository.WasLocked);
        Assert.True(versionRepository.WasLocked);
    }

    [Fact]
    public async Task PublishAsync_UsesCompareAndSetWhenPromotingDraftVersion()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 12,
            RuleCode = "R-CAS",
            ItemCode = "ITEM012",
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.Add(new RuleVersion
        {
            VersionId = 121,
            RuleId = 12,
            VersionNo = 1,
            VersionStatus = "DRAFT"
        });
        actionRepository.Add(12, 1, new RuleAction
        {
            RuleId = 12,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 12, 1, 2012, 3012);

        await service.PublishAsync(12, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        var promoted = Assert.Single(versionRepository.StatusUpdates, u => u.VersionId == 121);
        Assert.Equal("DRAFT", promoted.ExpectedStatus);
        Assert.Equal("PUBLISHED", promoted.NewStatus);
        var headerUpdate = Assert.Single(headerRepository.StatusUpdates);
        Assert.Equal("DRAFT", headerUpdate.ExpectedStatus);
        Assert.Equal("PUBLISHED", headerUpdate.NewStatus);
    }

    [Fact]
    public async Task PublishAsync_AllowsPublishedHeaderToPromoteNextDraftVersion()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var testCaseRepository = new InMemoryRuleTestCaseRepository();
        var testRunRepository = new InMemoryRuleTestRunRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            testCaseRepository: testCaseRepository,
            testRunRepository: testRunRepository);

        headerRepository.Headers.Add(new RuleHeader
        {
            RuleId = 13,
            RuleCode = "R-UPGRADE",
            ItemCode = "ITEM013",
            CurrentVersion = 1,
            Status = "PUBLISHED",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion
            {
                VersionId = 131,
                RuleId = 13,
                VersionNo = 1,
                VersionStatus = "PUBLISHED"
            },
            new RuleVersion
            {
                VersionId = 132,
                RuleId = 13,
                VersionNo = 2,
                VersionStatus = "DRAFT"
            }
        });
        actionRepository.Add(13, 2, new RuleAction
        {
            RuleId = 13,
            VersionNo = 2,
            ActionType = "FORMULA_CALC",
            OnError = "STOP",
            IsEnabled = "Y"
        });
        AddPassingTestCase(testCaseRepository, testRunRepository, 13, 2, 2013, 3013);

        await service.PublishAsync(13, new RulePublishRequest { VersionNo = 2, PublishedBy = "tester" });

        var headerUpdate = Assert.Single(headerRepository.StatusUpdates);
        Assert.Equal("PUBLISHED", headerUpdate.ExpectedStatus);
        Assert.Equal(2, headerRepository.Headers.Single().CurrentVersion);
        Assert.Equal("DISABLED", versionRepository.Versions.Single(v => v.VersionId == 131).VersionStatus);
        Assert.Equal("PUBLISHED", versionRepository.Versions.Single(v => v.VersionId == 132).VersionStatus);
    }

    private static void AddPassingTestCase(
        InMemoryRuleTestCaseRepository testCaseRepository,
        InMemoryRuleTestRunRepository testRunRepository,
        long ruleId,
        int versionNo,
        long testCaseId,
        long testRunId)
    {
        testCaseRepository.Items.Add(new RuleTestCase
        {
            TestCaseId = testCaseId,
            RuleId = ruleId,
            VersionNo = versionNo,
            CaseName = $"Rule-{ruleId}-V{versionNo}",
            InputJson = $"{{\"ruleId\":{ruleId},\"versionNo\":{versionNo}}}",
            ExpectedJson = "{\"isPass\":true}",
            IsEnabled = "Y"
        });
        testRunRepository.Items.Add(new RuleTestRun
        {
            TestRunId = testRunId,
            TestCaseId = testCaseId,
            RuleId = ruleId,
            IsPass = "Y",
            RunAt = new DateTime(2026, 5, 22, 10, 0, 0)
        });
    }

    private static RulePublishService CreateService(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository? dictRepository = null,
        IRuleRuntimeCacheInvalidator? runtimeCacheInvalidator = null,
        IRulePublishRepository? publishRepository = null,
        IRuleApprovalRepository? approvalRepository = null,
        IRuleChangeLogRepository? changeLogRepository = null,
        IRuleTestCaseRepository? testCaseRepository = null,
        IRuleTestRunRepository? testRunRepository = null,
        ICacheVersionSynchronizer? cacheVersionSynchronizer = null,
        IRuleCacheInvalidationOutboxRepository? cacheInvalidationOutboxRepository = null)
    {
        cacheVersionSynchronizer ??= new NoopCacheVersionSynchronizer();
        cacheInvalidationOutboxRepository ??= new InMemoryRuleCacheInvalidationOutboxRepository();
        var outboxProcessor = new RuleCacheInvalidationOutboxProcessor(
            cacheInvalidationOutboxRepository,
            cacheVersionSynchronizer,
            NullLogger<RuleCacheInvalidationOutboxProcessor>.Instance);

        return new RulePublishService(
            new RulePublishLifecycleRepositories(
                headerRepository,
                versionRepository,
                publishRepository ?? new EmptyRulePublishRepository(),
                changeLogRepository ?? new EmptyRuleChangeLogRepository(),
                approvalRepository ?? new AutoApprovedRuleApprovalRepository(versionRepository)),
            new RulePublishDefinitionRepositories(
                conditionRepository,
                actionRepository,
                dictRepository ?? new EmptyDictRepository(),
                testCaseRepository ?? new InMemoryRuleTestCaseRepository(),
                testRunRepository ?? new InMemoryRuleTestRunRepository()),
            new MemoryCache(new MemoryCacheOptions()),
            new NoopUnitOfWork(),
            cacheVersionSynchronizer,
            cacheInvalidationOutboxRepository,
            outboxProcessor,
            runtimeCacheInvalidator ?? new EmptyRuleRuntimeCacheInvalidator(),
            NullLogger<RulePublishService>.Instance);
    }

    private sealed class NoopUnitOfWork : IUnitOfWork
    {
        public Task BeginAsync() => Task.CompletedTask;

        public Task CommitAsync() => Task.CompletedTask;

        public Task RollbackAsync() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    private sealed class EmptyRuleRuntimeCacheInvalidator : IRuleRuntimeCacheInvalidator
    {
        public void ClearRuntimeCache()
        {
        }
    }

    private sealed class CapturingRuleRuntimeCacheInvalidator : IRuleRuntimeCacheInvalidator
    {
        public int ClearCount { get; private set; }

        public void ClearRuntimeCache()
        {
            ClearCount++;
        }
    }

    private sealed class NoopCacheVersionSynchronizer : ICacheVersionSynchronizer
    {
        public Task SyncAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default) =>
            Task.FromResult(0L);
    }
    private sealed class ThrowingCacheVersionSynchronizer : ICacheVersionSynchronizer
    {
        public Task SyncAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException($"broadcast failed: {cacheScope}");
        }
    }

    private sealed class InMemoryRuleCacheInvalidationOutboxRepository : IRuleCacheInvalidationOutboxRepository
    {
        public List<RuleCacheInvalidationOutbox> Items { get; } = new();

        public Task<long> InsertAsync(RuleCacheInvalidationOutbox entity)
        {
            entity.OutboxId = Items.Count == 0 ? 1 : Items.Max(i => i.OutboxId) + 1;
            Items.Add(entity);
            return Task.FromResult(entity.OutboxId);
        }

        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetPendingAsync(DateTime now, int maxCount)
        {
            var items = Items
                .Where(i => i.Status != CacheInvalidationOutboxStatusCodes.Processed)
                .Where(i => i.NextRetryAt is null || i.NextRetryAt <= now)
                .OrderBy(i => i.CreatedAt)
                .ThenBy(i => i.OutboxId)
                .Take(maxCount)
                .ToList();
            return Task.FromResult((IReadOnlyList<RuleCacheInvalidationOutbox>)items);
        }

        public Task<bool> MarkProcessedAsync(long outboxId, DateTime processedAt)
        {
            var item = Items.Single(i => i.OutboxId == outboxId);
            item.Status = CacheInvalidationOutboxStatusCodes.Processed;
            item.ProcessedAt = processedAt;
            item.NextRetryAt = null;
            item.LastError = null;
            return Task.FromResult(true);
        }

        public Task<bool> MarkFailedAsync(long outboxId, string lastError, int retryCount, DateTime nextRetryAt)
        {
            var item = Items.Single(i => i.OutboxId == outboxId);
            item.Status = CacheInvalidationOutboxStatusCodes.Failed;
            item.LastError = lastError;
            item.RetryCount = retryCount;
            item.NextRetryAt = nextRetryAt;
            return Task.FromResult(true);
        }
    }

    private sealed class InMemoryRuleHeaderRepository : IRuleHeaderRepository
    {
        public List<RuleHeader> Headers { get; } = new();
        public List<(long RuleId, string NewStatus, string? ExpectedStatus)> StatusUpdates { get; } = new();
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult(Clone(Headers.SingleOrDefault(h => h.RuleId == ruleId)));
        public bool WasLocked { get; private set; }
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId)
        {
            WasLocked = true;
            return Task.FromResult(Clone(Headers.SingleOrDefault(h => h.RuleId == ruleId)));
        }
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult(Clone(Headers.SingleOrDefault(h => h.RuleCode == ruleCode)));
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Headers.Where(h => h.ItemCode == itemCode).ToList());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Headers.ToList(), Headers.Count));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Headers.Where(h => h.IsEnabled == "Y" && h.Status == "PUBLISHED").ToList());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(entity.RuleId);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null)
        {
            var current = Headers.Single(h => h.RuleId == entity.RuleId);
            if (!string.IsNullOrWhiteSpace(expectedCurrentStatus) &&
                !string.Equals(current.Status, expectedCurrentStatus, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            StatusUpdates.Add((entity.RuleId, entity.Status, expectedCurrentStatus));
            current.RuleCode = entity.RuleCode;
            current.RuleName = entity.RuleName;
            current.RuleCategory = entity.RuleCategory;
            current.RuleScope = entity.RuleScope;
            current.ItemCode = entity.ItemCode;
            current.ItemName = entity.ItemName;
            current.GroupCode = entity.GroupCode;
            current.Priority = entity.Priority;
            current.CurrentVersion = entity.CurrentVersion;
            current.Status = entity.Status;
            current.IsEnabled = entity.IsEnabled;
            current.EffectiveFrom = entity.EffectiveFrom;
            current.EffectiveTo = entity.EffectiveTo;
            current.RollbackMode = entity.RollbackMode;
            current.Remark = entity.Remark;
            current.UpdatedBy = entity.UpdatedBy;
            current.UpdatedAt = entity.UpdatedAt;
            return Task.FromResult(true);
        }
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(Headers.Any(h => h.RuleCode == ruleCode));

        private static RuleHeader? Clone(RuleHeader? source)
        {
            if (source is null)
            {
                return null;
            }

            return new RuleHeader
            {
                RuleId = source.RuleId,
                RuleCode = source.RuleCode,
                RuleName = source.RuleName,
                RuleCategory = source.RuleCategory,
                RuleScope = source.RuleScope,
                ItemCode = source.ItemCode,
                ItemName = source.ItemName,
                GroupCode = source.GroupCode,
                Priority = source.Priority,
                CurrentVersion = source.CurrentVersion,
                Status = source.Status,
                IsEnabled = source.IsEnabled,
                EffectiveFrom = source.EffectiveFrom,
                EffectiveTo = source.EffectiveTo,
                RollbackMode = source.RollbackMode,
                Remark = source.Remark,
                CreatedBy = source.CreatedBy,
                CreatedAt = source.CreatedAt,
                UpdatedBy = source.UpdatedBy,
                UpdatedAt = source.UpdatedAt
            };
        }
    }

    private sealed class InMemoryRuleVersionRepository : IRuleVersionRepository
    {
        public List<RuleVersion> Versions { get; } = new();
        public List<(long VersionId, string NewStatus, string? ExpectedStatus)> StatusUpdates { get; } = new();
        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult(Versions.SingleOrDefault(v => v.VersionId == versionId));
        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo) => Task.FromResult(Versions.SingleOrDefault(v => v.RuleId == ruleId && v.VersionNo == versionNo));
        public bool WasLocked { get; private set; }
        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo)
        {
            WasLocked = true;
            return Task.FromResult(Versions.SingleOrDefault(v => v.RuleId == ruleId && v.VersionNo == versionNo));
        }
        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleVersion>)Versions.Where(v => v.RuleId == ruleId).ToList());
        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(entity.VersionId);
        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null)
        {
            var version = Versions.Single(v => v.VersionId == versionId);
            if (!string.IsNullOrWhiteSpace(expectedCurrentStatus) &&
                !string.Equals(version.VersionStatus, expectedCurrentStatus, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            StatusUpdates.Add((versionId, status, expectedCurrentStatus));
            version.VersionStatus = status;
            return Task.FromResult(true);
        }
    }

    private sealed class InMemoryRuleConditionRepository : IRuleConditionRepository
    {
        private readonly Dictionary<(long RuleId, int VersionNo), List<RuleCondition>> _items = new();

        public void Add(long ruleId, int versionNo, RuleCondition condition)
        {
            var key = (ruleId, versionNo);
            if (!_items.TryGetValue(key, out var items))
            {
                items = new List<RuleCondition>();
                _items[key] = items;
            }

            items.Add(condition);
        }

        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleCondition>)(_items.TryGetValue((ruleId, versionNo), out var items) ? items : new List<RuleCondition>()));

        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class InMemoryRuleActionRepository : IRuleActionRepository
    {
        private readonly Dictionary<(long RuleId, int VersionNo), List<RuleAction>> _items = new();

        public void Add(long ruleId, int versionNo, RuleAction action)
        {
            var key = (ruleId, versionNo);
            if (!_items.TryGetValue(key, out var items))
            {
                items = new List<RuleAction>();
                _items[key] = items;
            }

            items.Add(action);
        }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleAction>)(_items.TryGetValue((ruleId, versionNo), out var items) ? items : new List<RuleAction>()));

        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class EmptyRulePublishRepository : IRulePublishRepository
    {
        public Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RulePublish>)Array.Empty<RulePublish>());
        public Task<long> InsertAsync(RulePublish entity) => Task.FromResult(0L);
    }

    private sealed class InMemoryRulePublishRepository : IRulePublishRepository
    {
        public List<RulePublish> Items { get; } = new();

        public Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RulePublish>)Items
                .Where(p => p.RuleId == ruleId)
                .OrderByDescending(p => p.PublishedAt)
                .ThenByDescending(p => p.PublishId)
                .ToList());

        public Task<long> InsertAsync(RulePublish entity)
        {
            Items.Add(entity);
            return Task.FromResult(entity.PublishId);
        }
    }

    private sealed class InMemoryRuleTestCaseRepository : IRuleTestCaseRepository
    {
        public List<RuleTestCase> Items { get; } = new();

        public Task<IReadOnlyList<RuleTestCase>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleTestCase>)Items
                .Where(t => t.RuleId == ruleId && t.VersionNo == versionNo)
                .ToList());

        public Task<RuleTestCase?> GetByIdAsync(long testCaseId) =>
            Task.FromResult(Items.SingleOrDefault(t => t.TestCaseId == testCaseId));

        public Task<long> InsertAsync(RuleTestCase entity)
        {
            Items.Add(entity);
            return Task.FromResult(entity.TestCaseId);
        }

        public Task DeleteAsync(long testCaseId)
        {
            Items.RemoveAll(t => t.TestCaseId == testCaseId);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryRuleTestRunRepository : IRuleTestRunRepository
    {
        public List<RuleTestRun> Items { get; } = new();

        public Task<IReadOnlyList<RuleTestRun>> GetByTestCaseIdAsync(long testCaseId) =>
            Task.FromResult((IReadOnlyList<RuleTestRun>)Items
                .Where(r => r.TestCaseId == testCaseId)
                .OrderByDescending(r => r.RunAt)
                .ToList());

        public Task<long> InsertAsync(RuleTestRun entity)
        {
            Items.Add(entity);
            return Task.FromResult(entity.TestRunId);
        }
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }

    private sealed class InMemoryRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public List<RuleChangeLog> Items { get; } = new();

        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleChangeLog>)Items
                .Where(i => i.RuleId == ruleId)
                .OrderByDescending(i => i.ChangedAt)
                .ThenByDescending(i => i.ChangeId)
                .ToList());

        public Task<long> InsertAsync(RuleChangeLog entity)
        {
            Items.Add(entity);
            return Task.FromResult(entity.ChangeId);
        }
    }

    private sealed class InMemoryRuleApprovalRepository : IRuleApprovalRepository
    {
        public List<RuleApproval> Items { get; } = new();
        public List<(long ApprovalId, string Status, string ReviewedBy, string ReviewComment)> StatusUpdates { get; } = new();

        public Task<IReadOnlyList<RuleApproval>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleApproval>)Items
                .Where(i => i.RuleId == ruleId)
                .OrderByDescending(i => i.SubmittedAt)
                .ThenByDescending(i => i.ApprovalId)
                .ToList());

        public Task<IReadOnlyList<RuleApproval>> GetPendingAsync() =>
            Task.FromResult((IReadOnlyList<RuleApproval>)Items
                .Where(i => string.Equals(i.ApprovalStatus, "PENDING", StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.SubmittedAt)
                .ThenBy(i => i.ApprovalId)
                .ToList());

        public Task<long> InsertAsync(RuleApproval entity)
        {
            if (entity.ApprovalId == 0)
            {
                entity.ApprovalId = Items.Count == 0 ? 1 : Items.Max(i => i.ApprovalId) + 1;
            }

            Items.Add(entity);
            return Task.FromResult(entity.ApprovalId);
        }

        public Task<bool> UpdateStatusAsync(
            long approvalId,
            string status,
            string reviewedBy,
            string reviewComment,
            string? expectedCurrentStatus = null)
        {
            var item = Items.Single(i => i.ApprovalId == approvalId);
            if (!string.IsNullOrWhiteSpace(expectedCurrentStatus) &&
                !string.Equals(item.ApprovalStatus, expectedCurrentStatus, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            item.ApprovalStatus = status;
            item.ReviewedBy = reviewedBy;
            item.ReviewComment = reviewComment;
            item.ReviewedAt = item.ReviewedAt ?? DateTime.Now;
            StatusUpdates.Add((approvalId, status, reviewedBy, reviewComment));
            return Task.FromResult(true);
        }
    }

    private sealed class AutoApprovedRuleApprovalRepository : IRuleApprovalRepository
    {
        private readonly IRuleVersionRepository _versionRepository;

        public AutoApprovedRuleApprovalRepository(IRuleVersionRepository versionRepository)
        {
            _versionRepository = versionRepository;
        }

        public async Task<IReadOnlyList<RuleApproval>> GetByRuleIdAsync(long ruleId)
        {
            var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
            var approvals = new List<RuleApproval>();

            foreach (var version in versions)
            {
                approvals.Add(CreateApproved(ruleId, version.VersionNo, "PUBLISH"));
                approvals.Add(CreateApproved(ruleId, version.VersionNo, "DISABLE"));
                approvals.Add(CreateApproved(ruleId, version.VersionNo, "ROLLBACK"));
            }

            return approvals
                .OrderByDescending(i => i.SubmittedAt)
                .ThenByDescending(i => i.ApprovalId)
                .ToList();
        }

        public Task<IReadOnlyList<RuleApproval>> GetPendingAsync() =>
            Task.FromResult((IReadOnlyList<RuleApproval>)Array.Empty<RuleApproval>());

        public Task<long> InsertAsync(RuleApproval entity) => Task.FromResult(entity.ApprovalId);

        public Task<bool> UpdateStatusAsync(
            long approvalId,
            string status,
            string reviewedBy,
            string reviewComment,
            string? expectedCurrentStatus = null) =>
            Task.FromResult(true);

        private static RuleApproval CreateApproved(long ruleId, int versionNo, string actionType)
        {
            return new RuleApproval
            {
                ApprovalId = long.Parse($"{ruleId}{versionNo}{actionType.Length}"),
                RuleId = ruleId,
                VersionNo = versionNo,
                ActionType = actionType,
                ApprovalStatus = "APPROVED",
                SubmittedBy = "auto",
                SubmittedAt = new DateTime(2026, 5, 22, 8, 0, 0),
                ReviewedBy = "auto-checker",
                ReviewedAt = new DateTime(2026, 5, 22, 8, 5, 0),
                ReviewComment = "auto-approved-for-test"
            };
        }
    }

    private sealed class EmptyDictRepository : IDictRepository
    {
        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)Array.Empty<Dict>());
        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);
        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());
        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);
        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }

    private sealed class FixedDictRepository : IDictRepository
    {
        private readonly IReadOnlyList<Dict> _items;

        public FixedDictRepository(IReadOnlyList<Dict> items)
        {
            _items = items;
        }

        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)_items
                .Where(d => d.DictType == dictType && d.IsEnabled == "Y")
                .OrderBy(d => d.SortNo)
                .ToList());
        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);
        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());
        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);
        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }
}


