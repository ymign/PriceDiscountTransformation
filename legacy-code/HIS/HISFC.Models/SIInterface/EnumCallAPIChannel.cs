using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.SIInterface
{ 
    /// <summary>
    /// 注意：
    /// ZDWY开头的为中大五院相关接口，ZDXQ为中大校区相关接口
    /// 后面第一个_  CK 代表窗口缩写  WX代表微信  ZZJ代表自助机
    /// 后面第二个_  GH代表挂号业务   MZJF门诊缴费
    /// </summary>
    public enum EnumCallAPIChannel
    {
        ZDWY_CK_GH,//--中大五院窗口挂号
        ZDWY_CK_MZJF,//--中大五院窗口门诊缴费
        ZDWY_WX_GH,//--中大五院微信挂号
        ZDWY_WX_MZJF,//--中大五院微信门诊缴费
        ZDWY_ZZJ_GH,//中大五院自助机挂号
        ZDWY_ZZJ_MZJF,//中大五院自助机门诊缴费
        ZDWY_JKZH_GH,//中大五院健康珠海挂号
        ZDWY_JKZH_MZJF,//中大五院健康珠海门诊缴费
        ZDXQ_CK_GH,//校区窗口挂号
        ZDXQ_CK_MZJF,//校区窗口门诊缴费
        ZDWY_YBXYF_MZJF,//中大五院信用付门诊收费
        ZDXQ_ZZJ_GH,//校区自助机挂号
        ZDXQ_ZZJ_MZJF,//校区自助机门诊缴费
    }
}
