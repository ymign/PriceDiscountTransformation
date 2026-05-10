using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则发布流水实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_PUBLISH
/// </para>
/// <para>
/// 在规则生命周期中的角色：发布流水记录规则发布、停用和回滚事件。
/// 它用于审计和页面展示，不直接作为计价匹配依据。
/// 计价匹配以 PR_RULE_HEADER.CurrentVersion 指向的 PR_RULE_VERSION 为准。
/// </para>
/// <para>
/// 发布前校验（规则发布服务必须执行）：
/// <list type="bullet">
/// <item>规则重叠校验 — 同一项目、同一场景、同一生效期内不允许多套规则同时生效</item>
/// <item>动作组冲突校验 — 同组动作不应存在矛盾配置</item>
/// <item>重复子项目校验 — 主子项目不应重复引用</item>
/// <item>缺少测试用例校验 — 发布前必须有对应的 PR_RULE_TEST_CASE</item>
/// </list>
/// </para>
/// <para>
/// 缓存失效：规则发布/停用/回滚时必须立即失效计价引擎的规则缓存。
/// </para>
/// </remarks>
[SugarTable("PR_RULE_PUBLISH")]
public sealed class RulePublish
{
    /// <summary>
    /// 发布流水主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 PUBLISH_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条发布流水记录。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "PUBLISH_ID")]
    public long PublishId { get; set; }

    /// <summary>
    /// 发布流水号。
    /// </summary>
    /// <remarks>
    /// 业务流水号，用于审计和日志交叉定位。
    /// 格式建议：PUB + 时间戳 + 随机数。
    /// </remarks>
    [SugarColumn(ColumnName = "PUBLISH_NO")]
    public string PublishNo { get; set; } = string.Empty;

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，标识本次发布操作针对哪条规则。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    /// <summary>
    /// 操作前版本号。
    /// </summary>
    /// <remarks>
    /// 首次发布时可以为空（表示从无到有）。
    /// 回滚时指向被回滚的版本号。
    /// 停用时指向被停用的版本号。
    /// </remarks>
    [SugarColumn(ColumnName = "FROM_VERSION", IsNullable = true)]
    public int? FromVersion { get; set; }

    /// <summary>
    /// 操作后版本号。
    /// </summary>
    /// <remarks>
    /// 发布时指向新发布的版本号。
    /// 回滚时指向回滚目标版本号。
    /// 停用时通常与 FromVersion 相同。
    /// </remarks>
    [SugarColumn(ColumnName = "TO_VERSION")]
    public int ToVersion { get; set; }

    /// <summary>
    /// 发布操作类型。
    /// </summary>
    /// <remarks>
    /// 标识本次发布流水的操作类型：
    /// <list type="bullet">
    /// <item>PUBLISH — 正式发布，规则开始生效</item>
    /// <item>DISABLE — 停用，规则不再参与匹配</item>
    /// <item>ROLLBACK — 回滚，恢复到之前版本</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    [SugarColumn(ColumnName = "ACTION_TYPE")]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// 发布、停用或回滚操作人。
    /// </summary>
    /// <remarks>
    /// 记录操作执行者，用于审计和责任追溯。
    /// 来源为工作台登录用户。
    /// </remarks>
    [SugarColumn(ColumnName = "PUBLISHED_BY", IsNullable = true)]
    public string? PublishedBy { get; set; }

    /// <summary>
    /// 发布、停用或回滚发生时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和时间线展示。
    /// </remarks>
    [SugarColumn(ColumnName = "PUBLISHED_AT")]
    public DateTime PublishedAt { get; set; }

    /// <summary>
    /// 发布、停用或回滚备注。
    /// </summary>
    /// <remarks>
    /// 操作人填写的备注信息，用于审计和说明操作原因。
    /// 例如："根据收费处 2026 年 5 月通知，调整 XX 项目折价规则"。
    /// </remarks>
    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }
}
