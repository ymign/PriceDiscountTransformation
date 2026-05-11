using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    /// 采集类别
    /// </summary>
    public static class CollectTypeEnum
    {
        /// <summary>
        /// 销售
        /// </summary>
        public const string Sale = "0";

        /// <summary>
        /// 退货
        /// </summary>
        public const string ReturnOfGoods = "1";


        /// <summary>获取描述</summary>
        public static string GetDescription(string code)
        {
            switch (code)
            {
                case Sale: return "销售";
                case ReturnOfGoods: return "退货";
                default: return "未知采集类别";
            }
        }

        /// <summary>判断码值是否合法</summary>
        public static bool IsValid(string code)
        {
            return code == Sale
                || code == ReturnOfGoods
                ;
        }
    }
}
