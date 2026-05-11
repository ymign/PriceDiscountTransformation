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
    /// 表名注释：药品目录表
    /// 数据表名：pha_com_baseinfo
    /// 生成时间：2025-08-21 09:11:20
    /// </summary>
    public class PhaComBaseinfo
    {
        /// <summary>
        /// 字段说明:药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:12
        /// 是否可空:否
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:商品名称
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:trade_name
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 字段说明:商品名拼音码
        /// 数据类型:VARCHAR2
        /// 字段长度:40
        /// 是否可空:是
        /// 字段名称:spell_code
        /// </summary>
        public string SpellCode { get; set; }

        /// <summary>
        /// 字段说明:商品名五笔码
        /// 数据类型:VARCHAR2
        /// 字段长度:80
        /// 是否可空:是
        /// 字段名称:wb_code
        /// </summary>
        public string WbCode { get; set; }

        /// <summary>
        /// 字段说明:商品名自定义码
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:custom_code
        /// </summary>
        public string CustomCode { get; set; }

        /// <summary>
        /// 字段说明:通用名
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:regular_name
        /// </summary>
        public string RegularName { get; set; }

        /// <summary>
        /// 字段说明:通用名拼音码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:regular_spell
        /// </summary>
        public string RegularSpell { get; set; }

        /// <summary>
        /// 字段说明:通用名五笔码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:regular_wb
        /// </summary>
        public string RegularWb { get; set; }

        /// <summary>
        /// 字段说明:通用名自定义码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:regular_custom
        /// </summary>
        public string RegularCustom { get; set; }

        /// <summary>
        /// 字段说明:学名
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:formal_name
        /// </summary>
        public string FormalName { get; set; }

        /// <summary>
        /// 字段说明:学名拼音码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:formal_spell
        /// </summary>
        public string FormalSpell { get; set; }

        /// <summary>
        /// 字段说明:学名五笔码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:formal_wb
        /// </summary>
        public string FormalWb { get; set; }

        /// <summary>
        /// 字段说明:学名自定义码
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:formal_custom
        /// </summary>
        public string FormalCustom { get; set; }

        /// <summary>
        /// 字段说明:别名
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:other_name
        /// </summary>
        public string OtherName { get; set; }

        /// <summary>
        /// 字段说明:别名拼音码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:other_spell
        /// </summary>
        public string OtherSpell { get; set; }

        /// <summary>
        /// 字段说明:别名五笔码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:other_wb
        /// </summary>
        public string OtherWb { get; set; }

        /// <summary>
        /// 字段说明:别名自定义码
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:other_custom
        /// </summary>
        public string OtherCustom { get; set; }

        /// <summary>
        /// 字段说明:英文通用名
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:english_regular
        /// </summary>
        public string EnglishRegular { get; set; }

        /// <summary>
        /// 字段说明:英文别名
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:english_other
        /// </summary>
        public string EnglishOther { get; set; }

        /// <summary>
        /// 字段说明:英文名
        /// 数据类型:VARCHAR2
        /// 字段长度:60
        /// 是否可空:是
        /// 字段名称:english_name
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 字段说明:国际编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:international_code
        /// </summary>
        public string InternationalCode { get; set; }

        /// <summary>
        /// 字段说明:国家编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:gb_code
        /// </summary>
        public string GbCode { get; set; }

        /// <summary>
        /// 字段说明:系统类别
        /// 数据类型:VARCHAR2
        /// 字段长度:3
        /// 是否可空:是
        /// 字段名称:class_code
        /// </summary>
        public string ClassCode { get; set; }

        /// <summary>
        /// 字段说明:最小费用代码
        /// 数据类型:VARCHAR2
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:fee_code
        /// </summary>
        public string FeeCode { get; set; }

        /// <summary>
        /// 字段说明:药品类别
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:drug_type
        /// </summary>
        public string DrugType { get; set; }

        /// <summary>
        /// 字段说明:药品性质
        /// 数据类型:VARCHAR2
        /// 字段长度:3
        /// 是否可空:是
        /// 字段名称:drug_quality
        /// </summary>
        public string DrugQuality { get; set; }

        /// <summary>
        /// 字段说明:项目等级，1甲类，2乙类，3丙类 HIS4.5整合 存储药品等级(与医生职级对应)
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:item_grade
        /// </summary>
        public string ItemGrade { get; set; }

        /// <summary>
        /// 字段说明:规格
        /// 数据类型:VARCHAR2
        /// 字段长度:32
        /// 是否可空:是
        /// 字段名称:specs
        /// </summary>
        public string Specs { get; set; }

        /// <summary>
        /// 字段说明:参考零售价
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:retail_price
        /// </summary>
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// 字段说明:参考批发价
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:wholesale_price
        /// </summary>
        public decimal WholesalePrice { get; set; }

        /// <summary>
        /// 字段说明:最新购入价
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:purchase_price
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// 字段说明:最高零售价
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:top_retailprice
        /// </summary>
        public decimal TopRetailprice { get; set; }

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
        /// 字段说明:剂型编码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:dose_model_code
        /// </summary>
        public string DoseModelCode { get; set; }

        /// <summary>
        /// 字段说明:基本剂量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:base_dose
        /// </summary>
        public decimal BaseDose { get; set; }

        /// <summary>
        /// 字段说明:剂量单位
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:dose_unit
        /// </summary>
        public string DoseUnit { get; set; }

        /// <summary>
        /// 字段说明:用法编码
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:usage_code
        /// </summary>
        public string UsageCode { get; set; }

        /// <summary>
        /// 字段说明:频次编码
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:frequency_code
        /// </summary>
        public string FrequencyCode { get; set; }

        /// <summary>
        /// 字段说明:一次用量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:once_dose
        /// </summary>
        public decimal OnceDose { get; set; }

        /// <summary>
        /// 字段说明:注意事项
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:caution
        /// </summary>
        public string Caution { get; set; }

        /// <summary>
        /// 字段说明:一级药理作用
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:是
        /// 字段名称:phy_function1
        /// </summary>
        public string PhyFunction1 { get; set; }

        /// <summary>
        /// 字段说明:二级药理作用
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:是
        /// 字段名称:phy_function2
        /// </summary>
        public string PhyFunction2 { get; set; }

        /// <summary>
        /// 字段说明:三级药理作用
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:是
        /// 字段名称:phy_function3
        /// </summary>
        public string PhyFunction3 { get; set; }

        /// <summary>
        /// 字段说明:有效性标志 1 在用 0 停用 2 废弃
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:valid_state
        /// </summary>
        public string ValidState { get; set; }

        /// <summary>
        /// 字段说明:自制标志 0-非自产，1-自产
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:self_flag
        /// </summary>
        public string SelfFlag { get; set; }

        /// <summary>
        /// 字段说明:OCT标志 0非处方药 1处方药
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:oct_flag
        /// </summary>
        public string OctFlag { get; set; }

        /// <summary>
        /// 字段说明:GMP标志 0非GMP,1GMP
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:gmp_flag
        /// </summary>
        public string GmpFlag { get; set; }

        /// <summary>
        /// 字段说明:是否需要试敏 0不需要1需要
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:test_flag
        /// </summary>
        public string TestFlag { get; set; }

        /// <summary>
        /// 字段说明:新药标记，用户维护1－新药，0－非新药
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:new_flag
        /// </summary>
        public string NewFlag { get; set; }

        /// <summary>
        /// 字段说明:附材标志 0-否，1-是
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:append_flag
        /// </summary>
        public string AppendFlag { get; set; }

        /// <summary>
        /// 字段说明:缺药标志 0-否，1-是
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:lack_flag
        /// </summary>
        public string LackFlag { get; set; }

        /// <summary>
        /// 字段说明:大屏幕显示标记 0非屏幕显示  1为大屏幕显示
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:show_flag
        /// </summary>
        public string ShowFlag { get; set; }

        /// <summary>
        /// 字段说明:招标标志 0非招标1招标
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:tender_flag
        /// </summary>
        public string TenderFlag { get; set; }

        /// <summary>
        /// 字段说明:招标价
        /// 数据类型:NUMBER
        /// 字段长度:14
        /// 是否可空:是
        /// 字段名称:tender_price
        /// </summary>
        public decimal TenderPrice { get; set; }

        /// <summary>
        /// 字段说明:中标公司
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:tender_company
        /// </summary>
        public string TenderCompany { get; set; }

        /// <summary>
        /// 字段说明:中标开始日期
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:tender_begindate
        /// </summary>
        public DateTime TenderBegindate { get; set; }

        /// <summary>
        /// 字段说明:中标结束日期
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:tender_enddate
        /// </summary>
        public DateTime TenderEnddate { get; set; }

        /// <summary>
        /// 字段说明:最新供药公司(在入库时更新，用于生成药品采购单)
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:company_code
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 字段说明:价格形式
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:price_form
        /// </summary>
        public string PriceForm { get; set; }

        /// <summary>
        /// 字段说明:招标采购合同编号
        /// 数据类型:VARCHAR2
        /// 字段长度:30
        /// 是否可空:是
        /// 字段名称:contract_code
        /// </summary>
        public string ContractCode { get; set; }

        /// <summary>
        /// 字段说明:产地
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:producing_area
        /// </summary>
        public string ProducingArea { get; set; }

        /// <summary>
        /// 字段说明:生产厂家
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:producer_code
        /// </summary>
        public string ProducerCode { get; set; }

        /// <summary>
        /// 字段说明:批文信息
        /// 数据类型:VARCHAR2
        /// 字段长度:32
        /// 是否可空:是
        /// 字段名称:approve_info
        /// </summary>
        public string ApproveInfo { get; set; }

        /// <summary>
        /// 字段说明:商标
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 字段说明:有效成分
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:ingredient
        /// </summary>
        public string Ingredient { get; set; }

        /// <summary>
        /// 字段说明:执行标准
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:execute_standard
        /// </summary>
        public string ExecuteStandard { get; set; }

        /// <summary>
        /// 字段说明:储藏条件
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:store_condition
        /// </summary>
        public string StoreCondition { get; set; }

        /// <summary>
        /// 字段说明:药品简介
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:brief_introduction
        /// </summary>
        public string BriefIntroduction { get; set; }

        /// <summary>
        /// 字段说明:说明书内容
        /// 数据类型:VARCHAR2
        /// 字段长度:2000
        /// 是否可空:是
        /// 字段名称:manual
        /// </summary>
        public string Manual { get; set; }

        /// <summary>
        /// 字段说明:条形码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:bar_code
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 字段说明:旧系统药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:old_drug_code
        /// </summary>
        public string OldDrugCode { get; set; }

        /// <summary>
        /// 字段说明:备注
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:mark
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 字段说明:操作员
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:oper_code
        /// </summary>
        public string OperCode { get; set; }

        /// <summary>
        /// 字段说明:操作时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:oper_date
        /// </summary>
        public DateTime OperDate { get; set; }

        /// <summary>
        /// 字段说明:省限制   0不限制 1限制   可通过控制参数进行意义自定义
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:special_flag
        /// </summary>
        public string SpecialFlag { get; set; }

        /// <summary>
        /// 字段说明:市限制   0不限制 1限制   可通过控制参数进行意义自定义
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:special_flag1
        /// </summary>
        public string SpecialFlag1 { get; set; }

        /// <summary>
        /// 字段说明:自费项目  0假 1真            可通过控制参数进行意义自定义
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:special_flag2
        /// </summary>
        public string SpecialFlag2 { get; set; }

        /// <summary>
        /// 字段说明:特限药品标记   0 无限制 1 等级限制  2 特限药品 对特限药品指定了医生/科室
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:special_flag3
        /// </summary>
        public string SpecialFlag3 { get; set; }

        /// <summary>
        /// 字段说明:特殊标记
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:special_flag4
        /// </summary>
        public string SpecialFlag4 { get; set; }

        /// <summary>
        /// 字段说明:变动类型(U更新, M特殊修改 ,N新药, S停用, A调价)
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:shift_type
        /// </summary>
        public string ShiftType { get; set; }

        /// <summary>
        /// 字段说明:变动时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:shift_date
        /// </summary>
        public DateTime ShiftDate { get; set; }

        /// <summary>
        /// 字段说明:变动原因
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:shift_mark
        /// </summary>
        public string ShiftMark { get; set; }

        /// <summary>
        /// 字段说明:药品外观图片
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:trade_picture
        /// </summary>
        public string TradePicture { get; set; }

        /// <summary>
        /// 字段说明:门诊住院临嘱拆分类型:0：最小单位总量取整 1：包装单位总量取整2：最小单位每次取整 3：包装单位每次取整
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:split_type
        /// </summary>
        public string SplitType { get; set; }

        /// <summary>
        /// 字段说明:协定处方标志:0不是 1是
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:nostrum_flag
        /// </summary>
        public string NostrumFlag { get; set; }

        /// <summary>
        /// 字段说明:扩展数据1(外延药品标志)
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:extend1
        /// </summary>
        public string Extend1 { get; set; }

        /// <summary>
        /// 字段说明:扩展数据2
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:extend2
        /// </summary>
        public string Extend2 { get; set; }

        /// <summary>
        /// 字段说明:字典建立时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:create_time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 字段说明:drug_typetwo
        /// 数据类型:VARCHAR2
        /// 字段长度:5
        /// 是否可空:是
        /// 字段名称:drug_typetwo
        /// </summary>
        public string DrugTypetwo { get; set; }

        /// <summary>
        /// 字段说明:参考零售价2
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:retail_price2
        /// </summary>
        public decimal RetailPrice2 { get; set; }

        /// <summary>
        /// 字段说明:预留数字01
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:ext_num1
        /// </summary>
        public decimal ExtNum1 { get; set; }

        /// <summary>
        /// 字段说明:预留数字02
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:ext_num2
        /// </summary>
        public decimal ExtNum2 { get; set; }

        /// <summary>
        /// 字段说明:0-本院字典 1-自营药房字典
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:extend3
        /// </summary>
        public string Extend3 { get; set; }

        /// <summary>
        /// 字段说明:扩展数据4
        /// 数据类型:VARCHAR2
        /// 字段长度:6
        /// 是否可空:是
        /// 字段名称:extend4
        /// </summary>
        public string Extend4 { get; set; }

        /// <summary>
        /// 字段说明:住院长嘱拆分
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:cdsplit_type
        /// </summary>
        public string CdsplitType { get; set; }

        /// <summary>
        /// 字段说明:住院临瞩拆分
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:lzsplit_type
        /// </summary>
        public string LzsplitType { get; set; }

        /// <summary>
        /// 字段说明:第二基本剂量
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:second_base_dose
        /// </summary>
        public decimal SecondBaseDose { get; set; }

        /// <summary>
        /// 字段说明:第二剂量单位
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:second_dose_unit
        /// </summary>
        public string SecondDoseUnit { get; set; }

        /// <summary>
        /// 字段说明:once_dose_unit
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:once_dose_unit
        /// </summary>
        public string OnceDoseUnit { get; set; }

        /// <summary>
        /// 字段说明:产品ID
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:productid
        /// </summary>
        public string Productid { get; set; }

        /// <summary>
        /// 字段说明:大包装单位
        /// 数据类型:VARCHAR2
        /// 字段长度:16
        /// 是否可空:是
        /// 字段名称:bigpackunit
        /// </summary>
        public string Bigpackunit { get; set; }

        /// <summary>
        /// 字段说明:大包装数量
        /// 数据类型:NUMBER
        /// 字段长度:4
        /// 是否可空:是
        /// 字段名称:bigpackqty
        /// </summary>
        public decimal Bigpackqty { get; set; }

        /// <summary>
        /// 字段说明:抗肿瘤药物
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:controlsdrug
        /// </summary>
        public string Controlsdrug { get; set; }

        /// <summary>
        /// 字段说明:集中采购药品标识(1，中选药品)
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:controlslevel
        /// </summary>
        public string Controlslevel { get; set; }

        /// <summary>
        /// 字段说明:谈判药品
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:negotiated_drugs
        /// </summary>
        public string NegotiatedDrugs { get; set; }

        /// <summary>
        /// 字段说明:出院带药拆分
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:cydysplit_type
        /// </summary>
        public string CydysplitType { get; set; }

    }
}
