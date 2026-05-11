using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Registration
{

    /// <summary>
    /// 急诊系统分诊视图实体
    /// </summary>
    public class JZTriageWithoutRegModel
    {
        /// <summary>
        /// 分诊流水号
        /// </summary>
        public string TriageNum { get; set; }
        /// <summary>
        /// 分诊时间
        /// </summary>
        public string TriageTime { get; set; }
        /// <summary>
        /// 患者名称
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 患者年龄
        /// </summary>
        public string Age { get; set; }

        public string Sex { get; set; }

        public string IDCard { get; set; }
        public string Tel { get; set; }
        public string TDeptID { get; set; }
        public string TDeptName { get; set; }
    }
}
