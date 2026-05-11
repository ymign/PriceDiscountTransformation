using System;
//using Neusoft.NFC;
using Neusoft.HISFC;
using Neusoft.FrameWork.Models;
namespace Neusoft.HISFC.Models.Order
{
	/// <summary>
	/// Neusoft.HISFC.Models.Order.Order<br></br>
	/// [功能描述: 医嘱资料实体]<br></br>
	/// [创 建 者: 李云凡]<br></br>
	/// [创建时间: 2006-09-10]<br></br>
	/// <修改记录
	///		修改人=''
	///		修改时间='yyyy-mm-dd'
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary>
    [Serializable]
    public class Order : Neusoft.FrameWork.Models.NeuObject,
        Neusoft.HISFC.Models.Base.IDept,
        Neusoft.HISFC.Models.Base.IBaby, Neusoft.HISFC.Models.Base.ISort
    {

        /// <summary>
        /// 医嘱资料实体
        /// ID 医嘱流水号
        /// </summary>
        public Order()
        {

        }

        #region 变量

        #region 私有

        /// <summary>
        /// 患者基本信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo patient;

        /// <summary>
        /// 0 不需皮试；1 免试；2 需皮试；3 皮试阳性[+]；4 皮试阴性[-]
        /// </summary>
        private EnumHypoTest hypoTest;

        /// <summary>
        /// 院内注射次数
        /// </summary>
        private int injectCount;

        /// <summary>
        /// 开立医生
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject doctor;

        /// <summary>
        /// 开立科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject doctorDept;

        /// <summary>
        /// 审核/执行护士
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject nurse;

        /// <summary>
        /// 录入人
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper;


        /// <summary>
        /// 停止者
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment dcOper;


        /// <summary>
        /// 执行者护士
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment execOper;


        /// <summary>
        /// 停止原因
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject dcReason;

        /// <summary>
        /// 合理用药审核结果
        /// </summary>
        private string passCheckResults;

        /// <summary>
        /// 医嘱类型
        /// </summary>
        private string orderType;
        /// <summary>
        /// 医嘱加密信息
        /// </summary>
        public string orderEncrypt;


        /// <summary>
        /// 合理用药审核结果
        /// </summary>
        public string PassCheckResults
        {
            get
            {
                if (passCheckResults == null)
                {
                    passCheckResults = string.Empty;
                }
                return passCheckResults;
            }
            set
            {
                passCheckResults = value;
            }
        }

        /// <summary>
        /// 药品项目/非药品项目
        /// </summary>
        private Neusoft.HISFC.Models.Base.Item item;


        /// <summary>
        /// 医嘱开立时间
        /// </summary>
        private DateTime dtMOTime;

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime beginTime;


        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// 本次分解时间
        /// </summary>
        private DateTime curMOTime;

        /// <summary>
        /// 下次分解时间
        /// </summary>
        private DateTime nextMOTime;

        /// <summary>
        /// 审核时间
        /// </summary>
        private DateTime confirmTime;

        /// <summary>
        /// 状态（0、保存 1、审核 2 执行 3 作废）
        /// </summary>
        private int status;

        /// <summary>
        /// 组合信息
        /// </summary>
        private Combo combo =null;// new Combo();

        /// <summary>
        /// 用法
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject usage;//new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 每次剂量
        /// </summary>
        private decimal doseOnce;

        /// <summary>
        /// 每次剂量单位
        /// </summary>
        private string doseUnit;

        /// <summary>
        /// 频次
        /// </summary>
        private Frequency frequency =null;// new Frequency();


        /// <summary>
        /// 草药付数
        /// </summary>
        private decimal herbalQty;

        /// <summary>
        /// 总量单位
        /// </summary>
        private string unit;

        /// <summary>
        /// 使用数量
        /// </summary>
        private int usetimes;

        /// <summary>
        /// 执行状态
        /// </summary>
        private int execStatus;

        /// <summary>
        /// 执行情况
        /// </summary>
        private string execResult = string.Empty;

        /// <summary>
        /// 临时医嘱执行时间使用
        /// 备注1
        /// </summary>
        [Obsolete("处理接瓶用到，作废了", true)]
        private string mark1;

        /// <summary>
        /// 备注2
        /// </summary>
        [Obsolete("用来处理签名的，作废了", true)]
        private string mark2;

        private string mark;

        private string mark0;

        private string mark7;

        private string mark8;

        private string mark9;


        /// <summary>
        /// 住院存储 重整医嘱  原医嘱流水号
        /// </summary>
        private string reTidyInfo;

        /// <summary>
        /// 检查部位记录
        /// </summary>
        private string checkPartRecord;

        /// <summary>
        /// 批注信息 如：胰岛素用法 平片位置 药品服用注意事项
        /// </summary>
        private string note;

        /// <summary>
        /// 处方号
        /// </summary>
        private string recipeNO;

        /// <summary>
        /// 处方流水序号
        /// </summary>
        private int sequenceNO;

        /// <summary>
        /// 送检样本
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject sample;

        /// <summary>
        /// 复合项目父项信息 ID 复合项目大项编码 NAME 复合项目大项名称
        /// </summary>
        private Neusoft.HISFC.Models.Base.Item package;

        /// <summary>
        /// 开单医生
        /// </summary>
        private NeuObject reciptDoctor;

        /// <summary>
        /// <br>加急</br>
        /// </summary>
        private bool isEmergency;

        /// <summary>
        /// 是否附材
        /// </summary>
        private bool isSubtbl;

        /// <summary>
        /// 是否包含附材
        /// </summary>
        private bool isHaveSubtbl;

        /// <summary>
        /// 是否扣药房库存
        /// </summary>
        private bool isStock;

        /// <summary>
        /// 是否是医保患者同意用药
        /// </summary>
        private bool isPermission;

        /// <summary>
        /// 特殊频次，执行剂量
        /// 执行时间放在频次执行时间里面进行处理
        /// </summary>
        private string execDose = "";

        /// <summary>
        /// 配液信息
        /// </summary>
        private Compound compound;

        //{E1902932-1839-4a92-8A6A-E42F448FA27F}
        /// <summary>
        /// 申请单号
        /// </summary>
        private string applyNo;

        /// <summary>
        /// 组号 {5BC77A1A-C3BB-4117-8791-4D4E664DC63E} 增加组号 houwb
        /// </summary>
        private int subCombNO;

        /// <summary>
        /// 最小单位标记（1 总量单位为最小单位，0 总量单位为剂量单位）
        /// </summary>
        private string minunitFlag;

        /// <summary>
        /// 最小单位标记（1 总量单位为最小单位，0 总量单位为剂量单位）
        /// </summary>
        public string MinunitFlag
        {
            get
            {
                if (minunitFlag == null)
                {
                    minunitFlag = string.Empty;
                }
                return minunitFlag;
            }
            set
            {
                minunitFlag = value;
            }
        }

         /// <summary>
        /// 界面显示的每次量：存储开立显示的数量 如1/3
        /// </summary>
        private string doseOnceDisplay;

        /// <summary>
        /// 界面显示的每次量单位：开立显示的单位 如片
        /// </summary>
        private string doseUnitDisplay;

        /// <summary>
        /// 首日量
        /// </summary>
        private string firstUseNum;

        #endregion

        #region 保护变量
        /// <summary>
        /// 开立科室
        /// </summary>
        protected Neusoft.FrameWork.Models.NeuObject reciptDept;

        /// <summary>
        /// 执行科室
        /// </summary>
        protected Neusoft.FrameWork.Models.NeuObject ExecDept;

        /// <summary>
        /// 药房科室
        /// </summary>
        protected Neusoft.FrameWork.Models.NeuObject DrugDept;

        /// <summary>
        /// 医嘱排列序号（医生可以通过拖拽设置医嘱所在位置）
        /// </summary>
        protected int sortid;

        /// <summary>
        /// 整理医嘱标志
        /// </summary>
        public bool Reorder;

        /// <summary>
        /// 是否婴儿
        /// </summary>
        protected bool bIsBaby;

        /// <summary>
        /// 婴儿序号信息
        /// </summary>
        protected string strBabyNo;

        /// <summary>
        /// 页号
        /// </summary>
        public int PageNo = -1;

        /// <summary>
        /// 行号
        /// </summary>
        public int RowNo = -1;

        /// <summary>
        /// 是否日间手术术前检查项目
        /// </summary>
        private bool isDayOperation;

      
        #endregion

        #endregion

        #region 属性
        /// <summary>
        /// 特殊频次，执行剂量
        /// 执行时间放在频次执行时间里面进行处理
        /// </summary>
        public string ExecDose
        {
            get
            {
                if (execDose == null)
                {
                    execDose = string.Empty;
                }
                return execDose;
            }
            set
            {
                execDose = value;
            }
        }

        /// <summary>
        /// 患者基本信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo Patient
        {
            get
            {
                if (patient == null)
                {
                    patient = new Neusoft.HISFC.Models.RADT.PatientInfo();
                }
                return this.patient;
            }
            set
            {
                this.patient = value;
            }
        }

        /// <summary>
        /// 0 不需皮试；1 免试；2 需皮试；3 皮试阳性[+]；4 皮试阴性[-]
        /// </summary>
        public EnumHypoTest HypoTest
        {
            get
            {
                if (hypoTest == null)
                {
                    hypoTest = EnumHypoTest.FreeHypoTest;
                }
                return this.hypoTest;
            }
            set
            {
                this.hypoTest = value;
            }
        }

        /// <summary>
        /// 院内注射次数
        /// </summary>
        public int InjectCount
        {
            get
            {
                return injectCount;
            }
            set
            {
                injectCount = value;
            }
        }

        /// <summary>
        /// 开立医生
        /// </summary>
        [Obsolete("作废，用ReciptDoctor", true)]
        public Neusoft.FrameWork.Models.NeuObject Doctor
        {
            get
            {
                if (doctor == null)
                {
                    doctor = new NeuObject();
                }
                return this.doctor;
            }
            set
            {
                this.doctor = value;
            }
        }

        /// <summary>
        /// 审核/执行护士
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Nurse
        {
            get
            {
                if (nurse == null)
                {
                    nurse = new NeuObject();
                }
                return this.nurse;
            }
            set
            {
                this.nurse = value;
            }
        }

        /// <summary>
        /// 录入人
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                if (oper == null)
                {
                    oper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }
        /// <summary>
        /// 停止者
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment DCOper
        {
            get
            {
                if (dcOper == null)
                {
                    dcOper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }
                return this.dcOper;
            }
            set
            {
                this.dcOper = value;
            }
        }

        /// <summary>
        /// 执行者
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment ExecOper
        {
            get
            {
                if (execOper == null)
                {
                    execOper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }
                return this.execOper;
            }
            set
            {
                this.execOper = value;
            }
        }


        /// <summary>
        /// 停止原因
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject DcReason
        {
            get
            {
                if (dcReason == null)
                {
                    dcReason = new NeuObject();
                }
                return this.dcReason;
            }
            set
            {

                this.dcReason = value;
            }
        }
        
        /// <summary>
        /// 当前开立所属的组套ID
        /// </summary>
        public string Groupid { get; set; }

        #region 时间
        /// <summary>
        /// 药品项目/非药品项目
        /// </summary>
        public Neusoft.HISFC.Models.Base.Item Item
        {
            get
            {
                if (item == null)
                {
                    item = new Neusoft.HISFC.Models.Base.Item();
                }

                return this.item;
            }
            set
            {
                this.item = value;
            }
        }
        /// <summary>
        /// 医嘱开立时间
        /// </summary>
        public DateTime MOTime
        {
            get
            {
                return this.dtMOTime;
            }
            set
            {
                this.dtMOTime = value;
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return this.beginTime;
            }
            set
            {
                this.beginTime = value;
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
            set
            {
                this.endTime = value;
            }
        }

        /// <summary>
        /// 本次分解时间
        /// </summary>
        public DateTime CurMOTime
        {
            get
            {
                return this.curMOTime;
            }
            set
            {
                this.curMOTime = value;
            }
        }

        /// <summary>
        /// 下次分解时间
        /// </summary>
        public DateTime NextMOTime
        {
            get
            {
                return this.nextMOTime;
            }
            set
            {
                this.nextMOTime = value;
            }
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime ConfirmTime
        {
            get
            {
                return this.confirmTime;
            }
            set
            {
                this.confirmTime = value;
            }
        }

        #endregion

        /// <summary>
        /// 状态（0、保存 1、审核 2 执行 3 作废）
        /// </summary>
        public int Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }

        /// <summary>
        /// 组合信息
        /// </summary>
        public Combo Combo
        {
            get
            {
                if (combo == null)
                {
                    combo = new Combo();
                }
                return this.combo;
            }
            set
            {
                this.combo = value;
            }
        }

        /// <summary>
        /// 组号
        /// </summary>
        public int SubCombNO
        {
            get
            {
                return subCombNO;
            }
            set
            {
                subCombNO = value;
            }
        }

        /// <summary>
        /// 用法
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Usage
        {
            get
            {
                if (usage == null)
                {
                    usage = new NeuObject();
                }
                return this.usage;
            }
            set
            {
                this.usage = value;
            }
        }

        /// <summary>
        /// 每次剂量
        /// </summary>
        public decimal DoseOnce
        {
            get
            {
                return this.doseOnce;
            }
            set
            {
                this.doseOnce = value;
            }
        }

        /// <summary>
        /// 每次剂量单位
        /// </summary>
        public string DoseUnit
        {
            get
            {
                if (doseUnit == null)
                {
                    doseUnit = string.Empty;
                }
                return this.doseUnit;
            }
            set
            {
                this.doseUnit = value;
            }
        }

        /// <summary>
        /// 频次
        /// </summary>
        public Frequency Frequency
        {
            get
            {
                if (frequency == null)
                {
                    frequency = new Frequency();
                }
                return this.frequency;
            }
            set
            {
                this.frequency = value;
            }
        }

        /// <summary>
        /// 总量
        /// 收费时候是草药的 ==数量*付数
        /// </summary>
        public decimal Qty
        {
            get
            {
                return this.Item.Qty;
            }
            set
            {
                this.Item.Qty = value;
            }
        }

        /// <summary>
        /// 草药付数
        /// </summary>
        public decimal HerbalQty
        {
            get
            {
                return this.herbalQty;
            }
            set
            {
                this.herbalQty = value;
            }
        }

        /// <summary>
        /// 总量单位
        /// </summary>
        public string Unit
        {
            get
            {
                if (unit == null)
                {
                    unit = string.Empty;
                }
                return this.unit;
            }
            set
            {
                this.unit = value;
            }
        }

        /// <summary>
        /// 使用数量
        /// </summary>
        public int Usetimes
        {
            get
            {
                return this.usetimes;
            }
            set
            {
                this.usetimes = value;
            }
        }

        /// <summary>
        /// 执行状态
        /// </summary>
        public int ExecStatus
        {
            get
            {
                return this.execStatus;
            }
            set
            {
                this.execStatus = value;
            }
        }

        /// <summary>
        /// 临时医嘱执行时间使用：用来处理接瓶数了  现在无用了
        /// </summary>
        [Obsolete("处理接瓶用到，作废了", false)]
        public string ExtendFlag1
        {
            get
            {
                if (mark1 == null)
                {
                    mark1 = string.Empty;
                }
                return this.mark1;
            }
            set
            {
                this.mark1 = value;
            }
        }

        /// <summary>
        /// 备注2
        /// </summary>
        [Obsolete("用来处理签名的，作废了", false)]
        public string ExtendFlag2
        {
            get
            {
                if (mark2 == null)
                {
                    mark2 = string.Empty;
                }
                return this.mark2;
            }
            set
            {
                this.mark2 = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string ExtendFlag
        {
            get
            {
                if (mark == null)
                {
                    mark = string.Empty;
                }
                return this.mark;
            }
            set
            {
                this.mark = value;
            }
        }

        /// <summary>
        /// 备注0
        /// </summary>
        public string ExtendFlag0
        {
            get
            {
                if (mark0 == null)
                {
                    mark0 = string.Empty;
                }
                return this.mark0;
            }
            set
            {
                this.mark0 = value;
            }
        }

        /// <summary>
        /// 备注7（手术）
        /// </summary>
        public string ExtendFlag7
        {
            get
            {
                if (mark7 == null)
                {
                    mark7 = string.Empty;
                }
                return this.mark7;
            }
            set
            {
                this.mark7 = value;
            }
        }

        /// <summary>
        /// 备注8（切口）
        /// </summary>
        public string ExtendFlag8
        {
            get
            {
                if (mark8 == null)
                {
                    mark8 = string.Empty;
                }
                return this.mark8;
            }
            set
            {
                this.mark8 = value;
            }
        }

        /// <summary>
        /// 备注9（抗生素特注）
        /// </summary>
        public string ExtendFlag9
        {
            get
            {
                if (mark9 == null)
                {
                    mark9 = string.Empty;
                }
                return this.mark9;
            }
            set
            {
                this.mark9 = value;
            }
        }

        /// <summary>
        /// 住院存储 重整医嘱  原医嘱流水号
        /// </summary>
        [Obsolete("换做ReTidyInfo", true)]
        public string ExtendFlag3
        {
            get
            {
                if (reTidyInfo == null)
                {
                    reTidyInfo = string.Empty;
                }
                return this.reTidyInfo;
            }
            set
            {
                this.reTidyInfo = value;
            }
        }

        /// <summary>
        /// 住院存储 重整医嘱  原医嘱流水号
        /// </summary>
        public string ReTidyInfo
        {
            get
            {
                if (reTidyInfo == null)
                {
                    reTidyInfo = string.Empty;
                }
                return this.reTidyInfo;
            }
            set
            {
                this.reTidyInfo = value;
            }
        }

        /// <summary>
        /// 检查部位记录
        /// </summary>
        public string CheckPartRecord
        {
            get
            {
                if (checkPartRecord == null)
                {
                    checkPartRecord = string.Empty;
                }
                return this.checkPartRecord;
            }
            set
            {
                this.checkPartRecord = value;
            }
        }

        /// <summary>
        /// 批注信息 如：胰岛素用法 平片位置 药品服用注意事项
        /// </summary>
        public string Note
        {
            get
            {
                if (note == null)
                {
                    note = string.Empty;
                }
                return this.note;
            }
            set
            {
                this.note = value;
            }
        }

        /// <summary>
        /// 处方号
        /// </summary>
        public string ReciptNO
        {
            get
            {
                if (recipeNO == null)
                {
                    recipeNO = string.Empty;
                }
                return this.recipeNO;
            }
            set
            {
                this.recipeNO = value;
            }
        }

        /// <summary>
        /// 处方流水序号
        /// </summary>
        public int SequenceNO
        {
            get
            {
                return this.sequenceNO;
            }
            set
            {
                this.sequenceNO = value;
            }
        }

        /// <summary>
        /// 送检样本
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Sample
        {
            get
            {
                if (sample == null)
                {
                    sample = new NeuObject();
                }
                return this.sample;
            }
            set
            {
                this.sample = value;
            }
        }

        /// <summary>
        /// 复合项目父项信息 ID 复合项目大项编码 NAME 复合项目大项名称
        /// </summary>
        public Neusoft.HISFC.Models.Base.Item Package
        {
            get
            {
                if (package == null)
                {
                    package = new Neusoft.HISFC.Models.Base.Item();
                }
                return this.package;
            }
            set
            {
                this.package = value;
            }
        }

        /// <summary>
        /// 开单医生
        /// </summary>
        public NeuObject ReciptDoctor
        {
            get
            {
                if (reciptDoctor == null)
                {
                    reciptDoctor = new NeuObject();
                }
                return this.reciptDoctor;
            }
            set
            {
                this.reciptDoctor = value;
            }
        }

        #region 标志

        /// <summary>
        /// 是否为GCP医嘱 0否1是
        /// </summary>
        public bool IsGCPOrder { get; set; }

        /// <summary>
        /// <br>加急</br>
        /// </summary>
        public bool IsEmergency
        {
            get
            {
                return this.isEmergency;
            }
            set
            {
                this.isEmergency = value;
            }
        }
        /// <summary>
        /// 是否附材
        /// </summary>
        public bool IsSubtbl
        {
            get
            {
                return this.isSubtbl;
            }
            set
            {
                this.isSubtbl = value;
            }
        }
        /// <summary>
        /// 是否包含附材
        /// </summary>
        public bool IsHaveSubtbl
        {
            get
            {
                return this.isHaveSubtbl;
            }
            set
            {
                this.isHaveSubtbl = value;
            }
        }

        /// <summary>
        /// 是否扣药房库存
        /// </summary>
        public bool IsStock
        {
            get
            {
                return this.isStock;
            }
            set
            {
                this.isStock = value;
            }
        }

        /// <summary>
        /// 是否是医保患者同意用药
        /// </summary>
        public bool IsPermission
        {
            get
            {
                return this.isPermission;
            }
            set
            {
                this.isPermission = value;
            }
        }

        #endregion

        /// <summary>
        /// 是否配液
        /// </summary>
        public Compound Compound
        {
            get
            {
                if (compound == null)
                {
                    compound = new Compound();
                }

                return this.compound;
            }
            set
            {
                this.compound = value;
            }
        }


        /// <summary>
        /// 申请单号
        /// </summary>
        public string ApplyNo
        {
            get
            {
                if (applyNo == null)
                {
                    applyNo = string.Empty;
                }
                return applyNo;
            }
            set
            {
                applyNo = value;
            }
        }

        /// <summary>
        /// 界面显示的每次量：存储开立显示的数量 如1/3
        /// </summary>
        [Obsolete("换做DoseOnceDisplay", true)]
        public string ExtendFlag4
        {
            get
            {
                if (doseOnceDisplay == null)
                {
                    doseOnceDisplay = string.Empty;
                }
                return this.doseOnceDisplay;
            }
            set
            {
                this.doseOnceDisplay = value;
            }
        }

        /// <summary>
        /// 界面显示的每次量：存储开立显示的数量 如1/3
        /// </summary>
        public string DoseOnceDisplay
        {
            get
            {
                if (doseOnceDisplay == null)
                {
                    doseOnceDisplay = string.Empty;
                }
                return this.doseOnceDisplay;
            }
            set
            {
                this.doseOnceDisplay = value;
            }
        }

        /// <summary>
        /// 界面显示的每次量单位：开立显示的单位 如片
        /// </summary>
        [Obsolete("换做DoseUnitDisplay", true)]
        public string ExtendFlag5
        {
            get
            {
                if (doseUnitDisplay == null)
                {
                    doseUnitDisplay = string.Empty;
                }
                return this.doseUnitDisplay;
            }
            set
            {
                this.doseUnitDisplay = value;
            }
        }

        /// <summary>
        /// 界面显示的每次量单位：开立显示的单位 如片
        /// </summary>
        public string DoseUnitDisplay
        {
            get
            {
                if (doseUnitDisplay == null)
                {
                    doseUnitDisplay = string.Empty;
                }
                return this.doseUnitDisplay;
            }
            set
            {
                this.doseUnitDisplay = value;
            }
        }

        /// <summary>
        /// 首日量
        /// </summary>
        [Obsolete("换做FirstUseNum", true)]
        public string ExtendFlag6
        {
            get
            {
                if (firstUseNum == null)
                {
                    firstUseNum = string.Empty;
                }
                return this.firstUseNum;
            }
            set
            {
                this.firstUseNum = value;
            }
        }

        /// <summary>
        /// 首日量
        /// </summary>
        public string FirstUseNum
        {
            get
            {
                if (firstUseNum == null)
                {
                    firstUseNum = string.Empty;
                }
                return this.firstUseNum;
            }
            set
            {
                this.firstUseNum = value;
            }
        }

        /// <summary>
        /// 医嘱类型
        /// </summary>
        public string OrderType
        {
            get
            {
                if (orderType == null)
                {
                    orderType = string.Empty;
                }
                return orderType;
            }
            set
            {
                orderType = value;
            }
        }

        /// <summary>
        /// 是否日间手术术前检查项目
        /// </summary>
        public bool IsDayOperation
        {
            get { return isDayOperation; }
            set { isDayOperation = value; }
        }
        #endregion

        #region 接口实现

        #region IDept 成员
        /// <summary>
        /// 患者在院科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject InDept
        {
            get
            {
                // TODO:  添加 Order.InDept getter 实现
                return this.Patient.PVisit.PatientLocation.Dept;
            }
            set
            {
                // TODO:  添加 Order.InDept setter 实现
                this.Patient.PVisit.PatientLocation.Dept = value;
            }
        }

        /// <summary>
        /// 执行科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject ExeDept
        {
            get
            {
                if (ExecDept == null)
                {
                    ExecDept = new NeuObject();
                }

                // TODO:  添加 Order.ExeDept getter 实现
                return this.ExecDept;
            }
            set
            {
                // TODO:  添加 Order.ExeDept setter 实现
                this.ExecDept = value;
            }
        }

        /// <summary>
        /// 开立科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject ReciptDept
        {
            get
            {
                if (reciptDept == null)
                {
                    reciptDept = new NeuObject();
                }
                // TODO:  添加 Order.ReciptDept getter 实现
                return this.reciptDept;
            }
            set
            {
                // TODO:  添加 Order.ReciptDept setter 实现
                this.reciptDept = value;
            }
        }

        /// <summary>
        /// 开立执行护士站
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject NurseStation
        {
            get
            {
                // TODO:  添加 Order.NurseStation getter 实现
                return this.Patient.PVisit.PatientLocation.NurseCell;
            }
            set
            {
                // TODO:  添加 Order.NurseStation setter 实现
                this.Patient.PVisit.PatientLocation.NurseCell = value;
            }
        }

        /// <summary>
        /// 扣库科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject StockDept
        {
            get
            {
                if (DrugDept == null)
                {
                    DrugDept = new NeuObject();
                }
                // TODO:  添加 Order.StockDept getter 实现
                return this.DrugDept;
            }
            set
            {
                // TODO:  添加 Order.StockDept setter 实现
                this.DrugDept = value;
            }
        }

        /// <summary>
        /// 开立医生所在科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject DoctorDept
        {
            get
            {
                if (doctorDept == null)
                {
                    doctorDept = new NeuObject();
                }

                // TODO:  添加 Order.ReciptDoct getter 实现
                return this.doctorDept;
            }
            set
            {
                // TODO:  添加 Order.ReciptDoct setter 实现
                this.doctorDept = value;
            }
        }

        #endregion

        #region IBaby 成员
        /// <summary>
        /// 婴儿序号
        /// </summary>
        public string BabyNO
        {
            get
            {
                // TODO:  添加 Order.Neusoft.HISFC.Models.Base.IBaby.BabyNo getter 实现
                if (strBabyNo == null)
                {
                    this.strBabyNo = "0";
                }
                return this.strBabyNo;
            }
            set
            {
                // TODO:  添加 Order.Neusoft.HISFC.Models.Base.IBaby.BabyNo setter 实现
                strBabyNo = value;
            }
        }

        /// <summary>
        /// 是否婴儿
        /// </summary>
        public bool IsBaby
        {
            get
            {
                // TODO:  添加 Order.Neusoft.HISFC.Models.Base.IBaby.IsBaby getter 实现
                return this.bIsBaby;
            }
            set
            {
                // TODO:  添加 Order.Neusoft.HISFC.Models.Base.IBaby.IsBaby setter 实现
                this.bIsBaby = value;
            }
        }

        /// <summary>
        /// 执行情况
        /// </summary>
        public string ExecResult
        {
            get { return execResult; }
            set { execResult = value; }
        }

        #endregion

        #region ISort 成员

        public int SortID
        {
            get
            {
                // TODO:  添加 Order.SortID getter 实现
                return this.sortid;
            }
            set
            {
                // TODO:  添加 Order.SortID setter 实现
                this.sortid = value;
            }
        }

        #endregion

        #endregion

        #region 方法

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Order Clone()
        {
            // TODO:  添加 Order.Clone 实现
            Order obj = base.Clone() as Order;
            obj.Combo = this.Combo.Clone();
            obj.DcReason = this.DcReason.Clone();

            obj.Frequency = (Frequency)this.Frequency.Clone();

            try { obj.ExeDept = this.ExeDept.Clone(); }
            catch { };
            try { obj.InDept = this.InDept.Clone(); }
            catch { };
            try { obj.NurseStation = this.NurseStation.Clone(); }
            catch { };
            try { obj.ReciptDept = this.ReciptDept.Clone(); }
            catch { };
            try { obj.DoctorDept = this.DoctorDept.Clone(); }
            catch { };
            try { obj.StockDept = this.StockDept.Clone(); }
            catch { };

            obj.Item = this.Item.Clone();
            obj.Nurse = this.Nurse.Clone();

            try { obj.Patient = this.Patient.Clone(); }
            catch { };

            obj.Usage = this.Usage.Clone();
            obj.oper = this.Oper.Clone();
            obj.execOper = this.ExecOper.Clone();
            obj.dcOper = this.DCOper.Clone();

            obj.compound = this.Compound.Clone();
            return obj;
        }


        #endregion

        #endregion
    }

    /// <summary>
    /// 皮试
    /// </summary>
    public enum EnumHypoTest
    {
        /// <summary>
        /// 0 不需要皮试
        /// </summary>
        [Neusoft.FrameWork.Public.Description("")]
        FreeHypoTest = 0,

        /// <summary>
        /// 1 免试
        /// </summary>
        [Neusoft.FrameWork.Public.Description("[免试]")]
        NoHypoTest,

        /// <summary>
        /// 2 需要皮试
        /// </summary>
        [Neusoft.FrameWork.Public.Description("[需皮试]")]
        NeedHypoTest,

        /// <summary>
        /// 3 阳性
        /// </summary>
        [Neusoft.FrameWork.Public.Description("[+]")]
        Negative,

        /// <summary>
        /// 4 阴性
        /// </summary>
        [Neusoft.FrameWork.Public.Description("[-]")]
        Positive
    }
}
