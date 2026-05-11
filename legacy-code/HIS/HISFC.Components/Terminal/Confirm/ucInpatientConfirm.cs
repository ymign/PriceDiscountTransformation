using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using FarPoint.Win.Spread;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Components.Common.Controls;
using Neusoft.HISFC.Models.Base;
using Neusoft.FrameWork.Management;
using Neusoft.HISFC.Models.Fee.Inpatient;
using System.Collections.Generic;
using System.Linq;
using HisCallExternalServiceProject.FunctionModule.IntelligentMonitoringModule.Models;
using HisCallExternalServiceProject.FunctionModule.IntelligentMonitoringModule.Forms;
using HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Service;
using HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Model;
namespace Neusoft.HISFC.Components.Terminal.Confirm
{
    public partial class ucInpatientConfirm : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        public ucInpatientConfirm()
        {
            InitializeComponent();
            this.fpExecOrder.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(fpExecOrder_ColumnWidthChanged);

            this.ckCancel.CheckedChanged += new EventHandler(ckComplete_CheckedChanged);
            this.ckNone.CheckedChanged += new EventHandler(ckComplete_CheckedChanged);
            this.ckComplete.CheckedChanged += new EventHandler(ckComplete_CheckedChanged);

            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(ucQueryInpatientNo1_myEvent);

            this.fpExecOrder.ComboSelChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpExecOrder_ComboSelChange);
            this.fpExecOrder.ButtonClicked += new EditorNotifyEventHandler(fpExecOrder_ButtonClicked);

