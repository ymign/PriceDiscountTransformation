using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 金额四舍五入、四舍五舍接口
    /// cao-lin
    /// </summary>
    public interface ITruncFee
    {
        /// <summary>
        /// 金额四舍五入、四舍五舍接口
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        object[] TruncFee(object []args);

        /// <summary>
        /// 金额四舍五入、四舍五舍接口
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        object TruncFee(object arg);
    }
}
