using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views
{
    public class QueryResult
    {
        /// <summary>
        /// 医院订单号
        /// </summary>
        public string HospitalNum { get; set; }

        /// <summary>
        /// 医院就诊地址
        /// </summary>
        public string VisitAddress { get; set; }

        /// <summary>
        /// 就诊序号
        /// </summary>
        public string VisitNo { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public string OrderTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public string PayTime { get; set; }

        /// <summary>
        /// 取号时间
        /// </summary>
        public string TakeTime { get; set; }

        /// <summary>
        /// 退号时间
        /// </summary>
        public string CancelTime { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public string RefundTime { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public string PayAmt { get; set; }

        /// <summary>
        /// 平台退款金额
        /// </summary>
        public string RefundFee { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
    }
}
