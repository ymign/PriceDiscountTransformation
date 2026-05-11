using System;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Order;
using System.Collections.Generic;
using Neusoft.HISFC.Models.RADT;
using Neusoft.HISFC.Models.SPD;

namespace Neusoft.HISFC.Models.Fee.Inpatient
{


	/// <summary>
	///费用项目信息类
	///ID住院流水号
	///name 患者姓名
	/// </summary>
    /// 

    [System.Serializable]
	public class FeeItemList : FeeItemBase, IBaby
	{
		/// <summary>
		/// FeeItemBase<br></br>
		/// [功能描述: 住院费用明细类]<br></br>
		/// [创 建 者: 王宇]<br></br>
		/// [创建时间: 2006-09-13]<br></br>
		/// <修改记录 
		///		修改人='' 
		///		修改时间='yyyy-mm-dd' 
		///		修改目的=''
		///		修改描述=''
		///  />
		/// </summary>
		public FeeItemList()
		{
            this.Patient = new Neusoft.HISFC.Models.RADT.PatientInfo();
            this.Order = new Neusoft.HISFC.Models.Order.Inpatient.Order();
		}
		
		#region 变量

		/// <summary>
		/// 结算序号
		/// </summary>
		private int balanceNO;
		
		/// <summary>
		/// 结算状态
		/// </summary>
        private string balanceState;

		/// <summary>
		/// 婴儿序号
		/// </summary>
        private string babyNO;
		
		/// <summary>
		/// 是否婴儿
		/// </summary>
		private bool isBaby;
		
		/// <summary>
		/// 出库单号
		/// </summary>
		private int sendSequence;

        /// <summary>
        /// 耗材收费ID
        /// </summary>
        private string mdhChargeId;

		/// <summary>
		/// 设备编号
		/// </summary>
        private string machineNO;
		
		/// <summary>
		/// 费用比例(这里为手术比例服务)
		/// </summary>
		private FTRate ftRate;

		/// <summary>
		/// 审核序号
		/// </summary>
		private string auditingNO;//string.Empty;
		
		/// <summary>
		/// 结算操作环境(结算人，结算时间，结算人所在科室）
		/// </summary>
		private OperEnvironment balanceOper;// new OperEnvironment();
		
		/// <summary>
		/// 医嘱执行档
		/// </summary>
		private ExecOrder execOrder;// new ExecOrder();
		
		/// <summary>
		/// 扩展标志
		/// </summary>
        private string extFlag;

        /// <summary>
        /// 扩展标志1
        /// </summary>
        private string extFlag1;// string.Empty;

        /// <summary>
        /// 扩展标志2
        /// </summary>
        private string extFlag2;// string.Empty;

        /// <summary>
        /// 扩展编码
        /// </summary>
        private string extCode;// string.Empty;

        /// <summary>
        /// 扩展操作环境(扩展人,扩展时间,扩展人所在科室)
        /// </summary>
        private OperEnvironment extOper;

		/// <summary>
		/// 审批号
		/// </summary>
		private string approveNO;// string.Empty;

        /// <summary>
        /// 是否需要更新可退数量 true 需要 false 不需要
        /// </summary>
        private bool isNeedUpdateNoBackQty;

        //{AC6A5576-BA29-4dba-8C39-E7C5EBC7671E} 增加医疗组处理
        /// <summary>
        /// 医疗组
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject medicalTeam;

        /// <summary>
        /// 手术编码{0604764A-3F55-428f-9064-FB4C53FD8136}
        /// </summary>
        private string operationNO ;// string.Empty;

        /// <summary>
        /// 费用来源（重写）
        /// </summary>
        private FTSource ftSource ;

        /// <summary>
        /// 拆分标记
        /// </summary>
        private bool splitFlag = false;
        /// <summary>
        /// 拆分ID
        /// </summary>
        private string splitID = string.Empty;

        /// <summary>
        /// 拆分后 高收费标记
        /// </summary>
        private bool splitFeeFlag = false;
		#endregion

		#region 属性

        /// <summary>
        /// 医嘱信息
        /// </summary>
        public new Order.Inpatient.Order Order
        {
            get
            {
                if (base.Order == null)
                {
                    base.Order = new Order.Inpatient.Order();
                }
                return base.Order as Order.Inpatient.Order;
            }
            set
            {
                base.Order = value;
            }
        }

