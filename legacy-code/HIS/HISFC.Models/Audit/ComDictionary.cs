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
    /// 表名注释：公共_常数表
    /// 数据表名：com_dictionary
    /// 生成时间：2025-08-06 17:10:14
    /// </summary>
    public class ComDictionary
    {
        /// <summary>
        /// 字段说明:常数类型
        /// 数据类型:VARCHAR2
        /// 字段长度:40
        /// 是否可空:否
        /// 字段名称:type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 字段说明:编码
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:否
        /// 字段名称:code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 字段说明:名称
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段说明:备注
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:mark
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 字段说明:拼音码
        /// 数据类型:VARCHAR2
        /// 字段长度:40
        /// 是否可空:是
        /// 字段名称:spell_code
        /// </summary>
        public string SpellCode { get; set; }

        /// <summary>
        /// 字段说明:五笔
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:wb_code
        /// </summary>
        public string WbCode { get; set; }

        /// <summary>
        /// 字段说明:输入
        /// 数据类型:VARCHAR2
        /// 字段长度:80
        /// 是否可空:是
        /// 字段名称:input_code
        /// </summary>
        public string InputCode { get; set; }

        /// <summary>
        /// 字段说明:顺序号
        /// 数据类型:NUMBER
        /// 字段长度:38
        /// 是否可空:是
        /// 字段名称:sort_id
        /// </summary>
        public decimal SortId { get; set; }

        /// <summary>
        /// 字段说明:有效性标志 0 在用 1 停用 2 废弃
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:valid_state
        /// </summary>
        public string ValidState { get; set; }

        /// <summary>
        /// 字段说明:操作员
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:oper_code
        /// </summary>
        public string OperCode { get; set; }

        /// <summary>
        /// 字段说明:操作时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:oper_date
        /// </summary>
        public DateTime OperDate { get; set; }

        /// <summary>
        /// 字段说明:是否常用
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:is_common
        /// </summary>
        public string IsCommon { get; set; }

        /// <summary>
        /// 字段说明:kind_id
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:kind_id
        /// </summary>
        public string KindId { get; set; }

        /// <summary>
        /// 字段说明:父级医疗机构编码
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:否
        /// 字段名称:parent_code
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 字段说明:本机医疗机构编码
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:否
        /// 字段名称:current_code
        /// </summary>
        public string CurrentCode { get; set; }

    }
}
