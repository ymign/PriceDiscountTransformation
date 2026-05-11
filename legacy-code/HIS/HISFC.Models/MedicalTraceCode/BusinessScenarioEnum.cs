using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    /// 业务场景
    /// </summary>
    public static class BusinessScenarioEnum
    {
        /// <summary>
        /// 门诊配药
        /// </summary>
        public const string OutpatientPrepare = "0"; // 门诊配药

        /// <summary>
        /// 门诊发药
        /// </summary>
        public const string OutpatientDispense = "1"; // 门诊发药

        /// <summary>
        /// 门诊直接发药
        /// </summary>
        public const string OutpatientDirect = "2"; // 门诊直接发药

        /// <summary>
        /// 门诊隔天发药
        /// </summary>
        public const string OutpatientNextDay = "3"; // 门诊隔天发药

        /// <summary>
        /// 门诊退药审核
        /// </summary>
        public const string OutpatientReturnAudit = "4"; // 门诊退药审核

        /// <summary>
        /// 住院发药
        /// </summary>
        public const string InpatientDispense = "5"; // 住院发药

        /// <summary>
        /// 住院出院带药
        /// </summary>
        public const string InpatientDischarge = "6"; // 住院出院带药

        /// <summary>
        /// 住院退药
        /// </summary>
        public const string InpatientReturn = "7"; // 住院退药

        /// <summary>
        /// 拆零入库
        /// </summary>
        public const string SplitInbound = "8";

        /// <summary>获取描述</summary>
        public static string GetDescription(string code)
        {
            switch (code)
            {
                case OutpatientPrepare: return "门诊配药";
                case OutpatientDispense: return "门诊发药";
                case OutpatientDirect: return "门诊直接发药";
                case OutpatientNextDay: return "门诊隔天发药";
                case OutpatientReturnAudit: return "门诊退药审核";
                case InpatientDispense: return "住院发药";
                case InpatientDischarge: return "住院出院带药";
                case InpatientReturn: return "住院退药";
                case SplitInbound: return "拆零入库";
                default: return "未知场景";
            }
        }

        /// <summary>判断是否为合法场景</summary>
        public static bool IsValid(string code)
        {
            return code == OutpatientPrepare
                || code == OutpatientDispense
                || code == OutpatientDirect
                || code == OutpatientNextDay
                || code == OutpatientReturnAudit
                || code == InpatientDispense
                || code == InpatientDischarge
                || code == InpatientReturn
                || code == SplitInbound;
        }

    }
}
