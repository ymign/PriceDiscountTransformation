using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：追溯码操作记录表(存储追溯码的操作历史记录,一个子追溯码可以有多条记录，记录所有操作历史)
    /// 数据表名：yb_trace_code_record
    /// 生成时间：2025-09-09 16:59:27
    /// </summary>
    public class YbTraceCodeRecord
    {
        /// <summary>
        /// 字段说明:唯一主键ID
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 字段说明:药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:64
        /// 是否可空:否
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品名称
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:否
        /// 字段名称:drug_name
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 字段说明:父级追溯码
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:parent_trace_code
        /// </summary>
        public string ParentTraceCode { get; set; }

        /// <summary>
        /// 字段说明:子级追溯码
        /// 数据类型:VARCHAR2
        /// 字段长度:120
        /// 是否可空:是
        /// 字段名称:child_trace_code
        /// </summary>
        public string ChildTraceCode { get; set; }

        /// <summary>
        /// 字段说明:子级序号
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:sequence_no
        /// </summary>
        public decimal SequenceNo { get; set; }

        /// <summary>
        /// 字段说明:关联单据号
        /// 数据类型:VARCHAR2
        /// 字段长度:64
        /// 是否可空:是
        /// 字段名称:related_order_no
        /// </summary>
        public string RelatedOrderNo { get; set; }

        /// <summary>
        /// 字段说明:关键ID
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:related_id
        /// </summary>
        public string RelatedId { get; set; }

        /// <summary>
        /// 字段说明:关联表名称
        /// 数据类型:VARCHAR2
        /// 字段长度:64
        /// 是否可空:是
        /// 字段名称:related_table_name
        /// </summary>
        public string RelatedTableName { get; set; }

        /// <summary>
        /// 字段说明:业务场景 0门诊配药 1门诊发药 2门诊直接发药 3门诊隔天发药 4门诊退药审核 5住院发药 6住院出院带药 7住院退药 8拆零入库
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:business_scenario
        /// </summary>
        public string BusinessScenario { get; set; }

        /// <summary>
        /// 字段说明:操作类型 0拆零入库 1出库预扣 2出库成功 3退货入库 4直接退库
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:否
        /// 字段名称:operation_type
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 字段说明:操作时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:否
        /// 字段名称:operation_time
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 字段说明:操作描述
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:operation_description
        /// </summary>
        public string OperationDescription { get; set; }

        /// <summary>
        /// 字段说明:操作JSON记录
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:operation_json
        /// </summary>
        public string OperationJson { get; set; }

        /// <summary>
        /// 字段说明:创建人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:created_code
        /// </summary>
        public string CreatedCode { get; set; }

        /// <summary>
        /// 字段说明:创建人名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:created_name
        /// </summary>
        public string CreatedName { get; set; }

        /// <summary>
        /// 字段说明:创建时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:create_time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 字段说明:修改人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:modified_code
        /// </summary>
        public string ModifiedCode { get; set; }

        /// <summary>
        /// 字段说明:修改人名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:modified_name
        /// </summary>
        public string ModifiedName { get; set; }

        /// <summary>
        /// 字段说明:修改时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:modified_time
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 字段说明:删除标记 N未删除 Y已删除
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_deleted
        /// </summary>
        public string IsDeleted { get; set; }

        /// <summary>
        /// 字段说明:有效标记 N无效 Y有效
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_valid
        /// </summary>
        public string IsValid { get; set; }

        /// <summary>
        /// 字段说明:备注
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// 字段名称:memo
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 字段说明:拓展1
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:backup_1
        /// </summary>
        public string Backup1 { get; set; }

        /// <summary>
        /// 字段说明:拓展2
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:backup_2
        /// </summary>
        public string Backup2 { get; set; }

        /// <summary>
        /// 字段说明:拓展3
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:backup_3
        /// </summary>
        public string Backup3 { get; set; }

    }
}
