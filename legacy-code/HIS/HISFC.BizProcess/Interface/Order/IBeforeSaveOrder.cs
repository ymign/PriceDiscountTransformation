using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// 门诊处方保存、住院医嘱保存前，调用接口
    /// [创 建 者: houwb]<br></br>
    /// [创建时间: 2011-8-1]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public interface IBeforeSaveOrder
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
        /// 门诊保存处方
        /// </summary>
        /// <param name="regObj">挂号实体</param>
        /// <param name="reciptDept">开立科室</param>
        /// <param name="reciptDoct">开立医生</param>
        /// <param name="alOrder">医嘱列表</param>
        /// <returns></returns>
        int BeforeSaveOrderForOutPatient(Neusoft.HISFC.Models.Registration.Register regObj, Neusoft.FrameWork.Models.NeuObject reciptDept, Neusoft.FrameWork.Models.NeuObject reciptDoct, ArrayList alOrder);

        /// <summary>
        /// 住院保存医嘱
        /// </summary>
        /// <param name="patientInfo">住院患者实体</param>
        /// <param name="reciptDept">开立科室</param>
        /// <param name="reciptDoct">开立医生</param>
        /// <param name="alOrder">医嘱列表</param>
        /// <returns></returns>
        int BeforeSaveOrderForInPatient(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.FrameWork.Models.NeuObject reciptDept, Neusoft.FrameWork.Models.NeuObject reciptDoct, ArrayList alOrder);

    }
}
