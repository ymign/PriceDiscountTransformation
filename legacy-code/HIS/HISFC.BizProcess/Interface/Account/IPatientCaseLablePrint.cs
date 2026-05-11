using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Account
{
    /// <summary>
    /// 病案标签打印
    /// </summary>
    public interface IPatientCaseLablePrint
    {

        /// <summary>
        /// 打印患者病案标签
        /// </summary>
        /// <param name="inpatientno">住院流水号</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        int PrintPatientCaseLable(string inpatientno, ref string err);

        /// <summary>
        /// 打印患者病案标签
        /// </summary>
        /// <param name="patientinfo">患者信息(Neusoft.HISFC.Models.RADT.PatientInfo)</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        int PrintPatientCaseLable(Neusoft.HISFC.Models.RADT.PatientInfo patientinfo, ref string err);
    }
}
