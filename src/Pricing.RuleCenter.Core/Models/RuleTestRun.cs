namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则测试运行记录实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_TEST_RUN
/// </para>
/// <para>
/// 在规则生命周期中的角色：记录每次测试用例的运行结果，
/// 用于验证规则配置的正确性和发布前的质量把关。
/// </para>
/// <para>
/// 测试运行记录与测试用例（PR_RULE_TEST_CASE）是一对多关系，
/// 同一个测试用例可以运行多次，记录每次的实际结果和通过状态。
/// </para>
/// <para>
/// 测试结果说明：
/// <list type="bullet">
/// <item>IS_PASS = "Y" — 实际结果与期望结果一致，测试通过</item>
/// <item>IS_PASS = "N" — 实际结果与期望结果不一致，测试失败</item>
/// </list>
/// 发布前校验应检查所有关联测试用例的最新运行记录是否全部通过。
/// </para>
/// </remarks>
public sealed class RuleTestRun
{
    /// <summary>
    /// 测试运行记录主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 TEST_RUN_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条测试运行记录。
    /// </remarks>
        public long TestRunId { get; set; }

    /// <summary>
    /// 关联的测试用例主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_TEST_CASE.TEST_CASE_ID，标识本次运行针对哪个测试用例。
    /// </remarks>
        public long TestCaseId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，冗余存储便于按规则维度查询测试运行记录。
    /// </remarks>
        public long RuleId { get; set; }

    /// <summary>
    /// 实际结果 JSON。
    /// </summary>
    /// <remarks>
    /// 本次测试运行的实际输出结果，JSON 格式序列化，CLOB 类型存储。
    /// 包含计价引擎的实际计算结果：折后金额、是否触发限制、匹配的规则版本等。
    /// 用于与 EXPECTED_JSON 比对，判断测试是否通过。
    /// 可为空，表示运行异常未产生结果。
    /// </remarks>
        public string? ActualJson { get; set; }

    /// <summary>
    /// 是否通过标识。
    /// </summary>
    /// <remarks>
    /// 标识本次测试运行是否通过：
    /// <list type="bullet">
    /// <item>Y — 测试通过，实际结果与期望结果一致</item>
    /// <item>N — 测试失败，实际结果与期望结果不一致</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
        public string IsPass { get; set; } = "N";

    /// <summary>
    /// 运行时间。
    /// </summary>
    /// <remarks>
    /// 测试运行执行的时间，由计价中心自动填充。
    /// 用于记录测试历史和排序。
    /// </remarks>
        public DateTime RunAt { get; set; }

    /// <summary>
    /// 运行人。
    /// </summary>
    /// <remarks>
    /// 执行测试运行的操作人，来源为工作台登录用户。
    /// 可为空，表示系统自动运行。
    /// </remarks>
        public string? RunBy { get; set; }

    /// <summary>
    /// 错误信息。
    /// </summary>
    /// <remarks>
    /// 测试运行过程中发生的异常信息。
    /// 当测试运行出现异常（如输入参数解析失败、计价引擎抛出异常等）时记录。
    /// 正常运行时为空。
    /// </remarks>
        public string? ErrorMessage { get; set; }
}
