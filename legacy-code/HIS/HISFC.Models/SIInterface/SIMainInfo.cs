using System;
using Neusoft.FrameWork.Models;


namespace Neusoft.HISFC.Models.SIInterface
{


    /// <summary>
    /// SIMainInfo 的摘要说明。
    /// Id inpatientNo, name 患者姓名
    /// </summary>
    [Serializable]
    public class SIMainInfo : Neusoft.FrameWork.Models.NeuObject
    {
        public SIMainInfo()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        #region 扩展属性
        private System.Collections.Generic.Dictionary<string, NeuObject> extendProperty = new System.Collections.Generic.Dictionary<string, NeuObject>();
        /// <summary>
        /// 扩展属性
        /// </summary>
        public System.Collections.Generic.Dictionary<string, NeuObject> ExtendProperty
        {
            get { return extendProperty; }
            set { extendProperty = value; }
        }
        #endregion

        private int feeTimes;
        /// <summary>
        /// 费用批次
        /// </summary>
        public int FeeTimes
        {
            set
            {
                feeTimes = value;
            }
            get
            {
                return feeTimes;
            }
        }
        private int readFlag;
        /// <summary>
        /// 读入标志
        /// </summary>
        public int ReadFlag
        {
            get
            {
                return readFlag;
            }
            set
            {
                readFlag = value;
            }
        }

        private string regNo;
        /// <summary>
        /// 就诊登记号、铁路医保个人编号
        /// </summary>
        public string RegNo
        {
            set
            {
                regNo = value;
            }
            get
            {
                return regNo;
            }
        }

        private string hosNo;
        /// <summary>
        /// 医院编号
        /// </summary>
        public string HosNo
        {
            set { hosNo = value; }
            get { return hosNo; }
        }

        private string balNo;
        /// <summary>
        ///  结算序号
        /// </summary>
        public string BalNo
        {
            get
            {
                if (balNo == null || balNo == "")
                {
                    balNo = "0";
                }
                return balNo;
            }
            set { balNo = value; }
        }
        private string invoiceNo;
        /// <summary>
        /// 主发票号
        /// </summary>
        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { invoiceNo = value; }
        }
        private Neusoft.FrameWork.Models.NeuObject medicalType = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 医疗类别 1-住院 2 -门诊特定项目
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject MedicalType
        {
            get { return medicalType; }
            set { medicalType = value; }
        }
        //		private string patientNo;
        //		/// <summary>
        //		/// 住院号
        //		/// </summary>
        //		public string PatientNo
        //		{
        //			get{return patientNo;}
        //			set{patientNo = value;}
        //		}
        //		private string cardNo;
        //		/// <summary>
        //		/// 就诊卡号
        //		/// </summary>
        //		public string CardNo
        //		{
        //			get{return cardNo;}
        //			set{cardNo = value;}
        //		}
        //		private string mCardNo;
        //		/// <summary>
        //		/// 医疗证号
        //		/// </summary>
        //		public string MCardNo
        //		{
        //			get{return mCardNo;}
        //			set{mCardNo = value;}
        //		}
        private string proceatePcNo;
        /// <summary>
        /// 生育保险患者电脑号
        /// </summary>
        public string ProceatePcNo
        {
            get { return proceatePcNo; }
            set { proceatePcNo = value; }
        }
        private DateTime siBeginDate;
        /// <summary>
        /// 参保日期
        /// </summary>
        public DateTime SiBegionDate
        {
            get { return siBeginDate; }
            set { siBeginDate = value; }
        }
        private string siState;
        /// <summary>
        /// 参保状态 3-参保缴费、4-暂停缴费、7-终止参保
        /// </summary>
        public string SiState
        {
            get { return siState; }
            set { siState = value; }
        }
        private string emplType;
        /// <summary>
        /// 人员类别 1-在职、2-退休
        /// </summary>
        public string EmplType
        {
            get { return emplType; }
            set { emplType = value; }
        }
        private string clinicDiagNose;
        /// <summary>
        /// 门诊诊断
        /// </summary>
        public string ClinicDiagNose
        {
            get { return clinicDiagNose; }
            set { clinicDiagNose = value; }
        }
        private DateTime inDiagnoseDate;
        /// <summary>
        /// 入院诊断日期
        /// </summary>
        public DateTime InDiagnoseDate
        {
            get { return inDiagnoseDate; }
            set { inDiagnoseDate = value; }
        }

        private Neusoft.FrameWork.Models.NeuObject inDiagnose = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 入院诊断信息
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject InDiagnose
        {
            get { return inDiagnose; }
            set { inDiagnose = value; }
        }

        private decimal totCost;
        /// <summary>
        /// 住院总金额
        /// </summary>
        public decimal TotCost
        {
            get { return totCost; }
            set { totCost = value; }
        }
        private decimal addTotCost = 0;
        /// <summary>
        /// 费用累计
        /// </summary>
        public decimal AddTotCost
        {
            get { return addTotCost; }
            set { addTotCost = value; }
        }
        private decimal payCost;
        /// <summary>
        /// 帐户支付金额
        /// </summary>
        public decimal PayCost
        {
            get { return payCost; }
            set { payCost = value; }
        }

        /// <summary>
        /// 社保支付金额(除自费和账户支付的所有金额的合计)
        /// </summary>
        private decimal pubCost;
        /// <summary>
        /// 社保支付金额(除自费和账户支付的所有金额的合计)
        /// </summary>
        public decimal PubCost
        {
            get { return pubCost; }
            set { pubCost = value; }
        }
        //{06A3389F-B19E-4482-A55C-89269995B142}
        /// <summary>
        /// 医保返回的统筹金额
        /// </summary>
        private decimal siPubCost;

        /// <summary>
        /// 医保返回的统筹金额
        /// </summary>
        public decimal SiPubCost
        {
            get { return this.siPubCost; }
            set { this.siPubCost = value; }

        }

        private decimal itemPayCost;
        /// <summary>
        /// 部分项目自付金额 
        /// </summary>
        public decimal ItemPayCost
        {
            get { return itemPayCost; }
            set { itemPayCost = value; }
        }
        private decimal baseCost;
        /// <summary>
        /// 个人起付金额
        /// </summary>
        public decimal BaseCost
        {
            get { return baseCost; }
            set { baseCost = value; }
        }
        private decimal ownCost;
        /// <summary>
        /// 个人自费项目金额
        /// </summary>
        public decimal OwnCost
        {
            get { return ownCost; }
            set { ownCost = value; }
        }
        private decimal itemYLCost;
        /// <summary>
        /// 个人自付金额（乙类自付部分）
        /// </summary>
        public decimal ItemYLCost
        {
            get { return itemYLCost; }
            set { itemYLCost = value; }
        }

        private decimal pubOwnCost;
        /// <summary>
        /// 个人自负金额
        /// </summary>
        public decimal PubOwnCost
        {
            set { pubOwnCost = value; }
            get { return pubOwnCost; }
        }

        private decimal overTakeOwnCost;
        /// <summary>
        /// 超统筹支付限额个人自付金额
        /// </summary>
        public decimal OverTakeOwnCost
        {
            get { return overTakeOwnCost; }
            set { overTakeOwnCost = value; }
        }

        private decimal hosCost;
        /// <summary>
        /// 医药机构分担金额
        /// </summary>
        public decimal HosCost
        {
            set
            {
                hosCost = value;
            }
            get
            {
                return hosCost;
            }
        }

