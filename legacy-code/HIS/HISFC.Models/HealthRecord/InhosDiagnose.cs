using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord
{
    public class InhosDiagnose: Neusoft.HISFC.Models.Base.Spell, Neusoft.HISFC.Models.Base.IValid
    {
          public InhosDiagnose()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //.
        }
        #region 私有变量
        /// <summary>
        /// 患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.Patient patient = new Neusoft.HISFC.Models.RADT.Patient();


        private string patientType;
        public string PatientType
        {
            get { return this.patientType; }
            set { this.patientType = value; }
        }

        /// <summary>
        /// 发生序号(10位整数)
        /// </summary>
        private string orderNO;
       
        /// <summary>
        /// ICD10
        /// </summary>
        private ICD icd10 = new ICD();
        /// <summary>
        /// 诊断时间
        /// </summary>
        private DateTime firstDiagDate;

        /// <summary>
        /// 修改诊断时间
        /// </summary>
        private DateTime updateDate;
        /// <summary>
        /// 诊断医生
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject doctor = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 诊断科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject dept = new Neusoft.FrameWork.Models.NeuObject();
 
        /// <summary>
        /// 是否有效
        /// </summary>
        private bool isValid;

        private DateTime cancelDate;

        private DateTime diagDate;

        private DateTime operDate;

        private string mainFlag;

        private string prefix;
        private string suffix;

        private Neusoft.FrameWork.Models.NeuObject inhosDiag = new Neusoft.FrameWork.Models.NeuObject();
        private Neusoft.FrameWork.Models.NeuObject diagType = new Neusoft.FrameWork.Models.NeuObject();
        #endregion

        #region 属性
        public DateTime OperDate
        {
            get { return this.operDate; }
            set { this.operDate = value; }
        }

        public Neusoft.FrameWork.Models.NeuObject InhosDiag
        {
            get { return this.inhosDiag; }
            set { this.inhosDiag = value; }

        }

        public string Prefix
        {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        public string Suffix
        {
            get { return this.suffix; }
            set { this.suffix = value; }
        }

        public DateTime CancelDate
        {
            get { return this.cancelDate; }
            set { this.cancelDate = value; }
        }


        /// <summary>
        /// 患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.Patient Patient
        {
            get
            {
                return patient;
            }
            set
            {
                patient = value;
            }
        }
        /// <summary>
        /// 发生序号(10位整数)
        /// </summary>
        public string OrderNO
        {
            get
            {
                return orderNO;
            }
            set
            {
                orderNO = value;
            }
        }
        /// <summary>
        /// 诊断类别
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject DiagType
        {
            get
            {
                return diagType;
            }
            set
            {
                diagType = value;
            }
        }
        /// <summary>
        /// ICD10
        /// </summary>
        public ICD ICD10
        {
            get
            {
                return icd10;
            }
            set
            {
                icd10 = value;
            }
        }
        
        /// <summary>
        /// 诊断时间
        /// </summary>
        public DateTime DiagDate
        {
            get
            {
                return diagDate;
            }
            set
            {
                diagDate = value;
            }
        }
        /// <summary>
        /// 诊断医生
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Doctor
        {
            get
            {
                return doctor;
            }
            set
            {
                doctor = value;
            }
        }
        /// <summary>
        /// 诊断科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Dept
        {
            get
            {
                return dept;
            }
            set
            {
                dept = value;
            }
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }
        /// <summary>
        /// 是否主诊断
        /// </summary>
        public string MainFlag
        {
            get
            {
                return mainFlag;
            }
            set
            {
                mainFlag = value;
            }
        }

        #endregion

        #region 函数
        public new DiagnoseBase Clone()
        {
            DiagnoseBase obj = base.Clone() as DiagnoseBase;
            //obj.patient = patient.Clone();
            //obj.DiagType = DiagType.Clone();
            obj.ICD10 = ICD10.Clone();
            obj.Dept = Dept.Clone();
            obj.Doctor = Doctor.Clone();
            return obj;
        }
        #endregion

      

        #region IValid 成员

        bool Neusoft.HISFC.Models.Base.IValid.IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }

        #endregion
    }
}
