using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class Patientopr
    {
        private string patientid;
        /// <summary>
        /// 患者ID号
        /// </summary>
        public string PATIENTID
        {
            get
            {
                return patientid;
            }
            set
            {
                patientid = value;
            }
        }

        private string empi;
        /// <summary>
        /// 患者主索引号
        /// </summary>
        public string EMPI
        {
            get
            {
                return empi;
            }
            set
            {
                empi = value;
            }
        }

        private string outpatientno;

        /// <summary>
        /// 门诊号
        /// </summary>
        public string OUTPATIENTNO
        {
            get
            {
                return outpatientno;
            }
            set
            {
                outpatientno = value;
            }
        }

        private string cardstatus;
        /// <summary>
        /// 卡状态
        /// </summary>
        public string CARDSTATUS
        {
            get
            {
                return cardstatus;
            }
            set
            {
                cardstatus = value;
            }
        }

        private string accountid;
        /// <summary>
        /// 预交金帐户ID
        /// </summary>
        public string ACCOUNTID
        {
            get
            {
                return accountid;
            }
            set
            {
                accountid = value;
            }
        }

        private string accbalance;
        /// <summary>
        /// 预交金帐户余额
        /// </summary>
        public string ACCBALANCE
        {
            get
            {
                return accbalance;
            }
            set
            {
                accbalance = value;
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
        /// 办卡证件号码
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

        private string feetype;
        /// <summary>
        /// 患者费别
        /// </summary>
        public string FEETYPE
        {
            get
            {
                return feetype;
            }
            set
            {
                feetype = value;
            }
        }

        private string phoneno;
        /// <summary>
        /// 电话号码
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

        private string note;
        /// <summary>
        /// 备注
        /// </summary>
        public string NOTE
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
            }
        }

        private string cardtypecode;
        /// <summary>
        /// 卡类型
        /// </summary>
        public string CARDTYPECODE
        {
            get
            {
                return cardtypecode;
            }
            set
            {
                cardtypecode = value;
            }
        }

        private string cardno;
        /// <summary>
        /// 卡号
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

        //public string IsShield { g}

        private string isShield;
        /// <summary>
        /// 是否屏蔽99号段
        /// </summary>
        public string IsShield
        {
            get
            {
                return isShield;
            }
            set
            {
                isShield = value;
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

        private string funcode;
        /// <summary>
        /// 业务编号
        /// </summary>
        public string FUNCODE
        {
            get
            {
                return funcode;
            }
            set
            {
                funcode = value;
            }
        }

        private string reqtime;
        /// <summary>
        /// 请求时间
        /// </summary>
        public string REQTIME
        {
            get
            {
                return reqtime;
            }
            set
            {
                reqtime = value;
            }
        }

        private string reqtraceno;
        /// <summary>
        /// 请求流水号
        /// </summary>
        public string REQTRACENO
        {
            get
            {
                return reqtraceno;
            }
            set
            {
                reqtraceno = value;
            }
        }
        private string elderlyvoucherflag;
        /// <summary>
        /// 长者券标识
        /// </summary>
        public string ELDERLYVOUCHERFLAG
        {
            get
            {
                return elderlyvoucherflag;
            }
            set
            {
                elderlyvoucherflag = value;
            }
        }

    }
}
