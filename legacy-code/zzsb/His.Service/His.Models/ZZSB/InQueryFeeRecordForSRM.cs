using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InQueryFeeRecordForSRM
    {

        private string itemid;
        /// <summary>
        /// 费用ID
        /// </summary>
        public string ITEMID
        {
            get
            {
                return itemid;
            }
            set
            {
                itemid = value;
            }
        }

        private string busitype;
        /// <summary>
        /// 业务类型
        /// </summary>
        public string BUSITYPE
        {
            get
            {
                return busitype;
            }
            set
            {
                busitype = value;
            }
        }

        private string itemfee;
        /// <summary>
        /// 费用金额
        /// </summary>
        public string ITEMFEE
        {
            get
            {
                return itemfee;
            }
            set
            {
                itemfee = value;
            }
        }

        private string execdeptname;
        /// <summary>
        /// 执行科室
        /// </summary>
        public string EXECDEPTNAME
        {
            get
            {
                return execdeptname;
            }
            set
            {
                execdeptname = value;
            }
        }

        private string feedate;
        /// <summary>
        /// 费用发生时间
        /// </summary>
        public string FEEDATE
        {
            get
            {
                return feedate;
            }
            set
            {
                feedate = value;
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
