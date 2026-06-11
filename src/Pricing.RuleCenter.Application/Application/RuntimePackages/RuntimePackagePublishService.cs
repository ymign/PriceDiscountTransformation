using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Interfaces.Policies;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 运行包发布服务，负责校验策略版本、编译运行包并立即激活。
/// </summary>
/// <remarks>
/// <para>
/// 发布入口面向管理端操作。它会先检查每个策略版本是否满足发布条件，再调用
/// <see cref="RuntimePackageCompiler"/> 生成运行包，最后通过 <see cref="RuntimePackageActivationService"/>
/// 切换当前激活包。
/// </para>
/// <para>
/// 本服务不直接修改运行期规则内容，规则快照由编译器统一生成；激活过程由激活服务统一处理，
/// 便于保持发布、回滚和缓存失效逻辑一致。
/// </para>
/// </remarks>
public sealed class RuntimePackagePublishService
{
    /// <summary>
    /// 策略仓储，用于读取候选版本和回写最近构建包 ID。
    /// </summary>
    private readonly IPolicyRepository _policyRepository;

    /// <summary>
    /// 发布资格校验服务，负责检查审批、状态和 checksum 是否满足发布要求。
    /// </summary>
    private readonly IPolicyPublishEligibilityService _eligibilityService;

    /// <summary>
    /// 运行包编译器，负责把策略版本投影成运行期规则快照。
    /// </summary>
    private readonly RuntimePackageCompiler _compiler;

    /// <summary>
    /// 运行包激活服务，负责切换当前激活包并失效规则缓存。
    /// </summary>
    private readonly RuntimePackageActivationService _activationService;

    /// <summary>
    /// 初始化运行包发布服务。
    /// </summary>
    /// <param name="policyRepository">策略仓储。</param>
    /// <param name="eligibilityService">发布资格校验服务。</param>
    /// <param name="compiler">运行包编译器。</param>
    /// <param name="activationService">运行包激活服务。</param>
    public RuntimePackagePublishService(
        IPolicyRepository policyRepository,
        IPolicyPublishEligibilityService eligibilityService,
        RuntimePackageCompiler compiler,
        RuntimePackageActivationService activationService)
    {
        _policyRepository = policyRepository;
        _eligibilityService = eligibilityService;
        _compiler = compiler;
        _activationService = activationService;
    }

    /// <summary>
    /// 发布指定策略版本并激活新运行包。
    /// </summary>
    /// <param name="policyVersionIds">待发布的策略版本 ID 集合。</param>
    /// <param name="publishedBy">发布人。</param>
    /// <param name="buildAt">可选构建时间；为空时使用系统时钟。</param>
    /// <returns>运行包构建结果。</returns>
    public async Task<RuntimePackageBuildResult> PublishAsync(
        IReadOnlyCollection<long> policyVersionIds,
        string publishedBy,
        DateTime? buildAt = null)
    {
        // ========== 第一阶段：规范化发布范围 ==========
        // 过滤无效 ID 并去重，避免同一个策略版本重复参与编译导致冲突误判。
        var requestedPolicyVersionIds = policyVersionIds
            .Where(policyVersionId => policyVersionId > 0)
            .Distinct()
            .ToArray();
        if (requestedPolicyVersionIds.Length == 0)
        {
            throw new BizException(
                BizErrorCode.PolicyStatusNotAllowed,
                400,
                "发布策略版本不能为空。");
        }

        // ========== 第二阶段：逐个校验发布资格 ==========
        // REVIEW_REQUIRED 策略必须审批通过且审批 checksum 与当前版本一致，防止审批后改动绕过发布门禁。
        foreach (var policyVersionId in requestedPolicyVersionIds)
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
            var policy = await _policyRepository.GetByIdAsync(version.PolicyId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在: {version.PolicyId}");

            await _eligibilityService.EnsureEligibleAsync(policy, version);
        }

        // ========== 第三阶段：编译运行包 ==========
        // 显式发布路径已经做过发布资格校验，因此这里不要求版本必须处于 PUBLISH_READY。
        var result = await _compiler.CompileAsync(new RuntimePackageBuildContext
        {
            BuiltBy = publishedBy.Trim(),
            BuildAt = buildAt,
            PolicyVersionIds = requestedPolicyVersionIds,
            RequirePublishReadyStatus = false
        });

        // ========== 第四阶段：回写最近构建包 ==========
        // 管理端可以通过该字段追踪某个策略版本最近一次进入了哪个运行包。
        foreach (var policyVersionId in requestedPolicyVersionIds)
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
            version.LastBuiltPackageId = result.Package.PackageId;
            await _policyRepository.UpdateVersionAsync(version);
        }

        // ========== 第五阶段：激活运行包 ==========
        // 只有激活后计价链路才会读取新包；激活服务负责旧包 supersede 和缓存失效。
        await _activationService.ActivateAsync(result.Package.PackageId, publishedBy);
        return result;
    }
}
