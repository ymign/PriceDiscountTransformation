using System;

namespace Neusoft.HISFC.Models.Nurse
{
	/// <summary>
	/// Assign<br></br>
	/// [功能描述: 分诊实体]<br></br>
	/// [创 建 者: 孙晓华]<br></br>
	/// [创建时间: 2006-09-01]<br></br>
	/// <修改记录
	///		修改人='徐伟哲'
	///		修改时间='2007-02-07'
	///		修改目的='改一改'
	///		修改描述=''
	///  />
	/// </summary>
    /// 
    [System.Serializable]
	public class Assign : Neusoft.FrameWork.Models.NeuObject
	{

		/// <summary>
		/// 构造函数
		/// </summary>
		public Assign()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		#region 变量

		/// <summary>
		/// 实际看诊序号
		/// </summary>
		private int seeNO = 0; 
		/// <summary>
		/// 看诊日期
		/// </summary>
		private DateTime seeTime = DateTime.MinValue;
		/// <summary>
		/// 分诊科室
		/// </summary>
        private string triageDept = "";
		/// <summary>
		/// 分诊时间
		/// </summary>
        private DateTime triageTime = DateTime.MinValue;
		/// <summary>
		/// 进诊室时间
		/// </summary>
        private DateTime inTime = DateTime.MinValue;
		/// <summary>
		/// 出诊室时间
		/// </summary>
        private DateTime outTime = DateTime.MinValue;
		/// <summary>
		/// 分诊状态
		/// </summary>
        private EnuTriageStatus triageStatus;

        private Neusoft.HISFC.Models.Base.OperEnvironment oper;
        
		/// <summary>
        /// 患者挂号信息
        /// </summary>
        private Neusoft.HISFC.Models.Registration.Register register;

        /// <summary>
        /// 队列信息
        /// </summary>
        private Neusoft.HISFC.Models.Nurse.Queue queue;
		#endregion


        #region 属性

        /// <summary>
		/// 实际看诊序号
		/// </summary>
		public int SeeNO
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
        /// 看诊日期
        /// </summary>
        public DateTime SeeTime
        {
            get
            {
                return this.seeTime;
            }
            set
            {
                this.seeTime = value;
            }
        }

		/// <summary>
		/// 分诊科室
		/// </summary>
		public string TriageDept
		{
			get
			{
                if (triageDept == null)
                {
                    triageDept = string.Empty;
                }
				return this.triageDept;
			}
			set
			{
				this.triageDept = value;
			}
		}

        /// <summary>
        /// 分诊时间
        /// </summary>
        public DateTime TirageTime
        {
            get
            {
                return this.triageTime;
            }
            set
            {
                this.triageTime = value;
            }
        }

        /// <summary>
        /// 进诊室时间
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
        /// 出诊室时间
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
		/// 分诊状态
		/// </summary>
		public EnuTriageStatus TriageStatus
		{
			get
			{
                if (triageStatus == null)
                {
                    triageStatus = EnuTriageStatus.None;
                }
				return this.triageStatus;
			}
			set
			{
				this.triageStatus = value;
			}
		}

        /// <summary>
        /// 操作环境
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                if (oper == null)
                {
                    oper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }
        /// <summary>
        /// 患者挂号信息
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register Register
        {
            get
            {
                if (register == null)
                {
                    register = new Neusoft.HISFC.Models.Registration.Register();
                }
                return this.register;
            }
            set
            {
                this.register = value;
            }
        }

        /// <summary>
        /// 队列信息
        /// </summary>
        public Neusoft.HISFC.Models.Nurse.Queue Queue
        {
            get
            {
                if (queue == null)
                {
                    queue = new Queue();
                }
                return this.queue;
            }
            set
            {
                this.queue = value;
            }
        }
		#endregion

        #region 过期
        /// <summary>
        /// 操作员
        /// </summary>
        private string operID = "";
        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operDate = DateTime.MinValue;

        /// <summary>
        /// 操作员
        /// </summary>
        [Obsolete("使用Oper.ID", true)]
        public string OperID
        {
            get
            {
                return this.operID;
            }
            set
            {
                this.operID = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Obsolete("使用Oper.OperTime", true)]
        public DateTime OperDate
        {
            get
            {
                return this.operDate;
            }
            set
            {
                this.operDate = value;
            }
        }

        /// <summary>
        /// 出诊室时间
        /// </summary>
        [Obsolete("使用OutTime", true)]
        public DateTime OutDate
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
        /// 进诊室时间
        /// </summary>
        [Obsolete("使用InTime", true)]
        public DateTime InDate
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
        /// 分诊时间
        /// </summary>
        [Obsolete("使用TriageTime", true)]
        public DateTime TriageDate
        {
            get
            {
                return this.triageTime;
            }
            set
            {
                this.triageTime = value;
            }
        }

        /// <summary>
        /// 看诊日期
        /// </summary>
        [Obsolete("使用SeeTime", true)]
        public DateTime SeeDate
        {
            get
            {
                return this.seeTime;
            }
            set
            {
                this.seeTime = value;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Assign Clone()
        {
            Assign assign = base.Clone() as Assign;
            assign.oper = this.Oper.Clone();
            assign.register = this.Register.Clone();
            assign.queue = this.Queue.Clone();
            return assign;
        }

		#endregion

	}

	#region 枚举

    //现有数据的分诊状态
    //1分诊/2进诊/3诊出

	/// <summary>
	/// 分诊状态
	/// </summary>
	public enum EnuTriageStatus
	{
        /*
         * 患者挂号时为 None待分诊状态
         * 护士分诊（或挂号自动分诊）后为 Triage已分诊状态
         * 医生叫号（或护士手动进诊）后，为In 已进诊状态
         * 医生开立看诊时，为Arrive 已到诊状态  这个状态只是用于外屏显示当前正在看诊患者 
                 当医生点击开立而未保存，退出时，返回原有状态（可能是In或Out）
                 当医生开立保存后，更新患者状态为 Out
                 这个状态属于临时状态，允许多次出现；
         * 当医生开立保存后，更新患者状态为 Out
         * 当患者叫号进诊后，如果患者未到，可以置为Delay 延迟状态；延迟患者加载到未看诊患者列表最后
         * 当护士退号时，更新患者状态为cancel
         * */


        /// <summary>
		/// 待分诊
		/// </summary>
		None,

		/// <summary>
		/// 已分诊
		/// </summary>
		Triage,

		/// <summary>
		/// 已进诊
		/// </summary>
		In,

		/// <summary>
		/// 已出诊
		/// </summary>
		Out,

        /// <summary>
        /// 已到诊-正在看诊
        /// </summary>
        Arrive,

        /// <summary>
        /// 延迟看诊-已分诊
        /// </summary>
        Delay,

        /// <summary>
        /// 作废分诊
        /// </summary>
        Cancel
	}

	#endregion
}
