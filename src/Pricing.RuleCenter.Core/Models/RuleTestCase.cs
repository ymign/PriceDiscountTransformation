namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则测试用例表实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_TEST_CASE
/// </para>
/// <para>
/// 在规则生命周期中的角色：测试用例保存规则的测试输入和期望输出，
/// 用于发布前验证规则配置的正确性。发布前校验必须检查是否存在对应的测试用例。
/// </para>
/// <para>
/// 测试用例与规则版本绑定（通过 RULE_ID + VERSION_NO），
/// 每个规则版本可以有多个测试用例，覆盖不同场景。
/// </para>
/// <para>
/// 输入与期望格式：
/// <list type="bullet">
/// <item>INPUT_JSON — 输入参数的 JSON 序列化，包含项目编码、数量、场景、部位等</item>
/// <item>EXPECTED_JSON — 期望结果的 JSON 序列化，包含折后金额、是否触发限制等</item>
/// </list>
/// 两者均为 CLOB 类型，存储在 Oracle 中无原生 JSON 支持，使用 CLOB 存储。
/// </para>
/// </remarks>
public sealed class RuleTestCase
{
    /// <summary>
    /// 测试用例主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 TEST_CASE_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条测试用例记录。
    /// </remarks>
        public long TestCaseId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，标识该测试用例属于哪条规则。
    /// 与 VERSION_NO 联合定位特定规则版本的测试用例。
    /// </remarks>
        public long RuleId { get; set; }

    /// <summary>
    /// 版本号。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_VERSION.VERSION_NO，标识该测试用例属于规则的哪个版本。
    /// 与 RULE_ID 联合定位特定规则版本的测试用例。
    /// </remarks>
        public int VersionNo { get; set; }

    /// <summary>
    /// 用例名称。
    /// </summary>
    /// <remarks>
    /// 面向配置人员展示的测试用例名称，用于标识测试场景。
    /// 例如："皮肤科治疗项目-正常数量折价"、"超出日限额-应返回0元"。
    /// </remarks>
        public string CaseName { get; set; } = string.Empty;

    /// <summary>
    /// 输入参数 JSON。
    /// </summary>
    /// <remarks>
    /// 测试用例的输入参数，JSON 格式序列化，CLOB 类型存储。
    /// 包含计价请求的核心参数：项目编码、数量、收费场景、部位、业务时间等。
    /// 对应计价接口的请求体结构。
    /// 可为空，表示该用例暂未配置输入。
    /// </remarks>
        public string? InputJson { get; set; }

    /// <summary>
    /// 期望结果 JSON。
    /// </summary>
    /// <remarks>
    /// 测试用例的期望输出结果，JSON 格式序列化，CLOB 类型存储。
    /// 包含计价结果的核心字段：折后金额、是否触发限制、匹配的规则版本等。
    /// 对应计价接口的响应体结构。
    /// 可为空，表示该用例暂未配置期望结果。
    /// </remarks>
        public string? ExpectedJson { get; set; }

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该测试用例参与测试运行，"N" 表示禁用。
    /// 禁用的用例在批量测试时跳过，但保留历史运行记录。
    /// 默认值为 "Y"。
    /// </remarks>
        public string IsEnabled { get; set; } = "Y";

    /// <summary>
    /// 创建人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计。
    /// </remarks>
        public string? CreatedBy { get; set; }

    /// <summary>
    /// 记录创建时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和排序。
    /// </remarks>
        public DateTime CreatedAt { get; set; }
}
