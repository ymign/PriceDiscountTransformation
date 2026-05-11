using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// Neusoft.HISFC.Models.HR.WorkCheckDeduct<br></br>
    /// [功能描述: 考勤扣款实体类]<br></br>
    /// [创 建 者: 赵阳]<br></br>
    /// [创建时间: 2008-07-11]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class WorkCheckDeduct : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 考勤类型
        /// </summary>
        private string workCheckType = "";

        /// <summary>
        /// 工资细目
        /// </summary>
        private string payItem = "";

        /// <summary>
        /// 饭补标准
        /// </summary>
        private decimal foodSubsidy = 0;

        /// <summary>
        /// 扣款比例
        /// </summary>
        private decimal deductPercent = 0;

        /// <summary>
        /// 扣款比例上限
        /// </summary>
        private decimal deductPercentLimit = 0;

        /// <summary>
        /// 扣款金额
        /// </summary>
        private decimal deductAmount = 0;

        /// <summary>
        /// 扣款金额上限
        /// </summary>
        private decimal deductAmountLimit = 0;

        /// <summary>
        /// 缺勤次数/天数上限
        /// </summary>
        private decimal checkNumLimit = 0;

        /// <summary>
        /// 造作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();


        #endregion

        #region 属性

        /// <summary>
        /// 缺勤原因
        /// </summary>
        public string WorkCheckType
        {
            get
            {
                return workCheckType;
            }
            set
            {
                workCheckType = value;
            }
        }

        /// <summary>
        /// 工资细目
        /// </summary>
        public string PayItem
        {
            get
            {
                return payItem;
            }
            set
            {
                payItem = value;
            }
        }

        /// <summary>
        /// 饭补标准
        /// </summary>
        public decimal FoodSubsidy
        {
            get
            {
                return foodSubsidy;
            }
            set
            {
                foodSubsidy = value;
            }
        }

        /// <summary>
        /// 扣款比例
        /// </summary>
        public decimal DeductPercent
        {
            get
            {
                return deductPercent;
            }
            set
            {
                deductPercent = value;
            }
        }

        /// <summary>
        /// 扣款比例上限
        /// </summary>
        public decimal DeductPercentLimit
        {
            get
            {
                return deductPercentLimit;
            }
            set
            {
                deductPercentLimit = value;
            }
        }

        /// <summary>
        /// 扣款金额
        /// </summary>
        public decimal DeductAmount
        {
            get
            {
                return deductAmount;
            }
            set
            {
                deductAmount = value;
            }
        }

        /// <summary>
        /// 扣款金额上限
        /// </summary>
        public decimal DeductAmountLimit
        {
            get
            {
                return deductAmountLimit;
            }
            set
            {
                deductAmountLimit = value;
            }
        }

        /// <summary>
        /// 缺勤次数/天数上限
        /// </summary>
        public decimal CheckNumLimit
        {
            get
            {
                return checkNumLimit;
            }
            set
            {
                checkNumLimit = value;
            }
        }

        /// <summary>
        /// 造作员
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

        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new WorkCheckDeduct Clone()
        {
            WorkCheckDeduct workCheckDeduct = base.Clone() as WorkCheckDeduct;

            workCheckDeduct.Oper = this.Oper.Clone();

            return workCheckDeduct;
        }
        #endregion
    }
}
