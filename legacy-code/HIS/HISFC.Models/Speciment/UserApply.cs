using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 用户申请实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_USERAPPLICATION
    /// <修改记录 
    ///		修改人='lingk' 
    ///		修改时间='2011-07-18' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>

    public class UserApply : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int userAppyId = 0;
        private string userId = "";
        private int appId = 0;
        private string schedule = "";
        private DateTime curDate = new DateTime();

        private string operId = string.Empty;
        private string operName = string.Empty;
        private string scheduleId = string.Empty;
        #endregion

        #region 属性
        /// <summary>
        /// 用户申请表ID
        /// </summary>
        public int UserAppId
        {
            get
            {
                return userAppyId;
            }
            set
            {
                userAppyId = value;
            }
        }
        /// <summary>
        /// 申请人ID
        /// </summary>
        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }
        /// <summary>
        /// 申请表ID
        /// </summary>
        public int ApplyId
        {
            get
            {
                return appId;
            }
            set
            {
                appId = value;
            }
        }
        /// <summary>
        /// 当前进度情况
        /// </summary>
        public string Schedule
        {
            get
            {
                return this.schedule;
            }
            set
            {
                this.schedule = value;
            }
        }
        /// <summary>
        /// 进度时间
        /// </summary>
        public DateTime CurDate
        {
            get
            {
                return this.curDate;
            }
            set
            {
                this.curDate = value;
            }
        }
        /// <summary>
        /// 当前操作员ID
        /// </summary>
        public string OperId
        {
            get
            {
                return this.operId;
            }
            set
            {
                this.operId = value;
            }
        }
        /// <summary>
        /// 当前操作员姓名
        /// </summary>
        public string OperName
        {
            get
            {
                return this.operName;
            }
            set
            {
                this.operName = value;
            }
        }

        /// <summary>
        /// 进度ID
        /// </summary>
        public string ScheduleId
        {
            get
            {
                return this.scheduleId;
            }
            set
            {
                this.scheduleId = value;
            }
        }
        #endregion

        #region 方法
        public new UserApply Clone()
        {
            return base.Clone() as UserApply;
        }
        #endregion
    }
}
