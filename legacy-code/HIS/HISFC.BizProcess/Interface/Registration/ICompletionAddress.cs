using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Registration
{
    /// <summary>
    /// 补全地址接口
    /// </summary>
    public interface ICompletionAddress
    {
        /// <summary>
        /// 补全地址
        /// </summary>
        /// <param name="address">输入地址</param>
        /// <returns>补完后地址</returns>
        string CompletionAddress(string address);
    }
}
