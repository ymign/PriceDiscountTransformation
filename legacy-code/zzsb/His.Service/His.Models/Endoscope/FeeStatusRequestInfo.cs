using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Endoscope
{
   
    public class FeeStatusRequestInfo
    {
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


        private string oper_code;
        public string OPER_CODE
        {
            get
            {
                return oper_code;
            }
            set
            {
                oper_code = value;
            }
        }

        private string oper_name ;
        public string  OPER_NAME
        {
            get
            {
                return oper_name;
            }
            set
            {
                 oper_name= value;
            }
        }


        private string exam_sys_code;
        public string EXAM_SYSTEM_CODE
        {
            get
            {
                return exam_sys_code;
            }
            set
            {
                exam_sys_code = value;
            }
        }

        private string item_code;
        public string APLY_ITM_CODE
        {
            get
            {
                return item_code;
            }
            set
            {
                item_code = value;
            }
        }

        private string item_name;
        public string APLY_ITM_NAME
        {
            get
            {
                return item_name;
            }
            set
            {
                item_name = value;
            }
        }

        /// <summary>
        /// 到检登记流水号
        /// </summary>
        private string check_reg_num;
        public string CHECK_REG_NUM
        {
            get
            {
                return check_reg_num;
            }
            set
            {
                check_reg_num = value;
            }
        }

        private string check_time;
        public string CHECK_REG_TIME
        {
            get
            {
                return check_time;
            }
            set
            {
                check_time = value;
            }
        }

        private string cancel_time;
        public string CANCEL_CHECK_TIME
        {
            get
            {
                return cancel_time;
            }
            set
            {
                cancel_time = value;
            }
        }

        private string reason;
        public string CANCEL_CHECK_REASON
        {
            get
            {
                return reason;
            }
            set
            {
                reason = value;
            }
        }
    }
}
