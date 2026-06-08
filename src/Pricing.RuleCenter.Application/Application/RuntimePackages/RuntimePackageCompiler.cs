using System.Security.Cryptography;
using System.Text;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 运行包编译器，负责把已通过校验的策略版本编译为运行期规则快照。
/// </summary>
/// <remarks>
/// <para>
/// 新规则平台的维护模型是“策略 + 模板 + 参数”，但计价引擎运行时需要的是稳定、扁平、可快速查询的
/// 规则、条件、动作快照。本编译器负责完成这层投影，并生成可激活的 runtime package。
/// </para>
/// <para>
/// 编译阶段只构建包和运行期规则，不直接替换当前激活包；真正的激活由
/// <see cref="RuntimePackageActivationService"/> 完成。这样可以支持发布前 diff、失败回滚和审计。
/// </para>
/// </remarks>
public sealed class RuntimePackageCompiler
{
    /// <summary>
    /// 策略仓储，用于读取策略主表、版本、绑定、作用域和参数。
    /// </summary>
    private readonly IPolicyRepository _policyRepository;

    /// <summary>
    /// 模板仓储，用于读取模板版本、参数定义、步骤定义和作用域定义。
    /// </summary>
    private readonly ITemplateRepository _templateRepository;

    /// <summary>
    /// 运行包仓储，用于写入包头状态和包级元数据。
    /// </summary>
    private readonly IRuntimePackageRepository _runtimePackageRepository;

    /// <summary>
    /// 运行期规则构建仓储，用于批量写入包策略、规则、条件和动作。
    /// </summary>
    private readonly IRuntimeRuleBuildRepository _runtimeRuleBuildRepository;

    /// <summary>
    /// 策略编译校验服务，负责检查绑定、作用域、参数和动作类型是否满足模板约束。
    /// </summary>
    private readonly PolicyValidationService _validationService;

    /// <summary>
    /// 策略冲突校验服务，负责阻断同项目同场景下不可并存的规则组合。
    /// </summary>
    private readonly PolicyConflictService _conflictService;

    /// <summary>
    /// 运行期规则投影工厂，把策略版本投影成引擎可执行的 RuntimeRuleSnapshot。
    /// </summary>
    private readonly RuntimeRuleProjectionFactory _projectionFactory;

    /// <summary>
    /// 统一时钟，用于生成包版本和构建时间。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// 初始化运行包编译器。
    /// </summary>
    /// <param name="policyRepository">策略仓储。</param>
    /// <param name="templateRepository">模板仓储。</param>
    /// <param name="runtimePackageRepository">运行包仓储。</param>
    /// <param name="runtimeRuleBuildRepository">运行期规则构建仓储。</param>
    /// <param name="validationService">策略编译校验服务。</param>
    /// <param name="conflictService">策略冲突校验服务。</param>
    /// <param name="projectionFactory">运行期规则投影工厂。</param>
    /// <param name="clock">统一时钟。</param>
    public RuntimePackageCompiler(
        IPolicyRepository policyRepository,
        ITemplateRepository templateRepository,
        IRuntimePackageRepository runtimePackageRepository,
        IRuntimeRuleBuildRepository runtimeRuleBuildRepository,
        PolicyValidationService validationService,
        PolicyConflictService conflictService,
        RuntimeRuleProjectionFactory projectionFactory,
        IClock clock)
    {
        _policyRepository = policyRepository;
        _templateRepository = templateRepository;
        _runtimePackageRepository = runtimePackageRepository;
        _runtimeRuleBuildRepository = runtimeRuleBuildRepository;
        _validationService = validationService;
        _conflictService = conflictService;
        _projectionFactory = projectionFactory;
        _clock = clock;
    }

