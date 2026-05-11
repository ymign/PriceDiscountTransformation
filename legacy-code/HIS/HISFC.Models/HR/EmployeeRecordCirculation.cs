using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>EmployeeRecordCirculation</br>
    /// <br>[功能描述: 员工档案流转实体]</br>
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
    public class EmployeeRecordCirculation : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 员工档案对象
        /// </summary>
        private Neusoft.HISFC.Models.HR.EmployeeRecord emplRecord = new EmployeeRecord();

        /// <summary>
        /// 档案去向
        /// </summary>
        private string recordPlace=string.Empty;

        /// <summary>
        /// 接收单位
        /// </summary>
        private string inPlace = string.Empty;

        /// <summary>
        /// 查阅时间
        /// </summary>
        private DateTime queryDate = new DateTime();

        /// <summary>
        /// 借阅时间
        /// </summary>
        private DateTime borrowDate = new DateTime();


        /// <summary>
        /// 借阅人
        /// </summary>
        private string borrowPerson = string.Empty;

        /// <summary>
        ///离职时间
        /// </summary>
        private DateTime leaveDate = new DateTime();

        /// <summary>
        ///转出时间
        /// </summary>
        private DateTime outDate = new DateTime();
 
        #endregion

        #region 属性
        /// <summary>
        /// 档案对象
        /// </summary>
        public Neusoft.HISFC.Models.HR.EmployeeRecord EmplRecord
        {
            get 
            { 
                return emplRecord; 
            }
            set 
            { 
                emplRecord = value; 
            }
        }

        /// <summary>
        /// 存档去向
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
        /// 接收单位
        /// </summary>
        public string InPlace
        {
            get 
            { 
                return inPlace; 
            }
            set 
            { 
                inPlace = value; 
            }
        }

        /// <summary>
        /// 查阅时间
        /// </summary>
        public DateTime QueryDate
        {
            get 
            {
                return queryDate; 
            }
            set 
            { 
                queryDate = value; 
            }
        }

        /// <summary>
        /// 借阅时间
        /// </summary>
        public DateTime BorrowDate
        {
            get 
            { 
                return borrowDate; 
            }
            set 
            { 
                borrowDate = value; 
            }
        }

        /// <summary>
        /// 借阅人
        /// </summary>
        public string BorrowPerson
        {
            get 
            { 
                return borrowPerson; 
            }
            set 
            { 
                borrowPerson = value; 
            }
        }

        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime LeaveDate
        {
            get 
            { 
                return leaveDate; 
            }
            set 
            { 
                leaveDate = value; 
            }
        }

        /// <summary>
        /// 转出时间
        /// </summary>
        public DateTime OutDate
        {
            get 
            { 
                return outDate; 
            }
            set 
            { 
                outDate = value; 
            }
        }
        #endregion

        #region 克隆方法
        /// <summary>
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public new EmployeeRecordCirculation Clone()
        {
            EmployeeRecordCirculation employeeRecordCirculation = base.Clone() as EmployeeRecordCirculation;
            employeeRecordCirculation.EmplRecord = this.EmplRecord.Clone();
           
            return employeeRecordCirculation;
        }
        #endregion

    }
}
