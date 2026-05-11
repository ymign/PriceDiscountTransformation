using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// LIS申请单打印
    /// </summary>
    public interface ILisReportPrint
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 门诊LIS申请单打印
        /// </summary>
        /// <param name="regObj">挂号实体</param>
        /// <param name="reciptDept">开立科室</param>
        /// <param name="reciptDoct">开立医生</param>
        /// <param name="alLisOrder">检验医嘱列表</param>
        /// <returns></returns>
        int LisReportPrintForOutPatient(Neusoft.HISFC.Models.Registration.Register regObj, Neusoft.FrameWork.Models.NeuObject reciptDept, Neusoft.FrameWork.Models.NeuObject reciptDoct, ArrayList alLisOrder);

        /// <summary>
        /// 住院LIS申请单打印
        /// </summary>
        /// <param name="regObj">挂号实体</param>
        /// <param name="reciptDept">开立科室</param>
        /// <param name="reciptDoct">开立医生</param>
        /// <param name="alOrder">医嘱列表</param>
        /// <returns></returns>
        int LisReportPrintForInPatient(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.FrameWork.Models.NeuObject reciptDept, Neusoft.FrameWork.Models.NeuObject reciptDoct, ArrayList alLisOrder);

    }
}
