using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.NuerseWork
{
    /// <summary>
    /// 病案报表专用实体
    /// </summary>
 
    [Serializable]
    public class NuerseWork : Neusoft.FrameWork.Models.NeuObject
    {
        public NuerseWork()
        {
        }
        /// <summary>
        /// 日期
        /// </summary>
        protected string data_Date;
        /// <summary>
        /// 编码
        /// </summary>
        private string data_Code="";
        /// <summary>
        /// 名称
        /// </summary>
        protected string data_Name;
        /// <summary>
        /// 数量
        /// </summary>
        protected decimal data_Num;
        /// <summary>
        /// 科室编码
        /// </summary>
        protected string dept_Code;
        /// <summary>
        /// 操作人
        /// </summary>
        protected string oper_Code;
        /// <summary>
        /// 操作时间
        /// </summary>
        protected string oper_Date;
        /// <summary>
        /// 是否解锁
        /// </summary>
        protected string valid_State;
        /// <summary>
        /// 是否月报
        /// </summary>
        protected string is_State;
        /// <summary>
        /// 月报数据名称
        /// </summary>
        protected string month_data;
        /// <summary>
        /// 备注
        /// </summary>
        protected string note;
        /// <summary>
        /// 备注1
        /// </summary>
        protected string note1;
        /// <summary>
        /// 日期
        /// </summary>
        protected string op_date;
        /// <summary>
        /// 科别
        /// </summary>
        protected string dept_name;
        /// <summary>
        /// 医生编码
        /// </summary>
        protected string doc_code;
        /// <summary>
        /// 医生姓名
        /// </summary>
        protected string doc_name;
        /// <summary>
        /// 手术名称
        /// </summary>
        protected string operation_name;
        /// <summary>
        /// 手术数量
        /// </summary>
        protected string operation;
        /// <summary>
        /// 工时
        /// </summary>
        protected string work_hours;
        /// <summary>
        /// 合计
        /// </summary>
        protected string all_tot;
        /// <summary>
        /// 出车
        /// </summary>
        protected string out_car;
        /// <summary>
        /// 抢救人数
        /// </summary>
        protected string save;
        /// <summary>
        /// 抢救成功人数
        /// </summary>
        protected string saved;
        /// <summary>
        /// 死亡人数
        /// </summary>
        protected string death;
        /// <summary>
        /// 死亡姓名
        /// </summary>
        protected string death_name;
        /// <summary>
        /// 院前/院后死亡
        /// </summary>
        protected string death_about;
        /// <summary>
        /// 急诊手术例数
        /// </summary>
        protected string eme_ops;
        /// <summary>
        /// 序号
        /// </summary>
        protected string soid_id;
        /// <summary>
        /// herp对照科室编码字段
        /// </summary>
        protected string hos_code;
        /// <summary>
        /// herp对照科室编码1字段
        /// </summary>
        protected string dept_code2;

        /// <summary>
        /// herp对照科室名称1字段
        /// </summary>
        protected string dept_name2;



        /// <summary>
        /// 日期
        /// </summary>
        public string Data_date
        {
            get
            {
                return this.data_Date;
            }
            set
            {
                this.data_Date = value;
            }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string Data_code
        {
            get
            {
                return this.data_Code;
            }
            set
            {
                this.data_Code = value;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Data_name
        {
            get
            {
                return this.data_Name;
            }
            set
            {
                this.data_Name = value;
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Data_num
        {
            get
            {
                return this.data_Num;
            }
            set
            {
                this.data_Num = value;
            }
        }

        /// <summary>
        /// 科室编码
        /// </summary>
        public string Dept_code
        {
            get
            {
                return this.dept_Code;
            }
            set
            {
                this.dept_Code = value;
            }
        }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Oper_code
        {
            get
            {
                return this.oper_Code;
            }
            set
            {
                this.oper_Code = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        public string Oper_date
        {
            get
            {
                return this.oper_Date;
            }
            set
            {
                this.oper_Date = value;
            }
        }

        /// <summary>
        /// 解锁状态
        /// </summary>
        public string Valid_state
        {
            get
            {
                return this.valid_State;
            }
            set
            {
                this.valid_State = value;
            }
        }

        /// <summary>
        ///是否月报
        /// </summary>
        public string Is_state
        {
            get
            {
                return this.is_State;
            }
            set
            {
                this.is_State = value;
            }
        }

        /// <summary>
        ///备用
        /// </summary>
        public string Month_data
        {
            get
            {
                return this.month_data;
            }
            set
            {
                this.month_data = value;
            }
        }

        /// <summary>
        ///备用
        /// </s1ummary>
        public string Note
        {
            get
            {
                return this.note;
            }
            set
            {
                this.note = value;
            }
        }

        /// <summary>
        ///备用1
        /// </s1ummary>
        public string Note1
        {
            get
            {
                return this.note1;
            }
            set
            {
                this.note1 = value;
            }
        }

        /// <summary>
        ///日期
        /// </s1ummary>
        public string Op_date
        {
            get
            {
                return this.op_date;
            }
            set
            {
                this.op_date = value;
            }
        }

        /// <summary>
        ///科别
        /// </s1ummary>
        public string Dept_Name
        {
            get
            {
                return this.dept_name;
            }
            set
            {
                this.dept_name = value;
            }
        }

        /// <summary>
        ///医生工号
        /// </s1ummary>
        public string Doc_code
        {
            get
            {
                return this.doc_code;
            }
            set
            {
                this.doc_code = value;
            }
        }

        /// <summary>
        ///医生姓名
        /// </s1ummary>
        public string Doc_name
        {
            get
            {
                return this.doc_name;
            }
            set
            {
                this.doc_name = value;
            }
        }

        /// <summary>
        ///手术名称
        /// </s1ummary>
        public string Operation_Name
        {
            get
            {
                return this.operation_name;
            }
            set
            {
                this.operation_name = value;
            }
        }

        /// <summary>
        ///手术数量
        /// </s1ummary>
        public string Operation
        {
            get
            {
                return this.operation;
            }
            set
            {
                this.operation = value;
            }
        }

        /// <summary>
        ///工时
        /// </s1ummary>
        public string Work_Hours
        {
            get
            {
                return this.work_hours;
            }
            set
            {
                this.work_hours = value;
            }
        }

        /// <summary>
        ///合计
        /// </s1ummary>
        public string All_Tot
        {
            get
            {
                return this.all_tot;
            }
            set
            {
                this.all_tot = value;
            }
        }

        /// <summary>
        ///出车
        /// </s1ummary>
        public string Out_Car
        {
            get
            {
                return this.out_car;
            }
            set
            {
                this.out_car = value;
            }
        }

        /// <summary>
        ///抢救人数
        /// </s1ummary>
        public string Save
        {
            get
            {
                return this.save;
            }
            set
            {
                this.save = value;
            }
        }

        /// <summary>
        ///抢救成功人数
        /// </s1ummary>
        public string Saved
        {
            get
            {
                return this.saved;
            }
            set
            {
                this.saved = value;
            }
        }

        /// <summary>
        ///死亡人数
        /// </s1ummary>
        public string Death
        {
            get
            {
                return this.death;
            }
            set
            {
                this.death = value;
            }
        }

        /// <summary>
        ///死亡姓名
        /// </s1ummary>
        public string Death_Name
        {
            get
            {
                return this.death_name;
            }
            set
            {
                this.death_name = value;
            }
        }

        /// <summary>
        ///院前/院后死亡
        /// </s1ummary>
        public string Death_About
        {
            get
            {
                return this.death_about;
            }
            set
            {
                this.death_about = value;
            }
        }

        /// <summary>
        ///急诊手术人数
        /// </s1ummary>
        public string Eme_Ops
        {
            get
            {
                return this.eme_ops;
            }
            set
            {
                this.eme_ops = value;
            }
        }

        /// <summary>
        ///序号
        /// </s1ummary>
        public string Soid_Id
        {
            get
            {
                return this.soid_id;
            }
            set
            {
                this.soid_id = value;
            }
        }

        /// <summary>
        ///herp对照科室编码字段
        /// </s1ummary>
        public string Hos_Code
        {
            get
            {
                return this.hos_code;
            }
            set
            {
                this.hos_code = value;
            }
        }

        /// <summary>
        ///herp对照科室编码1字段
        /// </s1ummary>
        public string Dept_code2
        {
            get
            {
                return this.dept_code2;
            }
            set
            {
                this.dept_code2 = value;
            }
        }

        /// <summary>
        ///herp对照科室名称2字段
        /// </s1ummary>
        public string Dept_name2
        {
            get
            {
                return this.dept_name2;
            }
            set
            {
                this.dept_name2 = value;
            }
        }

    }
}

