using System;
using System.Collections.Generic;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.Registration
{
    /// <summary>
    /// DayReport<br></br>
    /// [功能描述: 挂号日结信息实体]<br></br>
    /// [创 建 者: 黄小卫]<br></br>
    /// [创建时间: 2007-2-2]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [Serializable]
    public class DayReport:Neusoft.FrameWork.Models.NeuObject
	{
		public DayReport()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
        }

        #region 变量
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime beginDate = DateTime.MinValue;
        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime endDate = DateTime.MinValue;
        /// <summary>
        /// 日结明细处方累计数
        /// </summary>
        private int sumCount = 0;
        /// <summary>
        /// 挂号费总数
        /// </summary>
        private decimal sumRegFee = 0m;
        /// <summary>
        /// 诊查费总数
        /// </summary>
        private decimal sumDigFee = 0m;
        /// <summary>
        /// 检查费总数
        /// </summary>
        private decimal sumChkFee = 0m;
        /// <summary>
        /// 其他费总数
        /// </summary>
        private decimal sumOthFee = 0m;
        /// <summary>
        /// 现金总数
        /// </summary>
        private decimal sumOwnCost = 0m;
        /// <summary>
        /// 记帐总数
        /// </summary>
        private decimal sumPubCost = 0m;
        /// <summary>
        /// 自负总数
        /// </summary>
        private decimal sumPayCost = 0m;
        /// <summary>
        /// 日结明细
        /// </summary>
        private List<DayDetail> details = new List<DayDetail>();

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new OperEnvironment();

        /// <summary>
        /// 日结核查人
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperStat checker = new OperStat();

        /// <summary>
        /// 卡工本费-新增加
        /// </summary>
        private decimal sumCardFee = 0m;

        /// <summary>
        /// 病历本费-新增加
        /// </summary>
        private decimal sumCaseFee = 0m;

        /// <summary>
        /// 费用总计- 新增加
        /// </summary>
        private decimal sumTotal = 0m;

        #endregion

        #region 属性
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate
        {
            get { return this.beginDate; }
            set { this.beginDate = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value; }
        }

        /// <summary>
        /// 日结明细处方累计数
        /// </summary>
        public int SumCount
        {
            get { return this.sumCount; }
            set { this.sumCount = value; }
        }

        /// <summary>
        /// 挂号费总数
        /// </summary>
        public decimal SumRegFee
        {
            get { return this.sumRegFee; }
            set { this.sumRegFee = value; }
        }

        /// <summary>
        /// 诊查费总数
        /// </summary>
        public decimal SumDigFee
        {
            get { return this.sumDigFee; }
            set { this.sumDigFee = value; }
        }

        /// <summary>
        /// 检查费总数
        /// </summary>
        public decimal SumChkFee
        {
            get { return this.sumChkFee; }
            set { this.sumChkFee = value; }
        }

        /// <summary>
        /// 其他费总数
        /// </summary>
        public decimal SumOthFee
        {
            get { return this.sumOthFee; }
            set { this.sumOthFee = value; }
        }

        /// <summary>
        /// 现金总数
        /// </summary>
        public decimal SumOwnCost
        {
            get { return this.sumOwnCost; }
            set { this.sumOwnCost = value; }
        }

        /// <summary>
        /// 记帐总数
        /// </summary>
        public decimal SumPubCost
        {
            get { return this.sumPubCost; }
            set { this.sumPubCost = value; }
        }

        /// <summary>
        /// 自负总数
        /// </summary>
        public decimal SumPayCost
        {
            get { return this.sumPayCost; }
            set { this.sumPayCost = value; }
        }

        /// <summary>
        /// 日结明细
        /// </summary>
        public List<DayDetail> Details
        {
            get { return this.details; }
            set { this.details = value; }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get { return this.oper; }
            set { this.oper = value; }
        }

        /// <summary>
        /// 日结核查人
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperStat Checker
        {
            get { return this.checker; }
            set { this.checker = value; }
        }

        /// <summary>
        /// 卡工本费-新增加
        /// </summary>
        public decimal SumCardFee
        {
            get
            {
                return this.sumCardFee;
            }
            set
            {
                this.sumCardFee = value;
            }
        }

        /// <summary>
        /// 病历本费-新增加
        /// </summary>
        public decimal SumCaseFee
        {
            get
            {
                return this.sumCaseFee;
            }
            set
            {
                this.sumCaseFee = value;
            }
        }

        /// <summary>
        /// 费用总计- 新增加
        /// </summary>
        public decimal SumTotal
        {
            get { return sumTotal; }
            set { sumTotal = value; }
        }
        

        #endregion

        #region 方法
        /// <summary>
        ///clone
        /// </summary>
        /// <returns></returns>
        public new DayReport Clone()
        {
            DayReport dayReport = base.Clone() as DayReport;
                       
            dayReport.Checker = this.checker.Clone();
            dayReport.Oper = this.oper.Clone();

            return dayReport;
        }
        #endregion

    }

    /// <summary>
    /// 日结明细
    /// </summary>
    public class DayDetail:Neusoft.FrameWork.Models.NeuObject
    {
        public DayDetail() { }

        #region 变量
        /// <summary>
        /// 明细序号
        /// </summary>
        private string orderNO = "";
        /// <summary>
        /// 开始处方号
        /// </summary>
        private string beginRecipeNo = "";
        /// <summary>
        /// 结束处方号
        /// </summary>
        private string endRecipeNo = "";

        /// <summary>
        /// 处方总数
        /// </summary>
        private int count = 0;
        /// <summary>
        /// 挂号费总数
        /// </summary>
        private decimal regFee = 0m;
        /// <summary>
        /// 诊查费总数
        /// </summary>
        private decimal digFee = 0m;
        /// <summary>
        /// 检查费总数
        /// </summary>
        private decimal chkFee = 0m;
        /// <summary>
        /// 其他费总数
        /// </summary>
        private decimal othFee = 0m;
        /// <summary>
        /// 现金总数
        /// </summary>
        private decimal ownCost = 0m;
        /// <summary>
        /// 记帐总数
        /// </summary>
        private decimal pubCost = 0m;
        /// <summary>
        /// 自负总数
        /// </summary>
        private decimal payCost = 0m;

        /// <summary>
        /// 状态
        /// </summary>
        private EnumRegisterStatus status = EnumRegisterStatus.Valid;

        /// <summary>
        /// 卡工本费-新增加
        /// </summary>
        private decimal cardFee = 0m;

        /// <summary>
        /// 病历本费-新增加
        /// </summary>
        private decimal caseFee = 0m;

        /// <summary>
        /// 挂号全部费用 - 新增加
        /// </summary>
        private decimal totalFee = 0m;
        #endregion

        #region 属性
        /// <summary>
        /// 明细序号
        /// </summary>
        public string OrderNO
        {
            get { return this.orderNO; }
            set { this.orderNO = value; }
        }

        /// <summary>
        /// 开始处方号
        /// </summary>
        public string BeginRecipeNo
        {
            get { return this.beginRecipeNo; }
            set { this.beginRecipeNo = value; }
        }

        /// <summary>
        /// 结束处方号
        /// </summary>
        public string EndRecipeNo
        {
            get { return this.endRecipeNo; }
            set { this.endRecipeNo = value; }
        }

        /// <summary>
        /// 处方总数
        /// </summary>
        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        /// <summary>
        /// 挂号费总数
        /// </summary>
        public decimal RegFee
        {
            get { return this.regFee; }
            set { this.regFee = value; }
        }

        /// <summary>
        /// 诊查费总数
        /// </summary>
        public decimal DigFee
        {
            get { return this.digFee; }
            set { this.digFee = value; }
        }

        /// <summary>
        /// 检查费总数
        /// </summary>
        public decimal ChkFee
        {
            get { return this.chkFee; }
            set { this.chkFee = value; }
        }

        /// <summary>
        /// 其他费总数
        /// </summary>
        public decimal OthFee
        {
            get { return this.othFee; }
            set { this.othFee = value; }
        }

        /// <summary>
        /// 现金总数
        /// </summary>
        public decimal OwnCost
        {
            get { return this.ownCost; }
            set { this.ownCost = value; }
        }

        /// <summary>
        /// 记帐总数
        /// </summary>
        public decimal PubCost
        {
            get { return this.pubCost; }
            set { this.pubCost = value; }
        }

        /// <summary>
        /// 自负总数
        /// </summary>
        public decimal PayCost
        {
            get { return this.payCost; }
            set { this.payCost = value; }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public EnumRegisterStatus Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// 卡工本费-新增加
        /// </summary>
        public decimal CardFee
        {
            get
            {
                return this.cardFee;
            }
            set
            {
                this.cardFee = value;
            }
        }

        /// <summary>
        /// 病历本费-新增加
        /// </summary>
        public decimal CaseFee
        {
            get
            {
                return this.caseFee;
            }
            set
            {
                this.caseFee = value;
            }
        }

        /// <summary>
        /// 挂号全部费用 - 新增加
        /// </summary>
        public decimal TotalFee
        {
            get { return totalFee; }
            set { totalFee = value; }
        }

        #endregion

        #region clone
        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public new DayDetail Clone()
        {
            return base.Clone() as DayDetail;
        }
        #endregion
    }
}

