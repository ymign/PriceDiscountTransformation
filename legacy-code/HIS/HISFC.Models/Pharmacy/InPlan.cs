using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy
{
    /// <summary>
    /// [功能描述: 入库计划类]<br></br>
    /// [创 建 者: 梁俊泽]<br></br>
    /// [创建时间: 2007-02-03]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    ///  ID 入库计划流水号
    /// </summary>
    [Serializable]
    public class InPlan : Base.PlanBase
    {
        public InPlan() 
		{

		}

		#region 变量

		/// <summary>
		/// 采购类型
		/// </summary>
		private System.String myPlanType ;

        /// <summary>
        /// 采购单流水号
        /// </summary>
        private string stockNO;

        /// <summary>
        ///  采购类型
        /// </summary>
        private string stockType;

        /// <summary>
        /// 作废、替代计划单流水号 多条时以 '|' 分割 对作废计划单 存储新合并计划单流水号 对新合并计划单 存储原计划单流水号
        /// </summary>
        private string replacePlanNO;

		#endregion

        /// <summary>
        /// 是否包含多条采购记录
        /// </summary>
        public bool IsMultiStockRecord
        {
            get
            {
                if (this.stockNO.IndexOf("|") != -1)
                    return true;
                else
                    return false;
            }
        }

		/// <summary>
		/// 采购类型0手工计划，1警戒线，2消耗，3时间，4日消耗
		/// </summary>
		public System.String PlanType 
		{
			get
			{
				return this.myPlanType; 
			}
			set
			{
				this.myPlanType = value; 
			}
		}

        /// <summary>
        /// 采购类型 0 正常 1 并单 2 拆单
        /// </summary>
        public string StockType
        {
            get
            {
                return this.stockType;
            }
            set
            {
                this.stockType = value;
            }
        }

        /// <summary>
        /// 采购流水号 多条采购记录时以 '|' 分割
        /// </summary>
        public string StockNO
        {
            get
            {
                return this.stockNO;
            }
            set
            {
                this.stockNO = value;
            }
        }

        /// <summary>
        /// 作废、替代计划单流水号 多条时以 '|' 分割 对作废计划单 存储新合并计划单流水号 对新合并计划单 存储原计划单流水号
        /// </summary>
        public string ReplacePlanNO
        {
            get
            {
                return this.replacePlanNO;
            }
            set
            {
                this.replacePlanNO = value;
            }
        }

		#region 方法

		/// <summary>
		/// 克隆函数
		/// </summary>
		/// <returns>成功返回当前实例的克隆实体</returns>
		public new InPlan Clone()
		{
            InPlan inPlan = base.Clone() as InPlan;

			return inPlan;
		}


		#endregion


        #region 华南新增

        private decimal stockQty;

        /// <summary>
        /// 采购量
        /// </summary>
        public decimal StockQty
        {
            get { return stockQty; }
            set { stockQty = value; }
        }

        private Neusoft.FrameWork.Models.NeuObject company = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 供货公司
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Company
        {
            get { return company; }
            set { company = value; }
        }

        private decimal inQty;

        /// <summary>
        /// 入库量
        /// </summary>
        public decimal InQty
        {
            get { return inQty; }
            set { inQty = value; }
        }

        private Neusoft.HISFC.Models.Base.OperEnvironment inOper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        /// <summary>
        /// 入库人
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment InOper
        {
            get { return inOper; }
            set { inOper = value; }
        }

        private string stencilName = "";

        /// <summary>
        /// 模板名称
        /// </summary>
        public string StencilName
        {
            get { return stencilName; }
            set { stencilName = value; }
        }

        private string formula = "";

        /// <summary>
        /// 生成计划使用的公式
        /// </summary>
        public string Formula
        {
            get { return formula; }
            set { formula = value; }
        }


        private decimal store_totxhsum;

        /// <summary>
        /// 全院消耗数量总和
        /// </summary>
        public decimal Store_totxhsum
        {
            get { return store_totxhsum; }
            set { store_totxhsum = value; }
        }


        private decimal rstock_num;

        /// <summary>
        /// 预采购量
        /// </summary>
        public decimal Rstock_num
        {
            get { return rstock_num; }
            set { rstock_num = value; }
        }
        #endregion
    }
}