        private Neusoft.FrameWork.Models.NeuObject operInfo = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 操作员信息
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject OperInfo
        {
            get { return operInfo; }
            set { operInfo = value; }
        }
        private DateTime operDate;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate
        {
            get { return operDate; }
            set { operDate = value; }
        }
        private int appNo;
        /// <summary>
        /// 审批号
        /// </summary>
        public int AppNo
        {
            get { return appNo; }
            set { appNo = value; }
        }
        private DateTime balanceDate;
        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime BalanceDate
        {
            get { return balanceDate; }
            set { balanceDate = value; }
        }
        private decimal yearCost;
        /// <summary>
        /// 本年度可用定额
        /// </summary>
        public decimal YearCost
        {
            get
            {
                return yearCost;
            }
            set
            {
                yearCost = value;
            }
        }
        private Neusoft.FrameWork.Models.NeuObject outDiagnose = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 出院诊断
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject OutDiagnose
        {
            set { outDiagnose = value; }
            get { return outDiagnose; }
        }

        private bool isValid;
        /// <summary>
        /// 是否有效 True有效 False 无效
        /// </summary>
        public bool IsValid
        {
            set
            {
                isValid = value;
            }
            get
            {
                return isValid;
            }
        }

        private bool isBalanced;
        /// <summary>
        /// 是否结算 True 结算 False 未结算
        /// </summary>
        public bool IsBalanced
        {
            get
            {
                return isBalanced;
            }
            set
            {
                isBalanced = value;
            }
        }


        #region 铁路医保附加属性
        #region 变量
        string icCardCode = "";
        Neusoft.FrameWork.Models.NeuObject personType = new NeuObject();
        Neusoft.FrameWork.Models.NeuObject civilianGrade = new NeuObject();
        Neusoft.FrameWork.Models.NeuObject specialCare = new NeuObject();
        string duty = "";
        Neusoft.FrameWork.Models.NeuObject anotherCity = new NeuObject();
        Neusoft.FrameWork.Models.NeuObject corporation = new NeuObject();
        decimal individualBalance = 0;
        string freezeMessage = "";
        string applySequence = "";
        Neusoft.FrameWork.Models.NeuObject disease = new NeuObject();
        Neusoft.FrameWork.Models.NeuObject applyType = new NeuObject();
        Neusoft.FrameWork.Models.NeuObject fund = new NeuObject();
        string businessSequence = "";
        Neusoft.FrameWork.Models.NeuObject specialWorkKind = new NeuObject();
        string hospitalBusinessSequence = "";
        string opositeBusinessSequence = "";
        #endregion
        /// <summary>
        /// IC卡号码
        /// </summary>
        public string ICCardCode
        {
            get
            {
                return this.icCardCode;
            }
            set
            {
                this.icCardCode = value;
            }
        }

        /// <summary>
        /// 人员类别
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject PersonType
        {
            get
            {
                return this.personType;
            }
            set
            {
                this.personType = value;
            }
        }
        /// <summary>
        /// 公务员级别
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject CivilianGrade
        {
            get
            {
                return this.civilianGrade;
            }
            set
            {
                this.civilianGrade = value;
            }
        }
        /// <summary>
        /// 特殊照顾人群
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject SpecialCare
        {
            get
            {
                return this.specialCare;
            }
            set
            {
                this.specialCare = value;
            }
        }
        /// <summary>
        /// 职务
        /// </summary>
        public string Duty
        {
            get
            {
                return this.duty;
            }
            set
            {
                this.duty = value;
            }
        }
        /// <summary>
        /// 异地安置城市
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject AnotherCity
        {
            get
            {
                return this.anotherCity;
            }
            set
            {
                this.anotherCity = value;
            }
        }
        /// <summary>
        /// 参保人单位
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Corporation
        {
            get
            {
                return this.corporation;
            }
            set
            {
                this.corporation = value;
            }
        }
        /// <summary>
        /// 个人帐户余额
        /// </summary>
        public decimal IndividualBalance
        {
            get
            {
                return this.individualBalance;
            }
            set
            {
                this.individualBalance = value;
            }
        }
        /// <summary>
        /// 已冻结基金信息
        /// </summary>
        public string FreezeMessage
        {
            get
            {
                return this.freezeMessage;
            }
            set
            {
                this.freezeMessage = value;
            }
        }
        /// <summary>
        /// 申请序号
        /// </summary>
        public string ApplySequence
        {
            get
            {
                return this.applySequence;
            }
            set
            {
                this.applySequence = value;
            }
        }
        /// <summary>
        /// 疾病
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Disease
        {
            get
            {
                return this.disease;
            }
            set
            {
                this.disease = value;
            }
        }
        /// <summary>
        /// 申请类型
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject ApplyType
        {
            get
            {
                return this.applyType;
            }
            set
            {
                this.applyType = value;
            }
        }
        /// <summary>
        /// 基金
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Fund
        {
            get
            {
                return this.fund;
            }
            set
            {
                this.fund = value;
            }
        }
        /// <summary>
        /// 业务序号
        /// </summary>
        public string BusinessSequence
        {
            get
            {
                return this.businessSequence;
            }
            set
            {
                this.businessSequence = value;
            }
        }
        /// <summary>
        /// 特殊工种
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject SpecialWorkKind
        {
            get
            {
                return this.specialWorkKind;
            }
            set
            {
                this.specialWorkKind = value;
            }
        }
        /// <summary>
        /// 医院费用序列号
        /// </summary>
        public string HospitalBusinessSequence
        {
            get
            {
                return this.hospitalBusinessSequence;
            }
            set
            {
                this.hospitalBusinessSequence = value;
            }
        }
        /// <summary>
        /// 对应费用序列号
        /// </summary>
        public string OpositeBusinessSequence
        {
            get
            {
                return this.opositeBusinessSequence;
            }
            set
            {
                this.opositeBusinessSequence = value;
            }
        }

        public new SIMainInfo Clone()
        {
            SIMainInfo obj = base.Clone() as SIMainInfo;
            obj.medicalType = this.medicalType.Clone();
            obj.inDiagnose = this.inDiagnose.Clone();
            obj.operInfo = this.operInfo.Clone();
            obj.PersonType = this.PersonType.Clone();
            obj.CivilianGrade = this.CivilianGrade.Clone();
            obj.SpecialCare = this.SpecialCare.Clone();
            obj.AnotherCity = this.AnotherCity.Clone();
            obj.Corporation = this.Corporation.Clone();
            obj.Disease = this.Disease.Clone();
            obj.ApplyType = this.ApplyType.Clone();
            obj.Fund = this.Fund.Clone();
            obj.SpecialWorkKind = this.SpecialWorkKind.Clone();
            System.Collections.Generic.Dictionary<string, NeuObject> ep = new System.Collections.Generic.Dictionary<string, NeuObject>();
            foreach (string s in this.ExtendProperty.Keys)
            {
                ep.Add(s, this.ExtendProperty[s].Clone());
            }
            obj.ExtendProperty = ep;
            return obj;
        }
        #endregion

        #region 沈阳医保增加属性

        #region 变量

        /// <summary>
        /// 发卡机构编码
        /// </summary>
        private string cardOrgID = string.Empty;

