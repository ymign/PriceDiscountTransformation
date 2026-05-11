using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.YYSS
{
    /// <summary>
    /// 病人医嘱信息表
    /// </summary>
    public class PatientOrder
    {
        private string empi;
        /// <summary>
        /// 患者主索引
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

        private string patient_id;

        /// <summary>
        /// 病人唯一标识
        /// </summary>
        public string PATIENT_ID
        {
            get
            {
                return this.patient_id;
            }
            set
            {
                this.patient_id = value;
            }
        }

        private string inp_no;
        /// <summary>
        /// 住院号
        /// </summary>
        public string INP_NO
        {
            get
            {
                return this.inp_no;
            }
            set
            {
                this.inp_no = value;
            }
        }

        private string visit_id;
        /// <summary>
        /// 住院次序号（第几次住院）
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

        private string order_no=string.Empty;
        /// <summary>
        /// 医嘱序号
        /// 一个病人的所有医嘱独立分配序号，按时间顺序，从小到大排序
        /// </summary>
        public string ORDER_NO
        {
            get 
            {
                return order_no;
            }
            set
            {
                order_no = value;
            }
        }

        private string order_sub_no=string.Empty;
        /// <summary>
        /// 医嘱子序号
        /// 用于标识成组医嘱中的各医嘱项目，对独立的医嘱，为1，在成组医嘱内部，从1开始顺序排列
        /// </summary>
        public string ORDER_SUB_NO
        {
            get
            {
                return order_sub_no;
            }
            set
            {
                order_sub_no = value;
            }
        }

        private DateTime start_date_time=System.DateTime.Now;
        /// <summary>
        /// 医嘱执行开始时间
        /// yyyy-MM-dd HH:mm:ss格式
        /// </summary>
        public DateTime START_DATE_TIME
        {
            get
            {
                return start_date_time;
            }
            set
            {
                start_date_time = value;
            }
        }

        private string stop_date_time;
        /// <summary>
        /// 医嘱执行结束时间
        /// yyyy-MM-dd HH:mm:ss格式
        /// </summary>
        public string STOP_DATE_TIME
        {
            get
            {
                return stop_date_time;
            }
            set
            {
                stop_date_time = value;
            }
        }

        private string repeat_indicator="1";
        /// <summary>
        /// 长期医嘱标志
        /// 本医嘱是否长期医嘱：1-长期、0-临时
        /// </summary>
        public string REPEAT_INDICATOR
        {
            get
            {
                return repeat_indicator;
            }
            set
            {
                repeat_indicator = value;
            }
        }

        private string order_class=string.Empty;
        /// <summary>
        /// 医嘱类别代码
        /// </summary>
        public string ORDER_CLASS
        {
            get
            {
                return order_class;
            }
            set
            {
                order_class = value;
            }
        }

        private string order_class_name;
        /// <summary>
        /// 医嘱类别名称
        /// 如：饮食医嘱、肠内肠外制剂
        /// </summary>
        public string ORDER_CLASS_NAME
        {
            get
            {
                return order_class_name;
            }
            set
            {
                order_class_name = value;
            }
        }

        private string order_code=string.Empty;
        /// <summary>
        /// 医嘱编码
        /// </summary>
        public string ORDER_CODE
        {
            get
            {
                return order_code;
            }
            set
            {
                order_code = value;
            }
        }

        private string order_text=string.Empty;
        /// <summary>
        /// 医嘱正文 医嘱内容(如：普食)
        /// </summary>
        public string ORDER_TEXT
        {
            get
            {
                return order_text;
            }
            set
            {
                order_text = value;
            }
        }

        private string order_status=string.Empty;
        /// <summary>
        /// 医嘱状态
        /// 反映医嘱的执行状态，如新开、校对、执行、停止等
        /// </summary>
        public string ORDER_STATUS
        {
            get
            {
                return order_status;
            }
            set
            {
                order_status = value;
            }
        }

        private string dosage=string.Empty;
        /// <summary>
        /// 药品一次使用剂量
        /// </summary>
        public string DOSAGE
        {
            get
            {
                return dosage;
            }
            set
            {
                dosage = value;
            }
        }

        private string dosage_units=string.Empty;
        /// <summary>
        /// 剂量单位
        /// </summary>
        public string DOSAGE_UNITS
        {
            get
            {
                return dosage_units;
            }
            set
            {
                dosage_units = value;
            }
        }

        private string duration;
        /// <summary>
        /// 持续时间
        /// </summary>
        public string DURATION
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        private string duration_units=string.Empty;
        /// <summary>
        /// 持续时间单位
        /// </summary>
        public string DURATION_UNITS
        {
            get
            {
                return duration_units;
            }
            set
            {
                duration_units = value;
            }
        }

        private string freq_counter;
        /// <summary>
        /// 频率次数 执行频率的次数部分
        /// </summary>
        public string FREQ_COUNTER
        {
            get
            {
                return freq_counter;
            }
            set
            {
                freq_counter = value;
            }
        }

        private string freq_interval=string.Empty;

        /// <summary>
        /// 频率间隔 执行频率的间隔部分
        /// </summary>
        public string FREQ_INTERVAL
        {
            get
            {
                return freq_interval;
            }
            set
            {
                freq_interval = value;
            }
        }

        private string freq_interval_unit=string.Empty;
        /// <summary>
        /// 频率间隔单位
        /// </summary>
        public string FREQ_INTERVAL_UNIT
        {
            get
            {
                return freq_interval_unit;
            }
            set
            {
                freq_interval_unit = value;
            }
        }

        private string freq_detail=string.Empty;
        /// <summary>
        /// 执行时间详细描述
        ///医嘱执行的详细时间表，用于对执行频率的补充，如：执行频率为3/日，补充为饭前执行或直接指定时间
        /// </summary>
        public string FREQ_DETAIL
        {
            get
            {
                return freq_detail;
            }
            set
            {
                freq_detail = value;
            }
        }

        private DateTime perform_schedule=System.DateTime.Now;
        /// <summary>
        /// 护士执行时间
        /// </summary>
        public DateTime PERFORM_SCHEDULE
        {
            get
            {
                return perform_schedule;
            }
            set
            {
                perform_schedule = value;
            }
        }

        private string perform_result= string.Empty;
        /// <summary>
        /// 执行结果
        /// </summary>
        public string PERFORM_RESULT
        {
            get
            {
                return perform_result;
            }
            set
            {
                perform_result = value;
            }
        }

        private string ordering_dept = string.Empty;
        /// <summary>
        /// 开医嘱科室
        /// </summary>
        public string ORDERING_DEPT
        {
            get
            {
                return ordering_dept;
            }
            set
            {
                ordering_dept = value;
            }
        }

        private string doctor = string.Empty;
        /// <summary>
        /// 开医嘱医生
        /// 医生姓名
        /// </summary>
        public string DOCTOR
        {
            get
            {
                return doctor;
            }
            set
            {
                doctor = value;
            }
        }

        private string stop_doctor = string.Empty;
        /// <summary>
        /// 停医嘱医生
        /// </summary>
        public string STOP_DOCTOR
        {
            get
            {
                return stop_doctor;
            }
            set
            {
                stop_doctor = value;
            }
        }

        private string nurse = string.Empty;
        /// <summary>
        /// 开医嘱校对护士
        /// </summary>
        public string NURSE
        {
            get
            {
                return nurse;
            }
            set
            {
                nurse = value;
            }
        }

        private string stop_nurse = string.Empty;
        /// <summary>
        /// 停医嘱校对护士
        /// </summary>
        public string STOP_NURSE
        {
            get
            {
                return stop_nurse;
            }
            set
            {
                stop_nurse = value;
            }
        }

        private DateTime enter_date_time = System.DateTime.Now;
        /// <summary>
        /// 开医嘱录入日期及时间
        /// </summary>
        public DateTime ENTER_DATE_TIME
        {
            get
            {
                return enter_date_time;
            }
            set
            {
                enter_date_time = value;
            }
        }

        private string stop_order_date_time = string.Empty;
        /// <summary>
        /// 停医嘱录入日期及时间
        /// </summary>
        public string STOP_ORDER_DATE_TIME
        {
            get
            {
                return stop_order_date_time;
            }
            set
            {
                stop_order_date_time = value;
            }
        }
    }
}
