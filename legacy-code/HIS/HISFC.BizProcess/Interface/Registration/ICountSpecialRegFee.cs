using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Registration
{
    /// <summary>
    /// 计算特殊挂号费接口
    /// </summary>
    public interface ICountSpecialRegFee
    {
        /// <summary>
        /// 计算特殊挂号费接口
        /// </summary>
        /// <param name="birthday">生日</param>
        /// <param name="name">姓名</param>
        /// <param name="idenno">身份证号</param>
        /// <param name="regfee">挂号费</param>
        /// <param name="digfee">诊金</param>
        /// <param name="othfee">病历本费</param>
        /// <param name="regObj">挂号患者实体</param>
        /// <returns></returns>
        int CountSpecialRegFee(DateTime birthday, string name, string idenno, ref decimal regfee, ref decimal digfee, ref decimal othfee, ref Neusoft.HISFC.Models.Registration.Register regObj);
    }
}
