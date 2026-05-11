using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy.Common
{
    public class DrugCodeMapping
    {
        /// <summary>
        /// 数据主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 药品编码
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 药品标识码 (追溯码前七位)
        /// </summary>
        public string IdentifierCode { get; set; }

        /// <summary>
        /// 有效标志 0无效 1有效
        /// </summary>
        public string ValidFlag { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public string OpterTime { get; set; }

        /// <summary>
        /// 操作人工号
        /// </summary>
        public string OpterCode { get; set; }

        /// <summary>
        /// 操作人名称
        /// </summary>
        public string OpterName { get; set; }
    }
}
