using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.POS
{
    /// <summary>
    /// 杉德金融pos机入参实体类
    /// </summary>
    public class SDCCBPosInfo
    {
        /// <summary>
        /// 操作类型 固定值（见操作类型表）
        /// </summary>
        public string OperateType { get; set; }
        /// <summary>
        /// 交易类型 固定值（见交易类型表）联华积分卡取密码用E7
        /// </summary>
        public string TransType { get; set; }
        /// <summary>
        /// 卡类型 固定值（见卡类型表）
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 收银机编号 商场内唯一（左补零，无则全补空格）
        /// </summary>
        public string CashRegNo { get; set; }
        /// <summary>
        /// 柜员号 （左补零，无则全补空格）
        /// </summary>
        public string CasherNo { get; set; }
        /// <summary>
        /// 金额  以分位单位（左补零）
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 收银流水号 同一收银机内唯一（左补零，无则全补空格）
        /// </summary>
        public string CashTraceNo { get; set; }
        /// <summary>
        /// 系统流水号（原凭证号） 撤销时填写（医保/电子医保退货时需填写）
        /// </summary>
        public string OriginTraceNo { get; set; }
        /// <summary>
        /// 预留字段
        /// </summary>
        public string Reserved { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 门诊号
        /// </summary>
        public string Card_NO { get; set; }
        /// <summary>
        /// 1挂号 2门诊 3住院预交金  4出院结算
        /// </summary>
        public string SourceFlag { get; set; }

        /// <summary>
        /// 流水号 （门诊或者住院）
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 状态：1收费 2退费
        /// </summary>
        public string State { get; set; }

        public SDCCBPosOutInfo outInfo { get; set; }

    }
}
