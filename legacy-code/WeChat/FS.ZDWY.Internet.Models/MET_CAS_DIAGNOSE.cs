using System;
using System.Linq;
using System.Text;

namespace FS.ZDWY.Internet.Models
{
    ///<summary>
    ///病案患者诊断库
    ///</summary>
    public  class MET_CAS_DIAGNOSE
    {
           public MET_CAS_DIAGNOSE(){


           }
           /// <summary>
           /// Desc:住院流水号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string INPATIENT_NO {get;set;}

           /// <summary>
           /// Desc:发生序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int HAPPEN_NO {get;set;}

           /// <summary>
           /// Desc:住院诊断类型  3 并发诊断 4 感染诊断 5 损伤诊断 6 病理诊断 10 门诊诊断 11 入院诊断 14 出院诊断 15 术后诊断 16 死亡诊断 17 临床诊断 18 确诊诊断 19 术前诊断 20 住院诊断  1 主要诊断 2 其他诊断  7 过敏药 8 新生儿疾病 9 新生儿院感
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DIAG_KIND {get;set;}

           /// <summary>
           /// Desc:诊断级别
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LEVEL_CODE {get;set;}

           /// <summary>
           /// Desc:诊断分期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PERIOR_CODE {get;set;}

           /// <summary>
           /// Desc:诊断ICD码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ICD_CODE {get;set;}

           /// <summary>
           /// Desc:诊断名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DIAG_NAME {get;set;}

           /// <summary>
           /// Desc:诊断日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime DIAG_DATE {get;set;}

           /// <summary>
           /// Desc:医师代号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DOCT_CODE {get;set;}

           /// <summary>
           /// Desc:医师姓名(诊断)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DOCT_NAME {get;set;}

           /// <summary>
           /// Desc:入院日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime IN_DATE {get;set;}

           /// <summary>
           /// Desc:出院日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime OUT_DATE {get;set;}

           /// <summary>
           /// Desc:治疗情况 0 治愈1 好转 2 未愈3 死亡 4 其他
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DIAG_OUTSTATE {get;set;}

           /// <summary>
           /// Desc:第二ICD
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SECOND_ICD {get;set;}

           /// <summary>
           /// Desc:并发症类别
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SYNDROME_ID {get;set;}

           /// <summary>
           /// Desc:病理符合
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CL_PA {get;set;}

           /// <summary>
           /// Desc:是否疑诊
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DUBDIAG_FLAG {get;set;}

           /// <summary>
           /// Desc:是否主诊断
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MAIN_FLAG {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string REMARK {get;set;}

           /// <summary>
           /// Desc:操作员
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPER_CODE {get;set;}

           /// <summary>
           /// Desc:操作时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime OPER_DATE {get;set;}

           /// <summary>
           /// Desc:类别 1 医生站录入诊断  2 病案室录入诊断
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPER_TYPE {get;set;}

           /// <summary>
           /// Desc:手术标志
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPERATION_FLAG {get;set;}

           /// <summary>
           /// Desc:是否是30种疾病
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS30DISEASE {get;set;}

           /// <summary>
           /// Desc:有效标志 0 无效 1 有效
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string VALID_FLAG {get;set;}

           /// <summary>
           /// Desc:患者类别：0 门诊 1 住院
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PERSSON_TYPE {get;set;}

           /// <summary>
           /// Desc:附属诊断ICD码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SUBSIDIARY_ICDCODE {get;set;}

           /// <summary>
           /// Desc:附属诊断名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SUBSIDIARY_ICDNAME {get;set;}

           /// <summary>
           /// Desc:珠海ICD编码上传标志 0未上传 1上传
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS_ZHUHAII_CDUPLOAP {get;set;}

           /// <summary>
           /// Desc:入院病情1(1有 2 临床未确定3 情况不明 4 无)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DIAG_OUTSTATE1 {get;set;}

           /// <summary>
           /// Desc:院内诊断编码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ICD10TEMPCODE {get;set;}

           /// <summary>
           /// Desc:诊断名称前缀
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PREFIX {get;set;}

           /// <summary>
           /// Desc:诊断名称后缀
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SUFFIX {get;set;}

           /// <summary>
           /// Desc:数据来源;null/1:his,2:急诊,3:emr,4:其它
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SOURCE_FLAG {get;set;}

           /// <summary>
           /// Desc:诊断流水号,由EMR、急诊系统生成
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FLOW_ID {get;set;}

           /// <summary>
           /// Desc:发病时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime DISEASE_DATE {get;set;}

    }
}
