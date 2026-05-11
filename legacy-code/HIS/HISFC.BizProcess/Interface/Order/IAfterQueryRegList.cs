using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// 门诊医生站输入卡号查询挂号列表后操作
    /// </summary>
    public interface IAfterQueryRegList
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
        /// 查询患者挂号列表后操作
        /// </summary>
        /// <param name="alReginfo">有效挂号列表</param>
        /// <param name="dept">当前登录科室</param>
        /// <returns></returns>
        int OnAfterQueryRegList( ArrayList alReginfo, Neusoft.FrameWork.Models.NeuObject dept);


        /// <summary>
        /// 医生站自助挂号后操作
        /// </summary>
        /// <param name="regInfo"></param>
        /// <returns></returns>
        int OnConfirmRegInfo(Neusoft.HISFC.Models.Registration.Register regInfo);
    }
}
