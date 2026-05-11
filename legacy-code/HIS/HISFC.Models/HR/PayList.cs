using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br> PayList</br>
    /// <br>[功能描述: 工资单实体]</br>
    /// <br> [创 建 者: 宋德宏]</br>
    /// <br>[创建时间: 2008-07-16]</br>
    ///     <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class PayList : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 员工
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Employee();

        /// <summary>
        /// 年度和月份
        /// </summary>
        private string yearAndMonth = string.Empty;

        /// <summary>
        /// 工资细项代码
        /// </summary>
        private string payItemCode = string.Empty;
       
        /// <summary>
        /// 工资细项值
        /// </summary>
        private decimal payItemValue = Decimal.MinValue;
    
        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new OperEnvironment();
       #endregion

        #region 属性
        /// <summary>
        /// 员工
        /// </summary>
        public Neusoft.HISFC.Models.Base.Employee Employee
        {
            get 
            { 
                return employee; 
            }
            set 
            { 
                employee = value; 
            }
        }

        /// <summary>
        /// 年度和月份
        /// </summary>
        public string YearAndMonth
        {
            get 
            { 
                return yearAndMonth; 
            }
            set 
            { 
                yearAndMonth = value; 
            }
        }

        /// <summary>
        /// 工资细项代码
        /// </summary>
        public string PayItemCode
        {
            get 
            { 
                return payItemCode; 
            }
            set 
            { 
                payItemCode = value; 
            }
        }

        /// <summary>
        /// 工资细项值
        /// </summary>
        public decimal PayItemValue
        {
            get 
            { 
                return payItemValue; 
            }
            set 
            { 
                payItemValue = value; 
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
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public new PayList Clone()
        {
            PayList payList = base.Clone() as PayList;
            payList.Oper = this.Oper.Clone();
            payList.Employee = this.Employee.Clone();

            return payList;
        }
        #endregion

    }
}
