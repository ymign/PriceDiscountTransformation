using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则测试运行记录仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_TEST_RUN 表的查询和写入操作，包括：
/// 按测试用例查询运行历史、插入运行记录。
/// </para>
/// <para>
/// 【测试运行模型】
/// 测试运行记录保存每次测试用例的实际执行结果，包括：
/// - 实际结果 JSON（ACTUAL_JSON）
/// - 是否通过（IS_PASS）
/// - 运行时间和运行人
/// - 错误信息（异常时记录）
/// </para>
/// <para>
/// 【CLOB 字段说明】
/// ACTUAL_JSON 为 CLOB 类型，存储在 Oracle 中无原生 JSON 支持。
/// 使用 SqlSugar 的 ColumnDataType = "CLOB" 映射。
/// </para>
/// <para>
/// 【与测试用例的关系】
/// PR_RULE_TEST_CASE 保存用例定义（输入 + 期望输出），
/// PR_RULE_TEST_RUN 保存每次运行的实际结果。
/// 同一个测试用例可以运行多次，记录每次的实际结果和通过状态。
/// </para>
/// </remarks>
public sealed class RuleTestRunRepository : IRuleTestRunRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和测试运行表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则测试运行记录仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleTestRunRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按测试用例主键查询运行记录，按运行时间倒序排列。
    /// </summary>
    /// <param name="testCaseId">测试用例主键（PR_RULE_TEST_CASE.TEST_CASE_ID）。</param>
    /// <returns>按运行时间倒序排列的运行记录集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_TEST_RUN
    /// WHERE TEST_CASE_ID = :testCaseId
    /// ORDER BY RUN_AT DESC
    /// </code>
    /// 【排序策略】倒序排列使最近一次运行记录出现在列表最前面，便于查看最新结果。
    /// 【使用场景】测试用例详情页展示运行历史、判断用例最新一次运行是否通过。
    /// </remarks>
    public async Task<IReadOnlyList<RuleTestRun>> GetByTestCaseIdAsync(long testCaseId)
    {
        return await _db.Queryable<RuleTestRun>()
            .Where(r => r.TestCaseId == testCaseId)
            .OrderByDescending(r => r.RunAt)
            .ToListAsync();
    }

    /// <summary>
    /// 插入测试运行记录。
    /// </summary>
    /// <param name="entity">待写入的测试运行记录实体。</param>
    /// <returns>Oracle 序列生成的测试运行主键（TEST_RUN_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_TEST_RUN.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_TEST_RUN (...) VALUES (...)
    /// </code>
    /// 【事务边界】本方法不开启事务。调用方（测试服务）应在更大的事务中调用本方法，
    /// 保证运行记录与测试用例状态的原子更新。
    /// </remarks>
    public async Task<long> InsertAsync(RuleTestRun entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_TEST_RUN.NEXTVAL FROM DUAL");
        entity.TestRunId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}