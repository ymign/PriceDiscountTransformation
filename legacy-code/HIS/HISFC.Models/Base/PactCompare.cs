namespace Neusoft.HISFC.Models.Base
{
    /// <summary>
    /// PactInfo<br></br>
    /// [功能描述: 合同单位信息，用于业务实现]<br></br>
    /// [创 建 者: 赫一阳]<br></br>
    /// [创建时间: 2006-08-28]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class PactCompare : Pact, ISort
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PactCompare()
        {

        }

        #region 变量

        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private bool alreadyDisposed = false;

        /// <summary>
        /// 结算类别
        /// </summary>
        private PayKind payKind = new PayKind();

        private string pactCode;	//		合同单位代码
        private string pactHead;	//		合同单位统计字头
        private string pactName;	//		合同单位名称
        private string parentPact;	//			父级合同单位代码

        private string parentName;		//	父级合同单位名称
        private string pactFlag;		//	//合同单位属性

        private string valldState;			//0有效 1无效


        /// <summary>
        /// 合同单位对照医保待遇dll名称
        /// </summary>
        private string pactDllName = string.Empty;
        /// <summary>
        /// 合同单位对照医保待遇dll描述
        /// </summary>
        private string pactDllDescription = string.Empty;
        /// <summary>
        /// 合同单位系统类别
        /// 适用类别 0=全院、1=门诊、2=住院、3=系统后台使用
        /// </summary>
        private string pactSystemType = string.Empty;

        #endregion

        #region 属性

        /// <summary>
        /// 结算类别
        /// </summary>
        public PayKind PayKind
        {
            get
            {
                return this.payKind;
            }
            set
            {
                this.payKind = value;
            }
        }

        /// <summary>
        /// 合同单位代码
        /// </summary>
        public string PactCode
        {
            get { return pactCode; }
            set { pactCode = value; }
        }
        /// <summary>
        /// 合同单位统计字头
        /// </summary>
        public string PactHead
        {
            get { return pactHead; }
            set { pactHead = value; }
        }
        /// <summary>
        /// 合同单位名称
        /// </summary>
        public string PactName
        {
            get { return pactName; }
            set { pactName = value; }
        }
        /// <summary>
        /// 父级合同单位代码
        /// </summary>
        public string ParentPact
        {
            get { return parentPact; }
            set { parentPact = value; }
        }
        /// <summary>
        /// 父级合同单位名称
        /// </summary>
        public string ParentName
        {
            get { return parentName; }
            set { parentName = value; }
        }
        /// <summary>
        /// 合同单位属性
        /// </summary>
        public string PactFlag
        {
            get { return pactFlag; }
            set { pactFlag = value; }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        public string ValldState
        {
            get { return valldState; }
            set
            {
                valldState = value;
            }
        }
        /*
		/// <summary>
		/// 是否要求必须有医疗证号
		/// </summary>
		public bool IsNeedMCard
		{
			get
			{
				return this.isNeedMCard;
			}
			set
			{
				this.isNeedMCard = value;
			}
		}
       
		/// <summary>
		/// 是否受监控
		/// </summary>
		public bool IsInControl
		{
			get
			{
				return this.isInControl;
			}
			set
			{
				this.isInControl = value;
			}
		}

		/// <summary>
		/// 项目类别标记 0 全部, 1 药品, 2 非药品
		/// </summary>
		public string ItemType
		{
			get
			{
				return this.itemType;
			}
			set
			{
				this.itemType = value;
			}
		}
         */


        /// <summary>
        /// 合同单位对照医保待遇dll名称
        /// </summary>
        public string PactDllName
        {
            get
            {
                return pactDllName;
            }
            set
            {
                pactDllName = value;
            }
        }
        /// <summary>
        /// 合同单位对照医保待遇dll描述
        /// </summary>
        public string PactDllDescription
        {
            get
            {
                return pactDllDescription;
            }
            set
            {
                pactDllDescription = value;
            }
        }
        /// <summary>
        /// 合同单位系统类别
        /// 适用类别 0=全院、1=门诊、2=住院、3=系统后台使用
        /// </summary>
        public string PactSystemType
        {
            get
            {
                return pactSystemType;
            }
            set
            {
                pactSystemType = value;
            }
        }
        #endregion

        #region 方法

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isDisposing"></param>
        protected override void Dispose(bool isDisposing)
        {
            if (this.alreadyDisposed)
            {
                return;
            }

            if (this.payKind != null)
            {
                this.payKind.Dispose();
                this.payKind = null;
            }
            /*if (this.rate != null)
            {
                this.rate.Dispose();
                this.rate = null;
            }*/

            base.Dispose(isDisposing);

            this.alreadyDisposed = true;
        }

        #endregion

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>当前对象实例的副本</returns>
        public new PactInfo Clone()
        {
            PactInfo pactInfo = base.Clone() as PactInfo;

            pactInfo.PayKind = this.PayKind.Clone();
            //pactInfo.Rate = this.Rate.Clone();

            return pactInfo;
        }

        #endregion

        #endregion

        #region 接口实现

        #region ISort 成员
        /// <summary>
        /// 排列序号
        /// </summary>
        /*public new int SortID
        {
            get
            {
                return this.sortID ;
            }
            set
            {
                this.sortID = value;
            }
        }*/
        #endregion

        #endregion

    }
}
