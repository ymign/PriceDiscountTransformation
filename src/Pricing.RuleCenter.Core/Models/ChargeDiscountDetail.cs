using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_DISCOUNT_DETAIL")]
/// <summary>
/// 计价折扣明细实体，对应 PR_CHARGE_DISCOUNT_DETAIL。
/// </summary>
/// <remarks>
/// 该实体保存一次计价中原始数量/金额、换算数量、最终数量/金额和折价原因。它跟随请求状态流转，
/// 用于 HIS 对账、规则追踪和后续退费冲正。
/// </remarks>
public sealed class ChargeDiscountDetail
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DISCOUNT_ID")]
    /// <summary>
    /// 折扣明细主键。
    /// </summary>
    public long DiscountId { get; set; }

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

    [SugarColumn(ColumnName = "CHARGE_NO", IsNullable = true)]
    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; set; }

    [SugarColumn(ColumnName = "CHARGE_DETAIL_NO", IsNullable = true)]
    /// <summary>
    /// 收费明细号，用于定位单条收费项目
    /// </summary>
    public string? ChargeDetailNo { get; set; }

    [SugarColumn(ColumnName = "PATIENT_ID", IsNullable = true)]
    /// <summary>
    /// 患者标识，是限额累计和追溯查询的重要维度
    /// </summary>
    public string? PatientId { get; set; }

    [SugarColumn(ColumnName = "VISIT_ID", IsNullable = true)]
    /// <summary>
    /// 就诊标识，可为空，存在时用于缩小追溯和对账范围
    /// </summary>
    public string? VisitId { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "RULE_ID", IsNullable = true)]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long? RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_VERSION_NO", IsNullable = true)]
    /// <summary>
    /// 产生本次折价结果的规则版本号。
    /// </summary>
    public int? RuleVersionNo { get; set; }

    [SugarColumn(ColumnName = "DISCOUNT_TYPE", IsNullable = true)]
    /// <summary>
    /// 折价类型，例如公式折算、限额截断或超额归零。
    /// </summary>
    public string? DiscountType { get; set; }

    [SugarColumn(ColumnName = "STATUS")]
    /// <summary>
    /// 当前记录状态，具体含义由所在表的状态机定义
    /// </summary>
    public string Status { get; set; } = "PENDING";

    [SugarColumn(ColumnName = "RESULT_GROUP_NO", IsNullable = true)]
    /// <summary>
    /// 主子项目同组结果号，用于保证主项目和加收子项目原子处理
    /// </summary>
    public string? ResultGroupNo { get; set; }

    [SugarColumn(ColumnName = "PARENT_DISCOUNT_ID", IsNullable = true)]
    /// <summary>
    /// 父折扣明细主键，用于主子项目或拆分明细之间建立层级关系。
    /// </summary>
    public long? ParentDiscountId { get; set; }

    [SugarColumn(ColumnName = "PART_SEQ", IsNullable = true)]
    /// <summary>
    /// 多部位或多片段计价时的片段序号。
    /// </summary>
    public int? PartSeq { get; set; }

    [SugarColumn(ColumnName = "ORIGINAL_QTY", IsNullable = true)]
    /// <summary>
    /// 原始数量，保存折价前的业务输入
    /// </summary>
    public decimal? OriginalQty { get; set; }

    [SugarColumn(ColumnName = "CONVERTED_QTY", IsNullable = true)]
    /// <summary>
    /// 换算后的计价数量
    /// </summary>
    public decimal? ConvertedQty { get; set; }

    [SugarColumn(ColumnName = "FINAL_QTY", IsNullable = true)]
    /// <summary>
    /// 最终可收费数量
    /// </summary>
    public decimal? FinalQty { get; set; }

    [SugarColumn(ColumnName = "UNIT_PRICE", IsNullable = true)]
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验
    /// </summary>
    public decimal? UnitPrice { get; set; }

    [SugarColumn(ColumnName = "ORIGINAL_AMT", IsNullable = true)]
    /// <summary>
    /// 原始金额，通常为原始数量乘权威单价
    /// </summary>
    public decimal? OriginalAmt { get; set; }

    [SugarColumn(ColumnName = "CALCULATED_AMT", IsNullable = true)]
    /// <summary>
    /// 公式或动作链中间计算金额
    /// </summary>
    public decimal? CalculatedAmt { get; set; }

    [SugarColumn(ColumnName = "FINAL_AMT", IsNullable = true)]
    /// <summary>
    /// 最终收费金额
    /// </summary>
    public decimal? FinalAmt { get; set; }

    [SugarColumn(ColumnName = "DISCOUNT_AMT", IsNullable = true)]
    /// <summary>
    /// 折价金额，等于原始金额减最终金额
    /// </summary>
    public decimal? DiscountAmt { get; set; }

    [SugarColumn(ColumnName = "REASON_CODE", IsNullable = true)]
    /// <summary>
    /// 折价原因编码，便于与规则或业务原因字典关联。
    /// </summary>
    public string? ReasonCode { get; set; }

    [SugarColumn(ColumnName = "REASON_DESC", IsNullable = true)]
    /// <summary>
    /// 折价原因描述，面向追踪页面或对账说明展示。
    /// </summary>
    public string? ReasonDesc { get; set; }

    [SugarColumn(ColumnName = "LIMIT_BASE_INFO", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 限额计算依据快照 JSON，用于解释当时累计了哪些维度和窗口。
    /// </summary>
    public string? LimitBaseInfo { get; set; }

    [SugarColumn(ColumnName = "OCCURRED_AT")]
    /// <summary>
    /// 折价明细生成时间。
    /// </summary>
    public DateTime OccurredAt { get; set; }

    [SugarColumn(ColumnName = "CONFIRMED_BY", IsNullable = true)]
    /// <summary>
    /// 确认操作人或来源系统账号。
    /// </summary>
    public string? ConfirmedBy { get; set; }

    [SugarColumn(ColumnName = "COMMITTED_AT", IsNullable = true)]
    /// <summary>
    /// HIS 成功落账并调用 commit 的时间。
    /// </summary>
    public DateTime? CommittedAt { get; set; }

    [SugarColumn(ColumnName = "CANCELLED_AT", IsNullable = true)]
    /// <summary>
    /// 保护占用被取消的时间。
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    [SugarColumn(ColumnName = "EXPIRED_AT", IsNullable = true)]
    /// <summary>
    /// confirm 保护期过期并被清理的时间。
    /// </summary>
    public DateTime? ExpiredAt { get; set; }

    [SugarColumn(ColumnName = "REVERSED_AT", IsNullable = true)]
    /// <summary>
    /// 已提交折扣明细被冲正的时间。
    /// </summary>
    public DateTime? ReversedAt { get; set; }
}
