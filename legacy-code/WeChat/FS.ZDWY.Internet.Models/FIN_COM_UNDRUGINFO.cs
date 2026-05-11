using SqlSugar;
using System;
using System.Linq;
using System.Text;

namespace FS.ZDWY.Internet.Models
{
    ///<summary>
    ///非药品信息表
    ///</summary>
    public class FIN_COM_UNDRUGINFO
    {
        public FIN_COM_UNDRUGINFO()
        {


        }
        /// <summary>
        /// Desc:非药品编码
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)] //设置主键
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// Desc:非药品名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_NAME { get; set; }

        /// <summary>
        /// Desc:系统类别
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SYS_CLASS { get; set; }

        /// <summary>
        /// Desc:最小费用代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FEE_CODE { get; set; }

        /// <summary>
        /// Desc:输入码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INPUT_CODE { get; set; }

        /// <summary>
        /// Desc:拼音码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPELL_CODE { get; set; }

        /// <summary>
        /// Desc:五笔
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string WB_CODE { get; set; }

        /// <summary>
        /// Desc:国家编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string GB_CODE { get; set; }

        /// <summary>
        /// Desc:国际标准代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INTERNATIONAL_CODE { get; set; }

        /// <summary>
        /// Desc:三甲价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? UNIT_PRICE { get; set; }

        /// <summary>
        /// Desc:儿童价(校区材料价)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? UNIT_PRICE1 { get; set; }

        /// <summary>
        /// Desc:特诊价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? UNIT_PRICE2 { get; set; }

        /// <summary>
        /// Desc:住院MDT价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? UNIT_PRICE3 { get; set; }

        /// <summary>
        /// Desc:单价2
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? UNIT_PRICE4 { get; set; }

        /// <summary>
        /// Desc:急诊加成比例
        /// Default:
        /// Nullable:True
        /// </summary>           
        public Single? EMERG_SCALE { get; set; }

        /// <summary>
        /// Desc:单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string STOCK_UNIT { get; set; }

        /// <summary>
        /// Desc:省限制          0不限制 1限制
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_FLAG { get; set; }

        /// <summary>
        /// Desc:市限制          0不限制 1限制
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_FLAG1 { get; set; }

        /// <summary>
        /// Desc:自费项目        0假 1真
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_FLAG2 { get; set; }

        /// <summary>
        /// Desc:特定治疗项目        0假 1真
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_FLAG3 { get; set; }

        /// <summary>
        /// Desc:中山一：是否强制出单
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_FLAG4 { get; set; }

        /// <summary>
        /// Desc:计划生育标记 0假 1真
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FAMILY_PLANE { get; set; }

        /// <summary>
        /// Desc:望海-是否条码管理
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_ITEM { get; set; }

        /// <summary>
        /// Desc:甲乙类标志 1 甲 2 乙
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_GRADE { get; set; }

        /// <summary>
        /// Desc:确认标志  0均不确认;1门诊住院确认;2门诊确认;3住院确认
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CONFIRM_FLAG { get; set; }

        /// <summary>
        /// Desc:有效性标识 1 在用 0停用 2 废弃
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string VALID_STATE { get; set; }

        /// <summary>
        /// Desc:规格
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECS { get; set; }

        /// <summary>
        /// Desc:允许选择的执行科室，为空则不限制
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXEDEPT_CODE { get; set; }

        /// <summary>
        /// Desc:设备编号 用 | 区分
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FACILITY_NO { get; set; }

        /// <summary>
        /// Desc:默认检查部位或标本
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DEFAULT_SAMPLE { get; set; }

        /// <summary>
        /// Desc:手术编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPERATE_CODE { get; set; }

        /// <summary>
        /// Desc:手术分类
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPERATE_KIND { get; set; }

        /// <summary>
        /// Desc:手术规模
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPERATE_TYPE { get; set; }

        /// <summary>
        /// Desc:是否有物资项目与之对照(1有，0没有)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string COLLATE_FLAG { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARK { get; set; }

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
        /// Desc:疾病分类(开立检验项目时使用)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DISEASE_CLASS { get; set; }

        /// <summary>
        /// Desc:专科名称(开立检验项目时使用)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIAL_DEPT { get; set; }

        /// <summary>
        /// Desc:是否需要打印知情同意书（0需要，1不需要）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CONSENT_FLAG { get; set; }

        /// <summary>
        /// Desc:病史及检查(开立检查申请单时使用)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARK1 { get; set; }

        /// <summary>
        /// Desc:检查要求(开立检查申请单时使用)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARK2 { get; set; }

        /// <summary>
        /// Desc:注意事项(开立检查申请单时使用)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARK3 { get; set; }

        /// <summary>
        /// Desc:检查申请单名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARK4 { get; set; }

        /// <summary>
        /// Desc:是否需要预约 1 需要 0 不需要
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NEEDBESPEAK { get; set; }

        /// <summary>
        /// Desc:项目范围
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_AREA { get; set; }

        /// <summary>
        /// Desc:项目例外
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_NOAREA { get; set; }

        /// <summary>
        /// Desc:单位标识(0,明细; 1,组套)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string UNITFLAG { get; set; }

        /// <summary>
        /// Desc:适用范围 0 全部  1 门诊 2住院   具体解释看常数维护 APPLICABILITYAREA
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string APPLICABILITYAREA { get; set; }

        /// <summary>
        /// Desc:允许开立该非药品的科室列表，为ALL表示所有科室都有权限
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DEPT_LIST { get; set; }

        /// <summary>
        /// Desc:病案分类（物价费用类别（物价局定义的分类））
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_PRICETYPE { get; set; }

        /// <summary>
        /// Desc:该项目是否打印医嘱单
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ORDERPRINT_TAG { get; set; }

        /// <summary>
        /// Desc:别名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OTHER_NAME { get; set; }

        /// <summary>
        /// Desc:别名拼音码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OTHER_SPELL { get; set; }

        /// <summary>
        /// Desc:别名五笔码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OTHER_WB { get; set; }

        /// <summary>
        /// Desc:别名自定义码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OTHER_CUSTOM { get; set; }

        /// <summary>
        /// Desc:英文名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ENGLISH_NAME { get; set; }

        /// <summary>
        /// Desc:英文别名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ENGLISH_OTHER { get; set; }

        /// <summary>
        /// Desc:英文通用名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ENGLISH_REGULAR { get; set; }

        /// <summary>
        /// Desc:停用原因
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string STOP_RESEON { get; set; }

        /// <summary>
        /// Desc:医技类型编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MTTYPECODE { get; set; }

        /// <summary>
        /// Desc:医技类型名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MTTYPENAME { get; set; }

        /// <summary>
        /// Desc:生产厂家
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PRODUCER_INFO { get; set; }

        /// <summary>
        /// Desc:注册码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string REGISTER_CODE { get; set; }

        /// <summary>
        /// Desc:注册时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? REGISTER_DATE { get; set; }

        /// <summary>
        /// Desc:门诊默认执行科室
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXEC_DEPT_OUT { get; set; }

        /// <summary>
        /// Desc:住院默认执行科室
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXEC_DEPT_IN { get; set; }

        /// <summary>
        /// Desc:高值耗材
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string IS_HIGHVALUE { get; set; }

        /// <summary>
        /// Desc:门诊MDT价格
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? MDT_PRICE { get; set; }

        /// <summary>
        /// Desc:校区价（服务项目）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? SCHOOL_PRICE { get; set; }

        /// <summary>
        /// Desc:围产中心价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? WEICHAN_PRICE { get; set; }

        /// <summary>
        /// Desc:性别限制（0全部，1男，2女）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SEXAREA { get; set; }

        /// <summary>
        /// Desc:是否是6岁儿童加收项目
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MARK5 { get; set; }

        /// <summary>
        /// Desc:是否可收费，1可收费0不可收费
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CHARGE_FLAG { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SIXYEARSOLDCOUNTRYCODE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECIALNEEDSCOUNTRYCODE { get; set; }

        /// <summary>
        /// Desc:申请单类型编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string APPLY_LIST_TYPE { get; set; }

    }
}
