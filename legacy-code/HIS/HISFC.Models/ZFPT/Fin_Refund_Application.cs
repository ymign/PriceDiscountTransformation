using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.ZFPT
{
    public class Fin_Refund_Application
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 流水号(门诊/住院)
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string PatientNo { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 交易单据号(比如his发票号)
        /// </summary>
        public string TransactionNo { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrderNo { get; set; }

        /// <summary>
        /// 应用系统订单号
        /// </summary>
        public string ApplicationOrderNo { get; set; }

        /// <summary>
        /// 应用系统退款单号
        /// </summary>
        public string AppliactionRefundOrderNo { get; set; }

        /// <summary>
        /// 平台退款订单号
        /// </summary>
        public string PlatformRefundOrderNo { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TransAmount { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 退款状态 0申请退款 1退款中 2退款成功 3退款失败
        /// </summary>
        public string RefundState { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        public string RefundReason { get; set; }

        /// <summary>
        /// 支付渠道编码
        /// </summary>
        public string PayChannelCode { get; set; }

        /// <summary>
        /// 订单大类型
        /// </summary>
        public string OrderBigType { get; set; }

        /// <summary>
        /// 订单小类型
        /// </summary>
        public string OrderSmallType { get; set; }

        /// <summary>
        /// 医院编号
        /// </summary>
        public string HospitalCode { get; set; }

        /// <summary>
        /// 退费交易完成时间
        /// </summary>
        public DateTime RefundTransFinishTime { get; set; }

        /// <summary>
        /// 退款失败信息
        /// </summary>
        public string RefundErrMessage { get; set; }

        /// <summary>
        /// 退款重试次数
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// 创建人工号
        /// </summary>
        public string CreatedCode { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatedName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
