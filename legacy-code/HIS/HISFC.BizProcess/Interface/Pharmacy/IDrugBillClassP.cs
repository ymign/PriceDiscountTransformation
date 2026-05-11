using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Pharmacy
{
    public interface IDrugBillClassP
    {
        Neusoft.HISFC.Models.Pharmacy.DrugMessage GetDrugMessage(Neusoft.HISFC.Models.Pharmacy.ApplyOut applyOut);
    }
}
