using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views
{
    /// <summary>
    /// 预约挂号(锁定号源)时返回的结果信息
    /// </summary>
    public class OrderResult
    {
        /// <summary>
        /// 平台定单号   必须返回
        /// </summary>
        public string OrderId { get; set; }//平台定单号   必须返回
        /// <summary>
        /// 医院订单号   必须返回
        /// </summary>
        public string HospitalNum { get; set; }//医院订单号   必须返回
        /// <summary>
        /// 就诊序号    当日挂号 必须返回
        /// </summary>
        public string VisitNo { get; set; }//就诊序号    当日挂号 必须返回
        /// <summary>
        /// 就诊位置    当日挂号 必须返回 例：门诊大楼四楼外科一诊室
        /// </summary>
        public string VisitAddress { get; set; }//就诊位置    当日挂号 必须返回 例：门诊大楼四楼外科一诊室
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }//备注
        /// <summary>
        /// 取号凭证    当日挂号 必须返回
        /// </summary>
        public string Proof { get; set; }//取号凭证    当日挂号 必须返回

        public string RegFee { get; set; }

    }
}
