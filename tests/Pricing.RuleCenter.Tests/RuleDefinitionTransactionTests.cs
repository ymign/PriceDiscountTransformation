using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleDefinitionTransactionTests
{
    [Fact]
    public async Task SaveConditionsAsync_RollsBackDeleteWhenInsertFails()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleConditionRepository(unitOfWork)
        {
            ThrowOnInsert = true
        };
        repository.Seed(100, 1, new RuleCondition
        {
            ConditionId = 1,
            RuleId = 100,
            VersionNo = 1,
            ConditionGroup = "DEFAULT",
            ConditionType = "ITEM_CODE",
            OperatorType = "EQ",
            LeftKey = "itemCode",
            RightValue = "OLD",
            SortNo = 1,
            IsEnabled = "Y"
        });

        var service = new RuleConditionAppService(
            unitOfWork,
            repository,
            new DraftRuleVersionRepository(100, 1),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleConditionAppService>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SaveAsync(100, 1, new RuleConditionSaveRequest
        {
            Conditions = new[]
            {
                new RuleConditionItemRequest
                {
                    ConditionGroup = "DEFAULT",
                    ConditionType = "ITEM_CODE",
                    OperatorType = "EQ",
                    LeftKey = "itemCode",
                    RightValue = "NEW",
                    SortNo = 1,
                    IsEnabled = "Y"
                }
            }
        }));

        var persisted = await repository.GetByRuleAndVersionAsync(100, 1);

        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(0, unitOfWork.CommitCount);
        Assert.Equal(1, unitOfWork.RollbackCount);
        Assert.Single(persisted);
        Assert.Equal("OLD", persisted[0].RightValue);
    }

    [Fact]
    public async Task SaveConditionsAsync_CommitsRebuiltCollectionWhenInsertSucceeds()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleConditionRepository(unitOfWork);
        repository.Seed(100, 1, new RuleCondition
        {
            ConditionId = 1,
            RuleId = 100,
            VersionNo = 1,
            ConditionGroup = "DEFAULT",
            ConditionType = "ITEM_CODE",
            OperatorType = "EQ",
            LeftKey = "itemCode",
            RightValue = "OLD",
            SortNo = 1,
            IsEnabled = "Y"
        });

        var service = new RuleConditionAppService(
            unitOfWork,
            repository,
            new DraftRuleVersionRepository(100, 1),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleConditionAppService>.Instance);

        await service.SaveAsync(100, 1, new RuleConditionSaveRequest
        {
            Conditions = new[]
            {
                new RuleConditionItemRequest
                {
                    ConditionGroup = "DEFAULT",
                    ConditionType = "ITEM_CODE",
                    OperatorType = "EQ",
                    LeftKey = "itemCode",
                    RightValue = "NEW",
                    SortNo = 1,
                    IsEnabled = "Y"
                }
            }
        });

        var persisted = await repository.GetByRuleAndVersionAsync(100, 1);

        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(1, unitOfWork.CommitCount);
        Assert.Equal(0, unitOfWork.RollbackCount);
        Assert.Single(persisted);
        Assert.Equal("NEW", persisted[0].RightValue);
    }

    [Fact]
    public async Task SaveActionsAsync_RollsBackDeleteWhenInsertFails()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleActionRepository(unitOfWork)
        {
            ThrowOnInsert = true
        };
        repository.Seed(200, 2, new RuleAction
        {
            ActionId = 1,
            RuleId = 200,
            VersionNo = 2,
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            ParamsJson = "{\"rate\":0.5}",
            SortNo = 1,
            OnError = "STOP",
            IsEnabled = "Y"
        });

        var service = new RuleActionAppService(
            unitOfWork,
            repository,
            new DraftRuleVersionRepository(200, 2),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleActionAppService>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SaveAsync(200, 2, new RuleActionSaveRequest
        {
            Actions = new[]
            {
                new RuleActionItemRequest
                {
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "INCREMENT_PERCENT",
                    ParamsJson = "{\"rate\":0.8}",
                    SortNo = 1,
                    OnError = "STOP",
                    IsEnabled = "Y"
                }
            }
        }));

        var persisted = await repository.GetByRuleAndVersionAsync(200, 2);

        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(0, unitOfWork.CommitCount);
        Assert.Equal(1, unitOfWork.RollbackCount);
        Assert.Single(persisted);
        Assert.Equal("{\"rate\":0.5}", persisted[0].ParamsJson);
    }

    [Fact]
    public async Task SaveActionsAsync_CommitsRebuiltCollectionWhenInsertSucceeds()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleActionRepository(unitOfWork);
        repository.Seed(200, 2, new RuleAction
        {
            ActionId = 1,
            RuleId = 200,
            VersionNo = 2,
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            ParamsJson = "{\"rate\":0.5}",
            SortNo = 1,
            OnError = "STOP",
            IsEnabled = "Y"
        });

        var service = new RuleActionAppService(
            unitOfWork,
            repository,
            new DraftRuleVersionRepository(200, 2),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleActionAppService>.Instance);

        await service.SaveAsync(200, 2, new RuleActionSaveRequest
        {
            Actions = new[]
            {
                new RuleActionItemRequest
                {
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "INCREMENT_PERCENT",
                    ParamsJson = "{\"rate\":0.8}",
                    SortNo = 1,
                    OnError = "STOP",
                    IsEnabled = "Y"
                }
            }
        });

        var persisted = await repository.GetByRuleAndVersionAsync(200, 2);

        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(1, unitOfWork.CommitCount);
        Assert.Equal(0, unitOfWork.RollbackCount);
        Assert.Single(persisted);
        Assert.Equal("{\"rate\":0.8}", persisted[0].ParamsJson);
    }

    [Fact]
    public async Task SaveConditionsAsync_ReturnsRuleVersionNotFoundBizCodeWhenVersionIsMissing()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleConditionRepository(unitOfWork);
        var service = new RuleConditionAppService(
            unitOfWork,
            repository,
            new DraftRuleVersionRepository(999, 9),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleConditionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(100, 1, new RuleConditionSaveRequest
        {
            Conditions = Array.Empty<RuleConditionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.RuleVersionNotFound, ex.Code);
    }

    [Fact]
    public async Task SaveConditionsAsync_ReturnsVersionStatusNotAllowedBizCodeWhenVersionIsNotDraft()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleConditionRepository(unitOfWork);
        var service = new RuleConditionAppService(
            unitOfWork,
            repository,
            new FixedRuleVersionRepository(new RuleVersion
            {
                RuleId = 100,
                VersionNo = 1,
                VersionStatus = "PUBLISHED"
            }),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleConditionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(100, 1, new RuleConditionSaveRequest
        {
            Conditions = Array.Empty<RuleConditionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.VersionStatusNotAllowed, ex.Code);
    }

    [Fact]
    public async Task SaveActionsAsync_ReturnsRuleVersionNotFoundBizCodeWhenVersionIsMissing()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleActionRepository(unitOfWork);
        var service = new RuleActionAppService(
            unitOfWork,
            repository,
            new DraftRuleVersionRepository(999, 9),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleActionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(200, 2, new RuleActionSaveRequest
        {
            Actions = Array.Empty<RuleActionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.RuleVersionNotFound, ex.Code);
    }

    [Fact]
    public async Task SaveActionsAsync_ReturnsVersionStatusNotAllowedBizCodeWhenVersionIsNotDraft()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleActionRepository(unitOfWork);
        var service = new RuleActionAppService(
            unitOfWork,
            repository,
            new FixedRuleVersionRepository(new RuleVersion
            {
                RuleId = 200,
                VersionNo = 2,
                VersionStatus = "DISABLED"
            }),
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleActionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(200, 2, new RuleActionSaveRequest
        {
            Actions = Array.Empty<RuleActionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.VersionStatusNotAllowed, ex.Code);
    }

    [Fact]
    public async Task SaveConditionsAsync_LocksVersionInsideTransactionAndRejectsWhenStatusChangedAfterPreCheck()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleConditionRepository(unitOfWork);
        var versionRepository = new FlippingRuleVersionRepository(
            firstReadStatus: "DRAFT",
            lockedReadStatus: "PUBLISHED");
        var service = new RuleConditionAppService(
            unitOfWork,
            repository,
            versionRepository,
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleConditionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(300, 3, new RuleConditionSaveRequest
        {
            Conditions = Array.Empty<RuleConditionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.VersionStatusNotAllowed, ex.Code);
        Assert.True(versionRepository.WasLocked);
        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(0, unitOfWork.CommitCount);
        Assert.Equal(1, unitOfWork.RollbackCount);
    }

    [Fact]
    public async Task SaveConditionsAsync_RejectsWhenPublishApprovalIsPending()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleConditionRepository(unitOfWork);
        var service = new RuleConditionAppService(
            unitOfWork,
            repository,
            new FixedRuleVersionRepository(new RuleVersion
            {
                RuleId = 500,
                VersionNo = 5,
                VersionStatus = "DRAFT"
            }),
            new PendingPublishRuleChangeLogRepository(500, 5),
            NullLogger<RuleConditionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(500, 5, new RuleConditionSaveRequest
        {
            Conditions = Array.Empty<RuleConditionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.ApprovalPendingEditNotAllowed, ex.Code);
    }

    [Fact]
    public async Task SaveActionsAsync_LocksVersionInsideTransactionAndRejectsWhenStatusChangedAfterPreCheck()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleActionRepository(unitOfWork);
        var versionRepository = new FlippingRuleVersionRepository(
            firstReadStatus: "DRAFT",
            lockedReadStatus: "DISABLED");
        var service = new RuleActionAppService(
            unitOfWork,
            repository,
            versionRepository,
            new EmptyRuleChangeLogRepository(),
            NullLogger<RuleActionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(400, 4, new RuleActionSaveRequest
        {
            Actions = Array.Empty<RuleActionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.VersionStatusNotAllowed, ex.Code);
        Assert.True(versionRepository.WasLocked);
        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(0, unitOfWork.CommitCount);
        Assert.Equal(1, unitOfWork.RollbackCount);
    }

    [Fact]
    public async Task SaveActionsAsync_RejectsWhenPublishApprovalIsPending()
    {
        var unitOfWork = new FakeUnitOfWork();
        var repository = new TransactionalRuleActionRepository(unitOfWork);
        var service = new RuleActionAppService(
            unitOfWork,
            repository,
            new FixedRuleVersionRepository(new RuleVersion
            {
                RuleId = 600,
                VersionNo = 6,
                VersionStatus = "DRAFT"
            }),
            new PendingPublishRuleChangeLogRepository(600, 6),
            NullLogger<RuleActionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.SaveAsync(600, 6, new RuleActionSaveRequest
        {
            Actions = Array.Empty<RuleActionItemRequest>()
        }));

        Assert.Equal(BizErrorCode.ApprovalPendingEditNotAllowed, ex.Code);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        private readonly List<ITransactionalParticipant> _participants = new();

        public int BeginCount { get; private set; }
        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }
        public bool IsInTransaction { get; private set; }

        public void Enlist(ITransactionalParticipant participant)
        {
            if (_participants.Contains(participant))
            {
                return;
            }

            _participants.Add(participant);
        }

        public Task BeginAsync()
        {
            BeginCount++;
            IsInTransaction = true;
            foreach (var participant in _participants)
            {
                participant.Begin();
            }

            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            CommitCount++;
            foreach (var participant in _participants)
            {
                participant.Commit();
            }

            IsInTransaction = false;
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            RollbackCount++;
            foreach (var participant in _participants)
            {
                participant.Rollback();
            }

            IsInTransaction = false;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private interface ITransactionalParticipant
    {
        void Begin();
        void Commit();
        void Rollback();
    }

    private sealed class TransactionalRuleConditionRepository : IRuleConditionRepository, ITransactionalParticipant
    {
        private readonly FakeUnitOfWork _unitOfWork;
        private readonly Dictionary<(long RuleId, int VersionNo), List<RuleCondition>> _store = new();
        private Dictionary<(long RuleId, int VersionNo), List<RuleCondition>>? _staged;

        public TransactionalRuleConditionRepository(FakeUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork.Enlist(this);
        }

        public bool ThrowOnInsert { get; set; }

        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            var source = _unitOfWork.IsInTransaction && _staged is not null ? _staged : _store;
            return Task.FromResult((IReadOnlyList<RuleCondition>)(source.TryGetValue((ruleId, versionNo), out var list)
                ? list.Select(Clone).ToList()
                : new List<RuleCondition>()));
        }

        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities)
        {
            if (ThrowOnInsert)
            {
                throw new InvalidOperationException("Simulated condition insert failure.");
            }

            if (entities.Count == 0)
            {
                return Task.CompletedTask;
            }

            var target = GetWritableStore();
            target[(entities[0].RuleId, entities[0].VersionNo)] = entities.Select(Clone).ToList();
            return Task.CompletedTask;
        }

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            GetWritableStore().Remove((ruleId, versionNo));
            return Task.CompletedTask;
        }

        public void Seed(long ruleId, int versionNo, params RuleCondition[] conditions)
        {
            _store[(ruleId, versionNo)] = conditions.Select(Clone).ToList();
        }

        public void Begin() => _staged = CloneStore(_store);

        public void Commit()
        {
            if (_staged is not null)
            {
                _store.Clear();
                foreach (var pair in _staged)
                {
                    _store[pair.Key] = pair.Value.Select(Clone).ToList();
                }
            }

            _staged = null;
        }

        public void Rollback() => _staged = null;

        private Dictionary<(long RuleId, int VersionNo), List<RuleCondition>> GetWritableStore()
        {
            return _unitOfWork.IsInTransaction && _staged is not null ? _staged : _store;
        }

        private static Dictionary<(long RuleId, int VersionNo), List<RuleCondition>> CloneStore(
            Dictionary<(long RuleId, int VersionNo), List<RuleCondition>> source)
        {
            return source.ToDictionary(pair => pair.Key, pair => pair.Value.Select(Clone).ToList());
        }

        private static RuleCondition Clone(RuleCondition source)
        {
            return new RuleCondition
            {
                ConditionId = source.ConditionId,
                RuleId = source.RuleId,
                VersionNo = source.VersionNo,
                ConditionGroup = source.ConditionGroup,
                ConditionType = source.ConditionType,
                OperatorType = source.OperatorType,
                LeftKey = source.LeftKey,
                RightValue = source.RightValue,
                ParamsJson = source.ParamsJson,
                SortNo = source.SortNo,
                IsEnabled = source.IsEnabled
            };
        }
    }

    private sealed class TransactionalRuleActionRepository : IRuleActionRepository, ITransactionalParticipant
    {
        private readonly FakeUnitOfWork _unitOfWork;
        private readonly Dictionary<(long RuleId, int VersionNo), List<RuleAction>> _store = new();
        private Dictionary<(long RuleId, int VersionNo), List<RuleAction>>? _staged;

        public TransactionalRuleActionRepository(FakeUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork.Enlist(this);
        }

        public bool ThrowOnInsert { get; set; }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            var source = _unitOfWork.IsInTransaction && _staged is not null ? _staged : _store;
            return Task.FromResult((IReadOnlyList<RuleAction>)(source.TryGetValue((ruleId, versionNo), out var list)
                ? list.Select(Clone).ToList()
                : new List<RuleAction>()));
        }

        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities)
        {
            if (ThrowOnInsert)
            {
                throw new InvalidOperationException("Simulated action insert failure.");
            }

            if (entities.Count == 0)
            {
                return Task.CompletedTask;
            }

            var target = GetWritableStore();
            target[(entities[0].RuleId, entities[0].VersionNo)] = entities.Select(Clone).ToList();
            return Task.CompletedTask;
        }

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            GetWritableStore().Remove((ruleId, versionNo));
            return Task.CompletedTask;
        }

        public void Seed(long ruleId, int versionNo, params RuleAction[] actions)
        {
            _store[(ruleId, versionNo)] = actions.Select(Clone).ToList();
        }

        public void Begin() => _staged = CloneStore(_store);

        public void Commit()
        {
            if (_staged is not null)
            {
                _store.Clear();
                foreach (var pair in _staged)
                {
                    _store[pair.Key] = pair.Value.Select(Clone).ToList();
                }
            }

            _staged = null;
        }

        public void Rollback() => _staged = null;

        private Dictionary<(long RuleId, int VersionNo), List<RuleAction>> GetWritableStore()
        {
            return _unitOfWork.IsInTransaction && _staged is not null ? _staged : _store;
        }

        private static Dictionary<(long RuleId, int VersionNo), List<RuleAction>> CloneStore(
            Dictionary<(long RuleId, int VersionNo), List<RuleAction>> source)
        {
            return source.ToDictionary(pair => pair.Key, pair => pair.Value.Select(Clone).ToList());
        }

        private static RuleAction Clone(RuleAction source)
        {
            return new RuleAction
            {
                ActionId = source.ActionId,
                RuleId = source.RuleId,
                VersionNo = source.VersionNo,
                ActionType = source.ActionType,
                ExecutorCode = source.ExecutorCode,
                ParamsJson = source.ParamsJson,
                ExclusiveGroup = source.ExclusiveGroup,
                SortNo = source.SortNo,
                OnError = source.OnError,
                IsEnabled = source.IsEnabled
            };
        }
    }

    private sealed class DraftRuleVersionRepository : IRuleVersionRepository
    {
        private readonly long _ruleId;
        private readonly int _versionNo;

        public DraftRuleVersionRepository(long ruleId, int versionNo)
        {
            _ruleId = ruleId;
            _versionNo = versionNo;
        }

        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult<RuleVersion?>(null);

        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            return Task.FromResult(ruleId == _ruleId && versionNo == _versionNo
                ? new RuleVersion
                {
                    RuleId = _ruleId,
                    VersionNo = _versionNo,
                    VersionStatus = "DRAFT"
                }
                : null);
        }

        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo) =>
            GetByRuleAndVersionAsync(ruleId, versionNo);

        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleVersion>)Array.Empty<RuleVersion>());

        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(0L);

        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null) =>
            Task.FromResult(true);
    }

    private sealed class FixedRuleVersionRepository : IRuleVersionRepository
    {
        private readonly RuleVersion? _version;

        public FixedRuleVersionRepository(RuleVersion? version)
        {
            _version = version;
        }

        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult(_version);

        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            return Task.FromResult(_version is not null && _version.RuleId == ruleId && _version.VersionNo == versionNo
                ? _version
                : null);
        }

        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo) =>
            GetByRuleAndVersionAsync(ruleId, versionNo);

        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleVersion>)(_version is not null && _version.RuleId == ruleId
                ? new[] { _version }
                : Array.Empty<RuleVersion>()));

        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(0L);

        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null) =>
            Task.FromResult(true);
    }

    private sealed class FlippingRuleVersionRepository : IRuleVersionRepository
    {
        private readonly string _firstReadStatus;
        private readonly string _lockedReadStatus;

        public FlippingRuleVersionRepository(string firstReadStatus, string lockedReadStatus)
        {
            _firstReadStatus = firstReadStatus;
            _lockedReadStatus = lockedReadStatus;
        }

        public bool WasLocked { get; private set; }

        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult<RuleVersion?>(null);

        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo)
        {
            return Task.FromResult<RuleVersion?>(new RuleVersion
            {
                RuleId = ruleId,
                VersionNo = versionNo,
                VersionStatus = _firstReadStatus
            });
        }

        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo)
        {
            WasLocked = true;
            return Task.FromResult<RuleVersion?>(new RuleVersion
            {
                RuleId = ruleId,
                VersionNo = versionNo,
                VersionStatus = _lockedReadStatus
            });
        }

        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleVersion>)Array.Empty<RuleVersion>());

        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(0L);

        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null) =>
            Task.FromResult(true);
    }

    private sealed class PendingPublishRuleChangeLogRepository : IRuleChangeLogRepository
    {
        private readonly long _ruleId;
        private readonly int _versionNo;

        public PendingPublishRuleChangeLogRepository(long ruleId, int versionNo)
        {
            _ruleId = ruleId;
            _versionNo = versionNo;
        }

        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId)
        {
            if (ruleId != _ruleId)
            {
                return Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
            }

            return Task.FromResult((IReadOnlyList<RuleChangeLog>)new[]
            {
                new RuleChangeLog
                {
                    RuleId = _ruleId,
                    VersionNo = _versionNo,
                    ChangeType = "SUBMIT_APPROVAL",
                    ChangeSummary = "提交PUBLISH审批",
                    ChangedAt = new DateTime(2026, 5, 22, 9, 0, 0)
                }
            });
        }

        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());

        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }
}
