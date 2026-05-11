using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.RADT
{
    /// <summary>
    ///  住院床头卡打印接口
    /// </summary>
    public interface IBedCard
    {
        /// <summary>
        /// 打印
        /// </summary>
        int Print();

        /// <summary>
        /// 打印
        /// </summary>
        int PrintPreview();

        /// <summary>
        /// 查询显示床头卡
        /// </summary>
        /// <param name="patients"></param>
        int ShowBedCard(ArrayList patients);
    }
}
