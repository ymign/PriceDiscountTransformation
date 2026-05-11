using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.SqlSugar
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：
    /// 数据表名：mnt_biz_adjust_log
    /// 生成时间：2026-01-05 15:48:17
    /// </summary>
    public class MntBizAdjustLog
    {
        /// <summary>
        /// 字段说明:log_id
        /// 数据类型:NUMBER
        /// 字段长度:19
        /// 是否可空:否
        /// 字段名称:log_id
        /// </summary>
        public decimal LogId { get; set; }

        /// <summary>
        /// 字段说明:biz_type
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:biz_type
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 字段说明:origin_pk
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:origin_pk
        /// </summary>
        public string OriginPk { get; set; }

        /// <summary>
        /// 字段说明:invoice_no
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:invoice_no
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 字段说明:patient_name
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:patient_name
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 字段说明:outpatient_id
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:outpatient_id
        /// </summary>
        public string OutpatientId { get; set; }

        /// <summary>
        /// 字段说明:id_card
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:id_card
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 字段说明:item_type
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:否
        /// 字段名称:item_type
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// 字段说明:old_value
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:old_value
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// 字段说明:new_value
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:new_value
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// 字段说明:oper_code
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:否
        /// 字段名称:oper_code
        /// </summary>
        public string OperCode { get; set; }

        /// <summary>
        /// 字段说明:oper_name
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:oper_name
        /// </summary>
        public string OperName { get; set; }

        /// <summary>
        /// 字段说明:oper_date
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:oper_date
        /// </summary>
        public DateTime OperDate { get; set; }

        /// <summary>
        /// 字段说明:oper_ip
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:oper_ip
        /// </summary>
        public string OperIp { get; set; }

        /// <summary>
        /// 字段说明:remark
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:remark
        /// </summary>
        public string Remark { get; set; }

        public string ChangeReason { get; set; }

    }
}
