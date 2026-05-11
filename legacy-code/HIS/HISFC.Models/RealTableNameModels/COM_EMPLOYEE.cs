using System;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.RealTableNameModels
{
    ///<summary>
    ///员工代码表
    ///</summary>
    public  class COM_EMPLOYEE
    {
           public COM_EMPLOYEE(){


           }
           /// <summary>
           /// Desc:员工代码
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string EMPL_CODE {get;set;}

           /// <summary>
           /// Desc:工资号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SALARY_ID {get;set;}

           /// <summary>
           /// Desc:员工姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EMPL_NAME {get;set;}

           /// <summary>
           /// Desc:拼音码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SPELL_CODE {get;set;}

           /// <summary>
           /// Desc:五笔
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WB_CODE {get;set;}

           /// <summary>
           /// Desc:性别
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SEX_CODE {get;set;}

           /// <summary>
           /// Desc:出生日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime BIRTHDAY {get;set;}

           /// <summary>
           /// Desc:职务代号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string POSI_CODE {get;set;}

           /// <summary>
           /// Desc:职级代号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LEVL_CODE {get;set;}

           /// <summary>
           /// Desc:学历
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EDUCATION_CODE {get;set;}

           /// <summary>
           /// Desc:身份证号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IDENNO {get;set;}

           /// <summary>
           /// Desc:所属科室号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEPT_CODE {get;set;}

           /// <summary>
           /// Desc:所属护理站
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string NURSE_CELL_CODE {get;set;}

           /// <summary>
           /// Desc:人员类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EMPL_TYPE {get;set;}

           /// <summary>
           /// Desc:是否专家
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EXPERT_FLAG {get;set;}

           /// <summary>
           /// Desc:是否有修改票据权限 1允许 0不允许
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MODIFY_FLAG {get;set;}

           /// <summary>
           /// Desc:不挂号就收费权限 0 不允许 1允许
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string NOREGFEE_FLAG {get;set;}

           /// <summary>
           /// Desc:有效性标志 1 有效 0 停用 2 废弃
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string VALID_STATE {get;set;}

           /// <summary>
           /// Desc:顺序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? SORT_ID {get;set;}

           /// <summary>
           /// Desc:扩展标志
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EXT_FLAG {get;set;}

           /// <summary>
           /// Desc:扩展标志1
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EXT1_FLAG {get;set;}

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
           /// Desc:自定义码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string USER_CODE {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string REMARK {get;set;}

           /// <summary>
           /// Desc:推送标识 0-未推送 1-新增 3-删除
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SEND_FLAG {get;set;}

           /// <summary>
           /// Desc:职工状态类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EMPSTATE {get;set;}

    }
}
