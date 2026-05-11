using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>[功能描述: 轮转计划单类]</br>
    /// <br> [创 建 者: 欧宪成]</br>
    /// <br>[创建时间: 2008-07]</br>
    /// </summary>
    [System.Serializable]
    public class TurnPlan : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 轮转计划单号
        /// </summary>
        string cyclePlanNo;

        /// <summary>
        /// 员工
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject employee = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 科室
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject depart = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime cycleFromDate;

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime cycleToDate;

        /// <summary>
        ///  轮转方式
        /// </summary>
        string cycleType;

        /// <summary>
        /// 操作员
        /// </summary>
        Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();
        #endregion

        #region 属性
        /// <summary>
        /// 轮转计划单号
        /// </summary>
        public string CyclePlanNo
        {
            get
            {
                return cyclePlanNo;
            }
            set
            {
                cyclePlanNo = value;
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
        /// 开始时间
        /// </summary>
        public DateTime CycleFromDate
        {
            get
            {
                return cycleFromDate;
            }
            set
            {
                cycleFromDate = value;
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime CycleToDate
        {
            get
            {
                return cycleToDate;
            }
            set
            {
                cycleToDate = value;
            }
        }

        /// <summary>
        /// 轮转方式
        /// </summary>
        public string CycleType
        {
            get
            {
                return cycleType;
            }
            set
            {
                cycleType = value;
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
        public new TurnPlan Clone()
        {
            TurnPlan turnPlan = base.Clone() as TurnPlan;
            turnPlan.Employee = this.Employee;
            turnPlan.Depart = this.Depart;
            turnPlan.Oper = this.Oper;
            return turnPlan;
        }
        #endregion

    }
}
