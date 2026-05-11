using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.Models.Registration;
using System.Collections;
using Neusoft.HISFC.Models.Base;
using System.Windows.Forms;
using Neusoft.FrameWork.Models;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Management;

namespace Neusoft.HISFC.BizProcess.Integrate
{
    /// <summary>
    /// [功能描述: 整合的预交金流程扣费管理类]
    /// {823CF7C5-34C5-4b76-9601-9F83A8A5738E}
    /// </summary>
    public class FeeAccount : IntegrateBase, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
