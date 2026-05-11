using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InRegisterDept
    {
        private string deptcode;
        /// <summary>
        /// 科室编号
        /// </summary>
        public string DEPTCODE
        {
            get
            {
                return deptcode;
            }
            set
            {
                deptcode = value;
            }
        }

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

        private string deptfloor;

        /// <summary>
        /// 科室楼层
        /// </summary>
        public string DEPTFLOOR
        {
            get
            {
                return deptfloor;
            }
            set
            {
                deptfloor = value;
            }
        }

        private string description;
        /// <summary>
        /// 科室简介
        /// </summary>
        public string DESCRIPTION
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        private string message;
        /// <summary>
        /// 科室提示信息
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

        private string nextflag;
        /// <summary>
        /// 是否有下级科室
        /// </summary>
        public string NEXTFLAG
        {
            get
            {
                return nextflag;
            }
            set
            {
                nextflag = value;
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

        private string regdate;
        /// <summary>
        /// 看诊日期
        /// </summary>
        public string REGDATE
        {
            get
            {
                return regdate;
            }
            set
            {
                regdate = value;
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

        private string patientid;
        /// <summary>
        /// 患者ID
        /// </summary>
        public string PATIENTID
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
        private string elderlyvoucherregdeptflag;
        /// <summary>
        /// 长者券科室标识
        /// </summary>
        public string ELDERLYVOUCHERREGDEPTFLAG
        {
            get
            {
                return elderlyvoucherregdeptflag;
            }
            set
            {
                elderlyvoucherregdeptflag = value;
            }
        }

    }
}
