using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.DCP
{
    /// <summary>
    /// 传染病报告接口类
    /// </summary>
    public interface IDCP
    {
        /// <summary>
        /// 患者传染病报告
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="patientType">患者类型</param>
        /// <returns></returns>
        int RegisterDiseaseReport(System.Windows.Forms.IWin32Window owner, Neusoft.HISFC.Models.RADT.Patient patient, Neusoft.HISFC.Models.Base.ServiceTypes patientType);

        /// <summary>
        /// 必须填写传染病报告
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="patient"></param>
        /// <param name="patientType"></param>
        /// <param name="diagName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        int CheckDiseaseReport(System.Windows.Forms.IWin32Window owner, Neusoft.HISFC.Models.RADT.Patient patient, Neusoft.HISFC.Models.Base.ServiceTypes patientType, string diagName, out string msg);

        /// <summary>
        /// 登录时是否提示不合格的报告卡
        /// </summary>
        /// <param name="patientType"></param>
        /// <returns></returns>
        int LoadNotice(System.Windows.Forms.IWin32Window owner, Neusoft.HISFC.Models.Base.ServiceTypes patientType);
    }
}
