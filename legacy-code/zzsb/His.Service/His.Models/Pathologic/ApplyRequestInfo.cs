using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Pathologic
{
    public class ApplyRequestInfo
    {
        private string apply_flow_num;
        public string APLY_FLOW_NUM
        {
            get
            {
                return apply_flow_num;
            }
            set
            {
                apply_flow_num = value;
            }
        }

        private string bill;
        public string BILL_NO
        {
            get
            {
                return bill;
            }
            set
            {
                bill = value;
            }
        }

        private string p_type;
        public string PATIENT_TYPE
        {
            get
            {
                return p_type;
            }
            set
            {
                p_type = value;
            }
        }

        private string p_id;
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

        private string sample_barnum;
        public string SAMPLE_BARNUM
        {
            get
            {
                return sample_barnum;
            }
            set
            {
                sample_barnum = value;
            }
        }


        private string cardno;
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

        private string empi;
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

        private string start_time;
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

        private string patient_name;
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

        private string exam_type;
        public string EXAM_TYPE
        {
            get
            {
                return exam_type;
            }
            set
            {
                exam_type = value;
            }
        }
    }
}
