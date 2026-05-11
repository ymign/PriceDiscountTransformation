using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InQueryFetchMedicineQueueForSRM
    {
 
        private string deptname;
        /// <summary>
        /// 科室名称
        /// </summary>
        public string DEPTNAME
        {
            get
            {
                return deptname;
            }
            set
            {
                deptname = value;
            }
        }

        private string receiptnum;
        /// <summary>
        /// 处方张数
        /// </summary>
        public string RECEIPTNUM
        {
            get
            {
                return receiptnum;
            }
            set
            {
                receiptnum = value;
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

        private string waitnum;
        /// <summary>
        /// 等候人次
        /// </summary>
        public string WAITNUM
        {
            get
            {
                return waitnum;
            }
            set
            {
                waitnum = value;
            }
        }

        private string quedate;
        /// <summary>
        /// 队列时间
        /// </summary>
        public string QUEDATE
        {
            get
            {
                return quedate;
            }
            set
            {
                quedate = value;
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
