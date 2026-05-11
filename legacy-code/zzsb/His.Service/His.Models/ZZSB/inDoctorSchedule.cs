using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InDoctorSchedule
    {
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

        private string regsourcename;

        /// <summary>
        /// 排班名称
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

        private string schematype;
        /// <summary>
        /// 排班类型
        /// </summary>
        public string SCHEMATYPE
        {
            get
            {
                return schematype;
            }
            set
            {
                schematype = value;
            }
        }

        private string typecode;
        /// <summary>
        /// 号类编号
        /// </summary>
        public string TYPECODE
        {
            get
            {
                return typecode;
            }
            set
            {
                typecode = value;
            }
        }

        private string typename;
        /// <summary>
        /// 号类描述
        /// </summary>
        public string TYPENAME
        {
            get
            {
                return typename;
            }
            set
            {
                typename = value;
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

        private string doctorcode;
        /// <summary>
        /// 医生编号
        /// </summary>
        public string DOCTORCODE
        {
            get
            {
                return doctorcode;
            }
            set
            {
                doctorcode = value;
            }
        }

        private string doctorname;
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string DOCTORNAME
        {
            get
            {
                return doctorname;
            }
            set
            {
                doctorname = value;
            }
        }

        private string specify;
        /// <summary>
        /// 医生专长
        /// </summary>
        public string SPECIFY
        {
            get
            {
                return specify;
            }
            set
            {
                specify = value;
            }
        }

        private string rankid;
        /// <summary>
        /// 医生级别编号
        /// </summary>
        public string RANKID
        {
            get
            {
                return rankid;
            }
            set
            {
                rankid = value;
            }
        }

        private string rankname;
        /// <summary>
        /// 医生级别名称
        /// </summary>
        public string RANKNAME
        {
            get
            {
                return rankname;
            }
            set
            {
                rankname = value;
            }
        }

        private string starttime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public string STARTTIME
        {
            get
            {
                return starttime;
            }
            set
            {
                starttime = value;
            }
        }

        private string endtime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string ENDTIME
        {
            get
            {
                return endtime;
            }
            set
            {
                endtime = value;
            }
        }

        private string sessioncode;
        /// <summary>
        /// 出诊时段编号
        /// </summary>
        public string SESSIONCODE
        {
            get
            {
                return sessioncode;
            }
            set
            {
                sessioncode = value;
            }
        }

        private string sessionname;
        /// <summary>
        /// 出诊时段名称
        /// </summary>
        public string SESSIONNAME
        {
            get
            {
                return sessionname;
            }
            set
            {
                sessionname = value;
            }
        }

        private string allcount;
        /// <summary>
        /// 全部号源数
        /// </summary>
        public string ALLCOUNT
        {
            get
            {
                return allcount;
            }
            set
            {
                allcount = value;
            }
        }

        private string outcount;
        /// <summary>
        /// 已挂号数
        /// </summary>
        public string OUTCOUNT
        {
            get
            {
                return outcount;
            }
            set
            {
                outcount = value;
            }
        }

        private string havecount;
        /// <summary>
        /// 剩余号源数
        /// </summary>
        public string HAVECOUNT
        {
            get
            {
                return havecount;
            }
            set
            {
                havecount = value;
            }
        }

        private string totalregfee;
        /// <summary>
        /// 总挂号费
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
        /// 检查费
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

        private string servicefee;
        /// <summary>
        /// 服务费
        /// </summary>
        public string SERVICEFEE
        {
            get
            {
                return servicefee;
            }
            set
            {
                servicefee = value;
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

        private string waitno;
        /// <summary>
        /// 等候人数
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

        private string elderlyvoucherdoctorflag;
        /// <summary>
        /// 长者券医生标识
        /// </summary>
        public string ElderlyVoucherDoctorFlag
        {
            get
            {
                return elderlyvoucherdoctorflag;
            }
            set
            {
                elderlyvoucherdoctorflag = value;
            }
        }


    }
}
