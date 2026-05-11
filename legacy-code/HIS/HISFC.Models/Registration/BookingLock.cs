using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Registration
{
    public class BookingLock
    {
        /// <summary>
        /// 号源锁定ID
        /// </summary>
        public string LockID { get; set; }
        /// <summary>
        /// 科室ID
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 医生代码
        /// </summary>
        public string DoctorID { get; set; }
        /// <summary>
        /// 出诊日期
        /// </summary>
        public DateTime RegDate { get; set; }
        /// <summary>
        /// 时段/午别
        /// </summary>
        public TimeFlag TimeFlag { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 预约类型
        /// </summary>
        public OrderType OrderType { get; set; }
        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal RegFee { get; set; }
        /// <summary>
        /// 治疗费
        /// </summary>
        public decimal TreatFee { get; set; }
        /// <summary>
        /// 排班ID
        /// </summary>
        public string SchemaID { get; set; }
        /// <summary>
        /// 锁定状态
        /// </summary>
        public string LockState { get; set; }
        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperID { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperDate { get; set; }
    }
    /// <summary>
    /// 预约类型
    /// </summary>
    public enum OrderType
    {
        网络 = 0,
        电话12580 = 1,
        电话114 = 2,
        自助终端 = 3
    }

    public enum TimeFlag
    {
        上午 = 1, 下午 = 2, 晚上 = 3
    }
}
