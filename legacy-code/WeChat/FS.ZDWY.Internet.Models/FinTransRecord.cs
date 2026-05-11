using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 支付平台订单记录表
    /// </summary>
    [SugarTable("fin_trans_record", "HIS支付平台交易记录表")]
    public class FinTransRecord
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [SugarColumn(ColumnName = "TRANS_TYPE")]
        public string TransType { get; set; }

        /// <summary>
        /// 业务流水号
        /// </summary>
        [SugarColumn(ColumnName = "businessno")]
        public string Businessno { get; set; }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        [SugarColumn(ColumnName = "PATIENT_NO")]
        public string PatientNo { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        [SugarColumn(ColumnName = "PATIENT_NAME")]
        public string PatientName { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        [SugarColumn(ColumnName = "CLIENT_CODE")]
        public string ClientCode { get; set; }

        /// <summary>
        /// 交易单据号(比如his发票号)
        /// </summary>
        [SugarColumn(ColumnName = "TRANSACTIONNO")]
        public string TransactionNo { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        [SugarColumn(ColumnName = "PLATFORM_ORDER_NO")]
        public string PlatformOrderNo { get; set; }

        /// <summary>
        /// 应用系统退款单号
        /// </summary>
        [SugarColumn(ColumnName = "APPLICATION_REFUND_ORDER_NO")]
        public string AppliactionRefundOrderNo { get; set; }

        /// <summary>
        /// 平台退款订单号
        /// </summary>
        [SugarColumn(ColumnName = "PLATFORM_REFUND_ORDER_NO")]
        public string PlatformRefundOrderNo { get; set; }

        /// <summary>
        /// 应用系统订单号
        /// </summary>
        [SugarColumn(ColumnName = "APPLICATION_ORDER_NO")]
        public string ApplicationOrderNo { get; set; }

        /// <summary>
        /// 支付渠道编码
        /// </summary>
        [SugarColumn(ColumnName = "PAY_CHANNEL_CODE")]
        public string PayChannelCode { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        [SugarColumn(ColumnName = "TRANS_AMOUNT")]
        public decimal TransAmount { get; set; }

        /// <summary>
        /// 订单大类型
        /// </summary>
        [SugarColumn(ColumnName = "ORDER_BIG_TYPE")]
        public string OrderBigType { get; set; }

        /// <summary>
        /// 订单小类型
        /// </summary>
        [SugarColumn(ColumnName = "ORDER_SMALL_TYPE")]
        public string OrderSmallType { get; set; }

        /// <summary>
        /// 医院编号
        /// </summary>
        [SugarColumn(ColumnName = "HOSPITAL_CODE")]
        public string HospitalCode { get; set; }

        /// <summary>
        /// 退费交易完成时间
        /// </summary>
        [SugarColumn(ColumnName = "REFUND_TRANS_FINISH_TIME")]
        public DateTime RefundTransFinishTime { get; set; }

        /// <summary>
        /// 支付交易完成时间
        /// </summary>
        [SugarColumn(ColumnName = "PAY_TRANS_FINISH_TIME")]
        public string PayTransFinishTime { get; set; }

        /// <summary>
        /// 创建人工号
        /// </summary>
        [SugarColumn(ColumnName = "CREATED_CODE")]
        public string CreatedCode { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [SugarColumn(ColumnName = "CREATED_NAME")]
        public string CreatedName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CREATED_TIME")]
        public DateTime CreatedTime { get; set; }
    }
}
