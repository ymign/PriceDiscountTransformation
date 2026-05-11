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
    /// 数据表名：pha_com_codemapping
    /// 生成时间：2025-08-04 14:58:55
    /// </summary>
    public class PhaComCodemapping
    {
        /// <summary>
        /// 字段说明:id
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 字段说明:drug_code
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:identifier_code
        /// 数据类型:VARCHAR2
        /// 字段长度:7
        /// 是否可空:否
        /// 字段名称:identifier_code
        /// </summary>
        public string IdentifierCode { get; set; }

        /// <summary>
        /// 字段说明:valid_flag
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:valid_flag
        /// </summary>
        public string ValidFlag { get; set; }

        /// <summary>
        /// 字段说明:opter_time
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:opter_time
        /// </summary>
        public DateTime OpterTime { get; set; }

        /// <summary>
        /// 字段说明:opter_code
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:opter_code
        /// </summary>
        public string OpterCode { get; set; }

        /// <summary>
        /// 字段说明:opter_name
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:opter_name
        /// </summary>
        public string OpterName { get; set; }

    }
}