    /// <summary>
    /// 编译运行包。
    /// </summary>
    /// <param name="context">运行包构建上下文，包含候选策略版本、构建人和构建时间。</param>
    /// <returns>运行包构建结果，包含包头、包策略、运行期规则、条件和动作。</returns>
    public async Task<RuntimePackageBuildResult> CompileAsync(RuntimePackageBuildContext context)
    {
        // ========== 第一阶段：加载候选策略版本 ==========
        // 显式传入版本时只编译指定版本；未传入时编译所有 PUBLISH_READY 版本。
        var buildAt = context.BuildAt ?? _clock.Now;
        var candidateVersions = await LoadCandidateVersionsAsync(context.PolicyVersionIds);
        var packagePolicies = new List<RuntimePackagePolicy>();
        var projections = new List<RuntimeRuleSnapshot>();
        var checksumBuilder = new StringBuilder();

        foreach (var version in candidateVersions)
        {
            // ========== 第二阶段：读取策略完整定义并校验 ==========
            // 编译时必须同时校验模板约束和策略参数，避免把不可执行的配置写入运行包。
            var policy = await _policyRepository.GetByIdAsync(version.PolicyId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在，PolicyId={version.PolicyId}");
            var templateVersion = await _templateRepository.GetVersionAsync(version.TemplateVersionId)
                ?? throw new BizException(BizErrorCode.TemplateVersionNotFound, 404, $"模板版本不存在，TemplateVersionId={version.TemplateVersionId}");
            var bindings = await _policyRepository.GetBindingsAsync(version.PolicyVersionId);
            var scopes = await _policyRepository.GetScopesAsync(version.PolicyVersionId);
            var parameters = await _policyRepository.GetParamsAsync(version.PolicyVersionId);
            var paramDefs = await _templateRepository.GetParamDefsAsync(version.TemplateVersionId);
            var stepDefs = await _templateRepository.GetStepDefsAsync(version.TemplateVersionId);
            var scopeDefs = await _templateRepository.GetScopeDefsAsync(version.TemplateVersionId);

            _validationService.ValidateForCompile(
                policy,
                version,
                templateVersion,
                paramDefs,
                stepDefs,
                scopeDefs,
                bindings,
                scopes,
                parameters,
                context.RequirePublishReadyStatus);

            packagePolicies.Add(new RuntimePackagePolicy
            {
                PolicyVersionId = version.PolicyVersionId,
                PolicyCode = policy.PolicyCode,
                TemplateVersionId = templateVersion.TemplateVersionId,
                CapabilityFamily = templateVersion.CapabilityFamily
            });

            // ========== 第三阶段：投影为运行期规则 ==========
            // 投影后的结构是计价引擎的运行时事实，不再依赖策略表和模板表的多表 join。
            projections.AddRange(_projectionFactory.Create(version, templateVersion, bindings, scopes, parameters, stepDefs));
            checksumBuilder
                .Append(policy.PolicyCode).Append('|')
                .Append(version.PolicyVersionId).Append('|')
                .Append(version.Checksum).Append('|')
                .Append(templateVersion.TemplateVersionId).Append('|')
                .Append(templateVersion.Checksum).AppendLine();
        }

        // ========== 第四阶段：发布前冲突校验 ==========
        // 这里拦截同一项目的互斥公式、额度规则、金额上下限等不可并存配置。
        _conflictService.EnsureNoConflicts(projections);

        // ========== 第五阶段：写入 BUILDING 包头 ==========
        // 先落 BUILDING 状态，便于后续定位失败构建；只有规则、条件、动作全部写入后才更新为 BUILT。
        var package = new RuntimePackage
        {
            PackageVersion = buildAt.Ticks,
            PackageStatus = RuntimePackageStatusCodes.Building,
            BuildScope = string.IsNullOrWhiteSpace(context.BuildScope) ? RuntimeBuildScopeCodes.Full : context.BuildScope,
            BuiltBy = context.BuiltBy,
            SourceChecksum = BuildChecksum(checksumBuilder.ToString())
        };

        package.PackageId = await _runtimePackageRepository.InsertAsync(package);
        await AssignIdentifiersAsync(package, packagePolicies, projections);

        // ========== 第六阶段：批量写入运行期快照 ==========
        // 包策略、规则、条件、动作共同构成一个不可变运行包；激活后计价只读取这些快照表。
        await _runtimeRuleBuildRepository.InsertPackagePoliciesAsync(packagePolicies);
        await _runtimeRuleBuildRepository.InsertRulesAsync(projections.Select(snapshot => snapshot.Rule).ToList());
        await _runtimeRuleBuildRepository.InsertConditionsAsync(projections.SelectMany(snapshot => snapshot.Conditions).ToList());
        await _runtimeRuleBuildRepository.InsertActionsAsync(projections.SelectMany(snapshot => snapshot.Actions).ToList());

        // ========== 第七阶段：标记构建完成 ==========
        // BUILT 只表示包可被激活，不代表已经对计价生效。
        package.PackageStatus = RuntimePackageStatusCodes.Built;
        package.BuiltAt = buildAt;
        await _runtimePackageRepository.UpdateAsync(package);

        return new RuntimePackageBuildResult
        {
            Package = package,
            PackagePolicies = packagePolicies,
            Rules = projections.Select(snapshot => snapshot.Rule).ToList(),
            Conditions = projections.SelectMany(snapshot => snapshot.Conditions).ToList(),
            Actions = projections.SelectMany(snapshot => snapshot.Actions).ToList()
        };
    }

    /// <summary>
    /// 加载候选策略版本。
    /// </summary>
    /// <param name="policyVersionIds">显式指定的策略版本 ID 集合；为空时加载所有待发布版本。</param>
    /// <returns>候选策略版本集合。</returns>
    private async Task<IReadOnlyList<PolicyVersion>> LoadCandidateVersionsAsync(IReadOnlyCollection<long>? policyVersionIds)
    {
        if (policyVersionIds is null || policyVersionIds.Count == 0)
        {
            return await _policyRepository.GetPublishReadyVersionsAsync();
        }

        var requested = policyVersionIds.ToHashSet();
        var filtered = new List<PolicyVersion>(requested.Count);
        foreach (var policyVersionId in requested)
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId);
            if (version is not null)
            {
                filtered.Add(version);
            }
        }

