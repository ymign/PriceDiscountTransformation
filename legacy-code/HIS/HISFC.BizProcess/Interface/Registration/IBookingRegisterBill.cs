using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Registration
{
    /// <summary>
    /// 预约挂号打印{C9ABAAAF-18E3-4553-B5A0-822E527BE685}
    /// </summary>
    public interface IBookingRegisterBill
    {

        int Print(Neusoft.HISFC.Models.Registration.Booking booking);
    }
}
