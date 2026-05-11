using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则主档实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_HEADER
/// </para>
/// <para>
/// 在规则体系中的角色：规则主档保存规则身份、适用范围、优先级和当前发布版本。
/// 条件（PR_RULE_CONDITION）和动作（PR_RULE_ACTION）挂在具体版本上，
/// 主档的 CurrentVersion 决定计价时读取哪套版本明细。
/// </para>
/// <para>
/// 规则生命周期：DRAFT（草稿）→ PUBLISHED（已发布）→ DISABLED（已停用）
/// → ROLLED_BACK（已回滚）。版本管理通过 PR_RULE_VERSION 实现。
/// </para>
/// <para>
/// 规则冲突校验（发布前必须执行）：
/// <list type="bullet">
/// <item>同一项目、同一场景、同一生效期内不允许多套折价公式或不同折价额度规则同时生效</item>
/// <item>同一项目允许按不同部位维护不同换算规则</item>
/// </list>
/// </para>
/// </remarks>
[SugarTable("PR_RULE_HEADER")]
public sealed class RuleHeader
{
    /// <summary>
    /// 规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 RULE_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条规则，是关联规则头、版本、条件、动作和追溯结果的核心外键。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    /// <summary>
    /// 规则编码。
    /// </summary>
    /// <remarks>
    /// 全局唯一的业务键，用于业务配置和运维识别。
    /// 格式建议：RULE_ + 分类缩写 + 序号，如 RULE_DISCOUNT_001。
    /// 修改编码需评估所有引用该规则的追溯记录。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_CODE")]
    public string RuleCode { get; set; } = string.Empty;

    /// <summary>
    /// 规则名称。
    /// </summary>
    /// <remarks>
    /// 面向配置人员展示的中文名称，用于工作台展示和审计。
    /// 例如："皮肤科治疗项目折价规则"。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_NAME")]
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// 规则类别编码。
    /// </summary>
    /// <remarks>
    /// 标识该规则的业务类别：
    /// <list type="bullet">
    /// <item>DISCOUNT — 纯折价规则</item>
    /// <item>FORMULA — 纯公式规则</item>
    /// <item>LIMIT — 纯限额规则</item>
    /// <item>MIXED — 混合规则（同时包含折价、公式、限额）</item>
    /// </list>
    /// 默认值为 "MIXED"。该值应与 PR_DICT 中的 RULE_CATEGORY 字典对应。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_CATEGORY")]
    public string RuleCategory { get; set; } = "MIXED";

    /// <summary>
    /// 规则作用范围。
    /// </summary>
    /// <remarks>
    /// 标识该规则作用于什么粒度：
    /// <list type="bullet">
    /// <item>ITEM — 单个项目，通过 ITEM_CODE 匹配</item>
    /// <item>GROUP — 项目组，通过 GROUP_CODE 匹配</item>
    /// <item>SCENE — 场景级，通过条件中的收费场景匹配</item>
    /// </list>
    /// 默认值为 "ITEM"。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_SCOPE")]
    public string RuleScope { get; set; } = "ITEM";

    /// <summary>
    /// 项目编码。
    /// </summary>
    /// <remarks>
    /// 对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// 当 RULE_SCOPE = "ITEM" 时，该字段指定规则针对哪个项目。
    /// 空值表示该规则不针对特定项目（如按项目组或场景匹配）。
    /// </remarks>
    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    public string? ItemCode { get; set; }

    /// <summary>
    /// 项目名称。
    /// </summary>
    /// <remarks>
    /// 来源为 HIS 物价主数据，用于工作台展示和审计。
    /// 非计算字段，仅用于可读性。
    /// </remarks>
    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    public string? ItemName { get; set; }

    /// <summary>
    /// 项目组编码。
    /// </summary>
    /// <remarks>
    /// 对应 PR_ITEM_GROUP.GROUP_CODE。
    /// 当 RULE_SCOPE = "GROUP" 时，该字段指定规则针对哪个项目组。
    /// 空值表示该规则不针对特定项目组。
    /// </remarks>
    [SugarColumn(ColumnName = "GROUP_CODE", IsNullable = true)]
    public string? GroupCode { get; set; }

    /// <summary>
    /// 规则优先级。
    /// </summary>
    /// <remarks>
    /// 数字越小优先级越高，越先参与匹配和动作排序。
    /// 当同一项目命中多条规则时，按优先级排序执行。
    /// 默认值为 100，建议间隔 10，便于插入。
    /// </remarks>
    [SugarColumn(ColumnName = "PRIORITY")]
    public int Priority { get; set; } = 100;

