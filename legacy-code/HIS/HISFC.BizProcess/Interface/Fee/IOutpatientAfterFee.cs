using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// [功能描述: 门诊收费保存后发生]<br></br>
    /// [创 建 者: cube]<br></br>
    /// [创建时间: 2011-03]<br></br>
    /// </summary>
    public interface IOutpatientAfterFee
    {
        /// <summary>
        /// 门诊收费保存后发生
        /// </summary>
        /// <param name="alFeeItem">本次收费项目</param>
        /// <param name="info">信息</param>
        /// <returns></returns>
        int AfterFee(ArrayList alFeeItem, string info);       
    }
}
