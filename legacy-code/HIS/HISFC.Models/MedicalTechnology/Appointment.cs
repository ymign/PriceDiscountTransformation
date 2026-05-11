using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTechnology
{
    public class Appointment : Neusoft.FrameWork.Models.NeuObject, Neusoft.HISFC.Models.Base.IValid
    {
        /// <summary>
        /// 病历号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 门诊/住院号
        /// </summary>
        public string ClinicCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }
        /// <summary>
        /// 到达方式
        /// </summary>
        public string ArrivalPattern { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 床号
        /// </summary>
        public string BedNo { get; set; }
        /// <summary>
        /// 医嘱单项目流水号
        /// </summary>
        public string SequenceNo { get; set; }
        /// <summary>
        /// 申请项目编号
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 申请项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项目费用
        /// </summary>
        public decimal ItemCost { get; set; }
        /// <summary>
        /// 医技项目编号
        /// </summary>
        public string MTCode { get; set; }
        /// <summary>
        /// 医技项目名称
        /// </summary>
        public string MTName { get; set; }
        /// <summary>
        /// 医技类型编号
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 医技类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public EnumApplyStatus ApplyStatus { get; set; }
        /// <summary>
        /// 医技排班序号
        /// </summary>
        public string ArrangeID { get; set; }
        /// <summary>
        /// 预约日期
        /// </summary>
        public DateTime ArrangeDate { get; set; }
        /// <summary>
        /// 预约星期
        /// </summary>
        public DayOfWeek ArrangeWeek { get { return ArrangeDate.DayOfWeek; } }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 预约时间段
        /// </summary>
        public string ArrangeTime { get { return BeginTime.ToString("HH:mm") + " - " + EndTime.ToString("HH:mm"); } }
        /// <summary>
        /// 预约类型
        /// </summary>
        public EnumApplyType OrderType { get; set; }
        /// <summary>
        /// 每日预约序号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 病史及体征
        /// </summary>
        public string MedicalHistory { get; set; }
        /// <summary>
        /// 诊断
        /// </summary>
        public string Diagnosis { get; set; }
        /// <summary>
        /// 执行科室代码
        /// </summary>
        public string ExecDeptCode { get; set; }
        /// <summary>
        /// 执行科室
        /// </summary>
        public string ExecDeptName { get; set; }
        /// <summary>
        /// 执行医生工号
        /// </summary>
        public string ExecDoctCode { get; set; }
        /// <summary>
        /// 执行医生
        /// </summary>
        public string ExecDoctName { get; set; }
        /// <summary>
        /// 开立科室代码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 开立科室
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 开立医生工号
        /// </summary>
        public string DoctCode { get; set; }
        /// <summary>
        /// 开立医生姓名
        /// </summary>
        public string DoctName { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime ApplyDate { get; set; }
        private Neusoft.HISFC.Models.Base.CancelTypes cancleFlag = Base.CancelTypes.Valid;
        /// <summary>
        /// 取消标志
        /// </summary>
        public Neusoft.HISFC.Models.Base.CancelTypes CancleFlag { get { return this.cancleFlag; } set { this.cancleFlag = value; } }
        /// <summary>
        /// 被改约的预约ID
        /// </summary>
        public string CancleAppointment { get; set; }
        /// <summary>
        /// 操作员工号
        /// </summary>
        public string OperCode { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperDate { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
    }
}
