using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Endoscope
{
    /// <summary>
    /// 病历申请单
    /// </summary>
    public class ApplyBill
    {
        #region bill
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

        private string order_name = string.Empty;
        public string ORDER_NAME
        {
            get
            {
                return order_name;
            }
            set
            {
                order_name = value;
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

        private string applyType = string.Empty;
        public string APLY_TYPE
        {
            get
            {
                return applyType;
            }
            set
            {
                applyType = value;
            }
        }

        private string emcy_mrk = string.Empty;
        public string EMCY_MRK
        {
            get
            {
                return emcy_mrk;
            }
            set
            {
                emcy_mrk = value;
            }
        }

        private string order_pri_code = string.Empty;
        public string ORDER_PRIORITY_CODE
        {
            get
            {
                return order_pri_code;
            }
            set
            {
                order_pri_code = value;
            }
        }

        private string order_pri = string.Empty;
        public string ORDER_PRIORITY
        {
            get
            {
                return order_pri;
            }
            set
            {
                order_pri = value;
            }
        }

        private string diag_info = string.Empty;
        public string DIAG_INFO
        {
            get
            {
                return diag_info;
            }
            set
            {
                diag_info = value;
            }
        }

        private string clinic_disease = string.Empty;
        public string CLINIC_DISEASE
        {
            get
            {
                return clinic_disease;
            }
            set
            {
                clinic_disease = value;
            }
        }

        private string operation_info = string.Empty;
        public string OPERATION_INFO
        {
            get
            {
                return operation_info;
            }
            set
            {
                operation_info = value;
            }
        }

        private string other_info = string.Empty;
        public string OTHER_INFO
        {
            get
            {
                return other_info;
            }
            set
            {
                other_info = value;
            }
        }

        private string remark = string.Empty;
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

        private string doccode = string.Empty;
        public string DOC_CODE
        {
            get
            {
                return doccode;
            }
            set
            {
                doccode = value;
            }
        }

        private string docname = string.Empty;
        public string DOC_NAME
        {
            get
            {
                return docname;
            }
            set
            {
                docname = value;
            }
        }

        private string dept_code;
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

        private string aply_create_time;
        public string APLY_CREATE_TIME
        {
            get { return aply_create_time; }
            set { aply_create_time = value; }
        }

        private string aply_date;
        public string APLY_DATE
        {
            get
            {
                return aply_date;
            }
            set
            {
                aply_date = value;
            }
        }

        private string exe_dept_code;
        public string EXE_DEPT_CODE
        {
            get
            {
                return exe_dept_code;
            }
            set
            {
                exe_dept_code = value;
            }
        }

        private string exe_dept_name;
        public string EXE_DEPT_NAME
        {
            get
            {
                return exe_dept_name;
            }
            set
            {
                exe_dept_name = value;
            }
        }

        private string bodypart_code;
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

        private string bodypart_name;
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

        private string sample_code;
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

        private string sample_name;
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

        private string cur_case;
        public string CUR_CASE
        {
            get
            {
                return cur_case;
            }
            set
            {
                cur_case = value;
            }
        }

        private string destination;
        public string DESTINATION
        {
            get
            {
                return destination;
            }
            set
            {
                destination = value;
            }
        }

#endregion

        #region patient
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

        private string sex;
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

        private string patient_work;
        public string PATIENT_WORK
        {
            get
            {
                return patient_work;
            }
            set
            {
                patient_work = value;
            }
        }

        private string patient_regtion;
        public string PATIENT_REGILION
        {
            get
            {
                return patient_regtion;
            }
            set
            {
                patient_regtion = value;
            }
        }

        private string patient_allergy;
        public string PATIENT_ALLERGY
        {
            get
            {
                return patient_allergy;
            }
            set
            {
                patient_allergy = value;
            }
        }

        private string nation;
        public string PATIENT_NATION
        {
            get
            {
                return nation;
            }
            set
            {
                nation = value;
            }
        }

        private string origin;
        public string PATIENT_ORIGIN
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }

        private string addr;
        public string PATIENT_ADDRESS
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

        private string tel;
        public string PATIENT_TELEPHONE
        {
            get
            {
                return tel;
            }
            set
            {
                tel = value;
            }
        }

        private string brithday;
        public string PATIENT_BIRTH
        {
            get
            {
                return brithday;
            }
            set
            {
                brithday = value;
            }
        }

        private string work_code;
        public string WARD_CODE
        {
            get
            {
                return work_code;
            }
            set
            {
                work_code = value;
            }
        }

        private string work;
        public string WARD
        {
            get
            {
                return work;
            }
            set
            {
                work = value;
            }
        }

        private string room;
        public string ROOM_CODE
        {
            get
            {
                return room;
            }
            set
            {
                room = value;
            }
        }

        private string room_name;
        public string ROOM
        {
            get
            {
                return room_name;
            }
            set
            {
                room_name = value;
            }
        }

        private string bedno;
        public string BED_NO
        {
            get
            {
                return bedno;
            }
            set
            {
                bedno = value;
            }
        }

#endregion

        #region  fee

         private string feecode;
        public string ITEM_FEE_CODE
        {
            get
            {
                return feecode;
            }
            set
            {
                feecode = value;
            }


        }

        private string feename;
        public string ITEM_FEE_NAME
        {
            get
            {
                return feename;
            }
            set
            {
                feename = value;
            }
        }

        private string fee_count;
        public string FEE_COUNT
        {
            get
            {
                return fee_count;
            }
            set
            {
                fee_count = value;
            }
        }

        private string price;
        public string ITEM_PRICE
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        private string status;
        public string FEE_STATUS
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        #endregion   

    }
}
