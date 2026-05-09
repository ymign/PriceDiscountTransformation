using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_REQUEST_LOG")]
/// <summary>
/// 计价请求日志实体，对应 PR_CHARGE_REQUEST_LOG。
/// </summary>
/// <remarks>
/// 这是计价链路的主表。每次试算、确认、提交、取消或冲正都会形成请求日志，
/// 幂等判断、响应重放、追踪查询和过期清理都以该表为入口。
/// </remarks>
public sealed class ChargeRequestLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "REQUEST_ID")]
    /// <summary>
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_NO")]
    /// <summary>
    /// 调用方或服务端生成的技术请求流水号，用于排查单次 HTTP 调用
    /// </summary>
    public string RequestNo { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "BUSINESS_REQUEST_NO", IsNullable = true)]
    /// <summary>
    /// 调用方稳定业务号，confirm 重试必须复用，用于幂等保护
    /// </summary>
    public string? BusinessRequestNo { get; set; }

    [SugarColumn(ColumnName = "REQUEST_FINGERPRINT", IsNullable = true)]
    /// <summary>
    /// 规范化请求指纹，用于判断同一业务号下参数是否发生变化
    /// </summary>
    public string? RequestFingerprint { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    /// <summary>
    /// 计价追踪号，用于跨表查看一次计价过程
    /// </summary>
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "CALL_TYPE")]
    /// <summary>
    /// 调用类型，例如 SIMULATE、CONFIRM、COMMIT、CANCEL 或 REVERSE
    /// </summary>
    public string CallType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "BUSINESS_STATUS")]
    /// <summary>
    /// 业务状态，描述请求在试算、待确认、已落账、取消、过期、冲正等状态机中的位置
    /// </summary>
    public string BusinessStatus { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "SOURCE_SYSTEM")]
    /// <summary>
    /// 来源系统编码，例如 HIS、自助机或微信公众号
    /// </summary>
    public string SourceSystem { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "SOURCE_TERMINAL", IsNullable = true)]
    /// <summary>
    /// 来源终端或站点标识，用于定位具体调用入口
    /// </summary>
    public string? SourceTerminal { get; set; }

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

    [SugarColumn(ColumnName = "CHARGE_SCENE", IsNullable = true)]
    /// <summary>
    /// 收费场景编码，用于匹配门诊、住院、手术批费等差异化规则
    /// </summary>
    public string? ChargeScene { get; set; }

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

    [SugarColumn(ColumnName = "RESULT_GROUP_NO", IsNullable = true)]
    /// <summary>
    /// 主子项目同组结果号，用于保证主项目和加收子项目原子处理
    /// </summary>
    public string? ResultGroupNo { get; set; }

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

    [SugarColumn(ColumnName = "INPUT_QTY", IsNullable = true)]
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    public decimal? InputQty { get; set; }

    [SugarColumn(ColumnName = "INPUT_UNIT", IsNullable = true)]
    /// <summary>
    /// 调用方录入数量的单位，例如 PART、CM2、EACH
    /// </summary>
    public string? InputUnit { get; set; }

    [SugarColumn(ColumnName = "BODY_PART_CODE", IsNullable = true)]
    /// <summary>
    /// 身体部位编码，用于分部位换算、部位差异规则和请求指纹
    /// </summary>
    public string? BodyPartCode { get; set; }

    [SugarColumn(ColumnName = "BUSINESS_CHARGE_TIME", IsNullable = true)]
    /// <summary>
    /// 业务收费发生时间，用于规则生效判断和滑动窗口累计
    /// </summary>
    public DateTime? BusinessChargeTime { get; set; }

    [SugarColumn(ColumnName = "PRICE_VERSION", IsNullable = true)]
    /// <summary>
    /// 权威价格版本或价格快照版本，用于金额追溯
    /// </summary>
    public string? PriceVersion { get; set; }

    [SugarColumn(ColumnName = "REQUEST_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 原始请求快照 JSON，用于幂等排查和事后复算
    /// </summary>
    public string? RequestJson { get; set; }

    [SugarColumn(ColumnName = "RESPONSE_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 计价响应快照 JSON，用于幂等重放和追溯展示
    /// </summary>
    public string? ResponseJson { get; set; }

    [SugarColumn(ColumnName = "REQUEST_AT")]
    /// <summary>
    /// 计价中心收到请求的技术时间
    /// </summary>
    public DateTime RequestAt { get; set; }

    [SugarColumn(ColumnName = "RESPONSE_AT", IsNullable = true)]
    /// <summary>
    /// 计价中心完成响应或状态更新的技术时间
    /// </summary>
    public DateTime? ResponseAt { get; set; }

    [SugarColumn(ColumnName = "IS_SUCCESS")]
    /// <summary>
    /// 请求是否成功落入计价中心处理链路，Y 表示成功
    /// </summary>
    public string IsSuccess { get; set; } = "N";

    [SugarColumn(ColumnName = "ERROR_MESSAGE", IsNullable = true)]
    /// <summary>
    /// 请求失败时记录的错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
}
