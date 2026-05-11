using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmRegistrationByDoctor : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        public frmRegistrationByDoctor(string patientName) 
        {
            InitializeComponent();
            this.txtName.Text = patientName;
        }

        #region 变量

        /// <summary>
        /// 自动生成的卡号
        /// </summary>
        protected string autoCardNO = string.Empty;

        /// <summary>
        /// 自助挂号相关接口
        /// </summary>
        public Neusoft.HISFC.BizProcess.Interface.Order.IAfterQueryRegList IAfterQueryRegList = null;

        /// <summary>
        /// 门诊流水号
        /// </summary>
        protected string clinicNO = string.Empty;

        /// <summary>
        /// 没有挂号患者,卡号第一位标志,默认以9开头
        /// </summary>
        protected string noRegFlagChar = "9";

        /// <summary>
        /// 挂号信息实体
        /// </summary>
        protected Neusoft.HISFC.Models.Registration.Register register = new Neusoft.HISFC.Models.Registration.Register();

        /// <summary>
        /// 患者基本信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = null;

        /// <summary>
        /// 门诊医嘱业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Order.OutPatient.Order orderManagement = new Neusoft.HISFC.BizLogic.Order.OutPatient.Order();
        /// <summary>
        /// 合同单位业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager interMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 是否普诊科室，普诊科室挂号级别始终是普诊
        /// </summary>
        bool isOrdinaryRegDept = false;

        /// <summary>
        /// 费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeManagement = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 挂号业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Registration.Registration regManagement = new Neusoft.HISFC.BizProcess.Integrate.Registration.Registration();

        /// <summary>
        /// 控制参数业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        /// <summary>
        /// 常数管理业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant conManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 操作员
        /// </summary>
        protected Neusoft.HISFC.Models.Base.Employee employee = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;

        private Neusoft.HISFC.BizLogic.RADT.InPatient radtMgr = new Neusoft.HISFC.BizLogic.RADT.InPatient();

        /// <summary>
        /// 开立医生
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee doct = null;

        /// <summary>
        /// 不允许自动挂号的合同单位
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper noAutoRegPactHelper = null;

        #endregion

        #region 属性

        /// <summary>
        /// 急诊挂号级别
        /// </summary>
        private string emergencyLevlCode;

        /// <summary>
        /// 急诊挂号级别
        /// </summary>
        public string EmergencyLevlCode
        {
            get
            {
                return emergencyLevlCode;
            }
            set
            {
                emergencyLevlCode = value;
            }
        }

        /// <summary>
        /// 患者挂号信息
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register PatientInfo
        {
            get
            {
                return this.register;
            }
        }

        /// <summary>
        /// 挂号级别帮助类
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper regLevlHelper = null;
        /// <summary>
        /// 是否打开身份证验证开关
        /// </summary>
        private Boolean _isNeedVerifyIDCard = false;

        #endregion

        #region 方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitControl()
        {
            //初始化合同单位
            ArrayList pactList = this.interMgr.QueryPactUnitOutPatient();
            if (pactList == null)
            {
                MessageBox.Show("初始化合同单位出错!" + this.interMgr.Err);

                return;
            }
            this.cmbPact.AddItems(pactList);

            //初始化性别
            this.cmbSex.AddItems(Neusoft.HISFC.Models.Base.SexEnumService.List());

            //获得卡号前补位规则
            this.noRegFlagChar = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.NO_REG_CARD_RULES, false, "9");

            this.autoCardNO = this.feeManagement.GetAutoCardNO();
            if (autoCardNO == string.Empty || autoCardNO == "" || autoCardNO == null)
            {
                MessageBox.Show("获得门诊卡号出错!" + this.feeManagement.Err);

                return;
            }
            //autoCardNO = this.noRegFlagChar + autoCardNO;
            autoCardNO = this.autoCardNO.PadLeft(10, '0');
            //this.txtCardNo.Text = this.autoCardNO;

            this.clinicNO = this.orderManagement.GetSequence("Registration.Register.ClinicID");
            if (clinicNO == string.Empty || clinicNO == "" || clinicNO == null)
            {
                MessageBox.Show("获得门诊就诊号出错!" + this.orderManagement.Err);

                return;
            }

            this.cmbSex.Tag = "M";

            this.cmbPact.Tag = "1";

            this.doct = this.interMgr.GetEmployeeInfo(this.employee.ID);
            
            if (this.doct == null)
            {
                MessageBox.Show(this.interMgr.Err);
            }

            this.lblTip.Text = "";

            if (noAutoRegPactHelper == null)
            {
                noAutoRegPactHelper = new Neusoft.FrameWork.Public.ObjectHelper();
                noAutoRegPactHelper.ArrayObject = this.interMgr.GetConstantList("NoAutoRegPact");
            }

            #region 获取所有挂号级别
            if (regLevlHelper == null)
            {
                regLevlHelper = new Neusoft.FrameWork.Public.ObjectHelper();

                //获取所有的挂号级别
                ArrayList al = regManagement.QueryAllRegLevel();

                regLevlHelper.ArrayObject = al;

                //有效的挂号级别
                ArrayList alValidReglevl = new ArrayList();

                if (al == null || al.Count == 0)
                {
                    MessageBox.Show("查询所有挂号级别错误！会导致补收挂号费错误!\r\n请联系信息科重新维护" + regManagement.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    bool isValidEmergency = true;
                    foreach (Neusoft.HISFC.Models.Registration.RegLevel regLevl in al)
                    {
                        if (regLevl.IsValid)
                        {
                            alValidReglevl.Add(regLevl);

                            if (regLevl.IsEmergency)
                            {
                                emergencyLevlCode = regLevl.ID;
                                //break;
                            }
                        }
                        else if (regLevl.IsEmergency)
                        {
                            isValidEmergency = false;
                        }
                    }
                    
                    this.SetRegLevel(alValidReglevl);
                    //this.cmbRegLevl.AddItems(alValidReglevl);
                   
                }
            }
            #endregion

            this.SetEnabled(false);

            #region 设置是否允许首诊挂号
            //建档需要到挂号处
            btAutoCardNo.Visible = false;
            //if (FrameWork.WinForms.Classes.Function.IsManager()
            //    //||这里要做控制参数设置了,交给后来人吧~
            //    )
            //{
            //    btAutoCardNo.Visible = true;
            //}

            #endregion

            this.GetConst();
            
        }

        /// <summary>
        /// 设置挂号级别
        /// </summary>
        /// <param name="al"></param>
        private void SetRegLevel(ArrayList al)
        {
            //主任：9 副主任：10 主治医师：11 医师：13 
            
            if (object.Equals(this.doct, null)) return;
            switch (this.doct.Level.ID)
            {
                case "09": break;
                case "10": al.RemoveAt((int)enumReglevel.Z主任医师);
                    break;
                case "11": al.RemoveAt((int)enumReglevel.Z主任医师);
                    al.RemoveAt((int)enumReglevel.F副主任医师);
                    al.RemoveAt((int)enumReglevel.T特需门诊);
                    break;
                case "13": al.RemoveAt((int)enumReglevel.Z主任医师);
                    al.RemoveAt((int)enumReglevel.F副主任医师);
                    al.RemoveAt((int)enumReglevel.T特需门诊);
                    break;
                default:
                    al.RemoveAt((int)enumReglevel.Z主任医师);
                    al.RemoveAt((int)enumReglevel.F副主任医师);
                    al.RemoveAt((int)enumReglevel.T特需门诊);
                    al.RemoveAt((int)enumReglevel.J急诊挂号);
                    //al.Clear();
                    
                    break;
            }
            this.cmbRegLevl.AddItems(al);
        }

        private enum enumReglevel
        {
            P普通,
            J急诊挂号,
            T特需门诊,
            F副主任医师,
            Z主任医师
        }

        /// <summary>
        /// 初始化常数
        /// </summary>
        private void GetConst()
        {
            //这里放到常数表是信息科维护要求的，正常来讲这种开关应该放到参数表
            string type = "RegVerifyOfIdcard";
            string id = "1";
            _isNeedVerifyIDCard = string.IsNullOrEmpty(this.conManager.GetConstant(type, id).ID) ? false : true;
        }

        /// <summary>
        /// 自动生成卡号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAutoCardNo_Click(object sender, EventArgs e)
        {
            Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();

            this.autoCardNO = regMgr.AutoGetCardNO().ToString(); //this.feeManagement.GetAutoCardNO();
            if (autoCardNO == string.Empty || autoCardNO == "" || autoCardNO == null)
            {
                MessageBox.Show("获得门诊卡号出错!" + this.feeManagement.Err);
                return;
            }
            //autoCardNO = this.noRegFlagChar + autoCardNO;
            autoCardNO = this.autoCardNO.PadLeft(10, '0');
            this.txtCardNo.Text = this.autoCardNO;

            this.SetEnabled(true);
            this.txtName.Focus();
        }

        /// <summary>
        /// 设置患者信息
        /// </summary>
        /// <returns></returns>
        private int SetRegister()
        {
            DateTime now = this.orderManagement.GetDateTimeFromSysDateTime();
            this.register.ID = clinicNO;
            this.register.Name = this.txtName.Text.Trim();
            //this.register.Card.ID = autoCardNO;
            //this.register.PID.CardNO = autoCardNO;
            this.register.Card.ID = this.txtCardNo.Text;
            this.register.PID.CardNO = this.txtCardNo.Text;
            this.register.IDCard = this.txtIDCard.Text;

            if (this.register.PID.CardNO.Length < 10)
            {
                this.register.PID.CardNO.PadLeft(10, '0');
            }

            this.register.PhoneHome = this.txtPhone.Text;
            this.register.AddressHome = this.txtAddress.Text;

            #region 合同单位

            if (this.cmbPact.Tag == null || string.IsNullOrEmpty(this.cmbPact.Tag.ToString()))
            {
                MessageBox.Show("请选择合同单位！");
                return -1;
            }

            Neusoft.HISFC.Models.Base.PactInfo pactObj = interMgr.GetPactUnitInfoByPactCode(this.cmbPact.Tag.ToString());
            if (pactObj == null)
            {
                MessageBox.Show("获取合同单位信息出错：" + interMgr.Err);
                return -1;
            }
            this.register.Pact = pactObj;
            #endregion


            this.register.Sex.ID = this.cmbSex.Tag.ToString();
            this.register.Birthday = this.dtPickerBirth.Value;
            this.register.DoctorInfo.SeeDate = now; 
            this.register.DoctorInfo.SeeNO = -1;
            this.register.DoctorInfo.Templet.Dept = this.employee.Dept;

            this.register.InputOper.ID = this.employee.ID;
            this.register.InputOper.OperTime = this.orderManagement.GetDateTimeFromSysDateTime();
            this.register.DoctorInfo.SeeDate = this.orderManagement.GetDateTimeFromSysDateTime();
            this.register.DoctorInfo.Templet.Begin = this.orderManagement.GetDateTimeFromSysDateTime();
            this.register.DoctorInfo.Templet.End = this.orderManagement.GetDateTimeFromSysDateTime();
            this.register.RegType = Neusoft.HISFC.Models.Base.EnumRegType.Reg;

            #region 午别
            if (this.register.DoctorInfo.SeeDate.Hour < 12 && this.register.DoctorInfo.SeeDate.Hour > 6)
            {
                //上午
                this.register.DoctorInfo.Templet.Noon.ID = "1";
            }
            else if (this.register.DoctorInfo.SeeDate.Hour > 12 && this.register.DoctorInfo.SeeDate.Hour < 18)
            {
                //下午
                this.register.DoctorInfo.Templet.Noon.ID = "2";
            }
            else
            {
                //晚上
                this.register.DoctorInfo.Templet.Noon.ID = "3";
            }
            #endregion

            #region 挂号级别

            this.register.DoctorInfo.Templet.RegLevel = this.cmbRegLevl.SelectedItem as Neusoft.HISFC.Models.Registration.RegLevel;

            #endregion

            this.register.IsFee = false;
            this.register.Status = Neusoft.HISFC.Models.Base.EnumRegisterStatus.Valid;
            this.register.IsSee = false;
            this.register.PVisit.InState.ID = "N";

            register.DoctorInfo.Templet.Doct = employee;

            //return this.register;
            return 1;
        }

        /// <summary>
        /// 有效性校验
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private bool CheckRegister(Neusoft.HISFC.Models.Registration.Register reg)
        {
            if (reg.ID.Trim() == "" || reg.ID == null)
            {
                MessageBox.Show("门诊就诊号不可为空！");
                return false;
            }
            if (reg.Name.Trim() == "" || reg.Name == null)
            {
                MessageBox.Show("姓名不可为空！");
                return false;
            }
            if (reg.PID.CardNO.Trim() == "" || reg.PID.CardNO == null)
            {
                MessageBox.Show("门诊卡号不可为空！");
                return false;
            }
            //if (reg.PID.CardNO.Trim().ToString().Substring(0,2).Contains("99"))
            //{
            //    MessageBox.Show("门诊卡号为空99号段不允许补挂号！");
            //    return false;
            //}
            //if (string.IsNullOrEmpty(reg.IDCard)) {
            //    MessageBox.Show("补挂号不允许身份证号为空！");
            //    return false;
            //}


            if (reg.Sex.ID.ToString().Trim() == "" || reg.Sex.ID == null)
            {
                MessageBox.Show("性别不可为空！");
                return false;
            }

            Neusoft.HISFC.Models.Base.Const conObj = noAutoRegPactHelper.GetObjectFromID(cmbPact.Tag.ToString()) as Neusoft.HISFC.Models.Base.Const;

            if (this.cmbPact.Tag != null && !string.IsNullOrEmpty(this.cmbPact.Tag.ToString()) && conObj != null)
            {
                MessageBox.Show("合同单位【" + cmbPact.Text + "】" + conObj.Memo);
                return false;
            }

            if (IAfterQueryRegList != null)
            {
                if (IAfterQueryRegList.OnConfirmRegInfo(this.register) == -1)
                {
                    MessageBox.Show(IAfterQueryRegList.ErrInfo, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

           
            if (!this.VarifyIdNO(reg)) return false;

            return true;
        }

        /// <summary>
        /// 验证身份证信息
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private Boolean VarifyIdNO(Neusoft.HISFC.Models.Registration.Register reg)
        {
            #region {E8AD3099-28AA-4c53-9E5F-159F652A1411} wubiqiu 增加判断身份证是否与出生年月一致
            if (_isNeedVerifyIDCard)
            {
                string idCard = reg.IDCard;
                string errMsg = string.Empty;
                //一周岁一下儿童跳过验证
                DateTime birthdayTemp = reg.Birthday;
                var today = DateTime.Today;
                var age = today.Year - birthdayTemp.Year;
                if (birthdayTemp > today.AddYears(-age)) age--;
                if (age < 1) return true;
                //
                if (string.IsNullOrEmpty(idCard) || Neusoft.FrameWork.WinForms.Classes.Function.CheckIDInfo(idCard, ref errMsg) == -1)
                {
                    MessageBox.Show("身份证信息有误，请到挂号处录入完整信息！");
                    return false;
                }
                else
                {
                    DateTime birthday = DateTime.Parse(idCard.Substring(6, 4) + "-" + idCard.Substring(10, 2) + "-" + idCard.Substring(12, 2));
                    if (!DateTime.Equals(birthday, reg.Birthday))
                    {
                        MessageBox.Show("身份证信息有误，请到挂号处录入完整信息！");
                        return false;
                    }
                }
            }
            return true;
            #endregion

        }

        private int InsertRegInfo(Neusoft.HISFC.Models.Registration.Register reg)
        {
            this.regManagement.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            int iReturn = -1;
            reg.InputOper.ID = this.employee.ID;
            reg.InputOper.Name = this.employee.Name;
            //reg.InputOper.OperTime = reg.DoctorInfo.SeeDate;
            iReturn = this.regManagement.Insert(reg);
            if (iReturn == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                if (regManagement.DBErrCode != 1)//不是主键重复
                {
                    MessageBox.Show("插入挂号信息出错!" + regManagement.Err);

                    return -1;
                }
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            return iReturn;
        }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            //这里要判断一下，调取患者信息后是不是又修改过卡号
            if (patientInfo != null
                && !string.IsNullOrEmpty(patientInfo.PID.CardNO)
                && patientInfo.PID.CardNO != txtCardNo.Text)
            {
                MessageBox.Show("修改门诊号后，请回车确认！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.txtCardNo.Focus();
                return;
            }

            if (this.SetRegister() == -1)
            {
                return;
            }

            #region 判断挂号信息
            if (!this.CheckRegister(this.register))
            {
                return;
            }
            #endregion

            #region 保存患者基本信息

            #region 判断患者信息

            if (string.IsNullOrEmpty(this.patientInfo.Card.ID))
            {
                this.patientInfo.Name = this.txtName.Text.Trim();
                this.patientInfo.Card.ID = this.txtCardNo.Text;
                this.patientInfo.PID.CardNO = this.txtCardNo.Text;

                this.patientInfo.PhoneHome = this.txtPhone.Text;
                this.patientInfo.AddressHome = this.txtAddress.Text;
                this.patientInfo.IDCard = this.txtIDCard.Text;

                #region 合同单位

                if (this.cmbPact.Tag == null || string.IsNullOrEmpty(this.cmbPact.Tag.ToString()))
                {
                    MessageBox.Show("请选择合同单位！");
                    return;
                }
                Neusoft.HISFC.Models.Base.PactInfo pactObj = interMgr.GetPactUnitInfoByPactCode(this.cmbPact.Tag.ToString());
                if (pactObj == null)
                {
                    MessageBox.Show("获取合同单位信息出错：" + interMgr.Err);
                    return;
                }
                this.patientInfo.Pact = pactObj;
                #endregion

                this.patientInfo.Sex.ID = this.cmbSex.Tag.ToString();
                this.patientInfo.Birthday = this.dtPickerBirth.Value;

                //增加判断，避免医生人为修改卡号，导致挂号的信息和患者实际分配的卡号不一致
                Neusoft.HISFC.Models.RADT.Patient patientCommonInfo = this.interMgr.QueryComPatientInfo(patientInfo.PID.CardNO);
                if (!string.IsNullOrEmpty(patientCommonInfo.PID.CardNO)  
                    && patientCommonInfo.Name != patientInfo.Name)
                {
                    MessageBox.Show("请在卡号处回车确认！\r\n\r\n原因：卡号【" + patientInfo.PID.CardNO + "】对应的姓名【" + patientCommonInfo.Name + "】和显示的姓名【" + patientInfo.Name + "】不一致！\r\n如需修改患者信息，请患者到门诊收费处修改！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            #endregion


            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            this.regManagement.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            this.register.InputOper.ID = this.employee.ID;
            register.InputOper.Name = this.employee.Name;
            int iReturn = this.regManagement.Insert(register);
            if (iReturn == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                if (regManagement.DBErrCode != 1)//不是主键重复
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入挂号信息出错!" + regManagement.Err);
                    return;
                }
            }

            iReturn = this.regManagement.UpdateHosCode(register, Neusoft.FrameWork.Management.Connection.Hospital.ID);
            if (iReturn == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新患者医院标识!" + regManagement.Err);
                return;
            }

            this.radtMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            if (this.radtMgr.UpdatePatientInfo(this.patientInfo) <= 0)
            {
                if (this.radtMgr.InsertPatientInfo(this.patientInfo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入患者基本信息出错：" + radtMgr.Err);
                    return;
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            #endregion
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmRegistrationByDoctor_Load(object sender, EventArgs e)
        {
            this.InitControl();
        }

        private void btnCaecel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SetEnabled(bool val)
        {
            //this.cmbPact.Enabled = val;
            this.cmbSex.Enabled = val;
            this.txtName.Enabled = val;
            this.txtIDCard.Enabled = val;
            this.dtPickerBirth.Enabled = val;
            this.txtCardNo.Enabled = !val;
            this.txtPhone.Enabled = val; //新增电话与地址
            this.txtAddress.Enabled = val;
            //this.btAutoCardNo.Visible = val;
        }

        private void Clear()
        {
            this.txtName.Text = "";

            this.txtIDCard.Text = "";

            this.txtPhone.Text = "";

            this.txtAddress.Text = "";
            cmbPact.Tag = null;
            this.lblTip.Text = "";

            this.cmbRegLevl.Tag = null;

            this.cmbSex.Tag = null;
            this.dtPickerBirth.Value = DateTime.Now;

        }

        /// <summary>
        /// 设置患者基本信息
        /// </summary>
        /// <param name="patientObj"></param>
        public void SetPatientInfo(Neusoft.HISFC.Models.RADT.Patient patientObj)
        {
            if (patientObj != null)
            {
                this.SetEnabled(false);

                this.txtCardNo.Text = patientObj.PID.CardNO;

                this.txtName.Text = patientObj.Name;

                this.txtIDCard.Text = patientObj.IDCard;

                this.txtPhone.Text = patientObj.PhoneHome;  //新增加的联系电话与家庭住址 by zhy 

                this.txtAddress.Text = patientObj.AddressHome;

                #region 合同单位

                //this.cmbPact.Enabled = true;
                this.cmbPact.Tag = patientObj.Pact.ID;

                if (this.cmbPact.Tag == null || string.IsNullOrEmpty(this.cmbPact.Tag.ToString()))
                {
                    this.cmbPact.Tag = "1";
                }

                this.lblTip.Text = "";

                #region 合同单位全天自费处理

                ArrayList alOwnFeeRegDept = this.conManager.GetList("OwnFeeRegDept");
                if (alOwnFeeRegDept == null)
                {
                    MessageBox.Show("获取自费看诊科室失败！" + conManager.Err);
                }

                foreach (Neusoft.HISFC.Models.Base.Const constObj in alOwnFeeRegDept)
                {
                    if (constObj.IsValid && constObj.ID.Trim() == this.employee.Dept.ID)
                    {
                        ArrayList alOwnFeeRegLevl = this.conManager.GetList("OwnFeeRegLevl");
                        if (alOwnFeeRegLevl == null || alOwnFeeRegLevl.Count == 0)
                        {
                            MessageBox.Show("获取自费挂号级别失败！" + conManager.Err);
                        }

                        foreach (Neusoft.HISFC.Models.Base.Const obj in alOwnFeeRegLevl)
                        {
                            if (obj.IsValid)
                            {
                                this.cmbPact.Tag = obj.ID;
                                this.lblTip.Text = "提示：系统设置本科室只能挂号【" + cmbPact.Text + "】合同单位！";
                                //this.cmbPact.Enabled = false;
                                break;
                            }
                        }

                        break;
                    }
                }
                #endregion

                #endregion

                #region 挂号级别

                string regLevl = "";

                isOrdinaryRegDept = false;

                #region 普诊挂号科室
                ArrayList alOrdinaryRegDept = this.conManager.GetList("OrdinaryRegLevlDept");
                if (alOrdinaryRegDept == null)
                {
                    MessageBox.Show("获取普诊挂号科室失败！" + conManager.Err);
                    return;
                }

                foreach (Neusoft.HISFC.Models.Base.Const constObj in alOrdinaryRegDept)
                {
                    if (constObj.IsValid && constObj.ID.Trim() == this.employee.Dept.ID)
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
                        return;
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
                    bool isEmerg = this.regManagement.IsEmergency(this.employee.Dept.ID);

                    string diagItemCode = "";
                    if (isEmerg && !string.IsNullOrEmpty(emergencyLevlCode))
                    {
                        regLevl = this.emergencyLevlCode;
                    }
                    else
                    {
                        if (this.regManagement.GetSupplyRegInfo(employee.ID, this.doct.Level.ID.ToString(), employee.Dept.ID, ref regLevl, ref diagItemCode) == -1)
                        {
                            MessageBox.Show(regManagement.Err);
                            return;
                        }
                    }
                }

                Neusoft.HISFC.Models.Registration.RegLevel regLevlObj = this.regLevlHelper.GetObjectFromID(regLevl) as Neusoft.HISFC.Models.Registration.RegLevel;
                if (regLevlObj == null)
                {
                    MessageBox.Show("查询挂号级别错误，编码[" + regLevl + "]！请联系信息科重新维护!");
                    return;
                }

                this.cmbRegLevl.Tag = regLevlObj.ID;

                #endregion

                this.cmbSex.Tag = patientObj.Sex.ID;
                if (patientObj.Birthday > new DateTime(1800, 1, 1))
                {
                    this.dtPickerBirth.Value = patientObj.Birthday;
                }
            }
        }

        private void txtCardNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int a = 0;
                //输入的是数字，则认为是卡号查询，否则是名称查询
                if (int.TryParse(txtCardNo.Text.Trim(), out a))
                {
                    Neusoft.HISFC.Models.Account.AccountCard accountCard = new Neusoft.HISFC.Models.Account.AccountCard();
                    Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
                    string cardNO = this.txtCardNo.Text;
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

                    this.txtCardNo.Text = cardNO;

                    if (!string.IsNullOrEmpty(this.txtCardNo.Text))
                    {
                        this.txtCardNo.Text = this.txtCardNo.Text.PadLeft(10, '0');
                    }

                    if (!string.IsNullOrEmpty(txtCardNo.Text.Trim()))
                    {
                        Clear();

                        this.patientInfo = this.radtMgr.QueryComPatientInfo(this.txtCardNo.Text);
                        if (patientInfo != null && !string.IsNullOrEmpty(patientInfo.PID.CardNO))
                        {
                            this.SetPatientInfo(this.patientInfo);
                        }
                    }
                }
                else
                {
                    frmQueryPatientByName frmQuery = new frmQueryPatientByName();
                    frmQuery.QueryByName(txtCardNo.Text.Trim());
                    frmQuery.SelectedPatient += new frmQueryPatientByName.GetPatient(frmQuery_SelectedPatient);
                    frmQuery.ShowDialog(this);
                }

                if (this.patientInfo != null
                    && !string.IsNullOrEmpty(txtName.Text))
                {
                    this.btnOK.Focus();
                }
            }
        }

        void frmQuery_SelectedPatient(Neusoft.HISFC.Models.RADT.PatientInfo pInfo)
        {
            this.patientInfo = pInfo;
            if (patientInfo != null && !string.IsNullOrEmpty(patientInfo.PID.CardNO))
            {
                this.SetPatientInfo(this.patientInfo);
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                frmQueryPatientByName frmQuery = new frmQueryPatientByName();
                frmQuery.QueryByName(txtCardNo.Text.Trim());
                frmQuery.SelectedPatient += new frmQueryPatientByName.GetPatient(frmQuery_SelectedPatient);
                frmQuery.ShowDialog(this);
            }
        }
    }
}

