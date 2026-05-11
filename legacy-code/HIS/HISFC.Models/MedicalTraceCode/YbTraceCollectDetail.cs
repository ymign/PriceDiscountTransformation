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
    /// 表名注释：追溯码采集明细表-存储每个追溯码的具体采集信息，与主表yb_trace_collect_main形成一对多关系
    /// 数据表名：yb_trace_collect_detail
    /// 生成时间：2025-08-07 15:02:38
    /// </summary>
    public class YbTraceCollectDetail
    {
        /// <summary>
        /// 字段说明:主键ID-全局唯一标识，用于唯一标识每条追溯码采集明细记录
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 字段说明:主表关联ID-外键关联yb_trace_collect_main.id，建立主明细关系
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:main_id
        /// </summary>
        public string MainId { get; set; }

        /// <summary>
        /// 字段说明:申请流水号[pha_com_applyout表中主键]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:apply_number
        /// </summary>
        public string ApplyNumber { get; set; }

        /// <summary>
        /// 字段说明:追溯码 药品包装上的唯一标识码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:trace_code
        /// </summary>
        public string TraceCode { get; set; }

        /// <summary>
        /// 字段说明:追溯码类型 0包装  1拆零
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:trace_code_type
        /// </summary>
        public string TraceCodeType { get; set; }

        /// <summary>
        /// 字段说明:追溯码来源-SCAN:扫描获取，MANUAL:手工录入，IMPORT:批量导入
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:trace_code_source
        /// </summary>
        public string TraceCodeSource { get; set; }

        /// <summary>
        /// 字段说明:追溯码格式-CODE128:条形码，QR:二维码，DATA_MATRIX:矩阵码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:trace_code_format
        /// </summary>
        public string TraceCodeFormat { get; set; }

        /// <summary>
        /// 字段说明:采集顺序-在同一主记录中的采集序号，从1开始递增
        /// 数据类型:NUMBER
        /// 字段长度:5
        /// 是否可空:否
        /// 字段名称:collect_sequence
        /// </summary>
        public decimal CollectSequence { get; set; }

        /// <summary>
        /// 字段说明:采集时间戳-记录具体的追溯码采集时间，精确到秒级
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:否
        /// 字段名称:collect_timestamp
        /// </summary>
        public DateTime CollectTimestamp { get; set; }

        /// <summary>
        /// 字段说明:药品编码-冗余字段，便于直接查询药品信息，来源于主表
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品名称-冗余字段，便于直接显示药品名称，来源于主表
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:drug_name
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 字段说明:创建人工号-记录创建该明细记录的操作人员工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:created_code
        /// </summary>
        public string CreatedCode { get; set; }

        /// <summary>
        /// 字段说明:创建人姓名-记录创建该明细记录的操作人员姓名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:created_name
        /// </summary>
        public string CreatedName { get; set; }

        /// <summary>
        /// 字段说明:创建时间-记录该明细记录的创建时间，默认为系统当前时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:create_time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 字段说明:最后修改人工号-记录最后一次修改该明细记录的人员工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:modified_by
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 字段说明:最后修改人姓名-记录最后一次修改该明细记录的人员姓名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:modified_name
        /// </summary>
        public string ModifiedName { get; set; }

        /// <summary>
        /// 字段说明:最后修改时间-记录该明细记录的最后修改时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:modified_time
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 字段说明:是否逻辑删除-Y:已删除，N:未删除，用于逻辑删除控制
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_deleted
        /// </summary>
        public string IsDeleted { get; set; }

        /// <summary>
        /// 字段说明:是否有效-Y:有效，N:无效，用于数据有效性控制
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_valid
        /// </summary>
        public string IsValid { get; set; }

        /// <summary>
        /// 字段说明:数据版本号-记录数据的版本，每次更新时自动加1，用于并发控制
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:data_version
        /// </summary>
        public decimal DataVersion { get; set; }

        /// <summary>
        /// 字段说明:备注信息-记录该明细记录的补充说明信息，最多1000字符
        /// 数据类型:VARCHAR2
        /// 字段长度:1000
        /// 是否可空:是
        /// 字段名称:memo
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 字段说明:扩展字段1-预留扩展字段，可根据业务需要存储额外信息
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext_field1
        /// </summary>
        public string ExtField1 { get; set; }

        /// <summary>
        /// 字段说明:扩展字段2-预留扩展字段，可根据业务需要存储额外信息
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext_field2
        /// </summary>
        public string ExtField2 { get; set; }

        /// <summary>
        /// 字段说明:扩展字段3-预留扩展字段，可根据业务需要存储额外信息
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext_field3
        /// </summary>
        public string ExtField3 { get; set; }

        /// <summary>
        /// 字段说明:扩展字段4-预留扩展字段，可根据业务需要存储额外信息
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext_field4
        /// </summary>
        public string ExtField4 { get; set; }

        /// <summary>
        /// 字段说明:扩展字段5-预留扩展字段，可根据业务需要存储额外信息
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext_field5
        /// </summary>
        public string ExtField5 { get; set; }

    }
}
