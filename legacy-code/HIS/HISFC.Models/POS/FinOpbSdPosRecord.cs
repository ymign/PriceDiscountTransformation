using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.POS
{
    public class FinOpbSdPosRecord
    {
        /// <summary>
        /// 数据主键
        /// </summary>
        public string RecordId { get; set; }

        /// <summary>
        /// 流水号:门诊或者住院
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 门诊号
        /// </summary>
        public string Card_No { get; set; }

        /// <summary>
        /// 数据来源：1挂号 2门诊 3住院预交金 4出院结算
        /// </summary>
        public string SourceFlag { get; set; }

        /// <summary>
        /// 数据状态：1收费 2退费
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperTime { get; set; }

        /// <summary>
        /// 传入结算金额(单位：分)
        /// </summary>
        public string Amount { get; set; }

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
        /// POS机器返回真正的结算金额(单位：分)
        /// </summary>
        public string OutAmount { get; set; }

        /// <summary>
        /// 结算批次
        /// </summary>
        public string SellteNum { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 卡有效期
        /// </summary>
        public string ExpDate { get; set; }

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
        public string Sysrefno { get; set; }
        /// <summary>
        /// 收银流水号
        /// </summary>
        public string CashTraceno { get; set; }
        /// <summary>
        /// 原收银流水号
        /// </summary>
        public string OriginTraceno { get; set; }
        /// <summary>
        /// 系统流水号
        /// </summary>
        public string Systracdno { get; set; }
        /// <summary>
        /// 原系统流水号
        /// </summary>
        public string OriginSysTraceno { get; set; }
        /// <summary>
        /// 预留字段
        /// </summary>
        public string Reserved { get; set; }
    }
}
