using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Account
{
    public class ZFPTOrder
    {

        /// <summary>
        /// 订单号
        /// </summary>
        public string payorderId { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string payMode { get; set; }

        /// <summary>
        /// 订单类型 1:挂号 2：门诊缴费 3：住院押金
        /// </summary>
        public string orderType { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PAYAMT { get; set; }

        /// <summary>
        /// 已退金额
        /// </summary>           
        public decimal RETURNEDATM { get; set; }

        /// <summary>
        /// 操作员工号
        /// </summary>
        public string operCode { get; set; }

        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string operName { get; set; }
    }

}