        if (filtered.Count != requested.Count)
        {
            throw new BizException(
                BizErrorCode.PolicyNotFound,
                404,
                "存在不存在的策略版本，不能参与候选包构建。");
        }

        return filtered;
    }

    /// <summary>
    /// 为运行包策略、规则、条件和动作分配数据库主键并回填外键。
    /// </summary>
    /// <param name="package">运行包头。</param>
    /// <param name="packagePolicies">包内策略集合。</param>
    /// <param name="projections">运行期规则投影集合。</param>
    private async Task AssignIdentifiersAsync(
        RuntimePackage package,
        IReadOnlyList<RuntimePackagePolicy> packagePolicies,
        IReadOnlyList<RuntimeRuleSnapshot> projections)
    {
        // 主键由仓储一次性预留，减少逐条 NEXTVAL 调用导致的数据库往返。
        var packagePolicyIds = await _runtimeRuleBuildRepository.ReservePackagePolicyIdsAsync(packagePolicies.Count);
        for (var i = 0; i < packagePolicies.Count; i++)
        {
            packagePolicies[i].PackagePolicyId = packagePolicyIds[i];
            packagePolicies[i].PackageId = package.PackageId;
            packagePolicies[i].PriorityKey = projections
                .Where(snapshot => snapshot.Rule.SourcePolicyVersionId == packagePolicies[i].PolicyVersionId)
                .Select(snapshot => snapshot.Rule.PriorityKey)
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault() ?? string.Empty;
        }

        var ruleIds = await _runtimeRuleBuildRepository.ReserveRuleIdsAsync(projections.Count);
        for (var i = 0; i < projections.Count; i++)
        {
            projections[i].Rule.RuntimeRuleId = ruleIds[i];
            projections[i].Rule.PackageId = package.PackageId;
        }

        var allConditions = projections.SelectMany(snapshot => snapshot.Conditions).ToList();
        var conditionIds = await _runtimeRuleBuildRepository.ReserveConditionIdsAsync(allConditions.Count);
        for (var i = 0; i < allConditions.Count; i++)
        {
            allConditions[i].RuntimeConditionId = conditionIds[i];
        }

        var allActions = projections.SelectMany(snapshot => snapshot.Actions).ToList();
        var actionIds = await _runtimeRuleBuildRepository.ReserveActionIdsAsync(allActions.Count);
        for (var i = 0; i < allActions.Count; i++)
        {
            allActions[i].RuntimeActionId = actionIds[i];
        }

        foreach (var snapshot in projections)
        {
            // 条件和动作必须回填运行期规则 ID，否则激活包读取时无法按规则聚合完整执行链。
            foreach (var condition in snapshot.Conditions)
            {
                condition.RuntimeRuleId = snapshot.Rule.RuntimeRuleId;
            }

            foreach (var action in snapshot.Actions)
            {
                action.RuntimeRuleId = snapshot.Rule.RuntimeRuleId;
            }
        }
    }

    /// <summary>
    /// 根据参与构建的策略和模板校验和生成运行包来源校验和。
    /// </summary>
    /// <param name="source">参与构建的策略/模板摘要文本。</param>
    /// <returns>SHA-256 十六进制校验和。</returns>
    private static string BuildChecksum(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes);
    }
}
