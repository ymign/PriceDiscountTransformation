using System;
using System.Linq;
using System.Text;

namespace FS.ZDWY.Internet.Models
{
    ///<summary>
    ///处方明细表
    ///</summary>
    public class FIN_OPB_FEEDETAIL
    {
        public FIN_OPB_FEEDETAIL()
        {


        }
        /// <summary>
        /// Desc:处方号[3]
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string RECIPE_NO { get; set; }

        /// <summary>
        /// Desc:处方内项目流水号[4]
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int SEQUENCE_NO { get; set; }

        /// <summary>
        /// Desc:交易类型,1正交易，2反交易[5]
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string TRANS_TYPE { get; set; }

        /// <summary>
        /// Desc:门诊号[6]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CLINIC_CODE { get; set; }

        /// <summary>
        /// Desc:病历卡号[7]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CARD_NO { get; set; }

        /// <summary>
        /// Desc:挂号日期[8]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? REG_DATE { get; set; }

        /// <summary>
        /// Desc:开单科室[9]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string REG_DPCD { get; set; }

        /// <summary>
        /// Desc:开方医师[10]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DOCT_CODE { get; set; }

        /// <summary>
        /// Desc:开方医师所在科室[11]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DOCT_DEPT { get; set; }

        /// <summary>
        /// Desc:项目代码[12]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// Desc:项目名称[13]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_NAME { get; set; }

        /// <summary>
        /// Desc:1药品/0非要[14]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DRUG_FLAG { get; set; }

        /// <summary>
        /// Desc:规格[15]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SPECS { get; set; }

        /// <summary>
        /// Desc:自制药标志[16]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SELF_MADE { get; set; }

        /// <summary>
        /// Desc:药品性质，麻药，普药[17]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DRUG_QUALITY { get; set; }

        /// <summary>
        /// Desc:剂型[18]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DOSE_MODEL_CODE { get; set; }

        /// <summary>
        /// Desc:最小费用代码[19]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FEE_CODE { get; set; }

        /// <summary>
        /// Desc:系统类别[20]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CLASS_CODE { get; set; }

        /// <summary>
        /// Desc:单价[21]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? UNIT_PRICE { get; set; }

        /// <summary>
        /// Desc:数量[22]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? QTY { get; set; }

        /// <summary>
        /// Desc:草药的付数，其他药品为1[23]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public short? DAYS { get; set; }

        /// <summary>
        /// Desc:频次代码[24]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FREQUENCY_CODE { get; set; }

        /// <summary>
        /// Desc:用法代码[25]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string USAGE_CODE { get; set; }

        /// <summary>
        /// Desc:用法名称[26]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string USE_NAME { get; set; }

        /// <summary>
        /// Desc:院内注射次数[27]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public short? INJECT_NUMBER { get; set; }

        /// <summary>
        /// Desc:加急标记:1普通/2加急[28]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EMC_FLAG { get; set; }

        /// <summary>
        /// Desc:样本类型[29]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string LAB_TYPE { get; set; }

        /// <summary>
        /// Desc:检体[30]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CHECK_BODY { get; set; }

        /// <summary>
        /// Desc:每次用量[31]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? DOSE_ONCE { get; set; }

        /// <summary>
        /// Desc:每次用量单位[32]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DOSE_UNIT { get; set; }

        /// <summary>
        /// Desc:基本剂量[33]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? BASE_DOSE { get; set; }

        /// <summary>
        /// Desc:包装数量[34]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public short? PACK_QTY { get; set; }

        /// <summary>
        /// Desc:计价单位[35]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PRICE_UNIT { get; set; }

        /// <summary>
        /// Desc:可报效金额[36]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? PUB_COST { get; set; }

        /// <summary>
        /// Desc:自付金额[37]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? PAY_COST { get; set; }

        /// <summary>
        /// Desc:现金金额[38]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? OWN_COST { get; set; }

