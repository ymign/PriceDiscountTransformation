using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// Neusoft.HISFC.Models.HR.Pluralism<br></br>
    /// [功能描述: 社会兼职实体]<br></br>
    /// [创 建 者: 赵阳]<br></br>
    /// [创建时间: 2008-07-10]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class Pluralism : Neusoft.FrameWork.Models.NeuObject, Neusoft.HISFC.Models.Base.IValidState
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
        /// 任职时间
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// 终止时间
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// 任职性质
        /// </summary>
        private string postKind = "";

        /// <summary>
        /// 学会名称
        /// </summary>
        private string organizationName = "";

        /// <summary>
        /// 职务
        /// </summary>
        private string postCode = "";

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        /// <summary>
        /// 有效性
        /// </summary>
        private Neusoft.HISFC.Models.Base.EnumValidState validState = Neusoft.HISFC.Models.Base.EnumValidState.Valid;

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
        /// 任职时间
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        /// <summary>
        /// 终止时间
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                endTime = value;
            }
        }

        /// <summary>
        /// 任职性质
        /// </summary>
        public string PostKind
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
        /// 学会名称
        /// </summary>
        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                organizationName = value;
            }
        }

        /// <summary>
        /// 职务
        /// </summary>
        public string PostCode
        {
            get
            {
                return postCode;
            }
            set
            {
                postCode = value;
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
        /// 有效性
        /// </summary>
        public Neusoft.HISFC.Models.Base.EnumValidState ValidState
        {
            get
            {
                return validState;
            }
            set
            {
                validState = value;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Pluralism Clone()
        {
            Pluralism pluralism = base.Clone() as Pluralism;

            pluralism.Employee = this.Employee.Clone();
            pluralism.Oper = this.Oper.Clone();

            return pluralism;
        }
        #endregion
    }
}
