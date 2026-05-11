using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>[功能描述: 考核计划单类]</br>
    /// <br>[创 建 者: 欧宪成]</br>
    /// <br>[创建时间: 2008-07]</br>
    /// </summary>
    [System.Serializable]
    public class TestPlan : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 计划单号
        /// </summary>
        string testPlanID;

        /// <summary>
        /// 考核计划编码
        /// </summary>
        string testPlanCode;

        /// <summary>
        /// 员工
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject employee = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 科室
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject depart = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 出科日期
        /// </summary>
        DateTime outDepartDate;

        /// <summary>
        /// 考核日期
        /// </summary>
        DateTime testDate;

        /// <summary>
        /// 考核组专家名单
        /// </summary>
        string expertList1;

        /// <summary>
        /// 督察组名单
        /// </summary>
        string expertList2;

        /// <summary>
        /// 备注
        /// </summary>
        string remark;

        /// <summary>
        /// 操作员
        /// </summary>
        Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();


        #endregion

        #region 属性

        /// <summary>
        /// 计划单号
        /// </summary>
        public string TestPlanID
        {
            get
            {
                return testPlanID;
            }
            set
            {
                testPlanID = value;
            }
        }

        /// <summary>
        /// 考核计划编码
        /// </summary>
        public string TestPlanCode
        {
            get 
            {
                return testPlanCode; 
            }
            set 
            {
                testPlanCode = value; 
            }
        }

        /// <summary>
        /// 员工
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Employee
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
        /// 科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Depart
        {
            get
            {
                return depart;
            }
            set
            {
                depart = value;
            }
        }

        /// <summary>
        /// 出科日期
        /// </summary>
        public DateTime OutDepartDate
        {
            get
            {
                return outDepartDate;
            }
            set
            {
                outDepartDate = value;
            }
        }

        /// <summary>
        /// 考核日期
        /// </summary>
        public DateTime TestDate
        {
            get
            {
                return testDate;
            }
            set
            {
                testDate = value;
            }
        }

        /// <summary>
        /// 考核组专家名单
        /// </summary>
        public string ExpertList1
        {
            get
            {
                return expertList1;
            }
            set
            {
                expertList1 = value;
            }
        }

        /// <summary>
        /// 督察组名单
        /// </summary>
        public string ExpertList2
        {
            get
            {
                return expertList2;
            }
            set
            {
                expertList2 = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>

        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
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

        #region 方法
        public new TestPlan Clone()
        {
            TestPlan testPlan = base.Clone() as TestPlan;
            testPlan.Employee = this.Employee;
            testPlan.Depart = this.Depart;
            testPlan.Oper = this.Oper;
            return testPlan;
        }
        #endregion
    }



}
