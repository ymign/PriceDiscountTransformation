using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Order;

namespace HISCommonClient.Message
{
    public class MessageService
    {
        #region 变量/属性

        /// <summary>
        /// 服务实例
        /// </summary>
        SOAPMsgSer.MessageService service;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public MessageService()
        {
            service = new HISCommonClient.SOAPMsgSer.MessageService();
        }

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
        public string Register(string userId, string password, string username, string sex, string mobile, string department)
        {
            return service.RegisterUser(userId, password, username, sex, mobile, department);
        }

        /// <summary>
        /// 读取某个用户的所有未读消息
        /// </summary>
        /// <param name="userid">用户编号</param>        
        /// <param name="dept">部门</param>
        /// <returns>消息列表</returns>
        public List<Neusoft.HISFC.Models.Message.MsgInfo> ReceiveMessage(string userid, string dept)
        {
            List<Neusoft.HISFC.Models.Message.MsgInfo> list = new List<Neusoft.HISFC.Models.Message.MsgInfo>(); SOAPMsgSer.MsgInfo[] msgs = service.ReceiveMessage(userid, dept);

            foreach (SOAPMsgSer.MsgInfo item in msgs)
            {
                Neusoft.HISFC.Models.Message.MsgInfo info = new Neusoft.HISFC.Models.Message.MsgInfo();
                info.Department = item.Department;
                info.DepartmentTo = item.DepartmentTo;
                info.HasRead = item.HasRead;
                info.HasTimeOutAlert = item.HasTimeOutAlert;
                info.MessageContent = item.MessageContent;
                info.MessageId = item.MessageId;
                info.MessageTitle = item.MessageTitle;
                info.MessageType = item.MessageType;
                info.ReadTime = item.ReadTime;
                info.SendTime = item.SendTime;
                info.UserId = item.UserId;
                info.UserIdTo = item.UserIdTo;
                info.UserName = item.UserName;
                info.UserNameTo = item.UserNameTo;
                info.DeptId = item.DeptCode;
                info.DeptToId = item.DeptToCode;
                list.Add(info);
            }
            return list;
        }

        /// <summary>
        /// 设置某个消息已读
        /// </summary>
        /// <param name="msgid">消息ID</param>
        public bool SetMessageReaded(long msgid)
        {
            return service.SetMessageReaded(msgid);
        }

        /// <summary>
        /// 根据部门编码获取所有未读信息。
        /// </summary>
        /// <param name="dept">科室id</param>
        /// <param name="isRead">0未读,1已读</param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.Message.MsgInfo> ReceiveMessageByDept(string dept, int isRead)
        {
            List<Neusoft.HISFC.Models.Message.MsgInfo> list = new List<Neusoft.HISFC.Models.Message.MsgInfo>();
            SOAPMsgSer.MsgInfo[] msgs = service.GetAllMessageByDept(dept, isRead);

            foreach (SOAPMsgSer.MsgInfo item in msgs)
            {
                Neusoft.HISFC.Models.Message.MsgInfo info = new Neusoft.HISFC.Models.Message.MsgInfo();
                info.Department = item.Department;
                info.DepartmentTo = item.DepartmentTo;
                info.HasRead = item.HasRead;
                info.HasTimeOutAlert = item.HasTimeOutAlert;
                info.MessageContent = item.MessageContent;
                info.MessageId = item.MessageId;
                info.MessageTitle = item.MessageTitle;
                info.MessageType = item.MessageType;
                info.ReadTime = item.ReadTime;
                info.SendTime = item.SendTime;
                info.UserId = item.UserId;
                info.DeptId = item.DeptCode;
                info.DeptToId = item.DeptToCode;
                info.UserIdTo = item.UserIdTo;
                info.UserName = item.UserName;
                info.UserNameTo = item.UserNameTo;
                list.Add(info);
            }

            return list;
        }
    }
}
