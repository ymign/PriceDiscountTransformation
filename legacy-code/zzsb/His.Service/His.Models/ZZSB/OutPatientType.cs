using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class OutPatientType
    {
        private string IsPrintable;
        /// <summary>
        /// 打印状态标记
        /// </summary>
        public string ISPRINTABLE
        {
            get
            {
                return IsPrintable;
            }
            set
            {
                IsPrintable = value;
            }
        }

        private string invoiceno;
        /// <summary>
        /// 发票号
        /// </summary>
        public string INVOICENO
        {
            get
            {
                return invoiceno;
            }
            set
            {
                invoiceno = value;
            }
        }

        private string patientid;
        /// <summary>
        /// 门诊号
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
    
    }
}
