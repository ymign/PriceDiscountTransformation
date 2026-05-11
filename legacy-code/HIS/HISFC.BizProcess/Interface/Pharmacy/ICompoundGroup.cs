using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace Neusoft.HISFC.BizProcess.Interface.Pharmacy
{
    /// <summary>
    /// 配液中心分批次算法接口
    /// </summary>
    public interface ICompoundGroup
    {
        /// <summary>
        /// 得到根据指定算法计算出的批次号
        /// </summary>
        /// <param name="info">药品申请实体</param>
        /// <returns>返回批次号</returns>
        string GetCompoundGroup(Neusoft.HISFC.Models.Order.ExecOrder exeOrder);
        /// <summary>
        /// 批次号赋值
        /// </summary>
        /// <param name="List">申请实体数组</param>
        /// <param name="List">错误信息</param>
        /// <returns></returns>
        int GetCompoundGroup(ArrayList List, ref string err);
    }
}
