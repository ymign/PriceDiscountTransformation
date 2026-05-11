using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord.Visit
{
    public class SendSMS : Case.CaseInfo
    {
        #region 变量
        /// <summary>
        /// 手机号码组
        /// </summary>
        private string mobiles;

        /// <summary>
        /// 短信内容
        /// </summary>
        private string content;

        /// <summary>
        /// 短信id
        /// </summary>
        private long smid;

        /// <summary>
        /// 终端原地址
        /// </summary>
        private long srcid;

        /// <summary>
        /// wappush短信url
        /// </summary>
        private string url;

        /// <summary>
        /// 定时发送时间(格式yyyy-MM-dd HH-mm-ss,为null时立即发送)
        /// </summary>
        private string sendtime;

        /// <summary>
        /// 住院号
        /// </summary>
        private string patientno;

        /// <summary>
        /// 病历号
        /// </summary>
        private string cardno;

        /// <summary>
        /// 住院流水号
        /// </summary>
        private string inpatientno;

        /// <summary>
        /// 联系者
        /// </summary>
        private long linkwayid;

        /// <summary>
        /// 短信模板
        /// </summary>
        private string models;

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
        /// 手机号码组
        /// </summary>
        public string Mobiles
        {
            get { return mobiles; }
            set { mobiles = value; }
        }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 短信id
        /// </summary>
        public long Smid
        {
            get { return smid; }
            set { smid = value; }
        }

        /// <summary>
        /// 终端原地址
        /// </summary>
        public long Srcid
        {
            get { return srcid; }
            set { srcid = value; }
        }

        /// <summary>
        /// wappush短信url
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>
        /// 定时发送时间(格式yyyy-MM-dd HH-mm-ss,为null时立即发送)
        /// </summary>
        public string Sendtime
        {
            get { return sendtime; }
            set { sendtime = value; }
        }

        /// <summary>
        /// 住院号
        /// </summary>
        public string Patientno
        {
            get { return patientno; }
            set { patientno = value; }
        }

        /// <summary>
        /// 病历号
        /// </summary>
        public string Cardno
        {
            get { return cardno; }
            set { cardno = value; }
        }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string Inpatientno
        {
            get { return inpatientno; }
            set { inpatientno = value; }
        }

        /// <summary>
        /// 联系者
        /// </summary>
        public long Linkwayid
        {
            get { return linkwayid; }
            set { linkwayid = value; }
        }

        /// <summary>
        /// 短信模板
        /// </summary>
        public string Models
        {
            get { return models; }
            set { models = value; }
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
        public new SendSMS Clone()
        {
            SendSMS sms = base.Clone() as SendSMS;

            return sms;
        }

        #endregion 
    }
}
