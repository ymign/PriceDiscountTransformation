using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// 医生站辅材处理
    /// </summary>
    public interface IDealSubjob
    {
        /// <summary>
        /// 用法带出的项目是否允许弹出选择
        /// 避免效率太慢，一般情况下不需要
        /// </summary>
        bool IsAllowUsageSubPopChoose
        {
            set;
            get;
        }

        /// <summary>
        /// 附材收取的时间
        /// </summary>
        DateTime FeeDate
        {
            get;
            set;
        }

        /// <summary>
        /// 是否弹出附材窗口供选择
        /// </summary>
        bool IsPopForChose
        {
            get;
            set;
        }

        /// <summary>
        /// 医生站处理辅材
        /// </summary>
        /// <param name="r">患者挂号信息</param>
        /// <param name="alFee">费用信息</param>
        /// <param name="errText">错误信息</param>
        /// <returns></returns>
        //int DealSubjob(Neusoft.HISFC.Models.Registration.Register r, ArrayList alFee, ref string errText);

        /// <summary>
        /// 门诊辅材带出
        /// </summary>
        /// <param name="r"></param>
        /// <param name="alOrders"></param>
        /// <param name="alSubOrders"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        int DealSubjob(Neusoft.HISFC.Models.Registration.Register r, ArrayList alOrders, Neusoft.HISFC.Models.Order.OutPatient.Order outOrder, ref ArrayList alSubOrders, ref string errText);

        /// <summary>
        /// 住院附材带出
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="isRealTime">是否实时收取，否则为后台收取</param>
        /// <param name="order"></param>
        /// <param name="alOrders"></param>
        /// <param name="alSubOrders"></param>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        int DealSubjob(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, bool isRealTime, Neusoft.HISFC.Models.Order.Inpatient.Order order, ArrayList alOrders, ref ArrayList alSubOrders, ref string errInfo);
    }
}
