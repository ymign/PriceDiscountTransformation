using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Registration
{
    /// <summary>
    /// 门诊患者NCP筛查登记表
    /// </summary>
    public class RegNCP
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string CLINIC_CODE { get; set; }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string CARD_NO { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDENNO { get; set; }

        /// <summary>
        /// 性别 M男 F女
        /// </summary>
        public string SEX { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string ADDRESS { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string RELA_PHONE { get; set; }

        /// <summary>
        /// 本人电话
        /// </summary>
        public string HOMEPHONE { get; set; }

        /// <summary>
        /// 起病前14天内是否有在武汉居住 1是 0否
        /// </summary>
        public string ISINWUHAN { get; set; }

        /// <summary>
        /// 起病前14天内有无接触过武汉人或类似症状病人 1是 0否
        /// </summary>
        public string ISTOUCHWUHAN { get; set; }

        /// <summary>
        /// 起病前14天内有无接触史或农贸生鲜市场活动    0否 1是 
        /// </summary>
        public string ISTOUCHANIMAL { get; set; }

        /// <summary>
        /// 起病前14天内有无去何处旅游，居住，工作
        /// </summary>
        public string ISTOUR { get; set; }

       

        /// <summary>
        /// 临床表现表现症状  发热体温  °C
        /// </summary>
        public string SYMPTOM_TEMPERATURE { get; set; }


        /// <summary>
        /// 临床表现表现症状  皮疹 0否 1是
        /// </summary>
        public string SYMPTOM_ERYTHRA { get; set; }

        /// <summary>
        /// 临床表现表现症状  咳嗽 0否 1是
        /// </summary>
        public string SYMPTOM_COUGH { get; set; }

        /// <summary>
        /// 临床表现表现症状  呕吐   次/天
        /// </summary>
        public string SYMPTOM_VOMIT { get; set; }

        /// <summary>
        /// 临床表现表现症状  腹泻   次/天
        /// </summary>
        public string SYMPTOM_DIARRHOEA { get; set; }

        /// <summary>
        /// 临床表现表现症状  头痛 0否 1是
        /// </summary>
        public string SYMPTOM_HEADACHE { get; set; }

        /// <summary>
        /// 临床表现表现症状 其他
        /// </summary>
        public string SYMPTOM_OTHER { get; set; }

        /// <summary>
        /// 科室编码
        /// </summary>
        public string DEPT_CODE { get; set; }

        /// <summary>
        /// 操作工号
        /// </summary>
        public string OPER_CODE { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OPER_NAME { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public string OPER_DATE { get; set; }


        /// <summary>
        /// 患者流行病学史
        /// </summary>
        public string ISTOUCHWUHAN_NOTE { get; set; }

        /// <summary>
        /// 初感不适时间
        /// </summary>
        public string SYMPTOM_TIME { get; set; }


        /// <summary>
        /// 是否需要核酸检测
        /// </summary>
        public string ISNEEDHSJC { get; set; }

        /// <summary>
        /// 是否拟新入院患者   0否 1是
        /// </summary>
        public string HSJC_TYPE_NEWIN { get; set; }

        /// <summary>
        /// 拟收科室
        /// </summary>
        public string HSJC_TYPE_NEWIN_DEPT { get; set; }

        /// <summary>
        /// 拟入院时间
        /// </summary>
        public string HSJC_TYPE_NEWIN_DATE { get; set; }

        /// <summary>
        /// 是否在院患者   0否 1是
        /// </summary>
        public string HSJC_TYPE_INPATIENT { get; set; }

        /// <summary>
        /// 所在科室
        /// </summary>
        public string HSJC_TYPE_INPATIENT_DEPT { get; set; }

        /// <summary>
        /// 户籍地
        /// </summary>
        public string HOMETOWN { get; set; }


        /// <summary>
        /// 职业
        /// </summary>
        public string WORK { get; set; }

        /// <summary>
        /// 是否核酸筛查类别其他 0否 1是
        /// </summary>
        public string HSJC_TYPE_OTHER { get; set; }

        /// <summary>
        /// 是否核酸筛查类别其他内容
        /// </summary>
        public string HSJC_TYPE_OTHERTEXT { get; set; }

        public RegNCP(){}

    }
}
