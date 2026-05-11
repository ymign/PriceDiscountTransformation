using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB.MedicalModel
{
    public class MedicalBase
    {
        /// <summary>
        /// 操作员编号
        /// </summary>
        public string UserID { get; set; }


        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceID { get; set; }


        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode { get; set; }


        /// <summary>
        /// 业务编号
        /// </summary>
        public string FunCode { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public string ReqTime { get; set; }


        /// <summary>
        /// 请求流水号
        /// </summary>
        public string ReqTraceNo { get; set; }
    }
}
