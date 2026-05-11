using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Pharmacy
{
    /// <summary>
    /// 配液中心项目判断接口
    /// </summary>
    public interface ICompoundJudge
    {
        /// <summary>
        /// 根据出库申请实体判断该项目是否需要发送到配置中心
        /// </summary>
        /// <param name="applyOut"></param>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        int SetCompoundApply(Neusoft.HISFC.Models.Pharmacy.ApplyOut applyOut, ref string errInfo);

        /// <summary>
        /// 获取组合项目列表
        /// </summary>
        /// <returns></returns>
        int GetComboItems(ArrayList execOrderList, DateTime dtNow, ref Hashtable hsComb);

        /// <summary>
        /// 修改需配置的医嘱扣库科室为配置中心
        /// </summary>
        /// <param name="execOrderList"></param>
        /// <returns></returns>
        int ConfigComoundDept(ref ArrayList execOrderList, ref string errInfo);
    }
}
