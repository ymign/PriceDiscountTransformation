using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.MedicalTechnology
{
    public class Templet : Neusoft.FrameWork.Models.NeuObject, IValid
    {
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项目部位
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 项目名称
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
        /// 预约限额
        /// </summary>
        public int BookLimit { get; set; }
        /// <summary>
        /// 住院预约限额
        /// </summary>
        public int HostLimit { get; set; }
        /// <summary>
        /// 是否停班
        /// </summary>
        public bool IsStop { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 停班原因
        /// </summary>
        public string StopReason { get; set; }
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
