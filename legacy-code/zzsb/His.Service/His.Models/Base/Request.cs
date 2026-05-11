using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Base
{
  public   class Request
    {
        private string reqTraceNo;
        /// <summary>
        /// 业务交易流水号
        /// </summary>
        public string ReqTraceNo
        {
            get
            {
                return reqTraceNo;
            }
            set
            {
                reqTraceNo = value;
            }
        }

        private string cardtypeCode;
        public string CardTypeCode
        {
            get
            {
                return cardtypeCode;
            }
            set
            {
                cardtypeCode = value;
            }
        }
       
     
        private string apptypeCode;
        public string AppTypeCode
        {
            get
            {
                return apptypeCode;
            }
            set
            {
                apptypeCode = value;
            }
        }

        private string reqTime;
        public string ReqTime
        {
            get
            {
                return reqTime;
            }
            set
            {
                reqTime = value;
            }
        }


        private string appCode;
        public string AppCode
        {
            get
            {
                return appCode;
            }
            set
            {
                appCode = value;
            }
        }

        /// <summary>
        /// 操作员编号
        /// </summary>
        private string userID;  
        /// <summary>
        /// 操作员编号
        /// </summary>
        public string UserID
        {
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        private string pwd;
        /// <summary>
        /// 操作员密码
        /// </summary>
        public string PassWord
        {
            get
            {
                return pwd;
            }
            set
            {
                pwd = value;
            }
        }

        /// <summary>
        /// 设备编号
        /// </summary>
        private string deviceID;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceID
        {
            get
            {
                return this.deviceID;
            }
            set
            {
                this.deviceID = value;
            }
        }

        /// <summary>
        /// 服务编码
        /// </summary>
        private string serviceCode;
        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode
        {
            get
            {
                return this.serviceCode;
            }
            set
            {
                this.serviceCode = value;
            }
        }

        /// <summary>
        /// 业务编号
        /// </summary>
        private string funCode;
        /// <summary>
        /// 业务编号
        /// </summary>
        public string FunCode
        {
            get
            {
                return this.funCode;
            }
            set
            {
                this.funCode = value;
            }
        }

        private string hosCode;
        public string HospCode
        {
            get
            {
                return hosCode;
            }
            set
            {
                hosCode = value;
            }
        }
        private string bankNo;
        public string BankCode
        {
            get
            {
                return bankNo;
            }
            set
            {
                bankNo = value;
            }
        }
    }
}
