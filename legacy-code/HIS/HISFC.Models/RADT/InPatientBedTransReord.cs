using System;
 
namespace Neusoft.HISFC.Models.RADT
{
	/// <summary>
    /// InPatientBedTransReord <br></br>
	/// [功能描述: 住院病人床位变更记录]<br></br>
	/// [创 建 者: xiaohf]<br></br>
	/// [创建时间: 2012年8月5日14:14:52]<br></br>
	/// </summary>
    [Serializable]
    public class InPatientBedTransReord : Neusoft.FrameWork.Models.NeuObject
	{
		/// <summary>
		/// 患者索引号
		/// </summary>
        public InPatientBedTransReord()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		
		}

		#region 变量
        /// <summary>
        /// 住院流水号
        /// </summary>
        private string inpatient_no = string.Empty;
        /// <summary>
        /// 住院号（处理婴儿情况）
        /// </summary>
        private string patient_no = string.Empty;
        /// <summary>
        /// 原科室编码
        /// </summary>
        private string old_dept_id = string.Empty;
        /// <summary>
        /// 原科室名称
        /// </summary>
        private string old_dept_name = string.Empty;
        /// <summary>
        /// 目标科室编码
        /// </summary>
        private string target_dept_id = string.Empty;
        /// <summary>
        /// 目标科室名称
        /// </summary>
        private string target_dept_name = string.Empty;
        /// <summary>
        /// 床位编码
        /// </summary>
        private string bed_no = string.Empty;
        /// <summary>
        /// 床位操作类型，0-上床，1-下床
        /// </summary>
        private string trans_type = string.Empty;
        /// <summary>
        /// 床位操作来源编码,记录操作类型如入院登记，护士站接诊等
        /// </summary>
        private string trans_code = string.Empty;
        /// <summary>
        /// 医疗组编码
        /// </summary>
        private string medical_group_code = string.Empty;
        /// <summary>
        /// 护理组编码
        /// </summary>
        private string care_group_code = string.Empty;
        /// <summary>
        /// 在院管床医生
        /// </summary>
        private string in_doct_code = string.Empty;
        /// <summary>
        /// 护士站编码
        /// </summary>
        private string nurse_station_code = string.Empty;
        /// <summary>
        /// 转归编码（同住院主表ZG）
        /// </summary>
        private string zg = string.Empty;
        /// <summary>
        /// 操作流水号，用于和明细表关联
        /// </summary>
        private string sequence_no = string.Empty;
        /// <summary>
        /// 操作人
        /// </summary>
        private string oper_code = string.Empty;
        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime oper_date = DateTime.MinValue;
        /// <summary>
        /// 病人当前所在科室编码
        /// </summary>
        private string dept_code = string.Empty;
        /// <summary>
        /// 原护士站（病区）编码
        /// </summary>
        private string old_nurse_id = string.Empty;
        /// <summary>
        /// 原护士站（病区）名称
        /// </summary>
        private string old_nurse_name = string.Empty;
        /// <summary>
        /// 目标护士站（病区）编码
        /// </summary>
        private string target_nurse_id = string.Empty;
        /// <summary>
        /// 目标护士站（病区）名称
        /// </summary>
        private string target_nurse_name = string.Empty;

		#endregion

