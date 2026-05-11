using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
namespace Neusoft.HISFC.Components.Common.Controls
{
    /// <summary>
    /// 【门诊、住院医生站】输入项目控件
    /// </summary>
    public partial class ucInputItem : UserControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        public ucInputItem()
        {

            InitializeComponent();
        }

        /// <summary>
        /// 刷新列表 定时器
        /// </summary>
        //private System.Threading.Timer initSIListTimer = null;

        /// <summary>
        /// 多线程加载医保对照信息
        /// </summary>
        //private System.Threading.TimerCallback initSIListCallBack = null;

        #region 初始化
        /// <summary>
        /// 住院模块隐藏外配处方
        /// </summary>
        public void HideWapPei()
        {
            this.chkoutflag.Visible = false;
        }
        public ucInputItem(bool showGroup)
        {
            // 该调用是 Windows.Forms 窗体设计器所必需的。
            InitializeComponent();
            if (!DesignMode)
            {
                this.isShowGroup = showGroup;
            }
        }

        public ucInputItem(bool isListShowAlway, bool showGroup)
        {
            InitializeComponent();
            if (!DesignMode)
            {
                this.bIsListShowAlways = isListShowAlway;
                this.isShowGroup = showGroup;
            }
        }

        #endregion

        #region 业务层变量

        /// <summary>
        /// 选择项目
        /// </summary>
        public event Neusoft.FrameWork.WinForms.Forms.SelectedItemHandler SelectedItem;//用于返回取到的项目信息

        /// <summary>
        /// 组套管理
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.UndrugztManager ztManager = new Neusoft.HISFC.BizLogic.Manager.UndrugztManager();

        /// <summary>
        /// 控制参数
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        /// <summary>
        /// 科常用管理类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.DeptItem deptItemManager = new Neusoft.HISFC.BizLogic.Manager.DeptItem();

        /// <summary>
        /// 整合的管理类
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Manager interMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 组套列表
        /// </summary>
        List<Neusoft.HISFC.Models.Fee.Item.UndrugComb> lstzt = null;

        /// <summary>
        /// 类别变化
        /// </summary>
        public event Neusoft.FrameWork.WinForms.Forms.SelectedItemHandler CatagoryChanged; //项目类别变化

        /// <summary>
        /// 返回的项目信息
        /// </summary>
        protected Neusoft.FrameWork.Models.NeuObject myFeeItem = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 合同单位信息
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.PactUnitInfo pactUnitInfoBizLogic = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo();
        /// <summary>
        /// 输入法类型显示信息：0:拼音码 1:五笔码 2:自定义码
        /// </summary>
        protected string QueryType = string.Empty;

        /// <summary>
        /// 输入法类型 0 拼音 1 五笔 2 自定义码
        /// </summary>
        private int intQueryType = 0;

        /// <summary>
        /// 设置显示的字体大小
        /// </summary>
        private Font fontSize = null;