            this.cmbDept.SelectedIndexChanged += new EventHandler(cmbDept_SelectedIndexChanged);
        }

        #region 私有变量
        public bool seeAll = false;
        public bool clearConfirm = false;
        Neusoft.HISFC.Models.Base.MessType messType = Neusoft.HISFC.Models.Base.MessType.N;
        private bool addFirstRow = false;
        Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm terminalMgr = new Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm();
        Neusoft.HISFC.Components.Common.Controls.EnumShowItemType itemType = Neusoft.HISFC.Components.Common.Controls.EnumShowItemType.DeptItem;
        Neusoft.ApplyInterface.HisInterface PACSApplyInterface = null;
        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam cpMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        protected Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
        Neusoft.HISFC.BizProcess.Integrate.Manager mgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        #region {5E5299D8-95A2-4498-B2F1-52D00E4FB11A}
        Neusoft.HISFC.Components.PacsApply.HisInterface PACSApplyInterfaceNew = null;
        /// <summary>
        /// 统计大类业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.FeeCodeStat feeCodeStatManager = new Neusoft.HISFC.BizLogic.Fee.FeeCodeStat();
        #endregion

        /// <summary>
        ///  确认人是否必填 {130607A7-01C7-4bdf-931C-D7CE7AC8111B}
        /// </summary>
        private bool isNeedOper = false;

        private Dictionary<string, int> hsRows = new Dictionary<string, int>();

        private Hashtable hsItemZT = null;

        private Hashtable hsFeeStats = null;

        private Hashtable hsFeeStatsCost = null;

        /// <summary>
        /// 默认勾选的确认状态
        /// </summary>
        private EnumCheckState defaultConfirmState = EnumCheckState.ComfirmComplete;

        /// <summary>
        /// 默认勾选的确认状态
        /// </summary>
        [Category("控件设置"), Description("默认勾选的确认状态：确认收费、取消收费、暂不处理等")]
        public EnumCheckState DefaultConfirmState
        {
            get
            {
                return defaultConfirmState;
            }
            set
            {
                defaultConfirmState = value;
            }
        }

        /// <summary>
        /// 是否使用WebEMR功能
        /// </summary>
        private bool isShowWebEMR = false;

        /// <summary>
        /// 是否使用WebEMR功能
        /// </summary>
        [Category("控件设置"), Description("是否使用WebEMR功能")]
        public bool IsShowWebEMR
        {
            get
            {
                return isShowWebEMR;
            }
            set
            {
                isShowWebEMR = value;
            }
        }

        /// <summary>
        /// WebEMR查询地址，需传入住院流水号
        /// </summary>
        private string webEMRUrl = "http://172.16.61.119:8080/emrweb/Default.aspx?hisInpatientID={0}&Out=0";

        /// <summary>
        /// WebEMR查询地址，需传入住院流水号
        /// </summary>
        [Category("控件设置"), Description("WebEMR查询地址，需传入住院流水号")]
        public string WebEMRUrl
        {
            get
            {
                return webEMRUrl;
            }
            set
            {
                webEMRUrl = value;
            }
        }

        #endregion

        #region  属性

        private bool isUseNewUndrugZT = false;
        /// <summary>
        /// 默认勾选的确认状态
        /// </summary>
        [Category("控件设置"), Description("是否启用新的检查项目收费模式 true启用 false关闭")]
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

        private string drugDeptSet;
        /// <summary> 
        /// 特殊科室取药药房设置  格式:9008$7015&7014#9004$7015&7014&7002#
        /// </summary>
        [Category("控件设置"), Description("特殊科室取药药房设置  格式:药房$科室&科室#药房$科室&科室&科室#")]
        public string DrugDeptSet
        {
            get
            {
                return drugDeptSet;
            }
            set
            {
                drugDeptSet = value;
            }
        }

        /// <summary>
        /// 控制欠费类型
        /// </summary>
        [Category("控件设置"), Description("欠费判断提示类型")]
        public Neusoft.HISFC.Models.Base.MessType MessType
        {
            get
            {
                return messType;
            }
            set
            {
                messType = value;
            }
        }
        /// <summary>
        /// 显示的项目类别
        /// </summary>
        [Category("控件设置"), Description("显示的项目类别")]
        public Neusoft.HISFC.Components.Common.Controls.EnumShowItemType ItemType
        {
            get
            {
                return itemType;
            }
            set
            {
                itemType = value;
            }
        }
        /// <summary>
        /// 查看所有科室终端确认项目
        /// </summary>
        [Category("控件设置"), Description("查看所有科室终端确认项目")]
        public bool SeeAll
        {
            get
            {
                return seeAll;
            }
            set
            {
                seeAll = value;
            }
        }
        /// <summary>
        /// 点击保存后是否删除已确认项目
        /// </summary>
        [Category("控件设置"), Description("点击保存后是否删除已确认项目")]
        public bool ClearConfirm
        {
            get
            {
                return clearConfirm;
            }
            set
            {
                clearConfirm = value;
            }
        }

        /// <summary>
        /// 新增行是否在第一行
        /// </summary>
        [Category("控件设置"), Description("新增行是否在第一行")]
        public bool AddFirstRow
        {
            get
            {
                return addFirstRow;
            }
            set
            {
                addFirstRow = value;
            }
        }

        /// <summary>
        /// 确认人是否必填 {130607A7-01C7-4bdf-931C-D7CE7AC8111B}
        /// </summary>
        [Category("控件设置"), Description("确认人是否必填 true 是;false 否")]
        public bool IsNeedOper
        {
            get
            {
                return isNeedOper;
            }
            set
            {
                isNeedOper = value;
            }
        }

        private int showdays = 999;

        [Category("控件设置"), Description("显示确认项目的天数，0为当天、1为前一天、以此类推")]
        public int ShowDays
        {
            get
            {
                return showdays;
            }
            set
            {
                this.showdays = value;
            }
        }

        /// <summary>
        /// 调用组套判断药品库存时，科室代码取值
        /// </summary>
        EnumDrugStorageDept drugStorageDept = EnumDrugStorageDept.CurrentLoginDept;

        [Category("控件设置"), Description("调用组套判断药品库存时，科室代码取值")]
        public EnumDrugStorageDept DrugStorageDept
        {
            get
            {
                return this.drugStorageDept;
            }
            set
            {
                this.drugStorageDept = value;
            }
        }
        private bool isHaveRight = false;
        /// <summary>
        /// 是否仅显示具有处方权医生 true 是  false 否"
        /// </summary>
        [Category("控件设置"), Description("是否仅显示具有处方权医生 true 是  false 否"), DefaultValue(false)]
        public bool IsHaveRight
        {
            get { return isHaveRight; }
            set { isHaveRight = value; }
        }
        #endregion

        #region 变量


        private enum Cols
        {
            IsExec, //0
            ExecDate,
            ItemCode,//1
            ItemName,//2
            CombText,
            ItemQty,//3
            ItemAlreadConfirmQty,//4
            ItemConfirmQty,//5
            Unit,//6
            Price,//7
            TotCost,//8
            OrderType,//9
            //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
            ItemStatus,//10项目状态：未确认、部分确认、全部确认，通过各数量进行综合比较
            Machine,//11项目使用设备：从医技载体表进行查找
            Operator,//12技师：默认是当前操作员，可以修改
            //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
            OrgPrice,//原价，零差价后需储存药品原价方便日后统计
            CombNo,
            ReciptDoctor,//开单医生
            UseDate//执行时间
        }
        /// <summary>
        /// 患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo myPatient = new Neusoft.HISFC.Models.RADT.PatientInfo();

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee oper = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 医嘱业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Order orderManager = new Neusoft.HISFC.BizProcess.Integrate.Order();

        /// <summary>
        /// 费用业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeManager = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        private Neusoft.HISFC.BizProcess.Integrate.Pharmacy pharmacyManager = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();
        /// <summary>
        /// 终端业务
        /// </summary>
        private Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm terminalManager = new Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm();

        /// <summary>
        /// 终端业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm confirmIntergrate = new Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm();
        //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
        /// <summary>
        /// 医技载体业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Terminal.TerminalCarrier terminalCarrier = new Neusoft.HISFC.BizLogic.Terminal.TerminalCarrier();

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        private Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager userPowerLogic = new Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager();
        private ArrayList alExecOrder = new ArrayList();

        private DataTable dtExecOrder = new DataTable();

        private DataView dvExecOrder = new DataView();

        private string filePath = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + @"\Profile\TecExecOrder.xml";

        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// ToolBarService
        /// </summary>
        private Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        #region 单价函数


        SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice itemPrice = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice();


        #endregion

        /// <summary>
        /// 项目选择控件
        /// </summary>
        public Neusoft.HISFC.Components.Common.Controls.ucItemList ucItemList;

        private int currentRow = 0;
        private int currentColumn = 0;
        private DateTime dtNow = DateTime.Now;

        //是否使用电子申请单
        private string isUseDL = "0";
        private Hashtable depts = new Hashtable();

        #endregion

        #region 方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载收费项目信息...");
            Application.DoEvents();

            oper = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Clone();

            //this.ucItemList = new ucItemList(Neusoft.HISFC.Components.Common.Controls.EnumShowItemType.Undrug);
            //this.ucItemList = new Neusoft.HISFC.Components.Common.Controls.ucItemList(itemType);
            //{A79AF523-6D9D-4d57-A8A8-277252334AF7}
            this.ucItemList = new Neusoft.HISFC.Components.Common.Controls.ucItemList();

            //ucItemList.enuShowItemType = itemType;{270CF1A4-2A6B-4137-8358-BEB5179E1800}
            // 添加项目列表
            this.Controls.Add(this.ucItemList);

            ucItemList.enuShowItemType = itemType;//挪到这里就好了{270CF1A4-2A6B-4137-8358-BEB5179E1800}
            ucItemList.Init(string.Empty);

            // 设置项目列表不可见

            this.ucItemList.Visible = false;
            // 使项目列表最前

            this.ucItemList.BringToFront();
            // 收费项目列表选择项目事件
            this.ucItemList.SelectItem += new Neusoft.HISFC.Components.Common.Controls.ucItemList.MyDelegate(ucItemList_SelectItem);

            this.InitFp();

            this.fpExecOrder.KeyEnter += new Neusoft.FrameWork.WinForms.Controls.NeuFpEnter.keyDown(fpExecOrder_KeyEnter);

            isUseDL = cpMgr.GetControlParam<string>("PACSZY");

            ArrayList alItemZT = this.mgr.GetConstantList("ItemZT");
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
                            obj = new Neusoft.FrameWork.Models.NeuObject();
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
                            obj = new Neusoft.FrameWork.Models.NeuObject();
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

            ArrayList feeCodeStats = this.feeCodeStatManager.QueryFeeCodeStatByReportCode("ZY01");

            hsFeeStats = new Hashtable();
            foreach (Neusoft.HISFC.Models.Fee.FeeCodeStat feeCodeStat in feeCodeStats)
            {
                if (!hsFeeStats.ContainsKey(feeCodeStat.MinFee.ID))
                {
                    hsFeeStats.Add(feeCodeStat.MinFee.ID, feeCodeStat);
                }
            }

            //查找病区包含的科室
            depts.Clear();
            ArrayList alDept = managerIntegrate.QueryDepartment((this.terminalCarrier.Operator as Neusoft.HISFC.Models.Base.Employee).Nurse.ID);
            if (alDept != null)
            {
                foreach (Neusoft.FrameWork.Models.NeuObject obj in alDept)
                {
                    if (depts.ContainsKey(obj.ID) == false)
                    {
                        depts.Add(obj.ID, obj.Name);
                    }
                }
            }

            if (depts.ContainsKey(this.oper.Dept.ID) == false)
            {
                depts.Add(this.oper.Dept.ID, this.oper.Dept.Name);
            }


            if (!isShowWebEMR)
            {
                this.tabControl1.TabPages.Remove(tpWebEMR);
            }

        }

        void cmbDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((tvInpatientConfirm)this.tv).FindDefaultDept(this.cmbDept.Tag.ToString());
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private void InitFp()
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm(Neusoft.FrameWork.Management.Language.Msg("正在初始化表格，请稍候....."));
            try
            {
                if (System.IO.File.Exists(this.filePath))
                {

                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(this.filePath, this.dtExecOrder, ref this.dvExecOrder, this.fpExecOrder_Sheet1);

                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpExecOrder_Sheet1, this.filePath);
                }
                else
                {
                    this.dtExecOrder.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("执行标记",typeof(bool)),
                        new DataColumn("发送日期",typeof(string)),
                        new DataColumn("项目编码",typeof(string)),
                        new DataColumn("项目名称",typeof(string)),
                        new DataColumn("总数量",typeof(decimal)),
                        new DataColumn("已确认数量",typeof(decimal)),
                        new DataColumn("确认数量",typeof(decimal)),
                        new DataColumn("单位",typeof(string)),
                        new DataColumn("单价",typeof(decimal)),
                        new DataColumn("总额",typeof(decimal)),
                        new DataColumn("SOURCE",typeof(string)),
                        //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
                        new DataColumn("状态",typeof(string)),
                        new DataColumn("对应设备",typeof(string)),
                        new DataColumn("技师",typeof(string)),
                        new DataColumn("原价",typeof(decimal)),
                        new DataColumn("开方医生",typeof(string))//by zhaoyq 2018-08-15
                        //by yuyun 08-7-7
                    });

                    this.dvExecOrder = new DataView(this.dtExecOrder);

                    this.fpExecOrder.DataSource = this.dvExecOrder;

                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpExecOrder_Sheet1, this.filePath);
                }
            }
            catch
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();

            this.fpExecOrder_Sheet1.DefaultStyle.Locked = true;

            this.fpExecOrder_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.IsExec].Locked = false;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ExecDate].Locked = true;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemName].Locked = false;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemQty].Locked = true;
            //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
            //this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemQty].Visible = false;
            //by yuyun 08-7-7
            #region {DEF0DE5C-96BF-4f48-80DC-297B9B16315A}
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemCode].Locked = true;
            #endregion
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemAlreadConfirmQty].Locked = true;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemConfirmQty].Locked = false;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.Unit].Locked = true;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.Price].Locked = true;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.TotCost].Locked = true;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.OrderType].Locked = true;
            //by yuyun 08-7-8{52355595-B401-4db9-82BC-A3650F11D2CC}
            this.fpExecOrder_Sheet1.Columns[(int)Cols.ItemStatus].Locked = true;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.Machine].Locked = false;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.Operator].Locked = false;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.OrgPrice].Locked = true;
            //在技师列中加入人员列表供选择
            ArrayList al = new ArrayList();
            Neusoft.HISFC.BizProcess.Integrate.Manager ztManager = new Neusoft.HISFC.BizProcess.Integrate.Manager();
            al = ztManager.QueryEmployeeAll();
            //.QueryEmployee(Neusoft.HISFC.Models.Base.EnumEmployeeType.T);
            this.fpExecOrder.SetColumnList(this.fpExecOrder_Sheet1, (int)Cols.Operator, al);
            //设备列加入医技设备数据，只显示当前操作员所在科室的医技设备
            al = this.terminalCarrier.GetDesigns(this.oper.Dept.ID);
            if (al.Count > 0)
            {
                ArrayList alTer = new ArrayList();
                foreach (HISFC.Models.Terminal.TerminalCarrier terObj in al)
                {
                    alTer.Add(new Neusoft.FrameWork.Models.NeuObject(terObj.CarrierCode, terObj.CarrierName, ""));
                }
                this.fpExecOrder.SetColumnList(this.fpExecOrder_Sheet1, (int)Cols.Machine, alTer);
            }
            this.fpExecOrder.SetItem += new Neusoft.FrameWork.WinForms.Controls.NeuFpEnter.setItem(fpExecOrder_SetItem);
            //by yuyun 08-7-8
            FarPoint.Win.Spread.CellType.MultiOptionCellType checkBoxCellType = new FarPoint.Win.Spread.CellType.MultiOptionCellType();

            ArrayList alArrNames = Neusoft.FrameWork.Public.EnumHelper.Current.EnumArrayList<EnumCheckState>();
            string[] arr = new string[alArrNames.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (alArrNames[i] as Neusoft.FrameWork.Models.NeuObject).Name;
            }
            checkBoxCellType.Items = arr;
            checkBoxCellType.Orientation = FarPoint.Win.RadioOrientation.Horizontal;
            checkBoxCellType.ItemData = arr;
            this.fpExecOrder_Sheet1.Columns[(int)Cols.IsExec].CellType = checkBoxCellType;
            //this.fpExecOrder_Sheet1.Columns[(int)Cols.IsExec].Width = 220F;

            FarPoint.Win.Spread.InputMap im = this.fpExecOrder.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Escape, Keys.None), Keys.None);

        }


        /// <summary>
        /// 添加医嘱数据到表格
        /// </summary>
        /// <param name="alExecOrder"></param>
        /// <returns></returns>
        protected virtual int AddExecOrderToFp(ArrayList alExecOrder)
        {
            try
            {
                decimal totCost = 0;
                foreach (Neusoft.HISFC.Models.Order.ExecOrder order in alExecOrder)
                {
                    #region {5197289A-AB55-410b-81EE-FC7C1B7CB5D7}
                    //已经判断
                    //修正长期终端确认医嘱，护士分解，但没有打勾没有保存的数据也显示出来的问题
                    //if (order.Order.OrderType.IsDecompose)
                    //{
                    //    if (!this.orderManager.CheckLongUndrugIsConfirm(order.ID))
                    //    {
                    //        continue;
                    //    }
                    //}
                    #endregion

                    int row = 0;

                    if (hsRows.ContainsKey(order.ID))
                    {
                        row = hsRows[order.ID];
                    }
                    else
                    {
                        this.fpExecOrder.Sheets[0].Rows.Add(this.fpExecOrder.Sheets[0].RowCount, 1);
                        row = this.fpExecOrder.Sheets[0].RowCount - 1;
                        hsRows.Add(order.ID, row);
                    }

                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.IsExec].Value = string.IsNullOrEmpty(this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.IsExec].Text) ? (int)defaultConfirmState : this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.IsExec].Value;

                    if (defaultConfirmState == EnumCheckState.ComfirmCancel)
                    {
                        this.fpExecOrder_Sheet1.Cells[row, (int)Cols.IsExec].ForeColor = Color.Red;
                    }
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ExecDate].Value = order.ExecOper.OperTime.ToShortDateString();
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemCode].Text = order.Order.Item.ID;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemName].Text = order.Order.Item.Name;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemName].Locked = true;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemQty].Text = order.Order.Qty.ToString();
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.CombNo].Locked = true;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.CombNo].Text = order.Order.Combo.ID;
                    //修改确认界面显示确认数量错误问题 {47531763-0983-44dc-BC92-59A5588AF2F8} 2014-12-12 by lixuelong
                    decimal packQty = terminalMgr.GetPackageItemPackQty(order.Order.ID, order.ID);
                    decimal AlreadyQty = terminalMgr.GetAlreadConfirmNum(order.Order.ID, order.ID) / packQty;
                    if (AlreadyQty < 0)
                    {
                        MessageBox.Show("获取已确认项目数量失败" + terminalMgr.Err);
                        return -1;
                    }
                    decimal LeaveQty = order.Order.Qty - AlreadyQty;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemAlreadConfirmQty].Text = AlreadyQty.ToString();
                    if (string.IsNullOrEmpty(this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemConfirmQty].Text) || LeaveQty <= Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemConfirmQty].Value))
                    {
                        this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemConfirmQty].Text = LeaveQty.ToString();
                    }
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.Unit].Text = order.Order.Unit;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.Price].Text = order.Order.Item.Price.ToString();
                    if (order.Order.Item.Price == 0 && order.Order.Unit != "[复合项]")//by zhouxs 2007-10-28
                    {
                        this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.Price].Locked = false;
                    }//end zhouxs
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.TotCost].Text = Convert.ToString(order.Order.Item.Price * order.Order.Qty);
                    totCost += order.Order.Item.Price * order.Order.Qty;

                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.OrderType].Text = "ORDER";
                    //by yuyun 08-7-8{07D1BACB-8E4F-4ac8-8254-81763D0F0699}
                    if (AlreadyQty > 0 && AlreadyQty < order.Order.Qty)
                    {
                        this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemStatus].Text = "部分确认";
                        this.fpExecOrder.Sheets[0].Rows[row].BackColor = Color.LightBlue;//将部分确认的项目用颜色区分
                    }
                    else
                    {
                        this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ItemStatus].Text = "未确认";
                    }
                    //医技设备
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.Machine].Text = "";
                    ////////////////////
                    //技师
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.Operator].Text = this.oper.Name;
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.Operator].Tag = this.oper.ID;
                    //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.OrgPrice].Text = order.Order.Item.DefPrice.ToString();
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ReciptDoctor].Text = order.Order.ReciptDoctor.Name.ToString();//by zhaoyq 20180815
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.ReciptDoctor].Tag = order.Order.ReciptDoctor.ID;//by zhaoyq 20180815

                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.UseDate].Text = order.DateUse.ToString();
                    this.fpExecOrder.Sheets[0].Cells[row, (int)Cols.UseDate].Tag = order.DateUse.ToString();
                    this.fpExecOrder.Sheets[0].Rows[row].Tag = order;
                }

                //lblTotCost.Text = "金额：" + totCost.ToString();
                //DrawCombFlag(this.fpExecOrder.Sheets[0], (int)Cols.CombText, 21, 11);
                DrawComboLeft(this.fpExecOrder.Sheets[0], (int)Cols.CombNo, (int)Cols.CombText, 0);
                this.ShowTotFee();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载表格格式错误！请更新TecExecOrder.xml文件" + ex.Message);

                return -1;
            }
            return 0;
        }


        #region {53061BA2-33FC-469d-9773-9A9454023612}
        /// <summary>
        /// 删除新项目事件

        /// </summary>

        void DeleteItemEvent()
        {
            this.MakeItemListDisappear();
            this.DeleteNew();
        }

        /// <summary>
        /// 删除一行(当前行)新项目

        /// </summary>
        public void DeleteNew()
        {
            // 如果没有记录，则返回
            if (this.fpExecOrder_Sheet1.RowCount == 0)
            {
                return;
            }
            // 只能删除新项目

            if (this.GetItem(this.fpExecOrder_Sheet1.ActiveRowIndex, (int)Cols.OrderType) != "NEW")
            {
                MessageBox.Show("该项目不允许删除", "医技终端确认");
                //this.Focus();
                //this.CellFocus(this.fpExecOrder_Sheet1.ActiveRowIndex, (int)Cols.OrderType);
                return;
            }
            // 删除
            this.DeleteRow(this.fpExecOrder_Sheet1.ActiveRowIndex, true);
        }
        /// <summary>
        /// 按指定的行号删除一行

        /// [参数1: int row - 要删除的行号]
        /// [参数2: bool confirm - 是否需要用户确认]
        /// </summary>
        /// <param name="row">要删除的行号</param>
        /// <param name="confirm">是否需要确认</param>
        public void DeleteRow(int row, bool confirm)
        {
            // 如果是空记录，则直接删除，否则需要确认删除

            if (confirm && (!this.IsNull(row)))
            {
                // 如果取消删除，那么返回

                if (MessageBox.Show("是否删除当前行？", "医技终端确认",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.Focus();
                    return;
                }
            }
            //{8C0239CE-4272-4f30-B547-9C3C27D694E4} 停止编辑模式，否则删除时保留了当前选中文本
            this.fpExecOrder.EditMode = false;
            // 删除指定的行
            this.fpExecOrder_Sheet1.RemoveRows(row, 1);
            // 设置焦点到上一行

            //this.fpExecOrder_Sheet1.Focus();
            //if (row - 1 >= 0)
            //{
            //    this.CurrentRow = row - 1;
            //    // 设置焦点到项目名称

            //    this.CurrentColumn = (int)DisplayField.ItemName;
            //}
        }
        /// <summary>
        /// 判断指定行是否为空（如果项目编号14列为空就认为是空记录）

        /// [参数: int row - 行号]
        /// [返回: bool,true - 空, false - 非空]
        /// </summary>
        /// <param name="row">指定的行号</param>
        /// <returns>true：为空/false：不为空</returns>
        public bool IsNull(int row)
        {
            // 如果项目编码为空，代表项目为空

            if (this.fpExecOrder_Sheet1.Cells[row, (int)Cols.ItemCode].Text.Equals(""))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 获取一个CELL的值

        /// [参数1: int row - 行号]
        /// [参数2: int column - 列号]
        /// [返回: string, 文本值]
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <returns>CELL里面的文本</returns>
        public string GetItem(int row, int column)
        {
            return this.fpExecOrder_Sheet1.Cells[row, column].Text;
        }
        /// <summary>
        /// 是收费项目选择控件不可见

        /// </summary>
        private void MakeItemListDisappear()
        {
            if (this.ucItemList.Visible == true)
            {
                this.ucItemList.Visible = false;
            }
        }
        #endregion
        private void Clear()
        {
            this.myPatient = new Neusoft.HISFC.Models.RADT.PatientInfo();
            //this.ucQueryInpatientNo1.Text = string.Empty;
            lblName.Text = "患者姓名：";
            lblPatientNO.Text = "住院号：";
            lblDept.Text = "患者科室：";
            lblFreeCost.Text = "可用余额：";
            this.fpExecOrder.Sheets[0].RowCount = 0;
            this.hsRows.Clear();
        }

        /// <summary>
        /// 树选择
        /// </summary>
        /// <param name="neuObject"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override int OnSetValue(object neuObject, TreeNode e)
        {

            Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
            if (neuObject != null)
            {
                if (neuObject.GetType() == patientInfo.GetType())
                {
                    patientInfo = neuObject as Neusoft.HISFC.Models.RADT.PatientInfo;
                    if (this.myPatient != patientInfo || this.myPatient == null || patientInfo == null || !this.myPatient.ID.Equals(patientInfo.ID))
                    {
                        this.myPatient = patientInfo;
                        this.fpExecOrder.Sheets[0].RowCount = 0;
                        this.hsRows.Clear();
                    }

                    lblName.Text = "患者姓名：" + this.myPatient.Name;
                    lblPatientNO.Text = "住院号：" + this.myPatient.PID.PatientNO;
                    lblDept.Text = "患者科室：" + this.myPatient.PVisit.PatientLocation.Dept.Name;
                    lblFreeCost.Text = "可用余额：" + this.myPatient.FT.LeftCost.ToString();
                    //1、界面中显示的患者基本信息中要显示出患者住院所在科室、病区及床号
                    lbNurseCellName.Text = "病区：" + this.myPatient.PVisit.PatientLocation.NurseCell.Name;
                    lbBedName.Text = "床号：" + this.myPatient.PVisit.PatientLocation.Bed.Name;
                    neuLabel1.Text = "年龄：" + this.myPatient.Age;
                    neuLabel3.Text = "性别：" + this.myPatient.Sex.Name;
                    neuLabel2.Text = "合同单位：" + this.myPatient.Pact.Name;
                    this.ucQueryInpatientNo1.Text = this.myPatient.PID.PatientNO;
                    //this.ucQueryInpatientNo1_myEvent();
                    if (this.seeAll)
                    {
                        alExecOrder = this.orderManager.QueryExecOrderByDept(patientInfo.ID, "2", false, "all");
                    }
                    else
                    {
                        alExecOrder = new ArrayList();
                        //查找所有的
                        foreach (DictionaryEntry de in depts)
                        {
                            ArrayList al = this.orderManager.QueryExecOrderByDept(patientInfo.ID, "2", false, de.Key.ToString());
                            if (al != null)
                            {
                                alExecOrder.AddRange(al);
                            }
                        }
                        //alExecOrder = this.orderManager.QueryExecOrderByDept(patientInfo.ID, "2", false, oper.Dept.ID);
                    }
                    if (alExecOrder == null || alExecOrder.Count == 0)
                    {
                        MessageBox.Show("该患者没有需要确认的项目");
                        //this.Clear();
                    }
                    else
                    {
                        if (this.myPatient.DayOperationFlag == "1" || this.myPatient.DayOperationFlag == "2")
                        {
                            Neusoft.HISFC.Models.Order.ExecOrder order = alExecOrder[0] as Neusoft.HISFC.Models.Order.ExecOrder;
                            this.cmbDoct.Tag = order.Order.ReciptDoctor;
                        }
                        this.AddExecOrderToFp(alExecOrder);
                    }

                    this.tabControl1.SelectedTab = this.tpItemInfo;

                    //查询WebEMR
                    string URL = webEMRUrl;
                    URL = string.Format(URL, patientInfo.ID);
                    this.webBrowser1.Refresh(WebBrowserRefreshOption.Normal);
                    this.webBrowser1.Navigate(URL);
                }
            }
            else
            {
                this.Clear();
                this.Refresh();
                this.ucQueryInpatientNo1.Text = string.Empty;
            }

            return base.OnSetValue(neuObject, e);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                this.Init();
                base.OnLoad(e);
                InputMap im;
                im = fpExecOrder.GetInputMap(InputMapMode.WhenAncestorOfFocused);
                im.Put(new Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

                im = fpExecOrder.GetInputMap(InputMapMode.WhenAncestorOfFocused);
                im.Put(new Keystroke(Keys.Down, Keys.None), FarPoint.Win.Spread.SpreadActions.None);

                im = fpExecOrder.GetInputMap(InputMapMode.WhenAncestorOfFocused);
                im.Put(new Keystroke(Keys.Up, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化错误:" + ex.Message + "   请删掉" + this.filePath + "后重试！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// 添加新划价数据
        /// </summary>
        private void AddNewRow()
        {
            if (this.myPatient == null || this.myPatient.ID == "")
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("您没有选择患者！"));
                return;
            }
            int rowcount = this.fpExecOrder.Sheets[0].RowCount;
            if (AddFirstRow)
            {
                this.fpExecOrder.Sheets[0].Rows.Add(0, 1);
                this.fpExecOrder.Sheets[0].Cells[0, (int)Cols.OrderType].Text = "NEW";
                #region {15181E09-0842-4e3f-A91C-A25F3930CA28}
                this.fpExecOrder.Sheets[0].SetActiveCell(0, (int)Cols.ItemName, true);
                #endregion

            }
            else
            {
                this.fpExecOrder.Sheets[0].Rows.Add(rowcount, 1);
                this.fpExecOrder.Sheets[0].Cells[rowcount, (int)Cols.OrderType].Text = "NEW";
                #region {15181E09-0842-4e3f-A91C-A25F3930CA28}
                this.fpExecOrder.Sheets[0].SetActiveCell(rowcount, (int)Cols.ItemName, true);
                #endregion
            }
            #region {15181E09-0842-4e3f-A91C-A25F3930CA28}
            this.fpExecOrder.EditMode = true;
            //this.fpExecOrder.Focus();
            #endregion
            this.currentRow = rowcount;
            this.ShowTotFee();
        }

        /// <summary>
        /// 添加新划价数据到表格
        /// </summary>
        private int InsertItem()
        {
            Neusoft.HISFC.Models.Base.Item item = null;
            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = null;
            Neusoft.HISFC.Models.Pharmacy.Item pharmacy = null;
            int intReturn = this.ucItemList.GetSelectItem(out item);

            if (item == null || item.ID == "")
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("没有找到项目"));
                return -1;
            }
            if (item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
            {

                undrug = feeManager.GetUndrugByCode(item.ID);

                //凤凰国际病区收取特殊费用
                decimal unitPrice = 0.0M; decimal orgPrice = item.Price;
                // SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice itemPrice = GetItemPrice();
                undrug.Price = itemPrice.GetPriceForInpatient(item.ID, myPatient, item.Price, item.ChildPrice, item.SpecialPrice, item.Price, ref orgPrice);
                item.Price = undrug.Price;
                undrug.DefPrice = orgPrice;
                item.DefPrice = undrug.DefPrice;

                if (intReturn > 0)
                {
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.IsExec].Value = (int)EnumCheckState.ComfirmComplete;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemCode].Text = undrug.ID;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemName].Text = undrug.Name;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Unit].Text = undrug.PriceUnit;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemConfirmQty].Value = 1;
                    if (undrug.UnitFlag == "0")
                    {
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Price].Text = undrug.Price.ToString();
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.OrgPrice].Text = orgPrice.ToString();
                    }
                    else if (undrug.UnitFlag == "1")
                    {
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Price].Text = undrug.Price.ToString();
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.OrgPrice].Text = orgPrice.ToString();
                    }
                    this.UnDisplayUcItemList();
                    this.fpExecOrder.Sheets[0].ActiveRow.Tag = item;
                }
            }
            else//药品录入
            {
                pharmacy = pharmacyManager.GetItem(item.ID);

                if (intReturn > 0)
                {
                    //HISFC.Models.RADT.Patient patient = myPatient as HISFC.Models.RADT.Patient;
                    //Neusoft.HISFC.Models.Registration.Register regModel = patient as Neusoft.HISFC.Models.Registration.Register;
                    //Neusoft.HISFC.Models.Registration.Register regModel = (Neusoft.HISFC.Models.Registration.Register)myPatient  ;
                    //Neusoft.HISFC.Models.RADT.PatientInfo regmodel = new Neusoft.HISFC.Models.Registration.Register();

                    decimal unitPrice = pharmacy.Price;
                    decimal orgPrice = pharmacy.RetailPrice2;
                    int i = this.feeManager.GetPriceForInpatient(myPatient, item, ref unitPrice, ref orgPrice);

                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.IsExec].Value = (int)EnumCheckState.ComfirmComplete;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemCode].Text = pharmacy.ID;
                    if (pharmacy.Specs != null && pharmacy.Specs != string.Empty)
                    {
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemName].Text = pharmacy.Name + "{" + pharmacy.Specs + "}";
                    }

                    //价格、单位
                    FarPoint.Win.Spread.CellType.ComboBoxCellType comboType = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                    comboType.Editable = true;
                    comboType.Items = (new string[]{pharmacy.MinUnit,
                                                pharmacy.PackUnit});
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Unit].CellType = comboType;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Unit].Locked = false;
                    decimal price = 0.00M;
                    if (item.SpecialFlag4 == "2") //包装单位
                    {
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Unit].Text = pharmacy.PackUnit;
                        pharmacy.PriceUnit = pharmacy.PackUnit;
                        //price = Neusoft.FrameWork.Public.String.FormatNumber(pharmacy.Price, 4);
                        price = Neusoft.FrameWork.Public.String.FormatNumber(unitPrice, 4);
                    }
                    else
                    {
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Unit].Text = pharmacy.MinUnit;
                        pharmacy.PriceUnit = pharmacy.MinUnit;
                        //price = Neusoft.FrameWork.Public.String.FormatNumber(pharmacy.Price / pharmacy.PackQty, 4);
                        price = Neusoft.FrameWork.Public.String.FormatNumber(unitPrice / pharmacy.PackQty, 4);
                    }
                    //数量
                    pharmacy.Qty = 1;
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemConfirmQty].Value = pharmacy.Qty;
                    //价格
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Price].Text = price.ToString();
                    this.UnDisplayUcItemList();
                    this.fpExecOrder.Sheets[0].ActiveRow.Tag = pharmacy;
                }
            }

            return 1;
        }

        /// <summary>
        /// 项目选择列表隐藏
        /// </summary>
        public void UnDisplayUcItemList()
        {
            if (this.ucItemList.Visible)
            {
                this.ucItemList.Visible = false;
            }
        }

        /// <summary>
        /// 表格数据转换成order
        /// </summary>
        /// <returns></returns>
        private ArrayList GetFeeOrder()
        {
            int rowCount = this.fpExecOrder.Sheets[0].RowCount;
            ArrayList alFeeOrder = new ArrayList();
            for (int i = 0; i < rowCount; i++)
            {
                Neusoft.HISFC.Models.Order.Inpatient.Order obj = new Neusoft.HISFC.Models.Order.Inpatient.Order();
                if (Neusoft.FrameWork.Function.NConvert.ToInt32(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value) == (int)EnumCheckState.ComfirmComplete)
                {
                    Neusoft.HISFC.Models.Base.OperEnvironment o = new Neusoft.HISFC.Models.Base.OperEnvironment();
                    o.ID = oper.ID;
                    o.Name = oper.Name;
                    o.Dept = oper.Dept;
                    o.OperTime = this.dtNow;
                    if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrderType].Text == "ORDER")
                    {
                        Neusoft.HISFC.Models.Order.ExecOrder order = new Neusoft.HISFC.Models.Order.ExecOrder(); ;
                        order = (Neusoft.HISFC.Models.Order.ExecOrder)this.fpExecOrder.Sheets[0].Rows[i].Tag;
                        Neusoft.HISFC.Models.Fee.Item.Undrug undrug = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        undrug = this.feeManager.GetUndrugByCode(order.Order.Item.ID);
                        decimal AlreadyQty = terminalMgr.GetAlreadConfirmNum(order.Order.ID, order.ID);
                        if (AlreadyQty < 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("获取已确认项目数量失败" + terminalMgr.Err);
                            return null;
                        }
                        decimal LeaveQty = order.Order.Qty - AlreadyQty;
                        order.Order.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.ItemConfirmQty].Value);
                        if (order.Order.Item.Qty > LeaveQty)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("数量大于未确认的数量，请刷新！");
                            return null;
                        }

                        if (order.Order.Item.Qty <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("数量必须大于零，请重新输入！");
                            return null;
                        }

                        obj = order.Order;
                        obj.Item.MinFee = undrug.MinFee;
                        obj.Item.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Price].Value);
                        obj.Item.DefPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrgPrice].Text);
                        obj.ExecOper = o;
                        obj.ExecOper.Dept = oper.Dept;//order.Order.ExeDept.Clone();
                        obj.Oper = o;
                        obj.User03 = order.ID;
                        //执行设备
                        //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
                        if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Machine].Value != null)
                        {
                            obj.Item.User01 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Machine].Value.ToString();
                        }
                        //执行技师
                        if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Operator].Value != null)
                        {
                            obj.Item.User02 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Operator].Value.ToString();
                        }
                        alFeeOrder.Add(obj);
                    }

                }
            }

            return alFeeOrder;
        }

        /// <summary>
        /// 表格中的新增收费数据转换成FeeItemList
        /// </summary>
        /// <returns></returns>
        private ArrayList GetNewFeeItemList()
        {
            int rowCount = this.fpExecOrder.Sheets[0].RowCount;
            ArrayList alFeeItemList = new ArrayList();
            for (int i = 0; i < rowCount; i++)
            {
                //{A22E7A8E-DE5E-40fc-8273-F774B286B7C8}改正复合项目补划价错误
                if (Neusoft.FrameWork.Function.NConvert.ToInt32(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value) == (int)EnumCheckState.ComfirmComplete)
                {
                    Neusoft.HISFC.Models.Base.OperEnvironment o = new Neusoft.HISFC.Models.Base.OperEnvironment();
                    o.ID = oper.ID;
                    o.Name = oper.Name;
                    o.Dept = oper.Dept;
                    o.OperTime = this.dtNow;
                    //------------------------
                    if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrderType].Text == "NEW")
                    {
                        Neusoft.HISFC.Models.Base.Item item = new Neusoft.HISFC.Models.Base.Item(); ;
                        Neusoft.HISFC.Models.Fee.Item.Undrug undrug = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        item = this.fpExecOrder.Sheets[0].Rows[i].Tag as Neusoft.HISFC.Models.Base.Item;

                        if (item == null)
                        {
                            continue;
                        }

                        if (item is Neusoft.HISFC.Models.Fee.Item.Undrug)
                        {
                            #region 非药品
                            undrug = this.feeManager.GetUndrugByCode(item.ID);
                            //{F6A2DCD5-10B9-4fac-8ACB-D4B6FE9D684F}当住院补记账的项目的unitflag为空时，该费用没计上但提示确认成功。
                            if (string.IsNullOrEmpty(undrug.UnitFlag))
                            {
                                MessageBox.Show(undrug.Name + "该项目既不是明细项目，也不是组套项目，不能对其进行计费，请重新维护该项目信息。");
                                return null;
                            }
                            //----------------------------------------------
                            if (undrug.UnitFlag == "0")
                            {
                                //{A22E7A8E-DE5E-40fc-8273-F774B286B7C8}改正复合项目补划价错误
                                Neusoft.HISFC.Models.Order.Inpatient.Order obj = new Neusoft.HISFC.Models.Order.Inpatient.Order();

                                obj.Item = item;
                                obj.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.ItemConfirmQty].Text);

                                if (obj.Qty <= 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("数量必须大于零，请重新输入！");
                                    return null;
                                }
                                if (oper.EmployeeType.ID.ToString() == "D")
                                {
                                    obj.ReciptDept = oper.Dept;
                                    obj.ReciptDoctor.ID = oper.ID;
                                }
                                else
                                {
                                    obj.ReciptDept = this.myPatient.PVisit.PatientLocation.Dept;
                                    obj.ReciptDoctor.ID = this.myPatient.PVisit.AttendingDoctor.ID;
                                }
                                if (obj.ReciptDoctor.ID == string.Empty)
                                {
                                    obj.ReciptDoctor.ID = this.cmbDoct.Tag.ToString();  
                                }
                                if (obj.ReciptDoctor.ID == string.Empty)
                                {
                                    MessageBox.Show("开方医生为空，请重新输入！");
                                    return null;
                                }
                                obj.ExecOper = o;
                                obj.Unit = item.PriceUnit;
                                obj.Oper = o;
                                obj.ExeDept = oper.Dept;//by yuyun 08-8-12 插入确认科室
                                //{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
                                obj.Item.User01 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Machine].Text;//执行设备
                                obj.Item.User02 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Operator].Text;//执行技师
                                obj.Item.MinFee = undrug.MinFee;
                                alFeeItemList.Add(obj);
                            }
                            else if (undrug.UnitFlag == "1")
                            {
                                Neusoft.HISFC.BizProcess.Integrate.Manager ztManager = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                                ztManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                                ArrayList alZtDetail = ztManager.QueryUndrugPackageDetailByCode(undrug.ID);
                                foreach (Neusoft.HISFC.Models.Fee.Item.Undrug undrugitem in alZtDetail)
                                {
                                    //{A22E7A8E-DE5E-40fc-8273-F774B286B7C8}改正复合项目补划价错误
                                    Neusoft.HISFC.Models.Order.Inpatient.Order obj = new Neusoft.HISFC.Models.Order.Inpatient.Order();

                                    Neusoft.HISFC.Models.Fee.Item.Undrug myUndrug = this.feeManager.GetUndrugByCode(undrugitem.ID);
                                    obj.Item = myUndrug as Neusoft.HISFC.Models.Base.Item;
                                    obj.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.ItemConfirmQty].Text);
                                    if (obj.Qty <= 0)
                                    {
                                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                        MessageBox.Show("数量必须大于零，请重新输入！");
                                        return null;
                                    }
                                    //{6EFEC5EC-2258-4d3e-877B-179215E2F783} 重新计算明细数量
                                    obj.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.ItemConfirmQty].Text) * undrugitem.Qty;
                                    if (obj.Item.Qty <= 0)
                                    {
                                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                        MessageBox.Show("数量必须大于零，请重新输入！");
                                        return null;
                                    }
                                    if (oper.EmployeeType.ID.ToString() == "D")
                                    {
                                        obj.ReciptDept = oper.Dept;
                                        obj.ReciptDoctor.ID = oper.ID;
                                    }
                                    else
                                    {
                                        obj.ReciptDept = this.myPatient.PVisit.PatientLocation.Dept;
                                        obj.ReciptDoctor.ID = this.myPatient.PVisit.AttendingDoctor.ID;
                                    }
                                    if (obj.ReciptDoctor.ID == string.Empty)
                                    {
                                        obj.ReciptDoctor.ID = this.cmbDoct.Tag.ToString();
                                    }
                                    if (obj.ReciptDoctor.ID == string.Empty)
                                    {
                                        MessageBox.Show("开方医生为空，请重新输入！");
                                        return null;
                                    }
                                    obj.Unit = undrugitem.PriceUnit;
                                    obj.ExecOper = o;
                                    obj.ExeDept = oper.Dept;
                                    obj.Oper = o;
                                    //{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
                                    obj.Item.User01 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Machine].Text;//执行设备
                                    obj.Item.User02 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Operator].Text;//执行技师
                                    obj.Item.MinFee = undrugitem.MinFee;
                                    alFeeItemList.Add(obj);
                                }
                            }
                            #endregion

                        }
                        else if (item is Neusoft.HISFC.Models.Pharmacy.Item)
                        {
                            #region 药品

                            Neusoft.HISFC.Models.Pharmacy.Item pharmacy = pharmacyManager.GetItem(item.ID);

                            Neusoft.HISFC.Models.Order.Inpatient.Order obj = new Neusoft.HISFC.Models.Order.Inpatient.Order();
                            obj.Item = pharmacy;
                            obj.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.ItemConfirmQty].Text);

                            if (obj.Qty <= 0)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(string.Format("{0}的数量必须大于零，请重新输入！", item.Name));
                                return null;
                            }
                            string priceUnit = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Unit].Text;
                            decimal price = Neusoft.FrameWork.Function.NConvert.ToDecimal(Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Price].Text));
                            decimal orgPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrgPrice].Text));
                            if (oper.EmployeeType.ID.ToString() == "D")
                            {
                                obj.ReciptDept = oper.Dept;
                                obj.ReciptDoctor.ID = oper.ID;
                            }
                            else
                            {
                                obj.ReciptDept = this.myPatient.PVisit.PatientLocation.Dept;
                                obj.ReciptDoctor.ID = this.myPatient.PVisit.AttendingDoctor.ID;
                            }
                            obj.ExecOper = o;
                            obj.Item.Price = price;
                            obj.Item.DefPrice = orgPrice;
                            obj.Item.Qty = obj.Qty;
                            obj.Unit = priceUnit;
                            obj.Item.PriceUnit = priceUnit;

                            obj.Oper = o;


                            //根据药品id获取药品实体
                            Neusoft.HISFC.Models.Pharmacy.Storage drugStorate = null;

                            if (this.drugStorageDept == EnumDrugStorageDept.CurrentLoginDept)
                            {
                                //执行科室
                                obj.ExeDept = oper.Dept;
                                obj.ReciptDoctor.ID = oper.ID;
                            }
                            else if (this.drugStorageDept == EnumDrugStorageDept.PatientInDept)
                            {
                                obj.ExeDept = this.myPatient.PVisit.PatientLocation.Dept;
                                obj.ReciptDoctor.ID = this.myPatient.PVisit.AttendingDoctor.ID;
                            }
                            if (obj.ReciptDoctor.ID == string.Empty)
                            {
                                obj.ReciptDoctor.ID = this.cmbDoct.Tag.ToString();
                            }
                            if (obj.ReciptDoctor.ID == string.Empty)
                            {
                                MessageBox.Show("开方医生为空，请重新输入！");
                                return null;
                            }

                            obj.Item.User01 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Machine].Text;//执行设备
                            obj.Item.User02 = this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.Operator].Text;//执行技师
                            obj.Item.MinFee = pharmacy.MinFee;
                            alFeeItemList.Add(obj);
                            #endregion
                        }

                    }
                }
            }

            return alFeeItemList;
        }

        /// <summary>
        /// 表格数据转换成execorder
        /// </summary>
        /// <returns></returns>
        private ArrayList GetExecOrder()
        {
            int rowCount = this.fpExecOrder.Sheets[0].RowCount;
            ArrayList alNeedExecOrder = new ArrayList();
            for (int i = 0; i < rowCount; i++)
            {

                if (Neusoft.FrameWork.Function.NConvert.ToInt32(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value) == (int)EnumCheckState.ComfirmComplete)
                {
                    Neusoft.HISFC.Models.Base.OperEnvironment o = new Neusoft.HISFC.Models.Base.OperEnvironment();
                    o.ID = oper.ID;
                    o.Name = oper.Name;
                    o.OperTime = this.dtNow;
                    o.Dept = oper.Dept;
                    if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrderType].Text == "ORDER")
                    {
                        Neusoft.HISFC.Models.Order.ExecOrder order = new Neusoft.HISFC.Models.Order.ExecOrder(); ;
                        order = (Neusoft.HISFC.Models.Order.ExecOrder)this.fpExecOrder.Sheets[0].Rows[i].Tag;

                        order.Order.ExecOper = o;
                        order.ExecOper = o;
                        order.ExecOper.Dept = order.Order.ExeDept.Clone();
                        order.IsExec = order.IsExec;
                        order.ChargeOper = o;
                        order.IsCharge = order.IsCharge;
                        order.Order.User03 = order.ID;
                        alNeedExecOrder.Add(order);
                    }

                }
            }

            return alNeedExecOrder;
        }

        /// <summary>
        /// 获取取消收费的执行档
        /// </summary>
        /// <returns></returns>
        private ArrayList GetCancelComfirmExecOrder()
        {
            int rowCount = this.fpExecOrder.Sheets[0].RowCount;
            ArrayList alNeedExecOrder = new ArrayList();
            for (int i = 0; i < rowCount; i++)
            {

                if (Neusoft.FrameWork.Function.NConvert.ToInt32(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value) == (int)EnumCheckState.ComfirmCancel)
                {
                    Neusoft.HISFC.Models.Base.OperEnvironment o = new Neusoft.HISFC.Models.Base.OperEnvironment();
                    o.ID = oper.ID;
                    o.Name = oper.Name;
                    o.OperTime = this.dtNow;
                    o.Dept = oper.Dept;
                    if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrderType].Text == "ORDER")
                    {
                        Neusoft.HISFC.Models.Order.ExecOrder order = (Neusoft.HISFC.Models.Order.ExecOrder)this.fpExecOrder.Sheets[0].Rows[i].Tag;

                        order.Order.ExecOper = o;
                        order.ExecOper = o;
                        order.ExecOper.Dept = order.Order.ExeDept.Clone();
                        order.IsExec = order.IsExec;
                        order.ChargeOper = o;
                        order.Order.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.ItemConfirmQty].Value);

                        if (order.Order.Item.Qty <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("数量必须大于零，请重新输入！");
                            return null;
                        }
                        //不收费
                        order.IsCharge = order.IsCharge;
                        order.Order.User03 = order.ID;
                        alNeedExecOrder.Add(order);
                    }

                }
            }

            return alNeedExecOrder;
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        private bool ValidState()
        {
            for (int i = 0; i < this.fpExecOrder_Sheet1.RowCount; i++)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value))
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder_Sheet1.Cells[i, (int)Cols.ItemConfirmQty].Text) <= 0)
                    {
                        MessageBox.Show("请输入需要确认的数量");
                        return false;
                    }

                    //{130607A7-01C7-4bdf-931C-D7CE7AC8111B}
                    if (isNeedOper)
                    {
                        if (string.IsNullOrEmpty(this.fpExecOrder_Sheet1.Cells[i, (int)Cols.Operator].Text))
                        {
                            MessageBox.Show("请输入确认人.");

                            return false;
                        }
                    }

                    if (this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.OrderType].Text == "ORDER")
                    {
                        decimal totQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder_Sheet1.Cells[i, (int)Cols.ItemQty].Text);
                        decimal alreadQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder_Sheet1.Cells[i, (int)Cols.ItemAlreadConfirmQty].Text);
                        decimal confirmQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder_Sheet1.Cells[i, (int)Cols.ItemConfirmQty].Text);
                        if (totQty < alreadQty + confirmQty)
                        {
                            MessageBox.Show("可确认数量过大，最大可确认数量不能大于" + (totQty - alreadQty).ToString());
                            return false;
                        }
                    }
                    if (this.fpExecOrder_Sheet1.Cells[i, (int)Cols.OrderType].Text == "NEW")
                    {
                        #region {B56C131D-2600-421c-9D51-12A1C214CA1E}
                        Neusoft.HISFC.Models.Base.Item item = new Neusoft.HISFC.Models.Base.Item(); ;
                        item = this.fpExecOrder.Sheets[0].Rows[i].Tag as Neusoft.HISFC.Models.Base.Item;
                        if (item == null)
                        {
                            MessageBox.Show("请输入需要确认的项目！");
                            return false;
                        }
                        #endregion
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 数据转换成feeitemlist
        /// </summary>
        /// <param name="alOrder"></param>
        /// <returns></returns>
        private ArrayList ChangeOrderToFeeItemList(ArrayList alOrder)
        {
            ArrayList alFeeItemList = new ArrayList();
            foreach (Neusoft.HISFC.Models.Order.Inpatient.Order order in alOrder)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList = new Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList();
                feeItemList.Item = order.Item.Clone();
                feeItemList.Item.Qty = order.Qty;
                feeItemList.Item.PriceUnit = order.Unit;//单位重新付
                feeItemList.RecipeOper.Dept = order.ReciptDept.Clone();
                feeItemList.RecipeOper.ID = order.ReciptDoctor.ID;
                feeItemList.RecipeOper.Name = order.ReciptDoctor.Name;
                feeItemList.ExecOper = order.ExecOper.Clone();
                feeItemList.ExecOper.Dept = order.ExeDept.Clone();
                feeItemList.StockOper.Dept = order.StockDept.Clone();
                if (feeItemList.Item.PackQty == 0)
                {
                    feeItemList.Item.PackQty = 1;
                }

                if (feeItemList.Item.ItemType == EnumItemType.Drug)
                {
                    ///不知为何最小单位要除以包装数量
                    //if (feeItemList.Item.PriceUnit.Equals(((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).PackUnit))
                    //{
                    //    feeItemList.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber((feeItemList.Item.Price * feeItemList.Item.Qty), 2);
                    //    feeItemList.FeePack = "1";
                    //}
                    //else
                    //{
                    //    feeItemList.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber((feeItemList.Item.Price * feeItemList.Item.Qty / feeItemList.Item.PackQty), 2);
                    //    feeItemList.FeePack = "0";
                    //}


                    if (feeItemList.Item.PriceUnit.Equals(((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).PackUnit))
                    {
                        feeItemList.FeePack = "1";
                    }
                    else
                    {
                        feeItemList.FeePack = "0";
                    }
                    feeItemList.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber((feeItemList.Item.Price * feeItemList.Item.Qty), 2);
                }
                else
                {
                    feeItemList.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber((feeItemList.Item.Price * feeItemList.Item.Qty / feeItemList.Item.PackQty), 2);
                }

                feeItemList.FT.OwnCost = feeItemList.FT.TotCost;
                feeItemList.IsBaby = order.IsBaby;
                feeItemList.IsEmergency = order.IsEmergency;
                feeItemList.Order = order.Clone();
                feeItemList.ExecOrder.ID = order.User03;
                feeItemList.NoBackQty = feeItemList.Item.Qty;
                feeItemList.FTRate.OwnRate = 1;
                feeItemList.BalanceState = "0";
                feeItemList.ChargeOper = order.Oper.Clone();
                feeItemList.FeeOper = order.Oper.Clone();
                feeItemList.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                feeItemList.Item.User01 = order.Item.User01;
                feeItemList.Item.User02 = order.Item.User02;
                feeItemList.Item.MinFee = order.Item.MinFee;
                alFeeItemList.Add(feeItemList);
            }
            return alFeeItemList;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        private int Save()
        {
            int iReturn = 0;
            //Neusoft.HISFC.BizProcess.Integrate.Terminal.Result result = Neusoft.HISFC.BizProcess.Integrate.Terminal.Result.None;
            if (!ValidState())
            {
                return -1;
            }


            var re = CheckWJJK(this.myPatient.ID);
            if (re < 0)
            {
                MessageBox.Show("收费失败：物价监控系统审核不通过！");
                return -1;
            }

            #region 变量及Trans
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            this.feeManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.confirmIntergrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.orderManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            terminalMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.feeManager.MessageType = messType;

            //{58B76F7C-A35D-4cbb-8948-8163EA3C5191}
            this.dtNow = this.terminalManager.GetDateTimeFromSysDateTime();
            ArrayList alOrder = new ArrayList();
            ArrayList alFeeItemList = new ArrayList();
            ArrayList alNeedExecOrder = new ArrayList();
            ArrayList alCancelOrder = new ArrayList();
            ArrayList alSendInfo = new ArrayList();

            #endregion

            #region 取得需要操作的数据ArrayList

            alOrder = this.GetFeeOrder();
            if (alOrder == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            alFeeItemList = this.GetNewFeeItemList();
            if (alFeeItemList == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }

            alFeeItemList = this.ChangeOrderToFeeItemList(alFeeItemList);
            alNeedExecOrder = this.GetExecOrder();
            if (alNeedExecOrder == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            //获取取消收费的数据
            alCancelOrder = this.GetCancelComfirmExecOrder();
            if (alCancelOrder == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            #endregion

            #region 拆分复合项目{856164A9-000A-482f-B9F4-2A2FF44F96B3}

            Neusoft.HISFC.BizProcess.Integrate.Manager managerPack = new Neusoft.HISFC.BizProcess.Integrate.Manager();
            Neusoft.HISFC.BizProcess.Integrate.Fee tempManagerFee = new Neusoft.HISFC.BizProcess.Integrate.Fee();

            ArrayList alOrderTemp = new ArrayList();
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            for (int i = 0; i < alOrder.Count; i++)
            {
                Neusoft.HISFC.Models.Order.Inpatient.Order oldOrder = alOrder[i] as Neusoft.HISFC.Models.Order.Inpatient.Order;
                Neusoft.HISFC.Models.Fee.Item.Undrug ug = tempManagerFee.GetItem((oldOrder as Neusoft.HISFC.Models.Order.Inpatient.Order).Item.ID);

                if (ug.UnitFlag == "1")
                {
                    //ArrayList al = managerPack.QueryUndrugPackageDetailByCode(oldOrder.Item.ID);
                    ArrayList al = null;
                    if (isUseNewUndrugZT)
                    {
                        if (hsItemZT.ContainsKey(oldOrder.Item.ID))
                        {
                            ArrayList alItem = (ArrayList)hsItemZT[oldOrder.Item.ID];
                            string type = (alItem[0] as Neusoft.FrameWork.Models.NeuObject).User02;
                            if (type == "DR")
                            {
                                al = GetDRItems(oldOrder, !isFindDRFirst, ref hsDROnlyOneItem, ref drCount);
                                isFindDRFirst = true;
                            }
                            else if (type == "CT")
                            {
                                al = GetCTItems(oldOrder, !isFindCTFirst, ref hsCTOnlyOneItem);
                                isFindCTFirst = true;
                            }
                        }
                        else
                        {
                            al = managerPack.QueryUndrugPackageDetailByCode(oldOrder.Item.ID);
                        }
                    }
                    else
                    {
                        al = managerPack.QueryUndrugPackageDetailByCode(oldOrder.Item.ID);
                    }
                    foreach (Neusoft.HISFC.Models.Fee.Item.Undrug undrug in al)
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug tmpUndrug = tempManagerFee.GetItem(undrug.ID);
                        decimal rate = this.GetItemRateForZT(oldOrder.Item.ID, undrug.ID);
                        Neusoft.HISFC.Models.Order.Inpatient.Order myorder = null;
                        decimal qty = oldOrder.Qty;
                        myorder = oldOrder.Clone();
                        myorder.Item = tmpUndrug.Clone();
                        myorder.Item.Price = tmpUndrug.Price * rate;
                        myorder.Item.User01 = oldOrder.Item.User01;
                        myorder.Item.User02 = oldOrder.Item.User02;
                        myorder.Qty = qty * undrug.Qty;//数量==复合项目数量*小项目数量
                        myorder.Item.Qty = qty * undrug.Qty;//数量==小项目数量
                        myorder.Package.ID = oldOrder.Item.ID;//符合项目编码
                        myorder.Package.Name = oldOrder.Item.Name; //符合项目名称
                        myorder.Package.Qty = qty;
                        alOrderTemp.Add(myorder);
                    }
                }
                else
                {
                    alOrderTemp.Add(alOrder[i]);
                }
            }
            bool isFindLast = false;
            for (int i = alOrderTemp.Count - 1; i >= 0; i--)
            {
                Neusoft.HISFC.Models.Order.Inpatient.Order order = alOrderTemp[i] as Neusoft.HISFC.Models.Order.Inpatient.Order;
                if (hsDROnlyOneItem.ContainsKey(order.Item.ID))
                {
                    if (isFindLast == false)
                    {
                        isFindLast = true;
                    }
                    else
                    {
                        alOrderTemp.RemoveAt(i);
                    }
                }
                if (hsCTOnlyOneItem.ContainsKey(order.Item.ID))
                {
                    if (hsCTOnlyOneItem[order.Item.ID].ToString() != "true")
                    {
                        hsCTOnlyOneItem.Remove(order.Item.ID);
                        hsCTOnlyOneItem.Add(order.Item.ID, "true");
                    }
                    else
                    {
                        alOrderTemp.RemoveAt(i);
                    }
                }
            }

            alOrder = alOrderTemp;
            hsFeeStatsCost = new Hashtable();
            decimal totCost = 0;
            try
            {
                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order order in alOrder)
                {
                    string feeStatName = (hsFeeStats[order.Item.MinFee.ID] as Neusoft.HISFC.Models.Fee.FeeCodeStat).StatCate.Name;
                    if (hsFeeStatsCost.ContainsKey(feeStatName))
                    {
                        decimal iCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(hsFeeStatsCost[feeStatName]);
                        hsFeeStatsCost.Remove(feeStatName);
                        hsFeeStatsCost.Add(feeStatName, iCost + order.Item.Price * order.Item.Qty);
                        totCost += order.Item.Price * order.Item.Qty;
                    }
                    else
                    {
                        hsFeeStatsCost.Add(feeStatName, order.Item.Price * order.Item.Qty);
                        totCost += order.Item.Price * order.Item.Qty;
                    }
                }
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList in alFeeItemList)
                {
                    string feeStatName = (hsFeeStats[feeItemList.Item.MinFee.ID] as Neusoft.HISFC.Models.Fee.FeeCodeStat).StatCate.Name;
                    if (hsFeeStatsCost.ContainsKey(feeStatName))
                    {
                        decimal iCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(hsFeeStatsCost[feeStatName]);
                        hsFeeStatsCost.Remove(feeStatName);
                        hsFeeStatsCost.Add(feeStatName, iCost + feeItemList.FT.TotCost);
                        totCost += feeItemList.FT.TotCost;
                    }
                    else
                    {
                        hsFeeStatsCost.Add(feeStatName, feeItemList.FT.TotCost);
                        totCost += feeItemList.FT.TotCost;
                    }
                }
                string PopShowMessage = string.Empty;
                foreach (DictionaryEntry en in hsFeeStatsCost)
                {
                    PopShowMessage += en.Key + ":" + Neusoft.FrameWork.Function.NConvert.ToDecimal(en.Value).ToString("0.00") + "     ";
                }

                PopShowMessage = "住院号：" + this.myPatient.PID.PatientNO + "         姓名：" + this.myPatient.Name + "\n"
                    + "------------------------------------------\n" + "\n合 计：" + totCost.ToString() + "\n" + PopShowMessage + "\n是否确认收费？";
                if ((DialogResult)MessageBox.Show(PopShowMessage, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    return -1;
                }
            }
            catch
            { }

            #endregion

            #region 判断

            if ((alOrder == null || alOrder.Count == 0) && (alFeeItemList == null || alFeeItemList.Count == 0) && (alCancelOrder == null || alCancelOrder.Count == 0))
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("没有选择保存的医嘱数据！"));
                return 100;
            }

            #endregion

            #region 插入收费，并更新可退数量

            if (alOrder != null && alOrder.Count > 0)
            {
                iReturn = this.feeManager.FeeItem(this.myPatient, ref alOrder);
                if (iReturn < 0)
                {
                    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                    feeManager.Rollback();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("收取患者费用失败！" + this.feeManager.Err));
                    return iReturn;
                }

                alSendInfo.AddRange(alOrder);

                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem in alOrder)
                {
                    //{07D1BACB-8E4F-4ac8-8254-81763D0F0699}
                    iReturn = this.feeManager.UpdateConfirmNumForUndrug(feeItem.RecipeNO, feeItem.SequenceNO, feeItem.Item.Qty, feeItem.BalanceState);
                    if (iReturn < 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新费用明细可退数量失败！" + this.feeManager.Err));
                        return -1;
                    }
                    iReturn = this.feeManager.UpdateExtFlagForUndrug(feeItem.RecipeNO, feeItem.SequenceNO, "150", feeItem.BalanceState);
                    if (iReturn < 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新费用明细扩展标记失败！" + this.feeManager.Err));
                        return -1;
                    }
                }

            }
            if (alFeeItemList != null && alFeeItemList.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem in alFeeItemList)
                {
                    if (feeItem.Item.ItemType == EnumItemType.Drug)
                    {
                        #region 特殊科室取药药房
                        try
                        {
                            ArrayList storeDept = this.pharmacyManager.QueryReciveDrugDept(feeItem.FeeOper.Dept.ID, "A");
                            Dictionary<string, List<string>> drugDeptList = new Dictionary<string, List<string>>();
                            if (this.DrugDeptSet.Length > 0)
                            {
                                string[] list = this.DrugDeptSet.Split('#');
                                foreach (string str in list)
                                {
                                    string[] list2 = str.Split('$');
                                    if (list2.Length > 1)
                                    {
                                        string drugDept = list2[0];
                                        List<string> li = new List<string>();
                                        string[] depts = list2[1].Split('&');

                                        foreach (string obj in depts)
                                        {
                                            li.Add(obj);
                                        }

                                        drugDeptList.Add(drugDept, li);

                                    }
                                }
                            }
                            if (drugDeptList.Count > 0)
                            {
                                foreach (string item in drugDeptList.Keys)
                                {
                                    foreach (string obj in drugDeptList[item])
                                    {
                                        if (obj == feeItem.FeeOper.Dept.ID)
                                        {
                                            for (int i = 0; i < storeDept.Count; i++)
                                            {
                                                if ((storeDept[i] as Neusoft.FrameWork.Models.NeuObject).ID == item)
                                                {
                                                    feeItem.StockOper.Dept.ID = item;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { }
                        #endregion

                        //取库存
                        if (string.IsNullOrEmpty(feeItem.StockOper.Dept.ID))
                        {
                            Neusoft.HISFC.Models.Pharmacy.Storage storage =
                           this.pharmacyManager.GetItemForInpatient(feeItem.ExecOper.Dept.ID, feeItem.Item.ID);
                            if (storage == null || storage.Item == null || string.IsNullOrEmpty(storage.Item.ID))
                            {
                                feeManager.Rollback();
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(string.Format("{0}在开立项目{1}时，对应药房不存在此药品！",
                                    feeItem.ExecOper.Dept.Name, feeItem.Item.Name) + this.pharmacyManager.Err);
                                return -1;
                            }
                            feeItem.StockOper.Dept.ID = storage.StockDept.ID;

                        }

                        #region 库存数量-已预扣数量-申请数量 > 0 ??

                        decimal privStoreNum = this.pharmacyManager.GetPreOutNum(feeItem.StockOper.Dept.ID, feeItem.Item.ID);
                        Neusoft.HISFC.Models.Pharmacy.Storage stNum
                            = this.pharmacyManager.GetStockInfoByDrugCode(feeItem.StockOper.Dept.ID, feeItem.Item.ID);
                        if (stNum.StoreQty - privStoreNum - feeItem.Item.Qty < 0)
                        {
                            feeManager.Rollback();
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(feeItem.Item.Name + feeItem.Item.Specs + " 收费数量大于药房有效库存,不能进行收费!\r\n请联系药房补充库存.");
                            return -1;
                        }

                        #endregion
                    }



                }

                iReturn = this.feeManager.FeeItem(this.myPatient, ref alFeeItemList);
                if (iReturn < 0)
                {
                    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                    feeManager.Rollback();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("收取患者费用失败！" + this.feeManager.Err));
                    return iReturn;
                }

                alSendInfo.AddRange(alFeeItemList);

                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem in alFeeItemList)
                {
                    if (feeItem.Item.ItemType == EnumItemType.UnDrug)
                    {
                        //{07D1BACB-8E4F-4ac8-8254-81763D0F0699}
                        iReturn = this.feeManager.UpdateConfirmNumForUndrug(feeItem.RecipeNO, feeItem.SequenceNO, feeItem.Item.Qty, feeItem.BalanceState);
                        if (iReturn < 0)
                        {
                            //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                            feeManager.Rollback();
                            MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新费用明细可退数量失败！" + this.feeManager.Err));
                            return -1;
                        }

                        iReturn = this.feeManager.UpdateExtFlagForUndrug(feeItem.RecipeNO, feeItem.SequenceNO, "150", feeItem.BalanceState);
                        if (iReturn < 0)
                        {
                            //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                            feeManager.Rollback();
                            MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新费用明细扩展标记失败！" + this.feeManager.Err));
                            return -1;
                        }
                    }
                    else if (feeItem.Item.ItemType == EnumItemType.Drug)
                    {
                        if (this.pharmacyManager.ApplyOut(this.myPatient, feeItem, feeItem.FeeOper.OperTime, false) == -1)
                        {
                            feeManager.Rollback();
                            MessageBox.Show(this.pharmacyManager.Err);
                            return -1;
                        }
                    }
                }

            }

            #endregion

            #region 插入终端

            #region 确认完成的数据
            //目前不插入终端，需要时修改本部分代码
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem in alOrder)
            {
                Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail detail = new Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail();

                #region 构建确认明细
                string applySequence = "";
                int sReturn = terminalMgr.GetNextSequence(ref applySequence);
                if (sReturn == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show("获取确认流水号失败");
                    return -1;
                }
                detail.MoOrder = feeItem.Order.ID;//医嘱流水号 0
                detail.ExecMoOrder = feeItem.ExecOrder.ID;//医嘱内执行单流水号1 
                detail.Sequence = applySequence;//2
                detail.Apply.Item.ID = feeItem.Item.ID;//3
                detail.Apply.Item.Name = feeItem.Item.Name;//4
                detail.Apply.Item.ConfirmedQty = feeItem.Item.Qty;//5
                detail.Apply.ConfirmOperEnvironment.ID = this.oper.ID;//6
                detail.Apply.ConfirmOperEnvironment.Dept.ID = this.oper.Dept.ID;//7
                detail.Apply.ConfirmOperEnvironment.OperTime = this.dtNow;//8
                detail.Status.ID = "0";//9 0-正常，1-取消，2-退费
                detail.Apply.Patient.ID = feeItem.Patient.ID;
                detail.Apply.Item.RecipeNO = feeItem.RecipeNO;
                detail.Apply.Item.SequenceNO = feeItem.SequenceNO;
                detail.ExecDevice = feeItem.Item.User01;
                detail.Oper.ID = feeItem.Item.User02;

                #endregion
                if (terminalMgr.InsertInpatientConfirmDetail(detail) == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("插入终端确认明细失败！" + this.terminalMgr.Err));
                    return -1;
                }
            }
            #endregion

            #region 不收费的数据

            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem in alFeeItemList)
            {
                Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail detail = new Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail();

                #region 构建确认明细
                string applySequence = "";
                int sReturn = terminalMgr.GetNextSequence(ref applySequence);
                if (sReturn == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show("获取确认流水号失败");
                    return -1;
                }
                detail.MoOrder = feeItem.Order.ID;//医嘱流水号 0
                detail.ExecMoOrder = feeItem.ExecOrder.ID;//医嘱内执行单流水号1 
                detail.Sequence = applySequence;//2
                detail.Apply.Item.ID = feeItem.Item.ID;//3
                detail.Apply.Item.Name = feeItem.Item.Name;//4
                detail.Apply.Item.ConfirmedQty = feeItem.Item.Qty;//5
                detail.Apply.ConfirmOperEnvironment.ID = this.oper.ID;//6
                detail.Apply.ConfirmOperEnvironment.Dept.ID = this.oper.Dept.ID;//7
                detail.Apply.ConfirmOperEnvironment.OperTime = this.dtNow;//8
                //---------------------------------------------------------------------------------
                detail.Status.ID = "0";//9 0-正常，1-取消，2-退费
                detail.Apply.Patient.ID = feeItem.Patient.ID;
                detail.Apply.Item.RecipeNO = feeItem.RecipeNO;
                detail.Apply.Item.SequenceNO = feeItem.SequenceNO;
                detail.ExecDevice = feeItem.Item.User01;
                detail.Oper.ID = feeItem.Item.User02;
                #endregion
                if (terminalMgr.InsertInpatientConfirmDetail(detail) == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("插入终端确认明细失败！" + this.terminalMgr.Err));
                    return -1;
                }
            }

            #endregion

            #region 确认取消的医嘱

            foreach (Neusoft.HISFC.Models.Order.ExecOrder execOrder in alCancelOrder)
            {
                Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail detail = new Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail();

                #region 构建确认明细
                string applySequence = "";
                int sReturn = terminalMgr.GetNextSequence(ref applySequence);
                if (sReturn == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show("获取确认流水号失败");
                    return -1;
                }
                detail.MoOrder = execOrder.Order.ID;//医嘱流水号 0
                detail.ExecMoOrder = execOrder.ID;//医嘱内执行单流水号1 
                detail.Sequence = applySequence;//2
                detail.Apply.Item.ID = execOrder.Order.Item.ID;//3
                detail.Apply.Item.Name = execOrder.Order.Item.Name;//4
                detail.Apply.Item.ConfirmedQty = execOrder.Order.Item.Qty;//5
                detail.Apply.ConfirmOperEnvironment.ID = this.oper.ID;//6
                detail.Apply.ConfirmOperEnvironment.Dept.ID = this.oper.Dept.ID;//7
                detail.Apply.ConfirmOperEnvironment.OperTime = this.dtNow;//8
                //---------------------------------------------------------------------------------
                detail.Status.ID = "1";//9 0-正常，1-取消，2-退费
                detail.Apply.Patient.ID = execOrder.Order.Patient.ID;
                detail.Apply.Item.RecipeNO = "";
                detail.Apply.Item.SequenceNO = 0;
                detail.ExecDevice = execOrder.Order.Item.User01;
                detail.Oper.ID = execOrder.Order.Item.User02;
                #endregion
                if (terminalMgr.InsertInpatientConfirmDetail(detail) == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("插入终端确认明细失败！" + this.terminalMgr.Err));
                    return -1;
                }
            }

            #endregion

            #endregion

            #region 更新医嘱确认和收费

            #region 确认完成的医嘱

            foreach (Neusoft.HISFC.Models.Order.ExecOrder execOrder in alNeedExecOrder)
            {
                execOrder.ExecOper.OperTime = dtNow;
                execOrder.ChargeOper.OperTime = dtNow;

                if (!execOrder.IsExec)
                {
                    execOrder.IsExec = true;
                    execOrder.IsConfirm = true;
                    iReturn = this.orderManager.UpdateRecordExec(execOrder);
                    if (iReturn < 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新医嘱确认信息失败！" + this.orderManager.Err));
                        return iReturn;
                    }
                    else if (iReturn == 0)
                    {
                        feeManager.Rollback();
                        MessageBox.Show(this, "更新医嘱确认信息失败！【" + execOrder.Order.Name + "】已经确认，请刷新！", "提示>>", MessageBoxButtons.OK);
                        return iReturn;
                    }
                    iReturn = this.orderManager.UpdateOrderStatus(execOrder.Order.ID, 2);
                    if (iReturn < 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新医嘱主档执行信息失败！" + this.orderManager.Err));
                        return iReturn;
                    }
                }

                if (!execOrder.IsCharge)
                {
                    execOrder.IsCharge = true;
                    execOrder.IsValid = true;
                    iReturn = this.orderManager.UpdateChargeExec(execOrder);
                    if (iReturn <= 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新医嘱收费信息失败！" + this.orderManager.Err));
                        return iReturn;
                    }
                }

            }
            #endregion

            #region 确认取消的医嘱

            foreach (Neusoft.HISFC.Models.Order.ExecOrder execOrder in alCancelOrder)
            {
                execOrder.ExecOper.OperTime = dtNow;
                execOrder.ChargeOper.OperTime = dtNow;

                if (!execOrder.IsExec)
                {
                    execOrder.IsExec = true;
                    execOrder.IsConfirm = true;
                    iReturn = this.orderManager.UpdateRecordExec(execOrder);
                    if (iReturn < 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新医嘱确认信息失败！" + this.orderManager.Err));
                        return iReturn;
                    }
                    else if (iReturn == 0)
                    {
                        feeManager.Rollback();
                        MessageBox.Show(this, "更新医嘱确认信息失败！【" + execOrder.Order.Name + "】已经确认，请刷新！", "提示>>", MessageBoxButtons.OK);
                        return iReturn;
                    }

                    iReturn = this.orderManager.UpdateOrderStatus(execOrder.Order.ID, 2);
                    if (iReturn < 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新医嘱主档执行信息失败！" + this.orderManager.Err));
                        return iReturn;
                    }
                }

                if (!execOrder.IsCharge)
                {
                    execOrder.IsValid = false;
                    iReturn = this.orderManager.UpdateChargeExec(execOrder);
                    if (iReturn <= 0)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //{3EE6172A-301B-4d16-91C7-E5D8AC94D942}
                        feeManager.Rollback();
                        MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新医嘱收费信息失败！" + this.orderManager.Err));
                        return iReturn;
                    }
                }

            }

            #endregion

            #endregion

            //发送消息
            #region HL7消息发送
            object curInterfaceImplement = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(Neusoft.HISFC.BizProcess.Integrate.Fee), typeof(Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder));
            if (curInterfaceImplement is Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder)
            {
                Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder curIOrderControl = curInterfaceImplement as Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder;

                int param = curIOrderControl.SendDrugInfo(myPatient, alSendInfo, true);
                if (param == -1)
                {
                    feeManager.Rollback();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("发送消息失败！" + curIOrderControl.Err));
                    return -1;
                }
            }

            #endregion

            feeManager.Commit();
            // Neusoft.FrameWork.Management.PublicTrans.Commit();


            MessageBox.Show(this, "确认成功！", "提示>>", MessageBoxButtons.OK);

            this.fpExecOrder.Sheets[0].RowCount = 0;
            this.hsRows.Clear();
            if (this.oper.Dept.ID != "5018")
            {
                ((tvInpatientConfirm)this.tv).Refresh();
            }
            this.Clear();
            this.ucQueryInpatientNo1.Text = string.Empty;
            this.ucQueryInpatientNo1.Focus();
            this.ucQueryInpatientNo1.Select();
            ((tvInpatientConfirm)this.tv).SelectedNode = null;

            //this.OnSetValue(this.myPatient, null);
            //for (int i = this.fpExecOrder.Sheets[0].RowCount - 1; i >= 0; i--)
            //{

            //    if (Neusoft.FrameWork.Function.NConvert.ToInt32(this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value)==(int)EnumCheckState.ComfirmComplete)
            //    {
            //        if (clearConfirm)
            //        {
            //            this.fpExecOrder.Sheets[0].Rows.Remove(i, 1);
            //        }
            //        else
            //        {
            //            this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value = (int)EnumCheckState.ComfirmComplete;
            //            this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Locked = true;
            //            this.fpExecOrder_Sheet1.Rows[i].BackColor = System.Drawing.Color.Azure;
            //        }
            //    }
            //}

            #region 电子申请单 {6FAEEEC2-CF03-4b2e-B73F-92C1C8CAE1C0} 接入电子申请单 yangw 20100504

            if (!string.IsNullOrEmpty(isUseDL) && isUseDL == "1")
            {
                if (PACSApplyInterface == null)
                {
                    PACSApplyInterface = new Neusoft.ApplyInterface.HisInterface();
                }
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f in alOrder)
                {
                    if (f.Item.SysClass.ID.ToString() == "UC" && f.Order.ID != null)
                    {
                        try
                        {
                            string applyNo = string.Empty;
                            terminalMgr.GetApplyNoByOrderNo(f.Order.ID, ref applyNo);
                            Neusoft.HISFC.Models.Order.Inpatient.Order order = orderManager.QueryOneOrder(f.Order.ID);
                            applyNo = order.ApplyNo;
                            int a = PACSApplyInterface.Charge(applyNo, "1");
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("更新电子申请单收费标志时出错：\n" + e.Message);
                        }
                    }
                }
            }
            #endregion
            #region  打印
            if (this.IsPrint)
            {
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList obj in alOrder)
                {
                    if (OnPrint(new object(), obj) == -1)
                    {
                        return -1;
                    }
                }
            }
            #endregion
            return 0;
        }

        private int GetNeedFeeItemList(ref List<FeeItemList> list)
        {
            ArrayList alFeeItemList = new ArrayList();

            alFeeItemList = this.GetNewFeeItemList();
            if (alFeeItemList == null)
            {
               
                return -1;
            }

            alFeeItemList = this.ChangeOrderToFeeItemList(alFeeItemList);

            list = alFeeItemList.Cast<FeeItemList>().Where(a => a.Item.ItemType == EnumItemType.UnDrug).ToList();
            return 1;
        }

        /// <summary>
        /// 院内物价监控系统
        /// </summary>
        /// <returns></returns>
        private int CheckWJJK(string inPatientNo)
        {

            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("物价监控中...", 0, false);
                Application.DoEvents();
                var priceService = new PriceMonitorService();
                var responseModel = new ResponseModel();

                List<FeeItemList> list = new List<FeeItemList>();
                var aa = GetNeedFeeItemList(ref list);
                if (aa < 0)
                {
                    MessageBox.Show("物价监控数据费用转化失败！");
                    return -1;
                }

                var priceResult = priceService.ZYCourseAnalyze(inPatientNo, DateTime.Now, list, ref responseModel);

                if (priceResult == 2) //代表开关关闭 不调用接口
                {
                    return 1;
                }

                if (priceResult == -1)
                {
                    MessageBox.Show(priceService.ErrMsg);
                    return -1;
                }
                if (responseModel.Data == null || !responseModel.Data.Any())
                {
                    return 1;
                }

                frmPreAlert f = new frmPreAlert();
                f.SysType = "0";
                List<BeforeAnalyzeDetail> anList = new List<BeforeAnalyzeDetail>();
                BeforeAnalyzeDetail a;
                foreach (var item in responseModel.Data)
                {
                    a = new BeforeAnalyzeDetail();
                    switch (item.IllegalLevel)
                    {
                        case "30":
                            a.SevDeg = "明确违规";
                            break;
                        case "21":
                            a.SevDeg = "高度可疑";
                            break;
                        case "20":
                            a.SevDeg = "疑似违规";
                            break;
                        default:
                            a.SevDeg = "未知等级";
                            break;
                    }

                    a.RuleName = item.RuleName;
                    a.VolaAmt = Convert.ToDecimal(item.IllegalCost);
                    a.VolaCont = item.Message;


                    anList.Add(a);
                }

                f.LoadData(anList);
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                f.ShowDialog();
                if (f.DialogResult != DialogResult.Yes)
                {
                    return -1;
                }

                return 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show("调用物价监控系统接口异常：" + ex.Message);
                return -1;
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }

        }

        /// <summary>
        /// 获取复合项目明细
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isFirst"></param>
        /// <param name="hsItem"></param>
        /// <param name="drCount"></param>
        /// <returns></returns>
        private ArrayList GetDRItems(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool isFirst, ref Hashtable hsItem, ref decimal drCount)
        {
            ArrayList alItem = new ArrayList();
            ArrayList al = mgr.QueryUndrugZTBypackageCode(order.Item.ID);

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in al)
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
                    if (undrugCombo.Name.ToUpper().Contains("DR"))
                    {
                        drCount += undrugCombo.Qty;
                    }
                }
            }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in al)
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
                if (undrugCombo.SpellCode == "0")
                {
                    //总量取整的，做标识
                    if (hsItem.ContainsKey(undrugCombo.ID))
                    {
                        Neusoft.HISFC.Models.Fee.Item.UndrugComb temp = hsItem[undrugCombo.ID] as Neusoft.HISFC.Models.Fee.Item.UndrugComb;
                        undrugCombo.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        temp = undrugCombo;
                    }
                    else
                    {
                        undrugCombo.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        hsItem.Add(undrugCombo.ID, undrugCombo);
                    }
                }
                alItem.Add(undrugCombo);
            }
            return alItem;
        }

        private ArrayList GetCTItems(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool isFirst, ref Hashtable hsItem)
        {
            ArrayList alItem = new ArrayList();
            ArrayList al = mgr.QueryUndrugZTBypackageCode(order.Item.ID);
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in al)
            {
                alItem.Add(undrugCombo);
                if (undrugCombo.SortID == 3)
                {
                    if (hsItem.ContainsKey(undrugCombo.ID))
                    {
                        continue;
                    }
                    else
                    {
                        string itemName = undrugCombo.Name;
                        if (itemName.Contains("三维重建"))
                        {
                            if (!hsItem.ContainsValue("四维"))
                            {
                                hsItem.Add(undrugCombo.ID, "三维");
                            }
                            else
                            {
                                hsItem.Add(undrugCombo.ID, "true");
                            }
                        }
                        else if (itemName.Contains("四维重建"))
                        {
                            Hashtable hsTemp = hsItem.Clone() as Hashtable;
                            foreach (DictionaryEntry de in hsTemp)
                            {
                                if (de.Value.ToString() == "三维")
                                {
                                    hsItem.Remove(de.Key);
                                    hsItem.Add(de.Key.ToString(), "true");
                                }
                            }
                            hsItem.Add(undrugCombo.ID, "四维");
                        }
                        else
                        {
                            hsItem.Add(undrugCombo.ID, "其他");
                        }
                    }
                }
            }
            return alItem;
        }

        Hashtable hsUndrugZTDetail = null;
        /// <summary>
        /// 获取非药品明细价格的加成（优惠）比例
        /// </summary>
        /// <param name="ztCode"></param>
        /// <param name="detailCode"></param>
        /// <returns></returns>
        public decimal GetItemRateForZT(string ztCode, string detailCode)
        {
            //feeIntegrate.GetItemRateForZT(detailCode, ztCode);
            if (hsUndrugZTDetail == null)
            {
                hsUndrugZTDetail = new Hashtable();

            }
            List<Neusoft.HISFC.Models.Fee.Item.UndrugComb> ztList = new List<Neusoft.HISFC.Models.Fee.Item.UndrugComb>();
            Neusoft.HISFC.BizLogic.Manager.UndrugztManager undrugZTMgr = new Neusoft.HISFC.BizLogic.Manager.UndrugztManager();

            if (!hsUndrugZTDetail.ContainsKey(ztCode))
            {
                undrugZTMgr.QueryUnDrugztDetail(ztCode, ref ztList);
                hsUndrugZTDetail.Add(ztCode, ztList);
            }

            List<Neusoft.HISFC.Models.Fee.Item.UndrugComb> listZT = hsUndrugZTDetail[ztCode] as List<Neusoft.HISFC.Models.Fee.Item.UndrugComb>;

            if (listZT != null)
            {
                foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugComb in listZT)
                {
                    if (undrugComb.ID == detailCode)
                    {
                        return undrugComb.ItemRate;
                    }
                }
            }

            return 1;
        }

        /// <summary>
        /// 显示检查申请单
        /// </summary>
        private void ShowPacsApply(int rowIndex)
        {
            if (!string.IsNullOrEmpty(isUseDL) && isUseDL == "1")
            {
                #region {5E5299D8-95A2-4498-B2F1-52D00E4FB11A} UpdateApply需要使用Neusoft.HISFC.Components.PacsApply.HisInterface,以后需要电子申请单重构到Neusoft.ApplyInterface.HisInterface中
                //if (PACSApplyInterface == null)
                //{
                //    PACSApplyInterface = new Neusoft.ApplyInterface.HisInterface();
                //}
                //int rowIndex = this.fpExecOrder.Sheets[0].ActiveRowIndex;
                if (rowIndex == -1)
                {
                    return;
                }
                if (PACSApplyInterfaceNew == null)
                {
                    PACSApplyInterfaceNew = new Neusoft.HISFC.Components.PacsApply.HisInterface(Neusoft.FrameWork.Management.Connection.Operator.ID, (Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID);
                }
                #endregion

                if (this.fpExecOrder.Sheets[0].Cells[rowIndex, (int)Cols.OrderType].Text == "ORDER")
                {
                    Neusoft.HISFC.Models.Order.ExecOrder exeOrder = new Neusoft.HISFC.Models.Order.ExecOrder(); ;
                    exeOrder = (Neusoft.HISFC.Models.Order.ExecOrder)this.fpExecOrder.Sheets[0].Rows[rowIndex].Tag;
                    Neusoft.HISFC.Models.Order.Inpatient.Order order = orderManager.QueryOneOrder(exeOrder.Order.ID);
                    if (order == null || order.Item.SysClass.ID.ToString() != "UC")
                        return;
                    if (!string.IsNullOrEmpty(order.ApplyNo))
                    {
                        #region {5E5299D8-95A2-4498-B2F1-52D00E4FB11A}
                        //if (PACSApplyInterface.UpdateApply(order.ApplyNo) < 0)
                        if (PACSApplyInterfaceNew.UpdateApply(order.ApplyNo, false) < 0)
                        #endregion
                        {
                            MessageBox.Show("查询电子申请单失败！");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算合计金额
        /// </summary>
        private void ShowTotFee()
        {
            decimal totFee = 0;
            for (int i = 0; i < this.fpExecOrder_Sheet1.RowCount; i++)
            {
                if (this.fpExecOrder_Sheet1.Cells[i, (int)Cols.IsExec].Value != null && this.fpExecOrder_Sheet1.Cells[i, (int)Cols.IsExec].Value.ToString() == "0")
                {
                    if (this.fpExecOrder_Sheet1.Cells[i, (int)Cols.TotCost].Value != null)
                    {
                        totFee = decimal.Parse(this.fpExecOrder_Sheet1.Cells[i, (int)Cols.TotCost].Value.ToString()) + totFee;
                    }
                }
            }
            this.lblTotCost.Text = "金额：" + totFee.ToString();
        }

        public static void DrawComboLeft(object sender, int column, int DrawColumn, int ChildViewLevel)
        {
            switch (sender.GetType().ToString().Substring(sender.GetType().ToString().LastIndexOf(".") + 1))
            {
                case "SheetView":
                    FarPoint.Win.Spread.SheetView o = sender as FarPoint.Win.Spread.SheetView;
                    int i = 0;
                    string tmp = "", curComboNo = "";
                    if (ChildViewLevel == 0)
                    {
                        for (i = 0; i < o.RowCount; i++)
                        {
                            #region "画"
                            if (o.Cells[i, column].Text == "0") o.Cells[i, column].Text = "";
                            tmp = o.Cells[i, column].Text + "";
                            o.Cells[i, column].Tag = tmp;
                            if (curComboNo != tmp && tmp != "") //是头
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
                                        o.Cells[i - 1, DrawColumn].Text = "";
                                    }
                                }
                                catch { }
                            }
                            else if (curComboNo == tmp && tmp != "")
                            {
                                o.Cells[i, DrawColumn].Text = "┃";
                            }
                            else if (curComboNo != tmp && tmp == "")
                            {
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┛";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┓")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "";
                                    }
                                }
                                catch { }
                                o.Cells[i, DrawColumn].Text = "";
                                curComboNo = "";
                            }
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┃") o.Cells[i, DrawColumn].Text = "┛";
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┓") o.Cells[i, DrawColumn].Text = "";
                            o.Cells[i, DrawColumn].ForeColor = Color.Red;
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
                                if (c.Cells[j, column].Text == "0") c.Cells[j, column].Text = "";
                                tmp = c.Cells[j, column].Text + "";

                                c.Cells[j, column].Tag = tmp;
                                if (curComboNo != tmp && tmp != "") //是头
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
                                            c.Cells[j - 1, DrawColumn].Text = "";
                                        }
                                    }
                                    catch { }
                                }
                                else if (curComboNo == tmp && tmp != "")
                                {
                                    c.Cells[j, DrawColumn].Text = "┃";
                                }
                                else if (curComboNo != tmp && tmp == "")
                                {
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┛";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┓")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "";
                                        }
                                    }
                                    catch { }
                                    c.Cells[j, DrawColumn].Text = "";
                                    curComboNo = "";
                                }
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┃") c.Cells[j, DrawColumn].Text = "┛";
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┓") c.Cells[j, DrawColumn].Text = "";
                                //c.Cells[j, DrawColumn].ForeColor = System.Drawing.Color.Black;
                                #endregion

                            }
                        }
                    }
                    break;
            }

        }

        public override void Refresh()
        {
            //Neusoft.HISFC.Models.RADT.PatientInfo temp = this.myPatient.Clone();

            DateTime now = this.terminalMgr.GetDateTimeFromSysDateTime();
            DateTime beginTime = DateTime.MinValue;
            if ((now - DateTime.MinValue).Days > this.showdays)
            {
                beginTime = now.AddDays(-this.showdays);
            }
            ((tvInpatientConfirm)this.tv).Init(beginTime, now);
            RefleshDeptList();
            InitDoctors();
            this.ShowTotFee();
            //this.OnSetValue(temp, null);
            Application.DoEvents();
            base.Refresh();
        }
        #endregion

        #region 事件


        void ucQueryInpatientNo1_myEvent()
        {
            //清空
            this.Clear();

            //判断是否有该患者
            if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo.Trim() == "")
            {
                if (this.ucQueryInpatientNo1.Err == "")
                {
                    ucQueryInpatientNo1.Err = "此患者不在院!";
                }
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.ucQueryInpatientNo1.Err, 211);

                this.ucQueryInpatientNo1.Focus();
                return;
            }
            //获取住院号赋值给实体
            Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = this.radtIntegrate.GetPatientInfomation(this.ucQueryInpatientNo1.InpatientNo);

            if (patientInfo == null)
            {
                MessageBox.Show(this.radtIntegrate.Err);
                return;
            };

            ((tvInpatientConfirm)this.tv).SelectedNode = ((tvInpatientConfirm)this.tv).FindDefaultNode(patientInfo);

            this.OnSetValue(patientInfo, null);
        }

        void ckComplete_CheckedChanged(object sender, EventArgs e)
        {
            if (this.fpExecOrder_Sheet1.RowCount > 0)
            {
                if (this.ckComplete.Checked)
                {
                    this.fpExecOrder_Sheet1.Cells[0, 0, this.fpExecOrder_Sheet1.RowCount - 1, 0].Value = (int)EnumCheckState.ComfirmComplete;
                }
                else if (this.ckCancel.Checked)
                {
                    this.fpExecOrder_Sheet1.Cells[0, 0, this.fpExecOrder_Sheet1.RowCount - 1, 0].Value = (int)EnumCheckState.ComfirmCancel;
                }
                else if (this.ckNone.Checked)
                {
                    this.fpExecOrder_Sheet1.Cells[0, 0, this.fpExecOrder_Sheet1.RowCount - 1, 0].Value = (int)EnumCheckState.None;
                }
                this.ShowTotFee();
            }
        }

        /// <summary>
        /// 选择项目列表数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int ucItemList_SelectItem(Keys key)
        {
            return this.InsertItem();

        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override Neusoft.FrameWork.WinForms.Forms.ToolBarService Init(object sender, object neuObject, object param)
        {
            base.Init(sender, neuObject, param);

            this.toolBarService.AddToolButton("新增", "补收项目的划价信息", Neusoft.FrameWork.WinForms.Classes.EnumImageList.T添加, true, false, null);
            this.toolBarService.AddToolButton("刷新", "刷新患者信息", Neusoft.FrameWork.WinForms.Classes.EnumImageList.S刷新, true, false, null);
            #region {53061BA2-33FC-469d-9773-9A9454023612}
            this.toolBarService.AddToolButton("删除", "删除新增的补收项目的划价信息", Neusoft.FrameWork.WinForms.Classes.EnumImageList.S删除, true, false, null);
            #endregion

            //add by allan 20160803
            toolBarService.AddToolButton("身份证", "身份证", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.M模版, true, false, null);
            toolBarService.AddToolButton("读卡", "", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.S手动录入, true, false, null);

            //end add
            //by yuyun 08-7-8{C46CFCDE-132B-47c0-ADAC-4AD73DA2FD90}
            //this.toolBarService.AddToolButton("开立处方", "开立处方", Neusoft.FrameWork.WinForms.Classes.EnumImageList.D医嘱, true, false, null);
            //by yuyun 08-7-8
            return toolBarService;
        }

        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "新增":
                    if (this.seeAll)
                    {
                        MessageBox.Show("查询全院确认信息时 不能新增收费项目", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    this.AddNewRow();
                    break;
                #region {53061BA2-33FC-469d-9773-9A9454023612}
                case "删除":
                    DeleteItemEvent();
                    break;
                #endregion
                case "刷新":
                    this.Refresh();
                    break;
                case "身份证":
                    ucTerminalConfirm_ReadIdCardEvent();
                    break;
                case "读卡":
                    ucTerminalConfirm_ReadMCardEvent();
                    break;
            }
            base.ToolStrip_ItemClicked(sender, e);
        }


        #region =======读取医保卡--allan 160803=========
        /// <summary>
        /// 读取医保卡  add by allan 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucTerminalConfirm_ReadMCardEvent()
        {
            Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy MedcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();
            Neusoft.HISFC.Models.RADT.PatientInfo patient = new Neusoft.HISFC.Models.RADT.PatientInfo();

            int returnValue = MedcareInterfaceProxy.SetPactCode("13");
            if (returnValue != 1)
            {
                MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                // {DBCB798D-2F21-449e-BBE7-8F95E0F08B8A}
                if (MedcareInterfaceProxy.Rollback() < 0)
                {
                    MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                    return;
                }
            }

            returnValue = (int)MedcareInterfaceProxy.Connect();
            if (returnValue != 1)
            {
                MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                // {DBCB798D-2F21-449e-BBE7-8F95E0F08B8A}
                if (MedcareInterfaceProxy.Rollback() < 0)
                {
                    MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                    return;
                }
                return;
            }

            returnValue = MedcareInterfaceProxy.GetRegInfoInpatient(patient);
            if (returnValue != 1)
            {
                // MessageBox.Show(this.MedcareInterfaceProxy.ErrMsg);
                // {DBCB798D-2F21-449e-BBE7-8F95E0F08B8A}
                if (MedcareInterfaceProxy.Rollback() < 0)
                {
                    MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                    return;
                }
                return;
            }
            returnValue = (int)MedcareInterfaceProxy.Disconnect();
            if (returnValue != 1)
            {
                MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                // {DBCB798D-2F21-449e-BBE7-8F95E0F08B8A}
                if (MedcareInterfaceProxy.Rollback() < 0)
                {
                    MessageBox.Show(MedcareInterfaceProxy.ErrMsg);
                }
            }
            Neusoft.HISFC.BizProcess.Integrate.RADT patientMgr = new Neusoft.HISFC.BizProcess.Integrate.RADT();
            ArrayList alPatient = patientMgr.QueryComPatientInfoListByIDNO(patient.IDCard);
            if (alPatient != null && alPatient.Count > 0)
            {
                Neusoft.HISFC.Models.RADT.PatientInfo patientinfo = null;
                foreach (Neusoft.HISFC.Models.RADT.PatientInfo comPatient in alPatient)
                {
                    ArrayList alPatientInfo = patientMgr.QureyPatientInfobyCardno(comPatient.PID.CardNO);
                    if (alPatientInfo != null && alPatientInfo.Count > 0)
                    {
                        patientinfo = alPatientInfo[0] as Neusoft.HISFC.Models.RADT.PatientInfo;
                        break;
                    }

                }
                if (patientinfo == null || string.IsNullOrEmpty(patientinfo.PID.PatientNO))
                {
                    MessageBox.Show("没有此患者信息");
                }
                else
                {
                    ucQueryInpatientNo1.txtInputCode.Text = patientinfo.PID.PatientNO;
                    ucQueryInpatientNo1_myEvent();
                }

            }
            else
            {
                MessageBox.Show("没有此患者信息");
            }
        }
        #endregion

        #region =======读取身份证--allan 160803=====================
        /// <summary>
        /// 读取身份证 add by allan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucTerminalConfirm_ReadIdCardEvent()
        {
            Neusoft.HISFC.BizProcess.Interface.Account.IReadIDCard readIDCard = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.Account.IReadIDCard)) as Neusoft.HISFC.BizProcess.Interface.Account.IReadIDCard;
            //需要测试

            if (readIDCard == null)
            {
                MessageBox.Show("读身份证接口没有实现");
                return;
            }
            string code = "", name = "", sex = "", nation = "", agent = "", add = "", message = "";
            DateTime birth = DateTime.MinValue, bigen = DateTime.MinValue, end = DateTime.MinValue;
            string photoFileName = "";
            int rtn = readIDCard.GetIDInfo(ref code, ref name, ref sex, ref birth, ref nation, ref add, ref agent, ref bigen, ref end, ref photoFileName, ref message);
            if (rtn == -1)
            {
                MessageBox.Show("读卡失败，" + message);
            }
            else if (rtn == 0)
            {
                MessageBox.Show("读卡失败，" + message);
            }
            else
            {
                Neusoft.HISFC.BizProcess.Integrate.RADT patientMgr = new Neusoft.HISFC.BizProcess.Integrate.RADT();
                ArrayList alPatient = patientMgr.QueryComPatientInfoListByIDNO(code);
                if (alPatient.Count > 0)
                {
                    Neusoft.HISFC.Models.RADT.PatientInfo patientinfo = alPatient[0] as Neusoft.HISFC.Models.RADT.PatientInfo;
                    ucQueryInpatientNo1.txtInputCode.Text = patientinfo.PID.PatientNO;
                    ucQueryInpatientNo1_myEvent();
                }
                else
                {
                    MessageBox.Show("没有此患者信息");
                }
            }
        }
        #endregion


        void fpExecOrder_ButtonClicked(object sender, EditorNotifyEventArgs e)
        {
            string CombNo = this.fpExecOrder.Sheets[0].Cells[e.Row, (int)Cols.CombNo].Text.ToString();
            string UseDate = this.fpExecOrder.Sheets[0].Cells[e.Row, (int)Cols.UseDate].Text.ToString();
            if (e.Column == (int)Cols.IsExec)
            {
                if (e.EditingControl is FarPoint.Win.FpMultiOption)
                {
                    switch (((FarPoint.Win.FpMultiOption)e.EditingControl).Value)
                    {
                        case (int)EnumCheckState.ComfirmComplete:
                            ((FarPoint.Win.FpMultiOption)e.EditingControl).ForeColor = Color.Blue;
                            this.fpExecOrder_Sheet1.Cells[e.Row, e.Column].ForeColor = Color.Blue;

                            for (int i = 0; i < this.fpExecOrder.Sheets[0].RowCount; i++)
                            {
                                if (CombNo == this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.CombNo].Text.ToString() && UseDate == this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.UseDate].Text.ToString())
                                {
                                    this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value = (int)EnumCheckState.ComfirmComplete;
                                    this.fpExecOrder_Sheet1.Cells[i, (int)Cols.IsExec].ForeColor = Color.Blue;
                                }

                            }
                            this.ShowTotFee();
                            break;
                        case (int)EnumCheckState.ComfirmCancel:
                            ((FarPoint.Win.FpMultiOption)e.EditingControl).ForeColor = Color.Red;
                            this.fpExecOrder_Sheet1.Cells[e.Row, e.Column].ForeColor = Color.Red;

                            for (int i = 0; i < this.fpExecOrder.Sheets[0].RowCount; i++)
                            {
                                if (CombNo == this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.CombNo].Text.ToString() && UseDate == this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.UseDate].Text.ToString())
                                {
                                    this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value = (int)EnumCheckState.ComfirmCancel;
                                    this.fpExecOrder_Sheet1.Cells[i, (int)Cols.IsExec].ForeColor = Color.Red;
                                }

                            }
                            this.ShowTotFee();
                            break;
                        case (int)EnumCheckState.None:
                            ((FarPoint.Win.FpMultiOption)e.EditingControl).ForeColor = Color.Black;
                            this.fpExecOrder_Sheet1.Cells[e.Row, e.Column].ForeColor = Color.Black;

                            for (int i = 0; i < this.fpExecOrder.Sheets[0].RowCount; i++)
                            {
                                if (CombNo == this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.CombNo].Text.ToString() && UseDate == this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.UseDate].Text.ToString())
                                {
                                    this.fpExecOrder.Sheets[0].Cells[i, (int)Cols.IsExec].Value = (int)EnumCheckState.None;
                                    this.fpExecOrder_Sheet1.Cells[i, (int)Cols.IsExec].ForeColor = Color.Black;
                                }

                            }
                            this.ShowTotFee();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //by yuyun 08-7-7{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
        private int fpExecOrder_SetItem(Neusoft.FrameWork.Models.NeuObject obj)
        {
            this.fpExecOrder_Sheet1.Cells[this.fpExecOrder_Sheet1.ActiveRowIndex, this.fpExecOrder_Sheet1.ActiveColumnIndex].Text = obj.Name;
            this.fpExecOrder_Sheet1.Cells[this.fpExecOrder_Sheet1.ActiveRowIndex, this.fpExecOrder_Sheet1.ActiveColumnIndex].Tag = obj;
            return 0;
        }

        /// <summary>
        /// 表格EditChange
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpExecOrder_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            string source = this.fpExecOrder.Sheets[0].Cells[e.Row, (int)Cols.OrderType].Text.ToString();
            if (source == "NEW")//e.Row == this.currentRow &&
            {
                if (e.Column == (int)Cols.ItemName || e.Column == (int)Cols.ItemCode)
                {
                    System.Windows.Forms.Control cellControl = this.fpExecOrder.EditingControl;
                    //设置位置
                    this.ucItemList.Location = new System.Drawing.Point(cellControl.Location.X, cellControl.Location.Y + cellControl.Height + 80);
                    ucItemList.BringToFront();
                    // 过滤项目
                    this.ucItemList.Filter(this.fpExecOrder_Sheet1.ActiveCell.Text);
                    this.ucItemList.Visible = true;
                    // 保存当前行，用于保证移动上下箭头不改变当前记录
                    this.fpExecOrder_Sheet1.ActiveRowIndex = e.Row;
                    this.currentRow = e.Row;
                }
                else
                {
                    this.UnDisplayUcItemList();
                }
            }


        }

        /// <summary>
        /// 表格回车
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int fpExecOrder_KeyEnter(Keys key)
        {
            if (key == Keys.Up)
            {
                if (this.ucItemList.Visible)
                {

                    this.ucItemList.PriorRow();
                    this.fpExecOrder_Sheet1.ActiveRowIndex = this.currentRow;
                }
                return 0;
            }
            if (key == Keys.Escape)
            {
                if (this.ucItemList.Visible)
                {

                    this.ucItemList.Visible = false;
                }
                return 0;
            }
            if (key == Keys.Down)
            {
                if (this.ucItemList.Visible)
                {
                    this.ucItemList.NextRow();
                    this.fpExecOrder_Sheet1.ActiveRowIndex = this.currentRow;
                }
                return 0;
            }

            if (key == Keys.Enter)
            {
                if (this.fpExecOrder.Sheets[0].ActiveColumnIndex == (int)Cols.ItemName)
                {
                    if (this.InsertItem() > 0)
                    {
                        this.fpExecOrder_Sheet1.ActiveColumnIndex = (int)Cols.ItemConfirmQty;
                        return 0;
                    }
                }
                #region 更改确认数量
                if (this.fpExecOrder.Sheets[0].ActiveColumnIndex == (int)Cols.ItemConfirmQty)
                {
                    decimal price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Price].Text);
                    decimal qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemConfirmQty].Text);
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.TotCost].Text = Convert.ToString(price * qty);

                    if (price == 0)//by zhouxs 2007-10-28
                    {
                        this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder_Sheet1.ActiveRowIndex, (int)Cols.Price].Locked = false;
                        this.fpExecOrder.Sheets[0].SetActiveCell(this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Price);
                        return 0;//end zhouxs
                    }

                    if (this.fpExecOrder_Sheet1.ActiveRowIndex == this.fpExecOrder_Sheet1.Rows.Count - 1)
                    {
                        fpExecOrder_Sheet1.SetActiveCell(this.fpExecOrder_Sheet1.ActiveRowIndex, (int)Cols.ItemConfirmQty);
                        if (qty > 0)
                        {
                            if (DialogResult.Yes.Equals(MessageBox.Show("是否增加新收费项目?", "医技终端确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)))
                            {
                                this.AddNewRow();
                            }
                        }
                    }
                    else
                    {
                        fpExecOrder_Sheet1.SetActiveCell(this.fpExecOrder_Sheet1.ActiveRowIndex + 1, (int)Cols.ItemName);
                    }
                    return 0;
                }
                #endregion
                if (this.fpExecOrder.Sheets[0].ActiveColumnIndex == (int)Cols.Price)
                {
                    decimal price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Price].Text);
                    decimal qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemConfirmQty].Text);
                    this.fpExecOrder.Sheets[0].Cells[this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.TotCost].Text = Convert.ToString(price * qty);
                    this.fpExecOrder.Sheets[0].SetActiveCell(this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.ItemConfirmQty);
                    return 0;
                }
                //by yuyun 08-7-8{810581A3-6DF5-49af-8A5F-D7F843CBEA89}
                if (this.fpExecOrder.Sheets[0].ActiveColumnIndex == (int)Cols.Machine)
                {
                    this.fpExecOrder.Sheets[0].SetActiveCell(this.fpExecOrder.Sheets[0].ActiveRowIndex, (int)Cols.Operator);
                }

            }
            this.ShowTotFee();
            return 0;
        }

        private void fpExecOrder_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            if (!System.IO.File.Exists(this.filePath))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(this.filePath, this.dtExecOrder, ref this.dvExecOrder, this.fpExecOrder_Sheet1);
            }

            else
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpExecOrder_Sheet1, this.filePath);
            }
        }

        /// <summary>
        /// 选择包装单位和最小单位时候触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpExecOrder_ComboSelChange(object sender, EditorNotifyEventArgs e)
        {
            if (e.Column == (int)Cols.Unit)
            {
                FarPoint.Win.Spread.CellType.ComboBoxCellType comboType = (FarPoint.Win.Spread.CellType.ComboBoxCellType)this.fpExecOrder_Sheet1.Cells[e.Row, e.Column].CellType;

                string text = e.EditingControl.Text;
                if (((FarPoint.Win.FpCombo)e.EditingControl).SelectedIndex == 0)
                {
                    //按最小单位收费
                    object obj = this.fpExecOrder_Sheet1.Rows[e.Row].Tag;
                    if (obj == null)
                    {
                        return;
                    }
                    decimal price = Neusoft.FrameWork.Public.String.FormatNumber(
                        (obj as Neusoft.HISFC.Models.Pharmacy.Item).Price /
                        (obj as Neusoft.HISFC.Models.Pharmacy.Item).PackQty, 4);

                    this.fpExecOrder_Sheet1.SetValue(e.Row, (int)Cols.Price, price, false);
                    //计算总额
                    text = this.fpExecOrder_Sheet1.GetText(e.Row, (int)Cols.ItemConfirmQty);//数量
                    if (text == string.Empty)
                    {
                        text = "0";
                    }
                    decimal qty = NConvert.ToDecimal(text);


                    this.fpExecOrder_Sheet1.SetValue(e.Row, (int)Cols.TotCost, price * qty, false);
                }
                else if (((FarPoint.Win.FpCombo)e.EditingControl).SelectedIndex == 1)
                {
                    //按包装单位收费
                    object obj = this.fpExecOrder_Sheet1.Rows[e.Row].Tag;
                    if (obj == null)
                    {
                        return;
                    }
                    decimal price = (obj as Neusoft.HISFC.Models.Pharmacy.Item).Price;
                    this.fpExecOrder_Sheet1.SetValue(e.Row, (int)Cols.Price, price, false);
                    //计算总额
                    text = this.fpExecOrder_Sheet1.GetText(e.Row, (int)Cols.ItemConfirmQty);//数量
                    if (text == string.Empty)
                    {
                        text = "0";
                    }
                    decimal qty = NConvert.ToDecimal(text);


                    this.fpExecOrder_Sheet1.SetValue(e.Row, (int)Cols.TotCost, price * qty, false);
                }
            }
        }

        protected override int OnSave(object sender, object neuObject)
        {
            this.Save();
            return base.OnSave(sender, neuObject);
        }

        protected override int OnPrint(object sender, object neuObject)
        {
            #region {019B78E5-8076-4d17-8CEE-4F2FC66AD0D3}
            return base.OnPrint(sender, neuObject);
            #endregion
            if (this.terminalInterface == null)
            {
                this.terminalInterface = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(TerminalInterface)) as TerminalInterface;
                if (this.terminalInterface == null)
                {
                    MessageBox.Show("获得接口TerminalInterface错误\n，可能没有维护相关的打印控件或打印控件没有实现接口TerminalInterface\n请与系统管理员联系。");
                    return -11;
                }
            }
            if (this.terminalInterface.Reset() == -1)
            {
                return -1;
            }
            //for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            //{
            if (this.terminalInterface.ControlValue(neuObject) == -1)
            {
                return -1;
            }
            if (this.terminalInterface.Print() == -1)
            {
                return -1;
            }
            return base.OnPrint(sender, neuObject);
        }

        private void ucInpatientConfirm_Load(object sender, EventArgs e)
        {
            if (this.tv != null)
            {
                try
                {
                    tvInpatientConfirm t = (tvInpatientConfirm)tv;
                    t.IsExpand = false;
                    if (this.seeAll)
                    {
                        t.OperDept = "all";
                        //t.Init();  
                    }
                    DateTime now = this.terminalMgr.GetDateTimeFromSysDateTime();
                    DateTime beginTime = DateTime.MinValue;
                    if ((now - DateTime.MinValue).Days > this.showdays)
                    {
                        beginTime = now.AddDays(-this.showdays);
                    }
                    t.Init(beginTime, now);

                    RefleshDeptList();
                    InitDoctors();
                    this.BeginAutoRefresh();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void fpExecOrder_CellDoubleClick(object sender, CellClickEventArgs e)
        {
            ShowPacsApply(e.Row);
        }

        public override int SetPrint(object sender, object neuObject)
        {
            DateTime now = this.terminalMgr.GetDateTimeFromSysDateTime();
            DateTime beginTime = DateTime.MinValue;
            if ((now - DateTime.MinValue).Days > this.showdays)
            {
                beginTime = now.AddDays(-this.showdays);
            }
            DateTime endTime = DateTime.MinValue;
            int returnValues = Neusoft.FrameWork.WinForms.Classes.Function.ChooseDate(ref beginTime, ref endTime);
            if (returnValues < 0)
            {
                return -1;
            }
            tvInpatientConfirm t = (tvInpatientConfirm)tv;
            if (t != null)
            {
                t.Init(beginTime, endTime);

                RefleshDeptList();
                InitDoctors();
            }
            return 1;
        }

        private void RefleshDeptList()
        {
            tvInpatientConfirm t = (tvInpatientConfirm)tv;
            ArrayList allDept = new ArrayList();
            if (t.AllCurDept != null && t.AllCurDept.Count > 0)
            {
                this.cmbDept.Items.Clear();
                foreach (string sID in t.AllCurDept)
                {
                    allDept.Add(SOC.HISFC.BizProcess.Cache.Common.GetDeptInfo(sID));
                }
                this.cmbDept.AddItems(allDept);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (this.autoRefreshCallBack != null)
            {
                this.autoRefreshCallBack = null;
            }

            if (this.autoRefreshTimer != null)
            {
                this.autoRefreshTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region 自动刷新

        /// <summary>
        /// 终端设置的刷新间隔
        /// </summary>
        private int refreshInterval = 10;

        [Category("控件设置"), Description("刷新间隔时间，默认为10秒")]
        public int RefreshInterval
        {
            get
            {
                return refreshInterval;
            }
            set
            {
                this.refreshInterval = value;
            }
        }

        private System.Threading.Timer autoRefreshTimer = null;
        private System.Threading.TimerCallback autoRefreshCallBack = null;
        private delegate void autoRefreshHandler();
        private autoRefreshHandler autoRefreshEven;
        private bool isRefresh = false;

        private void BeginAutoRefresh()
        {
            if (this.autoRefreshCallBack == null)
            {
                this.autoRefreshCallBack = new System.Threading.TimerCallback(this.AutoRefreshTimerCallback);
            }
            this.autoRefreshTimer = new System.Threading.Timer(this.autoRefreshCallBack, null, refreshInterval * 1000, this.refreshInterval * 1000);
        }

        /// <summary>
        /// 刷新处方列表
        /// </summary>
        /// <param name="param">参数（没有使用）</param>
        /// <returns></returns>
        private void AutoRefreshTimerCallback(object param)
        {
            if (this.autoRefreshEven == null)
            {
                autoRefreshEven = new autoRefreshHandler(this.AutoRefresh);
            }

            if (this.ParentForm != null)
            {
                this.ParentForm.BeginInvoke(this.autoRefreshEven);
            }

        }

        /// <summary>
        /// 自动刷新
        /// </summary>
        public void AutoRefresh()
        {
            if (isRefresh)
            {
                return;
            }
            isRefresh = true;
            try
            {
                this.Refresh();
            }
            finally
            {
                isRefresh = false;
            }
        }

        #endregion

        #region IInterfaceContainer 成员
        TerminalInterface terminalInterface = null;
        public Type[] InterfaceTypes
        {
            get { return new Type[] { typeof(TerminalInterface) }; }
        }

        #endregion

        private void fpExecOrder_CellClick(object sender, CellClickEventArgs e)
        {
            Neusoft.HISFC.Models.Order.ExecOrder inOrder = fpExecOrder.Sheets[0].Rows[e.Row].Tag as Neusoft.HISFC.Models.Order.ExecOrder;
            if (inOrder != null)
            {
                try
                {
                    this.txtFeeDetail.ReadOnly = true;

                    #region 显示项目信息

                    string showInfo = "";

                    //医嘱备注
                    showInfo += "医嘱备注：" + inOrder.Order.Memo + "\r\n\r\n";

                    //项目信息
                    if (inOrder.Order.Item.ID == "999")
                    {
                        showInfo += inOrder.Order.Item.Name + " 【规格】" + inOrder.Order.Item.Specs + " 【单价】" + inOrder.Order.Item.Price.ToString() + "元/" + inOrder.Order.Item.PriceUnit;
                    }
                    else
                    {
                        if (inOrder.Order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            showInfo += SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(inOrder.Order.Item.ID).UserCode + " " + inOrder.Order.Item.Name + " 【规格】" + inOrder.Order.Item.Specs + " 【单价】" + inOrder.Order.Item.Price.ToString() + "元/" + SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(inOrder.Order.Item.ID).PackUnit;
                        }
                        else
                        {
                            showInfo += SOC.HISFC.BizProcess.Cache.Fee.GetItem(inOrder.Order.Item.ID).UserCode + " " + inOrder.Order.Item.Name + " 【规格】" + inOrder.Order.Item.Specs + " 【单价】" + inOrder.Order.Item.Price.ToString() + "元/" + inOrder.Order.Item.PriceUnit;
                        }
                    }
                    if (inOrder.Order.Item.ID != "999")
                    {
                        //套餐明细
                        if (inOrder.Order.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = SOC.HISFC.BizProcess.Cache.Fee.GetItem(inOrder.Order.Item.ID);
                            if (undrug.UnitFlag == "1")
                            {
                                showInfo += "\r\n【套餐包含】：";

                                ArrayList alZt = managerIntegrate.QueryUndrugPackageDetailByCode(inOrder.Order.Item.ID);
                                foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb comb in alZt)
                                {
                                    Neusoft.HISFC.Models.Fee.Item.Undrug combUndrug = SOC.HISFC.BizProcess.Cache.Fee.GetItem(comb.ID);
                                    showInfo += "\r\n" + combUndrug.Name + (string.IsNullOrEmpty(combUndrug.Specs) ? "" : "[" + combUndrug.Specs + "]") + "   数量：" + comb.Qty + combUndrug.PriceUnit + "   价格：" + combUndrug.Price.ToString() + "   金额：" + (comb.Qty * combUndrug.Price).ToString();
                                }
                            }
                        }
                    }

                    this.txtFeeDetail.Text = showInfo;

                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// 初始化医生信息
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitDoctors()
        {
            ArrayList doctors = new ArrayList();
            if (isHaveRight)
            {
                try
                {
                    ArrayList alDept = this.managerIntegrate.QueryDepartment(((Neusoft.HISFC.Models.Base.Employee)this.userPowerLogic.Operator).Dept.ID);
                    if (alDept == null || alDept.Count < 1 || ((Neusoft.HISFC.Models.Base.Employee)this.userPowerLogic.Operator).Dept.ID == "3023")
                    {
                        doctors = this.managerIntegrate.QueryEmployeeHaveRight(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
                    }
                    else
                    {
                        string deptStr = string.Empty;
                        string docStr = string.Empty;
                        ArrayList alDocs = new ArrayList();
                        foreach (Neusoft.HISFC.Models.Base.Department dept in alDept)
                        {
                            deptStr += dept.ID + "|";
                            ArrayList alUserPowerDetail = this.userPowerLogic.LoadUser("00", dept.ID);
                            foreach (Neusoft.HISFC.Models.Admin.UserPowerDetail info in alUserPowerDetail)
                            {
                                docStr += info.User.ID + "|";
                            }
                        }
                        doctors = this.managerIntegrate.QueryEmployeeHaveRight(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);

                        foreach (Neusoft.HISFC.Models.Base.Employee empl in doctors)
                        {
                            if (deptStr.Contains(empl.Dept.ID) || docStr.Contains(empl.ID))
                            {
                                alDocs.Add(empl);
                            }
                        }
                        doctors = alDocs;
                        if (doctors == null || doctors.Count < 1)
                        {
                            doctors = this.managerIntegrate.QueryEmployeeHaveRight(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Language.Msg("加载医生信息出错!\n") + ex.Message);
                    doctors = this.managerIntegrate.QueryEmployee(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
                }
            }
            else
            {
                doctors = this.managerIntegrate.QueryEmployee(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
            }
            if (doctors == null)
            {
                MessageBox.Show(Language.Msg("加载医生信息出错!") + this.managerIntegrate.Err);

                return -1;
            }

            this.cmbDoct.AddItems(doctors);

            return 1;
        }
    }

    public enum EnumCheckState
    {
        /// <summary>
        /// 确认收费
        /// </summary>
        [Neusoft.FrameWork.Public.Description("收费")]
        ComfirmComplete,

        /// <summary>
        /// 取消收费
        /// </summary>
        [Neusoft.FrameWork.Public.Description("不收费")]
        ComfirmCancel,

        /// <summary>
        /// 暂不处理
        /// </summary>
        [Neusoft.FrameWork.Public.Description("暂不处理")]
        None
    }
}

