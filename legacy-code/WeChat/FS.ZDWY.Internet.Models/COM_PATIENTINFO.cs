using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 病人基本信息表
    /// </summary>
    public class COM_PATIENTINFO
    {
        /// <summary>
        /// 病人基本信息表
        /// </summary>
        public COM_PATIENTINFO()
        {
        }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String CARD_NO { get; set; }

        /// <summary>
        /// 电脑号
        /// </summary>
        public System.String IC_CARDNO { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get; set; }

        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String SPELL_CODE { get; set; }

        /// <summary>
        /// 五笔
        /// </summary>
        public System.String WB_CODE { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public System.DateTime BIRTHDAY { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public System.String SEX_CODE { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String IDENNO { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        public System.String BLOOD_CODE { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        public System.String PROF_CODE { get; set; }

        /// <summary>
        /// 工作单位
        /// </summary>
        public System.String WORK_HOME { get; set; }

        /// <summary>
        /// 单位电话
        /// </summary>
        public System.String WORK_TEL { get; set; }

        /// <summary>
        /// 单位邮编
        /// </summary>
        public System.String WORK_ZIP { get; set; }

        /// <summary>
        /// 户口或家庭所在
        /// </summary>
        public System.String HOME { get; set; }

        /// <summary>
        /// 家庭电话
        /// </summary>
        public System.String HOME_TEL { get; set; }

        /// <summary>
        /// 户口或家庭邮政编码
        /// </summary>
        public System.String HOME_ZIP { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public System.String DISTRICT { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public System.String NATION_CODE { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public System.String LINKMAN_NAME { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public System.String LINKMAN_TEL { get; set; }

        /// <summary>
        /// 联系人住址
        /// </summary>
        public System.String LINKMAN_ADD { get; set; }

        /// <summary>
        /// 联系人关系
        /// </summary>
        public System.String RELA_CODE { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public System.String MARI { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        public System.String COUN_CODE { get; set; }

        /// <summary>
        /// 结算类别
        /// </summary>
        public System.String PAYKIND_CODE { get; set; }

        /// <summary>
        /// 结算类别名称
        /// </summary>
        public System.String PAYKIND_NAME { get; set; }

        /// <summary>
        /// 合同代码
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
        /// 出生地
        /// </summary>
        public System.String AREA_CODE { get; set; }

        /// <summary>
        /// 医疗费用
        /// </summary>
        public System.Double? FRAMT { get; set; }

        /// <summary>
        /// 药物过敏
        /// </summary>
        public System.String ANAPHY_FLAG { get; set; }

        /// <summary>
        /// 重要疾病
        /// </summary>
        public System.String HEPATITIS_FLAG { get; set; }

        /// <summary>
        /// 帐户密码
        /// </summary>
        public System.String ACT_CODE { get; set; }

        /// <summary>
        /// 帐户总额
        /// </summary>
        public System.Double? ACT_AMT { get; set; }

        /// <summary>
        /// 上期帐户余额
        /// </summary>
        public System.Double? LACT_SUM { get; set; }

        /// <summary>
        /// 上期银行余额
        /// </summary>
        public System.Double? LBANK_SUM { get; set; }

        /// <summary>
        /// 欠费次数
        /// </summary>
        public System.Int16? ARREAR_TIMES { get; set; }

        /// <summary>
        /// 欠费金额
        /// </summary>
        public System.Double? ARREAR_SUM { get; set; }

        /// <summary>
        /// 住院来源
        /// </summary>
        public System.String INHOS_SOURCE { get; set; }

        /// <summary>
        /// 最近住院日期
        /// </summary>
        public System.DateTime? LIHOS_DATE { get; set; }

        /// <summary>
        /// 住院次数
        /// </summary>
        public System.Int16? INHOS_TIMES { get; set; }

        /// <summary>
        /// 最近出院日期
        /// </summary>
        public System.DateTime? LOUTHOS_DATE { get; set; }

        /// <summary>
        /// 初诊日期
        /// </summary>
        public System.DateTime? FIR_SEE_DATE { get; set; }

        /// <summary>
        /// 最近挂号日期
        /// </summary>
        public System.DateTime? LREG_DATE { get; set; }

        /// <summary>
        /// 违约次数
        /// </summary>
        public System.Decimal? DISOBY_CNT { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public System.DateTime? END_DATE { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public System.String MARK { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public System.String OPER_CODE { get; set; }

        /// <summary>
        /// 操作日期
        /// </summary>
        public System.DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// 是否有效1有效0无效2作废
        /// </summary>
        public System.String IS_VALID { get; set; }

        /// <summary>
        /// 算法类别  0 全部
        /// </summary>
        public System.String FEE_KIND { get; set; }

        /// <summary>
        /// 旧卡号,新老数据切换用
        /// </summary>
        public System.String OLD_CARDNO { get; set; }

        /// <summary>
        /// 是否加密姓名
        /// </summary>
        public System.String IS_ENCRYPTNAME { get; set; }

        /// <summary>
        /// 密文
        /// </summary>
        public System.String NORMALNAME { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public System.String IDCARDTYPE { get; set; }

        /// <summary>
        /// 是否Vip 1是 0不是
        /// </summary>
        public System.String VIP_FLAG { get; set; }

        /// <summary>
        /// 母亲姓名
        /// </summary>
        public System.String MONTHER_NAME { get; set; }

        /// <summary>
        /// 是否急诊
        /// </summary>
        public System.String IS_TREATMENT { get; set; }

        /// <summary>
        /// 病案号
        /// </summary>
        public System.String CASE_NO { get; set; }

        /// <summary>
        /// 保险公司编码
        /// </summary>
        public System.String INSURANCE_ID { get; set; }

        /// <summary>
        /// 保险公司名称
        /// </summary>
        public System.String INSURANCE_NAME { get; set; }

        /// <summary>
        /// 家庭住址门牌号
        /// </summary>
        public System.String HOME_DOOR_NO { get; set; }

        /// <summary>
        /// 联系人地址门牌号
        /// </summary>
        public System.String LINKMAN_DOOR_NO { get; set; }

        /// <summary>
        /// email地址
        /// </summary>
        public System.String EMAIL { get; set; }

        /// <summary>
        /// EMR患者基本信息流水号 SEQ_COM_PATID
        /// </summary>
        public System.Int64? EMR_PATID { get; set; }

        /// <summary>
        /// 患者现住址
        /// </summary>
        public System.String HOME_NOW { get; set; }

        /// <summary>
        /// 患者类别（普通、VIP、特诊等） 常数PatientType
        /// </summary>
        public System.String PATIENT_TYPE { get; set; }

        /// <summary>
        /// 监护人证件号码
        /// </summary>
        public System.String GUARDIDNO { get; set; }
    }
}
