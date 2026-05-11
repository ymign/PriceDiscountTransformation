using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord.CaseHistory
{
    /// <summary>
    /// 病案回收实体
    /// 后来在东莞发现其实仅有一种超期情况，都记录7天超期  保留原来的10天超期 2011-8-2 by chengym
    /// </summary>
    public class CallBack : Neusoft.FrameWork.Models.NeuObject
    {
        #region variable
        private  Neusoft.HISFC.Models.RADT.PatientInfo patient=new Neusoft.HISFC.Models.RADT.PatientInfo();
        private string isCallback;
        private Neusoft.HISFC.Models.Base.OperEnvironment callbackOper=new Neusoft.HISFC.Models.Base.OperEnvironment();
        private string isDocument;
        private Neusoft.HISFC.Models.Base.OperEnvironment documentOper=new Neusoft.HISFC.Models.Base.OperEnvironment();
        private int sevenDaysTimeout;
        private int fourteenDaysTimeout;
        #endregion

        #region attribute
        /// <summary>
        /// 7天超期
        /// </summary>
        public int SevenDaysTimeout
        {
            get { return sevenDaysTimeout; }
            set { sevenDaysTimeout = value; }
        }
        /// <summary>
        /// 十天超期
        /// </summary>
        public int FourteenDaysTimeout
        {
            get { return fourteenDaysTimeout; }
            set { fourteenDaysTimeout = value; }
        }
        /// <summary>
        /// 患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo Patient
        {
            get { return this.patient; }
            set { this.patient = value; }
        }
        /// <summary>
        /// 回收标志 0未回收 1 回收
        /// </summary>
        public string IsCallback
        {
            get { return isCallback; }
            set { isCallback = value; }
        }

        /// <summary>
        /// 回收人员编码
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment CallbackOper
        {
            get { return this.callbackOper; }
            set { this.callbackOper = value; }
        }
        /// <summary>
        /// 是否归档
        /// </summary>
        public string IsDocument
        {
            get { return isDocument; }
            set { isDocument = value; }
        }

        /// <summary>
        /// 归档人
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment DocumentOper
        {
            get { return this.documentOper; }
            set { this.documentOper = value; }
        }
      
        #endregion

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public new CallBack Clone()
        {
            CallBack cb = base.Clone() as CallBack;
            cb.patient = this.patient.Clone();
            cb.callbackOper = this.callbackOper.Clone();
            cb.documentOper = this.documentOper.Clone();
            return cb;
        }


    }
}