        /// <summary>
        /// 卡有效期
        /// </summary>
        private DateTime cardValidTime = DateTime.MinValue;

        /// <summary>
        /// 变更日期
        /// </summary>
        private DateTime shiftTime = DateTime.MinValue;

        /// <summary>
        /// 是否卡已经锁定
        /// </summary>
        private bool isCardLocked = false;

        /// <summary>
        /// 本年统筹支出累计
        /// </summary>
        private decimal yearPubCost = 0;

        /// <summary>
        /// 本年救助金支出累计
        /// </summary>
        private decimal yearHelpCost = 0;

        /// <summary>
        /// 转出医院起伏标准
        /// </summary>
        private decimal turnOutHosStandardCost = 0;

        /// <summary>
        /// 转出医院起伏标准自付
        /// </summary>
        private decimal turnOutHosOnwCost = 0;

        /// <summary>
        /// 住院次数
        /// </summary>
        private int inHosTimes = 0;

        /// <summary>
        /// 账户支付累计
        /// </summary>
        private decimal payAddCost = 0;

        /// <summary>
        /// 账户支付年度
        /// </summary>
        private string payYear = string.Empty;

        /// <summary>
        /// 现金支付金额累计
        /// </summary>
        private decimal ownCashAddCost = 0;

        /// <summary>
        /// 个人自负(乙类项目)金额累计
        /// </summary>
        private decimal ownAddCost = 0;
        /// <summary>
        /// 年度个人自付累计
        /// </summary>
        private decimal yearOwnAddCost = 0;

        /// <summary>
        /// 公务员支付金额累计
        /// </summary>
        private decimal gwyPayAddCost = 0;

        /// <summary>
        /// 特殊门诊支付累计
        /// </summary>
        private decimal spOutpatientPayAddCost = 0;

        /// <summary>
        /// 门诊慢性病支付累计
        /// </summary>
        private decimal slowOutpatientPayAddCost = 0;
        /// <summary>
        /// 帐户注入累计
        /// </summary>
        private decimal yearAddPayCost = 0;
        /// <summary>
        ///  帐户注入刷新日期
        /// </summary>
        private DateTime freshAddPayDate = DateTime.MinValue;
        /// <summary>
        /// 结转帐户支出累计
        /// </summary>
        private decimal yearAddPayTurnCost = 0;

        #endregion

        #region 属性

        /// <summary>
        /// 发卡机构编码
        /// </summary>
        public string CardOrgID
        {
            get
            {
                return this.cardOrgID;
            }
            set
            {
                this.cardOrgID = value;
            }
        }

        /// <summary>
        /// 卡有效期
        /// </summary>
        public DateTime CardValidTime
        {
            get
            {
                return this.cardValidTime;
            }
            set
            {
                this.cardValidTime = value;
            }
        }

        /// <summary>
        /// 变更日期
        /// </summary>
        public DateTime ShiftTime
        {
            get
            {
                return this.shiftTime;
            }
            set
            {
                this.shiftTime = value;
            }
        }

        /// <summary>
        /// 是否卡已经锁定
        /// </summary>
        public bool IsCardLocked
        {
            get
            {
                return this.isCardLocked;
            }
            set
            {
                this.isCardLocked = value;
            }
        }

        /// <summary>
        /// 本年统筹支出累计
        /// </summary>
        public decimal YearPubCost
        {
            get
            {
                return this.yearPubCost;
            }
            set
            {
                this.yearPubCost = value;
            }
        }

        /// <summary>
        /// 本年救助金支出累计
        /// </summary>
        public decimal YearHelpCost
        {
            get
            {
                return this.yearHelpCost;
            }
            set
            {
                this.yearHelpCost = value;
            }
        }

        /// <summary>
        /// 转出医院起伏标准
        /// </summary>
        public decimal TurnOutHosStandardCost
        {
            get
            {
                return this.turnOutHosStandardCost;
            }
            set
            {
                this.turnOutHosStandardCost = value;
            }
        }

        /// <summary>
        /// 转出医院起伏标准自付
        /// </summary>
        public decimal TurnOutHosOnwCost
        {
            get
            {
                return this.turnOutHosOnwCost;
            }
            set
            {
                this.turnOutHosOnwCost = value;
            }
        }

        /// <summary>
        /// 住院次数
        /// </summary>
        public int InHosTimes
        {
            get
            {
                return this.inHosTimes;
            }
            set
            {
                this.inHosTimes = value;
            }
        }

        /// <summary>
        /// 账户支付累计
        /// </summary>
        public decimal PayAddCost
        {
            get
            {
                return this.payAddCost;
            }
            set
            {
                this.payAddCost = value;
            }
        }

        /// <summary>
        /// 账户支付年度
        /// </summary>
        public string PayYear
        {
            get
            {
                return this.payYear;
            }
            set
            {
                this.payYear = value;
            }
        }

        /// <summary>
        /// 现金支付金额累计
        /// </summary>
        public decimal OwnCashAddCost
        {
            get
            {
                return this.ownCashAddCost;
            }
            set
            {
                this.ownCashAddCost = value;
            }
        }

        /// <summary>
        /// 个人自负(乙类项目)金额累计
        /// </summary>
        public decimal OwnAddCost
        {
            get
            {
                return this.ownAddCost;
            }
            set
            {
                this.ownAddCost = value;
            }
        }
        /// <summary>
        /// 年度个人自付累计
        /// </summary>
        public decimal YearOwnAddCost
        {
            get
            {
                return this.yearAddPayCost;
            }
            set
            {
                this.yearAddPayCost = value;
            }
        }
        /// <summary>
        /// 公务员支付金额累计
        /// </summary>
        public decimal GwyPayAddCost
        {
            get
            {
                return this.gwyPayAddCost;
            }
            set
            {
                this.gwyPayAddCost = value;
            }
        }



        /// <summary>
        /// 特殊门诊支付累计
        /// </summary>
        public decimal SpOutpatientPayAddCost
        {
            get
            {
                return this.spOutpatientPayAddCost;
            }
            set
            {
                this.spOutpatientPayAddCost = value;
            }
        }

        /// <summary>
        /// 门诊慢性病支付累计
        /// </summary>
        public decimal SlowOutpatientPayAddCost
        {
            get
            {
                return this.slowOutpatientPayAddCost;
            }
            set
            {
                this.slowOutpatientPayAddCost = value;
            }
        }
        /// <summary>
        /// 帐户注入累计
        /// </summary>
        public decimal YearAddPayCost
        {
            set
            {
                this.yearAddPayCost = value;
            }
            get
            {
                return this.yearAddPayCost;
            }
        }
        /// <summary>
        /// 帐户注入刷新日期
        /// </summary>
        public DateTime FreshAddPayDate
        {
            set
            {
                this.freshAddPayDate = value;
            }
            get
            {
                return this.freshAddPayDate;
            }
        }
        /// <summary>
        /// 结转帐户支出累计
        /// </summary>
        public decimal YearAddPayTurnCost
        {
            set
            {
                this.yearAddPayCost = value;
            }
            get
            {
                return this.yearAddPayCost;
            }

        }
        /// <summary>
        /// 是否公务员
        /// </summary>
        private bool isOffice = false;
        /// <summary>
        /// 是否公务员
        /// </summary>
        public bool IsOffice
        {
            set
            {
                this.isOffice = value;
            }
            get
            {
                return this.isOffice;
            }

        }

