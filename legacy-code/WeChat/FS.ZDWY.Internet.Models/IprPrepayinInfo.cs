using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    ///<summary>
    ///住院预约表
    ///</summary>
    [SugarTable("FIN_IPR_PREPAYIN")]
    public partial class IprPrepayinInfo
    {
        public IprPrepayinInfo()
        {


        }
        /// <summary>
        /// Desc:就诊卡号
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string CARD_NO { get; set; }

        /// <summary>
        /// Desc:发生序号
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public int HAPPEN_NO { get; set; }

        /// <summary>
        /// Desc:姓名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NAME { get; set; }

        /// <summary>
        /// Desc:性别
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SEX_CODE { get; set; }

        /// <summary>
        /// Desc:身份证号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string IDENNO { get; set; }

        /// <summary>
        /// Desc:生日
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? BIRTHDAY { get; set; }

        /// <summary>
        /// Desc:医疗证号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MCARD_NO { get; set; }

        /// <summary>
        /// Desc:结算类别 01-自费  02-保险 03-公费在职 04-公费退休 05-公费高干
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PAYKIND_CODE { get; set; }

        /// <summary>
        /// Desc:合同单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACT_CODE { get; set; }

        /// <summary>
        /// Desc:床号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string BED_NO { get; set; }

        /// <summary>
        /// Desc:护士站代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NURSE_CELL_CODE { get; set; }

        /// <summary>
        /// Desc:职务
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PROF_CODE { get; set; }

        /// <summary>
        /// Desc:工作单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string WORK_NAME { get; set; }

        /// <summary>
        /// Desc:工作单位电话
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string WORK_TEL { get; set; }

        /// <summary>
        /// Desc:家庭住址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOME { get; set; }

        /// <summary>
        /// Desc:家庭电话
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOME_TEL { get; set; }

        /// <summary>
        /// Desc:籍贯
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DIST { get; set; }

        /// <summary>
        /// Desc:出生地
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string BIRTH_AREA { get; set; }

        /// <summary>
        /// Desc:民族
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NATION_CODE { get; set; }

        /// <summary>
        /// Desc:联系人
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string LINKMA_NAME { get; set; }

        /// <summary>
        /// Desc:联系人电话
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string LINKMAN_TEL { get; set; }

        /// <summary>
        /// Desc:联系人地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string LINKMAN_ADD { get; set; }

        /// <summary>
        /// Desc:联系人关系
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string RELA_CODE { get; set; }

        /// <summary>
        /// Desc:婚姻状况
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARI { get; set; }

        /// <summary>
        /// Desc:国籍
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string COUN_CODE { get; set; }

        /// <summary>
        /// Desc:诊断代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DIAG_CODE { get; set; }

        /// <summary>
        /// Desc:诊断名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DIAG_NAME { get; set; }

        /// <summary>
        /// Desc:预约科室
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DEPT_CODE { get; set; }

        /// <summary>
        /// Desc:科室名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DEPT_NAME { get; set; }

        /// <summary>
        /// Desc:预约医师
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PREDOCT_CODE { get; set; }

        /// <summary>
        /// Desc:状态
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PRE_STATE { get; set; }

        /// <summary>
        /// Desc:预约日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? PRE_DATE { get; set; }

        /// <summary>
        /// Desc:操作员
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPER_CODE { get; set; }

        /// <summary>
        /// Desc:操作日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// Desc:入院押金
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? FOREGIFT { get; set; }

        /// <summary>
        /// Desc:入院指征
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INDICATIONS { get; set; }

        /// <summary>
        /// Desc:现住址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ADDRESS { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INPATIENT_NO { get; set; }

        /// <summary>
        /// Desc:电子邮箱
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOME_ZIP { get; set; }

        /// <summary>
        /// Desc:数据来源;null/1:HIS,2:急诊,3:EMR
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SOURCE_FLAG { get; set; }

        /// <summary>
        /// Desc:预约流水号,由其它系统生成
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string APPLY_NO { get; set; }

        /// <summary>
        /// Desc:日间手术 1:是,0:否
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DAY_OPERATION_FLAG { get; set; }

        /// <summary>
        /// Desc:手术操作代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPRN_OPRT_CODE { get; set; }

        /// <summary>
        /// Desc:手术操作名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPRN_OPRT_NAME { get; set; }

    }
}
