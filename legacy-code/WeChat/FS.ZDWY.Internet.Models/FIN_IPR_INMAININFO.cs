using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    public class FIN_IPR_INMAININFO
    {
        /// <summary>
        /// 住院主表
        /// </summary>

        private System.String _INPATIENT_NO;
        /// <summary>
        /// 住院流水号
        /// </summary>
        public System.String INPATIENT_NO { get { return this._INPATIENT_NO; } set { this._INPATIENT_NO = value; } }

        private System.String _MEDICAL_TYPE;
        /// <summary>
        /// 医疗类别
        /// </summary>
        public System.String MEDICAL_TYPE { get { return this._MEDICAL_TYPE; } set { this._MEDICAL_TYPE = value; } }

        private System.String _PATIENT_NO;
        /// <summary>
        /// 住院号
        /// </summary>
        public System.String PATIENT_NO { get { return this._PATIENT_NO; } set { this._PATIENT_NO = value; } }

        private System.String _CARD_NO;
        /// <summary>
        /// 就诊卡号
        /// </summary>
        public System.String CARD_NO { get { return this._CARD_NO; } set { this._CARD_NO = value; } }

        private System.String _MCARD_NO;
        /// <summary>
        /// 医疗证号
        /// </summary>
        public System.String MCARD_NO { get { return this._MCARD_NO; } set { this._MCARD_NO = value; } }

        private System.String _NAME;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get { return this._NAME; } set { this._NAME = value; } }

        private System.String _SEX_CODE;
        /// <summary>
        /// 性别
        /// </summary>
        public System.String SEX_CODE { get { return this._SEX_CODE; } set { this._SEX_CODE = value; } }

        private System.String _IDENNO;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String IDENNO { get { return this._IDENNO; } set { this._IDENNO = value; } }

        private System.String _SPELL_CODE;
        /// <summary>
        /// 拼音
        /// </summary>
        public System.String SPELL_CODE { get { return this._SPELL_CODE; } set { this._SPELL_CODE = value; } }

        private System.DateTime? _BIRTHDAY;
        /// <summary>
        /// 生日
        /// </summary>
        public System.DateTime? BIRTHDAY { get { return this._BIRTHDAY; } set { this._BIRTHDAY = value; } }

        private System.String _PROF_CODE;
        /// <summary>
        /// 职业代码
        /// </summary>
        public System.String PROF_CODE { get { return this._PROF_CODE; } set { this._PROF_CODE = value; } }

        private System.String _WORK_NAME;
        /// <summary>
        /// 工作单位
        /// </summary>
        public System.String WORK_NAME { get { return this._WORK_NAME; } set { this._WORK_NAME = value; } }

        private System.String _WORK_TEL;
        /// <summary>
        /// 工作单位电话
        /// </summary>
        public System.String WORK_TEL { get { return this._WORK_TEL; } set { this._WORK_TEL = value; } }

        private System.String _WORK_ZIP;
        /// <summary>
        /// 单位邮编
        /// </summary>
        public System.String WORK_ZIP { get { return this._WORK_ZIP; } set { this._WORK_ZIP = value; } }

        private System.String _HOME;
        /// <summary>
        /// 户口或家庭地址
        /// </summary>
        public System.String HOME { get { return this._HOME; } set { this._HOME = value; } }

        private System.String _HOME_TEL;
        /// <summary>
        /// 家庭电话
        /// </summary>
        public System.String HOME_TEL { get { return this._HOME_TEL; } set { this._HOME_TEL = value; } }

        private System.String _HOME_ZIP;
        /// <summary>
        /// 户口或家庭邮编
        /// </summary>
        public System.String HOME_ZIP { get { return this._HOME_ZIP; } set { this._HOME_ZIP = value; } }

        private System.String _DIST;
        /// <summary>
        /// 籍贯
        /// </summary>
        public System.String DIST { get { return this._DIST; } set { this._DIST = value; } }

        private System.String _BIRTH_AREA;
        /// <summary>
        /// 出生地代码
        /// </summary>
        public System.String BIRTH_AREA { get { return this._BIRTH_AREA; } set { this._BIRTH_AREA = value; } }

        private System.String _NATION_CODE;
        /// <summary>
        /// 民族
        /// </summary>
        public System.String NATION_CODE { get { return this._NATION_CODE; } set { this._NATION_CODE = value; } }

        private System.String _LINKMAN_NAME;
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public System.String LINKMAN_NAME { get { return this._LINKMAN_NAME; } set { this._LINKMAN_NAME = value; } }

        private System.String _LINKMAN_TEL;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public System.String LINKMAN_TEL { get { return this._LINKMAN_TEL; } set { this._LINKMAN_TEL = value; } }

        private System.String _LINKMAN_ADD;
        /// <summary>
        /// 联系人地址
        /// </summary>
        public System.String LINKMAN_ADD { get { return this._LINKMAN_ADD; } set { this._LINKMAN_ADD = value; } }

        private System.String _RELA_CODE;
        /// <summary>
        /// 联系人关系
        /// </summary>
        public System.String RELA_CODE { get { return this._RELA_CODE; } set { this._RELA_CODE = value; } }

        private System.String _MARI;
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public System.String MARI { get { return this._MARI; } set { this._MARI = value; } }

        private System.String _COUN_CODE;
        /// <summary>
        /// 国籍
        /// </summary>
        public System.String COUN_CODE { get { return this._COUN_CODE; } set { this._COUN_CODE = value; } }

        private System.Single? _HEIGHT;
        /// <summary>
        /// 身高
        /// </summary>
        public System.Single? HEIGHT { get { return this._HEIGHT; } set { this._HEIGHT = value; } }

        private System.Single? _WEIGHT;
        /// <summary>
        /// 体重
        /// </summary>
        public System.Single? WEIGHT { get { return this._WEIGHT; } set { this._WEIGHT = value; } }

        private System.String _BLOOD_DRESS;
        /// <summary>
        /// 血压
        /// </summary>
        public System.String BLOOD_DRESS { get { return this._BLOOD_DRESS; } set { this._BLOOD_DRESS = value; } }

        private System.String _BLOOD_CODE;
        /// <summary>
        /// 血型编码
        /// </summary>
        public System.String BLOOD_CODE { get { return this._BLOOD_CODE; } set { this._BLOOD_CODE = value; } }

        private System.String _HEPATITIS_FLAG;
        /// <summary>
        /// 重大疾病标志Y:有  N:无
        /// </summary>
        public System.String HEPATITIS_FLAG { get { return this._HEPATITIS_FLAG; } set { this._HEPATITIS_FLAG = value; } }

        private System.String _ANAPHY_FLAG;
        /// <summary>
        /// 过敏标志Y:有  N:无
        /// </summary>
        public System.String ANAPHY_FLAG { get { return this._ANAPHY_FLAG; } set { this._ANAPHY_FLAG = value; } }

        private System.DateTime _IN_DATE;
        /// <summary>
        /// 入院日期
        /// </summary>
        public System.DateTime IN_DATE { get { return this._IN_DATE; } set { this._IN_DATE = value; } }

        private System.String _DEPT_CODE;
        /// <summary>
        /// 科室代码
        /// </summary>
        public System.String DEPT_CODE { get { return this._DEPT_CODE; } set { this._DEPT_CODE = value; } }

        private System.String _DEPT_NAME;
        /// <summary>
        /// 科室名称
        /// </summary>
        public System.String DEPT_NAME { get { return this._DEPT_NAME; } set { this._DEPT_NAME = value; } }

        private System.String _PAYKIND_CODE;
        /// <summary>
        /// 结算类别 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
        /// </summary>
        public System.String PAYKIND_CODE { get { return this._PAYKIND_CODE; } set { this._PAYKIND_CODE = value; } }

        private System.String _PACT_CODE;
        /// <summary>
        /// 合同代码
        /// </summary>
        public System.String PACT_CODE { get { return this._PACT_CODE; } set { this._PACT_CODE = value; } }

        private System.String _PACT_NAME;
        /// <summary>
        /// 合同单位名称
        /// </summary>
        public System.String PACT_NAME { get { return this._PACT_NAME; } set { this._PACT_NAME = value; } }

        private System.String _BED_NO;
        /// <summary>
        /// 床号
        /// </summary>
        public System.String BED_NO { get { return this._BED_NO; } set { this._BED_NO = value; } }

        private System.String _NURSE_CELL_CODE;
        /// <summary>
        /// 护理单元代码
        /// </summary>
        public System.String NURSE_CELL_CODE { get { return this._NURSE_CELL_CODE; } set { this._NURSE_CELL_CODE = value; } }

        private System.String _NURSE_CELL_NAME;
        /// <summary>
        /// 护理单元名称
        /// </summary>
        public System.String NURSE_CELL_NAME { get { return this._NURSE_CELL_NAME; } set { this._NURSE_CELL_NAME = value; } }

        private System.String _HOUSE_DOC_CODE;
        /// <summary>
        /// 医师代码(住院)
        /// </summary>
        public System.String HOUSE_DOC_CODE { get { return this._HOUSE_DOC_CODE; } set { this._HOUSE_DOC_CODE = value; } }

        private System.String _HOUSE_DOC_NAME;
        /// <summary>
        /// 医师姓名(住院)
        /// </summary>
        public System.String HOUSE_DOC_NAME { get { return this._HOUSE_DOC_NAME; } set { this._HOUSE_DOC_NAME = value; } }

        private System.String _CHARGE_DOC_CODE;
        /// <summary>
        /// 医师代码(主治)
        /// </summary>
        public System.String CHARGE_DOC_CODE { get { return this._CHARGE_DOC_CODE; } set { this._CHARGE_DOC_CODE = value; } }

        private System.String _CHARGE_DOC_NAME;
        /// <summary>
        /// 医师姓名(主治)
        /// </summary>
        public System.String CHARGE_DOC_NAME { get { return this._CHARGE_DOC_NAME; } set { this._CHARGE_DOC_NAME = value; } }

        private System.String _CHIEF_DOC_CODE;
        /// <summary>
        /// 医师代码(主任)
        /// </summary>
        public System.String CHIEF_DOC_CODE { get { return this._CHIEF_DOC_CODE; } set { this._CHIEF_DOC_CODE = value; } }

        private System.String _CHIEF_DOC_NAME;
        /// <summary>
        /// 医师姓名(主任)
        /// </summary>
        public System.String CHIEF_DOC_NAME { get { return this._CHIEF_DOC_NAME; } set { this._CHIEF_DOC_NAME = value; } }

        private System.String _DUTY_NURSE_CODE;
        /// <summary>
        /// 护士代码(责任)
        /// </summary>
        public System.String DUTY_NURSE_CODE { get { return this._DUTY_NURSE_CODE; } set { this._DUTY_NURSE_CODE = value; } }

        private System.String _DUTY_NURSE_NAME;
        /// <summary>
        /// 护士姓名(责任)
        /// </summary>
        public System.String DUTY_NURSE_NAME { get { return this._DUTY_NURSE_NAME; } set { this._DUTY_NURSE_NAME = value; } }

        private System.String _IN_CIRCS;
        /// <summary>
        /// 入院情况
        /// </summary>
        public System.String IN_CIRCS { get { return this._IN_CIRCS; } set { this._IN_CIRCS = value; } }

        private System.String _IN_AVENUE;
        /// <summary>
        /// 入院途径
        /// </summary>
        public System.String IN_AVENUE { get { return this._IN_AVENUE; } set { this._IN_AVENUE = value; } }

        private System.String _IN_SOURCE;
        /// <summary>
        /// 入院来源 1:急诊，2:门诊，3:转科，4:转院
        /// </summary>
        public System.String IN_SOURCE { get { return this._IN_SOURCE; } set { this._IN_SOURCE = value; } }

        private System.Int16? _IN_TIMES;
        /// <summary>
        /// 住院次数
        /// </summary>
        public System.Int16? IN_TIMES { get { return this._IN_TIMES; } set { this._IN_TIMES = value; } }

        private System.Double? _PREPAY_COST;
        /// <summary>
        /// 预交金额(未结)
        /// </summary>
        public System.Double? PREPAY_COST { get { return this._PREPAY_COST; } set { this._PREPAY_COST = value; } }

        private System.Double? _CHANGE_PREPAYCOST;
        /// <summary>
        /// 转入预交金额（未结)
        /// </summary>
        public System.Double? CHANGE_PREPAYCOST { get { return this._CHANGE_PREPAYCOST; } set { this._CHANGE_PREPAYCOST = value; } }

        private System.Double? _MONEY_ALERT;
        /// <summary>
        /// 警戒线
        /// </summary>
        public System.Double? MONEY_ALERT { get { return this._MONEY_ALERT; } set { this._MONEY_ALERT = value; } }

        private System.Double? _TOT_COST;
        /// <summary>
        /// 费用金额(未结)
        /// </summary>
        public System.Double? TOT_COST { get { return this._TOT_COST; } set { this._TOT_COST = value; } }

        private System.Double? _OWN_COST;
        /// <summary>
        /// 自费金额(未结)
        /// </summary>
        public System.Double? OWN_COST { get { return this._OWN_COST; } set { this._OWN_COST = value; } }

        private System.Double? _PAY_COST;
        /// <summary>
        /// 自付金额(未结)
        /// </summary>
        public System.Double? PAY_COST { get { return this._PAY_COST; } set { this._PAY_COST = value; } }

        private System.Double? _PUB_COST;
        /// <summary>
        /// 公费金额(未结)
        /// </summary>
        public System.Double? PUB_COST { get { return this._PUB_COST; } set { this._PUB_COST = value; } }

        private System.Double? _ECO_COST;
        /// <summary>
        /// 优惠金额(未结)
        /// </summary>
        public System.Double? ECO_COST { get { return this._ECO_COST; } set { this._ECO_COST = value; } }

        private System.Double? _FREE_COST;
        /// <summary>
        /// 余额(未结)
        /// </summary>
        public System.Double? FREE_COST { get { return this._FREE_COST; } set { this._FREE_COST = value; } }

        private System.Double? _CHANGE_TOTCOST;
        /// <summary>
        /// 转入费用金额(未结)
        /// </summary>
        public System.Double? CHANGE_TOTCOST { get { return this._CHANGE_TOTCOST; } set { this._CHANGE_TOTCOST = value; } }

        private System.Double? _UPPER_LIMIT;
        /// <summary>
        /// 待遇上限
        /// </summary>
        public System.Double? UPPER_LIMIT { get { return this._UPPER_LIMIT; } set { this._UPPER_LIMIT = value; } }

        private System.Int16? _FEE_INTERVAL;
        /// <summary>
        /// 固定费用间隔天数
        /// </summary>
        public System.Int16? FEE_INTERVAL { get { return this._FEE_INTERVAL; } set { this._FEE_INTERVAL = value; } }

        private System.Int16 _BALANCE_NO;
        /// <summary>
        /// 结算序号
        /// </summary>
        public System.Int16 BALANCE_NO { get { return this._BALANCE_NO; } set { this._BALANCE_NO = value; } }

        private System.Double? _BALANCE_COST;
        /// <summary>
        /// 费用金额(已结)
        /// </summary>
        public System.Double? BALANCE_COST { get { return this._BALANCE_COST; } set { this._BALANCE_COST = value; } }

        private System.Double? _BALANCE_PREPAY;
        /// <summary>
        /// 预交金额(已结)
        /// </summary>
        public System.Double? BALANCE_PREPAY { get { return this._BALANCE_PREPAY; } set { this._BALANCE_PREPAY = value; } }

        private System.DateTime? _BALANCE_DATE;
        /// <summary>
        /// 结算日期(上次)
        /// </summary>
        public System.DateTime? BALANCE_DATE { get { return this._BALANCE_DATE; } set { this._BALANCE_DATE = value; } }

        private System.String _STOP_ACOUNT;
        /// <summary>
        /// 是否关帐
        /// </summary>
        public System.String STOP_ACOUNT { get { return this._STOP_ACOUNT; } set { this._STOP_ACOUNT = value; } }

        private System.String _BABY_FLAG;
        /// <summary>
        /// 婴儿标志 1:有婴儿；0:无婴儿
        /// </summary>
        public System.String BABY_FLAG { get { return this._BABY_FLAG; } set { this._BABY_FLAG = value; } }

        private System.String _CASE_FLAG;
        /// <summary>
        /// 病案状态: 0 无需病案 1 需要病案 2 医生站形成病案 3 病案室形成病案 4病案封存
        /// </summary>
        public System.String CASE_FLAG { get { return this._CASE_FLAG; } set { this._CASE_FLAG = value; } }

        private System.String _IN_STATE;
        /// <summary>
        /// R-住院登记  I-病房接诊 B-出院登记 O-出院结算 P-预约出院,N-无费退院
        /// </summary>
        public System.String IN_STATE { get { return this._IN_STATE; } set { this._IN_STATE = value; } }

        private System.String _LEAVE_FLAG;
        /// <summary>
        /// 是否请假 0 非 1是
        /// </summary>
        public System.String LEAVE_FLAG { get { return this._LEAVE_FLAG; } set { this._LEAVE_FLAG = value; } }

        private System.DateTime? _PREPAY_OUTDATE;
        /// <summary>
        /// 出院日期(预约)
        /// </summary>
        public System.DateTime? PREPAY_OUTDATE { get { return this._PREPAY_OUTDATE; } set { this._PREPAY_OUTDATE = value; } }

        private System.DateTime? _OUT_DATE;
        /// <summary>
        /// 出院日期
        /// </summary>
        public System.DateTime? OUT_DATE { get { return this._OUT_DATE; } set { this._OUT_DATE = value; } }

        private System.String _ZG;
        /// <summary>
        /// 转归代号
        /// </summary>
        public System.String ZG { get { return this._ZG; } set { this._ZG = value; } }

        private System.String _EMPL_CODE;
        /// <summary>
        /// 开据医师
        /// </summary>
        public System.String EMPL_CODE { get { return this._EMPL_CODE; } set { this._EMPL_CODE = value; } }

        private System.String _IN_ICU;
        /// <summary>
        /// 是否在ICU 0 no 1 yes
        /// </summary>
        public System.String IN_ICU { get { return this._IN_ICU; } set { this._IN_ICU = value; } }

        private System.String _CASESEND_FLAG;
        /// <summary>
        /// 病案送入病案室否0未1送
        /// </summary>
        public System.String CASESEND_FLAG { get { return this._CASESEND_FLAG; } set { this._CASESEND_FLAG = value; } }

        private System.String _TEND;
        /// <summary>
        /// 护理级别(TEND):名称显示护理级别名称(一级护理，二级护理，三级护理)
        /// </summary>
        public System.String TEND { get { return this._TEND; } set { this._TEND = value; } }

        private System.String _CRITICAL_FLAG;
        /// <summary>
        /// 病危：0 普通 1 病重 2 病危
        /// </summary>
        public System.String CRITICAL_FLAG { get { return this._CRITICAL_FLAG; } set { this._CRITICAL_FLAG = value; } }

        private System.DateTime? _PREFIXFEE_DATE;
        /// <summary>
        /// 上次固定费用时间
        /// </summary>
        public System.DateTime? PREFIXFEE_DATE { get { return this._PREFIXFEE_DATE; } set { this._PREFIXFEE_DATE = value; } }

        private System.String _OPER_CODE;
        /// <summary>
        /// 操作员
        /// </summary>
        public System.String OPER_CODE { get { return this._OPER_CODE; } set { this._OPER_CODE = value; } }

        private System.DateTime _OPER_DATE;
        /// <summary>
        /// 操作日期
        /// </summary>
        public System.DateTime OPER_DATE { get { return this._OPER_DATE; } set { this._OPER_DATE = value; } }

        private System.Double? _BLOOD_LATEFEE;
        /// <summary>
        /// 血滞纳金
        /// </summary>
        public System.Double? BLOOD_LATEFEE { get { return this._BLOOD_LATEFEE; } set { this._BLOOD_LATEFEE = value; } }

        private System.Double? _DAY_LIMIT;
        /// <summary>
        /// 公费患者日限额
        /// </summary>
        public System.Double? DAY_LIMIT { get { return this._DAY_LIMIT; } set { this._DAY_LIMIT = value; } }

        private System.Double? _LIMIT_TOT;
        /// <summary>
        /// 公费患者日限额累计
        /// </summary>
        public System.Double? LIMIT_TOT { get { return this._LIMIT_TOT; } set { this._LIMIT_TOT = value; } }

        private System.Double? _LIMIT_OVERTOP;
        /// <summary>
        /// 公费患者日限额超标部分金额
        /// </summary>
        public System.Double? LIMIT_OVERTOP { get { return this._LIMIT_OVERTOP; } set { this._LIMIT_OVERTOP = value; } }

        private System.String _CLINIC_DIAGNOSE;
        /// <summary>
        /// 门诊诊断
        /// </summary>
        public System.String CLINIC_DIAGNOSE { get { return this._CLINIC_DIAGNOSE; } set { this._CLINIC_DIAGNOSE = value; } }

        private System.String _PROCREATE_PCNO;
        /// <summary>
        /// 生育保险患者电脑号
        /// </summary>
        public System.String PROCREATE_PCNO { get { return this._PROCREATE_PCNO; } set { this._PROCREATE_PCNO = value; } }

        private System.String _DIETETIC_MARK;
        /// <summary>
        /// 饮食
        /// </summary>
        public System.String DIETETIC_MARK { get { return this._DIETETIC_MARK; } set { this._DIETETIC_MARK = value; } }

        private System.Double? _BURSARY_TOTMEDFEE;
        /// <summary>
        /// 公费患者公费药品累计(日限额)
        /// </summary>
        public System.Double? BURSARY_TOTMEDFEE { get { return this._BURSARY_TOTMEDFEE; } set { this._BURSARY_TOTMEDFEE = value; } }

        private System.String _MEMO;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String MEMO { get { return this._MEMO; } set { this._MEMO = value; } }

        private System.Double? _BED_LIMIT;
        /// <summary>
        /// 床位上限
        /// </summary>
        public System.Double? BED_LIMIT { get { return this._BED_LIMIT; } set { this._BED_LIMIT = value; } }

        private System.Double? _AIR_LIMIT;
        /// <summary>
        /// 空调上限
        /// </summary>
        public System.Double? AIR_LIMIT { get { return this._AIR_LIMIT; } set { this._AIR_LIMIT = value; } }

        private System.String _BEDOVERDEAL;
        /// <summary>
        /// 床费超标处理 0超标不限 1超标自理 2超标不计
        /// </summary>
        public System.String BEDOVERDEAL { get { return this._BEDOVERDEAL; } set { this._BEDOVERDEAL = value; } }

        private System.String _EXT_FLAG;
        /// <summary>
        /// 扩展标记（公医超日限额是否同意：0不同意，1同意）
        /// </summary>
        public System.String EXT_FLAG { get { return this._EXT_FLAG; } set { this._EXT_FLAG = value; } }

        private System.String _EXT_FLAG1;
        /// <summary>
        /// 扩展标记1
        /// </summary>
        public System.String EXT_FLAG1 { get { return this._EXT_FLAG1; } set { this._EXT_FLAG1 = value; } }

        private System.String _EXT_FLAG2;
        /// <summary>
        /// 扩展标记2
        /// </summary>
        public System.String EXT_FLAG2 { get { return this._EXT_FLAG2; } set { this._EXT_FLAG2 = value; } }

        private System.Double? _BOARD_COST;
        /// <summary>
        /// 膳食花费总额
        /// </summary>
        public System.Double? BOARD_COST { get { return this._BOARD_COST; } set { this._BOARD_COST = value; } }

        private System.Double? _BOARD_PREPAY;
        /// <summary>
        /// 膳食预交金额
        /// </summary>
        public System.Double? BOARD_PREPAY { get { return this._BOARD_PREPAY; } set { this._BOARD_PREPAY = value; } }

        private System.String _BOARD_STATE;
        /// <summary>
        /// 膳食结算状态：0在院 1出院
        /// </summary>
        public System.String BOARD_STATE { get { return this._BOARD_STATE; } set { this._BOARD_STATE = value; } }

        private System.Single? _OWN_RATE;
        /// <summary>
        /// 自费比例
        /// </summary>
        public System.Single? OWN_RATE { get { return this._OWN_RATE; } set { this._OWN_RATE = value; } }

        private System.Single? _PAY_RATE;
        /// <summary>
        /// 自付比例
        /// </summary>
        public System.Single? PAY_RATE { get { return this._PAY_RATE; } set { this._PAY_RATE = value; } }

        private System.Double? _EXT_NUMBER;
        /// <summary>
        /// 扩展数值（中山一用作－剩余统筹金额）
        /// </summary>
        public System.Double? EXT_NUMBER { get { return this._EXT_NUMBER; } set { this._EXT_NUMBER = value; } }

        private System.String _EXT_CODE;
        /// <summary>
        /// 扩展编码（）
        /// </summary>
        public System.String EXT_CODE { get { return this._EXT_CODE; } set { this._EXT_CODE = value; } }

        private System.String _DIAG_NAME;
        /// <summary>
        /// 诊断名称（建议用此保存主诊断）
        /// </summary>
        public System.String DIAG_NAME { get { return this._DIAG_NAME; } set { this._DIAG_NAME = value; } }

        private System.String _IS_ENCRYPTNAME;
        /// <summary>
        /// 是否加密
        /// </summary>
        public System.String IS_ENCRYPTNAME { get { return this._IS_ENCRYPTNAME; } set { this._IS_ENCRYPTNAME = value; } }

        private System.String _NORMALNAME;
        /// <summary>
        /// 密文
        /// </summary>
        public System.String NORMALNAME { get { return this._NORMALNAME; } set { this._NORMALNAME = value; } }

        private System.String _IDCARDTYPE;
        /// <summary>
        /// 证件类型
        /// </summary>
        public System.String IDCARDTYPE { get { return this._IDCARDTYPE; } set { this._IDCARDTYPE = value; } }

        private System.String _ALTER_TYPE;
        /// <summary>
        /// M 金额 D时间段
        /// </summary>
        public System.String ALTER_TYPE { get { return this._ALTER_TYPE; } set { this._ALTER_TYPE = value; } }

        private System.DateTime? _ALTER_BEGIN;
        /// <summary>
        /// 警戒线开始时间
        /// </summary>
        public System.DateTime? ALTER_BEGIN { get { return this._ALTER_BEGIN; } set { this._ALTER_BEGIN = value; } }

        private System.DateTime? _ALTER_END;
        /// <summary>
        /// 警戒线结束时间
        /// </summary>
        public System.DateTime? ALTER_END { get { return this._ALTER_END; } set { this._ALTER_END = value; } }

        private System.String _ALTER_APPROVE_CODE;
        /// <summary>
        /// 警戒线批准人
        /// </summary>
        public System.String ALTER_APPROVE_CODE { get { return this._ALTER_APPROVE_CODE; } set { this._ALTER_APPROVE_CODE = value; } }

        private System.DateTime? _ALTER_APPROVE_DATE;
        /// <summary>
        /// 警戒线批准原因
        /// </summary>
        public System.DateTime? ALTER_APPROVE_DATE { get { return this._ALTER_APPROVE_DATE; } set { this._ALTER_APPROVE_DATE = value; } }

        private System.Int64? _EMR_INPATIENTID;
        /// <summary>
        /// EMR住院流水号 SEQ_FIN_INPATIENTID
        /// </summary>
        public System.Int64? EMR_INPATIENTID { get { return this._EMR_INPATIENTID; } set { this._EMR_INPATIENTID = value; } }

        private System.String _HOME_NOW;
        /// <summary>
        /// 患者现住址
        /// </summary>
        public System.String HOME_NOW { get { return this._HOME_NOW; } set { this._HOME_NOW = value; } }

        private System.String _ALTER_FLAG;
        /// <summary>
        /// 是否启用警戒线，1表示启用，其他表示未启用
        /// </summary>
        public System.String ALTER_FLAG { get { return this._ALTER_FLAG; } set { this._ALTER_FLAG = value; } }

        private System.String _IS_MEALS;
        /// <summary>
        /// 是否自动收取配餐费 0 no 1 yes
        /// </summary>
        public System.String IS_MEALS { get { return this._IS_MEALS; } set { this._IS_MEALS = value; } }
    }
}
