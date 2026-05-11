using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Common
{
    /// <summary>
    /// IJob 的摘要说明。
    /// </summary>
    public interface IJob
    {
        string Message
        {
            get;
        }

        int Start();

    }
}
