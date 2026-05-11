using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Registration;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    // 摘要:
    //     指引单，中大五院
    public interface IOutpatientGuideNew
    {
        // 摘要:
        //     打印
        void Print();
        //
        // 摘要:
        //     为打印UC赋值
        //
        // 参数:
        //   rInfo:
        //
        //   invoices:
        //
        //   feeDetails:
        void SetValue(Register rInfo, ArrayList invoices, List<MZGuide> feeDetails);
    }
}
