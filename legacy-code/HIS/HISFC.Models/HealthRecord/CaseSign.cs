using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.HealthRecord
{
    /// <summary>
    /// 病案签名记录
    /// </summary>
    [Serializable]

    public class CaseSign : NeuObject
    {
        
        /// <summary>
        /// 住院流水号
        /// </summary>
        private string inptient_no = "";
        /// <summary>
        /// 住院流水号
        /// </summary>
        public string Inptient_no
        {
            get { return inptient_no; }
            set { inptient_no = value; }
        }

        /// <summary>
        /// 签名类型
        /// </summary>
        private string sign_type = "";
        /// <summary>
        /// 签名类型
        /// </summary>
        public string Sign_type
        {
            get { return sign_type; }
            set { sign_type = value; }
        }


        /// <summary>
        /// 签名工号
        /// </summary>
        private string confirm_oper = "";
        /// <summary>
        /// 签名工号
        /// </summary>
        public string Confirm_oper
        {
            get { return confirm_oper; }
            set { confirm_oper = value; }
        }

        /// <summary>
        /// 签名姓名
        /// </summary>
        private string confirm_name = "";
        /// <summary>
        /// 签名姓名
        /// </summary>
        public string Confirm_name
        {
            get { return confirm_name; }
            set { confirm_name = value; }
        }

        /// <summary>
        /// 确认时间
        /// </summary>
        private DateTime confirm_date = DateTime.Now;
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime Confirm_date
        {
            get { return confirm_date; }
            set { confirm_date = value; }
        }

        /// <summary>
        /// 解签工号
        /// </summary>
        private string cancel_oper = "";
        /// <summary>
        /// 解签工号
        /// </summary>
        public string Cancel_oper
        {
            get { return cancel_oper; }
            set { cancel_oper = value; }
        }

        /// <summary>
        /// 解签姓名
        /// </summary>
        private string cancel_name = "";
        /// <summary>
        /// 解签姓名
        /// </summary>
        public string Cancel_name
        {
            get { return cancel_name; }
            set { cancel_name = value; }
        }

        /// <summary>
        ///  取消时间
        /// </summary>
        private DateTime cancel_date = DateTime.Now;
        /// <summary>
        /// 取消时间
        /// </summary>
        public DateTime Cancel_date
        {
            get { return cancel_date; }
            set { cancel_date = value; }
        }

        /// <summary>
        /// 解签姓名
        /// </summary>
        private string state = "";
        /// <summary>
        /// 解签姓名
        /// </summary>
        public string State
        {
            get { return state; }
            set { state = value; }
        }
    }
}
