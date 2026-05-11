using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

namespace Neusoft.HISFC.Components.Common.Controls
{
    /// <summary>
    /// 门诊卡号输入控件
    /// 返回
    /// </summary>
    public partial class ucQuerySeeNoByCardNo : UserControl
    {
        public ucQuerySeeNoByCardNo()
        {
            InitializeComponent();
        }

        #region 私有变量

        /// <summary>
        /// 多个挂号记录，显示选择框
        /// </summary>
        private System.Windows.Forms.Form listform;

        private System.Windows.Forms.ListBox lst;

        /// <summary>
        /// 是否启用一个患者多重身份选择
        /// </summary>
        private bool isUserOnePersonMorePact = false;

        /// <summary>
        /// 是否启用一个患者多重身份选择
        /// </summary>
        public bool IsUserOnePersonMorePact
        {
            get
            {
                return isUserOnePersonMorePact;
            }
            set
            {
                isUserOnePersonMorePact = value;
            }
        }

        /// <summary>
        /// 挂号、退号有效天数
        /// 负数表示只查询当天挂号患者
        /// </summary>
        private decimal validDays = 1;

        /// <summary>
        /// 返回信息
        /// </summary>
        public event Controls.myEventDelegate myEvents;

        /// <summary>
        /// 是否按照看诊医生过滤有效挂号记录，否则只要是同一个科室的都认为有效
        /// </summary>
        private bool isValideRegByDoct = true;

        /// <summary>
        /// 哈希表存储：clinic_no/regObj
        /// </summary>
        private Hashtable hsReg = new Hashtable();

        //private Neusoft.FrameWork.Public.ObjectHelper deptHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        //private Neusoft.FrameWork.Public.ObjectHelper emplHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 免挂号科室
        /// </summary>
        private Hashtable hsNoSupplyRegDept = new Hashtable();

        /// <summary>
        /// 查询挂号列表后操作接口
        /// </summary>
        private Neusoft.HISFC.BizProcess.Interface.Order.IAfterQueryRegList IAfterQueryRegList = null;

        #region 业务层

        /// <summary>
        /// 患者基本信息
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Registration.Registration regInterMgr = new Neusoft.HISFC.BizProcess.Integrate.Registration.Registration();

        /// <summary>
        /// 常数管理业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant conManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        //{BCF08E42-A911-42b5-946A-703B8AD81D7C}
        private Neusoft.HISFC.BizLogic.Fee.Account account = new Neusoft.HISFC.BizLogic.Fee.Account();

        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 控制参数管理
        /// </summary>
        Neusoft.FrameWork.Management.ControlParam contrlManager = new Neusoft.FrameWork.Management.ControlParam();

        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctrlMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        #endregion

        #endregion

        #region 属性

        /// <summary>
        /// 得到多条看诊序号信息数组
        /// </summary>
        private ArrayList alSeeNo = new ArrayList();

        /// <summary>
        /// 该控件使用的地方：挂号、办卡、医生站、收费处、其他
        /// </summary>
        private enumUseType useType = enumUseType.Other;

        /// <summary>
        /// 是否提示补挂号
        /// </summary>
        //private bool isTipAddNewReg = true;

        /// <summary>
        /// 该控件使用的地方：挂号、办卡、医生站、收费处、其他
        /// </summary>
        public enumUseType UseType
        {
            get
            {
                return useType;
            }
            set
            {
                useType = value;
            }
        }

        /// <summary>
        /// 隔日是否提示补收挂号费 0 不提示，不补收；1 提示是否补收；2 不提示，补收(HNMZ21)
        /// </summary>
        private int isAddRegFee_OtherDay = 1;

        /// <summary>
        /// 换医生是否提示补收挂号费 0 不提示，不补收；1 提示是否补收；2 不提示，补收 (HNMZ22)
        /// </summary>
        private int isAddRegFee_OtherDoct = 1;

        /// <summary>
        /// 当前患者是否重新补收挂号费、诊金
        /// </summary>
        //private bool isAddRegFee = false;

        /// <summary>
        /// 是否允许用cardNo直接看诊(避免门诊账户情况下，部分医生在患者不在的情况下 自己开立扣费）
        /// </summary>
        private bool isAllowUserCardNoAdded = true;

        /// <summary>
        /// 不挂号是否允许看诊
        /// houwb 2011-3-16 {3F83B7FE-39C7-4f13-87BB-B0B229F2949F}
        /// </summary>
        private bool isAllowNoRegSee = false;

        /// <summary>
        /// 不挂号是否允许看诊
        /// </summary>
        public bool IsAllowNoRegSee
        {
            get
            {
                return isAllowNoRegSee;
            }
            set
            {
                isAllowNoRegSee = value;
            }
        }

        /// <summary>
        /// 是否允许补挂号
        /// </summary>
        private bool isAllowAddNewReg = false;

        /// <summary>
        /// 是否允许补挂号
        /// </summary>
        //public bool IsAllowAddNewReg
        //{
        //    get
        //    {
        //        //return this.isAllowAddNewReg;

        //        //先处理为简易门诊允许补挂号，其他门诊不允许
        //        if (((Neusoft.HISFC.Models.Base.Employee)FrameWork.Management.Connection.Operator).Dept.Name.Contains("简易"))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        /// <summary>
        /// 允许挂号的科室列表 同时和isAllowAddNewReg有关
        /// 如果科室列表为空，则所有科室都可以补挂号
        /// </summary>
        private ArrayList alAllowAddRegDept = null;

        /// <summary>
        /// 限制允许不挂号就看诊的科室列表，为空，则表示不限制科室
        /// </summary>
        private ArrayList alAllowNoRegSeeDept = null;

        /// <summary>
        /// 当前患者
        /// </summary>
        private Neusoft.HISFC.Models.Registration.Register myRegister = new Neusoft.HISFC.Models.Registration.Register();

        /// <summary>
        /// 当前登记信息
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Neusoft.HISFC.Models.Registration.Register Register
        {
            get
            {
                return this.myRegister;
            }
            set
            {
                this.myRegister = value;
            }
        }

        #endregion

        #region 方法

        

        /// <summary>
        /// 挂号级别帮助类
        /// </summary>
        Neusoft.FrameWork.Public.ObjectHelper regLevlHelper = null;

        /// <summary>
        /// 急诊挂号级别编码
        /// </summary>
        string emergencyLevlCode = "";

