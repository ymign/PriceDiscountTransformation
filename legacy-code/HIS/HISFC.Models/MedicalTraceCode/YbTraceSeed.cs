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
    /// 表名注释：追溯码种子表(每个原始追溯码1条记录)，入库时同步生成
    /// 数据表名：yb_trace_seed
    /// 生成时间：2025-09-12 10:20:19
    /// </summary>
    public class YbTraceSeed
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
        /// 字段说明:入库单主键表[yb_trace_inbound_order]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:inbound_order_id
        /// </summary>
        public string InboundOrderId { get; set; }

        /// <summary>
        /// 字段说明:入库单号表[yb_trace_inbound_order]
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:inbound_order_no
        /// </summary>
        public string InboundOrderNo { get; set; }

        /// <summary>
        /// 字段说明:药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:否
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:drug_name
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 字段说明:药房编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
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
        /// 字段说明:包装单位
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_pack_unit
        /// </summary>
        public string DrugPackUnit { get; set; }

        /// <summary>
        /// 字段说明:包装数量
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_pack_qty
        /// </summary>
        public string DrugPackQty { get; set; }

        /// <summary>
        /// 字段说明:最小单位
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:drug_min_unit
        /// </summary>
        public string DrugMinUnit { get; set; }

        /// <summary>
        /// 字段说明:包装级别 0小包装 1中包装 2大包装
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:drug_pack_level
        /// </summary>
        public string DrugPackLevel { get; set; }

        /// <summary>
        /// 字段说明:批次号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:batch_no
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 字段说明:父级追溯码
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:parent_trace_code
        /// </summary>
        public string ParentTraceCode { get; set; }

        /// <summary>
        /// 字段说明:拆零数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:total_qty
        /// </summary>
        public decimal TotalQty { get; set; }

        /// <summary>
        /// 字段说明:可用数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:available_qty
        /// </summary>
        public decimal AvailableQty { get; set; }

        /// <summary>
        /// 字段说明:当前偏移量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:current_offset
        /// </summary>
        public decimal CurrentOffset { get; set; }

        /// <summary>
        /// 字段说明:供应商编号
        /// 数据类型:VARCHAR2
        /// 字段长度:20
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
        /// 字段说明:状态 0待使用 1使用中 2已用完 3直接退库
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:seed_status
        /// </summary>
        public string SeedStatus { get; set; }

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
