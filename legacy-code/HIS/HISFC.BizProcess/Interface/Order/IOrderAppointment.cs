using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// 诊间预约
    /// </summary>
    public interface IOrderAppointment
    {
        /// <summary>
        /// 诊间预约
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        int Appointment(Models.Registration.Register patient);

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        int SetInfo(Models.Registration.Register patient);
    }
}
