using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class DictService
{
    private readonly IDictRepository _repository;
    private readonly ILogger<DictService> _logger;

    public DictService(IDictRepository repository, ILogger<DictService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DictResponse>> GetByTypeAsync(string dictType)
    {
        var items = await _repository.GetByTypeAsync(dictType);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<DictResponse?> GetByIdAsync(long dictId)
    {
        var entity = await _repository.GetByIdAsync(dictId);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<IReadOnlyList<string>> GetAllTypesAsync()
    {
        return await _repository.GetAllTypesAsync();
    }

    public async Task<long> CreateAsync(DictCreateRequest request)
    {
        if (await _repository.ExistsAsync(request.DictType, request.DictCode))
        {
            throw new InvalidOperationException(
                $"字典项已存在: {request.DictType}/{request.DictCode}");
        }

        var entity = new Dict
        {
            DictType = request.DictType,
            DictCode = request.DictCode,
            DictName = request.DictName,
            ParentCode = request.ParentCode,
            SortNo = request.SortNo,
            IsEnabled = "Y",
            Remark = request.Remark
        };

        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增字典项 {DictType}/{DictCode}, ID={DictId}",
            request.DictType, request.DictCode, id);
        return id;
    }

    public async Task UpdateAsync(long dictId, DictUpdateRequest request)
    {
        var entity = await _repository.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");

        entity.DictName = request.DictName;
        entity.ParentCode = request.ParentCode;
        entity.SortNo = request.SortNo;
        entity.Remark = request.Remark;

        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(long dictId)
    {
        var exists = await _repository.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");

        await _repository.SetEnabledAsync(dictId, "N");
        _logger.LogInformation("停用字典项 {DictId}", dictId);
    }

    private static DictResponse MapToResponse(Dict entity)
    {
        return new DictResponse
        {
            DictId = entity.DictId,
            DictType = entity.DictType,
            DictCode = entity.DictCode,
            DictName = entity.DictName,
            ParentCode = entity.ParentCode,
            SortNo = entity.SortNo,
            IsEnabled = entity.IsEnabled,
            Remark = entity.Remark
        };
    }
}
