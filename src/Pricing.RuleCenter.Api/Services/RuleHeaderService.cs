using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class RuleHeaderService
{
    private readonly IRuleHeaderRepository _repository;
    private readonly ILogger<RuleHeaderService> _logger;

    public RuleHeaderService(IRuleHeaderRepository repository, ILogger<RuleHeaderService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<RuleHeaderResponse>> GetPagedAsync(RuleHeaderPagedRequest request)
    {
        var (items, total) = await _repository.GetPagedAsync(
            request.ItemCode, request.Status, request.Category,
            request.PageIndex, request.PageSize);

        return new PagedResponse<RuleHeaderResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            Total = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<RuleHeaderResponse?> GetByIdAsync(long ruleId)
    {
        var entity = await _repository.GetByIdAsync(ruleId);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<IReadOnlyList<RuleHeaderResponse>> GetByItemCodeAsync(string itemCode)
    {
        var items = await _repository.GetByItemCodeAsync(itemCode);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<long> CreateAsync(RuleHeaderCreateRequest request)
    {
        if (await _repository.ExistsAsync(request.RuleCode))
        {
            throw new InvalidOperationException($"规则编码已存在: {request.RuleCode}");
        }

        var now = DateTime.Now;
        var entity = new RuleHeader
        {
            RuleCode = request.RuleCode,
            RuleName = request.RuleName,
            RuleCategory = request.RuleCategory,
            RuleScope = request.RuleScope,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            GroupCode = request.GroupCode,
            Priority = request.Priority,
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            Remark = request.Remark,
            CreatedBy = request.CreatedBy,
            CreatedAt = now,
            UpdatedAt = now
        };

        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增规则 {RuleCode}, ID={RuleId}", request.RuleCode, id);
        return id;
    }

    public async Task UpdateAsync(long ruleId, RuleHeaderUpdateRequest request)
    {
        var entity = await _repository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        entity.RuleName = request.RuleName;
        entity.RuleCategory = request.RuleCategory;
        entity.RuleScope = request.RuleScope;
        entity.ItemCode = request.ItemCode;
        entity.ItemName = request.ItemName;
        entity.GroupCode = request.GroupCode;
        entity.Priority = request.Priority;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.Remark = request.Remark;
        entity.UpdatedBy = request.UpdatedBy;
        entity.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(entity);
    }

    private static RuleHeaderResponse MapToResponse(RuleHeader entity)
    {
        return new RuleHeaderResponse
        {
            RuleId = entity.RuleId,
            RuleCode = entity.RuleCode,
            RuleName = entity.RuleName,
            RuleCategory = entity.RuleCategory,
            RuleScope = entity.RuleScope,
            ItemCode = entity.ItemCode,
            ItemName = entity.ItemName,
            GroupCode = entity.GroupCode,
            Priority = entity.Priority,
            CurrentVersion = entity.CurrentVersion,
            Status = entity.Status,
            IsEnabled = entity.IsEnabled,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            Remark = entity.Remark,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
