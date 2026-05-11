using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 获取执行科室
    /// </summary>
    public interface IExecDept
    {
        /// <summary>
        /// 获取执行科室
        /// </summary>
        /// <param name="recipeDept"></param>
        /// <param name="item"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        ArrayList GetExecDept(Neusoft.FrameWork.Models.NeuObject recipeDept, Neusoft.HISFC.Models.Fee.Item.Undrug item, ref string errorInfo);
    }
}
