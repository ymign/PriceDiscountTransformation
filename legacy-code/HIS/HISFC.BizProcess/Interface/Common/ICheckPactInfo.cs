using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Common
{
    /// <summary>
    /// [功能描述: 校验患者合同单位信息]<br></br>
    /// [创 建 者: houwb]<br></br>
    /// [创建时间: 2011-12-6]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public interface ICheckPactInfo
    {
        /// <summary>
        /// 患者基本信息
        /// </summary>
        Neusoft.HISFC.Models.RADT.Patient PatientInfo
        {
            set;
            get;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        string Err
        {
            set;
            get;
        }

        /// <summary>
        /// 校验是否有效
        /// </summary>
        /// <returns></returns>
        int CheckIsValid();
    }
}
