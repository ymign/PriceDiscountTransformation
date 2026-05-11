using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InTestNetwork
    {
        private string totalregfee;
        /// <summary>
        /// 挂号总费用
        /// </summary>
        public string TOTALREGFEE
        {
            get
            {
                return totalregfee;
            }
            set
            {
                totalregfee = value;
            }
        }

        private string regfee;

        /// <summary>
        /// 挂号费
        /// </summary>
        public string REGFEE
        {
            get
            {
                return regfee;
            }
            set 
            {
                regfee = value;
            }
        }

        private string treatfee;
        /// <summary>
        /// 诊查费
        /// </summary>
        public string TREATFEE
        {
            get
            {
                return treatfee;
            }
            set
            {
                treatfee = value;
            }
        }

        private string servicesfee;
        /// <summary>
        /// 服务费
        /// </summary>
        public string SERVICESFEE
        {
            get
            {
                return servicesfee;
            }
            set
            {
                servicesfee = value;
            }
        }

        private string metafee;
        /// <summary>
        /// 材料费
        /// </summary>
        public string METAFEE
        {
            get
            {
                return metafee;
            }
            set
            {
                metafee = value;
            }
        }

        private string otherfee;
        /// <summary>
        /// 其它费用
        /// </summary>
        public string OTHERFEE
        {
            get
            {
                return otherfee;
            }
            set
            {
                otherfee = value;
            }
        }

        private string admitaddress;
        /// <summary>
        /// 候诊地点
        /// </summary>
        public string ADMITADDRESS
        {
            get
            {
                return admitaddress;
            }
            set
            {
                admitaddress = value;
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

        private string dept_name;
        /// <summary>
        /// 科室名称
        /// </summary>
        public string DEPT_NAME
        {
            get
            {
                return dept_name;
            }
            set
            {
                dept_name = value;
            }
        }

        private string doct_name;
        /// <summary>
        /// 医生名称
        /// </summary>
        public string DOCT_NAME
        {
            get
            {
                return doct_name;
            }
            set
            {
                doct_name = value;
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


        private string hospcode;
        /// <summary>
        /// 院区编码
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

        private string patientid;
        /// <summary>
        /// 门诊ID
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

        private string regdate;
        /// <summary>
        /// 挂号日期
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

        private string regsourceid;
        /// <summary>
        /// 排班编号
        /// </summary>
        public string REGSOURCEID
        {
            get
            {
                return regsourceid;
            }
            set
            {
                regsourceid = value;
            }
        }

    }
}
