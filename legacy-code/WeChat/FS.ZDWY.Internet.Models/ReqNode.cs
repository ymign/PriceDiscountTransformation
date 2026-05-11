using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    public class ReqNode
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 节点值
        /// </summary>
        public string NodeValue { get; set; }
        /// <summary>
        /// 节点说明
        /// </summary>
        public string NodeInstruction { get; set; }
        /// <summary>
        /// 是否必须
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
