using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.wirelessAnalgesic
{
    public class PatientInfo
    {
        private string p_id;
        /// <summary>
        /// 住院号码
        /// </summary>
        public string PATIENT_ID
        {
            get
            {
                return p_id;
            }
            set
            {
                p_id = value;
            }
        }

        private string cardNo;
        /// <summary>
        /// 卡号
        /// </summary>
        public string CARDNO
        {
            get
            {
                return cardNo;
            }
            set
            {
                cardNo = value;
            }
        }

        private string empi;
        /// <summary>
        /// 患者主索引号
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

        private string sex;
        /// <summary>
        /// 性别
        /// </summary>
        public string PATIENT_SEX
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

        private string ward;
        /// <summary>
        /// 病区
        /// </summary>
        public string WARD
        {
            get
            {
                return ward;
            }
            set
            {
                ward = value;
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

        private string patient_weight;
        /// <summary>
        /// 体重
        /// </summary>
        public string PATIENT_WEIGHT
        {
            get
            {
                return patient_weight;
            }
            set
            {
                patient_weight = value;
            }
        }

        private string operation_name;
        /// <summary>
        /// 手术名称
        /// </summary>
        public string OPERATION_NAME
        {
            get
            {
                return operation_name;
            }
            set
            {
                operation_name = value;
            }
        }

        private string asa_level;
        /// <summary>
        /// 分级
        /// </summary>
        public string ASA_LEVEL
        {
            get
            {
                return asa_level;
            }
            set
            {
                asa_level = value;
            }
        }

        private string doctor_name;
        /// <summary>
        /// 医师
        /// </summary>
        public string DOCTOR_NAME
        {
            get
            {
                return doctor_name;
            }
            set
            {
                doctor_name = value;
            }
        }

        private string analgesia_style;
        /// <summary>
        /// 镇痛方式
        /// </summary>
        public string ANALGESIA_STYLE
        {
            get
            {
                return analgesia_style;
            }
            set
            {
                analgesia_style = value;
            }
        }

        private string formula_name;
        /// <summary>
        /// 配方
        /// </summary>
        public string FORMULA_NAME
        {
            get
            {
                return formula_name;
            }
            set
            {
                formula_name = value;
            }
        }

        private string operation_time;
        /// <summary>
        /// 最后一次的手术申请时间
        /// </summary>
        public string OPERATION_TIME
        {
            get
            {
                return operation_time;
            }
            set
            {
                operation_time = value;
            }
        }

        /// <summary>
        /// 住址
        /// </summary>
        private string addr;
        public string PATIENT_ADDR
        {
            get
            {
                return addr;
            }
            set
            {
                addr = value;
            }
        }

        /// <summary>
        /// 身份证
        /// </summary>
        private string idno;
        public string ID_CARD
        {
            get
            {
                return idno;
            }
            set
            {
                idno = value;
            }
        }

        /// <summary>
        /// 费别
        /// </summary>
        private string fee_type;
        public string FEE_TYPE
        {
            get
            {
                return fee_type;
            }
            set
            {
                fee_type = value;
            }
        }

        /// <summary>
        /// 科别（住院科室）
        /// </summary>
        private string dept_name;
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

        /// <summary>
        /// 诊断
        /// </summary>
        private string diag;
        public string DIAGNOSE
        {
            get
            {
                return diag;
            }
            set
            {
                diag = value;
            }
        }
    }
}
