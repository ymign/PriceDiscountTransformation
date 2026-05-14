using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Rules;

/// <summary>
/// 规则测试用例仓储实现。
/// </summary>
/// <remarks>
/// <para>
/// 【职责范围】
/// 封装 PR_RULE_TEST_CASE 表的查询、写入和删除操作，包括：
/// 按规则+版本查询用例列表、按主键查询单个用例、插入用例、删除用例。
/// </para>
/// <para>
/// 【测试用例模型】
/// 测试用例保存规则的测试输入（INPUT_JSON）和期望输出（EXPECTED_JSON），
/// 用于发布前验证规则配置的正确性。每个规则版本可以有多个测试用例，
/// 覆盖不同场景（正常折价、超出限额、边界条件等）。
/// </para>
/// <para>
/// 【CLOB 字段说明】
/// INPUT_JSON 和 EXPECTED_JSON 均为 CLOB 类型，存储在 Oracle 中无原生 JSON 支持。
/// 使用 SqlSugar 的 ColumnDataType = "CLOB" 映射。
/// </para>
/// <para>
/// 【与测试运行记录的关系】
/// PR_RULE_TEST_CASE 保存用例定义，PR_RULE_TEST_RUN 保存运行结果。
/// 删除用例时应同步考虑关联的运行记录（可通过数据库外键级联删除或应用层处理）。
/// </para>
/// </remarks>
public sealed class RuleTestCaseRepository : IRuleTestCaseRepository
{
    /// <summary>
    /// SqlSugar 数据库客户端实例。
    /// </summary>
    /// <remarks>
    /// 由 DI 容器按 Scoped 生命周期注入，用于访问 Oracle 序列和测试用例表。
    /// </remarks>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则测试用例仓储。
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端，由 DI 容器按 Scoped 生命周期注入。</param>
    public RuleTestCaseRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按规则主键和版本号查询测试用例列表。
    /// </summary>
    /// <param name="ruleId">规则主键（PR_RULE_HEADER.RULE_ID）。</param>
    /// <param name="versionNo">版本号（PR_RULE_VERSION.VERSION_NO）。</param>
    /// <returns>指定规则版本的测试用例集合；无记录时返回空集合。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_TEST_CASE
    /// WHERE RULE_ID = :ruleId AND VERSION_NO = :versionNo
    /// </code>
    /// 【使用场景】规则版本详情页展示测试用例列表、批量运行测试时获取用例集。
    /// </remarks>
    public async Task<IReadOnlyList<RuleTestCase>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return await _db.Queryable<RuleTestCase>()
            .Where(t => t.RuleId == ruleId && t.VersionNo == versionNo)
            .ToListAsync();
    }

    /// <summary>
    /// 按主键查询单个测试用例。
    /// </summary>
    /// <param name="testCaseId">测试用例主键（PR_RULE_TEST_CASE.TEST_CASE_ID）。</param>
    /// <returns>测试用例实体，不存在时返回 null。</returns>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// SELECT * FROM PR_RULE_TEST_CASE WHERE TEST_CASE_ID = :testCaseId
    /// </code>
    /// 【使用场景】查看测试用例详情、编辑测试用例时回显数据。
    /// </remarks>
    public async Task<RuleTestCase?> GetByIdAsync(long testCaseId)
    {
        return await _db.Queryable<RuleTestCase>()
            .Where(t => t.TestCaseId == testCaseId)
            .FirstAsync();
    }

    /// <summary>
    /// 插入测试用例记录。
    /// </summary>
    /// <param name="entity">待写入的测试用例实体。</param>
    /// <returns>Oracle 序列生成的测试用例主键（TEST_CASE_ID）。</returns>
    /// <remarks>
    /// 【SQL 语义】分两步执行：
    /// <code>
    /// SELECT SEQ_PR_RULE_TEST_CASE.NEXTVAL FROM DUAL
    /// INSERT INTO PR_RULE_TEST_CASE (...) VALUES (...)
    /// </code>
    /// 【事务边界】本方法不开启事务。调用方（测试服务）应在更大的事务中调用本方法，
    /// 保证测试用例与规则版本的原子更新。
    /// </remarks>
    public async Task<long> InsertAsync(RuleTestCase entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_TEST_CASE.NEXTVAL FROM DUAL");
        entity.TestCaseId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <summary>
    /// 按主键删除单个测试用例。
    /// </summary>
    /// <param name="testCaseId">测试用例主键（PR_RULE_TEST_CASE.TEST_CASE_ID）。</param>
    /// <remarks>
    /// 【SQL 语义】等价于：
    /// <code>
    /// DELETE FROM PR_RULE_TEST_CASE WHERE TEST_CASE_ID = :testCaseId
    /// </code>
    /// 【级联删除】如果数据库配置了外键级联删除，关联的 PR_RULE_TEST_RUN 记录会自动清理。
    /// 否则应用层应在删除用例前手动清理关联的运行记录。
    /// 【使用场景】配置人员删除测试用例时调用。
    /// </remarks>
    public async Task DeleteAsync(long testCaseId)
    {
        await _db.Deleteable<RuleTestCase>()
            .Where(t => t.TestCaseId == testCaseId)
            .ExecuteCommandAsync();
    }
}