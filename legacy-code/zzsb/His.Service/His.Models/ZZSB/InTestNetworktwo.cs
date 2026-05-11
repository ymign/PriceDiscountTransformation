using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InTestNetworktwo
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

        private string regsourcename;
        /// <summary>
        /// 医生级别+医生名称
        /// </summary>
        public string REGSOURCENAME
        {
            get
            {
                return regsourcename;
            }
            set
            {
                regsourcename = value;
            }
        }

        private string execlocation;
        /// <summary>
        /// 就诊位置
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
        /// 当前序号
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
        /// 前面人数
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

        private string time;
        /// <summary>
        /// 就诊日期
        /// </summary>
        public string TIME
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        
        private string note;
        /// <summary>
        /// 备用
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

        private string patientid;
        /// <summary>
        /// 住院流水号
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
    }
}
