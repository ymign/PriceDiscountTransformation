using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB.PayPlatform
{
    public class InvoiceBinding
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string payorderId { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>           
        public string invoiceNo { get; set; }

        /// <summary>
        /// 支付方式 1:信用付
        /// </summary>
        public string payMode { get; set; }
        /// <summary>
        /// 订单类型 1:挂号 2：门诊缴费 3：住院押金
        /// </summary>
        public string orderType { get; set; }
    }
}
