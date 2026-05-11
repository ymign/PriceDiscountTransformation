using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 诊疗收费凭证打印接口
    /// </summary>
    public interface IPrintCancleFee
    {
        /// <summary>
        /// 为打印UC赋值
        /// </summary>
        /// <param name="cardFee"></param>
        void SetValue(Neusoft.HISFC.Models.Account.AccountRecord CancelRecord, Neusoft.HISFC.Models.Account.AccountRecord FeeRecord);
        /// <summary>
        /// 打印
        /// </summary>
        void Print();
    }

}
