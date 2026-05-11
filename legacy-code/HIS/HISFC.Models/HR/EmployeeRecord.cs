using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>EmployeeRecord</br>
    /// [功能描述: 人事档案管理实体]<br></br>
    /// [创 建 者: 宋德宏]<br></br>
    /// [创建时间: 2008-07-03]<br></br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class EmployeeRecord : Neusoft.FrameWork.Models.NeuObject
    {

        #region 字段

        /// <summary>
        /// 员工
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 档案状态
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject recordStatus=new NeuObject();

        /// <summary>
        /// 存档位置
        /// </summary>
        private string recordPlace=string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        private SexEnumService sex = new SexEnumService();

        /// <summary>
        /// 档案去向
        /// </summary>
        private string record_In;

        /// <summary>
        /// 顺序号
        /// </summary>
        private int sortID;

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
        /// 存档位置
        /// </summary>
        public string RecordPlace
        {
            get 
            { 
                return recordPlace; 
            }
            set 
            { 
                recordPlace = value; 
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public SexEnumService Sex
        {
            get
            {
                return this.sex;
            }
            set
            {
                this.sex = value;
            }
        }

        /// <summary>
        /// 存档去向
        /// </summary>
        public string Record_In
        {
            get
            {
                return record_In;
            }
            set
            {
                record_In = value;
            }
        }

        /// <summary>
        /// 档案状态
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject RecordStatus
        {
            get 
            { 
                return recordStatus; 
               
            }
            set 
            { 
                recordStatus = value; 
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
        #endregion

        #region 克隆方法

        /// <summary>
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public new EmployeeRecord Clone()
        {
            EmployeeRecord employeeRecord = base.Clone() as EmployeeRecord;
            employeeRecord.Employee = this.Employee.Clone();
            employeeRecord.RecordStatus = this.RecordStatus.Clone();
            employeeRecord.Oper = this.Oper.Clone();

            return employeeRecord;

        }
        #endregion
    }
}
