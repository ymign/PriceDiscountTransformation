using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Registration;
using Neusoft.HISFC.Models.Order.OutPatient;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    public interface IOutPatientPrint
    {

        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 打印接口
        /// </summary>
        /// <param name="regObj">挂号实体</param>
        /// <param name="reciptDept">处方开立科室</param>
        /// <param name="reciptDoct">处方开立医生</param>
        /// <param name="orderList">处方List</param>
        /// <param name="orderSelectList">选中的List</param>
        /// <param name="IsReprint">是否补打</param>
        /// <param name="IsPreview">是否预览</param>
        /// <param name="printType">打印类型</param>
        /// <param name="obj">预留字段</param>
        /// <returns></returns>
        int OnOutPatientPrint(Register regObj, NeuObject reciptDept, NeuObject reciptDoct, List<Neusoft.HISFC.Models.Order.OutPatient.Order> orderPrintList, List<Neusoft.HISFC.Models.Order.OutPatient.Order> orderSelectList, bool IsReprint, bool IsPreview, string printType, object obj);


    }
}
