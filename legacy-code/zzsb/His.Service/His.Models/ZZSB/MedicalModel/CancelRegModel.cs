using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB.MedicalModel
{
    public class CancelRegModel
    {
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string MdtrtId { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string PsnNo { get; set; }
        /// <summary>
        /// 住院/门诊号
        /// </summary>
        public string IptOtpNo { get; set; }
        /// <summary>
        /// 收费批次号
        /// </summary>
        public string ChrgBchno { get; set; }
        /// <summary>
        /// 结算ID
        /// </summary>
        public string SetlId { get; set; }
        /// <summary>
        /// 就医地医保区划
        /// </summary>
        public string MdtrtareaAdmvs { get; set; }
        /// <summary>
        /// 参保地医保区划
        /// </summary>
        public string InsuplcAdmdvs { get; set; }

        /// <summary>
        /// 医院编码
        /// </summary>
        public string FixmedinsCode { get; set; }

        /// <summary>
        /// 医院名称
        /// </summary>
        public string FixmedinsName { get; set; }
    }
}
