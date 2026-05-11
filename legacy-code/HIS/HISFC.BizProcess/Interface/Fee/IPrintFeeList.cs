using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 打印费用清单接口
    /// </summary>
    public interface IPrintFeeList
    {
        /// <summary>
        /// 为打印UC赋值
        /// </summary>
        /// <param name="invoice"></param>
        bool SetValue(Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice);
        /// <summary>
        /// 打印
        /// </summary>
        bool PrintFeeList();
    }
}
