using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.LIS
{
    public class InPatient
    {

        private string inpatient_no;
        /// <summary>
        /// 住院流水号
        /// </summary>
        public string Inpatient_no
        {
            get { return inpatient_no; }
            set { inpatient_no = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private string medical_type;

        private string patient_no;

        /// <summary>
        /// 住院号
        /// </summary>
        public string Patient_no
        {
            get { return patient_no; }
            set { patient_no = value; }
        }

        private string card_no;

        /// <summary>
        /// 门诊号
        /// </summary>
        public string Card_no
        {
            get { return card_no; }
            set { card_no = value; }
        }
        private string mcard_no;
        private string name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string sex_code;
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return sex_code; }
            set { sex_code = value; }
        }
        private string idenno;
        private string spell_code;
        private string birthday;
        private string prof_code;
        private string work_name;
        private string work_tel;
        private string work_zip;
        private string home;
        private string home_tel;
        private string home_zip;
        private string dist;
        private string birth_area;
        private string nation_code;
        private string linkman_name;
        private string linkman_tel;
        private string linkman_add;
        private string rela_code;
        private string mari;
        private string coun_code;
        private string height;
        private string weight;
        private string blood_dress;
        private string blood_code;
        private string hepatitis_flag;
        private string anaphy_flag;
        private string in_date;

        private string dept_code;
        /// <summary>
        /// 科室编码
        /// </summary>
        public string Dept_code
        {
            get { return dept_code; }
            set { dept_code = value; }
        }
        private string dept_name;
        /// <summary>
        /// 科室名称
        /// </summary>
        public string Dept_name
        {
            get { return dept_name; }
            set { dept_name = value; }
        }
        private string paykind_code;
        private string pact_code;
        private string pact_name;

        private string bed_no;

        /// <summary>
        /// 床位
        /// </summary>
        public string Bed_no
        {
            get { return bed_no; }
            set { bed_no = value; }
        }


        private string nurse_cell_code;

        /// <summary>
        /// 护士站编码
        /// </summary>
        public string Nurse_cell_code
        {
            get { return nurse_cell_code; }
            set { nurse_cell_code = value; }
        }

        private string nurse_cell_name;

        /// <summary>
        /// 护士站名称
        /// </summary>
        public string Nurse_cell_name
        {
            get { return nurse_cell_name; }
            set { nurse_cell_name = value; }
        }

        private string house_doc_code;
        private string house_doc_name;
        private string charge_doc_code;
        private string charge_doc_name;
        private string chief_doc_code;
        private string chief_doc_name;
        private string duty_nurse_code;
        private string duty_nurse_name;
        private string in_circs;
        private string in_avenue;
        private string in_source;
        private string in_times;
        private string prepay_cost;
        private string change_prepaycost;
        private string money_alert;
        private string tot_cost;
        private string own_cost;
        private string pay_cost;
        private string pub_cost;
        private string eco_cost;
        private string free_cost;
        private string change_totcost;
        private string upper_limit;
        private string fee_interval;
        private string balance_no;
        private string balance_cost;
        private string balance_prepay;
        private string balance_date;
        private string stop_acount;
        private string baby_flag;
        private string case_flag;
        private string in_state;
        private string leave_flag;
        private string prepay_outdate;
        private string out_date;
        private string zg;
        private string empl_code;
        private string in_icu;
        private string casesend_flag;
        private string tend;
        private string critical_flag;
        private string prefixfee_date;
        private string oper_code;
        private string oper_date;
        private string blood_latefee;
        private string day_limit;
        private string limit_tot;
        private string limit_overtop;
        private string clinic_diagnose;
        private string procreate_pcno;
        private string dietetic_mark;
        private string bursary_totmedfee;
        private string memo;
        private string bed_limit;
        private string air_limit;
        private string bedoverdeal;
        private string ext_flag;
        private string ext_flag1;
        private string ext_flag2;
        private string board_cost;
        private string board_prepay;
        private string board_state;
        private string own_rate;
        private string pay_rate;
        private string ext_number;
        private string ext_code;
        private string diag_name;
        private string is_encryptname;
        private string normalname;
        private string idcardtype;
        private string alter_type;
        private string alter_begin;
        private string alter_end;
        private string alter_approve_code;
        private string alter_approve_date;
        private string emr_inpatientid;
        private string home_now;
        private string alter_flag;
    }
}
