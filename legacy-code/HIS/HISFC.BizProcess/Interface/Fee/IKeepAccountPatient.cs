using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// [功能描述：收费前--记帐患者处理]
    /// [创建时间： 2011-12-30]
    /// </summary>
    public interface IKeepAccountPatient
    {
        /// <summary>
        /// 判断是否记帐患者
        /// </summary>
        /// <param name="PatientInfo"></param>
        /// <returns></returns>
        bool IsKeepAccountPatient(Neusoft.HISFC.Models.Registration.Register PatientInfo);
        /// <summary>
        /// 记帐处理
        /// </summary>
        /// <param name="PatientInfo"></param>
        /// <param name="balancePays"></param>
        /// <param name="invoices"></param>
        /// <param name="invoiceDetails"></param>
        /// <param name="invoiceFeeDetails"></param>
        /// <param name="comFeeItemLists"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool DealwithKeepAccount(ref Neusoft.HISFC.Models.Registration.Register PatientInfo, ref ArrayList balancePays, ref ArrayList invoices, ref ArrayList invoiceDetails, ref ArrayList invoiceFeeDetails, ref ArrayList comFeeItemLists, out string errMsg);
    }
}
