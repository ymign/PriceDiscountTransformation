using System;
using System.Collections.Generic;

using System.Text;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.RADT;
namespace Neusoft.HISFC.Models.PacsApply
{
    public class PacsApplication : Neusoft.FrameWork.Models.NeuObject
    {




        private string hisPatitentID;
        /// <summary>
        ///患者流水号
        /// </summary>
        public String HisPatitentID
        {
            get
            {
                return hisPatitentID;
            }
            set
            {
                this.hisPatitentID = value;
            }
        }

        private string hisPatitentState;
        /// <summary>
        ///患者类型标记（0：门诊 1：住院2：体检）；
        /// </summary>
        public String HisPatitentState
        {
            get
            {
                return hisPatitentState;
            }
            set
            {
                this.hisPatitentState = value;
            }
        }

        private NeuObject applicationType = new NeuObject();
        /// <summary>
        ///申请单类型
        /// </summary>
        public NeuObject ApplicationType
        {
            get
            {
                return applicationType;
            }
            set
            {
                this.applicationType = value;
            }
        }

        private PatientInfo patient = new PatientInfo();
        /// <summary>
        ///患者信息
        /// </summary>
        public PatientInfo Patient
        {
            get
            {
                return patient;
            }
            set
            {
                this.patient = value;
            }
        }

        private string diagnose;
        /// <summary>
        ///申请单诊断,即检查诊断
        /// </summary>
        public String Diagnose
        {
            get
            {
                return diagnose;
            }
            set
            {
                this.diagnose = value;
            }
        }

        private string abstracthistory;
        /// <summary>
        ///申请单病历史，分别针对不同的申请所对应的病史。
        /// </summary>
        public String Abstracthistory
        {
            get
            {
                return abstracthistory;
            }
            set
            {
                this.abstracthistory = value;
            }
        }

        private string applicationCost;
        /// <summary>
        ///申请单费用
        /// </summary>
        public String ApplicationCost
        {
            get
            {
                return applicationCost;
            }
            set
            {
                this.applicationCost = value;
            }
        }

        private string isFee;
        /// <summary>
        ///是否收费（0：否，1：是）
        /// </summary>
        public String IsFee
        {
            get
            {
                return isFee;
            }
            set
            {
                this.isFee = value;
            }
        }



        private NeuObject applicationDoc = new NeuObject();
        /// <summary>
        ///申请医生
        /// </summary>
        public NeuObject ApplicationDoc
        {
            get
            {
                return applicationDoc;
            }
            set
            {
                this.applicationDoc = value;
            }
        }

        private NeuObject operDoc = new NeuObject();
        /// <summary>
        ///操作（确认）医生
        /// </summary>
        public NeuObject OperDoc
        {
            get
            {
                return operDoc;
            }
            set
            {
                this.operDoc = value;
            }
        }

        private NeuObject execDept = new NeuObject();
        /// <summary>
        ///执行科室
        /// </summary>
        public NeuObject ExecDept
        {
            get
            {
                return execDept;
            }
            set
            {
                this.execDept = value;
            }
        }

        private string useTime;
        /// <summary>
        ///检查消耗时间
        /// </summary>
        public string UseTime
        {
            get
            {
                return useTime;
            }
            set
            {
                this.useTime = value;
            }
        }

        private DateTime operDate;
        /// <summary>
        ///操作时间
        /// </summary>
        public DateTime OperDate
        {
            get
            {
                return operDate;
            }
            set
            {
                this.operDate = value;
            }
        }

        private string getPerson;
        /// <summary>
        ///取报告人
        /// </summary>
        public String GetPerson
        {
            get
            {
                return getPerson;
            }
            set
            {
                this.getPerson = value;
            }
        }

        private string getPersonRel;
        /// <summary>
        ///取报告人关系
        /// </summary>
        public String GetPersonRel
        {
            get
            {
                return getPersonRel;
            }
            set
            {
                this.getPersonRel = value;
            }
        }

        private DateTime reserveTime;
        /// <summary>
        ///预订时间
        /// </summary>
        public DateTime ReserveTime
        {
            get
            {
                return reserveTime;
            }
            set
            {
                this.reserveTime = value;
            }
        }

        private List<PacsItem> pactItems;
        /// <summary>
        ///项目列表
        /// </summary>
        public List<PacsItem> PactItems
        {
            get
            {
                return pactItems;
            }
            set
            {
                this.pactItems = value;
            }
        }

        private string state;
        /// <summary>
        ///申请单状态
        /// </summary>
        public String State
        {
            get
            {
                return state;
            }
            set
            {
                this.state = value;
            }
        }


        private string fileID;

        /// <summary>
        /// 电子病历里文件ID
        /// </summary>
        public string FileID
        {
            get { return this.fileID; }
            set { this.fileID = value; }
        }


        private string consulatin;

        /// <summary>
        /// 会诊目的
        /// </summary>
        public string Consulatin
        {
            get { return this.consulatin; }
            set { this.consulatin = value; }
        }


        private string otherCheck;

        /// <summary>
        ///相关检查
        /// </summary>
        public string OtherCheck
        {
            get { return this.otherCheck; }
            set { this.otherCheck = value; }
        }


        private string applyMemo;

        /// <summary>
        /// 申请单备注
        /// </summary>
        public string ApplyMemo
        {
            get { return this.applyMemo; }
            set { this.applyMemo = value; }
        }
        private string strPatientCC;

        /// <summary>
        /// 申请单备注 cheng {1525D5E5-B266-4292-A6B3-2C08229621AE}
        /// </summary>
        public string StrPatientCC
        {
            get { return this.strPatientCC; }
            set { this.strPatientCC = value; }
        }

    }
}