    /// <summary>
    /// 当前生效版本号。
    /// </summary>
    /// <remarks>
    /// 指向 PR_RULE_VERSION.VERSION_NO，决定计价时读取哪套版本明细。
    /// 由规则生命周期服务在发布或回滚时维护。
    /// 0 或特殊值表示该规则尚未发布任何版本。
    /// </remarks>
    [SugarColumn(ColumnName = "CURRENT_VERSION")]
    public int CurrentVersion { get; set; }

    /// <summary>
    /// 规则状态。
    /// </summary>
    /// <remarks>
    /// 规则生命周期状态：
    /// <list type="bullet">
    /// <item>DRAFT — 草稿，正在编辑中</item>
    /// <item>PUBLISHED — 已发布，规则生效中</item>
    /// <item>DISABLED — 已停用，不再参与匹配</item>
    /// <item>ROLLED_BACK — 已回滚，恢复到之前版本</item>
    /// </list>
    /// 默认值为 "DRAFT"。
    /// </remarks>
    [SugarColumn(ColumnName = "STATUS")]
    public string Status { get; set; } = "DRAFT";

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该规则参与计价匹配，"N" 表示禁用。
    /// 禁用的规则不出现在生效规则查询结果中。
    /// 与 STATUS 的区别：STATUS 描述生命周期，IS_ENABLED 描述是否参与匹配。
    /// 默认值为 "Y"。
    /// </remarks>
    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

    /// <summary>
    /// 规则生效开始时间。
    /// </summary>
    /// <remarks>
    /// 规则的生效开始时间，业务收费时间早于该时间的请求不匹配该规则。
    /// 空值表示无生效开始时间限制（从创建时起生效）。
    /// 规则生效期判断必须使用业务收费发生时间（BusinessChargeTime），
    /// 不得使用技术占用时间替代。
    /// </remarks>
    [SugarColumn(ColumnName = "EFFECTIVE_FROM", IsNullable = true)]
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// 规则生效结束时间。
    /// </summary>
    /// <remarks>
    /// 规则的生效结束时间，业务收费时间晚于该时间的请求不匹配该规则。
    /// 空值表示未设失效时间（长期有效）。
    /// </remarks>
    [SugarColumn(ColumnName = "EFFECTIVE_TO", IsNullable = true)]
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// 回滚模式，决定计价服务不可用时的降级策略。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 当计价服务出现故障或超时时，各渠道根据此字段决定降级行为：
    /// <list type="bullet">
    /// <item><c>STOP_CHARGE</c> — 暂停收费转人工（默认，最安全）。适用于资金风险最高的项目。</item>
    /// <item><c>LEGACY_EQUIVALENT</c> — 自动切回旧计价逻辑（需审批）。适用于已验证等价性的项目。</item>
    /// <item><c>MANUAL_REVIEW</c> — 继续使用新服务但标记需人工复核。适用于金额较小或影响面低的项目。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 资金安全约束：<c>LEGACY_EQUIVALENT</c> 必须经过审批流程，
    /// 且需要确认新旧逻辑在所有边界场景下的计算结果一致。
    /// 其他模式不允许自动回退到旧逻辑，必须转人工、暂停或继续使用新服务。
    /// </para>
    /// <para>
    /// 空值行为：NULL 等价于 <c>STOP_CHARGE</c>（最安全策略）。
    /// </para>
    /// </remarks>
    [SugarColumn(ColumnName = "ROLLBACK_MODE", IsNullable = true)]
    public string? RollbackMode { get; set; }

    /// <summary>
    /// 规则备注。
    /// </summary>
    /// <remarks>
    /// 配置人员或开发人员填写的补充说明。
    /// 例如："该规则根据收费处 2026 年 5 月通知新增"。
    /// </remarks>
    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计。
    /// </remarks>
    [SugarColumn(ColumnName = "CREATED_BY", IsNullable = true)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 记录创建时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和排序。
    /// </remarks>
    [SugarColumn(ColumnName = "CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后修改人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计。
    /// </remarks>
    [SugarColumn(ColumnName = "UPDATED_BY", IsNullable = true)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 记录最后更新时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心在每次更新时自动填充，用于乐观锁和审计。
    /// </remarks>
    [SugarColumn(ColumnName = "UPDATED_AT")]
    public DateTime UpdatedAt { get; set; }
}
