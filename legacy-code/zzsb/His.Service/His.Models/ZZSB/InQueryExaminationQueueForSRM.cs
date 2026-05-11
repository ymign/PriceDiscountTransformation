using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InQueryExaminationQueueForSRM
    {
 
        private string checkid;
        /// <summary>
        /// 检查编号
        /// </summary>
        public string CHECKID
        {
            get
            {
                return checkid;
            }
            set
            {
                checkid = value;
            }
        }

        private string checkname;
        /// <summary>
        /// 检查名称
        /// </summary>
        public string CHECKNAME
        {
            get
            {
                return checkname;
            }
            set
            {
                checkname = value;
            }
        }

        private string execlocation;
        /// <summary>
        /// 执行位置
        /// </summary>
        public string EXECLOCATION
        {
            get
            {
                return execlocation;
            }
            set
            {
                execlocation = value;
            }
        }

        private string waitno;
        /// <summary>
        /// 取药序号
        /// </summary>
        public string WAITNO
        {
            get
            {
                return waitno;
            }
            set
            {
                waitno = value;
            }
        }

        private string currentno;
        /// <summary>
        /// 当前序号
        /// </summary>
        public string CURRENTNO
        {
            get
            {
                return currentno;
            }
            set
            {
                currentno = value;
            }
        }

        private string note;
        /// <summary>
        /// 备用字段
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