		/// <summary>
		/// 结算序号
		/// </summary>
		public int BalanceNO
		{
			get
			{
				return this.balanceNO;
			}
			set
			{
				this.balanceNO = value;
			}
		}
		
		/// <summary>
		/// 结算状态
		/// </summary>
		public string BalanceState
		{
			get
			{
                if (balanceState == null)
                {
                    balanceState = string.Empty;
                }
				return this.balanceState;
			}
			set
			{
				this.balanceState = value;
			}
		}

		/// <summary>
		/// 出库单号
		/// </summary>
		public int SendSequence
		{
			get
			{
				return this.sendSequence;
			}
			set
			{
				this.sendSequence = value;
			}
		}

		/// <summary>
		/// 设备编号
		/// </summary>
		public string MachineNO
		{
			get
			{
                if (machineNO == null)
                {
                    machineNO = string.Empty;
                }
				return this.machineNO;
			}
			set
			{
				this.machineNO = value;
			}
		}

		/// <summary>
		/// 费用比例(这里为手术比例服务)
		/// </summary>
		public FTRate FTRate
		{
			get
			{
                if (ftRate == null)
                {
                    ftRate = new FTRate();
                }
				return this.ftRate;
			}
			set
			{
				this.ftRate = value;
			}
		}

		/// <summary>
		/// 审核序号
		/// </summary>
		public string AuditingNO
		{
			get
			{
                if (auditingNO == null)
                {
                    auditingNO = string.Empty;
                }
				return this.auditingNO;
			}
			set
			{
				this.auditingNO = value;
			}
		}

		/// <summary>
		/// 结算操作环境(结算人，结算时间，结算人所在科室）
		/// </summary>
		public OperEnvironment BalanceOper
		{
			get
			{
                if (this.balanceOper == null)
                {
                    this.balanceOper = new OperEnvironment();
                }
				return this.balanceOper;
			}
			set
			{
				this.balanceOper = value;
			}
		}

		/// <summary>
		/// 扩展标志
		/// </summary>
		public string ExtFlag
		{
			get
			{
                if (extFlag == null)
                {
                    extFlag = string.Empty;
                }
				return this.extFlag;	
			}
			set
			{
				this.extFlag = value;
			}
		}

        /// <summary>
        /// 扩展标志1
        /// </summary>
        public string ExtFlag1
        {
            get
            {
                if (extFlag1 == null)
                {
                    extFlag1 = string.Empty;
                }
                return this.extFlag1;
            }
            set
            {
                this.extFlag1 = value;
            }
        }

        /// <summary>
        /// 扩展标志2
        /// </summary>
        public string ExtFlag2
        {
            get
            {
                if (extFlag2 == null)
                {
                    extFlag2 = string.Empty;
                }
                return this.extFlag2;
            }
            set
            {
                this.extFlag2 = value;
            }
        }

        /// <summary>
        /// 扩展编码
        /// </summary>
        public string ExtCode
        {
            get
            {
                if (extCode == null)
                {
                    extCode = string.Empty;
                }
                return this.extCode;
            }
            set
            {
                this.extCode = value;
            }
        }

        /// <summary>
        ///  扩展操作环境(扩展人,扩展时间,扩展人所在科室)
        /// </summary>
        public OperEnvironment ExtOper
        {
            get
            {
                if (this.extOper == null)
                {
                    this.extOper = new OperEnvironment();
                }
                return this.extOper;
            }
            set
            {
                this.extOper = value;
            }
        }

		/// <summary>
		/// 医嘱执行档
		/// </summary>
		public ExecOrder ExecOrder 
		{
			get
			{
                if (execOrder == null)
                {
                    execOrder = new ExecOrder();
                }
				return this.execOrder;
			}
			set
			{
				this.execOrder = value;
			}
		}

		/// <summary>
		/// 审批号
		/// </summary>
		public string ApproveNO
		{
			get
			{
                if (approveNO == null)
                {
                    approveNO = string.Empty;
                }
				return this.approveNO;
			}
			set
			{
				this.approveNO = value;
			}
		}

        /// <summary>
        /// 是否需要更新可退数量 true 需要 false 不需要
        /// </summary>
        public bool IsNeedUpdateNoBackQty 
        {
            get 
            {
                return this.isNeedUpdateNoBackQty;
            }
            set 
            {
                this.isNeedUpdateNoBackQty = value;
            }
        }

