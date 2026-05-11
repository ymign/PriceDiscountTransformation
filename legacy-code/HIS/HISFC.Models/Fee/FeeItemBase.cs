using System;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.SIInterface;
using Neusoft.HISFC.Models.RADT;
using System.Collections.Generic;

namespace Neusoft.HISFC.Models.Fee 
{
	/// <summary>
	/// FeeItemBase<br></br>
	/// [功能描述: 费用明细基类]<br></br>
	/// [创 建 者: 王宇]<br></br>
	/// [创建时间: 2006-09-13]<br></br>
	/// <修改记录 
	///		修改人='' 
	///		修改时间='yyyy-mm-dd' 
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary>
    /// 
    [System.Serializable]
	public abstract class FeeItemBase : NeuObject
	{
		#region 变量
        #region {46381C92-851C-42b6-BCD2-7559A375240F}
        /// <summary>
        /// 医保患者基本信息,结算信息
        /// </summary>
        private SIFeeItemBase siFeeItemBase;
        #endregion
   
		/// <summary>
		/// 交易类型 TransTypes.Positive 正交易(1), TransTypes.Negative 负交易(2)
		/// </summary>
		private TransTypes transType;
		
		/// <summary>
		///  作废信息 有效:Valid(0),退费作废:Canceled(1) 重打:Reprint(2) 注销:LogOut(3)
		/// </summary>
		private CancelTypes cancelType;
		
		/// <summary>
		/// 患者基本信息
		/// </summary>
        private Patient patient ;
		
		/// <summary>
		/// 基本项目信息
		/// </summary>
		private Base.Item item ;// new Neusoft.HISFC.Models.Base.Item();
		
		/// <summary>
		/// 是否组套
		/// </summary>
		private bool isGroup;
		
		/// <summary>
		/// 发票信息
		/// </summary>
		private Invoice invoice ;// new Invoice();

		/// <summary>
		/// 处方号
		/// </summary>
        private string recipeNO;
		
		/// <summary>
		/// 处方内流水号
		/// </summary>
		private int sequenceNO;
		
		/// <summary>
		/// 收费状态信息
		/// </summary>
		private PayTypes payType;
		
		/// <summary>
		/// 医嘱信息
		/// </summary>
		private Order.Order order;//new Neusoft.HISFC.Models.Order.Order();

		/// <summary>
		/// 费用信息
		/// </summary>
        private FT ft;
		
		/// <summary>
		/// 付数
		/// </summary>
		private decimal days;
		
		/// <summary>
		/// 收费时的单位 1 包装单位 0 最小单位
		/// </summary>
        private string feePack;

		/// <summary>
		/// 可退数量
		/// </summary>
		private decimal noBackQty;
		
		/// <summary>
		/// 费用来源 收费员操作(0) 医嘱(1) 终端(2) 体检(3) 
		/// </summary>
        private string ftSource;
		
		/// <summary>
		/// 组合项目信息
		/// </summary> 
        private Item.UndrugComb undrugComb;//new Item.UndrugComb(); 
		
		/// <summary>
		/// 开立操作环境(开立医生,开立医生所在科室,医生开立时间)
		/// </summary>
		private OperEnvironment recipeOper;// new OperEnvironment();
		
		/// <summary>
		/// 划价操作环境(划价人,划价科室,划价时间)
		/// </summary>
		private OperEnvironment chargeOper;// new OperEnvironment();
		
		/// <summary>
		/// 收费操作环境(收费人,收费科室,收费时间)
		/// </summary>
		private OperEnvironment feeOper;// new OperEnvironment();
		
		/// <summary>
		/// 退费操作环境(退费人,退费人所在科室,退费时间)
		/// </summary>
		private OperEnvironment cancelOper ;// new OperEnvironment();
		
		/// <summary>
		/// 执行操作环境(执行人,执行科室, 执行时间)
		/// </summary>
		private OperEnvironment execOper;// new OperEnvironment();
		
		/// <summary>
		/// 扣库存操作环境(扣库存人,扣库科室,扣库时间)
		/// </summary>
		private OperEnvironment stockOper ;// new OperEnvironment();
		
		/// <summary>
		/// 是否已经终端确认
		/// </summary>
		private bool isConfirmed;
		
		/// <summary>
		/// 终端确认的数量
		/// </summary>
		private decimal confirmedQty;

