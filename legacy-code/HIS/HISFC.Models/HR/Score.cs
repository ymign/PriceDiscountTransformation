using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// [功能描述: 成绩单类]<br></br>
    /// [创 建 者: 欧宪成]<br></br>
    /// [创建时间: 2008-07]<br></br>
    /// </summary>
    [System.Serializable]
    public class Score : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 考核计划单号
        /// </summary>
        string testPlanID;

        /// <summary>
        /// 员工
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject employee = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 科室
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject depart = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 入科时间
        /// </summary>
        DateTime inDepartDate;

        /// <summary>
        /// 考核时间
        /// </summary>
        DateTime testDate;

        /// <summary>
        /// 医德医风成绩
        /// </summary>
        decimal score1;

        /// <summary>
        /// 检诊成绩
        /// </summary>
        decimal score2;

        /// <summary>
        /// 值交接班成绩
        /// </summary>
        decimal score3;

        /// <summary>
        /// 查房成绩
        /// </summary>
        decimal score4;

        /// <summary>
        /// 病历质量成绩
        /// </summary>
        decimal score5;

        /// <summary>
        /// 病例分析成绩
        /// </summary>
        decimal score6;

        /// <summary>
        /// 操作成绩
        /// </summary>
        decimal score7;

        /// <summary>
        /// 加减成绩
        /// </summary>
        decimal score8;

        /// <summary>
        /// 合计
        /// </summary>
        decimal totalScore;

        /// <summary>
        /// 来院时间
        /// </summary>
        DateTime inCompannyDate;

        /// <summary>
        /// 所在单位
        /// </summary>
        string compannyName;

        /// <summary>
        /// 操作员
        /// </summary>
        Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 考核计划单号
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
        /// 入科时间
        /// </summary>
        public DateTime InDepartDate
        {
            get
            {
                return inDepartDate;
            }
            set
            {
                inDepartDate = value;
            }
        }

        /// <summary>
        /// 考核时间
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
        /// 医德医风成绩
        /// </summary>
        public decimal Score1
        {
            get
            {
                return score1;
            }
            set
            {
                score1 = value;
            }
        }

        /// <summary>
        /// 检诊成绩
        /// </summary>
        public decimal Score2
        {
            get
            {
                return score2;
            }
            set
            {
                score2 = value;
            }
        }

        /// <summary>
        /// 值交接班成绩
        /// </summary>
        public decimal Score3
        {
            get
            {
                return score3;
            }
            set
            {
                score3 = value;
            }
        }

        /// <summary>
        /// 查房成绩
        /// </summary>
        public decimal Score4
        {
            get
            {
                return score4;
            }
            set
            {
                score4 = value;
            }
        }

        /// <summary>
        /// 病历质量成绩
        /// </summary>
        public decimal Score5
        {
            get
            {
                return score5;
            }
            set
            {
                score5 = value;
            }
        }

        /// <summary>
        /// 病例分析成绩
        /// </summary>
        public decimal Score6
        {
            get
            {
                return score6;
            }
            set
            {
                score6 = value;
            }
        }

        /// <summary>
        /// 操作成绩
        /// </summary>
        public decimal Score7
        {
            get
            {
                return score7;
            }
            set
            {
                score7 = value;
            }
        }

        /// <summary>
        /// 加减成绩
        /// </summary>
        public decimal Score8
        {
            get
            {
                return score8;
            }
            set
            {
                score8 = value;
            }
        }

        /// <summary>
        /// 合计
        /// </summary>
        public decimal TotalScore
        {
            get
            {
                return totalScore;
            }
            set
            {
                totalScore = value;
            }
        }

        /// <summary>
        /// 来院时间
        /// </summary>
        public DateTime InCompannyDate
        {
            get
            {
                return inCompannyDate;
            }
            set
            {
                inCompannyDate = value;
            }
        }

        /// <summary>
        /// 所在单位
        /// </summary>
        public string CompannyName
        {
            get
            {
                return compannyName;
            }
            set
            {
                compannyName = value;
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

        public new Score Clone()
        {
            Score score = base.Clone() as Score;
            score.Depart = this.Depart;
            score.Employee = this.Employee;
            score.Oper = this.Oper;
            return score;
        }

        #endregion
    }
}
