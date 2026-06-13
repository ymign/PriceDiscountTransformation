namespace Pricing.RuleCenter.Core.Aggregates.Charging;

/// <summary>
/// 计价追踪步骤实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_CHARGE_TRACE_STEP
/// </para>
/// <para>
/// 在计价链路中的角色：每条记录描述计价过程中的一个阶段，按 StepNo 串起来即可还原
/// 规则匹配和动作执行顺序。试算时返回给调用方用于前端展示，确认计价时持久化到数据库。
/// </para>
/// <para>
/// 可追溯性作为架构原则：每笔折价必须可沿三条链路追溯——
/// <list type="bullet">
/// <item>规则变更链 — 通过 RuleId + RuleVersionNo 关联 PR_RULE_HEADER 和 PR_RULE_VERSION</item>
/// <item>计算过程链 — 通过 ChargeTraceStep 的 INPUT_SNAPSHOT 和 OUTPUT_SNAPSHOT 还原每一步</item>
/// <item>最终结果链 — 通过 RequestId 关联 PR_CHARGE_DISCOUNT_DETAIL</item>
/// </list>
/// </para>
/// <para>
/// 步骤类型说明：
/// <list type="bullet">
/// <item>MATCH — 规则匹配阶段，记录命中了哪些规则和条件</item>
/// <item>CONVERT — 双单位换算阶段，记录换算前后的数量</item>
/// <item>FORMULA — 公式计算阶段，记录公式输入和输出</item>
/// <item>LIMIT — 限额校验阶段，记录累计占用和剩余额度</item>
/// <item>DISCOUNT — 折价计算阶段，记录原始金额和折后金额</item>
/// <item>VALIDATE — 校验阶段，记录校验结果</item>
/// <item>ERROR — 异常阶段，记录错误信息</item>
/// </list>
/// </para>
/// </remarks>
public sealed class ChargeTraceStep
{
    /// <summary>
    /// 计价步骤日志主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 STEP_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条计价步骤记录。
    /// </remarks>
        public long StepId { get; set; }

    /// <summary>
    /// 关联的计价请求日志主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_CHARGE_REQUEST_LOG.REQUEST_ID，用于串联请求、步骤、折价明细和限额占用四张表。
    /// 一条请求日志可以产生多条追踪步骤。
    /// </remarks>
        public long RequestId { get; set; }

    /// <summary>
    /// 计价追踪号。
    /// </summary>
    /// <remarks>
    /// 全局唯一的追踪标识，用于跨表查看一次完整计价过程。
    /// 与 ChargeRequest.TraceId 一致。
    /// </remarks>
        public string? TraceId { get; set; }

    /// <summary>
    /// 步骤序号。
    /// </summary>
    /// <remarks>
    /// 按计价链路执行顺序从 1 开始递增。
    /// 顺序为：MATCH → CONVERT → FORMULA → LIMIT → DISCOUNT → VALIDATE。
    /// 前端按 StepNo 排序展示完整的计算过程。
    /// </remarks>
        public int StepNo { get; set; }

    /// <summary>
    /// 步骤名称。
    /// </summary>
    /// <remarks>
    /// 人类可读的步骤名称，用于追溯页面展示。
    /// 例如："规则匹配"、"双单位换算"、"公式计算"、"日数量限额校验"。
    /// </remarks>
        public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// 步骤类型编码。
    /// </summary>
    /// <remarks>
    /// 只使用 DDL 允许的值：MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT、VALIDATE、ERROR。
    /// 决定前端如何渲染该步骤（如 MATCH 步骤显示规则列表，LIMIT 步骤显示额度进度条）。
    /// </remarks>
        public string StepType { get; set; } = string.Empty;

    /// <summary>
    /// 产生该步骤的规则主键。
    /// </summary>
    /// <remarks>
    /// 关联 PR_RULE_HEADER.RULE_ID，用于追溯该步骤是由哪条规则触发的。
    /// 部分步骤类型（如 VALIDATE）可能不关联规则，此时为空。
    /// </remarks>
        public long? RuleId { get; set; }

    /// <summary>
    /// 产生该步骤的规则版本号。
    /// </summary>
    /// <remarks>
    /// 关联 PR_RULE_VERSION.VERSION_NO，用于锁定产生步骤时的规则版本。
    /// 便于后续版本回滚或变更时评估影响范围。
    /// </remarks>
        public int? RuleVersionNo { get; set; }

    /// <summary>
    /// 历史运行包兼容列，当前直接规则链路不再写入。
    /// </summary>
    public long? RuntimePackageId { get; set; }

    /// <summary>
    /// 历史运行时规则兼容列，当前直接规则链路不再写入。
    /// </summary>
    public long? RuntimeRuleId { get; set; }

    /// <summary>
    /// 产生本步骤的来源策略版本主键。
    /// </summary>
    public long? SourcePolicyVersionId { get; set; }

    /// <summary>
    /// 产生本步骤的来源模板版本主键。
    /// </summary>
    public long? SourceTemplateVersionId { get; set; }

    /// <summary>
    /// 步骤执行前的输入快照 JSON。
    /// </summary>
    /// <remarks>
    /// 存储该步骤执行前的输入参数，用于还原计算过程。
    /// 例如：FORMULA 步骤的输入快照包含换算后数量、单价、公式参数等。
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// </remarks>
        public string? InputSnapshot { get; set; }

    /// <summary>
    /// 步骤执行后的输出快照 JSON。
    /// </summary>
    /// <remarks>
    /// 存储该步骤执行后的输出结果，用于还原计算过程。
    /// 例如：FORMULA 步骤的输出快照包含计算后的金额。
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// </remarks>
        public string? OutputSnapshot { get; set; }

    /// <summary>
    /// 步骤说明。
    /// </summary>
    /// <remarks>
    /// 解释该步骤的执行结果，面向追踪页面展示。
    /// 例如：
    /// <list type="bullet">
    /// <item>MATCH — "命中规则 RULE_DISCOUNT_001，条件：门诊 + 皮肤科项目"</item>
    /// <item>LIMIT — "日限额 3 次，已用 2 次，本次 1 次，未超限"</item>
    /// <item>DISCOUNT — "原始金额 100.00 元，折后金额 80.00 元，折让 20.00 元"</item>
    /// </list>
    /// </remarks>
        public string? StepDesc { get; set; }

    /// <summary>
    /// 记录创建时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和排序。
    /// </remarks>
        public DateTime CreatedAt { get; set; }
}
