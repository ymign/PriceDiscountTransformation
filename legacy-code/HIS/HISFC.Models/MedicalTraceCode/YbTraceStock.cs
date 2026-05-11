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
    /// 表名注释：药品库存表
    /// 数据表名：yb_trace_stock
    /// 生成时间：2025-08-25 16:53:53
    /// </summary>
    public class YbTraceStock
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
        /// 字段说明:总数量
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
        /// 字段说明:预扣数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:prededucted_qty
        /// </summary>
        public decimal PreDeductedQty { get; set; }

        /// <summary>
        /// 字段说明:过期数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:expired_qty
        /// </summary>
        public decimal ExpiredQty { get; set; }

        /// <summary>
        /// 字段说明:损坏数量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:damaged_qty
        /// </summary>
        public decimal DamagedQty { get; set; }

        /// <summary>
        /// 字段说明:首次入库时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:first_inbound_time
        /// </summary>
        public DateTime FirstInboundTime { get; set; }

        /// <summary>
        /// 字段说明:最后入库时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:last_inbound_time
        /// </summary>
        public DateTime LastInboundTime { get; set; }

        /// <summary>
        /// 字段说明:最后出库时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:last_outbound_time
        /// </summary>
        public DateTime LastOutboundTime { get; set; }

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



        #region 非表中字段

        /// <summary>
        /// 本次入库数量 非表中字段
        /// </summary>
        public decimal InBoundQty { get; set; } 

        #endregion

    }
}
