using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;

namespace Neusoft.HISFC.Components.OutpatientFee
{
    public class InterfaceManager
    {
        private static Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder IOrder = null;
        /// <summary>
        /// 获取业务操作ADT信息消息收发接口
        /// </summary>
        /// <returns></returns>
        public static Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder GetIOrder()
        {
            if (IOrder == null)
            {
                IOrder = ControllerFactroy.Instance.CreateInferface<Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder>(typeof(InterfaceManager), null);
            }

            return IOrder;
        }

        /// <summary>
        /// 获取执行科室的接口实现
        /// </summary>
        /// <returns></returns>
        public static Neusoft.HISFC.BizProcess.Interface.Fee.IExecDept GetIExecDept()
        {
            return ControllerFactroy.Instance.CreateInferface<Neusoft.HISFC.BizProcess.Interface.Fee.IExecDept>(typeof(InterfaceManager), null);
        }

        /// <summary>
        /// 获取分发票接口
        /// </summary>
        /// <returns></returns>
        public static Neusoft.HISFC.BizProcess.Interface.FeeInterface.ISplitInvoice GetISplitInvoice()
        {
            return ControllerFactroy.Instance.CreateInferface<Neusoft.HISFC.BizProcess.Interface.FeeInterface.ISplitInvoice>(typeof(InterfaceManager), null);
        }

        public static Neusoft.HISFC.BizProcess.Interface.Account.IReadIDCard GetIReadIDCard()
        {
            return ControllerFactroy.CreateFactory().CreateInferface<Neusoft.HISFC.BizProcess.Interface.Account.IReadIDCard>(typeof(InterfaceManager), null);
        }

        private static Neusoft.HISFC.BizProcess.Interface.Account.IOperCard IOperCard = null;
        /// <summary>
        /// 读卡接口
        /// </summary>
        /// <returns></returns>
        public static Neusoft.HISFC.BizProcess.Interface.Account.IOperCard GetIOperCard()
        {
            if (IOperCard == null)
            {
                IOperCard = Neusoft.SOC.HISFC.BizProcess.CommonInterface.ControllerFactroy.Instance.CreateInferface<Neusoft.HISFC.BizProcess.Interface.Account.IOperCard>(typeof(InterfaceManager), null);
            }
            return IOperCard;
        }

    }
}
