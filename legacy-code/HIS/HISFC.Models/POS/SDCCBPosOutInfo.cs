using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.POS
{
    public class SDCCBPosOutInfo
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TransType { get; set; }

        /// <summary>
        /// 卡类型
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 返回码
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string ResponseMsg { get; set; }

        /// <summary>
        /// 收银机编号
        /// </summary>
        public string CashRegNo { get; set; }

        /// <summary>
        /// 柜员号
        /// </summary>
        public string CasherNo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 结算批次
        /// </summary>
        public string SellteNum { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TerminalID { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 卡有效期
        /// </summary>
        public string Exp_Date { get; set; }

        /// <summary>
        /// 发卡行编码
        /// </summary>
        public string BankNo { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public string TransDate { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public string TransTime { get; set; }

        /// <summary>
        /// 授权号
        /// </summary>
        public string Auth_Code { get; set; }

        /// <summary>
        /// 系统参照号
        /// </summary>
        public string SysRefNo { get; set; }

        /// <summary>
        /// 收银流水号
        /// </summary>
        public string CashTraceNo { get; set; }

        /// <summary>
        /// 原收银流水号
        /// </summary>
        public string OriginTraceNo { get; set; }

        /// <summary>
        /// 系统流水号
        /// </summary>
        public string SysTracdNo { get; set; }

        /// <summary>
        /// 原系统流水号
        /// </summary>
        public string OriginSysTraceNo { get; set; }

        /// <summary>
        /// 预留字段
        /// </summary>
        public string Reserved { get; set; }

    }
}
