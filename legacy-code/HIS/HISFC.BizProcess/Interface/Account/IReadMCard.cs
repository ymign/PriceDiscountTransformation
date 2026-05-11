using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Account
{
    /// <summary>
    /// 读取医保卡信息接口
    /// </summary>
    public interface IReadMCard
    {

        /// <summary>
        /// 获取社保卡信息
        /// </summary>
        /// <param name="cardId">卡识别码</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="cardIssueDate">发卡日期</param>
        /// <param name="userId">身份证号码</param>
        /// <param name="userName">姓名</param>
        /// <param name="userSex">性别</param>
        /// <param name="userPhoneNumber">联系电话</param>
        /// <param name="dtBirth">生日</param>
        /// <param name="errInfo">错误信息</param>
        /// <returns>0找不到读卡动态库，-1打开设备失败，-2关闭设备失败，-3读取社保卡信息失败，1成功</returns>
        int GetMCardInfo(ref string cardId, ref string cardNo, ref DateTime cardIssueDate,
            ref string userId, ref string userName, ref string userSex, ref string userPhoneNumber, ref DateTime dtBirth, ref string errInfo);

    }
}
