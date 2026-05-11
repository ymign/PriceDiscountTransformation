using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：出库申请表
    /// 数据表名：pha_com_applyout
    /// 生成时间：2025-11-20 14:54:14
    /// </summary>
    public class PhaComApplyout
    {
        /// <summary>
        /// 字段说明:申请流水号
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:否
        /// 字段名称:apply_number
        /// </summary>
        public decimal ApplyNumber { get; set; }

        /// <summary>
        /// 字段说明:申请部门编码（科室或者病区）
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:dept_code
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 字段说明:发药部门编码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:drug_dept_code
        /// </summary>
        public string DrugDeptCode { get; set; }

        /// <summary>
        /// 字段说明:出库申请分类
        /// 数据类型:VARCHAR2
        /// 字段长度:8
        /// 是否可空:是
        /// 字段名称:class3_meaning_code
        /// </summary>
        public string Class3MeaningCode { get; set; }

        /// <summary>
        /// 字段说明:批次号
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:group_code
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 字段说明:药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品商品名
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:trade_name
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 字段说明:批号
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:batch_no
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 字段说明:药品类别
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:drug_type
        /// </summary>
        public string DrugType { get; set; }

        /// <summary>
        /// 字段说明:药品性质
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:drug_quality
        /// </summary>
        public string DrugQuality { get; set; }

        /// <summary>
        /// 字段说明:规格
        /// 数据类型:VARCHAR2
        /// 字段长度:32
        /// 是否可空:是
        /// 字段名称:specs
        /// </summary>
        public string Specs { get; set; }

        /// <summary>
        /// 字段说明:包装单位
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:pack_unit
        /// </summary>
        public string PackUnit { get; set; }

        /// <summary>
        /// 字段说明:包装数
        /// 数据类型:NUMBER
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:pack_qty
        /// </summary>
        public decimal PackQty { get; set; }

        /// <summary>
        /// 字段说明:最小单位
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:min_unit
        /// </summary>
        public string MinUnit { get; set; }

        /// <summary>
        /// 字段说明:显示的单位标记(0最小单位,1包装单位)
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:show_flag
        /// </summary>
        public string ShowFlag { get; set; }

        /// <summary>
        /// 字段说明:显示的单位
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:show_unit
        /// </summary>
        public string ShowUnit { get; set; }

        /// <summary>
        /// 字段说明:零售价
        /// 数据类型:NUMBER
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:retail_price
        /// </summary>
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// 字段说明:批发价
        /// 数据类型:NUMBER
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:wholesale_price
        /// </summary>
        public decimal WholesalePrice { get; set; }

        /// <summary>
        /// 字段说明:购入价
        /// 数据类型:NUMBER
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:purchase_price
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// 字段说明:申请单号
        /// 数据类型:VARCHAR2
        /// 字段长度:18
        /// 是否可空:是
        /// 字段名称:apply_billcode
        /// </summary>
        public string ApplyBillcode { get; set; }

        /// <summary>
        /// 字段说明:申请人编码
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:apply_opercode
        /// </summary>
        public string ApplyOpercode { get; set; }

        /// <summary>
        /// 字段说明:申请日期
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:apply_date
        /// </summary>
        public DateTime ApplyDate { get; set; }

        /// <summary>
        /// 字段说明:申请状态 0申请，1（配药）打印，2核准（出库），3作废，4暂不摆药
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:apply_state
        /// </summary>
        public string ApplyState { get; set; }

        /// <summary>
        /// 字段说明:申请出库量(每付的总数量)
        /// 数据类型:NUMBER
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:apply_num
        /// </summary>
        public decimal ApplyNum { get; set; }

        /// <summary>
        /// 字段说明:付数（草药）
        /// 数据类型:NUMBER
        /// 字段长度:5
        /// 是否可空:是
        /// 字段名称:days
        /// </summary>
        public decimal Days { get; set; }

        /// <summary>
        /// 字段说明:是否预扣库存1是0否
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:preout_flag
        /// </summary>
        public string PreoutFlag { get; set; }

        /// <summary>
        /// 字段说明:收费状态：0未收费，1已收费
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:charge_flag
        /// </summary>
        public string ChargeFlag { get; set; }

        /// <summary>
        /// 字段说明:患者编号
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:patient_id
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 字段说明:患者科室
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:patient_dept
        /// </summary>
        public string PatientDept { get; set; }

        /// <summary>
        /// 字段说明:摆药单号
        /// 数据类型:VARCHAR2
        /// 字段长度:18
        /// 是否可空:是
        /// 字段名称:druged_bill
        /// </summary>
        public string DrugedBill { get; set; }

        /// <summary>
        /// 字段说明:摆药科室
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:druged_dept
        /// </summary>
        public string DrugedDept { get; set; }

        /// <summary>
        /// 字段说明:摆药人
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:druged_empl
        /// </summary>
        public string DrugedEmpl { get; set; }

        /// <summary>
        /// 字段说明:摆药日期
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:druged_date
        /// </summary>
        public DateTime DrugedDate { get; set; }

        /// <summary>
        /// 字段说明:摆药数量
        /// 数据类型:NUMBER
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:druged_num
        /// </summary>
        public decimal DrugedNum { get; set; }

        /// <summary>
        /// 字段说明:每次剂量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:dose_once
        /// </summary>
        public decimal DoseOnce { get; set; }

        /// <summary>
        /// 字段说明:剂量单位
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:dose_unit
        /// </summary>
        public string DoseUnit { get; set; }

        /// <summary>
        /// 字段说明:用法代码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:usage_code
        /// </summary>
        public string UsageCode { get; set; }

        /// <summary>
        /// 字段说明:用法名称
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:use_name
        /// </summary>
        public string UseName { get; set; }

        /// <summary>
        /// 字段说明:频次代码
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:dfq_freq
        /// </summary>
        public string DfqFreq { get; set; }

        /// <summary>
        /// 字段说明:频次名称
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:是
        /// 字段名称:dfq_cexp
        /// </summary>
        public string DfqCexp { get; set; }

        /// <summary>
        /// 字段说明:剂型代码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:dose_model_code
        /// </summary>
        public string DoseModelCode { get; set; }

        /// <summary>
        /// 字段说明:医嘱类别
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:order_type
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 字段说明:医嘱流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:mo_order
        /// </summary>
        public string MoOrder { get; set; }

        /// <summary>
        /// 字段说明:组合序号
        /// 数据类型:VARCHAR2
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:comb_no
        /// </summary>
        public string CombNo { get; set; }

        /// <summary>
        /// 字段说明:执行单流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:exec_sqn
        /// </summary>
        public string ExecSqn { get; set; }

        /// <summary>
        /// 字段说明:处方号
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:recipe_no
        /// </summary>
        public string RecipeNo { get; set; }

        /// <summary>
        /// 字段说明:处方内项目流水号
        /// 数据类型:NUMBER
        /// 字段长度:5
        /// 是否可空:是
        /// 字段名称:sequence_no
        /// </summary>
        public decimal SequenceNo { get; set; }

        /// <summary>
        /// 字段说明:医嘱发送类型2临时，1集中，0全部
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:send_type
        /// </summary>
        public string SendType { get; set; }

        /// <summary>
        /// 字段说明:摆药单分类代码
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:billclass_code
        /// </summary>
        public string BillclassCode { get; set; }

        /// <summary>
        /// 字段说明:打印状态（0未打印，1已打印）
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:print_state
        /// </summary>
        public string PrintState { get; set; }

        /// <summary>
        /// 字段说明:门诊调剂标记1是0否
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:relieve_flag
        /// </summary>
        public string RelieveFlag { get; set; }

        /// <summary>
        /// 字段说明:调剂单流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:relieve_code
        /// </summary>
        public string RelieveCode { get; set; }

        /// <summary>
        /// 字段说明:操作员（打印人）
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:print_empl
        /// </summary>
        public string PrintEmpl { get; set; }

        /// <summary>
        /// 字段说明:操作日期（打印时间）
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:print_date
        /// </summary>
        public DateTime PrintDate { get; set; }

        /// <summary>
        /// 字段说明:出库单流水号（退库申请时，保存申请退库记录的出库单流水号号）
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:out_bill_code
        /// </summary>
        public decimal OutBillCode { get; set; }

        /// <summary>
        /// 字段说明:有效标记（1有效，0无效，2不摆药）
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:valid_state
        /// </summary>
        public string ValidState { get; set; }

        /// <summary>
        /// 字段说明:备注
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:mark
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 字段说明:取消操作员
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:cancel_empl
        /// </summary>
        public string CancelEmpl { get; set; }

        /// <summary>
        /// 字段说明:取消日期
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:cancel_date
        /// </summary>
        public DateTime CancelDate { get; set; }

        /// <summary>
        /// 字段说明:货位号
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:place_code
        /// </summary>
        public string PlaceCode { get; set; }

        /// <summary>
        /// 字段说明:开方科室
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:recipe_dept
        /// </summary>
        public string RecipeDept { get; set; }

        /// <summary>
        /// 字段说明:开方医生
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:recipe_oper
        /// </summary>
        public string RecipeOper { get; set; }

        /// <summary>
        /// 字段说明:是否婴儿 1 是 0 否
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:baby_flag
        /// </summary>
        public string BabyFlag { get; set; }

        /// <summary>
        /// 字段说明:扩展字段
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:ext_flag
        /// </summary>
        public string ExtFlag { get; set; }

        /// <summary>
        /// 字段说明:扩展字段1
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:ext_flag1
        /// </summary>
        public string ExtFlag1 { get; set; }

        /// <summary>
        /// 字段说明:批次流水号,根据医嘱执行时间及组合号赋值
        /// 数据类型:VARCHAR2
        /// 字段长度:25
        /// 是否可空:是
        /// 字段名称:compound_group
        /// </summary>
        public string CompoundGroup { get; set; }

        /// <summary>
        /// 字段说明:是否需配液 ‘1’ 是 0 否
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:compound_flag
        /// </summary>
        public string CompoundFlag { get; set; }

        /// <summary>
        /// 字段说明:是否配液已执行 1 是 0 否
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:compound_exec
        /// </summary>
        public string CompoundExec { get; set; }

        /// <summary>
        /// 字段说明:配液执行人
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:compound_oper
        /// </summary>
        public string CompoundOper { get; set; }

        /// <summary>
        /// 字段说明:配液时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:compound_date
        /// </summary>
        public DateTime CompoundDate { get; set; }

        /// <summary>
        /// 字段说明:对应的所有执行档流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:execseqall
        /// </summary>
        public string Execseqall { get; set; }

        /// <summary>
        /// 字段说明:发药机状态（住院药房用）
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:pkstatus
        /// </summary>
        public string Pkstatus { get; set; }

        /// <summary>
        /// 字段说明:手麻处方编号
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:smrecipe_no
        /// </summary>
        public string SmrecipeNo { get; set; }

        /// <summary>
        /// 字段说明:追溯码采集状态 0或空待采集 1采集中 2不用采集 3采集成功 4跳过采集 5采集失败 6采集完成(但未全部采集成功)
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:tracecodecollectionstatus
        /// </summary>
        public string Tracecodecollectionstatus { get; set; }

        /// <summary>
        /// 字段说明:包装已采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:alreadycollectqty
        /// </summary>
        public decimal Alreadycollectqty { get; set; }

        /// <summary>
        /// 字段说明:包装需要采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:needcollectqty
        /// </summary>
        public decimal Needcollectqty { get; set; }

        /// <summary>
        /// 字段说明:包装申诉数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:appealcollectqty
        /// </summary>
        public decimal Appealcollectqty { get; set; }

        /// <summary>
        /// 字段说明:是否需要采集追溯码标识 0不需要 1需要
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:needcollecttracecodeflag
        /// </summary>
        public string NeedCollectTraceCodeFlag { get; set; }

        /// <summary>
        /// 字段说明:无需采集追溯码原因
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:notcollecttracecodereason
        /// </summary>
        public string NotCollectTraceCodeReason { get; set; }

        /// <summary>
        /// 字段说明:拆零追溯码已采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:alreadycollectspiltqty
        /// </summary>
        public decimal AlreadyCollectSpiltQty { get; set; }

        /// <summary>
        /// 字段说明:拆零追溯码应采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:needcollectspiltqty
        /// </summary>
        public decimal NeedCollectSpiltQty { get; set; }

        /// <summary>
        /// 字段说明:拆零追溯码申诉数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:appealcollectspiltqty
        /// </summary>
        public decimal AppealCollectSpiltQty { get; set; }

        /// <summary>
        /// 字段说明:包装转拆零标志 0否 1是
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:packconverttosplitflag
        /// </summary>
        public string PackConvertToSplitFlag { get; set; }

    }

}
