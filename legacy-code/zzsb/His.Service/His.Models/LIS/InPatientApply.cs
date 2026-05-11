using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.LIS
{
    public class InPatientApply
    {
        private string aply_flow_num;
        /// <summary>
        /// 申请单流水号
        /// </summary>
        public string APLY_FLOW_NUM
        {
            get
            {
                return aply_flow_num;
            }
            set
            {
                aply_flow_num = value;
            }
        }

        private string patient_type;

        /// <summary>
        /// 患者来源（患者类型：门诊、住院、体检）
        /// </summary>
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

        private string barcode;
        /// <summary>
        /// 样本条码号
        /// </summary>
        public string BARCODE
        {
            get 
            {
                return barcode;
            }
            set
            {
                barcode = value;
            }
        }

        private string invoice_no;
        /// <summary>
        /// 发票号
        /// </summary>
        public string BILL_NO
        {
            get
            {
                return invoice_no;
            }
            set
            {
                invoice_no = value;
            }
        }

        private string ptnt_id;
        /// <summary>
        /// 就诊患者ID（门诊号）
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

        private string empi;
        /// <summary>
        /// 患者主索引号码
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

        private string lab_type;
        /// <summary>
        /// 检验类型
        /// </summary>
        public string LAB_TYPE
        {
            get
            {
                return lab_type;
            }
            set
            {
                lab_type = value;
            }
        }

        private string errmsg;
        /// <summary>
        /// 失败时，返还错误信息；成功时，不返回信息。
        /// </summary>
        public string ERRMSG
        {
            get
            {
                return errmsg;
            }
            set
            {
                errmsg = value;
            }
        }

        private string returnFlag;
        /// <summary>
        /// 1-成功 0-失败
        /// </summary>
        public string RETURN
        {
            get
            {
                return returnFlag;
            }
            set
            {
                returnFlag = value;
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

        private string visit_id;
        /// <summary>
        /// 就诊次数
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

        private string ptnt_no;
        /// <summary>
        /// 病历号
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

        private string ctat_addr;
        /// <summary>
        /// 联系地址
        /// </summary>
        public string CTAT_ADDR
        {
            get
            {
                return ctat_addr;
            }
            set
            {
                ctat_addr = value;
            }
        }

        private string phone_num;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PHONE_NUM
        {
            get
            {
                return phone_num;
            }
            set
            {
                phone_num = value;
            }
        }

        private string ptnt_no_type;
        /// <summary>
        /// 病历号类型
        /// </summary>
        public string PTNT_NO_TYPE
        {
            get
            {
                return ptnt_no_type;
            }
            set
            {
                ptnt_no_type = value;
            }
        }

        private string ptnt_name;
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PTNT_NAME
        {
            get
            {
                return ptnt_name;
            }
            set
            {
                ptnt_name = value;
            }
        }

        private string ptnt_sex;
        /// <summary>
        /// 患者性别
        /// </summary>
        public string PTNT_SEX
        {
            get
            {
                return ptnt_sex;
            }
            set
            {
                ptnt_sex = value;
            }
        }

        private string ptnt_age;
        /// <summary>
        /// 患者年龄
        /// </summary>
        public string PTNT_AGE
        {
            get
            {
                return ptnt_age;
            }
            set
            {
                ptnt_age = value;
            }
        }

        private string ptnt_age_unit;
        /// <summary>
        /// 年龄单位
        /// </summary>
        public string PTNT_AGE_UNIT
        {
            get
            {
                return ptnt_age_unit;
            }
            set
            {
                ptnt_age_unit = value;
            }
        }

        private string admisse_date;
        /// <summary>
        /// 入院日期/就诊日期
        /// </summary>
        public string ADMISSE_DATE
        {
            get
            {
                return admisse_date;
            }
            set
            {
                admisse_date = value;
            }
        }

        private string ptnt_birth;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string PTNT_BIRTH
        {
            get
            {
                return ptnt_birth;
            }
            set
            {
                ptnt_birth = value;
            }
        }

        private string ptnt_bed_no;
        /// <summary>
        /// 床号
        /// </summary>
        public string PTNT_BED_NO
        {
            get
            {
                return ptnt_bed_no;
            }
            set
            {
                ptnt_bed_no = value;
            }
        }

        private string diag_info;
        /// <summary>
        /// 诊断信息
        /// </summary>
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

        private string aply_detl_id;
        /// <summary>
        /// PK,可用申请项目的流水号
        /// </summary>
        public string APLY_ID
        {
            get
            {
                return aply_detl_id;
            }
            set
            {
                aply_detl_id = value;
            }
        }

        private string aply_src;
        /// <summary>
        /// 申请来源 0 - HIS系统；1 - LIS系统；2 - 体检系统；3 - 其他来源
        /// </summary>
        public string APLY_SRC
        {
            get
            {
                return aply_src;
            }
            set
            {
                aply_src = value;
            }
        }

        private string aply_create_date;
        /// <summary>
        /// 申请创建日期
        /// </summary>
        public string APLY_CREATE_DATE
        {
            get
            {
                return aply_create_date;
            }
            set
            {
                aply_create_date = value;
            }
        }

        private string emcy_mrk;
        /// <summary>
        /// 急诊标记
        /// </summary>
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

        private string aply_date;
        /// <summary>
        /// 申请日期
        /// </summary>
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

        private string dept_key;
        /// <summary>
        /// 申请科室编码
        /// </summary>
        public string DEPT_KEY
        {
            get
            {
                return dept_key;
            }
            set
            {
                dept_key = value;
            }
        }

        private string dept_name;
        /// <summary>
        /// 申请科室名称
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

        private string doc_key;
        /// <summary>
        /// 申请医生编码
        /// </summary>
        public string DOC_KEY
        {
            get
            {
                return doc_key;
            }
            set
            {
                doc_key = value;
            }
        }

        private string doc_name;
        /// <summary>
        /// 申请医生名称
        /// </summary>
        public string DOC_NAME
        {
            get
            {
                return doc_name;
            }
            set
            {
                doc_name = value;
            }
        }

        private string aply_itm_key;
        /// <summary>
        /// 申请项目对照编码
        /// </summary>
        public string APLY_ITM_KEY
        {
            get
            {
                return aply_itm_key;
            }
            set
            {
                aply_itm_key = value;
            }
        }

        private string aply_itm_name;
        /// <summary>
        /// 申请项目名称
        /// </summary>
        public string APLY_ITM_NAME
        {
            get
            {
                return aply_itm_name;
            }
            set
            {
                aply_itm_name = value;
            }
        }

        private string smpl_key;
        /// <summary>
        /// 样本类型编码
        /// </summary>
        public string SMPL_KEY
        {
            get
            {
                return smpl_key;
            }
            set
            {
                smpl_key = value;
            }
        }

        private string smpl_name;
        /// <summary>
        /// 样本类型名称
        /// </summary>
        public string SMPL_NAME
        {
            get
            {
                return smpl_name;
            }
            set
            {
                smpl_name = value;
            }
        }

        private string body_part;
        /// <summary>
        /// 取材部位
        /// </summary>
        public string BODY_PART
        {
            get
            {
                return body_part;
            }
            set
            {
                body_part = value;
            }
        }

        private string remark;
        /// <summary>
        /// 执行说明
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

        private string exec_status;
        /// <summary>
        /// 执行状态
        /// </summary>
        public string EXEC_STATUS
        {
            get
            {
                return exec_status;
            }
            set
            {
                exec_status = value;
            }
        }

      
    }
}
