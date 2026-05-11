using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    /// 追溯码业务常量定义
    /// </summary>
    public static class TraceCodeConstants
    {
        /// <summary>
        /// 需要采集追溯码标识
        /// </summary>
        public const string NEED_COLLECT_FLAG = "1";

        /// <summary>
        /// 不需要采集追溯码标识
        /// </summary>
        public const string NOT_NEED_COLLECT_FLAG = "0";

        /// <summary>
        /// 注射剂剂型代码
        /// </summary>
        public const string INJECTION_DOSAGE_FORM = "01";

        /// <summary>
        /// 胰岛素类药物三级药理代码
        /// </summary>
        public const string INSULIN_PHY_FUNCTION = "11603";

        /// <summary>
        /// 中草药类型代码
        /// </summary>
        public const string CHINESE_HERB_TYPE = "C";

        /// <summary>
        /// 拆零采集开关参数名
        /// </summary>
        public const string SPLIT_COLLECT_SWITCH = "TraceCodeSplit";
        
        /// <summary>
        /// 无码目录常量类型
        /// </summary>
        public const string NO_TRACE_CODE_CONST_TYPE = "DrugTracCodgFreeSacn";
       
        /// <summary>
        /// 缓存过期时间（分钟）
        /// </summary>
        public const int CACHE_TTL_MINUTES = 100;
    }
}
