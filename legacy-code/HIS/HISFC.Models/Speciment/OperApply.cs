using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 手术申请单实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2010-02-22]<br></br>
    /// Table : SPEC_OPERAPPLY
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class OperApply : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int operApplyId = 0;
        private string operName = "";
        private string operId = "";
        private string operdeptName = "";
        private string operDeptId = "";
        private string inhosNum = "";
        private string patientName = "";
        private MedDoc medDoc = new MedDoc();
        private DateTime operTime = DateTime.Now;
        private string hadCollect = "0";
        private DateTime colTime = DateTime.MinValue;
        private string noColReason = "";
        private string hadOperInfo = "0";
        private DateTime getOperInfoTime = DateTime.MinValue;
        private string getPeriod = "";
        private string mainDiaICD = "";
        private string mainDiaName = "";
        private string mainDiaICD1 = "";
        private string mainDiaICD2 = "";
        private string mainDiaName1 = "";
        private string mainDiaName2 = "";
        private string comment = "";
        private string operLocation = "";
        private string tumorPor = "";
        private string operPosId = "";
        private string operPosName = "";
        private string orgOrB = "";
        private string orderId = "";
        #endregion
        
        #region 属性
        /// <summary>
        /// Id
        /// </summary>
        public int OperApplyId
        {
            get
            {
                return operApplyId;
            }
            set
            {
                operApplyId = value;
            }
        }

        /// <summary>
        /// 手术名称
        /// </summary>
        public string OperName
        {
            get
            {
                return operName;
            }
            set
            {
                operName = value;
            }
        }

        /// <summary>
        /// 手术申请Id
        /// </summary>
        public string OperId
        {
            get
            {
                return operId;
            }
            set
            {
                operId = value;
            }
        }

        /// <summary>
        /// 手术申请科室Id
        /// </summary>
        public string OperDeptId
        {
            get
            {
                return operDeptId;
            }
            set
            {
                operDeptId = value;
            }
        }

        /// <summary>
        /// 手术科室名称
        /// </summary>
        public string OperDeptName
        {
            get
            {
                return operdeptName;
            }
            set
            {
                operdeptName = value;
            }
        }

        /// <summary>
        /// 病人住院号
        /// </summary>
        public string InHosNum
        {
            get
            {
                return inhosNum;
            }
            set
            {
                inhosNum = value;
            }
        }

        /// <summary>
        /// 病人姓名
        /// </summary>
        public string PatientName
        {
            get
            {
                return patientName;
            }
            set
            {
                patientName = value;
            }
        }

        /// <summary>
        /// 手术医生
        /// </summary>
        public MedDoc MediDoc
        {
            get
            {
                return medDoc;
            }
            set
            {
                medDoc = value;
            }
        }

        /// <summary>
        /// 手术时间    
        /// </summary>
        public DateTime OperTime
        {
            get
            {
                return operTime;
            }
            set
            {
                operTime = value;
            }
        }

        /// <summary>
        /// 是否采集标本：0，未采集，1：已采集
        /// </summary>
        public string HadCollect
        {
            get
            {
                return hadCollect;
            }
            set
            {
                hadCollect = value;
            }
        }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime ColTime
        {
            get
            {
                return colTime;
            }
            set
            {
                colTime = value;
            }
        }

        /// <summary>
        /// 没有采集到的原因
        /// </summary>
        public string NoColReason
        {
            get
            {
                return noColReason;
            }
            set
            {
                noColReason = value;
            }
        }

        /// <summary>
        /// 是否录入手术信息：0，未采集，1：已采集
        /// </summary>
        public string HadOperInfo
        {
            get
            {
                return hadOperInfo;
            }
            set
            {
                hadOperInfo = value;
            }
        }

        /// <summary>
        /// 录入手术信息的时间
        /// </summary>
        public DateTime GetOperInfoTime
        {
            get
            {
                return getOperInfoTime;
            }
            set
            {
                getOperInfoTime = value;
            }
        }

        /// <summary>
        /// 标本取自病人的哪个阶段：放疗前，放疗后等等
        /// </summary>
        public string GetPeriod
        {
            get
            {
                return getPeriod;
            }
            set
            {
                getPeriod = value;
            }
        }

        /// <summary>
        /// 主诊断ICD
        /// </summary>
        public string MainDiaICD
        {
            get
            {
                return mainDiaICD;
            }
            set
            {
                mainDiaICD = value;
            }
        }

        /// <summary>
        /// 主诊断ICD名称
        /// </summary>
        public string MainDiaName
        {
            get
            {
                return mainDiaName;
            }
            set
            {
                mainDiaName = value;
            }
        }

        /// <summary>
        /// 主诊断ICD1
        /// </summary>
        public string MainDiaICD1
        {
            get
            {
                return mainDiaICD1;
            }
            set
            {
                mainDiaICD1 = value;
            }
        }

        /// <summary>
        /// 主诊断ICD1名称
        /// </summary>
        public string MainDiaName1
        {
            get
            {
                return mainDiaName1;
            }
            set
            {
                mainDiaName1 = value;
            }
        }

        /// <summary>
        /// 主诊断ICD2
        /// </summary>
        public string MainDiaICD2
        {
            get
            {
                return mainDiaICD2;
            }
            set
            {
                mainDiaICD2 = value;
            }
        }

        /// <summary>
        /// 主诊断ICD2名称
        /// </summary>
        public string MainDiaName2
        {
            get
            {
                return mainDiaName2;
            }
            set
            {
                mainDiaName2 = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }

        /// <summary>
        /// 手术室
        /// </summary>
        public string OperLocation
        {
            get
            {
                return operLocation;
            }
            set
            {
                operLocation = value;
            }
        }

        /// <summary>
        /// 标本属性1.原发癌 2.复发癌 3.转移癌
        /// </summary>
        public string TumorPor
        {
            get
            {
                return tumorPor;
            }
            set
            {
                tumorPor = value;
            }
        }

        /// <summary>
        /// 手术部位代码
        /// </summary>
        public string OperPosId
        {
            get
            {
                return operPosId;
            }
            set
            {
                operPosId = value;
            }
        }

        /// <summary>
        /// 手术部位名称
        /// </summary>
        public string OperPosName
        {
            get
            {
                return operPosName;
            }
            set
            {
                operPosName = value;
            }
        }

        public string OrgOrBlood
        {
            get
            {
                return orgOrB;
            }
            set
            {
                orgOrB = value;
            }
        }

        public string OrderId
        {
            get
            {
                return orderId;
            }
            set
            {
                orderId = value;
            }
        }
        #endregion

        #region 方法
        public new OperApply Clone()
        {
            OperApply oper = base.Clone() as OperApply;
            oper.MediDoc = this.MediDoc.Clone();
            return oper;

        }
        #endregion
    }
}
