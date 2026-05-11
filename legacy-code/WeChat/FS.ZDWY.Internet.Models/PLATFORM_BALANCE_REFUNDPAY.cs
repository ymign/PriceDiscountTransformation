using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    public class PLATFORM_BALANCE_REFUNDPAY
    {
        /// <summary>
        /// 平台订单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String ORDERID { get; set; }

        /// <summary>
        /// 医院订单号
        /// </summary>
        public System.String HOSPITALNUM { get; set; }

        /// <summary>
        /// 就诊号
        /// </summary>
        public System.String VISITNO { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public System.String PAYMODE { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public System.String PAYAMT { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public System.DateTime PAYTIME { get; set; }

        /// <summary>
        /// 院内用户ID
        /// </summary>
        public System.String PATIENTID { get; set; }

        /// <summary>
        /// 用户证件类型
        /// </summary>
        public System.String CERTIFCATETYPE { get; set; }

        /// <summary>
        /// 用户证件号码
        /// </summary>
        public System.String CERTIFCATENO { get; set; }

        /// <summary>
        /// 病历号
        /// </summary>
        public System.String MEDICALNO { get; set; }

        /// <summary>
        /// 用户卡类型
        /// </summary>
        public System.String CARDTYPE { get; set; }

        /// <summary>
        /// 用户卡号
        /// </summary>
        public System.String CARDNO { get; set; }

        /// <summary>
        /// 第三方服务商 ID
        /// </summary>
        public System.String FRONTPROVIDERID { get; set; }

        /// <summary>
        /// 医保减免
        /// </summary>
        public System.Decimal HCAREAMOUNT { get; set; }

        /// <summary>
        /// 自费金额
        /// </summary>
        public System.Decimal SELFAMOUNT { get; set; }

        /// <summary>
        /// 医保报销
        /// </summary>
        public System.Decimal EXPENSEAMOUNT { get; set; }

        /// <summary>
        /// 费用总金额
        /// </summary>
        public System.Decimal TOTALAMOUNT { get; set; }

        /// <summary>
        /// 医院支付单号
        /// </summary>
        public System.String HOSPTRADEID { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public System.String INVOICEID { get; set; }

        /// <summary>
        /// 收据号
        /// </summary>
        public System.String RECEIPTID { get; set; }

        /// <summary>
        /// 医院就诊地址
        /// </summary>
        public System.String VISITADDRESS { get; set; }

        /// <summary>
        /// 排队列号
        /// </summary>
        public System.String SEQUENCENO { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public System.String REMARK { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public System.String STATUS { get; set; }

        /// <summary>
        /// 收单机构流水号
        /// </summary>
        public System.String TRANSACTIONNO { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public System.DateTime REFUNDTIME { get; set; }

        /// <summary>
        /// 医保交易许可证号
        /// </summary>
        public System.String TRANNO { get; set; }

        /// <summary>
        /// 医保交易许可证号
        /// </summary>
        public System.String CANCELTRANNO { get; set; }

        /// <summary>
        /// 业务系统退款单号 
        /// </summary>
        public System.String REFUNDID { get; set; }

        public System.String OPERCODE { get; set; }
        public System.String OPERNAME { get; set; }
        public System.DateTime OPERTIME { get; set; }
    }
}
