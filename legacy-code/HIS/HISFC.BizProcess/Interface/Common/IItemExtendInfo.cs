using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Common
{
    /// <summary>
    /// 获取项目扩展信息接口
    /// 获取医保、公费对照信息
    /// </summary>
    public interface IItemCompareInfo
    {
        /// <summary>
        /// 获取医保对照项目
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="compareItem"></param>
        /// <returns></returns>
        int GetCompareItemInfo(Neusoft.HISFC.Models.Base.Item item, Neusoft.HISFC.Models.Base.PactInfo pactInfo, ref Neusoft.HISFC.Models.SIInterface.Compare compare, ref string strCompareInfo);

        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrInfo
        {
            get;
            set;
        }
    }
}
