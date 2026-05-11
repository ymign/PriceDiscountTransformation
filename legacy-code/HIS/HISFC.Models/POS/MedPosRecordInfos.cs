using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.POS
{
    /// <summary>
    /// 交易信息记录
    /// </summary>
    public class MedPosRecordInfos
    {
        /// <summary>
        /// 病人编号
        /// </summary>
        public string Card_No { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string Invoice_No { get; set; }

        /// <summary>
        /// 功能编码
        /// </summary>
        public string GNBM { get; set; }
        /// <summary>
        /// 反馈标志
        /// </summary>
        public string FKBZ { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public string QQSJ { get; set; }
        /// <summary>
        /// 联机交易金额
        /// </summary>
        public string LJJYJE { get; set; }
        /// <summary>
        /// 医保个账交易金额
        /// </summary>
        public string YBGZJE { get; set; }

        /// <summary>
        /// 微信交易金额
        /// </summary>
        public string WXJE { get; set; }

        /// <summary>
        /// 支付宝交易金额
        /// </summary>
        public string ZFBJE { get; set; }

        /// <summary>
        /// 银联交易金额
        /// </summary>
        public string YLJE { get; set; }
        /// <summary>
        /// 交易凭证号
        /// </summary>
        public string JYPZH { get; set; }

        /// <summary>
        /// 交易认证码（TAC）
        /// </summary>
        public string JYRZM { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public string JYJE { get; set; }
        /// <summary>
        /// 终端机PSAM卡号
        /// </summary>
        public string ZDJKH { get; set; }
        /// <summary>
        /// 卡交易序号
        /// </summary>
        public string KJYXH { get; set; }
        /// <summary>
        /// 终端交易序号
        /// </summary>
        public string ZDJYXH { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string JYSJ { get; set; }

        /// <summary>
        /// 社保卡所属城市代码
        /// </summary>
        public string SBKSSCSDM { get; set; }
        /// <summary>
        /// 卡片复位信息
        /// </summary>
        public string KPFWXX { get; set; }
        /// <summary>
        /// 社保卡卡号
        /// </summary>
        public string SBKKH { get; set; }
        /// <summary>
        /// POS终端编号
        /// </summary>
        public string POSZDH { get; set; }
        /// <summary>
        /// POS机版本
        /// </summary>
        public string POSBB { get; set; }

        /// <summary>
        /// 身份号码
        /// </summary>
        public string SFHM { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string XM { get; set; }
        /// <summary>
        /// 状态信息
        /// </summary>
        public string STATE { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public string Amount { get; set; }
    }
}
