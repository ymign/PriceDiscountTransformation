using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_LIMIT_OCCUPY")]
/// <summary>
/// 限额占用记录实体，对应 PR_LIMIT_OCCUPY。
/// </summary>
/// <remarks>
/// 该表是限额控制的事实表。confirm 阶段先写 PENDING 保护占用，commit 后推进为 CONFIRMED，
/// cancel/expire/reverse 再释放或终止占用。累计限额时需要同时看 PENDING 和 CONFIRMED，防止并发突破上限。
/// </remarks>
public sealed class LimitOccupy
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "OCCUPY_ID")]
    /// <summary>
    /// 限额占用主键。
    /// </summary>
    public long OccupyId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_ID")]
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    /// <summary>
    /// 计价追踪号，用于跨表查看一次计价过程
    /// </summary>
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "PATIENT_ID")]
    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    public string PatientId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "RULE_ID", IsNullable = true)]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long? RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_VERSION_NO", IsNullable = true)]
    /// <summary>
    /// 产生本次占用的规则版本号。
    /// </summary>
    public int? RuleVersionNo { get; set; }

    [SugarColumn(ColumnName = "LIMIT_TYPE")]
    /// <summary>
    /// 限额类型，例如 DAY_QTY、TIME_WINDOW、SAME_OPERATION
    /// </summary>
    public string LimitType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "LIMIT_KEY")]
    /// <summary>
    /// 限额锁键或占额键，由计价中心生成，渠道不得传入
    /// </summary>
    public string LimitKey { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "OCCUPY_QTY")]
    /// <summary>
    /// 本次占用数量，冲正释放时可为负数
    /// </summary>
    public decimal OccupyQty { get; set; }

    [SugarColumn(ColumnName = "OCCUPY_AMT")]
    /// <summary>
    /// 本次占用金额，冲正释放时可为负数
    /// </summary>
    public decimal OccupyAmt { get; set; }

    [SugarColumn(ColumnName = "OCCUPY_TYPE")]
    /// <summary>
    /// 占用类型，CHARGE 表示收费占用，REVERSE 表示退费释放
    /// </summary>
    public string OccupyType { get; set; } = "CHARGE";

    [SugarColumn(ColumnName = "ORIGINAL_OCCUPY_ID", IsNullable = true)]
    /// <summary>
    /// 冲正记录关联的原占用记录主键
    /// </summary>
    public long? OriginalOccupyId { get; set; }

    [SugarColumn(ColumnName = "BUSINESS_CHARGE_TIME")]
    /// <summary>
    /// 业务收费发生时间，用于规则生效判断和滑动窗口累计
    /// </summary>
    public DateTime BusinessChargeTime { get; set; }

    [SugarColumn(ColumnName = "LIMIT_DIMENSION_CODE", IsNullable = true)]
    /// <summary>
    /// 限额查询维度，用于按业务时间稳定累计
    /// </summary>
    public string? LimitDimensionCode { get; set; }

    [SugarColumn(ColumnName = "PART_SEQ", IsNullable = true)]
    /// <summary>
    /// 多部位或多片段计价时的片段序号。
    /// </summary>
    public int? PartSeq { get; set; }

    [SugarColumn(ColumnName = "STATUS")]
    /// <summary>
    /// 当前记录状态，具体含义由所在表的状态机定义
    /// </summary>
    public string Status { get; set; } = "PENDING";

    [SugarColumn(ColumnName = "OCCUPIED_AT")]
    /// <summary>
    /// 计价中心创建占额记录的技术时间
    /// </summary>
    public DateTime OccupiedAt { get; set; }

    [SugarColumn(ColumnName = "CONFIRMED_AT", IsNullable = true)]
    /// <summary>
    /// 业务系统 commit 成功后的确认时间
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    [SugarColumn(ColumnName = "EXPIRE_AT", IsNullable = true)]
    /// <summary>
    /// confirm 结果过期时间，超过后不能继续 commit
    /// </summary>
    public DateTime? ExpireAt { get; set; }
}
