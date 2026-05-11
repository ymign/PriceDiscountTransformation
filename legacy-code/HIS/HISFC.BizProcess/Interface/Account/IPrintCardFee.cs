using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Account
{
    /// <summary>
    /// 诊疗收费凭证打印接口
    /// </summary>
    public interface IPrintCardFee
    {
        /// <summary>
        /// 为打印UC赋值
        /// </summary>
        /// <param name="cardFee"></param>
        void SetValue(HISFC.Models.Account.AccountCardFee cardFee);
        /// <summary>
        /// 打印
        /// </summary>
        void Print();
    }

    /// <summary>
    /// 诊疗返还凭证打印接口
    /// </summary>
    public interface IPrintReturnCardFee
    {
        /// <summary>
        /// 为打印UC赋值
        /// </summary>
        /// <param name="cardFee"></param>
        void SetValue(HISFC.Models.Account.AccountCardFee cardFee);
        /// <summary>
        /// 打印
        /// </summary>
        void Print();
    }
}
