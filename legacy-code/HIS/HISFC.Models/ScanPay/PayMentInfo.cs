using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.ScanPay
{
    /// <summary>
    /// 扫码墩 扫码支付交易信息
    /// </summary>
    public class PayMentInfo
    {
        public PayMentInfo()
        { }
        /// <summary>
        /// His订单号
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 订单类型 1当天挂号 2预约挂号 3门诊缴费 4住院按金 5门诊预交金充值 6住院预交金充值
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 金额（元）
        /// </summary>
        public string fee { get; set; }
        /// <summary>
        /// 付款码，支持微信、支付宝，需与pay_type入参对应
        /// </summary>
        public string pay_code { get; set; }
        /// <summary>
        /// 患者ID
        /// </summary>
        public string patient_id { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string patient_name { get; set; }
        /// <summary>
        /// 支付类型。微信：3；支付宝：4
        /// </summary>
        public string pay_type { get; set; }
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 充值时的商户订单号
        /// </summary>
        public string data_order_id { get; set; }
        /// <summary>
        /// 订单流水号
        /// </summary>
        public string transaction_id { get; set; }
    }
}
