using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Neusoft.HISFC.Models.Registration;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    ///  病人指引单接口
    /// </summary>
    public interface IOutpatientGuide
    {
        /// <summary>
        /// 为打印UC赋值
        /// </summary>
        /// <param name="rInfo"></param>
        /// <param name="invoices"></param>
        /// <param name="feeDetails"></param>
        void SetValue(Register rInfo, ArrayList invoices, ArrayList feeDetails);
        /// <summary>
        /// 为打印UC赋值
        /// </summary>
        void SetValue(DataTable dt);
        /// <summary>
        /// 打印
        /// </summary>
        void Print();
    }
}
