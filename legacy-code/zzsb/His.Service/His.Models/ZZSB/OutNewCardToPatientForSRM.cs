using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class OutNewCardToPatientForSRM
    {
        private string cardno;
        /// <summary>
        /// 物理卡号
        /// </summary>
        public string CARDNO
        {
            get
            {
                return cardno;
            }
            set
            {
                cardno = value;
            }
        }

        private string name;
        /// <summary>
        /// 名字
        /// </summary>
        public string NAME
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private string sex;
        /// <summary>
        /// 性别
        /// </summary>
        public string SEX
        {
            get
            {
                return sex;
            }
            set
            {
                sex = value;
            }
        }

        private string birthday;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BIRTHDAY
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

        private string idcardno;
        /// <summary>
        /// 办证证号
        /// </summary>
        public string IDCARDNO
        {
            get
            {
                return idcardno;
            }
            set
            {
                idcardno = value;
            }
        }

        private string idcardtype;
        /// <summary>
        /// 证件类型
        /// </summary>
        public string IDCARDTYPE
        {
            get
            {
                return idcardtype;
            }
            set
            {
                idcardtype = value;
            }
        }

        private string nationality;
        /// <summary>
        /// 国籍
        /// </summary>
        public string NATIONALITY
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

        private string nation;
        /// <summary>
        /// 民族
        /// </summary>
        public string NATION
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

        private string address;
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDRESS
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

        private string phoneno;
        /// <summary>
        /// 电话
        /// </summary>
        public string PHONENO
        {
            get
            {
                return phoneno;
            }
            set
            {
                phoneno = value;
            }
        }
    
    }
}
