using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views.OutPatient
{
    public class PayResult
    {
        /// <summary>
        /// 医院支付单号
        /// </summary>
        public string HospTradeId { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// 收据号
        /// </summary>
        public string ReceiptId { get; set; }

        /// <summary>
        /// 医院就诊地址
        /// </summary>
        public string VisitAddress { get; set; }

        /// <summary>
        /// 就诊序号
        /// </summary>
        public string VisitNo { get; set; }

        /// <summary>
        /// 取号凭证
        /// </summary>
        public string Proof { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 挂号流水号
        /// </summary>
        public string ClinicCode { get; set; }
    }
}
