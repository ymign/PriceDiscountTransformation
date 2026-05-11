using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// 检查申请单打印
    /// </summary>
    public interface IPacsReportPrint
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
        /// 门诊检查申请单打印
        /// </summary>
        /// <param name="regObj">挂号实体</param>
        /// <param name="reciptDept">开立科室</param>
        /// <param name="reciptDoct">开立医生</param>
        /// <param name="alLisOrder">医嘱列表</param>
        /// <returns></returns>
        int PacsReportPrintForOutPatient(Neusoft.HISFC.Models.Registration.Register regObj, Neusoft.FrameWork.Models.NeuObject reciptDept, Neusoft.FrameWork.Models.NeuObject reciptDoct, ArrayList alOrder);

        /// <summary>
        /// 住院检查申请单打印
        /// </summary>
        /// <param name="regObj">病人信息实体</param>
        /// <param name="reciptDept">开立科室</param>
        /// <param name="reciptDoct">开立医生</param>
        /// <param name="alOrder">医嘱列表</param>
        /// <returns></returns>
        int PacsReportPrintForInPatient(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.FrameWork.Models.NeuObject reciptDept, Neusoft.FrameWork.Models.NeuObject reciptDoct, ArrayList alOrder);

    }
}
