using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    public interface IOutpatientEvaluation
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
        /// 工作评价
        /// </summary>
        /// <param name="szDoctorName">操作员名称</param>
        /// <param name="szOfisName">科室名称</param>
        /// <param name="szSickCard">社保卡号(CardNO)</param>
        /// <param name="szSickName">患者姓名</param>
        /// <returns></returns>
        bool WorkAppriaseEvaluation(string szDoctorName, string szOfisName, string szSickCard, string szSickName);

        /// <summary>
        /// 工作评价
        /// </summary>
        /// <param name="szSickCard">社保卡号(CardNO)</param>
        /// <param name="szSickName">患者姓名</param>
        /// <returns></returns>
        bool WorkAppriaseEvaluation(string szSickCard, string szSickName);
    }
}
