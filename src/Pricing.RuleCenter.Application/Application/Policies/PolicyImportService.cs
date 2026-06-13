using System.Security.Cryptography;
using System.Text;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 历史规则导入应用服务，负责把旧规则转换为策略平台草稿版本。
/// </summary>
public sealed class PolicyImportService
{
    private const string LegacyActionParamsJsonParamCode = "LEGACY_ACTION_PARAMS_JSON";

    private readonly IRuleHeaderRepository _ruleHeaderRepository;
    private readonly IRuleConditionRepository _ruleConditionRepository;
    private readonly IRuleActionRepository _ruleActionRepository;
    private readonly IPolicyRepository _policyRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化历史规则导入应用服务。
    /// </summary>
    public PolicyImportService(
        IRuleHeaderRepository ruleHeaderRepository,
        IRuleConditionRepository ruleConditionRepository,
        IRuleActionRepository ruleActionRepository,
        IPolicyRepository policyRepository,
        ITemplateRepository templateRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _ruleHeaderRepository = ruleHeaderRepository;
        _ruleConditionRepository = ruleConditionRepository;
        _ruleActionRepository = ruleActionRepository;
        _policyRepository = policyRepository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    /// <summary>
    /// 批量导入旧规则到策略平台。
    /// </summary>
    public async Task<IReadOnlyList<long>> ImportAsync(IReadOnlyCollection<long> ruleIds, string importedBy)
    {
        var importedVersionIds = new List<long>();
        foreach (var ruleId in ruleIds.Distinct())
        {
            var header = await _ruleHeaderRepository.GetByIdAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"旧规则不存在: {ruleId}");
            if (header.CurrentVersion <= 0)
            {
                continue;
            }

            var conditions = await _ruleConditionRepository.GetByRuleAndVersionAsync(ruleId, header.CurrentVersion);
            var actions = await _ruleActionRepository.GetByRuleAndVersionAsync(ruleId, header.CurrentVersion);
            var unsupportedConditions = GetUnsupportedEnabledConditions(conditions);

            foreach (var action in actions.Where(item => item.IsEnabled == EnableFlag.Yes))
            {
                var templateCode = ResolveTemplateCode(action);
                if (templateCode is null)
                {
                    continue;
                }

                EnsureNoUnsupportedConditions(header, unsupportedConditions);

                var template = await _templateRepository.GetByCodeAsync(templateCode)
                    ?? throw new BizException(BizErrorCode.TemplateNotFound, 404, $"导入模板不存在: {templateCode}");
                var templateVersion = (await _templateRepository.GetVersionsByTemplateIdAsync(template.TemplateId))
                    .OrderByDescending(item => item.VersionNo)
                    .FirstOrDefault()
                    ?? throw new BizException(BizErrorCode.TemplateVersionNotFound, 404, $"导入模板缺少版本: {templateCode}");

                var policyCode = BuildImportedPolicyCode(header.RuleCode, action);
                var existingPolicy = await _policyRepository.GetByCodeAsync(policyCode);
                if (existingPolicy is not null)
                {
                    continue;
                }

                var now = _clock.Now;
                var policy = new PolicyAggregate
                {
                    PolicyCode = policyCode,
                    PolicyName = BuildImportedPolicyName(header.RuleName, action),
                    TemplateId = template.TemplateId,
                    OwnerType = "MIGRATION",
                    PublishProfile = PolicyPublishProfileCodes.ReviewRequired,
                    Status = PolicyLifecycleCodes.Draft,
                    CurrentVersionNo = 1,
                    CreatedBy = importedBy.Trim(),
                    CreatedAt = now,
                    UpdatedBy = importedBy.Trim(),
                    UpdatedAt = now
                };

                await _unitOfWork.BeginAsync();
                try
                {
                    policy.PolicyId = await _policyRepository.InsertAsync(policy);
                    var policyVersion = new PolicyVersion
                    {
                        PolicyId = policy.PolicyId,
                        TemplateVersionId = templateVersion.TemplateVersionId,
                        VersionNo = 1,
                        PolicyStatus = PolicyLifecycleCodes.Draft,
                        EffectiveFrom = header.EffectiveFrom,
                        EffectiveTo = header.EffectiveTo,
                        BindingType = ResolveBindingType(header),
                        ScopeLevel = ResolveScopeLevel(conditions),
                        PriorityWeight = header.Priority,
                        Checksum = BuildChecksum(header, action, conditions)
                    };
                    policyVersion.PolicyVersionId = await _policyRepository.InsertVersionAsync(policyVersion);
                    await _policyRepository.ReplaceBindingsAsync(policyVersion.PolicyVersionId, BuildBindings(header, policyVersion));
                    await _policyRepository.ReplaceScopesAsync(policyVersion.PolicyVersionId, BuildScopes(conditions, policyVersion));
                    await _policyRepository.ReplaceParamsAsync(policyVersion.PolicyVersionId, BuildParams(action, policyVersion));
                    await _unitOfWork.CommitAsync();
                    importedVersionIds.Add(policyVersion.PolicyVersionId);
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
        }

        return importedVersionIds;
    }

    private static string? ResolveTemplateCode(RuleAction action)
    {
        if (string.Equals(action.ActionType, RuleActionTypeCodes.FormulaCalc, StringComparison.OrdinalIgnoreCase))
        {
            if (FormulaExecutorCodes.IsAreaStepIncrement(action.ExecutorCode))
            {
                return "TPL_AREA_STEP_INCREMENT";
            }

            if (FormulaExecutorCodes.IsIncrementPercent(action.ExecutorCode))
            {
                return "TPL_INCREMENT_PERCENT";
            }
        }

        return action.ActionType?.Trim().ToUpperInvariant() switch
        {
            RuleActionTypeCodes.ApplyTimeWindowLimit => "TPL_TIME_WINDOW_QTY_LIMIT",
            RuleActionTypeCodes.ApplyDayLimitQty => "TPL_DAILY_QTY_LIMIT",
            RuleActionTypeCodes.ApplyOnceLimitQty => "TPL_ONCE_QTY_LIMIT",
            RuleActionTypeCodes.ConvertQty => "TPL_UNIT_CONVERT_BY_BODY_PART",
            _ => null
        };
    }

    private static string BuildImportedPolicyCode(string ruleCode, RuleAction action)
    {
        var suffix = string.Equals(action.ActionType, RuleActionTypeCodes.FormulaCalc, StringComparison.OrdinalIgnoreCase)
            ? action.ExecutorCode
            : action.ActionType;
        return $"IMP_{ruleCode}_{suffix}".ToUpperInvariant();
    }

    private static string BuildImportedPolicyName(string ruleName, RuleAction action)
    {
        var suffix = string.Equals(action.ActionType, RuleActionTypeCodes.FormulaCalc, StringComparison.OrdinalIgnoreCase)
            ? action.ExecutorCode
            : action.ActionType;
        return $"{ruleName}-{suffix}-导入草稿";
    }

    private static string ResolveBindingType(RuleAggregate header)
    {
        return string.IsNullOrWhiteSpace(header.ItemCode)
            ? string.IsNullOrWhiteSpace(header.GroupCode) ? "GLOBAL" : "GROUP"
            : "ITEM";
    }

    private static string ResolveScopeLevel(IReadOnlyList<RuleCondition> conditions)
    {
        return conditions.Any(condition => RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.ChargeScene)
            .Contains(condition.ConditionType, StringComparer.OrdinalIgnoreCase))
            ? "SCENE"
            : "HOSPITAL";
    }

    private static IReadOnlyList<PolicyBinding> BuildBindings(RuleAggregate header, PolicyVersion version)
    {
        return new[]
        {
            new PolicyBinding
            {
                PolicyVersionId = version.PolicyVersionId,
                BindingType = ResolveBindingType(header),
                ItemCode = header.ItemCode,
                ItemName = header.ItemName,
                GroupCode = header.GroupCode
            }
        };
    }

    private static IReadOnlyList<PolicyScope> BuildScopes(IReadOnlyList<RuleCondition> conditions, PolicyVersion version)
    {
        var scopes = new List<PolicyScope>();
        foreach (var condition in conditions.Where(item => item.IsEnabled == EnableFlag.Yes))
        {
            var scopeDimension = ResolveScopeDimension(condition.ConditionType);
            if (scopeDimension is null)
            {
                continue;
            }

            scopes.Add(new PolicyScope
            {
                PolicyVersionId = version.PolicyVersionId,
                ScopeDimension = scopeDimension,
                ScopeOperator = string.IsNullOrWhiteSpace(condition.OperatorType) ? "EQ" : condition.OperatorType,
                ScopeValueText = condition.RightValue,
                ScopeJson = condition.ParamsJson
            });
        }

        return scopes;
    }

    private static string? ResolveScopeDimension(string? conditionType)
    {
        if (string.IsNullOrWhiteSpace(conditionType))
        {
            return null;
        }

        if (RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.ChargeScene).Contains(conditionType, StringComparer.OrdinalIgnoreCase))
        {
            return "SCENE";
        }

        if (RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.BodyPart).Contains(conditionType, StringComparer.OrdinalIgnoreCase))
        {
            return "BODY_PART";
        }

