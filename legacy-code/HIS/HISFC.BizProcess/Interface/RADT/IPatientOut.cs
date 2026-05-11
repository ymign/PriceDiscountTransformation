using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.RADT
{
    /// <summary>
    /// 出院登记调用接口
    /// [创 建 者: houwb]<br></br>
    /// [创建时间: 2011-8-23]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public interface IPatientOut
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
        /// 出院登记前调用
        /// </summary>
        /// <param name="patientInfo">住院患者实体</param>
        /// <param name="oper">当前登陆操作员</param>
        /// <returns></returns>
        int BeforePatientOut(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.FrameWork.Models.NeuObject oper);

        /// <summary>
        /// 出院登记过程中调用
        /// </summary>
        /// <param name="patientInfo">住院患者实体</param>
        /// <param name="oper">当前登陆操作员</param>
        /// <returns></returns>
        int OnPatientOut(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.FrameWork.Models.NeuObject oper);

        /// <summary>
        /// 出院登记后调用
        /// </summary>
        /// <param name="patientInfo">住院患者实体</param>
        /// <param name="oper">当前登陆操作员</param>
        /// <returns></returns>
        int AfterPatientOut(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.FrameWork.Models.NeuObject oper);
    }
}
