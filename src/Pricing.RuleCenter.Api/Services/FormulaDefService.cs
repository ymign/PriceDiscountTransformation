using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class FormulaDefService
{
    private readonly IFormulaDefRepository _repository;
    private readonly ILogger<FormulaDefService> _logger;

    public FormulaDefService(IFormulaDefRepository repository, ILogger<FormulaDefService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FormulaDefResponse>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToResponse).ToList();
    }

    public async Task<FormulaDefResponse?> GetByIdAsync(long formulaId)
    {
        var entity = await _repository.GetByIdAsync(formulaId);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<long> CreateAsync(FormulaDefCreateRequest request)
    {
        var existing = await _repository.GetByCodeAsync(request.FormulaCode);
        if (existing is not null)
        {
            throw new InvalidOperationException($"公式编码已存在: {request.FormulaCode}");
        }

        var entity = new FormulaDef
        {
            FormulaCode = request.FormulaCode,
            FormulaName = request.FormulaName,
            FormulaDesc = request.FormulaDesc,
            ExecutorCode = request.ExecutorCode,
            ParamSchemaJson = request.ParamSchemaJson,
            IsEnabled = "Y",
            Remark = request.Remark
        };

        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增公式定义 {FormulaCode}, ID={FormulaId}",
            request.FormulaCode, id);
        return id;
    }

    public async Task UpdateAsync(long formulaId, FormulaDefUpdateRequest request)
    {
        var entity = await _repository.GetByIdAsync(formulaId)
            ?? throw new KeyNotFoundException($"公式定义不存在: {formulaId}");

        entity.FormulaName = request.FormulaName;
        entity.FormulaDesc = request.FormulaDesc;
        entity.ExecutorCode = request.ExecutorCode;
        entity.ParamSchemaJson = request.ParamSchemaJson;
        entity.Remark = request.Remark;

        await _repository.UpdateAsync(entity);
    }

    public async Task ToggleAsync(long formulaId)
    {
        var entity = await _repository.GetByIdAsync(formulaId)
            ?? throw new KeyNotFoundException($"公式定义不存在: {formulaId}");

        var newEnabled = entity.IsEnabled == "Y" ? "N" : "Y";
        await _repository.SetEnabledAsync(formulaId, newEnabled);

        _logger.LogInformation("切换公式 {FormulaId} 状态为 {Enabled}", formulaId, newEnabled);
    }

    private static FormulaDefResponse MapToResponse(FormulaDef entity)
    {
        return new FormulaDefResponse
        {
            FormulaId = entity.FormulaId,
            FormulaCode = entity.FormulaCode,
            FormulaName = entity.FormulaName,
            FormulaDesc = entity.FormulaDesc,
            ExecutorCode = entity.ExecutorCode,
            ParamSchemaJson = entity.ParamSchemaJson,
            IsEnabled = entity.IsEnabled,
            Remark = entity.Remark
        };
    }
}
