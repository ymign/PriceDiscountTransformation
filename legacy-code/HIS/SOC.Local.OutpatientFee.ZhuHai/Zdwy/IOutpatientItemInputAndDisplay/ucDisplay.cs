using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.Models.Registration;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Models;
using Neusoft.FrameWork.Management;
using FarPoint.Win.Spread;
using Neusoft.HISFC.Models.Base;
using System.Threading;
using System.Collections.Generic;
using Neusoft.HISFC.Models.Fee.Item;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientItemInputAndDisplay
{
    public partial class ucDisplay : UserControl, Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientItemInputAndDisplay, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        public ucDisplay()
        {
            InitializeComponent();
        }

        #region 变量

        private bool isTransferTreat = false;
        /// <summary>
        /// 是否转诊
        /// </summary>
        public bool IsTransferTreat
        {
            get { return isTransferTreat; }
            set { isTransferTreat = value; }
        }

        /// <summary>
        /// 是否退费调用
        /// </summary>
        protected bool isQuitFee = false;

        /// <summary>
        /// 输入项目控件是否获得焦点
        /// </summary>
        protected bool isFocus = false;

        /// <summary>
        /// 价格警戒线颜色
        /// </summary>
        protected int priceWarinningColor = 0;

        /// <summary>
        /// 价格警戒线
        /// </summary>
        protected decimal priceWarnning = 0;
        /// <summary>
        /// 公医价格限额
        /// </summary>
        protected decimal sumPubCost = 0;
        /// <summary>
        /// 费用转换数量
        /// </summary>
        int itenqty = 0;
        /// <summary>
        /// 每次用量是否可以为空
        /// </summary>
        protected bool isDoseOnceNull = true;

        /// <summary>
        /// 总量是否上取整
        /// </summary>
        protected bool isQtyToCeiling = false;

        /// <summary>
        /// 是否可以增加项目;
        /// </summary>
        protected bool isCanAddItem = false;

        /// <summary>
        /// 显示缺药药品
        /// </summary>
        protected bool displayLackPha = false;

        /// <summary>
        /// 患者基本信息
        /// </summary>
        protected Register rInfo = null;

        /// <summary>
        /// 临时挂号科室
        /// </summary>
        protected string tempDept = null;

        /// <summary>
        /// 没有挂号患者,卡号第一位标志,默认以9开头
        /// </summary>
        protected string noRegFlagChar = "9";

        /// <summary>
        /// 临时挂号费费用编码
        /// </summary>
        protected string regFeeItemCode = string.Empty;

        /// <summary>
        /// 自费诊查费项目编码
        /// </summary>
        protected string ownDiagFeeCode = string.Empty;

        /// <summary>
        /// 通用挂号级别
        /// </summary>
        protected string comRegLevel = string.Empty;

        /// <summary>
        /// 默认的收费包装单位
        /// </summary>
        protected string defaultPriceUnit = "0";

        /// <summary>
        /// 频次显示形式
        /// </summary>
        protected string freqDisplayType = "0";

        /// <summary>
        /// 错误信息
        /// </summary>
        protected string errText = string.Empty;

        /// <summary>
        /// 自费病人显示医保标记
        /// </summary>
        protected bool isOwnDisplayYB = false;

        /// <summary>
        /// 医保代码，该合同单位的对照信息中的医保目录等级用于公费时项目的费用项目显示甲乙类
        /// </summary>
        protected string ybPactCode = string.Empty;

        /// <summary>
        /// 门诊收费的界面上是否可以不录入用法频次等
        /// </summary>
        protected bool isDoseOnceCanNull = false;

        /// <summary>
        /// 收费组套维护，药品现实最小单位
        /// </summary>
        protected bool isShowMinPact = false;
        /// <summary>
        /// 是否可以更改划价信息
        /// </summary>
        protected bool isCanModifyCharge = false;

        /// <summary>
        /// 划价信息
        /// </summary>
        protected ArrayList alChargeInfo = null;

        /// <summary>
        /// 是否判断库存
        /// </summary>
        protected bool isJudgeStore = false;

        /// <summary>
        /// 是否判断预扣库存
        /// </summary>
        protected bool isUsePreStore = false;

        /// <summary>
        /// 添加的行
        /// </summary>
        private ArrayList alAddRows = new ArrayList();

        /// <summary>
        /// 公费待遇
        /// </summary>
        private ArrayList alBillPact = new ArrayList();

        /// <summary>
        /// 当前收费序列
        /// </summary>
        protected string recipeSeq = string.Empty;

        /// <summary>
        /// 院注次数
        /// </summary>
        private decimal injec = 0;

        /// <summary>
        /// 默认草药付数
        /// </summary>
        private decimal hDays = 1;

        /// <summary>
        /// 返回值
        /// </summary>
        private int iReturn = 0;

        /// <summary>
        /// 当前控件是否有效
        /// </summary>
        private bool isValid = true;

        /// <summary>
        /// 手工输入项目是否要判断患者信息
        /// </summary>
        private bool isInputItemsNoSpe = true;

        /// <summary>
        /// 操作员信息 操作员基本信息
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee myOperator = new Neusoft.HISFC.Models.Base.Employee();
        /// <summary>
        /// 加载项目类别
        /// </summary>
        protected Neusoft.HISFC.Models.Base.ItemKind itemKind = Neusoft.HISFC.Models.Base.ItemKind.All;
        /// <summary>
        /// 非药品业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Item undrugManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        //{CA82280B-51B6-4462-B63E-43F4ECF456A3}
        Dictionary<string, Neusoft.FrameWork.Models.NeuObject> dictDept = new Dictionary<string, Neusoft.FrameWork.Models.NeuObject>();

        /// <summary>
        /// 参数控制类
        /// </summary>
        private Neusoft.FrameWork.Management.ControlParam ctlMgr = new Neusoft.FrameWork.Management.ControlParam();
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string filePath = Application.StartupPath + @".\profile\门诊收费项目录入.xml";

        /// <summary>
        /// 门诊收费员是否可以删除医生的医嘱:true可以删除；false不可以删除
        /// </summary>
        private bool isCanDeleteDoctOrder = true;

        /// <summary>
        /// 新收费模式明细
        /// </summary>
        private Hashtable hsItemZT = null;


        #region 业务层变量

        /// <summary>
        /// 常数管理业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant consManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 挂号业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Registration.Registration registerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Registration.Registration();

        /// <summary>
        /// 管理业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();

        /// <summary>
        /// 医嘱业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Order orderIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Order();

        /// <summary>
        /// 非药品组合项目业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.UndrugPackAge undrugPackAgeManager = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge();

        /// <summary>
        /// 合同单位比例管理业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.PactUnitItemRate pactUnitItemRateManager = new Neusoft.HISFC.BizLogic.Fee.PactUnitItemRate();

        /// <summary>
        /// 医保接口业务层(本地)
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Interface interfaceManager = new Neusoft.HISFC.BizLogic.Fee.Interface();

        /// <summary>
        /// 药品业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Pharmacy pharmacyIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();

        /// <summary>
        /// 优惠业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.EcoRate ecoRateManager = new Neusoft.HISFC.BizLogic.Fee.EcoRate();

        /// <summary>
        /// 控制参数业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        /// <summary>
        /// 费用综合业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 常数业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Constant constantManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 科室业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Department departManager = new Neusoft.HISFC.BizLogic.Manager.Department();

        /// <summary>
        /// 复合项目业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.UndrugztManager ztManager = new Neusoft.HISFC.BizLogic.Manager.UndrugztManager();
        /// <summary>
        /// 计算限制收费项目
        /// </summary>
        ZDWY.SpecialRule.Price.Restrictingfee setRestrictingfee = new ZDWY.SpecialRule.Price.Restrictingfee();

        #endregion

        #region 列表变量

        /// <summary>
        /// 频次列表
        /// </summary>
        private ArrayList alFreq = new ArrayList();

        /// <summary>
        /// 用法列表
        /// </summary>
        private ArrayList alUsage = new ArrayList();

        /// <summary>
        /// 科室信息
        /// </summary>
        private ArrayList alDept = new ArrayList();

        /// <summary>
        /// 院注项目集合
        /// </summary>
        private ArrayList alInjec = new ArrayList();

        #endregion

        /// <summary>
        /// 发票信息
        /// </summary>
        private DataSet dsInvoice = new DataSet();

        /// <summary>
        /// 加载的项目
        /// </summary>
        DataSet dsItem = new DataSet();

        /// <summary>
        /// 项目视图
        /// </summary>
        DataView dvItem = new DataView();

        /// <summary>
        /// 转换的单位
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper invertUnitHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 每次量单位特殊转换的项目
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper specialInvertUnitHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 算入药费的最小费用代码
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper phaFeeCodeHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 公费待遇
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper myBillPactHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper apprItemHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper specialItemHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 用法列表, 查找编码和名称用
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper myHelpUsage = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 频次列表, 查找编码和名称用
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper myHelpFreq = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 设备号
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper myMachineNO = new Neusoft.FrameWork.Public.ObjectHelper();
        //{21C33D5B-5583-4b1d-8023-278336C0C6C7}
        Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade myIGetSiItemGrade = null;

        Neusoft.HISFC.BizProcess.Interface.Fee.ITruncFee myITruncFee = null;


        #region  控件变量

        /// <summary>
        /// 执行科室选择
        /// </summary>
        private Neusoft.FrameWork.WinForms.Controls.PopUpListBox lbDept = new Neusoft.FrameWork.WinForms.Controls.PopUpListBox();

        /// <summary>
        /// 执行科室选择
        /// </summary>
        private Neusoft.FrameWork.WinForms.Controls.PopUpListBox lbMachineNO = new Neusoft.FrameWork.WinForms.Controls.PopUpListBox();

        /// <summary>
        /// 频次选择列表
        /// </summary>
        private Neusoft.FrameWork.WinForms.Controls.PopUpListBox lbFreq = new Neusoft.FrameWork.WinForms.Controls.PopUpListBox();

        /// <summary>
        /// 用法选择列表
        /// </summary>
        private Neusoft.FrameWork.WinForms.Controls.PopUpListBox lbUsage = new Neusoft.FrameWork.WinForms.Controls.PopUpListBox();

        /// <summary>
        /// 院内注射次数控件
        /// </summary>
        private ucInjec myInjec = new ucInjec();

        private Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IChooseItemForOutpatient chooseItemControl;

        /// <summary>
        /// 过滤FarPoint
        /// </summary>
        private FarPoint.Win.Spread.SheetView fpSheetItem = new SheetView();

        /// <summary>
        /// 左侧信息显示列表
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientOtherInfomationLeft leftControl = null;

        /// <summary>
        /// 右侧信息显示列表
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientOtherInfomationRight rightControl = null;

        /// <summary>
        /// 是否处理CellChange事件
        /// </summary>
        private bool isDealCellChange = true;

        #endregion

        #region 事件变量

        /// <summary>
        /// 项目列表发生变化后触发
        /// </summary>
        public event Neusoft.HISFC.BizProcess.Integrate.FeeInterface.delegateFeeItemListChanged FeeItemListChanged;

        #endregion

        //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入
        public System.Threading.Thread threadItemInit = null;
        //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入 结束

        /// <summary>
        /// 是否可以选择项目收费{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
        /// </summary>
        protected bool isCanSelectItemAndFee = false;
        //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束

        /// <summary>
        /// 是否可以输入负数量{0F98A513-A9EA-4110-B35F-E353A390E350}
        /// </summary>
        protected bool isCanInputNegativeQty = false;
        //{0F98A513-A9EA-4110-B35F-E353A390E350}结束

        #endregion

        #region 属性

        private bool isUseNewUndrugZT = false;
        /// <summary>
        /// 是否启用新的检查项目收费模式
        /// </summary>
        public bool IsUseNewUndrugZT
        {
            get
            {
                return isUseNewUndrugZT;
            }
            set
            {
                isUseNewUndrugZT = value;
            }
        }

        /// <summary>
        /// 是否可以选择项目收费{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
        /// </summary>
        public bool IsCanSelectItemAndFee
        {
            get
            {
                return this.isCanSelectItemAndFee;
            }
            set
            {
                this.isCanSelectItemAndFee = value;

                this.SetIsCanSelectItemAndFee();
            }
        }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束

        /// <summary>
        /// 是否退费调用
        /// </summary>
        public bool IsQuitFee
        {
            get
            {
                return this.isQuitFee;
            }
            set
            {
                this.isQuitFee = value;
            }
        }

        /// <summary>
        /// 加载类别
        /// </summary>
        public Neusoft.HISFC.Models.Base.ItemKind ItemKind
        {
            get
            {
                return this.itemKind;
            }
            set
            {
                this.itemKind = value;

            }
        }

        /// <summary>
        /// 当前控件是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.IsInputValid();
            }
            set
            {
                this.isValid = value;
            }
        }

        /// <summary>
        /// 右侧信息显示列表
        /// </summary>
        public Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientOtherInfomationRight RightControl
        {
            get
            {
                return this.rightControl;
            }
            set
            {
                this.rightControl = value;
            }
        }

        /// <summary>
        /// 左侧信息显示列表
        /// </summary>
        public Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientOtherInfomationLeft LeftControl
        {
            get
            {
                return this.leftControl;
            }
            set
            {
                this.leftControl = value;
            }
        }

        /// <summary>
        /// 当前收费序列
        /// </summary>
        public string RecipeSequence
        {
            get
            {
                return this.recipeSeq;
            }
            set
            {
                this.recipeSeq = value;
            }
        }

        /// <summary>
        /// 是否获得焦点
        /// </summary>
        public bool IsFocus
        {
            get
            {
                return this.isFocus;
            }
            set
            {
                this.isFocus = value;
            }
        }

        /// <summary>
        /// 价格警戒线颜色
        /// </summary>
        public int PriceWarinningColor
        {
            get
            {
                return this.priceWarinningColor;
            }
            set
            {
                this.priceWarinningColor = value;
            }
        }


        /// <summary>
        /// 价格警戒线
        /// </summary>
        public decimal PriceWarnning
        {
            get
            {
                return this.priceWarnning;
            }
            set
            {
                this.priceWarnning = value;
            }
        }

        /// <summary>
        /// 每次用量是否可以为空
        /// </summary>
        public bool IsDoseOnceNull
        {
            get
            {
                return this.isDoseOnceNull;
            }
            set
            {
                this.isDoseOnceNull = value;
            }
        }

        /// <summary>
        /// 总量是否上取整
        /// </summary>
        public bool IsQtyToCeiling
        {
            get
            {
                return this.isQtyToCeiling;
            }
            set
            {
                this.isQtyToCeiling = value;
            }
        }

        /// <summary>
        /// 是否可以增加项目;
        /// </summary>
        public bool IsCanAddItem
        {
            get
            {
                return this.isCanAddItem;
            }
            set
            {
                this.isCanAddItem = value;
            }
        }

        /// <summary>
        /// 显示缺药药品
        /// </summary>
        public bool IsDisplayLackPha
        {
            get
            {
                return this.displayLackPha;
            }
            set
            {
                this.displayLackPha = value;
            }
        }

        /// <summary>
        /// 患者基本信息
        /// </summary>
        public Register PatientInfo
        {
            get
            {
                return this.rInfo;
            }
            set
            {
                this.rInfo = value;
            }
        }

        /// <summary>
        /// 临时挂号科室
        /// </summary>
        public string RegisterDept
        {
            get
            {
                return this.tempDept;
            }
            set
            {
                this.tempDept = value;
            }
        }
        /// <summary>
        /// 没有挂号患者,卡号第一位标志,默认以9开头
        /// </summary>

        public string NoRegFlagChar
        {
            get
            {
                return this.noRegFlagChar;
            }
            set
            {
                this.noRegFlagChar = value;
            }
        }

        /// <summary>
        /// 临时挂号费费用编码
        /// </summary>
        public string RegFeeItemCode
        {
            get
            {
                return this.regFeeItemCode;
            }
            set
            {
                this.regFeeItemCode = value;
            }
        }

        /// <summary>
        /// 自费诊查费项目编码
        /// </summary>
        public string OwnDiagFeeCode
        {
            get
            {
                return this.ownDiagFeeCode;
            }
            set
            {
                this.ownDiagFeeCode = value;
            }
        }

        /// <summary>
        /// 通用挂号级别
        /// </summary>
        public string ComRegLevel
        {
            get
            {
                return this.comRegLevel;
            }
            set
            {
                this.comRegLevel = value;
            }
        }

        /// <summary>
        /// 默认的收费包装单位
        /// </summary>
        public string DefaultPriceUnit
        {
            get
            {
                return this.defaultPriceUnit;
            }
            set
            {
                this.defaultPriceUnit = value;
            }
        }

        /// <summary>
        /// 频次显示形式
        /// </summary>
        public string FreqDisplayType
        {
            get
            {
                return this.freqDisplayType;
            }
            set
            {
                this.freqDisplayType = value;
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrText
        {
            get
            {
                return this.errText;
            }
            set
            {
                this.errText = value;
            }
        }

        /// <summary>
        /// 医保代码，该合同单位的对照信息中的医保目录等级用于公费时项目的费用项目显示甲乙类
        /// 例如广州医保：YBPactCode = 2
        /// </summary>
        public string YBPactCode
        {
            get
            {
                return this.ybPactCode;
            }
            set
            {
                this.ybPactCode = value;
            }
        }

        /// <summary>
        /// 自费病人显示医保标记
        /// </summary>
        public bool IsOwnDisplayYB
        {
            get
            {
                return this.isOwnDisplayYB;
            }
            set
            {
                this.isOwnDisplayYB = value;
            }
        }

        /// <summary>
        /// 是否可以更改划价信息
        /// </summary>
        public bool IsCanModifyCharge
        {
            get
            {
                return this.isCanModifyCharge;
            }
            set
            {
                this.isCanModifyCharge = value;
            }
        }

        /// <summary>
        /// 划价信息
        /// </summary>
        public ArrayList ChargeInfoList
        {
            get
            {
                return this.alChargeInfo;
            }
            set
            {
                this.alChargeInfo = value;

                if (value == null)
                {
                    return;
                }

                //屏蔽该事件,避免取划价信息时,进行多余的费用计算
                this.isDealCellChange = false;
                //显示划价信息.
                this.SetChargeInfo();
                //打开该事件
                this.isDealCellChange = true;
            }
        }

        /// <summary>
        /// 是否判断库存
        /// </summary>
        public bool IsJudgeStore
        {
            get
            {
                return this.isJudgeStore;
            }
            set
            {
                this.isJudgeStore = value;
            }
        }

        /// <summary>
        /// 门诊收费员是否可以删除医生的医嘱
        /// </summary>
        public bool IsCanDeleteDoctOrder
        {
            get
            {
                return this.isCanDeleteDoctOrder;
            }
            set
            {
                this.isCanDeleteDoctOrder = value;
            }
        }

        /// <summary>
        /// 收费项目列表
        /// </summary>
        public DataSet FeeItem
        {
            get { return this.dsItem; }
            set { this.dsItem = value; }
        }

        /// <summary>
        /// 字典缓存
        /// </summary>
        private Dictionary<string, PactItemRate> dictionaryPactItemRate = new Dictionary<string, PactItemRate>();
        #endregion

        #region 枚举

        /// <summary>
        /// 列枚举{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
        /// </summary>
        private enum Columns
        {
            /// <summary>
            /// 选择
            /// </summary>
            Select = 0,

            /// <summary>
            /// 输入码
            /// </summary>
            InputCode = 1,

            /// <summary>
            /// 名称
            /// </summary>
            ItemName = 2,

            /// <summary>
            /// 显示组合
            /// </summary>
            CombNoDisplay = 4,

            /// <summary>
            /// 数量
            /// </summary>
            Amount = 6,

            /// <summary>
            /// 单位
            /// </summary>
            PriceUnit = 7,

            /// <summary>
            /// 付数
            /// </summary>
            Days = 8,

            /// <summary>
            /// 每次用量
            /// </summary>
            DoseOnce = 9,

            /// <summary>
            /// 用量单位
            /// </summary>
            DoseUnit = 10,

            /// <summary>
            /// 组合号
            /// </summary>
            CombNo = 11,

            /// <summary>
            /// 频次
            /// </summary>
            Freq = 12,

            /// <summary>
            /// 用法
            /// </summary>
            Usage = 13,

            ///// <summary>
            ///// 执行科室
            ///// </summary>
            //ExeDept = 13,
            /// <summary>
            /// 金额
            /// </summary>
            Cost = 14,

            /// <summary>
            /// 自付比例
            /// </summary>
            PayRate = 15,

            /// <summary>
            /// 医保类型
            /// </summary>
            SIPactType = 16,

            /// <summary>
            /// 公费类型
            /// </summary>
            GFPactType = 17,

            /// <summary>
            /// 设备编号
            /// </summary>
            MachineNO = 18,

            ///// <summary>
            ///// 金额
            ///// </summary>
            //Cost = 18,
            /// <summary>
            /// 执行科室
            /// </summary>
            ExeDept = 19,

            /// <summary>
            /// 自费药
            /// </summary>
            Self = 20,

            /// <summary>
            /// 小计
            /// </summary>
            LittleCost = 21,

            /// <summary>
            /// 单价
            /// </summary>
            Price = 5,

            /// <summary>
            /// 备注
            /// </summary>
            Memo = 22,

            /// <summary>
            /// 最小费用
            /// </summary>
            FeeCode = 23,

            /// <summary>
            /// 项目类别
            /// </summary>
            ItemType = 24,

            /// <summary>
            /// 项目编码
            /// </summary>
            ItemCode = 25,

            /// <summary>
            /// 是否更改
            /// </summary>
            Change = 26,

            /// <summary>
            /// 是否发送申请
            /// </summary>
            IsSend = 3
        }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束

        #endregion

        #region 方法

        #region 私有方法

        /// <summary>
        /// 初始化控制参数
        /// </summary>
        /// <returns>成功 1 失败-1</returns>
        protected int InitControlParams()
        {
            //价格警戒线颜色
            this.priceWarinningColor = this.controlParamIntegrate.GetControlParam<int>(Neusoft.HISFC.BizProcess.Integrate.Const.TOP_PRICE_WARNNING_COLOR, true, Color.Red.ToArgb());

            //价格警戒线
            this.priceWarnning = this.controlParamIntegrate.GetControlParam<decimal>(Neusoft.HISFC.BizProcess.Integrate.Const.TOP_PRICE_WARNNING, true, 1000000);

            //每次用量是否可以为空
            this.isDoseOnceNull = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.DOSE_ONCE_NULL, true, true);

            //总量是否上取整
            this.isQtyToCeiling = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.QTY_TO_CEILING, true, false);

            //显示缺药药品
            this.displayLackPha = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.DISPLAY_LACK_PHAMARCY, true, false);

            //没有挂号患者,卡号第一位标志,默认以9开头
            this.noRegFlagChar = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.NO_REG_CARD_RULES, true, "9");

            //临时挂号费费用编码
            this.regFeeItemCode = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.AUTO_REG_FEE_ITEM_CODE, true, string.Empty);

            //自费诊查费项目编码
            this.ownDiagFeeCode = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.AUTO_PUB_FEE_DIAG_FEE_CODE, true, string.Empty);

            //通用挂号级别
            this.comRegLevel = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.COM_REG_LEVEL, true, string.Empty);

            //默认的收费包装单位
            this.defaultPriceUnit = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.PRICEUNIT, true, "0");

            //频次显示形式
            this.freqDisplayType = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.FREQ_DISPLAY_TYPE, true, "0");

            //自费病人显示医保标记
            this.isOwnDisplayYB = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.OWN_DISPLAY_YB, true, false);

            //是否可以更改划价信息
            this.isCanModifyCharge = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.MODIFY_CHARGE_INFO, true, false);

            //是否判断库存
            this.isJudgeStore = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.JUDGE_STORE, true, false);

            //是否判断预扣库存
            this.isUsePreStore = this.controlParamIntegrate.GetControlParam<bool>("P00320", false, false);

            // 是否收费员录入的界面，可以不录入频次，用法等
            this.isDoseOnceCanNull = controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.DOSE_ONCE_NULL, false, true);

            //收费组套获取药品最小单位  先上线，后考虑
            this.isShowMinPact = controlParamIntegrate.GetControlParam<bool>("MZFY01", false, true);

            //门诊收费员是否可以删除医生的医嘱
            this.isCanDeleteDoctOrder = controlParamIntegrate.GetControlParam<bool>("MZ0911", false, true);

            //手工方输入项目是否不需要判断患者信息
            this.isInputItemsNoSpe = this.controlParamIntegrate.GetControlParam<bool>("MZ9930", false, true);

            return 1;
        }

        /// <summary>
        /// 初始化合同单位公费待遇
        /// </summary>
        /// <returns></returns>
        private int InitBillPact()
        {
            try
            {
                ArrayList al = this.managerIntegrate.GetConstantList("BILLPACT");
                this.alBillPact = al;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载合同单位公费待遇出错!" + ex.Message, "提示");
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 初始化设备信息
        /// </summary>
        /// <returns></returns>
        private int InitMachine()
        {
            ArrayList al = this.managerIntegrate.GetConstantList("MachineNO");//设备号
            if (alFreq == null)
            {
                MessageBox.Show("获得设备列表出错!" + this.managerIntegrate.Err);
                return -1;
            }
            myMachineNO.ArrayObject = al;

            this.lbMachineNO.AddItems(al);
            Controls.Add(lbMachineNO);
            lbMachineNO.Hide();
            lbMachineNO.BorderStyle = BorderStyle.FixedSingle;
            lbMachineNO.BringToFront();

            lbMachineNO.SelectItem += new Neusoft.FrameWork.WinForms.Controls.PopUpListBox.MyDelegate(lbMachineNO_SelectItem);

            return 1;
        }

        /// <summary>
        /// 获得发票信息
        /// </summary>
        /// <returns>0 成功 -1 失败</returns>
        private int GetInvoiceClass()
        {
            int iReturn = this.outpatientManager.GetInvoiceClass("MZ01", ref dsInvoice);

            if (iReturn != -1)
            {
                dsInvoice.Tables[0].PrimaryKey = new DataColumn[] { dsInvoice.Tables[0].Columns["FEE_CODE"] };
            }

            return iReturn;
        }

        /// <summary>
        /// 重新计算所有小计金额
        /// 当重新插入一条，删除一条，或者变更项目后
        /// </summary>
        private void SumLittleCostAll()
        {
            decimal littleCost = 0;
            int itemqty = 0;
            string tempName = string.Empty;
            string tempCod = string.Empty;
            decimal pricesz = 0;
            decimal pricece = 0;
            this.undrugManager.GetPricesz("F00000010769", ref pricesz);//获取加收项目价格
            this.undrugManager.GetPricesz("F00000010768", ref pricece);//获取加收项目价格
            pricece = pricece - pricesz;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                tempName = this.fpSpread1_Sheet1.Cells[i, (int)Columns.ItemName].Text;
                tempCod = this.fpSpread1_Sheet1.Cells[i, (int)Columns.ItemCode].Text;
                if (tempName == "小计")
                {
                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Text = littleCost.ToString();
                    littleCost = 0;
                }
                else
                {
                    if (this.undrugManager.SetUltrasound(tempCod))
                    {

                        if (itemqty > 0)
                        {
                            littleCost += NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Text) - pricece;  //减去价格差额
                        }
                        else
                        {
                            littleCost += NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Text);
                        }
                        itemqty = itemqty + 1;
                    }
                    else
                    {
                        littleCost += NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Text);
                    }
                }
            }
        }

        /// <summary>
        /// 查询项目，全部模糊查询 包括拼音，五笔，自定义，项目名称，别名，别名拼音，五笔，自定义
        /// </summary>
        /// <param name="inputCode">输入的编码</param>
        /// <param name="row">当前行</param>
        private void QueryItem(string inputCode, int row)
        {
            ClearRow(row);
            SumCost();
            string sFilter = string.Empty;

            this.chooseItemControl.IsSelectItem = false;

            switch (this.chooseItemControl.QueryType)
            {
                case "0":
                    sFilter = "SPELL_CODE like '%" + inputCode + "'" +
                    " OR " + "WB_CODE like '%" + inputCode + "'" +
                    " OR " + "User_Code like '%" + inputCode.PadLeft(6, '0') + "'" +
                    " OR " + "ITEM_NAME like '%" + inputCode + "'" +
                    " OR " + "CUS_SPELL_CODE like '%" + inputCode + "'" +
                    " OR " + "CUS_WB_CODE like '%" + inputCode + "'" +
                    " OR " + "CUS_User_Code like '%" + inputCode + "'" +
                    " OR " + "CUS_NAME like '%" + inputCode + "'" +
                    " OR " + "OTHER_NAME like '%" + inputCode + "'" +
                    " OR " + "OTHER_SPELL like '%" + inputCode + "'" +
                    " OR " + "EN_NAME like '%" + inputCode + "'";
                    break;
                case "1":
                    sFilter = "SPELL_CODE like '" + inputCode + "%'" +
                    " OR " + "WB_CODE like '" + inputCode + "%'" +
                    " OR " + "User_Code like '" + inputCode.PadLeft(6, '0') + "%'" +
                    " OR " + "ITEM_NAME like '" + inputCode + "%'" +
                    " OR " + "CUS_SPELL_CODE like '" + inputCode + "%'" +
                    " OR " + "CUS_WB_CODE like '" + inputCode + "%'" +
                    " OR " + "CUS_User_Code like '" + inputCode + "%'" +
                    " OR " + "CUS_NAME like '" + inputCode + "%'" +
                    " OR " + "OTHER_NAME like '" + inputCode + "%'" +
                    " OR " + "OTHER_SPELL like '" + inputCode + "%'" +
                    " OR " + "EN_NAME like '" + inputCode + "%'";
                    break;
                case "2":
                    sFilter = "SPELL_CODE like '%" + inputCode + "%'" +
                    " OR " + "WB_CODE like '%" + inputCode + "%'" +
                    " OR " + "User_Code like '%" + inputCode.PadLeft(6, '0') + "%'" +
                    " OR " + "ITEM_NAME like '%" + inputCode + "%'" +
                    " OR " + "CUS_SPELL_CODE like '%" + inputCode + "%'" +
                    " OR " + "CUS_WB_CODE like '%" + inputCode + "%'" +
                    " OR " + "CUS_User_Code like '%" + inputCode + "%'" +
                    " OR " + "CUS_NAME like '%" + inputCode + "%'" +
                    " OR " + "OTHER_NAME like '%" + inputCode + "%'" +
                    " OR " + "OTHER_SPELL like '%" + inputCode + "%'" +
                    " OR " + "EN_NAME like '%" + inputCode + "%'";
                    break;
                case "3":
                    sFilter = "SPELL_CODE like '" + inputCode + "'" +
                    " OR " + "WB_CODE like '" + inputCode + "'" +
                    " OR " + "User_Code like '" + inputCode.PadLeft(6, '0') + "'" +
                    " OR " + "ITEM_NAME like '" + inputCode + "'" +
                    " OR " + "CUS_SPELL_CODE like '" + inputCode + "'" +
                    " OR " + "CUS_WB_CODE like '" + inputCode + "'" +
                    " OR " + "CUS_User_Code like '" + inputCode + "'" +
                    " OR " + "CUS_NAME like '" + inputCode + "'" +
                    " OR " + "OTHER_NAME like '" + inputCode + "'" +
                    " OR " + "OTHER_SPELL like '" + inputCode + "'" +
                    " OR " + "EN_NAME like '" + inputCode + "'";
                    break;
                default:
                    sFilter = "SPELL_CODE like '" + inputCode + "%'" +
                    " OR " + "WB_CODE like '" + inputCode + "%'" +
                    " OR " + "User_Code like '" + inputCode.PadLeft(6, '0') + "%'" +
                    " OR " + "ITEM_NAME like '" + inputCode + "%'" +
                    " OR " + "CUS_SPELL_CODE like '" + inputCode + "%'" +
                    " OR " + "CUS_WB_CODE like '" + inputCode + "%'" +
                    " OR " + "CUS_User_Code like '" + inputCode + "%'" +
                    " OR " + "CUS_NAME like '" + inputCode + "%'" +
                    " OR " + "OTHER_NAME like '" + inputCode + "%'" +
                    " OR " + "OTHER_SPELL like '" + inputCode + "%'" +
                    " OR " + "EN_NAME like '" + inputCode + "%'";
                    break;
            }
            //如果输入的编码为空，清空当前行
            if (inputCode == string.Empty)
            {
                ClearRow(row);
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.SetActiveCell(row, (int)Columns.InputCode, false);
                return;
            }
            else//过滤项目
            {

                sFilter = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(sFilter, new string[] { "[", "]", "#", "@", "^", "&", "$", "*" });
                this.chooseItemControl.SetInputChar(this.fpSpread1, inputCode, Neusoft.HISFC.Models.Base.InputTypes.Spell);
                dvItem.RowFilter = sFilter;
                ///this.chooseItemControl.i.ucItem.InitPrev();
                //if (this.chooseItemControl.InputPrev.Length <= 0)
                //{
                //    dvItem.Sort = "DRUG_FLAG DESC";
                //}
                //else
                //{
                //    dvItem.Sort = "DRUG_FLAG DESC," + this.chooseItemControl.InputPrev;
                //}
                //选择控件当选择一条项目后触发
                //如果只有一行，控件不显示，直接填写项目信息

                //选择项目控件接收过滤后的项目信息
                this.chooseItemControl.DeptCode = myOperator.Dept.ID;
                this.chooseItemControl.ObjectFilterObject = this.fpSheetItem;

                if (this.chooseItemControl.IsSelectItem == false)
                {
                    this.fpSpread1.Select();
                    this.fpSpread1.Focus();
                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.SetActiveCell(row, (int)Columns.InputCode, false);
                    this.SumCost();
                }
                if (this.fpSheetItem.RowCount > 1)
                {
                    ((Form)this.chooseItemControl).ShowDialog();
                }

                if (this.chooseItemControl.IsSelectItem == false)
                {
                    this.fpSpread1.Select();
                    this.fpSpread1.Focus();
                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.SetValue(row, (int)Columns.InputCode, inputCode);
                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.SetActiveCell(row, (int)Columns.InputCode, false);

                    if (this.fpSpread1.EditingControl != null)
                    {
                        this.fpSpread1.EditingControl.Select();
                    }


                }
            }
        }

        /// <summary>
        /// 获得项目列表
        /// </summary>
        /// <param name="deptCode">收费科室代码</param>
        /// <returns> -1 失败 >=0 成功</returns>
        private int LoadItem(string deptCode)
        {
            int iReturn = 0;

            //获得收费员所在科室的维护药房中的药品，非药品和组合项目，组套全部获得
            // iReturn = this.outpatientManager.QueryItemList(deptCode, ref dsItem);

            dsItem = new DataSet();

            iReturn = this.outpatientManager.QueryItemList(deptCode, this.itemKind, ref dsItem);
            if (iReturn == -1)
            {
                MessageBox.Show("获得项目列表出错!" + this.outpatientManager.Err);

                return -1;
            }

            //根据参数决定是否加载缺药药品
            if (this.displayLackPha)
            {
                DataSet dsItemSupply = new DataSet();
                ////iReturn = this.outpatientManager.GetItemListSupply(deptCode, ref dsItemSupply);
                if (iReturn == -1)
                {
                    MessageBox.Show("获得项目列表(缺药部分)出错!");
                    return -1;
                }

                dsItem.Merge(dsItemSupply);
            }
            try
            {
                //设置项目列表的主键为项目编码（药品，非药品编码，组套项目的package_code)
                //dsItem.Tables[0].PrimaryKey = new DataColumn[] { dsItem.Tables[0].Columns["ITEM_CODE"], dsItem.Tables[0].Columns["EXE_DEPT"] };
                //dsItem.Tables[0].Clear();
                //dsItem.Tables[0].PrimaryKey = null;
                dvItem = new DataView(dsItem.Tables[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return -1;
            }

            return iReturn;
        }

        /// <summary>
        /// 处理频次信息
        /// </summary>
        /// <returns></returns>
        private int ProcessFreq()
        {
            if (this.lbFreq.Visible == false)
            {
                return -1;
            }
            int CurrentRow = this.fpSpread1_Sheet1.ActiveRowIndex;
            if (CurrentRow < 0)
            {
                return 0;
            }
            fpSpread1.StopCellEditing();
            string IsDeptChange = fpSpread1_Sheet1.GetText(CurrentRow, (int)Columns.Change);
            if ((IsDeptChange == "0" || IsDeptChange == string.Empty) && fpSpread1_Sheet1.GetText(CurrentRow, (int)Columns.Freq) == string.Empty)
            {
                MessageBox.Show(Language.Msg("频次不能为空,请输入!"), Language.Msg("提示"));
                fpSpread1.Focus();
                fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.Freq, true);

                return -1;
            }

            NeuObject item1 = null;
            Neusoft.HISFC.Models.Order.Frequency item = null;
            int rtn = lbFreq.GetSelectedItem(out item1);
            //{565BF156-98AB-41ae-B657-93BC408FF641}
            if (item1 == null || string.IsNullOrEmpty(item1.ID))
            {
                return 0;
            }//{565BF156-98AB-41ae-B657-93BC408FF641}完毕

            item = (Neusoft.HISFC.Models.Order.Frequency)item1;
            if (rtn == -1)
            {
                return -1;
            }
            if (item == null)
            {
                return -1;
            }

            if (freqDisplayType == "0")//汉字
            {
                if (item.UserCode != null && item.UserCode.Length > 0)
                {
                    fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Freq, item.User03);
                }
                else
                {
                    fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Freq, item.Name);
                }
            }
            else //编码
            {
                if (item.UserCode != null && item.UserCode.Length > 0)
                {
                    fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Freq, item.Name);
                }
                else
                {
                    fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Freq, item.ID);
                }
            }

            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Change, "0");
            if (item.UserCode != null && item.UserCode.Length > 0)
            {
                ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Frequency.ID = item.Name;
                ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Frequency.Name = item.User03;
            }
            else
            {
                ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Frequency.ID = item.ID;
                ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Frequency.Name = item.Name;
            }
            lbFreq.Visible = false;
            this.fpSpread1_Sheet1.Cells[CurrentRow, (int)Columns.Usage].Locked = false;
            this.fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.Usage, false);

            return 1;
        }

        /// <summary>
        /// 用法回车事件
        /// </summary>
        /// <returns></returns>
        private int ProcessUsage()
        {
            if (this.lbUsage.Visible == false)
            {
                return -1;
            }
            int CurrentRow = this.fpSpread1_Sheet1.ActiveRowIndex;
            if (CurrentRow < 0)
            {
                return 0;
            }
            fpSpread1.StopCellEditing();
            string IsDeptChange = fpSpread1_Sheet1.GetText(CurrentRow, (int)Columns.Change);
            if ((IsDeptChange == "0" || IsDeptChange == string.Empty) && fpSpread1_Sheet1.GetText(CurrentRow, (int)Columns.Usage) == string.Empty)
            {
                MessageBox.Show(Language.Msg("用法不能为空,请输入!"), Language.Msg("提示"));
                fpSpread1.Focus();
                fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.Usage, true);
                return -1;
            }

            NeuObject item = null;
            int rtn = lbUsage.GetSelectedItem(out item);
            if (item != null)
            {
                string usageCode = item.ID;

                NeuObject obj = this.managerIntegrate.GetConstansObj("MZUSAGECODE", usageCode);

                if (obj != null && obj.Name != string.Empty)
                {
                    try
                    {
                        this.fpSpread1_Sheet1.RowHeader.Cells[CurrentRow, 0].BackColor = Color.FromArgb(NConvert.ToInt32(obj.Name));
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        this.fpSpread1_Sheet1.RowHeader.Cells[CurrentRow, 0].BackColor = Color.FromArgb(-1250856);
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    this.fpSpread1_Sheet1.RowHeader.Cells[CurrentRow, 0].BackColor = Color.FromArgb(-1250856);
                }
                catch { }
            }
            if (rtn == -1)
            {
                return -1;
            }
            if (item == null)
            {
                return -1;
            }

            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Usage, item.Name);
            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Change, "0");
            ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Usage.ID = item.ID;
            ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Usage.Name = item.Name;

            //if (((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Item.IsPharmacy)
            if (((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Item.ItemType == EnumItemType.Drug)
            {
                //去掉对用法的判断非空的判断 2007-08-24 luzhp@neusoft.com
                //if (this.fpSpread1_Sheet1.Cells[CurrentRow, (int)Columns.Usage].Text == string.Empty)
                //{
                //    MessageBox.Show(Language.Msg("请输入药品的用法!"));
                //    this.fpSpread1.Focus();
                //    this.fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.Usage);

                //    return -1;
                //}
                //else
                //{
                if (this.fpSpread1_Sheet1.Cells[CurrentRow, (int)Columns.Usage].Text != string.Empty)
                {
                    string usageCode = item.ID;

                    alInjec = this.outpatientManager.GetInjectInfoByUsage(usageCode);
                    if (alInjec == null)
                    {
                        MessageBox.Show("获得院注项目出错!" + this.outpatientManager.Err);

                        return -1;
                    }
                    if (alInjec.Count > 0)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(myInjec);
                    }
                }
            }

            lbUsage.Visible = false;

            this.fpSpread1_Sheet1.Cells[CurrentRow, (int)Columns.ExeDept].Locked = false;
            this.fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.ExeDept, false);

            return 0;
        }

        /// <summary>
        /// 执行科室的回车
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int ProcessDept()
        {
            if (lbDept.Visible == false)
            {
                return 1;
            }
            int CurrentRow = this.fpSpread1_Sheet1.ActiveRowIndex;
            if (CurrentRow < 0)
            {
                return 1;
            }
            fpSpread1.StopCellEditing();
            string IsDeptChange = fpSpread1_Sheet1.GetText(CurrentRow, (int)Columns.Change);
            if ((IsDeptChange == "0" || IsDeptChange == string.Empty) && fpSpread1_Sheet1.GetText(CurrentRow, (int)Columns.ExeDept) == string.Empty)
            {
                MessageBox.Show(Language.Msg("执行科室不能为空,请输入!"));
                fpSpread1.Focus();
                fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.ExeDept, true);

                return -1;
            }

            NeuObject item = null;
            int rtn = lbDept.GetSelectedItem(out item);
            if (rtn == -1)
            {
                MessageBox.Show(Language.Msg("输入的编码不正确,请重新输入"));

                return -1;
            }
            if (item == null)
            {
                MessageBox.Show(Language.Msg("输入的编码不正确,请重新输入"));

                return -1;
            }


            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.ExeDept, item.Name);
            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Change, "0");
            ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).ExecOper.Dept.ID = item.ID;
            ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).ExecOper.Dept.Name = item.Name;

            lbDept.Visible = false;
            //fpSpread1.StopCellEditing();
            if (isJudgeStore)
            {
                FeeItemList f = this.fpSpread1_Sheet1.Rows[CurrentRow].Tag as FeeItemList;
                //if (f.Item.IsPharmacy)
                if (f.Item.ItemType == EnumItemType.Drug)
                {
                    if (!IsStoreEnough(f, CurrentRow))
                    {
                        //f.ExecOper.Dept.ID = string.Empty;
                        //f.ExecOper.Dept.Name = string.Empty;
                        this.fpSpread1.Focus();
                        this.fpSpread1_Sheet1.SetActiveCell(CurrentRow, (int)Columns.Amount);
                        return -1;
                    };
                }
            }
            this.AddRow(CurrentRow);

            return 1;
        }

        /// <summary>
        /// 执行科室的回车
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int ProcessMachineNO()
        {
            if (lbMachineNO.Visible == false)
            {
                return 1;
            }
            int CurrentRow = this.fpSpread1_Sheet1.ActiveRowIndex;
            if (CurrentRow < 0)
            {
                return 1;
            }
            fpSpread1.StopCellEditing();

            NeuObject item = null;
            int rtn = lbMachineNO.GetSelectedItem(out item);
            if (rtn == -1)
            {
                MessageBox.Show(Language.Msg("输入的编码不正确,请重新输入"));

                return -1;
            }
            if (item == null)
            {
                MessageBox.Show(Language.Msg("输入的编码不正确,请重新输入"));

                return -1;
            }


            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.MachineNO, item.Name);
            fpSpread1_Sheet1.SetValue(CurrentRow, (int)Columns.Change, "0");
            ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Sample.ID = item.ID;
            ((FeeItemList)this.fpSpread1_Sheet1.Rows[CurrentRow].Tag).Order.Sample.Name = item.Name;

            lbMachineNO.Visible = false;
            this.AddRow(CurrentRow);

            return 1;
        }

        /// <summary>
        /// 初始化频次信息
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitFreq()
        {
            ArrayList alTemp = new ArrayList();

            alFreq = this.managerIntegrate.QuereyFrequencyList();

            if (alFreq == null)
            {
                MessageBox.Show("获得频次列表出错!" + this.managerIntegrate.Err);

                return -1;
            }

            foreach (Neusoft.HISFC.Models.Order.Frequency f in alFreq)
            {
                Neusoft.HISFC.Models.Order.Frequency temFre = f.Clone();
                string temp = string.Empty;
                if (f.UserCode != null && f.UserCode.Length > 0)
                {
                    temp = temFre.UserCode;
                    temFre.User03 = temFre.Name;
                    temFre.Name = temFre.ID;
                    temFre.ID = temp;
                }
                alTemp.Add(temFre);
            }

            lbFreq.AddItems(alTemp);
            Controls.Add(lbFreq);
            lbFreq.Hide();
            lbFreq.BorderStyle = BorderStyle.FixedSingle;
            lbFreq.BringToFront();
            lbFreq.Width = 80;

            lbFreq.SelectItem += new Neusoft.FrameWork.WinForms.Controls.PopUpListBox.MyDelegate(lbFreq_SelectItem);

            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int lbFreq_SelectItem(Keys key)
        {
            ProcessFreq();
            fpSpread1.Focus();
            fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.Freq, true);
            return 0;
        }

        /// <summary>
        /// 初始化执行科室列表
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitDept()
        {
            alDept = this.managerIntegrate.GetDepartment();
            if (alDept == null)
            {
                MessageBox.Show("获得科室列表出错!" + this.managerIntegrate.Err);

                return -1;
            }
            lbDept.AddItems(alDept);
            Controls.Add(lbDept);
            lbDept.Hide();
            lbDept.BorderStyle = BorderStyle.FixedSingle;
            lbDept.BringToFront();

            lbDept.SelectItem += new Neusoft.FrameWork.WinForms.Controls.PopUpListBox.MyDelegate(lbDept_SelectItem);

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int lbDept_SelectItem(Keys key)
        {
            ProcessDept();
            fpSpread1.Focus();
            fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.ExeDept, true);
            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int lbMachineNO_SelectItem(Keys key)
        {
            this.ProcessMachineNO();
            fpSpread1.Focus();
            fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.MachineNO, true);
            return 0;
        }
        /// <summary>
        /// 初始化用法列表
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitUsage()
        {
            alUsage = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.USAGE);
            if (alUsage == null)
            {
                MessageBox.Show("加载用法列表出错!" + this.managerIntegrate.Err);

                return -1;
            }
            lbUsage.AddItems(alUsage);
            Controls.Add(lbUsage);
            lbUsage.Hide();
            lbUsage.BorderStyle = BorderStyle.FixedSingle;
            lbUsage.BringToFront();
            lbUsage.Width = 90;

            lbUsage.SelectItem += new Neusoft.FrameWork.WinForms.Controls.PopUpListBox.MyDelegate(lbUsage_SelectItem);

            return 1;
        }

        int lbUsage_SelectItem(Keys key)
        {
            ProcessUsage();
            fpSpread1.Focus();
            fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.Usage, true);
            return 0;
        }

        /// <summary>
        /// 设置执行科室位置
        /// </summary>
        private void SetLocation()
        {
            if (this.fpSpread1_Sheet1.ActiveColumnIndex == (int)Columns.ExeDept)
            {
                Control cell = this.fpSpread1.EditingControl;
                lbDept.Location = new Point(this.fpSpread1.Location.X + cell.Location.X,
                    this.fpSpread1.Location.Y + cell.Location.Y + cell.Height + SystemInformation.Border3DSize.Height * 2);
                lbDept.Size = new Size(cell.Width + 50 + SystemInformation.Border3DSize.Width * 2, 150);
            }

            if (this.fpSpread1_Sheet1.ActiveColumnIndex == (int)Columns.MachineNO)
            {
                Control cell = this.fpSpread1.EditingControl;
                this.lbMachineNO.Location = new Point(this.fpSpread1.Location.X + cell.Location.X,
                    this.fpSpread1.Location.Y + cell.Location.Y + cell.Height + SystemInformation.Border3DSize.Height * 2);
                lbMachineNO.Size = new Size(cell.Width + 50 + SystemInformation.Border3DSize.Width * 2, 150);
            }

            if (this.fpSpread1_Sheet1.ActiveColumnIndex == (int)Columns.Freq)
            {
                Control cell = this.fpSpread1.EditingControl;
                lbFreq.Location = new Point(this.fpSpread1.Location.X + cell.Location.X,
                    this.fpSpread1.Location.Y + cell.Location.Y + cell.Height + SystemInformation.Border3DSize.Height * 2);
                lbFreq.Size = new Size(cell.Width + 50 + SystemInformation.Border3DSize.Width * 2, 150);
            }
            if (this.fpSpread1_Sheet1.ActiveColumnIndex == (int)Columns.Usage)
            {
                Control cell = this.fpSpread1.EditingControl;
                lbUsage.Location = new Point(this.fpSpread1.Location.X + cell.Location.X,
                    this.fpSpread1.Location.Y + cell.Location.Y + cell.Height + SystemInformation.Border3DSize.Height * 2);
                lbUsage.Size = new Size(cell.Width + 50 + SystemInformation.Border3DSize.Width * 2, 150);
            }
        }

        /// <summary>
        /// 上一行
        /// </summary>
        /// <param name="key">当前的按键</param>
        private void PutArrow(Keys key)
        {
            int currCol = this.fpSpread1_Sheet1.ActiveColumnIndex;
            int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;

            if (key == Keys.Right)
            {
                for (int i = 0; i < this.fpSpread1_Sheet1.Columns.Count; i++)
                {
                    if (i > currCol && this.fpSpread1_Sheet1.Cells[currRow, i].Locked == false)
                    {
                        this.fpSpread1_Sheet1.SetActiveCell(currRow, i, false);

                        return;
                    }
                }
            }
            if (key == Keys.Left)
            {
                for (int i = this.fpSpread1_Sheet1.Columns.Count - 1; i >= 0; i--)
                {
                    if (i < currCol && this.fpSpread1_Sheet1.Cells[currRow, i].Locked == false)
                    {
                        this.fpSpread1_Sheet1.SetActiveCell(currRow, i, false);

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化farpoint,屏蔽一些热键
        /// </summary>
        private void InitFp()
        {
            InputMap im;
            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Down, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Up, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Escape, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.F3, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

            im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.F4, Keys.None), FarPoint.Win.Spread.SpreadActions.None);


            // 设置
            if (System.IO.File.Exists(filePath))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(fpSpread1_Sheet1, this.filePath);
            }
            else
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpSpread1_Sheet1, filePath);
            }
        }


        /// <summary>
        /// 显示项目类别
        /// </summary>
        /// <param name="row">当前行</param>
        /// <param name="c">颜色</param>
        /// <param name="text">显示文字</param>
        /// <param name="f">当前费用项目</param>
        private void SetItemDisplay(int row, Color c, string text, Font f)
        {
            this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].Text = text;
            this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].Font = f;
            this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].ForeColor = c;
        }

        /// <summary>
        /// 显示患者的划价信息
        /// </summary>
        private void SetChargeInfo()
        {
            this.Clear();
            ArrayList hsNOREOnlyOneItem = new ArrayList();
            ArrayList hsZTNOREOnlyOneItem = new ArrayList();
            Hashtable hsREOnlyOneItem = new Hashtable();
            int rowCount = this.fpSpread1_Sheet1.RowCount;
            int currRow = 0;
            if (this.fpSpread1_Sheet1.RowCount == 0)
            {
                this.fpSpread1_Sheet1.Rows.Add(0, 1);
                currRow = 0;
            }
            ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            if (this.fpSpread1_Sheet1.Cells[rowCount - 1, (int)Columns.InputCode].Text == string.Empty)
            {
                currRow = rowCount - 1;
            }
            else
            {
                this.fpSpread1_Sheet1.Rows.Add(currRow, 1);
                currRow = rowCount;
            }

            string userCode = string.Empty;
            decimal totDisplayCost = 0;
            decimal price = 0;
            string minUnit = string.Empty;
            string packUnit = string.Empty;
            string specs = string.Empty;
            decimal pricesz = 0;
            decimal pricece = 0;
            this.undrugManager.GetPricesz("F00000010769", ref pricesz);//获取加收项目价格
            this.undrugManager.GetPricesz("F00000010768", ref pricece);//获取加收项目价格
            pricece = pricece - pricesz;
            sumPubCost = 0;
            int itemqty = 0;
            int returnRows = 0;//是否为限制收费药品
            decimal LimitNumber = 1;
            //限制药品收费
            int number = 1;
            ArrayList hsREOnlylistItem = new ArrayList();
            for (int i = alChargeInfo.Count - 1; i >= 0; i--)
            {
                string Discount_type = "1";//限制收费类型
                int TOPPRICE = 0;
                decimal DISCOUNT_RATE = 0;
                FeeItemList s = alChargeInfo[i] as FeeItemList;
                returnRows = this.undrugManager.SetRestrictingfee(s.Item.ID, ref  LimitNumber);
                Discount_type = this.undrugManager.SetDiscountfee(s.Item.ID, ref  DISCOUNT_RATE, ref  TOPPRICE);
                if (returnRows > 0 && this.rInfo.DoctorInfo.Templet.Dept.ID!="7021")
                {
                    this.setRestrictingfee.ConvertRestrictingfeeCharge(PatientInfo.PID.CardNO, s, ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem, number, LimitNumber, ref hsZTNOREOnlyOneItem, this.dsItem, this.rInfo);
                }
                if (Discount_type == "2")
                {
                    this.setRestrictingfee.ConvertDiscountfee(s, DISCOUNT_RATE, TOPPRICE, ref hsREOnlyOneItem, ref hsREOnlylistItem, number);
                }
                number++;
            }
            number = 1;
            for (int i = alChargeInfo.Count - 1; i >= 0; i--)
            {
                FeeItemList s = alChargeInfo[i] as FeeItemList;
                if (hsREOnlyOneItem.ContainsKey(s.Item.ID + number))
                {
                    alChargeInfo.RemoveAt(i);
                }
                number++;
            }
            foreach (FeeItemList ds in hsREOnlylistItem)
            {
                alChargeInfo.Add(ds);
            }

            foreach (FeeItemList f in alChargeInfo)
            {
                DataRow rowFind = null;
                string drugFlag = "0";

                //if (f.Item.IsPharmacy)
                if (f.Item.ItemType == EnumItemType.Drug)
                {
                    drugFlag = "1";
                }
                else if (f.Item.ID.Substring(0, 1) == "F")
                {
                    drugFlag = "0";
                }
                else
                {
                    drugFlag = "2";
                }

                string strExp = "ITEM_CODE = " + "'" + f.Item.ID + "'";// +" and DRUG_FLAG =" + "'" + drugFlag + "'";
                DataRow[] rowFinds = dsItem.Tables[0].Select(strExp);

                if (f.Item.ID != "999")
                {
                    if (rowFinds == null || rowFinds.Length == 0)
                    {
                        //根据处方号和项目流水号查询old_mo_order不为空/开立科室是管理中心
                        if (this.outpatientManager.IsFeeInfo(f.RecipeNO, f.SequenceNO.ToString()))
                        {
                            //重新赋值
                            DataSet dtItemSerch = new DataSet();
                            this.outpatientManager.QueryItemListForValid(myOperator.Dept.ID, f.Item.ID, ref dtItemSerch);
                            rowFinds = dtItemSerch.Tables[0].Select(strExp);
                        }
                        else
                        {
                            DialogResult dialogRes = MessageBox.Show("查找项目【" + f.Item.Name + "】失败！该项目已停用或库存为 0 ，是否继续？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                            if (dialogRes == DialogResult.Yes)
                            {
                                continue;
                            }
                            else
                            {
                                this.fpSpread1_Sheet1.RowCount = 0;
                                this.fpSpread1_Sheet1.RowCount = 1;
                                return;
                            }
                        }
                    }
                    rowFind = rowFinds[0];
                    if (rowFind == null)
                    {
                        DialogResult dialogRes = MessageBox.Show("查找项目【" + f.Item.Name + "】失败！该项目已停用或库存为 0 ，是否继续？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                        if (dialogRes == DialogResult.Yes)
                        {
                            continue;
                        }
                        else
                        {
                            this.fpSpread1_Sheet1.RowCount = 0;
                            this.fpSpread1_Sheet1.RowCount = 1;
                            return;
                        }
                    }

                    userCode = rowFind["User_Code"].ToString(); //自定义编码
                    try
                    {
                        DateTime nowDate = this.outpatientManager.GetDateTimeFromSysDateTime();
                        int age = (int)((new TimeSpan(nowDate.Ticks - this.rInfo.Birthday.Ticks)).TotalDays / 365);

                        if (age > 14)
                        {
                            price = NConvert.ToDecimal(rowFind["UNIT_PRICE"].ToString());
                        }
                        else
                        {
                            price = NConvert.ToDecimal(rowFind["CHILD_PRICE"].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);

                        return;
                    }
                    decimal pactQty = 0;
                    pactQty = NConvert.ToDecimal(rowFind["PACK_QTY"].ToString());
                    if (f.Item.PackQty == 0)
                    {
                        f.Item.PackQty = pactQty;
                    }
                    if (f.Item.PackQty == 0)
                    {
                        f.Item.PackQty = 1;
                    }
                    if (f.Item.Price == 0)
                    {
                        f.Item.Price = price;
                        f.OrgPrice = f.Item.Price;
                        f.Item.ChildPrice = f.Item.Price;
                    }
                }
                //--------------------------------------------------------------

                f.SpecialPrice = f.Item.Price;
                if (this.rInfo.Pact.Name == "公费")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    pactRate = this.pactUnitItemRateManager.GetOnePactUnitItemRateGY(rInfo.Name, rInfo.IDCard, 1);
                    if (sumPubCost < 200)
                    {
                        string Fee_Type = this.pactUnitItemRateManager.GetGYCOMCOMPARE(f.Item.ID);
                        if (Fee_Type == "gf")
                        {
                            f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * pactRate.Rate.PayRate) * (f.Item.Qty / f.Item.PackQty);
                            f.FT.PubCost = Convert.ToDecimal(f.Item.Price * pactRate.Rate.PubRate) * (f.Item.Qty / f.Item.PackQty);
                            f.FT.OwnCost = decimal.Round(f.FT.OwnCost, 2);
                            f.FT.PubCost = decimal.Round(f.FT.PubCost, 2);
                            totDisplayCost = pactRate.Rate.PayRate;

                            sumPubCost += f.FT.PubCost;
                            if (sumPubCost > 200)
                            {
                                f.FT.OwnCost = f.FT.OwnCost + (sumPubCost - 200);
                                f.FT.PubCost = f.FT.PubCost - (sumPubCost - 200);
                                //MessageBox.Show("报销金额超出限额，超出部分将按照自费进行收取");
                            }
                        }
                        else
                        {
                            totDisplayCost = f.FT.OwnCost + f.FT.PayCost + f.FT.PubCost;

                        }
                    }
                    else
                    {
                        totDisplayCost = f.FT.OwnCost + f.FT.PayCost + f.FT.PubCost;
                    }
                }
                else
                {
                    totDisplayCost = f.FT.OwnCost + f.FT.PayCost + f.FT.PubCost;
                }
                if (this.undrugManager.SetUltrasound(f.Item.ID))
                {
                    if (itemqty > 0)
                    {
                        totDisplayCost = f.Item.Price - pricece;  //减去价格差额
                    }
                    itemqty = itemqty + 1;
                }

                if (totDisplayCost == 0)
                {
                    returnRows = this.undrugManager.SetRestrictingfee(f.Item.ID, ref  LimitNumber);
                      if (returnRows <= 0)
                      {
                          totDisplayCost = Neusoft.FrameWork.Public.String.FormatNumber(f.Item.Price * f.Item.Qty / f.Item.PackQty, 2);
                      }
                }

                this.SetRowHeader(currRow, f, rowFind);

                //--------------------------------------------------------------
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemType].Text = f.Item.ItemType == EnumItemType.Drug ? "1" : "0";
                //--------------------------------------------------------------

                if (f.Item.ID != "999")
                {
                    ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text = userCode;
                    f.Item.UserCode = userCode;
                }

                //--------------------------------------------------------------
                if (f.Item.Specs == null || f.Item.Specs == string.Empty)
                {
                    specs = string.Empty;
                }
                else
                {
                    specs = "[" + f.Item.Specs + "]";
                }

                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemName].Text = f.Item.Name + specs;

                //--------------------------------------------------------------

                FarPoint.Win.Spread.CellType.ComboBoxCellType unitCell = new FarPoint.Win.Spread.CellType.ComboBoxCellType();

                unitCell.Editable = true;

                if (f.Item.ID != "999")
                {
                    //默认单位为天
                    minUnit = rowFind["MIN_UNIT"].ToString();
                    if (minUnit == string.Empty)
                    {
                        minUnit = "天";
                    }
                    packUnit = rowFind["PACK_UNIT"].ToString();
                    if (packUnit == string.Empty)
                    {
                        packUnit = "天";
                    }
                    unitCell.Items = new string[] { minUnit, packUnit };
                }

                //--------------------------------------------------------------

                else if (f.Item.ID == "999")
                {
                    unitCell.Items = new string[] { f.Item.PriceUnit };
                }

                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.PriceUnit].CellType = unitCell;
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.PriceUnit].Text = f.Item.PriceUnit;

                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.DoseOnce].Text = f.Order.DoseOnce == 0 ? string.Empty : f.Order.DoseOnce.ToString();
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.DoseUnit].Text = f.Order.DoseUnit;
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.CombNo].Text = f.Order.Combo.ID;
                if (freqDisplayType == "0")//汉字
                {
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text = myHelpFreq.GetName(f.Order.Frequency.ID);
                }
                else//代码
                {
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text = f.Order.Frequency.ID;
                }
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Text = myHelpUsage.GetName(f.Order.Usage.ID);

                //if (!f.Item.IsPharmacy)
                if (f.Item.ItemType != EnumItemType.Drug)
                {
                    string usageCode = f.Item.SysClass.ID.ToString();

                    NeuObject obj = this.managerIntegrate.GetConstansObj("MZUSAGECODE", usageCode);

                    if (obj != null && obj.Name != string.Empty)
                    {
                        try
                        {
                            this.fpSpread1_Sheet1.RowHeader.Cells[currRow, 0].BackColor = Color.FromArgb(NConvert.ToInt32(obj.Name));
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            this.fpSpread1_Sheet1.RowHeader.Cells[currRow, 0].BackColor = Color.FromArgb(-1250856);
                        }
                        catch { }
                    }
                }
                else
                {
                    if (f.Order.Usage != null)
                    {
                        string usageCode = f.Order.Usage.ID;

                        NeuObject obj = this.managerIntegrate.GetConstansObj("MZUSAGECODE", usageCode);

                        if (obj != null && obj.Name != string.Empty)
                        {
                            try
                            {
                                this.fpSpread1_Sheet1.RowHeader.Cells[currRow, 0].BackColor = Color.FromArgb(NConvert.ToInt32(obj.Name));
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                this.fpSpread1_Sheet1.RowHeader.Cells[currRow, 0].BackColor = Color.FromArgb(-1250856);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        try
                        {
                            this.fpSpread1_Sheet1.RowHeader.Cells[currRow, 0].BackColor = Color.FromArgb(-1250856);
                        }
                        catch { }
                    }
                }
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ExeDept].Text = f.ExecOper.Dept.Name;
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Cost].Text = totDisplayCost.ToString();

                if (f.FeePack == "1")//包装单位
                {
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Text = f.Item.Price.ToString();
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Amount].Text = (f.Item.Qty / f.Item.PackQty).ToString();
                }
                else
                {
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Text =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.Item.Price / f.Item.PackQty, 4).ToString();
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Amount].Text = (f.Item.Qty).ToString();
                }
                if (f.FeePack == "1")//包装单位
                {
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Text = f.Item.Price.ToString();
                }
                else
                {
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Text = Neusoft.FrameWork.Public.String.FormatNumber((f.Item.Price / f.Item.PackQty), 4).ToString();
                }
                //tangyi 默认不发送
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.IsSend].Value = false;
                f.IsSend = "0";
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Memo].Text = f.Memo;
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemCode].Text = f.Item.ID;
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.FeeCode].Text = f.Item.MinFee.ID;
                //this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemType].Text = f.Item.IsPharmacy == true ? "1" : "0";
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemType].Text = f.Item.ItemType == EnumItemType.Drug ? "1" : "0";
                this.fpSpread1_Sheet1.Rows.Add(currRow + 1, 1);
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.Cells[currRow + 1, (int)Columns.Select].Value = true;
                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Select].Value = true;

                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束
                this.fpSpread1_Sheet1.Rows[currRow].Tag = f;
                //if (f.Item.IsPharmacy)
                if (f.Item.ItemType == EnumItemType.Drug)
                {
                    //if (f.Item.SysClass.ID.ToString() == "PCC")
                    //{
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Days].Text = f.Days == 0 ? "1" : f.Days.ToString();
                    //}
                }
                if (f.Days == 0)
                {
                    f.Days = 1;
                }
                // f.FT.TotCost = totDisplayCost;//totDisplayCost
                //f.FT.PubCost = 0;
                //f.FT.PayCost = 0;
                //记账金额不变
                if (this.rInfo.Pact.Name == "公费")
                {
                    f.FT.TotCost = f.FT.PubCost + f.FT.OwnCost;
                }
                else
                {
                    f.FT.TotCost = totDisplayCost;
                    f.FT.OwnCost = totDisplayCost - f.FT.PubCost - f.FT.PayCost;
                }

                //--------------------------------------------------------------


                //--------------------------------------------------------------

                if (f.Item.ID == "999")
                {
                    f.Item.IsNeedBespeak = false;
                    f.Item.IsNeedConfirm = false;
                }

                //--------------------------------------------------------------
                else if (f.Item.ID != "999")
                {
                    f.Item.IsNeedBespeak = NConvert.ToBoolean(rowFind["NEEDBESPEAK"].ToString());
                    //if (rowFind["CONFIRM_FLAG"].ToString() == "2" || rowFind["CONFIRM_FLAG"].ToString() == "3" || rowFind["CONFIRM_FLAG"].ToString() == "1")
                    //{
                    //    f.Item.IsNeedConfirm = true;
                    //}
                    //else
                    //{
                    //    f.Item.IsNeedConfirm = false;
                    //}

                    if (string.IsNullOrEmpty(rowFind["CONFIRM_FLAG"].ToString()))
                    {
                        f.Item.NeedConfirm = EnumNeedConfirm.None;
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                            Neusoft.FrameWork.Function.NConvert.ToInt32(rowFind["CONFIRM_FLAG"].ToString())))
                        {
                            f.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFind["CONFIRM_FLAG"].ToString());
                        }
                    }
                }


                this.SetItemRateInfo(currRow, f);
                currRow++;
            }

            decimal totCost = SumCost();

            FeeItemList feeItem = new FeeItemList();

            if (!this.isCanModifyCharge || this.rInfo.ChkKind == "1" || this.rInfo.ChkKind == "2")//不可以修改划价信息
            {
                for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag != null && this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        feeItem = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                        if (feeItem.FTSource == "0" || feeItem.Item.IsMaterial)//自己批费,可以随便修改
                        {
                            this.SetColumnEnable(i);
                            //if (feeItem.Item.IsPharmacy)
                            if (feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                if (feeItem.Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                {
                                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.Days].Locked = false;
                                }
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Amount].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.DoseOnce].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Freq].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Usage].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.ExeDept].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.PriceUnit].Locked = false;
                            }
                            else
                            {
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Amount].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.ExeDept].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.MachineNO].Locked = false;

                            }
                            if (feeItem.Item.Price == 0)
                            {
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Locked = false;
                            }
                            if (feeItem.Order.Combo.ID == null || feeItem.Order.Combo.ID == string.Empty)
                            {
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.CombNo].Locked = false;
                            }
                        }
                        else//不是自己批费,不可以修改除了院注次数的任何信息
                        {

                            for (int j = 0; j < this.fpSpread1_Sheet1.Columns.Count; j++)
                            {
                                ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                if (j == (int)Columns.InputCode)
                                {
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType.ReadOnly = true;
                                    this.fpSpread1_Sheet1.Cells[i, j].CellType = textCellType;
                                }
                                else if (j == (int)Columns.Usage)
                                {
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType.ReadOnly = true;
                                    this.fpSpread1_Sheet1.Cells[i, j].CellType = textCellType;
                                    this.fpSpread1_Sheet1.Cells[i, j].Locked = false;
                                }
                                else if (j == (int)Columns.ExeDept)
                                {
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType.ReadOnly = true;
                                    this.fpSpread1_Sheet1.Cells[i, j].CellType = textCellType;
                                    this.fpSpread1_Sheet1.Cells[i, j].Locked = false;
                                }
                                else if (j == (int)Columns.MachineNO)
                                {
                                    this.fpSpread1_Sheet1.Cells[i, j].Locked = false;
                                }
                                else
                                {
                                    this.fpSpread1_Sheet1.Cells[i, j].Locked = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        this.SetColumnEnable(i);
                    }
                }
            }
            else //可以修改划价信息
            {
                for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                {
                    this.SetColumnEnable(i);

                    if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                    {
                        if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                        {
                            feeItem = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                            //如果已经门诊账户支付明细,不可以更改任何信息.
                            if (feeItem.IsAccounted)
                            {
                                continue;
                            }

                            //if (feeItem.Item.IsPharmacy)
                            if (feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                if (feeItem.Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                {
                                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.Days].Locked = false;
                                }
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Amount].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.DoseOnce].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Freq].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Usage].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.ExeDept].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.PriceUnit].Locked = false;
                            }
                            else
                            {
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Amount].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.ExeDept].Locked = false;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.MachineNO].Locked = false;

                            }
                            if (feeItem.Item.Price == 0)
                            {
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Locked = false;
                            }
                            if (feeItem.Order.Combo.ID == null || feeItem.Order.Combo.ID == string.Empty)
                            {
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.CombNo].Locked = false;
                            }
                        }
                        else
                        {   ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.InputCode].Locked = false;
                            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.Select].Locked = false;
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.IsSend].Locked = false;//tangyi
                        }
                    }
                    else
                    {   ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.InputCode].Locked = false;
                        //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Select].Locked = false;
                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.IsSend].Locked = false;//tangyi
                    }
                }
            }
            rowCount = this.fpSpread1_Sheet1.Rows.Count;

            this.DrawCombo(this.fpSpread1_Sheet1, (int)Columns.CombNo, (int)Columns.CombNoDisplay, 0);
            ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            this.fpSpread1_Sheet1.SetActiveCell(rowCount - 1, (int)Columns.InputCode, false);


            //--------------------------------------------------------------
        }

        /// <summary>
        /// 画组合信息
        /// </summary>
        /// <param name="sender">传入的farpointSheetView</param>
        /// <param name="column">列顺序</param>
        /// <param name="DrawColumn">话的顺序</param>
        /// <param name="ChildViewLevel"></param>
        private void DrawCombo(object sender, int column, int DrawColumn, int ChildViewLevel)
        {
            switch (sender.GetType().ToString().Substring(sender.GetType().ToString().LastIndexOf(".") + 1))
            {
                case "SheetView":
                    FarPoint.Win.Spread.SheetView o = sender as FarPoint.Win.Spread.SheetView;
                    int i = 0;
                    string tmp = string.Empty, curComboNo = string.Empty;
                    if (ChildViewLevel == 0)
                    {
                        for (i = 0; i < o.RowCount; i++)
                        {
                            #region "画"
                            if (o.Cells[i, column].Text == "0") o.Cells[i, column].Text = string.Empty;
                            tmp = o.Cells[i, column].Text + string.Empty;
                            o.Cells[i, column].Tag = tmp;
                            if (curComboNo != tmp && tmp != string.Empty) //是头
                            {
                                curComboNo = tmp;
                                o.Cells[i, DrawColumn].Text = "┓";
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┛";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┓")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = string.Empty;
                                    }
                                }
                                catch { }
                            }
                            else if (curComboNo == tmp && tmp != string.Empty)
                            {
                                o.Cells[i, DrawColumn].Text = "┃";
                            }
                            else if (curComboNo != tmp && tmp == string.Empty)
                            {
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┛";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┓")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = string.Empty;
                                    }
                                }
                                catch { }
                                o.Cells[i, DrawColumn].Text = string.Empty;
                                curComboNo = string.Empty;
                            }
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┃") o.Cells[i, DrawColumn].Text = "┛";
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┓") o.Cells[i, DrawColumn].Text = string.Empty;
                            o.Cells[i, DrawColumn].ForeColor = System.Drawing.Color.Red;
                            #endregion
                        }
                    }
                    else if (ChildViewLevel == 1)
                    {
                        for (int m = 0; m < o.RowCount; m++)
                        {
                            FarPoint.Win.Spread.SheetView c = o.GetChildView(m, 0);
                            for (int j = 0; j < c.RowCount; j++)
                            {
                                #region "画"
                                if (c.Cells[j, column].Text == "0") c.Cells[j, column].Text = string.Empty;
                                tmp = c.Cells[j, column].Text + string.Empty;

                                c.Cells[j, column].Tag = tmp;
                                if (curComboNo != tmp && tmp != string.Empty) //是头
                                {
                                    curComboNo = tmp;
                                    c.Cells[j, DrawColumn].Text = "┓";
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┛";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┓")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = string.Empty;
                                        }
                                    }
                                    catch { }
                                }
                                else if (curComboNo == tmp && tmp != string.Empty)
                                {
                                    c.Cells[j, DrawColumn].Text = "┃";
                                }
                                else if (curComboNo != tmp && tmp == string.Empty)
                                {
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┛";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┓")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = string.Empty;
                                        }
                                    }
                                    catch { }
                                    c.Cells[j, DrawColumn].Text = string.Empty;
                                    curComboNo = string.Empty;
                                }
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┃") c.Cells[j, DrawColumn].Text = "┛";
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┓") c.Cells[j, DrawColumn].Text = string.Empty;
                                c.Cells[j, DrawColumn].ForeColor = System.Drawing.Color.Red;
                                #endregion

                            }
                        }
                    }
                    break;
            }

        }

        /// <summary>
        /// 验证数据是否输入合法
        /// </summary>
        /// <param name="row">当前行</param>
        /// <param name="col">当前列</param>
        /// <param name="colName">列名字</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="currValue">返回的当前输入值</param>
        /// <returns>true合法 false不合法</returns>
        private bool InputDataIsValid(int row, int col, string colName, decimal maxValue, decimal minValue, ref decimal currValue)
        {
            try
            {
                currValue = NConvert.ToDecimal(
                    Neusoft.FrameWork.Public.String.ExpressionVal(
                    this.fpSpread1_Sheet1.Cells[row, col].Text.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(colName + Language.Msg("列输入的计算公式不正确，请重新输入!") + ex.Message);
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(row, col);

                return false;
            }

            if (currValue <= minValue)
            {
                MessageBox.Show(colName + Language.Msg("的值不能小于") + minValue.ToString() + Language.Msg("或者输入的数值过大超出允许范围!"));
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(row, col);

                return false;
            }
            if (currValue > maxValue)
            {
                MessageBox.Show(colName + Language.Msg("的值不能大于") + maxValue.ToString() + "!");
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(row, col);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得最大组合号
        /// </summary>
        /// <returns></returns>
        private string GetMaxCombNo()
        {
            double combNO = 0;
            double tempCombNO = 0;
            for (int i = 0; i < this.fpSpread1_Sheet1.Rows.Count; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null && this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    FeeItemList feeItem = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                    try
                    {
                        tempCombNO = System.Convert.ToInt64(feeItem.Order.Combo.ID);
                    }
                    catch
                    {

                    }

                    if (tempCombNO > combNO)
                    {
                        combNO = tempCombNO;
                    }
                }
            }

            return (combNO + 1).ToString();
        }

        /// <summary>
        /// 获得新行
        /// </summary>
        /// <returns> -1 失败 其他成功</returns>
        private int GetNewRow()
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Cells[i, (int)Columns.ItemName].Text == string.Empty)
                {
                    return i;
                }
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null && this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    continue;
                }
                if (this.fpSpread1_Sheet1.Cells[i, (int)Columns.ItemName].Text == "小计")
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <summary>
        /// 判断新项目是否在两个草药之间
        /// </summary>
        /// <param name="row"></param>
        /// <param name="days"></param>
        /// <param name="combNO"></param>
        /// <returns></returns>
        private bool JudgeInPCC(int row, ref decimal days, ref string combNO)
        {
            int tempRow = row - 1;

            if (tempRow < 0)
            {
                return false;
            }

            if (this.fpSpread1_Sheet1.Rows[tempRow].Tag == null)
            {
                return false;
            }

            if ((FeeItemList)this.fpSpread1_Sheet1.Rows[tempRow].Tag == null)
            {
                return false;
            }

            if (((FeeItemList)this.fpSpread1_Sheet1.Rows[tempRow].Tag).Item.SysClass.ID.ToString() != "PCC")
            {
                return false;
            }

            tempRow = row + 1;

            if (tempRow > this.fpSpread1_Sheet1.Rows.Count - 1)
            {
                return false;
            }

            if (this.fpSpread1_Sheet1.Rows[tempRow].Tag == null)
            {
                return false;
            }

            if ((FeeItemList)this.fpSpread1_Sheet1.Rows[tempRow].Tag == null)
            {
                return false;
            }

            if (((FeeItemList)this.fpSpread1_Sheet1.Rows[tempRow].Tag).Item.SysClass.ID.ToString() != "PCC")
            {
                return false;
            }

            days = ((FeeItemList)this.fpSpread1_Sheet1.Rows[row - 1].Tag).Days;
            combNO = ((FeeItemList)this.fpSpread1_Sheet1.Rows[row - 1].Tag).Order.Combo.ID;

            return true;
        }


        #region 获取默认执行科室
        private static Dictionary<string, Neusoft.SOC.HISFC.Fee.Models.Undrug> dicUndrugExec = new Dictionary<string, Neusoft.SOC.HISFC.Fee.Models.Undrug>();

        private static Neusoft.SOC.HISFC.Fee.Models.Undrug GetUndrugExecInfo(string itemCode)
        {
            Neusoft.SOC.HISFC.Fee.Models.Undrug item = null;
            if (dicUndrugExec.ContainsKey(itemCode))
            {
                item = dicUndrugExec[itemCode];
            }
            else
            {
                Neusoft.SOC.HISFC.Fee.BizLogic.Undrug undrugMgr = new Neusoft.SOC.HISFC.Fee.BizLogic.Undrug();
                item = undrugMgr.GetExecInfo(itemCode);

                dicUndrugExec.Add(itemCode, item);
            }

            return item;
        }
        #endregion

        /// <summary>
        /// 选择项目
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="drugFlag"></param>
        /// <param name="exeDeptCode"></param>
        /// <param name="row"></param>
        /// <param name="amount"></param>
        /// <param name="saleprice"></param>
        /// <param name="unitFlag">针对组套维护的包装单位 1 最小单位 2 包装单位 其他值 未知,不处理</param>
        private void SetItem(string itemCode, string drugFlag, string exeDeptCode, int row, decimal amount, decimal saleprice, string unitFlag)
        {
            if (isInputItemsNoSpe)
            {
                if (this.rInfo == null)
                {
                    MessageBox.Show(Language.Msg("请选择患者"));

                    this.isDealCellChange = true;

                    return;
                }

                if (this.rInfo.DoctorInfo.Templet.Dept.ID == null || this.rInfo.DoctorInfo.Templet.Dept.ID == string.Empty)
                {
                    MessageBox.Show(Language.Msg("请选择看诊科室!"));

                    this.isDealCellChange = true;

                    return;
                }
            }
            this.isDealCellChange = false;

            DataRow findRow;
            DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "' and drug_flag = '" + drugFlag + "'");

            if (rowFinds == null || rowFinds.Length == 0)
            {
                MessageBox.Show("编码为: [" + itemCode + " ] 的项目查找失败!");
                this.isDealCellChange = true;

                return;
            }

            findRow = rowFinds[0];
            #region {5D62CB1F-6134-48f4-B905-02AD69D6A433}我们的程序都应该做到取最新价格。
            //获得收费员所在科室的维护药房中的药品，非药品和组合项目，组套全部获得

            DataSet dsItemNow = new DataSet();
            iReturn = this.outpatientManager.QueryItemList(myOperator.Dept.ID, itemCode, ref dsItemNow);
            if (iReturn == -1)
            {
                MessageBox.Show("获得项目出错!" + this.outpatientManager.Err);
                return;
            }
            DataRow findRowNow;
            DataRow[] rowFindsNow = dsItemNow.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "' and drug_flag = '" + drugFlag + "'");

            if (rowFindsNow == null || rowFindsNow.Length == 0)
            {
                MessageBox.Show("编码为: [" + itemCode + " ] 的项目查找失败!");
                this.isDealCellChange = true;

                return;
            }

            findRowNow = rowFindsNow[0];

            bool isPriceChange = false;

            if (NConvert.ToDecimal(findRowNow["UNIT_PRICE"].ToString()) != NConvert.ToDecimal(findRow["UNIT_PRICE"].ToString()))
            {
                findRow["UNIT_PRICE"] = findRowNow["UNIT_PRICE"];
                isPriceChange = true;
            }
            if (NConvert.ToDecimal(findRowNow["SP_PRICE"].ToString()) != NConvert.ToDecimal(findRow["SP_PRICE"].ToString()))
            {
                findRow["SP_PRICE"] = findRowNow["SP_PRICE"];
                isPriceChange = true;
            }
            if (NConvert.ToDecimal(findRowNow["CHILD_PRICE"].ToString()) != NConvert.ToDecimal(findRow["CHILD_PRICE"].ToString()))
            {
                findRow["CHILD_PRICE"] = findRowNow["CHILD_PRICE"];
                isPriceChange = true;
            }
            // {B9303CFE-755D-4585-B5EE-8C1901F79450}
            if (NConvert.ToDecimal(findRowNow["PURCHASE_PRICE"].ToString()) != NConvert.ToDecimal(findRow["PURCHASE_PRICE"].ToString()))
            {
                findRow["PURCHASE_PRICE"] = findRowNow["PURCHASE_PRICE"];
                isPriceChange = true;
            }
            if (isPriceChange)
            {
                FillFilterControl();
            }
            #endregion

            //如果是物资项目，进行精确查找,因为可能存在多库存项目
            //{40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
            if (findRow["DRUG_FLAG"].ToString() == "6")
            {
                DataRow[] mateRow = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "'" + " and unit_price = " + saleprice + "" + " and EXE_DEPT = '" + exeDeptCode + "'");
                if (mateRow == null || mateRow.Length == 0)
                {
                    MessageBox.Show("编码为: [" + itemCode + " ] 的项目查找失败!");
                    this.isDealCellChange = true;

                    return;
                }
                findRow = mateRow[0];
            }

            //如果是药品,进行精确查找,因为可能存在多库存项目
            if (findRow["DRUG_FLAG"].ToString() == "1")
            {
                DataRow[] rowFindAgain = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "'" + " and EXE_DEPT = '" + exeDeptCode + "'");

                if (rowFindAgain == null || rowFindAgain.Length == 0)
                {
                    rowFindAgain = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "'");
                    if (rowFindAgain == null || rowFindAgain.Length == 0)
                    {
                        MessageBox.Show("编码为: [" + itemCode + " ] 的项目查找失败!");
                        this.isDealCellChange = true;

                        return;
                    }
                }
                findRow = rowFindAgain[0];
            }

            //项目基本信息实体
            FeeItemList feeItemList = new FeeItemList();

            //如果找到项目
            if (findRow != null)
            {
                decimal price = 0;		//单价

                decimal pactQty = 0;	//包装数量
                string specs = string.Empty;		//规格
                string exeDept = string.Empty;	//执行科室
                string itemType = string.Empty;	//项目类别
                string minUnit = string.Empty;	//最小单位
                string packUnit = string.Empty;   //包装单位
                string freqCode = string.Empty;	//频次代码
                string usageCode = string.Empty;	//用法代码
                decimal baseDose = 0m;//基本用量

                //保留添加的行
                this.alAddRows.Add(row);

                #region 项目类别
                itemType = findRow["DRUG_FLAG"].ToString();

                //非药品
                if (itemType == "0")
                {
                    feeItemList.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                    //feeItemList.Item.IsPharmacy = false;
                    feeItemList.Item.ItemType = EnumItemType.UnDrug;
                    feeItemList.IsGroup = false;
                }
                //药品
                if (itemType == "1")
                {
                    feeItemList.Item = new Neusoft.HISFC.Models.Pharmacy.Item();
                    //feeItemList.Item.IsPharmacy = true;
                    feeItemList.Item.ItemType = EnumItemType.Drug;

                    feeItemList.IsGroup = false;
                }
                //组合项目
                if (itemType == "2")
                {
                    //feeItemList.Item.IsPharmacy = false;
                    feeItemList.Item.ItemType = EnumItemType.UnDrug;
                    feeItemList.IsGroup = true;
                }

                //协定处方{ED51E97B-B752-4c32-BD93-F80209A24879}
                if (itemType == "4")
                {
                    //if (Neusoft.HISFC.Integrate.Pharmacy.IsNostrumManageStore)暂时屏蔽
                    {
                        feeItemList.Item = new Neusoft.HISFC.Models.Pharmacy.Item();
                        feeItemList.Item.ItemType = EnumItemType.Drug;
                        ((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).IsNostrum = true;
                        feeItemList.FeePack = "0";
                        feeItemList.IsGroup = false;
                        feeItemList.IsNostrum = true;
                    }
                    //else
                    //{
                    //    feeItemList.Item = new Neusoft.HISFC.Models.Pharmacy.Item();
                    //    feeItemList.Item.ItemType = EnumItemType.Drug;
                    //    ((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).IsNostrum = true;
                    //    feeItemList.FeePack = "0";
                    //    feeItemList.IsGroup = true;
                    //}

                    SetItemDisplay(row, Color.Sienna, "协定", new Font("宋体", 9, FontStyle.Bold));
                }//{ED51E97B-B752-4c32-BD93-F80209A24879}结束

                //组套
                if (itemType == "3")//组套
                {
                    ArrayList groupDetails = this.managerIntegrate.QueryGroupDetailByGroupCode(itemCode);
                    if (groupDetails == null)
                    {
                        MessageBox.Show("获得组套明细出错!" + this.managerIntegrate.Err);
                        this.isDealCellChange = true;

                        return;
                    }
                    int actIndex = row;
                    ucInputTimes uc = new ucInputTimes();
                    Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(uc);

                    int times = uc.Times;

                    foreach (Neusoft.HISFC.Models.Fee.ComGroupTail detail in groupDetails)
                    {
                        string drugflag = "1";

                        if (detail.drugFlag == "2")
                        {
                            drugflag = "0";
                        }
                        else if (detail.drugFlag == "3")
                        {
                            drugflag = "2";
                        }
                        //if (detail.deptCode == string.Empty)
                        {
                            detail.deptCode = exeDeptCode;
                        }
                        //{40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                        this.SetItem(detail.itemCode, drugflag, detail.deptCode, actIndex, detail.qty * times, 0, detail.unitFlag);
                        actIndex = GetNewRow();
                        if (actIndex == -1)
                        {
                            this.fpSpread1.StopCellEditing();
                            this.fpSpread1_Sheet1.Rows.Add(this.fpSpread1_Sheet1.RowCount, 1);
                            actIndex = this.fpSpread1_Sheet1.RowCount - 1;
                        }
                    }

                    return;
                }

                #region 物资收费(不对照的物资)
                //{40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                if (itemType == "6")
                {
                    feeItemList.Item = new Neusoft.HISFC.Models.FeeStuff.MaterialItem();
                    feeItemList.Item.ItemType = EnumItemType.MatItem;
                }
                #endregion

                #endregion

                #region 编码

                feeItemList.Item.ID = itemCode;
                feeItemList.ID = itemCode;
                feeItemList.CancelType = CancelTypes.Valid;
                #endregion

                #region 自定义编码

                //无论输入拼音码还是五笔等，都最后显示自定义码
                ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.InputCode].Text = findRow["User_Code"].ToString();
                feeItemList.Item.UserCode = findRow["User_Code"].ToString();

                #endregion

                #region 规格

                //显示项目名称，如果是药品和规格一起显示
                specs = findRow["SPECS"].ToString();
                feeItemList.Item.Specs = specs;
                if (specs == null || specs == string.Empty)
                {
                    specs = string.Empty;
                }
                else
                {
                    specs = "[" + specs + "]";
                }
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.ItemName].Text = findRow["ITEM_NAME"].ToString() + specs;

                #endregion

                #region 名称

                feeItemList.Item.Name = findRow["ITEM_NAME"].ToString();
                feeItemList.Name = feeItemList.Item.Name;

                #endregion

                #region 付数

                //付数
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Text = string.Empty;
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Locked = true;
                feeItemList.Days = 1;

                #endregion

                #region 系统类别和样本类别

                feeItemList.Item.SysClass.ID = findRow["SYS_CLASS"].ToString();
                feeItemList.Order.Sample.Name = findRow["DEFAULT_SAMPLE"].ToString();//样本

                #endregion

                #region 药品属性

                //如果过是药品
                //if (feeItemList.Item.IsPharmacy)
                if (feeItemList.Item.ItemType == EnumItemType.Drug)
                {
                    //如果是草药
                    if (feeItemList.Item.SysClass.ID.ToString() == "PCC")
                    {
                        decimal tempDays = 0m;
                        string tempCombNO = string.Empty;

                        // 草药默认值
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Value = "1";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.CombNo].Value = "1";

                        if (this.JudgeInPCC(row, ref tempDays, ref tempCombNO))
                        {
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Value = tempDays;//默认数量为1
                            feeItemList.Days = tempDays;
                            feeItemList.Order.Combo.ID = tempCombNO;
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.CombNo].Value = tempCombNO;
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Locked = false;

                        }
                        else
                        {
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Value = hDays;//默认数量为1
                            feeItemList.Days = hDays;
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Locked = false;
                        }

                        //协定处方的草药采用西药的跳转方式
                        if ((feeItemList.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                        {
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Days].Locked = true;
                        }
                    }

                    //剂量单位
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.DoseUnit].Text = findRow["DOSE_UNIT"].ToString();

                    //需要转换单位
                    if (this.invertUnitHelper.GetObjectFromName(findRow["MIN_UNIT"].ToString()) != null || this.specialInvertUnitHelper.GetObjectFromID(findRow["ITEM_CODE"].ToString()) != null)
                    {
                        feeItemList.Order.DoseUnit = findRow["MIN_UNIT"].ToString();
                    }
                    else
                    {
                        feeItemList.Order.DoseUnit = findRow["DOSE_UNIT"].ToString();
                    }

                    //剂量单位
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.DoseUnit].Text = feeItemList.Order.DoseUnit;

                    #region 妇幼先改，后加控制参数  控制参数是MZ0057. 后改
                    if (!this.isDoseOnceCanNull)
                    {
                        //频次(药品)
                        freqCode = findRow["FREQ_CODE"].ToString();
                        if (freqCode == string.Empty)
                        {
                            freqCode = "QD";
                        }
                        string freqName = myHelpFreq.GetName(freqCode);
                        if (freqDisplayType == "0")//汉字
                        {
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Freq].Text = freqName;
                        }
                        else//编码
                        {
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Freq].Text = freqCode;
                        }
                        feeItemList.Order.Frequency.ID = freqCode;
                        feeItemList.Order.Frequency.Name = freqName;
                        if (this.invertUnitHelper.GetObjectFromName(findRow["MIN_UNIT"].ToString()) != null || this.specialInvertUnitHelper.GetObjectFromID(findRow["ITEM_CODE"].ToString()) != null)
                        {
                            //基本用量
                            baseDose = NConvert.ToDecimal(findRow["ONCE_DOSE"].ToString());
                            if (baseDose <= 0)
                            {
                                baseDose = NConvert.ToDecimal(findRow["BASE_DOSE"].ToString());
                            }

                            if (NConvert.ToDecimal(findRow["ONCE_DOSE"].ToString()) > 0)
                            {
                                baseDose = baseDose / NConvert.ToDecimal(findRow["BASE_DOSE"].ToString());
                            }

                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.DoseOnce].Text = baseDose.ToString();
                            feeItemList.Order.DoseOnce = baseDose;
                        }
                        else
                        {
                            //基本用量
                            baseDose = NConvert.ToDecimal(findRow["ONCE_DOSE"].ToString());
                            if (baseDose <= 0)
                            {
                                baseDose = NConvert.ToDecimal(findRow["BASE_DOSE"].ToString());
                            }
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.DoseOnce].Text = baseDose.ToString();
                            feeItemList.Order.DoseOnce = baseDose;
                        }
                        //{1FAD3FA2-C7D8-4cac-845F-B9EBECDE2312}
                        (feeItemList.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose = NConvert.ToDecimal(findRow["BASE_DOSE"].ToString());


                        //用法(药品)
                        usageCode = findRow["USAGE_CODE"].ToString();
                        string useName = myHelpUsage.GetName(usageCode);
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Usage].Text = useName;
                        feeItemList.Order.Usage.ID = usageCode;
                        feeItemList.Order.Usage.Name = useName;
                    }
                    else
                    {
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Freq].Locked = true;
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.DoseOnce].Locked = true;
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Usage].Locked = true;
                    }
                    #endregion
                    feeItemList.Invoice.User01 = findRow["SPLIT_TYPE"].ToString();
                    (feeItemList.Item as Neusoft.HISFC.Models.Pharmacy.Item).PackUnit = findRow["pack_unit"].ToString();
                    (feeItemList.Item as Neusoft.HISFC.Models.Pharmacy.Item).MinUnit = findRow["min_unit"].ToString();

                }
                //if (!feeItemList.Item.IsPharmacy)
                if (feeItemList.Item.ItemType != EnumItemType.Drug)
                {
                    string idCode = feeItemList.Item.SysClass.ID.ToString();

                    Neusoft.FrameWork.Models.NeuObject obj = this.managerIntegrate.GetConstansObj("MZUSAGECODE", idCode);

                    if (obj != null && obj.Name != string.Empty)
                    {
                        try
                        {
                            this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].BackColor = Color.FromArgb(NConvert.ToInt32(obj.Name));
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].BackColor = Color.FromArgb(-1250856);
                        }
                        catch { }
                    }
                }
                else
                {
                    if (feeItemList.Order.Usage != null)
                    {
                        string idCode = feeItemList.Order.Usage.ID;

                        Neusoft.FrameWork.Models.NeuObject obj = this.managerIntegrate.GetConstansObj("MZUSAGECODE", idCode);

                        if (obj != null && obj.Name != string.Empty)
                        {
                            try
                            {
                                this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].BackColor = Color.FromArgb(NConvert.ToInt32(obj.Name));
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].BackColor = Color.FromArgb(-1250856);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        try
                        {
                            this.fpSpread1_Sheet1.RowHeader.Cells[row, 0].BackColor = Color.FromArgb(-1250856);
                        }
                        catch { }
                    }
                }
                #endregion

                #region 执行科室
                exeDept = findRow["EXE_DEPT"].ToString();

                //是否需要预约
                feeItemList.Item.IsNeedBespeak = NConvert.ToBoolean(findRow["NEEDBESPEAK"].ToString());

                feeItemList.Item.IsNeedConfirm = false;
                if (string.IsNullOrEmpty(findRow["CONFIRM_FLAG"].ToString()))
                {
                    feeItemList.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(findRow["CONFIRM_FLAG"].ToString())))
                    {
                        feeItemList.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), findRow["CONFIRM_FLAG"].ToString());
                    }
                }

                if (this.rInfo != null)
                {
                    feeItemList.RecipeOper.ID = this.rInfo.DoctorInfo.Templet.Doct.ID;
                    feeItemList.RecipeOper.Name = this.rInfo.DoctorInfo.Templet.Doct.Name;
                    //{33607355-C383-4271-B46C-0FBBAC251382} 开方医生所属科室编码
                    feeItemList.RecipeOper.Dept.ID = this.rInfo.DoctorInfo.Templet.Dept.ID;
                    feeItemList.RecipeOper.Dept.Name = this.rInfo.DoctorInfo.Templet.Dept.Name;
                }

                #region {3AEB5613-1CB0-4158-89E6-F82F0B643388}
                List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct> medicalGroup = new List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct>();
                medicalGroup = GetMedicalGroupCode(feeItemList.RecipeOper.Dept.ID, feeItemList.RecipeOper.ID);
                if (medicalGroup == null)
                {
                    medicalGroup = new List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct>();
                }
                if (medicalGroup.Count > 0)
                {
                    Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct doc = medicalGroup[0] as Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct;
                    feeItemList.MedicalGroupCode = doc.MedcicalTeam;
                }

                #endregion

                #region 修改费用来源,如果当前登录科室是医技,则FTSource赋值为2
                //feeItemList.FTSource = "0";//收费员自己收费

                NeuObject curOperDept = ((Neusoft.HISFC.Models.Base.Employee)this.constantManager.Operator).Dept;

                if ("T".Equals(departManager.GetDeptmentById(curOperDept.ID).DeptType.ID))
                {
                    feeItemList.FTSource = "2";
                }
                else
                {
                    feeItemList.FTSource = "0";
                }

                #endregion
                //{CA82280B-51B6-4462-B63E-43F4ECF456A3}
                if (drugFlag == "0")//非药品
                {
                    if (dsItem.Tables[0].Columns.Contains("FUNCTIONCLASS"))
                    {
                        (feeItemList.Item as Neusoft.HISFC.Models.Fee.Item.Undrug).ItemPriceType = findRow["FUNCTIONCLASS"].ToString();
                    }
                    if (exeDeptCode != null)
                    {
                        exeDept = exeDeptCode;
                    }
                    //else
                    //{
                    //    exeDept = string.Empty;
                    //}
                    (feeItemList.Item as Neusoft.HISFC.Models.Fee.Item.Undrug).ExecDept = exeDept;
                    exeDeptCode = this.SetExecDept(row, feeItemList);
                    if (!string.IsNullOrEmpty(exeDeptCode))
                    {
                        exeDept = exeDeptCode;
                    }
                }

                if (drugFlag == "1")
                {
                    ArrayList alExecDept = null;
                    Neusoft.FrameWork.Models.NeuObject dept = this.managerIntegrate.GetDepartment(exeDept);
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = dept.Name;
                    feeItemList.ExecOper.Dept.ID = dept.ID;
                    feeItemList.ExecOper.Dept.Name = dept.Name;
                    lbDept.Items.Clear();
                    SOC.HISFC.BizProcess.Cache.Common.GetDept("1");
                    alExecDept = SOC.HISFC.BizProcess.Cache.Common.deptHelper.ArrayObject;
                    lbDept.AddItems(alExecDept);

                }
                else
                {
                    ArrayList alExecDept = null;

                    string defaultExecDept = string.Empty;

                    SOC.HISFC.BizProcess.Cache.Common.SetExecDept(true, feeItemList.RecipeOper.Dept.ID, feeItemList.Item.ID, ref defaultExecDept, ref alExecDept);
                    if (alExecDept == null || alExecDept.Count == 0)
                    {
                        SOC.HISFC.BizProcess.Cache.Common.GetDept("1");
                        alExecDept = SOC.HISFC.BizProcess.Cache.Common.deptHelper.ArrayObject;
                    }
                    Neusoft.FrameWork.Models.NeuObject dept = this.managerIntegrate.GetDepartment(defaultExecDept);
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = dept.Name;
                    feeItemList.ExecOper.Dept.ID = dept.ID;
                    feeItemList.ExecOper.Dept.Name = dept.Name;
                    lbDept.Items.Clear();
                    lbDept.AddItems(alExecDept);
                }


                #region 屏蔽原有执行科室处理方式
                //if (exeDept.Contains("ALL"))
                //{
                //    exeDept = string.Empty;
                //}

                //if (exeDept != string.Empty)
                //{
                //    exeDept = exeDept.TrimEnd('|');
                //    string[] s = exeDept.Split('|');
                //    Neusoft.HISFC.Models.Base.Department dept = null;
                //    if (s.Length == 0)
                //    {
                //        lbDept.Items.Clear();
                //        lbDept.AddItems(alDept);
                //    }
                //    else if (s.Length == 1 && s[0] != "无")
                //    {
                //         Neusoft.SOC.HISFC.Fee.Models.Undrug item = GetUndrugExecInfo(itemCode);

                //         if (item != null && !string.IsNullOrEmpty(item.DefaultExecDeptForOut))
                //         {
                //             dept = this.managerIntegrate.GetDepartment(item.DefaultExecDeptForOut);
                //         }
                //         else
                //         {
                //             dept = this.managerIntegrate.GetDepartment(s[0]);
                //         }

                //        if (dept == null)
                //        {
                //            MessageBox.Show("获得执行科室出错!" + this.managerIntegrate.Err);
                //            this.isDealCellChange = true;

                //            return;
                //        }
                //        this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = dept.Name;
                //        feeItemList.ExecOper.Dept.ID = dept.ID;
                //        feeItemList.ExecOper.Dept.Name = dept.Name;
                //        //lbDept.alItems = null;
                //        //lbDept.AddItems(alDept);
                //    }
                //    else if (s.Length > 1)
                //    {
                //        Neusoft.SOC.HISFC.Fee.Models.Undrug item = GetUndrugExecInfo(itemCode);

                //        if (item != null && !string.IsNullOrEmpty(item.DefaultExecDeptForOut))
                //        {
                //            dept = this.managerIntegrate.GetDepartment(item.DefaultExecDeptForOut);
                //        }
                //        else
                //        {
                //            bool isRecipeDept = false;
                //            for (int index = 0; index < s.Length; index++)
                //            {
                //                if (s[index] == feeItemList.RecipeOper.Dept.ID)
                //                {
                //                    isRecipeDept = true;
                //                }
                //            }
                //            if (isRecipeDept)
                //            {
                //                dept = this.managerIntegrate.GetDepartment(feeItemList.RecipeOper.Dept.ID);
                //            }
                //            else
                //            {
                //                dept = this.managerIntegrate.GetDepartment(s[0]);
                //            }
                //        }

                //        if (dept == null)
                //        {
                //            MessageBox.Show("获得执行科室出错!" + this.managerIntegrate.Err);
                //            this.isDealCellChange = true;

                //            return;
                //        }
                //        this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = dept.Name;
                //        feeItemList.ExecOper.Dept.ID = dept.ID;
                //        feeItemList.ExecOper.Dept.Name = dept.Name;
                //        ArrayList deptListTemp = new ArrayList();

                //        foreach (string sDeptCode in s)
                //        {
                //            dept = this.managerIntegrate.GetDepartment(sDeptCode);
                //            if (dept == null)
                //            {
                //                MessageBox.Show("获得执行科室出错!" + this.managerIntegrate.Err);
                //                this.isDealCellChange = true;

                //                return;
                //            }
                //            deptListTemp.Add((Neusoft.FrameWork.Models.NeuObject)dept);
                //        }

                //        //lbDept.AddItems(deptListTemp);
                //    }
                //}
                #endregion

                #region 手工方，修改上面处方是，执行科室未赋值
                if (feeItemList.FTSource == "0" && (feeItemList.ExecOper.Dept.ID == "" || feeItemList.ExecOper.Dept.ID == "无") && string.IsNullOrEmpty(this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text))
                {
                    feeItemList.ExecOper.Dept.ID = this.rInfo.DoctorInfo.Templet.Dept.ID;
                    feeItemList.ExecOper.Dept.Name = this.rInfo.DoctorInfo.Templet.Dept.Name;
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = this.rInfo.DoctorInfo.Templet.Dept.Name;
                }
                #endregion
                #region 医技划价,且执行科室未赋值

                if (feeItemList.FTSource == "2" && (feeItemList.ExecOper.Dept.ID == "" || "无".Equals(feeItemList.ExecOper.Dept.ID)))
                {
                    feeItemList.ExecOper.Dept.ID = curOperDept.ID;
                    feeItemList.ExecOper.Dept.Name = curOperDept.Name;
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = curOperDept.Name;
                }

                #endregion


                #endregion

                #region 单价
                try
                {
                    if (this.rInfo != null)
                    {
                        if (this.isTransferTreat == true)
                        {
                            string priceForm = this.rInfo.Pact.PriceForm;
                            decimal unitPrice = NConvert.ToDecimal(findRow["UNIT_PRICE"]);
                            decimal childPrice = NConvert.ToDecimal(findRow["CHILD_PRICE"]);
                            decimal SPPrice = NConvert.ToDecimal(findRow["SP_PRICE"]);
                            decimal purchasePrice = NConvert.ToDecimal(findRow["PURCHASE_PRICE"]);

                            feeItemList.Item.ChildPrice = unitPrice;
                            decimal orgPrice = unitPrice;
                            feeItemList.OrgPrice = unitPrice;

                            //feeItemList.FT.TotCost

                            #region 转诊费用计算
                            if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")
                            {
                                if (SPPrice == 0m || SPPrice.ToString() == "")
                                {
                                    price = unitPrice;
                                }
                                else
                                {
                                    price = unitPrice * SPPrice;
                                }
                            }

                            if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")
                            {
                                Neusoft.HISFC.Models.Pharmacy.Item item = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                                //中成药，中草药，二类疫苗 不走零差价
                                if (item.Type.ID.ToString() == "C")// || item.ExtendData2.ToString() == "Y")
                                {
                                    if (item.PriceCollection.RetailPrice != 0)
                                    {
                                        price = item.PriceCollection.RetailPrice;
                                    }
                                    else
                                    {
                                        price = unitPrice;
                                    }
                                }
                                else
                                {
                                    if (purchasePrice != 0)
                                    {
                                        price = purchasePrice;
                                    }
                                    else
                                    {
                                        decimal retailPrice2 = item.RetailPrice2;
                                        if (retailPrice2 == 0)
                                        {
                                            price = unitPrice;
                                        }
                                        else
                                        {
                                            price = retailPrice2;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                price = unitPrice;
                            }
                            #endregion

                        }
                        else
                        {
                            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
                            int age = (int)((new TimeSpan(nowTime.Ticks - this.rInfo.Birthday.Ticks)).TotalDays / 365);

                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
                            string priceForm = this.rInfo.Pact.PriceForm;
                            decimal unitPrice = NConvert.ToDecimal(findRow["UNIT_PRICE"]);
                            decimal childPrice = NConvert.ToDecimal(findRow["CHILD_PRICE"]);
                            decimal SPPrice = NConvert.ToDecimal(findRow["SP_PRICE"]);
                            decimal purchasePrice = NConvert.ToDecimal(findRow["PURCHASE_PRICE"]);

                            // 保存原始默认价格
                            feeItemList.Item.ChildPrice = unitPrice;
                            decimal orgPrice = unitPrice;
                            price = this.feeIntegrate.GetPrice(feeItemList.Item.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice);
                            feeItemList.OrgPrice = orgPrice;
                        }

                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    this.isDealCellChange = true;

                    return;
                }

                ////判断优惠价格
                //Neusoft.HISFC.Models.Base.PactItemRate pRate = Class.Function.PactRate(this.rInfo, feeItemList, ref errText);

                //if (pRate == null)
                //{
                //    MessageBox.Show(Language.Msg(errText));
                //    this.fpSpread1_Sheet1.SetActiveCell(row, 0);
                //    this.isDealCellChange = true;

                //    return;
                //}
                //price *= 1-pRate.Rate.RebateRate;
                //------


                //包装数量，非药品，组合项目为1
                pactQty = NConvert.ToDecimal(findRow["PACK_QTY"].ToString());
                feeItemList.Item.PackQty = pactQty;
                if (pactQty == 0)
                {
                    MessageBox.Show(Language.Msg("该项目没有维护包装数量!"));
                    this.fpSpread1_Sheet1.SetActiveCell(row, 0);
                    this.isDealCellChange = true;

                    return;
                }

                #region 收费单位

                FarPoint.Win.Spread.CellType.ComboBoxCellType unitCell = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                unitCell.Editable = true;
                //默认单位为天
                minUnit = findRow["MIN_UNIT"].ToString();
                if (minUnit == string.Empty)
                {
                    minUnit = "天";
                }
                packUnit = findRow["PACK_UNIT"].ToString();
                if (packUnit == string.Empty)
                {
                    packUnit = "天";
                }
                unitCell.Items = new string[] { minUnit, packUnit };
                unitCell.Editable = true;

                this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].CellType = unitCell;

                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Amount].Text = amount.ToString();

                if (unitFlag == "1" || (feeItemList.Item.ItemType == EnumItemType.Drug && this.isShowMinPact && feeItemList.Item.SysClass.ID.ToString() != "PCC" && feeItemList.Invoice.User01 != "1"))//最小单位
                {
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = minUnit;
                    feeItemList.Item.PriceUnit = minUnit;
                    feeItemList.FeePack = "0";
                    feeItemList.Item.Qty = amount;
                    //给单价cell付值,默认最小单位
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber((price / pactQty), 4);
                }
                else if (unitFlag == "2") //包装单位
                {
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = packUnit;
                    feeItemList.Item.PriceUnit = packUnit;
                    feeItemList.FeePack = "1";
                    feeItemList.Item.Qty = amount * feeItemList.Item.PackQty;
                    //给单价cell付值,默认最小单位
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = price;
                    if (feeItemList.Item.SysClass.ID.ToString() == "PCC")//草药一直是最小单位
                    {
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = minUnit;
                        feeItemList.Item.PriceUnit = minUnit;
                        feeItemList.FeePack = "0";
                        feeItemList.Item.Qty = 1;
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber((price / pactQty), 4);
                    }
                }
                else//未知单位,取默认
                {

                    if (this.defaultPriceUnit == "0")//最小单位
                    {
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = minUnit;
                        feeItemList.Item.PriceUnit = minUnit;
                        feeItemList.FeePack = "0";
                        feeItemList.Item.Qty = amount;
                        //给单价cell付值,默认最小单位
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber((price / pactQty), 4);
                    }
                    else //包装单位
                    {
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = packUnit;
                        feeItemList.Item.PriceUnit = packUnit;
                        feeItemList.FeePack = "1";
                        feeItemList.Item.Qty = amount * feeItemList.Item.PackQty;
                        //给单价cell付值,默认最小单位
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = price;
                        //草药一直是最小单位,但是协定处方,按照西药的方式处理
                        if (feeItemList.Item.SysClass.ID.ToString() == "PCC" && !(feeItemList.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                        {
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = minUnit;
                            feeItemList.Item.PriceUnit = minUnit;
                            feeItemList.FeePack = "0";
                            feeItemList.Item.Qty = 1;
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber((price / pactQty), 4);
                        }
                    }
                }
                //if (feeItemList.Item.IsPharmacy)
                if (feeItemList.Item.ItemType == EnumItemType.Drug && feeItemList.Item.SysClass.ID.ToString() != "PCC")
                {
                    //add by cao-lin
                    if (feeItemList.Invoice.User01 == "1" || feeItemList.Invoice.User01 == "3")// 不能拆分包装单位
                    {

                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Text = packUnit;
                        feeItemList.Item.PriceUnit = packUnit;
                        feeItemList.FeePack = "1";
                        feeItemList.Item.Qty = amount * feeItemList.Item.PackQty;
                        //给单价cell付值,默认最小单位
                        //this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = price;
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber((price), 4);
                    }
                }

                #endregion
                //保存原始单价(包装单位)
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Tag = price;
                //包装单位单价，保留4位小数
                price = Neusoft.FrameWork.Public.String.FormatNumber(price, 4);
                feeItemList.Item.Price = price;
                feeItemList.SpecialPrice = price;


                #endregion

                this.fpSpread1_Sheet1.Cells[row, (int)Columns.FeeCode].Value = findRow["FEE_CODE"].ToString();
                feeItemList.Item.MinFee.ID = findRow["FEE_CODE"].ToString();
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.ItemType].Value = findRow["DRUG_FLAG"].ToString();
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.ItemCode].Value = findRow["ITEM_CODE"].ToString();

                feeItemList.RecipeSequence = this.recipeSeq;

                feeItemList.Patient = this.rInfo.Clone();

                this.SetItemRateInfo(row, feeItemList);

                #region 判断适应症
                //适应症接口{01DD7186-50F0-40fb-A91E-02A1A8358A83}
                Neusoft.HISFC.BizProcess.Interface.FeeInterface.IAdptIllnessOutPatient iAdptIllnessOutPatient = null;
                iAdptIllnessOutPatient = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IAdptIllnessOutPatient)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IAdptIllnessOutPatient;
                if (iAdptIllnessOutPatient != null)
                {
                    int returnValue = iAdptIllnessOutPatient.ProcessOutPatientFeeDetail(this.PatientInfo, ref feeItemList);
                    if (returnValue < 0)
                    {
                        return;
                    }

                }

                #endregion
                ArrayList hsZTNOREOnlyOneItem = new ArrayList();
                ArrayList hsNOREOnlyOneItem = new ArrayList();
                Hashtable hsREOnlyOneItem = new Hashtable();
                int number = 1;
                int returnRows= 0;//是否为限制收费药品
                decimal LimitNumber = 1;
                ArrayList hsREOnlylistItem = new ArrayList();
                string Discount_type = "1";//限制收费类型
                int TOPPRICE = 0;
                decimal DISCOUNT_RATE = 0;
                returnRows = this.undrugManager.SetRestrictingfee(feeItemList.Item.ID, ref  LimitNumber);
                Discount_type = this.undrugManager.SetDiscountfee(feeItemList.Item.ID, ref  DISCOUNT_RATE, ref  TOPPRICE);
                if (returnRows > 0 && this.rInfo.DoctorInfo.Templet.Dept.ID != "7021")
                {
                    this.setRestrictingfee.ConvertRestrictingfeeCharge(PatientInfo.PID.CardNO, feeItemList, ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem, number, LimitNumber, ref hsZTNOREOnlyOneItem, this.dsItem,this.rInfo);
                    foreach (FeeItemList ds in hsREOnlylistItem)
                    {
                        feeItemList.FT.TotCost = ds.FT.TotCost;
                        feeItemList.FT.OwnCost = ds.FT.OwnCost;
                    }
                }
                if (Discount_type == "2")
                {
                    this.setRestrictingfee.ConvertDiscountfee(feeItemList, DISCOUNT_RATE, TOPPRICE, ref hsREOnlyOneItem, ref hsREOnlylistItem, number);
                }
                this.fpSpread1_Sheet1.Rows[row].Tag = feeItemList;


                #region 优惠比例 add by zuowy

                //Neusoft.HISFC.Models.Fee.Outpatient..EcoRate ecoRate = new EcoRate();
                //ecoRate.RateType.ID = this.rInfo.User02;

                //if (this.rInfo.User02 == "NO" || this.rInfo.User02 == null || this.rInfo.User02 == string.Empty)
                //{
                //    ecoRate.Rate.RebateRate = 100;
                //}
                //else
                //{
                //    ecoRate.Item.ID = ((FeeItemList)this.fpSpread1_Sheet1.Rows[row].Tag).ID;
                //    //int iReturn = this.ecoRateManager.GetRate(ecoRate, false);
                //    ecoRate.Rate.RebateRate = 100;
                //    int varReturn = this.ecoRateManager.GetRateByItem(ecoRate);
                //    if (varReturn == -1)
                //    {
                //        MessageBox.Show(this.ecoRateManager.Err + "你所选择的优惠无效!");
                //    }
                //    else if (varReturn == 0)
                //    {
                //        DataRow findRowAgain;
                //        DataRow[] rowFindsAgain = this.dvItem.Table.Select("ITEM_CODE = " + "'" + ecoRate.Item.ID + "'");

                //        if (rowFinds != null && rowFinds.Length > 0)
                //        {
                //            findRowAgain = rowFindsAgain[0];

                //            string feeCode = findRowAgain["FEE_CODE"].ToString();

                //            ecoRate.Item.ID = feeCode;

                //            varReturn = this.ecoRateManager.GetRateByMinFee(ecoRate);

                //            if (varReturn == -1)
                //            {
                //                MessageBox.Show(this.ecoRateManager.Err + "你所选择的优惠无效!");
                //            }
                //        }
                //    }
                //}

                //Neusoft.FrameWork.Public.String.FormatNumber(((FeeItemList)this.fpSpread1_Sheet1.Rows[row].Tag).Price =
                //    ((FeeItemList)this.fpSpread1_Sheet1.Rows[row].Tag).Price1 * ecoRate.Rate.RebateRate / 100, 4);
                //Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(feeItemList.Price,
                //    0, feeItemList);

                //((FeeItemList)this.fpSpread1_Sheet1.Rows[row].Tag).FT.TotCost = ft.TotCost;


                //if (feeItemList.FeePack == "1")
                //{
                //    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = feeItemList.Price;
                //}
                //else
                //{
                //    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Price].Value = feeItemList.Price / feeItemList.PackQty;
                //}
                //this.fpSpread1_Sheet1.Cells[row, (int)Columns.Cost].Value = ft.TotCost;

                #endregion

                this.SetColumnEnable(row);
            }
            RefreshItemInfo();
            this.isDealCellChange = true;
        }

        /// <summary>
        /// 项目信息
        /// </summary>
        private void RefreshItemInfo()
        {
            int row = this.fpSpread1_Sheet1.ActiveRowIndex;
            if (this.fpSpread1_Sheet1.Rows[row].Tag != null)
            {
                if (this.fpSpread1_Sheet1.Rows[row].Tag is FeeItemList)
                {
                    FeeItemList f = this.fpSpread1_Sheet1.Rows[row].Tag as FeeItemList;
                    string siType = string.Empty;
                    decimal siRate = 0;
                    if (f.FeePack == "1")
                    {
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Locked = true;
                    }
                    else
                    {
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.PriceUnit].Locked = false;
                    }
                    ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.InputCode].Text = f.Item.UserCode;


                    this.SetItemRateInfo(row, f);

                    this.rightControl.SetSingleFeeItemInfomation(f);
                }
            }
        }

        /// <summary>
        /// 项目信息
        /// </summary>
        /// <param name="row"></param>
        private void RefreshItemInfo(int row)
        {
            if (this.fpSpread1_Sheet1.Rows[row].Tag != null)
            {
                if (this.fpSpread1_Sheet1.Rows[row].Tag is FeeItemList)
                {
                    FeeItemList f = this.fpSpread1_Sheet1.Rows[row].Tag as FeeItemList;
                    string siType = string.Empty;
                    decimal siRate = 0;
                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.InputCode].Text = f.Item.UserCode;


                    this.SetItemRateInfo(row, f);

                    this.rightControl.SetSingleFeeItemInfomation(f);

                }
            }
        }

        /// <summary>
        /// 计算费用
        /// </summary>
        /// <param name="price">单价</param>
        /// <param name="qty">数量</param>
        /// <param name="f">当前收费项目信息</param>
        /// <returns>成功返回FT信息,失败null</returns>
        private Neusoft.HISFC.Models.Base.FT ComputCost(decimal price, decimal qty, FeeItemList f)
        {
            Neusoft.HISFC.Models.Base.FT ft = new Neusoft.HISFC.Models.Base.FT();

            if (this.rInfo.Pact.PayKind.ID == "01")//自费
            {
                //if (f.FT.RebateCost > 0)
                //{
                //    ft = f.FT.Clone();
                //}
                //else
                //{

                if (myITruncFee != null)
                {
                    object[] args = new object[] { ft, f };
                    ft.TotCost = ((Neusoft.HISFC.Models.Base.FT)(myITruncFee.TruncFee(args))[0]).TotCost;
                }
                else
                {
                    ft.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(f.Item.Price * f.Item.Qty / f.Item.PackQty, 2);
                }

                if (ft.TotCost > 999999)
                {
                    MessageBox.Show(Language.Msg("金额不能超过999999请调整项目的数量!"));

                    return null;
                }

                ft.PayCost = 0;
                ft.PubCost = 0;
                ft.OwnCost = ft.TotCost;
                if (myITruncFee != null)
                {
                    object[] args = new object[] { ft, f };
                    ft.RebateCost = ((Neusoft.HISFC.Models.Base.FT)(myITruncFee.TruncFee(args))[0]).RebateCost;
                }
                else
                {
                    //add by Niuxy修改减免  
                    ft.RebateCost = Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost * f.Item.Qty / f.Item.PackQty, 2);
                }

                //}
            }
            if (this.rInfo.Pact.PayKind.ID == "02")//医保
            {
                //if (f.FT.RebateCost > 0)
                //{
                //    ft = f.FT.Clone();
                //}
                //else
                //{
                if (myITruncFee != null)
                {
                    object[] args = new object[] { ft, f };
                    ft.TotCost = ((Neusoft.HISFC.Models.Base.FT)(myITruncFee.TruncFee(args))[0]).TotCost;
                }
                else
                {
                    ft.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(f.Item.Price * f.Item.Qty / f.Item.PackQty, 2);
                }

                if (ft.TotCost > 999999)
                {
                    MessageBox.Show(Language.Msg("金额不能超过999999请调整项目的数量!"));

                    return null;
                }

                ft.PayCost = 0;
                ft.PubCost = 0;
                ft.OwnCost = ft.TotCost;
                //add by Niuxy修改减免  
                if (myITruncFee != null)
                {
                    object[] args = new object[] { ft, f };
                    ft.RebateCost = ((Neusoft.HISFC.Models.Base.FT)(myITruncFee.TruncFee(args))[0]).RebateCost;
                }
                else
                {
                    //add by Niuxy修改减免  
                    ft.RebateCost = Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost * f.Item.Qty / f.Item.PackQty, 2);
                }
                //}

            }
            if (this.rInfo.Pact.PayKind.ID == "03")//公费
            {
                if (f.FT.RebateCost > 0)
                {
                    ft = f.FT.Clone();
                }
                else if (f.IsGroup)
                {
                    if (myITruncFee != null)
                    {
                        object[] args = new object[] { ft, f };
                        ft.TotCost = ((Neusoft.HISFC.Models.Base.FT)(myITruncFee.TruncFee(args))[0]).TotCost;
                    }
                    else
                    {
                        ft.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(f.Item.Price * f.Item.Qty / f.Item.PackQty, 2);
                    }

                    if (ft.TotCost > 999999)
                    {
                        MessageBox.Show(Language.Msg("金额不能超过999999请调整项目的数量!"));

                        return null;
                    }

                    ft.OwnCost = ft.TotCost;
                }
                else
                {
                    if (myITruncFee != null)
                    {
                        object[] args = new object[] { ft, f };
                        ft.TotCost = ((Neusoft.HISFC.Models.Base.FT)(myITruncFee.TruncFee(args))[0]).TotCost;
                    }
                    else
                    {
                        ft.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(f.Item.Price * f.Item.Qty / f.Item.PackQty, 2);
                    }

                    if (ft.TotCost > 999999)
                    {
                        MessageBox.Show(Language.Msg("金额不能超过999999请调整项目的数量!"));

                        return null;
                    }
                    if (this.rInfo.Pact.Name == "公费")
                    {
                        Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;
                        pactRate = this.pactUnitItemRateManager.GetOnePactUnitItemRateGY(rInfo.Name, rInfo.IDCard, 1);

                        if (sumPubCost < 200)
                        {
                            string Fee_Type = this.pactUnitItemRateManager.GetGYCOMCOMPARE(f.Item.ID);
                            if (Fee_Type == "gf")
                            {
                                ft.OwnCost = Convert.ToDecimal(f.Item.Price * pactRate.Rate.PayRate) * qty;
                                ft.PubCost = Convert.ToDecimal(f.Item.Price * pactRate.Rate.PubRate) * qty;
                                ft.OwnCost = decimal.Round(ft.OwnCost, 2);
                                ft.PubCost = decimal.Round(ft.PubCost, 2);
                                sumPubCost += ft.PubCost;
                                ft.TotCost = ft.PubCost + ft.OwnCost;
                                if (sumPubCost > 200)
                                {
                                    ft.OwnCost = ft.OwnCost + (sumPubCost - 200);
                                    ft.PubCost = ft.PubCost - (sumPubCost - 200);
                                    MessageBox.Show(Language.Msg("报销金额超出限额，超出部分将按照自费进行收取!"));
                                    //MessageBox.Show("报销金额超出限额，超出部分将按照自费进行收取");
                                }

                            }
                            else
                            {
                                ft.OwnCost = ft.TotCost;
                            }
                        }
                        else
                        {
                            ft.OwnCost = ft.TotCost;
                        }
                    }
                    else
                    {
                        ft.OwnCost = ft.TotCost;
                    }
                    //修改[gmz]2011-10-06
                    //f.FT.TotCost = ft.TotCost;

                }
            }

            return ft;
        }

        /// <summary>
        /// 计算当前收费总额，暂时是全自费处理，以后考虑医保和公费合同单位
        /// </summary>
        protected virtual decimal SumCost()
        {
            decimal sumCost = 0;
            //获得所有项目信息,包括组合项目的明细
            ArrayList alFee = this.GetFeeItemList();
            //{5E8AC557-3442-42c5-8E12-86331BDAB453}
            if (rightControl != null)
            {
                //妇幼要求不实时显示，只显示患者的姓名以及收费按钮按后显示总额。
                //以后整合。  xingz
                this.rightControl.SetInfomation(this.rInfo, null, alFee, null, "0");
            }
            if (this.leftControl != null)
            {
                this.leftControl.PatientInfo = this.rInfo;
                this.leftControl.RefreshDisplayInfomation(alFee);
            }

            ArrayList alCharge = this.GetFeeItemListForCharge();
            //{BBE9766A-A539-485e-A03B-9972DC675538} 退费补收
            if (this.FeeItemListChanged != null)
            {
                this.FeeItemListChanged(alCharge);
            }
            //{BBE9766A-A539-485e-A03B-9972DC675538} 结束
            this.SumLittleCostAll();

            return sumCost;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ArrayList ConvertGroupToDetail(FeeItemList f)
        {
            ArrayList undrugCombList = this.undrugPackAgeManager.QueryUndrugPackagesBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + undrugPackAgeManager.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            decimal pricesz = 0;
            decimal pricece = 0;
            this.undrugManager.GetPricesz("F00000010769", ref pricesz);//获取加收项目价格
            this.undrugManager.GetPricesz("F00000010768", ref pricece);//获取加收项目价格
            pricece = pricece - pricesz;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.orderIntegrate.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";

                    return null;
                }
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.outpatientManager.GetAge(this.rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = this.rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.feeIntegrate.GetPrice(f.Item.ID, this.rInfo, age, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {

                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    if (isTransferTreat == true)
                    {
                        decimal orgPrice = price;
                        itemRate = 1;// feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                        price = unitPrice;// this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                        feeDetail.OrgPrice = orgPrice;
                    }
                    else
                    {
                        decimal orgPrice = price;
                        itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                        price = this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                        feeDetail.OrgPrice = orgPrice;
                    }
                }
                catch (Exception e)
                {
                    this.errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //中五打折不需要用计算的rate
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = Function.PactRate(this.rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    this.errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                if (this.undrugManager.SetUltrasound(f.Item.ID))
                {
                    if (feeDetail.Item.Name == "四肢血管彩色多普勒超声")
                    {
                        if (itenqty > 0)
                        {
                            feeDetail.Item.Name = "四肢血管彩色多普勒超声加收(每增加两根血管)";
                            feeDetail.Item.ID = "F00000010769";
                            price = price - pricece;
                        }
                        itenqty = itenqty + 1;
                    }
                }
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;

                if (this.rInfo.Pact.Name == "公费")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;
                    pactRate = this.pactUnitItemRateManager.GetOnePactUnitItemRateGY(rInfo.Name, rInfo.IDCard, 1);
                    if (sumPubCost < 200)
                    {
                        string Fee_Type = this.pactUnitItemRateManager.GetGYCOMCOMPARE(feeDetail.Item.ID);
                        if (Fee_Type == "gf")
                        {
                            feeDetail.FT.OwnCost = Convert.ToDecimal(feeDetail.Item.Price * pactRate.Rate.PayRate) * count;
                            feeDetail.FT.PubCost = Convert.ToDecimal(feeDetail.Item.Price * pactRate.Rate.PubRate) * count;
                            feeDetail.FT.OwnCost = decimal.Round(feeDetail.FT.OwnCost, 2);
                            feeDetail.FT.PubCost = decimal.Round(feeDetail.FT.PubCost, 2);
                            sumPubCost += feeDetail.FT.PubCost;
                            if (sumPubCost > 200)
                            {
                                feeDetail.FT.OwnCost = feeDetail.FT.OwnCost + (sumPubCost - 200);
                                feeDetail.FT.PubCost = feeDetail.FT.PubCost - (sumPubCost - 200);
                                //MessageBox.Show("报销金额超出限额，超出部分将按照自费进行收取");
                            }
                        }
                    }
                }
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (this.rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (this.rInfo.Pact.PayKind.ID != "01")
                    {
                        MessageBox.Show(Language.Msg("暂时不允许非自费患者减免!"));

                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <param name="isFirst"></param>
        /// <param name="hsOnlyOneItem"></param>
        /// <param name="drCount"></param>
        /// <returns></returns>
        private ArrayList ConvertDRGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem, ref decimal drCount)
        {
            ArrayList undrugCombList = this.undrugPackAgeManager.QueryUndrugZTBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + undrugPackAgeManager.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.orderIntegrate.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";

                    return null;
                }
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.outpatientManager.GetAge(this.rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = this.rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.feeIntegrate.GetPrice(f.Item.ID, this.rInfo, age, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                if (isFirst && undrugCombo.SortID == 2)
                {
                    //如果是第一个DR项目，并且细项是第二组起收的继续循环
                    continue;
                }
                else if (!isFirst && undrugCombo.SortID == 1)
                {
                    //如果不是第一个DR项目，并且细项是第一组收的继续循环
                    continue;
                }
                if (undrugCombo.SpellCode != "0")
                {
                    DataRow rowFindZT;
                    DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                    rowFindZT = rowFindZTs[0];
                    string itemName = rowFindZT["ITEM_NAME"].ToString();
                    if (itemName.ToUpper().Contains("DR"))
                    {
                        drCount += NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;
                    }
                }
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                if (isFirst && undrugCombo.SortID == 2)
                {
                    //如果是第一个DR项目，并且细项是第二组起收的继续循环
                    continue;
                }
                else if (!isFirst && undrugCombo.SortID == 1)
                {
                    //如果不是第一个DR项目，并且细项是第一组收的继续循环
                    continue;
                }
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    decimal orgPrice = price;
                    itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    price = this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    this.errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = Function.PactRate(this.rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    this.errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (this.rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                if (undrugCombo.SpellCode == "0")
                {
                    //总量取整的，做标识
                    if (hsOnlyOneItem.ContainsKey(feeDetail.Item.ID))
                    {
                        FeeItemList temp = hsOnlyOneItem[feeDetail.Item.ID] as FeeItemList;
                        //temp.UndrugComb.User02 = (Neusoft.FrameWork.Function.NConvert.ToInt32(temp.UndrugComb.User02) + 1).ToString();
                        //if (Neusoft.FrameWork.Function.NConvert.ToInt32(temp.UndrugComb.User02) % 2 != 0)
                        //{
                        //    temp.Item.Qty += feeDetail.Item.Qty;
                        //    temp.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(temp.Item.Qty));
                        //    temp.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Price * temp.Item.Qty, 2);
                        //    temp.FT.OwnCost = temp.FT.TotCost;
                        //}
                        //temp.Item.Qty += feeDetail.Item.Qty;
                        //temp.FT.TotCost += feeDetail.FT.TotCost;
                        //temp.FT.OwnCost += feeDetail.FT.OwnCost;

                        temp.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        temp.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Price * temp.Item.Qty, 2);
                        temp.FT.OwnCost = temp.FT.TotCost;
                    }
                    else
                    {
                        //feeDetail.UndrugComb.User02 = "1";

                        feeDetail.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        feeDetail.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(feeDetail.Item.Price * feeDetail.Item.Qty, 2);
                        feeDetail.FT.OwnCost = feeDetail.FT.TotCost;

                        hsOnlyOneItem.Add(feeDetail.Item.ID, feeDetail);
                    }
                }

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (this.rInfo.Pact.PayKind.ID != "01")
                    {
                        MessageBox.Show(Language.Msg("暂时不允许非自费患者减免!"));

                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ArrayList ConvertCTGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem)
        {
            ArrayList undrugCombList = this.undrugPackAgeManager.QueryUndrugZTBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + undrugPackAgeManager.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.orderIntegrate.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";

                    return null;
                }
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.outpatientManager.GetAge(this.rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = this.rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.feeIntegrate.GetPrice(f.Item.ID, this.rInfo, age, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                #region pacs项目收费新模式

                if (undrugCombo.SortID == 3)
                {
                    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                    {
                        continue;
                    }
                    else
                    {
                        string itemName = rowFindZT["ITEM_NAME"].ToString();
                        if (itemName.Contains("三维重建"))
                        {
                            if (!hsOnlyOneItem.ContainsValue("四维"))
                            {
                                hsOnlyOneItem.Add(undrugCombo.ID, "三维");
                            }
                            else
                            {
                                hsOnlyOneItem.Add(undrugCombo.ID, "true");
                            }
                        }
                        else if (itemName.Contains("四维重建"))
                        {
                            Hashtable hsTemp = hsOnlyOneItem.Clone() as Hashtable;
                            foreach (DictionaryEntry de in hsTemp)
                            {
                                if (de.Value.ToString() == "三维")
                                {
                                    hsOnlyOneItem.Remove(de.Key);
                                    hsOnlyOneItem.Add(de.Key.ToString(), "true");
                                }
                            }
                            hsOnlyOneItem.Add(undrugCombo.ID, "四维");
                        }
                        else
                        {
                            hsOnlyOneItem.Add(undrugCombo.ID, "其他");
                        }
                    }
                }

                #endregion
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                //if (undrugCombo.SortID == 3)
                //{
                //    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                //    {
                //        if (hsOnlyOneItem[undrugCombo.ID].ToString() != "true")
                //        {
                //            hsOnlyOneItem.Remove(undrugCombo.ID);
                //            hsOnlyOneItem.Add(undrugCombo.ID, "true");
                //        }
                //        else
                //        {
                //            continue;
                //        }
                //    }
                //    else
                //    {
                //        continue;
                //    }
                //}
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    decimal orgPrice = price;
                    itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    price = this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    this.errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = Function.PactRate(this.rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    this.errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (this.rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (this.rInfo.Pact.PayKind.ID != "01")
                    {
                        MessageBox.Show(Language.Msg("暂时不允许非自费患者减免!"));

                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        /// <summary>
        /// 判断执行科室为情况
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int JudegExeDept()
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                //{CA82280B-51B6-4462-B63E-43F4ECF456A3}
                FeeItemList f = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                if (f != null)
                {
                    //this.SetExecDept(f.Item.ID);
                    this.SetExecDept(i, f);
                }

                if (this.fpSpread1_Sheet1.Cells[i, (int)Columns.ExeDept].Text == string.Empty ||
                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.ExeDept].Text == "无")
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                    {
                        if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                        {
                            this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.ExeDept);

                            return -1;
                        }
                    }

                }
            }

            return 1;
        }

        /// <summary>
        /// 验证是否输入项目
        /// </summary>
        /// <param name="row">当前行</param>
        /// <param name="f">项目实体</param>
        /// <returns>成功 true 失败 false</returns>
        private bool IsInputItem(int row, ref FeeItemList f)
        {
            if (this.fpSpread1_Sheet1.Rows[row].Tag == null)
            {
                MessageBox.Show(Language.Msg("请先输入项目"));
                this.fpSpread1.Focus();
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.SetActiveCell(row, (int)Columns.InputCode);

                return false;
            }
            if (this.fpSpread1_Sheet1.Rows[row].Tag is FeeItemList)
            {
                f = this.fpSpread1_Sheet1.Rows[row].Tag as FeeItemList;
            }
            else
            {
                MessageBox.Show(Language.Msg("请先输入项目"));
                this.fpSpread1.Focus();
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.SetActiveCell(row, (int)Columns.InputCode);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新相同组合号的药品的院注次数为同一
        /// </summary>
        /// <param name="combNo">组合号</param>
        /// <param name="injects">注射次数</param>
        private void RefreshSameCombNoInjects(string combNo, int injects)
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        FeeItemList f = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                        //if (f.Item.IsPharmacy && f.Order.Combo.ID == combNo)
                        if (f.Item.ItemType == EnumItemType.Drug && f.Order.Combo.ID == combNo)
                        {
                            f.InjectCount = injects;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查找相同组合得院注次数
        /// </summary>
        /// <param name="combNo"></param>
        /// <returns></returns>
        private int GetInjectSameCombs(string combNo)
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                FeeItemList feeItem = null;

                if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        feeItem = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                        if (feeItem.Order.Combo.ID == combNo)
                        {
                            if (feeItem.InjectCount > 0)
                            {
                                return feeItem.InjectCount;
                            }
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 使相同组合的频次或用法相同
        /// </summary>
        /// <param name="currRow">当前行</param>
        /// <param name="combNO">组合号</param>
        /// <param name="obj">变更实体</param>
        /// <param name="type">类型</param>
        private void DealFreqOrUsageHaveSameCombNo(int currRow, string combNO, NeuObject obj, string type)
        {
            if (combNO == null || combNO.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                FeeItemList feeItem = null;

                if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        feeItem = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                        if (feeItem.Order.Combo.ID == combNO && i != currRow)
                        {
                            if (type == "1")
                            {
                                feeItem.Order.Frequency.ID = obj.ID;
                                feeItem.Order.Frequency.Name = obj.Name;
                                if (freqDisplayType == "0")//汉字
                                {
                                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.Freq].Text = obj.Name;
                                }
                                else
                                {
                                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.Freq].Text = obj.ID;
                                }
                            }
                            else
                            {
                                feeItem.Order.Usage.ID = obj.ID;
                                feeItem.Order.Usage.Name = obj.Name;
                                this.fpSpread1_Sheet1.Cells[i, (int)Columns.Usage].Text = feeItem.Order.Usage.Name;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 验证数据是否输入合法
        /// </summary>
        /// <param name="row">当前行</param>
        /// <param name="col">当前列</param>
        /// <param name="colName">列名字</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="currValue">返回的当前输入值</param>
        /// <param name="showErr">是否显示错误</param>
        /// <returns>true合法 false不合法</returns>
        private bool InputDataIsValid(int row, int col, string colName, decimal maxValue, decimal minValue, ref decimal currValue, bool showErr)
        {
            if (this.fpSpread1_Sheet1.Cells[row, col].Text.ToString() == string.Empty)
            {
                currValue = 0m;
            }
            else
            {
                try
                {
                    currValue = NConvert.ToDecimal(
                        Neusoft.FrameWork.Public.String.ExpressionVal(
                        this.fpSpread1_Sheet1.Cells[row, col].Text.ToString()));
                }
                catch
                { }
            }
            if (currValue < minValue)
            {
                MessageBox.Show(colName + Language.Msg("的值不能小于") + minValue.ToString() + "!");
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(row, col);

                return false;
            }
            if (currValue > maxValue)
            {
                MessageBox.Show(colName + Language.Msg("的值不能大于") + maxValue.ToString() + "!");
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(row, col);

                return false;
            }

            return true;
        }

        private bool IsInputValid()
        {
            this.isDealCellChange = false;

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null && this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    decimal qty = 0;
                    //判断数量
                    try
                    {
                        qty = NConvert.ToDecimal(Neusoft.FrameWork.Public.String.ExpressionVal(this.fpSpread1_Sheet1.Cells[i, (int)Columns.Amount].Text.ToString()));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Language.Msg("输入的计算公式不正确，请重新输入!") + ex.Message);
                        this.fpSpread1.Focus();
                        this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Amount);

                        this.isDealCellChange = true;

                        return false;
                    }

                    qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);

                    //如果可以输入负数量,这里不判断
                    //为收费补差价服务{0F98A513-A9EA-4110-B35F-E353A390E350}
                    if (!this.isCanInputNegativeQty)
                    {
                        if (qty <= 0)
                        {
                            MessageBox.Show(Language.Msg("数量不能小于或者等于零,请重新输入"));
                            this.fpSpread1.Select();
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Amount, false);

                            this.isDealCellChange = true;

                            return false;
                        }
                    }//{0F98A513-A9EA-4110-B35F-E353A390E350}结束

                    if (qty > 99999)
                    {
                        MessageBox.Show(Language.Msg("数量不能大于99999！请重新输入"));
                        this.fpSpread1.Select();
                        this.fpSpread1.Focus();
                        this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Amount, false);

                        this.isDealCellChange = true;

                        return false;
                    }


                    //判断付数

                    FeeItemList feeTemp = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                    //if (feeTemp.Item.IsPharmacy && feeTemp.Item.SysClass.ID.ToString() == "PCC")//草药
                    if (feeTemp.Item.ItemType == EnumItemType.Drug && feeTemp.Item.SysClass.ID.ToString() == "PCC" && !(feeTemp.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)//草药
                    {

                        decimal days = 0;

                        try
                        {
                            days = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, (int)Columns.Days].Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Language.Msg("输入的天数不合法") + ex.Message);
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Days);

                            this.isDealCellChange = true;

                            return false;
                        }
                        if (days <= 0)
                        {
                            MessageBox.Show(Language.Msg("输入的付数不合法, 付数必须大于0"));
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Days);

                            this.isDealCellChange = true;

                            return false;
                        }
                    }
                }
            }

            this.isDealCellChange = true;

            return true;
        }

        /// <summary>
        /// 判断库存是否不足
        /// </summary>
        /// <returns></returns>
        private bool IsStoreEnough(FeeItemList feeItem, int row)
        {
            //begin这里判断库存最好 zhouxs by 2007-10-17
            decimal storeSum = 0;
            decimal preStoreSum = 0;
            decimal storeSumTemp = 0;
            int iReturn = this.pharmacyIntegrate.GetStorageNum(feeItem.ExecOper.Dept.ID, feeItem.Item.ID, out storeSum);
            if (iReturn <= 0)
            {
                MessageBox.Show("查找库存失败!");
                return false;
            }

            #region 增加预扣库存判断
            if (this.isUsePreStore)
            {
                preStoreSum = this.GetPreSum(feeItem.ExecOper.Dept.ID, feeItem.Item.ID);
                storeSum = storeSum - preStoreSum;
            }
            #endregion

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                FeeItemList feeItem1 = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                if (feeItem1 != null)
                {

                    if (feeItem1.Item.ID == feeItem.Item.ID && feeItem1.ExecOper.Dept.ID == feeItem.ExecOper.Dept.ID)
                    {
                        storeSumTemp = storeSumTemp + feeItem1.Item.Qty;
                    }
                }
            }

            if (storeSum <= 0 || storeSum - storeSumTemp < 0)
            {
                if (feeItem.FeePack == "1")
                {
                    int outTemp = 0;

                    int store = Math.DivRem(NConvert.ToInt32(storeSum), NConvert.ToInt32(feeItem.Item.PackQty), out outTemp);

                    MessageBox.Show("当前库存数:" + store.ToString() +
                        (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).PackUnit + outTemp.ToString() + (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).MinUnit +
                        "|输入库存数:" + this.fpSpread1_Sheet1.Cells[row, 4].Value.ToString() + (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).PackUnit + "   库存不足!");
                }
                else
                {
                    MessageBox.Show("当前库存数:" + storeSum.ToString() + (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).MinUnit + "|输入库存数:" + this.fpSpread1_Sheet1.Cells[row, 4].Value.ToString() + (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).MinUnit + "   库存不足!");
                }
                this.fpSpread1_Sheet1.SetActiveCell(row, (int)Columns.Amount, true);
                //if (feeItem.FeePack == "1")
                //{
                //    this.fpSpread1_Sheet1.Cells[row, 3].Value = Neusoft.FrameWork.Function.NConvert.ToDecimal(storeSum / feeItem.Item.PackQty).ToString();
                //}
                //else
                //{
                //    //this.fpSpread1_Sheet1.Cells[row, 3].Value = storeSum;
                //}

                return false;
            }
            if (feeItem.User01 == "1")
            {
                MessageBox.Show("该项目已经缺药,不能选择!");
                return false;
            }
            return true;
            //end zhouxs
        }

        /// <summary>
        /// 根据药房，项目代码获取预扣数量
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        private decimal GetPreSum(string deptCode, string drugCode)
        {
            string strSql = @"select sum(t.apply_num) from pha_sto_preoutstore t where t.drug_dept_code = '{0}' and t.drug_code = '{1}'";
            strSql = string.Format(strSql, deptCode, drugCode);
            return Neusoft.FrameWork.Function.NConvert.ToDecimal(this.outpatientManager.ExecSqlReturnOne(strSql, "0"));
        }

        /// <summary>
        /// 控制是否可以选择收费{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
        /// </summary>
        private void SetIsCanSelectItemAndFee()
        {
            if (this.isCanSelectItemAndFee)
            {
                this.fpSpread1_Sheet1.Columns[(int)Columns.Select].Visible = true;
            }
            else
            {
                this.fpSpread1_Sheet1.Columns[(int)Columns.Select].Visible = false;
            }
        }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束

        #endregion

        #region 公有方法

        /// <summary>
        /// 修改合同单位后刷新项目信息.
        /// </summary>
        public void RefreshItemForPact()
        {
            this.isDealCellChange = false;
            for (int currRow = 0; currRow < this.fpSpread1_Sheet1.RowCount; currRow++)
            {
                if (this.fpSpread1_Sheet1.Rows[currRow].Tag != null && this.fpSpread1_Sheet1.Rows[currRow].Tag.GetType() == typeof(FeeItemList))
                {
                    EcoRate ecoRate = new EcoRate();
                    ecoRate = this.rInfo.EcoRate.Clone();

                    if (ecoRate.RateType.ID == null || ecoRate.RateType.ID == "NO" || ecoRate.RateType.ID == string.Empty)
                    {
                        ecoRate.Rate.RebateRate = 100;
                        //string errMsg = string.Empty;
                        //ecoRate.Rate = (Class.Function.PactRate(this.rInfo.Pact.ID, (FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag, ref errMsg)).Rate;

                        //ecoRate.Rate.RebateRate = (1 - ecoRate.Rate.RebateRate) * 100; 
                    }
                    else
                    {
                        ecoRate.Item.ID = ((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).ID;

                        ecoRate.Rate.RebateRate = 100;


                        int iReturn = this.ecoRateManager.GetRateByItem(ecoRate);

                        if (iReturn == -1)
                        {
                            MessageBox.Show(this.ecoRateManager.Err + Language.Msg("你所选择的优惠无效!"));
                        }
                        else if (iReturn == 0)
                        {
                            DataRow findRow;
                            DataRow[] rowFinds = this.dvItem.Table.Select("ITEM_CODE = " + "'" + ecoRate.Item.ID + "'");

                            if (rowFinds != null && rowFinds.Length > 0)
                            {
                                findRow = rowFinds[0];

                                string feeCode = findRow["FEE_CODE"].ToString();

                                ecoRate.Item.ID = feeCode;

                                iReturn = this.ecoRateManager.GetRateByMinFee(ecoRate);

                                if (iReturn == -1)
                                {
                                    MessageBox.Show(this.ecoRateManager.Err + Language.Msg("你所选择的优惠无效!"));
                                }
                            }
                        }
                    }

                    Neusoft.FrameWork.Public.String.FormatNumber(((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).Item.Price =
                        ((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).OrgPrice * ecoRate.Rate.RebateRate / 100, 4);
                    Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).Item.Price,
                        0, ((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag));

                    if (ft == null)
                    {
                        return;
                    }

                    ((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).FT.TotCost = ft.TotCost;

                    if (((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).FeePack == "1")
                    {
                        this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber(((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).Item.Price, 4);
                    }
                    else
                    {
                        this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Value = Neusoft.FrameWork.Public.String.FormatNumber(((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).Item.Price
                            / ((FeeItemList)this.fpSpread1_Sheet1.Rows[currRow].Tag).Item.PackQty, 4);
                    }
                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Cost].Value = ft.TotCost;
                    FeeItemList feeItemList = this.fpSpread1_Sheet1.Rows[currRow].Tag as FeeItemList;
                    this.SetItemRateInfo(currRow, feeItemList);
                }
            }
            SumCost();
            this.isDealCellChange = true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public int Init()
        {
            //校区异地转诊得手工选择项目收费 yhm 20210913
            if (Neusoft.FrameWork.Management.Connection.Hospital.ID == "CORE_HIS502")
            {
                isCanSelectItemAndFee = true;
            }
            if (this.InitControlParams() == -1)
            {
                MessageBox.Show("初始化参数列表出错!");

                return -1;
            }

            //获得操作员基本信息
            myOperator = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            if (myOperator == null)
            {
                MessageBox.Show("获得操作员基本信息出错!");

                return -1;
            }
            ArrayList alApprItem = this.managerIntegrate.GetConstantList("ApprItem");
            if (alApprItem != null)
            {
                this.apprItemHelper.ArrayObject = alApprItem;
            }

            ArrayList alPhaFeeCode = this.managerIntegrate.GetConstantList("DrugMinFee");
            if (alPhaFeeCode != null)
            {
                this.phaFeeCodeHelper.ArrayObject = alPhaFeeCode;
            }

            ArrayList alSpecialItem = this.managerIntegrate.GetConstantList("DrugRate");
            if (alSpecialItem != null)
            {
                this.specialItemHelper.ArrayObject = alSpecialItem;
            }
            ArrayList alInvertUnit = this.managerIntegrate.GetConstantList("InvertUnit");
            if (alInvertUnit != null)
            {
                this.invertUnitHelper.ArrayObject = alInvertUnit;
            }

            ArrayList alInvertUnitDrug = this.managerIntegrate.GetConstantList("InvertDrug");
            if (alInvertUnit != null)
            {
                this.specialInvertUnitHelper.ArrayObject = alInvertUnitDrug;
            }

            ArrayList alItemZT = this.consManager.GetAllList("ItemZT");
            if (alItemZT != null)
            {
                hsItemZT = new Hashtable();
                foreach (Neusoft.HISFC.Models.Base.Const conObj in alItemZT)
                {
                    Neusoft.FrameWork.Models.NeuObject obj = null;
                    if (!conObj.IsValid)
                    {
                        continue;
                    }
                    if (hsItemZT.ContainsKey(conObj.Name))
                    {
                        if (string.IsNullOrEmpty(conObj.Memo.Trim()))
                        {
                            continue;
                        }
                        string[] itemIDs = null;
                        //string[] temps = conObj.Memo.Split('&');

                        itemIDs = conObj.Memo.Split('|');
                        foreach (string itemID in itemIDs)
                        {
                            obj = new NeuObject();
                            obj.ID = itemID;
                            obj.Name = conObj.WBCode;//数量
                            switch (conObj.SortID.ToString())
                            {
                                case "0":
                                    obj.Memo = "每个项目收取";
                                    break;
                                case "1":
                                    obj.Memo = "第一个项目收取";
                                    break;
                                case "2":
                                    obj.Memo = "第二个项目起加收";
                                    break;
                                case "3":
                                    obj.Memo = "只收取一次";
                                    break;

                            }

                            //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                            switch (conObj.SpellCode)
                            {
                                case "0":
                                    obj.User01 = "总量取整";
                                    break;
                                case "1":
                                    obj.User01 = "单个取整";
                                    break;
                                case "2":
                                    obj.User01 = "固定数量";
                                    break;
                            }
                            //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                            switch (conObj.UserCode)
                            {
                                case "0":
                                    obj.User02 = "DR";
                                    break;
                                case "1":
                                    obj.User02 = "CT";
                                    break;
                            }
                            //obj.User02 = conObj.UserCode;//0 DR 1 CT

                            ((ArrayList)hsItemZT[conObj.Name]).Add(obj);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(conObj.Memo.Trim()))
                        {
                            continue;
                        }
                        ArrayList al = new ArrayList();
                        string[] itemIDs = null;
                        //string[] temps = conObj.Memo.Split('&');
                        itemIDs = conObj.Memo.Split('|');
                        foreach (string itemID in itemIDs)
                        {
                            obj = new NeuObject();
                            obj.ID = itemID;
                            obj.Name = conObj.WBCode;//数量
                            switch (conObj.SortID.ToString())
                            {
                                case "0":
                                    obj.Memo = "每个项目收取";
                                    break;
                                case "1":
                                    obj.Memo = "第一个项目收取";
                                    break;
                                case "2":
                                    obj.Memo = "第二个项目起加收";
                                    break;
                            }

                            //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                            switch (conObj.SpellCode)
                            {
                                case "0":
                                    obj.User01 = "总量取整";
                                    break;
                                case "1":
                                    obj.User01 = "单个取整";
                                    break;
                                case "2":
                                    obj.User01 = "固定数量";
                                    break;
                            }
                            //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                            switch (conObj.UserCode)
                            {
                                case "0":
                                    obj.User02 = "DR";
                                    break;
                                case "1":
                                    obj.User02 = "CT";
                                    break;
                            }
                            //obj.User02 = conObj.UserCode;//0 DR 1 CT

                            al.Add(obj);
                            hsItemZT.Add(conObj.Name, al);
                        }
                    }
                }
            }

            //载入项目列表
            this.LoadItem(myOperator.Dept.ID);

            this.fpSheetItem.DataSource = dvItem;

            //设置列表显示风格
            InitFp();

            //设置收费项目窗口显示风格。
            this.chooseItemControl = this.feeIntegrate.GetPlugIns<Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IChooseItemForOutpatient>
                (Neusoft.HISFC.BizProcess.Integrate.Const.INTERFACE_CHOOSE_ITEM, null);
            if (this.chooseItemControl == null)
            {
                this.chooseItemControl = new ucPopSelected();
            }
            chooseItemControl.ItemKind = this.itemKind;
            //{21C33D5B-5583-4b1d-8023-278336C0C6C7}
            myIGetSiItemGrade = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade;

            myITruncFee = (Neusoft.HISFC.BizProcess.Interface.Fee.ITruncFee)Neusoft.HISFC.BizProcess.Interface.Fee.InterfaceManager.GetTruncFeeType();

            if (myIGetSiItemGrade != null)
            {
                this.chooseItemControl.IGetSiItemGrade = myIGetSiItemGrade;
            }

            this.chooseItemControl.Init();

            //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入
            threadItemInit = new Thread(FillFilterControl);
            threadItemInit.Name = "add";
            threadItemInit.IsBackground = true;
            threadItemInit.Start();
            //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入 结束

            //this.chooseItemControl.SetDataSet(this.dsItem);

            if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging)
            {
                this.Parent.Parent.Controls.Add((Control)this.chooseItemControl);
            }

            //设置选择项目触发事件
            this.chooseItemControl.SelectedItem += new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.WhenGetItem(chooseItemControl_SelectedItem);
            //设置执行科室

            InitDept();

            //设置频次
            InitFreq();

            myHelpFreq.ArrayObject = alFreq;
            //设置用法
            InitUsage();
            myHelpUsage.ArrayObject = alUsage;
            //合同单位公费待遇
            InitBillPact();
            myBillPactHelper.ArrayObject = alBillPact;

            InitMachine();

            myInjec.WhenInputInjecs += new ucInjec.myDelegate(myInjec_WhenInputInjecs);

            if (this.rightControl != null)
            {
                this.rightControl.Init();
                this.rightControl.SetDataSet(this.dsItem);
                this.rightControl.SetFeeCodeIsDrugArrayListObj(this.phaFeeCodeHelper);
            }
            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Select].Value = true;

            //{3AB0201B-F5B6-442b-AC8B-DA3B5C106B01}
            this.fpSpread1_Sheet1.Columns[(int)Columns.LittleCost].Visible = false;

            //加载是否可以输入负数量//{0F98A513-A9EA-4110-B35F-E353A390E350}
            this.isCanInputNegativeQty = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.INPUT_NEGATIVE_QTY, true, false);
            //{0F98A513-A9EA-4110-B35F-E353A390E350}结束

            //{CA82280B-51B6-4462-B63E-43F4ECF456A3}
            ArrayList deptList = this.feeIntegrate.QueryDeptList("ALL", "1");
            foreach (Neusoft.FrameWork.Models.NeuObject neuObj in deptList)
            {
                dictDept.Add(neuObj.Memo + "|" + neuObj.ID, neuObj);
            }


            return 1;
        }

        //{6ACA3A64-8510-4152-957A-F2E8FB68C92E} 增加刷新项目列表按钮

        /// <summary>
        /// 刷新项目列表
        /// </summary>
        public void RefreshItem()
        {
            //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入,控制线程结束后,才能执行刷新
            if (this.threadItemInit.ThreadState == ThreadState.Stopped)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在刷新项目以及库存,请等待...");
                Application.DoEvents();

                (this.chooseItemControl as Control).Visible = false;
                int row = this.fpSpread1_Sheet1.ActiveRowIndex;

                this.LoadItem(myOperator.Dept.ID);
                this.fpSheetItem.DataSource = dvItem;
                this.chooseItemControl.Init();
                FillFilterControl();

                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                this.Focus();
                this.fpSpread1.Select();
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(row, 0);
            }
        }

        // //{6ACA3A64-8510-4152-957A-F2E8FB68C92E} 增加刷新项目列表按钮 完毕

        //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入
        /// <summary>
        /// 填充过滤项目控件
        /// </summary>
        private void FillFilterControl()
        {
            this.chooseItemControl.SetDataSet(this.dsItem);
        }
        //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入 结束
        //物资收费 {40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}

        /// <summary>
        /// 根据非要品编码获取非药品执行科室{CA82280B-51B6-4462-B63E-43F4ECF456A3}
        /// </summary>
        /// <param name="list"></param>
        public string SetExecDept(string itemID)
        {
            string id = string.Empty;
            ArrayList undrugDept = new ArrayList();
            if (dictDept.Count != 0)
            {
                foreach (string itemCode in dictDept.Keys)
                {
                    if (itemCode.Contains(itemID))
                    {
                        undrugDept.Add(dictDept[itemCode]);
                        if (dictDept[itemCode].User02 == "1")
                        {
                            id = dictDept[itemCode].ID;
                        }
                    }
                }
                if (undrugDept.Count != 0)
                {
                    this.lbDept.AddItems(undrugDept);
                }
                else
                {
                    this.lbDept.AddItems(this.alDept);
                    //return "-1";
                }
            }
            return id;
        }

        public string SetExecDept(int row, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList)
        {
            if (feeItemList.Item.ItemType != EnumItemType.UnDrug || !(feeItemList.Item is Neusoft.HISFC.Models.Fee.Item.Undrug))
            {
                return this.SetExecDept(feeItemList.Item.ID);
            }

            ArrayList al = Function.GetExecDept(feeItemList.RecipeOper.Dept, feeItemList.Item as Neusoft.HISFC.Models.Fee.Item.Undrug, ref this.errText);
            if (al == null)
            {
                return this.SetExecDept(feeItemList.Item.ID);
            }
            else
            {
                if (string.IsNullOrEmpty(feeItemList.ExecOper.Dept.ID))
                {
                    Neusoft.FrameWork.Models.NeuObject obj = al[0] as Neusoft.FrameWork.Models.NeuObject;
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = obj.Name;
                    feeItemList.ExecOper.Dept.ID = obj.ID;
                    feeItemList.ExecOper.Dept.Name = obj.Name;
                }
                else
                {
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.ExeDept].Text = feeItemList.ExecOper.Dept.Name;
                }
                lbDept.Items.Clear();
                lbDept.AddItems(al);
            }
            return string.Empty;

        }

        protected void chooseItemControl_SelectedItem(string itemCode, string drugFlag, string exeDeptCode, decimal price)
        {
            //清空
            this.alAddRows.Clear();
            if (isInputItemsNoSpe)
            {
                if (!this.isCanAddItem && !isQuitFee)
                {
                    MessageBox.Show(Language.Msg("请单选择一条处方进行增加项目!"));
                    this.fpSpread1.Focus();
                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.SetActiveCell(this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.InputCode, false);

                    //this.isDealCellChange = true;

                    return;
                }
            }
            if (drugFlag == "0")//非药品
            {
                //string exeDept = SetExecDept(itemCode);//{CA82280B-51B6-4462-B63E-43F4ECF456A3}                
                //if (!string.IsNullOrEmpty(exeDept) && exeDept != "-1")//没有维护多科室时
                //{
                //    exeDeptCode = exeDept;
                //}
            }
            else
            {
                this.lbDept.AddItems(this.alDept);
            }

            if (drugFlag == "2")
            {
                DataRow findRow;
                DataRow[] findRows = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "' and drug_flag = '" + drugFlag + "'");
                if (findRows == null || findRows.Length == 0)
                {
                    MessageBox.Show("编码为: [" + itemCode + " ] 的项目查找失败!");
                    return;
                }
                findRow = findRows[0];

                string classCode = findRow["SYS_CLASS"].ToString();
                string itemName = findRow["ITEM_NAME"].ToString();
                if (classCode == "UC" || classCode == "UT")   //检查和其他，才允许选择复合明细
                {
                    #region 检查项目允许选择明细-gumzh

                    List<Neusoft.HISFC.Models.Fee.Item.UndrugComb> lstzt = new List<Neusoft.HISFC.Models.Fee.Item.UndrugComb>();
                    if (this.ztManager.QueryUnDrugztDetail(itemCode, ref lstzt) == -1)
                    {
                        MessageBox.Show(this.ztManager.Err);
                        this.isDealCellChange = true;
                        return;
                    }
                    ArrayList alSubItems = new ArrayList();   //组套明细
                    foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrug in lstzt)
                    {
                        if (undrug.ValidState == "无效")
                        {
                            continue;
                        }
                        undrug.Qty = undrug.Qty * 1;  //数量*复合项目明细数量

                        alSubItems.Add(undrug);
                    }
                    if (alSubItems.Count <= 0)
                    {
                        MessageBox.Show("编码为: [" + itemCode + " ] 的项目无有效的明细项目!");
                        return;
                    }
                    Neusoft.HISFC.Components.Common.Forms.frmChoseItemCommon frmChoseItem = new Neusoft.HISFC.Components.Common.Forms.frmChoseItemCommon();
                    frmChoseItem.ServerType = ServiceTypes.C;
                    frmChoseItem.AlSubItems = alSubItems;
                    frmChoseItem.Text = itemName;
                    if (this.undrugManager.SetUltrasound(itemCode))
                    {
                        SetItem(itemCode, drugFlag, exeDeptCode, this.fpSpread1_Sheet1.ActiveRowIndex, 1, price, "0");
                    }
                    else
                    {
                        frmChoseItem.ShowDialog();
                        if (frmChoseItem.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            int actIndex = this.fpSpread1_Sheet1.ActiveRowIndex;  //当前活动行
                            alSubItems = frmChoseItem.AlSubItems;
                            if (alSubItems == null && alSubItems.Count <= 0)
                            {
                                //选择数目为0，直接返回
                                return;
                            }
                            foreach (Neusoft.HISFC.Models.Base.Item undrug in alSubItems)
                            {
                                //添加明细
                                this.SetItem(undrug.ID, "0", "", actIndex, undrug.Qty, undrug.Price, "0");
                                actIndex = GetNewRow();
                                if (actIndex == -1)
                                {
                                    this.AddNewRow();
                                    actIndex = this.fpSpread1_Sheet1.RowCount - 1;
                                }
                            }

                        }
                        else
                        {
                            //点击取消，表示是选择整个组合
                            SetItem(itemCode, drugFlag, exeDeptCode, this.fpSpread1_Sheet1.ActiveRowIndex, 1, price, "0");
                        }

                    }

                    #endregion
                }
                else
                {
                    SetItem(itemCode, drugFlag, exeDeptCode, this.fpSpread1_Sheet1.ActiveRowIndex, 1, price, "0");
                }


            }
            else
            {
                //物资收费 {40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                SetItem(itemCode, drugFlag, exeDeptCode, this.fpSpread1_Sheet1.ActiveRowIndex, 1, price, "0");
            }

            this.Focus();
            this.fpSpread1.Focus();

            DataRow rowFind;
            DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "'");

            if (rowFinds == null || rowFinds.Length == 0)
            {
                MessageBox.Show("查找项目出错!");
                return;
            }
            rowFind = rowFinds[0];

            if (rowFind == null)
            {
                MessageBox.Show("查找项目失败!");
                return;
            }
            for (int i = 0; i < this.alAddRows.Count; i++)
            {
                string itemType = rowFind["DRUG_FLAG"].ToString();
                if (itemType == "0")//非药品
                {
                    this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.Amount].Locked = false;
                    this.fpSpread1_Sheet1.SetActiveCell((int)alAddRows[i], (int)Columns.Amount, false);
                    this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.CombNo].Locked = false;
                    //妇幼  xingz
                    this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.Usage].Locked = false;
                }
                else
                {
                    if (rowFind["SYS_CLASS"].ToString() == "PCC" && itemType != "4")//草药直接输入用量,协定处方不在这里处理.采用西药的跳转方式.
                    {
                        isDealCellChange = false;
                        this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.DoseOnce].Locked = false;
                        this.fpSpread1_Sheet1.SetActiveCell((int)alAddRows[i], (int)Columns.DoseOnce, false);
                        this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.CombNo].Locked = false;
                        isDealCellChange = true;
                    }
                    else//其他药品跳转到数量输入位置
                    {
                        this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.Amount].Locked = false;
                        this.fpSpread1_Sheet1.SetActiveCell((int)alAddRows[i], (int)Columns.Amount, false);
                        this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.CombNo].Locked = false;
                    }
                    //妇幼  xingz
                    this.fpSpread1_Sheet1.Cells[(int)alAddRows[i], (int)Columns.Usage].Locked = false;
                }
            }
            if (alAddRows.Count > 1)
            {
                if ((int)alAddRows[alAddRows.Count - 2] <= this.fpSpread1_Sheet1.Rows.Count - 2)
                {    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.SetActiveCell((int)alAddRows[alAddRows.Count - 1] + 1, (int)Columns.InputCode, false);
                }
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        public void AddNewRow()
        {
            int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
            this.fpSpread1_Sheet1.Rows.Add(currRow + 1, 1);
            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            this.fpSpread1_Sheet1.SetActiveCell(currRow + 1, (int)Columns.InputCode);
            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            this.fpSpread1_Sheet1.Cells[currRow + 1, (int)Columns.Select].Value = true;

            SumLittleCostAll();
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteRow()
        {
            //this.fpSpread1.StopCellEditing();
            int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;

            if (this.fpSpread1_Sheet1.Rows[currRow].Tag != null)
            {
                if (this.fpSpread1_Sheet1.Rows[currRow].Tag is FeeItemList)
                {
                    FeeItemList feeTemp = this.fpSpread1_Sheet1.Rows[currRow].Tag as FeeItemList;
                    if (feeTemp.RecipeNO != null && feeTemp.RecipeNO != string.Empty && feeTemp.Order.ID != string.Empty)
                    {
                        ArrayList alTemp = this.outpatientManager.QueryFeeDetailFromMOOrder(feeTemp.Order.ID);

                        #region 判断医生医嘱是否可以删除

                        if (!this.isCanModifyCharge)
                        {
                            if (!string.IsNullOrEmpty(feeTemp.Order.ID) && feeTemp.FTSource == "1" && !feeTemp.Item.IsMaterial)
                            {
                                Neusoft.HISFC.Models.Order.OutPatient.Order orderTemp = orderIntegrate.GetOneOrder(feeTemp.Patient.ID, feeTemp.Order.ID);
                                if (orderTemp != null && !string.IsNullOrEmpty(orderTemp.ID))
                                {
                                    MessageBox.Show("医生开立的医嘱不允许删除!", "警告");
                                    return;
                                }
                            }
                        }

                        #endregion

                        if (alTemp != null && alTemp.Count > 0)
                        {
                            feeTemp = alTemp[0] as FeeItemList;

                            if (feeTemp.IsAccounted)
                            {
                                MessageBox.Show(Language.Msg("该项目已经扣取门诊账户,不能删除!"));

                                return;
                            }

                            if (feeTemp.IsConfirmed)
                            {
                                MessageBox.Show(Language.Msg("该项目已经被终端确认，不能删除!"));

                                return;
                            }
                            if (this.isTransferTreat != true)
                            {
                                if (feeTemp.PayType != Neusoft.HISFC.Models.Base.PayTypes.Charged)
                                {
                                    MessageBox.Show(Language.Msg("该项目不是划价状态，不能删除!"));

                                    return;
                                }


                                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                                this.outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                                int iReturn = this.outpatientManager.DeleteFeeItemListByRecipeNO(feeTemp.RecipeNO, feeTemp.SequenceNO.ToString());
                                if (iReturn <= 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("删除明细失败!" + this.outpatientManager.Err);
                                    return;
                                }
                                else
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.Commit();
                                }
                            }
                        }
                    }
                }
            }
            this.fpSpread1_Sheet1.Rows.Remove(currRow, 1);
            currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.InputCode);

            if (this.fpSpread1_Sheet1.RowCount == 0)
            {
                this.AddRow(-1);
            }

            SumCost();
        }

        /// <summary>
        /// 删除指定项目
        /// </summary>
        /// <param name="feeTemp"></param>
        /// <returns></returns>
        public int DeleteRow(FeeItemList feeTemp)
        {
            if (feeTemp.RecipeNO != null && feeTemp.RecipeNO != string.Empty && feeTemp.Order.ID != string.Empty)
            {
                ArrayList alTemp = null;//// this.outpatientManager.GetFeeDetailFromMOOrder(feeTemp.Order.ID);
                if (alTemp != null && alTemp.Count > 0)
                {
                    feeTemp = alTemp[0] as FeeItemList;

                    if (feeTemp.IsAccounted)
                    {
                        MessageBox.Show(Language.Msg("该项目已经扣取门诊账户,不能删除!"));

                        return -1;
                    }

                    if (feeTemp.IsConfirmed)
                    {
                        MessageBox.Show(Language.Msg("该项目已经被终端确认，不能删除!"));

                        return -1;
                    }
                    if (this.isTransferTreat != true)
                    {
                        if (feeTemp.PayType != Neusoft.HISFC.Models.Base.PayTypes.Charged)
                        {
                            MessageBox.Show(Language.Msg("该项目不是划价状态，不能删除!"));

                            return -1;
                        }

                        Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                        this.outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                        iReturn = this.outpatientManager.DeleteFeeItemListByRecipeNO(feeTemp.RecipeNO, feeTemp.SequenceNO.ToString());
                        if (iReturn <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("删除明细失败!" + outpatientManager.Err);

                            return -1;
                        }
                        else
                        {
                            Neusoft.FrameWork.Management.PublicTrans.Commit();
                        }
                    }
                }
            }

            return iReturn;
        }

        /// <summary>
        /// 停止编辑
        /// </summary>
        public void StopEdit()
        {
            this.fpSpread1.StopCellEditing();
        }

        /// <summary>
        /// 获得划价明细
        /// </summary>
        /// <returns>成功 划价明细 失败 null</returns>
        public ArrayList GetFeeItemListForCharge()
        {
            ArrayList alFeeItemList = new ArrayList();

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag == null || !(this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList))
                {
                    continue;
                }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                if (this.IsCanSelectItemAndFee && this.fpSpread1_Sheet1.Cells[i, (int)Columns.Select].Text.ToLower() == "false")
                {
                    continue;
                }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束

                alFeeItemList.Add(((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag));
            }

            return alFeeItemList;
        }

        /// <summary>
        /// 获得划价明细
        /// </summary>
        /// <returns>成功 划价明细 失败 null</returns>
        public ArrayList GetFeeItemListForCharge(bool isGroupDetail)
        {
            ArrayList alFeeItemList = new ArrayList();

            //for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            //{
            //    if (this.fpSpread1_Sheet1.Rows[i].Tag == null || !(this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList))
            //    {
            //        continue;
            //    }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            //    if (this.IsCanSelectItemAndFee && this.fpSpread1_Sheet1.Cells[i, (int)Columns.Select].Text.ToLower() == "false")
            //    {
            //        continue;
            //    }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束

            //    alFeeItemList.Add(((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag));
            //}

            return alFeeItemList;
        }

        /// <summary>
        /// 添加一笔小计金额
        /// </summary>
        public void SumLittleCost()
        {
            string tempName = string.Empty; //判断当前是否有小计
            string tempNameSec = string.Empty;//判断下一行是否有小计
            if (this.fpSpread1_Sheet1.RowCount <= 0)
            {
                return;
            }
            int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;

            tempName = this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemName].Text;
            if (tempName == "小计")
            {
                return;
            }
            if (this.fpSpread1_Sheet1.RowCount > currRow + 1)
            {
                tempNameSec = this.fpSpread1_Sheet1.Cells[currRow + 1, (int)Columns.ItemName].Text;
                if (tempNameSec == "小计")
                {
                    return;
                }
            }
            if (tempName != string.Empty)
            {
                this.fpSpread1_Sheet1.Rows.Add(currRow + 1, 1);
            }
            this.fpSpread1_Sheet1.ActiveRowIndex = currRow + 1;

            decimal sumTotCost = 0;//总金额
            decimal nowCost = 0;//当前金额
            int nowCount = 0;

            for (int i = currRow; i >= 0; i--)
            {
                tempName = this.fpSpread1_Sheet1.Cells[i, (int)Columns.ItemName].Text;
                if (tempName == "小计")
                {
                    break;
                }
                nowCost = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Text);
                if (nowCost > 0 && nowCount == 0)
                {
                    nowCount = i + 1;
                }
                sumTotCost += nowCost;
            }
            if (sumTotCost > 0)
            {
                nowCount = this.fpSpread1_Sheet1.ActiveRowIndex;
                int rowCount = this.fpSpread1_Sheet1.RowCount;
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.Cells[nowCount, (int)Columns.InputCode].Locked = true;
                this.fpSpread1_Sheet1.Cells[nowCount, (int)Columns.ItemName].Text = "小计";
                this.fpSpread1_Sheet1.Cells[nowCount, (int)Columns.Cost].Text = sumTotCost.ToString();
                if (nowCount + 1 == this.fpSpread1_Sheet1.RowCount)
                {
                    this.AddRow(nowCount);
                }
            }

            this.SumLittleCostAll();
        }

        /// <summary>
        /// 清空当前行的内容
        /// </summary>
        /// <param name="row">当前行</param>
        public void ClearRow(int row)
        {
            this.fpSpread1_Sheet1.Rows[row].Tag = null;

            for (int i = 0; i < this.fpSpread1_Sheet1.Columns.Count; i++)
            {
                this.fpSpread1_Sheet1.Cells[row, i].Value = null;
            }
        }

        /// <summary>
        /// 在显示项目信息控件添加一行，如果已经添加则跳到下一行的编码输入位置
        /// </summary>
        /// <param name="row">当前行</param>
        public void AddRow(int row)
        {
            if (JudegExeDept() == -1)
            {
                return;
            }
            if (row == this.fpSpread1_Sheet1.RowCount - 1)
            {
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.Rows.Add(this.fpSpread1_Sheet1.RowCount, 1);
                this.fpSpread1.Focus();

                this.fpSpread1_Sheet1.ActiveRowIndex = this.fpSpread1_Sheet1.RowCount - 1;
                this.SetColumnEnable(this.fpSpread1_Sheet1.ActiveRowIndex);
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.Select].Value = true;
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.SetActiveCell(this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.InputCode);
            }
            else
            {
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.ActiveRowIndex++;
                if (this.fpSpread1_Sheet1.Rows[this.fpSpread1_Sheet1.ActiveRowIndex].Tag != null)
                {
                    this.fpSpread1_Sheet1.SetActiveCell(this.fpSpread1_Sheet1.ActiveRowIndex, this.fpSpread1_Sheet1.ActiveColumnIndex);
                }
                else
                {
                    ////{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.Select].Value = true;
                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                    this.fpSpread1_Sheet1.SetActiveCell(this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.InputCode);
                }
            }
        }

        /// <summary>
        /// 收费或划价出错后设置焦点
        /// </summary>
        public void SetFocus()
        {
            if (this.fpSpread1_Sheet1.Rows.Count > 0)
            {
                this.fpSpread1.Select();
                this.fpSpread1.Focus();
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                //this.fpSpread1_Sheet1.SetActiveCell(0, (int)Columns.InputCode);
                //光标放在最后一行
                this.fpSpread1_Sheet1.SetActiveCell(this.fpSpread1_Sheet1.Rows.Count - 1, (int)Columns.InputCode);
            }
        }

        /// <summary>
        /// 收费或划价出错后设置焦点
        /// </summary>
        public void SetFocusToInputCode()
        {
            if (this.fpSpread1_Sheet1.Rows.Count > 0)
            {
                this.fpSpread1.Focus();
                //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                this.fpSpread1_Sheet1.SetActiveCell(this.fpSpread1_Sheet1.Rows.Count - 1, (int)Columns.InputCode);
            }
        }

        /// <summary>
        /// 清除所有收费信息
        /// </summary>
        public void Clear()
        {
            if (this.fpSpread1_Sheet1.RowCount > 0)
            {
                this.fpSpread1_Sheet1.Rows.Remove(0, this.fpSpread1_Sheet1.RowCount);
            }
            hDays = 1;
            this.fpSpread1_Sheet1.Rows.Add(0, 1);
            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Select].Value = true;

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                this.SetColumnEnable(i);
            }
        }

        /// <summary>
        /// 修改付数
        /// </summary>
        public void ModifyDays()
        {
            bool isHavePCC = false;
            FeeItemList fTemp = null;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        fTemp = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                        //if (fTemp.Item.IsPharmacy)
                        if (fTemp.Item.ItemType == EnumItemType.Drug)
                        {
                            if (fTemp.Item.SysClass.ID.ToString() == "PCC" && !(fTemp.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                            {
                                isHavePCC = true;
                            }
                        }
                    }
                }
            }
            if (isHavePCC)
            {
                ucInputDays frmDays = new ucInputDays();
                int index = this.fpSpread1_Sheet1.ActiveRowIndex;
                string existCombNO = string.Empty;
                if (this.fpSpread1_Sheet1.Rows[index].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[index].Tag is FeeItemList)
                    {
                        FeeItemList fTempIndex = this.fpSpread1_Sheet1.Rows[index].Tag as FeeItemList;
                        existCombNO = fTempIndex.Order.Combo.ID;
                    }
                }

                int day = 0;
                string combNo = string.Empty;
                this.Focus();
                if (existCombNO.Length > 0)
                {
                    frmDays.CombNO = existCombNO;
                }
                else
                {
                    frmDays.CombNO = this.GetMaxCombNo();
                }

                Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(frmDays);

                if (frmDays.IsSelect)
                {
                    day = frmDays.Days;
                    combNo = frmDays.CombNO;
                    for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                    {
                        if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                        {
                            if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                            {
                                fTemp = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                                //if (fTemp.Item.IsPharmacy)
                                if (fTemp.Item.ItemType == EnumItemType.Drug)
                                {
                                    if (fTemp.Item.SysClass.ID.ToString() == "PCC" && (fTemp.Order.Combo.ID == existCombNO) && !(fTemp.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                    {
                                        fTemp.Days = day;
                                        decimal days = 0;
                                        decimal price = 0;
                                        decimal qty = 0;
                                        decimal totQty = 0;
                                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Days].Text = day.ToString();

                                        bool bReturn = InputDataIsValid(i, (int)Columns.Days, "付数", 9999, 0, ref days);
                                        if (!bReturn)
                                        {
                                            return;
                                        }
                                        fTemp.Days = days;
                                        bReturn = InputDataIsValid(i, (int)Columns.Price, "单价", 99999, 0, ref price);
                                        if (!bReturn)
                                        {
                                            return;
                                        }

                                        bReturn = InputDataIsValid(i, (int)Columns.DoseOnce, "每次用量", 99999, 0, ref qty);
                                        if (!bReturn)
                                        {
                                            return;
                                        }

                                        qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);
                                        // {1FAD3FA2-C7D8-4cac-845F-B9EBECDE2312}
                                        totQty = qty * days / ((fTemp.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose == 0 ? 1 : (fTemp.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose);

                                        //totQty = qty * days;
                                        fTemp.Item.Qty = totQty;
                                        fTemp.Order.Combo.ID = combNo;
                                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.CombNo].Text = fTemp.Order.Combo.ID;

                                        Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, totQty, fTemp);

                                        if (ft == null)
                                        {
                                            return;
                                        }

                                        fTemp.FT.TotCost = ft.TotCost;
                                        fTemp.FT.OwnCost = ft.OwnCost;
                                        fTemp.FT.PayCost = ft.PayCost;
                                        fTemp.FT.PubCost = ft.PubCost;

                                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Amount].Value = totQty;
                                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Value = ft.TotCost;
                                    }
                                }
                            }
                        }
                    }
                    this.SumCost();
                }
            }
        }

        /// <summary>
        /// 更改价格
        /// </summary>
        public void ModifyPrice()
        {
            FeeItemList fTemp = null;
            this.isDealCellChange = false;
            sumPubCost = 0;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
                        int age = (int)((new TimeSpan(nowTime.Ticks - this.rInfo.Birthday.Ticks)).TotalDays / 365);

                        fTemp = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                        DataRow findRow = null;

                        if (fTemp.Item.ID != "999")
                        {
                            DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + fTemp.Item.ID + "'");

                            if (rowFinds == null || rowFinds.Length == 0)
                            {
                                //根据处方号和项目流水号查询old_mo_order不为空/开立科室是管理中心
                                if (this.outpatientManager.IsFeeInfo(fTemp.RecipeNO, fTemp.SequenceNO.ToString()))
                                {
                                    //重新赋值
                                    DataSet dtItemSerch = new DataSet();
                                    this.outpatientManager.QueryItemListForValid(myOperator.Dept.ID, fTemp.Item.ID, ref dtItemSerch);
                                    rowFinds = dtItemSerch.Tables[0].Select("ITEM_CODE = " + "'" + fTemp.Item.ID + "'");
                                }
                                else
                                {
                                    MessageBox.Show("编码为: [" + fTemp.Item.ID + " ] 的项目查找失败!");
                                    this.isDealCellChange = true;

                                    return;
                                }
                            }
                            findRow = rowFinds[0];

                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
                            string priceForm = this.rInfo.Pact.PriceForm;
                            decimal unitPrice = NConvert.ToDecimal(findRow["UNIT_PRICE"]);
                            decimal childPrice = NConvert.ToDecimal(findRow["CHILD_PRICE"]);
                            decimal SPPrice = NConvert.ToDecimal(findRow["SP_PRICE"]);
                            decimal purchasePrice = NConvert.ToDecimal(findRow["PURCHASE_PRICE"]);
                            //if (unitPrice != 0)
                            //{
                            string msgErr = string.Empty;
                            PactItemRate pRate = Function.PactRate(this.rInfo, fTemp, ref msgErr);
                            if (pRate == null)
                            {
                                MessageBox.Show("查询" + fTemp.Item.Name + "的优惠比例出错" + msgErr);
                                return;

                            }
                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}
                            // 保存原始默认价格
                            fTemp.Item.ChildPrice = unitPrice;
                            decimal orgPrice = unitPrice;
                            decimal price = this.feeIntegrate.GetPrice(fTemp.Item.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice);
                            fTemp.OrgPrice = orgPrice;
                            price *= 1 - pRate.Rate.RebateRate;
                            fTemp.Item.Price = price;
                            fTemp.SpecialPrice = fTemp.Item.Price;
                        }
                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Tag = fTemp.Item.Price;

                        Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(fTemp.Item.Price, fTemp.Item.Qty, fTemp);

                        if (ft == null)
                        {
                            this.fpSpread1.Select();
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Amount, false);

                            return;
                        }

                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Value = ft.TotCost;
                        fTemp.FT.OwnCost = ft.OwnCost;
                        fTemp.FT.TotCost = ft.TotCost;
                        fTemp.FT.PayCost = ft.PayCost;
                        fTemp.FT.PubCost = ft.PubCost;
                        if (fTemp.FeePack == "1")
                        {
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Text = fTemp.Item.Price.ToString();
                        }
                        else
                        {
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Text = Neusoft.FrameWork.Public.String.FormatNumber(fTemp.Item.Price / fTemp.Item.PackQty, 4).ToString();
                        }
                        //}

                        this.SetItemRateInfo(i, fTemp);
                    }
                }
            }
            this.SumCost();
            this.isDealCellChange = true;
        }

        /// <summary>
        /// 改变体检价格
        /// </summary>
        public void PhyExamModifyPrice()
        {
            FeeItemList fTemp = null;
            this.isDealCellChange = false;

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                    {
                        DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
                        int age = (int)((new TimeSpan(nowTime.Ticks - this.rInfo.Birthday.Ticks)).TotalDays / 365);

                        fTemp = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                        DataRow findRow = null;

                        if (fTemp.Item.ID != "999")
                        {
                            DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + fTemp.Item.ID + "'");

                            if (rowFinds == null || rowFinds.Length == 0)
                            {
                                MessageBox.Show("编码为: [" + fTemp.Item.ID + " ] 的项目查找失败!");
                                this.isDealCellChange = true;

                                return;
                            }
                            findRow = rowFinds[0];

                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
                            string priceForm = this.rInfo.Pact.PriceForm;
                            //decimal unitPrice = NConvert.ToDecimal(findRow["UNIT_PRICE"]);
                            decimal unitPrice = NConvert.ToDecimal(fTemp.FT.OwnCost / fTemp.Item.Qty);
                            decimal childPrice = NConvert.ToDecimal(findRow["CHILD_PRICE"]);
                            decimal SPPrice = NConvert.ToDecimal(findRow["SP_PRICE"]);
                            decimal purchasePrice = NConvert.ToDecimal(findRow["PURCHASE_PRICE"]);
                            //if (unitPrice != 0)
                            //{
                            string msgErr = string.Empty;
                            PactItemRate pRate = Function.PactRate(this.rInfo, fTemp, ref msgErr);
                            if (pRate == null)
                            {
                                MessageBox.Show("查询" + fTemp.Item.Name + "的优惠比例出错" + msgErr);
                                return;

                            }
                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}
                            // 保存原始默认价格
                            fTemp.Item.ChildPrice = unitPrice;
                            decimal orgPrice = unitPrice;
                            decimal price = this.feeIntegrate.GetPrice(fTemp.Item.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice);
                            fTemp.OrgPrice = orgPrice;
                            price *= 1 - pRate.Rate.RebateRate;
                            fTemp.Item.Price = price;
                            fTemp.SpecialPrice = fTemp.Item.Price;
                        }
                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Tag = fTemp.Item.Price;

                        Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(fTemp.Item.Price, fTemp.Item.Qty, fTemp);

                        if (ft == null)
                        {
                            this.fpSpread1.Select();
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(i, (int)Columns.Amount, false);

                            return;
                        }

                        this.fpSpread1_Sheet1.Cells[i, (int)Columns.Cost].Value = ft.TotCost;
                        fTemp.FT.OwnCost = ft.OwnCost;
                        fTemp.FT.TotCost = ft.TotCost;
                        fTemp.FT.PayCost = ft.PayCost;
                        fTemp.FT.PubCost = ft.PubCost;
                        if (fTemp.FeePack == "1")
                        {
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Text = fTemp.Item.Price.ToString();
                        }
                        else
                        {
                            this.fpSpread1_Sheet1.Cells[i, (int)Columns.Price].Text = Neusoft.FrameWork.Public.String.FormatNumber(fTemp.Item.Price / fTemp.Item.PackQty, 4).ToString();
                        }
                        //}

                        this.SetItemRateInfo(i, fTemp);
                    }
                }
            }
            this.SumCost();
            this.isDealCellChange = true;
        }

        /// <summary>
        /// 重新刷新显示费用
        /// </summary>
        public void RefreshNewRate()
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null && this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    this.SetItemRateInfo(i, (FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag);
                }
            }
        }

        /// <summary>
        /// 更新修改比例后的费用.
        /// </summary>
        /// <param name="feeDetails"></param>
        public void RefreshNewRate(ArrayList feeDetails)
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag != null && this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    if (((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).Item.ID == ((FeeItemList)feeDetails[i]).Item.ID)
                    {
                        ((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).NewItemRate = ((FeeItemList)feeDetails[i]).NewItemRate;
                        ((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = ((FeeItemList)feeDetails[i]).ItemRateFlag;

                        this.SetItemRateInfo(i, (FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag);
                    }
                }
            }

            SumCost();
        }

        /// <summary>
        /// 获得收费信息
        /// </summary>
        /// <returns></returns>
        public ArrayList GetFeeItemList()
        {
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            ArrayList hsNOREOnlyOneItem = new ArrayList();
            Hashtable hsREOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            ArrayList feeItemLists = new ArrayList();

            Hashtable hsDoct = new Hashtable();
            sumPubCost = 0;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag == null || !(this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList))
                {
                    continue;
                }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                if (this.IsCanSelectItemAndFee && this.fpSpread1_Sheet1.Cells[i, (int)Columns.Select].Text.ToLower() == "false")
                {
                    continue;
                }//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}结束


                if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    FeeItemList f = (FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag;
                    //增加开方医生所在科室
                    if (!string.IsNullOrEmpty(f.RecipeOper.ID) && string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                    {
                        if (hsDoct.ContainsKey(f.RecipeOper.ID))
                        {
                            f.DoctDeptInfo.ID = hsDoct[f.RecipeOper.ID].ToString();
                        }
                        else
                        {
                            Employee e = this.managerIntegrate.GetEmployeeInfo(f.RecipeOper.ID);
                            if (e != null)
                            {
                                f.DoctDeptInfo.ID = e.Dept.ID;
                                hsDoct.Add(f.RecipeOper.ID, e.Dept.ID);
                            }
                        }
                    }

                    if (f.IsGroup)
                    {
                        //ArrayList alDetail = ConvertGroupToDetail(f);
                        ArrayList alDetail = null;
                        if (this.IsUseNewUndrugZT)
                        {
                            if (hsItemZT.ContainsKey(f.Item.ID))
                            {
                                ArrayList alItem = (ArrayList)hsItemZT[f.Item.ID];
                                string type = (alItem[0] as NeuObject).User02;
                                if (type == "DR")
                                {
                                    alDetail = ConvertDRGroupToDetail(f, !isFindDRFirst, ref hsDROnlyOneItem, ref drCount);
                                    isFindDRFirst = true;
                                }
                                else if (type == "CT")
                                {
                                    alDetail = ConvertCTGroupToDetail(f, !isFindCTFirst, ref hsCTOnlyOneItem);
                                    isFindCTFirst = true;
                                }
                            }
                            else
                            {
                                alDetail = ConvertGroupToDetail(f);
                            }
                        }
                        else
                        {
                            alDetail = ConvertGroupToDetail(f);
                        }
                        if (alDetail == null)
                        {
                            errText = "获得组套明细出错!";
                            return null;
                        }

                        if (alDetail.Count == 0)
                        {
                            MessageBox.Show(((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).Item.Name + "是组合项目,但是没有维护明细或者明细已经停用！请与信息科联系！");
                        }
                        feeItemLists.AddRange(alDetail);
                    }
                    else
                    {
                        feeItemLists.Add(((FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag));
                    }
                }
            }
            itenqty = 0;//将标识重置
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList f = feeItemLists[i] as FeeItemList;
                if (hsDROnlyOneItem.ContainsKey(f.Item.ID))
                {
                    feeItemLists.RemoveAt(i);
                }
                if (hsCTOnlyOneItem.ContainsKey(f.Item.ID))
                {
                    if (hsCTOnlyOneItem[f.Item.ID].ToString() != "true")
                    {
                        hsCTOnlyOneItem.Remove(f.Item.ID);
                        hsCTOnlyOneItem.Add(f.Item.ID, "true");
                    }
                    else
                    {
                        feeItemLists.RemoveAt(i);
                    }
                }
            }
            foreach (DictionaryEntry de in hsDROnlyOneItem)
            {
                FeeItemList f = de.Value as FeeItemList;
                feeItemLists.Add(f);
            }
            int number = 1;
            int returnRows = 0;//是否为限制收费药品
            decimal LimitNumber = 1;
            ArrayList hsREOnlylistItem = new ArrayList();
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                string  Discount_type = "1";//限制收费类型
                int TOPPRICE = 0;
                decimal DISCOUNT_RATE = 0;
                FeeItemList s = feeItemLists[i] as FeeItemList;
                returnRows = this.undrugManager.SetRestrictingfee(s.Item.ID, ref  LimitNumber);
                Discount_type = this.undrugManager.SetDiscountfee(s.Item.ID, ref  DISCOUNT_RATE, ref  TOPPRICE);
                if (returnRows > 0 && this.rInfo.DoctorInfo.Templet.Dept.ID != "7021")
                {
                    this.setRestrictingfee.ConvertRestrictingfee(PatientInfo.PID.CardNO, s, ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem, number, LimitNumber);
                }
                if (Discount_type == "2") {
                    this.setRestrictingfee.ConvertDiscountfee(s, DISCOUNT_RATE, TOPPRICE, ref hsREOnlyOneItem, ref hsREOnlylistItem, number);
                }

                number++;
            }
            number = 1;
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList s = feeItemLists[i] as FeeItemList;
                if (hsREOnlyOneItem.ContainsKey(s.Item.ID + number))
                {
                    feeItemLists.RemoveAt(i);
                }
                number++;
            }
            foreach (FeeItemList ds in hsREOnlylistItem)
            {
                feeItemLists.Add(ds);
            }

            return feeItemLists;
        }
        //{3AEB5613-1CB0-4158-89E6-F82F0B643388}feng.ch 门诊费用插入医疗组
        private List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct> GetMedicalGroupCode(string deptCode, string doctID)
        {
            return managerIntegrate.GetMedicalGroup(deptCode, doctID);
        }
        /// <summary>
        /// 刷新看诊医生
        /// </summary>
        /// <param name="recipeSeq">当前收费序列</param>
        /// <param name="deptCode">变更后的看诊科室</param>
        /// <param name="obj">变更后的医生代码</param>
        public void RefreshSeeDoc(string recipeSeq, string deptCode, Neusoft.FrameWork.Models.NeuObject obj)
        {
            //{3AEB5613-1CB0-4158-89E6-F82F0B643388}
            List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct> medicalGroup = new List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct>();
            medicalGroup = GetMedicalGroupCode(deptCode, obj.ID);
            Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct doc = new Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct();
            if (medicalGroup.Count > 0)
            {
                doc = medicalGroup[0] as Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct;
            }
            if (medicalGroup == null)
            {
                medicalGroup = new List<Neusoft.HISFC.Models.Order.Inpatient.MedicalTeamForDoct>();
            }
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag == null || !(this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList))
                {
                    continue;
                }
                if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    FeeItemList tempFeeItemList = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                    if (tempFeeItemList.RecipeSequence == recipeSeq)
                    {
                        tempFeeItemList.RecipeOper.Dept.ID = deptCode;
                        tempFeeItemList.RecipeOper.ID = obj.ID;
                        tempFeeItemList.RecipeOper.Name = obj.Name;
                        //{3AEB5613-1CB0-4158-89E6-F82F0B643388}
                        tempFeeItemList.MedicalGroupCode = doc.MedcicalTeam;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新看诊科室
        /// </summary>
        /// <param name="recipeSeq">收费序列</param>
        /// <param name="obj">修改后的科室信息</param>
        public void RefreshSeeDept(string recipeSeq, Neusoft.FrameWork.Models.NeuObject obj)
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag == null || !(this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList))
                {
                    continue;
                }
                if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                {
                    FeeItemList tempFeeItemList = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;

                    if (tempFeeItemList.RecipeSequence == recipeSeq)
                    {
                        ((Register)tempFeeItemList.Patient).DoctorInfo.Templet.Dept = obj.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// 添加挂号费
        /// </summary>
        public void AddRegFee()
        {
            if (this.rInfo == null || this.tempDept == null)
            {
                return;
            }
            //如果不是直接收费患者,不增加挂号费
            if (this.rInfo.PID.CardNO.Substring(0, 1) != this.noRegFlagChar)
            {
                return;
            }

            //如果挂号费用项目没有维护,那么终止
            if (this.regFeeItemCode == string.Empty)
            {
                return;
            }

            //判断如果增加了挂号费项目,那么不继续增加
            if (this.fpSpread1_Sheet1.RowCount > 0)
            {
                for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                    {
                        if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                        {
                            FeeItemList fSame = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                            if (fSame.Item.ID == this.regFeeItemCode)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            //如果收费项目中不包含维护的挂号费项目,那么返回
            DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + this.regFeeItemCode + "'");

            if (rowFinds == null || rowFinds.Length == 0)
            {
                return;
            }
            //清空
            this.fpSpread1_Sheet1.Rows.Add(0, 1);
            this.alAddRows.Clear();
            this.SetItem(this.regFeeItemCode, "0", this.rInfo.DoctorInfo.Templet.Dept.ID, 0, 1, 0, "0");

            RegLvlFee tempRegFeeOfPact = this.registerIntegrate.GetRegLevelByPactCode(this.rInfo.Pact.ID, this.comRegLevel);
            if (tempRegFeeOfPact == null)
            {
                return;
            }
            if (tempRegFeeOfPact.RegFee <= 0)
            {
                return;
            }

            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Price].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.ExeDept].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Amount].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.CombNo].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Price].Text = tempRegFeeOfPact.RegFee.ToString();

            FeeItemList fTemp = this.fpSpread1_Sheet1.Rows[0].Tag as FeeItemList;

            fTemp.Item.Price = tempRegFeeOfPact.RegFee;
            fTemp.OrgPrice = fTemp.Item.Price;

            Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(fTemp.Item.Price, fTemp.Item.Qty, fTemp);

            if (ft == null)
            {
                this.fpSpread1.Select();
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(0, (int)Columns.Amount, false);

                return;
            }

            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Cost].Value = ft.TotCost;
            fTemp.FT.OwnCost = ft.OwnCost;
            fTemp.FT.TotCost = ft.TotCost;
            fTemp.FT.PayCost = ft.PayCost;
            fTemp.FT.PubCost = ft.PubCost;

            this.fpSpread1_Sheet1.SetActiveCell(0, (int)Columns.Price, false);
        }

        /// <summary>
        /// 添加自费诊金
        /// </summary>
        public void AddOwnDiagFee()
        {
            if (this.rInfo == null || this.tempDept == null)
            {
                return;
            }

            RegLvlFee tempRegFeeOfPact = this.registerIntegrate.GetRegLevelByPactCode(this.rInfo.Pact.ID, this.comRegLevel);
            if (tempRegFeeOfPact == null)
            {
                return;
            }
            if (tempRegFeeOfPact.OwnDigFee <= 0)
            {
                return;
            }

            if (this.ownDiagFeeCode == null || this.ownDiagFeeCode == "无" || this.ownDiagFeeCode == string.Empty || this.ownDiagFeeCode == "-1")
            {
                return;
            }

            if (this.fpSpread1_Sheet1.RowCount > 0)
            {
                for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                {
                    if (this.fpSpread1_Sheet1.Rows[i].Tag != null)
                    {
                        if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                        {
                            FeeItemList fSame = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                            if (fSame.Item.ID == this.ownDiagFeeCode && fSame.NewItemRate == 1)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            if (this.rInfo.PID.CardNO.Substring(0, 1) != this.noRegFlagChar)
            {
                return;
            }

            DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + this.ownDiagFeeCode + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                return;
            }

            this.fpSpread1_Sheet1.Rows.Add(0, 1);
            //清空
            this.alAddRows.Clear();
            //{40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
            this.SetItem(this.ownDiagFeeCode, "0", this.rInfo.DoctorInfo.Templet.Dept.ID, 0, 1, 0, "0");

            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Price].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.ExeDept].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Amount].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.CombNo].Locked = false;
            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Price].Text = tempRegFeeOfPact.OwnDigFee.ToString();
            this.fpSpread1_Sheet1.SetValue(0, (int)Columns.Price, tempRegFeeOfPact.OwnDigFee.ToString());

            FeeItemList fTemp = this.fpSpread1_Sheet1.Rows[0].Tag as FeeItemList;

            fTemp.Item.Price = tempRegFeeOfPact.OwnDigFee;
            fTemp.OrgPrice = fTemp.Item.Price;

            Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(fTemp.Item.Price, fTemp.Item.Qty, fTemp);

            if (ft == null)
            {
                this.fpSpread1.Select();
                this.fpSpread1.Focus();
                this.fpSpread1_Sheet1.SetActiveCell(0, (int)Columns.Amount, false);

                return;
            }

            this.fpSpread1_Sheet1.Cells[0, (int)Columns.Cost].Value = ft.TotCost;
            fTemp.FT.OwnCost = ft.OwnCost;
            fTemp.FT.TotCost = ft.TotCost;
            fTemp.FT.PayCost = ft.PayCost;
            fTemp.FT.PubCost = ft.PubCost;
            fTemp.ItemRateFlag = "1";
            fTemp.OrgItemRate = this.rInfo.Pact.Rate.PayRate;
            fTemp.NewItemRate = 1;
            this.fpSpread1_Sheet1.SetActiveCell(0, (int)Columns.Price, false);

            this.SumCost();
        }

        public void SetColumnEnable(int row)
        {
            for (int j = 0; j < this.fpSpread1_Sheet1.Columns.Count; j++)
            {
                if (j == (int)Columns.InputCode || j == (int)Columns.Select || j == (int)Columns.MachineNO || j == (int)Columns.IsSend)
                {
                    this.fpSpread1_Sheet1.Cells[row, j].Locked = false;
                }
                else
                {
                    this.fpSpread1_Sheet1.Cells[row, j].Locked = true;
                }
            }
        }

        //获取医保等级名称
        private string GetItemGradeName(string itemGradeCode)
        {
            switch (itemGradeCode)
            {
                case "1":
                    return "甲类";
                case "2":
                    return "乙类";
                case "3":
                    return "自费";
                default:
                    return "自费";
            }

        }

        //获取医保记账类型
        private string GetGFItemTypeName(decimal newRate)
        {
            if (newRate >= 1)
            {
                return "自费";
            }
            else
            {
                return "公费";
            }
        }

        //获取医保记账类型
        private string GetSIItemTypeName(string itemGradeCode)
        {
            switch (itemGradeCode)
            {
                case "1":
                case "2":
                    return "记账";
                case "3":
                default:
                    return "自费";
            }
        }

        /// <summary>
        /// 设置项目的自付比例，医保公医类型，结算类型
        /// </summary>
        /// <param name="row"></param>
        /// <param name="feeItemList"></param>
        private void SetItemRateInfo(int row, FeeItemList feeItemList)
        {
            //医保信息
            try
            {
                //对照信息为空或者对照信息与原始对照信息不同时
                if (feeItemList.Compare == null || feeItemList.Compare.ID != this.rInfo.Pact.ID)
                {
                    if (this.rInfo.Pact.PayKind.ID == "03"
                        ||
                        (this.isOwnDisplayYB && this.rInfo.Pact.PayKind.ID == "01")
                       )
                    {
                        //做个参数维护广州医保的合同单位代码
                        this.interfaceManager.GetCompareSingleItem(this.YBPactCode, feeItemList.Item.ID, ref feeItemList.Compare);
                        //取公费的自付比例，根据项目编码、最小费用编码查询自付比例信息
                    }
                    else
                    {
                        this.interfaceManager.GetCompareSingleItem(this.rInfo.Pact.ID, feeItemList.Item.ID, ref feeItemList.Compare);
                    }

                    feeItemList.Compare.ID = this.rInfo.Pact.ID;
                }
            }
            catch { }

            #region 自费

            if (this.rInfo.Pact.PayKind.ID == "01")//自费
            {
                feeItemList.OrgItemRate = 1;
                feeItemList.NewItemRate = 1;
                feeItemList.ItemRateFlag = "1";
                if (feeItemList.Compare != null && this.isOwnDisplayYB)
                {
                    //结算类型
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Value = this.GetSIItemTypeName(feeItemList.Compare.CenterItem.ItemGrade);
                }
                else
                {
                    //结算类型
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Value = this.GetSIItemTypeName(string.Empty);
                }

            }

            #endregion

            #region 公费和军人
            if (this.rInfo.Pact.PayKind.ID == "03" || this.rInfo.Pact.ID == "2")
            {
                Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                if (feeItemList.Item.ID != "999")
                {
                    DataRow findRow = null;
                    DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + feeItemList.Item.ID + "'");
                    if (rowFinds == null || rowFinds.Length == 0)
                    {
                        MessageBox.Show("编码为: [" + feeItemList.Item.ID + " ] 的项目查找失败!");
                        return;
                    }
                    findRow = rowFinds[0];

                    if (findRow["ZF"].ToString() == "1")
                    {
                        pactRate = new Neusoft.HISFC.Models.Base.PactItemRate();
                        pactRate.Rate.PayRate = 1;
                    }
                }

                //默认取比例
                if (pactRate == null)
                {
                    if (dictionaryPactItemRate.ContainsKey(this.rInfo.Pact.ID + feeItemList.Item.ID))
                    {
                        pactRate = dictionaryPactItemRate[this.rInfo.Pact.ID + feeItemList.Item.ID];
                    }
                    else
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnePactUnitItemRate(this.rInfo.Pact.ID, feeItemList.Item.MinFee.ID, feeItemList.Item.ItemType == EnumItemType.Drug ? 1 : 2, feeItemList.Item.ID);
                        if (pactRate != null)
                        {
                            if (dictionaryPactItemRate.ContainsKey(this.rInfo.Pact.ID + feeItemList.Item.ID))
                            {
                                dictionaryPactItemRate[this.rInfo.Pact.ID + feeItemList.Item.ID] = pactRate;
                            }
                        }
                    }
                }

                if (pactRate != null)
                {
                    #region  自付比例显示
                    if (feeItemList.ItemRateFlag != "3")
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeItemList.ItemRateFlag = "1";
                                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "自费";
                                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].ForeColor = Color.Red;
                            }
                            else
                            {
                                feeItemList.ItemRateFlag = "2";
                                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "记账";
                            }
                        }
                        else
                        {
                            feeItemList.ItemRateFlag = "2";
                            this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "记账";
                        }
                        feeItemList.OrgItemRate = this.rInfo.Pact.Rate.PayRate;
                        feeItemList.NewItemRate = pactRate.Rate.PayRate;
                    }
                    else
                    {
                        feeItemList.ItemRateFlag = "3";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "特殊";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].ForeColor = Color.Blue;
                    }
                    #endregion
                }
                else
                {
                    #region 为空
                    if (feeItemList.ItemRateFlag != "3")
                    {
                        feeItemList.OrgItemRate = this.rInfo.Pact.Rate.PayRate;
                        feeItemList.NewItemRate = this.rInfo.Pact.Rate.PayRate;
                        feeItemList.ItemRateFlag = "2";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "记账";
                    }
                    else
                    {
                        feeItemList.ItemRateFlag = "3";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "特殊";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].ForeColor = Color.Blue;
                    }
                    #endregion
                }

            }

            #endregion
            #region 公医自付比例获取
            if (this.rInfo.Pact.Name == "公费")
            {

                Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                pactRate = this.pactUnitItemRateManager.GetOnePactUnitItemRateGY(rInfo.Name, rInfo.IDCard, feeItemList.Item.ItemType == EnumItemType.Drug ? 1 : 2);
                if (pactRate != null)
                {
                    if (dictionaryPactItemRate.ContainsKey(this.rInfo.Pact.ID + feeItemList.Item.ID))
                    {
                        dictionaryPactItemRate[this.rInfo.Pact.ID + feeItemList.Item.ID] = pactRate;
                    }
                }
                feeItemList.ItemRateFlag = "2";
                this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "记账";
                feeItemList.OrgItemRate = this.rInfo.Pact.Rate.PayRate;
                feeItemList.NewItemRate = pactRate.Rate.PayRate;

            }

            #endregion

            #region 医保
            if (this.rInfo.Pact.PayKind.ID == "02")
            {
                if (feeItemList.Compare == null)
                {
                    feeItemList.ItemRateFlag = "1";
                    feeItemList.NewItemRate = 1;
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = "自费";
                    this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].ForeColor = Color.Red;
                }
                else
                {
                    feeItemList.NewItemRate = feeItemList.Compare.CenterItem.Rate;
                    if (feeItemList.NewItemRate == 1)
                    {
                        feeItemList.ItemRateFlag = "1";
                    }
                    else
                    {
                        feeItemList.ItemRateFlag = "2";
                        this.fpSpread1_Sheet1.Cells[row, (int)Columns.Self].Text = this.GetSIItemTypeName(feeItemList.Compare.CenterItem.ItemGrade);
                    }
                }
            }
            #endregion

            #region 自付比例

            //自付比例
            this.fpSpread1_Sheet1.Cells[row, (int)Columns.PayRate].Value = feeItemList.NewItemRate;
            //公费类型
            this.fpSpread1_Sheet1.Cells[row, (int)Columns.GFPactType].Value = this.GetGFItemTypeName(feeItemList.NewItemRate);
            //医保类型
            this.fpSpread1_Sheet1.Cells[row, (int)Columns.SIPactType].Value = this.GetItemGradeName(feeItemList.Compare == null ? string.Empty : feeItemList.Compare.CenterItem.ItemGrade);

            #endregion

        }

        /// <summary>
        /// 显示列名
        /// </summary>
        /// <param name="currRow"></param>
        /// <param name="f"></param>
        /// <param name="rowFind"></param>
        private void SetRowHeader(int currRow, FeeItemList f, DataRow rowFind)
        {
            if (f.Item.ID != "999")
            {
                string itemType = rowFind["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    f.Item.ItemType = EnumItemType.UnDrug;
                    f.IsGroup = false;
                    SetItemDisplay(currRow, Color.BlueViolet, "非药", new Font("宋体", 9, FontStyle.Bold));
                }
                if (itemType == "1")
                {
                    f.Item.ItemType = EnumItemType.Drug;
                    f.IsGroup = false;

                    SetItemDisplay(currRow, Color.Red, "药品", new Font("宋体", 9, FontStyle.Bold));
                }
                if (itemType == "2")
                {
                    f.Item.ItemType = EnumItemType.UnDrug;
                    f.IsGroup = true;
                    SetItemDisplay(currRow, Color.Salmon, "组合", new Font("宋体", 9, FontStyle.Bold));
                }

                if (itemType == "4")
                {
                    f.Item.ItemType = EnumItemType.Drug;
                    f.IsNostrum = true;
                    SetItemDisplay(currRow, Color.Sienna, "协定", new Font("宋体", 9, FontStyle.Bold));
                }

            }
            else if (f.Item.ID == "999")
            {
                if (f.Item.ItemType == EnumItemType.Drug)
                {
                    SetItemDisplay(currRow, Color.Red, "药品", new Font("宋体", 9, FontStyle.Bold));
                }
                else
                {
                    SetItemDisplay(currRow, Color.BlueViolet, "非药", new Font("宋体", 9, FontStyle.Bold));
                }
            }
        }

        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 按键处理
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            try
            {
                if (keyData == Keys.Left)
                {
                    PutArrow(Keys.Left);
                }
                if (keyData == Keys.Right)
                {
                    PutArrow(Keys.Right);
                }

                if (this.fpSpread1.ContainsFocus)
                {
                    if (keyData == Keys.Escape)
                    {
                        if (lbDept.Visible)
                        {
                            lbDept.Visible = false;
                        }
                        if (lbFreq.Visible)
                        {
                            lbFreq.Visible = false;
                        }
                        if (this.lbMachineNO.Visible)
                        {
                            lbMachineNO.Visible = false;
                        }
                        if (lbUsage.Visible)
                        {
                            lbUsage.Visible = false;
                        }
                        if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging && ((Control)this.chooseItemControl).Visible)
                        {
                            ((Control)this.chooseItemControl).Visible = false;
                        }
                    }

                    if (keyData == Keys.Down)
                    {
                        if (lbDept.Visible)
                        {
                            lbDept.NextRow();
                        }
                        else if (lbFreq.Visible)
                        {
                            lbFreq.NextRow();
                        }
                        else if (lbUsage.Visible)
                        {
                            lbUsage.NextRow();
                        }
                        else if (((Control)this.chooseItemControl).Visible && this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging)
                        {
                            this.chooseItemControl.NextRow();
                        }
                        else if (this.lbMachineNO.Visible)
                        {
                            lbMachineNO.NextRow();
                        }
                        else
                        {
                            string temp = this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.ItemName].Text;
                            if (temp != string.Empty)
                            {
                                AddRow(this.fpSpread1_Sheet1.ActiveRowIndex);
                            }
                            RefreshItemInfo();
                        }
                    }
                    if (keyData == Keys.Up)
                    {
                        if (lbDept.Visible)
                        {
                            lbDept.PriorRow();
                        }
                        else if (this.lbMachineNO.Visible)
                        {
                            lbMachineNO.PriorRow();
                        }
                        else if (lbFreq.Visible)
                        {
                            lbFreq.PriorRow();
                        }
                        else if (lbUsage.Visible)
                        {
                            lbUsage.PriorRow();
                        }
                        else if (((Control)this.chooseItemControl).Visible && this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging)
                        {
                            this.chooseItemControl.PriorRow();
                        }
                        else
                        {
                            int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
                            if (currRow > 0)
                            {
                                this.fpSpread1_Sheet1.ActiveRowIndex = currRow - 1;
                                this.fpSpread1_Sheet1.SetActiveCell(currRow - 1, this.fpSpread1_Sheet1.ActiveColumnIndex);
                            }
                            RefreshItemInfo();
                            //this.fpSpread1.StopCellEditing();
                        }
                    }

                    #region 热键
                    if (keyData.GetHashCode() == Keys.Control.GetHashCode() + Keys.I.GetHashCode())
                    {
                        int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
                        this.fpSpread1_Sheet1.Rows.Add(currRow + 1, 1);
                        this.fpSpread1_Sheet1.SetActiveCell(currRow + 1, 0);

                        SumLittleCostAll();
                    }
                    if (keyData.GetHashCode() == Keys.Control.GetHashCode() + Keys.E.GetHashCode())
                    {
                        this.ModifyDays();
                    }

                    #endregion

                    if (keyData == Keys.Enter)
                    {
                        int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
                        int currColumn = this.fpSpread1_Sheet1.ActiveColumnIndex;

                        this.isDealCellChange = false;

                        this.fpSpread1.StopCellEditing();
                        FeeItemList feeItem = null;//当前项目信息
                        //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                        if (currColumn != (int)Columns.InputCode && currColumn != (int)Columns.Select)
                        {
                            if (!IsInputItem(currRow, ref feeItem))
                            {
                                this.isDealCellChange = true;

                                return false;
                            }
                        }
                        #region 输入码

                        //如果当前列是项目编码，则查找项目
                        //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                        if (currColumn == (int)Columns.InputCode)
                        {
                            if (this.rInfo == null)
                            {
                                MessageBox.Show(Language.Msg("请选择患者"));

                                this.isDealCellChange = true;
                                return false;
                            }

                            if (isInputItemsNoSpe)
                            {
                                if (this.rInfo.DoctorInfo.Templet.Dept.ID == null || this.rInfo.DoctorInfo.Templet.Dept.ID == string.Empty)
                                {

                                    MessageBox.Show(Language.Msg("请选择看诊科室!"));

                                    this.isDealCellChange = true;

                                    return false;

                                }
                            }
                            if (this.fpSpread1_Sheet1.Rows[currRow].Tag != null)
                            {
                                if (this.fpSpread1_Sheet1.Rows[currRow].Tag is FeeItemList)
                                {
                                    feeItem = (FeeItemList)fpSpread1_Sheet1.Rows[currRow].Tag;
                                }
                            }
                            if (!this.isCanModifyCharge)
                            {
                                if (feeItem != null)
                                {
                                    //非手工方，不允许直接覆盖删除 gumzh
                                    if (feeItem.FTSource != "0" && feeItem.Order.ID != null && feeItem.Order.ID != string.Empty)
                                    {
                                        this.isDealCellChange = true;

                                        return false;
                                    }
                                }
                            }
                            if (isInputItemsNoSpe)
                            {
                                if (!this.isCanAddItem && !this.isQuitFee)
                                {
                                    MessageBox.Show(Language.Msg("请单选择一条处方进行增加项目!"));
                                    this.fpSpread1.Focus();
                                    //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                    this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.InputCode, false);

                                    this.isDealCellChange = true;

                                    return false;
                                }
                            }
                            if (feeItem != null)
                            {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                string sTempText = this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text;
                                if (sTempText == feeItem.Item.UserCode)
                                {
                                    //if (feeItem.Item.IsPharmacy && feeItem.Item.SysClass.ID.ToString() == "PCC")//草药,并且不是协定处方
                                    if (feeItem.Item.ItemType == EnumItemType.Drug && feeItem.Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)//草药
                                    {
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.DoseOnce, false);
                                    }
                                    else
                                    {
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Amount, false);
                                    }
                                }
                                else
                                {
                                    if (feeItem.PayType == Neusoft.HISFC.Models.Base.PayTypes.Charged)
                                    {
                                        if (DeleteRow(feeItem) == -1)
                                        {
                                            return false;
                                        }
                                    }

                                    //if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemInputEnd)
                                    //{  //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                    //    QueryItem(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text, currRow);
                                    //}
                                    //else 
                                    //{
                                    //    this.chooseItemControl.GetSelectedItem();
                                    //}
                                    if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemInputEnd)
                                    {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                        QueryItem(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text, currRow);
                                    }
                                    else
                                    {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                        if (this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text.Trim() == string.Empty)
                                        {
                                            if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging && (this.chooseItemControl as Control).Visible == true)
                                            {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                                this.chooseItemControl.GetSelectedItem();
                                            }
                                        }
                                        else
                                        {
                                            this.chooseItemControl.GetSelectedItem();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemInputEnd)
                                {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                    QueryItem(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text, currRow);
                                }
                                else
                                {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                    if (this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.InputCode].Text.Trim() == string.Empty)
                                    {
                                        if (this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging && (this.chooseItemControl as Control).Visible == true)
                                        {//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                            this.chooseItemControl.GetSelectedItem();
                                        }
                                    }
                                    else
                                    {
                                        this.chooseItemControl.GetSelectedItem();
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 数量

                        //数量
                        if (currColumn == (int)Columns.Amount)
                        {
                            decimal price = 0;
                            try
                            {
                                price = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Text);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("价格输入错误!" + ex.Message);
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Amount, false);

                                this.isDealCellChange = true;

                                return false;
                            }
                            decimal qty = 0;
                            if (price == 0)//项目没有价格，直接跳转到输入价格的位置
                            {
                                //if (feeItem.Item.IsPharmacy)
                                if (feeItem.Item.ItemType == EnumItemType.Drug)
                                {
                                    FarPoint.Win.Spread.CellType.ComboBoxCellType type =
                                        (FarPoint.Win.Spread.CellType.ComboBoxCellType)this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.PriceUnit].CellType;
                                    type.ListControl.SelectedIndex = 0;
                                }

                                //{0F98A513-A9EA-4110-B35F-E353A390E350}
                                //如果可以输入负数量,数量的下限制为-99999，否则为0
                                decimal minValue = 0;
                                if (this.isCanInputNegativeQty)
                                {
                                    minValue = -99999;
                                }
                                else
                                {
                                    minValue = 0;
                                }

                                bool bReturn = InputDataIsValid(currRow, (int)Columns.Amount, "数量", 99999, minValue, ref qty);
                                //{0F98A513-A9EA-4110-B35F-E353A390E350}修改完毕
                                if (!bReturn)
                                {
                                    this.isDealCellChange = true;

                                    return false;
                                }

                                #region 判断是否上取整

                                //if (this.isQtyToCeiling && feeItem.Item.IsPharmacy)
                                if (this.isQtyToCeiling && feeItem.Item.ItemType == EnumItemType.Drug)
                                {
                                    double qtyValue = System.Convert.ToDouble(qty);

                                    qtyValue = System.Math.Ceiling(qtyValue);

                                    qty = NConvert.ToDecimal(qtyValue);
                                }

                                #endregion

                                qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);
                                if (feeItem.FeePack == "1")//包装单位
                                {
                                    feeItem.Item.Qty = qty * feeItem.Item.PackQty;
                                }
                                else
                                {
                                    feeItem.Item.Qty = qty;
                                }
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Amount].Text = qty.ToString();
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Price].Locked = false;
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Price);
                            }
                            else
                            {

                                //{0F98A513-A9EA-4110-B35F-E353A390E350}
                                //如果可以输入负数量,数量的下限制为-99999，否则为0
                                decimal minValue = 0;
                                if (this.isCanInputNegativeQty)
                                {
                                    minValue = -99999;
                                }
                                else
                                {
                                    minValue = 0;
                                }

                                bool bReturn = InputDataIsValid(currRow, (int)Columns.Amount, "数量", 99999, minValue, ref qty);
                                //{0F98A513-A9EA-4110-B35F-E353A390E350}修改完毕
                                if (!bReturn)
                                {
                                    this.isDealCellChange = true;

                                    return false;
                                }

                                #region 判断是否上取整

                                //if (this.isQtyToCeiling && feeItem.Item.IsPharmacy)
                                if (this.isQtyToCeiling && feeItem.Item.ItemType == EnumItemType.Drug)
                                {
                                    double qtyValue = System.Convert.ToDouble(qty);

                                    qtyValue = System.Math.Ceiling(qtyValue);

                                    qty = NConvert.ToDecimal(qtyValue);
                                }

                                #endregion

                                qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);
                                if (feeItem.FeePack == "1")//包装单位
                                {
                                    feeItem.Item.Qty = qty * feeItem.Item.PackQty;
                                }
                                else
                                {
                                    feeItem.Item.Qty = qty;
                                }
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Amount].Text = qty.ToString();

                                Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, qty, feeItem);

                                if (ft == null)
                                {
                                    this.fpSpread1.Select();
                                    this.fpSpread1.Focus();
                                    this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Amount, false);

                                    this.isDealCellChange = true;

                                    return false;
                                }

                                feeItem.FT.TotCost = ft.TotCost;
                                feeItem.FT.OwnCost = ft.OwnCost;
                                feeItem.FT.PayCost = ft.PayCost;
                                feeItem.FT.PubCost = ft.PubCost;

                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Cost].Value = ft.TotCost;

                                //if (feeItem.Item.IsPharmacy)
                                if (feeItem.Item.ItemType == EnumItemType.Drug)
                                {
                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.DoseOnce].Locked = false;
                                    if (feeItem.Invoice.User01 == "1")//不可以拆分包装单位
                                    {
                                        this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.PriceUnit].Locked = true;
                                        this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.DoseOnce].Locked = false;
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.DoseOnce);
                                    }
                                    else
                                    {
                                        this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.PriceUnit].Locked = false;
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.DoseOnce, false);
                                    }
                                    //begin这里判断库存最好 zhouxs by 2007-10-17
                                    if (!IsStoreEnough(feeItem, currRow))
                                    {
                                        return false;
                                    }
                                    //end zhouxs
                                }
                                else//非药品
                                {
                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ExeDept].Locked = false;
                                    if (feeItem.Item.SysClass.ID.ToString() == "UL")
                                    {
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.CombNo, false);
                                    }
                                    else
                                    {
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.ExeDept, false);
                                    }
                                }
                                if (feeItem.FTSource != "1")
                                {
                                    string temp = this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRowIndex, (int)Columns.ItemName].Text;
                                    if (temp != string.Empty)
                                    {
                                        AddRow(this.fpSpread1_Sheet1.ActiveRowIndex);
                                    }
                                    RefreshItemInfo();
                                }
                            }


                            this.SumCost();
                        }
                        #endregion

                        #region 付数
                        if (currColumn == (int)Columns.Days)
                        {
                            decimal qty = 0; //数量
                            decimal days = 0; //付数
                            decimal price = 0; //单价
                            decimal totQty = 0; //总数量(计算付数后)

                            //if (feeItem.Item.IsPharmacy)
                            if (feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                //草药
                                if (feeItem.Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                {

                                    bool bReturn = InputDataIsValid(currRow, (int)Columns.Days, "付数", 9999, 0, ref days);
                                    if (!bReturn)
                                    {
                                        this.isDealCellChange = true;

                                        return false;
                                    }

                                    feeItem.Days = days;
                                    if (days != this.hDays)
                                    {
                                        hDays = days;
                                    }

                                    bReturn = InputDataIsValid(currRow, (int)Columns.Price, "单价", 99999, 0, ref price);
                                    if (!bReturn)
                                    {
                                        this.isDealCellChange = true;

                                        return false;
                                    }

                                    bReturn = InputDataIsValid(currRow, (int)Columns.DoseOnce, "每次用量", 99999, 0, ref qty);
                                    if (!bReturn)
                                    {
                                        this.isDealCellChange = true;

                                        return false;
                                    }

                                    qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);
                                    // {1FAD3FA2-C7D8-4cac-845F-B9EBECDE2312}
                                    totQty = qty * days / ((feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose == 0 ? 1 : (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose);
                                    // totQty = qty * days;
                                    feeItem.Item.Qty = totQty;
                                    this.isDealCellChange = true;
                                    Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, totQty, feeItem);

                                    if (ft == null)
                                    {
                                        this.fpSpread1.Select();
                                        this.fpSpread1.Focus();
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Amount, false);

                                        this.isDealCellChange = true;

                                        return false;
                                    }

                                    feeItem.FT.TotCost = ft.TotCost;
                                    feeItem.FT.OwnCost = ft.OwnCost;
                                    feeItem.FT.PayCost = ft.PayCost;
                                    feeItem.FT.PubCost = ft.PubCost;


                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Amount].Value = totQty;
                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Cost].Value = ft.TotCost;
                                    //{73AA7783-8B97-45f5-B430-0C7311E952C8}  
                                    this.SumCost();
                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.CombNo].Locked = false;
                                    this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.CombNo, false);

                                    // 草药处理
                                    if (this.isDoseOnceCanNull)
                                    {
                                        AddRow(this.fpSpread1_Sheet1.ActiveRowIndex);

                                        RefreshItemInfo();
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 每次用量
                        if (currColumn == (int)Columns.DoseOnce)
                        {
                            //if (feeItem.Item.IsPharmacy)
                            if (feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                decimal doseOnce = 0;

                                if (!this.isDoseOnceNull)//每次用量不能为空
                                {
                                    bool bReturn = InputDataIsValid(currRow, (int)Columns.DoseOnce, "每次用量", 99999, 0, ref doseOnce);
                                    if (!bReturn)
                                    {
                                        this.isDealCellChange = true;

                                        return false;
                                    }
                                }
                                else
                                {
                                    InputDataIsValid(currRow, (int)Columns.DoseOnce, "每次用量", 99999, 0, ref doseOnce, false);
                                }
                                //如果是草药,并且不是协定处方,数量是有剂量和负数相乘计算.
                                if (feeItem.Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                {
                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Days].Locked = false;
                                    this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Days, false);

                                    this.isDealCellChange = true;

                                    #region {46DA2449-F37C-45bf-B39F-8B8EEF5A6F00} 向实体写入每次用量
                                    feeItem.Order.DoseOnce = doseOnce;
                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.DoseOnce].Value = feeItem.Order.DoseOnce;
                                    #endregion

                                    return false;
                                }

                                feeItem.Order.DoseOnce = doseOnce;

                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.DoseOnce].Value = feeItem.Order.DoseOnce;
                            }

                            this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.CombNo].Locked = false;
                            this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.CombNo, false);
                        }
                        #endregion

                        #region 组合号
                        if (currColumn == (int)Columns.CombNo)
                        {
                            string strCombNo = this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.CombNo].Text;
                            if (strCombNo.Length > 14)
                            {
                                MessageBox.Show("组合号输入不能超过14位!");
                                this.fpSpread1.Focus();
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.CombNo);

                                this.isDealCellChange = true;

                                return false;
                            }
                            feeItem.Order.Combo.ID = strCombNo;

                            this.SumCost();
                            //if (feeItem.Item.IsPharmacy)
                            if (feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                if (currRow > 0)
                                {
                                    #region 获得第一个和当前行具有相同组合号的行号

                                    int combNoIndex = -1;
                                    for (int i = 0; i < this.fpSpread1_Sheet1.Rows.Count; i++)
                                    {
                                        if (this.fpSpread1_Sheet1.Rows[i].Tag is FeeItemList)
                                        {
                                            if (i == currRow)
                                            {
                                                continue;
                                            }

                                            FeeItemList fTemp = this.fpSpread1_Sheet1.Rows[i].Tag as FeeItemList;
                                            //if (fTemp.Item.IsPharmacy)
                                            if (fTemp.Item.ItemType == EnumItemType.Drug)
                                            {
                                                if (feeItem.Order.Combo.ID == fTemp.Order.Combo.ID && feeItem.Order.Combo.ID != string.Empty)
                                                {
                                                    combNoIndex = i;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                    if (combNoIndex != -1)
                                    {
                                        FeeItemList fTemp = this.fpSpread1_Sheet1.Rows[combNoIndex].Tag as FeeItemList;
                                        //if (fTemp.Item.IsPharmacy)
                                        if (fTemp.Item.ItemType == EnumItemType.Drug)
                                        {
                                            if (feeItem.Order.Combo.ID == fTemp.Order.Combo.ID && feeItem.Order.Combo.ID != string.Empty)
                                            {
                                                feeItem.Order.Frequency.ID = fTemp.Order.Frequency.ID;
                                                feeItem.Order.Frequency.Name = fTemp.Order.Frequency.Name;
                                                feeItem.Order.Usage.ID = fTemp.Order.Usage.ID;
                                                feeItem.Order.Usage.Name = fTemp.Order.Usage.Name;
                                                if (freqDisplayType == "0")//汉字
                                                {
                                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text = feeItem.Order.Frequency.Name;
                                                }
                                                else
                                                {
                                                    this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text = feeItem.Order.Frequency.ID;
                                                }

                                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Text = feeItem.Order.Usage.Name;
                                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Locked = false;
                                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Locked = false;
                                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ExeDept].Locked = false;
                                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.ExeDept, false);

                                                this.isDealCellChange = true;

                                                //return true;
                                            }
                                        }
                                    }
                                }
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Locked = false;
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Freq, false);
                            }
                            else
                            {
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ExeDept].Locked = false;
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.ExeDept, false);
                            }

                            this.DrawCombo(this.fpSpread1_Sheet1, (int)Columns.CombNo, (int)Columns.CombNoDisplay, 0);
                        }

                        #endregion

                        #region 频次
                        if (currColumn == (int)Columns.Freq)
                        {
                            if (this.ProcessFreq() == -1)
                            {
                                try
                                {
                                    //if (feeItem.Item.IsPharmacy)
                                    if (feeItem.Item.ItemType == EnumItemType.Drug)
                                    {
                                        //去掉对频次非空的判断　2007-8-24 luzhp@neusoft.com
                                        if (!this.isDoseOnceNull)
                                        {
                                            if (this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text == string.Empty)
                                            {
                                                MessageBox.Show("请输入药品的频次!");
                                                this.fpSpread1.Focus();
                                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Freq);

                                                this.isDealCellChange = true;

                                                return false;
                                            }
                                        }

                                        if (this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text != string.Empty)
                                        {
                                            if (freqDisplayType == "0")//汉字
                                            {
                                                feeItem.Order.Frequency.ID =
                                                    myHelpFreq.GetID(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text);
                                            }
                                            else
                                            {
                                                string tmpName = myHelpFreq.GetName(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text);
                                                if (tmpName == null || tmpName == string.Empty)
                                                {
                                                    MessageBox.Show("频次代码输入错误!");
                                                    this.fpSpread1.Focus();
                                                    this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Freq);

                                                    this.isDealCellChange = true;

                                                    return false;
                                                }
                                                feeItem.Order.Frequency.ID = this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Freq].Text;
                                            }
                                            if (feeItem.Order.Frequency.ID == null || feeItem.Order.Frequency.ID == string.Empty)
                                            {
                                                MessageBox.Show("频次代码输入错误!");
                                                this.fpSpread1.Focus();
                                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Freq);

                                                this.isDealCellChange = true;

                                                return false;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);

                                    this.isDealCellChange = true;

                                    return false;
                                }
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Locked = false;
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Usage, false);
                            }
                            this.DealFreqOrUsageHaveSameCombNo(currRow, feeItem.Order.Combo.ID, feeItem.Order.Frequency, "1");
                        }
                        #endregion

                        #region 用法
                        if (currColumn == (int)Columns.Usage)
                        {
                            if (this.ProcessUsage() == -1)
                            {
                                try
                                {
                                    //if (feeItem.Item.IsPharmacy)
                                    if (feeItem.Item.ItemType == EnumItemType.Drug)
                                    {
                                        // 通过用量属性来判断用法是否可以为空　2007-8-24 路志鹏
                                        if (!this.isDoseOnceNull)
                                        {
                                            if (this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Text == string.Empty)
                                            {
                                                MessageBox.Show("请输入药品的用法!");
                                                this.fpSpread1.Focus();
                                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Usage);

                                                this.isDealCellChange = true;

                                                return false;
                                            }
                                        }
                                        if (this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Text != string.Empty)
                                        {
                                            feeItem.Order.Usage.ID = myHelpUsage.GetID(this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Usage].Text);
                                            if (feeItem.Order.Usage.ID == null || feeItem.Order.Usage.ID == string.Empty)
                                            {
                                                MessageBox.Show("药品的用法输入不正确");
                                                this.fpSpread1.Focus();
                                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Usage);

                                                this.isDealCellChange = true;

                                                return false;
                                            }

                                            alInjec = this.outpatientManager.GetInjectInfoByUsage(feeItem.Order.Usage.ID);
                                            if (alInjec == null)
                                            {
                                                MessageBox.Show("获得院注项目出错!");

                                                this.isDealCellChange = true;

                                                return false;
                                            }
                                            if (alInjec.Count > 0)
                                            {
                                                Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(myInjec);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);

                                    this.isDealCellChange = true;

                                    return false;
                                }

                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ExeDept].Locked = false;
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.ExeDept, false);
                            }
                            //针对中草药 组合需要使用不同的用法
                            if (feeItem.Item.SysClass.ID.ToString() != Neusoft.HISFC.Models.Base.EnumSysClass.PCC.ToString())
                            {
                                this.DealFreqOrUsageHaveSameCombNo(currRow, feeItem.Order.Combo.ID, feeItem.Order.Usage, "2");
                            }
                        }

                        #endregion

                        #region 执行科室
                        if (currColumn == (int)Columns.ExeDept)
                        {
                            if (ProcessDept() == -1)
                            {
                                this.isDealCellChange = true;

                                return false;
                            }

                            if (injec > 0)
                            {

                                int actIndex = this.fpSpread1_Sheet1.RowCount - 1;
                                //int tmpRow = currRow;
                                foreach (NeuObject obj in alInjec)
                                {
                                    DataRow rowFind;
                                    DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + obj.ID + "'");

                                    if (rowFinds == null || rowFinds.Length == 0)
                                    {
                                        MessageBox.Show("查找院注项目出错!");

                                        this.isDealCellChange = true;

                                        return false;
                                    }
                                    rowFind = rowFinds[0];
                                    try
                                    {
                                        feeItem.InjectCount = NConvert.ToInt32(injec);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("院注次数输入不合法!" + ex.Message);
                                        this.fpSpread1.Focus();
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Usage, false);

                                        this.isDealCellChange = true;

                                        return false;
                                    }
                                    if (feeItem.InjectCount > 99)
                                    {
                                        MessageBox.Show("院内注射次数不能大于99!");
                                        this.fpSpread1.Focus();
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Usage, false);

                                        this.isDealCellChange = true;

                                        return false;

                                    }
                                    if (feeItem.Order.Combo.ID != null && feeItem.Order.Combo.ID != string.Empty)
                                    {
                                        RefreshSameCombNoInjects(feeItem.Order.Combo.ID, feeItem.InjectCount);
                                    }

                                    actIndex = GetNewRow();
                                    if (actIndex == -1)
                                    {
                                        this.fpSpread1.StopCellEditing();
                                        this.fpSpread1_Sheet1.Rows.Add(this.fpSpread1_Sheet1.RowCount, 1);
                                        actIndex = this.fpSpread1_Sheet1.RowCount - 1;
                                        //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Select].Value = true;
                                    }

                                    //清空
                                    this.alAddRows.Clear();
                                    string drugflag = "0";
                                    if (obj.ID.Substring(0, 1) != "F")
                                    {
                                        drugflag = "2";
                                    }
                                    //{40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                                    SetItem(rowFind["ITEM_CODE"].ToString(), drugflag, rowFind["EXE_DEPT"].ToString(), actIndex, 1, NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Price].Text), "0");
                                    this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Amount].Text = injec.ToString();
                                    ((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).Item.Qty = injec;
                                    ((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).Item.IsMaterial = true;
                                    //if (((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).Item.IsPharmacy)
                                    if (((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).Item.ItemType == EnumItemType.Drug)
                                    {
                                        if (((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                        {
                                            this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Days].Locked = false;
                                        }
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Amount].Locked = false;
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.DoseOnce].Locked = false;
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Freq].Locked = false;
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Usage].Locked = false;
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.ExeDept].Locked = false;
                                    }
                                    else
                                    {
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Amount].Locked = false;
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.ExeDept].Locked = false;
                                    }


                                    decimal price = 0;
                                    try
                                    {
                                        price = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Price].Text);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("价格输入不合法" + ex.Message);
                                        this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Days, false);

                                        this.isDealCellChange = true;

                                        return false;
                                    }

                                    decimal qty = 0;
                                    decimal cost = 0;
                                    if (price == 0)//项目没有价格，直接跳转到输入价格的位置
                                    {
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Price].Locked = false;
                                        this.fpSpread1_Sheet1.SetActiveCell(actIndex, (int)Columns.Price);
                                    }
                                    else
                                    {
                                        qty = injec;
                                        cost = Neusoft.FrameWork.Public.String.FormatNumber(price * qty, 2);
                                        ((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).FT.TotCost = cost;
                                        this.fpSpread1_Sheet1.Cells[actIndex, (int)Columns.Cost].Value = cost;
                                    }

                                }

                                // 最后一行是空行时会报错，临时处理下。
                                if (this.fpSpread1_Sheet1.Rows[actIndex].Tag != null)
                                {
                                    ((FeeItemList)this.fpSpread1_Sheet1.Rows[actIndex].Tag).InjectCount = (int)injec;
                                }
                            }
                            if (injec == 0)
                            {
                                AddRow(currRow);
                            }
                            else
                            {
                                AddRow(this.fpSpread1_Sheet1.RowCount - 1);
                            }
                            injec = 0;
                            alInjec = new ArrayList();

                            //ArrayList alFee = this.GetFeeItemListForCharge();
                            //this.FeeItemListChanged(alFee);
                            this.SumCost();

                            this.isDealCellChange = true;

                            return true;
                        }
                        #endregion

                        #region 设备号

                        if (currColumn == (int)Columns.MachineNO)
                        {
                            if (ProcessMachineNO() == -1)
                            {
                                this.isDealCellChange = true;

                                return false;
                            }
                            return true;
                        }

                        #endregion

                        #region 单价

                        //没有价格的项目输入价格后，计算当前行项目金额
                        if (currColumn == (int)Columns.Price)
                        {

                            decimal price = 0;
                            decimal qty = 0;

                            bool bReturn = InputDataIsValid(currRow, (int)Columns.Price, "单价", 999999, 0, ref price);
                            if (!bReturn)
                            {
                                this.isDealCellChange = true;

                                return false;
                            }
                            if (feeItem.FeePack == "0")//最小单位
                            {
                                price = price * feeItem.Item.PackQty;
                            }

                            feeItem.Item.Price = price;
                            feeItem.OrgPrice = feeItem.Item.Price;

                            if (feeItem.Item.Price >= this.priceWarnning)
                            {
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemName].ForeColor =
                                    Color.FromArgb(this.priceWarinningColor);
                            }
                            else
                            {
                                this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ItemName].ForeColor =
                                    Color.Black;
                            }

                            Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, qty, feeItem);

                            if (ft == null)
                            {
                                this.fpSpread1.Select();
                                this.fpSpread1.Focus();
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.Amount, false);

                                this.isDealCellChange = true;

                                return false;
                            }

                            this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.Cost].Value = ft.TotCost;

                            feeItem.FT.OwnCost = ft.OwnCost;
                            feeItem.FT.TotCost = ft.TotCost;
                            feeItem.FT.PayCost = ft.PayCost;
                            feeItem.FT.PubCost = ft.PubCost;
                            this.fpSpread1_Sheet1.Cells[currRow, (int)Columns.ExeDept].Locked = false;
                            this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.ExeDept, false);
                            this.SumCost();
                        }
                        #endregion

                        #region 计价单位
                        if (currColumn == (int)Columns.PriceUnit)
                        {
                            //if (feeItem.Item.IsPharmacy)
                            if (feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                this.fpSpread1_Sheet1.SetActiveCell(currRow, (int)Columns.DoseOnce);
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Focus();
                this.fpSpread1.Focus();

                this.isDealCellChange = true;

                return false;
            }

            this.isDealCellChange = true;

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 当输入内容发生变化时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_Sheet1_CellChanged(object sender, FarPoint.Win.Spread.SheetViewEventArgs e)
        {
            if (!isDealCellChange)
            {
                return;
            }

            if (e == null || sender == null)
            {
                return;
            }

            if (this.fpSpread1_Sheet1.Rows[e.Row].Tag != null)
            {
                if (this.fpSpread1_Sheet1.Rows[e.Row].Tag.GetType() == typeof(FeeItemList))
                {
                    FeeItemList feeItem = this.fpSpread1_Sheet1.Rows[e.Row].Tag as FeeItemList;

                    if (e.Column == (int)Columns.IsSend)
                    {
                        string flag = this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.IsSend].Text;
                        if (flag == "False")
                        {
                            feeItem.IsSend = "0";
                        }
                        else
                        {
                            feeItem.IsSend = "1";
                        }
                    }
                    if (e.Column == (int)Columns.Amount)
                    {
                        decimal price = 0;
                        price = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Price].Value);
                        decimal qty = 0;

                        if (price == 0)//项目没有价格，直接跳转到输入价格的位置
                        {
                            this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Price);
                        }
                        else
                        {
                            try
                            {
                                qty = NConvert.ToDecimal(Neusoft.FrameWork.Public.String.ExpressionVal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Amount].Text.ToString()));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("输入的计算公式不正确，请重新输入!" + ex.Message);
                                this.Enter -= new System.EventHandler(this.ucDisplay_Enter);
                                this.fpSpread1.Focus();
                                this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount);
                                this.Enter += new System.EventHandler(this.ucDisplay_Enter);

                                return;
                            }

                            qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);

                            //是否判断可以输入负数量,当可以输入负数量时,这里不判断
                            //为收费补差价服务{0F98A513-A9EA-4110-B35F-E353A390E350}
                            if (!this.isCanInputNegativeQty)
                            {
                                if (qty <= 0)
                                {
                                    MessageBox.Show("数量不能小于或者等于零,请重新输入");
                                    this.Enter -= new System.EventHandler(this.ucDisplay_Enter);
                                    this.fpSpread1.Select();
                                    this.fpSpread1.Focus();
                                    this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);
                                    this.Enter += new System.EventHandler(this.ucDisplay_Enter);

                                    return;
                                }
                            }//{0F98A513-A9EA-4110-B35F-E353A390E350}结束

                            if (qty > 99999)
                            {
                                MessageBox.Show("数量不能大于99999请重新输入");
                                this.Enter -= new System.EventHandler(this.ucDisplay_Enter);
                                this.fpSpread1.Select();
                                this.fpSpread1.Focus();
                                this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);
                                this.Enter += new System.EventHandler(this.ucDisplay_Enter);

                                return;
                            }

                            #region 判断是否上取整

                            //if (this.isQtyToCeiling && feeItem.Item.IsPharmacy)
                            if (this.isQtyToCeiling && feeItem.Item.ItemType == EnumItemType.Drug)
                            {
                                double qtyValue = System.Convert.ToDouble(qty);

                                qtyValue = System.Math.Ceiling(qtyValue);

                                qty = NConvert.ToDecimal(qtyValue);
                            }

                            this.isDealCellChange = false;

                            this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Amount].Text = qty.ToString();

                            this.isDealCellChange = true;

                            #endregion

                            if (feeItem.FeePack == "1")//包装单位
                            {
                                feeItem.Item.Qty = qty * feeItem.Item.PackQty;
                            }
                            else//最小单位
                            {
                                feeItem.Item.Qty = qty;
                            }

                            Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, qty, feeItem);

                            if (ft == null)
                            {
                                this.fpSpread1.Select();
                                this.fpSpread1.Focus();
                                this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);
                                this.isValid = false;

                                return;
                            }

                            feeItem.FT.TotCost = ft.TotCost;
                            feeItem.FT.OwnCost = ft.OwnCost;
                            feeItem.FT.PubCost = ft.PubCost;
                            feeItem.FT.PayCost = ft.PayCost;
                            //add by niuxy处理优惠
                            feeItem.FT.RebateCost = ft.RebateCost;

                            this.isDealCellChange = false;
                            this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Cost].Value = ft.TotCost;
                            SumCost();
                            this.isDealCellChange = true;
                            this.Focus();
                        }
                    }
                    if (e.Column == (int)Columns.CombNo)
                    {
                        string combNo = this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.CombNo].Text;
                        feeItem.Order.Combo.ID = combNo;
                        if (feeItem.InjectCount == 0)
                        {
                            int injectCount = GetInjectSameCombs(combNo);
                            feeItem.InjectCount = injectCount;
                        }
                        this.DrawCombo(this.fpSpread1_Sheet1, (int)Columns.CombNo, (int)Columns.CombNoDisplay, 0);
                    }
                    if (e.Column == (int)Columns.Usage)
                    {

                    }



                    if (e.Column == (int)Columns.Days)
                    {
                        decimal days = 0;
                        decimal qty = 0;
                        decimal totQty = 0;

                        try
                        {
                            days = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Days].Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("输入的天数不合法" + ex.Message);
                            this.Enter -= new System.EventHandler(this.ucDisplay_Enter);
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Days);
                            this.Enter += new System.EventHandler(this.ucDisplay_Enter);


                            return;
                        }
                        if (days <= 0)
                        {
                            MessageBox.Show("输入的天数不合法, 付数必须大于0");
                            this.Enter -= new System.EventHandler(this.ucDisplay_Enter);
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Days);
                            this.Enter += new System.EventHandler(this.ucDisplay_Enter);


                            return;
                        }
                        qty = NConvert.ToDecimal(Neusoft.FrameWork.Public.String.ExpressionVal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.DoseOnce].Text.ToString()));
                        qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 2);

                        feeItem.Days = days;

                        //{73AA7783-8B97-45f5-B430-0C7311E952C8}    
                        this.hDays = days;
                        // {1FAD3FA2-C7D8-4cac-845F-B9EBECDE2312}
                        totQty = qty * days / ((feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose == 0 ? 1 : (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose);
                        //totQty = qty * days;
                        feeItem.Item.Qty = totQty;

                        Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(feeItem.Item.Price, totQty, feeItem);

                        if (ft == null)
                        {
                            this.fpSpread1.Select();
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);


                            return;
                        }

                        feeItem.FT.TotCost = ft.TotCost;
                        feeItem.FT.OwnCost = ft.OwnCost;
                        feeItem.FT.PubCost = ft.PubCost;
                        feeItem.FT.PayCost = ft.PayCost;
                        //add by niuxy处理优惠
                        feeItem.FT.RebateCost = ft.RebateCost;
                        this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Cost].Value = ft.TotCost;
                        this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Amount].Value = totQty;
                        //{73AA7783-8B97-45f5-B430-0C7311E952C8}    
                        SumCost();
                        this.isDealCellChange = true;
                    }
                    if (e.Column == (int)Columns.Price)
                    {
                        if (feeItem.Item.Price >= this.priceWarnning)
                        {
                            this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.ItemName].ForeColor =
                                Color.FromArgb(this.priceWarinningColor);
                        }
                        else
                        {
                            this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.ItemName].ForeColor =
                                Color.Black;
                        }
                        decimal price = 0;
                        decimal qty = 0;

                        price =
                            Neusoft.FrameWork.Public.String.FormatNumber(
                            NConvert.ToDecimal(
                            Neusoft.FrameWork.Public.String.ExpressionVal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Price].Value.ToString())), 4);

                        if (price <= 0)
                        {
                            price = 0;
                        }
                        if (feeItem.FeePack == "0")//最小单位
                        {
                            price = price * feeItem.Item.PackQty;
                        }

                        feeItem.Item.Price = price;
                        feeItem.OrgPrice = feeItem.Item.Price;

                        Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, qty, feeItem);

                        if (ft == null)
                        {
                            this.fpSpread1.Select();
                            this.fpSpread1.Focus();
                            this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);


                            return;
                        }

                        this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Cost].Value = ft.TotCost;


                        feeItem.FT.OwnCost = ft.OwnCost;
                        feeItem.FT.TotCost = ft.TotCost;
                        feeItem.FT.PayCost = ft.PayCost;
                        feeItem.FT.PubCost = ft.PubCost;
                        //add by niuxy处理优惠
                        feeItem.FT.RebateCost = ft.RebateCost;
                        this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.ExeDept].Locked = false;
                        this.SumCost();
                    }

                    if (e.Column == (int)Columns.DoseOnce)
                    {
                        try
                        {
                            //if (((FeeItemList)this.fpSpread1_Sheet1.Rows[e.Row].Tag).Item.IsPharmacy)
                            if (((FeeItemList)this.fpSpread1_Sheet1.Rows[e.Row].Tag).Item.ItemType == EnumItemType.Drug)
                            {
                                if (this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.DoseOnce].Text == string.Empty)
                                {

                                }
                                else
                                {
                                    feeItem.Order.DoseOnce =
                                        Neusoft.FrameWork.Public.String.FormatNumber(
                                            NConvert.ToDecimal(
                                                Neusoft.FrameWork.Public.String.ExpressionVal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.DoseOnce].Text)), 3);
                                    this.isDealCellChange = false;
                                    this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.DoseOnce].Value = feeItem.Order.DoseOnce;
                                    this.isDealCellChange = true;
                                }
                                if (((FeeItemList)this.fpSpread1_Sheet1.Rows[e.Row].Tag).Item.SysClass.ID.ToString() == "PCC" && !(feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).IsNostrum)
                                {
                                    decimal days = 0;
                                    decimal qty = 0;
                                    decimal totQty = 0;

                                    try
                                    {
                                        days = NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Days].Text);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("输入的天数不合法" + ex.Message);
                                        this.Enter -= new System.EventHandler(this.ucDisplay_Enter);
                                        this.fpSpread1.Focus();
                                        this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Days);
                                        this.Enter += new System.EventHandler(this.ucDisplay_Enter);

                                        return;
                                    }

                                    qty = NConvert.ToDecimal(Neusoft.FrameWork.Public.String.ExpressionVal(this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.DoseOnce].Text.ToString()));
                                    qty = Neusoft.FrameWork.Public.String.FormatNumber(qty, 3);

                                    feeItem.Order.DoseOnce = qty;
                                    // {1FAD3FA2-C7D8-4cac-845F-B9EBECDE2312}
                                    totQty = qty * days / ((feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose == 0 ? 1 : (feeItem.Item as Neusoft.HISFC.Models.Pharmacy.Item).BaseDose);

                                    // totQty = qty * days;
                                    feeItem.Item.Qty = totQty;

                                    Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(feeItem.Item.Price, totQty, feeItem);

                                    if (ft == null)
                                    {
                                        this.fpSpread1.Select();
                                        this.fpSpread1.Focus();
                                        this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);


                                        return;
                                    }

                                    feeItem.FT.TotCost = ft.TotCost;
                                    feeItem.FT.OwnCost = ft.OwnCost;
                                    feeItem.FT.PubCost = ft.PubCost;
                                    feeItem.FT.PayCost = ft.PayCost;
                                    //add by niuxy处理优惠
                                    feeItem.FT.RebateCost = ft.RebateCost;
                                    this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Cost].Value = ft.TotCost;
                                    this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Amount].Value = totQty;
                                }
                            }
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void fpSpread1_EditModeOn(object sender, EventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            SetLocation();
            if (fpSpread1_Sheet1.ActiveColumnIndex != (int)Columns.ExeDept)
                lbDept.Visible = false;

            if (fpSpread1_Sheet1.ActiveColumnIndex != (int)Columns.MachineNO)
                this.lbMachineNO.Visible = false;
            if (fpSpread1_Sheet1.ActiveColumnIndex != (int)Columns.Freq)
            {
                lbFreq.Visible = false;
            }
            if (fpSpread1_Sheet1.ActiveColumnIndex != (int)Columns.Usage)
            {
                lbUsage.Visible = false;
            }
            if (fpSpread1_Sheet1.ActiveColumnIndex == (int)Columns.ExeDept)
            {
                if (this.fpSpread1_Sheet1.ActiveRow.Tag != null && this.fpSpread1_Sheet1.ActiveRow.Tag is FeeItemList)
                {
                    FeeItemList feeItemList = this.fpSpread1_Sheet1.ActiveRow.Tag as FeeItemList;
                    if (feeItemList.Item.ItemType == EnumItemType.UnDrug)
                    {
                        ArrayList alExecDept = null;

                        string defaultExecDept = string.Empty;
                        lbDept.Items.Clear();
                        SOC.HISFC.BizProcess.Cache.Common.SetExecDept(true, feeItemList.RecipeOper.Dept.ID, feeItemList.Item.ID, ref defaultExecDept, ref alExecDept);
                        lbDept.AddItems(alExecDept);
                    }
                }
            }
            this.fpSpread1.EditingControl.KeyDown += new KeyEventHandler(EditingControl_KeyDown);
        }

        void EditingControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                PutArrow(Keys.Left);
            }
            if (e.KeyCode == Keys.Right)
            {
                PutArrow(Keys.Right);
            }
            if (e.KeyCode == Keys.PageUp)
            {
                if (this.fpSpread1_Sheet1.ActiveRowIndex >= 9)
                {
                    this.fpSpread1_Sheet1.ActiveRowIndex = this.fpSpread1_Sheet1.ActiveRowIndex - 9;
                }
                else
                {
                    this.fpSpread1_Sheet1.ActiveRowIndex = 0;
                }
            }
            if (e.KeyCode == Keys.PageDown)
            {
                if (this.fpSpread1_Sheet1.ActiveRowIndex + 9 <= this.fpSpread1_Sheet1.Rows.Count - 1)
                {
                    this.fpSpread1_Sheet1.ActiveRowIndex = this.fpSpread1_Sheet1.ActiveRowIndex + 9;
                }
                else
                {
                    this.fpSpread1_Sheet1.ActiveRowIndex = this.fpSpread1_Sheet1.Rows.Count - 1;
                }
            }
        }

        /// <summary>
        /// 设置选择项目位置
        /// </summary>
        protected virtual Point GetChooseItemLocation(Control cell)
        {
            Point p = new Point(SystemInformation.Border3DSize.Height * 2 + this.fpSpread1.Location.X + cell.Location.X,
                    this.Parent.Location.Y + cell.Location.Y + cell.Height + SystemInformation.Border3DSize.Height * 2);
            return p;
        }

        private void fpSpread1_EditChange(object sender, EditorNotifyEventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            //{E027D856-6334-4410-8209-5E9E36E31B53} 项目列表多线程载入
            //如果线程没有结束,不响应项目录入
            if (this.threadItemInit.ThreadState != ThreadState.Stopped)
            {
                return;
            }
            //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
            if (e.Column == (int)Columns.InputCode && this.chooseItemControl.ChooseItemType == Neusoft.HISFC.BizProcess.Integrate.FeeInterface.ChooseItemTypes.ItemChanging)
            {
                string inputChar = e.EditingControl.Text.Trim();
                //{7FAF97A6-736D-428d-9932-26563EBDD324}
                //inputChar = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(inputChar);
                inputChar = inputChar.Replace("'", "");
                Control cell = e.EditingControl;
                Point p = GetChooseItemLocation(cell);
                this.chooseItemControl.SetLocation(p);
                //this.chooseItemControl.SetLocation(new Point(SystemInformation.Border3DSize.Height * 2 + this.fpSpread1.Location.X + cell.Location.X,
                //    this.Parent.Location.Y + cell.Location.Y + cell.Height + SystemInformation.Border3DSize.Height * 2));

                this.chooseItemControl.SetInputChar(this.fpSpread1, inputChar, Neusoft.HISFC.Models.Base.InputTypes.Spell);
            }

            if (e.Column == (int)Columns.ExeDept)
            {
                string text = fpSpread1_Sheet1.ActiveCell.Text;
                //{7FAF97A6-736D-428d-9932-26563EBDD324}
                text = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(text);

                lbDept.Filter(text);
                //记录执行科室已经修改，要重新赋值
                fpSpread1_Sheet1.SetValue(e.Row, (int)Columns.Change, "1", false);

                if (lbDept.Visible == false) lbDept.Visible = true;
            }

            if (e.Column == (int)Columns.MachineNO)
            {
                string text = fpSpread1_Sheet1.ActiveCell.Text;
                //{7FAF97A6-736D-428d-9932-26563EBDD324}
                text = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(text);

                this.lbMachineNO.Filter(text);
                //记录执行科室已经修改，要重新赋值
                fpSpread1_Sheet1.SetValue(e.Row, (int)Columns.Change, "1", false);

                if (this.lbMachineNO.Visible == false) lbMachineNO.Visible = true;
            }
            if (e.Column == (int)Columns.Freq)
            {
                string text = fpSpread1_Sheet1.ActiveCell.Text;
                //{7FAF97A6-736D-428d-9932-26563EBDD324}
                text = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(text);

                lbFreq.Filter(text);
                //记录频次已经修改，要重新赋值
                fpSpread1_Sheet1.SetValue(e.Row, (int)Columns.Change, "1", false);

                if (lbFreq.Visible == false) lbFreq.Visible = true;
            }
            if (e.Column == (int)Columns.Usage)
            {
                string text = fpSpread1_Sheet1.ActiveCell.Text;
                //{7FAF97A6-736D-428d-9932-26563EBDD324}
                text = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(text);
                lbUsage.Filter(text);
                //记录频次已经修改，要重新赋值
                fpSpread1_Sheet1.SetValue(e.Row, (int)Columns.Change, "1", false);

                if (lbUsage.Visible == false) lbUsage.Visible = true;
            }
            if (e.Column == (int)Columns.PriceUnit)
            {
                try
                {
                    string tempString = e.EditingControl.Text;

                    if (((FarPoint.Win.FpCombo)e.EditingControl).List.IndexOf(tempString) == -1)
                    {
                        if (this.fpSpread1_Sheet1.Rows[e.Row].Tag != null)
                        {
                            FeeItemList f = this.fpSpread1_Sheet1.Rows[e.Row].Tag as FeeItemList;
                            ((FarPoint.Win.FpCombo)e.EditingControl).SelectedIndex = NConvert.ToInt32(f.FeePack);
                        }
                    }
                }
                catch { }
            }
        }

        void myInjec_WhenInputInjecs(decimal injecs)
        {
            injec = injecs;
        }

        private void fpSpread1_Enter(object sender, EventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            isFocus = true;
        }

        private void fpSpread1_Leave(object sender, EventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            this.fpSpread1.StopCellEditing();

            isFocus = false;
        }

        private void ucDisplay_Enter(object sender, EventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            int rowCount = this.fpSpread1_Sheet1.RowCount;
            if (rowCount > 0)
            {
                try
                {
                    this.fpSpread1_Sheet1.SetActiveCell(rowCount - 1, 0, false);
                }
                catch { }
            }
        }

        private void fpSpread1_ComboSelChange(object sender, EditorNotifyEventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            if (e.Column == (int)Columns.PriceUnit)
            {
                try
                {
                    FeeItemList feeItem = this.fpSpread1_Sheet1.Rows[e.Row].Tag as FeeItemList;
                    decimal price = 0;
                    decimal qty = 0;

                    qty = NConvert.ToDecimal
                        (this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Amount].Text);

                    if (((FarPoint.Win.FpCombo)e.EditingControl).SelectedIndex == 1)//包装单位
                    {
                        feeItem.FeePack = "1";//包装单位
                        this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Price].Value = feeItem.Item.Price;
                        feeItem.Item.Qty = qty * feeItem.Item.PackQty;
                    }
                    else
                    {
                        feeItem.FeePack = "0"; //最小单位
                        this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Price].Value =
                            Neusoft.FrameWork.Public.String.FormatNumber(
                            NConvert.ToDecimal(feeItem.Item.Price / feeItem.Item.PackQty), 4);
                        feeItem.Item.Qty = qty;
                    }

                    Neusoft.HISFC.Models.Base.FT ft = this.ComputCost(price, qty, feeItem);

                    if (ft == null)
                    {
                        this.fpSpread1.Select();
                        this.fpSpread1.Focus();
                        this.fpSpread1_Sheet1.SetActiveCell(e.Row, (int)Columns.Amount, false);

                        return;
                    }

                    this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Cost].Value = ft.TotCost;

                    feeItem.FT.OwnCost = ft.OwnCost;
                    feeItem.FT.TotCost = ft.TotCost;
                    feeItem.FT.PayCost = ft.PayCost;
                    feeItem.FT.PubCost = ft.PubCost;
                    feeItem.Item.PriceUnit = ((FarPoint.Win.Spread.CellType.ComboBoxCellType)this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.PriceUnit].CellType).Items[((FarPoint.Win.FpCombo)e.EditingControl).SelectedIndex];
                    this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.PriceUnit].Text = feeItem.Item.PriceUnit;
                    SumCost();
                    if (!this.ContainsFocus && !this.fpSpread1.ContainsFocus)
                    {
                        this.Focus();
                        this.fpSpread1.Focus();
                    }
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void fpSpread1_CellClick(object sender, CellClickEventArgs e)
        {
            FarPoint.Win.Spread.Model.CellRange c = this.fpSpread1.GetCellFromPixel(0, 0, e.X, e.Y);

            this.RefreshItemInfo(c.Row);

            //{CA82280B-51B6-4462-B63E-43F4ECF456A3}
            if (e.Column == (int)Columns.ExeDept)
            {
                FeeItemList f = this.fpSpread1_Sheet1.Rows[c.Row].Tag as FeeItemList;
                if (f != null)
                {
                    this.SetExecDept(c.Row, f);
                    //this.SetExecDept(f.Item.ID);
                }
            }
        }

        #endregion

        //报表接口
        #region IInterfaceContainer 成员
        //{21C33D5B-5583-4b1d-8023-278336C0C6C7}
        public Type[] InterfaceTypes
        {
            get
            {
                Type[] type = new Type[2];
                type[0] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IAdptIllnessOutPatient);
                type[1] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade);

                return type;
            }
        }

        #endregion

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshItem();
        }

        //{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
        private void fpSpread1_ButtonClicked(object sender, EditorNotifyEventArgs e)
        {
            if (e.Column == (int)Columns.Select)
            {
                this.selectSameComb(e);
                this.SumCost();
            }
            if (e.Column == (int)Columns.IsSend)
            {
                this.selectSameIsSend(e);
            }
        }

        private void selectSameComb(EditorNotifyEventArgs e)
        {
            if (this.fpSpread1_Sheet1.Rows[e.Row].Tag == null)
            {
                return;
            }
            string selectCombID = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[e.Row].Tag).UndrugComb.ID;
            if (string.IsNullOrEmpty(selectCombID))
            {
                return;
            }
            string combID = string.Empty;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag == null)
                {
                    continue;
                }
                combID = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).UndrugComb.ID;
                if (selectCombID.Equals(combID))
                {
                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.Select].Value = this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.Select].Value;
                }
            }
        }

        private void selectSameIsSend(EditorNotifyEventArgs e)
        {
            if (this.fpSpread1_Sheet1.Rows[e.Row].Tag == null)
            {
                return;
            }
            string selectCombID = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[e.Row].Tag).UndrugComb.ID;
            if (string.IsNullOrEmpty(selectCombID))
            {
                return;
            }
            string combID = string.Empty;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                if (this.fpSpread1_Sheet1.Rows[i].Tag == null)
                {
                    continue;
                }
                combID = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).UndrugComb.ID;
                if (selectCombID.Equals(combID))
                {
                    this.fpSpread1_Sheet1.Cells[i, (int)Columns.IsSend].Value = this.fpSpread1_Sheet1.Cells[e.Row, (int)Columns.IsSend].Value;
                }
            }
        }


        private void fpSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpSpread1_Sheet1, filePath);
        }

        public void PreCountInvos()
        {

            DateTime endTime = DateTime.MinValue;
            string CardNO = "";
            ArrayList alInvoices = new ArrayList();
            ArrayList alPatientinfo = new ArrayList();
            ArrayList preFeeItemList = new ArrayList();
            frmChoosePatientInfo frmChoosePatientinfo = new frmChoosePatientInfo();
            frmChoosePatientinfo.ShowDialog();
            DialogResult result = frmChoosePatientinfo.DialogResult;

            if (result != DialogResult.OK)
            {
                return;
            }

            #region

            if (frmChoosePatientinfo.CkQuery == true)
            {
                alInvoices = this.outpatientManager.QueryBalancesAllByCardNO(frmChoosePatientinfo.CardNo.PadLeft(10, '0'), frmChoosePatientinfo.DateBegin, frmChoosePatientinfo.DateEnd);
                if (alInvoices == null)
                {
                    MessageBox.Show("通过卡号查询发票信息出错!" + this.outpatientManager.Err);
                    return;
                }
                if (alInvoices.Count == 0)
                {
                    MessageBox.Show("没有符合条件的发票信息!");
                    return;
                }
                if (alInvoices.Count > 1)
                {
                    bool isSelect = false;//默认不需要弹出选择发票窗口.
                    string SeqNo = string.Empty;//发票序列号
                    //循环检索当前获得的所有发票信息.
                    foreach (Balance balance in alInvoices)
                    {
                        if (SeqNo == string.Empty)
                        {
                            SeqNo = balance.CombNO;
                            continue;
                        }
                        else
                        {
                            //如果发现有发票序列不同的情况,说明存在重复发票号情况,需要弹出发票选择窗口.
                            if (SeqNo != balance.CombNO)
                            {
                                isSelect = true;
                            }
                        }
                    }
                    if (isSelect) //判断是否需要选择发票
                    {
                        //声明选择发票窗口实例
                        ucInvoiceSelect uc = new ucInvoiceSelect();
                        //装载本次检索的所有发票信息
                        uc.Add(alInvoices);
                        //弹出发票选择窗口
                        Neusoft.FrameWork.WinForms.Classes.Function.PopForm.TopMost = true;
                        Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(uc);
                        //如果操作员没有进行选择给予提示
                        if (uc.SelectedBalance == null || uc.SelectedBalance.CombNO == string.Empty)
                        {
                            MessageBox.Show("请选择要用的发票");
                            return;
                        }
                        //通过操作员选择的发票信息,选择了唯一发票序列,再根据发票序列获取本次应参与退费的所有发票信息.
                        alInvoices = outpatientManager.QueryBalancesAllByInvoiceSequence(uc.SelectedBalance.CombNO);
                        if (alInvoices == null)
                        {
                            MessageBox.Show("查询发票失败" + outpatientManager.Err);
                            return;
                        }
                        preFeeItemList = this.outpatientManager.QueryFeeItemListsByAllInvoiceSequence(uc.SelectedBalance.CombNO);
                    }

                }
                else
                {
                    preFeeItemList = this.outpatientManager.QueryFeeItemListsByAllInvoiceSequence((alInvoices[0] as Balance).CombNO);
                }


            }
            else
            {

                #region 获取历史处方信息 by cube 2010-09-10
                using (ucRecipeSelect ucRecipeSelect = new ucRecipeSelect())
                {
                    int parm = ucRecipeSelect.ShowFeeItemList(frmChoosePatientinfo.CardNo.PadLeft(10, '0'), frmChoosePatientinfo.DateBegin, frmChoosePatientinfo.DateEnd, true);
                    if (parm == 1)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.ShowControl(ucRecipeSelect);
                        if (ucRecipeSelect.FeeItemList == null)
                        {
                            return;
                        }
                        preFeeItemList = ucRecipeSelect.FeeItemList;
                    }
                }

                #endregion


            }
            preFeeItemList = ConvertDetailToGroup(preFeeItemList);
            foreach (FeeItemList f in preFeeItemList)
            {
                string drugFlag = "";
                switch (f.Item.ItemType)
                {
                    case EnumItemType.Drug:
                        drugFlag = "1";
                        break;
                    case EnumItemType.UnDrug:
                        drugFlag = "0";
                        break;
                    case EnumItemType.MatItem:
                        drugFlag = "2";
                        break;
                    default:
                        drugFlag = "0";
                        break;
                }
                if (f.Item.ItemType == EnumItemType.Drug)
                {
                    if (f.FeePack == "1")
                    {
                        //SetItem(f.Item.ID, drugFlag, f.ExecOper.Dept.ID, this.fpSpread1.Sheets[0].RowCount - 1, f.Item.Qty / f.Item.PackQty   /*, f.Item.Price*/ );
                        SetItem(f.Item.ID, drugFlag, f.ExecOper.Dept.ID, this.fpSpread1.Sheets[0].RowCount - 1, f.Item.Qty / f.Item.PackQty, f.Item.Price, f.Item.PriceUnit);
                        this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.Days].Text = f.Days.ToString();
                        this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.Amount].Text = (f.Item.Qty / f.Item.PackQty).ToString("F2");
                        this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.DoseOnce].Text = f.Order.DoseOnce.ToString(); //(f.Item.Qty /f.Days * ((Neusoft.HISFC.Object.Pharmacy.Item)f.Item).BaseDose).ToString("F2");
                    }
                    else
                    {
                        //SetItem(f.Item.ID, drugFlag, f.ExecOper.Dept.ID, this.fpSpread1.Sheets[0].RowCount - 1, f.Item.Qty    /*, f.Item.Price*/ );
                        SetItem(f.Item.ID, drugFlag, f.ExecOper.Dept.ID, this.fpSpread1.Sheets[0].RowCount - 1, f.Item.Qty, f.Item.Price, f.Item.PriceUnit);
                        this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.Days].Text = f.Days.ToString();
                        this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.Amount].Text = f.Item.Qty.ToString();
                        this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.DoseOnce].Text = f.Order.DoseOnce.ToString();//(f.Item.Qty / f.Days).ToString();
                    }

                }
                else
                {
                    //SetItem(f.Item.ID, drugFlag, f.ExecOper.Dept.ID, this.fpSpread1.Sheets[0].RowCount - 1, f.Item.Qty     /*, f.Item.Price*/  );
                    SetItem(f.Item.ID, drugFlag, f.ExecOper.Dept.ID, this.fpSpread1.Sheets[0].RowCount - 1, f.Item.Qty, f.Item.Price, f.Item.PriceUnit);
                }
                this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.Amount].Locked = false;
                this.fpSpread1_Sheet1.Cells[this.fpSpread1.Sheets[0].RowCount - 1, (int)Columns.ExeDept].Locked = false;
                this.fpSpread1.Sheets[0].AddRows(this.fpSpread1.Sheets[0].RowCount, 1);

            }
            #endregion

        }

        /// <summary>
        /// 明细把组套拆
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ArrayList ConvertDetailToGroup(ArrayList f)
        {
            Hashtable al1 = new Hashtable();

            ArrayList b1 = new ArrayList();

            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList al in f)
            {

                if (al1.Contains(al.UndrugComb.ID))
                {
                    continue;
                }
                else
                {
                    if (al.UndrugComb.ID != "")
                    {
                        al1.Add(al.UndrugComb.ID, al);
                        al.Item.Price = this.undrugPackAgeManager.GetUndrugCombPrice(al.UndrugComb.ID);
                        al.Item.ID = al.UndrugComb.ID;
                        al.Item.Name = al.UndrugComb.Name;
                        al.FT.OwnCost = al.Item.Qty * al.Item.Price;
                        al.FT.TotCost = al.Item.Qty * al.Item.Price;
                        b1.Add(al);

                    }
                    else
                    {

                        b1.Add(al);
                    }

                }

            }

            return b1;


        }
    }
}
