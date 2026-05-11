using System.Collections;
using System;
namespace Neusoft.HISFC.Models.Base
{
    /// <summary>
    /// Department<br></br>
    /// [功能描述: 科室实体]<br></br>
    /// [创 建 者: 王铁全]<br></br>
    /// [创建时间: 2006-08-28]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class Department : Spell, ISort, IValidState
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Department()
        {
        }


        #region 变量



        /// <summary>
        /// 科室类型
        /// </summary>
        private Base.DepartmentTypeEnumService deptType;

        /// <summary>
        /// 英文名称
        /// </summary>
        private string englishName;

        /// <summary>
        /// 特殊科室标记
        /// </summary>
        private string specialFlag;

        /// <summary>
        /// 排序号    
        /// </summary>
        private int sortID;

        /// <summary>
        /// 有效性状态
        /// </summary>
        private Base.EnumValidState validState;

        /// <summary>
        /// 是否统计科室
        /// </summary>
        private bool isStatDept;

        /// <summary>
        /// 是否挂号科室
        /// </summary>
        private bool isRegDept;

        /// <summary>
        /// 科室简称
        /// </summary>
        private string shortName;

        /// <summary>
        /// 医院编码
        /// </summary>
        private string hosCode;

        /// <summary>
        /// herp系统科室编码
        /// </summary>
        private string herpCode;

        /// <summary>
        /// 操作人
        /// </summary>
        private string operCode;
        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operDate;
       
        #endregion

        #region 属性

        /// <summary>
        /// 科室简称
        /// </summary>
        public string ShortName
        {
            get
            {
                return shortName;
            }
            set
            {
                shortName = value;
            }
        }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName
        {
            get
            {
                return this.englishName;
            }
            set
            {
                this.englishName = value;
            }
        }

        /// <summary>
        /// 科室类型
        /// </summary>
        public DepartmentTypeEnumService DeptType
        {
            get
            {
                if (deptType == null)
                {
                    deptType = new DepartmentTypeEnumService();
                }
                return deptType;
            }
            set
            {
                deptType = value;
            }
        }

        /// <summary>
        /// 是否挂号科室
        /// </summary>
        public bool IsRegDept
        {
            get
            {
                return isRegDept;
            }
            set
            {
                isRegDept = value;
            }
        }

        /// <summary>
        /// 是否统计科室
        /// </summary>
        public bool IsStatDept
        {
            get
            {
                return isStatDept;
            }
            set
            {
                isStatDept = value;
            }
        }

        /// <summary>
        /// 特殊科室标识
        /// </summary>
        public string SpecialFlag
        {
            get
            {
                return this.specialFlag;
            }
            set
            {
                this.specialFlag = value;
            }
        }

        [System.Obsolete("已更改为SpecialFlag", true)]
        public string DeptPro
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// 有效性状态
        /// </summary>
        public EnumValidState ValidState
        {
            get
            {
                return this.validState;
            }
            set
            {
                this.validState = value;
            }
        }
        /// <summary>
        /// 医院编码
        /// </summary>
        public string HosCode
        {
            get { return hosCode; }
            set { hosCode = value; }
        }

        /// <summary>
        /// herp科室编码
        /// </summary>
        public string HerpDeptCode
        {
            get { return this.herpCode; }
            set { this.herpCode = value; }
        }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperCode
        {
            get { return operCode; }
            set { operCode = value; }
        }


        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate
        {
            get { return operDate; }
            set { operDate = value; }
        }
        #endregion

        #region 方法

        #region 克降

        /// <summary>
        /// 克隆函数
        /// </summary>
        /// <returns>返回当前实例的副本</returns>
        public new Department Clone()
        {
            Department department = base.Clone() as Department;

            return department;
        }

        #endregion

        #endregion

        #region ISort 成员

        /// <summary>
        /// 排序ID
        /// </summary>
        public int SortID
        {
            get
            {
                return this.sortID;
            }
            set
            {
                this.sortID = value;
            }
        }

        #endregion
    }
}
