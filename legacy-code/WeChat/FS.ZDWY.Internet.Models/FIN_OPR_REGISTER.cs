using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    public class FIN_OPR_REGISTER
    {
        /// <summary>
        /// 门诊号/发票号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String CLINIC_CODE { get; set; }

        /// <summary>
        /// 交易类型,1正交易，2反交易
        /// </summary>
        ///[SugarColumn(IsPrimaryKey = true)]
        public System.String TRANS_TYPE { get; set; }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        public System.String CARD_NO { get; set; }

        /// <summary>
        /// 挂号日期
        /// </summary>
        public System.DateTime REG_DATE { get; set; }

        /// <summary>
        /// 午别
        /// </summary>
        public System.String NOON_CODE { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String IDENNO { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public System.String SEX_CODE { get; set; }

        /// <summary>
        /// 出生日
        /// </summary>
        public System.DateTime? BIRTHDAY { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String RELA_PHONE { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public System.String ADDRESS { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public System.String CARD_TYPE { get; set; }

        /// <summary>
        /// 结算类别号
        /// </summary>
        public System.String PAYKIND_CODE { get; set; }

        /// <summary>
        /// 结算类别名称
        /// </summary>
        public System.String PAYKIND_NAME { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public System.String PACT_CODE { get; set; }

        /// <summary>
        /// 合同单位名称
        /// </summary>
        public System.String PACT_NAME { get; set; }

        /// <summary>
        /// 医疗证号
        /// </summary>
        public System.String MCARD_NO { get; set; }

        /// <summary>
        /// 挂号级别
        /// </summary>
        public System.String REGLEVL_CODE { get; set; }

        /// <summary>
        /// 挂号级别名称
        /// </summary>
        public System.String REGLEVL_NAME { get; set; }

        /// <summary>
        /// 科室号
        /// </summary>
        public System.String DEPT_CODE { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public System.String DEPT_NAME { get; set; }

        /// <summary>
        /// 排班序号
        /// </summary>
        public System.String SCHEMA_NO { get; set; }

        /// <summary>
        /// 每日顺序号
        /// </summary>
        public System.Int32? ORDER_NO { get; set; }

        /// <summary>
        /// 看诊序号
        /// </summary>
        public System.Int64? SEENO { get; set; }

        /// <summary>
        /// 看诊开始时间
        /// </summary>
        public System.DateTime? BEGIN_TIME { get; set; }

        /// <summary>
        /// 看诊结束时间
        /// </summary>
        public System.DateTime? END_TIME { get; set; }

        /// <summary>
        /// 医师代号
        /// </summary>
        public System.String DOCT_CODE { get; set; }

        /// <summary>
        /// 医师姓名
        /// </summary>
        public System.String DOCT_NAME { get; set; }

        /// <summary>
        /// 挂号收费标志 1是/0否
        /// </summary>
        public System.String YNREGCHRG { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public System.String INVOICE_NO { get; set; }

        /// <summary>
        /// 处方号
        /// </summary>
        public System.String RECIPE_NO { get; set; }

        /// <summary>
        /// 0现场挂号/1预约挂号/2特诊挂号
        /// </summary>
        public System.String YNBOOK { get; set; }

        /// <summary>
        /// 1初诊/0复诊
        /// </summary>
        public System.String YNFR { get; set; }

        /// <summary>
        /// 1加号/0正常
        /// </summary>
        public System.String APPEND_FLAG { get; set; }

        /// <summary>
        /// 挂号费
        /// </summary>
        public System.Single? REG_FEE { get; set; }

        /// <summary>
        /// 检查费
        /// </summary>
        public System.Single? CHCK_FEE { get; set; }

        /// <summary>
        /// 诊察费
        /// </summary>
        public System.Single? DIAG_FEE { get; set; }

        /// <summary>
        /// 附加费
        /// </summary>
        public System.Single? OTH_FEE { get; set; }

        /// <summary>
        /// 自费金额
        /// </summary>
        public System.Single? OWN_COST { get; set; }

        /// <summary>
        /// 报销金额
        /// </summary>
        public System.Single? PUB_COST { get; set; }

        /// <summary>
        /// 自付金额
        /// </summary>
        public System.Single? PAY_COST { get; set; }

        /// <summary>
        /// 0退费,1有效,2作废
        /// </summary>
        public System.String VALID_FLAG { get; set; }

        /// <summary>
        /// 操作员代码
        /// </summary>
        public System.String OPER_CODE { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public System.DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// 作废人
        /// </summary>
        public System.String CANCEL_OPCD { get; set; }

        /// <summary>
        /// 作废时间
        /// </summary>
        public System.DateTime? CANCEL_DATE { get; set; }

        /// <summary>
        /// 医疗类别
        /// </summary>
        public System.String MEDICAL_TYPE { get; set; }

        /// <summary>
        /// 疾病代码
        /// </summary>
        public System.String ICD_CODE { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public System.String EXAM_CODE { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public System.DateTime? EXAM_DATE { get; set; }

        /// <summary>
        /// 0未核查/1已核查
        /// </summary>
        public System.String CHECK_FLAG { get; set; }

        /// <summary>
        /// 核查人
        /// </summary>
        public System.String CHECK_OPCD { get; set; }

        /// <summary>
        /// 核查时间
        /// </summary>
        public System.DateTime? CHECK_DATE { get; set; }

        /// <summary>
        /// 1已日结/0未日结
        /// </summary>
        public System.String BALANCE_FLAG { get; set; }

        /// <summary>
        /// 日结标识号
        /// </summary>
        public System.String BALANCE_NO { get; set; }

        /// <summary>
        /// 日结人
        /// </summary>
        public System.String BALANCE_OPCD { get; set; }

        /// <summary>
        /// 日结时间
        /// </summary>
        public System.DateTime? BALANCE_DATE { get; set; }

        /// <summary>
        /// 是否看诊 1是/0否
        /// </summary>
        public System.String YNSEE { get; set; }

        /// <summary>
        /// 看诊日期
        /// </summary>
        public System.DateTime? SEE_DATE { get; set; }

        /// <summary>
        /// 分诊标志,0未分/1已分
        /// </summary>
        public System.String TRIAGE_FLAG { get; set; }

        /// <summary>
        /// 分诊护士代码
        /// </summary>
        public System.String TRIAGE_OPCD { get; set; }

        /// <summary>
        /// 分诊时间
        /// </summary>
        public System.DateTime? TRIAGE_DATE { get; set; }

        /// <summary>
        /// 打印发票数量
        /// </summary>
        public System.Int16? PRINT_INVOICECNT { get; set; }

        /// <summary>
        /// 看诊科室代码
        /// </summary>
        public System.String SEE_DPCD { get; set; }

        /// <summary>
        /// 看诊医生代码
        /// </summary>
        public System.String SEE_DOCD { get; set; }

        /// <summary>
        /// 患者来源
        /// </summary>
        public System.String IN_SOURCE { get; set; }

        /// <summary>
        /// 1：需要提取病案0：不需要提取病案
        /// </summary>
        public System.String IS_SENDINHOSCASE { get; set; }

        /// <summary>
        /// 是否加密姓名
        /// </summary>
        public System.String IS_ENCRYPTNAME { get; set; }

        /// <summary>
        /// 密文
        /// </summary>
        public System.String NORMALNAME { get; set; }

        /// <summary>
        /// 开始留观日期
        /// </summary>
        public System.DateTime? IN_DATE { get; set; }

        /// <summary>
        /// 留观结束日期
        /// </summary>
        public System.DateTime? OUT_DATE { get; set; }

        /// <summary>
        /// 转归代号
        /// </summary>
        public System.String ZG { get; set; }

        /// <summary>
        /// N 正常挂号 R 留观登记 I 正在留观 P 出观登记 B 留观出院完成 E 留观转住院登记 C 留观转住院完成
        /// </summary>
        public System.String IN_STATE { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public System.Double? ECO_COST { get; set; }

        /// <summary>
        /// 账户流程标识1 账户挂号 0普通
        /// </summary>
        public System.String IS_ACCOUNT { get; set; }

        /// <summary>
        /// 是否急诊号
        /// </summary>
        public System.String IS_EMERGENCY { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public System.String MARK1 { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        public System.Double? HEIGHT { get; set; }

        /// <summary>
        /// 体重
        /// </summary>
        public System.Double? WEIGHT { get; set; }

        /// <summary>
        /// 收缩压
        /// </summary>
        public System.Double? SBP { get; set; }

        /// <summary>
        /// 舒张压
        /// </summary>
        public System.Double? DBP { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        public System.Single? TEMPERATURE { get; set; }

        /// <summary>
        /// 上传标记
        /// </summary>
        public System.String UP_FLAG { get; set; }

        /// <summary>
        /// 血糖
        /// </summary>
        public System.Double? BLOODGLU { get; set; }

        /// <summary>
        /// 当前使用卡类型
        /// </summary>
        public System.String CURRENT_CARDTYPE { get; set; }

        /// <summary>
        /// 当前使用卡号
        /// </summary>
        public System.String CURRENT_CARD { get; set; }

        /// <summary>
        /// 登记次数
        /// </summary>
        public System.Single? IN_TIMES { get; set; }

        /// <summary>
        /// EMR挂号流水号 SEQ_FIN_REGID
        /// </summary>
        public System.Int64? EMR_REGID { get; set; }

        /// <summary>
        /// 患者类别（普通、VIP、特诊等） 常数PersonType
        /// </summary>
        public System.String PATIENT_TYPE { get; set; }

        /// <summary>
        /// 诊金登记单号
        /// </summary>
        public System.String REG_NO { get; set; }

        /// <summary>
        /// 诊金金额
        /// </summary>
        public System.Single? REG_DIAG_FEE { get; set; }

        /// <summary>
        /// 诊金代码
        /// </summary>
        public System.String REG_DIAG_CODE { get; set; }

        /// <summary>
        /// 医生所属科室
        /// </summary>
        public System.String DOC_INDEPT { get; set; }

        /// <summary>
        /// 医院编码
        /// </summary>
        public System.String HOS_CODE { get; set; }

        /// <summary>
        /// 绿色通道
        /// </summary>
        public System.String GREENWAY { get; set; }

        /// <summary>
        /// 评估状态，内镜中心用（1,待评估，2评估完成）
        /// </summary>
        //public System.String ASSESSS_FLAG { get; set; }
        public string SOURCE_FLAG { get; set; }
    }
}
