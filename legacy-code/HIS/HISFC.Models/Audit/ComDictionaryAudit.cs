using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Audit
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：
    /// 数据表名：com_dictionary_audit
    /// 生成时间：2025-08-06 15:00:53
    /// </summary>
    public class ComDictionaryAudit
    {
        /// <summary>
        /// 字段说明:数据主键
        /// 数据类型:NUMBER
        /// 字段长度:38
        /// 是否可空:否
        /// 字段名称:audit_id
        /// </summary>
        public decimal AuditId { get; set; }

        /// <summary>
        /// 字段说明:操作类型: 0新增 1修改 2删除
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:action_type
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 字段说明:操作时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:action_date
        /// </summary>
        public DateTime ActionDate { get; set; }

        /// <summary>
        /// 字段说明:操作员编码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:action_user_code
        /// </summary>
        public string ActionUserCode { get; set; }

        /// <summary>
        /// 字段说明:操作员名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:action_user_name
        /// </summary>
        public string ActionUserName { get; set; }

        /// <summary>
        /// 字段说明:客户端IP地址
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:client_ip
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 字段说明:客户端主机名或MAC地址（可选）
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:client_hostname
        /// </summary>
        public string ClientHostname { get; set; }

        /// <summary>
        /// 字段说明:所属系统模块，如“基本信息维护”
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:system_module
        /// </summary>
        public string SystemModule { get; set; }

        /// <summary>
        /// 字段说明:操作来源: 0HIS 1手工 2API
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:source_app
        /// </summary>
        public string SourceApp { get; set; }

        /// <summary>
        /// 字段说明:主表type
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:dict_type
        /// </summary>
        public string DictType { get; set; }

        /// <summary>
        /// 字段说明:主表code
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:否
        /// 字段名称:dict_code
        /// </summary>
        public string DictCode { get; set; }

        /// <summary>
        /// 字段说明:字典中文名称(描述)
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:dict_name
        /// </summary>
        public string DictName { get; set; }

        /// <summary>
        /// 字段说明:修改前的数据快照 JSON格式
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:data_before_json
        /// </summary>
        public string DataBeforeJson { get; set; }

        /// <summary>
        /// 字段说明:修改后的数据快照 JSON格式
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:data_after_json
        /// </summary>
        public string DataAfterJson { get; set; }

        /// <summary>
        /// 字段说明:操作备注
        /// 数据类型:VARCHAR2
        /// 字段长度:1000
        /// 是否可空:是
        /// 字段名称:remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 字段说明:预留字段1
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext1
        /// </summary>
        public string Ext1 { get; set; }

        /// <summary>
        /// 字段说明:预留字段2
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext2
        /// </summary>
        public string Ext2 { get; set; }

        /// <summary>
        /// 字段说明:预留字段3
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:ext3
        /// </summary>
        public string Ext3 { get; set; }

    }
}
