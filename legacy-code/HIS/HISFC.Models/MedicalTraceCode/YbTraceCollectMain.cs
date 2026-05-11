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
    /// 表名注释：医保追溯码采集记录主表
    /// 数据表名：yb_trace_collect_main
    /// 生成时间：2025-07-17 16:56:49
    /// </summary>
    public class YbTraceCollectMain
    {
        /// <summary>
        /// 字段说明:主键
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 字段说明:业务流水号[门诊或住院流水号]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:serial_no
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 字段说明:门诊号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:card_no
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 字段说明:患者名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:patient_name
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 字段说明:住院号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:patient_no
        /// </summary>
        public string PatientNo { get; set; }

        /// <summary>
        /// 字段说明:药品编码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:drug_code
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 字段说明:药品名称
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:否
        /// 字段名称:drug_name
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 字段说明:药品规格
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:drug_specs
        /// </summary>
        public string DrugSpecs { get; set; }

        /// <summary>
        /// 字段说明:药品自定义码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:drug_custom_code
        /// </summary>
        public string DrugCustomCode { get; set; }

        /// <summary>
        /// 字段说明:药品包装单位
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:drug_pact_unit
        /// </summary>
        public string DrugPactUnit { get; set; }

        /// <summary>
        /// 字段说明:药品包装数量
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:drug_pact_qty
        /// </summary>
        public string DrugPactQty { get; set; }

        /// <summary>
        /// 字段说明:药品最小单位
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:drug_min_unit
        /// </summary>
        public string DrugMinUnit { get; set; }

        /// <summary>
        /// 字段说明:药房编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:pharmacy_code
        /// </summary>
        public string PharmacyCode { get; set; }

        /// <summary>
        /// 字段说明:药房名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:pharmacy_name
        /// </summary>
        public string PharmacyName { get; set; }

        /// <summary>
        /// 字段说明:科室编码
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:dept_code
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 字段说明:科室名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:dept_name
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 字段说明:申请流水号[pha_com_applyout表中主键]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:apply_number
        /// </summary>
        public string ApplyNumber { get; set; }

        /// <summary>
        /// 字段说明:医嘱流水号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:mo_order_no
        /// </summary>
        public string MoOrderNo { get; set; }

        /// <summary>
        /// 字段说明:医嘱执行流水号[住院患者专有]
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:exec_order_no
        /// </summary>
        public string ExecOrderNo { get; set; }

        /// <summary>
        /// 字段说明:发票号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:invoice_no
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 字段说明:采集时维护的标识码映射集合
        /// 数据类型:VARCHAR2
        /// 字段长度:1000
        /// 是否可空:是
        /// 字段名称:identifiy_code_list
        /// </summary>
        public string IdentifiyCodeList { get; set; }

        /// <summary>
        /// 字段说明:采集时对应的标识码
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:identifiy_code
        /// </summary>
        public string IdentifiyCode { get; set; }

        /// <summary>
        /// 字段说明:是否含有拆零 0否 1是
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:is_have_split
        /// </summary>
        public string IsHaveSplit { get; set; }

        /// <summary>
        /// 字段说明:是否含有包装 0否 1是
        /// 数据类型:VARCHAR2
        /// 字段长度:1
        /// 是否可空:是
        /// 字段名称:is_have_pact
        /// </summary>
        public string IsHavePact { get; set; }

        /// <summary>
        /// 字段说明:拆零单位
        /// 数据类型:VARCHAR2
        /// 字段长度:5
        /// 是否可空:是
        /// 字段名称:drug_split_unit
        /// </summary>
        public string DrugSplitUnit { get; set; }

        /// <summary>
        /// 字段说明:包装追溯码,多个以;分割
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:pact_trac_codgs
        /// </summary>
        public string PactTracCodgs { get; set; }

        /// <summary>
        /// 字段说明:包装需采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:pact_need_collect_qty
        /// </summary>
        public decimal PactNeedCollectQty { get; set; }

        /// <summary>
        /// 字段说明:包装实际采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:pact_actual_collect_qty
        /// </summary>
        public decimal PactActualCollectQty { get; set; }

        /// <summary>
        /// 字段说明:包装未采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:pact_un_collect_qty
        /// </summary>
        public decimal PactUnCollectQty { get; set; }

        /// <summary>
        /// 字段说明:包装申诉数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:pact_appeal_collect_qty
        /// </summary>
        public decimal PactAppealCollectQty { get; set; }

        /// <summary>
        /// 字段说明:包装采集完成率
        /// 数据类型:VARCHAR2(10)
        /// 字段长度:5
        /// 是否可空:是
        /// 字段名称:pact_collect_complete_rate
        /// </summary>
        public string PactCollectCompleteRate { get; set; }

        /// <summary>
        /// 字段说明:包装采集状态 0或空待采集 1采集中 2不用采集 3采集成功 4跳过采集 5采集失败 6采集完成(但未全部采集成功) 7部分采集成功
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:pact_collect_status
        /// </summary>
        public string PactCollectStatus { get; set; }

        /// <summary>
        /// 字段说明:包装采集方式
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:pact_collect_method
        /// </summary>
        public string PactCollectMethod { get; set; }

        /// <summary>
        /// 字段说明:采集开始时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:collect_start_time
        /// </summary>
        public DateTime CollectStartTime { get; set; }

        /// <summary>
        /// 字段说明:采集结束时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:collect_end_time
        /// </summary>
        public DateTime CollectEndTime { get; set; }

        /// <summary>
        /// 字段说明:采集耗时
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:collect_duration_ms
        /// </summary>
        public decimal CollectDurationMs { get; set; }

        /// <summary>
        /// 字段说明:拆零追溯码,多个以;分割
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:split_trac_codgs
        /// </summary>
        public string SplitTracCodgs { get; set; }

        /// <summary>
        /// 字段说明:拆零需采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:split_need_collect_qty
        /// </summary>
        public decimal SplitNeedCollectQty { get; set; }

        /// <summary>
        /// 字段说明:拆零实际采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:split_actual_collect_qty
        /// </summary>
        public decimal SplitActualCollectQty { get; set; }

        /// <summary>
        /// 字段说明:拆零未采集数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:split_un_collect_qty
        /// </summary>
        public decimal SplitUnCollectQty { get; set; }

        /// <summary>
        /// 字段说明:拆零申诉数量
        /// 数据类型:NUMBER
        /// 字段长度:12
        /// 是否可空:是
        /// 字段名称:split_appeal_collect_qty
        /// </summary>
        public decimal SplitAppealCollectQty { get; set; }

        /// <summary>
        /// 字段说明:拆零采集完成率
        /// 数据类型:VARCHAR2(10)
        /// 字段长度:5
        /// 是否可空:是
        /// 字段名称:split_collect_complete_rate
        /// </summary>
        public string SplitCollectCompleteRate { get; set; }

        /// <summary>
        /// 字段说明:拆零采集状态 0或空待采集 1采集中 2不用采集 3采集成功 4跳过采集 5采集失败 6采集完成(但未全部采集成功) 7部分采集成功
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:split_collect_status
        /// </summary>
        public string SplitCollectStatus { get; set; }

        /// <summary>
        /// 字段说明:拆零采集方式
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:split_collect_method
        /// </summary>
        public string SplitCollectMethod { get; set; }

        /// <summary>
        /// 字段说明:采集设备IP
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:collect_ip
        /// </summary>
        public string CollectIp { get; set; }

        /// <summary>
        /// 字段说明:采集类型 0销售 1退货
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:collect_type
        /// </summary>
        public string CollectType { get; set; }

        /// <summary>
        /// 字段说明:采集人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:collect_oper_code
        /// </summary>
        public string CollectOperCode { get; set; }

        /// <summary>
        /// 字段说明:采集人名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:collect_oper_name
        /// </summary>
        public string CollectOperName { get; set; }

        /// <summary>
        /// 字段说明:是否需要生成上传任务 N不需要 Y需要
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_need_upload
        /// </summary>
        public string IsNeedUpload { get; set; }

        /// <summary>
        /// 字段说明:是否已经生成上传任务 N未生成 Y已生成
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_create_upload_task
        /// </summary>
        public string IsCreateUploadTask { get; set; }

        /// <summary>
        /// 字段说明:是否上传 N未上传 Y已上传
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_uploaded
        /// </summary>
        public string IsUploaded { get; set; }

        /// <summary>
        /// 字段说明:生成上传任务时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:upload_task_time
        /// </summary>
        public DateTime UploadTaskTime { get; set; }

        /// <summary>
        /// 字段说明:实际上传完成时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:upload_time
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 字段说明:业务场景 如 门诊发药/出院带药/门诊退药
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:business_scenario
        /// </summary>
        public string BusinessScenario { get; set; }

        /// <summary>
        /// 字段说明:数据来源 如 HIS/智慧园
        /// 数据类型:VARCHAR2
        /// 字段长度:10
        /// 是否可空:否
        /// 字段名称:source_system
        /// </summary>
        public string SourceSystem { get; set; }

        /// <summary>
        /// 字段说明:业务类型 0门诊 1住院
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:否
        /// 字段名称:business_type
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 字段说明:创建人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:created_code
        /// </summary>
        public string CreatedCode { get; set; }

        /// <summary>
        /// 字段说明:创建人名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:created_name
        /// </summary>
        public string CreatedName { get; set; }

        /// <summary>
        /// 字段说明:创建时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:create_time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 字段说明:最近修改人工号
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:modified_by
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 字段说明:最近修改人名称
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:modified_name
        /// </summary>
        public string ModifiedName { get; set; }

        /// <summary>
        /// 字段说明:最近修改时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:modified_time
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 字段说明:是否删除
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_deleted
        /// </summary>
        public string IsDeleted { get; set; }

        /// <summary>
        /// 字段说明:是否有效
        /// 数据类型:VARCHAR2
        /// 字段长度:2
        /// 是否可空:是
        /// 字段名称:is_valid
        /// </summary>
        public string IsValid { get; set; }

        /// <summary>
        /// 字段说明:备注
        /// 数据类型:VARCHAR2
        /// 字段长度:500
        /// 是否可空:是
        /// 字段名称:memo
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 字段说明:拓展字段1  是否工伤 0否 1是
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:ext_field1
        /// </summary>
        public string ExtField1 { get; set; }

        /// <summary>
        /// 字段说明:拓展字段2 结算类别编码 01现金 02医保
        /// 数据类型:VARCHAR2
        /// 字段长度:100 
        /// 是否可空:是
        /// 字段名称:ext_field2
        /// </summary>
        public string ExtField2 { get; set; }

        /// <summary>
        /// 字段说明:拓展字段3 处方号
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:ext_field3
        /// </summary>
        public string ExtField3 { get; set; }

        /// <summary>
        /// 字段说明:院区编码
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:hospital_code
        /// </summary>
        public string HospitalCode { get; set; }

        /// <summary>
        /// 字段说明:院区名称
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:否
        /// 字段名称:hospital_name
        /// </summary>
        public string HospitalName { get; set; }

        #region 非表字段

        private List<YbTraceCollectDetail> detailList = new List<YbTraceCollectDetail>();

        /// <summary>
        /// 字段说明:采集明细集合,方便数据操作 非数据库字段   
        /// </summary>
        public List<YbTraceCollectDetail> DetailList
        {
            get
            {
                return this.detailList;
            }
            set
            {
                this.detailList = value;
            }
        }

        private List<string> pactTracCodgsList = new List<string>();
        /// <summary>
        /// 字段说明:包装追溯码string集合,方便数据操作 非数据库字段  
        /// </summary>
        public List<string> PactTracCodgsList 
        {
            get
            {
                return this.pactTracCodgsList;
            }
            set
            {
                this.pactTracCodgsList = value;
            }
        }

        private List<string> splitTracCodgsList = new List<string>();
        /// <summary>
        /// 字段说明:拆零追溯码string集合,方便数据操作 非数据库字段  
        /// </summary>
        public List<string> SplitTracCodgsList
        {
            get
            {
                return this.splitTracCodgsList;
            }
            set
            {
                this.splitTracCodgsList = value;
            }
        }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortIndex { get; set; }

        ///// <summary>
        ///// (发药时)包装单位采集的数量 退费使用
        ///// </summary>
        //public decimal PactOriginalDispensedQty { get; set; }

        ///// <summary>
        ///// (发药时)拆零原本采集数量 退费使用
        ///// </summary>
        //public decimal SplitOriginalDispensedQty { get; set; }

        #endregion

    }
}
