using System;
using Neusoft.HISFC.Models.Base;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.RADT
{
	/// <summary>
	/// PVisit <br></br>
	/// [功能描述: 患者访问实体]<br></br>
	/// [创 建 者: 李云凡]<br></br>
	/// [创建时间: 2004-05]<br></br>
	/// <修改记录
	///		修改人='赫一阳'
	///		修改时间='2006-09-11'
	///		修改目的='版本整合'
	///		修改描述=''
	///  />
	/// </summary>
    ///	 <修改记录
    ///		修改人='蒋飞'
    ///		修改时间='2007-09-05'
    ///		修改目的='版本整合'
    ///		修改描述='增加科主任attendingDirector'
    ///  />
    /// </summary>
    [Serializable]
    public class PVisit : Neusoft.FrameWork.Models.NeuObject
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public PVisit()
		{
		}

		#region 变量

		/// <summary>
		/// 患者类别
		/// </summary>
        private PatientTypeEnumService enumPatientType;

		/// <summary>
		/// 医疗类别
		/// </summary>
        private NeuObject medicalType;

		/// <summary>
		/// 患者位置
		/// </summary>
        private Location patientLocation;

		/// <summary>
		/// 主治医生 
		/// </summary>
        private NeuObject attendingDoctor;

        /// <summary>
        /// 科主任
        /// </summary>
        private NeuObject attendingDirector;
                
		/// <summary>
		/// 副主任医师
		/// </summary>
        private NeuObject referringDoctor;

		/// <summary>
		/// 主任医师
		/// </summary>
        private NeuObject consultingDoctor;

        /// <summary>
        /// 住培学员
        /// </summary>
        private NeuObject residentTraineesDoctor;

		/// <summary>
		/// 责任医师，住院医师 门诊为挂号医师
		/// </summary>
        private NeuObject admittingDoctor;

		/// <summary>
		/// 实习医生 
		/// </summary>
        private NeuObject tempDoctor;

		/// <summary>
		/// 转入/转出科室 
		/// </summary>
        private Location temporaryLocation;

		/// <summary>
		/// 在ICU位置
		/// </summary>
        private Location icuLocation;

		/// <summary>
		/// 入院途径
		/// </summary>
        private NeuObject admitSource;

		/// <summary>
		/// 入院来源
		/// </summary>
        private NeuObject inSource;

		/// <summary>
		/// 入院情况
		/// </summary>
        private NeuObject circs;

		/// <summary>
		/// 责任护士
		/// </summary>
        private Models.Base.Employee admittingNurse;

		/// <summary>
		/// 费用状态 自费，公费，合同...
		/// </summary>
        private NeuObject accountStatus;

		/// <summary>
		/// 住院状态 0-住院登记  1-病房接诊 2-出院登记 3-出院结算 4-预约出院,5-无费退院
		/// </summary>
        private InStateEnumService inState;

		/// <summary>
		/// 住院日期
		/// </summary>
		private DateTime inTime;

		/// <summary>
		/// 出院日期
		/// </summary>
		private DateTime outTime;

		/// <summary>
		/// 预约出院日期
		/// </summary>
		private DateTime preOutTime;

		/// <summary>
		/// 注册日期
		/// </summary>
		private DateTime registTime;

		/// <summary>
		/// 警戒线
		/// </summary>
		private decimal moneyAlert;

		/// <summary>
		/// 午别
		/// </summary>
        private NeuObject noon;

		/// <summary>
		/// 看诊序号
		/// </summary>
		private string seeNO;

		/// <summary>
		/// 转归情况
		/// </summary>
        private NeuObject zg;

        /// <summary>
        /// 医保类型：0普通  1肿瘤 2恶性肿瘤  3ICU   4内镜   5眼科    6骨科指定手术  7生育保险
        /// </summary>
        private string healthCareType;

		/// <summary>
		/// 膳食结算状态
		/// </summary>
        private string boardState;

        /// <summary>
        /// 警戒线设置类别
        /// </summary>
        private EnumAlertTypeService alertType;

        /// <summary>
        /// 警戒线时间段设置开始时间
        /// </summary>
        private DateTime beginDate = DateTime.MinValue;

        /// <summary>
        /// 警戒线时间段设置结束时间
        /// </summary>
        private DateTime endDate = DateTime.MinValue;

        #region 增加警戒线批准人和批准时间
        //{6D09DD27-A256-44ad-9D89-33DF0B8DF9FA}
        /// <summary>
        /// 批准人
        /// </summary>
        private NeuObject approveOper;

        /// <summary>
        /// 批准时间
        /// </summary>
        private DateTime approveDate = DateTime.MinValue;
        #endregion 
		#endregion

		#region 属性

		/// <summary>
		/// 患者类别
		/// </summary>
		public PatientTypeEnumService PatientType
		{
			get
			{
                if (enumPatientType == null)
                {
                    enumPatientType = new PatientTypeEnumService();
                }
				return this.enumPatientType;
			}
			set
			{
				this.enumPatientType = value;
			}
		}

		/// <summary>
		/// 医疗类别
		/// </summary>
		public NeuObject MedicalType
		{
			get
			{
                if (medicalType == null)
                {
                    medicalType = new NeuObject();
                }

				return this.medicalType;
			}
			set
			{
				this.medicalType = value;
			}
		}

		/// <summary>
		/// 患者位置
		/// </summary>
		public Location PatientLocation
		{
			get
			{
                if (patientLocation == null)
                {
                    patientLocation = new Location();
                }

				return this.patientLocation;
			}
			set
			{
				this.patientLocation = value;
			}
		}

		/// <summary>
		/// 主治医生
		/// </summary>
		public NeuObject AttendingDoctor
		{
			get
			{
                if (attendingDoctor == null)
                {
                    attendingDoctor = new NeuObject();
                }
				return this.attendingDoctor;
			}
			set
			{
				this.attendingDoctor = value;
			}
		}

        /// <summary>
        /// 科主任
        /// </summary>   
        public NeuObject AttendingDirector
        {
            get
            {
                if (attendingDirector == null)
                {
                    attendingDirector = new NeuObject();
                }
                return attendingDirector;
            }
            set
            {
                attendingDirector = value;
            }
        }

		/// <summary>
		/// 副主任医师
		/// </summary>
		public NeuObject ReferringDoctor
		{
			get
			{
                if (referringDoctor == null)
                {
                    referringDoctor = new NeuObject();
                }
				return this.referringDoctor;
			}
			set
			{
				this.referringDoctor = value;
			}
		}

		/// <summary>
		/// 主任医师
		/// </summary>
		public NeuObject ConsultingDoctor
		{
			get
			{
                if (consultingDoctor == null)
                {
                    consultingDoctor = new NeuObject();
                }

				return this.consultingDoctor;
			}
			set
			{
				this.consultingDoctor = value;
			}
		}

        /// <summary>
		/// 住培学员
		/// </summary>
        public NeuObject ResidentTraineesDoctor
		{
			get
			{
                if (residentTraineesDoctor == null)
                {
                    residentTraineesDoctor = new NeuObject();
                }

                return this.residentTraineesDoctor;
			}
			set
			{
                this.residentTraineesDoctor = value;
			}
		}
        
		/// <summary>
		/// 责任医师，住院医师
		/// </summary>
		public NeuObject AdmittingDoctor
		{
			get
			{
                if (admittingDoctor == null)
                {
                    admittingDoctor = new NeuObject();
                }

				return this.admittingDoctor;
			}
			set
			{
				this.admittingDoctor = value;
			}
		}

		/// <summary>
		/// 实习医生
		/// </summary>
		public NeuObject TempDoctor
		{
			get
			{
                if (tempDoctor == null)
                {
                    tempDoctor = new NeuObject();
                }

				return this.tempDoctor;
			}
			set
			{
				this.tempDoctor = value;
			}
		}

		/// <summary>
		/// 转入/转出科室
		/// </summary>
		public Location TemporaryLocation
		{
			get
			{
                if (temporaryLocation == null)
                {
                    temporaryLocation = new Location();
                }
				return this.temporaryLocation;
			}
			set
			{
				this.temporaryLocation = value;
			}
		}

		/// <summary>
		/// 在ICU位置
		/// </summary>
		public Location ICULocation
		{
			get
			{
                if (icuLocation == null)
                {
                    icuLocation = new Location();
                }
				return this.icuLocation;
			}
			set
			{
				this.icuLocation = value;
			}
		}

		/// <summary>
		/// 入院途径
		/// </summary>
		public NeuObject AdmitSource
		{
			get
			{
                if (admitSource == null)
                {
                    admitSource = new NeuObject();
                }
				return this.admitSource;
			}
			set
			{
				this.admitSource = value;
			}
		}

		/// <summary>
		/// 入院来源
		/// </summary>
		public NeuObject InSource
		{
			get
			{
                if (inSource == null)
                {
                    inSource = new NeuObject();
                }

				return this.inSource;
			}
			set
			{
				this.inSource = value;
			}
		}

		/// <summary>
		/// 入院情况
		/// </summary>
		public NeuObject Circs
		{
			get
			{
                if (circs == null)
                {
                    circs = new NeuObject();
                }

				return this.circs;
			}
			set
			{
				this.circs = value;
			}
		}

		/// <summary>
		/// 责任护士
		/// </summary>
		public Employee AdmittingNurse
		{
			get
			{
                if (admittingNurse == null)
                {
                    admittingNurse = new Employee();
                }
				return this.admittingNurse;
			}
			set
			{
				this.admittingNurse = value;
			}
		}

		/// <summary>
		/// 费用状态 自费，公费，合同...
		/// </summary>
		public NeuObject AccountStatus
		{
			get
			{
                if (accountStatus == null)
                {
                    accountStatus = new NeuObject();
                }
				return this.accountStatus;
			}
			set
			{
				this.accountStatus = value;
			}
		}

		/// <summary>
		/// 住院状态 0-住院登记  1-病房接诊 2-出院登记 3-出院结算 4-预约出院,5-无费退院
		/// </summary>
		public InStateEnumService InState
		{
			get
			{
                if (inState == null)
                {
                    inState = new InStateEnumService();
                }
				return this.inState;
			}
			set
			{
				this.inState = value;
			}
		}

		/// <summary>
		/// 住院日期
		/// </summary>
		public DateTime InTime
		{
			get
			{
				return this.inTime;
			}
			set
			{
				this.inTime = value;
			}
		}

		/// <summary>
		/// 出院日期
		/// </summary>
		public DateTime OutTime
		{
			get
			{
				return this.outTime;
			}
			set
			{
				this.outTime = value;
			}
		}

		/// <summary>
		/// 预约出院日期
		/// </summary>
		public DateTime PreOutTime
		{
			get
			{
				return this.preOutTime;
			}
			set
			{
				this.preOutTime = value;
			}
		}

		/// <summary>
		/// 注册日期
		/// </summary>
		public DateTime RegistTime
		{
			get
			{
				return this.registTime;
			}
			set
			{
				this.registTime = value;
			}
		}

        /// <summary>
        /// 警戒线启用标记
        /// </summary>
        private bool alertFlag = false;

        /// <summary>
        /// 警戒线启用标记
        /// </summary>
        public bool AlertFlag
        {
            get
            {
                return alertFlag;
            }
            set
            {
                alertFlag = value;
            }
        }

		/// <summary>
		/// 警戒线
		/// </summary>
		public decimal MoneyAlert
		{
			get
			{
				return this.moneyAlert;
			}
			set
			{
				this.moneyAlert = value;
			}
		}

		/// <summary>
		/// 午别
		/// </summary>
		public NeuObject Noon
		{
			get
			{
                if (noon == null)
                {
                    noon = new NeuObject();
                }
				return this.noon;
			}
			set
			{
				this.noon = value;
			}
		}

		/// <summary>
		/// 看诊序号
		/// </summary>
		public string SeeNO
		{
			get
			{
				return this.seeNO;
			}
			set
			{
				this.seeNO = value;
			}
		}

		/// <summary>
		/// 转归情况
		/// </summary>
		public NeuObject ZG
		{
			get
			{
                if (zg == null)
                {
                    zg = new NeuObject();
                }
				return this.zg;
			}
			set
			{
				this.zg = value;
			}
		}

        /// <summary>
        /// 医保类型：0普通  1肿瘤 2恶性肿瘤  3ICU   4内镜   5眼科    6骨科指定手术  7生育保险
        /// </summary>
        public string HealthCareType
        {
            get
            {
                return healthCareType;
            }
            set
            {
                this.healthCareType = value;
            }
        }

		/// <summary>
		/// 膳食结算状态
		/// </summary>
		public string BoardState
		{
			get
			{
                if (boardState == null)
                {
                    boardState = string.Empty;
                }
				return this.boardState;
			}
			set
			{
				this.boardState = value;
			}
		}

        /// <summary>
        /// 警戒线设置类别
        /// </summary>
        public EnumAlertTypeService AlertType
        {
            get
            {
                if (alertType == null)
                {
                    alertType = new EnumAlertTypeService();
                }

                return alertType;
            }
            set
            {
                alertType = value;
            }
        }

        /// <summary>
        /// 警戒线时间段设置开始时间
        /// </summary>
        public DateTime BeginDate
        {
            get
            {
                return beginDate;
            }
            set
            {
                beginDate = value;
            }
        }

        /// <summary>
        /// 警戒线时间段设置结束时间
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;
            }
        }

        #region 增加警戒线批准人和批准时间
        //{6D09DD27-A256-44ad-9D89-33DF0B8DF9FA}
        /// <summary>
        /// 批准人
        /// </summary>
        public NeuObject ApproveOper
        {
            get
            {
                if (approveOper == null)
                {
                    approveOper = new NeuObject();
                }
                return approveOper;
            }
            set
            {
                approveOper = value;
            }
        }

        /// <summary>
        /// 批准时间
        /// </summary>
        public DateTime ApproveDate
        {
            get
            {
                return approveDate;
            }
            set
            {
                approveDate = value;
            }
        }
        #endregion
		#endregion

		#region 过期

		/// <summary>
		/// 患者类别
		/// </summary>
        [Obsolete("已经过期，更改为PatientType", true)]
        public EnumPatientType PatientClass
        {
            get
            {
                return  EnumPatientType.C;
            }
            set
            {
            }
        }

		/// <summary>
		/// 在ICU位置
		/// </summary>
        [Obsolete("已经过期，更改为ICULocation", true)]
        private Location IcuLocation;

		/// <summary>
		/// 住院状态 0-住院登记  1-病房接诊 2-出院登记 3-出院结算 4-预约出院,5-无费退院
		/// </summary>
        [Obsolete("已经过期，更改为InState", true)]
        public EnumInState In_State
        {
            get
            {
                return EnumInState.N;
            }
            set
            {
            }
        }

		/// <summary>
		/// 住院日期
		/// </summary>
		[Obsolete("已经过期，更改为InTime", true)]
		public DateTime Date_In;

		/// <summary>
		/// 出院日期
		/// </summary>
		[Obsolete("已经过期，更改为OutTime", true)]
		public DateTime Date_Out;

		/// <summary>
		/// 预约出院日期
		/// </summary>
		[Obsolete("已经过期，更改为preOutTime", true)]
		public DateTime Date_PreOut;

		/// <summary>
		/// 注册日期
		/// </summary>
		[Obsolete("已经过期，更改为RegistTime", true)]
		public DateTime Date_Register;

		/// <summary>
		/// 看诊序号
		/// </summary>
		[Obsolete("已经过期，更改为SeeNO", true)]
		public string SeeNo="0";

		/// <summary>
		/// 转归情况
		/// </summary>
        [Obsolete("已经过期，更改为ZG", true)]
        public NeuObject Zg
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

		#endregion

		#region 方法
		
		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns></returns>
		public new PVisit Clone()
		{
			PVisit pVisit = base.Clone() as PVisit;

			pVisit.MedicalType = this.MedicalType.Clone();
			pVisit.PatientLocation = this.PatientLocation.Clone();
			pVisit.AttendingDoctor = this.AttendingDoctor.Clone();
            pVisit.AttendingDirector = this.AttendingDirector.Clone();
			pVisit.ReferringDoctor = this.ReferringDoctor.Clone();
			pVisit.ConsultingDoctor = this.ConsultingDoctor.Clone();
			pVisit.ResidentTraineesDoctor = this.ResidentTraineesDoctor.Clone();
			pVisit.AdmittingDoctor = this.AdmittingDoctor.Clone();
			pVisit.TempDoctor = this.TempDoctor.Clone();
			pVisit.TemporaryLocation = this.TemporaryLocation.Clone();
			pVisit.AdmitSource = this.AdmitSource.Clone();
			pVisit.InSource = this.InSource.Clone();
			pVisit.Circs = this.Circs.Clone();
			pVisit.AdmittingNurse = this.AdmittingNurse.Clone();
			pVisit.AccountStatus = this.AccountStatus.Clone();
			pVisit.Noon = this.Noon.Clone();
			pVisit.ZG = this.ZG.Clone();
			pVisit.PatientType = this.PatientType.Clone();
            pVisit.alertType = this.AlertType.Clone();
			return pVisit;
		}

		#endregion
	}
}
