using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// [功能描述: 奖金类]<br></br>
    /// [创 建 者: 欧宪成]<br></br>
    /// [创建时间: 2008-07]<br></br>
    /// </summary>
    [System.Serializable]
    public class Premium : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 年度月份
        /// </summary>
        string bonusDate;

        /// <summary>
        /// 员工
        /// </summary>
        Neusoft.HISFC.Models.Base.Employee empl = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 奖金系数
        /// </summary>
        decimal bonusQuotiety;

        /// <summary>
        /// 奖金基数
        /// </summary>
        decimal bonusBase;

        /// <summary>
        /// 奖金金额
        /// </summary>
        decimal bonusCost;

        /// <summary>
        /// 操作员
        /// </summary>
        Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion 字段

        #region 属性
        /// <summary>
        /// 年度月份
        /// </summary>
        public string BonusDate
        {
            get
            {
                return bonusDate;
            }
            set
            {
                bonusDate = value;
            }
        }

        /// <summary>
        /// 员工
        /// </summary>
        public Neusoft.HISFC.Models.Base.Employee Empl
        {
            get
            {
                return empl;
            }
            set
            {
                empl = value;
            }
        }

        /// <summary>
        /// 奖金系数
        /// </summary>
        public decimal BonusQuotiety
        {
            get
            {
                return bonusQuotiety;
            }
            set
            {
                bonusQuotiety = value;
            }
        }

        /// <summary>
        /// 奖金基数
        /// </summary>
        public decimal BonusBase
        {
            get
            {
                return bonusBase;
            }
            set
            {
                bonusBase = value;
            }
        }

        /// <summary>
        /// 奖金金额
        /// </summary>
        public decimal BonusCost
        {
            get
            {
                return bonusCost;
            }
            set
            {
                bonusCost = value;
            }
        }

        /// <summary>
        /// 操作员
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
        #endregion 属性

        #region 克隆
        public new Premium Clone()
        {
            Premium premium = base.Clone() as Premium;
            premium.Oper = this.Oper;
            premium.Empl = this.Empl;
            return premium;
        }
        #endregion 克隆

    }
}