		/// <summary>
		/// 确认操作环境(确认人,确认科室,确认时间)
		/// </summary>
		private OperEnvironment confirmOper ;// new OperEnvironment();
		
		/// <summary>
		/// 医保对照信息
		/// </summary>
		private Compare compare;// new Compare();
		
		/// <summary>
		/// 急诊标志
		/// </summary>
		private bool isEmergency;

        /// <summary>
        /// 原来处方号（作废前）
        /// </summary>
        private string cancelRecipeNO;

        /// <summary>
        /// 原处方内流水号（作废前）
        /// </summary>
        private int cancelSequenceNO;

        /// <summary>
        /// 扣库流水号
        /// </summary>
        ////{25934862-5C82-409c-A0D7-7BE5A24FFC75}
        private int updateSequence;
        /// <summary>
        /// 收费中非药品所对应的物资
        /// </summary>
        //{25934862-5C82-409c-A0D7-7BE5A24FFC75}
        //private List<HISFC.Object.Material.Output> mateList = new List<Neusoft.HISFC.Models.FeeStuff.Output>();
        private List<Neusoft.HISFC.Models.FeeStuff.Output> mateList;// new List<Neusoft.HISFC.Models.FeeStuff.Output>();

        /// <summary>
        /// 是否协定处方
        /// </summary>
        private bool isNostrum = false;


        /// <summary>
        /// 是否上传
        /// </summary>
        private bool isUpload = false;

        /// <summary>
        /// 统计大类
        /// </summary>
        private FeeCodeStat feeCodeStat;

		#endregion

		#region 属性
        #region {46381C92-851C-42b6-BCD2-7559A375240F}
        /// <summary>
        /// 医保患者基本信息,结算信息
        /// </summary>
        public SIFeeItemBase SIFeeItemBase
        {
            get {
                if (siFeeItemBase == null)
                {
                    siFeeItemBase = new SIFeeItemBase();
                }
                return siFeeItemBase; }
            set { siFeeItemBase = value; }
        } 
        #endregion
		/// <summary>
		/// 交易类型 TransTypes.Positive 正交易(1), TransTypes.Negative 负交易(2)
		/// </summary>
		public TransTypes TransType
		{
			get
			{
				return transType;
			}
			set
			{
				transType = value;
			}
		}
		
		/// <summary>
		/// 作废信息 有效:Valid(0),退费作废:Canceled(1) 重打:Reprint(2) 注销:LogOut(3)
		/// </summary>
		public CancelTypes CancelType
		{
			get
			{
				return this.cancelType;
			}
			set
			{
				this.cancelType = value;
			}
		}
		
		/// <summary>
		/// 患者基本信息
		/// </summary>
		public Patient Patient
		{
			get
			{
                if (this.patient == null)
                {
                    this.patient = new Patient();
                }
				return this.patient;
			}
			set
			{
				this.patient = value;
			}
		}

