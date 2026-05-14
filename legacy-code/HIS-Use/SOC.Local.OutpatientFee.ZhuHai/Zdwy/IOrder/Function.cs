using System;
using System.Collections;
using System.Text;
using Neusoft.FrameWork.Models;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Registration;
using System.Data;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.BizProcess.Interface.FeeInterface;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using System.Collections.Generic;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOrder
{
    public class Function
    {
        /// <summary>
        /// 获取公费报表统计大类
        /// </summary>
        /// <returns></returns>
        public static DataTable GetGFReportDataFeeCodeStat()
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            DataSet ds = new DataSet();
            outpatientManager.GetInvoiceClass("MZGF", ref ds);

            return ds.Tables[0];
        }

    }
}
