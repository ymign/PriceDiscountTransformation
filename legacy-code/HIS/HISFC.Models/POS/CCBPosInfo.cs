using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.POS
{
    /// <summary>
    /// 建行pos机实体类 20190916
    /// </summary>
    public class CCBPosInfo
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string PosAppName { get; set; }

        /// <summary>
        /// 交易类型 旧
        /// 扫码在线查询 = 0x1F
        /// 银行卡交易 TransType = 0x02 
        /// 取消银行卡交易transtype = 0x03
        /// 支付宝交易 transtype = 0x95
        /// 取消支付宝交易 transtype = 0x98
        /// 微信交易 transtype = 0x96
        /// 取消微信交易 transtype = 0x99
        /// </summary>
        public string TransType { get; set; }

        /// <summary>
        /// 交易类型
        /// 签到 00
        /// 扣费 02
        /// 撤销 03
        /// 退货 04
        /// </summary>
        public string OperType { get; set; }

        /// <summary>
        /// 交易金额 12位分为单位，不足位补0
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 交易金额 元为单位
        /// </summary>
        public decimal TotCost { get; set; }
        
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TerminalNo { get; set; }

        /// <summary>
        /// 操作员号
        /// </summary>
        public string OperatorNo { get; set; }

        /// <summary>
        /// 收单行号
        /// </summary>
        public string AcquirNo { get; set; }

        /// <summary>
        /// 发卡行号
        /// </summary>
        public string IssuerNo { get; set; }

        /// <summary>
        /// 发卡行名
        /// </summary>
        public string IssuerName { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TraceNo { get; set; }

        /// <summary>
        /// 交易授权码
        /// </summary>
        public string AuthNo { get; set; }

        /// <summary>
        /// 系统参考号
        /// </summary>
        public string ReferenceNo { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public string TransDate { get; set; }

        /// <summary>
        /// 费用日期 YYYYMMDD
        /// </summary>
        public string FeeDate { get; set; }
        /// <summary>
        /// 交易时间 HHmmss （6位,HHmmss）
        /// </summary>
        public string TransTime { get; set; }

        /// <summary>
        /// 返回码
        /// </summary>
        public string RspCode { get; set; }

        /// <summary>
        /// 返回码中文解释
        /// </summary>
        public string  RspDes { get; set; }

        /// <summary>
        /// 交易唯一标识 TransCheck参考格式为YYYYMMDD + hhmmss + 收银小票号（保证每笔交易的TransCheck唯一值即可，最好不要超过20位）
        /// </summary>
        public string TransCheck { get; set; }

        /// <summary>
        /// 原交易唯一标识
        /// </summary>
        public string OriTransCheck { get; set; }

        /// <summary>
        /// 卡有效期
        /// </summary>
        public string CardExpireDate { get; set; }

        /// <summary>
        /// 清算日期
        /// </summary>
        public string SettleDate { get; set; }

        /// <summary>
        /// 原交易流水号（6位，不足左补0）
        /// </summary>
        public string OriTraceNo { get; set; }

        /// <summary>
        /// 扫码支付订单号
        /// </summary>
        public string ScanOrderId { get; set; }

        /// <summary>
        /// 原扫码支付订单号
        /// </summary>
        public string OriScanOrderId { get; set; }

        /// <summary>
        /// 扫码支付用户ID
        /// </summary>
        public string ScanUserId { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 保险单据号
        /// </summary>
        public string InsurOrderNo { get; set; }

        /// <summary>
        /// 原数据传送域
        /// </summary>
        public string OriDataField { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string ExtraDataField { get; set; }

        /// <summary>
        /// 病人编号
        /// </summary>
        public string Card_No { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string Invoice_No { get; set; }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string VouchNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }

        public CCBPosInfo()
        {

        }
    }
}