		/// <summary>
		/// 基本项目信息
		/// </summary>
		public Base.Item Item
		{
			get
			{
                if (this.item == null)
                {
                    this.item = new Neusoft.HISFC.Models.Base.Item();
                }
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		/// <summary>
		/// 是否组套 ture 是 false 不是
		/// </summary>
		public bool IsGroup
		{
			get
			{
				return isGroup;
			}
			set
			{
				isGroup = value;
			}
		}
		
		/// <summary>
		/// 发票信息
		/// </summary>
		public Invoice Invoice
		{
			get
			{
                if (invoice == null)
                {
                    invoice = new Invoice();
                }
				return this.invoice;
			}
			set
			{
				this.invoice = value;
			}
		}

        /// <summary>
        /// 统计大类
        /// </summary>
        public FeeCodeStat FeeCodeStat
        {
            get
            {
                if (feeCodeStat == null)
                {
                    feeCodeStat = new FeeCodeStat();
                }

                return this.feeCodeStat;
            }
            set
            {
                this.feeCodeStat = value;
            }
        }
		
		/// <summary>
		/// 处方号
		/// </summary>
		public string RecipeNO
		{
			get
			{
                if (recipeNO == null)
                {
                    recipeNO = string.Empty;
                }
				return this.recipeNO;
			}
			set
			{
				this.recipeNO = value;
			}
		}
		
		/// <summary>
		/// 处方内流水号
		/// </summary>
		public int SequenceNO
		{
			get
			{
				return this.sequenceNO;
			}
			set
			{
				this.sequenceNO = value;
			}
		}

		/// <summary>
		/// 收费状态信息 Charged 划价 Balanced 收费
		/// </summary>
		public PayTypes PayType
		{
			get
			{
				return payType;
			}
			set
			{
				payType = value;
			}
		}	
		
		/// <summary>
		/// 费用信息
		/// </summary>
		public FT FT
		{
			get
			{
                if (ft == null)
                {
                    ft = new FT();
                }
				return this.ft;
			}
			set
			{
				this.ft = value;
			}
		}
		
		/// <summary>
		/// 医嘱信息
		/// </summary>
		public Order.Order Order
		{
			get
			{
                if (this.order == null)
                {
                    this.order = new Neusoft.HISFC.Models.Order.Order();
                }
				return this.order;
			}
			set
			{
				this.order = value;
			}
		}
		
		/// <summary>
		/// 付数
		/// </summary>
		public decimal Days
		{
			get
			{
				return this.days;
			}
			set
			{
				this.days = value;
			}
		}
		
		/// <summary>
		/// 可退数量
		/// </summary>
		public decimal NoBackQty
		{
			get
			{
				return this.noBackQty;
			}
			set
			{
				this.noBackQty = value;
			}
		}
		
		/// <summary>
		/// 收费时的单位 1 包装单位 0 最小单位
		/// </summary>
		public string FeePack
		{
			get
			{
                if (feePack == null)
                {
                    feePack = string.Empty;
                }
				return this.feePack;
			}
			set
			{
				this.feePack = value;
			}
		}

		/// <summary>
		/// 费用来源 收费员操作(0) 医嘱(1) 终端(2) 体检(3) 
		/// </summary>
		public string FTSource
		{
			get
			{
                if (ftSource == null)
                {
                    ftSource = string.Empty;
                }
				return this.ftSource;
			}
			set
			{
				this.ftSource = value;
			}
		}

		/// <summary>
		/// 组合项目信息
		/// </summary>  
		public Item.UndrugComb UndrugComb
		{ 
			get
			{
                if (this.undrugComb == null)
                {
                    this.undrugComb = new Neusoft.HISFC.Models.Fee.Item.UndrugComb();
                }
                return this.undrugComb;
			}
            set
            {
                this.undrugComb = value;
            }
		}

		/// <summary>
		/// 开立操作环境(开立医生,开立医生所在科室,医生开立时间)
		/// </summary>
		public OperEnvironment RecipeOper
		{
			get
			{
                if (recipeOper == null)
                {
                    recipeOper = new OperEnvironment();
                }
				return this.recipeOper;
			}	
			set
			{
				this.recipeOper = value;
			}
		}
		
		/// <summary>
		/// 划价操作环境(划价人,划价科室,划价时间)
		/// </summary>
		public OperEnvironment ChargeOper
		{
			get
			{
                if (chargeOper == null)
                {
                    chargeOper = new OperEnvironment();
                }
				return this.chargeOper;
			}
			set
			{
				this.chargeOper = value;
			}
		}
		
		/// <summary>
		/// 执行操作环境(执行人,执行科室, 执行时间)
		/// </summary>
		public OperEnvironment FeeOper
		{
			get
			{
                if (feeOper == null)
                {
                    feeOper = new OperEnvironment();
                }
				return this.feeOper;
			}
			set
			{
				this.feeOper = value;
			}
		}
		
		/// <summary>
		/// 退费操作环境(退费人,退费人所在科室,退费时间)
		/// </summary>
		public OperEnvironment CancelOper
		{
			get
			{
                if (cancelOper == null)
                {
                    cancelOper = new OperEnvironment();
                }
				return this.cancelOper;
			}
			set
			{
				this.cancelOper = value;
			}
		}

		/// <summary>
		///  执行操作环境(执行人,执行科室, 执行时间)
		/// </summary>
		public OperEnvironment ExecOper
		{
			get
			{
                if (execOper == null)
                {
                    execOper = new OperEnvironment();
                }
				return this.execOper;
			}
			set
			{
				this.execOper = value;
			}
		}
		
		/// <summary>
		/// 是否已经终端确认
		/// </summary>
		public bool IsConfirmed
		{
			get
			{
				return this.isConfirmed;
			}
			set
			{
				this.isConfirmed = value;
			}
		}
		
		/// <summary>
		/// 终端确认的数量
		/// </summary>
		public decimal ConfirmedQty
		{
			get
			{
				return this.confirmedQty;
			}
			set
			{
				this.confirmedQty = value;
			}
		}
		
		/// <summary>
		/// 确认操作环境(确认人,确认科室,确认时间)
		/// </summary>
		public OperEnvironment ConfirmOper
		{
			get
			{
                if (confirmOper == null)
                {
                    confirmOper = new OperEnvironment();
                }
				return this.confirmOper;
			}
			set
			{
				this.confirmOper = value;
			}
		}

		/// <summary>
		/// 扣库存操作环境(扣库存人,扣库科室,扣库时间)
		/// </summary>
		public OperEnvironment StockOper
		{
			get
			{
                if (stockOper == null)
                {
                    stockOper = new OperEnvironment();
                }
				return this.stockOper;
			}
			set
			{
				this.stockOper = value;
			}
		}

		/// <summary>
		/// 医保对照信息
		/// </summary>
        public Compare Compare = new Compare();

		/// <summary>
		/// 急诊标志
		/// </summary>
		public bool IsEmergency
		{
			get
			{
				return this.isEmergency;
			}
			set
			{
				this.isEmergency = value;
			}
		}

        /// <summary>
        /// 原处方号
        /// </summary>
        public string CancelRecipeNO
        {
            get
            {
                if (cancelRecipeNO == null)
                {
                    cancelRecipeNO = string.Empty;
                }
                return this.cancelRecipeNO;
            }
            set
            {
                this.cancelRecipeNO = value;
            }
        }

        /// <summary>
        /// 处方内流水号
        /// </summary>
        public int CancelSequenceNO
        {
            get
            {
                return this.cancelSequenceNO;
            }
            set
            {
                this.cancelSequenceNO = value;
            }
        }

        /// <summary>
        /// 扣库流水号
        /// </summary>
        //{25934862-5C82-409c-A0D7-7BE5A24FFC75}
        public int UpdateSequence
        {
            get
            {
                return this.updateSequence;
            }
            set
            {
                this.updateSequence = value;
            }
        }

        /// <summary>
        /// 非药品所对应的物资
        /// </summary>
        //{25934862-5C82-409c-A0D7-7BE5A24FFC75}
        public List<HISFC.Models.FeeStuff.Output> MateList
        {
            get
            {
                if (mateList == null)
                {
                    mateList = new List<Neusoft.HISFC.Models.FeeStuff.Output>();
                }
                return mateList;
            }
            set
            {
                mateList = value;
            }
        }

        /// <summary>
        /// 是否协定处方
        /// </summary>
        public bool IsNostrum
        {
            get
            {
                return isNostrum;
            }
            set
            {
                isNostrum = value;
            }
        }


        /// <summary>
        /// 是否上传
        /// </summary>
        public bool IsUpload
        {
            get { return isUpload; }
            set { isUpload = value; }
        }

		#endregion

		#region 方法
		
		#region 克隆

		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns>返回当前对象的实例副本</returns>
		public new FeeItemBase Clone()
		{
			FeeItemBase feeItemBase = base.Clone() as FeeItemBase;
			
			feeItemBase.ChargeOper = this.ChargeOper.Clone();
			feeItemBase.ExecOper = this.ExecOper.Clone();
			feeItemBase.FeeOper = this.FeeOper.Clone();
			feeItemBase.FT = this.FT.Clone();
			feeItemBase.Invoice = this.Invoice.Clone();
			feeItemBase.Item = this.Item.Clone();
			feeItemBase.Order = this.Order.Clone();
			feeItemBase.Patient = this.Patient.Clone();
			feeItemBase.RecipeOper = this.RecipeOper.Clone();
			feeItemBase.StockOper = this.StockOper.Clone();
            feeItemBase.UndrugComb = this.UndrugComb.Clone();
			feeItemBase.Compare = this.Compare.Clone();
            if (this.feeCodeStat!=null)
            {
                feeItemBase.FeeCodeStat = this.FeeCodeStat.Clone();
            }
            //{25934862-5C82-409c-A0D7-7BE5A24FFC75}
            List<Neusoft.HISFC.Models.FeeStuff.Output> list = new List<Neusoft.HISFC.Models.FeeStuff.Output>();
            foreach (Neusoft.HISFC.Models.FeeStuff.Output item in this.MateList)
            {
                list.Add(item.Clone());
            }
            feeItemBase.MateList = list;
			return feeItemBase;
		}

		#endregion

		#endregion
	}
}
