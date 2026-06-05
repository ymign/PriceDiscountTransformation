using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Interfaces.Rules;

/// <summary>
/// 规则测试用例仓储接口 —— 规则维护中心的测试用例数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_TEST_CASE。
///
/// 职责边界：
///   - 管理规则版本的测试用例（增删查）。
///   - 支持按规则+版本查询用例列表、按主键查询单个用例。
///   - 测试用例是发布前校验的必要条件，发布前必须存在对应的测试用例。
///
/// 与测试运行记录（PR_RULE_TEST_RUN）的关系：
///   - PR_RULE_TEST_CASE 保存测试用例定义（输入 + 期望输出）。
///   - PR_RULE_TEST_RUN 保存每次运行的实际结果。
///   - 两者是一对多关系：一个用例可运行多次。
/// </summary>
public interface IRuleTestCaseRepository
{
    /// <summary>
    /// 查询指定规则版本的所有测试用例。
    ///
    /// 使用场景：规则版本详情页展示测试用例列表，或批量运行测试时获取用例集。
    /// </summary>
    /// <param name="ruleId">规则主表ID（PR_RULE_HEADER.RULE_ID）。</param>
    /// <param name="versionNo">版本号（PR_RULE_VERSION.VERSION_NO）。</param>
    /// <returns>测试用例列表。</returns>
    Task<IReadOnlyList<RuleTestCase>> GetByRuleAndVersionAsync(long ruleId, int versionNo);

    /// <summary>
    /// 按主键查询单个测试用例。
    ///
    /// 使用场景：查看测试用例详情、编辑测试用例时回显数据。
    /// </summary>
    /// <param name="testCaseId">测试用例主键（PR_RULE_TEST_CASE.TEST_CASE_ID）。</param>
    /// <returns>测试用例实体，不存在时返回 null。</returns>
    Task<RuleTestCase?> GetByIdAsync(long testCaseId);

    /// <summary>
    /// 插入一条测试用例，返回自增主键。
    ///
    /// 调用时机：配置人员创建测试用例时写入。
    /// </summary>
    /// <param name="entity">
    /// 测试用例实体，包含：
    /// - RuleId：关联的规则ID
    /// - VersionNo：关联的版本号
    /// - CaseName：用例名称
    /// - InputJson：输入参数 JSON（CLOB）
    /// - ExpectedJson：期望结果 JSON（CLOB）
    /// - CreatedBy：创建人
    /// </param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RuleTestCase entity);

    /// <summary>
    /// 按主键删除单个测试用例。
    ///
    /// 调用时机：配置人员删除测试用例时调用。
    /// 注意：删除用例时应同步删除关联的测试运行记录（PR_RULE_TEST_RUN），
    /// 或由数据库外键级联删除。
    /// </summary>
    /// <param name="testCaseId">测试用例主键（PR_RULE_TEST_CASE.TEST_CASE_ID）。</param>
    Task DeleteAsync(long testCaseId);
}
