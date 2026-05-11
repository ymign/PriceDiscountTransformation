using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord.Visit
{
    public class ReceiveSMS : Case.CaseInfo
    {
        #region 变量
        /// <summary>
        /// (mo,mt) //短信类型
        /// </summary>
        private string type;

        /// <summary>
        /// 手机号码
        /// </summary>
        private string mobile;

        /// <summary>
        /// mt短信id
        /// </summary>
        private long smid;

        /// <summary>
        /// mo短信id
        /// </summary>
        private long srcid;

        /// <summary>
        /// 回执编码
        /// </summary>
        private int code;

        /// <summary>
        /// 回执描述或短息内容
        /// </summary>
        private string content;

        /// <summary>
        /// 短信时间
        /// </summary>
        private string receivetime;

        /// <summary>
        /// mo短信编码格式
        /// </summary>
        private string msgFmt;

        /// <summary>
        /// 扩展字段
        /// </summary>
        private string expend1;

        /// <summary>
        /// 扩展字段
        /// </summary>
        private string expend2;

        /// <summary>
        /// 扩展字段
        /// </summary>
        private string expend3;

        /// <summary>
        /// 操作员
        /// </summary>
        private string opercode;

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operdate;

        #endregion

        #region 属性

        /// <summary>
        /// (mo,mt) //短信类型
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        } 
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile
        {
            get { return mobile; }
            set { mobile = value; }
        }
        /// <summary>
        /// mt短信id
        /// </summary>
        public long Smid
        {
            get { return smid; }
            set { smid = value; }
        } 
        /// <summary>
        /// mo短信id
        /// </summary>
        public long Srcid
        {
            get { return srcid; }
            set { srcid = value; }
        } 
        /// <summary>
        /// 回执编码
        /// </summary>
        public int Code
        {
            get { return code; }
            set { code = value; }
        } 
        /// <summary>
        /// 回执描述或短息内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        /// <summary>
        /// 短信时间
        /// </summary>
        public string Receivetime
        {
            get { return receivetime; }
            set { receivetime = value; }
        }

        /// <summary>
        /// mo短信编码格式
        /// </summary>
        public string MsgFmt
        {
            get { return msgFmt; }
            set { msgFmt = value; }
        }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Expend1
        {
            get { return expend1; }
            set { expend1 = value; }
        }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Expend2
        {
            get { return expend2; }
            set { expend2 = value; }
        }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Expend3
        {
            get { return expend3; }
            set { expend3 = value; }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Opercode
        {
            get { return opercode; }
            set { opercode = value; }
        }


        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Operdate
        {
            get { return operdate; }
            set { operdate = value; }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>联系方式</returns>
        public new ReceiveSMS Clone()
        {
            ReceiveSMS sms = base.Clone() as ReceiveSMS;

            return sms;
        }

        #endregion
    }
}