        /// <summary>
        /// Desc:执行科室代码[39]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXEC_DPCD { get; set; }

        /// <summary>
        /// Desc:执行科室名称[40]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXEC_DPNM { get; set; }

        /// <summary>
        /// Desc:医保中心项目代码[41]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CENTER_CODE { get; set; }

        /// <summary>
        /// Desc:项目等级，1甲类，2乙类，3丙类[42]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ITEM_GRADE { get; set; }

        /// <summary>
        /// Desc:主药标志[43]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MAIN_DRUG { get; set; }

        /// <summary>
        /// Desc:组合号[44]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string COMB_NO { get; set; }

        /// <summary>
        /// Desc:划价人[45]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPER_CODE { get; set; }

        /// <summary>
        /// Desc:划价时间[46]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// Desc:0划价 1收费 3预收费团体体检 4 药品预审核
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PAY_FLAG { get; set; }

        /// <summary>
        /// Desc:0退费，1正常，2重打，3注销[48]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CANCEL_FLAG { get; set; }

        /// <summary>
        /// Desc:收费员代码[49]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FEE_CPCD { get; set; }

        /// <summary>
        /// Desc:收费日期[50]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? FEE_DATE { get; set; }

        /// <summary>
        /// Desc:票据号[51]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INVOICE_NO { get; set; }

        /// <summary>
        /// Desc:发票科目代码[52]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INVO_CODE { get; set; }

        /// <summary>
        /// Desc:发票内流水号[53]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string INVO_SEQUENCE { get; set; }

        /// <summary>
        /// Desc:0未确认/1确认[54]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CONFIRM_FLAG { get; set; }

        /// <summary>
        /// Desc:确认人[55]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CONFIRM_CODE { get; set; }

        /// <summary>
        /// Desc:确认科室[56]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CONFIRM_DEPT { get; set; }

        /// <summary>
        /// Desc:确认时间[57]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? CONFIRM_DATE { get; set; }

        /// <summary>
        /// Desc:优惠金额[58]
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? ECO_COST { get; set; }

        /// <summary>
        /// Desc:发票序号，一次结算产生多张发票的combNo
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string INVOICE_SEQ { get; set; }

        /// <summary>
        /// Desc:新项目比例
        /// Default:
        /// Nullable:True
        /// </summary>           
        public Single? NEW_ITEMRATE { get; set; }

        /// <summary>
        /// Desc:原项目比例
        /// Default:
        /// Nullable:True
        /// </summary>           
        public Single? OLD_ITEMRATE { get; set; }

        /// <summary>
        /// Desc:扩展标志 特殊项目标志 1 0 非
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXT_FLAG { get; set; }

        /// <summary>
        /// Desc:0 正常/1个人体检/2 集体体检
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXT_FLAG1 { get; set; }

        /// <summary>
        /// Desc:日结标志：0：未日结/1：已日结
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXT_FLAG2 { get; set; }

        /// <summary>
        /// Desc:1 包装 单位 0, 最小单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACT_UNIT_FLAG { get; set; }

        /// <summary>
        /// Desc:复合项目代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACKAGE_CODE { get; set; }

        /// <summary>
        /// Desc:复合项目名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACKAGE_NAME { get; set; }

        /// <summary>
        /// Desc:可退数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? NOBACK_NUM { get; set; }

        /// <summary>
        /// Desc:确认数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public Single? CONFIRM_NUM { get; set; }

        /// <summary>
        /// Desc:已确认院注次数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public short? CONFIRM_INJECT { get; set; }

        /// <summary>
        /// Desc:医嘱项目流水号或者体检项目流水号
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string MO_ORDER { get; set; }

        /// <summary>
        /// Desc:条码号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SAMPLE_ID { get; set; }

        /// <summary>
        /// Desc:收费序列
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string RECIPE_SEQ { get; set; }

        /// <summary>
        /// Desc:超标金额
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? OVER_COST { get; set; }

