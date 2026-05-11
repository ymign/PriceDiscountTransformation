using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Message
{
    /// <summary>
    /// 消息机制接口
    /// </summary>
    public interface IMessage:IDisposable
    {
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userId">登录名</param>
        /// <param name="password">密码（明文）</param>
        /// <param name="username">姓名</param>
        /// <param name="sex">性别</param>
        /// <param name="mobile">手机</param>
        /// <param name="department">科室</param>
        /// <returns>发送成功返回空字符串，失败返回错误信息</returns>
        string Register(string userId, string password, string username, string sex, string mobile, string department);

        /// <summary>
        /// 读取某个用户的所有未读消息
        /// </summary>
        /// <param name="userid">用户编号</param>
        /// <returns>消息列表</returns>
        List<Neusoft.HISFC.Models.Message.MsgInfo> ReceiveMessage(string userid,string deptCode);

        /// <summary>
        /// 设置某个消息已读
        /// </summary>
        /// <param name="msgid">消息ID</param>
        bool SetMessageReaded(long msgid);

        /// <summary>
        /// 消息提醒函数
        /// </summary>
        void ShowMessage();


        /// <summary>
        /// 读取某个科室所有未读消息
        /// </summary>
        /// <param name="deptId">部门Id</param>
        /// <returns>消息列表</returns>
        List<Neusoft.HISFC.Models.Message.MsgInfo> ReceiveMessageByDept(string deptId,bool isRead);
    }
}
