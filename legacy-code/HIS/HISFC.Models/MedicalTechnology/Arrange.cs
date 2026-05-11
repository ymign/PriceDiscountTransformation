using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTechnology
{
    public class Arrange : Neusoft.FrameWork.Models.NeuObject, Neusoft.HISFC.Models.Base.IValid
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime SeeDate { get; set; }
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 部位代码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 部位名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 医生工号
        /// </summary>
        public string DoctCode { get; set; }
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string DoctName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 时间段(开始时间到结束时间区间)
        /// </summary>
        public string ArrangeTime
        {
            get { return BeginTime.ToString("HH:mm") + " - " + EndTime.ToString("HH:mm"); }
        }
        /// <summary>
        /// 预约限额
        /// </smmary>
        public int BookLimit { get; set; }
        /// <summary>
        /// 住院预约限额
        /// </summary>
        public int HostLimit { get; set; }
        /// <summary>
        /// 已预约数量
        /// </summary>
        public int BookNum { get; set; }
        /// <summary>
        /// 住院已预约数量
        /// </summary>
        public int HostNum { get; set; }
        /// <summary>
        /// 是否停班
        /// </summary>
        public bool IsStop { get; set; }
        /// <summary>
        /// 停班原因
        /// </summary>
        public string StopReason { get; set; }
        /// <summary>
        /// 停诊日期
        /// </summary>
        public DateTime StopDate { get; set; }
        /// <summary>
        /// 停诊人ID
        /// </summary>
        public string StopOper { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string OperCode { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        public DayOfWeek Week { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
    }
}
