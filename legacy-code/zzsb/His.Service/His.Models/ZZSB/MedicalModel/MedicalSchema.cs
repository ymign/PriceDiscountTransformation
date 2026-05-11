using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB.MedicalModel
{
    /// <summary>
    /// 新医保排班信息表
    /// </summary>
    public class MedicalSchema
    {
        /// <summary>
        /// 排班id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 科室编码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 看诊医生编码
        /// </summary>
        public string DoctCode { get; set; }

        /// <summary>
        /// 看诊医生名称
        /// </summary>
        public string DoctName { get; set; }

        /// <summary>
        /// 挂号级别
        /// </summary>
        public string ReglevlCode { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
    }
}