        /// <summary>
        /// 费用来源
        /// </summary>
        public new FTSource FTSource
        {
            get
            {
                if (ftSource == null)
                {
                    ftSource = new FTSource();
                }
                return ftSource;
            }
            set
            {
                ftSource = value;
            }
        }


        //{AC6A5576-BA29-4dba-8C39-E7C5EBC7671E} 增加医疗组处理
        /// <summary>
        /// 医疗组
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject MedicalTeam
        {
            get {
                if (medicalTeam == null)
                {
                    medicalTeam = new NeuObject();
                }
                return medicalTeam; }
            set { medicalTeam = value; }
        }

        /// <summary>
        /// 手术编码{0604764A-3F55-428f-9064-FB4C53FD8136}
        /// </summary>
        public string OperationNO
        {
            get {
                if (operationNO == null)
                {
                    operationNO = string.Empty;
                }
                return operationNO; }
            set { operationNO = value; }
        }

        /// <summary>
        /// 拆分标记
        /// </summary>
        public bool SplitFlag
        {
            get
            {
                return splitFlag;
            }
            set
            {
                splitFlag = value;
            }
        }

        /// <summary>
        /// 拆分ID
        /// </summary>
        public string SplitID
        {
            get
            {
                return splitID;
            }
            set
            {
                splitID = value;
            }
        }

        /// <summary>
        /// 拆分后  高收费标记
        /// </summary>
        public bool SplitFeeFlag
        {
            get
            {
                return splitFeeFlag;
            }
            set
            {
                splitFeeFlag = value;
            }
        }


        /// <summary>
        /// 耗材收费ID
        /// </summary>
        public string MdhChargeId
        {
            get
            {
                return mdhChargeId;
            }
            set
            {
                mdhChargeId = value;
            }
        }

        public List<ScanSpdPackageModel> ScanSpdPackageList { get; set; }
		#endregion

		#region 方法

		#region 克隆
		
		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns>返回当前对象实例副本</returns>
		public new FeeItemList Clone()
		{
			FeeItemList feeItemList = base.Clone() as FeeItemList;

			feeItemList.FTRate = this.FTRate.Clone();
            if (this.balanceOper != null)
            {
                feeItemList.balanceOper = this.BalanceOper.Clone();
            }
			feeItemList.ExecOrder = this.ExecOrder.Clone();
            //{AC6A5576-BA29-4dba-8C39-E7C5EBC7671E} 增加医疗组处理
            feeItemList.MedicalTeam = this.MedicalTeam.Clone();
            feeItemList.ftSource = this.FTSource.Clone();
			return feeItemList;
		}

		#endregion

		#endregion 

		#region 接口实现
		
		#region IBaby 成员
		
		/// <summary>
		/// 婴儿序号
		/// </summary>
		public string BabyNO
		{
			get
			{
                if (babyNO == null)
                {
                    babyNO = string.Empty;
                }
				return this.babyNO;
			}
			set
			{
				this.babyNO = value;
			}
		}
		
		/// <summary>
		/// 是否婴儿
		/// </summary>
		public bool IsBaby
		{
			get
			{
				return this.isBaby;
			}
			set
			{
				this.isBaby = value;
			}
		}

		#endregion

		#endregion

		#region 无用方法属性

		//[Obsolete("作废", true)]
		//public FeeInfo FeeInfo=new FeeInfo();

		/// <summary>
		/// 医嘱流水号
		/// </summary>
		[Obsolete("作废,基类Order.ID代替", true)]
		public string MoOrder
		{
			get
			{	
				return "";
			}
			set
			{
				
			}
		}
		
		/// <summary>
		/// 新物价组套信息
		/// </summary>
        [Obsolete("作废,基类UndrugComb代替", true)]
        public NeuObject ItemGroup
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
		/// <summary>
		/// 0划价1收费2执行发放
		/// </summary>
		[Obsolete("作废,基类PayType代替", true)]
		public string SendState;

		/// <summary>
		/// 是否出院带疗(改为医嘱类型)
		/// </summary>
		[Obsolete("作废,基类Order代替", true)]
		public string IsBrought;
		
		[Obsolete("作废,基类NoBackQty代替", true)]
		public decimal NoBackNum=0m;
		/// <summary>
		/// 收费比例(手术用)
		/// </summary>
		[Obsolete("作废,FTRate代替", true)]
		public decimal Rate=0m;

		#endregion
	}

