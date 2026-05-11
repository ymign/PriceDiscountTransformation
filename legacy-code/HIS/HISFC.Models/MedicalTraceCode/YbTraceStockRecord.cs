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
    /// 表名注释：
    /// 数据表名：yb_trace_stock_record
    /// 生成时间：2025-09-09 14:33:31
    /// </summary>
    public class YbTraceStockRecord
    {
        /// <summary>
        /// 字段说明:主键ID
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
        /// 是否可空:是
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品名称
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:drug_name
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 字段说明:药房编码
        /// 数据类型:VARCHAR2
        /// 字段长度:64
        /// 是否可空:是
        /// 字段名称:drug_dept_code
        /// </summary>
        public string DrugDeptCode { get; set; }

        /// <summary>
        /// 字段说明:药房名称
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:drug_dept_name
        /// </summary>
        public string DrugDeptName { get; set; }

        /// <summary>
        /// 字段说明:变更单号 触发器[TRG_YB_TRACE_STOCK_RECORD_BI]自动生成
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:change_no
        /// </summary>
        public string ChangeNo { get; set; }

        /// <summary>
        /// 字段说明:变更类型 0拆零入库 1预扣占用 2拆零出库 3调整增加 4调整减少 5过期 6损坏 7退货 8调拨入库 9调拨出库 10盘点调整
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:否
        /// 字段名称:change_type
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>
        /// 字段说明:变更前总数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:before_total_qty
        /// </summary>
        public decimal BeforeTotalQty { get; set; }

        /// <summary>
        /// 字段说明:变更前可用数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:before_available_qty
        /// </summary>
        public decimal BeforeAvailableQty { get; set; }

        /// <summary>
        /// 字段说明:变更前预扣占用数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:before_prededucted_qty
        /// </summary>
        public decimal BeforePredeductedQty { get; set; }

        /// <summary>
        /// 字段说明:变更前过期数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:before_expired_qty
        /// </summary>
        public decimal BeforeExpiredQty { get; set; }

        /// <summary>
        /// 字段说明:变更前损坏数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:before_damaged_qty
        /// </summary>
        public decimal BeforeDamagedQty { get; set; }

        /// <summary>
        /// 字段说明:变更后总数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:after_total_qty
        /// </summary>
        public decimal AfterTotalQty { get; set; }

        /// <summary>
        /// 字段说明:变更后可用数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:after_available_qty
        /// </summary>
        public decimal AfterAvailableQty { get; set; }

        /// <summary>
        /// 字段说明:变更后预扣占用数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:after_prededucted_qty
        /// </summary>
        public decimal AfterPredeductedQty { get; set; }

        /// <summary>
        /// 字段说明:变更后过期数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:after_expired_qty
        /// </summary>
        public decimal AfterExpiredQty { get; set; }

        /// <summary>
        /// 字段说明:变更后损坏数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:after_damaged_qty
        /// </summary>
        public decimal AfterDamagedQty { get; set; }

        /// <summary>
        /// 字段说明:关联表名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:related_table
        /// </summary>
        public string RelatedTable { get; set; }

        /// <summary>
        /// 字段说明:关联记录ID
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:related_id
        /// </summary>
        public string RelatedId { get; set; }

        /// <summary>
        /// 字段说明:关联单号(入库单号、出库单号等)
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:related_no
        /// </summary>
        public string RelatedNo { get; set; }

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
