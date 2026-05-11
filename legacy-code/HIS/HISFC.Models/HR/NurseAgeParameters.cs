using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    ///<br> NurseAgeParameters</br>
    ///<br> [功能描述: 护龄工资实体]</br>
    ///<br> [创 建 者: 宋德宏]</br>
    ///<br> [创建时间: 2008-07-03]</br>
    ///     <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class NurseAgeParameters : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 工作年限下限
        /// </summary>
        private decimal workYearLow = Decimal.MinValue;

        /// <summary>
        /// 工作年限上限
        /// </summary>
        private decimal workYearHigh = Decimal.MinValue;

        /// <summary>
        /// 护龄工资
        /// </summary>
        private decimal nurseAgePay = Decimal.MinValue;

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new OperEnvironment();
        #endregion

        #region 属性
        /// <summary>
        /// 工作年限下限
        /// </summary>
        public decimal WorkYearLow
        {
            get 
            { 
                return workYearLow; 
            }
            set 
            { 
                workYearLow = value; 
            }
        }

        /// <summary>
        /// 工作年限上限
        /// </summary>
        public decimal WorkYearHigh
        {
            get 
            { 
                return workYearHigh;
            }
            set 
            { 
                workYearHigh = value; 
            }
        }

        /// <summary>
        /// 护龄工资
        /// </summary>
        public decimal NurseAgePay
        {
            get 
            { 
                return nurseAgePay; 
            }
            set 
            { 
                nurseAgePay = value; 
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
        #endregion

        #region 克隆方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new NurseAgeParameters Clone()
        {
            NurseAgeParameters nurseAgeParameters = base.Clone() as NurseAgeParameters;      
            nurseAgeParameters.Oper = this.Oper.Clone();

            return nurseAgeParameters;

        }
        #endregion

    }
}
