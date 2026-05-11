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
    /// 表名注释：拆零入库主表
    /// 数据表名：yb_trace_inbound_order
    /// 生成时间：2025-08-25 16:51:17
    /// </summary>
    public class YbTraceInboundOrder
    {
        /// <summary>
        /// 字段说明:唯一主键
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 字段说明:药房编码
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:否
        /// 字段名称:drug_dept_code
        /// </summary>
        public string DrugDeptCode { get; set; }

        /// <summary>
        /// 字段说明:药房名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:drug_dept_name
        /// </summary>
        public string DrugDeptName { get; set; }

        /// <summary>
        /// 字段说明:入库单号
        /// 数据类型:VARCHAR2
        /// 字段长度:64
        /// 是否可空:否
        /// 字段名称:inbound_no
        /// </summary>
        public string InboundNo { get; set; }

        /// <summary>
        /// 字段说明:供应商ID
        /// 数据类型:VARCHAR2
        /// 字段长度:18
        /// 是否可空:是
        /// 字段名称:supplier_id
        /// </summary>
        public string SupplierId { get; set; }

        /// <summary>
        /// 字段说明:供应商编码
        /// 数据类型:VARCHAR2
        /// 字段长度:64
        /// 是否可空:是
        /// 字段名称:supplier_code
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 字段说明:供应商名称
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:supplier_name
        /// </summary>
        public string SupplierName { get; set; }

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
        /// 字段说明:药品规格
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:drug_specs
        /// </summary>
        public string DrugSpecs { get; set; }

        /// <summary>
        /// 字段说明:药品自定义码
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_custom_code
        /// </summary>
        public string DrugCustomCode { get; set; }

        /// <summary>
        /// 字段说明:药品包装单位
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_pact_unit
        /// </summary>
        public string DrugPactUnit { get; set; }

        /// <summary>
        /// 字段说明:药品包装数量
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_pact_qty
        /// </summary>
        public string DrugPactQty { get; set; }

        /// <summary>
        /// 字段说明:药品最小单位
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_min_unit
        /// </summary>
        public string DrugMinUnit { get; set; }

        /// <summary>
        /// 字段说明:批次号
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:bch_no
        /// </summary>
        public string BchNo { get; set; }

        /// <summary>
        /// 字段说明:生产批号
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:manu_lotnum
        /// </summary>
        public string ManuLotnum { get; set; }

        /// <summary>
        /// 字段说明:生产日期
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:manu_date
        /// </summary>
        public DateTime ManuDate { get; set; }

        /// <summary>
        /// 字段说明:有效期止
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:expy_end
        /// </summary>
        public DateTime ExpyEnd { get; set; }

        /// <summary>
        /// 字段说明:原始包装追溯码
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:original_trace_code
        /// </summary>
        public string OriginalTraceCode { get; set; }

        /// <summary>
        /// 字段说明:原始数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:original_qty
        /// </summary>
        public decimal OriginalQty { get; set; }

        /// <summary>
        /// 字段说明:拆零数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:split_qty
        /// </summary>
        public decimal SplitQty { get; set; }

        /// <summary>
        /// 字段说明:状态
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 字段说明:来源
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:source_type
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 字段说明:入库客户端IP
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:inbound_client_ip
        /// </summary>
        public string InboundClientIp { get; set; }

        /// <summary>
        /// 字段说明:入库操作员工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:inbound_oper_code
        /// </summary>
        public string InboundOperCode { get; set; }

        /// <summary>
        /// 字段说明:入库操作员名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:inbound_oper_name
        /// </summary>
        public string InboundOperName { get; set; }

        /// <summary>
        /// 字段说明:入库时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:inbound_oper_time
        /// </summary>
        public DateTime InboundOperTime { get; set; }

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
