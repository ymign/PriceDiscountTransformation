using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 接口实现配置
    /// </summary>
    public class InterfaceManager
    {
        /// <summary>
        /// 获取金额四舍五入、四舍五舍接口配置
        /// </summary>
        public static object GetTruncFeeType()
        {
            return Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(Neusoft.HISFC.BizProcess.Interface.Fee.InterfaceManager), typeof(Neusoft.HISFC.BizProcess.Interface.Fee.ITruncFee));
        }

        public static object GetIDrugBillClassP()
        {
            return Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(Neusoft.HISFC.BizProcess.Interface.Fee.InterfaceManager),
                typeof(Neusoft.HISFC.BizProcess.Interface.Pharmacy.IDrugBillClassP));
        }
    }
}
