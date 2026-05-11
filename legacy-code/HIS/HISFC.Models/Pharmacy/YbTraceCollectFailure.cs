using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：追溯码采集失败记录表
    /// 数据表名：yb_trace_collect_failure
    /// 生成时间：2025-07-09 22:10:51
    /// </summary>
    public class YbTraceCollectFailure
    {
        /// <summary>
        /// 字段说明:主键ID
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 字段说明:主表关联ID，外键关联yb_trace_collect_main.id
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// </summary>
        public string MainId { get; set; }

        /// <summary>
        /// 字段说明:申请流水号[pha_com_applyout表中主键]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// </summary>
        public string ApplyNumber { get; set; }

        /// <summary>
        /// 字段说明:药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品名称
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:否
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 字段说明:失败发生时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:否
        /// </summary>
        public DateTime FailureTime { get; set; }

        /// <summary>
        /// 字段说明:预期采集数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// </summary>
        public decimal ExpectedQty { get; set; }

        /// <summary>
        /// 字段说明:实际采集数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// </summary>
        public decimal ActualQty { get; set; }

        /// <summary>
        /// 字段说明:失败数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// </summary>
        public decimal FailedQty { get; set; }

        /// <summary>
        /// 字段说明:失败的追溯码列表（如果部分已知）
        /// 数据类型:VARCHAR2
        /// 字段长度:2000
        /// 是否可空:是
        /// </summary>
        public string FailedTraceCodes { get; set; }

        /// <summary>
        /// 字段说明:失败原因代码（标准化）
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:否
        /// </summary>
        public string FailureReasonCode { get; set; }

        /// <summary>
        /// 字段说明:失败原因详细描述
        /// 数据类型:VARCHAR2
        /// 字段长度:1000
        /// 是否可空:否
        /// </summary>
        public string FailureReasonDesc { get; set; }

        /// <summary>
        /// 字段说明:失败发生位置
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// </summary>
        public string FailureLocation { get; set; }

        /// <summary>
        /// 字段说明:操作员工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// </summary>
        public string OperatorCode { get; set; }

        /// <summary>
        /// 字段说明:操作员姓名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 字段说明:操作员部门编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// </summary>
        public string OperatorDeptCode { get; set; }

        /// <summary>
        /// 字段说明:操作员部门名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// </summary>
        public string OperatorDeptName { get; set; }

        /// <summary>
        /// 字段说明:处理决策：CONTINUE-继续发药，RETRY-重新采集，REJECT-拒绝发药
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:否
        /// </summary>
        public string HandleDecision { get; set; }

        /// <summary>
        /// 字段说明:处理决策理由
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// </summary>
        public string HandleReason { get; set; }

        /// <summary>
        /// 字段说明:处理决策时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:否
        /// </summary>
        public DateTime HandleTime { get; set; }

        /// <summary>
        /// 字段说明:是否允许继续发药：Y-允许，N-不允许
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// </summary>
        public string AllowDispense { get; set; }

        /// <summary>
        /// 字段说明:发药条件或要求
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// </summary>
        public string DispenseCondition { get; set; }

        /// <summary>
        /// 字段说明:风险等级：HIGH-高风险，MEDIUM-中风险，LOW-低风险
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// </summary>
        public string RiskLevel { get; set; }

        /// <summary>
        /// 字段说明:是否需要上级审批：Y-需要，N-不需要
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// </summary>
        public string NeedApproval { get; set; }

        /// <summary>
        /// 字段说明:审批人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// </summary>
        public string ApproverCode { get; set; }

        /// <summary>
        /// 字段说明:审批人姓名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// 字段说明:审批时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// </summary>
        public DateTime ApprovalTime { get; set; }

        /// <summary>
        /// 字段说明:审批意见
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// </summary>
        public string ApprovalComment { get; set; }

        /// <summary>
        /// 字段说明:审批状态：APPROVED-已批准，REJECTED-已拒绝，PENDING-待审批
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// </summary>
        public string ApprovalStatus { get; set; }

        /// <summary>
        /// 字段说明:补救措施：MANUAL_RECORD-手工记录，PHOTO_RECORD-拍照记录，OTHER-其他
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// </summary>
        public string RemedyAction { get; set; }

        /// <summary>
        /// 字段说明:补救措施描述
        /// 数据类型:VARCHAR2
        /// 字段长度:1000
        /// 是否可空:是
        /// </summary>
        public string RemedyDesc { get; set; }

        /// <summary>
        /// 字段说明:补救措施附件路径
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// </summary>
        public string RemedyAttachment { get; set; }

        /// <summary>
        /// 字段说明:是否已发送通知：Y-已发送，N-未发送
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// </summary>
        public string NotificationSent { get; set; }

        /// <summary>
        /// 字段说明:通知对象
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// </summary>
        public string NotificationTarget { get; set; }

        /// <summary>
        /// 字段说明:通知时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// </summary>
        public DateTime NotificationTime { get; set; }

        /// <summary>
        /// 字段说明:通知方式：SMS-短信，EMAIL-邮件，SYSTEM-系统内
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// </summary>
        public string NotificationMethod { get; set; }

        /// <summary>
        /// 字段说明:创建人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 字段说明:创建人姓名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// </summary>
        public string CreatedName { get; set; }

        /// <summary>
        /// 字段说明:创建时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 字段说明:最后修改人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 字段说明:最后修改人姓名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// </summary>
        public string ModifiedName { get; set; }

        /// <summary>
        /// 字段说明:最后修改时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 字段说明:是否逻辑删除
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// </summary>
        public string IsDeleted { get; set; }

        /// <summary>
        /// 字段说明:是否有效
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// </summary>
        public string IsValid { get; set; }

        /// <summary>
        /// 字段说明:备注信息
        /// 数据类型:VARCHAR2
        /// 字段长度:1000
        /// 是否可空:是
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 字段说明:扩展字段1
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// </summary>
        public string ExtField1 { get; set; }

        /// <summary>
        /// 字段说明:扩展字段2
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// </summary>
        public string ExtField2 { get; set; }

        /// <summary>
        /// 字段说明:扩展字段3
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// </summary>
        public string ExtField3 { get; set; }

    }
}