        /// <summary>
        /// 医保住院状态
        /// </summary>
        private string inStateForYB = string.Empty;
        /// <summary>
        /// 医保住院状态
        /// </summary>
        public String InStateForYB
        {
            set
            {
                this.inStateForYB = value;
            }
            get
            {
                return this.inStateForYB;
            }
        }
        /// <summary>
        /// 出生地
        /// </summary>
        private string birthPlace = string.Empty;
        /// <summary>
        /// 出生地
        /// </summary>
        public string BirthPlace
        {
            set
            {
                this.birthPlace = value;
            }
            get
            {
                return this.birthPlace;
            }
        }
        /// <summary>
        /// 离院日期
        /// </summary>
        private DateTime leaveHosDate = DateTime.MinValue;
        /// <summary>
        /// 离院日期
        /// </summary>
        public DateTime LeaveHosDate
        {
            set
            {
                this.leaveHosDate = value;
            }
            get
            {
                return this.leaveHosDate;
            }
        }
        /// <summary>
        /// 家庭病床支出累计
        /// </summary>
        private decimal homeBedFeeAddCost = 0;
        /// <summary>
        /// 家庭病床支出累计
        /// </summary>
        public decimal HomeBedFeeAddCost
        {
            set
            {
                this.homeBedFeeAddCost = value;
            }
            get
            {
                return this.homeBedFeeAddCost;
            }
        }
        /// <summary>
        /// 超过最高限额公务员补助支出累计(26) 
        /// </summary>
        private decimal gwyBeyondPayAddCost = 0;
        /// <summary>
        /// 超过最高限额公务员补助支出累计(26) 
        /// </summary>
        public decimal GwyBeyondPayAddCost
        {
            get
            {
                return this.gwyBeyondPayAddCost;
            }
            set
            {
                this.gwyBeyondPayAddCost = value;
            }
        }
        /// <summary>
        /// 离休统筹支出累计
        /// </summary>
        private decimal lxAddPubCost = 0;
        /// <summary>
        /// 离休统筹支出累计
        /// </summary>
        public decimal LxAddPubCost
        {
            set
            {
                this.lxAddPubCost = value;
            }
            get
            {
                return this.lxAddPubCost;
            }
        }
        /// <summary>
        /// 门诊现金支出累计
        /// </summary>
        private decimal cashAddCostForMZ = 0;
        /// <summary>
        /// 门诊现金支出累计
        /// </summary>
        public decimal CashAddCostForMZ
        {
            set
            {
                this.cashAddCostForMZ = value;
            }
            get
            {
                return this.cashAddCostForMZ;
            }
        }
        /// <summary>
        /// 门诊公务员补助支出累计
        /// </summary>

        private decimal officalSupplyCostForMZ = 0;
        /// <summary>
        /// 门诊公务员补助支出累计
        /// </summary>
        public decimal OfficalSupplyCostForMZ
        {
            set
            {
                this.officalSupplyCostForMZ = value;
            }
            get
            {
                return this.officalSupplyCostForMZ;
            }
        }
        /// <summary>
        /// 生育保险是否最后结算标志
        /// </summary>
        private bool proceateLastFlag = false;
        /// <summary>
        /// 生育保险是否最后结算标志
        /// </summary>
        public bool ProceateLastFlag
        {
            get
            {
                return proceateLastFlag;
            }
            set
            {
                proceateLastFlag = value;
            }
        }
        /// <summary>
        /// 大额补助
        /// </summary>
        private decimal overCost = 0;
        /// <summary>
        /// 大额补助
        /// </summary>
        public decimal OverCost
        {
            set
            {
                this.overCost = value;
            }
            get
            {
                return this.overCost;
            }
        }

        /// <summary>
        /// 公务员补助支付
        /// </summary>
        private decimal officalCost = 0;
        /// <summary>
        /// 公务员补助支付
        /// </summary>
        public decimal OfficalCost
        {
            set
            {
                this.officalCost = value;
            }
            get
            {
                return this.officalCost;
            }
        }


        //private string reimbFlag = string.Empty;
        //public string ReimbFlag
        //{
        //    set
        //    {
        //        this.reimbFlag = value;
        //    }
        //    get
        //    {
        //        return this.reimbFlag;
        //    }
        //}

        //private int transType = 0;
        //public int TransType
        //{
        //    set
        //    {
        //        this.transType = value;
        //    }
        //    get
        //    {
        //        return this.transType;
        //    }
        //}

        #endregion

        #endregion

        #region 中日友好新增属性
        //{BA600C87-44A9-4dbc-86C7-5478796201A3}开始

        /// <summary>
        /// 是否已经变更
        /// </summary>
        private bool isShifted = false;

        /// <summary>
        /// 是否已经变更
        /// </summary>
        public bool IsShifted
        {
            get
            {
                return this.isShifted;
            }
            set
            {
                this.isShifted = value;
            }
        }

        /// <summary>
        /// 变更记录
        /// </summary>
        private Neusoft.HISFC.Models.Base.ShiftRecord shiftRecord = new Neusoft.HISFC.Models.Base.ShiftRecord();

        /// <summary>
        /// 变更记录
        /// </summary>
        public Neusoft.HISFC.Models.Base.ShiftRecord ShiftRecord
        {
            get
            {
                return this.shiftRecord;
            }
            set
            {
                this.shiftRecord = value;
            }
        }

        /// <summary>
        /// 医保上传号
        /// </summary>
        private string transNo;

        /// <summary>
        /// 医保上传号
        /// </summary>
        public string TransNo
        {
            get
            {
                return transNo;
            }
            set
            {
                this.transNo = value;
            }
        }

        /// <summary>
        /// 普通医保内费用
        /// </summary>
        private decimal internalFee;
        /// <summary>
        /// 普通医保内费用
        /// </summary>
        public decimal InternalFee
        {
            get
            {
                return internalFee;
            }
            set
            {
                this.internalFee = value;
            }
        }

        /// <summary>
        /// 普通医保外费用
        /// </summary>
        private decimal externalFee;

        /// <summary>
        /// 普通医保外费用
        /// </summary>
        public decimal ExternalFee
        {
            get
            {
                return externalFee;
            }
            set
            {
                this.externalFee = value;
            }
        }
        /// <summary>
        /// 大额/公务员自付金额
        /// </summary>
        private decimal officalOwnCost;

        /// <summary>
        /// 大额/公务员自付金额
        /// </summary>
        public decimal OfficalOwnCost
        {
            get
            {
                return officalCost;
            }
            set
            {
                this.officalCost = value;
            }
        }

        /// <summary>
        /// 本次交易统筹封顶后医保内金额
        /// </summary>
        private decimal overInterFee;

        /// <summary>
        /// 本次交易统筹封顶后医保内金额
        /// </summary>
        public decimal OverInterFee
        {
            get
            {
                return this.overInterFee;
            }
            set
            {
                this.overInterFee = value;
            }
        }

        /// <summary>
        /// 个人应付总金额(个人帐户支付+现金)
        /// </summary>
        private decimal ownCountFee;
        /// <summary>
        /// 个人应付总金额(个人帐户支付+现金)
        /// </summary>
        public decimal OwnCountFee
        {
            set
            {
                this.ownCountFee = value;
            }
            get
            {
                return ownCountFee;
            }
        }

