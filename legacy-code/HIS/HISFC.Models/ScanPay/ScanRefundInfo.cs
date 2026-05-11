using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.ScanPay
{
    public class ScanRefundInfo
    {
        public ScanRefundInfo()
        { }
        /// <summary>
        /// 订单号
        /// </summary>
        public string ORDER_ID { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string refund_fee { get; set; }
        /// <summary>
        /// HIS订单号
        /// </summary>
        public string refund_order_id { get; set; }
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string MSG { get; set; }
        /// <summary>
        /// 退款订单号
        /// </summary>
        public string date_refund_order_id { get; set; }
        /// <summary>
        /// 微信/支付宝平台退款订单号
        /// </summary>
        public string refund_transaction_id { get; set; }
        /// <summary>
        /// 退款类型 0异常订单退款
        /// </summary>
        public string REFUNDTYPE { get; set; }
        /// <summary>
        /// 操作员工号
        /// </summary>
        public string OPERUserID { get; set; }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string Patient_Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Patient_Name { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public string Payment_At { get; set; }
    }
}