		#region 属性

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string INPATIENT_NO
        {
            get
            {
                return this.inpatient_no;
            }
            set
            {
                this.inpatient_no = value;
            }
        }
        /// <summary>
        /// 住院号（处理婴儿情况）
        /// </summary>
        public string PATIENT_NO
        {
            get
            {
                return this.patient_no;
            }
            set
            {
                this.patient_no = value;
            }
        }
        /// <summary>
        /// 原科室编码
        /// </summary>
        public string OLD_DEPT_ID
        {
            get
            {
                return this.old_dept_id;
            }
            set
            {
                this.old_dept_id = value;
            }
        }
        /// <summary>
        /// 原科室名称
        /// </summary>
        public string OLD_DEPT_NAME
        {
            get
            {
                return this.old_dept_name;
            }
            set
            {
                this.old_dept_name = value;
            }
        }
        /// <summary>
        /// 目标科室编码
        /// </summary>
        public string TARGET_DEPT_ID
        {
            get
            {
                return this.target_dept_id;
            }
            set
            {
                this.target_dept_id = value;
            }
        }
        /// <summary>
        /// 目标科室名称
        /// </summary>
        public string TARGET_DEPT_NAME
        {
            get
            {
                return this.target_dept_name;
            }
            set
            {
                this.target_dept_name = value;
            }
        }
        /// <summary>
        /// 床位编码
        /// </summary>
        public string BED_NO
        {
            get
            {
                return this.bed_no;
            }
            set
            {
                this.bed_no = value;
            }
        }
        /// <summary>
        /// 床位操作类型，0-上床，1-下床
        /// </summary>
        public string TRANS_TYPE
        {
            get
            {
                return this.trans_type;
            }
            set
            {
                this.trans_type = value;
            }
        }
        /// <summary>
        /// 床位操作来源编码,记录操作类型如入院登记，护士站接诊等
        /// </summary>
        public string TRANS_CODE
        {
            get
            {
                return this.trans_code;
            }
            set
            {
                this.trans_code = value;
            }
        }
        /// <summary>
        /// 医疗组编码
        /// </summary>
        public string MEDICAL_GROUP_CODE
        {
            get
            {
                return this.medical_group_code;
            }
            set
            {
                this.medical_group_code = value;
            }
        }
        /// <summary>
        /// 护理组编码
        /// </summary>
        public string CARE_GROUP_CODE
        {
            get
            {
                return this.care_group_code;
            }
            set
            {
                this.care_group_code = value;
            }
        }
        /// <summary>
        /// 在院管床医生
        /// </summary>
        public string IN_DOCT_CODE
        {
            get
            {
                return this.in_doct_code;
            }
            set
            {
                this.in_doct_code = value;
            }
        }
        /// <summary>
        /// 护士站编码
        /// </summary>
        public string NURSE_STATION_CODE
        {
            get
            {
                return this.nurse_station_code;
            }
            set
            {
                this.nurse_station_code = value;
            }
        }
        /// <summary>
        /// 转归编码（同住院主表ZG）
        /// </summary>
        public string ZG
        {
            get
            {
                return this.zg;
            }
            set
            {
                this.zg = value;
            }
        }
        /// <summary>
        /// 操作流水号，用于和明细表关联
        /// </summary>
        public string SEQUENCE_NO
        {
            get
            {
                return this.sequence_no;
            }
            set
            {
                this.sequence_no = value;
            }
        }
        /// <summary>
        /// 操作人
        /// </summary>
        public string OPER_CODE
        {
            get
            {
                return this.oper_code;
            }
            set
            {
                this.oper_code = value;
            }
        }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OPER_DATE
        {
            get
            {
                return this.oper_date;
            }
            set
            {
                this.oper_date = value;
            }
        }
           /// <summary>
        /// 病人当前所在科室编码
        /// </summary>
        public string DEPT_CODE
        {
            get
            {
                return this.dept_code;
            }
            set
            {
                this.dept_code = value;
            }
        }

        /// <summary>
        /// 原科室编码
        /// </summary>
        public string OLD_NURSE_ID
        {
            get
            {
                return this.old_nurse_id;
            }
            set
            {
                this.old_nurse_id = value;
            }
        }
        /// <summary>
        /// 原科室名称
        /// </summary>
        public string OLD_NURSE_NAME
        {
            get
            {
                return this.old_nurse_name;
            }
            set
            {
                this.old_nurse_name = value;
            }
        }
        /// <summary>
        /// 目标科室编码
        /// </summary>
        public string TARGET_NURSE_ID
        {
            get
            {
                return this.target_nurse_id;
            }
            set
            {
                this.target_nurse_id = value;
            }
        }
        /// <summary>
        /// 目标科室名称
        /// </summary>
        public string TARGET_NURSE_NAME
        {
            get
            {
                return this.target_nurse_name;
            }
            set
            {
                this.target_nurse_name = value;
            }
        }
		#endregion

		#region 方法

		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns></returns>
        public new InPatientBedTransReord Clone()
		{
            return this.MemberwiseClone() as InPatientBedTransReord;
		}

		#endregion
	}
}
