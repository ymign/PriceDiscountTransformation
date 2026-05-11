using System;
using System.Linq;
using System.Text;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    public  class MET_IPM_ANAESTHESIAORDER
    {
           public MET_IPM_ANAESTHESIAORDER(){


           }
           /// <summary>
           /// Desc:医嘱流水号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERSN {get;set;}

           /// <summary>
           /// Desc:操作人ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPERATORID {get;set;}

           /// <summary>
           /// Desc:操作人姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPERATORNAME {get;set;}

           /// <summary>
           /// Desc:病人住院流水号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IPID {get;set;}

           /// <summary>
           /// Desc:病人ID号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PID {get;set;}

           /// <summary>
           /// Desc:记录创建者ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CREATOR {get;set;}

           /// <summary>
           /// Desc:记录创建者名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CREATENAME {get;set;}

           /// <summary>
           /// Desc:修改时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? MODIFYTIME {get;set;}

           /// <summary>
           /// Desc:主医嘱流水号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PARENTORDERSN {get;set;}

           /// <summary>
           /// Desc:是否长期医嘱 长嘱‘Y’；临嘱‘N’
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LONGFLAG {get;set;}

           /// <summary>
           /// Desc:医嘱类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERTYPE {get;set;}

           /// <summary>
           /// Desc:医嘱分类 医嘱分类【IN：住院；JZ：急诊；OUT：出院】
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERCLASS {get;set;}

           /// <summary>
           /// Desc:医嘱代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERCODE {get;set;}

           /// <summary>
           /// Desc:医嘱内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERCONTENT {get;set;}

           /// <summary>
           /// Desc:医嘱状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERSTATUS {get;set;}

           /// <summary>
           /// Desc:医嘱开始时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? STARTTIME {get;set;}

           /// <summary>
           /// Desc:开医嘱科室代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEPTCODE {get;set;}

           /// <summary>
           /// Desc:开医嘱科室
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEPTNAME {get;set;}

           /// <summary>
           /// Desc:开医嘱病区代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WARDCODE {get;set;}

           /// <summary>
           /// Desc:开医嘱病区
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WARDNAME {get;set;}

           /// <summary>
           /// Desc:是否子医嘱
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ISSUBORDER {get;set;}

           /// <summary>
           /// Desc:提交时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? SUBMITTIME {get;set;}

           /// <summary>
           /// Desc:提交者ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SUBMITID {get;set;}

           /// <summary>
           /// Desc:提交者
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SUBMITNAME {get;set;}

           /// <summary>
           /// Desc:申请单流水号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RELATIONKEY {get;set;}

           /// <summary>
           /// Desc:频率代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FREQUENCYCODE {get;set;}

           /// <summary>
           /// Desc:频率名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FREQUENCYNAME {get;set;}

           /// <summary>
           /// Desc:医嘱属性
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TYPEPROPERTY {get;set;}

           /// <summary>
           /// Desc:执行科室代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PERFORMEDDEPTCODE {get;set;}

           /// <summary>
           /// Desc:执行科室
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PERFORMEDDEPT {get;set;}

           /// <summary>
           /// Desc:剂量
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DOSE {get;set;}

           /// <summary>
           /// Desc:剂量单位代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DOSEUNITCODE {get;set;}

           /// <summary>
           /// Desc:剂量单位
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DOSEUNIT {get;set;}

           /// <summary>
           /// Desc:价格
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? PRICE {get;set;}

           /// <summary>
           /// Desc:是否是急诊手术/紧急使用
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ISEMERGENCY {get;set;}

           /// <summary>
           /// Desc:途径代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ROUTECODE {get;set;}

           /// <summary>
           /// Desc:途径名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ROUTENAME {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CREATETIME {get;set;}

    }
}
