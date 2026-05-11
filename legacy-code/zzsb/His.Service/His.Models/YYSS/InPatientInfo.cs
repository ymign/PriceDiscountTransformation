using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.YYSS
{
    /// <summary>
    /// 住院病人信息表
    /// </summary>
    public class InPatientInfo
    {
        private string empi;
        /// <summary>
        /// 患者主索引
        /// </summary>
        public string EMPI
        {
            get
            {
                return empi;
            }
            set
            {
                empi = value;
            }
        }

        private string patient_id;

        /// <summary>
        /// 病人唯一标识
        /// </summary>
        public string PATIENT_ID
        {
            get
            {
                return this.patient_id;
            }
            set
            {
                this.patient_id = value;
            }
        }

        private string inp_no;
        /// <summary>
        /// 住院号
        /// </summary>
        public string INP_NO
        {
            get
            {
                return this.inp_no;
            }
            set
            {
                this.inp_no = value;
            }
        }

        private string visit_id;
        /// <summary>
        /// 住院次序号（第几次住院）
        /// </summary>
        public string VISIT_ID
        {
            get
            {
                return visit_id;
            }
            set
            {
                visit_id = value;
            }
        }

        private string dept_code;
        /// <summary>
        /// 科室代码
        /// </summary>
        public string DEPT_CODE
        {
            get 
            {
                return dept_code;
            }
            set
            {
                dept_code = value;
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

        private string ward_code;
        /// <summary>
        /// 病区编码
        /// </summary>
        public string WARD_CODE
        {
            get
            {
                return ward_code;
            }
            set
            {
                ward_code = value;
            }
        }

        private string ward_name;
        /// <summary>
        /// 病区名称
        /// </summary>
        public string WARD_NAME
        {
            get
            {
                return ward_name;
            }
            set
            {
                ward_name = value;
            }
        }

        private string bed_no;
        /// <summary>
        /// 床号
        /// </summary>
        public string BED_NO
        {
            get
            {
                return bed_no;
            }
            set
            {
                bed_no = value;
            }
        }

        private string name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string NAME
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private string sex;
        /// <summary>
        /// 性别 男、女
        /// </summary>
        public string SEX
        {
            get
            {
                return sex;
            }
            set
            {
                sex = value;
            }
        }

        private DateTime birthday=System.DateTime.Now.Date;
        /// <summary>
        /// 出生年月 yyyy-MM-dd格式
        /// </summary>
        public DateTime BIRTHDAY
        {
            get
            {
                return birthday;
            }
            set
            {
                birthday = value;
            }
        }

        private string age_of_year;
        /// <summary>
        /// 年龄
        /// </summary>
        public string AGE_OF_YEAR
        {
            get
            {
                return age_of_year;
            }
            set
            {
                age_of_year = value;
            }
        }

        private string age_of_month;
        /// <summary>
        /// 月龄
        /// 儿童信息中可提供，成人信息可不提供（可为null值）
        /// </summary>
        public string AGE_OF_MONTH
        {
            get
            {
                return age_of_month;
            }
            set
            {
                age_of_month = value;
            }
        }

        private string age_of_day;
        /// <summary>
        /// 天
        /// 儿童信息中可提供，成人信息可不提供（可为null值）
        /// </summary>
        public string AGE_OF_DAY
        {
            get
            {
                return age_of_day;
            }
            set
            {
                age_of_day = value;
            }
        }

        private string height;
        /// <summary>
        /// 身高
        /// 可为null
        /// </summary>
        public string HEIGHT
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        private string weight;
        /// <summary>
        /// 体重
        /// 可为null
        /// </summary>
        public string WEIGHT
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        private string mobile_phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string MOBILE_PHONE
        {
            get
            {
                return mobile_phone;
            }
            set
            {
                mobile_phone = value;
            }
        }

        private string address;
        /// <summary>
        /// 现住址
        /// </summary>
        public string ADDRESS
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        private string id_no;

        /// <summary>
        /// 身份证号
        /// </summary>
        public string ID_NO
        {
            get
            {
                return id_no;
            }
            set
            {
                id_no = value;
            }
        }

        private string charge_type="0";
        /// <summary>
        /// 收费类型
        /// 0-标准价格 1-优惠价格 2-外宾价格
        /// </summary>
        public string CHARGE_TYPE
        {
            get
            {
                return charge_type;
            }
            set
            {
                charge_type = value;
            }
        }

        private string balance=string.Empty;
        /// <summary>
        /// 预交金余额
        ///只上临床营养系统，该字段可为空
        /// </summary>
        public string BALANCE
        {
            get
            {
                return balance;
            }
            set
            {
                balance = value;
            }
        }

        private DateTime in_hos_date_time=System.DateTime.Now;
        /// <summary>
        /// 入院时间
        /// yyyy-MM-dd  HH:mm:ss格式
        /// </summary>
        public DateTime IN_HOS_DATE_TIME
        {
            get
            {
                return in_hos_date_time;
            }
            set
            {
                in_hos_date_time = value;
            }
        }

        private string diagnosis=string.Empty;
        /// <summary>
        /// 主要诊断
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

        private string settled_indicator = "否";
        /// <summary>
        /// 出院结算标识
        /// 是、否
        /// </summary>
        public string SETTLED_INDICATOR
        {
            get
            {
                return settled_indicator;
            }
            set
            {
                settled_indicator = value;
            }
        }

        private string out_hos_date = string.Empty;
        /// <summary>
        /// 出院时间
        /// yyyy-MM-dd HH:mm:ss格式、可为null
        /// </summary>
        public string OUT_HOS_DATE
        {
            get
            {
                return out_hos_date;
            }
            set
            {
                out_hos_date = value;
            }
        }

        private string out_status = string.Empty;
        /// <summary>
        /// 出院时病人情况
        /// 可为null
        /// </summary>
        public string OUT_STATUS
        {
            get
            {
                return out_status;
            }
            set
            {
                out_status = value;
            }
        }
    }
}
