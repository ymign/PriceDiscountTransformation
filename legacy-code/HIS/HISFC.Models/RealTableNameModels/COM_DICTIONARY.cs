using System;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.RealTableNameModels
{
    ///<summary>
    ///公共_常数表
    ///</summary>
    public  class COM_DICTIONARY
    {
           public COM_DICTIONARY(){


           }
           /// <summary>
           /// Desc:常数类型 银行BANK 床等级BEDGRADE 减免类型DERATEFEETYPE  国家COUNTRY,民族NATION,职业PROFESSION 关系RELATIVE 职务POSITION 政治面貌POLITICS 职级LEVEL 地区ARAE 员工状态EMPLSTATUS 学历EDUCATION 用法USAGE 使用方式USWAY	                	挂号级别（REGTYPE),籍贯(DIST),	                	转归(ZG)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TYPE {get;set;}

           /// <summary>
           /// Desc:编码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CODE {get;set;}

           /// <summary>
           /// Desc:名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string NAME {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MARK {get;set;}

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
           /// Desc:输入
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string INPUT_CODE {get;set;}

           /// <summary>
           /// Desc:顺序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal SORT_ID {get;set;}

           /// <summary>
           /// Desc:有效性标志 0 在用 1 停用 2 废弃
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string VALID_STATE {get;set;}

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
           /// Desc:是否常用
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS_COMMON {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string KIND_ID {get;set;}

           /// <summary>
           /// Desc:父级医疗机构编码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PARENT_CODE {get;set;}

           /// <summary>
           /// Desc:本机医疗机构编码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_CODE {get;set;}

    }
}
