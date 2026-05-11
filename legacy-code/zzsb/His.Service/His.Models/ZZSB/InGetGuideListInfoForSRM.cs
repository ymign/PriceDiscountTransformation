using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InGetGuideListInfoForSRM
    {
 
        private string transerno;
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TRANSERNO
        {
            get
            {
                return transerno;
            }
            set
            {
                transerno = value;
            }
        }

        private string invoiceno;
        /// <summary>
        /// 发票号
        /// </summary>
        public string INVOICENO
        {
            get
            {
                return invoiceno;
            }
            set
            {
                invoiceno = value;
            }
        }

        private string execadress;
        /// <summary>
        /// 执行地点
        /// </summary>
        public string EXECADRESS
        {
            get
            {
                return execadress;
            }
            set
            {
                execadress = value;
            }
        }

        private string message;
        /// <summary>
        /// 发票号
        /// </summary>
        public string MESSAGE
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
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


        private string regid;
        /// <summary>
        /// 就诊记录编码
        /// </summary>
        public string REGID
        {
            get
            {
                return regid;
            }
            set
            {
                regid = value;
            }
        }


        private string recipeno;
        /// <summary>
        /// 处方号
        /// </summary>
        public string RECIPENO
        {
            get
            {
                return recipeno;
            }
            set
            {
                recipeno = value;
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

        private string hospcode;
        /// <summary>
        /// 院区编号
        /// </summary>
        public string HOSPCODE
        {
            get
            {
                return hospcode;
            }
            set
            {
                hospcode = value;
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

    }
}
