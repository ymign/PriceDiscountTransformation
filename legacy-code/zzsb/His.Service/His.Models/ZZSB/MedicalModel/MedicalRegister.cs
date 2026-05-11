using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB.MedicalModel
{
    public class MedicalRegister : MedicalBase
    {
        /// <summary>
        /// 门诊流水号
        /// </summary>
        public string ClincCode { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 合同单位
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 排班ID(即fin_opr_schema表的ID)
        /// </summary>
        public string RegSourceID { get; set; }

        /// <summary>
        /// 就诊凭证类型
        /// </summary>
        public string MdtrtCertType { get; set; }

        /// <summary>
        /// 就诊凭证编号 (就诊凭证类型为“01”时填写电子凭证令牌，为“02”时填写身份证号，为“03”时填写社会保障卡卡号)
        /// </summary>
        public string MdtrtCertNo { get; set; }

        /// <summary>
        /// 卡识别码(就诊凭证类型为“03”时必填)
        /// </summary>
        public string CardSN { get; set; }

        /// <summary>
        /// 人员证件类型 1居民身份证（户口簿） 90社会保障卡 99其他身份证件
        /// 4 香港特区护照/港澳居民来往内地通行证
        /// 5 澳门特区护照/港澳居民来往内地通行证
        /// </summary>
        public string PsnCertType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string PsnName { get; set; }

        /// <summary>
        /// 挂号费用
        /// </summary>
        public string RegFee { get; set; }

        /// <summary>
        /// 挂号科室
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 减免方式
        /// </summary>
        public string SettlementType { get; set; }

        public MedicalSchema medicalSchema { get; set; }

        public string Insuplcadmdvs { get; set; }
    }
}
