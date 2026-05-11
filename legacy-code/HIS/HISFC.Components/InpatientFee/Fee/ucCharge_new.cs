using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Models.RADT;
using Neusoft.FrameWork.Management;
using Neusoft.FrameWork.WinForms.Forms;
using System.Collections;
using Neusoft.HISFC.Components.Common.Controls;

namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    /// <summary>
    /// ucCharge_new<br></br>
    /// [功能描述: 住院收费UC]<br></br>
    /// [创 建 者: ]<br></br>
    /// [创建时间: ]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public partial class ucCharge_new : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucCharge_new()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 加载项目类别
        /// </summary>
        protected Neusoft.HISFC.Components.Common.Controls.EnumShowItemType itemKind = Neusoft.HISFC.Components.Common.Controls.EnumShowItemType.Undrug;

        /// <summary>
        /// 患者基本信息
        /// </summary>
        protected PatientInfo patientInfo = null;

        /// <summary>
        /// toolBarService
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        /// <summary>
        /// 多个患者同时收费列表
        /// </summary>
        protected ArrayList patientList = new ArrayList();

        /// <summary>
        /// 是否拥有患者树,如果有,那么住院号输入控件不可以,否则可以用
        /// </summary>
        protected bool isHavePatientTree = false;

        /// <summary>
        /// 是否验证项目及时停用
        /// </summary>
        protected bool isJudgeValid = false;

        private bool isHaveRight = false;   //是否具有处方权 add by allan 



        /// <summary>
        /// 是否判断欠费,Y：判断欠费，不允许继续收费,M：判断欠费，提示是否继续收费,N：不判断欠费
        /// </summary>
        Neusoft.HISFC.Models.Base.MessType messtype = Neusoft.HISFC.Models.Base.MessType.Y;

        /// <summary>
        /// 调用组套判断药品库存时，科室代码取值
        /// </summary>
        EnumDrugStorageDept drugStorageDept = EnumDrugStorageDept.PatientInDept;

        #region 业务层变量

        /// <summary>
        /// 患者入出转业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        protected Neusoft.HISFC.BizLogic.RADT.InPatient RADT = new Neusoft.HISFC.BizLogic.RADT.InPatient();
        #endregion

        /// <summary>
        /// 打印接口
        /// </summary>
        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInpatientChargePrint printInterface = null;

        #endregion

        #region 属性

        /// <summary>
        /// 是否拆分复合项目到界面上{F4912030-EF65-4099-880A-8A1792A3B449}
        /// </summary>
        [Category("控件设置"), Description("设置该控件是否在界面上分解复合项目 true分解 false不分解")]
        public bool IsSplitUndrugCombItem
        {
            get
            {
                return this.ucInpatientCharge_new1.IsSplitUndrugCombItem;
            }
            set
            {
                this.ucInpatientCharge_new1.IsSplitUndrugCombItem = value;
            }
        }
        //{F4912030-EF65-4099-880A-8A1792A3B449} 结束

        /// <summary>
        /// 是否拥有患者树,如果有,那么住院号输入控件不可以,否则可以用
        /// </summary>
        public bool IsHavePatientTree
        {
            get
            {
                return this.isHavePatientTree;
            }
            set
            {
                this.isHavePatientTree = value;

                this.ucQueryPatientInfo.Enabled = !this.isHavePatientTree;
            }
        }

        /// <summary>
        /// 加载项目类别
        /// </summary>
        [Category("控件设置"), Description("加载的项目类别 All所有 Undrug非药品 drug药品")]
        public Neusoft.HISFC.Components.Common.Controls.EnumShowItemType ItemKind
        {
            set
            {
                this.itemKind = value;

                this.ucInpatientCharge_new1.加载项目类别 = this.itemKind;
            }
            get
            {
                return this.ucInpatientCharge_new1.加载项目类别;
            }
        }
        /// <summary>
        /// 是否允许收取0单价项目
        /// </summary>
        [Category("控件设置"), Description("设置或者获得是否允许收取0单价项目 true 可以 false 不可以")]
        public bool isChargeZero
        {
            get
            {
                return this.ucInpatientCharge_new1.IsChargeZero;
            }
            set
            {
                this.ucInpatientCharge_new1.IsChargeZero = value;
            }
        }

        /// <summary>
        /// 是否验证项目及时停用
        /// </summary>
        [Category("控件设置"), Description("设置或者获得是否及时验证项目停用 true 验证 false 不验证")]
        public bool IsJudgeValid
        {
            get
            {
                return this.ucInpatientCharge_new1.IsJudgeValid;
            }
            set
            {
                this.ucInpatientCharge_new1.IsJudgeValid = value;
            }
        }

        [Category("控件设置"), Description("是否判断欠费,Y：判断欠费，不允许继续收费,M：判断欠费，提示是否继续收费,N：不判断欠费")]
        public Neusoft.HISFC.Models.Base.MessType MessageType
        {
            get
            {
                return this.messtype;
            }
            set
            {
                this.messtype = value;
            }
        }
        [Category("控件设置"), Description("数量为零是否提示")]
        public bool IsJudgeQty
        {
            get
            {
                return this.ucInpatientCharge_new1.IsJudgeQty;
            }
            set
            {
                this.ucInpatientCharge_new1.IsJudgeQty = value;
            }
        }
        [Category("控件设置"), Description("执行科室是否默认为登陆科室")]
        public bool DefaultExeDeptIsDeptIn
        {
            get
            {
                return this.ucInpatientCharge_new1.DefaultExeDeptIsDeptIn;
            }
            set
            {
                this.ucInpatientCharge_new1.DefaultExeDeptIsDeptIn = value;
            }
        }

        [Category("控件设置"), Description("是否显示收费组套树"), DefaultValue(false)]
        //{7F5CF034-5DCD-4d10-B2FD-CDD02F45E58D}
        public bool IsShowGroupTree
        {
            get
            {
                return this.ucInpatientCharge_new1.IsShowGroupTree;
            }
            set
            {
                this.ucInpatientCharge_new1.IsShowGroupTree = value;
            }
        }
        /// <summary>
        /// 是否仅显示具有处方权医生 true 是  false 否"
        /// </summary>
        [Category("控件设置"), Description("是否仅显示具有处方权医生 true 是  false 否"), DefaultValue(false)]
        public bool IsHaveRight
        {
            get { return isHaveRight; }
            set { isHaveRight = value; }
        }

        /// <summary>
        /// 是否启用高值耗材的条码输入判断
        /// </summary>
        [Category("控件设置"), Description("是否启用高值耗材的条码输入判断 true启用 false不启用")]
        public bool IsUseAdmin
        {
            get
            {
                return this.ucInpatientCharge_new1.IsUseAdmin;
            }
            set
            {
                this.ucInpatientCharge_new1.IsUseAdmin = value;
            }
        }

        /// <summary>
        /// 患者基本信息
        /// </summary>
        public PatientInfo PatientInfo
        {
            set
            {
                this.patientInfo = value;

                this.SetPatientInfomation();

                this.ucInpatientCharge_new1.PatientInfo = this.patientInfo;

                this.cmbDoct.Focus();
            }
        }

        /// <summary>
        /// 当前窗口是否设计模式
        /// </summary>
        protected new bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }

        [Category("控件设置"), Description("调用组套判断药品库存时，科室代码取值")]
        public EnumDrugStorageDept DrugStorageDept
        {
            get
            {
                return this.ucInpatientCharge_new1.DrugStorageDept;
            }
            set
            {
                this.drugStorageDept = value;
                this.ucInpatientCharge_new1.DrugStorageDept = this.drugStorageDept;
            }
        }


        #endregion

        #region 私有方法

        /// <summary>
        /// 获得患者基本信息
        /// </summary>
        private void ucQueryPatientInfo_myEvent()
        {
            if (this.ucQueryPatientInfo.InpatientNo == null || this.ucQueryPatientInfo.InpatientNo == string.Empty)
            {
                MessageBox.Show(Language.Msg("该患者不存在!请验证后输入"));

                return;
            }

            PatientInfo patientTemp = this.radtIntegrate.GetPatientInfomation(this.ucQueryPatientInfo.InpatientNo);
            if (patientTemp == null || patientTemp.ID == null || patientTemp.ID == string.Empty)
            {
                MessageBox.Show(Language.Msg("该患者不存在!请验证后输入"));

                return;
            }

            if (patientTemp.PVisit.InState.ID.ToString() == "N")
            {
                MessageBox.Show(Language.Msg("该患者已经无费退院，不允许收费!"));

                this.Clear();
                this.ucQueryPatientInfo.Focus();

                return;
            }

            if (patientTemp.PVisit.InState.ID.ToString() == "O")
            {
                MessageBox.Show(Language.Msg("该患者已经出院结算，不允许收费!"));

                this.Clear();
                this.ucQueryPatientInfo.Focus();

                return;
            }
            this.patientInfo = patientTemp;

            this.SetPatientInfomation();

            this.ucInpatientCharge_new1.PatientInfo = this.patientInfo;

            this.cmbDoct.Focus();
        }

        /// <summary>
        /// 设置患者基本信息
        /// </summary>
        protected virtual void SetPatientInfomation()
        {
            if (this.patientInfo == null)
            {
                return;
            }

            this.ucQueryPatientInfo.Text = this.patientInfo.PID.PatientNO;
            this.txtName.Text = this.patientInfo.Name;
            this.txtBedNO.Text = this.patientInfo.PVisit.PatientLocation.Bed.ID.Substring(4);
            this.txtInTime.Text = this.patientInfo.PVisit.InTime.ToString("d");
            this.txtPact.Text = this.patientInfo.Pact.Name;
            this.txtInDept.Text = this.patientInfo.PVisit.PatientLocation.Dept.Name;
            this.cmbDoct.Tag = this.patientInfo.PVisit.AdmittingDoctor.ID;
            this.txtAlarm.Text = this.patientInfo.PVisit.MoneyAlert.ToString();
            this.txtLeft.Text = this.patientInfo.FT.LeftCost.ToString();
            this.txtAge.Text = this.patientInfo.Age;
            //物资扣库科室
            this.cmbDept.Tag = this.patientInfo.PVisit.PatientLocation.Dept.ID;
        }

        /// <summary>
        /// 清空函数
        /// </summary>
        protected virtual void Clear()
        {
            this.txtName.Text = string.Empty;
            this.txtBedNO.Text = string.Empty;
            this.txtInTime.Text = string.Empty;
            this.txtPact.Text = string.Empty;
            this.txtInDept.Text = string.Empty;
            this.cmbDoct.Tag = string.Empty;
            this.txtAlarm.Text = string.Empty;
            this.txtLeft.Text = string.Empty;
            this.dateTimePicker1.Value = Neusoft.FrameWork.Function.NConvert.ToDateTime(RADT.GetSysDateTime());
            this.ucInpatientCharge_new1.Clear();

            this.ucQueryPatientInfo.Focus();
        }

        /// <summary>
        /// 保存函数
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        protected virtual int Save()
        {
            this.Cursor = Cursors.WaitCursor;

            if (this.patientList != null && this.patientList.Count > 0)
            {
                int iRow = 0;
                foreach (PatientInfo patient in this.patientList)
                {
                    this.ucInpatientCharge_new1.PatientInfo = patient;

                    int returnValue = this.ucInpatientCharge_new1.Save();
                    if (returnValue < 0)
                    {

                        break;
                        //this.Cursor = Cursors.Arrow;
                    }
                    iRow++;

                    this.Print(patient, this.ucInpatientCharge_new1.FeeItemCollection);
                }

                //if (noFeedPatientList.Count > 0) 
                if (iRow < this.patientList.Count)
                {
                    string msg = string.Empty;

                    for (; iRow < this.patientList.Count; iRow++)
                    {
                        PatientInfo patient = (PatientInfo)patientList[iRow];

                        msg += "       " + patient.Name + " " + "\n";
                    }
                    msg = "以下患者没有收费成功 \n" + msg
                          + "   请单独处理";

                    MessageBox.Show(Language.Msg(msg));

                    return -1;
                }

                this.Cursor = Cursors.Arrow;

                MessageBox.Show(this.ucInpatientCharge_new1.SucessMsg);

                this.Clear();

                return 1;
            }
            else
            {
                int returnValue = this.ucInpatientCharge_new1.Save();

                if (returnValue < 0)
                {
                    return -1;
                }
                else
                {
                    MessageBox.Show(this.ucInpatientCharge_new1.SucessMsg);
                }

                this.Print(this.patientInfo, this.ucInpatientCharge_new1.FeeItemCollection);

                this.Clear();

                this.Cursor = Cursors.Arrow;

                return 1;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        protected virtual int Init()
        {

            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载项目信息,请等待...");
            Application.DoEvents();
            
            //初始化医生信息
            if (InitDoctors() == -1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                return -1;
            }
            //{7376038F-EFE8-46c8-BA63-3147C6EF67F0}
            //初始化科室信息
            if (InitDept() == -1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                return -1;
            }

            Neusoft.HISFC.Models.Base.Employee emplTemp = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;

            //初始化项目批费控件
            if (this.ucInpatientCharge_new1.Init(emplTemp.Dept.ID) == -1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();

                return -1;
            }
            this.ucInpatientCharge_new1.MessageType = this.MessageType;
            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();

            return 1;
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
                doctors = this.managerIntegrate.QueryEmployeeHaveRight(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
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
        /// <summary>
        /// 初始化科室信息
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        //{7376038F-EFE8-46c8-BA63-3147C6EF67F0}
        private int InitDept()
        {
            ArrayList depts = managerIntegrate.GetDepartment();
            if (depts == null)
            {
                MessageBox.Show(Language.Msg("加载科室信息出错!") + this.managerIntegrate.Err);
                return -1;
            }
            this.cmbDept.AddItems(depts);
            return 1;
        }

        /// <summary>
        /// 单据打印
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="feeItemCollection"></param>
        protected void Print(Neusoft.HISFC.Models.RADT.PatientInfo patient, List<Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList> feeItemCollection)
        {
            if (this.printInterface == null)
            {
                this.printInterface = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInpatientChargePrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInpatientChargePrint;
            }

            if (this.printInterface != null)
            {
                this.printInterface.Patient = patient;
                this.printInterface.SetData(feeItemCollection);

                this.printInterface.Print();
            }
            else
            {
                return;
                //MessageBox.Show(Language.Msg("打印接口未进行设置,不进行单据打印"));
            }
        }

        #endregion

        #region 控件操作

        /// <summary>
        /// 增加ToolBar控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("清屏", "清除录入的信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q清空, true, false, null);
            toolBarService.AddToolButton("帮助", "打开帮助文件", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B帮助, true, false, null);
            toolBarService.AddToolButton("增加", "增加一条项目录入行", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.T添加, true, false, null);
            toolBarService.AddToolButton("删除", "删除一条录入的项目", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.S删除, true, false, null);
            //{1E64A9A8-F0CC-449d-B16C-1C8B6D226839}
            toolBarService.AddToolButton("保存组套", "保存患者收费组套", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Z增量保存, true, false, null);
            toolBarService.AddToolButton("调入组套", "调入患者的收费组套", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.H换单, true, false, null);
           // this.toolBarService.AddToolButton("高值耗材扫码", "高值耗材扫码", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C查找, true, false, null);
            return this.toolBarService;
        }

        /// <summary>
        /// 自定义按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "清屏":
                    this.Clear();
                    break;
                case "增加":
                    this.ucInpatientCharge_new1.AddRow();
                    break;
                case "删除":
                    this.ucInpatientCharge_new1.DelRow();
                    break;
                //{1E64A9A8-F0CC-449d-B16C-1C8B6D226839}
                case "调入组套":
                    {
                        this.ucInpatientCharge_new1.ExpFeeGroup();
                        break;
                    }
                case "保存组套":
                    {
                        this.ucInpatientCharge_new1.SaveFeeGroup();
                        break;
                    }
                //case "高值耗材扫码":
                //    {
                //        this.ucInpatientCharge_new1.ScanCodeHighHC();
                //        break;
                //    }
            }

            base.ToolStrip_ItemClicked(sender, e);
        }

        protected override int OnQuery(object sender, object neuObject)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.LoadFrom(@"Report.dll");

            object uc = a.CreateInstance("Report.InpatientFee.ucPatientFeeQuery");

            if (uc != null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowControl((Control)uc);
            }

            return base.OnQuery(sender, neuObject);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 按键设置
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.F11)
            {
                this.ucQueryPatientInfo.Focus();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void ucCharge_new_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.Init();
            }
        }

        /// <summary>
        /// Save事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <returns></returns>
        protected override int OnSave(object sender, object neuObject)
        {
            if (Neusoft.FrameWork.WinForms.Classes.Function.Msg("是否要进行收费？", 422) == DialogResult.No)
            {
                return -1;
            }
            this.Save();

            return base.OnSave(sender, neuObject);
        }

        private void cmbDoct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDoct.Tag != null)
            {
                if (this.patientInfo == null)
                {
                    return;
                }

                this.patientInfo.PVisit.AdmittingDoctor.ID = this.cmbDoct.Tag.ToString();
                this.patientInfo.PVisit.AdmittingDoctor.Name = this.cmbDoct.Text;

                this.ucInpatientCharge_new1.RecipeDoctCode = this.cmbDoct.Tag.ToString();
            }
        }

        private void cmbDoct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ucInpatientCharge_new1.Focus();
            }
        }

        /// <summary>
        /// TreeView双击或者Selected响应事件 {B8E87F3F-E397-42f3-9418-6FC4DEE36729} wbo 2010-08-27
        /// </summary>
        /// <param name="neuObject"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override int OnSetValue(object neuObject, TreeNode e)
        {
            this.tv.Nodes[0].Checked = false;
            e.Checked = !e.Checked;
            if (!e.Checked)
            {
                this.Clear();
                return -2;
            }

            this.Clear();
            this.patientInfo = neuObject as Neusoft.HISFC.Models.RADT.PatientInfo;
            this.PatientInfo = this.radtIntegrate.GetPatientInfomation(this.patientInfo.ID);
            return base.OnSetValue(neuObject, e);
        }

        #endregion

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get
            {
                return new Type[] { typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInpatientChargePrint) };
            }
        }

        #endregion
        //{7376038F-EFE8-46c8-BA63-3147C6EF67F0}
        private void cmbDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ucInpatientCharge_new1.ChangeDept(this.cmbDept.SelectedItem);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value != null && this.patientInfo!=null)
            {
                if (dateTimePicker1.Value< this.patientInfo.PVisit.InTime)//判断执行时间是否在入院时间之前
                {
                    MessageBox.Show("执行时间在患者入院时间之前，请判断执行时间是否输入正确！");
                }
                this.ucInpatientCharge_new1.ExecOrderOperTime = dateTimePicker1.Value;
            }
        }
    }
}
