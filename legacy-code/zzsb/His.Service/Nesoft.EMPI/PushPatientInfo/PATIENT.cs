using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.EMPI.PushPatientInfo
{
    /// <summary>
    /// 修改后的病人信息
    /// </summary>
    public class PATIENT : EMPI.PATIENT
    {
        /// <summary>
        /// 卡号（门诊号、住院号、体检流水号）唯一 
        /// </summary>
        public string CARDNO { get; set; }
        /// <summary>
        /// 卡类型（门诊O住院I体检T） 
        /// </summary>
        public string CARDTYPE { get; set; }
        /// <summary>
        /// 患者类型
        /// </summary>
        public string PATIENTTYPE { get; set; }
        /// <summary>
        /// EMPI号
        /// </summary>
        public string EMPINO { get; set; }
        /// <summary>
        /// 操作人编码 
        /// </summary>
        public string OPERCODE { get; set; }
        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OPERNAME { get; set; }
        /// <summary>
        /// 新增N 更新U 删除D（可以作为判断是否修改标志）
        /// </summary>
        public string DEVOTE { get; set; }
        /// <summary>
        /// 备用1
        /// </summary>
        public string NOTE1 { get; set; }
        /// <summary>
        /// 邮编2
        /// </summary>
        public string NOTE2 { get; set; }
        /// <summary>
        /// 邮编3
        /// </summary>
        public string NOTE3 { get; set; }
      
    }
}
