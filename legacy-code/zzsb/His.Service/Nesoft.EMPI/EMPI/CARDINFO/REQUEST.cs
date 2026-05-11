using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.EMPI.EMPI.CARDINFO
{
    //add by allan 20160726 EMPI接口内容
    /// <summary>
    /// 查询卡信息
    /// </summary>
    public class REQUEST
    {
        /// <summary>
        /// 卡号
        /// </summary>
        public string CARDNO { get; set; }
        /// <summary>
        /// 卡类型
        /// </summary>
        public string CARDTYPE { get; set; }
        /// <summary>
        /// 域名 默认001
        /// </summary>
        public string DOMAIN { get; set; }
    }
}
