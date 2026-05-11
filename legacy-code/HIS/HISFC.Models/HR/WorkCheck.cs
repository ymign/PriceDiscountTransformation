using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// Neusoft.HISFC.Models.HR.WorkCheck<br></br>
    /// [功能描述: 考勤实体 对应表为 goa_check_work]<br></br>
    /// [创 建 者: 赵阳]<br></br>
    /// [创建时间: 2008-07-03]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class WorkCheck : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 年度月份
        /// </summary>
        private string yearAndMonth = "";

        /// <summary>
        /// 员工实体。
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 考勤类型
        /// </summary>
        private string checkType = "";

        /// <summary>
        /// 考勤天数/次数
        /// </summary>
        private decimal checkNum = 0;

        /// <summary>
        /// 本月天数
        /// </summary>
        private decimal monthDays = 0;

        /// <summary>
        /// 本月工作日天数
        /// </summary>
        private decimal monthWorkDays = 0;
        
        /// <summary>
        /// 考勤状态0保存，1提交
        /// </summary>
        private string  checkStatus;

        /// <summary>
        /// 操作员。
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 年度月份
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
        /// 员工实体
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
        /// 缺勤类型
        /// </summary>
        public string CheckType
        {
            get
            {
                return checkType;
            }
            set
            {
                checkType = value;
            }
        }

        /// <summary>
        /// 缺勤天数
        /// </summary>
        public decimal CheckNum
        {
            get
            {
                return checkNum;
            }
            set
            {
                checkNum = value;
            }
        }

        /// <summary>
        /// 本月天数
        /// </summary>
        public decimal MonthDays
        {
            get
            {
                return monthDays;
            }
            set
            {
                monthDays = value;
            }
        }

        /// <summary>
        /// 本月工作日天数
        /// </summary>
        public decimal MonthWorkDays
        {
            get
            {
                return monthWorkDays;
            }
            set
            {
                monthWorkDays = value;
            }
        }

        /// <summary>
        /// 考勤状态0保存，1提交
        /// </summary>
        public string CheckStatus
        {
            get
            {
                return checkStatus;
            }
            set
            {
                checkStatus = value;
            }
        }

        /// <summary>
        /// 操作员。
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
        public new WorkCheck Clone()
        {
            WorkCheck workCheck = base.Clone() as WorkCheck;

            workCheck.Employee = this.Employee.Clone();
            workCheck.Oper = this.Oper.Clone();

            return workCheck;
        }
        #endregion
    }
}
