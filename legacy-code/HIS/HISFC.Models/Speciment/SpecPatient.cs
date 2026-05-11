using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 病人的基本信息]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-22]<br></br>
    /// Table : SPEC_NOHISPATIENT
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecPatient : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int patientID;
        private string patientName = "";
        private DateTime birthday = DateTime.Now;
        private string icCardNo = "";
        private string cardNo = "";
        private string contactNum = "";
        private string homePhoneNum = "";
        private string nationality = "";
        private string nation = "";
        private string idCardNO = "";
        private char gender = ' ';
        private string home = "";
        private string address = "";
        private string bloodType = "";
        private string isMar = "";
        private string inHosNum = "";
        private string comment = "";
        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public int PatientID
        {
            get
            {
                return patientID;
            }
            set
            {
                patientID = value;
            }
        }

        /// <summary>
        /// 病人的名称
        /// </summary>
        public string PatientName
        {
            get
            {
                return patientName;
            }
            set
            {
                patientName = value;
            }
        }

        /// <summary>
        /// 病人卡上的号
        /// </summary>
        public string IcCardNo
        {
            get
            {
                return icCardNo;
            }
            set
            {
                icCardNo = value;
            }
        }

        /// <summary>
        /// 病历号
        /// </summary>
        public string CardNo
        {
            get
            {
                return cardNo;
            }
            set
            {
                cardNo = value;
            }
        }

        /// <summary>
        /// 病人身份证号
        /// </summary>
        public string IdCardNo
        {
            get
            {
                return idCardNO;
            }
            set
            {
                idCardNO = value;
            }
        }

        /// <summary>
        /// 病人性别
        /// </summary>
        public char Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string Home
        {
            get
            {
                return home;
            }
            set
            {
                home = value;
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
        /// 联系电话
        /// </summary>
        public string ContactNum
        {
            get
            {
                return contactNum;
            }
            set
            {
                contactNum = value;
            }
        }

        /// <summary>
        /// 家庭电话
        /// </summary>
        public string HomePhoneNum
        {
            get
            {
                return homePhoneNum;
            }
            set
            {
                homePhoneNum = value;
            }
        }

        /// <summary>
        /// 国籍
        /// </summary>
        public string Nationality
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
        /// 民族
        /// </summary>
        public string Nation
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
        /// 血型
        /// </summary>
        public string BloodType
        {
            get
            {
                return bloodType;
            }
            set
            {
                bloodType = value;
            }
        }

        public string IsMarried
        {
            get
            {
                return isMar;
            }
            set
            {
                isMar = value;
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }

        public DateTime Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                birthday = value;
            }
        }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InHosNum
        {
            get
            {
                return inHosNum;
            }
            set
            {
                inHosNum = value;
            }
        }
        #endregion

        #region 方法
        public new SpecPatient Clone()
        {
            return base.Clone() as SpecPatient;
        }
        #endregion
    }
}
