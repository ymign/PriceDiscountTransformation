using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.FrameWork.Models ;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>EmployeeEducation</br>
    /// <br>[功能描述: 继续教育管理实体]</br>
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
    public class EmployeeEducation : Neusoft.FrameWork.Models.NeuObject
    {
    
        #region 字段    
        /// <summary>
        /// 员工
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee =new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 培训项目
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject item=new NeuObject();
        
        /// <summary>
        /// 培训地点   
        /// </summary>
        private string place=string.Empty;
        
        /// <summary>
        /// 负责人
        /// </summary>
        private string manager=string.Empty ;
        
        /// <summary>
        /// 培训级别 
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject level=new NeuObject();

        /// <summary>
        ///培训开始时间 
        /// </summary>
        private DateTime startTime=DateTime.Now;
        
        /// <summary>
        /// 培训结束时间 
        /// </summary>
        private DateTime endTime=DateTime.Now;
        
        /// <summary>
        ///学时
        /// </summary>
        private decimal period=Decimal.MinValue;
        
        /// <summary>
        /// 学分
        /// </summary>
        private decimal credit = Decimal.MinValue;
        
        /// <summary>
        /// 培训结果评价
        /// </summary>
        private string evaluation=string.Empty;

        /// <summary>
        /// 第一次插入数据时间
        /// </summary>
        private DateTime insertTime = DateTime.Now;

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
        /// 培训项目
        /// </summary>
        public NeuObject Item
        {
            get 
            { 
                return item; 
            }
            set 
            { 
                item = value; 
            }
        }
 
        /// <summary>
        /// 培训地点
        /// </summary>
        public string Place
        {
            get 
            { 
                return place; 
            }
            set 
            { 
                place = value; 
            }
        }
     
        /// <summary>
        /// 负责人
        /// </summary>
        public string  Manager
        {
            get 
            { 
                return manager; 
            }
            set 
            { 
                manager = value; 
               
            }
        }
   
        /// <summary>
        /// 培训级别
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject  Level
        {
            get 
            { 
                return level; 
            }
            set 
            { 
                level = value; 
            }
        }
    
        /// <summary>
        /// 培训开始时间
        /// </summary>
        public DateTime StartTime
        {
            get 
            { 
                return startTime; 
            }
            set 
            { 
                startTime = value; 
            }
        }
    
        /// <summary>
        /// 培训结束时间
        /// </summary>
        public DateTime EndTime
        {
            get 
            { 
                return endTime; 
            }
            set 
            { 
                endTime = value; 
            }
        }

        /// <summary>
        /// 学时
        /// </summary>
        public decimal Period
        {
            get 
            { 
                return period; 
            }
            set 
            { 
                period = value; 
            }
        }
  
        /// <summary>
        /// 学分
        /// </summary>
        public decimal Credit
        {
            get 
            { 
                return credit; 
            }
            set 
            { 
                credit = value; 
            }
        }

        /// <summary>
        /// 培训结果评价
        /// </summary>
        public string Evaluation
        {
            get 
            { 
                return evaluation; 
            }
            set 
            { 
                evaluation = value; 
            }
        }

        /// <summary>
        /// 第一次插入时间
        /// </summary>
        public DateTime InsertTime
        {
            get 
            { 
                return insertTime; 
            }
            set 
            { 
                insertTime = value; 
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
        /// <returns>培训记录实体</returns>
        public new EmployeeEducation Clone()
        {
            EmployeeEducation employeeEducation = base.Clone() as EmployeeEducation;
            employeeEducation.Employee = this.Employee.Clone();
            employeeEducation.Level = this.Level.Clone();
            employeeEducation.Oper = this.Oper.Clone();
           
            return employeeEducation;
        }
        #endregion



    }
}
