using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.BizLogic.Privilege.Model
{
    /// <summary>
    /// 限制用户登录
    /// </summary>
    [Serializable]
    public class LimitUserLogin : NeuObject
    {
        /// <summary>
        /// 员工编码
        /// </summary>
        private string empl_code = "";

        /// <summary>
        /// 员工编码
        /// </summary>
        public string Empl_code
        {
            get { return empl_code; }
            set { empl_code = value; }
        }

        /// <summary>
        /// 员工姓名
        /// </summary>
        private string empl_name = "";

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Empl_name
        {
            get { return empl_name; }
            set { empl_name = value; }
        }

        /// <summary>
        /// 登录时间
        /// </summary>
        private string login_time = "";

        /// <summary>
        /// 登录时间
        /// </summary>
        public string Login_time
        {
            get { return login_time; }
            set { login_time = value; }
        }

        /// <summary>
        /// 规定时间内
        /// </summary>
        private string limitlogin_time = "";

        /// <summary>
        /// 规定时间内
        /// </summary>
        public string Limitlogin_time
        {
            get { return limitlogin_time; }
            set { limitlogin_time = value; }
        }

        /// <summary>
        /// 重新登录时间
        /// </summary>
        private string relogin_time = "";

        /// <summary>
        /// 重新登录时间
        /// </summary>
        public string Relogin_time
        {
            get { return relogin_time; }
            set { relogin_time = value; }
        }

        /// <summary>
        /// 错误密码次数
        /// </summary>
        private int times = 1;

        /// <summary>
        /// 错误密码次数
        /// </summary>
        public int Times
        {
            get { return times; }
            set { times = value; }
        }

        /// <summary>
        /// 限制登录标记
        /// </summary>
        private string lockflag = "0";

        /// <summary>
        /// 限制登录标记
        /// </summary>
        public string Lockflag
        {
            get { return lockflag; }
            set { lockflag = value; }
        }


    }
}
