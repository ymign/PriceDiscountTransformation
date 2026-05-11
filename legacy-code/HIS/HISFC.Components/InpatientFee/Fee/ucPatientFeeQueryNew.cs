using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.Management;
using System.Collections;
using Neusoft.HISFC.BizLogic.RADT;
using Neusoft.HISFC.Models.RADT;
using Neusoft.HISFC.Models.Fee.Inpatient;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.WinForms.Forms;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using Neusoft.HISFC.Models.Fee;
using System.Linq;

namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    public partial class ucPatientFeeQueryNew : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        /// <summary>
        /// 
        /// </summary>
        public ucPatientFeeQueryNew()
        {
            InitializeComponent();
            this.fpMainInfo.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(fpMainInfo_SelectionChanged);
            this.fpMainInfo.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(fpMainInfo_CellDoubleClick);
            this.fpFee.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(fpFee_SelectionChanged);
            this.fpFeeDetail.MouseUp += new MouseEventHandler(fpFeeDetail_MouseUp);
            this.fpFeeDetail.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(fpFeeDetail_ButtonClicked);
            this.fpFeeDetail.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(fpFeeDetail_SeleCount);
            this.fpFeeItemSum.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(fpFeeItemSum_SelectionChanged);

            //过滤
            this.cmbFeeTypeFilter.TextChanged += new EventHandler(cmbFeeTypeFilter_TextChanged);
            this.txtItemFilter.TextChanged += new EventHandler(txtItemFilter_TextChanged);
            this.dtpItemDayFilter.ValueChanged += new EventHandler(dtpItemDayFilter_ValueChanged);
            this.ckUnCombo.CheckedChanged += new EventHandler(ckUnCombo_CheckedChanged);
        }

        #region 变量
        /// <summary>
        /// toolBarService
        /// </summary>
        ToolBarService toolBarService = new ToolBarService();

        private FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
        /// <summary>
        /// 住院入出转业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.RADT.InPatient radtManager = new Neusoft.HISFC.BizLogic.RADT.InPatient();
        protected Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
        protected Neusoft.HISFC.BizLogic.Fee.PactUnitInfo PactManagment = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo();

        /// <summary>
        /// 科室业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Manager.Department deptManager = new Neusoft.HISFC.BizLogic.Manager.Department();
        /// <summary>
        /// 人员信息业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Manager.Person personManager = new Neusoft.HISFC.BizLogic.Manager.Person();

        /// <summary>
        /// 常数业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Manager.Constant consManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
        /// <summary>
        /// 退费申请业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.ReturnApply returnApplyManager = new Neusoft.HISFC.BizLogic.Fee.ReturnApply();

        /// <summary>
        /// 药品业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Pharmacy phamarcyIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();

        /// <summary>
        /// 费用业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.InPatient feeManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 诊断业务层-met_cas_diagnose
        /// </summary>
        protected Neusoft.HISFC.BizLogic.HealthRecord.Diagnose diagMgr = new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose();

        /// <summary>
        /// 当前患者
        /// </summary>
        Neusoft.HISFC.Models.RADT.PatientInfo currentPatient = new Neusoft.HISFC.Models.RADT.PatientInfo();

        /// <summary>
        /// 临时患者信息
        /// </summary>
        Neusoft.HISFC.Models.RADT.PatientInfo myPatientInfoTemp = new Neusoft.HISFC.Models.RADT.PatientInfo();

        /// <summary>
        /// Tab
        /// </summary>
        protected Hashtable hashTableFp = new Hashtable();
        protected ArrayList alAllFeeItemLists = new ArrayList();

        protected Hashtable hashTablePeople = new Hashtable();
        protected Hashtable hashTableDept = new Hashtable();

        protected Hashtable hsInvoceType = new Hashtable();
        protected Hashtable AllInvoceType = new Hashtable();

        /// <summary>
        /// 最小费用帮助类
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper minFeeHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        private Hashtable hsMinFee = new Hashtable();
        private Hashtable hsDayFeeItemList = new Hashtable();

        private Hashtable hsQuitFeeByPackage = new Hashtable();
        /// <summary>
        /// 退费申请或退药申请信息
        /// </summary>
        private Hashtable dicReturnApply = new Hashtable();

        /// <summary>
        /// 科室 护士站包括的科室
        /// </summary>
        private Hashtable depts = new Hashtable();

        #region DataTalbe相关变量

        string pathNameMainInfo = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientMainInfo.xml";
        string pathNamePrepay = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientPrepay.xml";
        string pathNameFee = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientFee.xml";
        string pathNameFeeItemSum = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientFeeItemSum.xml";
        string pathNameFeeDaySum = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientFeeDaySum.xml";
        string pathNameFeeDetail = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientFeeDetail.xml";
        string pathNameBalance = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientBalance.xml";
        string pathNameDiagnose = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientDiagnose.xml";
        string pathNameShiftData = Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\QueryPatientShiftData.xml";

        /// <summary>
        /// 患者主信息
        /// </summary>
        DataTable dtMainInfo = new DataTable();

        /// <summary>
        /// 患者主信息视图
        /// </summary>
        DataView dvMainInfo = new DataView();

        /// <summary>
        /// 药品明细
        /// </summary>
        DataTable dtFeeItemSum = new DataTable();

        /// <summary>
        /// 药品明细视图
        /// </summary>
        DataView dvFeeItemSum = new DataView();

        /// <summary>
        /// 药品明细
        /// </summary>
        DataTable dtFeeDaySum = new DataTable();

        /// <summary>
        /// 药品明细视图
        /// </summary>
        DataView dvFeeDaySum = new DataView();

        /// <summary>
        /// 非药品信息
        /// </summary>
        DataTable dtFeeDetail = new DataTable();

        /// <summary>
        /// 非药品信息视图
        /// </summary>
        DataView dvFeeDetail = new DataView();

        /// <summary>
        /// 预交金信息
        /// </summary>
        DataTable dtPrepay = new DataTable();

        /// <summary>
        /// 预交金视图
        /// </summary>
        DataView dvPrepay = new DataView();

        /// <summary>
        /// 费用汇总信息
        /// </summary>
        DataTable dtFee = new DataTable();

        /// <summary>
        /// 费用汇总信息视图
        /// </summary>
        DataView dvFee = new DataView();

        /// <summary>
        /// 费用结算信息
        /// </summary>
        DataTable dtBalance = new DataTable();

        /// <summary>
        /// 费用结算信息视图
        /// </summary>
        DataView dvBalance = new DataView();

        /// <summary>
        /// 变更信息表
        /// </summary>
        DataTable dtShiftData = new DataTable();

        /// <summary>
        /// 变更信息视图
        /// </summary>
        DataView dvShiftData = new DataView();

        /// <summary>
        /// 所有费用
        /// </summary>
        decimal totcost = 0;

        #endregion

        #endregion

        #region 属性

        /// <summary>
        /// 住院患者基本信息
        /// </summary>
        public PatientInfo PatientInfo
        {
            get
            {
                return this.currentPatient;
            }
            set
            {
                this.currentPatient = value;

                //Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载数据，请稍后^^");
                //Application.DoEvents();

                this.QueryPatientByInpatientNO(currentPatient);

                // Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
        }

        [Category("参数设置"), Description("住院号输入框默认输入0住院号，5床号")]
        public int DefaultInput
        {
            get
            {
                return this.ucQueryInpatientNo1.DefaultInputType;
            }
            set
            {
                this.ucQueryInpatientNo1.DefaultInputType = value;
            }
        }

        [Category("参数设置"), Description("按床号查询患者信息时，按患者的状态查询，如果全部则'ALL'")]
        public string PatientInState
        {
            get
            {
                return this.ucQueryInpatientNo1.PatientInState;
            }
            set
            {
                this.ucQueryInpatientNo1.PatientInState = value;
            }
        }

        /// <summary>
        /// 是否允许退其他科室费用
        /// </summary>
        private bool isCanQuitOtherDeptFee = true;
        [Category("参数设置"), Description("是否允许退其他科室费用 True:可以 False:不可以")]
        public bool IsCanQuitOtherDeptFee
        {
            get
            {
                return isCanQuitOtherDeptFee;
            }
            set
            {
                isCanQuitOtherDeptFee = value;
            }
        }

        /// <summary>
        /// 本科室是否需要申请
        /// </summary>
        private bool isCurrentDeptNeedAppy = true;
        [Category("参数设置"), Description("本科室费用是否需要申请，True:需要 False:不需要")]
        public bool IsCurrentDeptNeedAppy
        {
            get
            {
                return isCurrentDeptNeedAppy;
            }
            set
            {
                isCurrentDeptNeedAppy = value;
            }
        }


        /// <summary>
        /// 是否允许退其他操作员费用
        /// </summary>
        private bool isCanQuitOtherOperator = true;
        [Category("参数设置"), Description("是否允许退其他操作员费用，True:可以 False:不可以")]
        public bool IsCanQuitOtherOperator
        {
            get
            {
                return this.isCanQuitOtherOperator;
            }
            set
            {
                this.isCanQuitOtherOperator = value;
            }
        }

        /// <summary>
        /// 是否允许退费和取消退费操作
        /// </summary>
        private bool isQuitFee = false;
        [Category("参数设置"), Description("是否允许退费和取消退费操作，True:可以 False:不可以")]
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

        [Category("参数设置"), Description("显示Tab页的顺序和可见性")]
        public string ShowTab
        {
            get
            {
                string str = "";
                foreach (TabPage tp in this.neuTabControl1.TabPages)
                {
                    str += tp.Name + ",";
                }
                return str.Length > 0 ? str.Substring(0, str.Length - 1) : str;
            }
            set
            {

                this.neuTabControl1.TabPages.Clear();
                string[] strArr = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (strArr != null && strArr.Length > 0)
                {
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        if (this.tpFeeInfo.Name.Equals(strArr[i]))
                        {
                            this.neuTabControl1.TabPages.Insert(this.neuTabControl1.TabPages.Count, tpFeeInfo);
                        }
                        else if (this.tpMainInfo.Name.Equals(strArr[i]))
                        {
                            this.neuTabControl1.TabPages.Insert(this.neuTabControl1.TabPages.Count, tpMainInfo);
                        }
                    }
                }
            }
        }

        private bool isShowCurrentDeptFee = false;
        [Category("参数设置"), Description("是否只显示本科室的费用")]
        public bool IsShowCurrentDeptFee
        {
            get
            {
                return isShowCurrentDeptFee;
            }
            set
            {
                isShowCurrentDeptFee = value;
            }
        }

        #endregion

        #region 私有方法

        private void InitHashTable()
        {
            //查找病区包含的科室
            depts.Clear();
            ArrayList alDept = this.deptManager.GetDeptFromNurseStation((this.deptManager.Operator as Neusoft.HISFC.Models.Base.Employee).Nurse);
            if (alDept != null)
            {
                foreach (Neusoft.FrameWork.Models.NeuObject obj in alDept)
                {
                    if (depts.ContainsKey(obj.ID) == false)
                    {
                        depts.Add(obj.ID, null);
                    }
                }
            }

            if (depts.ContainsKey((this.deptManager.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID) == false)
            {
                depts.Add((this.deptManager.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID, null);
            }

            //查找最小费用
            ArrayList alMinFee = this.consManager.GetAllList(Neusoft.HISFC.Models.Base.EnumConstant.MINFEE);
            if (alMinFee != null)
            {
                this.minFeeHelper.ArrayObject = alMinFee;
                this.cmbFeeTypeFilter.AddItems(alMinFee);
            }
        }

        /// <summary>
        /// 显示患者基本信息
        /// </summary>
        /// <param name="patient">成功 1 失败 -1</param>
        private void SetPatientToFpMain(Neusoft.HISFC.Models.RADT.PatientInfo patient)
        {
            if (patient == null)
            {
                return;
            }

            DataRow row = this.dtMainInfo.NewRow();

            try
            {

                row["住院流水号"] = patient.ID;
                row["住院号"] = patient.PID.PatientNO;
                row["姓名"] = patient.Name;
                row["住院科室"] = patient.PVisit.PatientLocation.Dept.Name;
                row["床号"] = patient.PVisit.PatientLocation.Bed.ID.Length > 4 ? patient.PVisit.PatientLocation.Bed.ID.Substring(4) : patient.PVisit.PatientLocation.Bed.ID;
                row["患者类别"] = patient.Pact.Name;
                row["预交金(未结)"] = patient.FT.PrepayCost;
                row["费用合计(未结)"] = patient.FT.TotCost;
                row["余额"] = patient.FT.LeftCost;
                row["自费"] = patient.FT.OwnCost;
                row["自负"] = patient.FT.PayCost;
                row["公费"] = patient.FT.PubCost;
                row["入院日期"] = patient.PVisit.InTime;
                row["在院状态"] = patient.PVisit.InState.Name;

                row["出院日期"] = patient.PVisit.OutTime.Date == new DateTime(1, 1, 1).Date ? string.Empty : patient.PVisit.OutTime.ToString();

                row["预交金(已结)"] = patient.FT.BalancedPrepayCost;
                row["费用合计(已结)"] = patient.FT.BalancedCost;
                //row["医疗类别"] = patient.PVisit.MedicalType.Name;
                row["结算日期"] = patient.BalanceDate;

                this.dtMainInfo.Rows.Add(row);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return;
            }
        }

        /// <summary>
        /// 根据输入的住院号查询患者基本信息
        /// </summary>
        private void QueryPatientByInpatientNO(PatientInfo patients)
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在查询信息，请稍后...");
            Application.DoEvents();
            this.Clear();
            this.dtMainInfo.Rows.Clear();
            Cursor.Current = Cursors.WaitCursor;
            //住院主表信息
            this.SetPatientToFpMain(patients);
            Cursor.Current = Cursors.Arrow;
            this.SetPatientInfo();
            //设置查询时间
            //设置查询时间
            DateTime beginTime = this.currentPatient.PVisit.InTime;
            DateTime endTime = this.radtManager.GetDateTimeFromSysDateTime();
            this.QueryAllInfomaition(beginTime, endTime);
            this.fpMainInfo_Sheet1.Columns[13].Width = 180;
            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
        }

        /// <summary>
        /// 获得患者药品明细
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void QueryPatientDrugList(DateTime beginTime, DateTime endTime)
        {
            ArrayList drugList = this.feeManager.GetMedItemsForInpatient(this.currentPatient.ID, beginTime, endTime);
            if (drugList == null)
            {
                MessageBox.Show(Language.Msg("获得患者药品明细出错!") + this.feeManager.Err);

                return;
            }

            //this.alAllFeeItemLists.AddRange(drugList);

            //string strTemp = null;
            Neusoft.HISFC.Models.Base.Employee employee = this.feeManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList obj in drugList)
            {
                if (this.isShowCurrentDeptFee && obj.ExecOper.Dept.ID.Equals(employee.Dept.ID) == false && obj.FeeOper.Dept.ID.Equals(employee.Dept.ID) == false)
                {
                    continue;
                }

                Neusoft.FrameWork.Models.NeuObject invotype = new Neusoft.FrameWork.Models.NeuObject();//发票分类
                invotype = AllInvoceType[obj.Item.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                obj.Item.MinFee.ID = invotype.ID;
                obj.UndrugComb.MinFee.ID = invotype.ID;

                if (this.hsMinFee.ContainsKey(invotype.ID) == false)
                {
                    hsMinFee[invotype.ID] = new ArrayList();
                }
                ((ArrayList)hsMinFee[invotype.ID]).Add(obj);

                if (this.hsDayFeeItemList.ContainsKey(obj.Item.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")) == false)
                {
                    hsDayFeeItemList[obj.Item.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")] = new ArrayList();
                }
                ((ArrayList)hsDayFeeItemList[obj.Item.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")]).Add(obj);

                //    DataRow row = dtDrugList.NewRow();

                //    row["药品名称"] = obj.Item.Name;
                //    Neusoft.HISFC.Models.Pharmacy.Item medItem = (Neusoft.HISFC.Models.Pharmacy.Item)obj.Item;
                //    row["规格"] = medItem.Specs;
                //    row["单价"] = obj.Item.Price;
                //    row["数量"] = obj.Item.Qty;
                //    row["付数"] = obj.Days;
                //    row["单位"] = obj.Item.PriceUnit;
                //    row["金额"] = obj.FT.TotCost;
                //    row["自费"] = obj.FT.OwnCost;
                //    row["公费"] = obj.FT.PubCost;
                //    row["自负"] = obj.FT.PayCost;
                //    row["优惠"] = obj.FT.RebateCost;

                //    strTemp = obj.ExecOper.Dept.ID;
                //    row["执行科室"] = this.hashTableDept.Contains(strTemp) ? this.hashTableDept[strTemp].ToString() : "";// this.deptManager.GetDeptmentById(obj.ExecOper.Dept.ID).Name;

                //    strTemp = ((Neusoft.HISFC.Models.RADT.PatientInfo)obj.Patient).PVisit.PatientLocation.Dept.ID;
                //    row["患者科室"] = this.hashTableDept.Contains(strTemp) ? this.hashTableDept[strTemp].ToString() : "";// this.deptManager.GetDeptmentById(((Neusoft.HISFC.Models.RADT.PatientInfo)obj.Patient).PVisit.PatientLocation.Dept.ID).Name;
                //    row["收费时间"] = obj.FeeOper.OperTime;

                //    Neusoft.HISFC.Models.Base.Employee empl = new Neusoft.HISFC.Models.Base.Employee();

                //    strTemp = obj.FeeOper.ID;
                //    row["收费员"] = this.hashTablePeople.Contains(strTemp) ? hashTablePeople[strTemp].ToString() : strTemp;

                //    row["发药时间"] = obj.ExecOper.OperTime.Date == new DateTime(1, 1, 1).Date ? string.Empty : obj.ExecOper.OperTime.ToString() ;


                //    strTemp = obj.ExecOper.ID;
                //    row["发药员"] = this.hashTablePeople.Contains(strTemp) ? hashTablePeople[obj.ExecOper.ID].ToString() : strTemp;

                //    dtDrugList.Rows.Add(row);
            }

            //this.AddSumInfo(dtDrugList, "药品名称", "合计:", "金额", "自费", "公费", "自负", "优惠");
        }

        /// <summary>
        /// 查询患者非药品明细
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void QueryPatientUndrugList(DateTime beginTime, DateTime endTime)
        {
            ArrayList undrugList = this.feeManager.QueryFeeItemLists(this.currentPatient.ID, beginTime, endTime);
            if (undrugList == null)
            {
                MessageBox.Show(Language.Msg("获得患者非药品明细出错!") + this.feeManager.Err);

                return;
            }

            //this.alAllFeeItemLists.AddRange(undrugList);
            //string strTemp = "";
            Neusoft.HISFC.Models.Base.Employee employee = this.feeManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList obj in undrugList)
            {
                if (this.isShowCurrentDeptFee && obj.ExecOper.Dept.ID.Equals(employee.Dept.ID) == false && obj.FeeOper.Dept.ID.Equals(employee.Dept.ID) == false)
                {
                    continue;
                }

                Neusoft.FrameWork.Models.NeuObject invotype = new Neusoft.FrameWork.Models.NeuObject();//发票分类
                if (this.IsPackageItem(obj))
                {
                    #region 复合项目最小费用
                    if (string.IsNullOrEmpty(obj.UndrugComb.MinFee.ID))
                    {
                        obj.UndrugComb.MinFee.ID = CommonController.Instance.GetItem(obj.UndrugComb.ID).MinFee.ID;
                        invotype = AllInvoceType[obj.UndrugComb.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                        obj.UndrugComb.MinFee.ID = invotype.ID;
                        obj.Item.MinFee.ID = invotype.ID;

                    }
                    else
                    {
                        invotype = AllInvoceType[obj.UndrugComb.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                        obj.UndrugComb.MinFee.ID = invotype.ID;
                        obj.Item.MinFee.ID = invotype.ID;
                    }
                    if (this.hsMinFee.ContainsKey(invotype.ID) == false)
                    {
                        hsMinFee[invotype.ID] = new ArrayList();
                    }
                    ((ArrayList)hsMinFee[invotype.ID]).Add(obj);

                    if (this.hsDayFeeItemList.ContainsKey(obj.UndrugComb.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")) == false)
                    {
                        hsDayFeeItemList[obj.UndrugComb.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")] = new ArrayList();
                    }
                    ((ArrayList)hsDayFeeItemList[obj.UndrugComb.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")]).Add(obj);
                    #endregion
                }
                else
                {
                    invotype = AllInvoceType[obj.Item.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                    obj.UndrugComb.MinFee.ID = invotype.ID;
                    obj.Item.MinFee.ID = invotype.ID;

                    #region 项目最小费用
                    if (this.hsMinFee.ContainsKey(invotype.ID) == false)
                    {
                        hsMinFee[invotype.ID] = new ArrayList();
                    }
                    ((ArrayList)hsMinFee[invotype.ID]).Add(obj);

                    if (this.hsDayFeeItemList.ContainsKey(obj.Item.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")) == false)
                    {
                        hsDayFeeItemList[obj.Item.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")] = new ArrayList();
                    }
                    ((ArrayList)hsDayFeeItemList[obj.Item.ID + obj.ChargeOper.OperTime.ToString("yyyy-MM-dd")]).Add(obj);
                    #endregion
                }

                //    DataRow row = dtUndrugList.NewRow();

                //    row["项目名称"] = obj.Item.Name;
                //    row["单价"] = obj.Item.Price;
                //    row["数量"] = obj.Item.Qty;
                //    row["单位"] = obj.Item.PriceUnit;
                //    row["金额"] = obj.FT.TotCost;
                //    row["自费"] = obj.FT.OwnCost;
                //    row["公费"] = obj.FT.PubCost;
                //    row["自负"] = obj.FT.PayCost;
                //    row["优惠"] = obj.FT.RebateCost;
                //    row["收费时间"] = obj.FeeOper.OperTime;

                //    strTemp = obj.FeeOper.ID;
                //    row["收费员"] = this.hashTablePeople.Contains(strTemp) ? hashTablePeople[strTemp].ToString() : strTemp;

                //    strTemp = obj.ExecOper.Dept.ID;
                //    row["执行科室"] = this.hashTableDept.Contains(strTemp) ? this.hashTableDept[strTemp].ToString() : strTemp;// this.deptManager.GetDeptmentById(obj.ExecOper.Dept.ID).Name;

                //    strTemp = ((Neusoft.HISFC.Models.RADT.PatientInfo)obj.Patient).PVisit.PatientLocation.Dept.ID;
                //    row["患者科室"] = this.hashTableDept.Contains(strTemp) ? this.hashTableDept[strTemp].ToString() : strTemp;// this.deptManager.GetDeptmentById(((Neusoft.HISFC.Models.RADT.PatientInfo)obj.Patient).PVisit.PatientLocation.Dept.ID).Name;
                //    //row["来源"] = obj.FTSource;

                //    dtUndrugList.Rows.Add(row);
            }

            //this.AddSumInfo(dtUndrugList, "项目名称", "合计:", "金额", "自费", "公费", "自负", "优惠");
        }

        /// <summary>
        /// 查询患者预交金信息
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void QueryPatientPrepayList(DateTime beginTime, DateTime endTime)
        {
            ArrayList prepayList = this.feeManager.QueryPrepays(this.currentPatient.ID);
            if (prepayList == null)
            {
                MessageBox.Show(Language.Msg("获得患者预交金明细出错!") + this.feeManager.Err);

                return;
            }

            string strTemp = string.Empty;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay in prepayList)
            {
                Neusoft.HISFC.Models.Base.Employee employeeObj = new Neusoft.HISFC.Models.Base.Employee();
                Neusoft.HISFC.Models.Base.Department deptObj = new Neusoft.HISFC.Models.Base.Department();
                DataRow row = dtPrepay.NewRow();

                row["票据号"] = prepay.RecipeNO;
                row["预交金额"] = prepay.FT.PrepayCost;
                row["支付方式"] = prepay.PayType.Name;

                strTemp = prepay.PrepayOper.ID;
                row["操作员"] = this.hashTablePeople.Contains(strTemp) ? this.hashTablePeople[strTemp] : strTemp;

                row["操作日期"] = prepay.PrepayOper.OperTime;

                strTemp = ((Neusoft.HISFC.Models.RADT.PatientInfo)prepay.Patient).PVisit.PatientLocation.Dept.ID;
                row["所在科室"] = this.hashTableDept.Contains(strTemp) ? this.hashTableDept[strTemp] : strTemp;

                string tempBalanceStatusName = string.Empty;
                switch (prepay.BalanceState)
                {
                    case "0":
                        tempBalanceStatusName = "未结算";
                        break;
                    case "1":
                        tempBalanceStatusName = "已结算";
                        break;
                    case "2":
                        tempBalanceStatusName = "已结转";
                        break;
                }
                row["结算状态"] = tempBalanceStatusName;
                string tempPrepayStateName = string.Empty;
                switch (prepay.PrepayState)
                {
                    case "0":
                        tempPrepayStateName = "收取";
                        break;
                    case "1":
                        tempPrepayStateName = "作废";
                        break;
                    case "2":
                        tempPrepayStateName = "补打";
                        break;
                }

                dtPrepay.Rows.Add(row);
            }

            this.AddSumInfo(dtPrepay, "票据号", "合计:", "预交金额");

            dvPrepay.Sort = "票据号 ASC";
        }

        /// <summary>
        /// 获得患者指定时间段内的最小费用汇总信息
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void QueryPatientFeeInfo(DateTime beginTime, DateTime endTime)
        {

            Neusoft.HISFC.Models.Base.Employee employee = this.feeManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            ArrayList feeInfoList = new ArrayList();

            Neusoft.HISFC.Models.Fee.Inpatient.FeeInfo suminfo = new FeeInfo();
            suminfo.Item.MinFee.ID = "0000";
            suminfo.FT.TotCost = 0;
            suminfo.FT.OwnCost = 0;
            suminfo.FT.PubCost = 0;
            suminfo.FT.PayCost = 0;
            suminfo.FT.RebateCost = 0;
            //显示明细

            feeInfoList = this.feeManager.QueryFeeInfosGroupByInvoceTypeByInpatientNO(this.currentPatient.ID, beginTime, endTime, "0", this.isShowCurrentDeptFee ? employee.Dept.ID : "ALL");

            if (feeInfoList == null)
            {
                MessageBox.Show(Language.Msg("获得患者费用汇总明细出错!") + this.feeManager.Err);

                return;
            }
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeInfo feeInfo in feeInfoList)
            {
                //if (this.isShowCurrentDeptFee && feeInfo.ExecOper.Dept.ID.Equals(employee.Dept.ID) == false && feeInfo.FeeOper.Dept.ID.Equals(employee.Dept.ID)==false)
                //{
                //    continue;
                //}
                DataRow row = dtFee.NewRow();

                row["费用代码"] = feeInfo.Item.MinFee.ID;
                row["费用名称"] = this.hsInvoceType[feeInfo.Item.MinFee.ID].ToString();
                row["金额"] = feeInfo.FT.TotCost;
                row["自费"] = feeInfo.FT.OwnCost;
                row["公费"] = feeInfo.FT.PubCost;
                row["自负"] = feeInfo.FT.PayCost;
                row["优惠金额"] = feeInfo.FT.RebateCost;
                row["结算状态"] = "未结算";
                dtFee.Rows.Add(row);

                suminfo.FT.TotCost += feeInfo.FT.TotCost;
                suminfo.FT.OwnCost += feeInfo.FT.OwnCost;
                suminfo.FT.PubCost += feeInfo.FT.PubCost;
                suminfo.FT.PayCost += feeInfo.FT.PayCost;
                suminfo.FT.RebateCost += feeInfo.FT.RebateCost;
            }

            ArrayList feeInfoListBalanced = new ArrayList();

            feeInfoListBalanced = this.feeManager.QueryFeeInfosGroupByInvoceTypeByInpatientNO(this.currentPatient.ID, beginTime, endTime, "1", this.isShowCurrentDeptFee ? employee.Dept.ID : "ALL");
            if (feeInfoListBalanced == null)
            {
                MessageBox.Show(Language.Msg("获得患者费用汇总明细出错!") + this.feeManager.Err);

                return;
            }

            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeInfo feeInfo in feeInfoListBalanced)
            {
                //if (this.isShowCurrentDeptFee && feeInfo.ExecOper.Dept.ID.Equals(employee.Dept.ID) == false && feeInfo.FeeOper.Dept.ID.Equals(employee.Dept.ID) == false)
                //{
                //    continue;
                //}

                DataRow row = dtFee.NewRow();
                row["费用代码"] = feeInfo.Item.MinFee.ID;
                row["费用名称"] = this.hsInvoceType[feeInfo.Item.MinFee.ID].ToString() + (feeInfo.SplitFeeFlag ? "(加收)" : string.Empty); //this.feeManager.GetComDictionaryNameByID("MINFEE", feeInfo.Item.MinFee.ID);
                row["金额"] = feeInfo.FT.TotCost;
                row["自费"] = feeInfo.FT.OwnCost;
                row["公费"] = feeInfo.FT.PubCost;
                row["自负"] = feeInfo.FT.PayCost;
                row["优惠金额"] = feeInfo.FT.RebateCost;
                row["结算状态"] = "已结算";

                dtFee.Rows.Add(row);

                suminfo.FT.TotCost += feeInfo.FT.TotCost;
                suminfo.FT.OwnCost += feeInfo.FT.OwnCost;
                suminfo.FT.PubCost += feeInfo.FT.PubCost;
                suminfo.FT.PayCost += feeInfo.FT.PayCost;
                suminfo.FT.RebateCost += feeInfo.FT.RebateCost;

            }

            DataRow sumrow = dtFee.NewRow();
            sumrow["费用代码"] = suminfo.Item.MinFee.ID;
            sumrow["费用名称"] = "全部费用";
            sumrow["金额"] = suminfo.FT.TotCost;
            sumrow["自费"] = suminfo.FT.OwnCost;
            sumrow["公费"] = suminfo.FT.PayCost;
            sumrow["自负"] = suminfo.FT.PayCost;
            sumrow["优惠金额"] = suminfo.FT.RebateCost;
            sumrow["结算状态"] = "全部费用";
            //dtFee.Rows.Add(sumrow);
            dtFee.Rows.InsertAt(sumrow, 0);

            totcost = suminfo.FT.TotCost;

            this.SelectFeeInfo();
        }

        /// <summary>
        /// 获得患者结算信息
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void QueryPatientBalance(DateTime beginTime, DateTime endTime)
        {
            ArrayList balanceList = this.feeManager.QueryBalancesByInpatientNO(this.currentPatient.ID);
            if (balanceList == null)
            {
                MessageBox.Show(Language.Msg("获得患者费用结算出错!") + this.feeManager.Err);

                return;
            }
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Balance balance in balanceList)
            {
                string temp = "";
                DataRow row = dtBalance.NewRow();

                row["发票号码"] = balance.Invoice.ID;
                row["预交金额"] = balance.FT.PrepayCost;
                row["总金额"] = balance.FT.TotCost;
                row["个人支付"] = balance.FT.OwnCost;
                row["基金支付"] = balance.FT.PubCost;
                row["优惠"] = balance.FT.RebateCost;
                row["返还金额"] = balance.FT.ReturnCost;
                row["补收金额"] = balance.FT.SupplyCost;
                row["结算时间"] = balance.BalanceOper.OperTime;

                temp = balance.BalanceOper.ID;
                row["操作员"] = this.hashTablePeople.Contains(temp) ? this.hashTablePeople[temp] : temp;
                row["结算类型"] = balance.BalanceType.Name;

                switch (balance.CancelType)
                {
                    case Neusoft.HISFC.Models.Base.CancelTypes.Valid:
                        temp = "正常结算";
                        break;
                    case Neusoft.HISFC.Models.Base.CancelTypes.LogOut:
                        temp = "结算召回";
                        break;
                    case Neusoft.HISFC.Models.Base.CancelTypes.Reprint:
                        temp = "发票重打";
                        break;

                }
                row["结算状态"] = temp;

                dtBalance.Rows.Add(row);
            }

            AddSumInfo(dtBalance, "发票号码", "合计:", "总金额", "个人支付", "基金支付", "优惠", "返还金额", "补收金额");
        }

        /// <summary>
        /// 查询变更信息
        /// </summary>
        /// <param name="patient"></param>
        private void QueryPatientShiftData(PatientInfo patient)
        {
            ArrayList shiftDataList = this.radtManager.QueryPatientShiftInfoNew(patient.ID);
            if (shiftDataList == null)
            {
                MessageBox.Show(Language.Msg("获得患者变更信息出错!") + this.radtManager.Err);

                return;
            }
            foreach (Neusoft.HISFC.Models.Invalid.CShiftData shiftData in shiftDataList)
            {
                DataRow row = dtShiftData.NewRow();

                row["变更类型"] = shiftData.ShitType;
                row["变更名称"] = shiftData.ShitCause;
                row["变更前编码"] = shiftData.OldDataCode;
                row["变更前名称"] = shiftData.OldDataName;
                row["变更后编码"] = shiftData.NewDataCode;
                row["变更后名称"] = shiftData.NewDataName;
                row["变更时间"] = shiftData.Memo;
                row["变更人"] = this.hashTablePeople.ContainsKey(shiftData.OperCode) ? this.hashTablePeople[shiftData.OperCode].ToString() : shiftData.OperCode;

                dtShiftData.Rows.Add(row);
            }

            ArrayList shiftMark = this.consManager.GetListByTypeAndName("FINIPRFEEMARK", patient.ID);
            foreach (Neusoft.HISFC.Models.Base.Const info in shiftMark)
            {
                DataRow row = dtShiftData.NewRow();

                row["变更类型"] = "收费处备注";
                row["变更名称"] = info.Memo;
                row["变更前编码"] = string.Empty;
                row["变更前名称"] = string.Empty;
                row["变更后编码"] = string.Empty;
                row["变更后名称"] = string.Empty;
                row["变更时间"] = info.OperEnvironment.OperTime.ToString();
                row["变更人"] = this.hashTablePeople.ContainsKey(info.OperEnvironment.ID) ? this.hashTablePeople[info.OperEnvironment.ID].ToString() : info.OperEnvironment.ID;

                dtShiftData.Rows.Add(row);
            }
        }

        /// <summary>
        /// 查找退费申请信息
        /// </summary>
        /// <param name="patient"></param>
        private void QueryPatientReturyApply(PatientInfo patient)
        {
            ArrayList returnApplys = this.returnApplyManager.QueryReturnApplys(patient.ID, false);
            if (returnApplys == null)
            {
                MessageBox.Show(Language.Msg("获得退费申请信息出错！"));
                return;
            }
            dicReturnApply.Clear();
            foreach (Neusoft.HISFC.Models.Fee.ReturnApply returnApply in returnApplys)
            {
                if (string.IsNullOrEmpty(returnApply.Item.MinFee.ID) && string.IsNullOrEmpty(returnApply.UndrugComb.MinFee.ID) && string.IsNullOrEmpty(returnApply.UndrugComb.ID))
                {//item.Item.ID
                    Neusoft.HISFC.Models.Base.Item baseItem = null;
                    if (returnApply.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                        baseItem = CommonController.Instance.GetItem(returnApply.Item.ID);
                    }
                    else if (returnApply.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        baseItem = this.phamarcyIntegrate.GetItem(returnApply.Item.ID);
                    }

                    returnApply.Item.MinFee.ID = baseItem.MinFee.ID;
                }


                //转换成发票分类
                Neusoft.FrameWork.Models.NeuObject obj = AllInvoceType[returnApply.Item.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                if (obj != null) 
                {
                    returnApply.Item.MinFee.ID = obj.ID;
                    returnApply.UndrugComb.MinFee.ID = obj.ID;
                }
                


                dicReturnApply.Add(returnApply.RecipeNO + returnApply.SequenceNO.ToString(), returnApply);
            }
            ArrayList alQuitDrug = this.phamarcyIntegrate.QueryDrugReturn("AAAA"/*this.operDept.ID*/, "AAAA", patient.ID);
            if (alQuitDrug == null)
            {
                MessageBox.Show(Language.Msg("获取退药申请信息发生错误"));
                return;
            }
            foreach (Neusoft.HISFC.Models.Pharmacy.ApplyOut info in alQuitDrug)
            {
                dicReturnApply.Add(info.RecipeNO + info.SequenceNO.ToString(), info);
            }
        }

        /// <summary>
        /// 查找发票分类
        /// </summary>
        /// <param name="patient"></param>
        private void QueryInvoceType()
        {
            hsInvoceType.Clear();
            ArrayList al = new ArrayList();
            al = this.feeManager.QueryFeeStatsByReportCode("ZY01");
            foreach (Neusoft.FrameWork.Models.NeuObject obj in al)
            {
                if (!hsInvoceType.ContainsKey(obj.ID))
                {
                    hsInvoceType.Add(obj.ID, obj.Name);
                }
            }
            hsInvoceType.Add("0000", "全部费用");
        }

        /// <summary>
        /// 查找所有发票分类
        /// </summary>
        /// <param name="patient"></param>
        private void QueryAllInvoceType()
        {
            AllInvoceType.Clear();
            ArrayList al = new ArrayList();
            al = this.feeManager.QueryAllFeeStatsByReportCode("ZY01");
            foreach (Neusoft.FrameWork.Models.NeuObject obj in al)
            {
                if (!this.AllInvoceType.ContainsKey(obj.Memo))
                {
                    AllInvoceType.Add(obj.Memo, obj);
                }
            }
        }

        /// <summary>
        /// 添加合计
        /// </summary>
        /// <param name="table">当前DataTalbe</param>
        /// <param name="totName">合计的名称位置</param>
        /// <param name="disName">合剂的名称</param>
        /// <param name="sumColName">统计列的数组</param>
        public void AddSumInfo(DataTable table, string totName, string disName, params string[] sumColName)
        {
            DataRow sumRow = table.NewRow();

            sumRow[totName] = disName;

            foreach (string s in sumColName)
            {
                object sum = table.Compute("SUM(" + s + ")", "");
                sumRow[s] = sum;
            }

            table.Rows.Add(sumRow);
        }

        /// <summary>
        /// 增加费用明细到数据表
        /// </summary>
        /// <param name="feeItemList"></param>
        public int AddFeeDetailToDataTable(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {

            if (this.ckShowBackDetail.Checked == false && feeItemList.NoBackQty == 0)
            {
                return -1;
            }
            DataRow dr = this.dtFeeDetail.NewRow();
            //this.fpFeeDetail_Sheet1.Rows.Add(0, 1);
            //dr["收费时间"] = feeItemList.ChargeOper.OperTime.ToString();
            if (feeItemList.FT.TotCost < 0)
            {
                dr["退费时间"] = feeItemList.FeeOper.OperTime.ToString();
            }
            dr["项目名称"] = this.GetItemName(feeItemList);
            dr["数量"] = feeItemList.FT.TotCost >= 0 ? feeItemList.Item.Qty : -feeItemList.Item.Qty;
            dr["可退数量"] = feeItemList.FT.TotCost > 0 ? feeItemList.NoBackQty : 0;
            dr["金额"] = feeItemList.FT.TotCost.ToString();
            dr["收费人"] = this.hashTablePeople.ContainsKey(feeItemList.FeeOper.ID) && this.hashTablePeople.ContainsKey(feeItemList.ChargeOper.ID) ? this.hashTablePeople[feeItemList.FeeOper.ID].ToString() : "系统";
            dr["开立科室"] = this.hashTableDept.ContainsKey(feeItemList.RecipeOper.Dept.ID) ? this.hashTableDept[feeItemList.RecipeOper.Dept.ID].ToString() : "系统";
            dr["开立医生"] = this.hashTablePeople.ContainsKey(feeItemList.RecipeOper.ID) ? this.hashTablePeople[feeItemList.RecipeOper.ID].ToString() : "系统";
            dr["处方号"] = feeItemList.RecipeNO;
            dr["退费"] = false;
            dr["费用来源"] = feeItemList.FTSource.ToString();
            dr["处方内流水号"] = feeItemList.SequenceNO.ToString();
            dr["交易类型"] = feeItemList.TransType.ToString();
            if (this.dtFeeDetail.Columns.Contains("单位"))
            {
                dr["单位"] = feeItemList.Item.PriceUnit;
            }
            if (this.dtFeeDetail.Columns.Contains("退费数量"))
            {
                dr["退费数量"] = 0.00M;
            }
            if (this.dtFeeDetail.Columns.Contains("应执行时间"))
            {
                dr["应执行时间"] = feeItemList.ExecOrder.DateUse;
            }
            if (this.dtFeeDetail.Columns.Contains("执行科室"))
            {
                dr["执行科室"] = this.hashTableDept.ContainsKey(feeItemList.ExecOper.Dept.ID) ? this.hashTableDept[feeItemList.ExecOper.Dept.ID].ToString() : "系统";
            }
            if (this.dtFeeDetail.Columns.Contains("取消申请"))
            {
                dr["取消申请"] = false;
            }


            this.dtFeeDetail.Rows.Add(dr);

            return 1;
        }

        /// <summary>
        /// 设置费用明细的格式
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <param name="row"></param>
        public void SetFpFeeDetailFormat(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList, int row, ReturnApply returnApply)
        {
            //this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["收费时间"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["应执行时间"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费时间"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["项目名称"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["数量"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["可退数量"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["单位"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["金额"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["收费人"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["开立科室"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["开立医生"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["费用来源"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["处方号"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["处方内流水号"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["交易类型"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["取消申请"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费"].Ordinal].Locked = true;
            this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Locked = true;
            if (this.dtFeeDetail.Columns.Contains("执行科室"))
            {
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["执行科室"].Ordinal].Locked = true;
            }

            //已结算的不允许在操作
            if (feeItemList.BalanceState == "1")
            {
                this.fpFeeDetail_Sheet1.RowHeader.Cells[row, 0].Text = "已结";
                this.fpFeeDetail_Sheet1.RowHeader.Cells[row, 0].ForeColor = Color.Red;
                return;
            }

            //处理已退信息，是否允许取消退费
            if (this.dtFeeDetail.Columns.Contains("已申请数量"))
            {
                numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                numberCellType.DecimalPlaces = 2;
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].CellType = numberCellType;
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Locked = true;
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["取消申请"].Ordinal].Locked = true;
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Value = 0.00M;
                if (feeItemList.TransType == Neusoft.HISFC.Models.Base.TransTypes.Positive)
                {
                    if (returnApply != null)
                    {
                        this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Value = returnApply.Item.Qty;
                    }
                    else
                    {
                        if (dicReturnApply.ContainsKey(feeItemList.RecipeNO + feeItemList.SequenceNO.ToString()))
                        {
                            if (dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO.ToString()] is Neusoft.HISFC.Models.Pharmacy.ApplyOut)
                            {
                                Neusoft.HISFC.Models.Pharmacy.ApplyOut applyOut = dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO.ToString()] as Neusoft.HISFC.Models.Pharmacy.ApplyOut;
                                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Value = applyOut.Operation.ApplyQty;
                                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["申请人"].Ordinal].Value = this.hashTablePeople.ContainsKey(applyOut.Operation.ApplyOper.ID) ? this.hashTablePeople[applyOut.Operation.ApplyOper.ID].ToString() : "系统";
                                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["确认人"].Ordinal].Value = this.hashTablePeople.ContainsKey(applyOut.Operation.ApproveOper.ID) ? this.hashTablePeople[applyOut.Operation.ApproveOper.ID].ToString() : "系统";
                                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["申请时间"].Ordinal].Value = applyOut.Operation.ApplyOper.OperTime.ToString();
                                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["确认时间"].Ordinal].Value = applyOut.Operation.ApproveOper.OperTime.ToString();
                            }
                            else if (dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO.ToString()] is Neusoft.HISFC.Models.Fee.ReturnApply)
                            {
                                returnApply = dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO.ToString()] as Neusoft.HISFC.Models.Fee.ReturnApply;
                                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Value = returnApply.Item.Qty;
                            }
                        }
                    }

                    if (Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Value) > 0)
                    {
                        this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["取消申请"].Ordinal].Locked = false;
                        this.fpFeeDetail_Sheet1.Rows[row].BackColor = Color.Aqua;
                    }
                }
            }

            //处理退费
            if (feeItemList.NoBackQty <= 0 || feeItemList.Item.Qty <= 0)
            {
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费"].Ordinal].Locked = true;
                if (feeItemList.Item.Qty <= 0)
                {
                    this.fpFeeDetail_Sheet1.Rows[row].BackColor = Color.Pink;
                }
            }
            else
            {
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费"].Ordinal].Locked = false;
                this.fpFeeDetail_Sheet1.Rows[row].BackColor = Color.White;
            }

            bool isQuitPriv = true;
            //处理退费权限的问题
            if (this.isCanQuitOtherOperator == false)
            {
                //判断当前登录人与收费人是否一致
                if (feeItemList.FeeOper.ID != this.feeManager.Operator.ID)
                {
                    isQuitPriv = false;
                }
            }

            //处理退费科室问题
            if (this.isCanQuitOtherDeptFee == false)
            {
                if (this.depts.ContainsKey(feeItemList.FeeOper.Dept.ID) == false)
                {
                    isQuitPriv = false;
                }
            }

            //最后是否允许退费
            if (isQuitPriv && this.dtFeeDetail.Columns.Contains("退费数量"))
            {
                numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                numberCellType.DecimalPlaces = 2;
                numberCellType.MinimumValue = 0.00F;
                numberCellType.MaximumValue = Double.Parse(feeItemList.NoBackQty.ToString("F2"));//只能退最大值为可退数量的
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费数量"].Ordinal].CellType = numberCellType;
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Locked = !isQuitPriv;
                this.fpFeeDetail_Sheet1.Cells[row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Value = 0.00F;
            }
        }

        /// <summary>
        /// 初始化DataTable
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitDataTable()
        {
            Type str = typeof(String);
            Type date = typeof(DateTime);
            Type dec = typeof(Decimal);
            Type bo = typeof(bool);

            this.fpMainInfo_Sheet1.DataAutoSizeColumns = false;
            this.fpPrepay_Sheet1.DataAutoSizeColumns = false;
            //this.fpFeeDaySum_Sheet1.DataAutoSizeColumns = false;
            this.fpFeeDetail_Sheet1.DataAutoSizeColumns = false;
            this.fpFee_Sheet1.DataAutoSizeColumns = false;
            this.fpBalance_Sheet1.DataAutoSizeColumns = false;
            this.fpShiftData_Sheet1.DataAutoSizeColumns = false;
            this.fpFeeItemSum_Sheet1.DataAutoSizeColumns = false;

            #region 住院主表信息

            if (System.IO.File.Exists(pathNameMainInfo))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameMainInfo, dtMainInfo, ref dvMainInfo, this.fpMainInfo_Sheet1);

                dtMainInfo.PrimaryKey = new DataColumn[] { dtMainInfo.Columns["住院流水号"] };

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpMainInfo_Sheet1, pathNameMainInfo);

            }
            else
            {

                dtMainInfo.Columns.AddRange(new DataColumn[]{new DataColumn("住院流水号", str),
																new DataColumn("住院号", str),
																new DataColumn("姓名", str),
																new DataColumn("住院科室", str),
																new DataColumn("床号", str),
																new DataColumn("患者类别", str),
																new DataColumn("预交金(未结)", dec),
																new DataColumn("费用合计(未结)", dec),
																new DataColumn("余额", dec),
																new DataColumn("自费", dec),
																new DataColumn("自负", dec),
																new DataColumn("公费", dec),
																new DataColumn("入院日期", date),
																new DataColumn("在院状态", str),
																new DataColumn("出院日期", str),
																new DataColumn("预交金(已结)", dec),
																new DataColumn("费用合计(已结)", dec),
																new DataColumn("结算日期", date)/*,
																new DataColumn("医疗类别", str)*/});

                dtMainInfo.PrimaryKey = new DataColumn[] { dtMainInfo.Columns["住院流水号"] };

                dvMainInfo = new DataView(dtMainInfo);

                this.fpMainInfo_Sheet1.DataSource = dvMainInfo;

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpMainInfo_Sheet1, pathNameMainInfo);
            }

            #endregion

            #region 费用项目汇总

            if (System.IO.File.Exists(this.pathNameFeeItemSum))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameFeeItemSum, dtFeeItemSum, ref dvFeeItemSum, this.fpFeeItemSum_Sheet1);

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpFeeItemSum_Sheet1, pathNameFeeItemSum);
            }
            else
            {
                dtFeeItemSum.Columns.AddRange(new DataColumn[]{
                                                                new DataColumn("项目名称", str),
																new DataColumn("总数量", dec),
																new DataColumn("金额", dec),
                                                                new DataColumn("项目编码", str),
                                                                new DataColumn("最小费用编码", str),
                                                                new DataColumn("拼音码", str),
                                                                new DataColumn("五笔码", str),
                                                                new DataColumn("自定义码", str),
                                                                new DataColumn("选中",bo)}
                                                                );

                dvFeeItemSum = new DataView(dtFeeItemSum);

                this.fpFeeItemSum_Sheet1.DataSource = dvFeeItemSum;

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFeeItemSum_Sheet1, this.pathNameFeeItemSum);
            }

            #endregion

            #region 费用当天汇总

            //if (System.IO.File.Exists(this.pathNameFeeDaySum))
            //{
            //    Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameFeeDaySum, dtFeeDaySum, ref dvFeeDaySum, this.fpFeeDaySum_Sheet1);

            //    Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpFeeDaySum_Sheet1, pathNameFeeDaySum);
            //}
            //else
            //{
            //    dtFeeDaySum.Columns.AddRange(new DataColumn[]{
            //                                                    new DataColumn("收费时间", date),
            //                                                    new DataColumn("项目名称", str),
            //                                                    new DataColumn("收费数量", dec),
            //                                                    new DataColumn("金额", dec),
            //                                                    new DataColumn("项目编码", str)}
            //                                                    );

            //    dvFeeDaySum = new DataView(dtFeeDaySum);

            //    this.fpFeeDaySum_Sheet1.DataSource = dvFeeDaySum;

            //    Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFeeDaySum_Sheet1, this.pathNameFeeDaySum);
            //}

            #endregion

            #region 费用明细信息
            if (System.IO.File.Exists(this.pathNameFeeDetail))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameFeeDetail, dtFeeDetail, ref dvFeeDetail, this.fpFeeDetail_Sheet1);

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpFeeDetail_Sheet1, pathNameFeeDetail);
            }
            else
            {
                dtFeeDetail.Columns.AddRange(new DataColumn[]{
                                                                  new DataColumn("收费时间", str),
                                                                  new DataColumn("退费时间", str),
																  new DataColumn("项目名称", str),
																  new DataColumn("数量", dec),
																  new DataColumn("可退数量", dec),
																  new DataColumn("单位", str),
																  new DataColumn("金额", dec),
																  new DataColumn("收费人", str),
																  new DataColumn("开立科室", str),
																  new DataColumn("开立医生", str),
                                                                  new DataColumn("应执行时间", str),
																  new DataColumn("执行科室", str),
				                                                  new DataColumn("费用来源", str),
				                                                  new DataColumn("退费", bo),
				                                                  new DataColumn("退费数量", dec),
				                                                  new DataColumn("取消申请", bo),
				                                                  new DataColumn("已申请数量", dec),
				                                                  new DataColumn("申请人", str),
				                                                  new DataColumn("申请时间", str),
				                                                  new DataColumn("确认人", str),
				                                                  new DataColumn("确认时间", str),
				                                                  new DataColumn("处方号", str),
				                                                  new DataColumn("处方内流水号", str),
                                                                  new DataColumn("交易类型", str)});

                dvFeeDetail = new DataView(dtFeeDetail);

                this.fpFeeDetail_Sheet1.DataSource = dvFeeDetail;

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFeeDetail_Sheet1, pathNameFeeDetail);
            }

            #endregion

            #region 预交金信息

            if (System.IO.File.Exists(pathNamePrepay))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNamePrepay, dtPrepay, ref dvPrepay, this.fpPrepay_Sheet1);

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpPrepay_Sheet1, pathNamePrepay);
            }
            else
            {
                dtPrepay.Columns.AddRange(new DataColumn[]{new DataColumn("票据号", str),
															  new DataColumn("预交金额", dec),
															  new DataColumn("支付方式", str),
															  new DataColumn("操作员", str),
															  new DataColumn("操作日期", date),
															  new DataColumn("所在科室", str),
															  new DataColumn("结算状态", str),
															  new DataColumn("来源", str)});

                dvPrepay = new DataView(dtPrepay);

                this.fpPrepay_Sheet1.DataSource = dvPrepay;
                dvPrepay.Sort = "票据号 ASC";

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpPrepay_Sheet1, pathNamePrepay);
            }

            #endregion

            #region 最小费用信息

            if (System.IO.File.Exists(pathNameFee))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameFee, dtFee, ref dvFee, this.fpFee_Sheet1);

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpFee_Sheet1, pathNameFee);
            }
            else
            {
                dtFee.Columns.AddRange(new DataColumn[]{new DataColumn("费用名称", str),
														   new DataColumn("金额", dec),
														   new DataColumn("自费", dec),
														   new DataColumn("公费", dec),
														   new DataColumn("自负", dec),
														   new DataColumn("优惠金额", dec),
														   new DataColumn("结算状态", str),
                 new DataColumn("费用代码", str)});

                dvFee = new DataView(dtFee);

                this.fpFee_Sheet1.DataSource = dvFee;

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFee_Sheet1, pathNameFee);
            }

            #endregion

            #region 结算信息
            if (System.IO.File.Exists(pathNameBalance))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameBalance, dtBalance, ref dvBalance, this.fpBalance_Sheet1);

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpBalance_Sheet1, pathNameBalance);
            }
            else
            {
                dtBalance.Columns.AddRange(new DataColumn[]{new DataColumn("发票号码", str),
															   new DataColumn("结算类型", str),
															   new DataColumn("结算状态", str),
															   new DataColumn("预交金额", dec),
															   new DataColumn("总金额", dec),
															   new DataColumn("个人支付", dec),
															   new DataColumn("基金支付", dec),
															   new DataColumn("优惠", dec),
															   new DataColumn("返还金额", dec),
															   new DataColumn("补收金额", dec),
															   new DataColumn("结算时间", date),
															   new DataColumn("操作员", str)});

                dvBalance = new DataView(dtBalance);

                this.fpBalance_Sheet1.DataSource = dvBalance;

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpBalance_Sheet1, pathNameBalance);
            }
            #endregion

            #region 变更信息

            if (System.IO.File.Exists(pathNameShiftData))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.CreatColumnByXML(pathNameShiftData, dtShiftData, ref dvShiftData, this.fpShiftData_Sheet1);
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.fpShiftData_Sheet1, pathNameShiftData);
            }
            else
            {
                dtShiftData.Columns.AddRange(new DataColumn[]{new DataColumn("变更类型", str),
															   new DataColumn("变更名称", str),
															   new DataColumn("变更前编码", str),
															   new DataColumn("变更前名称", str),
															   new DataColumn("变更后编码", str),
															   new DataColumn("变更后名称", str),
															   new DataColumn("变更时间", str),//要显示时间点不可用date
															   new DataColumn("变更人", str)});

                dvShiftData = new DataView(dtShiftData);

                this.fpShiftData_Sheet1.DataSource = dvShiftData;

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpShiftData_Sheet1, this.pathNameShiftData);
            }

            #endregion

            this.InitHashTable();

            return 1;
        }

        /// <summary>
        /// 显示患者基本信息
        /// </summary>
        protected void SetPatientInfo()
        {
            if (this.currentPatient == null || this.currentPatient.ID == null || this.currentPatient.ID == string.Empty)
            {
                return;
            }

            this.ucQueryInpatientNo1.Text = this.currentPatient.PID.PatientNO;//住院号
            this.lblName.Text = this.currentPatient.Name;//姓名;
            this.lblSex.Text = this.currentPatient.Sex.Name;
            this.lblAge.Text = this.currentPatient.Age;
            this.lblBed.Text = this.currentPatient.PVisit.PatientLocation.Bed.ID.Length > 4 ? this.currentPatient.PVisit.PatientLocation.Bed.ID.Substring(4) : this.currentPatient.PVisit.PatientLocation.Bed.ID;
            this.lblDept.Text = this.currentPatient.PVisit.PatientLocation.Dept.Name;

            if (this.currentPatient.Pact.PayKind.ID == "03")
            {
                Neusoft.HISFC.Models.Base.PactInfo pact = this.PactManagment.GetPactUnitInfoByPactCode(this.currentPatient.Pact.ID);
                this.lblPact.Text = this.currentPatient.Pact.Name + " 自付比例：" + pact.Rate.PayRate * 100 + "%" + " 日限额:" + this.currentPatient.FT.DayLimitCost;//合同单位
            }
            else
            {
                this.lblPact.Text = this.currentPatient.Pact.Name;//合同单位
            }

            this.lblDateIn.Text = this.currentPatient.PVisit.InTime.ToShortDateString();//住院日期
            if (this.currentPatient.PVisit.OutTime != DateTime.MinValue)
            {
                this.lblOutDate.Text = this.currentPatient.PVisit.OutTime.ToShortDateString();
            }
            this.lblInState.Text = this.currentPatient.PVisit.InState.Name;//在院状态
            decimal TotCost = this.currentPatient.FT.TotCost + this.currentPatient.FT.BalancedCost;
            //this.lblTotCost.Text = this.currentPatient.FT.TotCost.ToString();
            this.lblTotCost.Text = TotCost.ToString();


            //中大五院本地化费用控制相关显示
            ArrayList al = this.radtManager.GetAlertPersonZDWY("ALL", 99999999, this.currentPatient.ID);
            if (al.Count == 0)
            {
                this.neuLabel21.Visible = false;
                this.lblOwnCost.Visible = false;
                this.neuLabel23.Visible = false;
                this.lblPubCost.Visible = false;
            }
            else
            {
                this.myPatientInfoTemp = al[0] as Neusoft.HISFC.Models.RADT.PatientInfo;
                this.neuLabel21.Visible = true;
                this.lblOwnCost.Visible = true;
                this.neuLabel23.Visible = true;
                this.lblPubCost.Visible = true;
                this.lblOwnCost.Text = this.myPatientInfoTemp.FT.OwnCost.ToString();
                this.lblPubCost.Text = this.myPatientInfoTemp.FT.PubCost.ToString();
            }
            this.lblPrepayCost.Text = this.currentPatient.FT.PrepayCost.ToString();
            this.lblUnBalanceCost.Text = this.currentPatient.FT.TotCost.ToString();
            this.lblBalancedCost.Text = this.currentPatient.FT.BalancedCost.ToString();
            if ((this.currentPatient.FT.PrepayCost + this.currentPatient.FT.BalancedCost - this.myPatientInfoTemp.FT.OwnCost) < 0)//剩余金额少于0，字体变为红色
            {
                this.lblFreeCost.ForeColor = Color.Red;
                this.lblFreeCost.Text = (this.currentPatient.FT.PrepayCost + this.currentPatient.FT.BalancedCost - this.myPatientInfoTemp.FT.OwnCost).ToString(); //this.currentPatient.FT.LeftCost.ToString();
            }
            else
            {
                this.lblFreeCost.ForeColor = Color.LightSteelBlue ;
                this.lblFreeCost.Text = (this.currentPatient.FT.PrepayCost + this.currentPatient.FT.BalancedCost - this.myPatientInfoTemp.FT.OwnCost).ToString(); //this.currentPatient.FT.LeftCost.ToString();
            }
            this.lblDiagnose.Text = this.currentPatient.ClinicDiagnose;     //入院诊断-门诊诊断；而不是患者在院的主诊断
            this.lblMemo.Text = this.currentPatient.Memo;   //备注需要显示出来

            //取入院天数
            int day = this.radtIntegrate.GetInDays(this.currentPatient.ID);
            if (day > 1)
            {
                day -= 1;
            }
            this.txtDays.Text = day.ToString();
            //取接诊时间
            this.txtArriveDate.Text = this.radtIntegrate.GetArriveDate(this.currentPatient.ID).ToShortDateString();

            //取在院诊断
            try
            {
                ArrayList alInDiag = this.diagMgr.QueryDiagnoseByInpatientNOAndPersonType(this.currentPatient.ID, "1");
                if (alInDiag != null && alInDiag.Count > 0)
                {
                    //取最新的在院诊断
                    this.lblInDiagInfo.Text = (alInDiag[0] as Neusoft.HISFC.Models.HealthRecord.Diagnose).DiagInfo.ICD10.Name;
                }
            }
            catch (Exception ex) { }


        }

        /// <summary>
        /// 查询所有信息
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        protected void QueryAllInfomaition(DateTime beginTime, DateTime endTime)
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在查询信息，请稍后...");
            Application.DoEvents();
            hsMinFee.Clear();
            hsDayFeeItemList.Clear();
            dicReturnApply.Clear();

            this.QueryInvoceType();
            this.QueryAllInvoceType();

            if (this.neuTabControl1.TabPages.Contains(this.tpFeeInfo))
            {
                this.QueryPatientReturyApply(this.currentPatient);
                this.QueryPatientDrugList(beginTime, endTime);
                this.QueryPatientUndrugList(beginTime, endTime);
                this.QueryPatientFeeInfo(beginTime, endTime);
            }

            if (this.neuTabControl1.TabPages.Contains(this.tpMainInfo))
            {
                this.QueryPatientPrepayList(beginTime, endTime);
                this.QueryPatientBalance(beginTime, endTime);
                this.QueryPatientShiftData(this.currentPatient);
            }

            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
        }

        /// <summary>
        /// 返回"_"
        /// </summary>
        /// <param name="langth">长度</param>
        /// <returns>成功 "---" 失败 null</returns>
        private string RetrunSplit(int langth)
        {
            string s = string.Empty;

            for (int i = 0; i < langth; i++)
            {
                s += "_";
            }

            return s;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            this.ucQueryInpatientNo1.Text = this.RetrunSplit(10);
            this.lblName.Text = this.RetrunSplit(5);
            this.lblSex.Text = this.RetrunSplit(4);
            this.lblAge.Text = this.RetrunSplit(4);
            this.lblBed.Text = this.RetrunSplit(10);
            this.lblDept.Text = this.RetrunSplit(10);
            this.lblPact.Text = this.RetrunSplit(10);
            this.lblDateIn.Text = this.RetrunSplit(10);
            this.lblOutDate.Text = this.RetrunSplit(10);

            this.lblInState.Text = this.RetrunSplit(6);
            this.lblTotCost.Text = this.RetrunSplit(10);
            this.lblOwnCost.Text = this.RetrunSplit(10);
            this.lblPubCost.Text = this.RetrunSplit(10);
            this.lblPrepayCost.Text = this.RetrunSplit(10);
            this.lblUnBalanceCost.Text = this.RetrunSplit(10);
            this.lblBalancedCost.Text = this.RetrunSplit(10);
            this.lblDiagnose.Text = this.RetrunSplit(20);
            this.lblMemo.Text = this.RetrunSplit(30);
            this.txtArriveDate.Text = this.RetrunSplit(20);
            this.txtDays.Text = this.RetrunSplit(3);
            this.lblInDiagInfo.Text = this.RetrunSplit(30);  //在院诊断

            dtMainInfo.Rows.Clear();
            dtFeeDaySum.Rows.Clear();
            dtFeeDetail.Rows.Clear();
            dtPrepay.Rows.Clear();
            dtFee.Rows.Clear();
            dtBalance.Rows.Clear();
            dtShiftData.Clear();
            dtFeeItemSum.Rows.Clear();
            hsQuitFeeByPackage.Clear();
            dicReturnApply.Clear();
            alAllFeeItemLists = new ArrayList();
        }

        private void InitContr()
        {
            //this.neuLabel21.Visible = false;
            //this.lblOwnCost.Visible = false;
            //this.neuLabel23.Visible = false;
            //this.lblPubCost.Visible = false;
        }

        protected override int OnSetValue(object neuObject, TreeNode e)
        {
            if (neuObject is Neusoft.HISFC.Models.RADT.PatientInfo)
            {
                if (this.currentPatient != null && this.currentPatient.ID.Equals(((Neusoft.HISFC.Models.RADT.PatientInfo)neuObject).ID))
                {
                    return 1;
                }

                this.PatientInfo = neuObject as Neusoft.HISFC.Models.RADT.PatientInfo;
            }

            return base.OnSetValue(neuObject, e);
        }

        protected override int OnSetValues(ArrayList alValues, object e)
        {
            if (alValues != null && alValues.Count > 0)
            {
                this.Clear();
                foreach (Neusoft.HISFC.Models.RADT.PatientInfo patientInfo in alValues)
                {
                    this.SetPatientToFpMain(patientInfo);
                }
            }
            return base.OnSetValues(alValues, e);
        }

        private void SelectFeeInfo()
        {
            if (this.fpFee_Sheet1.RowCount > 0)
            {
                this.fpFee_Sheet1.ActiveRowIndex = 0;
                this.fpFee_Sheet1.AddSelection(0, 0, 1, 1);
                this.fpFee_SelectionChanged(null, null);

            }
        }

        private void SelectFeeItemSum(string feeCode)
        {
            if (this.fpFeeItemSum_Sheet1.RowCount > 0)
            {
                //如果当前选择的编码=第0行 则不变
                if (this.fpFeeItemSum_Sheet1.ActiveRowIndex < 0)
                {
                    this.fpFeeItemSum_Sheet1.ActiveRowIndex = 0;
                    this.fpFeeItemSum_Sheet1.AddSelection(0, 0, 1, 1);
                    this.fpFeeItemSum_SelectionChanged(null, null);
                }
                else
                {
                    this.fpFeeItemSum_Sheet1.ActiveRowIndex = 0;
                    this.fpFeeItemSum_Sheet1.AddSelection(0, 0, 1, 1);
                    if (this.dtFeeDaySum.Rows.Count == 0 || feeCode != this.fpFeeItemSum_Sheet1.Cells[0, this.dtFeeItemSum.Columns["项目编码"].Ordinal].Text)
                    {
                        this.fpFeeItemSum_SelectionChanged(null, null);
                    }
                }
            }
        }

        private void SelectFeeItemDaySum()
        {
            //this.fpFeeDaySum_Sheet1.SortRows(0, false, true);
            //if (this.fpFeeDaySum_Sheet1.RowCount > 0)
            //{
            //    this.fpFeeDaySum_Sheet1.ActiveRowIndex = 0;
            //    this.fpFeeDaySum_Sheet1.AddSelection(0, 0, 1, 1);
            //    this.fpFeeDaySum_SelectionChanged(null, null);
            //}
        }

        /// <summary>
        /// 根据费用信息获取组合项目的唯一值
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private string GetPackageItemKey(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {
            //是否按照明细显示
            if (this.ckUnCombo.Checked)
            {
                return feeItemList.UndrugComb.ID + feeItemList.TransType.ToString() + feeItemList.FeeOper.OperTime.ToString("yyyyMMddHHmm") + feeItemList.ExecOrder.ID;
            }
            else
            {
                return feeItemList.Item.ID + feeItemList.TransType.ToString() + feeItemList.FeeOper.OperTime.ToString("yyyyMMddHHmm") + feeItemList.ExecOrder.ID;
            }
        }
        /// <summary>
        /// 根据费用信息获取组合项目的唯一值
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private string GetPackageItemKey(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList, Neusoft.HISFC.Models.Base.TransTypes transType)
        {
            //是否按照明细显示
            if (this.ckUnCombo.Checked)
            {
                return feeItemList.UndrugComb.ID + transType.ToString() + feeItemList.FeeOper.OperTime.ToString("yyyyMMddHHmm") + feeItemList.ExecOrder.ID;
            }
            else
            {
                return feeItemList.Item.ID + transType.ToString() + feeItemList.FeeOper.OperTime.ToString("yyyyMMddHHmm") + feeItemList.ExecOrder.ID;
            }
        }

        /// <summary>
        /// 根据费用信息获取组合项目的唯一值
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private string GetPackageItemQuitKey(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {
            //是否按照明细显示
            if (this.ckUnCombo.Checked)
            {
                return feeItemList.UndrugComb.ID + "|" + feeItemList.TransType.ToString() + "|" + feeItemList.RecipeNO + feeItemList.FeeOper.OperTime.ToString("yyyyMMddHHmm") + feeItemList.ExecOrder.ID;
            }
            else
            {
                return feeItemList.Item.ID + "|" + feeItemList.TransType.ToString() + "|" + feeItemList.RecipeNO + feeItemList.FeeOper.OperTime.ToString("yyyyMMddHHmm") + feeItemList.ExecOrder.ID;
            }
        }

        /// <summary>
        /// 获取项目的最小费用
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private string GetItemFeeCode(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {
            //if (this.ckUnCombo.Checked)
            //{
            //    string id = string.IsNullOrEmpty(feeItemList.UndrugComb.MinFee.ID) ? feeItemList.Item.MinFee.ID : feeItemList.UndrugComb.MinFee.ID;
            //    Neusoft.FrameWork.Models.NeuObject obj = AllInvoceType[id] as Neusoft.FrameWork.Models.NeuObject;
            //    return obj.ID;
            //}
            //else
            //{
            //    Neusoft.FrameWork.Models.NeuObject obj = AllInvoceType[feeItemList.Item.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
            //    return obj.ID;
            //}

            if (this.ckUnCombo.Checked)
            {
                if (string.IsNullOrEmpty(feeItemList.UndrugComb.MinFee.ID))
                {
                    return feeItemList.Item.MinFee.ID;
                }
                else
                {
                    //Neusoft.FrameWork.Models.NeuObject obj = AllInvoceType[feeItemList.UndrugComb.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                    return feeItemList.UndrugComb.MinFee.ID;
                }
            }
            else
            {
                return feeItemList.Item.MinFee.ID;
            }

        }

        /// <summary>
        /// 获取项目的ID
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private string GetItemID(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {
            if (this.ckUnCombo.Checked)
            {
                return string.IsNullOrEmpty(feeItemList.UndrugComb.ID) ? feeItemList.Item.ID : feeItemList.UndrugComb.ID;
            }
            else
            {
                return feeItemList.Item.ID;
            }
        }

        /// <summary>
        /// 获取复合项目名称
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private string GetItemName(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {
            if (this.ckUnCombo.Checked == false)
            {
                return string.IsNullOrEmpty(feeItemList.UndrugComb.Name) ? feeItemList.Item.Name : feeItemList.UndrugComb.Name + "/" + feeItemList.Item.Name;
            }
            else
            {
                return feeItemList.Item.Name;
            }
        }

        /// <summary>
        /// 是否复合项目显示
        /// </summary>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        private bool IsPackageItem(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList)
        {
            return this.ckUnCombo.Checked ? string.IsNullOrEmpty(feeItemList.UndrugComb.ID) == false : false;
        }

        #endregion


        /// <summary>
        /// 增加ToolBar控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("刷新", "刷新患者信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.S刷新, true, false, null);

            return this.toolBarService;
        }

        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "刷新":

                    if (this.tv != null)
                    {
                        this.tv.Refresh();
                    }
                    break;
                default:
                    break;
            }
            base.ToolStrip_ItemClicked(sender, e);
        }

        protected override int OnPrint(object sender, object neuObject)
        {
            Neusoft.FrameWork.WinForms.Classes.Print print = new Neusoft.FrameWork.WinForms.Classes.Print();

            print.PrintPage(0, 0, this.neuTabControl1.SelectedTab);

            return base.OnPrint(sender, neuObject);
        }
        //打印预览
        public override int PrintPreview(object sender, object neuObject)
        {
            Neusoft.FrameWork.WinForms.Classes.Print printview = new Neusoft.FrameWork.WinForms.Classes.Print();

            printview.PrintPreview(0, 0, this.neuTabControl1.SelectedTab);
            return base.OnPrintPreview(sender, neuObject);
        }

        public override int Export(object sender, object neuObject)
        {
            object obj = this.hashTableFp[this.neuTabControl1.SelectedTab];

            FarPoint.Win.Spread.FpSpread fp = obj as FarPoint.Win.Spread.FpSpread;

            SaveFileDialog op = new SaveFileDialog();

            op.Title = "请选择保存的路径和名称";
            op.CheckFileExists = false;
            op.CheckPathExists = true;
            op.DefaultExt = "*.xls";
            op.Filter = "(*.xls)|*.xls";

            DialogResult result = op.ShowDialog();

            if (result == DialogResult.Cancel || op.FileName == string.Empty)
            {
                return -1;
            }

            string filePath = op.FileName;

            bool returnValue = fp.SaveExcel(filePath, FarPoint.Win.Spread.Model.IncludeHeaders.ColumnHeadersCustomOnly);


            return base.Export(sender, neuObject);
        }

        private void ucPatientFeeQuery_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            this.InitDataTable();
            this.InitContr();

            this.hashTableDept.Clear();
            this.hashTablePeople.Clear();

            ArrayList tdlist = deptManager.GetDeptAllUserStopDisuse();
            foreach (Neusoft.HISFC.Models.Base.Department tempinfo in tdlist)
            {
                hashTableDept.Add(tempinfo.ID, tempinfo.Name);
            }

            ArrayList tplist = personManager.GetEmployeeAll();
            foreach (Neusoft.HISFC.Models.Base.Employee tempinfo in tplist)
            {
                hashTablePeople.Add(tempinfo.ID, tempinfo.Name);
            }

            //默认勾上显示费用明细
            this.ckShowBackDetail.Checked = true;

        }

        private void fpMainInfo_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (this.fpMainInfo_Sheet1.RowCount <= 0)
            {
                return;
            }
            string inpatientNO = this.fpMainInfo_Sheet1.Cells[this.fpMainInfo_Sheet1.ActiveRowIndex, 0].Text;

            if (this.currentPatient == null || this.currentPatient.ID.Equals(inpatientNO) == false)
            {
                if (inpatientNO == null)
                {
                    return;
                }

                this.currentPatient = this.radtManager.QueryPatientInfoByInpatientNO(inpatientNO);
                if (this.currentPatient == null || this.currentPatient.ID == null || this.currentPatient.ID == string.Empty)
                {
                    MessageBox.Show(Language.Msg("查询患者基本信息出错!") + this.radtManager.Err);

                    return;
                }

                this.SetPatientInfo();

                dtFeeDaySum.Rows.Clear();
                dtFeeDetail.Rows.Clear();
                dtFeeItemSum.Rows.Clear();
                dtPrepay.Rows.Clear();
                dtFee.Rows.Clear();
                dtBalance.Rows.Clear();
                dtShiftData.Rows.Clear();

                //设置查询时间
                DateTime beginTime = this.currentPatient.PVisit.InTime;
                DateTime endTime = this.radtManager.GetDateTimeFromSysDateTime();

                this.QueryAllInfomaition(beginTime, endTime);
            }
        }

        private void fpMainInfo_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            this.fpMainInfo_SelectionChanged(sender, null);
            this.neuTabControl1.SelectedTab = this.tpFeeInfo;
        }

        private void fpFee_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            this.fpFeeItemSum_Sheet1.RowCount = 0;
            this.dtFeeItemSum.Clear();

            alAllFeeItemLists.Clear();

            this.txtItemFilter.Text = "";
            this.dvFeeItemSum.RowFilter = "1=1";


            this.dtFeeDaySum.Clear();
            this.dtpItemDayFilter.Checked = false;
            this.dvFeeDaySum.RowFilter = "1=1";

            this.fpFeeDetail_Sheet1.RowCount = 0;
            this.dtFeeDetail.Clear();

            string feeCode = this.fpFee_Sheet1.Cells[this.fpFee_Sheet1.ActiveRowIndex, 7].Text;
            string balanceState = this.fpFee_Sheet1.Cells[this.fpFee_Sheet1.ActiveRowIndex, 6].Text;

            this.invotypecost.Text = "金额:" + this.fpFee_Sheet1.Cells[this.fpFee_Sheet1.ActiveRowIndex, 1].Text;

            Hashtable hsItemsItemSum = new Hashtable();
            if (this.hsMinFee.ContainsKey(feeCode))
            {
                ArrayList al = this.hsMinFee[feeCode] as ArrayList;
                alAllFeeItemLists = (ArrayList)al.Clone();
            }

            if (feeCode == "0000")
            {

                foreach (DictionaryEntry de in hsMinFee)
                {
                    ArrayList al = de.Value as ArrayList;
                    this.alAllFeeItemLists.AddRange((ArrayList)al.Clone());
                }


                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList allitem = new FeeItemList();
                allitem.FT.TotCost = totcost;
                DataRow alldital = this.dtFeeItemSum.NewRow();
                alldital["项目名称"] = "全部项目明细";
                alldital["总数量"] = "0";
                alldital["金额"] = totcost.ToString();
                alldital["项目编码"] = "ALL";
                alldital["最小费用编码"] = "0000";
                alldital["拼音码"] = "";
                alldital["五笔码"] = "";
                alldital["自定义码"] = "";

                dtFeeItemSum.Rows.Add(alldital);// .InsertAt(alldital, 0);

            }


            //价格集合
            Dictionary<string, decimal> priceCollection = new Dictionary<string, decimal>();
            Dictionary<string, string> priceItemCollection = new Dictionary<string, string>();
            Dictionary<string, string> packageCollection = new Dictionary<string, string>();

            for (int i = alAllFeeItemLists.Count - 1; i >= 0; i--)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList item = this.alAllFeeItemLists[i] as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                if (this.GetItemFeeCode(item) != feeCode && feeCode != "0000")
                    continue;

                string itemCode = item.Item.ID;
                string itemName = item.Item.Name;

                #region 显示复合项目/明细
                if (this.IsPackageItem(item))
                {
                    string key = this.GetPackageItemKey(item);
                    if (item.UndrugComb != null && item.UndrugComb.ID.Length > 0)
                    {
                        itemCode = item.UndrugComb.ID;
                        itemName = item.UndrugComb.Name;
                        if (priceCollection.ContainsKey(key))
                        {
                            if (priceItemCollection.ContainsKey(key + item.Item.ID) == false)
                            {
                                priceCollection[key] += Math.Abs(item.FT.TotCost);
                                priceItemCollection[key + item.Item.ID] = item.Item.ID;
                            }
                        }
                        else
                        {
                            priceCollection[key] = Math.Abs(item.FT.TotCost);
                            priceItemCollection[key + item.Item.ID] = item.Item.ID;
                        }
                    }
                }
                #endregion

                if (!hsItemsItemSum.Contains(itemCode))
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList itemSum = new Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList();

                    itemSum.FeeOper.OperTime = item.FeeOper.OperTime;
                    itemSum.Item.ID = itemCode;
                    itemSum.Item.Name = itemName;
                    itemSum.RecipeNO = item.RecipeNO;
                    itemSum.TransType = item.TransType;
                    itemSum.NoBackQty = item.NoBackQty;
                    itemSum.UndrugComb = item.UndrugComb.Clone();
                    itemSum.ExecOrder.ID = item.ExecOrder.ID;

                    //重取项目
                    Neusoft.HISFC.Models.Base.Item baseItem = null;
                    if (item.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                        baseItem = CommonController.Instance.GetItem(itemCode);
                    }
                    else if (item.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        baseItem = this.phamarcyIntegrate.GetItem(itemCode);
                    }

                    itemSum.UndrugComb = item.UndrugComb.Clone();
                    if (this.IsPackageItem(item))
                    {
                        itemSum.Item.Qty = item.Item.Qty > 0 ? 1 : -1;
                        packageCollection.Add(this.GetPackageItemKey(item), null);
                    }
                    else
                    {
                        itemSum.Item.Qty = item.Item.Qty;
                    }

                    itemSum.FT.TotCost = item.FT.TotCost;
                    itemSum.FT.OwnCost = item.FT.OwnCost;
                    itemSum.FT.PubCost = item.FT.PubCost;
                    itemSum.Item.SpellCode = (baseItem == null ? string.Empty : baseItem.SpellCode);//拼音码为空时异常
                    itemSum.Item.WBCode = (baseItem == null ? string.Empty : baseItem.WBCode);
                    itemSum.Item.UserCode = (baseItem == null ? string.Empty : baseItem.UserCode);
                    itemSum.Item.Specs = (baseItem == null ? string.Empty : baseItem.Specs);
                    itemSum.FeeOper.OperTime = item.FeeOper.OperTime;
                    itemSum.ExecOrder.ID = item.ExecOrder.ID;

                    hsItemsItemSum.Add(itemSum.Item.ID, itemSum);
                }
                else
                {

                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList itemSum = hsItemsItemSum[itemCode] as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                    if (this.IsPackageItem(item))
                    {
                        string key = this.GetPackageItemKey(item);
                        if (packageCollection.ContainsKey(key) == false && item.UndrugComb.Qty != 0)
                        {
                            itemSum.UndrugComb.Qty += item.UndrugComb.Qty;
                            packageCollection.Add(key, null);
                        }
                    }
                    else
                    {
                        itemSum.Item.Qty += item.Item.Qty;
                    }
                    itemSum.FT.TotCost += item.FT.TotCost;
                    itemSum.FT.OwnCost += item.FT.OwnCost;
                    itemSum.FT.PubCost += item.FT.PubCost;
                }
            }

            foreach (System.Collections.DictionaryEntry objOrder in hsItemsItemSum)
            {
                //this.fpFeeSum_Sheet1.Rows.Add(0, 1);
                DataRow dr = this.dtFeeItemSum.NewRow();

                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList itemSum = objOrder.Value as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;

                dr["项目名称"] = this.GetItemName(itemSum) + (string.IsNullOrEmpty(itemSum.Item.Specs) ? "" : "[" + itemSum.Item.Specs + "]");
                if (itemSum.UndrugComb != null && itemSum.UndrugComb.ID.Length > 0)
                {
                    string key = this.GetPackageItemKey(itemSum);
                    string keyPositive = this.GetPackageItemKey(itemSum, Neusoft.HISFC.Models.Base.TransTypes.Positive);
                    if (itemSum.UndrugComb.Qty != 0)
                    {
                        dr["总数量"] = itemSum.UndrugComb.Qty;
                    }
                    else if (priceCollection.ContainsKey(keyPositive))
                    {

                        dr["总数量"] =
                               Neusoft.FrameWork.Public.String.FormatNumberReturnString(itemSum.FT.TotCost / priceCollection[keyPositive], 0);
                    }
                    else if (priceCollection.ContainsKey(key))
                    {
                        dr["总数量"] =
                               Neusoft.FrameWork.Public.String.FormatNumberReturnString(itemSum.FT.TotCost / priceCollection[key], 0);
                    }
                    else
                    {
                        dr["总数量"] = itemSum.Item.Qty;
                    }

                }
                else
                {
                    dr["总数量"] = itemSum.Item.Qty.ToString();
                }
                dr["金额"] = itemSum.FT.TotCost.ToString();
                dr["项目编码"] = itemSum.Item.ID;
                dr["最小费用编码"] = this.GetItemFeeCode(itemSum);
                dr["拼音码"] = itemSum.Item.SpellCode;
                dr["五笔码"] = itemSum.Item.WBCode;
                dr["自定义码"] = itemSum.Item.UserCode;

                dtFeeItemSum.Rows.Add(dr);
            }

            dtFeeItemSum.AcceptChanges();
            //this.fpFeeItemSum_Sheet1.SortRows(0, true, true);
            this.SelectFeeItemSum("");
        }

        private void fpFeeItemSum_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            alAllFeeItemLists.Clear();

            this.dtFeeDaySum.Clear();
            this.dtpItemDayFilter.Checked = false;


            this.fpFeeDetail_Sheet1.RowCount = 0;
            this.dtFeeDetail.Clear();

            string feeCode = this.fpFeeItemSum_Sheet1.Cells[this.fpFeeItemSum_Sheet1.ActiveRowIndex, 4].Text;

            this.itemcost.Text = "金额:" + this.fpFeeItemSum_Sheet1.Cells[this.fpFeeItemSum_Sheet1.ActiveRowIndex, 2].Text;

            //Neusoft.FrameWork.Models.NeuObject invotype = AllInvoceType[feeCode] as Neusoft.FrameWork.Models.NeuObject;
            //feeCode = invotype.ID;

            string itemCode = this.fpFeeItemSum_Sheet1.Cells[this.fpFeeItemSum_Sheet1.ActiveRowIndex, 3].Text;
            Hashtable hsItemsDaySum = new Hashtable();
            if (this.hsMinFee.ContainsKey(feeCode))
            {
                //this.alAllFeeItemLists = this.hsMinFee[feeCode] as ArrayList;
                ArrayList al = this.hsMinFee[feeCode] as ArrayList;
                alAllFeeItemLists = (ArrayList)al.Clone();
            }
            if (feeCode == "0000")
            {
                alAllFeeItemLists.Clear();
                foreach (DictionaryEntry de in hsMinFee)
                {
                    ArrayList al = de.Value as ArrayList;
                    this.alAllFeeItemLists.AddRange((ArrayList)al.Clone());
                }
            }


            //价格集合
            //Dictionary<string, decimal> priceCollection = new Dictionary<string, decimal>();
            //Dictionary<string, string> priceItemCollection = new Dictionary<string, string>();
            //Dictionary<string, string> packageCollection = new Dictionary<string, string>();

            //Hashtable 用来判断是否组套项目
            Hashtable hsPackageItem = new Hashtable();
            hsQuitFeeByPackage.Clear();
            //退费数量的格式
            numberCellType.DecimalPlaces = 2;//保留两位小数

            //ArrayList sortlist = (ArrayList)this.alAllFeeItemLists.Clone();

            //var list = from Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList sortt in sortlist orderby sortt.Item.ID select sortt;
            //this.alAllFeeItemLists = (ArrayList)list;

            if (feeCode == "0000")
            {
                #region 所有费用
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList item in this.alAllFeeItemLists)
                {

                    //重取数据库
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList = this.feeManager.GetItemListByRecipeNO(item.RecipeNO, item.SequenceNO, item.TransType, item.Item.ItemType);

                    if (this.IsPackageItem(feeItemList))
                    {
                        string key = this.GetPackageItemKey(feeItemList);
                        //组合项目的合并
                        if (string.IsNullOrEmpty(feeItemList.UndrugComb.ID) == false)
                        {
                            if (hsPackageItem.ContainsKey(key))
                            {
                                ((ArrayList)hsPackageItem[key]).Add(feeItemList);
                            }
                            else
                            {
                                //新建
                                ArrayList al = new ArrayList();
                                al.Add(feeItemList);
                                hsPackageItem[key] = al;
                            }
                            continue;
                        }
                    }

                    if (this.AddFeeDetailToDataTable(feeItemList) > 0)
                    {
                        this.fpFeeDetail_Sheet1.Rows[this.fpFeeDetail_Sheet1.RowCount - 1].Tag = feeItemList;
                        this.SetFpFeeDetailFormat(feeItemList, this.fpFeeDetail_Sheet1.RowCount - 1, null);
                    }

                }
                #endregion

                #region 处理复合项目
                //加载组合项目
                if (hsPackageItem.Count > 0)
                {
                    foreach (DictionaryEntry de in hsPackageItem)
                    {
                        ArrayList al = de.Value as ArrayList;
                        if (al != null && al.Count > 0)
                        {
                            FeeItemList feeItemListFirst = ((FeeItemList)al[0]).Clone();
                            ReturnApply returnApply = null;
                            decimal sumCost = 0.00M;
                            decimal sumNobackCost = 0.00M;
                            decimal sumReturnCost = 0.00M;

                            //价格集合
                            Dictionary<string, decimal> priceCollection = new Dictionary<string, decimal>();
                            Dictionary<string, string> priceItemCollection = new Dictionary<string, string>();
                            ArrayList alList = new ArrayList();
                            ArrayList alReturnApplyList = new ArrayList();
                            foreach (FeeItemList feeItemList in al)
                            {
                                if (feeItemList.Item.PackQty == 0)
                                {
                                    feeItemList.Item.PackQty = 1;
                                }

                                sumNobackCost += Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Price * feeItemList.NoBackQty / feeItemList.Item.PackQty, 2);
                                sumCost += Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Price * feeItemList.Item.Qty / feeItemList.Item.PackQty, 2);

                                string key = this.GetPackageItemKey(feeItemList);
                                if (priceCollection.ContainsKey(key))
                                {
                                    if (priceItemCollection.ContainsKey(key + feeItemList.Item.ID) == false)
                                    {
                                        priceCollection[key] += Math.Abs(feeItemList.FT.TotCost);
                                        priceItemCollection[key + feeItemList.Item.ID] = feeItemList.Item.ID;
                                    }
                                }
                                else
                                {
                                    priceCollection[key] = Math.Abs(feeItemList.FT.TotCost);
                                    priceItemCollection[key + feeItemList.Item.ID] = feeItemList.Item.ID;
                                }

                                if (feeItemList.NoBackQty > 0)
                                {
                                    alList.Add(feeItemList);
                                }
                                if (feeItemList.TransType == Neusoft.HISFC.Models.Base.TransTypes.Positive && dicReturnApply.ContainsKey(feeItemList.RecipeNO + feeItemList.SequenceNO))
                                {
                                    alReturnApplyList.Add(feeItemList);
                                    returnApply = (dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO] as ReturnApply).Clone();
                                    sumReturnCost += Neusoft.FrameWork.Public.String.FormatNumber(returnApply.Item.Price * returnApply.Item.Qty / returnApply.Item.PackQty, 2);

                                }

                            }

                            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = SOC.HISFC.BizProcess.CommonInterface.CommonController.Instance.GetItem(feeItemListFirst.UndrugComb.ID);

                            feeItemListFirst.Item = undrug;
                            if (feeItemListFirst.Item.PackQty == 0)
                            {
                                feeItemListFirst.Item.PackQty = 1;
                            }
                            string keyCombo = this.GetPackageItemKey(feeItemListFirst);

                            if (feeItemListFirst.UndrugComb.Qty != 0)
                            {
                                feeItemListFirst.Item.Qty = feeItemListFirst.UndrugComb.Qty;
                            }
                            else if (priceCollection.ContainsKey(keyCombo))
                            {
                                feeItemListFirst.Item.Qty = Neusoft.FrameWork.Public.String.FormatNumber(sumCost / priceCollection[keyCombo], 2);
                            }
                            else
                            {
                                feeItemListFirst.Item.Qty = sumCost >= 0 ? 1 : -1;
                            }

                            if (sumNobackCost <= 0)
                            {
                                feeItemListFirst.NoBackQty = 0;
                                feeItemListFirst.Item.Price = sumCost;
                                feeItemListFirst.FT.TotCost = sumCost;
                            }
                            else
                            {
                                if (feeItemListFirst.UndrugComb.Qty != 0)
                                {
                                    feeItemListFirst.NoBackQty = feeItemListFirst.UndrugComb.Qty;
                                }
                                else if (priceCollection.ContainsKey(keyCombo))
                                {
                                    feeItemListFirst.NoBackQty = Neusoft.FrameWork.Public.String.FormatNumber(sumNobackCost / priceCollection[keyCombo], 2);
                                }
                                else
                                {
                                    feeItemListFirst.NoBackQty = sumNobackCost >= 0 ? 1 : -1;
                                }
                                feeItemListFirst.Item.Price = sumCost;
                                feeItemListFirst.FT.TotCost = sumCost;
                            }

                            if (returnApply != null)
                            {
                                if (priceCollection.ContainsKey(keyCombo))
                                {
                                    returnApply.Item.Qty = Neusoft.FrameWork.Public.String.FormatNumber(sumReturnCost / priceCollection[keyCombo], 2);
                                }
                                else
                                {
                                    returnApply.Item.Qty = sumReturnCost >= 0 ? 1 : -1;
                                }
                            }
                            string quitKey = this.GetPackageItemQuitKey(feeItemListFirst);
                            hsQuitFeeByPackage[quitKey] = alList;
                            hsQuitFeeByPackage["Quit" + quitKey] = alReturnApplyList;

                            if (this.AddFeeDetailToDataTable(feeItemListFirst) > 0)
                            {
                                this.SetFpFeeDetailFormat(feeItemListFirst, this.fpFeeDetail_Sheet1.RowCount - 1, returnApply);
                                this.fpFeeDetail_Sheet1.Rows[this.fpFeeDetail_Sheet1.RowCount - 1].Tag = feeItemListFirst.Clone();
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {

                #region 所有费用
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList item in this.alAllFeeItemLists)
                {
                    if (this.GetItemFeeCode(item) != feeCode)
                    {
                        continue;
                    }

                    if (this.GetItemID(item) == itemCode)
                    {
                        //重取数据库
                        Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList = this.feeManager.GetItemListByRecipeNO(item.RecipeNO, item.SequenceNO, item.TransType, item.Item.ItemType);

                        if (this.IsPackageItem(feeItemList))
                        {
                            string key = this.GetPackageItemKey(feeItemList);
                            //组合项目的合并
                            if (string.IsNullOrEmpty(feeItemList.UndrugComb.ID) == false)
                            {
                                if (hsPackageItem.ContainsKey(key))
                                {
                                    ((ArrayList)hsPackageItem[key]).Add(feeItemList);
                                }
                                else
                                {
                                    //新建
                                    ArrayList al = new ArrayList();
                                    al.Add(feeItemList);
                                    hsPackageItem[key] = al;
                                }
                                continue;
                            }
                        }

                        if (this.AddFeeDetailToDataTable(feeItemList) > 0)
                        {
                            this.fpFeeDetail_Sheet1.Rows[this.fpFeeDetail_Sheet1.RowCount - 1].Tag = feeItemList;
                            this.SetFpFeeDetailFormat(feeItemList, this.fpFeeDetail_Sheet1.RowCount - 1, null);
                        }
                    }
                }
                #endregion

                #region 处理复合项目
                //加载组合项目
                if (hsPackageItem.Count > 0)
                {
                    foreach (DictionaryEntry de in hsPackageItem)
                    {
                        ArrayList al = de.Value as ArrayList;
                        if (al != null && al.Count > 0)
                        {
                            FeeItemList feeItemListFirst = ((FeeItemList)al[0]).Clone();
                            ReturnApply returnApply = null;
                            decimal sumCost = 0.00M;
                            decimal sumNobackCost = 0.00M;
                            decimal sumReturnCost = 0.00M;

                            //价格集合
                            Dictionary<string, decimal> priceCollection = new Dictionary<string, decimal>();
                            Dictionary<string, string> priceItemCollection = new Dictionary<string, string>();
                            ArrayList alList = new ArrayList();
                            ArrayList alReturnApplyList = new ArrayList();
                            foreach (FeeItemList feeItemList in al)
                            {
                                if (feeItemList.Item.PackQty == 0)
                                {
                                    feeItemList.Item.PackQty = 1;
                                }

                                sumNobackCost += Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Price * feeItemList.NoBackQty / feeItemList.Item.PackQty, 2);
                                sumCost += Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Price * feeItemList.Item.Qty / feeItemList.Item.PackQty, 2);

                                string key = this.GetPackageItemKey(feeItemList);
                                if (priceCollection.ContainsKey(key))
                                {
                                    if (priceItemCollection.ContainsKey(key + feeItemList.Item.ID) == false)
                                    {
                                        priceCollection[key] += Math.Abs(feeItemList.FT.TotCost);
                                        priceItemCollection[key + feeItemList.Item.ID] = feeItemList.Item.ID;
                                    }
                                }
                                else
                                {
                                    priceCollection[key] = Math.Abs(feeItemList.FT.TotCost);
                                    priceItemCollection[key + feeItemList.Item.ID] = feeItemList.Item.ID;
                                }

                                if (feeItemList.NoBackQty > 0)
                                {
                                    alList.Add(feeItemList);
                                }
                                if (feeItemList.TransType == Neusoft.HISFC.Models.Base.TransTypes.Positive && dicReturnApply.ContainsKey(feeItemList.RecipeNO + feeItemList.SequenceNO))
                                {
                                    alReturnApplyList.Add(feeItemList);
                                    returnApply = (dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO] as ReturnApply).Clone();
                                    sumReturnCost += Neusoft.FrameWork.Public.String.FormatNumber(returnApply.Item.Price * returnApply.Item.Qty / returnApply.Item.PackQty, 2);

                                }

                            }

                            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = SOC.HISFC.BizProcess.CommonInterface.CommonController.Instance.GetItem(feeItemListFirst.UndrugComb.ID);

                            feeItemListFirst.Item = undrug;
                            if (feeItemListFirst.Item.PackQty == 0)
                            {
                                feeItemListFirst.Item.PackQty = 1;
                            }
                            string keyCombo = this.GetPackageItemKey(feeItemListFirst);

                            if (feeItemListFirst.UndrugComb.Qty != 0)
                            {
                                feeItemListFirst.Item.Qty = feeItemListFirst.UndrugComb.Qty;
                            }
                            else if (priceCollection.ContainsKey(keyCombo))
                            {
                                feeItemListFirst.Item.Qty = Neusoft.FrameWork.Public.String.FormatNumber(sumCost / priceCollection[keyCombo], 2);
                            }
                            else
                            {
                                feeItemListFirst.Item.Qty = sumCost >= 0 ? 1 : -1;
                            }

                            if (sumNobackCost <= 0)
                            {
                                feeItemListFirst.NoBackQty = 0;
                                feeItemListFirst.Item.Price = sumCost;
                                feeItemListFirst.FT.TotCost = sumCost;
                            }
                            else
                            {
                                if (feeItemListFirst.UndrugComb.Qty != 0)
                                {
                                    feeItemListFirst.NoBackQty = feeItemListFirst.UndrugComb.Qty;
                                }
                                else if (priceCollection.ContainsKey(keyCombo))
                                {
                                    feeItemListFirst.NoBackQty = Neusoft.FrameWork.Public.String.FormatNumber(sumNobackCost / priceCollection[keyCombo], 2);
                                }
                                else
                                {
                                    feeItemListFirst.NoBackQty = sumNobackCost >= 0 ? 1 : -1;
                                }
                                feeItemListFirst.Item.Price = sumCost;
                                feeItemListFirst.FT.TotCost = sumCost;
                            }

                            if (returnApply != null)
                            {
                                if (priceCollection.ContainsKey(keyCombo))
                                {
                                    returnApply.Item.Qty = Neusoft.FrameWork.Public.String.FormatNumber(sumReturnCost / priceCollection[keyCombo], 2);
                                }
                                else
                                {
                                    returnApply.Item.Qty = sumReturnCost >= 0 ? 1 : -1;
                                }
                            }
                            string quitKey = this.GetPackageItemQuitKey(feeItemListFirst);
                            hsQuitFeeByPackage[quitKey] = alList;
                            hsQuitFeeByPackage["Quit" + quitKey] = alReturnApplyList;

                            if (this.AddFeeDetailToDataTable(feeItemListFirst) > 0)
                            {
                                this.SetFpFeeDetailFormat(feeItemListFirst, this.fpFeeDetail_Sheet1.RowCount - 1, returnApply);
                                this.fpFeeDetail_Sheet1.Rows[this.fpFeeDetail_Sheet1.RowCount - 1].Tag = feeItemListFirst.Clone();
                            }
                        }
                    }
                }

                #endregion
            }
            this.fpFeeDetail_Sheet1.SortRows(0, true, true);

        }

        private void fpFeeDetail_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.isQuitFee)
            {
                ContextMenuStrip contextMenu1 = new ContextMenuStrip();

                ToolStripMenuItem mnuPrint = new ToolStripMenuItem();
                mnuPrint.Click += new EventHandler(mnuPrintApply_Click);
                mnuPrint.Text = "退费申请";
                contextMenu1.Items.Add(mnuPrint);

                ToolStripSeparator spearator = new ToolStripSeparator();
                contextMenu1.Items.Add(spearator);

                mnuPrint = new ToolStripMenuItem();
                mnuPrint.Click += new EventHandler(mnuPrintCancel_Click);
                mnuPrint.Text = "取消退费申请";
                contextMenu1.Items.Add(mnuPrint);

                contextMenu1.Show(this.fpFeeDetail, new Point(e.X, e.Y));
            }
        }

        private void fpFeeDetail_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //当选择退费时，自动变成全退
            if (e.Column == this.dtFeeDetail.Columns["退费"].Ordinal)
            {
                if (this.dtFeeDetail.Columns.Contains("退费数量"))
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpFeeDetail_Sheet1.Cells[e.Row, e.Column].Value))
                    {
                        Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem = this.fpFeeDetail_Sheet1.Rows[e.Row].Tag as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                        string key = feeItem.UndrugComb.ID + "|" + feeItem.TransType.ToString() + "|" + feeItem.RecipeNO + feeItem.ExecOrder.ID;

                        if (hsQuitFeeByPackage.ContainsKey(key))//如果是复合项目，不允许修改数量
                        {
                            this.fpFeeDetail_Sheet1.Cells[e.Row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Locked = true;
                            this.fpFeeDetail_Sheet1.Cells[e.Row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Value = feeItem.NoBackQty;
                        }
                        else
                        {
                            this.fpFeeDetail_Sheet1.Cells[e.Row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Locked = false;
                            this.fpFeeDetail_Sheet1.Cells[e.Row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Value = feeItem.NoBackQty;
                        }
                    }
                    else
                    {
                        this.fpFeeDetail_Sheet1.Cells[e.Row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Locked = true;
                        this.fpFeeDetail_Sheet1.Cells[e.Row, this.dtFeeDetail.Columns["退费数量"].Ordinal].Value = 0.00F;
                    }
                }
            }
        }

        void mnuPrintApply_Click(object sender, EventArgs e)
        {
            ArrayList alFee = new ArrayList();

            for (int i = 0; i < this.fpFeeDetail.ActiveSheet.Rows.Count; i++)
            {
                if (this.fpFeeDetail.ActiveSheet.Cells[i, this.dtFeeDetail.Columns["退费"].Ordinal].Text == "True" &&
                    Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpFeeDetail.ActiveSheet.Cells[i, this.dtFeeDetail.Columns["退费数量"].Ordinal].Text) > 0)
                {
                    //取费用信息
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList item = this.fpFeeDetail_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                    if (item != null && item.NoBackQty > 0)
                    {
                        string key = this.GetPackageItemQuitKey(item);
                        //说明是复合项目，拆分
                        if (hsQuitFeeByPackage.ContainsKey(key))
                        {
                            ArrayList al = hsQuitFeeByPackage[key] as ArrayList;

                            decimal qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpFeeDetail.ActiveSheet.Cells[i, this.dtFeeDetail.Columns["退费数量"].Ordinal].Value);
                            foreach (FeeItemList f in al)
                            {
                                if (f.NoBackQty > 0)
                                {
                                    f.Item.Qty = f.NoBackQty * qty / item.NoBackQty;
                                    f.NoBackQty = 0;
                                    f.FT.TotCost = f.Item.Price * f.Item.Qty / f.Item.PackQty;
                                    f.FT.OwnCost = f.FT.TotCost;
                                    f.IsNeedUpdateNoBackQty = true;
                                    alFee.Add(f);
                                }
                            }
                        }
                        else
                        {
                            item.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpFeeDetail.ActiveSheet.Cells[i, this.dtFeeDetail.Columns["退费数量"].Ordinal].Value);
                            item.NoBackQty = item.NoBackQty - item.Item.Qty;
                            item.FT.TotCost = item.Item.Price * item.Item.Qty / item.Item.PackQty;
                            item.FT.OwnCost = item.FT.TotCost;
                            item.IsNeedUpdateNoBackQty = true;
                            alFee.Add(item);
                        }
                    }

                }
            }


            if (alFee.Count <= 0)
            {
                MessageBox.Show("没有需要退费申请的项目！");
                return;
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            Neusoft.HISFC.BizLogic.Fee.ReturnApply returnMgr = new Neusoft.HISFC.BizLogic.Fee.ReturnApply();
            returnMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            string applyBillCode = returnMgr.GetReturnApplyBillCode();
            if (applyBillCode == null || applyBillCode == string.Empty)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获取申请单号出错！" + returnMgr.Err);
                return;
            }
            DateTime nowTime = returnMgr.GetDateTimeFromSysDateTime();
            bool isApplyTip = false;

            string msgErr = string.Empty;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string msg = string.Empty;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList in alFee)
            {
                bool isApply = true;

                if (!this.isCanQuitOtherDeptFee && this.depts.ContainsKey(feeItemList.RecipeOper.Dept.ID) == false && this.depts.ContainsKey(feeItemList.ExecOper.Dept.ID) == false)
                {
                    MessageBox.Show(string.Format("不允许退其他科室费用【{0}】,请通知【{1}】进行退费", this.GetItemName(feeItemList), CommonController.Instance.GetDepartmentName(feeItemList.ExecOper.Dept.ID)));
                    return;
                }

                if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && SOC.HISFC.BizProcess.Cache.Fee.GetItem(feeItemList.Item.ID).IsNeedConfirm && this.depts.ContainsKey(feeItemList.ExecOper.Dept.ID) == false)
                {
                    MessageBox.Show(string.Format("不允许退需终端确认的费用【{0}】,请通知【{1}】进行退费", this.GetItemName(feeItemList), CommonController.Instance.GetDepartmentName(feeItemList.ExecOper.Dept.ID)));
                    return;
                }

                if (this.isCanQuitOtherDeptFee && this.depts.ContainsKey(feeItemList.ExecOper.Dept.ID) == false && this.isCurrentDeptNeedAppy == false)//不允许退其他科室的，而且执行科室不等于当前病区的，不需要退费申请
                {
                    isApply = false;
                }
                else if (this.depts.ContainsKey(feeItemList.ExecOper.Dept.ID) && this.isCurrentDeptNeedAppy == false)//如果执行科室是当前病区，而且不需要申请的，直接退费
                {
                    isApply = false;
                }
                else
                {
                    if (feeItemList.ConfirmedQty > 0)
                    {
                        string key = this.GetPackageItemKey(feeItemList);
                        if (dictionary.ContainsKey(key) == false)
                        {
                            msg += string.Format(Environment.NewLine + @"{1}【{0}  {2}{3}】", this.GetItemName(feeItemList), CommonController.Instance.GetDepartmentName(feeItemList.ExecOper.Dept.ID), feeItemList.ConfirmedQty, feeItemList.Item.PriceUnit);
                            dictionary[key] = msg;
                        }
                    }

                    isApplyTip = true;
                }

                if (isApply == false && feeItemList.ConfirmedQty > 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(string.Format("请通知【{1}】进行退费确认：【{0}  {2}{3}】", string.IsNullOrEmpty(feeItemList.UndrugComb.ID) ? feeItemList.Item.Name : feeItemList.UndrugComb.Name, CommonController.Instance.GetDepartmentName(feeItemList.ExecOper.Dept.ID), feeItemList.ConfirmedQty, feeItemList.Item.PriceUnit));
                    return;
                }

                if (this.feeIntegrate.QuitFeeApply(this.currentPatient, feeItemList, isApply, applyBillCode, nowTime, ref msgErr) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(isApply ? "退费申请失败！" : "退费失败！" + this.feeIntegrate.Err);
                    return;
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("退费成功！" + (string.IsNullOrEmpty(msg) ? (isApplyTip ? "存在退费申请，请通知退费" : "") : @"请通知以下科室进行退费确认：" + Environment.NewLine + Environment.NewLine + msg));

            //需要更新界面状态
            //重新加载退费申请信息
            this.QueryPatientReturyApply(this.currentPatient);
            //this.fpFeeDaySum_SelectionChanged(null, null);
        }

        void mnuPrintCancel_Click(object sender, EventArgs e)
        {
            ArrayList alFee = new ArrayList();

            for (int i = 0; i < this.fpFeeDetail.ActiveSheet.Rows.Count; i++)
            {
                if (this.fpFeeDetail.ActiveSheet.Cells[i, this.dtFeeDetail.Columns["取消申请"].Ordinal].Text == "True" &&
                    Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpFeeDetail.ActiveSheet.Cells[i, this.dtFeeDetail.Columns["已申请数量"].Ordinal].Text) > 0)
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList item = this.fpFeeDetail.ActiveSheet.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                    if (item.TransType == Neusoft.HISFC.Models.Base.TransTypes.Positive)
                    {
                        string key = "Quit" + this.GetPackageItemQuitKey(item);
                        if (hsQuitFeeByPackage.ContainsKey(key))
                        {
                            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList in (ArrayList)hsQuitFeeByPackage[key])
                            {
                                //取费用信息
                                alFee.Add(dicReturnApply[feeItemList.RecipeNO + feeItemList.SequenceNO.ToString()]);
                            }
                        }
                        else
                        {
                            //取费用信息
                            alFee.Add(dicReturnApply[item.RecipeNO + item.SequenceNO.ToString()]);
                        }
                    }
                }
            }


            if (alFee.Count <= 0)
            {
                MessageBox.Show("没有需要取消退费申请的项目！");
                return;
            }

            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList tempFeeItem = null;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            this.feeManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.phamarcyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.returnApplyManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            DateTime nowTime = this.feeManager.GetDateTimeFromSysDateTime();

            foreach (Object quitItem in alFee)
            {
                if (quitItem is Neusoft.HISFC.Models.Pharmacy.ApplyOut)     //退药申请
                {
                    #region 退药申请取消 此时尚没有退费申请信息

                    Neusoft.HISFC.Models.Pharmacy.ApplyOut tempApplyOut = quitItem as Neusoft.HISFC.Models.Pharmacy.ApplyOut;
                    //根据处方号、处方流水号获取结算状态
                    //tempFeeItem = this.inpatientManager.GetItemListByRecipeNO(tempApplyOut.RecipeNO, tempApplyOut.SequenceNO, true);
                    tempFeeItem = this.feeManager.GetItemListByRecipeNO(tempApplyOut.RecipeNO, tempApplyOut.SequenceNO, Neusoft.HISFC.Models.Base.EnumItemType.Drug);
                    if (tempFeeItem == null)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(Language.Msg("根据处方号、处方内项目流水号获取处方明细信息失败") + this.feeManager.Err);
                        return;
                    }
                    //更新药品明细表中的可退数量，防止并发
                    int parm = this.feeManager.UpdateNoBackQtyForDrug(tempApplyOut.RecipeNO, tempApplyOut.SequenceNO, -tempApplyOut.Days * tempApplyOut.Operation.ApplyQty, tempFeeItem.BalanceState);
                    if (parm == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(Language.Msg("更新药品可退数量失败" + this.feeManager.Err));
                        return;
                    }
                    else if (parm == 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(Language.Msg("数据发生变动!请刷新窗口"));
                        return;
                    }
                    //作废退药申请
                    parm = this.phamarcyIntegrate.CancelApplyOut(tempApplyOut.RecipeNO, tempApplyOut.SequenceNO);
                    if (parm == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(this.phamarcyIntegrate.Err);
                        return;
                    }
                    else if (parm == 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("该申请已被取消，无法再次撤销");
                        return;
                    }

                    #endregion

                    #region 临时费用信息tempFeeItem赋值

                    tempFeeItem.Item.Qty = tempApplyOut.Days * tempApplyOut.Operation.ApplyQty;
                    tempFeeItem.Item.Price = tempApplyOut.Item.PriceCollection.RetailPrice;

                    #endregion
                }

                if (quitItem is Neusoft.HISFC.Models.Fee.ReturnApply)       //退费申请
                {

                    Neusoft.HISFC.Models.Fee.ReturnApply tempReturnApply = quitItem as Neusoft.HISFC.Models.Fee.ReturnApply;
                    //ArrayList listReturnApply = new ArrayList();

                    //if (hsQuitFeeByPackage.ContainsKey( temp.ID + temp.UndrugComb.ID))
                    //{
                    //    //查找数据库
                    //    ArrayList altemp = this.returnApplyManager.QueryReturnApplys(this.currentPatient.ID, false, false);
                    //    foreach (Neusoft.HISFC.Models.Fee.ReturnApply tempReturnApply in altemp)
                    //    {
                    //        if (tempReturnApply.ApplyBillNO == temp.ApplyBillNO && tempReturnApply.UndrugComb.ID == temp.UndrugComb.ID)
                    //        {
                    //            listReturnApply.Add(tempReturnApply);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    listReturnApply.Add(temp);
                    //}

                    //foreach (Neusoft.HISFC.Models.Fee.ReturnApply tempReturnApply in listReturnApply)
                    {
                        #region 根据处方号、处方流水号获取费用信息

                        if (tempReturnApply.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            //根据处方号、处方流水号获取结算状态
                            tempFeeItem = this.feeManager.GetItemListByRecipeNO(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO, Neusoft.HISFC.Models.Base.EnumItemType.Drug);
                        }
                        else
                        {
                            tempFeeItem = this.feeManager.GetItemListByRecipeNO(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO, Neusoft.HISFC.Models.Base.EnumItemType.UnDrug);
                        }
                        if (tempFeeItem == null)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(Language.Msg("根据处方号、处方内项目流水号获取处方明细信息失败") + this.feeManager.Err);
                            return;
                        }

                        #endregion

                        if (tempReturnApply.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)     //药品退费申请
                        {
                            #region 药品退费申请作废

                            #region 更新明细表中的可退数量，防止并发

                            int parm = this.feeManager.UpdateNoBackQtyForDrug(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO, -tempReturnApply.Item.Qty, tempFeeItem.BalanceState);
                            if (parm == -1)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(Language.Msg("更新药品可退数量失败" + this.feeManager.Err));
                                return;
                            }
                            else if (parm == 0)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(Language.Msg("数据发生变动!请刷新窗口"));
                                return;
                            }

                            #endregion

                            //部分退的情况，此时药品已经存在部分退后的有效申请。需作废申请，根据取消后数量重新生成摆药申请
                            if (tempFeeItem.NoBackQty != 0 || tempFeeItem.Item.Qty != tempFeeItem.NoBackQty + tempReturnApply.Item.Qty)
                            {
                                #region 部分退取消情况
                                //首先作废摆药申请
                                int returnValue = this.phamarcyIntegrate.CancelApplyOut(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO);
                                if (returnValue == -1)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show(Language.Msg("作废摆药申请出错!") + phamarcyIntegrate.Err);

                                    return;
                                }
                                if (returnValue == 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show(Language.Msg("项目【") + tempFeeItem.Item.Name + Language.Msg("】已摆药，请重新检索数据"));

                                    return;
                                }

                                //取收费对应的摆药申请记录
                                Neusoft.HISFC.Models.Pharmacy.ApplyOut applyOutTemp = this.phamarcyIntegrate.GetApplyOut(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO);
                                if (applyOutTemp == null)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show(Language.Msg("获得申请信息出错!") + this.phamarcyIntegrate.Err);
                                    return;
                                }
                                //将剩余数量发送摆药申请
                                applyOutTemp.Operation.ApplyOper.OperTime = nowTime;
                                applyOutTemp.Operation.ApplyQty = tempFeeItem.NoBackQty + tempReturnApply.Item.Qty;//数量等于剩余数量
                                applyOutTemp.Operation.ApproveQty = tempFeeItem.NoBackQty + tempReturnApply.Item.Qty;//数量等于剩余数量
                                applyOutTemp.ValidState = Neusoft.HISFC.Models.Base.EnumValidState.Valid;	//有效状态                        
                                //将剩余数量发送摆药申请  {C37BEC96-D671-46d1-BCDD-C634423755A4}
                                if (this.phamarcyIntegrate.ApplyOut(applyOutTemp) != 1)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show(Language.Msg("重新插入发药申请出错!") + this.phamarcyIntegrate.Err);

                                    return;
                                }

                                #endregion
                            }
                            else
                            {
                                #region 全退情况

                                //恢复出库申请记录为有效   待添加 
                                parm = this.phamarcyIntegrate.UndoCancelApplyOut(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO);
                                if (parm == -1)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("恢复出库申请有效性发生错误" + this.phamarcyIntegrate.Err);
                                    return;
                                }
                                else if (parm == 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("该数据已被取消，无法撤销申请");
                                    return;
                                }

                                #endregion
                            }

                            #endregion
                        }
                        else
                        {
                            #region 非药品退费申请作废

                            //更新明细表中的可退数量，防止并发
                            int parm = this.feeManager.UpdateNoBackQtyForUndrug(tempReturnApply.RecipeNO, tempReturnApply.SequenceNO, -tempReturnApply.Item.Qty, tempFeeItem.BalanceState);
                            if (parm == -1)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(Language.Msg("更新药品可退数量失败" + this.feeManager.Err));
                                return;
                            }
                            else if (parm == 0)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(Language.Msg("数据发生变动!请刷新窗口"));
                                return;
                            }

                            #endregion

                            #region 作废物资退费申请
                            //更新物资出库表中的申请数量
                            //parm = mateInteger.ApplyMaterialFeeBack(tempReturnApply.MateList, true);
                            //if (parm < 0)
                            //{
                            //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //    MessageBox.Show(Language.Msg("更新物资申请数量失败" + this.inpatientManager.Err));
                            //    return;
                            //}
                            //if (parm == 0)
                            //{
                            //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //    MessageBox.Show(Language.Msg("数据发生变动!请刷新窗口"));
                            //    return;
                            //}
                            //parm = returnApplyManager.UpdateReturnApplyState(tempReturnApply.ApplyMateList, CancelTypes.Reprint);
                            //if (parm < 0)
                            //{
                            //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //    MessageBox.Show(Language.Msg("作废物资申请信息失败！" + this.inpatientManager.Err));
                            //    return;
                            //}
                            #endregion
                        }

                        #region 临时费用信息tempFeeItem赋值

                        tempFeeItem.Item.Qty = tempReturnApply.Item.Qty;
                        tempFeeItem.Item.Price = tempReturnApply.Item.Price;

                        #endregion

                        //作废退费申请
                        if (this.returnApplyManager.CancelReturnApply(tempReturnApply.ID, this.returnApplyManager.Operator.ID) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(Language.Msg("作废退费申请发生错误") + this.returnApplyManager.Err);
                            return;
                        }
                    }
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("取消申请申请成功！");

            //需要更新界面状态
            //重新加载退费申请信息
            this.QueryPatientReturyApply(this.currentPatient);
            //this.fpFeeDaySum_SelectionChanged(null, null);
        }

        private void neuLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //患者汇总信息
            //if (this.neuTabControl1.SelectedTab == tpMainInfo)
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpMainInfo_Sheet1, pathNameMainInfo);
                //预交金
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpPrepay_Sheet1, pathNamePrepay);
                //结算信息
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpBalance_Sheet1, pathNameBalance);
            }
            //费用当天汇总
            //else if (this.neuTabControl1.SelectedTab == tpDrugList)
            {

                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFeeItemSum_Sheet1, this.pathNameFeeItemSum);
                //Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFeeDaySum_Sheet1, this.pathNameFeeDaySum);
            }
            //费用明细
            //else if (this.neuTabControl1.SelectedTab == tpUnDrugList)
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFeeDetail_Sheet1, this.pathNameFeeDetail);
            }
            //费用汇总信息
            //else if (this.neuTabControl1.SelectedTab == tpFeeInfo)
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpFee_Sheet1, pathNameFee);
            }
            //变更信息

            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.fpShiftData_Sheet1, this.pathNameShiftData);

            MessageBox.Show("格式保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ucQueryInpatientNo1_myEvent()
        {
            if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo == string.Empty)
            {
                MessageBox.Show(Language.Msg("该患者不存在!请验证后输入"));

                return;
            }

            this.currentPatient = this.radtManager.QueryPatientInfoByInpatientNO(this.ucQueryInpatientNo1.InpatientNo);
            if (this.currentPatient == null || this.currentPatient.ID == null || this.currentPatient.ID == string.Empty)
            {
                MessageBox.Show(Language.Msg("查询患者基本信息出错!") + this.radtManager.Err);

                return;
            }
            this.QueryPatientByInpatientNO(currentPatient);

        }

        private void dtpItemDayFilter_ValueChanged(object sender, EventArgs e)
        {
            if (this.dvFeeDetail != null)
            {
                if (this.dtpItemDayFilter.Checked)
                {
                    this.dvFeeDetail.RowFilter = string.Format("收费时间 like '%{0}%'", this.dtpItemDayFilter.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    this.dvFeeDetail.RowFilter = "1=1";
                }
            }
        }

        private void txtItemFilter_TextChanged(object sender, EventArgs e)
        {
            if (this.dvFeeItemSum != null)
            {
                string feeCode = "";
                if (this.fpFeeItemSum_Sheet1.RowCount > 0 && this.fpFeeItemSum_Sheet1.ActiveRowIndex >= 0)
                {
                    feeCode = this.fpFeeItemSum_Sheet1.Cells[this.fpFeeItemSum_Sheet1.ActiveRowIndex, this.dtFeeItemSum.Columns["项目编码"].Ordinal].Text;
                }
                this.dvFeeItemSum.RowFilter = string.Format("项目名称 like '%{0}%' or 拼音码 like '%{0}%' or 五笔码 like '%{0}%' or 自定义码 like '%{0}%'", this.txtItemFilter.Text.Trim());
                this.SelectFeeItemSum(feeCode);
            }
        }

        private void cmbFeeTypeFilter_TextChanged(object sender, EventArgs e)
        {
            if (this.dvFee != null)
            {
                this.dvFee.RowFilter = string.Format("费用名称 like '%{0}%'", this.cmbFeeTypeFilter.Text.Trim());
                this.SelectFeeInfo();
            }
        }

        void ckUnCombo_CheckedChanged(object sender, EventArgs e)
        {
            if (this.currentPatient != null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在查询信息，请稍后...");
                Application.DoEvents();
                hsMinFee.Clear();
                hsDayFeeItemList.Clear();
                dicReturnApply.Clear();
                dtFee.Clear();
                dtFeeDaySum.Clear();
                dtFeeDetail.Clear();
                dtFeeItemSum.Clear();
                //设置查询时间
                DateTime beginTime = this.currentPatient.PVisit.InTime;
                DateTime endTime = this.radtManager.GetDateTimeFromSysDateTime();

                if (this.neuTabControl1.TabPages.Contains(this.tpFeeInfo))
                {
                    //this.QueryInvoceType();
                    this.QueryPatientReturyApply(this.currentPatient);
                    this.QueryPatientDrugList(beginTime, endTime);
                    this.QueryPatientUndrugList(beginTime, endTime);
                    this.QueryPatientFeeInfo(beginTime, endTime);

                }

                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
        }

        private void fpFeeDetail_SeleCount(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int row = 0;
                int count = 0;

                FarPoint.Win.Spread.Model.CellRange[] cr;
                cr = this.fpFeeDetail_Sheet1.GetSelections();
                row = cr[0].Row;
                count = cr[0].RowCount;
                decimal sum = 0;

                for (int i = row; i < count + row; i++)
                {
                    sum += Convert.ToDecimal(this.fpFeeDetail_Sheet1.Cells[i, 7].Text);
                    //7
                }

                this.summoney.Text = "合计:" + sum.ToString();
            }
            catch
            { }
        }

    }
}