        /// <summary>
        /// 设置显示的字体大小
        /// </summary>
        public Font FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                this.lblCategory.Font = fontSize;
                this.cmbCategory.Font = fontSize;
                this.neuLabel1.Font = fontSize;
                this.lblItemName.Font = fontSize;
                this.txtNote.Font = fontSize;
                this.txtItemCode.Font = fontSize;
                this.txtItemName.Font = fontSize;
            }
        }

        /// <summary>
        /// 自备标记
        /// </summary>
        public const string SelfMark = "[自备]";

        /// <summary>
        /// 显示的列表窗口
        /// </summary>
        protected Forms.frmShowItem frmShowItem = new Forms.frmShowItem();


        /// <summary>
        /// 当前FP
        /// </summary>
        public Neusoft.FrameWork.WinForms.Controls.NeuSpread fpItemList = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();

        /// <summary>
        /// 发药类型：O 门诊处方、I 住院医嘱、A 全部 (默认为A)
        /// </summary>
        private string drugSendType = "A";

        #region add by lijp 2016-07-18
        /// <summary>
        /// 静态变量，科室编码
        /// </summary>
        private static string deptcodeStatic = string.Empty;

        /// <summary>
        /// 静态变量显示项目类别
        /// </summary>
        private static EnumShowItemType showItemTypeStatic = EnumShowItemType.All;

        /// <summary>
        /// 静态变量，类别，门诊或住院
        /// </summary>
        private static Neusoft.HISFC.Models.Base.ServiceTypes typeStatic = Neusoft.HISFC.Models.Base.ServiceTypes.C;

        /// <summary>
        /// 静态变量，发送方式。
        /// </summary>
        private static string drugSendTypeStatic = "B";

        /// <summary>
        /// 静态变量，是否显示组套
        /// </summary>
        private static bool isShowGroupStatic = false;

        /// <summary>
        /// 静态变量，是否显示科室组套
        /// </summary>
        private static bool bShowDeptGroupStatic = false;

        /// <summary>
        /// 静态变量，是否显示组套项目
        /// </summary>
        private static string isShowZTItemStatic = string.Empty;

        /// <summary>
        /// 静态变量，？
        /// </summary>
        private static bool isControlUnShowStatic = false;

        private static EnumUndrugApplicabilityarea eUndrugApplicabilityareaStatic = EnumUndrugApplicabilityarea.All;

        #endregion

        /// <summary>
        /// 项目帮助类
        /// </summary>
        //[Obsolete("作废", true)]
        private Neusoft.FrameWork.Public.ObjectHelper itemHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 项目帮助类
        /// </summary>
        public Neusoft.FrameWork.Public.ObjectHelper ItemHelper
        {
            get
            {
                return this.itemHelper;
            }
        }

        /// <summary>
        /// 发药类型：O 门诊处方、I 住院医嘱、A 全部 (默认为A)
        /// </summary>
        public string DrugSendType
        {
            get
            {
                return drugSendType;
            }
            set
            {
                drugSendType = value;
            }
        }

        #region 科室常用项目

        /// <summary>
        /// 科室常用项目字符串
        /// </summary>
        string destItemId = "";

        /// <summary>
        /// 科室常用项目
        /// </summary>
        public static ArrayList arrDeptUsed = new ArrayList();

        #endregion

        /// <summary>
        /// 项目类别
        /// </summary>
        private ArrayList arrItemTypes = new ArrayList();

        /// <summary>
        /// 当前项目dataset
        /// </summary>
        private static System.Data.DataSet MyDataSet
        {
            get
            {
                if (longOrder)
                {
                    return longDataSet;
                }
                else
                {
                    return shortDataSet;
                }
            }
        }

        /// <summary>
        /// 长嘱列表
        /// </summary>
        private static System.Data.DataSet longDataSet = null;

        /// <summary>
        /// 临嘱列表
        /// </summary>
        private static System.Data.DataSet shortDataSet = null;

        private static System.Data.DataSet myDeptDataSet = null;

        /// <summary>
        /// 是否显示类别列表
        /// </summary>
        private bool bIsShowCategory = true;

        /// <summary>
        /// 进程
        /// </summary>
        protected Thread myThread = null;

        /// <summary>
        /// 初次显示：初次显示加载附加方法
        /// </summary>
        private bool isFristShow = true;

        /// <summary>
        /// 当前登陆人员
        /// </summary>
        protected Neusoft.FrameWork.Models.NeuObject oper = null;

        /// <summary>
        /// 获取项目医保标记接口 
        /// </summary>
        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade iGetSiFlag = null;

        /// <summary>
        /// 获取项目医保标记接口 
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade IGetSiFlag
        {
            get
            {
                if (this.iGetSiFlag == null)
                {
                    this.iGetSiFlag = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade;
                }

                return this.iGetSiFlag;
            }
        }

        #region 项目列表

        /// <summary>
        /// 长嘱项目列表
        /// </summary>
        private static ArrayList alLongItem = null;

        /// <summary>
        /// 临嘱项目列表
        /// </summary>
        private static ArrayList alShortItem = null;

        /// <summary>
        /// 项目列表
        /// </summary>
        public static ArrayList alItem = null;

        /// <summary>
        /// 非药品项目列表
        /// </summary>
        private static ArrayList alUndrugItem = new ArrayList();

        /// <summary>
        /// 本院药品项目列表
        /// </summary>
        private static ArrayList alBYDrugItem = new ArrayList();

        /// <summary>
        /// 自营药品项目列表
        /// </summary>
        private static ArrayList alZYDrugItem = new ArrayList();

        ///// <summary>
        ///// 本院药品基本信息哈希表
        ///// </summary>
        //private Hashtable hsAllDrug = null;

        ///// <summary>
        ///// 药品基本信息哈希表
        ///// </summary>
        //public Hashtable HsAllDrug
        //{
        //    get
        //    {
        //        return this.hsAllDrug;
        //    }
        //}

        #endregion

        /// <summary>
        /// 开立非药品项目是否过滤允许开立科室
        /// </summary>
        private static bool isFilteUndrugRecipeDept = false;

        /// <summary>
        /// 医生开立列表是否显示检查、检验的明细项目
        /// 1111:分别表示门诊检查、门诊检验、住院检查、住院检验
        /// </summary>
        private static string isShowDetailItem = string.Empty;

        /// <summary>
        /// 当前患者信息 
        /// </summary>
        private Neusoft.HISFC.Models.RADT.Patient patient = null;

        /// <summary>
        /// 当前患者信息 
        /// </summary>
        public Neusoft.HISFC.Models.RADT.Patient Patient
        {
            get
            {
                return this.patient;
            }
            set
            {
                this.patient = value;

                if (patient != null
                    && patient.Pact != null
                    && !string.IsNullOrEmpty(patient.Pact.ID))
                {
                    patient.Pact = SOC.HISFC.BizProcess.Cache.Fee.GetPactUnitInfo(patient.Pact.ID);
                }
            }
        }


        #region {9A40A1FE-C527-4f86-B6F5-E7F52FDD28C9}

        /// <summary>
        /// 详细列表
        /// 收费项目显示应用
        /// </summary>
        public Neusoft.FrameWork.WinForms.Controls.NeuSpread fpItemDetal = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();

        /// <summary>
        /// 详细列表
        /// 收费项目注意事项
        /// </summary>
        public Neusoft.FrameWork.WinForms.Controls.NeuPanel PanelItemMemo = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();

        /// <summary>
        /// 注意事项label
        /// </summary>
        Neusoft.FrameWork.WinForms.Controls.NeuLabel lblMemo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();


        /// <summary>
        /// 详细信息文本
        /// </summary>
        public System.Windows.Forms.TextBox txtNote = new TextBox();

        /// <summary>
        /// 登录科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject deptLogin = null;

        /// <summary>
        /// 医生站项目选择列表是否显示药房过滤下拉框{ECAE27F0-CC52-46be-A8C5-BC9F680988CD}
        /// </summary>
        private bool isShowCmbDrugDept = false;

        #endregion

        #endregion

        #region 属性

        /// <summary>
        /// 当前项目
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject FeeItem
        {
            get
            {
                if (this.myFeeItem == null)
                {
                    this.myFeeItem = new Neusoft.FrameWork.Models.NeuObject();
                }
                return this.myFeeItem;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.myFeeItem = value;
                this.txtItemName.Text = this.myFeeItem.Name;
            }
        }

        /// <summary>
        /// 是否显示组套
        /// </summary>
        private bool isShowGroup = false;

        /// <summary>
        /// 是否显示组套
        /// </summary>
        protected bool IsShowGroup
        {
            get
            {
                return isShowGroup;
            }
            set
            {
                isShowGroup = value;
            }
        }

        /// <summary>
        /// 显示的类别列：项目类别、系统类别
        /// </summary>
        private EnumCategoryType eShowCategory = 0;

        /// <summary>
        /// 显示的类别列：项目类别、系统类别
        /// </summary>
        public EnumCategoryType ShowCategory
        {
            get
            {
                return this.eShowCategory;
            }
            set
            {
                this.eShowCategory = value;
            }
        }

        /// <summary>
        /// 是否显示类别列表
        /// </summary>
        public bool IsShowCategory
        {
            get
            {

                return this.bIsShowCategory;
            }
            set
            {
                this.panel2.Visible = value;

                this.bIsShowCategory = value;
            }
        }

        /// <summary>
        /// 是否显示输入码
        /// </summary>
        protected bool bIsShowInput = true;

        /// <summary>
        /// 是否显示输入码
        /// </summary>
        public bool IsShowInput
        {
            get
            {
                return this.bIsShowInput;
            }
            set
            {
                this.bIsShowInput = value;
                this.txtItemCode.Visible = value;
                this.txtItemName.ReadOnly = value;
                if (value)
                {
                    //this.txtItemName.Left = 103;
                    this.txtItemName.BackColor = Color.LightSteelBlue;
                }
                else
                {
                    //this.txtItemName.Left = 4;
                    this.txtItemName.BackColor = Color.White;
                }

            }
        }

        /// <summary>
        /// 是否可以输入名称
        /// </summary>
        public bool IsCanInputName
        {
            set
            {
                this.txtItemName.ReadOnly = value;
                if (value)
                {
                    this.txtItemName.BackColor = Color.LightSteelBlue;
                }
                else
                {
                    this.txtItemName.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        /// 是否显示的类别可输入
        /// </summary>
        public bool IsCategoryDropDownList
        {
            set
            {
                if (value)
                {
                    this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }
        }

        /// <summary>
        /// 是否显示自备药字样
        /// </summary>
        protected bool bIsShowSelfMark = true;

        /// <summary>
        /// 是否显示自备药字样
        /// </summary>
        public bool IsShowSelfMark
        {
            get
            {
                return this.bIsShowSelfMark;
            }
            set
            {
                this.bIsShowSelfMark = value;
            }
        }

        /// <summary>
        /// 是否一直显示列表
        /// </summary>
        protected bool bIsListShowAlways = false;
        /// <summary>
        /// 是否一直显示列表
        /// </summary>
        public bool IsListShowAlways
        {
            get
            {
                return this.bIsListShowAlways;
            }
            set
            {
                this.bIsListShowAlways = value;
                if (value)
                {
                    fpItemList.Dock = DockStyle.Fill;
                    fpItemList.Visible = true;
                    this.panel4.Controls.Add(fpItemList);

                    //如果不是弹出选择，就默认不勾选科室常用项目
                    this.frmShowItem.chkDeptItem.Checked = false;
                    this.isDeptUsedFlag = false;
                }
            }

        }

        /// <summary>
        /// 是否使用零差价
        /// </summary>
        private bool isUserRetailPrice2 = false;

        /// <summary>
        /// 显示药品，非药品，全部
        /// </summary>
        protected EnumShowItemType eShowItemType = EnumShowItemType.All;

        /// <summary>
        /// 显示药品，非药品，全部
        /// </summary>
        public EnumShowItemType ShowItemType
        {
            get
            {
                return this.eShowItemType;
            }
            set
            {
                this.eShowItemType = value;
            }
        }

        /// <summary>
        /// 设置聚焦
        /// </summary>
        public new void Focus()
        {
            this.txtItemCode.Focus();
        }

        /// <summary>
        /// 当前类别
        /// </summary>
        public ArrayList AlCatagory
        {
            get
            {
                return this.arrItemTypes;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                ArrayList al = new ArrayList();
                foreach (Neusoft.FrameWork.Models.NeuObject obj in value)
                {
                    if (obj.ID != "PCC")
                    {
                        al.Add(obj);
                    }
                }

                this.cmbCategory.AddItems(al);
                this.cmbCategory.Text = Neusoft.FrameWork.Management.Language.Msg("全部");
            }
        }

        /// <summary>
        /// 是否长嘱
        /// </summary>
        private static bool longOrder = false;

        /// <summary>
        /// 是否长嘱
        /// </summary>
        public bool LongOrder
        {
            set
            {
                if (value)
                {
                    alItem = alLongItem;
                }
                else
                {
                    alItem = alShortItem;
                }
                longOrder = value;
            }
        }

        /// <summary>
        /// 获取列表的科室编码（用来获取药品列表）
        /// </summary>
        protected string deptcode = string.Empty;

        /// <summary>
        /// 获取列表的科室编码（用来获取药品列表）
        /// </summary>
        public string DeptCode
        {
            set
            {
                this.deptcode = value;
                if (string.IsNullOrEmpty(deptcode))
                {
                    deptcode = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID;
                }
            }
        }

        //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
        /// <summary>
        /// 是否包含物资项目
        /// </summary>
        private bool isIncludeMat = false;

        public bool IsIncludeMat
        {
            get
            {
                return isIncludeMat;
            }
            set
            {
                isIncludeMat = value;
            }
        }

        /// <summary>
        /// 当前工作线程
        /// </summary>
        public ThreadState WorkThreadState
        {
            get
            {
                return this.myThread.ThreadState;
            }
        }

        /// <summary>
        /// 是否显示科室组套 (护士组套/手术组套)
        /// </summary>
        protected bool bShowDeptGroup = false;

        /// <summary>
        /// 是否显示科室组套 (护士组套/手术组套)
        /// </summary>
        public bool IsShowDeptGroup
        {
            set
            {
                this.bShowDeptGroup = value;
            }
        }

        /// <summary>
        /// 是否科室常用项目标记
        /// </summary>
        private bool isDeptUsedFlag = false;

        /// <summary>
        /// 是否科室常用项目标记
        /// </summary>
        public bool IsDeptUsedFlag
        {
            get
            {
                return isDeptUsedFlag;
            }
            set
            {
                isDeptUsedFlag = value;
            }
        }

        /// <summary>
        /// 是否是合作医疗范围用药
        /// </summary>
        private bool isCompanyRang = false;

        /// <summary>
        /// 医保对照项目
        /// </summary>
        Neusoft.HISFC.Models.SIInterface.Compare compareItem = null;

        /// <summary>
        /// 非药品分类:门诊、住院、全部
        /// </summary>
        protected EnumUndrugApplicabilityarea eUndrugApplicabilityarea = EnumUndrugApplicabilityarea.All;

        /// <summary>
        /// 非药品分类:门诊、住院、全部
        /// </summary>
        public EnumUndrugApplicabilityarea UndrugApplicabilityarea
        {
            get
            {
                return this.eUndrugApplicabilityarea;
            }
            set
            {
                this.eUndrugApplicabilityarea = value;
            }
        }

        /// <summary>
        /// 存储合同单位和列的对应
        /// </summary>
        private Hashtable hsPactColumn = null;

        /// <summary>
        /// 存储合同单位对应的医保（公费）项目
        /// </summary>
        private Hashtable hsItemPactInfo = null;

        /// <summary>
        /// 当前合同单位信息
        /// </summary>
        [Obsolete("作废了，用Patient.Pact代替", true)]
        private Neusoft.HISFC.Models.Base.PactInfo pactInfo = null;

        /// <summary>
        /// 当前合同单位信息
        /// </summary>
        [Obsolete("作废了，用Patient.Pact代替", true)]
        public Neusoft.HISFC.Models.Base.PactInfo PactInfo
        {
            get
            {
                return pactInfo;
            }
            set
            {
                pactInfo = value;

                if (isUserThread == 3)
                {
                    string pactCode = "all";
                    if (pactInfo == null
                        || string.IsNullOrEmpty(pactInfo.ID)
                        || !hsPactColumn.ContainsKey(pactInfo.ID))
                    {
                        pactCode = "all";
                    }
                    else
                    {
                        pactCode = pactInfo.ID;
                    }

                    for (int i = 0; i < fpItemList.Sheets[0].ColumnCount; i++)
                    {
                        if (fpItemList.Sheets[0].Columns[i].Label == "标记")
                        {
                            if (!hsPactColumn.Contains(pactCode))
                            {
                                fpItemList.Sheets[0].Columns[i].Visible = false;
                            }
                            else
                            {
                                if (i == (Int32)hsPactColumn[pactCode])
                                {
                                    fpItemList.Sheets[0].Columns[i].Visible = true;
                                }
                                else
                                {
                                    fpItemList.Sheets[0].Columns[i].Visible = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否外延药品
        /// </summary>
        public bool IsOUT
        {
            get
            {
                return this.chkoutflag.Checked;
            }
            set
            {
                this.chkoutflag.Checked = value;
            }
        }

        /// <summary>
        /// 外延处方是否可以编辑 - MK
        /// </summary>
        /// <param name="isEdit"></param>
        public void SetExtendRecipeEdit(bool isEdit)
        {
            this.chkoutflag.Enabled = isEdit;
        }

        /// <summary>
        /// 外延处方药房 - MK
        /// </summary>
        private string extendDrugStore;

        /// <summary>
        /// 外延处方药房 - MK
        /// </summary>
        public string ExtendDrugStore
        {
            get
            {
                return this.extendDrugStore;
            }
        }

        /// <summary>
        /// 取药药房帮助类 - MK
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper drugDeptsHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        #endregion

        #region 函数

        #region 初始化

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void Init()
        {
            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem的Init();//初始化项目列表开始!\r\n");

            if (DesignMode)
            {
                return;
            }
            try
            {
                this.oper = new Neusoft.FrameWork.Models.NeuObject();
                this.oper = this.deptItemManager.Operator as Neusoft.FrameWork.Models.NeuObject;

                //{2A5608D8-26AD-47d7-82CC-81375722FF72}
                if (!isFilteUndrugRecipeDept)
                {
                    isFilteUndrugRecipeDept = contrIntegrate.GetControlParam<bool>("201026", false, true);
                }

                if (string.IsNullOrEmpty(isShowDetailItem))
                {
                    isShowDetailItem = contrIntegrate.GetControlParam<string>("HN0003", false, "1111");
                }

                if (isUserThread == -1)
                {
                    isUserThread = contrIntegrate.GetControlParam<int>("HNMZ35", true, 0);
                }

                isUserRetailPrice2 = contrIntegrate.GetControlParam<bool>("HNPHA2", false, false);

                //外延处方药房 - MK
                this.extendDrugStore = contrIntegrate.GetControlParam<string>("EXTDRU", false, "");

                this.frmShowItem.IsUserThread = isUserThread;

                if (this.fpItemList.Sheets.Count <= 0)
                {
                    this.fpItemList.Sheets.Add(new FarPoint.Win.Spread.SheetView());
                }

                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem的AddCategory();//初始化项目列表开始!\r\n");
                AddCategory();//添加类别
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem的AddCategory();//初始化项目列表结束!\r\n");
                this.InputType = 0;//默认拼音

                #region 科室常用项目

                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem的科室常用项目;//初始化项目列表开始!\r\n");

                //初始化科常用项目
                if (arrDeptUsed == null)
                {
                    arrDeptUsed = new ArrayList();
                }
                if (arrDeptUsed.Count <= 0 && this.isDeptUsedFlag)
                {
                    arrDeptUsed = deptItemManager.QueryItemByDeptID(string.IsNullOrEmpty(this.deptcode) ? ((Neusoft.HISFC.Models.Base.Employee)this.oper).Dept.ID : this.deptcode);
                    if (arrDeptUsed != null && arrDeptUsed.Count > 0)
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (Neusoft.FrameWork.Models.NeuObject icd in arrDeptUsed)
                        {
                            string temStr = "'";
                            string splitStr = ",";
                            sb.Append(temStr + icd.ID.Trim() + temStr + splitStr);
                            i++;
                        }
                        destItemId = sb.ToString().Substring(0, sb.ToString().Length - 1);
                    }
                }

                #endregion

                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载列表开始!\r\n");
                //加载列表
                try
                {
                    if (this.bIsListShowAlways)
                    {
                        this.AddItem();
                    }
                    else
                    {
                        #region 多线程屏蔽了{B8FFCAB8-A9FF-43b2-96E2-2DF17B7F3A91}
                        //ThreadStart myThreadDelegate = new ThreadStart(this.AddItem);
                        //myThread = new Thread(myThreadDelegate);
                        //myThread.Start();
                        #endregion
                    }
                }
                catch
                { }
                #region 屏蔽多线程，把下面的代码挪上来{B8FFCAB8-A9FF-43b2-96E2-2DF17B7F3A91}
                #region {9A40A1FE-C527-4f86-B6F5-E7F52FDD28C9}
                this.initFPdetail();
                #endregion
                #endregion

                fpItemList.Sheets[0].DataAutoCellTypes = false;
                fpItemList.Sheets[0].DataAutoSizeColumns = false;

                if (bIsListShowAlways)
                {
                    this.RefreshFP();
                }

                if (this.bIsListShowAlways == false)
                {
                    string ucInputItemGuid = "  " + Guid.NewGuid().ToString() + "  ";
                    Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory(ucInputItemGuid + "ucInputItem加载列表AddItem开始!\r\n");
                    #region 多线程屏蔽{B8FFCAB8-A9FF-43b2-96E2-2DF17B7F3A91}
                    this.AddItem();
                    #endregion

                    Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory(ucInputItemGuid + "ucInputItem加载列表AddItem结束!\r\n");
                    frmShowItem.AddControl(fpItemList);
                }
                else
                {
                    fpItemList.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.panel4.Controls.Add(fpItemList);
                }

                frmShowItem.Owner = this.FindForm();


                frmShowItem.writeWay += new Neusoft.HISFC.Components.Common.Forms.frmShowItem.SelectWriteWay(frmItemList_writeWay);
                frmShowItem.companyRang += new Neusoft.HISFC.Components.Common.Forms.frmShowItem.IsCompanyRang(frmItemList_companyRang);
                frmShowItem.isDeptUsedItem += new Neusoft.HISFC.Components.Common.Forms.frmShowItem.IsDeptUsedItem(frmItemList_isDeptUsedItem);

                frmShowItem.Init();

                frmShowItem.SetUserDefaultSetting();
                frmShowItem.Size = new Size(0, 0);
                frmShowItem.Show();
                frmShowItem.Hide();
                fpItemList.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                //fpItemList.Sheets[0].SetColumnAllowAutoSort(-1, true);
                //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl 首先去掉事件委托
                frmShowItem.Closing -= frmItemList_Closing;
                fpItemList.CellDoubleClick -= fpSpread1_CellDoubleClick;
                fpItemList.KeyDown -= fpSpread1_KeyDown;

                frmShowItem.Closing += new CancelEventHandler(frmItemList_Closing);
                fpItemList.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(fpSpread1_CellDoubleClick);
                fpItemList.KeyDown += new KeyEventHandler(fpSpread1_KeyDown);

                #region {9A40A1FE-C527-4f86-B6F5-E7F52FDD28C9}
                fpItemList.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(fpItemList_SelectionChanged);
                #endregion

                #region {ECAE27F0-CC52-46be-A8C5-BC9F680988CD}

                Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctrlParmMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

                this.isShowCmbDrugDept = ctrlParmMgr.GetControlParam<bool>("200310", true, false);
                if (this.isShowCmbDrugDept)
                {
                    Neusoft.HISFC.BizProcess.Integrate.Pharmacy phaManagement = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();

                    ArrayList alDrugDept = phaManagement.QueryReciveDrugDept(this.deptcode, "A");
                    if (alDrugDept != null && alDrugDept.Count > 1)
                    {
                        Neusoft.FrameWork.Models.NeuObject objAllTmp = new Neusoft.FrameWork.Models.NeuObject();
                        objAllTmp.ID = "ALL";
                        objAllTmp.Name = "全部";
                        alDrugDept.Insert(0, objAllTmp);
                        frmShowItem.cmbDrugDept.Visible = true;
                        frmShowItem.cmbDrugDept.AddItems(alDrugDept);
                        frmShowItem.cmbDrugDept.SelectedIndex = 0;
                        frmShowItem.cmbDrugDept.IsListOnly = true;
                        frmShowItem.cmbDrugDept.IsPopForm = false;
                        frmShowItem.cmbDrugDept.DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                    else
                    {
                        frmShowItem.cmbDrugDept.Visible = false;
                    }

                    //取药药房帮助类 - MK
                    this.drugDeptsHelper.ArrayObject = alDrugDept;
                }
                #endregion

                //zhangjunyi 去掉 绑定DataSet不能筛选，没有意义，而且妨碍下边DataView的绑定
                //if (this.myDataSet != null) fpItemList.Sheets[0].DataSource = this.myDataSet; 
                this.txtItemCode.Enter += new EventHandler(txtItemCode_Enter);
                this.txtItemCode.Leave += new EventHandler(txtItemCode_Leave);
                this.InputType = Neusoft.FrameWork.WinForms.Classes.Function.GetInputType();

                this.fpItemList.MouseWheel += new MouseEventHandler(fpItemList_MouseWheel);

                this.fpItemList.MouseMove += new MouseEventHandler(fpItemList_MouseWheel);

                this.fpItemList.KeyPress += new KeyPressEventHandler(fpItemList_KeyPress);

                this.frmShowItem.RefreshFP();
            }
            catch (Exception ee)
            {
                MessageBox.Show("Init" + ee.Message);
            }
        }

        void fpItemList_KeyPress(object sender, KeyPressEventArgs e)
        {
            ShowItemFlag();
        }

        void fpItemList_MouseWheel(object sender, MouseEventArgs e)
        {
            ShowItemFlag();
        }

        private void ShowItemFlag()
        {
            if (isUserThread != 2)
            {
                return;
            }

            Font font_Bold = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Bold);
            Font font_Regular = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Regular);

            //只刷新当前显示的行数
            for (int i = this.fpItemList.GetViewportTopRow(0); i < fpItemList.GetViewportBottomRow(0) + 1; i++)
            {
                if (i >= fpItemList.Sheets[0].RowCount)
                {
                    return;
                }
                #region 显示停用、缺药信息

                if (fpItemList.Sheets[0].Cells[i, (int)Neusoft.HISFC.Components.Common.Controls.EnumMainColumnSet.LackFlag].Text == "是")
                {
                    fpItemList.Sheets[0].Rows[i].BackColor = Color.LightCoral;
                }
                else
                {
                    fpItemList.Sheets[0].Rows[i].BackColor = Color.Transparent;
                }
                #endregion

                #region 显示医保等级信息

                string itemCode = fpItemList.Sheets[0].Cells[i, (int)Neusoft.HISFC.Components.Common.Controls.EnumMainColumnSet.ItemCode].Text.Trim();
                if (itemCode != "999")
                {
                    Neusoft.HISFC.Models.Base.Item item = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                    if (item == null)
                    {
                        item = SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                    }

                    if (item != null
                        && patient != null
                        && patient.Pact != null)
                    {
                        string strCompareInfo = string.Empty;
                        if (Classes.Function.IItemCompareInfo.GetCompareItemInfo(item, patient.Pact, ref compareItem, ref strCompareInfo) == -1)
                        {
                            this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Regular;
                            this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = this.fpItemList.Sheets[0].ColumnHeader.Columns[0].ForeColor;
                        }
                        else
                        {
                            //医保标记
                            switch (compareItem.CenterItem.ItemGrade)
                            {
                                case "1":
                                    this.fpItemList.Sheets[0].Rows[i].Label = "甲";
                                    this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Bold;
                                    this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = Color.Red;
                                    break;
                                case "2":
                                    this.fpItemList.Sheets[0].Rows[i].Label = "乙";
                                    this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Bold;
                                    this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = Color.Red;
                                    break;
                                default:
                                    this.fpItemList.Sheets[0].Rows[i].Label = i.ToString();
                                    this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Regular;
                                    this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = this.fpItemList.Sheets[0].ColumnHeader.Columns[0].ForeColor;
                                    break;
                            }
                        }
                    }
                }
                #endregion
            }
        }

        #endregion

        public void Clear()
        {
            this.txtItemCode.Text = "";
            this.txtItemName.Text = "";
            if (this.frmShowItem.Visible)
            {
                this.frmShowItem.Visible = false;
            }
        }

        public void addItem(string value, string title)
        {
            ChangeItem();
            int foundRow = 0;
            int foundColumn = 0;
            int columnIndex = 0;
            for (int j = 0; j < this.fpItemList.Sheets[0].ColumnCount; j++)
            {
                if (this.fpItemList.Sheets[0].ColumnHeader.Columns[j].Label == title)
                {
                    columnIndex = j;
                    break;
                }
            }
            this.fpItemList.Search(0, value, true, false, false, false, 0, columnIndex, ref foundRow, ref foundColumn);
            if (foundRow == -1)
                return;
            fpItemList.Sheets[0].ActiveRowIndex = foundRow;
            mySelectedItem();
        }
        #region 项目加载
        /// <summary>
        /// 初始化类别
        /// </summary>
        /// <returns></returns>
        protected virtual int AddCategory()
        {
            this.cmbCategory.ShowCustomerList = false;
            if (this.eShowCategory == EnumCategoryType.ItemType)
            {
                Neusoft.HISFC.BizLogic.Manager.Constant constant = new Neusoft.HISFC.BizLogic.Manager.Constant();
                arrItemTypes = constant.GetList(Neusoft.HISFC.Models.Base.EnumConstant.ITEMTYPE);
            }
            else if (this.eShowCategory == EnumCategoryType.SysClass)
            {
                //由此获取项目类别 西药、手术、描述性医嘱等
                arrItemTypes = Neusoft.HISFC.Models.Base.SysClassEnumService.List();
                if (eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.Clinic)
                {
                    ArrayList altemp = arrItemTypes.Clone() as ArrayList;
                    arrItemTypes.Clear();
                    if (this.eShowItemType == EnumShowItemType.All)
                    {

                        for (int i = 0; i < altemp.Count; i++)
                        {
                            //药品
                            if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID.Substring(0, 1) == "P")
                            {
                                arrItemTypes.Add(altemp[i]);
                            }


                            if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID.Substring(0, 1) == "U")
                            {
                                arrItemTypes.Add(altemp[i]);
                            }

                            if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID == "M"
                                || ((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID == "MC")
                            {
                                arrItemTypes.Add(altemp[i]);
                            }
                        }
                    }
                    else if (this.eShowItemType == EnumShowItemType.Pharmacy)
                    {
                        for (int i = 0; i < altemp.Count; i++)
                        {

                            if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID.Substring(0, 1) == "P")
                            {
                                arrItemTypes.Add(altemp[i]);
                            }
                        }

                    }
                    else if (this.eShowItemType == EnumShowItemType.Undrug)
                    {
                        for (int i = 0; i < altemp.Count; i++)
                        {

                            if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID.Substring(0, 1) == "U")
                            {
                                arrItemTypes.Add(altemp[i]);
                            }
                        }

                    }
                }
                else
                {
                    if (this.eShowItemType != EnumShowItemType.All)
                    {
                        ArrayList altemp = arrItemTypes.Clone() as ArrayList;
                        arrItemTypes.Clear();
                        for (int i = 0; i < altemp.Count; i++)
                        {
                            if (this.eShowItemType == EnumShowItemType.Pharmacy)
                            {
                                if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID.Substring(0, 1) == "P")
                                {
                                    arrItemTypes.Add(altemp[i]);
                                }
                            }
                            else if (this.eShowItemType == EnumShowItemType.Undrug)
                            {
                                if (((Neusoft.FrameWork.Models.NeuObject)altemp[i]).ID.Substring(0, 1) == "U")
                                {
                                    arrItemTypes.Add(altemp[i]);
                                }
                            }
                        }
                    }
                }
            }
            if (arrItemTypes != null)
            {
                Neusoft.FrameWork.Models.NeuObject o = new Neusoft.FrameWork.Models.NeuObject();
                o.Name = "全部";
                this.arrItemTypes.Add(o);
                this.cmbCategory.AddItems(arrItemTypes);
            }
            else
            {
                MessageBox.Show("类别加载失败，请重新操作！");
                return -1;
            }
            this.cmbCategory.Text = "全部";
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cbCategory_SelectedIndexChanged);
            return 0;
        }

        /// <summary>
        /// 刷新停用标记多线程
        /// </summary>
        //ThreadStart freshLackFlagThreadDelegate = null;

        /// <summary>
        /// 是否正在刷新缺药标记
        /// </summary>
        //bool isFreshingLackFlag = false;

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <returns></returns>
        protected virtual int AddItems()
        {
            Neusoft.HISFC.BizLogic.Fee.Item itemMgr = new Neusoft.HISFC.BizLogic.Fee.Item();
            Neusoft.HISFC.BizProcess.Integrate.Pharmacy itemIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();
            //string sql_1 = "select a.special_flag from pha_com_baseinfo a where a.drug_code ='{0}'";
            ArrayList tempList = new ArrayList();//本院药品临时列表
            ArrayList tempList1 = new ArrayList();//自营药品临时列表
            //this.hsAllDrug = new Hashtable();
            try
            {
                //List<Neusoft.HISFC.Models.Pharmacy.Item> drugList = itemIntegrate.QueryItemList(false);
                //foreach (Neusoft.HISFC.Models.Pharmacy.Item drugItem in drugList)
                //{
                //    if (!hsAllDrug.ContainsKey(drugItem.ID))
                //    hsAllDrug.Add(drugItem.ID, drugItem);
                //}
                //List<Neusoft.HISFC.Models.Pharmacy.Item> fhyDrugList = itemIntegrate.QueryItemListFHY(false);
                //foreach (Neusoft.HISFC.Models.Pharmacy.Item drugItem in fhyDrugList)
                //{
                //    if (!hsAllDrug.ContainsKey(drugItem.ID))
                //        hsAllDrug.Add(drugItem.ID, drugItem);
                //}

                Neusoft.HISFC.Models.Base.ServiceTypes type = Neusoft.HISFC.Models.Base.ServiceTypes.C;
                if (eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.InHos
                    || drugSendType == "I")
                {
                    type = Neusoft.HISFC.Models.Base.ServiceTypes.I;
                }


                if (showItemTypeStatic != this.eShowItemType
                    || deptcodeStatic != this.deptcode
                    || typeStatic != type
                    || drugSendTypeStatic != this.drugSendType
                    || isShowGroupStatic != this.isShowGroup
                    || bShowDeptGroupStatic != this.bShowDeptGroup
                    || isShowZTItemStatic != isShowDetailItem
                    //|| isControlUnShowStatic != this.isControlUnShow
                    || eUndrugApplicabilityareaStatic != this.eUndrugApplicabilityarea)
                {
                    deptcodeStatic = this.deptcode;
                    showItemTypeStatic = this.eShowItemType;
                    typeStatic = type;
                    drugSendTypeStatic = this.drugSendType;
                    isShowGroupStatic = this.isShowGroup;
                    bShowDeptGroupStatic = this.bShowDeptGroup;
                    isShowZTItemStatic = isShowDetailItem;
                    //isControlUnShowStatic = this.isControlUnShow;
                    eUndrugApplicabilityareaStatic = this.eUndrugApplicabilityarea;
                    alBYDrugItem.Clear();
                    alZYDrugItem.Clear();
                    alUndrugItem.Clear();
                    longDataSet = null;
                    shortDataSet = null;
                    Neusoft.HISFC.Models.Base.Employee employee = SOC.HISFC.BizProcess.Cache.Common.GetEmployeeInfo(this.oper.ID);
                    ((Neusoft.HISFC.Models.Base.Employee)oper).Level = employee.Level;

                    #region add by lijp 2016-07-14 酒精、安尔碘等药品不允许医生开立，但是又不能停用，直接从药房申领。这里使用常数维护后剔除。
                    List<string> cantUseDrugCodeList = new List<string>();

                    using (Neusoft.HISFC.BizLogic.Manager.Constant constant = new Neusoft.HISFC.BizLogic.Manager.Constant())
                    {
                        ArrayList cantUseDrugList = constant.GetList("CantUseDrugList");

                        foreach (Neusoft.HISFC.Models.Base.Const cons in cantUseDrugList)
                        {
                            cantUseDrugCodeList.Add(cons.ID);
                        }
                    }

                    #endregion

                    //TODO: 加载药品和非药品列表
                    #region 加载药品

                    if (this.eShowItemType == EnumShowItemType.Pharmacy)
                    {
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem开始加载药品!\r\n");
                        if (this.deptcode == string.Empty)
                        {
                            alItem = new ArrayList(itemIntegrate.QueryItemAvailableList(true).ToArray());//显示所有
                            //add by chbf 216-3-22
                            //添加危急药品标志

                            foreach (Neusoft.HISFC.Models.Pharmacy.Item item in alItem)
                            {
                                #region add by lijp 2016-07-14 酒精、安尔碘等药品不允许医生开立，但是又不能停用，直接从药房申领。这里使用常数维护后剔除。
                                if (cantUseDrugCodeList.Contains(item.ID))
                                {
                                    continue;
                                }
                                #endregion

                                //sql_1 = string.Format(sql_1, item.ID);
                                //string special_flag = deptItemManager.ExecSqlReturnOne(sql_1);
                                string special_flag = item.SpecialFlag;
                                if (special_flag == "1")
                                {
                                    item.Name = "[危]" + item.Name;

                                }
                                Neusoft.HISFC.Models.Pharmacy.Item drugBase = Classes.Function.GetItem((Neusoft.HISFC.Models.Pharmacy.Item)item);
                                if (drugBase.ExtendData3 == "1")
                                {
                                    tempList1.Add(item);
                                }
                                else
                                {
                                    tempList.Add(item);
                                }

                                // {58B47A30-BB9D-4ed8-983B-5D067B0242E0}
                                if (item.ExtendData6 == "1")
                                {
                                    item.Name = "▲" + item.Name;
                                }

                            }
                            alItem = tempList;
                            alBYDrugItem.AddRange(tempList);
                            alZYDrugItem.AddRange(tempList1);
                        }
                        else
                        {
                            //关联库存表、默认取药科室获取有效库存列表
                            //arrAllItems = new ArrayList(itemIntegrate.QueryItemAvailableList(this.deptcode, this.oper.ID, ((Neusoft.HISFC.Models.Base.Employee)oper).Level.ID).ToArray());
                            //获取列表增加类别控制，发药类型：O 门诊处方、I 住院医嘱、A 全部
                            alItem = new ArrayList(itemIntegrate.QueryItemAvailableListBySendType(this.deptcode, this.oper.ID, ((Neusoft.HISFC.Models.Base.Employee)oper).Level.ID, drugSendType).ToArray());

                            //add by chbf 216-3-22
                            //添加危急药品标志

                            foreach (Neusoft.HISFC.Models.Pharmacy.Item item in alItem)
                            {
                                #region add by lijp 2016-07-14 酒精、安尔碘等药品不允许医生开立，但是又不能停用，直接从药房申领。这里使用常数维护后剔除。
                                if (cantUseDrugCodeList.Contains(item.ID))
                                {
                                    continue;
                                }
                                #endregion
                                //sql_1 = string.Format(sql_1, item.ID);
                                //string special_flag = deptItemManager.ExecSqlReturnOne(sql_1);
                                string special_flag = item.SpecialFlag;
                                if (special_flag == "1")
                                {
                                    item.Name = "[危]" + item.Name;

                                }
                                Neusoft.HISFC.Models.Pharmacy.Item drugBase = Classes.Function.GetItem((Neusoft.HISFC.Models.Pharmacy.Item)item);
                                if (drugBase.ExtendData3 == "1")
                                {
                                    tempList1.Add(item);
                                }
                                else
                                {
                                    tempList.Add(item);
                                }
                                // {58B47A30-BB9D-4ed8-983B-5D067B0242E0}
                                if (item.ExtendData6 == "1")
                                {
                                    item.Name = "▲" + item.Name;
                                }
                            }
                            alItem = tempList;
                            alBYDrugItem.AddRange(tempList);
                            alZYDrugItem.AddRange(tempList1);
                        }
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem结束加载药品!\r\n");
                    }

                    #endregion

                    #region 加载非药品
                    else if (this.eShowItemType == EnumShowItemType.Undrug)
                    {
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem开始加载非药品!\r\n");
                        if (this.isShowGroup)//显示组套
                        {
                            alItem = itemMgr.GetAvailableListWithGroup();
                        }
                        else//不显示组套
                        {
                            //arrAllItems = itemMgr.QueryValidItems();
                            //arrAllItems = itemMgr.QueryValidItemsForOrder(deptcode);
                            alItem = SOC.HISFC.BizProcess.Cache.Fee.GetValidItem();
                        }

                        #region {8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
                        //if (this.isIncludeMat)
                        //{
                        //    Neusoft.HISFC.BizProcess.Integrate.Material.Material matIntergrate = new Neusoft.HISFC.BizProcess.Integrate.Material.Material();
                        //    ArrayList al3 = new ArrayList();
                        //    if (!string.IsNullOrEmpty(this.deptcode))
                        //    {
                        //        al3 = new ArrayList(matIntergrate.QueryStockHeadItemForFee(this.deptcode).ToArray());
                        //    }
                        //    arrAllItems.AddRange(al3);
                        //}
                        #endregion

                        alItem = this.FilterUndrug(alItem);
                        alUndrugItem.AddRange(alItem);
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem结束加载非药品!\r\n");
                    }
                    #endregion

                    #region 外派药品信息
                    else if (this.eShowItemType == EnumShowItemType.OutPharmacy)
                    {
                        string outDept = contrIntegrate.GetControlParam<string>("BJ001", false, "");

                        alItem = new ArrayList(itemIntegrate.QueryItemAvailableListByBoJi(outDept, this.oper.ID, ((Neusoft.HISFC.Models.Base.Employee)oper).Level.ID, drugSendType).ToArray());

                        //add by chbf 216-3-22
                        //添加危急药品标志

                        foreach (Neusoft.HISFC.Models.Pharmacy.Item item in alItem)
                        {
                            #region add by lijp 2016-07-14 酒精、安尔碘等药品不允许医生开立，但是又不能停用，直接从药房申领。这里使用常数维护后剔除。
                            if (cantUseDrugCodeList.Contains(item.ID))
                            {
                                continue;
                            }
                            #endregion
                            //sql_1 = string.Format(sql_1, item.ID);
                            //string special_flag = deptItemManager.ExecSqlReturnOne(sql_1);
                            string special_flag = item.SpecialFlag;
                            if (special_flag == "1")
                            {
                                item.Name = "[危]" + item.Name;

                            }
                            Neusoft.HISFC.Models.Pharmacy.Item drugBase = Classes.Function.GetItem((Neusoft.HISFC.Models.Pharmacy.Item)item);
                            if (drugBase.ExtendData3 == "1")
                            {
                                tempList1.Add(item);
                            }
                            else
                            {
                                tempList.Add(item);
                            }
                            // {58B47A30-BB9D-4ed8-983B-5D067B0242E0}
                            if (item.ExtendData6 == "1")
                            {
                                item.Name = "▲" + item.Name;
                            }
                        }
                        alItem = tempList;
                        alBYDrugItem.AddRange(tempList);
                        alZYDrugItem.AddRange(tempList1);
                    }
                    #endregion

                    #region 加载全部
                    else
                    {
                        ArrayList al1 = null;
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem开始加载全部项目!\r\n");
                        if (this.deptcode == string.Empty)
                        {
                            //从药品基本信息表获取全部有效药品信息
                            al1 = new ArrayList(itemIntegrate.QueryItemAvailableList(true).ToArray());
                            //add by chbf 216-3-22
                            //添加危急药品标志

                            foreach (Neusoft.HISFC.Models.Pharmacy.Item item in al1)
                            {
                                #region add by lijp 2016-07-14 酒精、安尔碘等药品不允许医生开立，但是又不能停用，直接从药房申领。这里使用常数维护后剔除。
                                if (cantUseDrugCodeList.Contains(item.ID))
                                {
                                    continue;
                                }
                                #endregion
                                //sql_1 = string.Format(sql_1, item.ID);
                                //string special_flag = deptItemManager.ExecSqlReturnOne(sql_1);
                                string special_flag = item.SpecialFlag;
                                if (special_flag == "1")
                                {
                                    item.Name = "[危]" + item.Name;

                                }
                                Neusoft.HISFC.Models.Pharmacy.Item drugBase = Classes.Function.GetItem((Neusoft.HISFC.Models.Pharmacy.Item)item);
                                if (drugBase.ExtendData3 == "1")
                                {
                                    tempList1.Add(item);
                                }
                                else
                                {
                                    tempList.Add(item);
                                }
                                // {58B47A30-BB9D-4ed8-983B-5D067B0242E0}
                                if (item.ExtendData6 == "1")
                                {
                                    item.Name = "▲" + item.Name;
                                }
                            }
                            al1 = tempList;

                            alBYDrugItem.AddRange(tempList);
                            alZYDrugItem.AddRange(tempList1);
                        }
                        else
                        {
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载列表AddItems加载全部开始!\r\n");

                            //关联库存表、默认取药科室获取有效库存列表
                            //al1 = new ArrayList(itemIntegrate.QueryItemAvailableList(this.deptcode, this.oper.ID, ((Neusoft.HISFC.Models.Base.Employee)oper).Level.ID).ToArray());
                            //获取列表增加类别控制，发药类型：O 门诊处方、I 住院医嘱、A 全部
                            al1 = new ArrayList(itemIntegrate.QueryItemAvailableListBySendTypeZDWY(this.deptcode, this.oper.ID, ((Neusoft.HISFC.Models.Base.Employee)oper).Level.ID, drugSendType).ToArray());
                            //获取凤凰园药品
                            List<Neusoft.HISFC.Models.Pharmacy.Item> listFHY = itemIntegrate.QueryItemAvailableListByFHY(this.deptcode, this.oper.ID, drugSendType);
                            if (listFHY != null)
                            {
                                tempList1 = new ArrayList(listFHY.ToArray());
                            }
                            //add by chbf 216-3-22
                            //添加危急药品标志
                            string ucInputItemguid = Guid.NewGuid().ToString();
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("  " + ucInputItemguid + "ucInputItem加载列表AddItems的foreach开始,al1.count=" + al1.Count + "!\r\n");
                            foreach (Neusoft.HISFC.Models.Pharmacy.Item item in al1)
                            {
                                #region add by lijp 2016-07-14 酒精、安尔碘等药品不允许医生开立，但是又不能停用，直接从药房申领。这里使用常数维护后剔除。
                                if (cantUseDrugCodeList.Contains(item.ID))
                                {
                                    continue;
                                }
                                #endregion
                                //sql_1 = string.Format(sql_1, item.ID);
                                //string special_flag = deptItemManager.ExecSqlReturnOne(sql_1);
                                string special_flag = item.SpecialFlag;
                                if (special_flag == "1")
                                {
                                    item.Name = "[危]" + item.Name;

                                }
                                Neusoft.HISFC.Models.Pharmacy.Item drugBase = Classes.Function.GetItem((Neusoft.HISFC.Models.Pharmacy.Item)item);
                                if (drugSendType != "O")
                                {
                                    tempList.Add(item);
                                }
                                else
                                {
                                    if (drugBase.ExtendData3 == "1")
                                    {
                                        tempList1.Add(item);
                                    }
                                    else
                                    {
                                        tempList.Add(item);
                                    }
                                }
                                // {58B47A30-BB9D-4ed8-983B-5D067B0242E0}
                                if (item.ExtendData6 == "1")
                                {
                                    item.Name = "▲" + item.Name;
                                }
                            }

                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("  " + ucInputItemguid + "ucInputItem加载列表AddItems的foreach结束!\r\n");
                            al1 = tempList;
                            alBYDrugItem.AddRange(tempList);
                            alZYDrugItem.AddRange(tempList1);
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载列表AddItems加载全部结束!\r\n");

                        }

                        ArrayList al2 = null;
                        if (this.isShowGroup)//显示组套
                        {
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem显示组套@AddItems开始!\r\n");
                            al2 = itemMgr.GetAvailableListWithGroup();
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("al2.count3=" + al2.Count + "\r\n");
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem显示组套@AddItems结束!\r\n");
                        }
                        else//不显示组套
                        {
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem不显示组套@AddItems开始!\r\n");
                            //al2 = itemMgr.QueryValidItems();
                            //al2 = SOC.HISFC.BizProcess.Cache.Fee.GetValidItem();
                            //al2 = itemMgr.QueryValidItemsForOrder(deptcode);
                            al2 = SOC.HISFC.BizProcess.Cache.Fee.GetValidItem(false);
                            //if (al2.Count==0)
                            //{
                            //    Thread.Sleep(5000);
                            //}
                            //al2 = SOC.HISFC.BizProcess.Cache.Fee.GetValidItem(false);


                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("al2.count4=" + al2.Count + "\r\n");
                            Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem不显示组套@AddItems结束!======" + al2.Count + "\r\n");
                        }
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("al2.count1=" + al2.Count + "\r\n");

                        al2 = this.FilterUndrug(al2);
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("al2.count2=" + al2.Count + "\r\n");
                        alUndrugItem.AddRange(al2);
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("alUndrugItem.count1=" + alUndrugItem.Count + "\r\n");
                        alItem = al1;
                        al1.AddRange(al2);
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("al1.count1=" + al1.Count + "\r\n");

                        ArrayList alTempLong = new ArrayList();
                        alTempLong.AddRange(al1);
                        alTempLong.AddRange(alLongItem);
                        alLongItem = alTempLong;

                        ArrayList alTempShort = new ArrayList();
                        alTempShort.AddRange(al1);
                        if (drugSendType == "I")
                        {
                            alTempShort.AddRange(tempList1);
                        }
                        //alTempShort.AddRange(alShortItem);
                        alShortItem = alTempShort;

                        #region {8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
                        //if (this.isIncludeMat)
                        //{
                        //    Neusoft.HISFC.BizProcess.Integrate.Material.Material matIntergrate = new Neusoft.HISFC.BizProcess.Integrate.Material.Material();
                        //    ArrayList al3 = new ArrayList();
                        //    if (!string.IsNullOrEmpty(this.deptcode))
                        //    {
                        //        al3 = new ArrayList(matIntergrate.QueryStockHeadItemForFee(this.deptcode).ToArray());
                        //    }
                        //    al1.AddRange(al3);
                        //}
                        #endregion
                        alItem = al1;
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem结束加载全部项目!alItem.count=" + alItem.Count + "\r\n");
                    }
                    #endregion

                    itemHelper.ArrayObject.AddRange(alItem);

                    //如设置显示科室组套(手术组套/护士组套)
                    if (this.bShowDeptGroup)
                    {
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem设置显示科室组套AddDeptGroup信息@AddItems开始!\r\n");
                        this.AddDeptGroup();

                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem设置显示科室组套AddDeptGroup信息@AddItems结束!\r\n");
                    }
                    //*********************************

                    //if (!bIsListShowAlways)
                    //{
                    //    this.RefreshFP();
                    //}

                    #region 加载医保对照信息 废弃 多个医保无法处理

                    if (isUserThread == 3)
                    {
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载医保对照信息@AddItems开始!\r\n");
                        DataSet dsTemp = new DataSet();

                        if (this.phaManger.ExecQuery(@"select t.pact_code,t.his_code,t.center_item_grade 
                                                        from fin_com_compare t
                                                        union 
                                                        select f.pact_code,f.fee_code,'all'
                                                        from fin_com_pactunitfeecoderate f
                                                        union 
                                                        select 'all',g.item_code,g.item_grade
                                                        from FIN_COM_ITEM_EXTENDINFO g", ref dsTemp) == -1)
                        {
                            MessageBox.Show("AddItems" + phaManger.Err);
                        }
                        if (dsTemp != null)
                        {
                            hsPactColumn = new Hashtable();
                            hsItemPactInfo = new Hashtable();
                            foreach (DataRow drow in dsTemp.Tables[0].Rows)
                            {
                                if (!hsPactColumn.Contains(drow[0].ToString()))
                                {
                                    hsPactColumn.Add(drow[0].ToString(), null);
                                }

                                if (!hsItemPactInfo.Contains(drow[0].ToString() + drow[1].ToString()))
                                {
                                    hsItemPactInfo.Add(drow[0].ToString() + drow[1].ToString(), drow[2].ToString());
                                }
                            }

                            Classes.Function.HsItemPactInfo = hsItemPactInfo;
                        }
                        Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载医保对照信息@AddItems结束!\r\n");
                    }
                    #endregion
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("AddItems" + ee.Message);
            }
            return 1;
        }

        /// <summary>
        /// 切换本院和自营药房药品项目
        /// </summary>
        /// <param name="drugStoreClass"></param>
        public void SwitchDrugItem(string drugStoreClass)
        {
            if (drugStoreClass == "1")
            {
                this.chkoutflag.Visible = false;
            }
            else
            {
                this.chkoutflag.Visible = true;
            }
            itemHelper.ArrayObject.Clear();
            ArrayList alAll = new ArrayList();
            ArrayList al1 = new ArrayList();
            ArrayList al2 = alUndrugItem;
            if (drugStoreClass == "1")
            {
                //自营药房项目
                al1 = alZYDrugItem;
                alItem = al1;
                //al1.AddRange(al2);
                alAll.AddRange(al1);

                ArrayList alTempLong = new ArrayList();
                alTempLong.AddRange(al1);
                alTempLong.AddRange(alLongItem);
                alLongItem = alTempLong;

                ArrayList alTempShort = new ArrayList();
                alTempShort.AddRange(al1);
                //alTempShort.AddRange(alShortItem);
                alShortItem = alTempShort;
                alItem = alAll;
                itemHelper.ArrayObject.AddRange(alItem);

                //如设置显示科室组套(手术组套/护士组套)
                if (this.bShowDeptGroup)
                {
                    this.AddDeptGroup();
                }
            }
            else
            {
                al1 = alBYDrugItem;
                alItem = al1;
                //al1.AddRange(al2);
                alAll.AddRange(al1);
                alAll.AddRange(al2);

                ArrayList alTempLong = new ArrayList();
                alTempLong.AddRange(al1);
                alTempLong.AddRange(al2);
                //alTempLong.AddRange(alLongItem);
                alLongItem = alTempLong;

                ArrayList alTempShort = new ArrayList();
                alTempShort.AddRange(al1);
                alTempShort.AddRange(al2);
                //alTempShort.AddRange(alShortItem);
                alShortItem = alTempShort;
                alItem = alAll;
                itemHelper.ArrayObject.AddRange(alItem);

                //如设置显示科室组套(手术组套/护士组套)
                if (this.bShowDeptGroup)
                {
                    this.AddDeptGroup();
                }
            }

            this.RefreshFP(true);
        }

        /// <summary>
        /// 加载科室组套 (护士组套、手术组套)
        /// </summary>
        /// <returns></returns>
        protected virtual void AddDeptGroup()
        {
            if (this.deptcode == null) return;
            //添加组套
            Neusoft.HISFC.BizLogic.Manager.ComGroup group = new Neusoft.HISFC.BizLogic.Manager.ComGroup();
            ArrayList al = group.GetValidGroupList(this.deptcode);

            if (al == null)
                return;
            if (alItem == null)
            {
                alItem = new ArrayList();
            }
            alItem.AddRange(al);
        }

        /// <summary>
        /// 加载列表
        /// </summary>
        public virtual void AddItem()
        {
            // [2007/02/08 徐伟哲]
            // 关键节同步对象,排它访问
            try
            {
                System.Threading.Monitor.Enter(this);

                bCanChange = false;
                if (this.bIsListShowAlways == false)
                {
                    this.txtItemCode.Text = "加载中,请稍候...";
                    this.txtItemCode.Enabled = false;
                }

                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载列表AddItems开始!\r\n");
                this.AddItems();

                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("ucInputItem加载列表AddItems结束!\r\n");
                RefreshFP();//这个耗时间很长 2021-11-26 by yhm 得优化
                this.txtItemCode.Text = "";
                this.txtItemCode.Enabled = true;
                if (this.frmShowItem.Visible)
                {
                    this.frmShowItem.Visible = false;
                }

                bCanChange = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddItem" + e.Message);
            }
            finally
            {
                System.Threading.Monitor.Exit(this);
                bCanChange = true;
            }

            // 释放关键节,其他线程可以访问           
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        public virtual void RefreshFP()
        {
            //更新FPList
            //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
            if (alItem != null && (MyDataSet == null || this.isIncludeMat))
            {
                //MyDataSet = this.CreateDataSet(this.alItem);
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP获取fp数据开始!\r\n");
                longDataSet = CreateDataSet(alLongItem);
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP获取fp数据开始!长嘱数量:" + alLongItem.Count + "\r\n");
                longDV = new DataView(longDataSet.Tables[0]);
                shortDataSet = CreateDataSet(alShortItem);
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP获取fp数据开始!临嘱数量:" + alShortItem.Count + "\r\n");
                shortDV = new DataView(shortDataSet.Tables[0]);

                myDeptDataSet = this.CreateDataSet(arrDeptUsed);
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP获取fp数据开始!科室常用数量:" + arrDeptUsed.Count + "\r\n");

                dvDeptUsed = new DataView(myDeptDataSet.Tables[0]);
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP获取fp数据结束!\r\n");
                //dv = new DataView(MyDataSet.Tables[0]);
                //fpItemList.Sheets[0].DataSource = dv;
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP渲染fp数据开始!\r\n");
                if (longOrder)
                {
                    fpItemList.Sheets[0].DataSource = longDV;
                }
                else
                {
                    fpItemList.Sheets[0].DataSource = shortDV;
                }
               
                //dv.Sort = "自定义码 ASC";
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.ItemCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.ItemName].Width = 150;
                // {E5BE0123-FD83-46a1-9455-16D9E7F0918A}执行科室放到名称后面


                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SysClassName].Width = 50;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Specs].Width = 90;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Price].Width = 40;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Unit].Width = 30;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SiFlag].Width = 40;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SiFlag].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Product].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SysClassCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SpellCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.WBCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.UserCode].Width = 50;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.RegularName].Width = 120;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.RegularNameSpellCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.WBCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.RegularNameUserCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.EnglishName].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.StorageQty].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.LackFlag].ForeColor = Color.Red;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.LackFlag].Font = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Bold);
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.OutFlag].Visible = false;

                //后面的医保标记列都 加粗 红色
                for (int i = (int)EnumMainColumnSet.LackFlag; i < fpItemList.Sheets[0].ColumnCount; i++)
                {
                    this.fpItemList.Sheets[0].Columns[i].ForeColor = Color.Red;
                    this.fpItemList.Sheets[0].Columns[i].Font = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Bold);
                }

                if (this.IsListShowAlways == false)
                {
                    Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP渲染frmShowItem数据开始!\r\n");
                    frmShowItem.DataView = MyDataView;
                    frmShowItem.RefreshFP();
                    Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP渲染frmShowItem数据结束!\r\n");
                }
                Neusoft.FrameWork.Function.OrderLogManager.WriteLogAndMemory("RefreshFP渲染fp数据结束!\r\n");
            }
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        public virtual void RefreshFP(bool isAlwaysRefresh)
        {
            //更新FPList
            //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
            if (alItem != null && isAlwaysRefresh)
            {
                //MyDataSet = this.CreateDataSet(this.alItem);

                longDataSet = CreateDataSet(alLongItem);
                longDV = new DataView(longDataSet.Tables[0]);
                shortDataSet = CreateDataSet(alShortItem);
                shortDV = new DataView(shortDataSet.Tables[0]);

                myDeptDataSet = this.CreateDataSet(arrDeptUsed);

                dvDeptUsed = new DataView(myDeptDataSet.Tables[0]);

                //dv = new DataView(MyDataSet.Tables[0]);
                //fpItemList.Sheets[0].DataSource = dv;

                if (longOrder)
                {
                    fpItemList.Sheets[0].DataSource = longDV;
                }
                else
                {
                    fpItemList.Sheets[0].DataSource = shortDV;
                }

                //dv.Sort = "自定义码 ASC";
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.ItemCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.ItemName].Width = 150;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SysClassName].Width = 50;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Specs].Width = 90;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Price].Width = 40;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Unit].Width = 30;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SiFlag].Width = 40;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SiFlag].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.Product].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SysClassCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.SpellCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.WBCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.UserCode].Width = 50;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.RegularName].Width = 120;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.RegularNameSpellCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.WBCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.RegularNameUserCode].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.EnglishName].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.StorageQty].Visible = false;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.LackFlag].ForeColor = Color.Red;
                this.fpItemList.Sheets[0].Columns[(int)EnumMainColumnSet.LackFlag].Font = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Bold);

                //后面的医保标记列都 加粗 红色
                for (int i = (int)EnumMainColumnSet.LackFlag; i < fpItemList.Sheets[0].ColumnCount; i++)
                {
                    this.fpItemList.Sheets[0].Columns[i].ForeColor = Color.Red;
                    this.fpItemList.Sheets[0].Columns[i].Font = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Bold);
                }

                if (this.IsListShowAlways == false)
                {
                    frmShowItem.DataView = MyDataView;
                    frmShowItem.RefreshFP();
                }
            }
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        protected virtual DataSet CreateDataSet(ArrayList al)
        {
            DataSet myDataSet = new DataSet();
            myDataSet.EnforceConstraints = false;//是否遵循约束规则
            //定义类型
            System.Type dtStr = System.Type.GetType("System.String");
            System.Type dtInt = System.Type.GetType("System.Int32");

            //定义表********************************************************
            //Main Table
            DataTable dtMain;
            dtMain = myDataSet.Tables.Add("Table");
            dtMain.Columns.AddRange(new DataColumn[]
            { 
                new DataColumn("编码",dtStr),//0
                new DataColumn("名称", dtStr),//1
                // {E5BE0123-FD83-46a1-9455-16D9E7F0918A}执行科室放到名称后面
                new DataColumn("执行科室", dtStr), //18

                new DataColumn("类别", dtStr),//2
                new DataColumn("类别编码",dtStr),//3
                new DataColumn("规格", dtStr),//4
                new DataColumn("价格",dtStr),//5
                new DataColumn("单位",dtStr),//6
                new DataColumn("医保标记",dtStr),//7
                new DataColumn("厂家", dtStr),//8
                new DataColumn("拼音码", dtStr),//9
                new DataColumn("五笔码", dtStr),//10
                new DataColumn("自定义码", dtStr),//11
                new DataColumn("通用名", dtStr),//12
                new DataColumn("通用名拼音码", dtStr),//13
                new DataColumn("通用名五笔码", dtStr),//14
                new DataColumn("通用名自定义码", dtStr),//15
                new DataColumn("英文商品名", dtStr),//16
                new DataColumn("库存可用数量", dtStr),
                //new DataColumn("执行科室", dtStr), //18
                new DataColumn("检查检体", dtStr),
                new DataColumn("疾病分类", dtStr),
                new DataColumn("专科名称", dtStr),
                new DataColumn("病史及检查", dtStr),
                new DataColumn("检查要求", dtStr),
                new DataColumn("注意事项", dtStr),
                new DataColumn("缺",dtStr),
                new DataColumn("外延药品",dtStr)
            });

            if (isUserThread == 3)
            {
                if (hsPactColumn != null && hsPactColumn.Count > 0)
                {
                    Hashtable hsTemp = new Hashtable();
                    ArrayList alTemp = new ArrayList(hsPactColumn.Keys);
                    alTemp.Sort();

                    foreach (string key in alTemp)
                    {
                        dtMain.Columns.Add("标记" + key);
                        //hsPactColumn[key] = dtMain.Columns.Count;
                        hsTemp.Add(key, dtMain.Columns.Count - 1);
                    }

                    hsPactColumn = hsTemp;
                }
            }

            if (this.IGetSiFlag == null)
            {
                this.iGetSiFlag = null;
            }

            string itemCode = "";


            //处理宜康门诊的价格显示：宜康门诊显示特诊价
            Neusoft.HISFC.BizLogic.Manager.Constant conMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();
            ArrayList alYKDept = conMgr.GetAllList("YkDept");
            Hashtable hsYKDept = null;
            if (alYKDept != null)
            {
                hsYKDept = new Hashtable();
                foreach (Neusoft.FrameWork.Models.NeuObject obj in alYKDept)
                {
                    if (!hsYKDept.Contains(obj.ID))
                    {
                        hsYKDept.Add(obj.ID, obj);
                    }
                }
            }

            //显示的价格
            decimal showPrice = 0;
            for (int i = 0; i < al.Count; i++)
            {
                if (al[i].GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))
                {
                    #region 药品

                    Neusoft.HISFC.Models.Pharmacy.Item obj;
                    obj = (Neusoft.HISFC.Models.Pharmacy.Item)al[i];
                    if (obj.User02 != string.Empty)
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item drugBase = Classes.Function.GetItem((Neusoft.HISFC.Models.Pharmacy.Item)obj);
                        if (drugBase.ExtendData3 == "1")
                        {
                            obj.User03 = Neusoft.SOC.HISFC.BizProcess.Cache.Common.GetDeptNameFHY(obj.User02);
                        }
                        else
                        {
                            obj.User03 = Neusoft.SOC.HISFC.BizProcess.Cache.Common.GetDeptName(obj.User02);
                        }
                        al[i] = obj;
                    }

                    //if (hsYKDept.Contains(((Neusoft.HISFC.Models.Base.Employee)oper).Dept.ID))
                    //{
                    //    showPrice = obj.SpecialPrice;
                    //}
                    //else
                    //{
                    if (isUserRetailPrice2)
                    {
                        //showPrice = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(obj.ID).RetailPrice2;
                        showPrice = Classes.Function.GetItem(obj.ID).RetailPrice2;
                    }
                    else
                    {
                        showPrice = obj.Price;
                    }
                    //}

                    dtMain.Rows.Add(new object[] {
                                    obj.ID,//编码
                                    obj.Name,//名称
                                    // {E5BE0123-FD83-46a1-9455-16D9E7F0918A}执行科室放到名称后面
                                    obj.User03,//执行科室

                                    obj.SysClass.Name, //系统类别
                                    obj.SysClass.ID,//系统类别编码
                                    obj.Specs, //规格
                                    showPrice, //价格
                                    obj.PriceUnit, //单位
                                    Neusoft.HISFC.BizLogic.Fee.Interface.ShowItemGradeByCode(obj.Grade) + Neusoft.HISFC.Components.Common.Classes.Function.ShowItemFlag(obj), //医保标记
                                    obj.Product.Producer.Name,//生产厂家
                                    obj.SpellCode,//拼音码
                                    obj.WBCode,//五笔码
                                    obj.UserCode, //自定义码
                                    obj.NameCollection.RegularName,//通用名（别名）
                                    obj.NameCollection.RegularSpell.SpellCode,//通用名（别名）拼音码
                                    obj.NameCollection.RegularSpell.WBCode,//通用名（别名）五笔码
                                    obj.NameCollection.RegularSpell.UserCode,//通用名（别名）自定义码
                                    obj.NameCollection.EnglishName,//英文名
                                    obj.User01,//库存数

                                    //obj.User03,//执行科室
                                    string.Empty,//检查检体
                                    string.Empty,//疾病分类
                                    string.Empty,//专科名称
                                    string.Empty,//病史及检查
                                    string.Empty,//检查要求
                                    string.Empty,//注意事项
                                    obj.IsLack?"是":"",//是否缺药
                                    obj.Extend1  //外延药品 
                    });

                    itemCode = obj.ID;

                    #endregion
                    al[i] = obj;
                }
                else if (al[i].GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))
                {
                    #region 非药品及复合项目
                    Neusoft.HISFC.Models.Fee.Item.Undrug undrug = (Neusoft.HISFC.Models.Fee.Item.Undrug)al[i];

                    #region 使用范围过滤

                    if (eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.Clinic)
                    {
                        if (isShowDetailItem.Substring(0, 1) == "0"
                            && undrug.SysClass.ID.ToString() == "UC")
                        {
                            if (undrug.UnitFlag != "1")
                            {
                                continue;
                            }
                        }
                        if (isShowDetailItem.Substring(1, 1) == "0"
                            && undrug.SysClass.ID.ToString() == "UL")
                        {
                            if (undrug.UnitFlag != "1")
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (isShowDetailItem.Substring(2, 1) == "0"
                            && undrug.SysClass.ID.ToString() == "UC")
                        {
                            if (undrug.UnitFlag != "1")
                            {
                                continue;
                            }
                        }
                        if (isShowDetailItem.Substring(3, 1) == "0"
                            && undrug.SysClass.ID.ToString() == "UL")
                        {
                            if (undrug.UnitFlag != "1")
                            {
                                continue;
                            }
                        }
                    }
                    #endregion

                    #region 根据该项目允许开立科室过滤项目

                    bool val = false;
                    string[] deptList = null;
                    if (!isFilteUndrugRecipeDept || undrug.DeptList == ""
                        || undrug.DeptList.ToUpper() == "ALL" || undrug.DeptList == null)
                    {
                        val = true;
                    }
                    else
                    {
                        if (this.deptLogin == null)
                        {
                            this.deptLogin = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept;
                        }
                        deptList = undrug.DeptList.Split('|');
                        for (int j = 0; j < deptList.Length; j++)
                        {
                            if (deptList[j].ToString() == this.deptLogin.ID.ToString())
                            {
                                val = true;
                                break;
                            }
                        }
                    }
                    #endregion

                    if (undrug.ExecDept != string.Empty)
                    {
                        try
                        {
                            string[] execDept = undrug.ExecDept.Split('|');
                            undrug.User01 = "";
                            for (int k = 0; k < execDept.Length; k++)
                            {
                                if (!string.IsNullOrEmpty(execDept[k]))
                                {
                                    undrug.User01 += (string.IsNullOrEmpty(undrug.User01) ? "" : "、") + Neusoft.SOC.HISFC.BizProcess.Cache.Common.GetDeptName(execDept[k]);
                                }
                            }
                            //this.arrAllItems[i] = obj;
                        }
                        catch { }
                    }

                    if (val)
                    {
                        if (hsYKDept.Contains(((Neusoft.HISFC.Models.Base.Employee)oper).Dept.ID))
                        {
                            showPrice = undrug.SpecialPrice == 0 ? undrug.Price : undrug.SpecialPrice;
                        }
                        else
                        {
                            showPrice = undrug.Price;
                        }

                        undrug.Price = showPrice;

                        dtMain.Rows.Add(new Object[] { 
                                            undrug.ID,//编码
                                            undrug.Name,//名称
                                            // {E5BE0123-FD83-46a1-9455-16D9E7F0918A}执行科室放到名称后面
                                            undrug.User01,//执行科室

                                            undrug.SysClass.Name,//系统类别
                                            undrug.SysClass.ID,//系统类别编码
                                            undrug.Specs,//规格
                                            showPrice,//价格
                                            undrug.PriceUnit,//单位
                                            Neusoft.HISFC.BizLogic.Fee.Interface.ShowItemGradeByCode(undrug.Grade) + Neusoft.HISFC.Components.Common.Classes.Function.ShowItemFlag(undrug),//医保标记
                                            string.Empty,//生产厂家
                                            undrug.SpellCode,//拼音码
                                            undrug.WBCode,//五笔码
                                            undrug.UserCode,//自定义码
                                            undrug.NameCollection.OtherName,//通用名（别名）
                                            undrug.NameCollection.OtherSpell.SpellCode,//通用名（别名）拼音码
                                            undrug.NameCollection.OtherSpell.WBCode,//通用名（别名）五笔码
                                            undrug.NameCollection.OtherSpell.UserCode,//通用名（别名）自定义码
                                            string.Empty,//英文名
                                            string.Empty,//库存数
                                            //undrug.User01,//执行科室
                                            undrug.CheckBody,//检查检体
                                            undrug.DiseaseType,//疾病分类
                                            undrug.SpecialDept,//专科名称
                                            undrug.MedicalRecord,//病史及检查
                                            undrug.CheckRequest,//检查要求
                                            undrug.Notice,//注意事项
                                            undrug.IsValid ? "" : "是" //是否缺药
                        });
                    }

                    itemCode = undrug.ID;

                    #endregion
                    al[i] = undrug;
                }
                else
                {
                    #region 科室组套

                    Neusoft.HISFC.Models.Fee.ComGroup obj;
                    obj = al[i] as Neusoft.HISFC.Models.Fee.ComGroup;
                    if (obj == null) continue;
                    dtMain.Rows.Add(new Object[] {
                            obj.ID,//编码
                            obj.Name,//名称
                            // {E5BE0123-FD83-46a1-9455-16D9E7F0918A}执行科室放到名称后面
                            string.Empty,//执行科室

                            "非药品",//系统类别
                            "U",//系统类别编码
                            string.Empty,//规格
                            0.0,//价格
                            "[组套]",//单位
                            string.Empty,//医保标记
                            string.Empty,//生产厂家
                            obj.spellCode,//拼音码
                            string.Empty,//五笔码
                            obj.inputCode,//自定义码
                            string.Empty,//通用名（别名）
                            string.Empty,//通用名（别名）拼音码
                            string.Empty,//通用名（别名）五笔码
                            string.Empty,//通用名（别名）自定义码
                            string.Empty,//英文名
                            string.Empty,//库存数
                            //string.Empty,//执行科室
                            string.Empty,//检查检体
                            string.Empty,//疾病分类
                            string.Empty,//专科名称
                            string.Empty,//病史及检查
                            string.Empty,//检查要求
                            string.Empty,//注意事项
                            string.Empty //是否缺药
                    });

                    itemCode = obj.ID;

                    #endregion
                    al[i] = obj;
                }

                if (isUserThread == 3)
                {
                    if (hsPactColumn != null && hsPactColumn.Count > 0)
                    {
                        foreach (string key in hsPactColumn.Keys)
                        {
                            dtMain.Rows[dtMain.Rows.Count - 1][(Int32)hsPactColumn[key]] = Neusoft.HISFC.BizLogic.Fee.Interface.ShowItemGradeByCode(hsItemPactInfo[key + itemCode] == null ? "" : hsItemPactInfo[key + itemCode].ToString());
                        }
                    }
                }
            }
            return myDataSet;
        }

        //Hashtable hsUCULItem = null;

        /// <summary>
        /// 过滤非药品（门诊、住院、全部）
        /// </summary>
        /// <param name="alUndrug"></param>
        /// <returns></returns>
        protected virtual ArrayList FilterUndrug(ArrayList alUndrug)
        {
            ArrayList al = new ArrayList();

            alLongItem = new ArrayList();
            alShortItem = new ArrayList();

            foreach (Neusoft.HISFC.Models.Fee.Item.Undrug undrug in alUndrug)
            {
                if (!undrug.IsValid)
                {
                    continue;
                }

                #region 按允许开立科室过滤

                if (!string.IsNullOrEmpty(undrug.DeptList)
                    && undrug.DeptList.Trim().ToUpper() != "ALL"
                    && !undrug.DeptList.Contains(this.deptcode))
                {
                    continue;
                }

                #endregion

                #region 按使用范围过滤
                if (this.eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.All)
                {
                }
                else if (this.eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.Clinic)
                {
                    if (undrug.ApplicabilityArea == "0"
                        || undrug.ApplicabilityArea == "1")
                    {
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (undrug.ApplicabilityArea == "0"
                        || undrug.ApplicabilityArea == "2")
                    {
                    }
                    else
                    {
                        continue;
                    }
                }
                #endregion

                al.Add(undrug);

                #region 按长嘱是否允许开立过滤

                //if (hsUCULItem == null)
                //{
                //    hsUCULItem = new Hashtable();
                //    ArrayList alUCUL = this.interMgr.GetConstantList("LongUCUL");
                //    foreach (Neusoft.HISFC.Models.Base.Const con in alUCUL)
                //    {
                //        if (con.IsValid && !hsUCULItem.Contains(con.ID))
                //        {
                //            hsUCULItem.Add(con.ID, null);
                //        }
                //    }
                //}
                //if (hsUCULItem != null
                //    && "UC、UL".Contains(undrug.SysClass.ID.ToString())
                //    && hsUCULItem.Contains(undrug.ID))
                //{
                //    alLongItem.Add(undrug);
                //}
                if (Classes.Function.isUCUCForLong(undrug))
                {
                    alLongItem.Add(undrug);
                }

                #endregion

                alShortItem.Add(undrug);
            }

            return al;

            #region 旧的作废

            //if (this.eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.All)
            //{
            //    return alUndrug;
            //}
            //else if (this.eUndrugApplicabilityarea == EnumUndrugApplicabilityarea.Clinic)
            //{
            //    foreach (Neusoft.HISFC.Models.Fee.Item.Undrug undrug in alUndrug)
            //    {
            //        if (undrug.ApplicabilityArea == "0")
            //        {
            //            al.Add(undrug);
            //        }
            //        else if (undrug.ApplicabilityArea == "1")
            //        {
            //            al.Add(undrug);
            //        }
            //    }
            //    return al;
            //}
            //else
            //{
            //    foreach (Neusoft.HISFC.Models.Fee.Item.Undrug undrug in alUndrug)
            //    {
            //        if (undrug.ApplicabilityArea == "0")
            //        {
            //            al.Add(undrug);
            //        }
            //        else if (undrug.ApplicabilityArea == "2")
            //        {
            //            al.Add(undrug);
            //        }
            //    }
            //    return al;
            //}

            #endregion
        }

        /// <summary>
        /// 刷新列表
        /// {112B7DB5-0462-4432-AD9D-17A7912FFDBE} 
        /// </summary>
        [Obsolete("作废", true)]
        public virtual void RefreshSIFlag()
        {
            if (MyDataSet != null && MyDataSet.Tables.Count > 0)
            {
                if (this.IGetSiFlag != null)
                {
                    foreach (DataRow dr in MyDataSet.Tables[0].Rows)
                    {
                        //{112B7DB5-0462-4432-AD9D-17A7912FFDBE}   获取医保项目标记

                        string itemGrade = "0";
                        if (this.patient != null && this.patient.Pact.ID != "")
                        {
                            if (this.iGetSiFlag.GetSiItemGrade(this.patient.Pact.ID, dr["编码"].ToString(), ref itemGrade) != -1)
                            {
                                dr["医保标记"] = Neusoft.HISFC.BizLogic.Fee.Interface.ShowItemGradeByCode(itemGrade);
                            }
                        }
                        else
                        {
                            if (this.iGetSiFlag.GetSiItemGrade(dr["编码"].ToString(), ref itemGrade) != -1)
                            {
                                dr["医保标记"] = Neusoft.HISFC.BizLogic.Fee.Interface.ShowItemGradeByCode(itemGrade);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected static System.Data.DataView MyDataView
        {
            set
            {
                shortDV = value;
            }
            get
            {
                if (longOrder)
                {
                    return longDV;
                }
                else
                {
                    return shortDV;
                }
            }
        }

        private static DataView longDV = null;
        private static DataView shortDV = null;

        /// <summary>
        /// 科室常用项目
        /// </summary>
        protected static System.Data.DataView dvDeptUsed;

        /// <summary>
        /// 是否第一次过滤，第一次过滤需要启动线程刷新停用标记
        /// </summary>
        //bool isFirst = false;

        /// <summary>
        /// 是否正在刷新
        /// </summary>
        //bool isInFresh = false;

        /// <summary>
        /// 是否当前列表已刷新完毕
        /// </summary>
        object isFreshFinish;

        /// <summary>
        /// 变化项目
        /// </summary>
        protected virtual void ChangeItem()
        {
            //TODO:过滤列表，与输入法有关
            if (MyDataSet == null)
            {
                return;
            }

            try
            {
                this.myShowList(); //显示列表
                //判断当前类别过滤DataSet
                if (MyDataSet == null)
                {
                    return;
                }
                if (MyDataSet.Tables.Count <= 0)
                {
                    return;
                }

                string sCategory = " and 类别编码 = '" + this.cmbCategory.Tag + "'";
                if (this.cmbCategory.Text == Neusoft.FrameWork.Management.Language.Msg("全部"))
                {
                    //因为全部包含的类别可能不同，所以不能这样写,by huangxw
                    sCategory = string.Empty;
                    foreach (Neusoft.FrameWork.Models.NeuObject obj in this.cmbCategory.alItems)
                    {
                        if (obj.Name != Neusoft.FrameWork.Management.Language.Msg("全部"))
                            sCategory = sCategory + " or 类别编码 = '" + obj.ID + "'" + " or 类别编码 = 'MF'";
                    }
                    if (sCategory != string.Empty)
                    {
                        sCategory = sCategory.Substring(3);//去掉第一个or
                        sCategory = " and (" + sCategory + ")";
                    }
                }

                ////需要屏蔽 
                //if (frmShowItem.IsOUT)
                //{
                //    sCategory += "  and 外延药品 = '1' ";
                //}


                string sInput = string.Empty;
                //取输入码
                string[] spChar = new string[] { "@", "#", "$", "%", "^", "&", "[", "]", "|" };
                string queryCode = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(this.txtItemCode.Text.Trim(), spChar);
                queryCode = queryCode.Replace("*", "[*]");

                //根据是否精确查找，决定是否进行模糊查询
                if (this.frmShowItem.IsReal == false)
                {
                    queryCode = '%' + Neusoft.FrameWork.Public.String.TakeOffSpecialChar(queryCode) + '%';
                }
                else
                {
                    queryCode = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(queryCode) + '%';
                }
                if (queryCode == "%%")
                {
                    queryCode = "%";
                }
                // {C81FD8D1-B166-45cb-8681-4F5AE2BFBFF2} 通过空格符号过滤两个条件如 123 通过1空格3 过滤
                string param1 = string.Empty;
                string param2 = string.Empty;
                if (queryCode.Contains(" ") && !queryCode.StartsWith(" ") && Regex.Matches(queryCode, " ").Count == 1)
                {

                    string[] param = queryCode.Split(' ');
                    param1 = param[0] + "%";
                    param2 = "%" + param[1];
                    sInput = "(";

                    if (frmShowItem.IsFilterSpellCode)
                    {
                        sInput += "(拼音码 LIKE '{0}' AND 拼音码 LIKE '{1}') or " + "(通用名拼音码 LIKE '{0}' AND 通用名拼音码 LIKE '{1}') or ";
                    }
                    if (frmShowItem.IsFilterWBCode)
                    {
                        sInput += "(五笔码 LIKE '{0}' AND 五笔码 LIKE '{1}') or " + "(通用名五笔码 LIKE '{0}' AND 通用名五笔码 LIKE '{1}') or ";
                    }
                    sInput += "(自定义码 LIKE '{0}' AND 自定义码 LIKE '{1}') or " + "(通用名自定义码 LIKE '{0}' AND 通用名自定义码 LIKE '{1}')  or " + "(英文商品名 LIKE '{0}' AND 通用名自定义码 LIKE '{1}') or " + "(名称 LIKE '{0}' AND 名称 LIKE '{1}') or " + "(通用名 LIKE '{0}' AND 通用名 LIKE '{1}'))";


                    sInput = string.Format(sInput, param1, param2);
                }
                else
                {

                    sInput = "(";
                    if (frmShowItem.IsFilterSpellCode)
                    {
                        sInput += "拼音码 LIKE '{0}' or " + "通用名拼音码 LIKE '{0}' or ";
                    }
                    if (frmShowItem.IsFilterWBCode)
                    {
                        sInput += "五笔码 LIKE '{0}' or " + "通用名五笔码 LIKE '{0}' or ";
                    }
                    sInput += "自定义码 LIKE '{0}' or " + "通用名自定义码 LIKE '{0}' or " + "英文商品名 LIKE '{0}' or " + "名称 LIKE '{0}' or " + "通用名 LIKE '{0}')";


                    sInput = string.Format(sInput, queryCode);
                }

                sInput = sInput + sCategory;
                //过滤
                #region 取药科室过滤

                //{ECAE27F0-CC52-46be-A8C5-BC9F680988CD}
                if (isShowCmbDrugDept)
                {
                    string filterDrugDept = string.Empty;
                    string filterUndrug = string.Empty;
                    if (frmShowItem.cmbDrugDept.alItems != null && frmShowItem.cmbDrugDept.alItems.Count > 0)
                    {
                        if (frmShowItem.cmbDrugDept.Tag != null && frmShowItem.cmbDrugDept.Tag.ToString() != "ALL")
                        {
                            filterDrugDept = " and (类别编码 in ('P','PCZ','PCC') and  执行科室 = '" + frmShowItem.cmbDrugDept.Text + "')";
                            filterUndrug = " or (" + sInput + "and (类别编码 not in ('P','PCZ','PCC')))";
                        }
                        else
                        {
                            filterDrugDept = "";
                            filterUndrug = "";
                        }


                        #region 外延处方 MK

                        if (!string.IsNullOrEmpty(this.extendDrugStore) && this.drugDeptsHelper != null)
                        {
                            Neusoft.FrameWork.Models.NeuObject objSpecialDrugDept = this.drugDeptsHelper.GetObjectFromID(this.extendDrugStore);
                            if (objSpecialDrugDept != null && !string.IsNullOrEmpty(objSpecialDrugDept.ID))
                            {
                                if (this.IsOUT)
                                {
                                    //限定只能取某一个药房
                                    filterDrugDept = " and (类别编码 in ('P','PCZ','PCC') and  执行科室 = '" + objSpecialDrugDept.Name + "')";
                                    //filterUndrug = " or (" + sInput + "and (类别编码 not in ('P','PCZ','PCC')))";
                                }
                                else
                                {
                                    //限定不能取某一个药房
                                    if (frmShowItem.cmbDrugDept.Tag != null && frmShowItem.cmbDrugDept.Tag.ToString() != "ALL")
                                    {
                                        //tangyi 注释原因：需要显示出外配药房药品
                                        //filterDrugDept = " and (类别编码 in ('P','PCZ','PCC') and  执行科室 = '" + frmShowItem.cmbDrugDept.Text + "' and  执行科室 <> '" + objSpecialDrugDept.Name + "')";
                                        filterDrugDept = " and (类别编码 in ('P','PCZ','PCC') and  执行科室 = '" + frmShowItem.cmbDrugDept.Text + "')";
                                        filterUndrug = " or (" + sInput + "and (类别编码 not in ('P','PCZ','PCC')))";
                                    }
                                    else
                                    {
                                        filterDrugDept = " and (类别编码 in ('P','PCZ','PCC') and  执行科室 <> '" + objSpecialDrugDept.Name + "')";
                                        filterUndrug = " or (" + sInput + "and (类别编码 not in ('P','PCZ','PCC')))";
                                    }
                                }
                            }
                            else
                            {
                                //不可以取外配药房的药品
                                if (this.IsOUT)
                                {
                                    //限定只能取某一个药房
                                    filterDrugDept = " and (类别编码 in ('P','PCZ','PCC') and  执行科室 = '')";
                                    //filterUndrug = " or (" + sInput + "and (类别编码 not in ('P','PCZ','PCC')))";
                                }
                            }
                        }

                        #endregion

                    }

                    sInput = "(" + sInput + filterDrugDept + ")" + filterUndrug;
                }

                #endregion

                #region 科常用项目+合作医疗用药

                sInput = string.Format(sInput, queryCode);
                string Protecteddrug = this.isCompanyRang ? "and ( 医保标记  like  '%X%')" : " ";
                sInput = sInput + sCategory + Protecteddrug;
                if (isDeptUsedFlag && !string.IsNullOrEmpty(destItemId.Trim()))
                {
                    sInput = "( 编码 in  ( " + destItemId + " ))  and  (" + sInput + ")";
                }
                #endregion

                #region 显示停用缺药标记

                this.FreshItemFlag();

                #endregion
                //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
                this.RefreshFP();
                //if (this.IsListShowAlways)
                //{
                fpItemList.Sheets[0].DataSource = MyDataView;
                //}


                MyDataView.RowFilter = sInput;
                dvDeptUsed.RowFilter = sInput;

                fpItemList.Sheets[0].ActiveRowIndex = 0;

                fpItemList.Sheets[0].ClearSelection();
                if (fpItemList.Sheets[0].RowCount > 0)
                {
                    fpItemList.Sheets[0].AddSelection(0, 0, 1, 1);
                }
                fpItemList_SelectionChanged(fpItemList, null);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ChangeItem" + ex.Message);
            }
        }

        private System.Threading.Timer autoRefreshTimer = null;
        private System.Threading.TimerCallback autoRefreshCallBack = null;
        private delegate void autoRefreshHandler();
        private autoRefreshHandler autoRefreshEven;

        /// <summary>
        /// 刷新的个数
        /// </summary>
        int freshRowCount = 0;

        /// <summary>
        /// 刷新处方列表
        /// </summary>
        /// <param name="param">参数（没有使用）</param>
        /// <returns></returns>
        private void AutoRefreshTimerCallback(object param)
        {
            //lock (this)
            //{

            return;

            if (this.autoRefreshEven == null)
            {
                autoRefreshEven = new autoRefreshHandler(this.DoFreshItemFlag);
            }

            if (this.frmShowItem.Visible)
            {
                if (!this.IsDisposed)
                {
                    this.Invoke(this.autoRefreshEven);
                }
            }
        }

        private void DoFreshItemFlag()
        {
            //lock (isFreshFinish)
            //{

            if (isUserThread != 2)
            {
                return;
            }

            try
            {
                Font font_Bold = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Bold);
                Font font_Regular = new Font(this.fpItemList.Font.FontFamily, this.fpItemList.Font.Size, FontStyle.Regular);

                for (int i = freshRowCount * 10; i < (freshRowCount + 1) * 10; i++)
                {
                    if (i >= fpItemList.Sheets[0].RowCount)
                    {
                        return;
                    }

                    if (fpItemList.Sheets[0].Cells[i, (int)EnumMainColumnSet.LackFlag].Text == "是")
                    {
                        //this.fpItemList.Sheets[0].Rows[i].Label = "停";
                        this.fpItemList.Sheets[0].Rows[i].BackColor = Color.LightCoral;
                    }
                    else
                    {
                        this.fpItemList.Sheets[0].Rows[i].BackColor = Color.Transparent;
                    }

                    #region 显示医保等级信息

                    string itemCode = fpItemList.Sheets[0].Cells[i, (int)Neusoft.HISFC.Components.Common.Controls.EnumMainColumnSet.ItemCode].Text.Trim();
                    if (itemCode != "999")
                    {
                        Neusoft.HISFC.Models.Base.Item item = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                        if (item == null)
                        {
                            item = SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                        }

                        if (item != null
                            && patient != null
                            && patient.Pact != null)
                        {
                            string strCompareInfo = string.Empty;
                            if (Classes.Function.IItemCompareInfo.GetCompareItemInfo(item, patient.Pact, ref compareItem, ref strCompareInfo) == -1)
                            {
                                this.fpItemList.Sheets[0].Rows[i].Label = i.ToString();
                                this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Regular;
                                this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = this.fpItemList.Sheets[0].ColumnHeader.Columns[0].ForeColor;
                            }
                            else
                            {
                                //医保标记
                                switch (compareItem.CenterItem.ItemGrade)
                                {
                                    case "1":
                                        this.fpItemList.Sheets[0].Rows[i].Label = "甲";
                                        this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Bold;
                                        this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = Color.Red;
                                        break;
                                    case "2":
                                        this.fpItemList.Sheets[0].Rows[i].Label = "乙";
                                        this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Bold;
                                        this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = Color.Red;
                                        break;
                                    default:
                                        this.fpItemList.Sheets[0].Rows[i].Label = i.ToString();
                                        this.fpItemList.Sheets[0].RowHeader.Rows[i].Font = font_Regular;
                                        this.fpItemList.Sheets[0].RowHeader.Rows[i].ForeColor = this.fpItemList.Sheets[0].ColumnHeader.Columns[0].ForeColor;
                                        break;
                                }
                            }
                        }
                    }
                    #endregion
                }
                this.freshRowCount += 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("DoFreshItemFlag" + ex.Message);
            }
            //}
        }

        /// <summary>
        /// 门诊医生站刷新列表标记（社保标记）的方式：0 不刷新；1 使用多线程；2 单线程过滤刷新；3 初始化加载（根据合同单位调整社保标记列）
        /// </summary>
        private static int isUserThread = -1;

        /// <summary>
        /// 刷新标记(停用、医保)
        /// </summary>
        private void FreshItemFlag()
        {
            if (isUserThread == 0)
            {
                return;
            }

            if (!this.frmShowItem.Visible)
            {
                return;
            }

            this.freshRowCount = 0;
            this.isFreshFinish = false;

            if (isUserThread == 1)
            {
                if (this.autoRefreshTimer != null)
                {
                    this.autoRefreshEven = null;
                    this.autoRefreshTimer.Dispose();
                }

                if (this.autoRefreshCallBack == null)
                {
                    this.autoRefreshCallBack = new System.Threading.TimerCallback(this.AutoRefreshTimerCallback);
                }
                this.autoRefreshTimer = new System.Threading.Timer(this.autoRefreshCallBack, null, 0, 500);
            }
            else if (isUserThread == 2)
            {
                DoFreshItemFlag();
            }
        }

        /// <summary>
        /// 输入法类型 0 拼音 1 五笔 2 自定义码
        /// </summary>
        public int InputType
        {
            get
            {
                return this.intQueryType;
            }
            set
            {
                this.intQueryType = value;
                ChangeQueryType();
            }
        }

        /// <summary>
        /// 更改过滤类别
        /// </summary>
        protected virtual void ChangeQueryType()
        {

            if (intQueryType > 3) intQueryType = 0;
            switch (intQueryType)
            {
                case 0:
                    QueryType = "拼音、五笔、自定义码";
                    this.txtItemCode.BackColor = Color.FromArgb(255, 255, 255);
                    break;
                case 1:
                    QueryType = "拼音码";
                    this.txtItemCode.BackColor = Color.FromArgb(255, 225, 225);
                    break;
                case 2:
                    this.txtItemCode.BackColor = Color.FromArgb(255, 200, 200);
                    QueryType = "五笔码";
                    break;
                case 3:
                    this.txtItemCode.BackColor = Color.FromArgb(255, 150, 150);
                    QueryType = "自定义码";
                    break;
                default:
                    this.txtItemCode.BackColor = Color.FromArgb(255, 255, 255);
                    QueryType = "拼音、五笔、自定义码";
                    break;
            }
            this.toolTip1.SetToolTip(this.txtItemCode, "当前输入法为：" + this.QueryType + "\nF2切换输入法。");

            this.toolTip1.InitialDelay = 0;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.AutomaticDelay = 100;
            this.toolTip1.Active = true;

            frmShowItem.TipText = "当前输入法为：" + this.QueryType;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected void myShowList()
        {
            //{E68EC2D3-2E6B-4062-A194-9E3C88B1AA98}
            if (isFristShow)
            {
                this.isFristShow = false;
            }

            //显示列表
            if (this.bIsListShowAlways == false)
            {
                if (!frmShowItem.Visible)
                {
                    //是否显示科室常用项目选项
                    //if (isDeptUsedFlag)
                    //{
                    //    this.frmShowItem.showDeptUsedCheckBox();
                    //}
                    //else
                    //{
                    //    this.frmShowItem.hideDeptUsedCheckBox();
                    //}
                    //修改位置
                    //frmItemList.Location = this.txtItemCode.PointToScreen(new Point(0, this.Bottom+50));
                    //frmItemList.Size = new Size(580, 400);

                    Point temPoint = this.txtItemCode.PointToScreen(new Point(0, this.Bottom + 50));
                    frmShowItem.Location = new Point(temPoint.X, temPoint.Y);
                    frmShowItem.Size = new Size(705, 408);
                    frmShowItem.Show();
                    frmShowItem.TopMost = true;

                }
            }
        }

        /// <summary>
        /// 显示科常用项目
        /// </summary>
        /// <param name="flag"></param>
        void frmItemList_isDeptUsedItem(bool flag)
        {
            if (flag)
            {
                this.isDeptUsedFlag = true;
                this.ChangeItem();
            }
            else
            {
                this.isDeptUsedFlag = false;
                this.ChangeItem();
            }
        }

        /// <summary>
        /// 是否合作医疗范围用药
        /// </summary>
        /// <param name="flag"></param>
        void frmItemList_companyRang(bool flag)
        {
            this.isCompanyRang = flag;
            this.ChangeItem();
        }

        /// <summary>
        /// 切换输入法{E68EC2D3-2E6B-4062-A194-9E3C88B1AA98}
        /// </summary>
        /// <param name="i"></param>
        void frmItemList_writeWay(int i)
        {
            this.intQueryType = i;
            ChangeQueryType();
        }

        /// <summary>
        /// 获得项目
        /// </summary>
        protected void mySelectedItem()
        {
            //TODO:选出项目
            try
            {
                if (this.bIsListShowAlways == false)
                {
                    if (this.frmShowItem != null)
                    {
                        this.frmShowItem.Hide();
                    }
                }

                int columnIndex = 0;
                for (int j = 0; j < this.fpItemList.Sheets[0].ColumnCount; j++)
                {
                    if (this.fpItemList.Sheets[0].ColumnHeader.Columns[j].Label == "执行科室")
                    {
                        columnIndex = j;
                        break;
                    }
                }

                Neusoft.HISFC.Models.Base.Item item = null;
                Neusoft.HISFC.Models.Fee.ComGroup group = null;

                for (int i = 0; i < alItem.Count; i++)
                {
                    item = alItem[i] as Neusoft.HISFC.Models.Base.Item;
                    if (item == null)
                    {
                        if (alItem[i].GetType() == typeof(Neusoft.HISFC.Models.Fee.ComGroup))
                        {
                            group = alItem[i] as Neusoft.HISFC.Models.Fee.ComGroup;
                            if (group == null)
                            {
                                continue;
                            }
                            string ItemID = this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 0].Text;//编码
                            if (group.ID == ItemID)
                            {
                                item = new Neusoft.HISFC.Models.Base.Item();
                                item.ID = group.ID;
                                item.Name = group.Name;
                                item.PriceUnit = "[组套]";
                                this.txtItemName.Text = group.Name;

                                txtItemCode.TextChanged -= new EventHandler(txtItemCode_TextChanged);
                                this.txtItemCode.Text = string.Empty;
                                txtItemCode.TextChanged += new EventHandler(txtItemCode_TextChanged);
                                //frmShowItem.Hide();

                                this.myFeeItem = item;

                                #region 医保适应症提示
                                if (compareItem != null && !string.IsNullOrEmpty(compareItem.Practicablesymptomdepiction.Trim()))
                                {
                                    MessageBox.Show(Classes.Function.IItemCompareInfo.ErrInfo, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (compareItem != null && !string.IsNullOrEmpty(compareItem.Practicablesymptomdepiction.Trim()))
                                {
                                    MessageBox.Show("此项目针对特定患者使用：\n" + compareItem.Practicablesymptomdepiction.Trim(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                #endregion

                                if (SelectedItem != null)
                                {
                                    SelectedItem(item);
                                }

                                return;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    if (item == null)
                    {
                        continue;
                    }

                    if (item.GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))//判断是药品
                    {
                        //item.IsPharmacy = true;
                        item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.Drug;
                    }
                    else if (item.GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))//非药品
                    {
                        //item.IsPharmacy = false;
                        item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                    }
                    else if (item.GetType() == typeof(Neusoft.HISFC.Models.FeeStuff.MaterialItem))
                    {
                        item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.MatItem;
                    }
                    else
                    {
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("非项目类型！") + item.GetType().ToString());
                        return;
                    }
                    this.myFeeItem = item;

                    if (item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)//药品选择
                    {
                        string ItemID = this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 0].Text;//编码
                        string Dept = this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, columnIndex].Text;//发药科室

                        if (this.myFeeItem.ID == ItemID)//&& this.myFeeItem.User03 == Dept)//发药科室相同
                        {
                            if (this.myFeeItem.User03 == Dept)
                            {
                                this.txtItemName.Text = this.myFeeItem.Name;

                                txtItemCode.TextChanged -= new EventHandler(txtItemCode_TextChanged);
                                this.txtItemCode.Text = string.Empty;
                                txtItemCode.TextChanged += new EventHandler(txtItemCode_TextChanged);
                                //frmShowItem.Hide();


                                //#region 医保适应症提示
                                //if (Classes.Function.IItemExtendInfo.GetCompareItemInfo(this.myFeeItem.ID, ref compareItem) == -1)
                                //{
                                //    MessageBox.Show(Classes.Function.IItemExtendInfo.ErrInfo, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //    return;
                                //}
                                //if (compareItem != null && !string.IsNullOrEmpty(compareItem.Practicablesymptomdepiction.Trim()))
                                //{
                                //    MessageBox.Show("此项目针对特定患者使用：\n" + compareItem.Practicablesymptomdepiction.Trim(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //}
                                //#endregion

                                if (SelectedItem != null)
                                    SelectedItem(this.FeeItem);
                                return;
                            }
                        }

                    }
                    //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl
                    else if (item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.MatItem)//物资选择
                    {
                        if (this.myFeeItem.ID == this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 0].Text
                            && item.Price == FrameWork.Function.NConvert.ToDecimal(this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 4].Text)) //编码相同
                        {
                            this.txtItemName.Text = this.myFeeItem.Name;

                            txtItemCode.TextChanged -= new EventHandler(txtItemCode_TextChanged);
                            this.txtItemCode.Text = string.Empty;
                            txtItemCode.TextChanged += new EventHandler(txtItemCode_TextChanged);
                            //frmShowItem.Hide();


                            //#region 医保适应症提示
                            //if (Classes.Function.IItemExtendInfo.GetCompareItemInfo(this.myFeeItem.ID, ref compareItem) == -1)
                            //{
                            //    MessageBox.Show(Classes.Function.IItemExtendInfo.ErrInfo, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //    return;
                            //}
                            //if (compareItem != null && !string.IsNullOrEmpty(compareItem.Practicablesymptomdepiction.Trim()))
                            //{
                            //    MessageBox.Show("此项目针对特定患者使用：\n" + compareItem.Practicablesymptomdepiction.Trim(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //}
                            //#endregion

                            if (SelectedItem != null)
                                SelectedItem(this.FeeItem);
                            return;
                        }
                    }
                    else//非药品选择
                    {
                        if (this.myFeeItem.ID == this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 0].Text) //编码相同
                        {
                            this.txtItemName.Text = this.myFeeItem.Name;
                            txtItemCode.TextChanged -= new EventHandler(txtItemCode_TextChanged);
                            this.txtItemCode.Text = string.Empty;
                            txtItemCode.TextChanged += new EventHandler(txtItemCode_TextChanged);
                            //frmShowItem.Hide();

                            //#region 医保适应症提示
                            //if (Classes.Function.IItemExtendInfo.GetCompareItemInfo(this.myFeeItem.ID, ref compareItem) == -1)
                            //{
                            //    MessageBox.Show(Classes.Function.IItemExtendInfo.ErrInfo, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //    return;
                            //}
                            //if (compareItem != null && !string.IsNullOrEmpty(compareItem.Practicablesymptomdepiction.Trim()))
                            //{
                            //    MessageBox.Show("此项目针对特定患者使用：\n" + compareItem.Practicablesymptomdepiction.Trim(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //}
                            //#endregion

                            if (SelectedItem != null)
                                SelectedItem(this.FeeItem);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("mySelectedItem" + ex.Message);
            }

            MessageBox.Show("error 没有找到 " + this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 0].Text
                + "1" + this.fpItemList.Sheets[0].Cells[this.fpItemList.Sheets[0].ActiveRowIndex, 1].Text);
        }

        /// <summary>
        /// 变化成组套
        /// </summary>
        /// <param name="isDept"></param>
        public void ChangeDataSet(bool isDept)
        {
            if (isDept)
            {
                fpItemList.Sheets[0].DataSource = dvDeptUsed;
                frmShowItem.DataView = dvDeptUsed;
            }
            else
            {
                fpItemList.Sheets[0].DataSource = MyDataView;
                frmShowItem.DataView = MyDataView;
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e != null)
            {
                this.mySelectedItem();
            }
        }

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtItemCode.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                frmShowItem.Hide();
                this.txtItemCode.Focus();
            }
            else
            {
                if (e.KeyCode.ToString().Length <= 1)
                {
                    txtItemCode.TextChanged -= new EventHandler(txtItemCode_TextChanged);
                    this.txtItemCode.Text = this.txtItemCode.Text + e.KeyCode.ToString();
                    txtItemCode.TextChanged += new EventHandler(txtItemCode_TextChanged);
                }
                this.txtItemCode.Focus();
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void frmItemList_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            frmShowItem.Hide();
            this.txtItemName.Focus();
        }

        /// <summary>
        /// 设置项目列表是否可见
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisibleForms(bool visible)
        {
            this.frmShowItem.Visible = visible;
        }

        /// <summary>
        /// 类别选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void cbCategory_SelectedIndexChanged(object sender, System.EventArgs e)
        {
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            try
            {
                if (this.frmShowItem.Visible)
                {
                    if (keyData == Keys.Up)
                    {
                        return true;
                    }
                    else if (keyData == Keys.Down)
                    {
                        return true;
                    }
                    else
                    {
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
                }
            }
            catch { }

            return base.ProcessCmdKey(ref msg, keyData);
            //    return true;
        }

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void txtItemCode_KeyUp(object sender, KeyEventArgs e)
        {
            //return;
            try
            {
                if (fpItemList.Sheets[0].ActiveRowIndex > 9)
                {
                    fpItemList.SetViewportTopRow(0, fpItemList.Sheets[0].ActiveRowIndex - 9);
                }
                if (e.KeyCode == Keys.Up)
                {
                    fpItemList.Sheets[0].ActiveRowIndex--;
                    fpItemList.Sheets[0].AddSelection(fpItemList.Sheets[0].ActiveRowIndex, 0, 1, 1);
                    fpItemList.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    fpItemList.Sheets[0].ActiveRowIndex++;
                    fpItemList.Sheets[0].AddSelection(fpItemList.Sheets[0].ActiveRowIndex, 0, 1, 1);
                    fpItemList_SelectionChanged(fpItemList, null);
                    fpItemList.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (fpItemList.Sheets[0].Rows.Count > 0 && fpItemList.Sheets[0].ActiveRowIndex >= 0 && this.fpItemList.Visible)
                    {
                        mySelectedItem();
                    }
                }
                else if (e.KeyCode == Keys.F3)//显示选择项目窗口
                {
                    if (this.bIsListShowAlways == false)
                    {
                        if (this.frmShowItem != null) this.frmShowItem.Hide();
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    frmShowItem.Hide();
                }
                //变换输入法
                else if (e.KeyCode == Keys.F2)
                {
                    intQueryType++;
                    try
                    {
                        ChangeQueryType();//raiseevent 变换输入法

                        if (this.FindForm().Visible) System.Windows.Forms.Cursor.Position = this.txtItemCode.PointToScreen(new Point(this.panel2.Left + this.txtItemCode.Width - 2, this.panel2.Top));
                    }
                    catch { }
                }
            }
            catch { }
        }

        bool bCanChange = true;
        /// <summary>
        /// 文本变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void txtItemCode_TextChanged(object sender, System.EventArgs e)
        {
            if (bCanChange == false)
            {
                return;
            }
            if (this.txtItemCode.Text.StartsWith("@"))
            {
                this.frmShowItem.Visible = false;
                return;
            }

            this.ChangeItem();
            this.txtItemCode.SelectionStart = this.txtItemCode.Text.Length;
            this.txtItemCode.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void cmbCategory_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtItemCode.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void cmbCategory_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtItemCode.Focus();
            try
            {
                if (frmShowItem != null && this.frmShowItem.Visible)
                {
                    this.ChangeItem();
                }
                if (this.IsListShowAlways)
                {
                    this.ChangeItem();
                }

                if (this.CatagoryChanged != null)
                {
                    this.CatagoryChanged(this.cmbCategory.alItems[this.cmbCategory.SelectedIndex] as Neusoft.FrameWork.Models.NeuObject);
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void txtItemName_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (this.txtItemName.Text.Trim() == string.Empty)
                {
                    return;
                }
                Neusoft.HISFC.Models.Base.Item item;
                if (this.cmbCategory.Tag.ToString().Substring(0, 1) == "P")
                {
                    Neusoft.HISFC.Models.Pharmacy.Item obj = new Neusoft.HISFC.Models.Pharmacy.Item();
                    item = obj;
                }
                else
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug obj = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                    item = obj;
                    obj.Qty = 1.0M;
                    obj.PriceUnit = "个";
                }

                if (this.cmbCategory.Text == "全部" || cmbCategory.Tag.ToString() == "ALL")
                {
                    MessageBox.Show("请选择项目类别！");
                    return;
                }

                item.ID = "999";//自定义
                item.SysClass.ID = this.cmbCategory.Tag.ToString();

                //if (item.IsPharmacy)
                if (item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                {
                    try
                    {
                        ((Neusoft.HISFC.Models.Pharmacy.Item)item).Type.ID = item.SysClass.ID.ToString().Substring(item.SysClass.ID.ToString().Length - 1, 1);

                    }
                    catch { }
                }
                if (this.bIsShowSelfMark && this.cmbCategory.Tag.ToString().Substring(0, 1) == "P")//有自备药字样
                {
                    if (this.txtItemName.Text.TrimEnd().Length > 4)
                    {
                        if (this.txtItemName.Text.TrimEnd().Substring(this.txtItemName.Text.TrimEnd().Length - SelfMark.Length) == SelfMark)
                        {
                            item.Name = this.txtItemName.Text;//有自备药字样
                            try
                            {
                                this.myFeeItem = item;
                                if (SelectedItem != null)
                                    SelectedItem(item);
                            }
                            catch { }
                            return;
                        }
                    }

                    item.Name = this.txtItemName.Text + SelfMark;
                }
                else
                {
                    item.Name = this.txtItemName.Text;//无自备药字样
                }
                try
                {
                    this.myFeeItem = item;
                    if (SelectedItem != null) SelectedItem(item);
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void txtItemCode_Enter(object sender, EventArgs e)
        {
            this.txtItemCode.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void txtItemCode_Leave(object sender, EventArgs e)
        {
            if (frmShowItem != null && frmShowItem.Visible)
            {
                frmShowItem.Hide();
            }
        }
        #endregion

        /// <summary>
        /// 初始化明细信息显示
        /// </summary>
        /// {46983F5B-E184-4b8b-B819-AA1C34993F1B} 修改为protected
        protected void initFPdetail()
        {
            if (this.fpItemDetal.Sheets.Count <= 0)
            {
                this.fpItemDetal.Sheets.Add(new FarPoint.Win.Spread.SheetView());
            }

            fpItemDetal.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            fpItemDetal.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Never;

            if (this.bIsListShowAlways == false)
            {
                frmShowItem.AddBottomControl(fpItemDetal);
                frmShowItem.AddBottomControl(txtNote);
            }
            else
            {
                fpItemDetal.Dock = System.Windows.Forms.DockStyle.Fill;
                //待修改
            }

            fpItemDetal.Size = new Size(0, 0);
            fpItemDetal.Show();
            fpItemDetal.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

            //修改门诊医生复合项目提示内容{8CA036D8-1BA8-4031-A71A-9591EA8B0ACA}
            fpItemDetal.Sheets[0].DataAutoCellTypes = true;
            fpItemDetal.Sheets[0].DataAutoSizeColumns = false;
            fpItemDetal.Sheets[0].ColumnHeaderVisible = true;
            fpItemDetal.Sheets[0].Columns.Count = 8;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[0].Label = "项目编码";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[0].Width = 0;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[1].Label = "编码";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[1].Width = 100;

            fpItemDetal.Sheets[0].ColumnHeader.Columns[2].Label = "名称";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[2].Width = 220;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[3].Label = "价格";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[3].Width = 40;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[4].Label = "数量";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[4].Width = 40;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[5].Label = "医保（公医）标记";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[5].Width = 300;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[6].Label = "有效";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[6].Width = 40;
            fpItemDetal.Sheets[0].ColumnHeader.Columns[7].Label = "适应症信息";
            fpItemDetal.Sheets[0].ColumnHeader.Columns[7].Width = 200;
            fpItemDetal.Sheets[0].Columns[6].CellType = new FarPoint.Win.Spread.CellType.CheckBoxCellType();

            #region 注意事项

            lblMemo.Text = string.Empty;
            lblMemo.Dock = System.Windows.Forms.DockStyle.Fill;
            PanelItemMemo.Controls.Add(lblMemo);
            //frmShowItem.AddBottomMemoControl(PanelItemMemo);

            #endregion
        }

        Neusoft.HISFC.BizLogic.Pharmacy.Item phaManger = new Neusoft.HISFC.BizLogic.Pharmacy.Item();
        Neusoft.HISFC.BizProcess.Integrate.Fee itemMgr = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 存放医保项目列表
        /// </summary>
        private static Hashtable hsSIItemList = new Hashtable();

        /// <summary>
        /// 选择项目变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpItemList_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            //选择变化，刷新收费项目列表显示
            if (this.IsListShowAlways == true)
            {
                return; //全部显示时候另外处理
            }

            if (fpItemList.Sheets[0].RowCount <= 0)
            {
                this.fpItemDetal.Sheets[0].Rows.Count = 0;
                return;
            }

            //显示收费项目信息
            this.fpItemDetal.Sheets[0].Rows.Count = 0;
            this.fpItemDetal.Sheets[0].SheetCornerStyle.BackColor = Color.YellowGreen;

            //string itmExtendInfo = "";
            ArrayList alExtendInfo = new ArrayList();

            string itemid = fpItemList.Sheets[0].Cells[fpItemList.Sheets[0].ActiveRowIndex, 0].Text;

            Neusoft.HISFC.Models.Pharmacy.Item itm = null;
            Neusoft.HISFC.Models.Fee.Item.Undrug itemtmp = null;
            itm = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemid);
            if (itm == null)
            {
                itemtmp = SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemid);
            }
            #region 药品和非药品（非复合项目）

            if ((itm != null && itm.ID != "")
                || itemtmp != null && itemtmp.UnitFlag != "1")
            {
                if (Classes.Function.IItemCompareInfo != null
                    && patient != null && !string.IsNullOrEmpty(patient.ID)
                    && (patient.Pact != null && !string.IsNullOrEmpty(patient.Pact.ID)))
                {
                    Neusoft.HISFC.Models.Base.Item item = null;
                    if (itm != null)
                    {
                        item = itm;
                    }
                    else
                    {
                        item = itemtmp;
                    }

                    Neusoft.HISFC.Models.SIInterface.Compare compare = null;

                    string strCompareInfo = string.Empty;

                    int iRtn = Classes.Function.IItemCompareInfo.GetCompareItemInfo(item, patient.Pact, ref compare, ref strCompareInfo);
                    if (iRtn == -1)
                    {
                        MessageBox.Show("fpItemList_SelectionChanged" + Classes.Function.IItemCompareInfo.ErrInfo);
                        return;
                    }

                    //Classes.Function.IItemExtendInfo.GetCompareItemInfo(item

                    if (string.IsNullOrEmpty(strCompareInfo))
                    {
                        this.fpItemDetal.Sheets[0].RowCount = 0;
                        fpItemDetal.Visible = false;
                    }
                    else
                    {
                        this.fpItemDetal.Sheets[0].RowCount = 5;
                        txtNote.Multiline = true;
                        txtNote.Text = strCompareInfo;
                        txtNote.ReadOnly = true;
                        txtNote.Multiline = true;
                        txtNote.ForeColor = Color.LightCoral;
                        txtNote.Visible = true;
                        txtNote.ScrollBars = ScrollBars.Both;
                        txtNote.Font = new Font("宋体", 13, FontStyle.Bold);
                        fpItemDetal.Visible = false;
                    }
                }
                else
                {
                    this.fpItemDetal.Sheets[0].RowCount = 0;
                    fpItemDetal.Visible = false;
                }
            }
            #endregion
            else
            {
                //修改门诊医生复合项目提示内容{8CA036D8-1BA8-4031-A71A-9591EA8B0ACA}
                lstzt = new List<Neusoft.HISFC.Models.Fee.Item.UndrugComb>();

                //非药品
                if (itemtmp != null && itemtmp.UnitFlag == "1")
                {
                    lstzt = SOC.HISFC.BizProcess.Cache.Fee.GetUndrugZTDetail(itemtmp.ID);

                    if (lstzt == null)
                    {
                        if (this.ztManager.QueryUnDrugztDetail(itemtmp.ID, ref lstzt) == -1)
                        {
                            MessageBox.Show("fpItemList_SelectionChanged" + this.ztManager.Err);
                            return;
                        }
                    }
                    if (lstzt.Count == 0)
                    {
                        return;
                    }

                    if (lstzt == null || lstzt.Count == 0)
                    {
                        this.fpItemDetal.Sheets[0].RowCount = 0;

                        txtNote.Visible = false;
                        this.fpItemDetal.Visible = true;
                        frmShowItem.Memo = this.lblMemo.Text;
                        frmShowItem.ResizeBottom();
                        return;
                    }

                    this.fpItemDetal.Sheets[0].RowCount = lstzt.Count;
                    string undruggrade = null;
                    for (int j = 0; j < lstzt.Count; j++)
                    {
                        Neusoft.HISFC.Models.Fee.Item.UndrugComb obj = lstzt[j];

                        this.fpItemDetal.Sheets[0].Rows[j].ForeColor = this.fpItemDetal.Sheets[0].RowHeader.Rows[j].ForeColor;
                        try
                        {
                            this.fpItemDetal.Sheets[0].Cells[j, 0].Text = ((Neusoft.HISFC.Models.Fee.Item.Undrug)this.itemHelper.GetObjectFromID(obj.ID)).UserCode;
                        }
                        catch
                        {
                            this.fpItemDetal.Sheets[0].Cells[j, 0].Text = obj.ID;
                        }
                        this.fpItemDetal.Sheets[0].Cells[j, 1].Text = obj.UserCode;
                        this.fpItemDetal.Sheets[0].Cells[j, 2].Text = obj.Name;
                        this.fpItemDetal.Sheets[0].Cells[j, 3].Value = obj.Price;
                        this.fpItemDetal.Sheets[0].Cells[j, 4].Value = obj.Qty;

                        if (Classes.Function.IItemCompareInfo != null
                            && patient != null
                            && patient.Pact != null)
                        {
                            string strCompareInfo = string.Empty;
                            Neusoft.HISFC.Models.Base.Item item = SOC.HISFC.BizProcess.Cache.Fee.GetItem(obj.ID);
                            Neusoft.HISFC.Models.SIInterface.Compare compare = null;
                            int iRtn = Classes.Function.IItemCompareInfo.GetCompareItemInfo(obj, patient.Pact, ref compare, ref strCompareInfo);
                            if (iRtn == -1)
                            {
                                MessageBox.Show("fpItemList_SelectionChanged" + Classes.Function.IItemCompareInfo.ErrInfo);
                                return;
                            }
                            this.fpItemDetal.Sheets[0].Cells[j, 5].Text = strCompareInfo;


                            /*
                            int iRtn = Classes.Function.IItemExtendInfo.GetCompareItemInfo(obj.ID, ref compareItem);
                            if (iRtn == -1)
                            {
                                MessageBox.Show(Classes.Function.IItemExtendInfo.ErrInfo);
                                return;
                            }
                            else if (compareItem == null)
                            {
                                //医保标记
                                this.fpItemDetal.Sheets[0].Cells[j, 5].Text = "非报销";
                            }
                            else
                            {
                                this.fpItemDetal.Sheets[0].Cells[j, 5].Text = Neusoft.HISFC.BizLogic.Fee.Interface.ShowItemGradeByCode(compareItem.CenterItem.ItemGrade);

                                this.fpItemDetal.Sheets[0].Cells[j, 7].Text = compareItem.Practicablesymptomdepiction;
                                this.fpItemDetal.Sheets[0].Rows[j].ForeColor = Color.LightCoral;
                            }
                            */
                        }

                        this.fpItemDetal.Sheets[0].Cells[j, 6].Value = obj.ValidState == "有效" || obj.ValidState == "1" ? true : false;
                        if (this.fpItemDetal.Sheets[0].Cells[j, 6].Value.ToString().ToLower() == "false")
                        {
                            this.fpItemDetal.Sheets[0].Rows[j].BackColor = Color.LightCoral;
                            this.fpItemDetal.Sheets[0].Rows[j].Visible = false;
                        }
                        txtNote.Visible = false;
                        fpItemDetal.Visible = true;
                    }
                }
            }
            //添加注意事项
            lblMemo.Text = string.Empty;
            if (itemtmp != null)
            {
                Neusoft.FrameWork.Models.NeuObject apply = interMgr.GetConstansObj("ApplyBillClass", itemtmp.CheckApplyDept);
                if (!string.IsNullOrEmpty(apply.Memo))
                {
                    lblMemo.Text = "[注意事项]" + apply.Memo;
                    this.PanelItemMemo.Size = new Size(700, 30);
                }
                else
                {
                    this.PanelItemMemo.Size = new Size(700, 0);
                }
            }
            frmShowItem.Memo = this.lblMemo.Text;
            frmShowItem.ResizeBottom();
        }

        #region IInterfaceContainer 成员
        //增加接口容器{112B7DB5-0462-4432-AD9D-17A7912FFDBE} 
        public Type[] InterfaceTypes
        {
            get
            {
                Type[] t = new Type[2];
                t[0] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IGetSiItemGrade);
                t[1] = typeof(Neusoft.HISFC.BizProcess.Interface.Common.IItemCompareInfo);
                return t;
            }
        }

        #endregion

        /// <summary>
        /// 最小费用帮助类
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper minFeeHelper = null;

        private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (!txtItemCode.Text.StartsWith("@"))
                    {
                        return;
                    }
                    if (this.txtItemCode.Text.TrimStart('@') == string.Empty)
                    {
                        return;
                    }

                    Neusoft.HISFC.Models.Base.Item item;

                    if (txtItemCode.Text.StartsWith("@"))
                    {
                        if (this.cmbCategory.Text == "全部")
                        {
                            this.frmShowItem.Visible = false;
                            MessageBox.Show("请选择项目类别！");
                            return;
                        }


                    }

                    if (this.cmbCategory.Tag.ToString().Substring(0, 1) == "P")
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item obj = new Neusoft.HISFC.Models.Pharmacy.Item();
                        item = obj;
                    }
                    else
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug obj = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        item = obj;
                        obj.Qty = 1.0M;
                        obj.PriceUnit = "个";
                    }

                    if (txtItemCode.Text.StartsWith("@"))
                    {
                        item.SysClass.ID = this.cmbCategory.Tag.ToString();
                        item.ID = "999";
                    }

                    txtItemCode.TextChanged -= this.txtItemCode_TextChanged;
                    this.txtItemCode.Text = txtItemCode.Text.Replace("@", "");
                    txtItemCode.TextChanged += this.txtItemCode_TextChanged;

                    if (item.ID == "999")
                    {
                        //门诊的自备药需要插入费用表 用于门诊输液
                        //所以最小费用随机取一个
                        if (minFeeHelper == null)
                        {
                            minFeeHelper = new Neusoft.FrameWork.Public.ObjectHelper();
                            Neusoft.HISFC.BizProcess.Integrate.Manager inteMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                            minFeeHelper.ArrayObject = inteMgr.GetConstantList("MINFEE");
                        }

                        if (minFeeHelper.ArrayObject != null && minFeeHelper.ArrayObject.Count > 0)
                        {
                            item.MinFee = (Neusoft.FrameWork.Models.NeuObject)minFeeHelper.ArrayObject[0];
                        }
                        else
                        {
                            item.MinFee.ID = "001";
                            item.MinFee.Name = "西药费";
                        }
                    }

                    if (item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        ((Neusoft.HISFC.Models.Pharmacy.Item)item).Type.ID = item.SysClass.ID.ToString().Substring(item.SysClass.ID.ToString().Length - 1, 1);
                    }

                    if (item.ID == "999")
                    {
                        if (this.bIsShowSelfMark
                            && this.cmbCategory.Tag.ToString().Substring(0, 1) == "P")//有自备药字样
                        {
                            item.Name = this.txtItemCode.Text + SelfMark;//有自备药字样
                        }
                        else
                        {
                            item.Name = this.txtItemCode.Text + "[嘱托]";//无自备药字样
                        }
                    }
                    else
                    {
                        item.Name = this.txtItemName.Text;
                    }

                    txtItemCode.TextChanged -= this.txtItemCode_TextChanged;
                    this.txtItemCode.Text = "";
                    txtItemCode.TextChanged += this.txtItemCode_TextChanged;
                    if (this.frmShowItem.Visible)
                    {
                        this.frmShowItem.Visible = false;
                    }

                    this.myFeeItem = item;
                    if (SelectedItem != null)
                    {
                        SelectedItem(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("txtItemCode_KeyDown" + ex.Message);
                }
            }
        }

        private void cmbCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtItemCode.Focus();
            }
        }

        private void chkoutflag_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkoutflag.CheckState == CheckState.Checked)
            {
                frmShowItem.IsOUT = true;
            }
            if (this.chkoutflag.CheckState == CheckState.Unchecked)
            {
                frmShowItem.IsOUT = false;
            }
        }
    }



    /// <summary>
    /// 项目列表列设置
    /// </summary>
    public enum EnumMainColumnSet
    {
        /// <summary>
        /// 编码
        /// </summary>
        ItemCode,

        /// <summary>
        /// 名称
        /// </summary>
        ItemName,

        // {E5BE0123-FD83-46a1-9455-16D9E7F0918A}执行科室放到名称后面
        /// <summary>
        /// 执行科室
        /// </summary>
        //ExecDept,

        /// <summary>
        /// 系统类别名称
        /// </summary>
        SysClassName,

        /// <summary>
        /// 规格
        /// </summary>
        Specs,

        /// <summary>
        /// 价格
        /// </summary>
        Price,

        /// <summary>
        /// 单位
        /// </summary>
        Unit,

        /// <summary>
        /// 社保标记
        /// </summary>
        SiFlag,

        /// <summary>
        /// 生产厂家
        /// </summary>
        Product,

        /// <summary>
        /// 系统类别编码
        /// </summary>
        SysClassCode,

        /// <summary>
        /// 拼音码
        /// </summary>
        SpellCode,

        /// <summary>
        /// 五笔码
        /// </summary>
        WBCode,

        /// <summary>
        /// 自定义码
        /// </summary>
        UserCode,

        /// <summary>
        /// 通用名
        /// </summary>
        RegularName,

        /// <summary>
        /// 通用名拼音码
        /// </summary>
        RegularNameSpellCode,

        /// <summary>
        /// 通用名五笔码
        /// </summary>
        RegularNameWBCode,

        /// <summary>
        /// 通用名自定义码
        /// </summary>
        RegularNameUserCode,

        /// <summary>
        /// 英文名称
        /// </summary>
        EnglishName,

        /// <summary>
        /// 剩余库存数量
        /// </summary>
        StorageQty,

        /// <summary>
        /// 执行科室
        /// </summary>
        //ExecDept,

        /// <summary>
        /// 检查部位
        /// </summary>
        CheckBody,

        /// <summary>
        /// 疾病分类
        /// </summary>
        DiseaseType,

        /// <summary>
        /// 专科名称
        /// </summary>
        SpecialDept,

        /// <summary>
        /// 病史及检查
        /// </summary>
        MedicalRecord,

        /// <summary>
        /// 检查要求
        /// </summary>
        CheckRequest,

        /// <summary>
        /// 注意事项
        /// </summary>
        Notice,

        /// <summary>
        /// 缺药、停用标记
        /// </summary>
        LackFlag,

        ///外延药品
        OutFlag
    }

    /// <summary>
    /// 显示类别
    /// </summary>
    public enum EnumCategoryType
    {
        /// <summary>
        /// 项目类别
        /// </summary>
        ItemType = 0,

        /// <summary>
        /// 系统类别
        /// </summary>
        SysClass = 2
    }

    /// <summary>
    /// 加载项目类别
    /// </summary>
    public enum EnumShowItemType
    {
        /// <summary>
        /// 药品
        /// </summary>
        Pharmacy,

        /// <summary>
        /// 非药品
        /// </summary>
        Undrug,

        /// <summary>
        /// 全部
        /// </summary>
        All,

        /// <summary>
        /// 科常用
        /// </summary>
        DeptItem,

        /// <summary>
        /// 外派药品
        /// </summary>
        OutPharmacy,

        // {4F4F0803-78F5-4c7f-9A0A-65474148B9E0} 护士站只收材料费
        /// <summary>
        /// 材料
        /// </summary>
        Material
    }

    /// <summary>
    /// 非药品分类:门诊、住院、全部
    /// </summary>
    public enum EnumUndrugApplicabilityarea
    {
        /// <summary>
        /// 所有
        /// </summary>
        All = 0,

        /// <summary>
        /// 门诊
        /// </summary>
        Clinic = 1,

        /// <summary>
        /// 住院
        /// </summary>
        InHos = 2
    }
}
