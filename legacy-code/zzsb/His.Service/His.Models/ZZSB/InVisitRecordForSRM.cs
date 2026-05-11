using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InVisitRecordForSRM
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

        private string startdate;
        /// <summary>
        /// 开始时间
        /// </summary>
        public string STARTDATE
        {
            get
            {
                return startdate;
            }
            set
            {
                startdate = value;
            }
        }

        private string enddate;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string ENDDATE
        {
            get
            {
                return enddate;
            }
            set
            {
                enddate = value;
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

        private string regdate;
        /// <summary>
        /// 就诊日期
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

        private string rankname;
        /// <summary>
        /// 级别名称
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

        private string doctname;
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string DOCTNAME
        {
            get
            {
                return doctname;
            }
            set
            {
                doctname = value;
            }
        }

        private string totalfee;
        /// <summary>
        /// 总费用
        /// </summary>
        public string TOTALFEE
        {
            get
            {
                return totalfee;
            }
            set
            {
                totalfee = value;
            }
        }

        private string feetype;
        /// <summary>
        /// 费别类型
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

        private string favorfee;
        /// <summary>
        /// 优惠金额
        /// </summary>
        public string FAVORFEE
        {
            get
            {
                return favorfee;
            }
            set
            {
                favorfee = value;
            }
        }

        private string medinsurefee;
        /// <summary>
        /// 社保支付金额
        /// </summary>
        public string MEDINSUREFEE
        {
            get
            {
                return medinsurefee;
            }
            set
            {
                medinsurefee = value;
            }
        }

        private string personalfee;
        /// <summary>
        /// 自费金额
        /// </summary>
        public string PERSONALFEE
        {
            get
            {
                return personalfee;
            }
            set
            {
                personalfee = value;
            }
        }

        private string diagnosis;
        /// <summary>
        /// 诊断
        /// </summary>
        public string DIAGNOSIS
        {
            get
            {
                return diagnosis;
            }
            set
            {
                diagnosis = value;
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
