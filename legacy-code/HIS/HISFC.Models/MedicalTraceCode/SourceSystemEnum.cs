using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    /// 数据来源
    /// </summary>
    public static class SourceSystemEnum
    {
        /// <summary>
        /// HIS系统
        /// </summary>
        public const string HIS = "0"; 

        /// <summary>
        /// 智慧园
        /// </summary>
        public const string ZHY = "1"; 


        /// <summary>获取描述</summary>
        public static string GetDescription(string code)
        {
            switch (code)
            {
                case HIS: return "HIS系统";
                case ZHY: return "智慧园";
              
                default: return "未知来源";
            }
        }

        /// <summary>判断码值是否合法</summary>
        public static bool IsValid(string code)
        {
            return code == HIS
                || code == ZHY
                ;
        }
    }
}
