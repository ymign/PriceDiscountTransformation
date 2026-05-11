using System;
using System.Collections;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Base;
using Neusoft.FrameWork.Function;

namespace Neusoft.HISFC.Models.Fee.Inpatient
{
	/// <summary>
	/// Balance<br></br>
	/// [功能描述: 住院结算类]<br></br>
	/// [创 建 者: 王宇]<br></br>
	/// [创建时间: 2006-09-06]<br></br>
	/// <修改记录
	///		修改人=''
	///		修改时间='yyyy-mm-dd'
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary>
    /// 
    [System.Serializable]
	public class Balance : BalanceBase 
	{

        #region 变量
		
		/// <summary>
		/// 开始时间
		/// </summary>
		private DateTime beginTime;
		
		/// <summary>
		/// 结束时间
		/// </summary>
		private DateTime endTime;
		
		/// <summary>
		/// 打印次数
		/// </summary>
		private int printTimes;
		
		/// <summary>
		/// 审核序号
		/// </summary>
		private string auditingNO;

		/// <summary>
		/// 是否主发票
		/// </summary>
		private bool isMainInvoice;
		
		/// <summary>
		/// 是否为生育保险最后结算
		/// </summary>
		private bool isLastBalance;

        /// <summary>
        /// 是否临时发票号
        /// </summary>
        private bool isTempInvoice = false;

        /// <summary>
        /// 欠费结算处理标志
        /// </summary>
        private string balanceSaveType = string.Empty;

        /// <summary>
        /// 欠费结算处理标志
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment balanceSaveOper = new OperEnvironment();
        /// <summary>
        /// 发票上的打印号
        /// {F6C3D16B-8F52-4449-814C-5262F90C583B}
        /// </summary>
        private string printedInvoiceNO;


        //// {ED91FE21-D029-438f-8A4B-5E6C43A1C990}增加日结时间--2015-01-23
        /// <summary>
        /// 日结时间
        /// </summary>
        private DateTime dayTime;
		#endregion

		#region  属性

		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime BeginTime
		{
			get
			{
				return this.beginTime;
			}
			set
			{
				this.beginTime = value;
			}
		}
		
		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime EndTime		
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}
		
		/// <summary>
		/// 打印次数
		/// </summary>
		public int PrintTimes
		{
			get
			{
				return this.printTimes;
			}
			set
			{
				this.printTimes = value;
			}
		}
		
		/// <summary>
		/// 审核序号
		/// </summary>
		public string AuditingNO
		{
			get
			{
				return this.auditingNO;
			}
			set
			{
				this.auditingNO = value;
			}
		}
		
		/// <summary>
		/// 是否主发票
		/// </summary>
		public bool IsMainInvoice
		{
			get
			{
				return this.isMainInvoice;
			}
			set
			{
				this.isMainInvoice = value;
			}
		}
		
		/// <summary>
		/// 是否为生育保险最后结算
		/// </summary>
		public bool IsLastBalance
		{
			get
			{
				return this.isLastBalance;
			}
			set
			{
				this.isLastBalance = value;
			}
		}

        /// <summary>
        /// 是否为临时发票号
        /// </summary>
        public bool IsTempInvoice
        {
            get
            {
                return isTempInvoice;
            }
            set
            {
                isTempInvoice = value;
            }
        }

        /// <summary>
        /// 欠费结算处理标志
        /// </summary>
        public string BalanceSaveType
        {
            get
            {
                return balanceSaveType;
            }
            set
            {
                balanceSaveType = value;
            }
        }

        /// <summary>
        /// 欠费结算处理标志
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment BalanceSaveOper
        {
            get
            {
                return balanceSaveOper;
            }
            set
            {
                balanceSaveOper = value;
            }
        }
        /// <summary>
        /// 发票上的打印号
        /// 
        /// {F6C3D16B-8F52-4449-814C-5262F90C583B}
        /// </summary>
        public string PrintedInvoiceNO
        {
            get
            {
                return this.printedInvoiceNO;
            }
            set
            {
                this.printedInvoiceNO = value;
            }
        }


        //// {ED91FE21-D029-438f-8A4B-5E6C43A1C990}增加日结时间--2015-01-23
        /// <summary>
        /// 日结时间
        /// </summary>
        public DateTime DayTime
        {
            get
            {
                return this.dayTime;
            }
            set
            {
                this.dayTime = value;
            }
        }
		#endregion
		
        #region 方法

        #region 克隆
		
		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns>返回当前类的实例副本</returns>
		public new Balance Clone()
		{
			return base.Clone() as Balance;
		}

		#endregion
		
		#endregion
	}
}
