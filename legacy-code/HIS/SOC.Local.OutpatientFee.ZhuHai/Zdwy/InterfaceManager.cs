using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy
{
    public class InterfaceManager
    {
        /// <summary>
        /// 获取执行科室的接口实现
        /// </summary>
        /// <returns></returns>
        public static Neusoft.HISFC.BizProcess.Interface.Fee.IExecDept GetIExecDept()
        {
            return ControllerFactroy.CreateFactory().CreateInferface<Neusoft.HISFC.BizProcess.Interface.Fee.IExecDept>(typeof(InterfaceManager), null);
        }
    }
}
