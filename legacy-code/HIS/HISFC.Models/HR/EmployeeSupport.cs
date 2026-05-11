using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>EmployeeSupport</br>
    /// <br>[功能描述: 职工支援边疆和少数民族地区、支援基层医疗实体]</br>
    /// <br>[创 建 者: 宋德宏]</br>
    /// <br>[创建时间: 2008-07-03]</br>
    ///     <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class EmployeeSupport : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 员工
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 项目名称
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject itemName=new NeuObject();

        /// <summary>
        /// 支边地点
        /// </summary>
        private NeuObject supplyPlace=new NeuObject();

        /// <summary>
        /// 项目来源
        /// </summary>
        private string itemSource = string.Empty;

        /// <summary>
        /// 起始时间
        /// </summary>
        private DateTime startDate=DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime endDate=DateTime.Now;
   
        /// <summary>
        /// 操作员
        /// </summary>
        Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();
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
        /// 项目名称
        /// </summary>
        public NeuObject ItemName
        {
            get 
            { 
                return itemName; 
            }
            set 
            { 
                itemName = value; 
            }
        }

        /// <summary>
        /// 项目来源
        /// </summary>
        public string ItemSource
        {
            get 
            { 
                return itemSource; 
            }
            set 
            { 
                itemSource = value; 
            }
        }

        /// <summary>
        /// 支边地点
        /// </summary>
        public NeuObject SupplyPlace
        {
            get 
            { 
                return supplyPlace; 
            }
            set 
            { 
                supplyPlace = value; 
            }
        }

        /// <summary>
        ///　超始时间
        /// </summary>
        public DateTime StartDate
        {
            get 
            { 
                return startDate; 
            }
            set 
            { 
                startDate = value; 
            }
        }

        /// <summary>
        /// 终止时间
        /// </summary>
        public DateTime EndDate
        {
            get 
            { 
                return endDate; 
            }
            set 
            { 
                endDate = value; 
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
        public new EmployeeSupport Clone()
        {
            EmployeeSupport employeeSupport = base.Clone() as EmployeeSupport;
            employeeSupport.Employee = this.Employee.Clone();
            employeeSupport.Oper = this.Oper.Clone();
        
            return employeeSupport;

        }
        #endregion



    }
}
