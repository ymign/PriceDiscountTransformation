using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.YYSS
{
    public class InPatientApply
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

        private string visit_id;

        /// <summary>
        /// 门诊次序号
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

        private string diagnose;
        /// <summary>
        /// 诊断
        /// </summary>
        public string DIAGNOSE
        {
            get
            {
                return diagnose;
            }
            set
            {
                diagnose = value;
            }
        }

        private string patient_telephone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PATIENT_TELEPHONE
        {
            get
            {
                return patient_telephone;
            }
            set
            {
                patient_telephone = value;
            }
        }

        private string ptnt_id;
        /// <summary>
        /// 患者就诊ID
        /// </summary>
        public string PTNT_ID
        {
            get 
            {
                return ptnt_id;
            }
            set
            {
                ptnt_id = value;
            }
        }

        private string ptnt_no;
        /// <summary>
        /// 病案号
        /// </summary>
        public string PTNT_NO
        {
            get
            {
                return ptnt_no;
            }
            set
            {
                ptnt_no = value;
            }
        }

        private string ic_card;
        /// <summary>
        /// 卡号
        /// </summary>
        public string IC_CARD
        {
            get
            {
                return ic_card;
            }
            set
            {
                ic_card = value;
            }
        }

        private string patient_name;
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PATIENT_NAME
        {
            get
            {
                return patient_name;
            }
            set
            {
                patient_name = value;
            }
        }

        private string patient_sex;
        /// <summary>
        /// 性别
        /// </summary>
        public string PATIENT_SEX
        {
            get
            {
                return patient_sex;
            }
            set
            {
                patient_sex = value;
            }
        }

        private string id_card;
        /// <summary>
        /// 身份证
        /// </summary>
        public string ID_CARD
        {
            get
            {
                return id_card;
            }
            set
            {
                id_card = value;
            }
        }

        private string patient_birth;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string PATIENT_BIRTH
        {
            get
            {
                return patient_birth;
            }
            set
            {
                patient_birth = value;
            }
        }

        private string patient_age;
        /// <summary>
        /// 年龄
        /// </summary>
        public string PATIENT_AGE
        {
            get
            {
                return patient_age;
            }
            set
            {
                patient_age = value;
            }
        }

        private string patient_phone;
        /// <summary>
        /// 患者电话
        /// </summary>
        public string PATIENT_PHONE
        {
            get
            {
                return patient_phone;
            }
            set
            {
                patient_phone = value;
            }
        }

        private string zip_code;
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string ZIP_CODE
        {
            get
            {
                return zip_code;
            }
            set
            {
                zip_code = value;
            }
        }

        private string address;
        /// <summary>
        /// 地址
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

        private string dept_id;
        /// <summary>
        /// 门诊科室编码
        /// </summary>
        public string DEPT_ID
        {
            get
            {
                return dept_id;
            }
            set
            {
                dept_id = value;
            }
        }

        private string dept_name;
        /// <summary>
        /// 门诊科室名称
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

        private string clinic_date;
        /// <summary>
        /// 就诊时间
        /// </summary>
        public string CLINIC_DATE
        {
            get
            {
                return clinic_date;
            }
            set
            {
                clinic_date = value;
            }
        }

        private string diagnosis;
        /// <summary>
        /// 诊断结果
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

        private string start_time;

        /// <summary>
        /// 开始时间
        /// </summary>
        public string START_TIME
        {
            get
            {
                return start_time;
            }
            set
            {
                start_time = value;
            }
        }

        private string end_time;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string END_TIME
        {
            get
            {
                return end_time;
            }
            set
            {
                end_time = value;
            }
        }
      
    }
}
