using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.EMPI
{
    //add by allan 20160726 EMPI接口内容
    /// <summary>
    /// 就诊卡
    ///  </summary>
    public class CARD
    {
        /// <summary>
        /// 卡号码
        /// </summary>
        public string CARDNO { get; set; }
        /// <summary>
        /// 卡类型
        /// </summary>
        public string CARDTYPE { get; set; }
        /// <summary>
        /// 操作员编号
        /// </summary>
        public string OPERCODE { get; set; }
        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string OPERNAME { get; set; }
    }
}