        /// <summary>
        /// Desc:药品超标金额
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? EXCESS_COST { get; set; }

        /// <summary>
        /// Desc:自费药金额
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? DRUG_OWNCOST { get; set; }

        /// <summary>
        /// Desc:费用来源 0 操作员 1 医嘱 2 终端 3 体检
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string COST_SOURCE { get; set; }

        /// <summary>
        /// Desc:附材标志
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SUBJOB_FLAG { get; set; }

        /// <summary>
        /// Desc:0没有扣账户 1 已经扣账户
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ACCOUNT_FLAG { get; set; }

        /// <summary>
        /// Desc:更新库存的流水号(物资)
        /// Default:
        /// Nullable:True
        /// </summary>           
        public long? UPDATE_SEQUENCENO { get; set; }

        /// <summary>
        /// Desc:医生所属科室
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DOCTINDEPT { get; set; }

        /// <summary>
        /// Desc:医疗组代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MEDICALGROUPCODE { get; set; }

        /// <summary>
        /// Desc:结算类别编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PAYKIND_CODE { get; set; }

        /// <summary>
        /// Desc:合同单位编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PACT_CODE { get; set; }

        /// <summary>
        /// Desc:项目原始价格（最小单位价格）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? OLD_UNIT_PRICE { get; set; }

        /// <summary>
        /// Desc:复合项目明细的数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public Single? PACKAGE_QTY { get; set; }

        /// <summary>
        /// Desc:处方备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string RECIPE_MEMO { get; set; }

        /// <summary>
        /// Desc:费用备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MEMO { get; set; }

        /// <summary>
        /// Desc:中大五院自助设备打印标志
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXT_FLAG3 { get; set; }

        /// <summary>
        /// Desc:开立医生所属科室
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string BELONG_DEPT { get; set; }

        /// <summary>
        /// Desc:医院编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HOS_CODE { get; set; }

        /// <summary>
        /// Desc:体检专用
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string TJ_CODE { get; set; }

        /// <summary>
        /// Desc:执行科室2
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXEC_DPCD2 { get; set; }

        /// <summary>
        /// Desc:收费员收费时登录科室ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPER_INDEPT { get; set; }

        /// <summary>
        /// Desc:收费员收费时登录科室名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string OPER_INDEPTNAME { get; set; }

        /// <summary>
        /// Desc:非药品退费与还原操作工号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string QUITUNDRUGOPERCODE { get; set; }

        /// <summary>
        /// Desc:非药品退费与还原操作时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? QUITUNDRUGOPERDATE { get; set; }

        /// <summary>
        /// Desc:app平台预结算号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PRECALID { get; set; }

        /// <summary>
        /// Desc:是否处方外延标记：1 是；0或者空 不是
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EXTEND_FLAG { get; set; }

        /// <summary>
        /// Desc:是否日间手术术前检查项目
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DAY_OPERATION_FLAG { get; set; }

        /// <summary>
        /// Desc:数据来源;null/1:HIS,2:急诊系统,3:EMR,4:其它系统
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SOURCE_FLAG { get; set; }

        /// <summary>
        /// Desc:是否推送平台标记;0:否,1:是
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PUSH_PLATFORM { get; set; }

        /// <summary>
        /// Desc:转住院标志 1为已转
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string TRANSFER_FLAG { get; set; }

        /// <summary>
        /// make输血系统记账Id ( 唯一值, 字符串类型, 重复插入的时候His要判断, 需正常返回成功, 并返回His的费用Id, 用于核对费用)
        /// </summary>
        public string MAKEACCOUNTITEMID { get; set; }

        /// <summary>
        /// his对接make输血系统产生的主键ID  生成规则 HIS+MAKEACCOUNTITEMID 
        /// </summary>
        public string HISACCOUNTITEMID { get; set; }

        /// <summary>
        /// 用血申请单号
        /// </summary>
        public string BloodApplyBillNo { get; set; }
    }
}
