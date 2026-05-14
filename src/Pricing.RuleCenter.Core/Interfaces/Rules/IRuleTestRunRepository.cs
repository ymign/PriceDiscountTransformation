using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Interfaces.Rules;

/// <summary>
/// 规则测试运行记录仓储接口 —— 规则维护中心的测试运行数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_RULE_TEST_RUN。
///
/// 职责边界：
///   - 记录每次测试用例的运行结果。
///   - 支持按测试用例查询运行历史。
///   - 运行记录是测试质量追溯的关键数据，不可物理删除。
///
/// 与测试用例（PR_RULE_TEST_CASE）的关系：
///   - PR_RULE_TEST_CASE 保存测试用例定义（输入 + 期望输出）。
///   - PR_RULE_TEST_RUN 保存每次运行的实际结果。
///   - 两者是一对多关系：一个用例可运行多次。
/// </summary>
public interface IRuleTestRunRepository
{
    /// <summary>
    /// 查询指定测试用例的所有运行记录。
    ///
    /// 使用场景：测试用例详情页展示运行历史，或判断用例最新一次运行是否通过。
    /// </summary>
    /// <param name="testCaseId">测试用例主键（PR_RULE_TEST_CASE.TEST_CASE_ID）。</param>
    /// <returns>运行记录列表，按运行时间倒序排列（最新的在前）。</returns>
    Task<IReadOnlyList<RuleTestRun>> GetByTestCaseIdAsync(long testCaseId);

    /// <summary>
    /// 插入一条测试运行记录，返回自增主键。
    ///
    /// 调用时机：测试运行完成后写入，记录实际结果、通过状态和可能的错误信息。
    /// </summary>
    /// <param name="entity">
    /// 测试运行记录实体，包含：
    /// - TestCaseId：关联的测试用例ID
    /// - RuleId：关联的规则ID
    /// - ActualJson：实际结果 JSON（CLOB）
    /// - IsPass：是否通过（Y/N）
    /// - RunAt：运行时间
    /// - RunBy：运行人（可选）
    /// - ErrorMessage：错误信息（可选）
    /// </param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(RuleTestRun entity);
}
