using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// Neusoft.HISFC.Models.HR.RewardPunish<br></br>
    /// [功能描述: 奖惩实体]<br></br>
    /// [创 建 者: 赵阳]<br></br>
    /// [创建时间: 2008-07-11]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class RewardPunish : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段
        /// <summary>
        /// 发生序号
        /// </summary>
        private string happenNo = "";

        /// <summary>
        /// 员工实体
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 奖惩时间
        /// </summary>
        private DateTime rewardFunishTime;

        /// <summary>
        /// 奖惩级别
        /// </summary>
        private string rewardFunishLevel = "";

        /// <summary>
        /// 评奖单位
        /// </summary>
        private string rewardOrganization = "";

        /// <summary>
        /// 奖惩分类
        /// </summary>
        private string rewardFunishCatagory = "";

        /// <summary>
        /// 奖惩内容
        /// </summary>
        private string rewardFunishContent = "";

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

         /// <summary>
        /// 发生序号
        /// </summary>
        public string HappenNo
        {
            get
            {
                return happenNo;
            }
            set
            {
                happenNo = value;
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
        /// 奖惩时间
        /// </summary>
        public DateTime RewardFunishTime
        {
            get
            {
                return rewardFunishTime;
            }
            set
            {
                rewardFunishTime = value;
            }
        }

        /// <summary>
        /// 奖惩级别
        /// </summary>
        public string RewardFunishLevel
        {
            get
            {
                return rewardFunishLevel;
            }
            set
            {
                rewardFunishLevel = value;
            }
        }

        /// <summary>
        /// 评奖单位
        /// </summary>
        public string RewardOrganization
        {
            get
            {
                return rewardOrganization;
            }
            set
            {
                rewardOrganization = value;
            }
        }

        /// <summary>
        /// 奖惩分类
        /// </summary>
        public string RewardFunishCatagory
        {
            get
            {
                return rewardFunishCatagory;
            }
            set
            {
                rewardFunishCatagory = value;
            }
        }

        /// <summary>
        /// 奖惩内容
        /// </summary>
        public string RewardFunishContent
        {
            get
            {
                return rewardFunishContent;
            }
            set
            {
                rewardFunishContent = value;
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
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new RewardPunish Clone()
        {
            RewardPunish rewardFunish = base.Clone() as RewardPunish;

            rewardFunish.Employee = this.Employee.Clone();
            rewardFunish.Oper = this.Oper.Clone();

            return rewardFunish;
        }
        #endregion
    }
}