        /// <summary>
        /// 个人自付二金额
        /// </summary>
        private decimal ownSecondCountFee;
        /// <summary>
        /// 个人自付二金额
        /// </summary>
        public decimal OwnSecondCountFee
        {
            get
            {
                return ownSecondCountFee;
            }
            set
            {
                this.ownSecondCountFee = value;
            }
        }

        /// <summary>
        /// 医保诊断代码
        /// </summary>
        private string siDiagnose = "";
        /// <summary>
        /// 医保诊断代码
        /// </summary>
        public string SIDiagnose
        {
            set
            {
                this.siDiagnose = value;
            }
            get
            {
                return this.siDiagnose;
            }
        }
        /// <summary>
        /// 医保诊断代码名称
        /// </summary>
        private string siDiagnoseName = "";
        /// <summary>
        /// 医保诊断代码名称
        /// </summary>
        public string SIDiagnoseName
        {
            set
            {
                this.siDiagnoseName = value;
            }
            get
            {
                return this.siDiagnoseName;
            }
        }
        /// <summary>
        /// 结算状态：1 结算 0 未结算
        /// </summary>
        private string balanceState = "";
        /// <summary>
        /// 结算状态：1 结算 0 未结算
        /// </summary>
        public string BalanceState
        {
            get
            {
                return this.balanceState;
            }
            set
            {
                this.balanceState = value;
            }
        }
        /// <summary>
        /// 交易类型 1：正交易 2：反交易
        /// </summary>
        private string transType = "";
        /// <summary>
        /// 交易类型 1：正交易 2：反交易
        /// </summary>
        public string TransType
        {
            get
            {
                return this.transType;
            }
            set
            {
                this.transType = value;
            }
        }
        /// <summary>
        /// 结算分类1-门诊2-住院
        /// </summary>
        private string typeCode = "";
        /// <summary>
        /// 结算分类1-门诊2-住院
        /// </summary>
        public string TypeCode
        {
            get
            {
                return this.typeCode;
            }
            set
            {
                this.typeCode = value;
            }
        }
        /// <summary>
        /// 统筹支付金额
        /// </summary>
        private decimal pubFeeCost = 0M;
        /// <summary>
        /// 统筹支付金额
        /// </summary>
        public decimal PubFeeCost
        {
            get
            {
                return this.pubFeeCost;
            }
            set
            {
                this.pubFeeCost = value;
            }
        }
        /// <summary>
        /// 是否已经交医保手册(单独使用，医保中间表不保存)
        /// </summary>
        private bool isGetSSN = false;
        /// <summary>
        /// 是否已经交医保手册(单独使用，医保中间表不保存)
        /// </summary>
        public bool IsGetSSN
        {
            get
            {
                return this.isGetSSN;
            }
            set
            {
                this.isGetSSN = value;
            }
        }
        //{BA600C87-44A9-4dbc-86C7-5478796201A3}结束
        #endregion

        #region 佛山禅城区居民医保增加属性
        // {4669B819-39AB-476b-B3A1-60AAF150FD45}
        private long ybMedNo = 0;
        /// <summary>
        /// 居民医保结算单号
        /// 本定点医院结算单的唯一标识。
        /// 保存以做为 注销居民门诊费用结算 参数之一
        /// </summary>
        public long YBMedNo
        {
            get { return ybMedNo; }
            set { ybMedNo = value; }
        }

        #endregion

        #region 深圳医保增加的属性

        #region 变量
        private string cblx = "";
        public string lxcbys = "";
        private string jbkyye = "";
        private string bckyye = "";

        #endregion

        public string CBLX
        {
            get
            {
                return cblx;
            }
            set
            {
                cblx = value;
            }
        }

        /// <summary>
        /// 连续参保月数
        /// </summary>


        public string LXCBYS
        {
            get
            {
                return lxcbys;
            }
            set
            {
                lxcbys = value;
            }

        }
        /// <summary>
        /// 基本医疗保险共济基金可用余额（当前）
        /// </summary>