        return conditionType.Trim().ToUpperInvariant() switch
        {
            RuleConditionTypeCodes.VisitTypeMatch => "VISIT_TYPE",
            RuleConditionTypeCodes.TimeRange => "TIME_RANGE",
            _ => null
        };
    }

    private static IReadOnlyList<RuleCondition> GetUnsupportedEnabledConditions(IReadOnlyList<RuleCondition> conditions)
    {
        return conditions
            .Where(condition => condition.IsEnabled == EnableFlag.Yes)
            .Where(condition => !IsRepresentedByBinding(condition.ConditionType))
            .Where(condition => ResolveScopeDimension(condition.ConditionType) is null)
            .ToList();
    }

    private static bool IsRepresentedByBinding(string? conditionType)
    {
        if (string.IsNullOrWhiteSpace(conditionType))
        {
            return false;
        }

        return RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.ItemMatch)
                   .Contains(conditionType, StringComparer.OrdinalIgnoreCase) ||
               string.Equals(conditionType, RuleConditionTypeCodes.GroupMatch, StringComparison.OrdinalIgnoreCase);
    }

    private static void EnsureNoUnsupportedConditions(
        RuleAggregate header,
        IReadOnlyList<RuleCondition> unsupportedConditions)
    {
        if (unsupportedConditions.Count == 0)
        {
            return;
        }

        var unsupportedSummary = string.Join(
            ", ",
            unsupportedConditions
                .Select(condition => string.IsNullOrWhiteSpace(condition.ConditionType)
                    ? "<EMPTY>"
                    : condition.ConditionType.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase));
        throw new BizException(
            BizErrorCode.PolicyScopeUnsupported,
            400,
            $"旧规则 {header.RuleCode} 包含当前策略平台暂不支持的条件类型: {unsupportedSummary}，已阻断导入以避免条件静默丢失。");
    }

    private static IReadOnlyList<PolicyParam> BuildParams(RuleAction action, PolicyVersion version)
    {
        var items = new List<PolicyParam>();
        if (!string.IsNullOrWhiteSpace(action.ParamsJson))
        {
            items.Add(new PolicyParam
            {
                PolicyVersionId = version.PolicyVersionId,
                ParamCode = LegacyActionParamsJsonParamCode,
                ValueType = "TEXT",
                ValueText = action.ParamsJson
            });
        }

        return items;
    }

    private static string BuildChecksum(RuleAggregate header, RuleAction action, IReadOnlyList<RuleCondition> conditions)
    {
        var payload = new StringBuilder()
            .Append(header.RuleCode).Append('|')
            .Append(header.CurrentVersion).Append('|')
            .Append(action.ActionType).Append('|')
            .Append(action.ExecutorCode).Append('|')
            .Append(action.ParamsJson).Append('|');
        foreach (var condition in conditions.OrderBy(item => item.SortNo))
        {
            payload.Append(condition.ConditionType).Append(':')
                .Append(condition.OperatorType).Append(':')
                .Append(condition.RightValue).Append('|');
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload.ToString())));
    }
}
