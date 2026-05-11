using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Message
{
    /// <summary>
    /// 消息机制实体
    /// </summary>
    public class MsgInfo
    {
        /// <summary>
        /// index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 消息编号
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// 发送者编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 发送者姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 发送者科室
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 发送者科室
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 接收者编号
        /// </summary>
        public string UserIdTo { get; set; }
        /// <summary>
        /// 接收者姓名
        /// </summary>
        public string UserNameTo { get; set; }
        /// <summary>
        /// 接收者科室
        /// </summary>
        public string DeptToId { get; set; }
        /// <summary>
        /// 接收者科室
        /// </summary>
        public string DepartmentTo { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        public DateTime ReadTime { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// 详细消息内容
        /// </summary>
        private string messageDetail;
        /// <summary>
        /// 详细消息内容
        /// </summary>
        public string MessageDetail
        {
            get { return messageDetail; }
            set { messageDetail = value; }
        }

        /// <summary>
        /// 消息类型:1危急值消息；P静配审方消息
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool HasRead { get; set; }

        /// <summary>
        /// 是否已发送超时提醒
        /// </summary>
        public bool HasTimeOutAlert { get; set; }
    }
}