        public string JBKYYE
        {
            get
            {
                return jbkyye;
            }
            set
            {
                jbkyye = value;
            }

        }
        /// <summary>
        /// 地方补充医疗保险共济基金可用余额（当前）
        /// </summary>
        public string BCKYYE
        {
            get
            {
                return bckyye;
            }
            set
            {
                bckyye = value;

            }

        }
        /// <summary>
        /// //--监护人1姓名*
        /// </summary>
        private string jhr1xm;
        public string JHR1XM
        {
            get
            {
                return jhr1xm;
            }
            set
            {
                jhr1xm = value;
            }
        }
        /// <summary>
        /// //--监护人2姓名*
        /// </summary>
        private string jhr2xm;
        public string JHR2XM
        {
            get
            {
                return jhr2xm;
            }
            set
            {
                jhr2xm = value;
            }
        }
        /// <summary>
        /// 监护人1身份证号*
        /// </summary>
        private string jhr1sfzh;   //--监护人1身份证号*
        public string JHR1SFZH
        {
            get
            {
                return jhr1sfzh;
            }

            set
            {
                jhr1sfzh = value;
            }
        }
        /// <summary>
        /// //--监护人1身份证号*
        /// </summary>
        private string jhr2sfzh;
        public string JHR2SFZH
        {
            get
            {
                return jhr2sfzh;
            }

            set
            {
                jhr2sfzh = value;
            }

        }
        /// <summary>
        /// 单据号
        /// </summary>
        private string djh;
        public string DJH
        {
            get
            {

                return djh;
            }

            set
            {
                djh = value;
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        private string cardpassword;
        public string CardPassWord
        {
            get
            {
                return cardpassword;
            }
            set
            {
                cardpassword = value;
            }

        }
        /// <summary>
        /// 医保人员对照码
        /// </summary>
        private string siemployeecode;
        public string SiEmployeeCode
        {
            get
            {
                return siemployeecode;
            }
            set
            {
                siemployeecode = value;
            }
        }

        /// <summary>
        /// 医保人员对照码
        /// </summary>
        private string siemployeename;
        public string SiEmployeeName
        {
            get
            {
                return siemployeename;
            }
            set
            {
                siemployeename = value;
            }
        }

        /// <summary>
        /// 门诊是否上传标志
        /// </summary>
        private string siUploadFeeFlag = "";
        public string SiUploadFeeFlag
        {
            get
            {
                return siUploadFeeFlag;
            }
            set
            {
                siUploadFeeFlag = value;
            }

        }
        #endregion

        #region 珠海医保增加属性

        private string siType = "";
        /// <summary>
        /// 参保险种
        /// 1 未成年医保
        /// 2 居民医保
        /// 3 基本医疗
        /// 4 基本医疗+补助
        /// 5 大病医保
        /// 6 生育医保
        /// 7 工伤医保
        /// 8 门诊统筹
        /// </summary>
        public string SiType
        {
            get { return this.siType; }
            set { this.siType = value; }
        }

        private string tacCode;

        /// <summary>
        /// tac码
        /// 珠海医保交易验证码
        /// </summary>
        public string TacCode
        {
            get { return tacCode; }
            set { tacCode = value; }
        }

        private string tacStatus;

        /// <summary>
        /// tac码状态
        /// 珠海医保交易验证码
        /// </summary>
        public string TacStatus
        {
            get { return tacStatus; }
            set { tacStatus = value; }
        }

        private string tacType;

        /// <summary>
        /// tac码
        /// 珠海医保交易验证码
        /// </summary>
        public string TacType
        {
            get { return tacType; }
            set { tacType = value; }
        }
        #endregion

        #region 广东省统一医保增加属性
        /// <summary>
        /// bka911	String	10	否	手术日期	生育
        /// </summary>
        private DateTime bka911;

        /// <summary>
        /// Bka911	String	10	否	手术日期	生育
        /// </summary>
        public DateTime Bka911
        {
            get { return bka911; }
            set { bka911 = value; }
        }

        /// <summary>
        /// bka912	String	10	否	生育类别	生育
        /// </summary>
        private string bka912 = string.Empty;

        /// <summary>
        /// Bka912	String	10	否	生育类别	生育
        /// </summary>
        public string Bka912
        {
            get { return bka912; }
            set { bka912 = value; }
        }

        /// <summary>
        ///  amc050 string 10  生育业务类型    生育
        /// </summary>
        private string amc050 = string.Empty;
        /// <summary>
        /// amc050 string 10  生育业务类型    生育
        /// </summary>
        public string Amc050
        {
            get { return amc050; }
            set { amc050 = value; }
        }
        /// <summary>
        /// amc 029 string 10 生育手术类别  生育
        /// </summary>
        private string amc029 = string.Empty;

        /// <summary>
        /// amc 029 string 10 生育手术类别  生育
        /// </summary>
        public string Amc029
        {
            get { return amc029; }
            set { amc029 = value; }
        }
        /// <summary>
        /// amc 029 string 10 生育胎次  生育
        /// </summary>
        private string amc031 = string.Empty;

        /// <summary>
        /// amc 029 string 10 生育胎次  生育
        /// </summary>
        public string Amc031
        {
            get { return amc031; }
            set { amc031 = value; }
        }
        /// <summary>
        /// bka913	String	10	否	胎儿数	生育
        /// </summary>
        private int bka913 = 0;

        /// <summary>
        /// Bka913	String	10	否	胎儿数	生育
        /// </summary>
        public int Bka913
        {
            get { return bka913; }
            set { bka913 = value; }
        }

        /// <summary>
        /// bka914	String	10	否	母亲情况	生育
        /// </summary>
        private string bka914 = string.Empty;

        /// <summary>
        /// bka914	String	10	否	母亲情况	生育
        /// </summary>
        public string Bka914
        {
            get { return bka914; }
            set { bka914 = value; }
        }

        /// <summary>
        /// bka915	String	10	否	母亲死亡时间	生育、格式：yyyyMMdd
        /// </summary>
        private DateTime bka915;

        /// <summary>
        /// Bka915	String	10	否	母亲死亡时间	生育、格式：yyyyMMdd
        /// </summary>
        public DateTime Bka915
        {
            get { return bka915; }
            set { bka915 = value; }
        }

        /// <summary>
        /// bka916	String	10	否	婴儿情况	生育
        /// </summary>
        private string bka916 = string.Empty;

        /// <summary>
        /// Bka916	String	10	否	婴儿情况	生育
        /// </summary>
        public string Bka916
        {
            get { return bka916; }
            set { bka916 = value; }
        }

        /// <summary>
        /// bka917	String	10	否	婴儿死亡时间	生育、格式：yyyyMMdd
        /// </summary>
        private DateTime bka917;

        /// <summary>
        /// Bka917	String	10	否	婴儿死亡时间	生育、格式：yyyyMMdd
        /// </summary>
        public DateTime Bka917
        {
            get { return bka917; }
            set { bka917 = value; }
        }


        /// <summary>
        /// bka042	String	20	工伤生育凭证号	只工伤、生育业务，才有此项 
        /// </summary>
        private string bka042 = string.Empty;

        /// <summary>
        /// Bka042	String	20	工伤生育凭证号	只工伤、生育业务，才有此项 
        /// </summary>
        public string Bka042
        {
            get { return bka042; }
            set { bka042 = value; }
        }

        /// <summary>
        /// aaz267	String	12	门诊选点、门慢申请序列号	 
        /// </summary>
        private string aaz267 = string.Empty;

        /// <summary>
        /// Aaz267	String	12	门诊选点、门慢申请序列号	
        /// </summary>
        public string Aaz267
        {
            get { return aaz267; }
            set { aaz267 = value; }
        }

        /// <summary>
        /// bka825	String	12	全自费费用	
        /// </summary>
        private decimal bka825 = 0m;

        /// <summary>
        /// Bka825	String	12	全自费费用	
        /// </summary>
        public decimal Bka825
        {
            get { return bka825; }
            set { bka825 = value; }
        }

        /// <summary>
        /// bka826	String	12	部分自费费用
        /// </summary>
        private decimal bka826 = 0m;

        /// <summary>
        /// Bka826	String	12	部分自费费用
        /// </summary>
        public decimal Bka826
        {
            get { return bka826; }
            set { bka826 = value; }
        }

        /// <summary>
        /// aka151	String	12	起付线费用
        /// </summary>
        private decimal aka151 = 0m;

        /// <summary>
        /// Aka151	String	12	起付线费用
        /// </summary>
        public decimal Aka151
        {
            get { return aka151; }
            set { aka151 = value; }
        }

        /// <summary>
        /// bka838	String	12	超共付段费用个人自付		
        /// </summary>
        private decimal bka838 = 0m;

        /// <summary>
        /// Bka838	String	12	超共付段费用个人自付	
        /// </summary>
        public decimal Bka838
        {
            get { return bka838; }
            set { bka838 = value; }
        }

        /// <summary>
        /// akb067	String	12	个人现金支付
        /// </summary>
        private decimal akb067 = 0m;

        /// <summary>
        /// Akb067	String	12	个人现金支付	
        /// </summary>
        public decimal Akb067
        {
            get { return akb067; }
            set { akb067 = value; }
        }

        /// <summary>
        /// akb066	String	12	个人账户支付
        /// </summary>
        private decimal akb066 = 0m;

        /// <summary>
        /// Akb066	String	12	个人账户支付
        /// </summary>
        public decimal Akb066
        {
            get { return akb066; }
            set { akb066 = value; }
        }

        /// <summary>
        /// bka821	String	12	民政救助金支付	
        /// </summary>
        private decimal bka821 = 0m;

        /// <summary>
        /// Bka821	String	12	民政救助金支付	
        /// </summary>
        public decimal Bka821
        {
            get { return bka821; }
            set { bka821 = value; }
        }

        /// <summary>
        /// bka839	String	12	其他支付	
        /// </summary>
        private decimal bka839 = 0m;

        /// <summary>
        /// Bka839	String	12	其他支付	
        /// </summary>
        public decimal Bka839
        {
            get { return bka839; }
            set { bka839 = value; }
        }

        /// <summary>
        /// ake039	String	12	医疗保险统筹基金支付
        /// </summary>
        private decimal ake039 = 0m;

        /// <summary>
        /// Ake039	String	12	医疗保险统筹基金支付
        /// </summary>
        public decimal Ake039
        {
            get { return ake039; }
            set { ake039 = value; }
        }

        /// <summary>
        /// ake035	String	12	公务员医疗补助基金支付
        /// </summary>
        private decimal ake035 = 0m;

        /// <summary>
        /// Ake035	String	12	公务员医疗补助基金支付
        /// </summary>
        public decimal Ake035
        {
            get { return ake035; }
            set { ake035 = value; }
        }

        /// <summary>
        /// ake026	String	12	企业补充医疗保险基金支付	
        /// </summary>
        private decimal ake026 = 0m;

        /// <summary>
        /// Ake026	String	12	企业补充医疗保险基金支付	
        /// </summary>
        public decimal Ake026
        {
            get { return ake026; }
            set { ake026 = value; }
        }

        /// <summary>
        /// ake029	String	12	大额医疗费用补助基金支付
        /// </summary>
        private decimal ake029 = 0m;

        /// <summary>
        /// Ake029	String	12	大额医疗费用补助基金支付
        /// </summary>
        public decimal Ake029
        {
            get { return ake029; }
            set { ake029 = value; }
        }

        /// <summary>
        /// bka841	String	12	单位支付	
        /// </summary>
        private decimal bka841 = 0m;

        /// <summary>
        /// Bka841	String	12	单位支付
        /// </summary>
        public decimal Bka841
        {
            get { return bka841; }
            set { bka841 = value; }
        }

        /// <summary>
        /// bka842	String	12	医院垫付
        /// </summary>
        private decimal bka842 = 0m;

        /// <summary>
        /// Bka842	String	12	医院垫付
        /// </summary>
        public decimal Bka842
        {
            get { return bka842; }
            set { bka842 = value; }
        }

        /// <summary>
        /// bka840	String	12	其他基金支付
        /// </summary>
        private decimal bka840 = 0m;

        /// <summary>
        /// Bka840	String	12	其他基金支付
        /// </summary>
        public decimal Bka840
        {
            get { return bka840; }
            set { bka840 = value; }
        }

        /// <summary>
        /// aaa027	String	6	是	医保分中心编码
        /// </summary>
        private string aaa027 = string.Empty;

        /// <summary>
        /// aaa027	String	6	是	医保分中心编码
        /// </summary>
        public string Aaa027
        {
            get { return aaa027; }
            set { aaa027 = value; }
        }

        /// <summary>
        /// bka438	String	2	业务场景阶段 0：业务未开始 	1：业务开始2：业务结算3：业务结束 (用于预结算) 
        /// 在住院中的费用录入时试算，传1；在出院登记保存时试算，传3；在出院结算时试算，传2
        /// </summary>
        private string bka438 = string.Empty;

        /// <summary>
        /// bka438	String	2	业务场景阶段 0：业务未开始 	1：业务开始2：业务结算3：业务结束 (用于预结算) 
        /// </summary>
        public string Bka438
        {
            get { return bka438; }
            set { bka438 = value; }
        }

        /// <summary>
        /// 参保人所属行政区划代码
        /// </summary>
        private string aab301 = string.Empty;

        /// <summary>
        /// 参保人所属行政区划代码
        /// </summary>
        public string Aab301
        {
            get { return aab301; }
            set { aab301 = value; }
        }

        /// <summary>
        /// 险种编码
        /// 310——"城镇职工基本医疗"
        /// 391——"城乡居民基本医疗"
        /// 410——"工伤"
        /// 510——"生育"
        /// </summary>
        private string aae140 = string.Empty;

        /// <summary>
        /// 险种编码
        /// 310——"城镇职工基本医疗"
        /// 391——"城乡居民基本医疗"
        /// 410——"工伤"
        /// 510——"生育"
        /// </summary>
        public string Aae140
        {
            get { return aae140; }
            set { aae140 = value; }
        }

        /// <summary>
        /// bka006	String	6	是	医疗待遇类型
        /// </summary>
        private string bka006 = string.Empty;
        /// <summary>
        /// bka006	String	6	是	医疗待遇类型
        /// </summary>
        public string Bka006
        {
            get { return bka006; }
            set { bka006 = value; }
        }
        public string Bka006Name { get; set; }
        /// <summary>
        /// aka130	String	6	是	医疗业务类型
        /// </summary>
        private string aka130 = string.Empty;
        /// <summary>
        /// aka130	String	医疗业务类型 11门诊 12住院 13门慢 16门特 41工伤门诊 42工伤住院 51生育门诊 52生育住院
        /// </summary>
        public string Aka130
        {
            get { return aka130; }
            set { aka130 = value; }
        }
        private string bka020 = string.Empty;
        /// <summary>
        /// bka020	String	 就诊科室名称
        /// </summary>
        public string Bka020
        {
            get { return bka020; }
            set { bka020 = value; }
        }
        private string bka004 = string.Empty;
        /// <summary>
        /// bka004	String	 人员类别
        /// </summary>
        public string Bka004
        {
            get { return bka004; }
            set { bka004 = value; }
        }

        /// <summary>
        /// 是否读取社保卡
        /// </summary>
        private bool isUserSICard = true;
        /// <summary>
        /// 是否读取社保卡,默认是需要读卡，由于省内异地有些患者没带卡，信息科要求这类患者不需要读卡校验
        /// </summary>
        public bool IsUserSICard
        {
            get { return isUserSICard; }
            set { isUserSICard = value; }
        }

        /// <summary>
        /// 结算召回是否要取消医保联网结算的数据
        /// </summary>
        private bool isCancelSIBanlance = true;
        /// <summary>
        /// 结算召回是否要取消医保联网结算的数据
        /// </summary>
        public bool IsCancelSIBanlance
        {
            get { return isCancelSIBanlance; }
            set { isCancelSIBanlance = value; }
        }

        /// <summary>
        /// bka841	String	12	医保支付金额	
        /// </summary>
        private decimal bka811 = 0m;

        /// <summary>
        /// Bka841	String	12	医保支付金额
        /// </summary>
        public decimal Bka811
        {
            get { return bka811; }
            set { bka811 = value; }
        }

        /// <summary>
        /// bka841	String	12	个人支付金额	
        /// </summary>
        private decimal bka812 = 0m;

        /// <summary>
        /// Bka841	String	12	个人支付金额
        /// </summary>
        public decimal Bka812
        {
            get { return bka812; }
            set { bka812 = value; }
        }
        #endregion

        #region 广东省珠海新医保平台增加属性
        /// <summary>
        /// 卡识别码PsnCertType
        /// </summary>
        public string CardSn { get; set; }
        /// <summary>
        /// 挂号科室
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 计划生育手术类别 生育门诊按需录入
        /// </summary>
        public string BirctrlType { get; set; }

        /// <summary>
        /// 计划生育手术或生育日期 生育门诊按需录入，yyyy-MM-dd
        /// </summary>
        public string BirctrlMatnDate { get; set; }

        /// <summary>
        /// 经办人类别 1-经办人；2-自助终端；3-移动终端
        /// </summary>
        public string OpterType { get; set; }

        /// <summary>
        /// 操作人编号
        /// </summary>
        public string OpterCode { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OpterName { get; set; }

        /// <summary>
        /// 住院/门诊流水号
        /// </summary>
        public string IptOptNo { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string PsnNo { get; set; }
        /// <summary>
        /// 就诊ID(医保返回唯一流水号)
        /// </summary>
        public string MdtrtID { get; set; }
        /// <summary>
        /// 就诊凭证类型
        /// </summary>
        public string MdtrtCertType { get; set; }
        /// <summary>
        /// 就诊凭证编号 就诊凭证类型为“01”时填写电子凭证令牌，为“02”时填写身份证号，为“03”时填写社会保障卡卡号
        /// </summary>
        public string MdtrtCertNo { get; set; }
        /// <summary>
        /// 收费批次号
        /// </summary>
        public string ChargeBatchNumber { get; set; }
        /// <summary>
        /// 最后一次修改人编码
        /// </summary>
        public string LastModifyCode { get; set; }
        /// <summary>
        /// 最后一次修改人名称
        /// </summary>
        public string LastModifyName { get; set; }
        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        public string LastModifyTime { get; set; }
        /// <summary>
        /// 结算ID
        /// </summary>
        public string SetlId { get; set; }

        /// <summary>
        /// 人员证件类型
        /// </summary>
        public string PsnCertType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string Certno { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gend { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Naty { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public decimal Age { get; set; }

        /// <summary>
        /// 险种类型
        /// </summary>
        public string Insutype { get; set; }

        /// <summary>
        /// 人员类别
        /// </summary>
        public string PsnType { get; set; }

        /// <summary>
        /// 公务员标志
        /// </summary>
        public string CvlservFlag { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public string SetlTime { get; set; }

        /// <summary>
        /// 个人结算方式
        /// </summary>
        public string PsnSetlway { get; set; }

        /// <summary>
        /// 医疗类别
        /// </summary>
        public string MedType { get; set; }

        /// <summary>
        /// 医疗费总额
        /// </summary>
        public decimal MedfeeSumamt { get; set; }

        /// <summary>
        /// 全自费金额
        /// </summary>
        public decimal FulamtOwnpayAmt { get; set; }

        /// <summary>
        /// 超限价自费费用
        /// </summary>
        public decimal OverlmtSelfpay { get; set; }

        /// <summary>
        /// 先行自付金额
        /// </summary>
        public decimal PreselfpayAmt { get; set; }

        /// <summary>
        /// 符合范围金额
        /// </summary>
        public decimal InscpScpAmt { get; set; }

        /// <summary>
        /// 医保认可费用总额
        /// </summary>
        public decimal MedSumfee { get; set; }

        /// <summary>
        /// 实际支付起付线
        /// </summary>
        public decimal ActPayDedc { get; set; }

        /// <summary>
        /// 基本医疗保险统筹基金支出
        /// </summary>
        public decimal HifpPay { get; set; }

        /// <summary>
        /// 基本医疗保险统筹基金支付比例
        /// </summary>
        public decimal PoolPropSelfpay { get; set; }

        /// <summary>
        /// 公务员医疗补助资金支出
        /// </summary>
        public decimal CvlservPay { get; set; }

        /// <summary>
        /// 企业补充医疗保险基金支出
        /// </summary>
        public decimal HifesPay { get; set; }

        /// <summary>
        /// 居民大病保险资金支出
        /// </summary>
        public decimal HifmiPay { get; set; }

        /// <summary>
        /// 职工大额医疗费用补助基金支出
        /// </summary>
        public decimal HifobPay { get; set; }

        /// <summary>
        /// 伤残人员医疗保障基金支出
        /// </summary>
        public decimal HifdmPay { get; set; }

        /// <summary>
        /// 医疗救助基金支出
        /// </summary>
        public decimal MafPay { get; set; }

        /// <summary>
        /// 其他基金支出
        /// </summary>
        public decimal OthPay { get; set; }
        /// <summary>
        /// 优抚报销金额
        /// </summary>
        public decimal OwnpayHospPart { get; set; }

        /// <summary>
        /// 基金支付总额
        /// </summary>
        public decimal FundPaySumamt { get; set; }

        /// <summary>
        /// 医院负担金额
        /// </summary>
        public decimal HospPartAmt { get; set; }

        /// <summary>
        /// 个人负担总金额
        /// </summary>
        public decimal PsnPartAmt { get; set; }

        /// <summary>
        /// 个人账户支出
        /// </summary>
        public decimal AcctPay { get; set; }

        /// <summary>
        /// 现金支付金额
        /// </summary>
        public decimal PsnCashPay { get; set; }

        /// <summary>
        /// 账户共济支付金额
        /// </summary>
        public decimal AcctMulaidPay { get; set; }

        /// <summary>
        /// 个人账户支出后余额
        /// </summary>
        public decimal Balc { get; set; }

        /// <summary>
        /// 清算经办机构
        /// </summary>
        public string ClrOptins { get; set; }

        /// <summary>
        /// 清算方式
        /// </summary>
        public string ClrWay { get; set; }

        /// <summary>
        /// 清算类别
        /// </summary>
        public string ClrType { get; set; }

        /// <summary>
        /// 医药机构结算ID
        /// </summary>
        public string MedinsSetlId { get; set; }

        /// <summary>
        /// 就医地医保区划
        /// </summary>
        public string MdtrtareaAdmvs { get; set; }

        /// <summary>
        /// 参保地医保区划
        /// </summary>
        public string InsuplcAdmdvs { get; set; }


        /// <summary>
        /// 接收方医保区划代码
        /// </summary>
        public string RecerAdmvs { get; set; }

        /// <summary>
        /// 计划生育服务证号
        /// </summary>
        public string FpscNo { get; set; }

        /// <summary>
        /// 生育类别
        /// </summary>
        public string MatnType { get; set; }


        /// <summary>
        /// 晚育标志
        /// </summary>
        public string LatechbFlag { get; set; }

        /// <summary>
        /// 孕周数
        /// </summary>
        public string GesoVal { get; set; }

        /// <summary>
        /// 胎次
        /// </summary>
        public string Fetts { get; set; }

        /// <summary>
        /// 胎儿数
        /// </summary>
        public int FetusCnt { get; set; }

        /// <summary>
        /// 早产标志
        /// </summary>
        public string PretFlag { get; set; }

        /// <summary>
        /// 医院编码
        /// </summary>
        public string Fixmedinscode { get; set; }

        /// <summary>
        /// 医院名称
        /// </summary>
        public string Fixmedinsname { get; set; }

        /// <summary>
        /// 个人账户使用标志
        /// </summary>
        public string AcctUsedFlag { get; set; }

        /// <summary>
        /// 中途结算标志
        /// </summary>
        public string MidSetlFlag { get; set; }

        /// <summary>
        /// 手术操作名称
        /// </summary>
        public string OprnOprtName { get; set; }
        /// <summary>
        /// 手术操作代码
        /// </summary>
        public string OprnOprtCode { get; set; }

        #endregion
        /// <summary>
        /// 渠道枚举
        /// </summary>
        public EnumCallAPIChannel enumCallAPIChannel { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BegnTime { get; set; }
        public string PsnName { get; set; }


        /// <summary>
        /// 复核标识：0 未复核；1 已复核；2 取消复核；
        /// </summary>
        public string CheckFlag { get; set; }
        /// <summary>
        /// 复核人工号
        /// </summary>
        public string CheckOperCode { get; set; }
        /// <summary>
        /// 复核时间
        /// </summary>
        public DateTime CheckDate { get; set; }
        /// <summary>
        /// 取消复核原因
        /// </summary>
        public string CheckCancelReason { get; set; }

        /// <summary>
        /// 收费模式 0正常模式  1记账模式  2家庭共济模式 3虚帐模式
        /// </summary>
        public int FeeType { get; set; }

    }
}
