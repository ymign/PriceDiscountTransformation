using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy
{
    /// <summary>
    /// 医保进销存实体对象
    /// </summary>
    public class ProductInventory
    {
         /// <summary>
        /// 数据主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 业务流水号[门诊或住院流水号]
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 患者名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 药品编码
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 药品名称
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 申请流水号[pha_com_applyout表中主键]
        /// </summary>
        public string ApplyNumber { get; set; }

        /// <summary>
        /// 医嘱流水号
        /// </summary>
        public string MoOrder { get; set; }

        /// <summary>
        /// 医疗目录编码
        /// </summary>
        public string MedListCode { get; set; }

        /// <summary>
        /// 定点医药机构目录编号
        /// </summary>
        public string FixMedInsHistId { get; set; }

        /// <summary>
        /// 定点医药机构目录名称
        /// </summary>
        public string FixMedInsHistName { get; set; }

        /// <summary>
        /// 定点医药机构批次流水号
        /// </summary>
        public string FixMedInsBchNo { get; set; }

        /// <summary>
        /// 开方医师证件类型
        /// </summary>
        public string PrsCdRCertType { get; set; }

        /// <summary>
        /// 开方医师证件号码
        /// </summary>
        public string PrsCdRCertNo { get; set; }

        /// <summary>
        /// 开方医师姓名
        /// </summary>
        public string PrsCdrName { get; set; }

        /// <summary>
        /// 药师证件类型
        /// </summary>
        public string PharCertType { get; set; }

        /// <summary>
        /// 药师证件号码
        /// </summary>
        public string PharCertNo { get; set; }

        /// <summary>
        /// 药师姓名
        /// </summary>
        public string PharName { get; set; }

        /// <summary>
        /// 药师执业资格证号
        /// </summary>
        public string PharPracCertNo { get; set; }

        /// <summary>
        /// 医保费用结算类型 0非医疗保险结算  1本地医疗保险结算
        /// </summary>
        public string HIFeeSetlType { get; set; }

        /// <summary>
        /// 结算ID
        /// </summary>
        public string SetlId { get; set; }

        /// <summary>
        /// 就医流水号 医保结算时为MDTRT_ID，自费结算时为医疗机构内就诊流水号
        /// </summary>
        public string MdtrTsN { get; set; }

        /// <summary>
        /// 人员编号
        /// </summary>
        public string PsnNo { get; set; }

        /// <summary>
        /// 人员证件类型
        /// </summary>
        public string PsnCertType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string PsnName { get; set; }

        /// <summary>
        /// 生产批号
        /// </summary>
        public string ManuLotNum { get; set; }

        /// <summary>
        /// 生产日期 yyyy-MM-dd
        /// </summary>
        public string ManuDate { get; set; }

        /// <summary>
        /// 有效期止 yyyy-MM-dd
        /// </summary>
        public string ExpyEnd { get; set; }

        /// <summary>
        /// 处方药标志 0非处方药 1处方药
        /// </summary>
        public string RxFlag { get; set; }

        /// <summary>
        /// 拆零标志  0否 1是
        /// </summary>
        public string TrdnFlag { get; set; }

        /// <summary>
        /// 最终成交单价
        /// </summary>
        public string FinlTrnsPric { get; set; }

        /// <summary>
        /// 处方号
        /// </summary>
        public string RxNo { get; set; }

        /// <summary>
        /// 外购处方标志 0否 1是
        /// </summary>
        public string RxCircFlag { get; set; }

        /// <summary>
        /// 零售单据号
        /// </summary>
        public string RtalDocNo { get; set; }

        /// <summary>
        /// 销售出库单据号
        /// </summary>
        public string StoOutNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BchNo { get; set; }

        /// <summary>
        /// 药品追溯码
        /// </summary>
        public string DrugTracCodg { get; set; }

        /// <summary>
        /// 药品条形码
        /// </summary>
        public string DrugProdBarc { get; set; }

        /// <summary>
        /// 货架位
        /// </summary>
        public string ShelfPosi { get; set; }

        /// <summary>
        /// 销售/退货数量
        /// </summary>
        public string SelRetnCnt { get; set; }

        /// <summary>
        /// 销售/退货时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string SelRetnTime { get; set; }

        /// <summary>
        /// 销售/退货经办人姓名
        /// </summary>
        public string SelRetnOperName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 就诊结算类型 1医保结算 2自费结算
        /// </summary>
        public string MdtrtSetlType { get; set; }

        /// <summary>
        /// 参保地医保区划
        /// </summary>
        public string InsuplCadmDvs { get; set; }

        /// <summary>
        /// 就医地医保区划
        /// </summary>
        public string MdtrtAreaAdmVs { get; set; }

        /// <summary>
        /// 经办人类别  1-经办人；2-自助终端；3-移动终端
        /// </summary>
        public string OpterType { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public string Opter { get; set; }

        /// <summary>
        /// 经办人姓名
        /// </summary>
        public string OpterName { get; set; }

        /// <summary>
        /// 定点医疗机构编号
        /// </summary>
        public string FixMedInsCode { get; set; }

        /// <summary>
        /// 定点医药机构名称
        /// </summary>
        public string FixMedInsName { get; set; }

        /// <summary>
        /// 推送类型 0销售 1退货
        /// </summary>
        public string SendType { get; set; }

        /// <summary>
        /// 推送标志 0未推送 1推送成功 2推送失败
        /// </summary>
        public string SendFlag { get; set; } 

        /// <summary>
        /// 删除标志 0未删除 1已删除
        /// </summary>
        public string DeleteFlag { get; set; } 

        /// <summary>
        /// 有效标志 0无效 1有效
        /// </summary>
        public string ValidFlag { get; set; } 

        /// <summary>
        /// 创建人工号
        /// </summary>
        public string CreatedCode { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatedName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 操作人工号
        /// </summary>
        public string OpterCode { get; set; }

        /// <summary>
        /// 操作人名称
        /// </summary>
        public string Opter_Name { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }

        /// <summary>
        /// 修改人工号
        /// </summary>
        public string UpdatedCode { get; set; }

        /// <summary>
        /// 修改人名称
        /// </summary>
        public string UpdatedName { get; set; }

        /// <summary>
        /// 数据备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 拓展字段1
        /// </summary>
        public string Backup1 { get; set; }

        /// <summary>
        /// 拓展字段2
        /// </summary>
        public string Backup2 { get; set; }

        /// <summary>
        /// 拓展字段3
        /// </summary>
        public string Backup3 { get; set; }

        /// <summary>
        /// 数据类型 0门诊 1住院
        /// </summary>
        public string DataType { get; set; }
    }
}