    /// <summary>
    /// 住院费用明细表中增加字段“费用来源”。默认值都是0
    ///长度为3。每一位都代表不同含义，可扩展
    ///第一位：代表收费的类别（0 手工收费 ，1医嘱收费，2自动收费）
    ///第二位：代表收费类别里面的收费地点或收费时间（见具体值）
    ///第三位：代表退费重收的类别（0正常，1身份变更，2结算召回）
    ///000-护士站手工收费
    ///010-住院处手工收费
    ///020-手术室手工收费
    ///001-护士站手工收费（身份变更）
    ///011-住院处手工收费（身份变更）
    ///021-手术室手工收费（身份变更）
    ///002-护士站手工收费（结算召回）
    ///012-住院处手工收费（结算召回）
    ///022-手术室手工收费（结算召回）
    ///100-审核医嘱收费
    ///110-分解医嘱收费
    ///120-打印时收费
    ///130-执行时收费
    ///140-发药时收费
    ///100-审核医嘱收费
    ///110-分解医嘱收费
    ///120-打印时收费
    ///130-执行时收费
    ///140-发药时收费
    ///150-终端收费
    ///101-审核医嘱收费（身份变更）
    ///111-分解医嘱收费（身份变更）
    ///121-打印时收费（身份变更）
    ///131-执行时收费（身份变更）
    ///141-发药时收费（身份变更）
    ///151-终端收费（身份变更）
    ///102-审核医嘱收费（结算召回）
    ///112-分解医嘱收费（结算召回）
    ///122-打印时收费（结算召回）
    ///132-执行时收费（结算召回）
    ///142-发药时收费（结算召回）
    ///152-终端收费（结算召回）
    ///200-自动收费（晚上定时）
    ///210-自动收费（出院补收，需回收）
    ///220-自动收费（出院补收，不需回收）
    ///201-自动收费（晚上定时）（身份变更）
    ///211-自动收费（出院补收，需回收）（身份变更）
    ///221-自动收费（出院补收，不需回收）（身份变更）
    ///202-自动收费（晚上定时）（结算召回）
    ///212-自动收费（出院补收，需回收）（结算召回）
    ///222-自动收费（出院补收，不需回收）（结算召回）
    ///程序里面重写FTSource。
    ///分三个字段表示
    ///SourceType1
    ///SourceType2
    ///SourceType3
    ///重写ToString()方法，返回整体的值
    /// </summary>
    public class FTSource
    {
        public FTSource()
        {
        }

        /// <summary>
        /// 费用来源
        /// </summary>
        /// <param name="sourceCost"></param>
        public FTSource(string sourceCost)
        {
            if (sourceCost == null)
            {
            }
            else if (sourceCost.Length==1)
            {
                sourceType1 = sourceCost;
            }
            else if (sourceCost.Length == 2)
            {
                sourceType1 = sourceCost[0].ToString();
                sourceType2 = sourceCost[1].ToString();
            }
            else if (sourceCost.Length == 3)
            {
                sourceType1 = sourceCost[0].ToString();
                sourceType2 = sourceCost[1].ToString();
                sourceType3 = sourceCost[2].ToString();
            }
        }

        private string sourceType1;
        private string sourceType2;
        private string sourceType3;

        /// <summary>
        ///费用来源第一位：代表收费的类别（0 手工收费 ，1医嘱收费，2自动收费）
        /// </summary>
        public string SourceType1
        {
            get
            {
                if (sourceType1 == null)
                {
                    sourceType1 = "0";
                }
                return this.sourceType1;
            }
            set
            {
                this.sourceType1=value;
            }
        }
        /// <summary>
        /// 费用来源第二位：代表收费类别里面的收费地点或收费时间（见具体值）
        /// </summary>
        public string SourceType2
        {
            get
            {
                if (sourceType2 == null)
                {
                    sourceType2 = "0";
                }
                return this.sourceType2;
            }
            set
            {
                this.sourceType2=value;
            }
        }
        /// <summary>
        /// 费用来源第三位：代表退费重收的类别（0正常，1身份变更，2结算召回）
        /// </summary>
        public string SourceType3
        {
            get
            {
                if (sourceType3 == null)
                {
                    sourceType3 = "0";
                }
                return this.sourceType3;
            }
            set
            {
                this.sourceType3=value;
            }
        }

        public new FTSource Clone()
        {
            FTSource s = base.MemberwiseClone() as FTSource;
            return s;
        }

        public override string ToString()
        {
            return sourceType1 + sourceType2+sourceType3;
        }
    }
}
