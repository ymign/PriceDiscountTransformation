using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    public class OperInfo
    {
        /// <summary>
        /// 操作人编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Time { get; set; }

    }
}
