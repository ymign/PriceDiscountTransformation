using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.EMPI
{
    //add by allan 20160726 EMPI接口内容
    /// <summary>
    /// 病人信息
    /// </summary>
    public class PATIENTINFO
    {
        public PATIENTINFO()
        {
            PATIENT = new PATIENT();
            CARDINFOS = new List<CARD>();
        }
        /// <summary>
        /// 病人信息
        /// </summary>
        public PATIENT PATIENT { get; set; }
        /// <summary>
        /// 卡信息
        /// </summary>
        public List<CARD> CARDINFOS { get; set; }
        /// <summary>
        /// 001
        /// </summary>
        public string DOMAIN { get; set; }
    }
}
