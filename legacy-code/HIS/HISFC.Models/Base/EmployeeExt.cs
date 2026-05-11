using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.RADT;

namespace Neusoft.HISFC.Models.Base
{
    /// <summary>
    /// EmployeeExt <br></br>
    /// [功能描述: 人员实体扩展]<br></br>
    /// [创 建 者: 孙久海]<br></br>
    /// [创建时间: 2008-06-20]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class EmployeeExt : Neusoft.FrameWork.Models.NeuObject
    {
        /// <summary>
		/// 构造函数

		/// </summary>
		public EmployeeExt()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        #region 字段

        /// <summary>
        /// 民族
        /// </summary>
        private NeuObject nation = new NeuObject();

        /// <summary>
        /// 政治面貌
        /// </summary>
        private NeuObject polityVisage = new NeuObject();

        /// <summary>
        /// 健康状况
        /// </summary>
        private NeuObject health = new NeuObject();

        /// <summary>
        /// 婚姻状况
        /// </summary>
        private NeuObject married = new NeuObject();

        /// <summary>
        /// 职工工作类型
        /// </summary>
        private  WorkTypeEnumService workType = new WorkTypeEnumService();

        /// <summary>
        /// 职工状态

        /// </summary>
        private NeuObject emplStatus = new NeuObject();

        /// <summary>
        /// 参工时间
        /// </summary>
        private DateTime startWorkDate;

        /// <summary>
        /// 离退时间
        /// </summary>
        private DateTime quitDate;
     
        /// <summary>
        /// 家庭住址
        /// </summary>
        private string address;

        /// <summary>
        /// 籍贯
        /// </summary>
        private string district;

        /// <summary>
        /// 国籍
        /// </summary>
        private NeuObject nationality = new NeuObject();

        /// <summary>
        /// 联系电话
        /// </summary>
        private string tell;

        /// <summary>
        /// 医生章号
        /// </summary>
        private string badge;

        /// <summary>
        /// 毕业时间
        /// </summary>
        private DateTime graduateDate;

        /// <summary>
        /// 毕业学校
        /// </summary>
        private string graduateSchool;

        /// <summary>
        /// 专业
        /// </summary>
        private string speciality;

        /// <summary>
        /// 学校类型
        /// </summary>
        private NeuObject schoolType = new NeuObject();

        /// <summary>
        /// 第一外语
        /// </summary>
        private NeuObject firstLang = new NeuObject();

        /// <summary>
        /// 第一外语级别
        /// </summary>
        private NeuObject firstLangLevel = new NeuObject();

        /// <summary>
        /// 第二外语
        /// </summary>
        private NeuObject secondLang = new NeuObject();

        /// <summary>
        /// 第二外语级别
        /// </summary>
        private NeuObject secondLangLevel = new NeuObject();

        /// <summary>
        /// 合同签订时间
        /// </summary>
        private DateTime startBragDate;

        /// <summary>
        /// 合同到期时间
        /// </summary>
        private DateTime endBragDate;

        /// <summary>
        /// 本院工作时间
        /// </summary>
        private DateTime localWorkDate;

        /// <summary>
        /// 合同类型
        /// </summary>
        private BargainTypeEnumService bargainType = new BargainTypeEnumService();

        /// <summary>
        /// 照片
        /// </summary>
        private byte[] picture;

        /// <summary>
        /// 备注
        /// </summary>
        private string remark;

        /// <summary>
        /// 操作员代码
        /// </summary>
        private string operCode;

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operDate;

        /// <summary>
        /// 发生序号
        /// </summary>
        private int happenNo;

        /// <summary>
        /// 入学时间
        /// </summary>
        private DateTime inSchoolTime;

        /// <summary>
        /// 职称晋升专业
        /// </summary>
        private string titleSpecial;

        /// <summary>
        /// 来院来源
        /// </summary>
        private string toHosSource;

        /// <summary>
        /// 来源类型
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject sourceType = new NeuObject();

        /// <summary>
        /// 岗位
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject post = new NeuObject();

        /// <summary>
        /// 入党时间
        /// </summary>
        private DateTime partyTime = new DateTime();

        /// <summary>
        /// 职务级别
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject postLevel = new NeuObject();

       
        /// <summary>
        /// 职称级别
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject titleLevel = new NeuObject();

        /// <summary>
        /// 调置本院时间
        /// </summary>
        private DateTime reverseTime = new DateTime();

       

        #endregion
        /// <summary>
        /// 调置本院时间
        /// </summary>
        public DateTime ReverseTime
        {
            get 
            { 
                return reverseTime; 
            }
            set 
            { 
                reverseTime = value; 
            }
        }

        #region 属性

        /// <summary>
        /// 职称级别
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject TitleLevel
        {
            get 
            { 
                return titleLevel; 
            }
            set 
            { 
                titleLevel = value; 
            }
        }

        /// <summary>
        /// 职务级别
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject PostLevel
        {
            get 
            { 
                return postLevel; 
            }
            set 
            {
                postLevel = value; 
            }
        }


        /// <summary>
        /// 入党时间
        /// </summary>
        public DateTime PartyTime
        {
            get 
            { 
                return partyTime; 
            }
            set 
            { 
                partyTime = value; 
            }
        }
        /// <summary>
        /// 岗位
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Post
        {
            get 
            { 
                return post; 
            }
            set 
            { 
                post = value; 
            }
        }
        /// <summary>
        /// 来源类型
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject SourceType
        {
            get 
            { 
                return sourceType; 
            }
            set 
            { 
                sourceType = value; 
            }
        }

        /// <summary>
        /// 来院来源
        /// </summary>
        public string ToHosSource
        {
            get 
            { 
                return toHosSource; 
            }
            set 
            { 
                toHosSource = value; 
            }
        }

        /// <summary>
        /// 职称晋升专业
        /// </summary>
        public string TitleSpecial
        {
            get 
            { 
                return titleSpecial; 
            }
            set 
            { 
                titleSpecial = value; 
            }
        }


        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime InSchoolTime
        {
            get 
            { 
                return inSchoolTime; 
            }
            set 
            { 
                inSchoolTime = value; 
            }
        }
        /// <summary>
        /// 民族
        /// </summary>
        public NeuObject Nation
        {
            get 
            { 
                return nation; 
            }
            set 
            { 
                nation = value; 
            }
        }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public NeuObject PolityVisage
        {
            get 
            {
                return polityVisage;
            }
            set 
            { 
                polityVisage = value;
            }
        }

        /// <summary>
        /// 健康状况
        /// </summary>
        public NeuObject Health
        {
            get 
            { 
                return health; 
            }
            set 
            { 
                health = value;
            }
        }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public NeuObject Married
        {
            get 
            {
                return married; 
            }
            set 
            {
                married = value; 
            }
        }

        /// <summary>
        /// 职工工作类型
        /// </summary>
        public WorkTypeEnumService WorkType
        {
            get 
            { 
                return workType; 
            }
            set 
            {
                workType = value; 
            }
        }

        /// <summary>
        /// 人员状态

        /// </summary>
        public NeuObject EmplStatus
        {
            get 
            { 
                return emplStatus; 
            }
            set 
            { 
                emplStatus = value; 
            }
        }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string Address
        {
            get 
            { 
                return address; 
            }
            set 
            {
                address = value; 
            }
        }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string District
        {
            get 
            { 
                return district;
            }
            set 
            { 
                district = value; 
            }
        }

        /// <summary>
        /// 国籍
        /// </summary>
        public NeuObject Nationality
        {
            get 
            { 
                return nationality; 
            }
            set 
            { 
                nationality = value; 
            }
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tell
        {
            get 
            { 
                return tell; 
            }
            set 
            { 
                tell = value;
            }
        }

        /// <summary>
        /// 医生章号
        /// </summary>
        public string Badge
        {
            get
            { 
                return badge; 
            }
            set
            { 
                badge = value; 
            }
        }

        /// <summary>
        /// 离退时间
        /// </summary>
        public DateTime QuitDate
        {
            get
            { 
                return quitDate; 
            }
            set 
            { 
                quitDate = value; 
            }
        }

        /// <summary>
        /// 开始工作时间
        /// </summary>
        public DateTime StartWorkDate
        {
            get 
            { 
                return startWorkDate; 
            }
            set 
            { 
                startWorkDate = value;
            }
        }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime GraduateDate
        {
            get 
            { 
                return graduateDate; 
            }
            set 
            { 
                graduateDate = value; 
            }
        }

        /// <summary>
        /// 毕业学校
        /// </summary>
        public string GraduateSchool
        {
            get 
            { 
                return graduateSchool; 
            }
            set 
            { 
                graduateSchool = value; 
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
        /// 学校类型
        /// </summary>
        public NeuObject SchoolType
        {
            get 
            { 
                return schoolType; 
            }
            set
            { 
                schoolType = value; 
            }
        }

        /// <summary>
        /// 第一外语
        /// </summary>
        public NeuObject FirstLang
        {
            get 
            { 
                return firstLang; 
            }
            set 
            { 
                firstLang = value; 
            }
        }

        /// <summary>
        /// 第一外语级别
        /// </summary>
        public NeuObject FirstLangLevel
        {
            get 
            { 
                return firstLangLevel; 
            }
            set 
            { 
                firstLangLevel = value; 
            }
        }

        /// <summary>
        /// 第二外语
        /// </summary>
        public NeuObject SecondLang
        {
            get 
            { 
                return secondLang; 
            }
            set 
            { 
                secondLang = value;
            }
        }

        /// <summary>
        /// 第二外语级别
        /// </summary>
        public NeuObject SecondLangLevel
        {
            get 
            { 
                return secondLangLevel; 
            }
            set 
            { 
                secondLangLevel = value;
            }
        }

        /// <summary>
        /// 合同签订时间
        /// </summary>
        public DateTime StartBragDate
        {
            get 
            {
                return startBragDate; 
            }
            set 
            {
                startBragDate = value; 
            }
        }

        /// <summary>
        /// 合同到期时间
        /// </summary>
        public DateTime EndBragDate
        {
            get 
            {
                return endBragDate; 
            }
            set 
            {
                endBragDate = value; 
            }
        }

        /// <summary>
        /// 本院工作时间
        /// </summary>
        public DateTime LocalWorkDate
        {
            get 
            { 
                return localWorkDate; 
            }
            set 
            { 
                localWorkDate = value; 
            }
        }

        /// <summary>
        /// 合同类型
        /// </summary>
        public BargainTypeEnumService BargainType
        {
            get 
            { 
                return bargainType; 
            }
            set 
            {
                bargainType = value;
            }
        }

        /// <summary>
        /// 照片
        /// </summary>
        public byte[] Picture
        {
            get 
            { 
                return picture; 
            }
            set 
            { 
                picture = value; 
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
        /// 操作员代码
        /// </summary>
        public string OperCode
        {
            get
            { 
                return operCode; 
            }
            set 
            { 
                operCode = value; 
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate
        {
            get 
            { 
                return operDate; 
            }
            set 
            { 
                operDate = value; 
            }
        }

        /// <summary>
        /// 发生序号
        /// </summary>
        public int HappenNo
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

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new EmployeeExt Clone()
        {
            EmployeeExt employeeext = base.Clone() as EmployeeExt;

            employeeext.Nation = this.Nation.Clone();
            employeeext.PolityVisage = this.PolityVisage.Clone();
            employeeext.Health = this.Health.Clone();
            employeeext.Married = this.Married.Clone();
            employeeext.WorkType = this.WorkType.Clone();
            employeeext.EmplStatus = this.EmplStatus.Clone();
            employeeext.Nationality = this.Nationality.Clone();
            employeeext.SchoolType = this.SchoolType.Clone();
            employeeext.FirstLang = this.FirstLang.Clone();
            employeeext.FirstLangLevel = this.FirstLangLevel.Clone();
            employeeext.SecondLang = this.SecondLang.Clone();
            employeeext.SecondLangLevel = this.SecondLangLevel.Clone();
            employeeext.BargainType = this.BargainType.Clone();
            //employeeext.Picture = this.Picture.Clone();

            return employeeext;
        }

        #endregion

    }
}
