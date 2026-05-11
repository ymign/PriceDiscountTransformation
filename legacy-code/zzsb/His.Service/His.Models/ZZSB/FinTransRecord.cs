using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class FinTransRecord
    {
        public string ApplicationOrderNo { get; set; }
        public string ApplicationRefundOrderNo { get; set; }
        public string ClientCode { get; set; }
        public string CreatedCode { get; set; }
        public string CreatedName { get; set; }
        public DateTime CreatedTime { get; set; }
        public string HospitalCode { get; set; }
        public string Id { get; set; }
        public string OrderBigType { get; set; }
        public string OrderSmallType { get; set; }
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PayChannelCode { get; set; }
        public DateTime PayTransFinishTime { get; set; }
        public string PlatformOrderNo { get; set; }
        public string PlatformRefundOrderNo { get; set; }
        public DateTime RefundTransFinishTime { get; set; }
        public string TransactionNo { get; set; }
        public decimal TransAmount { get; set; }
        public string TransType { get; set; }
        /// <summary>
        /// 业务流水号  比如门诊流水号或者住院流水号
        /// </summary>
        public string BusinessNo { get; set; }
    }
}
