using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.ClinicPath
{
   public interface IClinicPath
    {
        ///患者是否在临床路径
        /// <param name="inpatientNo">患者住院号</param>
        /// <returns>成功返回true 失败返回false</returns>
        bool PatientIsSelectedPath(string inpatientNo);

        /// <summary>
        /// 点击患者
        /// </summary>
        /// <param name="inpatientNo">患者住院号</param>
        ///  <returns>在临床路径返回1 不在返回-1</returns>
        int ClickClinicPath(string inpatientNo);

        /// <summary>
        /// 护士站出院操作
        /// </summary>
        /// <param name="inpatientNO">患者住院号</param>
        /// <returns>成功返回true 失败返回false</returns>
        bool PatientOutByNurse(string inpatientNO);

        /// <summary>
        /// 医生站出院操作
        /// </summary>
        /// <param name="inpatientNO">患者住院号</param>
        /// <returns>成功返回true 失败返回false</returns>

        bool PatientOutByDoctor(string inpatientNO);
        /// <summary>
        /// 保存医嘱时，自动判断医生开立的医嘱，是否在路径中，如不在路径中，则弹出提示，填写变异原因。
        /// </summary>
        /// <param name="inpatientNo">患者住院号</param>
        /// <param name="orderList">医嘱列表</param>
        /// <returns>成功返回true 失败返回false</returns>

        bool PatientSaveOrder(string inpatientNo, List<Neusoft.HISFC.Models.Order.Inpatient.Order> orderList);
    }
}