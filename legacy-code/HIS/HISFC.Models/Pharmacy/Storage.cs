using System;

namespace Neusoft.HISFC.Models.Pharmacy
{
	/// <summary>
	/// [功能描述: 药品库存管理类]<br></br>
	/// [创 建 者: 崔鹏]<br></br>
	/// [创建时间: 2004-12]<br></br>
	/// <修改记录
	///		修改人='梁俊泽'
	///		修改时间='2006-09-13'
	///		修改目的='系统重构'
	///		修改描述='命名规范整理 继承自IMAStoreBase基类'
	///  />
	/// </summary>
    [Serializable]
    public class Storage : StorageBase
	{
		public Storage () 
		{
			
		}

		#region 变量

		private decimal preOutQty;

		private decimal myPreOutCost;

		private decimal lastMonthQty;

		private decimal lowQty;

		private decimal topQty;

		private bool myIsCheck;

		private bool myIsStop;

        private bool myIsLack;

        /// <summary>
        /// 拆分类型(门诊)
        /// 0、最小单位总量取整
        /// 1、包装单位总量取整
        /// 2、最小单位每次取整
        /// 3、包装单位每次取整
        /// 4、最小单位可拆分
        /// </summary>
        private string splitType;

        /// <summary>
        /// 拆分类型(住院临时医嘱)
        /// 0、最小单位总量取整
        /// 1、包装单位总量取整
        /// 2、最小单位每次取整
        /// 3、包装单位每次取整
        /// 4、最小单位可拆分
        /// </summary>
        private string lZSplitType;

        /// <summary>
        /// 拆分类型(住院长期医嘱)
        /// 0、最小单位总量取整
        /// 1、包装单位总量取整
        /// 2、最小单位每次取整
        /// 3、包装单位每次取整
        /// 4、最小单位可拆分
        /// </summary>
        private string cDSplitType;

        /// <summary>
        /// 药品库存性质
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject manageQuality = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 负库存量
        /// </summary>
        private decimal negativeQty;       

		#endregion

		/// <summary>
		/// 预出库数量
		/// </summary>
		public decimal PreOutQty
		{
			get 
			{
				return this.preOutQty;
			}
			set 
			{
				this.preOutQty = value;
			}
		}

		/// <summary>
		/// 预出库金额
		/// </summary>
		public decimal PreOutCost 
		{
			get 
			{
				return myPreOutCost;
			}
			set 
			{
				myPreOutCost = value;
			}
		}
		
		/// <summary>
		/// 最低库存量
		/// </summary>
		public decimal LowQty 
		{
			get 
			{
				return this.lowQty;
			}
			set 
			{
				this.lowQty = value;
			}
		}
		
		/// <summary>
		/// 最高库存量
		/// </summary>
		public decimal TopQty 
		{
			get 
			{
				return this.topQty;
			}
			set 
			{
				this.topQty = value;
			}
		}

		/// <summary>
		/// 上月结存数量
		/// </summary>
		public decimal LastMonthQty
		{
			get 
			{
				return this.lastMonthQty;
			}
			set 
			{
				lastMonthQty = value;
			}
		}

		/// <summary>
		/// 是否停用 该属性不由数据库内获取，通过ValidState赋值
		/// </summary>
        [Obsolete("该属性不由数据库内获取",false)]
		public bool IsStop 
		{
			get 
			{
				return myIsStop;
			}
			set 
			{
				myIsStop = value;

                if (value)
                {
                    base.ValidState = Neusoft.HISFC.Models.Base.EnumValidState.Invalid;
                }
                else
                {
                    base.ValidState = Neusoft.HISFC.Models.Base.EnumValidState.Valid;
                }
			}
		}

		/// <summary>
		/// 是否每日盘点
		/// </summary>
		public bool IsCheck 
		{
			get 
			{
				return myIsCheck;
			}
			set 
			{
				myIsCheck = value;
			}
		}

        /// <summary>
        /// 是否缺药
        /// </summary>
        public bool IsLack
        {
            get
            {
                return this.myIsLack;
            }
            set
            {
                this.myIsLack = value;
            }
        }

        /// <summary>
        /// 药品库存性质
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject ManageQuality
        {
            get
            {
                return this.manageQuality;
            }
            set
            {
                this.manageQuality = value;
            }
        }

        /// <summary>
        /// 有效性状态
        /// </summary>
        public new Neusoft.HISFC.Models.Base.EnumValidState ValidState
        {
            get
            {
                return base.ValidState;
            }
            set
            {
                if (value == Neusoft.HISFC.Models.Base.EnumValidState.Valid)
                {
                    this.myIsStop = false;
                }
                else
                {
                    this.myIsStop = true;
                }

                base.ValidState = value;
            }
        }

        /// <summary>
        /// 负库存量
        /// </summary>
        public decimal NegativeQty
        {
            get
            {
                return this.negativeQty;
            }
            set
            {
                this.negativeQty = value;
            }
        }

        /// <summary>
        /// 门诊拆分
        /// </summary>
        public String SplitType
        {
            get
            {
                return this.splitType;
            }
            set
            {
                this.splitType = value;
            }
        }

        /// <summary>
        /// 临瞩拆分
        /// </summary>
        public String LZSplitType
        {
            get
            {
                return this.lZSplitType;
            }
            set
            {
                this.lZSplitType = value;
            }
        }

        /// <summary>
        /// 长嘱拆分
        /// </summary>
        public String CDSplitType
        {
            get
            {
                return this.cDSplitType;
            }
            set
            {
                this.cDSplitType = value;
            }
        }

		#region 方法

		/// <summary>
		/// 克隆函数
		/// </summary>
		/// <returns>成功返回当前实例的副本</returns>
		public new Storage Clone()
		{
            Storage cloneStorage = base.Clone() as Storage;

            cloneStorage.manageQuality = this.manageQuality.Clone();

            return cloneStorage;
		}

		#endregion  

        #region 华南新增
        /// <summary>
        /// 是否可以门诊患者使用
        /// </summary>
        private bool isUseForOutpatient = true;

        /// <summary>
        /// 是否可以门诊患者使用
        /// </summary>
        public bool IsUseForOutpatient
        {
            get { return isUseForOutpatient; }
            set { isUseForOutpatient = value; }
        }

        /// <summary>
        /// 是否可以住院患者使用
        /// </summary>
        private bool isUseForInpatient = true;

        /// <summary>
        /// 是否可以住院患者使用
        /// </summary>
        public bool IsUseForInpatient
        {
            get { return isUseForInpatient; }
            set { isUseForInpatient = value; }
        }

        /// <summary>
        /// 住院缺药标志
        /// </summary>
        private bool isLackForInpatient = false;

        /// <summary>
        /// 住院缺药标志
        /// </summary>
        public bool IsLackForInpatient
        {
            get { return isLackForInpatient; }
            set { isLackForInpatient = value; }
        }

        /// <summary>
        /// 是否基数药(科室常备药)
        /// </summary>
        private bool isRadix = false;

        /// <summary>
        /// 是否基数药(科室常备药)
        /// </summary>
        public bool IsRadix
        {
            get { return isRadix; }
            set { isRadix = value; }
        }
        #endregion
    }
}
