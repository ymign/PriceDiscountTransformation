using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.EMPI.CARDINFO
{
    //add by allan 20160726 EMPI接口内容
    /// <summary>
    /// 查询卡信息返回内容
    /// </summary>
    public class RESPONSE
    {
        public RESPONSE()
        {
            CARDINFOS = new List<CARD>();
        }
        public List<CARD> CARDINFOS { get; set; }
    }
}
