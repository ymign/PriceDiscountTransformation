using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.RADT
{
    public class INPATIENTREGIST
    {
        public INPATIENTREGIST()
        {


        }
        /// <summary>
        /// Desc:名字
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NAME { get; set; }

        /// <summary>
        /// Desc:医保号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SSN { get; set; }

        /// <summary>
        /// Desc:身份证
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string IDCARD { get; set; }

        /// <summary>
        /// Desc:住院流水号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ID { get; set; }

        /// <summary>
        /// Desc:电脑号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PROCREATENO { get; set; }

        /// <summary>
        /// Desc:住院号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PATIENTNO { get; set; }

        /// <summary>
        /// Desc:门诊卡号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CARDNO { get; set; }

        /// <summary>
        /// Desc:入院日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? INTIME { get; set; }

        /// <summary>
        /// Desc:合同单位编码（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACTID { get; set; }

        /// <summary>
        /// Desc:合同单位名称（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACTNAME { get; set; }

        /// <summary>
        /// Desc:性别（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SEX { get; set; }

        /// <summary>
        /// Desc:民族（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NATIONALITY { get; set; }

        /// <summary>
        /// Desc:生日
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? BIRTHDAY { get; set; }

        /// <summary>
        /// Desc:科室编码（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DEPTID { get; set; }

        /// <summary>
        /// Desc:工作单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string COMPANYNAME { get; set; }

        /// <summary>
        /// Desc:婚姻状况（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARITALSTATUS { get; set; }

        /// <summary>
        /// Desc:籍贯（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DIST { get; set; }

        /// <summary>
        /// Desc:出生地
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string AREACODE { get; set; }

        /// <summary>
        /// Desc:国籍ID（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string COUNTRY { get; set; }

        /// <summary>
        /// Desc:职位ID（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PROFESSION { get; set; }

        /// <summary>
        /// Desc:联系人姓名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string KINNAME { get; set; }

        /// <summary>
        /// Desc:联系人电话
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string KINRELATIONPHONE { get; set; }

        /// <summary>
        /// Desc:与患者关系（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string KINRELATION { get; set; }

        /// <summary>
        /// Desc:联系人地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string KINRELATIONADDRESS { get; set; }

        /// <summary>
        /// Desc:家庭地址邮编
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOMEZIP { get; set; }

        /// <summary>
        /// Desc:现住址（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NOWADDR { get; set; }

        /// <summary>
        /// Desc:现住址[街道}
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NOWADD { get; set; }

        /// <summary>
        /// Desc:户口地址（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOMEADDR { get; set; }

        /// <summary>
        /// Desc:户口地址{街道}
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOMEADD { get; set; }

        /// <summary>
        /// Desc:患者电话
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PHONEHOME { get; set; }

        /// <summary>
        /// Desc:单位电话
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PHONEBUSINESS { get; set; }

        /// <summary>
        /// Desc:入院途径（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ADMITSOURCE { get; set; }

        /// <summary>
        /// Desc:入院来源（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INSOURCE { get; set; }

        /// <summary>
        /// Desc:入院情况（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CIRCS { get; set; }

        /// <summary>
        /// Desc:收住医师（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DOCTORRECEIVER { get; set; }

        /// <summary>
        /// Desc:门诊诊断
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CLINICDIAGNOSE { get; set; }

        /// <summary>
        /// Desc:门诊诊断编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CLINICDIAGNOSENO { get; set; }

        /// <summary>
        /// Desc:是否日间手术标记（字典）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DAYOPERATIONFLAG { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime OPER_DATE { get; set; }
    }
}
