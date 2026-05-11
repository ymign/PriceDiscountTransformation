using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Nurse
{
    /// <summary>
    /// 治疗单打印
    /// </summary>
    public interface ITreatmentPrint
    {
        /// <summary>
        /// 初始化
        /// </summary>
        int Init();

        /// <summary>
        /// 加载数据（门诊费用明细）
        /// </summary>
        /// <param name="feeItemList"></param>
        int SetData(ArrayList feeItemList);

        /// <summary>
        /// 打印
        /// </summary>
        void PrintBill();
    }
}
