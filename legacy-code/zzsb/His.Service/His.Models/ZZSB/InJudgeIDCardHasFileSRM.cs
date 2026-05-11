using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InJudgeIDCardHasFileSRM
    {
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

        private string idcardno;
        /// <summary>
        /// 身份证号
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

        private string name;

        /// <summary>
        /// 姓名
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

        private string age;
        /// <summary>
        /// 年龄
        /// </summary>
        public string AGE
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        }


        private string bankcardno;
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BANKCARDNO
        {
            get
            {
                return bankcardno;
            }
            set
            {
                bankcardno = value;
            }
        }

        private string deviceid;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DEVICEID
        {
            get
            {
                return deviceid;
            }
            set
            {
                deviceid = value;
            }
        }

        private string servicecode;
        /// <summary>
        /// 服务编号
        /// </summary>
        public string SERVICECODE
        {
            get
            {
                return servicecode;
            }
            set
            {
                servicecode = value;
            }
        }


    }
}
