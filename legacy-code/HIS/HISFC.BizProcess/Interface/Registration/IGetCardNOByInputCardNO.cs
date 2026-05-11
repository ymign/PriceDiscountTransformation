using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Registration
{
    /// <summary>
    /// 根据输入的卡号判断患者的主索引和物理卡类型和患者主索引
    /// </summary>
    public interface IGetCardNOByInputCardNO
    {
        /// <summary>
        /// 根据输入的卡号判断患者的主索引和物理卡类型和患者主索引
        /// </summary>
        /// <param name="inputCardNO">输入卡号</param>
        /// <param name="cardNO">患者主索引</param>
        /// <param name="physicalTypeID">物理卡类型</param>
        /// <returns></returns>
        int GetCardNOByInputCardNO(string inputCardNO, ref string cardNO, ref string physicalTypeID, ref string errText);
    }
}
