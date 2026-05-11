using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// [功能描述: 轮转结束考试成绩类]<br></br>
    /// [创 建 者: 欧宪成]<br></br>
    /// [创建时间: 2008-07]<br></br>
    /// </summary>
    [System.Serializable]
    public class LastScore : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 考核单号
        /// </summary>
        string testBillCode;
        /// <summary>
        /// 员工
        /// </summary>
        Neusoft.HISFC.Models.Base.Employee empl = new Neusoft.HISFC.Models.Base.Employee();
        /// <summary>
        /// 所在单位
        /// </summary>
        string compannyName;
        /// <summary>
        /// 专业
        /// </summary>
        string speciality;
        /// <summary>
        /// 专业理论
        /// </summary>
        string specialityTheory;
        /// <summary>
        /// 成绩
        /// </summary>
        decimal specialityTheoryScore;
        /// <summary>
        /// 专业外语
        /// </summary>
        string foreignLanguage;
        /// <summary>
        /// 公共部份成绩
        /// </summary>
        decimal commonScore;
        /// <summary>
        /// 专业部分成绩
        /// </summary>
        decimal specialityScore;
        /// <summary>
        /// 合计成绩
        /// </summary>
        decimal totalScore;
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
        /// 考核单号
        /// </summary>
        public string TestBillCode
        {
            get
            {
                return testBillCode;
            }
            set
            {
                testBillCode = value;
            }
        }

        /// <summary>
        /// 员工
        /// </summary>
        public Neusoft.HISFC.Models.Base.Employee Empl
        {
            get
            {
                return empl;
            }
            set
            {
                empl = value;
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
        /// 专业
        /// </summary>
        public string Speciality
        {
            get
            {
                return speciality;
            }
            set
            {
                speciality = value;
            }
        }

        /// <summary>
        /// 专业理论
        /// </summary>
        public string SpecialityTheory
        {
            get
            {
                return specialityTheory;
            }
            set
            {
                specialityTheory = value;
            }
        }

        /// <summary>
        /// 专业理论成绩
        /// </summary>
        public decimal SpecialityTheoryScore
        {
            get
            {
                return specialityTheoryScore;
            }
            set
            {
                specialityTheoryScore = value;
            }
        }

        /// <summary>
        /// 专业外语
        /// </summary>
        public string ForeignLanguage
        {
            get
            {
                return foreignLanguage;
            }
            set
            {
                foreignLanguage = value;
            }
        }

        /// <summary>
        /// 公共部份成绩
        /// </summary>
        public decimal CommonScore
        {
            get
            {
                return commonScore;
            }
            set
            {
                commonScore = value;
            }
        }

        /// <summary>
        /// 专业部分成绩
        /// </summary>
        public decimal SpecialityScore
        {
            get
            {
                return specialityScore;
            }
            set
            {
                specialityScore = value;
            }
        }

        /// <summary>
        /// 合计成绩
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
        public new LastScore Clone()
        {
            LastScore lastScore = base.Clone() as LastScore;
            lastScore.Empl = this.Empl;
            lastScore.Oper = this.Oper;
            return lastScore;
        }
        #endregion
    }
}
