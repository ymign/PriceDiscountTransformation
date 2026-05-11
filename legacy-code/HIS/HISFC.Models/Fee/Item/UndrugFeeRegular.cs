using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Fee.Item
{
    /// <summary>
    /// 规则收费实体
    /// </summary>
    public class UndrugFeeRegular:Neusoft.HISFC.Models.Base.Spell
    {
        #region 变量

        /// <summary>
        /// 项目
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject item = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 收费规则
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject regular =new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 限制条件(按时间,数量)
        /// </summary>
        private string limitCondition = "";

        /// <summary>
        /// 数量限额
        /// </summary>
        private decimal dayLimit = 0;

        /// <summary>
        /// 互斥项目
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject limitItem = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 操作人
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        /// <summary>
        /// 是否出院计费
        /// </summary>
        private bool isOutFee = true;
        #endregion

        #region 属性

        /// <summary>
        /// 项目编码
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Item
        {
            get { return item; }
            set { item = value; }
        }

        /// <summary>
        /// 收费规则
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Regular
        {
            get { return regular; }
            set { regular = value; }
        }

        /// <summary>
        /// 限制条件(按时间,数量)
        /// </summary>
        public string LimitCondition
        {
            get { return limitCondition; }
            set { limitCondition = value; }
        }

        /// <summary>
        /// 数量限额  (收费规则为时间时为小时数)
        /// </summary>
        public decimal DayLimit
        {
            get { return dayLimit; }
            set { dayLimit = value; }
        }

        /// <summary>
        /// 互斥项目
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject LimitItem
        {
            get { return limitItem; }
            set { limitItem = value; }
        }

        /// <summary>
        /// 操作人
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                return oper;
            }
            set
            {
                oper = value;
            }
        }
        
        /// <summary>
        /// 是否出院计费
        /// </summary>
        public bool IsOutFee
        {
            get
            {
                return isOutFee;
            }
            set
            {
                isOutFee = value;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new UndrugFeeRegular Clone()
        {
            UndrugFeeRegular undrugFeeRegular = base.Clone() as UndrugFeeRegular;

            undrugFeeRegular.item = this.Item.Clone();
            undrugFeeRegular.regular = this.Regular.Clone();
            undrugFeeRegular.limitItem = this.LimitItem.Clone();
            undrugFeeRegular.oper = this.Oper.Clone();

            return undrugFeeRegular;
        }

        #endregion
    }
}
