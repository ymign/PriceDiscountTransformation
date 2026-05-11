using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Pathologic
{
   public class SampleReceivedRequestInfo
    {
        private string barnum=string.Empty;
        public string SAMPLE_BARNUM
        {
            get
            {
                return barnum;
            }
            set
            {
                barnum = value;
            }
        }

        private string order_id = string.Empty;
        public string ORDER_ID
        {
            get
            {
                return order_id;
            }
            set
            {
                order_id = value;
            }
        }

        private string apply_num = string.Empty;
        public string APLY_FLOW_NUM
        {
            get
            {
                return apply_num;
            }
            set
            {
                apply_num = value;
            }
        }

        private string rcvr_name = string.Empty;
        public string RCVR_NAME
        {
            get
            {
                return rcvr_name;
            }
            set
            {
                rcvr_name = value;
            }
        }

        private string rcv_time = string.Empty;
        public string RCV_TIME
        {
            get
            {
                return rcv_time;
            }
            set
            {
                rcv_time = value;
            }
        }

        private string dept_name = string.Empty;
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

        private string dept_code = string.Empty;
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

        private string bodypart_code = string.Empty;
        public string BODYPART_CODE
        {
            get
            {
                return bodypart_code;
            }
            set
            {
                bodypart_code = value;
            }
        }

        private string bodypart_name = string.Empty;
        public string BODYPART_NAME
        {
            get
            {
                return bodypart_name;
            }
            set
            {
                bodypart_name = value;
            }
        }

        private string sample_code = string.Empty;
        public string SAMPLE_CODE
        {
            get
            {
                return sample_code;
            }
            set
            {
                sample_code = value;
            }
        }

        private string sample_name = string.Empty;
        public string SAMPLE_NAME
        {
            get
            {
                return sample_name;
            }
            set
            {
                sample_name = value;
            }
        }


        private string patient_type=string.Empty;
        public string PATIENT_TYPE
        {
            get
            {
                return patient_type;
            }
            set
            {
                patient_type = value;
            }
        }

    }
}
