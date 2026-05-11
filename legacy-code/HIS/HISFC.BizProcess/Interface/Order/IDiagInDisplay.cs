using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface
{
    /// <summary>
    /// 门诊医生叫号接口
    /// </summary>
    public interface IDiagInDisplay
    {
        /// <summary>
        /// 患者挂号信息
        /// </summary>
        Neusoft.HISFC.Models.Registration.Register RegInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 医生诊室
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject ObjRoom
        {
            get;
            set;
        }

        /// <summary>
        /// 医生叫号
        /// </summary>
        void DiagInDisplay();
    }
}
