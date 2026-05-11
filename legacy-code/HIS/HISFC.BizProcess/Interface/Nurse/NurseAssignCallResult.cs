using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Nurse
{
    /// <summary>
    /// 代表叫号接口返回值的类
    /// </summary>
    /// {4316de20-69b3-40e2-80ac-7033f60cd1ed}
    [Serializable]
    public class NurseAssignCallResult
    {
        /// <summary>
        /// 返回值，-1出错。
        /// </summary>
        public int ReturnValue
        {
            get;
            set;
        }

        /// <summary>
        /// 叫号患者的流水号
        /// </summary>
        public string AssignRegiterId
        {
            get;
            set;
        }
    }
}
