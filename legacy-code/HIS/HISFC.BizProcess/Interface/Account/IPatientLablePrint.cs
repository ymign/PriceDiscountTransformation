using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Account
{
    public interface IPatientLablePrint
    {
        /// <summary>
        /// 打印病人标签
        /// </summary>
        /// <param name="cardno">卡号</param>
        /// <param name="mesg">信息</param>
        /// <returns></returns>
        int PatientLablePrint(string cardno,ref string msg);

        /// <summary>
        /// 打印病人标签
        /// </summary>
        /// <param name="cardno">门诊流水号</param>
        /// <param name="mesg">信息</param>
        /// <returns></returns>
        int PatientLablePrint(Neusoft.HISFC.Models.RADT.Patient p, ref string msg);

    }
}
