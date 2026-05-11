using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.BUltrasonic
{
    class RequestModel
    {
    }
    public class RequestApplyModel
    {
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

        //private string empi;
        //public string EMPI
        //{
        //    get
        //    {
        //        return empi;
        //    }
        //    set
        //    {
        //        empi = value;
        //    }
        //}

        private string cardNo;
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

        //private string patient_name;
        //public string PATIENT_NAME
        //{
        //    get
        //    {
        //        return patient_name;
        //    }
        //    set
        //    {
        //        patient_name = value;
        //    }
        //}

        //private string sample_barnum;
        //public string SAMPLE_BARNUM
        //{
        //    get
        //    {
        //        return sample_barnum;
        //    }
        //    set
        //    {
        //        sample_barnum = value;
        //    }
        //}

        //private string start_time;
        //public string START_TIME
        //{
        //    get
        //    {
        //        return start_time;
        //    }
        //    set
        //    {
        //        start_time = value;
        //    }
        //}

        //private string end_time;
        //public string END_TIME
        //{
        //    get
        //    {
        //        return end_time;
        //    }
        //    set
        //    {
        //        end_time = value;
        //    }
        //}

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

        private string exe_dept;
        public string EXECUTIVE_DEPT
        {
            get
            {
                return exe_dept;
            }
            set
            {
                exe_dept = value;
            }
        }

        private string str_date;
        public string START_TIME
        {
            get
            {
                return str_date;
            }
            set
            {
                str_date = value;
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

    }
}
