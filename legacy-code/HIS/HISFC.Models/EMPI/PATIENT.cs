using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.EMPI
{
    //add by allan 20160726 EMPI接口内容
    /// <summary>
    /// 病人基本信息
    /// </summary>
    public class PATIENT
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string NAME { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string SEX { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BIRTHDAY { get; set; }
        /// <summary>
        /// 国籍代码
        /// </summary>
        public string CNY { get; set; }
        /// <summary>
        /// 国家名
        /// </summary>
        public string CNYNAME { get; set; }
        /// <summary>
        /// 户籍代码
        /// </summary>
        public string ACT { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string ZPCODE { get; set; }
        /// <summary>
        /// 血型
        /// </summary>
        public string ABOBLD { get; set; }
        /// <summary>
        /// RH血型
        /// </summary>
        public string RHBLD { get; set; }
        /// <summary>
        /// 民族编码
        /// </summary>
        public string NTN { get; set; }
        /// <summary>
        /// 出生地
        /// </summary>
        public string BCP { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string CTOR { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string CTORTEL { get; set; }
        /// <summary>
        /// 联系人关系
        /// </summary>
        public string CTORLTN { get; set; }
        /// <summary>
        /// 家庭电话
        /// </summary>
        public string HMTEL { get; set; }
        /// <summary>
        /// 患者手机号码
        /// </summary>
        public string MOBILE { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string EML { get; set; }
        /// <summary>
        /// 患者工作单位
        /// </summary>
        public string CPY { get; set; }
        /// <summary>
        /// 患者单位电话
        /// </summary>
        public string CPYTEL { get; set; }
        /// <summary>
        /// 患者婚姻状况
        /// </summary>
        public string MRG { get; set; }
        /// <summary>
        /// 患者职业代码
        /// </summary>
        public string PFSN { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string MEMO { get; set; }
    }
}
