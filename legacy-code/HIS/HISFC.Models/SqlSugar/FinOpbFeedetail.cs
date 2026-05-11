using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.SqlSugar
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：处方明细表
    /// 数据表名：fin_opb_feedetail
    /// 生成时间：2025-12-28 17:19:10
    /// </summary>
    public class FinOpbFeedetail
    {
        /// <summary>
        /// 字段说明:处方号[3]
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:否
        /// 字段名称:recipe_no
        /// </summary>
        public string RecipeNo { get; set; }

        /// <summary>
        /// 字段说明:处方内项目流水号[4]
        /// 数据类型:NUMBER
        /// 字段长度:9
        /// 是否可空:否
        /// 字段名称:sequence_no
        /// </summary>
        public decimal SequenceNo { get; set; }

        /// <summary>
        /// 字段说明:交易类型,1正交易，2反交易[5]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:否
        /// 字段名称:trans_type
        /// </summary>
        public string TransType { get; set; }

        /// <summary>
        /// 字段说明:门诊号[6]
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:clinic_code
        /// </summary>
        public string ClinicCode { get; set; }

        /// <summary>
        /// 字段说明:病历卡号[7]
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:card_no
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 字段说明:挂号日期[8]
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:reg_date
        /// </summary>
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 字段说明:开单科室[9]
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:reg_dpcd
        /// </summary>
        public string RegDpcd { get; set; }

        /// <summary>
        /// 字段说明:开方医师[10]
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:doct_code
        /// </summary>
        public string DoctCode { get; set; }

        /// <summary>
        /// 字段说明:开方医师所在科室[11]
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:doct_dept
        /// </summary>
        public string DoctDept { get; set; }

        /// <summary>
        /// 字段说明:项目代码[12]
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:item_code
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 字段说明:项目名称[13]
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:item_name
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 字段说明:1药品/0非要[14]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:drug_flag
        /// </summary>
        public string DrugFlag { get; set; }

        /// <summary>
        /// 字段说明:规格[15]
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:specs
        /// </summary>
        public string Specs { get; set; }

        /// <summary>
        /// 字段说明:自制药标志[16]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:self_made
        /// </summary>
        public string SelfMade { get; set; }

        /// <summary>
        /// 字段说明:药品性质，麻药，普药[17]
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:drug_quality
        /// </summary>
        public string DrugQuality { get; set; }

        /// <summary>
        /// 字段说明:剂型[18]
        /// 数据类型:VARCHAR2
        /// 字段长度:3
        /// 是否可空:是
        /// 字段名称:dose_model_code
        /// </summary>
        public string DoseModelCode { get; set; }

        /// <summary>
        /// 字段说明:最小费用代码[19]
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:fee_code
        /// </summary>
        public string FeeCode { get; set; }

        /// <summary>
        /// 字段说明:系统类别[20]
        /// 数据类型:VARCHAR2
        /// 字段长度:3
        /// 是否可空:是
        /// 字段名称:class_code
        /// </summary>
        public string ClassCode { get; set; }

        /// <summary>
        /// 字段说明:单价[21]
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:unit_price
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 字段说明:数量[22]
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:qty
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 字段说明:草药的付数，其他药品为1[23]
        /// 数据类型:NUMBER
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:days
        /// </summary>
        public decimal Days { get; set; }

        /// <summary>
        /// 字段说明:频次代码[24]
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:frequency_code
        /// </summary>
        public string FrequencyCode { get; set; }

        /// <summary>
        /// 字段说明:用法代码[25]
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:usage_code
        /// </summary>
        public string UsageCode { get; set; }

        /// <summary>
        /// 字段说明:用法名称[26]
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:use_name
        /// </summary>
        public string UseName { get; set; }

        /// <summary>
        /// 字段说明:院内注射次数[27]
        /// 数据类型:NUMBER
        /// 字段长度:3
        /// 是否可空:是
        /// 字段名称:inject_number
        /// </summary>
        public decimal InjectNumber { get; set; }

        /// <summary>
        /// 字段说明:加急标记:1普通/2加急[28]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:emc_flag
        /// </summary>
        public string EmcFlag { get; set; }

        /// <summary>
        /// 字段说明:样本类型[29]
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:lab_type
        /// </summary>
        public string LabType { get; set; }

        /// <summary>
        /// 字段说明:检体[30]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:check_body
        /// </summary>
        public string CheckBody { get; set; }

        /// <summary>
        /// 字段说明:每次用量[31]
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:dose_once
        /// </summary>
        public decimal DoseOnce { get; set; }

        /// <summary>
        /// 字段说明:每次用量单位[32]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:dose_unit
        /// </summary>
        public string DoseUnit { get; set; }

        /// <summary>
        /// 字段说明:基本剂量[33]
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:base_dose
        /// </summary>
        public decimal BaseDose { get; set; }

        /// <summary>
        /// 字段说明:包装数量[34]
        /// 数据类型:NUMBER
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:pack_qty
        /// </summary>
        public decimal PackQty { get; set; }

        /// <summary>
        /// 字段说明:计价单位[35]
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:price_unit
        /// </summary>
        public string PriceUnit { get; set; }

        /// <summary>
        /// 字段说明:可报效金额[36]
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:pub_cost
        /// </summary>
        public decimal PubCost { get; set; }

        /// <summary>
        /// 字段说明:自付金额[37]
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:pay_cost
        /// </summary>
        public decimal PayCost { get; set; }

        /// <summary>
        /// 字段说明:现金金额[38]
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:own_cost
        /// </summary>
        public decimal OwnCost { get; set; }

        /// <summary>
        /// 字段说明:执行科室代码[39]
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:exec_dpcd
        /// </summary>
        public string ExecDpcd { get; set; }

        /// <summary>
        /// 字段说明:执行科室名称[40]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:exec_dpnm
        /// </summary>
        public string ExecDpnm { get; set; }

        /// <summary>
        /// 字段说明:医保中心项目代码[41]
        /// 数据类型:VARCHAR2
        /// 字段长度:25
        /// 是否可空:是
        /// 字段名称:center_code
        /// </summary>
        public string CenterCode { get; set; }

        /// <summary>
        /// 字段说明:项目等级，1甲类，2乙类，3丙类[42]
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:item_grade
        /// </summary>
        public string ItemGrade { get; set; }

        /// <summary>
        /// 字段说明:主药标志[43]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:main_drug
        /// </summary>
        public string MainDrug { get; set; }

        /// <summary>
        /// 字段说明:组合号[44]
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:comb_no
        /// </summary>
        public string CombNo { get; set; }

        /// <summary>
        /// 字段说明:划价人[45]
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:oper_code
        /// </summary>
        public string OperCode { get; set; }

        /// <summary>
        /// 字段说明:划价时间[46]
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:oper_date
        /// </summary>
        public DateTime OperDate { get; set; }

        /// <summary>
        /// 字段说明:0划价 1收费 3预收费团体体检 4 药品预审核
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:pay_flag
        /// </summary>
        public string PayFlag { get; set; }

        /// <summary>
        /// 字段说明:0退费，1正常，2重打，3注销[48]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:cancel_flag
        /// </summary>
        public string CancelFlag { get; set; }

        /// <summary>
        /// 字段说明:收费员代码[49]
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:fee_cpcd
        /// </summary>
        public string FeeCpcd { get; set; }

        /// <summary>
        /// 字段说明:收费日期[50]
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:fee_date
        /// </summary>
        public DateTime FeeDate { get; set; }

        /// <summary>
        /// 字段说明:票据号[51]
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:invoice_no
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 字段说明:发票科目代码[52]
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:invo_code
        /// </summary>
        public string InvoCode { get; set; }

        /// <summary>
        /// 字段说明:发票内流水号[53]
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:invo_sequence
        /// </summary>
        public string InvoSequence { get; set; }

        /// <summary>
        /// 字段说明:0未确认/1确认[54]
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:confirm_flag
        /// </summary>
        public string ConfirmFlag { get; set; }

        /// <summary>
        /// 字段说明:确认人[55]
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:confirm_code
        /// </summary>
        public string ConfirmCode { get; set; }

        /// <summary>
        /// 字段说明:确认科室[56]
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:confirm_dept
        /// </summary>
        public string ConfirmDept { get; set; }

        /// <summary>
        /// 字段说明:确认时间[57]
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:confirm_date
        /// </summary>
        public DateTime ConfirmDate { get; set; }

        /// <summary>
        /// 字段说明:优惠金额[58]
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:eco_cost
        /// </summary>
        public decimal EcoCost { get; set; }

        /// <summary>
        /// 字段说明:发票序号，一次结算产生多张发票的combNo
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:否
        /// 字段名称:invoice_seq
        /// </summary>
        public string InvoiceSeq { get; set; }

        /// <summary>
        /// 字段说明:新项目比例
        /// 数据类型:NUMBER
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:new_itemrate
        /// </summary>
        public decimal NewItemrate { get; set; }

        /// <summary>
        /// 字段说明:原项目比例
        /// 数据类型:NUMBER
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:old_itemrate
        /// </summary>
        public decimal OldItemrate { get; set; }

        /// <summary>
        /// 字段说明:扩展标志 特殊项目标志 1 0 非
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:ext_flag
        /// </summary>
        public string ExtFlag { get; set; }

        /// <summary>
        /// 字段说明:0 正常/1个人体检/2 集体体检
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:ext_flag1
        /// </summary>
        public string ExtFlag1 { get; set; }

        /// <summary>
        /// 字段说明:日结标志：0：未日结/1：已日结
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:ext_flag2
        /// </summary>
        public string ExtFlag2 { get; set; }

        /// <summary>
        /// 字段说明:1 包装 单位 0, 最小单位
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:pact_unit_flag
        /// </summary>
        public string PactUnitFlag { get; set; }

        /// <summary>
        /// 字段说明:复合项目代码
        /// 数据类型:VARCHAR2
        /// 字段长度:15
        /// 是否可空:是
        /// 字段名称:package_code
        /// </summary>
        public string PackageCode { get; set; }

        /// <summary>
        /// 字段说明:复合项目名称
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// 字段名称:package_name
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 字段说明:可退数量
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:noback_num
        /// </summary>
        public decimal NobackNum { get; set; }

        /// <summary>
        /// 字段说明:确认数量
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:confirm_num
        /// </summary>
        public decimal ConfirmNum { get; set; }

        /// <summary>
        /// 字段说明:已确认院注次数
        /// 数据类型:NUMBER
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:confirm_inject
        /// </summary>
        public decimal ConfirmInject { get; set; }

        /// <summary>
        /// 字段说明:医嘱项目流水号或者体检项目流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:否
        /// 字段名称:mo_order
        /// </summary>
        public string MoOrder { get; set; }

        /// <summary>
        /// 字段说明:条码号
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:sample_id
        /// </summary>
        public string SampleId { get; set; }

        /// <summary>
        /// 字段说明:收费序列
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:recipe_seq
        /// </summary>
        public string RecipeSeq { get; set; }

        /// <summary>
        /// 字段说明:超标金额
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:over_cost
        /// </summary>
        public decimal OverCost { get; set; }

        /// <summary>
        /// 字段说明:药品超标金额
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:excess_cost
        /// </summary>
        public decimal ExcessCost { get; set; }

        /// <summary>
        /// 字段说明:自费药金额
        /// 数据类型:NUMBER
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:drug_owncost
        /// </summary>
        public decimal DrugOwncost { get; set; }

        /// <summary>
        /// 字段说明:费用来源 0 操作员 1 医嘱 2 终端 3 体检
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:cost_source
        /// </summary>
        public string CostSource { get; set; }

        /// <summary>
        /// 字段说明:附材标志
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:subjob_flag
        /// </summary>
        public string SubjobFlag { get; set; }

        /// <summary>
        /// 字段说明:0没有扣账户 1 已经扣账户
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:account_flag
        /// </summary>
        public string AccountFlag { get; set; }

        /// <summary>
        /// 字段说明:更新库存的流水号(物资)
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:update_sequenceno
        /// </summary>
        public decimal UpdateSequenceno { get; set; }

        /// <summary>
        /// 字段说明:医生所属科室
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:doctindept
        /// </summary>
        public string Doctindept { get; set; }

        /// <summary>
        /// 字段说明:医疗组代码
        /// 数据类型:VARCHAR2
        /// 字段长度:15
        /// 是否可空:是
        /// 字段名称:medicalgroupcode
        /// </summary>
        public string Medicalgroupcode { get; set; }

        /// <summary>
        /// 字段说明:结算类别编码
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:paykind_code
        /// </summary>
        public string PaykindCode { get; set; }

        /// <summary>
        /// 字段说明:合同单位编码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:pact_code
        /// </summary>
        public string PactCode { get; set; }

        /// <summary>
        /// 字段说明:项目原始价格（最小单位价格）
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:old_unit_price
        /// </summary>
        public decimal OldUnitPrice { get; set; }

        /// <summary>
        /// 字段说明:复合项目明细的数量
        /// 数据类型:NUMBER
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:package_qty
        /// </summary>
        public decimal PackageQty { get; set; }

        /// <summary>
        /// 字段说明:处方备注
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:recipe_memo
        /// </summary>
        public string RecipeMemo { get; set; }

        /// <summary>
        /// 字段说明:费用备注
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:memo
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 字段说明:中大五院自助设备打印标志
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:ext_flag3
        /// </summary>
        public string ExtFlag3 { get; set; }

        /// <summary>
        /// 字段说明:开立医生所属科室
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:belong_dept
        /// </summary>
        public string BelongDept { get; set; }

        /// <summary>
        /// 字段说明:医院编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:hos_code
        /// </summary>
        public string HosCode { get; set; }

        /// <summary>
        /// 字段说明:体检专用
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:tj_code
        /// </summary>
        public string TjCode { get; set; }

        /// <summary>
        /// 字段说明:执行科室2
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:exec_dpcd2
        /// </summary>
        public string ExecDpcd2 { get; set; }

        /// <summary>
        /// 字段说明:收费员收费时登录科室ID
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:oper_indept
        /// </summary>
        public string OperIndept { get; set; }

        /// <summary>
        /// 字段说明:收费员收费时登录科室名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:oper_indeptname
        /// </summary>
        public string OperIndeptname { get; set; }

        /// <summary>
        /// 字段说明:非药品退费与还原操作工号
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:quitundrugopercode
        /// </summary>
        public string Quitundrugopercode { get; set; }

        /// <summary>
        /// 字段说明:非药品退费与还原操作时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:quitundrugoperdate
        /// </summary>
        public DateTime Quitundrugoperdate { get; set; }

        /// <summary>
        /// 字段说明:app平台预结算号
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:precalid
        /// </summary>
        public string Precalid { get; set; }

        /// <summary>
        /// 字段说明:是否处方外延标记：1 是；0或者空 不是
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:extend_flag
        /// </summary>
        public string ExtendFlag { get; set; }

        /// <summary>
        /// 字段说明:数据来源;null/1:HIS,2:急诊系统,3:EMR,4:其它系统
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:source_flag
        /// </summary>
        public string SourceFlag { get; set; }

        /// <summary>
        /// 字段说明:是否推送平台标记;0:否,1:是
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:push_platform
        /// </summary>
        public string PushPlatform { get; set; }

        /// <summary>
        /// 字段说明:是否日间手术术前检查项目
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:day_operation_flag
        /// </summary>
        public string DayOperationFlag { get; set; }

        /// <summary>
        /// 字段说明:转住院标志 1为已转
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:transfer_flag
        /// </summary>
        public string TransferFlag { get; set; }

        /// <summary>
        /// 字段说明:makeaccountitemid
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:makeaccountitemid
        /// </summary>
        public string Makeaccountitemid { get; set; }

        /// <summary>
        /// 字段说明:hisaccountitemid
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:hisaccountitemid
        /// </summary>
        public string Hisaccountitemid { get; set; }

        /// <summary>
        /// 字段说明:bloodapplybillno
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:bloodapplybillno
        /// </summary>
        public string Bloodapplybillno { get; set; }

        /// <summary>
        /// 字段说明:是否发送申请单给平台：0和空发送，1不发送
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:issend
        /// </summary>
        public string Issend { get; set; }

        /// <summary>
        /// 字段说明:处方类型 0或者空：门诊特定病种，1：急救和抢救  2：自费 3:国家谈判药 4：产检 5：日间预入院 6：普通门诊（含门诊共济） 7;门诊共济（谈判药）
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:recipe_flag
        /// </summary>
        public string RecipeFlag { get; set; }

        /// <summary>
        /// 字段说明:划价重收前的项目流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:old_mo_order
        /// </summary>
        public string OldMoOrder { get; set; }

    }
}