        private void ucQuerySeeNoByCardNo_Load(object sender, System.EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            try
            {
                if (this.useType == enumUseType.Charge)
                {
                    //收费允许的挂号有效天数
                    this.validDays = Neusoft.FrameWork.Function.NConvert.ToDecimal(contrlManager.QueryControlerInfo("MZ0014"));
                }
                else if (this.useType == enumUseType.Doct)
                {
                    //开立医嘱允许的挂号有效天数
                    this.validDays = Neusoft.FrameWork.Function.NConvert.ToDecimal(contrlManager.QueryControlerInfo("200022"));
                }
                else if (this.useType == enumUseType.CancelFee)
                {
                    //退费允许的挂号有效天数 暂时和收费一样
                    this.validDays = Neusoft.FrameWork.Function.NConvert.ToDecimal(contrlManager.QueryControlerInfo("MZ0014"));
                }
                else
                {
                    this.validDays = 99999;
                }

                this.isAllowAddNewReg = Neusoft.FrameWork.Function.NConvert.ToBoolean(contrlManager.QueryControlerInfo("200030"));


                //houwb 2011-3-16 {3F83B7FE-39C7-4f13-87BB-B0B229F2949F}
                this.isAllowNoRegSee = ctrlMgr.GetControlParam<bool>("200060", false, false);
                //this.isTipAddNewReg = this.ctrlMgr.GetControlParam<bool>("MZ0091", false, true);

                isAllowUserCardNoAdded = ctrlMgr.GetControlParam<bool>("HNMZ20", true, true);

                isAddRegFee_OtherDay = ctrlMgr.GetControlParam<int>("HNMZ21", true, 1);
                isAddRegFee_OtherDoct = ctrlMgr.GetControlParam<int>("HNMZ22", true, 1);

                alAllowAddRegDept = conManager.GetList("AllowAddRegDept");

                alAllowNoRegSeeDept = conManager.GetList("AllowNoRegSeeDept");

                #region 免挂号科室

                ArrayList alNoSupplyRegDept = this.conManager.GetList("NoSupplyRegDept");
                if (alNoSupplyRegDept == null)
                {
                    MessageBox.Show("ucQuerySeeNoByCardNo_Load" + this.conManager.Err);
                    //return -1;
                }
                foreach (Neusoft.HISFC.Models.Base.Const obj in alNoSupplyRegDept)
                {
                    if (!hsNoSupplyRegDept.Contains(obj.ID) && obj.IsValid)
                    {
                        hsNoSupplyRegDept.Add(obj.ID, obj);
                    }
                }
                #endregion

                #region 获取所有挂号级别

                Neusoft.HISFC.Models.Registration.RegLevel emergRegLevl = SOC.HISFC.BizProcess.Cache.Fee.GetEmergRegLevl();
                if (emergRegLevl == null)
                {
                    MessageBox.Show("急诊挂号级别没有维护！会导致补收挂号费错误!\r\n如果无急诊挂号级别，请增加维护并停用即可！\r\n请联系信息科重新维护" + regInterMgr.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                emergencyLevlCode = emergRegLevl.ID;

                //if (regLevlHelper == null)
                //{
                //    regLevlHelper = new Neusoft.FrameWork.Public.ObjectHelper();

                    //获取所有的挂号级别
                    //ArrayList al = regInterMgr.QueryAllRegLevel();

                    //有效的挂号级别
                    //ArrayList alValidReglevl = new ArrayList();

                    //if (al == null || al.Count == 0)
                    //{
                    //    MessageBox.Show("查询所有挂号级别错误！会导致补收挂号费错误!\r\n请联系信息科重新维护" + regInterMgr.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                    //else
                    //{
                    //    bool isValidEmergency = true;
                    //    foreach (Neusoft.HISFC.Models.Registration.RegLevel regLevl in al)
                    //    {
                    //        if (regLevl.IsValid)
                    //        {
                    //            alValidReglevl.Add(regLevl);

                    //            if (regLevl.IsEmergency)
                    //            {
                    //                emergencyLevlCode = regLevl.ID;
                    //                break;
                    //            }
                    //        }
                    //        else if (regLevl.IsEmergency)
                    //        {
                    //            isValidEmergency = false;
                    //        }
                    //    }

                    //    if (string.IsNullOrEmpty(emergencyLevlCode) && isValidEmergency)
                    //    {
                    //        MessageBox.Show("急诊挂号级别没有维护！会导致补收挂号费错误!\r\n如果无急诊挂号级别，请增加维护并停用即可！\r\n请联系信息科重新维护" + regInterMgr.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    }
                    //}
                //}
                #endregion

                //考虑到妇幼特殊需求，此处增加了接口处理
                if (IAfterQueryRegList == null)
                {
                    IAfterQueryRegList = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.Order.IAfterQueryRegList)) as Neusoft.HISFC.BizProcess.Interface.Order.IAfterQueryRegList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ucQuerySeeNoByCardNo_Load" + ex.Message);
            }
        }

        /// <summary>
        /// 不挂号是否允许看诊
        /// </summary>
        /// <returns></returns>
        private bool AllowNoRegSee()
        {
            bool isAllowedDept = true;
            if (alAllowNoRegSeeDept != null
                && alAllowNoRegSeeDept.Count > 0)
            {
                isAllowedDept = false;
                foreach (Neusoft.HISFC.Models.Base.Const conObj in alAllowAddRegDept)
                {
                    if (conObj.ID.Trim() == this.GetReciptDept().ID)
                    {
                        isAllowedDept = true;
                    }
                }
            }
            return isAllowedDept && isAllowNoRegSee;
        }

        /// <summary>
        /// 根据开立科室获取是否允许补挂号
        /// </summary>
        /// <returns></returns>
        private bool AllowAddNewReg()
        {
            if (isAllowAddNewReg)
            {
                if (alAllowAddRegDept != null && alAllowAddRegDept.Count > 0)
                {
                    foreach (Neusoft.HISFC.Models.Base.Const conObj in alAllowAddRegDept)
                    {
                        if (conObj.ID.Trim() == this.GetReciptDept().ID)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void AddNewReg()
        {
            #region 医生补挂号

            string name = string.Empty;
            if (txtInputCode.Text.Trim() != "输入卡号或姓名"
                && !string.IsNullOrEmpty(txtInputCode.Text)
                && (txtInputCode.Text.StartsWith("/") || txtInputCode.Text.StartsWith("+")))
            {
                name = this.txtInputCode.Text.Remove(0, 1);
            }

            Forms.frmRegistrationByDoctor frmDoctRegistration = new Forms.frmRegistrationByDoctor(name);
            frmDoctRegistration.EmergencyLevlCode = this.emergencyLevlCode;
            frmDoctRegistration.IAfterQueryRegList = this.IAfterQueryRegList;

            frmDoctRegistration.ShowDialog();
            if (frmDoctRegistration.DialogResult == DialogResult.Cancel)
            {
                return;
            }

            this.myRegister = frmDoctRegistration.PatientInfo;
            if (this.myRegister.ID == "" || this.myRegister.ID == null)
            {
                this.ClearInfo();
            }
            else
            {
                this.txtInputCode.Text = myRegister.PID.CardNO;

                if (isUserOnePersonMorePact)
                {
                    if (account.GetPatientPactInfo(myRegister) == -1)
                    {
                        MessageBox.Show("获取患者合同单位信息失败：" + account.Err);
                        return;
                    }

                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    if (myRegister.MutiPactInfo.Count > 0)
                    {
                        if (FrameWork.WinForms.Classes.Function.ChooseItem(new ArrayList(myRegister.MutiPactInfo), new string[] { "合同单位编码", "合同单位", "有效性" }, new bool[] { }, new int[] { 0, 100, 70 }, ref obj) != 1)
                        {
                            return;
                        }
                    }
                    myRegister.Pact = obj as Neusoft.HISFC.Models.Base.PactInfo;
                }
                this.myEvents();
            }

            #endregion
        }

        /// <summary>
        /// 病历号回车后，判断是否补录挂号信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInputCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //先把全角转换为半角,避免把符号误认为汉字 houwb 2011-3-11 {E0AA533A-F09B-47ed-B6EB-C9ADC591F333}
                this.txtInputCode.Text = Neusoft.FrameWork.Function.NConvert.ToDBC(this.txtInputCode.Text.Trim());

                string cardNO = this.txtInputCode.Text;
                if (AllowAddNewReg()
                    && this.useType == enumUseType.Doct
                    && (cardNO.StartsWith("/") || cardNO.StartsWith("+")))
                {
                    DialogResult r = MessageBox.Show("是否快速补挂号？\r\n", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (r == DialogResult.Yes)
                    {
                        AddNewReg();
                    }
                    else
                    {
                        this.ClearInfo();
                    }
                }
                else
                {
                    if (Regex.IsMatch(cardNO, @"^\d*$"))
                    {
                        Neusoft.HISFC.Models.Account.AccountCard accountCard = new Neusoft.HISFC.Models.Account.AccountCard();

                        int flag = feeIntegrate.ValidMarkNO(cardNO, ref accountCard);

                        if (flag > 0)
                        {
                            cardNO = accountCard.Patient.PID.CardNO;
                        }
                        //返回错误了
                        else
                        {
                            MessageBox.Show(feeIntegrate.Err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        this.txtInputCode.Text = cardNO;
                    }
                    else
                    {
                        #region 汉字开头默认是查询患者

                        Components.Common.Forms.frmQueryPatientByName frmQuery = new Neusoft.HISFC.Components.Common.Forms.frmQueryPatientByName();
                        frmQuery.QueryByName(this.txtInputCode.Text.Trim());
                        frmQuery.SelectedPatient += new Neusoft.HISFC.Components.Common.Forms.frmQueryPatientByName.GetPatient(frmQuery_SelectedPatient);
                        frmQuery.ShowDialog(this);

                        #region 旧的作废

                        //Neusoft.HISFC.BizLogic.RADT.InPatient myInpatient = new Neusoft.HISFC.BizLogic.RADT.InPatient();

                        //ArrayList alReg = myInpatient.QueryPatientByName(this.txtInputCode.Text.Trim());

                        //if (alReg == null || alReg.Count <= 0)
                        //{
                        //    MessageBox.Show("没有找到姓名为：[" + this.txtInputCode.Text + "]的患者!", "提示");
                        //    return;
                        //}

                        //Neusoft.FrameWork.WinForms.Controls.NeuListView lvAllReg = new Neusoft.FrameWork.WinForms.Controls.NeuListView();

                        //System.Windows.Forms.ColumnHeader colCardID1 = new ColumnHeader();
                        //System.Windows.Forms.ColumnHeader colName1 = new ColumnHeader();
                        //System.Windows.Forms.ColumnHeader colSex1 = new ColumnHeader();
                        //System.Windows.Forms.ColumnHeader colOrder1 = new ColumnHeader();
                        //System.Windows.Forms.ColumnHeader colDate1 = new ColumnHeader();
                        //System.Windows.Forms.ColumnHeader colRegDept1 = new ColumnHeader();

                        //colCardID1.Text = "病历号";
                        //colCardID1.Width = 114;
                        //colName1.Text = "姓名";
                        //colName1.Width = 70;
                        //colSex1.Text = "性别";
                        //colSex1.Width = 50;
                        //colDate1.Text = "电话";
                        //colDate1.Width = 150;
                        //colRegDept1.Text = "地址";
                        //colRegDept1.Width = 100;

                        //lvAllReg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                        //                        colCardID1,
                        //                        colName1,
                        //                        colSex1,
                        //                        colDate1,
                        //                        colRegDept1});

                        //lvAllReg.Dock = System.Windows.Forms.DockStyle.Fill;
                        //lvAllReg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        //lvAllReg.FullRowSelect = true;
                        //lvAllReg.GridLines = true;
                        //lvAllReg.Location = new System.Drawing.Point(0, 0);
                        //lvAllReg.Name = "lvAllReg";
                        //lvAllReg.Size = new System.Drawing.Size(500, 250);
                        //lvAllReg.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
                        //lvAllReg.TabIndex = 1;
                        //lvAllReg.UseCompatibleStateImageBehavior = false;
                        //lvAllReg.View = System.Windows.Forms.View.Details;

                        //foreach (Neusoft.HISFC.Models.RADT.PatientInfo regObj in alReg)
                        //{
                        //    ListViewItem item = new ListViewItem();
                        //    item.Text = regObj.PID.CardNO;
                        //    item.Tag = regObj;
                        //    item.SubItems.Add(regObj.Name);
                        //    item.SubItems.Add(regObj.Sex.Name);
                        //    item.SubItems.Add(regObj.PhoneHome);
                        //    item.SubItems.Add(regObj.AddressHome);

                        //    lvAllReg.Items.Add(item);
                        //}

                        //lvAllReg.DoubleClick += new EventHandler(lvAllReg_DoubleClick);

                        //Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(lvAllReg, FormBorderStyle.None);
                        #endregion
                        #endregion
                    }

                    int rev = this.Query();

                    if (rev == -1)
                    {
                        return;
                    }
                }
            }
        }

        void frmQuery_SelectedPatient(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            this.txtInputCode.Text = patientInfo.PID.CardNO;
            //Query(patientInfo.PID.CardNO);
        }

        public void Focus()
        {
            this.txtInputCode.Focus();
        }

        private void ucQuerySeeNoByCardNo_Leave(object sender, EventArgs e)
        {
            this.Text = "输入卡号或姓名";
        }

        #region 多个挂号记录时，弹出选择


        /// <summary>
        /// 弹出选择的患者列表
        /// </summary>
        ArrayList alPatientList = null;

        /// <summary>
        /// 选择的患者
        /// </summary>
        Neusoft.HISFC.Models.Base.Spell patientObj = null;

        Neusoft.HISFC.Models.Registration.Register regObj;

        /// <summary>
        /// 存放人员信息
        /// </summary>
        private Hashtable hsEmpl = new Hashtable();

        /// <summary>
        /// 存放科室信息
        /// </summary>
        private Hashtable hsDept = new Hashtable();

        /// <summary>
        /// 有多个挂号记录时，选择患者
        /// </summary>
        private void SelectPatient()
        {
            #region 之前的

            //lst = new ListBox();
            //lst.Dock = System.Windows.Forms.DockStyle.Fill;
            //lst.Items.Clear();
            //this.listform = new System.Windows.Forms.Form();
            //this.listform.Text = "选择挂号记录";

            //listform.Size = new Size(300, 200);
            //listform.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;

            //Neusoft.HISFC.Models.Registration.Register regObj;
            //for (int i = 0; i < this.alSeeNo.Count; i++)
            //{
            //    regObj = this.alSeeNo[i] as Neusoft.HISFC.Models.Registration.Register;

            //    //显示姓名，看诊科室、看诊日期（去掉时间）、看诊序号
            //    lst.Items.Add(regObj.ID + "  " + regObj.Name + "  " + regObj.DoctorInfo.Templet.Dept.Name + "  " + regObj.DoctorInfo.SeeDate.Date.ToString("yyyy年MM月dd日"));
            //}

            //if (lst.Items.Count == 1)
            //{
            //    try
            //    {
            //        if (this.CheckRegInfo(this.Register) == -1)
            //        {
            //            return;
            //        }

            //        this.listform.Close();
            //    }
            //    catch { }
            //    try
            //    {
            //        //this.txtInputCode.Text = this.strSeeNo.Substring(4, 10);
            //        this.myEvents();
            //    }
            //    catch { }
            //    return;
            //}

            //if (lst.Items.Count <= 0)
            //{
            //    if (this.CheckRegInfo(this.Register) == -1)
            //    {
            //        return;
            //    }
            //    //this.strSeeNo = "";
            //    this.myEvents();
            //    return;
            //}

            //lst.Visible = true;
            //lst.DoubleClick += new EventHandler(lst_DoubleClick);
            //lst.KeyDown += new KeyEventHandler(lst_KeyDown);
            //lst.Show();

            //listform.Controls.Add(lst);

            //listform.TopMost = true;

            //listform.Show();
            //listform.Location = this.txtInputCode.PointToScreen(new Point(this.txtInputCode.Width / 2 + this.txtInputCode.Left, this.txtInputCode.Height + this.txtInputCode.Top));
            //try
            //{
            //    lst.SelectedIndex = 0;
            //    lst.Focus();
            //    lst.LostFocus += new EventHandler(lst_LostFocus);
            //}
            //catch { }

            #endregion

            alPatientList = new ArrayList();

            Neusoft.HISFC.Models.Base.Employee doctObj=null;
            Neusoft.HISFC.Models.Base.Department seeDcpt=null;
            for (int i = 0; i < this.alSeeNo.Count; i++)
            {
                regObj = this.alSeeNo[i] as Neusoft.HISFC.Models.Registration.Register;

                patientObj = new Neusoft.HISFC.Models.Base.Spell();
                patientObj.ID = regObj.ID;
                patientObj.Name = regObj.Name;
                patientObj.Memo = regObj.Pact.Name;

                if (!string.IsNullOrEmpty(regObj.DoctorInfo.Templet.Dept.ID))
                {
                    patientObj.SpellCode = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(regObj.DoctorInfo.Templet.Dept.ID);
                }
                //if (!string.IsNullOrEmpty(regObj.SeeDoct.Dept.ID))
                //{
                //    if (hsDept.Contains(regObj.SeeDoct.Dept.ID))
                //    {
                //        seeDcpt = hsDept[regObj.SeeDoct.Dept.ID] as Neusoft.HISFC.Models.Base.Department;
                //    }
                //    else
                //    {
                //        seeDcpt = this.managerIntegrate.GetDepartment(regObj.SeeDoct.Dept.ID);
                //        if (seeDcpt == null)
                //        {
                //            MessageBox.Show("查询科室信息出错：" + managerIntegrate.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        }
                //        hsDept.Add(seeDcpt.ID, seeDcpt);
                //    }
                //    patientObj.SpellCode = seeDcpt.Name;
                //}


                if (!string.IsNullOrEmpty(regObj.DoctorInfo.Templet.Doct.ID))
                {
                    patientObj.WBCode = SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(regObj.DoctorInfo.Templet.Doct.ID);
                }

                //if (!string.IsNullOrEmpty(regObj.SeeDoct.ID))
                //{
                //    if (hsEmpl.Contains(regObj.SeeDoct.ID))
                //    {
                //        doctObj = hsEmpl[regObj.SeeDoct.ID] as Neusoft.HISFC.Models.Base.Employee;
                //    }
                //    else
                //    {
                //        doctObj = this.managerIntegrate.GetEmployeeInfo(regObj.SeeDoct.ID);
                //        if (doctObj == null)
                //        {
                //            MessageBox.Show("查询开立医生信息出错：" + managerIntegrate.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        }
                //        hsEmpl.Add(doctObj.ID, doctObj);
                //    }
                //    patientObj.WBCode = doctObj.Name;
                //}
                //patientObj.UserCode= regObj.DoctorInfo.SeeDate.Date.ToString("yyyy年MM月dd日");
                patientObj.UserCode = regObj.DoctorInfo.SeeDate.ToString();
                alPatientList.Add(patientObj);
            }

            if (alPatientList.Count <= 1)
            {
                try
                {
                    if (this.CheckRegInfo(ref regObj) == -1)
                    {
                        return;
                    }
                    this.Register = regObj;

                    this.myEvents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SelectPatient" + ex.Message);
                }
                return;
            }

            Neusoft.FrameWork.Models.NeuObject selectPatient = null;
            if (alPatientList.Count > 0)
            {
                if (FrameWork.WinForms.Classes.Function.ChooseItem(alPatientList, new string[] { "门诊流水号", "姓名", "合同单位", "挂号科室", "挂号医生", "挂号时间" }, new bool[] { false, true, true, true, true, true }, new int[] { 50, 50, 70, 100, 70, 160 }, ref selectPatient) != 1)
                {
                    return;
                }
            }

            try
            {
                regObj = this.hsReg[selectPatient.ID] as Neusoft.HISFC.Models.Registration.Register;

                if (this.CheckRegInfo(ref regObj) == -1)
                {
                    return;
                }
                Register = regObj;

                this.myEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SelectPatient" + ex.Message);
            }

            return;
        }

        private void lst_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                GetInfo();
            }
            catch { }
        }

        private void lst_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetInfo();
            }
        }

        private void lst_LostFocus(object sender, EventArgs e)
        {
            this.listform.Hide();
            //if (this.strSeeNo == "") ClearInfo();
        }

        /// <summary>
        /// 获得患者挂号信息
        /// </summary>
        private void GetInfo()
        {
            try
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

                obj.ID = lst.Items[lst.SelectedIndex].ToString();

                string clinicCode = obj.ID.Substring(0, obj.ID.IndexOf(" "));
                regObj = this.hsReg[clinicCode] as Neusoft.HISFC.Models.Registration.Register;

                if (this.CheckRegInfo(ref regObj) == -1)
                {
                    return;
                }
                this.Register = regObj;

                this.listform.Hide();
                this.myEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetInfo" + ex.ToString()); 
                ClearInfo();
            }
        }

        #endregion

        /// <summary>
        /// 清空信息
        /// </summary>
        private void ClearInfo()
        {
            this.txtInputCode.Text = "";
            this.txtInputCode.Focus();
        }

        /// <summary>
        /// 有效挂号信息
        /// </summary>
        ArrayList alReg = null;

        /// <summary>
        /// 获取挂号有效时间
        /// </summary>
        /// <returns></returns>
        public decimal GetRegValideDate(bool isEmergency)
        {
            //普通门诊当天有效，急诊24小时有效,对应控制参数为200022

            //默认24小时有效
            decimal valideDate = 24;

            valideDate = ctrlMgr.GetControlParam<decimal>("200022", false, 24);

            if (isEmergency)
            {
                valideDate = Math.Floor(valideDate / 24) * 24;
            }

            return valideDate;
        }

        /// <summary>
        /// 查询患者挂号信息
        /// </summary>
        /// <returns>1:查询到有效挂号记录 0:没有有效挂号记录 -1:出错</returns>
        protected int Query()
        {
            this.hsReg.Clear();
            this.alSeeNo.Clear();

            DateTime dtQueryBegin = this.contrlManager.GetDateTimeFromSysDateTime();
            //当天的23点59分59秒
            DateTime dtEnd = this.contrlManager.GetDateTimeFromSysDateTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            if (validDays <= 0)
            {
                dtQueryBegin = dtQueryBegin.Date;
            }
            else
            {
                dtQueryBegin = dtQueryBegin.AddDays(0 - (double)this.validDays);
            }

            //门诊当天有效，急诊24小时有效
            if (((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.Name.Contains("急") || ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.Name.Contains("发热"))
            {
                //validDays = Math.Ceiling(validDays) * 24;
                //if (validDays == 0)
                //{
                //    validDays = 24;
                //}
                validDays = Math.Ceiling(validDays) * 24;
                if (validDays <= 0)
                {
                    validDays = 24;
                }
                if (validDays >=24)
                {
                    dtQueryBegin = dtQueryBegin.AddDays(0 - (double)Math.Floor(validDays / 24));
                    //validDays = 1;
                }
                if (dtQueryBegin.Date != dtEnd.Date)//不同一天 说明跨天了 结束日期取当前日期就可以了 2017-4-25 chengym
                {
                    dtEnd = this.contrlManager.GetDateTimeFromSysDateTime();
                }
            }

            //dtQueryBegin = dtQueryBegin.AddDays(0 - (double)GetRegValideDate();

            try
            {
                //根据患者主索引接口获取门诊病历号
                Neusoft.HISFC.Models.Account.AccountCard accountCardObj = new Neusoft.HISFC.Models.Account.AccountCard();

                //对于办卡和挂号，只查询卡号就可以了
                if (this.useType == enumUseType.Register || this.useType == enumUseType.TransactCard)
                {
                    accountCardObj.Memo = this.useType == enumUseType.Register ? "1" : "2";

                    if (this.feeIntegrate.ValidMarkNO(this.txtInputCode.Text, ref accountCardObj) <= 0)
                    {
                        MessageBox.Show("Query" + this.feeIntegrate.Err);
                        return -1;
                    }
                    this.txtInputCode.Text = accountCardObj.Patient.PID.CardNO;
                    return 1;
                }
                else
                {
                    if (this.feeIntegrate.ValidMarkNO(this.txtInputCode.Text, ref accountCardObj) <= 0)
                    {
                        MessageBox.Show("Query" + this.feeIntegrate.Err);
                        return -1;
                    }

                    if (!isAllowUserCardNoAdded)
                    {
                        if (this.txtInputCode.Text.PadLeft(10, '0') == accountCardObj.Patient.PID.CardNO)
                        {
                            MessageBox.Show("只能通过刷卡开立！如有疑问请联系信息科！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return 1;
                        }
                    }

                    this.txtInputCode.Text = accountCardObj.Patient.PID.CardNO;

                    #region 查询有效的挂号记录

                    if (this.cbxNewReg.Checked)
                    {
                        alReg = new ArrayList();
                    }
                    else
                    {
                        //查询有效看诊时间段内所有有效的挂号记录
                        //alReg = regInterMgr.Query(accountCardObj.Patient.PID.CardNO, dtQueryBegin);
                        if (this.useType == enumUseType.Doct)
                        {
                            alReg = regInterMgr.QueryWithHosCode(accountCardObj.Patient.PID.CardNO, dtQueryBegin, dtEnd, Neusoft.FrameWork.Management.Connection.Hospital.ID);
                        }
                        else
                        {
                            alReg = regInterMgr.QueryWithHosCode(accountCardObj.Patient.PID.CardNO, dtQueryBegin, Neusoft.FrameWork.Management.Connection.Hospital.ID);
                        }
                    }

                    //考虑到妇幼特殊需求，此处增加了接口处理
                    if (IAfterQueryRegList != null)
                    {
                        if (IAfterQueryRegList.OnAfterQueryRegList(alReg, this.reciptDept) == -1)
                        {
                            MessageBox.Show(IAfterQueryRegList.ErrInfo, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return -1;
                        }
                    }

                    //1、挂号有效期内，未看诊
                    //2、挂号有效期内，已看诊，看诊医生是当前医生
                    if (alReg == null || alReg.Count <= 0)
                    {
                        #region 医生站补挂号
                        if (this.useType == enumUseType.Doct)
                        {
                            #region 查不到挂号记录时，可以自动挂号
                            //houwb 2011-3-16 {3F83B7FE-39C7-4f13-87BB-B0B229F2949F}
                            if (this.AllowNoRegSee()
                                && AllowAddNewReg())
                            {
                                Neusoft.HISFC.Models.Registration.Register regObj = this.GetRegInfoFromPatientInfo(accountCardObj.Patient.PID.CardNO);
                                if (regObj == null)
                                {
                                    return -1;
                                }

                                if (!this.hsReg.ContainsKey(regObj.ID))
                                {
                                    this.hsReg.Add(regObj.ID, regObj);
                                }

                                this.alSeeNo.Add(regObj);
                                this.Register = regObj;
                                //return 1;
                            }
                            else
                            {
                                MessageBox.Show("没有查找到该患者在有效时间内的挂号信息!", "警告");
                                this.txtInputCode.Focus();
                                txtInputCode.SelectAll();
                                return -1;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        Neusoft.HISFC.Models.Registration.Register regObj = null;
                        for (int i = 0; i < alReg.Count; i++)
                        {
                            regObj = alReg[i] as Neusoft.HISFC.Models.Registration.Register;

                            //判断是否本科室留观的患者
                            if (regObj.PVisit.InState.ID.ToString() != "N")
                            {
                                if (regObj.SeeDoct.Dept.ID == this.GetReciptDept().ID)
                                {
                                    MessageBox.Show("该患者已在本科室留观！");
                                    return 1;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (!this.hsReg.ContainsKey(regObj.ID))
                            {
                                this.hsReg.Add(regObj.ID, regObj);
                            }

                            #region 门诊医生要判断看诊科室和看诊医生

                            if (this.useType == enumUseType.Doct)
                            {
                                //已看诊，看诊医生不一致
                                if (regObj.IsSee
                                    && regObj.SeeDoct.ID != this.contrlManager.Operator.ID)
                                {
                                    if (this.isAddRegFee_OtherDoct == 0)
                                    {
                                        continue;
                                    }
                                }

                                //已看诊，看诊日期不一致
                                else if (regObj.IsSee
                                    && (regObj.SeeDoct.ID == this.contrlManager.Operator.ID)
                                    && regObj.DoctorInfo.SeeDate.Date != this.conManager.GetDateTimeFromSysDateTime().Date)
                                {
                                    if (isAddRegFee_OtherDay == 0 && ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID != "9225")
                                    {
                                        continue;
                                    }
                                }
                            }
                            #endregion

                            this.alSeeNo.Insert(0, regObj);
                            //this.Register = regObj;//在控件外面重新获得挂号信息
                        }
                    }

                    //未找到有效挂号记录，系统补挂号
                    if (this.alSeeNo == null || this.alSeeNo.Count <= 0)
                    {
                        //houwb 2011-3-16 {3F83B7FE-39C7-4f13-87BB-B0B229F2949F}
                        if (this.useType == enumUseType.Doct
                            && this.AllowNoRegSee()
                            && AllowAddNewReg())
                        {
                            Neusoft.HISFC.Models.Registration.Register regObj = this.GetRegInfoFromPatientInfo(accountCardObj.Patient.PID.CardNO);
                            if (regObj == null)
                            {
                                return -1;
                            }
                            if (!this.hsReg.ContainsKey(regObj.ID))
                            {
                                this.hsReg.Add(regObj.ID, regObj);
                            }

                            //this.alSeeNo.Add(regObj);
                            //this.Register = ((Neusoft.HISFC.Models.Registration.Register)this.alSeeNo[0]);
                            this.Register = regObj;
                        }
                        else
                        {
                            MessageBox.Show("没有查找到该患者在有效时间内的挂号信息!", "警告");
                            this.txtInputCode.Focus();
                            txtInputCode.SelectAll();
                            return -1;
                        }
                    }
                    else if (this.alSeeNo.Count == 1)
                    {
                        regObj = ((Neusoft.HISFC.Models.Registration.Register)this.alSeeNo[0]);
                        if (this.CheckRegInfo(ref regObj) == -1)
                        {
                            return -1;
                        }
                        this.Register = regObj;
                    }
                    else
                    {
                        this.SelectPatient();
                        return 1;
                    }
                    #endregion


                    if (this.listform != null)
                    {
                        this.listform.Close();
                    }
                    this.myEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Query" + ex.Message);
                ClearInfo();
                return -1;
            }

            return 1;
        }


        /// <summary>
        /// 是否补收诊金
        /// </summary>
        /// <param name="index">0 换医生；1 隔日</param>
        /// <param name="index">0 不提示，不补收(无效挂号）；1 提示是否补收；2 不提示，补收</param>
        /// <param name="regObj"></param>
        /// <param name="isAddRegFee"></param>
        /// <returns></returns>
        private int CheckRegInfo(int index, int checkType, ref Neusoft.HISFC.Models.Registration.Register regObj, ref bool isAddRegFee)
        {
            if (!regObj.IsSee)
            {
                return 1;
            }

            if (index == 0)
            {
                //科室、医生不一样 提示是否补收挂号费
                if (!(string.IsNullOrEmpty(regObj.SeeDoct.ID) && string.IsNullOrEmpty(regObj.SeeDoct.Dept.ID))
                    && (regObj.SeeDoct.ID != this.contrlManager.Operator.ID)
                    )
                {
                    //免挂号科室不再提示补收挂号费
                    if (hsNoSupplyRegDept != null
                        && hsNoSupplyRegDept.Contains(((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.ID))
                    {
                        if (checkType == 0)
                        {
                            isAddRegFee = true;
                            return -1;
                        }
                        else if (checkType == 1)
                        {
                            // {57D49CC0-3BEE-4168-8E71-4FAE394DF6A6}  //消化内镜中心属于医技科室，增加修改门诊科室处方功能（限非药品）
                            if (((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.ID == "7012")
                            {
                                regObj.Memo = "不补收";
                                isAddRegFee = false;
                            }
                            else
                            {
                                DialogResult diag = MessageBox.Show("该患者已经看诊，看诊科室为[" + SOC.HISFC.BizProcess.Cache.Common.GetDeptName(regObj.SeeDoct.Dept.ID) + "],\r\n\r\n看诊医生为[" + SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(regObj.SeeDoct.ID) + "]\r\n\r\n看诊时间为：" + regObj.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm:ss") + "\n\r\n本次是否补收挂号费？", "询问", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                if (diag == DialogResult.Yes)
                                {
                                    if (AllowAddNewReg())
                                    {
                                        //如果补收挂号费的话，需要修改患者状态等信息
                                        regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                        if (regObj == null)
                                        {
                                            return -1;
                                        }
                                        regObj.Memo = "不补收";
                                        isAddRegFee = false;
                                    }
                                    else
                                    {
                                        MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else if (diag == DialogResult.Cancel)
                                {
                                    isAddRegFee = false;
                                    return -1;
                                }
                                else
                                {
                                    regObj.Memo = "不补收";
                                    isAddRegFee = false;
                                }
                            }
                        }
                        else
                        {
                            if (AllowAddNewReg())
                            {
                                //如果补收挂号费的话，需要修改患者状态等信息
                                regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                if (regObj == null)
                                {
                                    return -1;
                                }
                                regObj.Memo = "不补收";
                                isAddRegFee = false;
                            }
                            else
                            {
                                MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        if (checkType == 0)
                        {
                            isAddRegFee = true;
                            return -1;
                        }
                        else if (checkType == 1)
                        {
                            DialogResult diag = MessageBox.Show("该患者已经看诊，看诊科室为[" + SOC.HISFC.BizProcess.Cache.Common.GetDeptName(regObj.SeeDoct.Dept.ID) + "],\r\n\r\n看诊医生为[" + SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(regObj.SeeDoct.ID) + "]\r\n\r\n看诊时间为：" + regObj.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm:ss") + "\n\r\n本次是否补收挂号费？", "询问", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (diag == DialogResult.Yes)
                            {
                                if (AllowAddNewReg())
                                {
                                    //如果补收挂号费的话，需要修改患者状态等信息
                                    regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                    if (regObj == null)
                                    {
                                        return -1;
                                    }
                                    isAddRegFee = true;
                                }
                                else
                                {
                                    MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else if (diag == DialogResult.Cancel)
                            {
                                isAddRegFee = false;
                                return -1;
                            }
                            else
                            {
                                regObj.Memo = "不补收";
                                isAddRegFee = false;
                            }
                        }
                        else
                        {
                            if (AllowAddNewReg())
                            {
                                //如果补收挂号费的话，需要修改患者状态等信息
                                regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                if (regObj == null)
                                {
                                    return -1;
                                }
                                isAddRegFee = true;
                            }
                            else
                            {
                                MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            else if (index == 1)
            {
                //看诊日期不一样提示是否补收挂号费
                if ((regObj.SeeDoct.ID == this.contrlManager.Operator.ID)
                    && regObj.DoctorInfo.SeeDate.Date != this.conManager.GetDateTimeFromSysDateTime().Date && ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID != "9225")
                {
                    //免挂号科室不再提示补收挂号费
                    if (hsNoSupplyRegDept != null
                        && hsNoSupplyRegDept.Contains(((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.ID))
                    {
                        if (checkType == 0)
                        {
                            isAddRegFee = true;
                            return -1;
                        }
                        else if (checkType == 1)
                        {
                            DialogResult diag = MessageBox.Show("该患者已经看诊，上次看诊医生为[" + SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(regObj.SeeDoct.ID) + "],\r\n\r\n看诊时间为[" + regObj.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm:ss") + "]\n\r\n是否重新挂号？(不再收取挂号费！)", "询问", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (diag == DialogResult.Yes)
                            {
                                if (AllowAddNewReg())
                                {
                                    //如果补收挂号费的话，需要修改患者状态等信息
                                    regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                    if (regObj == null)
                                    {
                                        return -1;
                                    }
                                    regObj.Memo = "不补收";
                                    isAddRegFee = false;
                                }
                                else
                                {
                                    MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else if (diag == DialogResult.Cancel)
                            {
                                isAddRegFee = false;
                                return -1;
                            }
                            else
                            {
                                regObj.Memo = "不补收";
                                isAddRegFee = false;
                            }
                        }
                        else
                        {
                            if (AllowAddNewReg())
                            {
                                //如果补收挂号费的话，需要修改患者状态等信息
                                regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                if (regObj == null)
                                {
                                    return -1;
                                }
                                regObj.Memo = "不补收";
                                isAddRegFee = false;
                            }
                            else
                            {
                                MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        if (checkType == 0)
                        {
                            isAddRegFee = true;
                            return -1;
                        }
                        else if (checkType == 1)
                        {
                            DialogResult diag = MessageBox.Show("该患者已经看诊，上次看诊医生为[" + SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(regObj.SeeDoct.ID) + "],\r\n\r\n看诊时间为[" + regObj.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm:ss") + "]\n\r\n是否重新挂号？(不再收取挂号费！)", "询问", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (diag == DialogResult.Yes)
                            {
                                if (AllowAddNewReg())
                                {
                                    //如果补收挂号费的话，需要修改患者状态等信息
                                    regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                    if (regObj == null)
                                    {
                                        return -1;
                                    }
                                    isAddRegFee = true;
                                }
                                else
                                {
                                    MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else if (diag == DialogResult.Cancel)
                            {
                                isAddRegFee = false;
                                return -1;
                            }
                            else
                            {
                                regObj.Memo = "不补收";
                                isAddRegFee = false;
                            }
                        }
                        else
                        {
                            if (AllowAddNewReg())
                            {
                                //如果补收挂号费的话，需要修改患者状态等信息
                                regObj = this.GetRegInfoFromPatientInfo(regObj.PID.CardNO);
                                if (regObj == null)
                                {
                                    return -1;
                                }
                                isAddRegFee = true;
                            }
                            else
                            {
                                MessageBox.Show("请患者到挂号处补挂号后再来看诊！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// 判断看诊信息
        /// </summary>
        /// <param name="regObj"></param>
        /// <returns>0 不补收，-1 错误</returns>
        private int CheckRegInfo(ref Neusoft.HISFC.Models.Registration.Register regObj)
        {
            //if (!isTipAddNewReg)
            //{
            //    return 1;
            //}

            if (regObj == null)
            {
                return -1;
            }

            //当前患者是否重新补收挂号费、诊金
            bool isAddRegFee = false;

            if (regObj.IsSee)
            {
                if (CheckRegInfo(0, isAddRegFee_OtherDoct, ref regObj, ref isAddRegFee) == -1)
                {
                    return -1;
                }

                if (!isAddRegFee)
                {
                    if (CheckRegInfo(1, isAddRegFee_OtherDay, ref regObj, ref isAddRegFee) == -1)
                    {
                        return -1;
                    }
                }
            }

            return 1;
        }

        /// <summary>
        /// 是否普诊科室，普诊科室挂号级别始终是普诊
        /// </summary>
        bool isOrdinaryRegDept = false;

        /// <summary>
        /// 根据基本信息获取挂号信息
        /// </summary>
        /// <param name="cardNO">患者卡号</param>
        /// <returns>挂号实体</returns>
        private Neusoft.HISFC.Models.Registration.Register GetRegInfoFromPatientInfo(string cardNO)
        {
            #region 获取患者基本信息

            Neusoft.HISFC.BizProcess.Integrate.Manager manager = new Neusoft.HISFC.BizProcess.Integrate.Manager();

            Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = manager.QueryComPatientInfo(cardNO);
            if (patientInfo == null
                ||string.IsNullOrEmpty(patientInfo.PID.CardNO))
            {
                MessageBox.Show("查询患者基本信息出错，无法补挂号！\r\n\r\n如果是首诊患者，请先经过挂号处挂号！\r\n" + manager.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }

            #endregion

            Neusoft.HISFC.Models.Registration.Register regObj = new Neusoft.HISFC.Models.Registration.Register();

            Neusoft.HISFC.Models.Base.Employee oper = this.managerIntegrate.GetEmployeeInfo(this.contrlManager.Operator.ID);
            try
            {
                //系统补挂号患者，流水号为新号
                //根据regObj.IsFee判断是否是补挂号
                regObj.ID = this.contrlManager.GetSequence("Registration.Register.ClinicID");
                regObj.TranType = Neusoft.HISFC.Models.Base.TransTypes.Positive;//正交易
                regObj.PID = patientInfo.PID;

                //根据时间段判断是否急诊
                //regObj.DoctorInfo.Templet.RegLevel.IsEmergency = (this.cmbRegLevel.SelectedItem as Neusoft.HISFC.Models.Registration.RegLevel).IsEmergency;

                regObj.DoctorInfo.Templet.Dept.ID = ((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.ID;
                regObj.DoctorInfo.Templet.Dept.Name = ((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.Name;
                regObj.DoctorInfo.Templet.Doct.ID = this.contrlManager.Operator.ID;
                regObj.DoctorInfo.Templet.Doct.Name = this.contrlManager.Operator.Name;

                regObj.Name = patientInfo.Name;//患者姓名
                regObj.Sex = patientInfo.Sex;//性别
                regObj.Birthday = patientInfo.Birthday;//出生日期			

                regObj.RegType = Neusoft.HISFC.Models.Base.EnumRegType.Reg;

                regObj.InputOper.ID = this.contrlManager.Operator.ID;
                regObj.InputOper.OperTime = this.contrlManager.GetDateTimeFromSysDateTime();
                regObj.DoctorInfo.SeeDate = this.contrlManager.GetDateTimeFromSysDateTime();
                regObj.SeeDoct.ID = this.conManager.Operator.ID;
                regObj.SeeDoct.Dept.ID = ((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.ID;
                regObj.DoctorInfo.Templet.Begin = this.contrlManager.GetDateTimeFromSysDateTime();
                regObj.DoctorInfo.Templet.End = this.contrlManager.GetDateTimeFromSysDateTime();

                #region 午别
                if (regObj.DoctorInfo.SeeDate.Hour < 12 && regObj.DoctorInfo.SeeDate.Hour > 6)
                {
                    //上午
                    regObj.DoctorInfo.Templet.Noon.ID = "1";
                }
                else if (regObj.DoctorInfo.SeeDate.Hour > 12 && regObj.DoctorInfo.SeeDate.Hour < 18)
                {
                    //下午
                    regObj.DoctorInfo.Templet.Noon.ID = "2";
                }
                else
                {
                    //晚上
                    regObj.DoctorInfo.Templet.Noon.ID = "3";
                }
                #endregion

                //对于专家扣限额 先不处理


                //合同单位根据办卡记录获取，具体待提取方法
                regObj.Pact = patientInfo.Pact;
                if (string.IsNullOrEmpty(regObj.Pact.ID))
                {
                    regObj.Pact.ID = "1";
                    regObj.Pact.Name = "普通";
                    regObj.Pact.PayKind.ID = "01";
                    regObj.Pact.PayKind.Name = "自费";
                }

                #region 全天自费处理

                ArrayList alOwnFeeRegDept = this.conManager.GetList("OwnFeeRegDept");
                if (alOwnFeeRegDept == null)
                {
                    MessageBox.Show("获取自费挂号科室失败！" + conManager.Err);
                    return null;
                }

                foreach (Neusoft.HISFC.Models.Base.Const constObj in alOwnFeeRegDept)
                {
                    if (constObj.IsValid && constObj.ID.Trim() == this.GetReciptDept().ID)
                    {
                        ArrayList alOwnFeeRegLevl = this.conManager.GetList("OwnFeeRegLevl");
                        if (alOwnFeeRegLevl == null || alOwnFeeRegLevl.Count == 0)
                        {
                            MessageBox.Show("获取自费挂号级别失败！" + conManager.Err);
                            return null;
                        }

                        foreach (Neusoft.HISFC.Models.Base.Const obj in alOwnFeeRegLevl)
                        {
                            if (obj.IsValid)
                            {
                                regObj.Pact.ID = obj.ID;
                                regObj.Pact.Name = "普通";
                                regObj.Pact.PayKind.ID = "01";
                                regObj.Pact.PayKind.Name = "自费";
                                break;
                            }
                        }

                        break;
                    }
                }
                #endregion

                #region 挂号级别

                string regLevl = "";

                isOrdinaryRegDept = false;

                #region 普诊挂号科室
                ArrayList alOrdinaryRegDept = this.conManager.GetList("OrdinaryRegLevlDept");
                if (alOrdinaryRegDept == null)
                {
                    MessageBox.Show("获取普诊挂号科室失败！" + conManager.Err);
                    return null;
                }

                foreach (Neusoft.HISFC.Models.Base.Const constObj in alOrdinaryRegDept)
                {
                    if (constObj.IsValid && constObj.ID.Trim() == this.GetReciptDept().ID)
                    {
                        isOrdinaryRegDept = true;
                        break;
                    }
                }

                #endregion

                //普诊
                if (isOrdinaryRegDept)
                {
                    ArrayList alOrdinaryLevl = this.conManager.GetList("OrdinaryRegLevel");
                    if (alOrdinaryLevl == null || alOrdinaryLevl.Count == 0)
                    {
                        MessageBox.Show("获取普通门诊对应的挂号级别错误：" + conManager.Err);
                        return null;
                    }

                    foreach (Neusoft.HISFC.Models.Base.Const constObj in alOrdinaryLevl)
                    {
                        if (constObj.IsValid)
                        {
                            regLevl = constObj.ID.Trim();
                            break;
                        }
                    }
                }
                else
                {
                    //是否急诊
                    bool isEmerg = this.regInterMgr.IsEmergency(this.GetReciptDept().ID);

                    string diagItemCode = "";
                    if (isEmerg)
                    {
                        regObj.DoctorInfo.Templet.RegLevel.IsEmergency = true;

                        regLevl = emergencyLevlCode;

                        if (string.IsNullOrEmpty(regLevl))
                        {
                            MessageBox.Show("获取挂号级别错误！\r\n原因:急诊挂号级别没有维护！\r\n如有问题请联系信息科！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        //MessageBox.Show("非急诊科室必须先挂号！");
                        //return null;
                        ///*
                        if (this.regInterMgr.GetSupplyRegInfo(oper.ID, oper.Level.ID.ToString(), regObj.DoctorInfo.Templet.Dept.ID, ref regLevl, ref diagItemCode) == -1)
                        {
                            MessageBox.Show("GetRegInfoFromPatientInfo" + regInterMgr.Err);
                            return null;
                        }
                        //*/ 
                    }
                }

                Neusoft.HISFC.Models.Registration.RegLevel regLevlObj = null;

                if (regLevlHelper != null && regLevlHelper.ArrayObject.Count != 0)
                {
                    regLevlObj = regLevlHelper.GetObjectFromID(regLevl) as Neusoft.HISFC.Models.Registration.RegLevel;
                }

                if (regLevlObj == null)
                {
                    regLevlObj = this.regInterMgr.QueryRegLevelByCode(regLevl);
                    if (regLevlObj == null)
                    {
                        MessageBox.Show("查询挂号级别错误，编码[" + regLevl + "]！请联系信息科重新维护" + regInterMgr.Err);
                        return null;
                    }
                }

                regObj.DoctorInfo.Templet.RegLevel = regLevlObj;
                #endregion

                regObj.SSN = patientInfo.SSN;//医疗证号

                regObj.PhoneHome = patientInfo.PhoneHome;//联系电话
                regObj.AddressHome = patientInfo.AddressHome;//联系地址
                regObj.CardType = patientInfo.IDCardType; //证件类型

                regObj.IsFee = false;
                regObj.Status = Neusoft.HISFC.Models.Base.EnumRegisterStatus.Valid;
                //之前为什么改为true呢？？
                regObj.IsSee = false;
                regObj.CancelOper.ID = "";
                regObj.CancelOper.OperTime = DateTime.MinValue;
                regObj.IDCard = patientInfo.IDCard;

                regObj.PVisit.InState.ID = "N";
                regObj.DoctorInfo.SeeNO = -1;

                //加密处理
                if (patientInfo.IsEncrypt)
                {
                    regObj.IsEncrypt = true;
                    regObj.NormalName = Neusoft.FrameWork.WinForms.Classes.Function.Encrypt3DES(patientInfo.Name);
                    regObj.Name = "******";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetRegInfoFromPatientInfo" + ex.Message);
                return null;
            }

            if (isUserOnePersonMorePact)
            {
                if (account.GetPatientPactInfo(regObj) == -1)
                {
                    MessageBox.Show("获取患者合同单位信息失败：" + account.Err);
                    return null;
                }

                if (regObj.MutiPactInfo.Count > 1)
                {
                    Neusoft.FrameWork.Models.NeuObject pactObj = new Neusoft.FrameWork.Models.NeuObject();
                    if (regObj.MutiPactInfo.Count > 0)
                    {
                        if (FrameWork.WinForms.Classes.Function.ChooseItem(new ArrayList(regObj.MutiPactInfo), new string[] { "合同单位编码", "合同单位", "有效性" }, new bool[] { false, true, true, false, false, false }, new int[] { 50, 100, 70 }, ref pactObj) != 1)
                        {
                            return null;
                        }
                    }

                    if (pactObj != null && !string.IsNullOrEmpty(pactObj.ID))
                    {
                        regObj.Pact = pactObj as Neusoft.HISFC.Models.Base.PactInfo;
                    }
                }

                if (this.cbxNewReg.Checked)
                {
                    if (MessageBox.Show("该患者将以[" + regObj.Pact.Name + "]的身份重新补挂号，是否重新补收挂号费？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        regObj.Memo = "不补收";
                    }
                }

                this.cbxNewReg.Checked = false;
            }

            return regObj;
        }

        /// <summary>
        /// 开立科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject reciptDept = null;

        /// <summary>
        /// 获取开方科室
        /// </summary>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject GetReciptDept()
        {
            try
            {
                if (this.reciptDept == null)
                {
                    Neusoft.HISFC.Models.Registration.Schema schema = this.regInterMgr.GetSchema(this.contrlManager.Operator.ID, this.contrlManager.GetDateTimeFromSysDateTime());
                    if (schema != null && schema.Templet.Dept.ID != "")
                    {
                        this.reciptDept = schema.Templet.Dept.Clone();
                    }
                    //没有排版取登陆科室作为开立科室
                    else
                    {
                        this.reciptDept = ((Neusoft.HISFC.Models.Base.Employee)this.contrlManager.Operator).Dept.Clone(); //开立科室
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetReciptDept" + ex.Message);
                return null;
            }
            return this.reciptDept;
        }

        #endregion

        #region 作废

        /// <summary>
        /// 自动挂号的病历号开头字符
        /// </summary>
        //private string strFormatHeader = "";

        ///// <summary>
        ///// 自动按照日期生成病历号，日期类型
        ///// </summary>
        //private int intDateType = 0;

        //protected ToolTip tooltip = new ToolTip();

        ///// <summary>
        ///// 病历号长度
        ///// </summary>
        //private int intLength = 10;





        #region 按照字头、日期 补充病历号

        /// <summary>
        /// 录入门诊号文本格式化—补零（参数：门诊号长度）
        /// </summary>
        /// <param name="Length"></param>
        public void SetFormat(int Length)
        {
            this.SetFormat("", 0, Length);
        }

        /// <summary>
        /// 录入门诊号文本格式化—加字头（参数：字头字符；门诊号长度）
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="Length"></param>
        public void SetFormat(string Header, int Length)
        {
            this.SetFormat(Header, 0, Length);
        }

        /// <summary>
        /// 录入门诊号文本格式化—加字头添加日期（参数：字头字符；时间；门诊号长度）
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="DateType"></param>
        /// <param name="Length"></param>
        public void SetFormat(string Header, int DateType, int Length)
        {
            //this.intLength = Length;
            //this.strFormatHeader = Header;
            //this.intDateType = DateType;
        }

        /// <summary>
        /// 按照字头、日期 补充病历号
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        private string formatInputCode(string Text)
        {
            return null;
            //string strText = Text;
            //try
            //{
            //    for (int i = 0; i < this.intLength - strText.Length; i++)
            //    {
            //        Text = "0" + Text;
            //    }
            //    string strDateTime = "";
            //    try
            //    {
            //        strDateTime = this.contrlManager.GetSysDateNoBar();
            //    }
            //    catch { }
            //    switch (this.intDateType)
            //    {
            //        case 1:
            //            strDateTime = strDateTime.Substring(2);
            //            Text = strDateTime + Text.Substring(strDateTime.Length);
            //            break;
            //        case 2:
            //            Text = strDateTime + Text.Substring(strDateTime.Length);
            //            break;
            //    }
            //    if (this.strFormatHeader != "") Text = this.strFormatHeader + Text.Substring(this.strFormatHeader.Length);
            //}
            //catch { }
            ////日期   
            //return Text;
        }

        #endregion

        /// <summary>
        /// 双击检索的患者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lvAllReg_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as Neusoft.FrameWork.WinForms.Controls.NeuListView).SelectedItems.Count > 0)
            {
                ListViewItem listItem = (sender as Neusoft.FrameWork.WinForms.Controls.NeuListView).SelectedItems[0];

                if (listItem != null)
                {
                    this.txtInputCode.Text = listItem.SubItems[0].Text;
                }
            }

            ((sender as ListView).Parent as Form).Close();
        }

        private void txtInputCode_Enter(object sender, EventArgs e)
        {
            try
            {
                foreach (InputLanguage input in InputLanguage.InstalledInputLanguages)
                {
                    if (input.LayoutName == "美式键盘" || input.LayoutName == "中文(简体) - 美式键盘")
                    {
                        InputLanguage.CurrentInputLanguage = input;
                    }
                }

                if (this.txtInputCode.Text.Length >= 2 && System.Text.Encoding.Default.GetBytes(this.txtInputCode.Text.Substring(0, 1)).Length > 1)
                {
                    this.txtInputCode.Text = "";
                }
            }
            catch
            { }
        }

        #endregion
    }

    /// <summary>
    /// 此控件使用的地方
    /// 各地方处理不一样
    /// </summary>
    public enum enumUseType
    {
        /// <summary>
        /// 挂号
        /// </summary>
        Register,

        /// <summary>
        /// 办卡
        /// </summary>
        TransactCard,

        /// <summary>
        /// 收费处
        /// </summary>
        Charge,

        /// <summary>
        /// 退号
        /// </summary>
        CancelFee,

        /// <summary>
        /// 门诊医生
        /// </summary>
        Doct,

        /// <summary>
        /// 其他地方
        /// </summary>
        Other
    }
}
