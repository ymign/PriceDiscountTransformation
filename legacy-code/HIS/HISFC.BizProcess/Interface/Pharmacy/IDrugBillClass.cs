using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Pharmacy
{
    public interface IDrugBillClass
    {
        /// <summary>
        /// 发药申请发送时获取对应的摆药单
        /// </summary>
        /// <returns></returns>
        Neusoft.HISFC.Models.Pharmacy.DrugBillClass GetDrugBillClass(Neusoft.HISFC.Models.Pharmacy.ApplyOut applyOut);
    }
}
