using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.CSSD
{
    public class InDisinfectionInfo
    {
        private string clinicnumber;
        /// <summary>
        /// 诊疗号
        /// </summary>
        public string CLINICNUMBER
        {
            get
            {
                return clinicnumber;
            }
            set
            {
                clinicnumber = value;
            }
        }


        private string serialnumber;
        /// <summary>
        /// 当前诊疗流水号
        /// </summary>
        public string SERIALNUMBER
        {
            get
            {
                return serialnumber;
            }
            set
            {
                serialnumber = value;
            }
        }

        private string patientname;
        /// <summary>
        /// 病人姓名
        /// </summary>
        public string PATIENTNAME
        {
            get
            {
                return patientname;
            }
            set
            {
                patientname = value;
            }
        }

        private string patientage;
        /// <summary>
        /// 病人年龄
        /// </summary>
        public string PATIENTAGE
        {
            get
            {
                return patientage;
            }
            set
            {
                patientage = value;
            }
        }

        private string patientsex;
        /// <summary>
        /// 病人性别
        /// </summary>
        public string PATIENTSEX
        {
            get
            {
                return patientsex;
            }
            set
            {
                patientsex = value;
            }
        }

        private string patientidcard;
        /// <summary>
        /// 病人身份证号
        /// </summary>
        public string PATIENTIDCARD
        {
            get
            {
                return patientidcard;
            }
            set
            {
                patientidcard = value;
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

        private string operation;
        /// <summary>
        /// 手术名称
        /// </summary>
        public string OPERATION
        {
            get
            {
                return operation;
            }
            set
            {
                operation = value;
            }
        }

        private string operationtime;
        /// <summary>
        /// 手术时间
        /// </summary>
        public string OPERATIONTIME
        {
            get
            {
                return operationtime;
            }
            set
            {
                operationtime = value;
            }
        }

        private string inpatientarea;
        /// <summary>
        /// 病人所属病区
        /// </summary>
        public string INPATIENTAREA
        {
            get
            {
                return inpatientarea;
            }
            set
            {
                inpatientarea = value;
            }
        }

        private string remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
            }
        }

        private string hospitalnumber;
        /// <summary>
        /// 住院号
        /// </summary>
        public string HOSPITALNUMBER
        {
            get
            {
                return hospitalnumber;
            }
            set
            {
                hospitalnumber = value;
            }
        }
    }
}

   
