using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br> PayList</br>
    /// <br>[功能描述: 工资细项实体]</br>
    /// <br> [创 建 者: 宋德宏]</br>
    /// <br>[创建时间: 2008-07-18]</br>
    ///     <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class PayItem : Spell, Base.ISort, Base.IValid
    {
 
        #region 字段
        /// <summary>
        /// 是否是复合项目
        /// </summary>
        private bool isComplexItem = false;

        /// <summary>
        /// 是否是输入项目
        /// </summary>
        private bool isInputItem = false;

        /// <summary>
        /// 是否是工资基项
        /// </summary>
        private bool isPayBase = false;

        /// <summary>
        /// 复合公式
        /// </summary>
        private string complexFormula = string.Empty;

        /// <summary>
        /// 是否是保险类型
        /// </summary>
        private bool isInsuranceType = false;

        /// <summary>
        /// 计算类型
        /// </summary>
        private string computeType = string.Empty;

        /// <summary>
        /// 排序
        /// </summary>
        private int sortID=0;

        /// <summary>
        /// 有效性
        /// </summary>
        private bool isValid=true;

        /// <summary>
        /// 操作信息
        /// </summary>				
        private OperEnvironment oper = new OperEnvironment();
        #endregion

        #region 属性
        /// <summary>
        /// 是否是复合项目
        /// </summary>
        public bool IsComplexItem
        {
            get 
            { 
                return isComplexItem; 
            }
            set 
            { 
                isComplexItem = value; 
            }
        }

        /// <summary>
        /// 是否是输入项目
        /// </summary>
        public bool IsInputItem
        {
            get 
            { 
                return isInputItem; 
            }
            set 
            { 
                isInputItem = value; 
            }
        }

        /// <summary>
        /// 是否是工资基数
        /// </summary>
        public bool IsPayBase
        {
            get 
            { 
                return isPayBase; 
            }
            set 
            { 
                isPayBase = value; 
            }
        }

        /// <summary>
        /// 复合项公式
        /// </summary>
        public string ComplexFormula
        {
            get 
            { 
                return complexFormula; 
            }
            set 
            { 
                complexFormula = value; 
            }
        }

        /// <summary>
        /// 是否是保险类型
        /// </summary>
        public bool IsInsuranceType
        {
            get 
            { 
                return isInsuranceType; 
            }
            set 
            { 
                isInsuranceType = value; 
            }
        }

        /// <summary>
        /// 计算类型
        /// </summary>
        public string ComputeType
        {
            get 
            { 
                return computeType; 
            }
            set 
            { 
                computeType = value; 
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortID
        {
            get
            {
                return this.sortID;
            }
            set
            {
                sortID = value;
            }
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }

        /// <summary>
        /// 操作环境
        /// </summary>
        public OperEnvironment Oper
        {
            get
            {
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }
        #endregion

        #region 克隆
        /// <summary>
        /// 克隆函数
        /// </summary>
        /// <returns>Const类实例</returns>
        public new PayItem Clone()
        {
            PayItem payItem = base.Clone() as PayItem;
            payItem.Oper = this.Oper.Clone();

            return payItem;

        }
        #endregion
    }
}
