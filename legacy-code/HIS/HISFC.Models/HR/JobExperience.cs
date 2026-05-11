using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    ///<br>JobExperience</br>
    ///<br> [功能描述: 工作经历实体]</br>
    ///<br> [创 建 者: 宋德宏]</br>
    ///<br>[创建时间: 2008-07-03]</br>
    ///    <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class JobExperience : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 员工
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 任职时间
        /// </summary>
        private DateTime startDate=DateTime.Now;

        /// <summary>
        /// 终止时间
        /// </summary>
        private DateTime endDate=DateTime.Now;

        /// <summary>
        /// 任职性质
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject postKind=new NeuObject();

        /// <summary>
        ///　单位名称 
        /// </summary>
        private string jobPalce=string.Empty;

        /// <summary>
        ///　聘期 
        /// </summary>
        private int useTime;

        /// <summary>
        /// 是否是特殊经历
        /// </summary>
        private bool is_Special = false;

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
        /// 任职日期
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
        /// 终止日期
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
        /// 任职性质
        /// </summary>
        public NeuObject PostKind
        {
            get 
            { 
                return postKind; 
            }
            set 
            { 
                postKind = value; 
            }
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string JobPalce
        {
            get 
            { 
                return jobPalce; 
            }
            set 
            { 
                jobPalce = value; 
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
        /// 聘期
        /// </summary>
        public int UseTime
        {
            get 
            { 
                return useTime; 
            }
            set
            { 
                useTime = value; 
            }
        }

        /// <summary>
        /// 是否是特殊经历
        /// </summary>
        public bool Is_Special
        {
            get 
            { 
                return is_Special; 
            }
            set 
            { 
                is_Special = value;
            }
        }

        #endregion

        #region 克隆方法

        /// <summary>
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public new JobExperience Clone()
        {
            JobExperience jobExperience = base.Clone() as JobExperience;
            jobExperience.PostKind = this.PostKind.Clone();
            jobExperience.Employee = this.Employee.Clone();
            jobExperience.Oper = this.Oper.Clone();

            return jobExperience;

        }
        #endregion

    }
}
