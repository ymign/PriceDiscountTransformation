using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 来源于平台的挂号订单信息
    /// </summary>
    public class PLATFORM_REGISTER_PAY
    {
        #region 入参
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
        /// 支付方式
        /// </summary>
        public System.String PAYMODE { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public System.String PAYAMT { get; set; }

        /// <summary>
        ///  收单机构流水号
        /// </summary>
        public System.String TRANSACTIONNO { get; set; }

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
        /// 诊疗卡类型
        /// </summary>
        public System.String CARDTYPE { get; set; }

        /// <summary>
        /// 诊疗卡号码
        /// </summary>
        public System.String CARDNO { get; set; }

        /// <summary>
        /// 医保账户
        /// </summary>
        public System.String MEDICALINSURANCEID { get; set; }

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
        /// 优惠金额
        /// </summary>
        public System.Decimal ECOSTAMOUNT { get; set; }

        public System.String OPERCODE { get; set; }

        public System.String OPERNAME { get; set; }

        /// <summary>
        /// 许可登记号
        /// </summary>
        public string TransNo { get; set; }

        /// <summary>
        /// 许可登记号
        /// </summary>
        public string CancelTransNo { get; set; }

        #endregion

        #region 退费记录数据

        /// <summary>
        /// 挂号流水号
        /// </summary>
        public System.String RegisterID { get; set; }

        /// <summary>
        /// 平台退款订单号
        /// </summary>
        public string PsRefOrdNum { get; set; }

        /// <summary>
        /// 医院支付单号
        /// </summary>
        public string HospTradeId { get; set; }

        /// <summary>
        /// 退费时间
        /// </summary>
        public DateTime PayRefTime { get; set; }

        /// <summary>
        /// 退费原因
        /// </summary>
        public string RefundReason { get; set; }

        /// <summary>
        /// 退费操作员
        /// </summary>
        public string RefundOpercode { get; set; }

        /// <summary>
        /// 退费操作名字
        /// </summary>
        public string RefundOpername { get; set; }

        #endregion
    }
}
