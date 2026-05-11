using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.Base
{
    /// <summary>
    /// 用户默认设置信息类
    /// </summary>
    [System.Serializable]
    public class UserDefaultSetting : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量

        private Employee empl = new Employee();

        /// <summary>
        /// 医生站查找项目的过滤条件：拼音码、五笔码
        /// </summary>
        private string setting1 = "";

        /// <summary>
        /// 医生站过滤项目是否选中：科常用项目
        /// </summary>
        private string setting2 = "";

        /// <summary>
        /// 医生站过滤项目是否选中：精确查找
        /// </summary>
        private string setting3 = "";

        /// <summary>
        /// 是否启用合理用药审查【门诊】
        /// </summary>
        private string setting4 = "";

        /// <summary>
        /// 是否启用合理用药审查【住院】
        /// </summary>
        private string setting5 = "";

        private string setting6 = "";

        private string setting7 = "";

        private string setting8 = "";

        private string setting9 = "";

        private string setting10 = "";

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 人员信息
        /// </summary>
        public Employee Empl
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
        /// 医生站查找项目的过滤条件：拼音码、五笔码
        /// </summary>
        public string Setting1
        {
            get
            {
                return setting1;
            }
            set
            {
                setting1 = value;
            }
        }

        /// <summary>
        /// 医生站过滤项目是否选中：科常用项目
        /// </summary>
        public string Setting2
        {
            get
            {
                return setting2;
            }
            set
            {
                setting2 = value;
            }
        }

        /// <summary>
        /// 医生站过滤项目是否选中：精确查找
        /// </summary>
        public string Setting3
        {
            get
            {
                return setting3;
            }
            set
            {
                setting3 = value;
            }
        }

        /// <summary>
        /// 是否启用合理用药审查【门诊】
        /// </summary>
        public string Setting4
        {
            get
            {
                return setting4;
            }
            set
            {
                setting4 = value;
            }
        }

        /// <summary>
        /// 是否启用合理用药审查【住院】
        /// </summary>
        public string Setting5
        {
            get
            {
                return setting5;
            }
            set
            {
                setting5 = value;
            }
        }

        public string Setting6
        {
            get
            {
                return setting6;
            }
            set
            {
                setting6 = value;
            }
        }

        public string Setting7
        {
            get
            {
                return setting7;
            }
            set
            {
                setting7 = value;
            }
        }

        public string Setting8
        {
            get
            {
                return setting8;
            }
            set
            {
                setting8 = value;
            }
        }

        public string Setting9
        {
            get
            {
                return setting9;
            }
            set
            {
                setting9 = value;
            }
        }

        public string Setting10
        {
            get
            {
                return setting10;
            }
            set
            {
                setting10 = value;
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

        #region 克隆

        public new UserDefaultSetting Clone()
        {
            UserDefaultSetting userDefaultSetting = base.Clone() as UserDefaultSetting;

            userDefaultSetting.Empl = Empl.Clone();
            userDefaultSetting.Oper = Oper.Clone();

            return userDefaultSetting;
        }

        #endregion
    }
}
