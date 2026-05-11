using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    /// 业务类型
    /// </summary>
    public static class BusinessTypeEnum
    {
        /// <summary>
        /// 门诊
        /// </summary>
        public const string MZ = "0";

        /// <summary>
        /// 住院
        /// </summary>
        public const string ZY = "1";


        /// <summary>获取描述</summary>
        public static string GetDescription(string code)
        {
            switch (code)
            {
                case MZ: return "门诊";
                case ZY: return "住院";
                default: return "未知业务类型";
            }
        }

        /// <summary>判断码值是否合法</summary>
        public static bool IsValid(string code)
        {
            return code == MZ
                || code == ZY
                ;
        }
    }
}
